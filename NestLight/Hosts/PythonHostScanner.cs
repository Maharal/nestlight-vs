using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Hosts
{
    /// <summary>
    /// Python: single, double and triple-quoted strings (any prefix, including f-strings and t-strings),
    /// marked by a <c>#</c> comment before the string. Replacement fields <c>{expr}</c> are interpolations
    /// of f-strings and t-strings; <c>{{</c> and <c>}}</c> are escaped braces.
    /// Implicit concatenation is not joined: each literal stands alone.
    /// </summary>
    internal sealed class PythonHostScanner : IHostScanner
    {
        private static readonly HashSet<string> Prefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "r", "u", "b", "f", "t", "br", "rb", "fr", "rf", "tr", "rt"
        };

        private readonly AcceptedEmbeddedLanguages _languages;

        public PythonHostScanner(AcceptedEmbeddedLanguages languages)
        {
            if (languages == null) throw new ArgumentNullException("languages");
            _languages = languages;
        }

        public IReadOnlyList<EmbeddedString> Scan(string text)
        {
            var result = new List<EmbeddedString>();
            new Run(text, _languages, result).ScanCode();
            result.Sort((a, b) => a.OuterStart.CompareTo(b.OuterStart));
            return result;
        }

        private sealed class Run
        {
            private readonly string _t;
            private readonly List<EmbeddedString> _result;
            private readonly MarkerTracker _markers;
            private readonly AcceptedEmbeddedLanguages _languages;
            private int _i;

            public Run(string text, AcceptedEmbeddedLanguages languages, List<EmbeddedString> result)
            {
                _t = text;
                _languages = languages;
                _result = result;
                _markers = new MarkerTracker(languages);
            }

            public void ScanCode()
            {
                while (_i < _t.Length)
                {
                    char c = _t[_i];
                    if (c == '#')
                    {
                        int e = _t.IndexOf('\n', _i);
                        int end = e < 0 ? _t.Length : e;
                        _markers.Comment(_t, _i + 1, end, end);
                        _i = end;
                    }
                    else if (!TryString())
                    {
                        if (TextUtil.IsWordStart(c)) SkipWord(); else _i++;
                    }
                }
            }

            private void SkipWord()
            {
                while (_i < _t.Length && TextUtil.IsWordChar(_t[_i])) _i++;
            }

            /// <summary>At a string literal (with or without prefix), reads it and returns true.</summary>
            private bool TryString()
            {
                int start = _i, q = _i;
                if (TextUtil.IsWordStart(_t[q]))
                {
                    while (q < _t.Length && TextUtil.IsWordChar(_t[q])) q++;
                    if (q >= _t.Length || (_t[q] != '"' && _t[q] != '\'')) return false;
                    if (!Prefixes.Contains(_t.Substring(start, q - start))) return false;
                }
                else if (_t[q] != '"' && _t[q] != '\'')
                {
                    return false;
                }

                string prefix = _t.Substring(start, q - start).ToLowerInvariant();
                bool raw = prefix.IndexOf('r') >= 0;
                bool formatted = prefix.IndexOf('f') >= 0 || prefix.IndexOf('t') >= 0;
                char quote = _t[q];
                bool triple = q + 2 < _t.Length && _t[q + 1] == quote && _t[q + 2] == quote;
                int quoteLength = triple ? 3 : 1;

                string marked = _markers.Take(_t, start);
                string id = _languages.Accepts(marked) ? marked : null;
                var s = new EmbeddedString(id) { OuterStart = start };

                s.Start = _i = q + quoteLength;
                while (_i < _t.Length)
                {
                    char c = _t[_i];
                    if (c == '\\')
                    {
                        char value;
                        if (!raw && _i + 1 < _t.Length && Escapes.TryDecode(_t[_i + 1], out value))
                            s.Escapes.Add(new EscapeSequence(_i, 2, value));
                        _i += 2; // a backslash before a line break continues the string
                    }
                    else if (c == quote && (!triple || (Next(1) == quote && Next(2) == quote)))
                    {
                        s.End = _i;
                        s.OuterEnd = _i += quoteLength;
                        if (id != null) _result.Add(s);
                        return true;
                    }
                    else if (c == '\n' && !triple)
                    {
                        break; // unclosed single-quoted string
                    }
                    else if (formatted && (c == '{' || c == '}'))
                    {
                        if (Next(1) == c) { s.Escapes.Add(new EscapeSequence(_i, 2, c)); _i += 2; }
                        else if (c == '{') ReadField(s, quote, triple);
                        else _i++;
                    }
                    else
                    {
                        _i++;
                    }
                }
                s.End = s.OuterEnd = Math.Min(_i, _t.Length);
                if (id != null) _result.Add(s);
                return true;
            }

            private char Next(int offset)
            {
                return _i + offset < _t.Length ? _t[_i + offset] : '\0';
            }

            /// <summary>Reads a replacement field "{expr!r:spec}" starting at its brace.</summary>
            private void ReadField(EmbeddedString s, char quote, bool triple)
            {
                int open = _i++;
                int braces = 0, parens = 0;
                bool spec = false;
                while (_i < _t.Length)
                {
                    char c = _t[_i];
                    if (c == '\n' && !triple) break; // unclosed field in a single-line string
                    if (spec)
                    {
                        if (c == '{') braces++;
                        else if (c == '}')
                        {
                            if (braces == 0) { Close(s, open, true); return; }
                            braces--;
                        }
                        _i++;
                    }
                    else if (c == '#' && triple)
                    {
                        // comments exist in the fields of triple-quoted f-strings (Python 3.12+)
                        int e = _t.IndexOf('\n', _i);
                        int end = e < 0 ? _t.Length : e;
                        _markers.Comment(_t, _i + 1, end, end);
                        _i = end;
                    }
                    else if (TryString())
                    {
                        // nested string, already consumed
                    }
                    else if (c == '}')
                    {
                        if (braces == 0) { Close(s, open, true); return; }
                        braces--; _i++;
                    }
                    else if (c == '{') { braces++; _i++; }
                    else if (c == '(' || c == '[') { parens++; _i++; }
                    else if (c == ')' || c == ']') { if (parens > 0) parens--; _i++; }
                    else if (c == ':' && braces == 0 && parens == 0) { spec = true; _i++; }
                    else if (TextUtil.IsWordStart(c)) SkipWord();
                    else _i++;
                }
                Close(s, open, false);
            }

            private void Close(EmbeddedString s, int open, bool closed)
            {
                int end = closed ? _i + 1 : Math.Min(_i, _t.Length);
                s.Interpolations.Add(new Interpolation(open, end, 1, closed ? 1 : 0));
                _i = end;
            }
        }
    }
}
