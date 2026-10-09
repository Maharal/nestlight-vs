using System;
using System.Collections.Generic;
using System.Linq;

namespace NestLight.Completion
{
    /// <summary>
    /// What the grammar of the language says about the place of the caret: the words that belong there come first, the words that
    /// do not belong come last. Nothing is dropped; the places are told apart with a few characters of look-behind, not a parser.
    /// </summary>
    internal sealed class Position
    {
        /// <summary>A name for the place, for tests and reports (<c>sql:table</c>, <c>css:value:display</c>).</summary>
        public readonly string Name;
        /// <summary>Offered first, in this order, even when the vocabulary of the language does not have them (the tags in a selector).</summary>
        public readonly IReadOnlyList<string> Expected;
        /// <summary>The words of the document (a table, a column, a class) go before the rest of the keywords.</summary>
        public readonly bool WordsFirst;
        /// <summary>The keywords that do not belong here go after the words of the document; null: none.</summary>
        public readonly Func<string, bool> Unlikely;
        /// <summary>What the place asks of the schema of the document (SQL only): a table, a column of the tables in the statement, a member of a qualifier.</summary>
        public readonly PlaceRole Role;
        /// <summary>For <see cref="PlaceRole.Member"/>, the word before the dot (an alias or a table).</summary>
        public readonly string Qualifier;

        public Position(string name, IEnumerable<string> expected = null, bool wordsFirst = false, Func<string, bool> unlikely = null,
            PlaceRole role = PlaceRole.None, string qualifier = null)
        {
            Name = name;
            Expected = expected == null ? new string[0] : expected.ToList();
            WordsFirst = wordsFirst;
            Unlikely = unlikely;
            Role = role;
            Qualifier = qualifier;
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
            if (Vocabularies.SameLanguage(site.LanguageId, "sql")) return Sql(text, floor, site);
            if (Vocabularies.SameLanguage(site.LanguageId, "css")) return Css(text, floor, site);
            if (Vocabularies.SameLanguage(site.LanguageId, "html")) return Html(text, floor, site);
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
        };

        // after these the next word is a table
        private static readonly HashSet<string> SqlTableAfter = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "from", "join", "into", "update", "table", "truncate" };
        // after these the next word is an expression: a column, a function, a value
        private static readonly HashSet<string> SqlExpressionAfter = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "select", "where", "and", "or", "on", "having", "set", "by", "when", "then", "else", "case", "using", "limit", "offset", "values", "between", "like", "in", "exists", "returning", "distinct" };
        private static readonly HashSet<string> SqlClauses = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "select", "from", "where", "group", "order", "having", "set", "values", "into", "update", "join", "on", "limit" };

        private static Position Sql(string text, int floor, CompletionSite site)
        {
            // read the statement up to the caret: the last two words, what lies between the last one and the caret, the clause
            string last = null, clause = null;
            var between = new List<char>();
            int i = floor;
            while (i < site.Start)
            {
                char c = text[i];
                if (c == '\'' || c == '"' || c == '`')
                {
                    // a literal or a quoted name counts as an operand
                    int close = text.IndexOf(c, i + 1);
                    i = close < 0 || close >= site.Start ? site.Start : close + 1;
                    last = "\u0001"; between.Clear();
                    continue;
                }
                if (c == '-' && i + 1 < site.Start && text[i + 1] == '-') { while (i < site.Start && text[i] != '\n') i++; continue; }
                if (c == '/' && i + 1 < site.Start && text[i + 1] == '*')
                {
                    int close = text.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    i = close < 0 || close >= site.Start ? site.Start : close + 2;
                    continue;
                }
                if (c == ';') { last = null; clause = null; between.Clear(); i++; continue; }
                if (IsWordChar(c))
                {
                    int start = i;
                    while (i < site.Start && IsWordChar(text[i])) i++;
                    last = text.Substring(start, i - start);
                    if (SqlClauses.Contains(last)) clause = last.ToLowerInvariant();
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
            if (last == null) return new Position("sql:statement", SqlStarters);
            char tail = between.Count > 0 ? between[between.Count - 1] : ' ';
            if (tail == '(' || tail == ',' || tail == '=' || tail == '<' || tail == '>' || tail == '+' || tail == '-' || tail == '*' || tail == '/')
                return new Position("sql:expression", wordsFirst: true, role: PlaceRole.Column);

            string[] next;
            if (last != "\u0001" && SqlNext.TryGetValue(last, out next))
                return new Position("sql:after-" + last.ToLowerInvariant(), next, wordsFirst: last.Equals("select", StringComparison.OrdinalIgnoreCase),
                    role: last.Equals("select", StringComparison.OrdinalIgnoreCase) ? PlaceRole.Column : PlaceRole.None);
            if (last != "\u0001" && SqlTableAfter.Contains(last)) return new Position("sql:table", wordsFirst: true, unlikely: Always, role: PlaceRole.Table);
            if (last != "\u0001" && SqlExpressionAfter.Contains(last)) return new Position("sql:expression", wordsFirst: true, role: PlaceRole.Column);

            // after a column, a table, a literal or a closing parenthesis: the word that continues the clause
            if (last == "\u0001" || Vocabularies.Find("sql", last) == null || tail == ')')
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
                    case "order": case "group": continuation = AfterOrder; break;
                    case "where": case "on": case "having": continuation = AfterCondition; break;
                    default: return null;
                }
                return new Position("sql:continue-" + clause, continuation);
            }
            return null;
        }

        // ---- CSS ------------------------------------------------------------------------------------------------------

        private static Position Css(string text, int floor, CompletionSite site)
        {
            int depth = 0, declarationStart = floor, colon = -1, parens = 0;
            int i = floor;
            while (i < site.Start)
            {
                char c = text[i];
                if (c == '/' && i + 1 < site.Start && text[i + 1] == '*')
                {
                    int close = text.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    i = close < 0 || close >= site.Start ? site.Start : close + 2;
                    continue;
                }
                if (c == '"' || c == '\'')
                {
                    int close = text.IndexOf(c, i + 1);
                    i = close < 0 || close >= site.Start ? site.Start : close + 1;
                    continue;
                }
                if (c == '(') parens++;
                else if (c == ')') { if (parens > 0) parens--; }
                else if (parens == 0)
                {
                    if (c == '{') { depth++; declarationStart = i + 1; colon = -1; }
                    else if (c == '}') { if (depth > 0) depth--; declarationStart = i + 1; colon = -1; }
                    else if (c == ';') { declarationStart = i + 1; colon = -1; }
                    else if (c == ':' && depth > 0 && colon < 0) colon = i;
                }
                i++;
            }
            if (parens > 0) return null; // inside url( ... ) or calc( ... )

            if (depth == 0)
                return new Position("css:selector", Vocabularies.For("html"), unlikely: w => Vocabularies.Find("css", w) != null && Vocabularies.Find("html", w) == null);

            if (colon < 0)
                return new Position("css:property", Vocabularies.CssProperties, unlikely: w => Vocabularies.IsCssValueOnly(w));

            // a value: the property is the word before the colon
            int end = colon;
            while (end > declarationStart && char.IsWhiteSpace(text[end - 1])) end--;
            int start = end;
            while (start > declarationStart && (IsWordChar(text[start - 1]) || text[start - 1] == '-')) start--;
            string property = text.Substring(start, end - start).ToLowerInvariant();
            IReadOnlyList<string> values = Vocabularies.CssValuesOf(property);
            return new Position("css:value:" + property, values, unlikely: w => !Vocabularies.IsCssValueOnly(w) && !values.Contains(w, StringComparer.OrdinalIgnoreCase));
        }

        // ---- HTML -----------------------------------------------------------------------------------------------------

        private static Position Html(string text, int floor, CompletionSite site)
        {
            // the last '<' or '>' before the caret says whether it is in a tag
            int open = -1;
            for (int i = site.Start - 1; i >= floor; i--)
            {
                if (text[i] == '>') break;
                if (text[i] == '<') { open = i; break; }
            }
            if (open < 0)
                return new Position("html:text", wordsFirst: true, unlikely: Always); // text between tags: not a tag name unless '<' was typed

            int nameStart = open + 1;
            if (nameStart < site.Start && text[nameStart] == '/') nameStart++;
            if (nameStart == site.Start) return new Position("html:tag"); // right after '<' (or '</'): the vocabulary is the tags

            // inside a tag: its name, then the attribute being written or the value of one
            int nameEnd = nameStart;
            while (nameEnd < site.Start && (IsWordChar(text[nameEnd]) || text[nameEnd] == '-' || text[nameEnd] == ':')) nameEnd++;
            if (nameEnd == site.Start) return new Position("html:tag"); // still typing the name
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
            if (quote != '\0' || (attribute >= 0 && attribute == LastNonSpace(text, site.Start) ))
            {
                // the value of an attribute: the attribute is the word before '='
                int end = attribute;
                if (attribute < 0) return new Position("html:value", wordsFirst: true, unlikely: Always);
                while (end > nameEnd && char.IsWhiteSpace(text[end - 1])) end--;
                int start = end;
                while (start > nameEnd && (IsWordChar(text[start - 1]) || text[start - 1] == '-')) start--;
                string name = text.Substring(start, end - start).ToLowerInvariant();
                return new Position("html:value:" + name, Vocabularies.HtmlValuesOf(tag, name), wordsFirst: true, unlikely: Always);
            }
            return new Position("html:attribute:" + tag, Vocabularies.HtmlAttributesOf(tag), unlikely: Always);
        }

        private static int LastNonSpace(string text, int before)
        {
            int i = before - 1;
            while (i >= 0 && char.IsWhiteSpace(text[i])) i--;
            return i;
        }
    }
}
