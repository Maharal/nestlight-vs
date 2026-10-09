using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Languages
{
    /// <summary>XML: tags, attributes, comments, CDATA sections, processing instructions and DOCTYPE.</summary>
    internal sealed class XmlTokenizer : IEmbeddedLanguageTokenizer
    {
        private const char Mask = TextUtil.Mask;
        private static readonly string[] XmlIds = { "xml" };

        public IReadOnlyList<string> Ids { get { return XmlIds; } }

        public void Tokenize(char[] m, int from, int to, TokenSink emit)
        {
            int i = from;
            while (i < to)
            {
                if (m[i] != '<') { i++; continue; }

                if (TextUtil.StartsWith(m, i, to, "<!--"))
                {
                    int e = TextUtil.IndexOf(m, "-->", i + 4, to);
                    int end = e < 0 ? to : e + 3;
                    emit(i, end, ClassificationNames.XmlComment);
                    i = end;
                    continue;
                }
                if (TextUtil.StartsWith(m, i, to, "<![CDATA["))
                {
                    int e = TextUtil.IndexOf(m, "]]>", i + 9, to);
                    int end = e < 0 ? to : e + 3;
                    emit(i, end, ClassificationNames.XmlCData);
                    i = end;
                    continue;
                }

                int p = i + 1;
                bool instruction = false;
                if (p < to && (m[p] == '/' || m[p] == '?' || m[p] == '!'))
                {
                    instruction = m[p] == '?';
                    p++;
                }
                if (p >= to || !(char.IsLetter(m[p]) || m[p] == '_' || m[p] == Mask)) { i++; continue; }

                emit(i, p, ClassificationNames.XmlDelimiter);   // "<", "</", "<?" or "<!"
                int nameStart = p;
                while (p < to && IsNameChar(m[p])) p++;
                emit(nameStart, p, ClassificationNames.XmlTag);
                i = TagBody(m, p, to, instruction, emit);
            }
        }

        /// <summary>Attributes up to the end of the tag. Returns where scanning resumes.</summary>
        private static int TagBody(char[] m, int p, int to, bool instruction, TokenSink emit)
        {
            while (p < to)
            {
                char c = m[p];
                if (char.IsWhiteSpace(c)) { p++; continue; }
                if (c == '>') { emit(p, p + 1, ClassificationNames.XmlDelimiter); return p + 1; }
                if ((c == '/' || c == '?') && p + 1 < to && m[p + 1] == '>')
                {
                    emit(p, p + 2, ClassificationNames.XmlDelimiter);
                    return p + 2;
                }
                if (c == '<') return p; // malformed tag; restart analysis

                int a = p;
                while (p < to && !char.IsWhiteSpace(m[p]) && m[p] != '=' && m[p] != '>' && m[p] != '"' &&
                       m[p] != '\'' && m[p] != '<' && !((m[p] == '/' || m[p] == '?') && p + 1 < to && m[p + 1] == '>'))
                    p++;
                if (p == a) { p++; continue; }
                emit(a, p, ClassificationNames.XmlAttribute);

                int q = p;
                while (q < to && char.IsWhiteSpace(m[q])) q++;
                if (q < to && m[q] == '=')
                {
                    q++;
                    while (q < to && char.IsWhiteSpace(m[q])) q++;
                    if (q < to && (m[q] == '"' || m[q] == '\''))
                    {
                        char quote = m[q];
                        int v = q++;
                        while (q < to && m[q] != quote) q++;
                        q = q < to ? q + 1 : to;
                        emit(v, q, ClassificationNames.XmlValue);
                    }
                    else
                    {
                        int v = q;
                        while (q < to && !char.IsWhiteSpace(m[q]) && m[q] != '>' &&
                               !((m[q] == '/' || m[q] == '?') && q + 1 < to && m[q + 1] == '>'))
                            q++;
                        emit(v, q, ClassificationNames.XmlValue);
                    }
                    p = q;
                }
            }
            return p;
        }

        private static bool IsNameChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == ':' || c == '.' || c == Mask;
        }
    }
}
