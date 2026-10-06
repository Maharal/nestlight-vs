using System;
using System.Collections.Generic;

namespace NestLight
{
    /// <summary>
    /// Walks through JS/TS code, skipping comments and ordinary strings, and locates template literals
    /// tagged with html`...`, svg`...`, htm`...` (HTML) or css`...` (CSS), or preceded by a
    /// marker comment such as /* html */ or /* css */.
    /// </summary>
    internal static class TplScanner
    {
        public static List<TemplateInfo> FindTemplates(string text)
        {
            var result = new List<TemplateInfo>();
            if (text.IndexOf('`') < 0) return result;
            int i = 0;
            ScanCode(text, ref i, false, result);
            result.Sort((a, b) => a.Start.CompareTo(b.Start));
            return result;
        }

        private static void ScanCode(string t, ref int i, bool stopAtCloseBrace, List<TemplateInfo> result)
        {
            int depth = 0;
            while (i < t.Length)
            {
                char c = t[i];
                if (c == '/' && i + 1 < t.Length && t[i + 1] == '/')
                {
                    int e = t.IndexOf('\n', i);
                    i = e < 0 ? t.Length : e;
                }
                else if (c == '/' && i + 1 < t.Length && t[i + 1] == '*')
                {
                    int e = t.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    i = e < 0 ? t.Length : e + 2;
                }
                else if (c == '"' || c == '\'')
                {
                    i = SkipString(t, i);
                }
                else if (c == '`')
                {
                    TemplateKind? kind = GetKind(t, i);
                    ScanTemplate(t, ref i, kind, result);
                }
                else if (c == '{')
                {
                    depth++; i++;
                }
                else if (c == '}')
                {
                    if (stopAtCloseBrace && depth == 0) return;
                    if (depth > 0) depth--;
                    i++;
                }
                else
                {
                    i++;
                }
            }
        }

        private static void ScanTemplate(string t, ref int i, TemplateKind? kind, List<TemplateInfo> result)
        {
            var info = new TemplateInfo { Start = i + 1, Kind = kind.HasValue ? kind.Value : TemplateKind.Html };
            i++; // opening backtick
            while (i < t.Length)
            {
                char c = t[i];
                if (c == '\\')
                {
                    i += 2;
                }
                else if (c == '`')
                {
                    info.End = i;
                    i++;
                    if (kind.HasValue) result.Add(info);
                    return;
                }
                else if (c == '$' && i + 1 < t.Length && t[i + 1] == '{')
                {
                    int exprStart = i;
                    i += 2;
                    ScanCode(t, ref i, true, result);
                    bool closed = i < t.Length;                    // stopped at a '}'
                    int exprEnd = closed ? i + 1 : t.Length;       // after the '}'
                    var range = new TextRange(exprStart, exprEnd);
                    range.Closed = closed;
                    info.Expressions.Add(range);
                    i = exprEnd;
                }
                else
                {
                    i++;
                }
            }
            info.End = t.Length;
            if (kind.HasValue) result.Add(info);
        }

        private static int SkipString(string t, int i)
        {
            char quote = t[i++];
            while (i < t.Length)
            {
                char c = t[i];
                if (c == '\\') i += 2;
                else if (c == quote) return i + 1;
                else if (c == '\n') return i; // unclosed string
                else i++;
            }
            return t.Length;
        }

        private static TemplateKind? KindFromName(string name)
        {
            if (string.Equals(name, "html", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "svg", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "htm", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "language=html", StringComparison.OrdinalIgnoreCase))
                return TemplateKind.Html;
            if (string.Equals(name, "css", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "language=css", StringComparison.OrdinalIgnoreCase))
                return TemplateKind.Css;
            return null;
        }

        private static TemplateKind? GetKind(string t, int backtick)
        {
            // Form 1: html`...` / css`...` (also lit.html`...`)
            int j = backtick - 1;
            while (j >= 0 && (char.IsLetterOrDigit(t[j]) || t[j] == '_' || t[j] == '$')) j--;
            int len = backtick - 1 - j;
            if (len > 0)
            {
                TemplateKind? k = KindFromName(t.Substring(j + 1, len));
                if (k.HasValue) return k;
            }

            // Form 2: /* html */ `...`  or  /* css */ `...`
            int p = backtick - 1;
            while (p >= 0 && char.IsWhiteSpace(t[p])) p--;
            if (p >= 1 && t[p] == '/' && t[p - 1] == '*')
            {
                int s = t.LastIndexOf("/*", p - 1, StringComparison.Ordinal);
                int innerLen = p - 1 - (s + 2);
                if (s >= 0 && innerLen >= 0)
                    return KindFromName(t.Substring(s + 2, innerLen).Trim());
            }
            return null;
        }
    }
}
