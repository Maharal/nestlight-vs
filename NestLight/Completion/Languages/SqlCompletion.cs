using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.EmbeddedLanguages;

namespace NestLight.Completion
{
    /// <summary>SQL: keywords that follow the case being typed, a grammar for the place of the caret, and the tables and columns the document declares.</summary>
    internal sealed class SqlCompletion : CompletionLanguage, ISchemaCompletion
    {
        public SqlCompletion() : base(new[] { "sql" }, SqlTokenizer.Keywords) { }

        public override bool KeywordsFollowTypedCase { get { return true; } }

        // ---- the schema of the document ------------------------------------------------------------------------

        public List<string> SchemaWords(string text, CompletionSite site, Position place, int[] ranges, int from, int to)
        {
            SqlSchema.Statement current;
            SqlSchema schema = SqlSchema.Read(text, ranges, from, to, site.Start, site.End, out current);

            var words = new List<string>();
            switch (place.Role)
            {
                case PlaceRole.Table:
                    foreach (SqlSchema.Table t in schema.Tables.OrderByDescending(t => t.Declared).ThenByDescending(t => t.Uses).ThenBy(t => t.Name, StringComparer.OrdinalIgnoreCase))
                        words.Add(t.Name);
                    break;
                case PlaceRole.Member:
                {
                    SqlSchema.Table table;
                    if (!current.Names.TryGetValue(place.Qualifier, out table)) table = schema.Find(place.Qualifier);
                    if (table != null) words.AddRange(table.Columns);
                    break;
                }
                case PlaceRole.Column:
                    foreach (SqlSchema.Table table in current.Tables)
                        foreach (string column in table.Columns)
                            if (!words.Contains(column, StringComparer.OrdinalIgnoreCase)) words.Add(column);
                    break;
            }
            return words.Count == 0 ? null : words;
        }

        // ---- the grammar --------------------------------------------------------------------------------------

        private static readonly string[] SqlStarters = { "select", "insert", "update", "delete", "with", "create", "alter", "drop", "truncate", "merge", "explain" };

        // what can follow a column or a table name, by the clause it is in: the most used first
        private static readonly string[] AfterSelectList = { "from", "as", "where", "order", "group", "limit", "union", "into" };
        private static readonly string[] AfterFrom = { "where", "join", "inner", "left", "right", "full", "cross", "on", "as", "group", "order", "limit", "union", "having" };
        private static readonly string[] AfterCondition = { "and", "or", "group", "order", "limit", "having", "in", "is", "like", "between", "not", "union", "offset" };
        private static readonly string[] AfterJoin = { "on", "using", "join", "inner", "left", "right", "where", "group", "order", "limit" };
        private static readonly string[] AfterUpdate = { "set", "where" };
        private static readonly string[] AfterLimit = { "offset" };
        private static readonly string[] AfterSet = { "where", "from", "returning" };
        private static readonly string[] AfterInto = { "values", "select", "set" };
        private static readonly string[] AfterOrder = { "asc", "desc", "limit", "offset", "nulls" };
        private static readonly string[] AfterGroup = { "having", "order", "limit", "union", "offset" };
        private static readonly string[] AfterAlterTable = { "add", "drop", "alter", "rename" };

        // the continuations by the clause the caret is in; a clause that is not here has no rule
        private static readonly Dictionary<string, string[]> AfterClause = new Dictionary<string, string[]>
        {
            { "select", AfterSelectList }, { "from", AfterFrom }, { "join", AfterJoin }, { "update", AfterUpdate }, { "set", AfterSet },
            { "limit", AfterLimit }, { "into", AfterInto }, { "group", AfterGroup }, { "order", AfterOrder },
            { "where", AfterCondition }, { "on", AfterCondition }, { "having", AfterCondition }
        };

        private static readonly string[] SqlTypes =
            { "int", "integer", "bigint", "smallint", "serial", "bigserial", "text", "varchar", "char", "boolean", "bool", "date", "timestamp", "timestamptz", "time", "numeric", "decimal", "float", "double", "real", "uuid", "json", "jsonb", "blob", "bytea" };
        private static readonly string[] SqlConstraints = { "not", "null", "primary", "key", "unique", "default", "references", "check", "constraint", "auto_increment", "generated", "collate" };

        private static readonly string[] SqlFunctions =
            { "count", "sum", "avg", "min", "max", "coalesce", "nullif", "now", "current_date", "current_timestamp", "upper", "lower", "length", "substring", "trim", "concat", "replace", "round", "floor", "ceil", "abs",
              "cast", "extract", "date_trunc", "row_number", "rank", "dense_rank", "lag", "lead", "first_value", "last_value", "ntile", "string_agg", "array_agg", "json_agg", "greatest", "least", "ifnull", "date_add", "date_sub" };

        private static readonly Dictionary<string, string[]> SqlNext = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "insert", new[] { "into" } },
            { "delete", new[] { "from" } },
            { "group", new[] { "by" } },
            { "order", new[] { "by" } },
            { "partition", new[] { "by" } },
            { "inner", new[] { "join" } },
            { "cross", new[] { "join" } },
            { "natural", new[] { "join" } },
            { "left", new[] { "join", "outer" } },
            { "right", new[] { "join", "outer" } },
            { "full", new[] { "join", "outer" } },
            { "outer", new[] { "join" } },
            { "union", new[] { "all", "select" } },
            { "is", new[] { "not", "null", "true", "false" } },
            { "not", new[] { "null", "in", "exists", "like", "between" } },
            { "primary", new[] { "key" } },
            { "foreign", new[] { "key" } },
            { "create", new[] { "table", "index", "view", "unique", "database", "schema", "or", "temporary" } },
            { "alter", new[] { "table", "column", "index", "view" } },
            { "drop", new[] { "table", "index", "view", "database", "schema", "column", "constraint" } },
            { "add", new[] { "column", "constraint", "primary", "foreign", "unique", "index" } },
            { "select", new[] { "distinct", "all" } },
            { "case", new[] { "when" } },
            { "do", new[] { "nothing", "update" } },
        };

        // after these the next word is a table
        private static readonly HashSet<string> SqlTableAfter = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "from", "join", "into", "update", "table", "truncate" };
        // after these the next word is an expression: a column, a function, a value
        private static readonly HashSet<string> SqlExpressionAfter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "select", "where", "and", "or", "on", "having", "set", "by", "when", "then", "else", "case", "using", "limit", "offset", "values", "between", "like", "in", "exists", "returning", "distinct" };
        private static readonly HashSet<string> SqlClauses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "select", "from", "where", "group", "order", "having", "set", "values", "into", "update", "join", "on", "limit" };

        protected override Position ReadPosition(string text, int floor, CompletionSite site)
        {
            // read the statement up to the caret: the last two words, what lies between the last one and the caret, the clause
            // (a parenthesis opens a clause of its own: the ORDER BY of an OVER( ) is not the clause of the select list)
            string last = null, previous = null, first = null, clause = null;
            var clauses = new Stack<string>();
            var between = new List<char>();
            int depth = 0, entryWords = 0;
            bool createTable = false;
            int i = floor;
            while (i < site.Start)
            {
                char c = text[i];
                if (c == '\'' || c == '"' || c == '`')
                {
                    // a literal or a quoted name counts as an operand; a caret inside one is typing text, not SQL
                    int close = text.IndexOf(c, i + 1);
                    if (close < 0 || close >= site.Start) return new Position("sql:literal", onlyWords: true);
                    i = close + 1;
                    previous = last; last = "\u0001"; between.Clear();
                    if (createTable && depth == 1) entryWords++;
                    continue;
                }
                if (c == '-' && i + 1 < site.Start && text[i + 1] == '-')
                {
                    while (i < site.Start && text[i] != '\n') i++;
                    if (i >= site.Start) return new Position("sql:comment", onlyWords: true);
                    continue;
                }
                if (c == '/' && i + 1 < site.Start && text[i + 1] == '*')
                {
                    int close = text.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    if (close < 0 || close >= site.Start) return new Position("sql:comment", onlyWords: true);
                    i = close + 2;
                    continue;
                }
                if (c == ';')
                {
                    last = previous = first = clause = null; clauses.Clear(); between.Clear();
                    depth = 0; entryWords = 0; createTable = false;
                    i++; continue;
                }
                if (c == '(')
                {
                    clauses.Push(clause); depth++; between.Add('(');
                    if (createTable && depth == 1) entryWords = 0;
                    i++; continue;
                }
                if (c == ')')
                {
                    if (depth > 0) { depth--; clause = clauses.Pop(); }
                    between.Clear(); between.Add(')');
                    i++; continue;
                }
                if (c == ',' && createTable && depth == 1) entryWords = 0;
                if (IsWordChar(c))
                {
                    int start = i;
                    while (i < site.Start && IsWordChar(text[i])) i++;
                    string word = text.Substring(start, i - start);
                    previous = last; last = word;
                    if (first == null) first = word.ToLowerInvariant();
                    if (SqlClauses.Contains(word)) clause = word.ToLowerInvariant();
                    if (first == "create" && word.Equals("table", StringComparison.OrdinalIgnoreCase)) createTable = true;
                    if (createTable && depth == 1) entryWords++;
                    between.Clear();
                    continue;
                }
                if (!char.IsWhiteSpace(c)) between.Add(c);
                i++;
            }
            // a number is an operand too
            if (last != null && char.IsDigit(last[0])) last = "\u0001";

            if (between.Contains('.'))
                return new Position("sql:member", wordsFirst: true, unlikely: Always, role: last != null && last != "\u0001" ? PlaceRole.Member : PlaceRole.None, qualifier: last);

            // inside the definition of a table: a column name, then its type, then its constraints
            if (createTable && depth == 1)
            {
                if (entryWords == 0) return new Position("sql:column-name", onlyWords: true);
                if (entryWords == 1) return new Position("sql:column-type", SqlTypes);
                return new Position("sql:column-constraint", SqlConstraints);
            }

            if (last == null) return new Position("sql:statement", SqlStarters);
            char tail = between.Count > 0 ? between[between.Count - 1] : ' ';
            if (tail == '(' || tail == ',' || tail == '=' || tail == '<' || tail == '>' || tail == '+' || tail == '-' || tail == '*' || tail == '/')
                return new Position("sql:expression", wordsFirst: true, role: PlaceRole.Column, secondary: SqlFunctions);

            bool isWord = last != "\u0001";
            if (isWord && first == "alter" && last.Equals("drop", StringComparison.OrdinalIgnoreCase))
                return new Position("sql:after-drop", new[] { "column", "constraint", "index" });
            if (isWord && first == "alter" && previous != null && previous.Equals("table", StringComparison.OrdinalIgnoreCase))
                return new Position("sql:alter-table", AfterAlterTable);
            if (isWord && first == "insert" && last.Equals("on", StringComparison.OrdinalIgnoreCase))
                return new Position("sql:after-on-insert", new[] { "conflict", "duplicate" });

            string[] next;
            if (isWord && SqlNext.TryGetValue(last, out next))
            {
                bool select = last.Equals("select", StringComparison.OrdinalIgnoreCase);
                return new Position("sql:after-" + last.ToLowerInvariant(), next, wordsFirst: select, role: select ? PlaceRole.Column : PlaceRole.None, secondary: select ? SqlFunctions : null);
            }
            if (isWord && SqlTableAfter.Contains(last)) return new Position("sql:table", wordsFirst: true, unlikely: Always, role: PlaceRole.Table);
            if (isWord && SqlExpressionAfter.Contains(last)) return new Position("sql:expression", wordsFirst: true, role: PlaceRole.Column, secondary: SqlFunctions);

            // after a column, a table, a literal or a closing parenthesis: the word that continues the clause
            if (!isWord || FindKeyword(last) == null || tail == ')')
            {
                string[] continuation;
                if (clause == null || !AfterClause.TryGetValue(clause, out continuation)) return null;
                return new Position("sql:continue-" + clause, continuation);
            }
            return null;
        }
    }
}
