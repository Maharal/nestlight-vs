using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NestLight.Completion
{
    /// <summary>
    /// What the grammar of the language says about the place of the caret: the words that belong there come first, the words that
    /// do not belong come last or are not offered. The places are told apart with a few characters of look-behind, not a parser.
    /// </summary>
    internal sealed class Position
    {
        /// <summary>A name for the place, for tests and reports (<c>sql:table</c>, <c>css:value:display</c>).</summary>
        public readonly string Name;
        /// <summary>Offered first, in this order, even when the vocabulary of the language does not have them (the tags in a selector).</summary>
        public readonly IReadOnlyList<string> Expected;
        /// <summary>Offered after the words of the document when <see cref="WordsFirst"/> is set, right after <see cref="Expected"/> otherwise (the functions of SQL).</summary>
        public readonly IReadOnlyList<string> Secondary;
        /// <summary>The words of the document (a table, a column, a class) go before the rest of the keywords.</summary>
        public readonly bool WordsFirst;
        /// <summary>The keywords that do not belong here go after the words of the document; null: none.</summary>
        public readonly Func<string, bool> Unlikely;
        /// <summary>The keywords of the language do not belong here at all (a class name, a JSON key, a string): only the lists above and the words of the document are offered.</summary>
        public readonly bool OnlyWords;
        /// <summary>The list in <see cref="Expected"/> is long and alphabetical (the properties of CSS): the most used come first.</summary>
        public readonly bool PriorOrder;
        /// <summary>What the place asks of the schema of the document (SQL only): a table, a column of the tables in the statement, a member of a qualifier.</summary>
        public readonly PlaceRole Role;
        /// <summary>For <see cref="PlaceRole.Member"/>, the word before the dot (an alias or a table).</summary>
        public readonly string Qualifier;

        public Position(string name, IEnumerable<string> expected = null, bool wordsFirst = false, Func<string, bool> unlikely = null,
            PlaceRole role = PlaceRole.None, string qualifier = null, bool onlyWords = false, IEnumerable<string> secondary = null, bool priorOrder = false)
        {
            Name = name;
            Expected = expected == null ? new string[0] : expected as IReadOnlyList<string> ?? expected.ToList(); // a list that is already one is kept (the order by use is cached by it)
            Secondary = secondary == null ? new string[0] : secondary as IReadOnlyList<string> ?? secondary.ToList();
            WordsFirst = wordsFirst;
            Unlikely = unlikely;
            Role = role;
            Qualifier = qualifier;
            OnlyWords = onlyWords;
            PriorOrder = priorOrder;
        }
    }

    internal enum PlaceRole { None, Table, Column, Member }

    internal static class Positions
    {
        /// <summary>How far back from the caret the place is looked for.</summary>
        private const int Reach = 4000;

        private static readonly Func<string, bool> Always = w => true;

        /// <summary>The place of the caret, or null when the language has no grammar here (or the place says nothing).</summary>
        public static Position At(string text, CompletionSite site)
        {
            if (text == null || site == null || site.Start > text.Length) return null;
            int floor = Math.Max(site.OwnerStart >= 0 ? site.OwnerStart : 0, site.Start - Reach);
            if (Vocabularies.SameLanguage(site.EmbeddedLanguageId, "sql")) return Sql(text, floor, site);
            if (Vocabularies.SameLanguage(site.EmbeddedLanguageId, "css")) return Css(text, floor, site);
            if (Vocabularies.SameLanguage(site.EmbeddedLanguageId, "html")) return Html(text, floor, site);
            if (Vocabularies.SameLanguage(site.EmbeddedLanguageId, "json")) return Json(text, floor, site);
            if (Vocabularies.SameLanguage(site.EmbeddedLanguageId, "yaml")) return Yaml(text, floor, site);
            if (Vocabularies.SameLanguage(site.EmbeddedLanguageId, "glsl")) return Glsl(text, floor, site);
            if (Vocabularies.SameLanguage(site.EmbeddedLanguageId, "wgsl")) return Wgsl(text, floor, site);
            return null;
        }

        private static bool IsWordChar(char c) { return char.IsLetterOrDigit(c) || c == '_'; }

        // ---- SQL ------------------------------------------------------------------------------------------------------

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

        internal static Position Sql(string text, int floor, CompletionSite site)
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
            if (!isWord || Vocabularies.Find("sql", last) == null || tail == ')')
            {
                string[] continuation;
                switch (clause)
                {
                    case "select": continuation = AfterSelectList; break;
                    case "from": continuation = AfterFrom; break;
                    case "join": continuation = AfterJoin; break;
                    case "update": continuation = AfterUpdate; break;
                    case "set": continuation = AfterSet; break;
                    case "limit": continuation = AfterLimit; break;
                    case "into": continuation = AfterInto; break;
                    case "group": continuation = AfterGroup; break;
                    case "order": continuation = AfterOrder; break;
                    case "where": case "on": case "having": continuation = AfterCondition; break;
                    default: return null;
                }
                return new Position("sql:continue-" + clause, continuation);
            }
            return null;
        }

        // ---- CSS ------------------------------------------------------------------------------------------------------

        private enum CssBlock { Rules, Keyframes, Declarations }

        private static readonly string[] CssRuleAtRules = { "@media", "@supports", "@layer", "@container", "@document" };

        internal static Position Css(string text, int floor, CompletionSite site)
        {
            var blocks = new List<CssBlock>(); // innermost last; none: the top level
            if (site.InlineDeclarations) blocks.Add(CssBlock.Declarations); // the value of a style attribute starts inside a rule
            int declarationStart = floor, colon = -1, parens = 0, brackets = 0;
            int i = floor;
            while (i < site.Start)
            {
                char c = text[i];
                if (c == '/' && i + 1 < site.Start && text[i + 1] == '*')
                {
                    int close = text.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    if (close < 0 || close >= site.Start) return new Position("css:comment", onlyWords: true);
                    i = close + 2;
                    continue;
                }
                if (c == '"' || c == '\'')
                {
                    int close = text.IndexOf(c, i + 1);
                    if (close < 0 || close >= site.Start) return new Position("css:string", onlyWords: true);
                    i = close + 1;
                    continue;
                }
                if (c == '(') parens++;
                else if (c == ')') { if (parens > 0) parens--; }
                else if (c == '[') brackets++;
                else if (c == ']') { if (brackets > 0) brackets--; }
                else if (parens == 0 && brackets == 0)
                {
                    CssBlock top = blocks.Count > 0 ? blocks[blocks.Count - 1] : CssBlock.Rules;
                    if (c == '{')
                    {
                        string prelude = text.Substring(declarationStart, i - declarationStart).Trim();
                        CssBlock kind = CssBlock.Declarations;
                        if (prelude.StartsWith("@", StringComparison.Ordinal))
                        {
                            if (prelude.IndexOf("keyframes", StringComparison.OrdinalIgnoreCase) >= 0) kind = CssBlock.Keyframes;
                            else if (CssRuleAtRules.Any(a => prelude.StartsWith(a, StringComparison.OrdinalIgnoreCase))) kind = CssBlock.Rules;
                        }
                        blocks.Add(kind); declarationStart = i + 1; colon = -1;
                    }
                    else if (c == '}') { if (blocks.Count > 0) blocks.RemoveAt(blocks.Count - 1); declarationStart = i + 1; colon = -1; }
                    else if (c == ';') { declarationStart = i + 1; colon = -1; }
                    else if (c == ':' && top == CssBlock.Declarations && colon < 0) colon = i;
                }
                i++;
            }

            CssBlock inside = blocks.Count > 0 ? blocks[blocks.Count - 1] : CssBlock.Rules;
            string pending = text.Substring(declarationStart, site.Start - declarationStart).TrimStart();

            if (parens > 0)
            {
                // the condition of a media query; anywhere else (url( ... ), calc( ... )) the place says nothing
                if (pending.StartsWith("@", StringComparison.Ordinal))
                {
                    if (char.IsDigit(site.Start > floor ? text[site.Start - 1] : ' ')) return new Position("css:unit", Vocabularies.CssUnits, onlyWords: true);
                    if (pending.StartsWith("@supports", StringComparison.OrdinalIgnoreCase)) return new Position("css:supports-feature", Vocabularies.CssProperties);
                    if (pending.StartsWith("@container", StringComparison.OrdinalIgnoreCase)) return new Position("css:container-feature", Vocabularies.CssContainerFeatures);
                    return new Position("css:media-feature", Vocabularies.CssMediaFeatures);
                }
                if (char.IsDigit(site.Start > floor ? text[site.Start - 1] : ' '))
                    return inside == CssBlock.Declarations ? new Position("css:unit", Vocabularies.CssUnits, onlyWords: true) : null; // rotate(45d|), not 2|n in :nth-child(2n)
                return null;
            }
            if (brackets > 0) return new Position("css:attribute-selector", Vocabularies.CssSelectorAttributes, onlyWords: true);

            if (char.IsDigit(site.Start > floor ? text[site.Start - 1] : ' ') && inside != CssBlock.Declarations) return null; // 50|% in a keyframe, 2|n in a selector: no word to complete
            if (inside == CssBlock.Keyframes)
                return new Position("css:keyframe-selector", new[] { "from", "to" }, onlyWords: true);

            if (pending.StartsWith("@", StringComparison.Ordinal))
            {
                int space = pending.IndexOfAny(new[] { ' ', '\t', '\n', '\r' });
                string name = space < 0 ? pending : pending.Substring(0, space);
                if (space < 0) return new Position("css:at-rule", Vocabularies.CssAtRules, onlyWords: true); // the name of the at-rule itself
                if (name.IndexOf("keyframes", StringComparison.OrdinalIgnoreCase) >= 0) return new Position("css:keyframes-name", onlyWords: true);
                if (name.Equals("@media", StringComparison.OrdinalIgnoreCase)) return new Position("css:media-query", Vocabularies.CssMediaTypes);
                return null;
            }

            char before = site.Start > floor ? text[site.Start - 1] : ' ';
            if (char.IsDigit(before) && inside == CssBlock.Declarations) return new Position("css:unit", Vocabularies.CssUnits, onlyWords: true); // 10px: the unit after a number
            if (inside == CssBlock.Rules)
            {
                // a selector: a class or an id after '.' or '#', a pseudo-class after ':', otherwise an element
                if (before == '.' || before == '#') return new Position("css:class", onlyWords: true);
                if (before == ':')
                {
                    bool element = site.Start - 2 >= floor && text[site.Start - 2] == ':';
                    return element
                        ? new Position("css:pseudo-element", Vocabularies.CssPseudoElements, onlyWords: true)
                        : new Position("css:pseudo-class", Vocabularies.CssPseudoClasses, onlyWords: true);
                }
                return new Position("css:selector", Vocabularies.For("html"), unlikely: w => Vocabularies.Find("css", w) != null && Vocabularies.Find("html", w) == null);
            }

            if (colon < 0)
                return new Position("css:property", Vocabularies.CssProperties, unlikely: w => Vocabularies.IsCssValueOnly(w), priorOrder: true);

            if (before == '#') return new Position("css:hex", onlyWords: true);
            if (before == '!') return new Position("css:important", new[] { "important" }, onlyWords: true);

            // a value: the property is the word before the colon
            int end = colon;
            while (end > declarationStart && char.IsWhiteSpace(text[end - 1])) end--;
            int start = end;
            while (start > declarationStart && (IsWordChar(text[start - 1]) || text[start - 1] == '-')) start--;
            string property = text.Substring(start, end - start).ToLowerInvariant();
            IReadOnlyList<string> values = Vocabularies.CssValuesOf(property);
            return new Position("css:value:" + property, values, unlikely: w => !Vocabularies.IsCssValueOnly(w) && !values.Contains(w, StringComparer.OrdinalIgnoreCase),
                secondary: Vocabularies.CssFunctionsFor(property));
        }

        // ---- HTML -----------------------------------------------------------------------------------------------------

        private static readonly HashSet<string> VoidElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "source", "track", "wbr", "path", "circle", "rect", "line", "ellipse", "polygon", "polyline", "stop", "use" };

        internal static Position Html(string text, int floor, CompletionSite site)
        {
            // the last '<' or '>' before the caret says whether it is in a tag
            int open = -1;
            for (int i = site.Start - 1; i >= floor; i--)
            {
                if (text[i] == '>') break;
                if (text[i] == '<') { open = i; break; }
            }
            if (open < 0)
                return new Position("html:text", onlyWords: true); // text between tags: not a tag name unless '<' was typed

            int nameStart = open + 1;
            bool closing = nameStart < site.Start && text[nameStart] == '/';
            if (closing) nameStart++;
            if (nameStart == site.Start)
            {
                // right after '<': the vocabulary is the tags; after '</': the element that is open first
                if (!closing) return new Position("html:tag");
                string openTag = OpenElement(text, floor, open);
                return openTag == null ? new Position("html:tag") : new Position("html:closing-tag", new[] { openTag });
            }

            // inside a tag: its name, then the attribute being written or the value of one
            int nameEnd = nameStart;
            while (nameEnd < site.Start && (IsWordChar(text[nameEnd]) || text[nameEnd] == '-' || text[nameEnd] == ':')) nameEnd++;
            if (nameEnd == site.Start)
            {
                if (!closing) return new Position("html:tag");
                string openTag = OpenElement(text, floor, open);
                return openTag == null ? new Position("html:tag") : new Position("html:closing-tag", new[] { openTag });
            }
            if (closing) return null;
            string tag = text.Substring(nameStart, nameEnd - nameStart).ToLowerInvariant();

            char quote = '\0';
            int attribute = -1;
            for (int i = nameEnd; i < site.Start; i++)
            {
                char c = text[i];
                if (quote != '\0') { if (c == quote) quote = '\0'; continue; }
                if (c == '"' || c == '\'') quote = c;
                else if (c == '=') attribute = i;
                else if (char.IsWhiteSpace(c) && attribute >= 0 && i > attribute + 1) attribute = -1; // an unquoted value ended
            }
            if (quote != '\0' || (attribute >= 0 && attribute == LastNonSpace(text, site.Start)))
            {
                // the value of an attribute: the attribute is the word before '='; a tag name never belongs here
                int end = attribute;
                if (attribute < 0) return new Position("html:value", onlyWords: true);
                while (end > nameEnd && char.IsWhiteSpace(text[end - 1])) end--;
                int start = end;
                while (start > nameEnd && (IsWordChar(text[start - 1]) || text[start - 1] == '-')) start--;
                string name = text.Substring(start, end - start).ToLowerInvariant();
                return new Position("html:value:" + name, Vocabularies.HtmlValuesOf(tag, name), wordsFirst: true, onlyWords: true);
            }

            // an attribute name: the ones the tag does not have yet
            HashSet<string> present = AttributesOf(text, nameEnd, site);
            return new Position("html:attribute:" + tag, Vocabularies.HtmlAttributesOf(tag).Where(a => !present.Contains(a)), onlyWords: true);
        }

        /// <summary>The attribute names written in the tag that contains the caret, before it and after it (the word being typed is not one).</summary>
        private static HashSet<string> AttributesOf(string text, int from, CompletionSite site)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            char quote = '\0';
            int i = from;
            int limit = Math.Min(text.Length, site.Start + Reach);
            while (i < limit)
            {
                if (i == site.Start) i = site.End; // skip the word under the caret
                if (i >= limit) break;
                char c = text[i];
                if (quote != '\0') { if (c == quote) quote = '\0'; i++; continue; }
                if (c == '"' || c == '\'') { quote = c; i++; continue; }
                if (c == '>' || c == '<') break;
                if (IsWordChar(c) || c == '-')
                {
                    int start = i;
                    while (i < limit && (IsWordChar(text[i]) || text[i] == '-' || text[i] == ':')) i++;
                    // an attribute name is not the unquoted value that follows '='
                    int p = start - 1;
                    while (p >= from && char.IsWhiteSpace(text[p])) p--;
                    if (!(p >= from && text[p] == '=')) names.Add(text.Substring(start, i - start));
                    continue;
                }
                i++;
            }
            return names;
        }

        /// <summary>The innermost element that is open at <paramref name="at"/>, found by reading the tags back to <paramref name="floor"/>; null when none is.</summary>
        private static string OpenElement(string text, int floor, int at)
        {
            var stack = new List<string>();
            int i = floor;
            while (i < at)
            {
                int lt = text.IndexOf('<', i, at - i);
                if (lt < 0) break;
                if (lt + 3 < text.Length && string.CompareOrdinal(text, lt, "<!--", 0, 4) == 0)
                {
                    int endComment = text.IndexOf("-->", lt + 4, StringComparison.Ordinal);
                    i = endComment < 0 || endComment >= at ? at : endComment + 3;
                    continue;
                }
                int p = lt + 1;
                bool closes = p < at && text[p] == '/';
                if (closes) p++;
                int nameStart = p;
                while (p < at && (IsWordChar(text[p]) || text[p] == '-' || text[p] == ':')) p++;
                if (p == nameStart) { i = lt + 1; continue; }
                string name = text.Substring(nameStart, p - nameStart);

                // the end of the tag, past quoted values
                char quote = '\0';
                int gt = -1;
                for (int k = p; k < at; k++)
                {
                    char c = text[k];
                    if (quote != '\0') { if (c == quote) quote = '\0'; continue; }
                    if (c == '"' || c == '\'') quote = c;
                    else if (c == '>') { gt = k; break; }
                }
                if (gt < 0) break;
                bool selfClosing = text[gt - 1] == '/';
                if (closes)
                {
                    int index = stack.FindLastIndex(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase));
                    if (index >= 0) stack.RemoveRange(index, stack.Count - index);
                }
                else if (!selfClosing && !VoidElements.Contains(name)) stack.Add(name);
                i = gt + 1;
            }
            return stack.Count == 0 ? null : stack[stack.Count - 1];
        }

        private static int LastNonSpace(string text, int before)
        {
            int i = before - 1;
            while (i >= 0 && char.IsWhiteSpace(text[i])) i--;
            return i;
        }

        // ---- shaders ------------------------------------------------------------------------------------------------------

        private static readonly string[] GlslDirectives = { "version", "define", "ifdef", "ifndef", "if", "else", "elif", "endif", "extension", "pragma", "undef", "include" };
        private static readonly string[] WgslAttributes = { "location", "builtin", "group", "binding", "vertex", "fragment", "compute", "workgroup_size", "size", "align", "id", "interpolate", "invariant", "must_use", "diagnostic" };
        private static readonly string[] WgslBuiltinValues =
            { "position", "vertex_index", "instance_index", "front_facing", "frag_depth", "local_invocation_id", "local_invocation_index", "global_invocation_id", "workgroup_id", "num_workgroups", "sample_index", "sample_mask" };
        private static readonly string[] WgslAddressSpaces = { "function", "private", "workgroup", "uniform", "storage", "handle" };
        private static readonly string[] WgslAccessModes = { "read", "write", "read_write" };

        /// <summary>The start of the line the offset is on, not before <paramref name="floor"/>.</summary>
        private static int LineStart(string text, int offset, int floor)
        {
            int i = offset;
            while (i > floor && text[i - 1] != '\n') i--;
            return i;
        }

        internal static Position Glsl(string text, int floor, CompletionSite site)
        {
            int lineStart = LineStart(text, site.Start, floor);
            string line = text.Substring(lineStart, site.Start - lineStart).TrimStart();
            if (line.StartsWith("#", StringComparison.Ordinal))
            {
                string rest = line.Substring(1).TrimStart();
                if (rest.IndexOf(' ') < 0 && rest.IndexOf('\t') < 0) return new Position("glsl:directive", GlslDirectives, onlyWords: true);
                if (rest.StartsWith("version", StringComparison.Ordinal)) return new Position("glsl:version", new[] { "es", "core", "compatibility" }, onlyWords: true);
            }
            return null;
        }

        internal static Position Wgsl(string text, int floor, CompletionSite site)
        {
            if (site.Start > floor && text[site.Start - 1] == '@') return new Position("wgsl:attribute", WgslAttributes, onlyWords: true);
            int lineStart = LineStart(text, site.Start, floor);
            string line = text.Substring(lineStart, site.Start - lineStart);

            int builtin = line.LastIndexOf("@builtin(", StringComparison.Ordinal);
            if (builtin >= 0 && line.IndexOf(')', builtin) < 0) return new Position("wgsl:builtin-value", WgslBuiltinValues, onlyWords: true);
            int interpolate = line.LastIndexOf("@interpolate(", StringComparison.Ordinal);
            if (interpolate >= 0 && line.IndexOf(')', interpolate) < 0)
                return line.IndexOf(',', interpolate) < 0
                    ? new Position("wgsl:interpolate-type", new[] { "perspective", "linear", "flat" }, onlyWords: true)
                    : new Position("wgsl:interpolate-sampling", new[] { "center", "centroid", "sample", "first", "either" }, onlyWords: true);

            int declaration = Math.Max(line.LastIndexOf("var<", StringComparison.Ordinal), line.LastIndexOf("ptr<", StringComparison.Ordinal));
            if (declaration >= 0 && line.IndexOf('>', declaration + 4) < 0)
                return line.IndexOf(',', declaration) < 0
                    ? new Position("wgsl:address-space", WgslAddressSpaces, onlyWords: true)
                    : new Position("wgsl:access-mode", WgslAccessModes, onlyWords: true);
            return null;
        }

        // ---- JSON and YAML ----------------------------------------------------------------------------------------------

        internal static Position Json(string text, int floor, CompletionSite site)
        {
            // a key is a string right after '{' or ',' inside an object; the keywords (true, false, null) are values
            var stack = new List<char>();
            bool inString = false, isKey = false;
            char previous = ' ';
            for (int i = floor; i < site.Start; i++)
            {
                char c = text[i];
                if (inString)
                {
                    if (c == '\\') i++;
                    else if (c == '"') { inString = false; previous = '"'; }
                    continue;
                }
                if (c == '"') { inString = true; isKey = stack.Count > 0 && stack[stack.Count - 1] == '{' && (previous == '{' || previous == ','); continue; }
                if (char.IsWhiteSpace(c)) continue;
                if (c == '{' || c == '[') stack.Add(c);
                else if ((c == '}' || c == ']') && stack.Count > 0) stack.RemoveAt(stack.Count - 1);
                previous = c;
            }
            if (inString) return new Position(isKey ? "json:key" : "json:string", onlyWords: true);
            return null; // a value outside a string: true, false and null belong there
        }

        internal static Position Yaml(string text, int floor, CompletionSite site)
        {
            int lineStart = site.Start;
            while (lineStart > floor && text[lineStart - 1] != '\n') lineStart--;
            string line = text.Substring(lineStart, site.Start - lineStart).TrimStart();
            while (line.StartsWith("- ", StringComparison.Ordinal)) line = line.Substring(2).TrimStart();

            char quote = '\0';
            bool value = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (quote != '\0') { if (c == quote) quote = '\0'; continue; }
                if (c == '"' || c == '\'') quote = c;
                else if (c == '#' && (i == 0 || char.IsWhiteSpace(line[i - 1]))) return new Position("yaml:comment", onlyWords: true);
                else if (c == ':' && (i + 1 == line.Length || char.IsWhiteSpace(line[i + 1]))) value = true; // after "key: ": true, false, yes, no are possible
            }
            if (quote != '\0') return new Position("yaml:string", onlyWords: true);
            return value ? null : new Position("yaml:key", onlyWords: true);
        }
    }
}
