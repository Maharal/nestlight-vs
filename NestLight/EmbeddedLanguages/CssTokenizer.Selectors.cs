using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.EmbeddedLanguages
{
    /// <summary>The selector part of <see cref="CssTokenizer"/>: compound selectors, pseudo-classes and attribute selectors.</summary>
    internal sealed partial class CssTokenizer
    {
        private static void Selector(char[] m, int s, int e, TokenSink add)
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
                    add(p, q, ClassificationNames.CssString);
                    p = q;
                    continue;
                }
                if ((c == '.' || c == '#') && p + 1 < e && IsNameChar(m[p + 1]))
                {
                    int q = p + 1;
                    while (q < e && IsNameChar(m[q])) q++;
                    add(p, q, c == '#' ? ClassificationNames.CssSelectorId : ClassificationNames.CssSelectorClass);
                    p = q;
                    continue;
                }
                if (c == ':')
                {
                    int q = p + 1;
                    if (q < e && m[q] == ':') q++;
                    int nameStart = q;
                    while (q < e && IsNameChar(m[q])) q++;
                    add(p, q, ClassificationNames.CssPseudo);
                    p = q;
                    if (p < e && m[p] == '(')
                    {
                        // :not(.a, .b), :is(...), :has(...) hold selectors; :nth-child(2n+1) holds a formula
                        int close = MatchParen(m, p, e);
                        add(p, p + 1, ClassificationNames.CssPunct);
                        if (IsNth(m, nameStart, q)) Value(m, p + 1, close, add);
                        else Selector(m, p + 1, close, add);
                        if (close < e) add(close, close + 1, ClassificationNames.CssPunct);
                        p = Math.Min(e, close + 1);
                    }
                    continue;
                }
                if (c == '[') { p = AttributeSelector(m, p, e, add); continue; }
                if (c == ',' || c == '>' || c == '+' || c == '~' || c == '(' || c == ')' || c == '|')
                {
                    add(p, p + 1, ClassificationNames.CssPunct);
                    p++;
                    continue;
                }
                if (StartsNumber(m, p, e))
                {
                    // the percentages of a keyframe: 50%
                    int q = p;
                    while (q < e && (char.IsDigit(m[q]) || m[q] == '.')) q++;
                    add(p, q, ClassificationNames.CssNumber);
                    if (q < e && m[q] == '%') { add(q, q + 1, ClassificationNames.CssUnit); q++; }
                    p = q;
                    continue;
                }
                int r = p + 1;
                while (r < e && !char.IsWhiteSpace(m[r]) && m[r] != '.' && m[r] != '#' && m[r] != ':' && m[r] != '[' && m[r] != ',' && m[r] != '>' && m[r] != '+'
                       && m[r] != '~' && m[r] != '(' && m[r] != ')' && m[r] != '|' && m[r] != '"' && m[r] != '\'' && !IsCommentStart(m, r, e))
                    r++;
                add(p, r, ClassificationNames.CssSelector);
                p = r;
            }
        }

        /// <summary>[name], [name=value], [name~="value" i]: the brackets and the operator are punctuation, the name an attribute, the value a string or a word.</summary>
        private static int AttributeSelector(char[] m, int p, int e, TokenSink add)
        {
            add(p, p + 1, ClassificationNames.CssPunct);
            int q = p + 1;
            bool value = false;
            while (q < e && m[q] != ']')
            {
                char c = m[q];
                if (char.IsWhiteSpace(c)) { q++; continue; }
                if (IsCommentStart(m, q, e)) { q = Comment(m, q, e, add); continue; }
                if (c == '"' || c == '\'')
                {
                    int end = StringEnd(m, q, e);
                    add(q, end, ClassificationNames.CssString);
                    q = end;
                    continue;
                }
                if (IsNameChar(c))
                {
                    int start = q;
                    while (q < e && IsNameChar(m[q])) q++;
                    // the first word is the attribute; after the operator, a word is a value (and a lone i or s is a flag)
                    add(start, q, value ? ClassificationNames.CssValue : ClassificationNames.CssAttribute);
                    continue;
                }
                if (c == '=') value = true;
                add(q, q + 1, ClassificationNames.CssPunct);
                q++;
            }
            if (q < e) { add(q, q + 1, ClassificationNames.CssPunct); q++; }
            return q;
        }

        private static bool IsNth(char[] m, int s, int e)
        {
            if (e - s < 4 || char.ToLowerInvariant(m[s]) != 'n' || char.ToLowerInvariant(m[s + 1]) != 't' || char.ToLowerInvariant(m[s + 2]) != 'h' || m[s + 3] != '-') return false;
            return true;
        }

        /// <summary>The index of the parenthesis that closes the one at <paramref name="open"/>, or <paramref name="e"/>.</summary>
        private static int MatchParen(char[] m, int open, int e)
        {
            int depth = 0;
            for (int p = open; p < e; p++)
            {
                char c = m[p];
                if (c == '"' || c == '\'') { p = StringEnd(m, p, e) - 1; continue; }
                if (c == '(') depth++;
                else if (c == ')' && --depth == 0) return p;
            }
            return e;
        }
    }
}
