using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.EmbeddedLanguages
{
    /// <summary>SQL: keywords, identifiers (bare and quoted), literals, operators and comments.</summary>
    internal sealed class SqlTokenizer : IEmbeddedLanguageTokenizer
    {
        private const char Mask = TextUtil.Mask;
        private static readonly string[] SqlIds = { "sql" };

        internal static readonly HashSet<string> Keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "select", "from", "where", "insert", "into", "values", "update", "set", "delete", "create", "alter", "drop",
            "table", "view", "index", "database", "schema", "join", "inner", "left", "right", "full", "outer", "cross",
            "natural", "on", "using", "group", "by", "order", "having", "limit", "offset", "fetch", "first", "next",
            "rows", "row", "only", "union", "all", "distinct", "intersect", "except", "as", "and", "or", "not", "null",
            "is", "in", "exists", "between", "like", "ilike", "similar", "case", "when", "then", "else", "end", "asc",
            "desc", "primary", "key", "foreign", "references", "unique", "check", "default", "constraint", "cascade",
            "truncate", "begin", "commit", "rollback", "transaction", "savepoint", "with", "recursive", "over",
            "partition", "window", "returning", "true", "false", "add", "column", "rename", "to", "grant", "revoke",
            "if", "replace", "temp", "temporary", "explain", "analyze", "vacuum", "declare", "cast", "any", "some",
            "escape", "lateral", "merge", "matched", "do", "nothing", "conflict", "for", "share", "lock", "unlock",
            "procedure", "function", "trigger", "returns", "return", "language", "execute", "call", "use", "top",
            "int", "integer", "bigint", "smallint", "tinyint", "varchar", "nvarchar", "char", "text", "boolean", "bool",
            "date", "time", "timestamp", "timestamptz", "interval", "decimal", "numeric", "float", "real", "double",
            "precision", "serial", "bigserial", "uuid", "json", "jsonb", "bytea", "blob", "money"
        };

        public IReadOnlyList<string> Ids { get { return SqlIds; } }

        public void Tokenize(char[] m, int from, int to, TokenSink emit)
        {
            int i = from;
            while (i < to)
            {
                char c = m[i];
                if (char.IsWhiteSpace(c)) { i++; continue; }

                if (c == '-' && TextUtil.StartsWith(m, i, to, "--"))
                {
                    int e = TextUtil.LineEnd(m, i, to);
                    emit(i, e, ClassificationNames.SqlComment);
                    i = e;
                }
                else if (c == '/' && TextUtil.StartsWith(m, i, to, "/*"))
                {
                    int e = TextUtil.IndexOf(m, "*/", i + 2, to);
                    int end = e < 0 ? to : e + 2;
                    emit(i, end, ClassificationNames.SqlComment);
                    i = end;
                }
                else if (c == '\'')
                {
                    int end = Quoted(m, i, to, '\'');
                    emit(i, end, ClassificationNames.SqlString);
                    i = end;
                }
                else if (c == '"' || c == '`')
                {
                    int end = Quoted(m, i, to, c);
                    emit(i, end, ClassificationNames.SqlIdentifier);
                    i = end;
                }
                else if (c == '[' && TryBracketIdentifier(m, i, to, out int bracketEnd))
                {
                    emit(i, bracketEnd, ClassificationNames.SqlIdentifier);
                    i = bracketEnd;
                }
                else if (char.IsDigit(c) || (c == '.' && i + 1 < to && char.IsDigit(m[i + 1])))
                {
                    int end = Number(m, i, to);
                    emit(i, end, ClassificationNames.SqlNumber);
                    i = end;
                }
                else if ((c == '@' || c == ':' || c == '$') && i + 1 < to && TextUtil.IsWordChar(m[i + 1]) &&
                         !(c == ':' && i > from && m[i - 1] == ':'))
                {
                    // parameters: @name, :name, $1
                    int end = i + 1;
                    while (end < to && TextUtil.IsWordChar(m[end])) end++;
                    emit(i, end, ClassificationNames.SqlParameter);
                    i = end;
                }
                else if (TextUtil.IsWordStart(c))
                {
                    int end = i + 1;
                    while (end < to && TextUtil.IsWordChar(m[end])) end++;
                    bool keyword = Keywords.Contains(TextUtil.Substring(m, i, end));
                    emit(i, end, keyword ? ClassificationNames.SqlKeyword : ClassificationNames.SqlIdentifier);
                    i = end;
                }
                else if (c == ':' && i + 1 < to && m[i + 1] == ':')
                {
                    emit(i, i + 2, ClassificationNames.SqlOperator);
                    i += 2;
                }
                else if ("+-*/%=<>!|&^~".IndexOf(c) >= 0)
                {
                    int end = i + 1;
                    while (end < to && "=<>|&".IndexOf(m[end]) >= 0) end++; // <=, <>, ||, &&...
                    emit(i, end, ClassificationNames.SqlOperator);
                    i = end;
                }
                else
                {
                    i++; // ( ) , ; . and anything else
                }
            }
        }

        /// <summary>End of a quoted run; a doubled quote is an escaped quote.</summary>
        private static int Quoted(char[] m, int i, int to, char quote)
        {
            int p = i + 1;
            while (p < to)
            {
                if (m[p] == quote)
                {
                    if (p + 1 < to && m[p + 1] == quote) { p += 2; continue; }
                    return p + 1;
                }
                p++;
            }
            return to;
        }

        private static bool TryBracketIdentifier(char[] m, int i, int to, out int end)
        {
            end = i;
            int p = i + 1;
            if (p >= to || !(char.IsLetter(m[p]) || m[p] == '_' || m[p] == Mask)) return false;
            while (p < to && m[p] != ']' && m[p] != '[' && m[p] != '\n') p++;
            if (p >= to || m[p] != ']') return false;
            end = p + 1;
            return true;
        }

        private static int Number(char[] m, int i, int to)
        {
            int p = i;
            if (m[p] == '0' && p + 1 < to && (m[p + 1] == 'x' || m[p + 1] == 'X'))
            {
                p += 2;
                while (p < to && Uri.IsHexDigit(m[p])) p++;
                return p;
            }
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
            return p;
        }
    }
}
