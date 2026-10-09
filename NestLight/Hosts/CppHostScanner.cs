using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Hosts
{
    /// <summary>
    /// C++: raw string literals <c>R"delim(...)delim"</c> (with the <c>u8R</c>, <c>uR</c>, <c>UR</c> and <c>LR</c> prefixes),
    /// marked by a comment before the string. Raw strings have no interpolation and no escapes.
    /// </summary>
    internal sealed class CppHostScanner : IHostScanner
    {
        private static readonly HashSet<string> RawPrefixes = new HashSet<string> { "R", "u8R", "uR", "UR", "LR" };
        private const int MaxDelimiterLength = 16;

        private readonly AcceptedEmbeddedLanguages _languages;

        public CppHostScanner(AcceptedEmbeddedLanguages languages)
        {
            if (languages == null) throw new ArgumentNullException("languages");
            _languages = languages;
        }

        public IReadOnlyList<EmbeddedString> Scan(string text)
        {
            var result = new List<EmbeddedString>();
            if (text.IndexOf("R\"", StringComparison.Ordinal) < 0) return result;
            new Run(text, _languages, result).ScanCode();
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
                    if (c == '/' && Next(1) == '/')
                    {
                        int e = _t.IndexOf('\n', _i);
                        int end = e < 0 ? _t.Length : e;
                        _markers.Comment(_t, _i + 2, end, end);
                        _i = end;
                    }
                    else if (c == '/' && Next(1) == '*')
                    {
                        int e = _t.IndexOf("*/", _i + 2, StringComparison.Ordinal);
                        if (e < 0) { _markers.Comment(_t, 0, 0, _t.Length); _i = _t.Length; }
                        else { _markers.Comment(_t, _i + 2, e, e + 2); _i = e + 2; }
                    }
                    else if (c == '"')
                    {
                        _markers.Take(_t, _i);
                        SkipQuoted('"');
                    }
                    else if (c == '\'')
                    {
                        // 1'000 is a digit separator, not a character literal
                        if (_i > 0 && char.IsLetterOrDigit(_t[_i - 1])) _i++;
                        else SkipQuoted('\'');
                    }
                    else if (TextUtil.IsWordStart(c))
                    {
                        ScanWord();
                    }
                    else
                    {
                        _i++;
                    }
                }
            }

            private char Next(int offset)
            {
                return _i + offset < _t.Length ? _t[_i + offset] : '\0';
            }

            private void ScanWord()
            {
                int start = _i;
                while (_i < _t.Length && TextUtil.IsWordChar(_t[_i])) _i++;
                if (_i >= _t.Length || _t[_i] != '"') return;

                string word = _t.Substring(start, _i - start);
                if (RawPrefixes.Contains(word)) ReadRaw(start);
                else if (word == "u8" || word == "u" || word == "U" || word == "L")
                {
                    _markers.Take(_t, start);
                    SkipQuoted('"');
                }
            }

            private void ReadRaw(int outerStart)
            {
                int quote = _i;
                int p = quote + 1;
                while (p < _t.Length && p - quote - 1 <= MaxDelimiterLength && IsDelimiterChar(_t[p])) p++;
                if (p >= _t.Length || _t[p] != '(' || p - quote - 1 > MaxDelimiterLength)
                {
                    // not a raw string after all: an ordinary literal
                    _markers.Take(_t, outerStart);
                    SkipQuoted('"');
                    return;
                }

                string delimiter = _t.Substring(quote + 1, p - quote - 1);
                string marked = _markers.Take(_t, outerStart);
                string id = _languages.Accepts(marked) ? marked : null;
                var s = new EmbeddedString(id) { OuterStart = outerStart, Start = p + 1 };

                string terminator = ")" + delimiter + "\"";
                int close = _t.IndexOf(terminator, p + 1, StringComparison.Ordinal);
                if (close < 0)
                {
                    s.End = s.OuterEnd = _t.Length;
                    _i = _t.Length;
                }
                else
                {
                    s.End = close;
                    s.OuterEnd = _i = close + terminator.Length;
                }
                if (id != null) _result.Add(s);
            }

            private static bool IsDelimiterChar(char c)
            {
                return c != '(' && c != ')' && c != '\\' && !char.IsWhiteSpace(c) && c != '"';
            }

            private void SkipQuoted(char quote)
            {
                _i++;
                while (_i < _t.Length)
                {
                    char c = _t[_i];
                    if (c == '\\') _i += 2;
                    else if (c == quote) { _i++; return; }
                    else if (c == '\n') return;
                    else _i++;
                }
            }
        }
    }
}
