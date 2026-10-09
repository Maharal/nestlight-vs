using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Languages
{
    /// <summary>JSON: keys, strings, numbers, literals (<c>true</c>, <c>false</c>, <c>null</c>) and punctuation.</summary>
    internal sealed class JsonTokenizer : IEmbeddedLanguageTokenizer
    {
        private static readonly string[] JsonIds = { "json" };

        public IReadOnlyList<string> Ids { get { return JsonIds; } }

        public void Tokenize(char[] m, int from, int to, TokenSink emit)
        {
            int i = from;
            while (i < to)
            {
                char c = m[i];
                if (c == '"')
                {
                    int end = StringEnd(m, i, to);
                    int next = end;
                    while (next < to && char.IsWhiteSpace(m[next])) next++;
                    bool key = next < to && m[next] == ':';
                    emit(i, end, key ? ClassificationNames.JsonKey : ClassificationNames.JsonString);
                    i = end;
                }
                else if (c == '-' || char.IsDigit(c))
                {
                    int end = Number(m, i, to);
                    if (end > i + (c == '-' ? 1 : 0)) emit(i, end, ClassificationNames.JsonNumber);
                    i = end > i ? end : i + 1;
                }
                else if (TextUtil.IsWordStart(c))
                {
                    int end = i + 1;
                    while (end < to && TextUtil.IsWordChar(m[end])) end++;
                    string word = TextUtil.Substring(m, i, end);
                    if (word == "true" || word == "false" || word == "null") emit(i, end, ClassificationNames.JsonLiteral);
                    i = end;
                }
                else if ("{}[],:".IndexOf(c) >= 0)
                {
                    emit(i, i + 1, ClassificationNames.JsonPunct);
                    i++;
                }
                else
                {
                    i++;
                }
            }
        }

        private static int StringEnd(char[] m, int i, int to)
        {
            int p = i + 1;
            while (p < to)
            {
                char c = m[p];
                if (c == '\\') p += 2;
                else if (c == '"') return p + 1;
                else if (c == '\n') return p; // unclosed string
                else p++;
            }
            return to;
        }

        private static int Number(char[] m, int i, int to)
        {
            int p = i;
            if (m[p] == '-') p++;
            while (p < to && char.IsDigit(m[p])) p++;
            if (p < to && m[p] == '.')
            {
                p++;
                while (p < to && char.IsDigit(m[p])) p++;
            }
            if (p < to && (m[p] == 'e' || m[p] == 'E'))
            {
                int q = p + 1;
                if (q < to && (m[q] == '+' || m[q] == '-')) q++;
                if (q < to && char.IsDigit(m[q]))
                {
                    while (q < to && char.IsDigit(m[q])) q++;
                    p = q;
                }
            }
            return p;
        }
    }
}
