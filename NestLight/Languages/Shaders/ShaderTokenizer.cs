using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;

namespace NestLight.Languages
{
    /// <summary>
    /// A C-like shader language (GLSL, WGSL): keywords, types, built-ins, numbers and comments.
    /// The language is defined by its word lists, so one tokenizer serves both.
    /// </summary>
    internal sealed class ShaderTokenizer : ILanguageTokenizer
    {
        private readonly IReadOnlyList<string> _ids;
        private readonly HashSet<string> _keywords;
        private readonly HashSet<string> _types;
        private readonly HashSet<string> _builtins;
        private readonly string _builtinPrefix;

        public ShaderTokenizer(IEnumerable<string> ids, IEnumerable<string> keywords, IEnumerable<string> types,
                               IEnumerable<string> builtins, string builtinPrefix = null)
        {
            _ids = ids.ToList();
            _keywords = new HashSet<string>(keywords);
            _types = new HashSet<string>(types);
            _builtins = new HashSet<string>(builtins);
            _builtinPrefix = builtinPrefix;
        }

        public IReadOnlyList<string> Ids { get { return _ids; } }

        public void Tokenize(char[] m, int from, int to, TokenSink emit)
        {
            int i = from;
            while (i < to)
            {
                char c = m[i];
                if (char.IsWhiteSpace(c)) { i++; continue; }

                if (c == '/' && TextUtil.StartsWith(m, i, to, "//"))
                {
                    int e = TextUtil.LineEnd(m, i, to);
                    emit(i, e, ClassificationNames.ShaderComment);
                    i = e;
                }
                else if (c == '/' && TextUtil.StartsWith(m, i, to, "/*"))
                {
                    int e = TextUtil.IndexOf(m, "*/", i + 2, to);
                    int end = e < 0 ? to : e + 2;
                    emit(i, end, ClassificationNames.ShaderComment);
                    i = end;
                }
                else if ((c == '#' || c == '@') && i + 1 < to && char.IsLetter(m[i + 1]) && (c == '@' || OnlyBlanksBefore(m, from, i)))
                {
                    // preprocessor directive (#version) or WGSL attribute (@vertex)
                    int end = i + 1;
                    while (end < to && TextUtil.IsWordChar(m[end])) end++;
                    emit(i, end, ClassificationNames.ShaderKeyword);
                    i = end;
                }
                else if (char.IsDigit(c) || (c == '.' && i + 1 < to && char.IsDigit(m[i + 1])))
                {
                    int end = Number(m, i, to);
                    emit(i, end, ClassificationNames.ShaderNumber);
                    i = end;
                }
                else if (TextUtil.IsWordStart(c))
                {
                    int end = i + 1;
                    while (end < to && TextUtil.IsWordChar(m[end])) end++;
                    string word = TextUtil.Substring(m, i, end);
                    string type = _keywords.Contains(word) ? ClassificationNames.ShaderKeyword
                                : _types.Contains(word) ? ClassificationNames.ShaderType
                                : _builtins.Contains(word) || (_builtinPrefix != null && word.StartsWith(_builtinPrefix, StringComparison.Ordinal))
                                    ? ClassificationNames.ShaderBuiltin
                                : null;
                    if (type != null) emit(i, end, type);
                    i = end;
                }
                else
                {
                    i++;
                }
            }
        }

        private static bool OnlyBlanksBefore(char[] m, int from, int i)
        {
            for (int k = i - 1; k >= from && m[k] != '\n'; k--)
                if (m[k] != ' ' && m[k] != '\t') return false;
            return true;
        }

        private static int Number(char[] m, int i, int to)
        {
            int p = i;
            if (m[p] == '0' && p + 1 < to && (m[p + 1] == 'x' || m[p + 1] == 'X'))
            {
                p += 2;
                while (p < to && Uri.IsHexDigit(m[p])) p++;
            }
            else
            {
                while (p < to && (char.IsDigit(m[p]) || m[p] == '.')) p++;
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
            }
            while (p < to && (m[p] == 'f' || m[p] == 'F' || m[p] == 'u' || m[p] == 'U' || m[p] == 'i' || m[p] == 'h' || m[p] == 'l' || m[p] == 'L')) p++;
            return p;
        }
    }
}
