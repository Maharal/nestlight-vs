using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Hosts
{
    /// <summary>
    /// Rust: raw string literals <c>r"..."</c>, <c>r#"..."#</c>, <c>r##"..."##</c> etc., and regular strings,
    /// marked by a comment before the string. Rust strings have no interpolation.
    /// </summary>
    internal sealed class RustHostScanner : HostScannerBase
    {
        /// <param name="safePointGap">The distance between the safe points a scan keeps to resume from (see <see cref="SafePoints"/>).</param>
        public RustHostScanner(AcceptedEmbeddedLanguages languages, int safePointGap = SafePoints.DefaultGap)
            : base(languages, safePointGap)
        {
        }

        protected override void ScanCode(string text, List<EmbeddedString> result, SafePoints safe, int start)
        {
            new Run(text, Languages, result, safe, start).ScanCode();
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
                    if (CStyleComments.TrySkip(_t, ref _i, _markers)) continue;

                    if (c == 'r' && (Next(1) == '"' || Next(1) == '#'))
                    {
                        TryRawString();
                    }
                    else if (c == 'b' && Next(1) == 'r' && (Next(2) == '"' || Next(2) == '#'))
                    {
                        _i++;
                        TryRawString();
                    }
                    else if (c == '"')
                    {
                        ReadQuoted();
                    }
                    else if (c == '\'')
                    {
                        SkipCharOrLifetime();
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

            private void TryRawString()
            {
                int outerStart = _i;
                _i++; // skip 'r'
                int hashes = 0;
                while (_i < _t.Length && _t[_i] == '#') { hashes++; _i++; }
                if (_i >= _t.Length || _t[_i] != '"')
                {
                    // not a raw string, just an identifier starting with r
                    return;
                }

                string marked = _markers.Take(_t, outerStart);
                string id = _languages.Accepts(marked) ? marked : null;
                var s = new EmbeddedString(id) { OuterStart = outerStart, Start = _i + 1 };

                _i++; // skip opening '"'
                while (_i < _t.Length)
                {
                    if (_t[_i] == '"')
                    {
                        int run = 0;
                        int p = _i + 1;
                        while (p < _t.Length && p - _i - 1 < hashes && _t[p] == '#') { run++; p++; }
                        if (run >= hashes)
                        {
                            s.End = _i;
                            s.OuterEnd = _i = _i + 1 + hashes;
                            if (_languages.Resolve(_t, s)) _result.Add(s);
                            return;
                        }
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

            private void SkipCharOrLifetime()
            {
                // 'a' is a char, 'a is a lifetime, '\\' is escaped char
                if (Next(1) == '\\')
                {
                    int e = _t.IndexOf('\'', Math.Min(_i + 3, _t.Length));
                    int nl = _t.IndexOf('\n', _i);
                    _i = e < 0 || (nl >= 0 && nl < e) ? _i + 1 : e + 1;
                }
                else if (Next(2) == '\'' && Next(1) != '\n') _i += 3;
                else _i++; // lifetime or lone quote
            }
        }
    }
}
