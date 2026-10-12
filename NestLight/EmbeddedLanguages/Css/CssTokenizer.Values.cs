using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.EmbeddedLanguages
{
    /// <summary>The value part of <see cref="CssTokenizer"/>: the tokens of a declaration's value.</summary>
    internal sealed partial class CssTokenizer
    {
        private static void Value(char[] m, int s, int e, TokenSink add)
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
                if (c == '!')
                {
                    int q = p + 1;
                    while (q < e && char.IsWhiteSpace(m[q])) q++;
                    while (q < e && IsNameChar(m[q])) q++;
                    add(p, q, ClassificationNames.CssImportant);
                    p = q;
                    continue;
                }
                if (c == '#')
                {
                    int q = p + 1;
                    while (q < e && IsNameChar(m[q])) q++;
                    add(p, q, ClassificationNames.CssNumber); // hex color
                    p = q;
                    continue;
                }
                if (StartsNumber(m, p, e))
                {
                    int q = p;
                    if (m[q] == '+' || m[q] == '-') q++;
                    while (q < e && (char.IsDigit(m[q]) || m[q] == '.')) q++;
                    // 1e3, 2.5E-2: an exponent, not a unit
                    if (q + 1 < e && (m[q] == 'e' || m[q] == 'E') && (char.IsDigit(m[q + 1]) || ((m[q + 1] == '+' || m[q + 1] == '-') && q + 2 < e && char.IsDigit(m[q + 2]))))
                    {
                        q += 2;
                        while (q < e && char.IsDigit(m[q])) q++;
                    }
                    int numberEnd = q;
                    while (q < e && (char.IsLetter(m[q]) || m[q] == '%' || m[q] == Mask)) q++;
                    add(p, numberEnd, ClassificationNames.CssNumber);
                    if (q > numberEnd) add(numberEnd, q, ClassificationNames.CssUnit); // 10px: the number and its unit
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
                    if (maskUnit)
                    {
                        // ${expr}px: the expression and then its unit
                        int k = nameStart;
                        while (k < q && m[k] == Mask) k++;
                        add(nameStart, k, ClassificationNames.CssNumber);
                        if (k < q) add(k, q, ClassificationNames.CssUnit);
                        p = q;
                        continue;
                    }
                    string type = isFn ? ClassificationNames.CssFunction
                                : (q - p >= 2 && m[p] == '-' && m[p + 1] == '-') ? ClassificationNames.CssCustomProperty
                                : ClassificationNames.CssValue;
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
                            add(q, q + 1, ClassificationNames.CssPunct);
                            add(a, close, ClassificationNames.CssString);
                            p = close;
                        }
                    }
                    continue;
                }

                add(p, p + 1, ClassificationNames.CssPunct);
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
    }
}
