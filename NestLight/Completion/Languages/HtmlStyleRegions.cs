using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Completion
{
    /// <summary>
    /// The CSS that lives inside an HTML string: the content of a <c>&lt;style&gt;</c> element and the value of a <c>style="..."</c>
    /// attribute. The highlighter already colors them as CSS; the completion needs to know where they are to complete them as CSS.
    /// </summary>
    internal static class HtmlStyleRegions
    {
        internal struct CssRange
        {
            public int Start, End;
            /// <summary>A style attribute: a list of declarations, not a style sheet.</summary>
            public bool Attribute;
        }

        /// <summary>The CSS inside the string, in order of appearance; an unclosed one runs to the end of the string.</summary>
        public static List<CssRange> CssIn(string text, EmbeddedString owner)
        {
            var ranges = new List<CssRange>();
            int n = Math.Min(owner.End, text.Length);
            var interpolations = owner.Interpolations;
            int i = owner.Start;
            while (i < n)
            {
                int skip = InterpolationEnd(interpolations, i);
                if (skip >= 0) { i = skip; continue; }
                if (text[i] != '<') { i++; continue; }
                if (string.CompareOrdinal(text, i, "<!--", 0, 4) == 0)
                {
                    int close = text.IndexOf("-->", i + 4, StringComparison.Ordinal);
                    i = close < 0 || close >= n ? n : close + 3;
                    continue;
                }

                int p = i + 1;
                bool closing = p < n && text[p] == '/';
                if (closing) p++;
                int nameStart = p;
                while (p < n && (char.IsLetterOrDigit(text[p]) || text[p] == '-' || text[p] == ':')) p++;
                if (p == nameStart) { i++; continue; }
                bool isStyleTag = p - nameStart == 5 && string.Compare(text, nameStart, "style", 0, 5, StringComparison.OrdinalIgnoreCase) == 0;

                bool opened = false;
                while (p < n)
                {
                    int s2 = InterpolationEnd(interpolations, p);
                    if (s2 >= 0) { p = s2; continue; }
                    char c = text[p];
                    if (char.IsWhiteSpace(c)) { p++; continue; }
                    if (c == '>') { opened = true; p++; break; }
                    if (c == '/' && p + 1 < n && text[p + 1] == '>') { p += 2; break; }
                    if (c == '<') break; // a tag that was never finished

                    int a = p;
                    while (p < n && !char.IsWhiteSpace(text[p]) && text[p] != '=' && text[p] != '>' && text[p] != '"' && text[p] != '\'' && text[p] != '<'
                           && !(text[p] == '/' && p + 1 < n && text[p + 1] == '>') && InterpolationEnd(interpolations, p) < 0)
                        p++;
                    if (p == a) { p++; continue; }
                    bool isStyleAttribute = !closing && p - a == 5 && string.Compare(text, a, "style", 0, 5, StringComparison.OrdinalIgnoreCase) == 0;

                    int q = p;
                    while (q < n && char.IsWhiteSpace(text[q])) q++;
                    if (q < n && text[q] == '=')
                    {
                        q++;
                        while (q < n && char.IsWhiteSpace(text[q])) q++;
                        if (q < n && (text[q] == '"' || text[q] == '\''))
                        {
                            char quote = text[q];
                            int valueStart = q + 1;
                            q = valueStart;
                            while (q < n && (text[q] != quote || InterpolationEnd(interpolations, q) >= 0))
                            {
                                int ie = InterpolationEnd(interpolations, q);
                                q = ie >= 0 ? ie : q + 1;
                            }
                            if (isStyleAttribute) ranges.Add(new CssRange { Start = valueStart, End = Math.Min(q, n), Attribute = true });
                            p = q < n ? q + 1 : n;
                        }
                        else
                        {
                            while (q < n && !char.IsWhiteSpace(text[q]) && text[q] != '>' && !(text[q] == '/' && q + 1 < n && text[q + 1] == '>'))
                            {
                                int ie = InterpolationEnd(interpolations, q);
                                q = ie >= 0 ? ie : q + 1;
                            }
                            p = q;
                        }
                    }
                }

                if (opened && !closing && isStyleTag)
                {
                    int close = text.IndexOf("</style", p, n - p, StringComparison.OrdinalIgnoreCase);
                    int end = close < 0 ? n : close;
                    ranges.Add(new CssRange { Start = p, End = end, Attribute = false });
                    p = end;
                }
                i = Math.Max(p, i + 1);
            }
            return ranges;
        }

        /// <summary>The CSS the caret is in, or false when it is in the HTML around it.</summary>
        public static bool TryCssAt(string text, EmbeddedString owner, int caret, out CssRange range)
        {
            foreach (CssRange r in CssIn(text, owner))
                if (r.Start <= caret && caret <= r.End) { range = r; return true; }
            range = default(CssRange);
            return false;
        }

        /// <summary>The end of the interpolation that contains the offset (its start included), or -1.</summary>
        private static int InterpolationEnd(List<Interpolation> interpolations, int offset)
        {
            int lo = 0, hi = interpolations.Count - 1;
            while (lo <= hi)
            {
                int mid = (lo + hi) / 2;
                Interpolation x = interpolations[mid];
                if (offset < x.Start) hi = mid - 1;
                else if (offset >= x.End) lo = mid + 1;
                else return x.End;
            }
            return -1;
        }
    }
}
