using System;

namespace NestLight
{
    /// <summary>
    /// Simple, error-tolerant CSS tokenizer. Works on "masked" text
    /// (the ${...} expressions have already been replaced by the Mask character). Used for css`...` templates,
    /// &lt;style&gt; blocks and style="..." attributes inside HTML templates.
    /// Recognizes nesting (CSS nesting), at-rules, selectors, properties and values.
    /// </summary>
    internal static class TplCssTokenizer
    {
        private const char Mask = '\u0001';

        /// <param name="m">Masked template text.</param>
        /// <param name="from">Start (inclusive) of the CSS range.</param>
        /// <param name="to">End (exclusive) of the CSS range.</param>
        /// <param name="add">Callback (start, end, type) in coordinates of <paramref name="m"/>.</param>
        public static void Tokenize(char[] m, int from, int to, Action<int, int, string> add)
        {
            int i = from;
            while (i < to)
            {
                char c = m[i];
                if (char.IsWhiteSpace(c)) { i++; continue; }
                if (IsCommentStart(m, i, to)) { i = Comment(m, i, to, add); continue; }
                if (c == '{' || c == '}' || c == ';')
                {
                    add(i, i + 1, TplNames.CssPunct);
                    i++;
                    continue;
                }

                // Reads the "prelude": everything up to the next top-level { ; }.
                int start = i, p = i, paren = 0;
                char term = '\0';
                while (p < to)
                {
                    char d = m[p];
                    if (IsCommentStart(m, p, to)) { p = CommentEnd(m, p, to); continue; }
                    if (d == '"' || d == '\'') { p = StringEnd(m, p, to); continue; }
                    if (d == '(' || d == '[') paren++;
                    else if ((d == ')' || d == ']') && paren > 0) paren--;
                    else if (paren == 0 && (d == '{' || d == ';' || d == '}')) { term = d; break; }
                    p++;
                }

                if (term == '{')
                {
                    // nested rule: selector or at-rule with a block
                    if (m[start] == '@') AtRule(m, start, p, add);
                    else Selector(m, start, p, add);
                    add(p, p + 1, TplNames.CssPunct);
                    i = p + 1;
                }
                else
                {
                    // declaration (property: value) or at-rule without a block
                    Declaration(m, start, p, add);
                    i = p; // the ; or } is handled at the top of the loop
                }
            }
        }

        private static void Selector(char[] m, int s, int e, Action<int, int, string> add)
        {
            int p = s;
            while (p < e)
            {
                char c = m[p];
                if (char.IsWhiteSpace(c)) { p++; continue; }
                if (IsCommentStart(m, p, e)) { p = Comment(m, p, e, add); continue; }
                if (c == '"' || c == '\'')
                {
                    int q = StringEnd(m, p, e);
                    add(p, q, TplNames.CssString);
                    p = q;
                    continue;
                }
                if ((c == '.' || c == '#') && p + 1 < e && IsNameChar(m[p + 1]))
                {
                    int q = p + 1;
                    while (q < e && IsNameChar(m[q])) q++;
                    add(p, q, TplNames.CssSelectorClass);
                    p = q;
                    continue;
                }
                if (c == ':')
                {
                    int q = p + 1;
                    if (q < e && m[q] == ':') q++;
                    while (q < e && IsNameChar(m[q])) q++;
                    add(p, q, TplNames.CssPseudo);
                    p = q;
                    continue;
                }
                int r = p + 1;
                while (r < e && !char.IsWhiteSpace(m[r]) && m[r] != '.' && m[r] != '#' && m[r] != ':' &&
                       m[r] != '"' && m[r] != '\'' && !IsCommentStart(m, r, e))
                    r++;
                add(p, r, TplNames.CssSelector);
                p = r;
            }
        }

        private static void AtRule(char[] m, int s, int e, Action<int, int, string> add)
        {
            int q = s + 1;
            while (q < e && IsNameChar(m[q])) q++;
            add(s, q, TplNames.CssAtRule);
            Value(m, q, e, add);
        }

        private static void Declaration(char[] m, int s, int e, Action<int, int, string> add)
        {
            int a = s;
            while (a < e && char.IsWhiteSpace(m[a])) a++;
            int b = e;
            while (b > a && char.IsWhiteSpace(m[b - 1])) b--;
            if (a >= b) return;

            // "${mixin} prop: value" -> skips stray expressions (followed by whitespace) at the start
            while (a < b && m[a] == Mask)
            {
                int k = a;
                while (k < b && m[k] == Mask) k++;
                if (k < b && char.IsWhiteSpace(m[k]))
                {
                    while (k < b && char.IsWhiteSpace(m[k])) k++;
                    a = k;
                }
                else break;
            }
            if (a >= b) return;

            if (m[a] == '@') { AtRule(m, a, b, add); return; }

            int colon = -1, paren = 0;
            for (int p = a; p < b;)
            {
                char c = m[p];
                if (IsCommentStart(m, p, b)) { p = CommentEnd(m, p, b); continue; }
                if (c == '"' || c == '\'') { p = StringEnd(m, p, b); continue; }
                if (c == '(' || c == '[') paren++;
                else if ((c == ')' || c == ']') && paren > 0) paren--;
                else if (c == ':' && paren == 0) { colon = p; break; }
                p++;
            }

            int nameEnd = colon < 0 ? b : colon;
            while (nameEnd > a && char.IsWhiteSpace(m[nameEnd - 1])) nameEnd--;
            if (nameEnd > a)
            {
                bool custom = nameEnd - a >= 2 && m[a] == '-' && m[a + 1] == '-';
                add(a, nameEnd, custom ? TplNames.CssCustomProperty : TplNames.CssProperty);
            }
            if (colon >= 0)
            {
                add(colon, colon + 1, TplNames.CssPunct);
                Value(m, colon + 1, b, add);
            }
        }

        private static void Value(char[] m, int s, int e, Action<int, int, string> add)
        {
            int p = s;
            while (p < e)
            {
                char c = m[p];
                if (char.IsWhiteSpace(c)) { p++; continue; }
                if (IsCommentStart(m, p, e)) { p = Comment(m, p, e, add); continue; }
                if (c == '"' || c == '\'')
                {
                    int q = StringEnd(m, p, e);
                    add(p, q, TplNames.CssString);
                    p = q;
                    continue;
                }
                if (c == '!')
                {
                    int q = p + 1;
                    while (q < e && char.IsWhiteSpace(m[q])) q++;
                    while (q < e && IsNameChar(m[q])) q++;
                    add(p, q, TplNames.CssAtRule); // !important
                    p = q;
                    continue;
                }
                if (c == '#')
                {
                    int q = p + 1;
                    while (q < e && IsNameChar(m[q])) q++;
                    add(p, q, TplNames.CssNumber); // hex color
                    p = q;
                    continue;
                }
                if (StartsNumber(m, p, e))
                {
                    int q = p;
                    if (m[q] == '+' || m[q] == '-') q++;
                    while (q < e && (char.IsDigit(m[q]) || m[q] == '.')) q++;
                    while (q < e && (char.IsLetter(m[q]) || m[q] == '%' || m[q] == Mask)) q++;
                    add(p, q, TplNames.CssNumber);
                    p = q;
                    continue;
                }
                if (IsNameChar(m[p]) && !(c == '-' && !(p + 1 < e && IsNameChar(m[p + 1]))))
                {
                    int nameStart = p;
                    int q = p;
                    while (q < e && IsNameChar(m[q])) q++;
                    bool isFn = q < e && m[q] == '(';
                    // ${expr}px  -> treated as a number with a unit
                    bool maskUnit = false;
                    if (!isFn && m[nameStart] == Mask)
                    {
                        int k = nameStart;
                        while (k < q && m[k] == Mask) k++;
                        maskUnit = k == q;
                        if (!maskUnit)
                        {
                            maskUnit = true;
                            for (int j = k; j < q; j++) if (!char.IsLetter(m[j])) { maskUnit = false; break; }
                        }
                    }
                    string type = maskUnit ? TplNames.CssNumber
                                : isFn ? TplNames.CssFunction
                                : (q - p >= 2 && m[p] == '-' && m[p + 1] == '-') ? TplNames.CssCustomProperty
                                : TplNames.CssValue;
                    add(p, q, type);
                    p = q;

                    // url(unquoted) -> content as a string
                    if (isFn && IsUrl(m, nameStart, q))
                    {
                        int a = q + 1, k = a;
                        while (k < e && char.IsWhiteSpace(m[k])) k++;
                        if (k < e && m[k] != '"' && m[k] != '\'')
                        {
                            int close = a;
                            while (close < e && m[close] != ')') close++;
                            add(q, q + 1, TplNames.CssPunct);
                            add(a, close, TplNames.CssString);
                            p = close;
                        }
                    }
                    continue;
                }

                add(p, p + 1, TplNames.CssPunct);
                p++;
            }
        }

        private static bool IsUrl(char[] m, int s, int e)
        {
            return e - s == 3 && char.ToLowerInvariant(m[s]) == 'u' && char.ToLowerInvariant(m[s + 1]) == 'r' &&
                   char.ToLowerInvariant(m[s + 2]) == 'l';
        }

        private static bool StartsNumber(char[] m, int p, int e)
        {
            char c = m[p];
            if (char.IsDigit(c)) return true;
            if (c == '.') return p + 1 < e && char.IsDigit(m[p + 1]);
            if (c == '+' || c == '-')
            {
                if (p + 1 >= e) return false;
                if (char.IsDigit(m[p + 1])) return true;
                return m[p + 1] == '.' && p + 2 < e && char.IsDigit(m[p + 2]);
            }
            return false;
        }

        private static bool IsNameChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == Mask;
        }

        private static bool IsCommentStart(char[] m, int p, int e)
        {
            return m[p] == '/' && p + 1 < e && m[p + 1] == '*';
        }

        private static int CommentEnd(char[] m, int p, int e)
        {
            for (int k = p + 2; k + 1 < e; k++)
                if (m[k] == '*' && m[k + 1] == '/') return k + 2;
            return e;
        }

        private static int Comment(char[] m, int p, int e, Action<int, int, string> add)
        {
            int end = CommentEnd(m, p, e);
            add(p, end, TplNames.CssComment);
            return end;
        }

        private static int StringEnd(char[] m, int p, int e)
        {
            char quote = m[p];
            int q = p + 1;
            while (q < e)
            {
                char c = m[q];
                if (c == '\\') q += 2;
                else if (c == quote) return q + 1;
                else if (c == '\n') return q;
                else q++;
            }
            return e;
        }
    }
}
