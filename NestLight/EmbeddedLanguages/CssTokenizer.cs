using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.EmbeddedLanguages
{
    /// <summary>
    /// Simple, error-tolerant CSS tokenizer. Works on "masked" text
    /// (the ${...} expressions have already been replaced by the Mask character). Used for css`...` templates,
    /// &lt;style&gt; blocks and style="..." attributes inside HTML templates.
    /// Recognizes nesting (CSS nesting), at-rules, selectors, properties and values.
    /// </summary>
    internal sealed partial class CssTokenizer : IEmbeddedLanguageTokenizer
    {
        private const char Mask = TextUtil.Mask;

        private static readonly string[] CssIds = { "css" };

        public IReadOnlyList<string> Ids { get { return CssIds; } }

        /// <param name="m">Masked template text.</param>
        /// <param name="from">Start (inclusive) of the CSS range.</param>
        /// <param name="to">End (exclusive) of the CSS range.</param>
        /// <param name="add">Callback (start, end, type) in coordinates of <paramref name="m"/>.</param>
        public void Tokenize(char[] m, int from, int to, TokenSink add)
        {
            int i = from;
            while (i < to)
            {
                char c = m[i];
                if (char.IsWhiteSpace(c)) { i++; continue; }
                if (IsCommentStart(m, i, to)) { i = Comment(m, i, to, add); continue; }
                if (c == '{' || c == '}' || c == ';')
                {
                    add(i, i + 1, ClassificationNames.CssPunct);
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
                    add(p, p + 1, ClassificationNames.CssPunct);
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

        private static void AtRule(char[] m, int s, int e, TokenSink add)
        {
            int q = s + 1;
            while (q < e && IsNameChar(m[q])) q++;
            add(s, q, ClassificationNames.CssAtRule);
            Prelude(m, q, e, add);
        }

        /// <summary>
        /// The prelude of an at-rule. A parenthesis that does not follow a name is a condition (a media feature, a supports test, a container
        /// size): <c>(min-width: 600px)</c>, <c>(width &gt; 600px)</c>, <c>(hover)</c>; what is outside is read as a value.
        /// </summary>
        private static void Prelude(char[] m, int s, int e, TokenSink add)
        {
            int p = s, segment = s;
            while (p < e)
            {
                char c = m[p];
                if (c == '"' || c == '\'') { p = StringEnd(m, p, e); continue; }
                if (IsCommentStart(m, p, e)) { p = CommentEnd(m, p, e); continue; }
                bool function = p > s && IsNameChar(m[p - 1]);
                if (c == '(' && !function)
                {
                    if (p > segment) Value(m, segment, p, add);
                    int close = MatchParen(m, p, e);
                    Condition(m, p, close, e, add);
                    p = Math.Min(e, close + 1);
                    segment = p;
                    continue;
                }
                if (c == '(') p = MatchParen(m, p, e) + 1;
                else p++;
            }
            if (segment < e) Value(m, segment, Math.Min(p, e), add);
        }

        private static void Condition(char[] m, int open, int close, int e, TokenSink add)
        {
            add(open, open + 1, ClassificationNames.CssPunct);
            int p = open + 1;
            while (p < close && char.IsWhiteSpace(m[p])) p++;
            int nameEnd = p;
            while (nameEnd < close && IsNameChar(m[nameEnd])) nameEnd++;
            int after = nameEnd;
            while (after < close && char.IsWhiteSpace(m[after])) after++;
            bool feature = nameEnd > p && !char.IsDigit(m[p]) &&
                           (after >= close || m[after] == ':' || m[after] == '<' || m[after] == '>' || m[after] == '=');
            if (feature)
            {
                add(p, nameEnd, ClassificationNames.CssProperty);
                p = nameEnd;
            }
            Value(m, p, close, add);
            if (close < e) add(close, close + 1, ClassificationNames.CssPunct);
        }

        private static void Declaration(char[] m, int s, int e, TokenSink add)
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
                add(a, nameEnd, custom ? ClassificationNames.CssCustomProperty : ClassificationNames.CssProperty);
            }
            if (colon >= 0)
            {
                add(colon, colon + 1, ClassificationNames.CssPunct);
                Value(m, colon + 1, b, add);
            }
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

        private static int Comment(char[] m, int p, int e, TokenSink add)
        {
            int end = CommentEnd(m, p, e);
            add(p, end, ClassificationNames.CssComment);
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
