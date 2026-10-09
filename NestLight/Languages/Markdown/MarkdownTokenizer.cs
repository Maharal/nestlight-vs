using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Languages
{
    /// <summary>
    /// Markdown: headings, emphasis and strong text, code spans and fenced blocks, links and list markers.
    /// Block rules look at one line at a time; inline rules never cross a line.
    /// </summary>
    internal sealed class MarkdownTokenizer : IEmbeddedLanguageTokenizer
    {
        private static readonly string[] MarkdownIds = { "markdown", "md" };

        public IReadOnlyList<string> Ids { get { return MarkdownIds; } }

        public void Tokenize(char[] m, int from, int to, TokenSink emit)
        {
            bool fenced = false;
            char fenceChar = '`';
            int fenceLength = 0;

            int line = from;
            while (line < to)
            {
                int end = TextUtil.LineEnd(m, line, to);
                int p = line, indent = 0;
                while (p < end && m[p] == ' ' && indent < 4) { p++; indent++; }

                int fence = indent < 4 ? FenceLength(m, p, end) : 0;
                if (fenced)
                {
                    if (end > line) emit(line, end, ClassificationNames.MdCode);
                    if (fence >= fenceLength && m[p] == fenceChar && OnlyBlanksAfter(m, p + fence, end)) fenced = false;
                }
                else if (fence > 0)
                {
                    fenced = true;
                    fenceChar = m[p];
                    fenceLength = fence;
                    emit(line, end, ClassificationNames.MdCode);
                }
                else if (indent < 4 && IsHeading(m, p, end))
                {
                    emit(p, end, ClassificationNames.MdHeading);
                }
                else
                {
                    int content = p;
                    int marker = ListMarkerEnd(m, p, end);
                    if (indent < 4 && marker > 0)
                    {
                        emit(p, marker, ClassificationNames.MdList);
                        content = marker;
                    }
                    Inline(m, content, end, emit);
                }

                line = end;
                while (line < to && (m[line] == '\n' || m[line] == '\r')) line++;
            }
        }

        private static int FenceLength(char[] m, int p, int end)
        {
            if (p >= end || (m[p] != '`' && m[p] != '~')) return 0;
            int n = 0;
            while (p + n < end && m[p + n] == m[p]) n++;
            return n >= 3 ? n : 0;
        }

        private static bool OnlyBlanksAfter(char[] m, int p, int end)
        {
            return TextUtil.SkipBlanks(m, p, end) == end;
        }

        private static bool IsHeading(char[] m, int p, int end)
        {
            int n = 0;
            while (p + n < end && m[p + n] == '#') n++;
            return n >= 1 && n <= 6 && (p + n == end || m[p + n] == ' ' || m[p + n] == '\t');
        }

        /// <summary>End of a list marker ("-", "*", "+", "1." or "1)") followed by a space, or 0.</summary>
        private static int ListMarkerEnd(char[] m, int p, int end)
        {
            if (p >= end) return 0;
            int q = p;
            if (m[p] == '-' || m[p] == '*' || m[p] == '+') q = p + 1;
            else if (char.IsDigit(m[p]))
            {
                while (q < end && char.IsDigit(m[q]) && q - p < 9) q++;
                if (q >= end || (m[q] != '.' && m[q] != ')')) return 0;
                q++;
            }
            else return 0;
            return q < end && (m[q] == ' ' || m[q] == '\t') ? q : 0;
        }

        private static void Inline(char[] m, int p, int end, TokenSink emit)
        {
            while (p < end)
            {
                char c = m[p];
                if (c == '\\') { p += 2; continue; }

                if (c == '`')
                {
                    int run = Run(m, p, end, '`');
                    int close = FindRun(m, p + run, end, '`', run);
                    if (close < 0) { p += run; continue; }
                    emit(p, close + run, ClassificationNames.MdCode);
                    p = close + run;
                }
                else if (c == '[' || (c == '!' && p + 1 < end && m[p + 1] == '['))
                {
                    int linkEnd = LinkEnd(m, c == '!' ? p + 1 : p, end);
                    if (linkEnd < 0) { p++; continue; }
                    emit(p, linkEnd, ClassificationNames.MdLink);
                    p = linkEnd;
                }
                else if (c == '<' && IsAutolink(m, p, end, out int autolinkEnd))
                {
                    emit(p, autolinkEnd, ClassificationNames.MdLink);
                    p = autolinkEnd;
                }
                else if (c == '*' || c == '_')
                {
                    int run = Run(m, p, end, c);
                    int after = p + run;
                    bool canOpen = after < end && !char.IsWhiteSpace(m[after]) && !(c == '_' && IsInWord(m, p));
                    int close = canOpen ? FindEmphasisClose(m, after, end, c, run) : -1;
                    if (close < 0) { p = after; continue; }
                    emit(p, close + run, run == 1 ? ClassificationNames.MdEmphasis : ClassificationNames.MdStrong);
                    p = close + run;
                }
                else
                {
                    p++;
                }
            }
        }

        private static bool IsInWord(char[] m, int p)
        {
            return p > 0 && TextUtil.IsWordChar(m[p - 1]);
        }

        private static int Run(char[] m, int p, int end, char c)
        {
            int n = 0;
            while (p + n < end && m[p + n] == c) n++;
            return n;
        }

        /// <summary>Start of the next run of exactly <paramref name="length"/> times <paramref name="c"/>, or -1.</summary>
        private static int FindRun(char[] m, int p, int end, char c, int length)
        {
            while (p < end)
            {
                if (m[p] == c)
                {
                    int run = Run(m, p, end, c);
                    if (run == length) return p;
                    p += run;
                }
                else p++;
            }
            return -1;
        }

        private static int FindEmphasisClose(char[] m, int p, int end, char c, int length)
        {
            while (p < end)
            {
                if (m[p] == '\\') { p += 2; continue; }
                if (m[p] == '`')
                {
                    int run = Run(m, p, end, '`');
                    int close = FindRun(m, p + run, end, '`', run);
                    p = close < 0 ? p + run : close + run;
                    continue;
                }
                if (m[p] == c)
                {
                    int run = Run(m, p, end, c);
                    bool closes = run >= length && !char.IsWhiteSpace(m[p - 1]) &&
                                  !(c == '_' && p + run < end && TextUtil.IsWordChar(m[p + run]));
                    if (closes) return p;
                    p += run;
                }
                else p++;
            }
            return -1;
        }

        /// <summary>End of "[text](url)", or -1. <paramref name="p"/> is at the "[".</summary>
        private static int LinkEnd(char[] m, int p, int end)
        {
            int depth = 0, q = p;
            for (; q < end; q++)
            {
                if (m[q] == '\\') { q++; continue; }
                if (m[q] == '[') depth++;
                else if (m[q] == ']' && --depth == 0) break;
            }
            if (q >= end - 1 || m[q] != ']' || m[q + 1] != '(') return -1;
            int close = q + 2, parens = 1;
            for (; close < end; close++)
            {
                if (m[close] == '(') parens++;
                else if (m[close] == ')' && --parens == 0) return close + 1;
            }
            return -1;
        }

        private static bool IsAutolink(char[] m, int p, int end, out int linkEnd)
        {
            linkEnd = -1;
            int q = p + 1;
            while (q < end && (char.IsLetterOrDigit(m[q]) || m[q] == '+' || m[q] == '.' || m[q] == '-')) q++;
            if (q == p + 1 || q >= end || m[q] != ':') return false;
            while (q < end && m[q] != '>' && !char.IsWhiteSpace(m[q]) && m[q] != '<') q++;
            if (q >= end || m[q] != '>') return false;
            linkEnd = q + 1;
            return true;
        }
    }
}
