using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Languages
{
    /// <summary>Regular expressions: groups, character classes, quantifiers, escapes and anchors.</summary>
    internal sealed class RegexTokenizer : ILanguageTokenizer
    {
        private static readonly string[] RegexIds = { "regex", "regexp" };

        public IReadOnlyList<string> Ids { get { return RegexIds; } }

        public void Tokenize(char[] m, int from, int to, TokenSink emit)
        {
            int i = from;
            while (i < to)
            {
                char c = m[i];
                switch (c)
                {
                    case '\\':
                        i = Escape(m, i, to, emit);
                        break;
                    case '[':
                        i = CharacterClass(m, i, to, emit);
                        break;
                    case '(':
                        i = GroupOpening(m, i, to, emit);
                        break;
                    case ')':
                    case '|':
                        emit(i, i + 1, ClassificationNames.RegexGroup);
                        i++;
                        break;
                    case '*':
                    case '+':
                    case '?':
                        i = Quantifier(m, i, i + 1, to, emit);
                        break;
                    case '{':
                        {
                            int close = RepeatEnd(m, i, to);
                            if (close < 0) i++;
                            else i = Quantifier(m, i, close, to, emit);
                            break;
                        }
                    case '^':
                    case '$':
                        emit(i, i + 1, ClassificationNames.RegexAnchor);
                        i++;
                        break;
                    case '.':
                        emit(i, i + 1, ClassificationNames.RegexClass);
                        i++;
                        break;
                    default:
                        i++;
                        break;
                }
            }
        }

        private static int Escape(char[] m, int i, int to, TokenSink emit)
        {
            if (i + 1 >= to) { emit(i, i + 1, ClassificationNames.RegexEscape); return i + 1; }
            char n = m[i + 1];
            int end = i + 2;

            if ("bBAZzG".IndexOf(n) >= 0)
            {
                emit(i, end, ClassificationNames.RegexAnchor);
                return end;
            }
            if (n == 'x') end = Hex(m, end, to, 2);
            else if (n == 'u') end = Hex(m, end, to, 4);
            else if (n == 'p' || n == 'P' || n == 'k')
            {
                if (end < to && (m[end] == '{' || m[end] == '<'))
                {
                    char close = m[end] == '{' ? '}' : '>';
                    int e = end + 1;
                    while (e < to && m[e] != close && m[e] != '\n') e++;
                    if (e < to && m[e] == close) end = e + 1;
                }
            }
            else if (char.IsDigit(n) && n != '0')
            {
                while (end < to && char.IsDigit(m[end])) end++;
            }
            emit(i, end, ClassificationNames.RegexEscape);
            return end;
        }

        private static int Hex(char[] m, int p, int to, int max)
        {
            int n = 0;
            while (p < to && n < max && System.Uri.IsHexDigit(m[p])) { p++; n++; }
            return p;
        }

        private static int CharacterClass(char[] m, int i, int to, TokenSink emit)
        {
            int p = i + 1;
            if (p < to && m[p] == '^') p++;
            if (p < to && m[p] == ']') p++; // a leading ] is a literal
            while (p < to && m[p] != ']')
            {
                if (m[p] == '\\') p++;
                else if (m[p] == '[' && p + 1 < to && m[p + 1] == ':')
                {
                    int e = TextUtil.IndexOf(m, ":]", p + 2, to);
                    if (e >= 0) p = e + 1;
                }
                p++;
            }
            int end = p < to ? p + 1 : to;
            emit(i, end, ClassificationNames.RegexClass);
            return end;
        }

        private static int GroupOpening(char[] m, int i, int to, TokenSink emit)
        {
            int p = i + 1;
            if (p < to && m[p] == '?')
            {
                p++;
                if (p < to && (m[p] == ':' || m[p] == '=' || m[p] == '!' || m[p] == '>' || m[p] == '|')) p++;
                else if (p < to && m[p] == '<' && p + 1 < to && (m[p + 1] == '=' || m[p + 1] == '!')) p += 2;
                else if (p < to && (m[p] == '<' || m[p] == '\'' || m[p] == 'P'))
                {
                    if (m[p] == 'P') p++;
                    if (p < to && (m[p] == '<' || m[p] == '\''))
                    {
                        char close = m[p] == '<' ? '>' : '\'';
                        int e = p + 1;
                        while (e < to && m[e] != close && m[e] != '\n') e++;
                        p = e < to && m[e] == close ? e + 1 : e;
                    }
                }
                else
                {
                    // inline flags: (?i) (?i:...) (?-i)
                    while (p < to && (char.IsLetter(m[p]) || m[p] == '-')) p++;
                    if (p < to && (m[p] == ':' || m[p] == ')')) p++;
                }
            }
            emit(i, p, ClassificationNames.RegexGroup);
            return p;
        }

        /// <summary>End of "{n}", "{n,}", "{n,m}" or "{,m}", or -1 when the brace is a literal.</summary>
        private static int RepeatEnd(char[] m, int i, int to)
        {
            int p = i + 1, digits = 0;
            while (p < to && char.IsDigit(m[p])) { p++; digits++; }
            if (p < to && m[p] == ',')
            {
                p++;
                while (p < to && char.IsDigit(m[p])) { p++; digits++; }
            }
            return p < to && m[p] == '}' && digits > 0 ? p + 1 : -1;
        }

        private static int Quantifier(char[] m, int start, int end, int to, TokenSink emit)
        {
            if (end < to && (m[end] == '?' || m[end] == '+')) end++; // lazy or possessive
            emit(start, end, ClassificationNames.RegexQuantifier);
            return end;
        }
    }
}
