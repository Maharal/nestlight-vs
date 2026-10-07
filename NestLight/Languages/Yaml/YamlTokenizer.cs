using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Languages
{
    /// <summary>
    /// YAML: keys, scalars, anchors / aliases / tags and comments. Works one line at a time, remembering
    /// only the block scalars (<c>|</c> and <c>&gt;</c>) whose lines are indented under their key.
    /// </summary>
    internal sealed class YamlTokenizer : ILanguageTokenizer
    {
        private static readonly string[] YamlIds = { "yaml", "yml" };

        private static readonly HashSet<string> Literals = new HashSet<string>
        {
            "true", "false", "null", "yes", "no", "on", "off", "~",
            "True", "False", "Null", "Yes", "No", "On", "Off", "TRUE", "FALSE", "NULL"
        };

        public IReadOnlyList<string> Ids { get { return YamlIds; } }

        public void Tokenize(char[] m, int from, int to, TokenSink emit)
        {
            int blockParent = -1; // indent of the line that opened a block scalar; -1 when none is open

            int line = from;
            while (line < to)
            {
                int end = TextUtil.LineEnd(m, line, to);
                int p = TextUtil.SkipBlanks(m, line, end);
                int indent = p - line;
                int next = end;
                while (next < to && (m[next] == '\n' || m[next] == '\r')) next++;

                if (p == end) { line = next; continue; } // blank line

                if (blockParent >= 0)
                {
                    if (indent > blockParent)
                    {
                        emit(p, TrimEnd(m, p, end), ClassificationNames.YamlString);
                        line = next;
                        continue;
                    }
                    blockParent = -1;
                }

                if (m[p] == '#')
                {
                    emit(p, end, ClassificationNames.YamlComment);
                }
                else if (indent == 0 && (TextUtil.StartsWith(m, p, end, "---") || TextUtil.StartsWith(m, p, end, "...")) &&
                         (p + 3 == end || m[p + 3] == ' ' || m[p + 3] == '\t'))
                {
                    emit(p, p + 3, ClassificationNames.YamlPunct);
                    p = TextUtil.SkipBlanks(m, p + 3, end);
                    if (p < end && blockParent < 0) Value(m, p, end, indent, ref blockParent, emit);
                }
                else
                {
                    // sequence entries: "- - item"
                    while (p < end && m[p] == '-' && (p + 1 == end || m[p + 1] == ' ' || m[p + 1] == '\t'))
                    {
                        emit(p, p + 1, ClassificationNames.YamlPunct);
                        p = TextUtil.SkipBlanks(m, p + 1, end);
                    }
                    if (p < end) Value(m, p, end, indent, ref blockParent, emit);
                }
                line = next;
            }
        }

        /// <summary>A key with its value, or a value alone, between p and the end of the line.</summary>
        private static void Value(char[] m, int p, int end, int lineIndent, ref int blockParent, TokenSink emit)
        {
            while (p < end)
            {
                p = TextUtil.SkipBlanks(m, p, end);
                if (p >= end) return;
                char c = m[p];

                if (c == '#' && (p == 0 || m[p - 1] == ' ' || m[p - 1] == '\t' || m[p - 1] == '\n'))
                {
                    emit(p, end, ClassificationNames.YamlComment);
                    return;
                }
                if (c == '&' || c == '*' || c == '!')
                {
                    int q = p + 1;
                    while (q < end && !char.IsWhiteSpace(m[q]) && m[q] != ',' && m[q] != ']' && m[q] != '}') q++;
                    emit(p, q, ClassificationNames.YamlAnchor);
                    p = q;
                    continue;
                }
                if (c == '[' || c == '{')
                {
                    Flow(m, p, end, emit);
                    return;
                }
                if ((c == '|' || c == '>') && IsBlockHeader(m, p, end))
                {
                    emit(p, TrimEnd(m, p, end), ClassificationNames.YamlPunct);
                    blockParent = lineIndent;
                    return;
                }
                if (c == '?' && (p + 1 == end || m[p + 1] == ' '))
                {
                    emit(p, p + 1, ClassificationNames.YamlPunct);
                    p++;
                    continue;
                }

                int stop;
                bool isKey;
                if (c == '"' || c == '\'')
                {
                    int close = QuotedEnd(m, p, end);
                    int after = TextUtil.SkipBlanks(m, close, end);
                    isKey = after < end && m[after] == ':' && (after + 1 == end || m[after + 1] == ' ' || m[after + 1] == '\t');
                    emit(p, close, isKey ? ClassificationNames.YamlKey : ClassificationNames.YamlString);
                    p = isKey ? after : close;
                    stop = p;
                }
                else
                {
                    stop = PlainEnd(m, p, end, out isKey);
                    int textEnd = TrimEnd(m, p, stop);
                    if (textEnd > p)
                    {
                        if (isKey) emit(p, textEnd, ClassificationNames.YamlKey);
                        else emit(p, textEnd, IsScalar(m, p, textEnd) ? ClassificationNames.YamlScalar : ClassificationNames.YamlString);
                    }
                    p = stop;
                }

                if (isKey && p < end && m[p] == ':')
                {
                    emit(p, p + 1, ClassificationNames.YamlPunct);
                    p++;
                    continue; // the value follows
                }
                if (!isKey) { p = Math.Max(p, stop); }
            }
        }

        /// <summary>Flow collections: [a, b] and {key: value}.</summary>
        private static void Flow(char[] m, int p, int end, TokenSink emit)
        {
            while (p < end)
            {
                char c = m[p];
                if (c == ' ' || c == '\t') { p++; continue; }
                if (c == '#' && p > 0 && (m[p - 1] == ' ' || m[p - 1] == '\t'))
                {
                    emit(p, end, ClassificationNames.YamlComment);
                    return;
                }
                if ("[]{},".IndexOf(c) >= 0)
                {
                    emit(p, p + 1, ClassificationNames.YamlPunct);
                    p++;
                    continue;
                }
                if (c == ':' && (p + 1 == end || " \t,]}".IndexOf(m[p + 1]) >= 0))
                {
                    emit(p, p + 1, ClassificationNames.YamlPunct);
                    p++;
                    continue;
                }
                if (c == '&' || c == '*' || c == '!')
                {
                    int q = p + 1;
                    while (q < end && !char.IsWhiteSpace(m[q]) && ",]}".IndexOf(m[q]) < 0) q++;
                    emit(p, q, ClassificationNames.YamlAnchor);
                    p = q;
                    continue;
                }

                int stop, textEnd;
                if (c == '"' || c == '\'')
                {
                    stop = textEnd = QuotedEnd(m, p, end);
                }
                else
                {
                    stop = p;
                    while (stop < end && ",]}".IndexOf(m[stop]) < 0 &&
                           !(m[stop] == ':' && (stop + 1 == end || " \t,]}".IndexOf(m[stop + 1]) >= 0)) &&
                           !(m[stop] == '#' && stop > p && (m[stop - 1] == ' ' || m[stop - 1] == '\t')))
                        stop++;
                    textEnd = TrimEnd(m, p, stop);
                }
                if (textEnd <= p) { p = Math.Max(stop, p + 1); continue; }

                int after = TextUtil.SkipBlanks(m, stop, end);
                bool isKey = after < end && m[after] == ':' && (after + 1 == end || " \t,]}".IndexOf(m[after + 1]) >= 0);
                string type = isKey ? ClassificationNames.YamlKey
                            : c == '"' || c == '\'' ? ClassificationNames.YamlString
                            : IsScalar(m, p, textEnd) ? ClassificationNames.YamlScalar : ClassificationNames.YamlString;
                emit(p, textEnd, type);
                p = stop;
            }
        }

        /// <summary>End of a plain scalar, and whether a ": " ends it (so it is a key).</summary>
        private static int PlainEnd(char[] m, int p, int end, out bool isKey)
        {
            isKey = false;
            for (int q = p; q < end; q++)
            {
                if (m[q] == ':' && (q + 1 == end || m[q + 1] == ' ' || m[q + 1] == '\t'))
                {
                    isKey = true;
                    return q;
                }
                if (m[q] == '#' && q > p && (m[q - 1] == ' ' || m[q - 1] == '\t')) return q;
            }
            return end;
        }

        private static int QuotedEnd(char[] m, int p, int end)
        {
            char quote = m[p];
            int q = p + 1;
            while (q < end)
            {
                if (quote == '"' && m[q] == '\\') { q += 2; continue; }
                if (m[q] == quote)
                {
                    if (quote == '\'' && q + 1 < end && m[q + 1] == '\'') { q += 2; continue; } // '' is an escaped quote
                    return q + 1;
                }
                q++;
            }
            return end;
        }

        private static bool IsBlockHeader(char[] m, int p, int end)
        {
            int q = p + 1;
            while (q < end && (m[q] == '+' || m[q] == '-' || char.IsDigit(m[q]))) q++;
            q = TextUtil.SkipBlanks(m, q, end);
            return q == end || m[q] == '#';
        }

        private static int TrimEnd(char[] m, int start, int end)
        {
            while (end > start && (m[end - 1] == ' ' || m[end - 1] == '\t')) end--;
            return end;
        }

        private static bool IsScalar(char[] m, int start, int end)
        {
            string text = TextUtil.Substring(m, start, end);
            if (Literals.Contains(text)) return true;
            double ignored;
            return text.Length > 0 && (char.IsDigit(text[0]) || text[0] == '-' || text[0] == '+' || text[0] == '.') &&
                   (double.TryParse(text, System.Globalization.NumberStyles.Float,
                        System.Globalization.CultureInfo.InvariantCulture, out ignored) ||
                    text.StartsWith("0x", StringComparison.OrdinalIgnoreCase) ||
                    text == ".inf" || text == "-.inf" || text == ".nan");
        }
    }
}
