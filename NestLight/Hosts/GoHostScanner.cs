using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Hosts
{
    /// <summary>
    /// Go: backtick strings (raw, no escapes, no interpolation) and regular strings,
    /// marked by a comment before the string.
    /// </summary>
    internal sealed class GoHostScanner : IResumableHostScanner
    {
        private readonly AcceptedEmbeddedLanguages _languages;
        private readonly int _safePointGap;

        public GoHostScanner(AcceptedEmbeddedLanguages languages, int safePointGap = SafePoints.DefaultGap)
        {
            if (languages == null) throw new ArgumentNullException("languages");
            _languages = languages;
            _safePointGap = safePointGap;
        }

        public IReadOnlyList<EmbeddedString> Scan(string text)
        {
            var result = new List<EmbeddedString>();
            new Run(text, _languages, result).ScanCode();
            return result;
        }

        public HostScan ScanAll(string text)
        {
            var result = new List<EmbeddedString>();
            var safe = new SafePoints(_safePointGap);
            new Run(text, _languages, result, safe, 0).ScanCode();
            return new HostScan(result, safe.ToArray());
        }

        public HostScan Resume(string text, int from, int minStop, Func<int, bool> knownSafe)
        {
            var result = new List<EmbeddedString>();
            var safe = new SafePoints(minStop, knownSafe, _safePointGap);
            new Run(text, _languages, result, safe, from).ScanCode();
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
                    else if (c == '`')
                    {
                        ReadBacktick();
                    }
                    else if (c == '"')
                    {
                        ReadQuoted();
                    }
                    else if (c == '\'')
                    {
                        SkipRune();
                    }
                    else if (c == '\n' && _safe != null && !_markers.Pending)
                    {
                        if (_safe.Reached(_i)) return;
                        _i++;
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

            private void ReadBacktick()
            {
                int outerStart = _i;
                string marked = _markers.Take(_t, outerStart);
                string id = _languages.Accepts(marked) ? marked : null;
                var s = new EmbeddedString(id) { OuterStart = outerStart, Start = _i + 1 };

                _i++; // skip opening backtick
                while (_i < _t.Length)
                {
                    if (_t[_i] == '`')
                    {
                        s.End = _i;
                        s.OuterEnd = ++_i;
                        if (_languages.Resolve(_t, s)) _result.Add(s);
                        return;
                    }
                    _i++;
                }
                s.End = s.OuterEnd = _t.Length;
                if (_languages.Resolve(_t, s)) _result.Add(s);
            }

            private void ReadQuoted()
            {
                int outerStart = _i;
                string marked = _markers.Take(_t, outerStart);
                string id = _languages.Accepts(marked) ? marked : null;
                var s = new EmbeddedString(id) { OuterStart = outerStart, Start = _i + 1 };

                _i++; // skip opening '"'
                while (_i < _t.Length)
                {
                    char c = _t[_i];
                    if (c == '\\')
                    {
                        char value;
                        if (_i + 1 < _t.Length && Escapes.TryDecode(_t[_i + 1], out value))
                            s.Escapes.Add(new EscapeSequence(_i, 2, value));
                        _i += _i + 1 < _t.Length && _t[_i + 1] == '\n' ? 1 : 2;
                    }
                    else if (c == '"')
                    {
                        s.End = _i;
                        s.OuterEnd = ++_i;
                        if (_languages.Resolve(_t, s)) _result.Add(s);
                        return;
                    }
                    else if (c == '\n')
                    {
                        break;
                    }
                    else
                    {
                        _i++;
                    }
                }
                s.End = s.OuterEnd = Math.Min(_i, _t.Length);
                if (_languages.Resolve(_t, s)) _result.Add(s);
            }

            private void SkipRune()
            {
                if (Next(1) == '\\')
                {
                    int e = _t.IndexOf('\'', Math.Min(_i + 3, _t.Length));
                    int nl = _t.IndexOf('\n', _i);
                    _i = e < 0 || (nl >= 0 && nl < e) ? _i + 1 : e + 1;
                }
                else if (Next(2) == '\'' && Next(1) != '\n') _i += 3;
                else _i++;
            }
        }
    }
}
