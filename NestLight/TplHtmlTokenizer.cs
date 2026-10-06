using System;
using System.Collections.Generic;

namespace NestLight
{
    /// <summary>
    /// Tokenizes template strings (HTML or CSS). The ${...} expressions are "masked"
    /// during analysis (replaced by a neutral character) and removed from the tokens at the end, so that
    /// they work in any position: text, attribute name, value, CSS property, etc.
    /// </summary>
    internal static class TplHtmlTokenizer
    {
        private const char Mask = '\u0001';

        public static void Tokenize(string text, TemplateInfo tpl, List<TemplateInfo> all, List<TplToken> output)
        {
            EmitExpressions(text, tpl, all, output);

            int s = tpl.Start;
            int n = tpl.End - tpl.Start;
            if (n <= 0) return;

            var m = new char[n];
            text.CopyTo(s, m, 0, n);
            foreach (var ex in tpl.Expressions)
            {
                int from = Math.Max(ex.Start, s);
                int to = Math.Min(ex.End, tpl.End);
                for (int k = from; k < to; k++) m[k - s] = Mask;
            }

            Action<int, int, string> add = (a, b, type) => AddClipped(s + a, s + b, type, tpl.Expressions, output);

            if (tpl.Kind == TemplateKind.Css)
            {
                TplCssTokenizer.Tokenize(m, 0, n, add);
                return;
            }

            int i = 0;
            while (i < n)
            {
                if (m[i] != '<') { i++; continue; }

                // Comment <!-- ... -->
                if (Starts(m, i, "<!--"))
                {
                    int end = IndexOf(m, "-->", i + 4);
                    int stop = end < 0 ? n : end + 3;
                    add(i, stop, TplNames.Comment);
                    i = stop;
                    continue;
                }

                int p = i + 1;
                bool closing = false;
                if (p < n && m[p] == '/') { closing = true; p++; }
                if (p >= n || !(char.IsLetter(m[p]) || m[p] == Mask)) { i++; continue; }

                add(i, p, TplNames.Delimiter);          // "<" or "</"
                int nameStart = p;
                while (p < n && IsNameChar(m[p])) p++;
                add(nameStart, p, TplNames.Tag);
                string tagName = new string(m, nameStart, p - nameStart).ToLowerInvariant();
                bool opened = false; // reached the '>' (not self-closing)

                // tag body: attributes
                while (p < n)
                {
                    char c = m[p];
                    if (char.IsWhiteSpace(c)) { p++; continue; }
                    if (c == '>') { add(p, p + 1, TplNames.Delimiter); p++; opened = true; break; }
                    if (c == '/' && p + 1 < n && m[p + 1] == '>') { add(p, p + 2, TplNames.Delimiter); p += 2; break; }
                    if (c == '<') break; // malformed tag; restart analysis

                    int a = p;
                    while (p < n && !char.IsWhiteSpace(m[p]) && m[p] != '=' && m[p] != '>' &&
                           m[p] != '"' && m[p] != '\'' && m[p] != '<' &&
                           !(m[p] == '/' && p + 1 < n && m[p + 1] == '>'))
                        p++;
                    if (p == a) { p++; continue; } // stray quotes etc.

                    add(a, p, AttributeType(m[a]));
                    bool isStyleAttr = string.Equals(new string(m, a, p - a), "style", StringComparison.OrdinalIgnoreCase);

                    int q = p;
                    while (q < n && char.IsWhiteSpace(m[q])) q++;
                    if (q < n && m[q] == '=')
                    {
                        q++;
                        while (q < n && char.IsWhiteSpace(m[q])) q++;
                        if (q < n && (m[q] == '"' || m[q] == '\''))
                        {
                            char quote = m[q];
                            int v = q;
                            q++;
                            while (q < n && m[q] != quote) q++;
                            bool closed = q < n;
                            if (isStyleAttr)
                            {
                                // style="prop: value; ..." -> CSS between the quotes
                                add(v, v + 1, TplNames.AttributeValue);
                                TplCssTokenizer.Tokenize(m, v + 1, q, add);
                                if (closed) add(q, q + 1, TplNames.AttributeValue);
                            }
                            else
                            {
                                add(v, closed ? q + 1 : n, TplNames.AttributeValue);
                            }
                            q = closed ? q + 1 : n;
                        }
                        else
                        {
                            int v = q;
                            while (q < n && !char.IsWhiteSpace(m[q]) && m[q] != '>' &&
                                   !(m[q] == '/' && q + 1 < n && m[q + 1] == '>'))
                                q++;
                            add(v, q, TplNames.AttributeValue);
                        }
                        p = q;
                    }
                }

                // <style> ... </style> -> CSS
                if (opened && !closing && tagName == "style")
                {
                    int close = IndexOfIgnoreCase(m, "</style", p);
                    int end = close < 0 ? n : close;
                    TplCssTokenizer.Tokenize(m, p, end, add);
                    p = end;
                }
                i = p;
            }
        }

        private static string AttributeType(char first)
        {
            switch (first)
            {
                case '@': return TplNames.AttributeEvent;
                case '.': return TplNames.AttributeProperty;
                case '?': return TplNames.AttributeBoolean;
                default: return TplNames.Attribute;
            }
        }

        private static bool IsNameChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == ':' || c == '.' || c == Mask;
        }

        private static bool Starts(char[] m, int i, string s)
        {
            if (i + s.Length > m.Length) return false;
            for (int k = 0; k < s.Length; k++) if (m[i + k] != s[k]) return false;
            return true;
        }

        private static int IndexOf(char[] m, string s, int from)
        {
            for (int i = from; i + s.Length <= m.Length; i++) if (Starts(m, i, s)) return i;
            return -1;
        }

        private static int IndexOfIgnoreCase(char[] m, string s, int from)
        {
            for (int i = from; i + s.Length <= m.Length; i++)
            {
                bool ok = true;
                for (int k = 0; k < s.Length && ok; k++)
                    if (char.ToLowerInvariant(m[i + k]) != s[k]) ok = false;
                if (ok) return i;
            }
            return -1;
        }

        /// <summary>Adds the token [start,end) removing the parts that fall inside ${...}.</summary>
        private static void AddClipped(int start, int end, string type, List<TextRange> exprs, List<TplToken> output)
        {
            foreach (var ex in exprs)
            {
                if (ex.End <= start) continue;
                if (ex.Start >= end) break;
                if (ex.Start > start) output.Add(new TplToken(start, ex.Start - start, type));
                start = Math.Max(start, ex.End);
                if (start >= end) return;
            }
            if (end > start) output.Add(new TplToken(start, end - start, type));
        }

        private static void EmitExpressions(string text, TemplateInfo tpl, List<TemplateInfo> all, List<TplToken> output)
        {
            foreach (var ex in tpl.Expressions)
            {
                int open = Math.Min(ex.Start + 2, ex.End);
                output.Add(new TplToken(ex.Start, open - ex.Start, TplNames.ExprDelimiter));

                int close = ex.Closed ? ex.End - 1 : ex.End;
                if (ex.Closed) output.Add(new TplToken(close, 1, TplNames.ExprDelimiter));

                // Templates nested inside the expression take care of their own color.
                var holes = new List<TextRange>();
                foreach (var other in all)
                {
                    if (ReferenceEquals(other, tpl)) continue;
                    // starts inside the expression (even if still unclosed, while typing)
                    if (other.Start - 1 >= open && other.Start - 1 < close)
                        holes.Add(new TextRange(other.Start - 1, Math.Min(other.End + 1, close)));
                }
                holes.Sort((x, y) => x.Start.CompareTo(y.Start));

                int cur = open;
                foreach (var h in holes)
                {
                    if (h.End <= cur) continue;
                    if (h.Start >= close) break;
                    if (h.Start > cur) output.Add(new TplToken(cur, h.Start - cur, TplNames.Expression));
                    cur = Math.Max(cur, h.End);
                }
                if (cur < close) output.Add(new TplToken(cur, close - cur, TplNames.Expression));
            }
        }
    }
}
