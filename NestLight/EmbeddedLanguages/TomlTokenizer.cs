using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.EmbeddedLanguages
{
    /// <summary>TOML: tables, keys, strings, numbers, booleans, dates and comments.</summary>
    internal sealed class TomlTokenizer : IEmbeddedLanguageTokenizer
    {
        private static readonly string[] TomlIds = { "toml" };

        public IReadOnlyList<string> Ids { get { return TomlIds; } }

        public void Tokenize(char[] m, int from, int to, TokenSink emit)
        {
            int line = from;
            while (line < to)
            {
                int end = TextUtil.LineEnd(m, line, to);
                int p = TextUtil.SkipBlanks(m, line, end);
                int next = end;
                while (next < to && (m[next] == '\n' || m[next] == '\r')) next++;

                if (p == end) { line = next; continue; }

                char c = m[p];

                if (c == '#')
                {
                    emit(p, end, ClassificationNames.TomlComment);
                    line = next;
                    continue;
                }

                if (c == '[')
                {
                    int close = p;
                    bool array = p + 1 < end && m[p + 1] == '[';
                    while (close < end && m[close] != ']') close++;
                    if (close < end) close++;
                    if (array && close < end && m[close] == ']') close++;
                    emit(p, close, ClassificationNames.TomlTable);
                    p = TextUtil.SkipBlanks(m, close, end);
                    if (p < end && m[p] == '#') emit(p, end, ClassificationNames.TomlComment);
                    line = next;
                    continue;
                }

                int keyEnd = Key(m, p, end);
                if (keyEnd > p)
                {
                    emit(p, keyEnd, ClassificationNames.TomlKey);
                    p = TextUtil.SkipBlanks(m, keyEnd, end);
                    if (p < end && m[p] == '=')
                    {
                        emit(p, p + 1, ClassificationNames.TomlPunct);
                        p = TextUtil.SkipBlanks(m, p + 1, end);
                    }
                }

                ParseValue(m, p, end, emit);
                line = next;
            }
        }

        private static void ParseValue(char[] m, int p, int end, TokenSink emit)
        {
            while (p < end)
            {
                p = TextUtil.SkipBlanks(m, p, end);
                if (p >= end) return;
                char c = m[p];

                if (c == '#')
                {
                    emit(p, end, ClassificationNames.TomlComment);
                    return;
                }

                if (c == '"' || c == '\'')
                {
                    bool triple = p + 2 < end && m[p + 1] == c && m[p + 2] == c;
                    int close = triple ? TripleStringEnd(m, p, end, c) : SingleStringEnd(m, p, end, c);
                    emit(p, close, ClassificationNames.TomlString);
                    p = close;
                    continue;
                }

                if (c == 't' && TextUtil.StartsWith(m, p, end, "true"))
                {
                    emit(p, p + 4, ClassificationNames.TomlLiteral);
                    p += 4;
                    continue;
                }
                if (c == 'f' && TextUtil.StartsWith(m, p, end, "false"))
                {
                    emit(p, p + 5, ClassificationNames.TomlLiteral);
                    p += 5;
                    continue;
                }

                if (c == '[' || c == '{')
                {
                    emit(p, p + 1, ClassificationNames.TomlPunct);
                    p++;
                    continue;
                }
                if (c == ']' || c == '}')
                {
                    emit(p, p + 1, ClassificationNames.TomlPunct);
                    p++;
                    continue;
                }
                if (c == ',')
                {
                    emit(p, p + 1, ClassificationNames.TomlPunct);
                    p++;
                    continue;
                }

                if (IsDateTimeStart(m, p, end))
                {
                    int dtEnd = DateTimeEnd(m, p, end);
                    emit(p, dtEnd, ClassificationNames.TomlDate);
                    p = dtEnd;
                    continue;
                }

                if (char.IsDigit(c) || ((c == '+' || c == '-') && p + 1 < end && (char.IsDigit(m[p + 1]) || m[p + 1] == 'i' || m[p + 1] == 'n')))
                {
                    int numEnd = NumberEnd(m, p, end);
                    emit(p, numEnd, ClassificationNames.TomlNumber);
                    p = numEnd;
                    continue;
                }

                if (TextUtil.StartsWith(m, p, end, "inf") || TextUtil.StartsWith(m, p, end, "nan"))
                {
                    emit(p, p + 3, ClassificationNames.TomlNumber);
                    p += 3;
                    continue;
                }

                if (TextUtil.IsWordStart(c))
                {
                    int wEnd = p + 1;
                    while (wEnd < end && (TextUtil.IsWordChar(m[wEnd]) || m[wEnd] == '-')) wEnd++;
                    p = wEnd;
                    continue;
                }

                p++;
            }
        }

        private static int Key(char[] m, int p, int end)
        {
            int start = p;
            while (p < end)
            {
                char c = m[p];
                if (c == '"' || c == '\'')
                {
                    p = SingleStringEnd(m, p, end, c);
                }
                else if (char.IsLetterOrDigit(c) || c == '_' || c == '-')
                {
                    p++;
                }
                else if (c == '.')
                {
                    p++;
                }
                else
                {
                    break;
                }
            }
            int keyEnd = p;
            while (keyEnd > start && (m[keyEnd - 1] == ' ' || m[keyEnd - 1] == '\t' || m[keyEnd - 1] == '.')) keyEnd--;
            return keyEnd > start && p < end && (TextUtil.SkipBlanks(m, p, end) < end && m[TextUtil.SkipBlanks(m, p, end)] == '=') ? keyEnd : start;
        }

        private static int SingleStringEnd(char[] m, int p, int end, char quote)
        {
            int q = p + 1;
            while (q < end)
            {
                if (quote == '"' && m[q] == '\\') { q += 2; continue; }
                if (m[q] == quote) return q + 1;
                if (m[q] == '\n') return q;
                q++;
            }
            return end;
        }

        private static int TripleStringEnd(char[] m, int p, int end, char quote)
        {
            int q = p + 3;
            while (q + 2 < end)
            {
                if (quote == '"' && m[q] == '\\') { q += 2; continue; }
                if (m[q] == quote && m[q + 1] == quote && m[q + 2] == quote) return q + 3;
                q++;
            }
            return end;
        }

        private static bool IsDateTimeStart(char[] m, int p, int end)
        {
            if (p + 10 > end) return false;
            return char.IsDigit(m[p]) && char.IsDigit(m[p + 1]) && char.IsDigit(m[p + 2]) && char.IsDigit(m[p + 3])
                   && m[p + 4] == '-' && char.IsDigit(m[p + 5]) && char.IsDigit(m[p + 6])
                   && m[p + 7] == '-' && char.IsDigit(m[p + 8]) && char.IsDigit(m[p + 9]);
        }

        private static int DateTimeEnd(char[] m, int p, int end)
        {
            int q = p;
            while (q < end && (char.IsDigit(m[q]) || m[q] == '-' || m[q] == 'T' || m[q] == 't'
                   || m[q] == ':' || m[q] == '.' || m[q] == '+' || m[q] == 'Z' || m[q] == 'z')) q++;
            return q;
        }

        private static int NumberEnd(char[] m, int p, int end)
        {
            int q = p;
            if (m[q] == '+' || m[q] == '-') q++;
            if (q < end && m[q] == '0' && q + 1 < end)
            {
                char prefix = m[q + 1];
                if (prefix == 'x' || prefix == 'X')
                {
                    q += 2;
                    while (q < end && (Uri.IsHexDigit(m[q]) || m[q] == '_')) q++;
                    return q;
                }
                if (prefix == 'o' || prefix == 'O')
                {
                    q += 2;
                    while (q < end && ((m[q] >= '0' && m[q] <= '7') || m[q] == '_')) q++;
                    return q;
                }
                if (prefix == 'b' || prefix == 'B')
                {
                    q += 2;
                    while (q < end && (m[q] == '0' || m[q] == '1' || m[q] == '_')) q++;
                    return q;
                }
            }
            if (q < end && TextUtil.StartsWith(m, q, end, "inf")) return q + 3;
            if (q < end && TextUtil.StartsWith(m, q, end, "nan")) return q + 3;
            while (q < end && (char.IsDigit(m[q]) || m[q] == '_')) q++;
            if (q < end && m[q] == '.')
            {
                q++;
                while (q < end && (char.IsDigit(m[q]) || m[q] == '_')) q++;
            }
            if (q < end && (m[q] == 'e' || m[q] == 'E'))
            {
                q++;
                if (q < end && (m[q] == '+' || m[q] == '-')) q++;
                while (q < end && (char.IsDigit(m[q]) || m[q] == '_')) q++;
            }
            return q;
        }
    }
}
