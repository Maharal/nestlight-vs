using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Languages
{
    /// <summary>
    /// GraphQL queries and schemas: operations, fields, arguments, variables, types and directives.
    /// A small amount of context decides what a name is (an argument before a colon inside parentheses,
    /// a type after <c>on</c> or after the colon of a variable or of a schema field...).
    /// </summary>
    internal sealed class GraphQlTokenizer : ILanguageTokenizer
    {
        private static readonly string[] GraphQlIds = { "graphql", "gql" };

        private static readonly HashSet<string> Operations = new HashSet<string> { "query", "mutation", "subscription", "fragment" };
        private static readonly HashSet<string> TypeDefinitions = new HashSet<string> { "type", "interface", "input", "enum", "scalar", "union" };
        private static readonly HashSet<string> OtherKeywords = new HashSet<string> { "schema", "extend", "directive", "implements", "repeatable" };
        private static readonly HashSet<string> Literals = new HashSet<string> { "true", "false", "null" };

        public IReadOnlyList<string> Ids { get { return GraphQlIds; } }

        public void Tokenize(char[] m, int from, int to, TokenSink emit)
        {
            int braces = 0, parens = 0;
            bool schema = false;      // a type system definition was seen: a colon is followed by a type
            bool expectType = false;  // the next name is a type
            bool typeList = false;    // union members and implemented interfaces: every name is a type
            bool operationName = false;
            bool afterVariable = false, afterSpread = false;
            bool fragmentDefinition = false, pendingOn = false; // fragment Name on Type

            int i = from;
            while (i < to)
            {
                char c = m[i];
                if (char.IsWhiteSpace(c) || c == ',') { i++; continue; }

                if (c == '#')
                {
                    int e = TextUtil.LineEnd(m, i, to);
                    emit(i, e, ClassificationNames.GqlComment);
                    i = e;
                    continue;
                }
                if (c == '"')
                {
                    int end = StringEnd(m, i, to);
                    emit(i, end, ClassificationNames.GqlString);
                    i = end;
                    afterVariable = afterSpread = false;
                    continue;
                }
                if (c == '$' || c == '@')
                {
                    int end = i + 1;
                    while (end < to && TextUtil.IsWordChar(m[end])) end++;
                    emit(i, end, c == '$' ? ClassificationNames.GqlVariable : ClassificationNames.GqlDirective);
                    i = end;
                    afterVariable = c == '$';
                    afterSpread = false;
                    continue;
                }
                if (char.IsDigit(c) || (c == '-' && i + 1 < to && char.IsDigit(m[i + 1])))
                {
                    int end = i + 1;
                    while (end < to && (char.IsDigit(m[end]) || m[end] == '.' || m[end] == 'e' || m[end] == 'E' ||
                                        ((m[end] == '+' || m[end] == '-') && (m[end - 1] == 'e' || m[end - 1] == 'E'))))
                        end++;
                    emit(i, end, ClassificationNames.GqlNumber);
                    i = end;
                    afterVariable = afterSpread = false;
                    continue;
                }
                if (TextUtil.IsWordStart(c))
                {
                    int end = i + 1;
                    while (end < to && TextUtil.IsWordChar(m[end])) end++;
                    string name = TextUtil.Substring(m, i, end);
                    string type;
                    bool afterFragmentName = pendingOn;
                    pendingOn = false;

                    if (braces == 0 && Operations.Contains(name))
                    {
                        type = ClassificationNames.GqlOperation;
                        operationName = true;
                        fragmentDefinition = name == "fragment";
                    }
                    else if (braces == 0 && TypeDefinitions.Contains(name))
                    {
                        type = ClassificationNames.GqlKeyword;
                        schema = true;
                        expectType = true;
                        typeList = name == "union";
                    }
                    else if (braces == 0 && OtherKeywords.Contains(name))
                    {
                        type = ClassificationNames.GqlKeyword;
                        schema = true;
                        if (name == "implements") typeList = true;
                    }
                    else if (name == "on" && (afterSpread || afterFragmentName || (schema && braces == 0 && parens == 0)))
                    {
                        type = ClassificationNames.GqlKeyword;
                        expectType = afterSpread || afterFragmentName;
                    }
                    else if (Literals.Contains(name))
                    {
                        type = ClassificationNames.GqlKeyword;
                    }
                    else if (operationName)
                    {
                        type = ClassificationNames.GqlOperation; // the name of the operation or fragment
                        operationName = false;
                        pendingOn = fragmentDefinition;
                        fragmentDefinition = false;
                    }
                    else if (expectType || typeList)
                    {
                        type = ClassificationNames.GqlType;
                        expectType = false;
                    }
                    else if (parens > 0 && NextSignificant(m, end, to) == ':')
                    {
                        type = ClassificationNames.GqlArgument;
                    }
                    else
                    {
                        type = ClassificationNames.GqlField;
                    }

                    emit(i, end, type);
                    i = end;
                    afterVariable = afterSpread = false;
                    continue;
                }

                // punctuation
                switch (c)
                {
                    case '{':
                        braces++; expectType = false; typeList = false; operationName = false; break;
                    case '}':
                        if (braces > 0) braces--;
                        break;
                    case '(':
                        parens++; operationName = false; break;
                    case ')':
                        if (parens > 0) parens--;
                        expectType = false;
                        break;
                    case ':':
                        if (schema || afterVariable) expectType = true;
                        break;
                    case '=':
                        expectType = false;
                        break;
                    case '.':
                        if (TextUtil.StartsWith(m, i, to, "...")) { afterSpread = true; i += 3; afterVariable = false; continue; }
                        break;
                }
                afterVariable = false;
                if (c != '.') afterSpread = false;
                i++;
            }
        }

        private static char NextSignificant(char[] m, int i, int to)
        {
            while (i < to)
            {
                char c = m[i];
                if (char.IsWhiteSpace(c) || c == ',') { i++; continue; }
                if (c == '#') { i = TextUtil.LineEnd(m, i, to); continue; }
                return c;
            }
            return '\0';
        }

        private static int StringEnd(char[] m, int i, int to)
        {
            if (TextUtil.StartsWith(m, i, to, "\"\"\""))
            {
                int e = TextUtil.IndexOf(m, "\"\"\"", i + 3, to);
                return e < 0 ? to : e + 3;
            }
            int p = i + 1;
            while (p < to)
            {
                char c = m[p];
                if (c == '\\') p += 2;
                else if (c == '"') return p + 1;
                else if (c == '\n') return p;
                else p++;
            }
            return Math.Min(p, to);
        }
    }
}
