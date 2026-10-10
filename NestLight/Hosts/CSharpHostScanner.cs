using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Hosts
{
    /// <summary>
    /// C#: regular, verbatim (<c>@"..."</c>) and raw (<c>"""..."""</c>) strings and their interpolated forms
    /// (<c>$</c>, <c>$$</c>...), marked by a comment before the string.
    /// An interpolation of a raw string opens with as many braces as there are <c>$</c>.
    /// </summary>
    internal sealed class CSharpHostScanner : IResumableHostScanner
    {
        private readonly AcceptedEmbeddedLanguages _languages;
        private readonly int _safePointGap;

        /// <param name="safePointGap">The distance between the safe points a scan keeps to resume from (see <see cref="SafePoints"/>).</param>
        public CSharpHostScanner(AcceptedEmbeddedLanguages languages, int safePointGap = SafePoints.DefaultGap)
        {
            if (languages == null) throw new ArgumentNullException("languages");
            _languages = languages;
            _safePointGap = safePointGap;
        }

        public IReadOnlyList<EmbeddedString> Scan(string text)
        {
            var result = new List<EmbeddedString>();
            new Run(text, _languages, result).ScanCode(0);
            result.Sort((a, b) => a.OuterStart.CompareTo(b.OuterStart));
            return result;
        }

        public HostScan ScanAll(string text)
        {
            var result = new List<EmbeddedString>();
            var safe = new SafePoints(_safePointGap);
            new Run(text, _languages, result, safe, 0).ScanCode(0);
            result.Sort((a, b) => a.OuterStart.CompareTo(b.OuterStart));
            return new HostScan(result, safe.ToArray());
        }

        public HostScan Resume(string text, int from, int minStop, Func<int, bool> knownSafe)
        {
            var result = new List<EmbeddedString>();
            var safe = new SafePoints(minStop, knownSafe, _safePointGap);
            new Run(text, _languages, result, safe, from).ScanCode(0);
            result.Sort((a, b) => a.OuterStart.CompareTo(b.OuterStart));
            return new HostScan(result, safe.ToArray(), safe.StoppedAt);
        }

        private sealed class Run
        {
            private readonly string _t;
            private readonly List<EmbeddedString> _result;
            private readonly MarkerTracker _markers;
            private readonly AcceptedEmbeddedLanguages _languages;
            private readonly SafePoints _safe;
            private int _i;

            public Run(string text, AcceptedEmbeddedLanguages languages, List<EmbeddedString> result, SafePoints safe = null, int start = 0)
            {
                _t = text;
                _languages = languages;
                _result = result;
                _markers = new MarkerTracker(languages);
                _safe = safe;
                _i = start;
            }

            /// <summary>
            /// Scans code up to the end of the text, or, inside an interpolation (<paramref name="closeRun"/> &gt; 0),
            /// up to the brace run that closes it. Returns the index of the closing run, or -1.
            /// </summary>
            public int ScanCode(int closeRun)
            {
                int braces = 0, parens = 0;
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
                    else if (c == '\'')
                    {
                        SkipChar();
                    }
                    else if (c == '"' || c == '$' || c == '@')
                    {
                        if (!TryString()) _i++;
                    }
                    else if (c == '{')
                    {
                        braces++; _i++;
                    }
                    else if (c == '}')
                    {
                        if (closeRun > 0 && braces == 0)
                        {
                            int run = BraceRun(_i);
                            if (run >= closeRun) return _i;
                        }
                        if (braces > 0) braces--;
                        _i++;
                    }
                    else if (c == '(' || c == '[')
                    {
                        parens++; _i++;
                    }
                    else if (c == ')' || c == ']')
                    {
                        if (parens > 0) parens--;
                        _i++;
                    }
                    else if (c == ':' && Next(1) == ':')
                    {
                        _i += 2;
                    }
                    else if (c == ':' && closeRun > 0 && braces == 0 && parens == 0)
                    {
                        // alignment / format specifier: plain text up to the closing brace
                        _i++;
                        while (_i < _t.Length && !(_t[_i] == '}' && BraceRun(_i) >= closeRun)) _i++;
                        if (_i < _t.Length) return _i;
                    }
                    else if (c == '\n' && _safe != null && closeRun == 0 && !_markers.Pending)
                    {
                        if (_safe.Reached(_i)) return -1;
                        _i++;
                    }
                    else
                    {
                        _i++;
                    }
                }
                return -1;
            }

            private char Next(int offset)
            {
                return _i + offset < _t.Length ? _t[_i + offset] : '\0';
            }

            private int BraceRun(int i)
            {
                int n = 0;
                while (i + n < _t.Length && _t[i + n] == '}') n++;
                return n;
            }

            private void SkipChar()
            {
                // 'a', '\n', '\'', '"'. Anything else is not a char literal.
                if (Next(1) == '\\')
                {
                    int e = _t.IndexOf('\'', Math.Min(_i + 3, _t.Length));
                    int nl = _t.IndexOf('\n', _i);
                    _i = e < 0 || (nl >= 0 && nl < e) ? _i + 1 : e + 1;
                }
                else if (Next(2) == '\'' && Next(1) != '\n') _i += 3;
                else _i++;
            }

            private bool TryString()
            {
                int start = _i, j = _i, dollars = 0, ats = 0;
                while (j < _t.Length && (_t[j] == '$' || _t[j] == '@') && j - start < 4)
                {
                    if (_t[j] == '$') dollars++; else ats++;
                    j++;
                }
                if (j >= _t.Length || _t[j] != '"' || ats > 1) return false;
                if (dollars > 1 && ats > 0) return false;

                string marked = _markers.Take(_t, start);
                string id = _languages.Accepts(marked) ? marked : null;
                var s = new EmbeddedString(id) { OuterStart = start };

                int quotes = 0;
                while (j + quotes < _t.Length && _t[j + quotes] == '"') quotes++;

                if (ats == 0 && quotes >= 3) ReadRaw(s, j, quotes, dollars);
                else ReadQuoted(s, j, ats > 0, dollars > 0);

                if (_languages.Resolve(_t, s)) _result.Add(s);
                return true;
            }

            private void ReadRaw(EmbeddedString s, int quoteStart, int quotes, int dollars)
            {
                s.Start = _i = quoteStart + quotes;
                while (_i < _t.Length)
                {
                    char c = _t[_i];
                    if (c == '"')
                    {
                        int run = 0;
                        while (_i + run < _t.Length && _t[_i + run] == '"') run++;
                        if (run >= quotes)
                        {
                            s.End = _i;
                            s.OuterEnd = _i = _i + quotes;
                            return;
                        }
                        _i += run;
                    }
                    else if (c == '{' && dollars > 0)
                    {
                        int run = 0;
                        while (_i + run < _t.Length && _t[_i + run] == '{') run++;
                        if (run < dollars) { _i += run; continue; }
                        ReadInterpolation(s, _i + run - dollars, dollars, dollars);
                    }
                    else
                    {
                        _i++;
                    }
                }
                s.End = s.OuterEnd = _t.Length;
            }

            private void ReadQuoted(EmbeddedString s, int quote, bool verbatim, bool interpolated)
            {
                s.Start = _i = quote + 1;
                while (_i < _t.Length)
                {
                    char c = _t[_i];
                    if (c == '"')
                    {
                        if (verbatim && Next(1) == '"')
                        {
                            s.Escapes.Add(new EscapeSequence(_i, 2, '"'));
                            _i += 2;
                            continue;
                        }
                        s.End = _i;
                        s.OuterEnd = ++_i;
                        return;
                    }
                    if (c == '\\' && !verbatim)
                    {
                        char value;
                        if (_i + 1 < _t.Length && Escapes.TryDecode(_t[_i + 1], out value))
                            s.Escapes.Add(new EscapeSequence(_i, 2, value));
                        _i += _i + 1 < _t.Length && _t[_i + 1] == '\n' ? 1 : 2; // never swallow the line break
                    }
                    else if (c == '\n' && !verbatim)
                    {
                        break; // unclosed regular string
                    }
                    else if (interpolated && (c == '{' || c == '}'))
                    {
                        if (Next(1) == c) { s.Escapes.Add(new EscapeSequence(_i, 2, c)); _i += 2; }
                        else if (c == '{') ReadInterpolation(s, _i, 1, 1);
                        else _i++;
                    }
                    else
                    {
                        _i++;
                    }
                }
                s.End = s.OuterEnd = Math.Min(_i, _t.Length);
            }

            /// <summary>Reads "{" code "}" starting at the opening delimiter; scans the code for nested strings.</summary>
            private void ReadInterpolation(EmbeddedString s, int open, int openLength, int closeRun)
            {
                _i = open + openLength;
                int close = ScanCode(closeRun);
                bool closed = close >= 0;
                int end = closed ? close + closeRun : _t.Length;
                s.Interpolations.Add(new Interpolation(open, end, openLength, closed ? closeRun : 0));
                _i = end;
            }
        }
    }
}
