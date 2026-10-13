using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.EmbeddedLanguages
{
    /// <summary>
    /// Markdown: headings, emphasis and strong text, code spans and fenced blocks, links, math, and list markers.
    /// Block rules look at one line at a time. Inline rules look at a paragraph, so they cross line breaks (a paragraph runs until a
    /// blank line or until another block starts). The indentation that every line shares is the indentation of the code around the
    /// string, not of the Markdown, so it is taken off before the rules look at it.
    /// </summary>
    internal sealed class MarkdownTokenizer : IEmbeddedLanguageTokenizer
    {
        private static readonly string[] MarkdownIds = { "markdown", "md" };

        /// <summary>How far an inline rule looks for its closing delimiter: an unmatched opener must not cost the whole paragraph.</summary>
        private const int MaxSpan = 2000;

        public IReadOnlyList<string> Ids { get { return MarkdownIds; } }

        public void Tokenize(char[] m, int from, int to, TokenSink emit)
        {
            int baseColumns = CommonIndent(m, from, to);
            bool fenced = false, math = false;
            char fenceChar = '`';
            int fenceLength = 0;
            int paragraphStart = -1, paragraphEnd = -1;

            int line = from;
            while (line < to)
            {
                int end = TextUtil.LineEnd(m, line, to);
                int p = line;
                int columns = Leading(m, ref p, end);
                int indent = Math.Max(0, columns - baseColumns);
                bool blank = p >= end;
                bool code = indent >= 4; // four columns past the indentation of the string: not a block rule

                int fence = !code && !blank ? FenceLength(m, p, end) : 0;
                if (fenced)
                {
                    if (end > line) emit(line, end, ClassificationNames.MdCode);
                    if (fence >= fenceLength && m[p] == fenceChar && OnlyBlanksAfter(m, p + fence, end)) fenced = false;
                }
                else if (math)
                {
                    if (!blank) emit(p, end, ClassificationNames.MdMath);
                    if (EndsWithMathDelimiter(m, p, end)) math = false;
                }
                else if (blank)
                {
                    Flush(m, ref paragraphStart, paragraphEnd, emit);
                }
                else if (fence > 0)
                {
                    Flush(m, ref paragraphStart, paragraphEnd, emit);
                    fenced = true;
                    fenceChar = m[p];
                    fenceLength = fence;
                    emit(line, end, ClassificationNames.MdCode);
                }
                else if (!code && IsMathFence(m, p, end))
                {
                    Flush(m, ref paragraphStart, paragraphEnd, emit);
                    math = true;
                    emit(p, end, ClassificationNames.MdMath);
                }
                else if (!code && IsHeading(m, p, end))
                {
                    Flush(m, ref paragraphStart, paragraphEnd, emit);
                    emit(p, end, ClassificationNames.MdHeading);
                }
                else
                {
                    int marker = code ? 0 : ListMarkerEnd(m, p, end);
                    if (marker > 0)
                    {
                        Flush(m, ref paragraphStart, paragraphEnd, emit);
                        emit(p, marker, ClassificationNames.MdList);
                        paragraphStart = marker;
                    }
                    else if (paragraphStart < 0) paragraphStart = p;
                    paragraphEnd = end;
                }

                line = NextLine(m, end, to);
            }
            Flush(m, ref paragraphStart, paragraphEnd, emit);
        }

        /// <summary>The inline markup of the paragraph [start, end), which may be several lines long.</summary>
        private static void Flush(char[] m, ref int start, int end, TokenSink emit)
        {
            if (start >= 0) Inline(m, start, end, emit);
            start = -1;
        }

        /// <summary>
        /// The columns of indentation that every line has, apart from the first one (it starts where the string does, after the quote or
        /// the backtick, so its column says nothing about the layout of the code around). A tab counts to the next multiple of four.
        /// </summary>
        private static int CommonIndent(char[] m, int from, int to)
        {
            int common = int.MaxValue;
            int line = NextLine(m, TextUtil.LineEnd(m, from, to), to);
            while (line < to)
            {
                int end = TextUtil.LineEnd(m, line, to);
                int p = line;
                int columns = Leading(m, ref p, end);
                if (p < end && columns < common) common = columns;
                line = NextLine(m, end, to);
            }
            return common == int.MaxValue ? 0 : common;
        }

        /// <summary>The start of the next line: after one line break (<c>\n</c>, <c>\r</c> or <c>\r\n</c>), so that a blank line is a line.</summary>
        private static int NextLine(char[] m, int end, int to)
        {
            if (end >= to) return to;
            if (m[end] == '\r' && end + 1 < to && m[end + 1] == '\n') return end + 2;
            return end + 1;
        }

        /// <summary>Skips the blanks at the start of the line and returns how many columns they take.</summary>
        private static int Leading(char[] m, ref int p, int end)
        {
            int columns = 0;
            while (p < end && (m[p] == ' ' || m[p] == '\t'))
            {
                columns += m[p] == '\t' ? 4 - columns % 4 : 1;
                p++;
            }
            return columns;
        }

        /// <summary>A line that is only <c>$$</c>: it opens (and, further down, closes) a block of math.</summary>
        private static bool IsMathFence(char[] m, int p, int end)
        {
            return p + 2 <= end && m[p] == '$' && m[p + 1] == '$' && OnlyBlanksAfter(m, p + 2, end);
        }

        /// <summary>A line of a math block that ends with <c>$$</c>.</summary>
        private static bool EndsWithMathDelimiter(char[] m, int p, int end)
        {
            int q = end;
            while (q > p && (m[q - 1] == ' ' || m[q - 1] == '\t')) q--;
            return q - 2 >= p && m[q - 1] == '$' && m[q - 2] == '$';
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
                    int close = FindRun(m, p + run, Math.Min(end, p + MaxSpan), '`', run);
                    if (close < 0) { p += run; continue; }
                    emit(p, close + run, ClassificationNames.MdCode);
                    p = close + run;
                }
                else if (c == '$')
                {
                    int run = Run(m, p, end, '$');
                    int close = run <= 2 ? MathClose(m, p, run, Math.Min(end, p + MaxSpan)) : -1;
                    if (close < 0) { p += run; continue; }
                    emit(p, close + run, ClassificationNames.MdMath);
                    p = close + run;
                }
                else if (c == '[' || (c == '!' && p + 1 < end && m[p + 1] == '['))
                {
                    int linkEnd = LinkEnd(m, c == '!' ? p + 1 : p, Math.Min(end, p + MaxSpan));
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
                    int close = canOpen ? FindEmphasisClose(m, after, Math.Min(end, p + MaxSpan), c, run) : -1;
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

        /// <summary>
        /// Where the math that opens with <paramref name="run"/> dollar signs at <paramref name="p"/> closes, or -1. <c>$$</c> closes with
        /// <c>$$</c>. A single <c>$</c> opens when a non-blank follows it and closes at a <c>$</c> that has a non-blank before it and no digit
        /// after it, so prices (<c>$5 and $6</c>) are not math.
        /// </summary>
        private static int MathClose(char[] m, int p, int run, int end)
        {
            int content = p + run;
            if (content >= end) return -1;
            if (run == 2) return content < end && m[content] != '$' ? FindRun(m, content, end, '$', 2) : -1;

            if (char.IsWhiteSpace(m[content]) || m[content] == '$') return -1;
            for (int q = content; q < end; q++)
            {
                if (m[q] == '\\') { q++; continue; }
                if (m[q] != '$') continue;
                int length = Run(m, q, end, '$');
                if (length == 1 && q > content && !char.IsWhiteSpace(m[q - 1]) && !(q + 1 < end && char.IsDigit(m[q + 1]))) return q;
                q += length - 1;
            }
            return -1;
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
