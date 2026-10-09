using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Hosts
{
    /// <summary>
    /// JavaScript / TypeScript: template literals, marked by a tag glued to the backtick
    /// (<c>html`...`</c>, <c>ui.html`...`</c>) or by a marker comment (<c>/* css */</c>, <c>// language=sql</c>).
    /// Skips comments and ordinary strings. Templates nested in a <c>${}</c> are found at every level.
    /// </summary>
    internal sealed class JavaScriptHostScanner : IHostScanner
    {
        private readonly AcceptedEmbeddedLanguages _languages;

        public JavaScriptHostScanner(AcceptedEmbeddedLanguages languages)
        {
            if (languages == null) throw new ArgumentNullException("languages");
            _languages = languages;
        }

        public IReadOnlyList<EmbeddedString> Scan(string text)
        {
            var result = new List<EmbeddedString>();
            if (text.IndexOf('`') < 0) return result;
            new Run(text, _languages, result).ScanCode(false);
            result.Sort((a, b) => a.OuterStart.CompareTo(b.OuterStart));
            return result;
        }

        private sealed class Run
        {
            private readonly string _t;
            private readonly AcceptedEmbeddedLanguages _languages;
            private readonly List<EmbeddedString> _result;
            private readonly MarkerTracker _markers;
            private int _i;

            public Run(string text, AcceptedEmbeddedLanguages languages, List<EmbeddedString> result)
            {
                _t = text;
                _languages = languages;
                _result = result;
                _markers = new MarkerTracker(languages);
            }

            public void ScanCode(bool stopAtCloseBrace)
            {
                int depth = 0;
                while (_i < _t.Length)
                {
                    char c = _t[_i];
                    if (c == '/' && _i + 1 < _t.Length && _t[_i + 1] == '/')
                    {
                        int e = _t.IndexOf('\n', _i);
                        int end = e < 0 ? _t.Length : e;
                        _markers.Comment(_t, _i + 2, end, end);
                        _i = end;
                    }
                    else if (c == '/' && _i + 1 < _t.Length && _t[_i + 1] == '*')
                    {
                        int e = _t.IndexOf("*/", _i + 2, StringComparison.Ordinal);
                        int end = e < 0 ? _t.Length : e + 2;
                        if (e < 0) _markers.Comment(_t, 0, 0, _t.Length); // unterminated: never a marker
                        else _markers.Comment(_t, _i + 2, e, end);
                        _i = end;
                    }
                    else if (c == '"' || c == '\'')
                    {
                        _markers.Take(_t, _i);
                        _i = SkipString(_i);
                    }
                    else if (c == '`')
                    {
                        ScanTemplate();
                    }
                    else if (c == '{')
                    {
                        depth++; _i++;
                    }
                    else if (c == '}')
                    {
                        if (stopAtCloseBrace && depth == 0) return;
                        if (depth > 0) depth--;
                        _i++;
                    }
                    else
                    {
                        _i++;
                    }
                }
            }

            private void ScanTemplate()
            {
                int backtick = _i;
                string marked = _markers.Take(_t, backtick);
                string id = MarkerComment.IdBefore(_t, backtick);
                if (!_languages.Accepts(id)) id = _languages.Accepts(marked) ? marked : null;

                var info = new EmbeddedString(id) { OuterStart = backtick, Start = backtick + 1 };
                _i++; // opening backtick
                while (_i < _t.Length)
                {
                    char c = _t[_i];
                    if (c == '\\')
                    {
                        _i += 2;
                    }
                    else if (c == '`')
                    {
                        info.End = _i;
                        info.OuterEnd = ++_i;
                        if (id != null) _result.Add(info);
                        return;
                    }
                    else if (c == '$' && _i + 1 < _t.Length && _t[_i + 1] == '{')
                    {
                        int exprStart = _i;
                        _i += 2;
                        ScanCode(true);
                        bool closed = _i < _t.Length;              // stopped at a '}'
                        int exprEnd = closed ? _i + 1 : _t.Length; // after the '}'
                        info.Interpolations.Add(new Interpolation(exprStart, exprEnd, 2, closed ? 1 : 0));
                        _i = exprEnd;
                    }
                    else
                    {
                        _i++;
                    }
                }
                info.End = info.OuterEnd = _t.Length;
                if (id != null) _result.Add(info);
            }

            private int SkipString(int i)
            {
                char quote = _t[i++];
                while (i < _t.Length)
                {
                    char c = _t[i];
                    if (c == '\\') i += 2;
                    else if (c == quote) return i + 1;
                    else if (c == '\n') return i; // unclosed string
                    else i++;
                }
                return _t.Length;
            }
        }
    }
}
