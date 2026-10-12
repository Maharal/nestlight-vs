using System;
using System.Collections.Generic;

namespace NestLight.Common
{
    internal static class InterpolationHighlighter
    {
        private static readonly HashSet<string> Keywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "const", "let", "var", "function", "return", "if", "else", "for", "while", "do",
            "switch", "case", "break", "continue", "new", "delete", "typeof", "instanceof",
            "in", "of", "class", "extends", "super", "this", "import", "export", "default",
            "try", "catch", "finally", "throw", "async", "await", "yield", "from",
            "true", "false", "null", "undefined", "void",
            "int", "string", "bool", "double", "float", "long", "char", "byte", "object",
            "as", "is", "not", "and", "or", "None", "True", "False", "self",
            "auto", "static", "public", "private", "protected", "internal", "override", "virtual", "abstract",
            "readonly", "sealed", "ref", "out", "params", "where", "select", "when",
            "lambda", "def", "pass", "print", "range", "len", "type",
            "fn", "mut", "pub", "impl", "trait", "struct", "enum", "match", "mod", "use", "crate",
            "func", "package", "defer", "go", "chan", "map", "make",
            "final", "implements", "interface", "throws", "native", "synchronized", "volatile", "transient",
        };

        public static void Highlight(string text, int start, int end, List<Token> output)
        {
            int i = start;
            while (i < end)
            {
                char c = text[i];

                if (char.IsWhiteSpace(c)) { i++; continue; }

                if (c == '/' && i + 1 < end)
                {
                    if (text[i + 1] == '/')
                    {
                        int nl = text.IndexOf('\n', i + 2, end - i - 2);
                        i = nl < 0 ? end : nl + 1;
                        continue;
                    }
                    if (text[i + 1] == '*')
                    {
                        int close = text.IndexOf("*/", i + 2, end - i - 2, StringComparison.Ordinal);
                        i = close < 0 ? end : close + 2;
                        continue;
                    }
                }

                if (c == '"' || c == '\'' || c == '`')
                {
                    int strEnd = i + 1;
                    while (strEnd < end)
                    {
                        if (text[strEnd] == '\\') { strEnd += 2; continue; }
                        if (text[strEnd] == c) { strEnd++; break; }
                        strEnd++;
                    }
                    output.Add(new Token(i, strEnd - i, ClassificationNames.ExprString));
                    i = strEnd;
                    continue;
                }

                if (char.IsDigit(c) || (c == '.' && i + 1 < end && char.IsDigit(text[i + 1])))
                {
                    int numEnd = i + 1;
                    while (numEnd < end && (char.IsLetterOrDigit(text[numEnd]) || text[numEnd] == '.' || text[numEnd] == '_' || text[numEnd] == 'x' || text[numEnd] == 'X')) numEnd++;
                    output.Add(new Token(i, numEnd - i, ClassificationNames.ExprNumber));
                    i = numEnd;
                    continue;
                }

                if (char.IsLetter(c) || c == '_' || c == '$')
                {
                    int wordEnd = i + 1;
                    while (wordEnd < end && (char.IsLetterOrDigit(text[wordEnd]) || text[wordEnd] == '_' || text[wordEnd] == '$')) wordEnd++;
                    string word = text.Substring(i, wordEnd - i);
                    if (Keywords.Contains(word))
                        output.Add(new Token(i, wordEnd - i, ClassificationNames.ExprKeyword));
                    i = wordEnd;
                    continue;
                }

                i++;
            }
        }
    }
}
