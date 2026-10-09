using System;
using System.Collections.Generic;
using System.Linq;

namespace NestLight.Completion
{
    /// <summary>
    /// The tables and columns an SQL document talks about, read from the strings themselves: <c>CREATE TABLE</c> and <c>ALTER TABLE</c>,
    /// the column list of <c>INSERT INTO</c>, the <c>SET</c> of an <c>UPDATE</c>, and the references <c>alias.column</c> resolved
    /// through the <c>FROM</c> / <c>JOIN</c> of their own statement. It is a reader of the common shapes, not a parser: a statement it
    /// does not understand contributes nothing.
    /// </summary>
    internal sealed class SqlSchema
    {
        public sealed class Table
        {
            public string Name;
            public readonly List<string> Columns = new List<string>();
            /// <summary>Declared by a CREATE TABLE: its columns are known, not guessed from the queries.</summary>
            public bool Declared;
            /// <summary>The times a statement named it.</summary>
            public int Uses;
            public bool Add(string column)
            {
                if (Columns.Exists(c => string.Equals(c, column, StringComparison.OrdinalIgnoreCase))) return false;
                Columns.Add(column);
                return true;
            }
        }

        private readonly Dictionary<string, Table> _tables = new Dictionary<string, Table>(StringComparer.OrdinalIgnoreCase);

        public IEnumerable<Table> Tables { get { return _tables.Values; } }

        public Table Find(string name)
        {
            Table t;
            return name != null && _tables.TryGetValue(name, out t) ? t : null;
        }

        private Table Get(string name)
        {
            Table t;
            if (!_tables.TryGetValue(name, out t)) _tables[name] = t = new Table { Name = name };
            return t;
        }

        /// <summary>The tables a statement refers to, in order, and the names (aliases or the tables' own) that stand for them.</summary>
        public sealed class Statement
        {
            public readonly List<Table> Tables = new List<Table>();
            public readonly Dictionary<string, Table> Names = new Dictionary<string, Table>(StringComparer.OrdinalIgnoreCase);
        }

        // ---- reading ---------------------------------------------------------------------------------------------------

        private struct Tok
        {
            public int Start, Length;
            public char Punctuation;
            public bool IsWord;
        }

        private static bool IsWordChar(char c) { return char.IsLetterOrDigit(c) || c == '_'; }

        private static List<Tok> Tokenize(string text, int from, int to, int skipFrom, int skipTo)
        {
            var tokens = new List<Tok>();
            int i = from;
            while (i < to)
            {
                if (i >= skipFrom && i < skipTo) { i = skipTo; continue; }
                char c = text[i];
                if (c == '-' && i + 1 < to && text[i + 1] == '-') { while (i < to && text[i] != '\n') i++; continue; }
                if (c == '/' && i + 1 < to && text[i + 1] == '*')
                {
                    int close = text.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    i = close < 0 || close >= to ? to : close + 2;
                    continue;
                }
                if (c == '\'' || c == '"' || c == '`')
                {
                    int close = text.IndexOf(c, i + 1);
                    i = close < 0 || close >= to ? to : close + 1;
                    tokens.Add(new Tok { Start = i, Length = 0, Punctuation = '\'' }); // a literal: an operand without a name
                    continue;
                }
                if (IsWordChar(c))
                {
                    int start = i;
                    while (i < to && IsWordChar(text[i])) i++;
                    tokens.Add(new Tok { Start = start, Length = i - start, IsWord = !char.IsDigit(text[start]), Punctuation = char.IsDigit(text[start]) ? '\'' : '\0' });
                    continue;
                }
                if (!char.IsWhiteSpace(c)) tokens.Add(new Tok { Start = i, Length = 1, Punctuation = c });
                i++;
            }
            return tokens;
        }

        private static readonly HashSet<string> NotAnAlias = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "where", "on", "join", "inner", "left", "right", "full", "cross", "natural", "outer", "group", "order", "having", "limit", "offset",
            "union", "using", "set", "returning", "values", "select", "from", "into", "as", "and", "or", "window", "for", "fetch", "lateral",
        };

        private static readonly HashSet<string> NotAColumn = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "primary", "foreign", "unique", "constraint", "check", "key", "index", "like", "exclude", "fulltext", "spatial" };

        private static readonly HashSet<string> EndsFrom = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "where", "group", "order", "having", "limit", "offset", "union", "set", "returning", "values", "select", "on", "using", "window", "fetch", "for" };

        private sealed class Reader
        {
            private readonly string _text;
            private readonly SqlSchema _schema;
            private readonly List<Tok> _t;
            public readonly Statement Statement = new Statement();
            private readonly List<KeyValuePair<string, string>> _references = new List<KeyValuePair<string, string>>();

            public Reader(string text, SqlSchema schema, List<Tok> tokens)
            {
                _text = text; _schema = schema; _t = tokens;
            }

            private string Word(int i) { return i >= 0 && i < _t.Count && _t[i].IsWord ? _text.Substring(_t[i].Start, _t[i].Length) : null; }
            private bool Is(int i, string keyword)
            {
                return i >= 0 && i < _t.Count && _t[i].IsWord && _t[i].Length == keyword.Length
                    && string.Compare(_text, _t[i].Start, keyword, 0, keyword.Length, StringComparison.OrdinalIgnoreCase) == 0;
            }
            private bool Punct(int i, char c) { return i >= 0 && i < _t.Count && !_t[i].IsWord && _t[i].Punctuation == c; }

            /// <summary>The name of a table at i, past a schema prefix (<c>public.users</c>); next is the index after it.</summary>
            private string TableName(int i, out int next)
            {
                next = i;
                string name = Word(i);
                if (name == null) return null;
                next = i + 1;
                while (Punct(next, '.') && Word(next + 1) != null) { name = Word(next + 1); next += 2; }
                return name;
            }

            private void Refer(string name, string alias)
            {
                Table table = _schema.Get(name);
                table.Uses++;
                if (!Statement.Tables.Contains(table)) Statement.Tables.Add(table);
                Statement.Names[name] = table;
                if (alias != null) Statement.Names[alias] = table;
            }

            public void Read()
            {
                bool inFrom = false;
                int depth = 0;
                for (int i = 0; i < _t.Count; i++)
                {
                    if (Punct(i, '(')) depth++;
                    else if (Punct(i, ')')) { if (depth > 0) depth--; }

                    if (_t[i].IsWord && EndsFrom.Contains(_text.Substring(_t[i].Start, _t[i].Length)) && !Is(i, "from") ) inFrom = false;

                    if (Is(i, "create")) { i = Create(i); continue; }
                    if (Is(i, "alter") && Is(i + 1, "table")) { i = Alter(i); continue; }
                    if (Is(i, "insert") && Is(i + 1, "into")) { i = Insert(i); continue; }
                    if (Is(i, "update")) { i = Update(i); continue; }

                    if (Is(i, "from") || Is(i, "join") || (inFrom && depth == 0 && Punct(i, ',')))
                    {
                        inFrom = true;
                        int next;
                        string name = TableName(i + 1, out next);
                        if (name == null || NotAnAlias.Contains(name)) continue;
                        string alias = null;
                        if (Is(next, "as")) next++;
                        string candidate = Word(next);
                        if (candidate != null && !NotAnAlias.Contains(candidate)) { alias = candidate; next++; }
                        Refer(name, alias);
                        i = next - 1;
                        continue;
                    }

                    // alias.column
                    if (_t[i].IsWord && Punct(i + 1, '.') && Word(i + 2) != null && !Punct(i - 1, '.'))
                        _references.Add(new KeyValuePair<string, string>(Word(i), Word(i + 2)));
                }

                foreach (var reference in _references)
                {
                    Table table;
                    if (Statement.Names.TryGetValue(reference.Key, out table)) table.Add(reference.Value);
                }
            }

            private int Create(int i)
            {
                int j = i + 1;
                while (j < _t.Count && j < i + 4 && !Is(j, "table")) j++;
                if (!Is(j, "table")) return i;
                j++;
                if (Is(j, "if") && Is(j + 1, "not") && Is(j + 2, "exists")) j += 3;
                int next;
                string name = TableName(j, out next);
                if (name == null) return i;
                Table table = _schema.Get(name);
                table.Declared = true; table.Uses++;
                if (!Punct(next, '(')) return next - 1;
                // the entries at depth 1: the first word of each is a column unless it starts a constraint
                int depth = 0, k = next;
                bool entryStart = false;
                for (; k < _t.Count; k++)
                {
                    if (Punct(k, '(')) { depth++; if (depth == 1) entryStart = true; continue; }
                    if (Punct(k, ')')) { depth--; if (depth == 0) break; continue; }
                    if (depth == 1 && Punct(k, ',')) { entryStart = true; continue; }
                    if (depth == 1 && entryStart)
                    {
                        entryStart = false;
                        string column = Word(k);
                        if (column != null && !NotAColumn.Contains(column)) table.Add(column);
                    }
                }
                return k;
            }

            private int Alter(int i)
            {
                int next;
                string name = TableName(i + 2, out next);
                if (name == null) return i;
                Refer(name, null);
                for (int j = next; j < _t.Count; j++)
                {
                    if (Is(j, "add"))
                    {
                        int c = j + 1;
                        if (Is(c, "column")) c++;
                        string column = Word(c);
                        if (column != null && !NotAColumn.Contains(column)) _schema.Get(name).Add(column);
                    }
                }
                return _t.Count;
            }

            private int Insert(int i)
            {
                int next;
                string name = TableName(i + 2, out next);
                if (name == null) return i + 1;
                Refer(name, null);
                if (!Punct(next, '(')) return next - 1;
                Table table = _schema.Get(name);
                int k = next + 1;
                for (; k < _t.Count && !Punct(k, ')'); k++)
                {
                    string column = Word(k);
                    if (column != null && !(Punct(k - 1, '.')) && (Punct(k - 1, '(') || Punct(k - 1, ','))) table.Add(column);
                }
                return k;
            }

            private int Update(int i)
            {
                int next;
                string name = TableName(i + 1, out next);
                if (name == null || NotAnAlias.Contains(name)) return i;
                string alias = null;
                if (Is(next, "as")) next++;
                string candidate = Word(next);
                if (candidate != null && !NotAnAlias.Contains(candidate)) { alias = candidate; next++; }
                Refer(name, alias);
                Table table = _schema.Get(name);
                if (Is(next, "set"))
                {
                    int depth = 0;
                    for (int k = next + 1; k < _t.Count; k++)
                    {
                        if (Punct(k, '(')) depth++;
                        else if (Punct(k, ')')) { if (depth > 0) depth--; }
                        if (depth == 0 && Is(k, "where")) break;
                        string column = Word(k);
                        if (column != null && depth == 0 && Punct(k + 1, '=') && (Is(k - 1, "set") || Punct(k - 1, ',') || Punct(k - 1, '.')))
                            table.Add(column);
                    }
                }
                return next - 1;
            }
        }

        // ---- the document and the statement under the caret ----------------------------------------------------------------

        /// <summary>
        /// Reads the strings of the language, given as sorted [start, end) pairs (see the engine), inside [from, to), each statement
        /// on its own. The word at [caretStart, caretEnd) is the one being typed and is left out; <paramref name="current"/> is what the
        /// statement under the caret refers to.
        /// </summary>
        public static SqlSchema Read(string text, int[] ranges, int from, int to, int caretStart, int caretEnd, out Statement current)
        {
            var schema = new SqlSchema();
            current = new Statement();
            for (int r = 0; r + 1 < ranges.Length; r += 2)
            {
                int start = Math.Max(ranges[r], from), end = Math.Min(ranges[r + 1], to);
                if (start >= end) continue;
                foreach (var statement in Split(text, start, end))
                {
                    var reader = new Reader(text, schema, Tokenize(text, statement.Key, statement.Value, caretStart, caretEnd));
                    reader.Read();
                    if (caretStart >= statement.Key && caretStart <= statement.Value) current = reader.Statement;
                }
            }
            return schema;
        }

        /// <summary>The statements of [start, end): cut at the semicolons that are not inside a literal or a comment.</summary>
        private static List<KeyValuePair<int, int>> Split(string text, int start, int end)
        {
            var result = new List<KeyValuePair<int, int>>();
            int begin = start, i = start;
            while (i < end)
            {
                char c = text[i];
                if (c == '\'' || c == '"' || c == '`') { int close = text.IndexOf(c, i + 1); i = close < 0 || close >= end ? end : close + 1; continue; }
                if (c == '-' && i + 1 < end && text[i + 1] == '-') { while (i < end && text[i] != '\n') i++; continue; }
                if (c == '/' && i + 1 < end && text[i + 1] == '*') { int close = text.IndexOf("*/", i + 2, StringComparison.Ordinal); i = close < 0 || close >= end ? end : close + 2; continue; }
                if (c == ';') { result.Add(new KeyValuePair<int, int>(begin, i)); begin = i + 1; }
                i++;
            }
            if (begin < end) result.Add(new KeyValuePair<int, int>(begin, end));
            return result;
        }
    }
}
