using System;
using System.Collections.Generic;
using NestLight.Highlighting;

namespace NestLight.Experiments.Detection
{
    /// <summary>
    /// Finds the string literals that are NOT marked, which is where an automatic detector would look. A simplified lexer, enough for the
    /// synthetic files of the experiment (plain, verbatim and raw strings, template literals, triple quotes, line and block comments,
    /// character literals). In the plugin this work would be done by the host scanner in the pass it already makes, so the time of this
    /// finder is reported apart: it is an upper bound of the cost of finding the strings, not a cost the detector adds.
    /// </summary>
    internal static class LiteralFinder
    {
        /// <summary>Fills <paramref name="bounds"/> with start, end pairs of the content of every unmarked literal.</summary>
        public static void Find(HostLanguage host, string t, List<int> bounds)
        {
            bounds.Clear();
            bool hash = host == HostLanguage.Python;
            bool backtick = host == HostLanguage.JavaScript;
            bool charLiterals = host == HostLanguage.CSharp || host == HostLanguage.Cpp;
            bool marked = false;
            int i = 0, n = t.Length;
            while (i < n)
            {
                char c = t[i];
                if (!hash && c == '/' && i + 1 < n && t[i + 1] == '/') { int e = LineEnd(t, i); marked = IsMarker(t, i, e); i = e; }
                else if (hash && c == '#') { int e = LineEnd(t, i); marked = IsMarker(t, i, e); i = e; }
                else if (!hash && c == '/' && i + 1 < n && t[i + 1] == '*')
                {
                    int e = t.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    e = e < 0 ? n : e + 2;
                    marked = IsMarker(t, i, e);
                    i = e;
                }
                else if (charLiterals && c == '\'') i = Math.Min(n, i + (i + 1 < n && t[i + 1] == '\\' ? 4 : 3));
                else if (c == '"' || c == '\'' || (backtick && c == '`'))
                {
                    bool tagged = c == '`' && i > 0 && (char.IsLetterOrDigit(t[i - 1]) || t[i - 1] == '_');
                    int start, end;
                    i = Literal(host, t, i, out start, out end);
                    if (!marked && !tagged && end > start) { bounds.Add(start); bounds.Add(end); }
                    marked = false;
                }
                else i++;
            }
        }

        private static int LineEnd(string t, int from)
        {
            int e = t.IndexOf('\n', from);
            return e < 0 ? t.Length : e;
        }

        private static bool IsMarker(string t, int start, int end)
        {
            return t.IndexOf("language=", start, end - start, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static int Literal(HostLanguage host, string t, int at, out int start, out int end)
        {
            int n = t.Length;
            char q = t[at];
            if (host == HostLanguage.Cpp && q == '"' && at > 0 && t[at - 1] == 'R' && at + 1 < n && t[at + 1] == '(')
            {
                start = at + 2;
                int close = t.IndexOf(")\"", start, StringComparison.Ordinal);
                end = close < 0 ? n : close;
                return close < 0 ? n : close + 2;
            }
            if (host == HostLanguage.Python && at + 2 < n && t[at + 1] == q && t[at + 2] == q)
            {
                start = at + 3;
                int close = t.IndexOf(new string(q, 3), start, StringComparison.Ordinal);
                end = close < 0 ? n : close;
                return close < 0 ? n : close + 3;
            }
            bool verbatim = host == HostLanguage.CSharp && at > 0 && t[at - 1] == '@';
            bool multiline = q == '`' || verbatim;
            int i = at + 1;
            start = i;
            while (i < n)
            {
                char c = t[i];
                if (c == '\\' && !verbatim) i += 2;
                else if (c == q)
                {
                    if (verbatim && i + 1 < n && t[i + 1] == q) { i += 2; continue; }
                    end = i;
                    return i + 1;
                }
                else if (c == '\n' && !multiline) { end = i; return i; }
                else i++;
            }
            end = n;
            return n;
        }
    }
}
