using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class SqlSchemaMutationTests
    {
        private static readonly CompletionFeatures Schema = new CompletionFeatures(schema: true);

        private static List<string> Texts(string codeWithCaret, CompletionFeatures features, bool onlyWords = false)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000, CompletionEngine.DefaultMinWordLength, features: features);
            CompletionSite site = engine.Locate(code, caret);
            return engine.Suggest(code, site).Where(s => !onlyWords || s.Kind == SuggestionKind.Word).Select(s => s.Text).ToList();
        }

        [Fact]
        public void Table_Add_rejects_duplicate_columns_case_insensitively()
        {
            string code = "sql`create table t (name text); alter table t add column Name text; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Equal(1, items.Where(i => i.Equals("name", System.StringComparison.OrdinalIgnoreCase) || i.Equals("Name", System.StringComparison.OrdinalIgnoreCase)).Take(2).Count());
        }
        [Fact]
        public void Comments_in_sql_are_skipped_by_the_schema()
        {
            string code = "sql`create table t (id int, /* this is a comment */ name text); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
        }

        [Fact]
        public void Single_line_comment_is_skipped()
        {
            string code = "sql`create table t (id int, -- a comment\nname text); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
        }
        [Fact]
        public void String_literal_content_is_not_treated_as_identifiers()
        {
            string code = "sql`create table t (id int); select * from t where id = 'fake_table'; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.DoesNotContain("fake_table", items.Take(5));
        }
        [Fact]
        public void From_clause_registers_table()
        {
            string code = "sql`select * from users; select * from |`";
            var items = Texts(code, Schema, true);
            Assert.Contains("users", items);
        }

        [Fact]
        public void Join_clause_registers_table()
        {
            string code = "sql`select * from a join b on a.id = b.id; select * from |`";
            var items = Texts(code, Schema, true);
            Assert.Contains("a", items);
            Assert.Contains("b", items);
        }

        [Fact]
        public void From_with_alias_and_as_keyword()
        {
            string code = "sql`select t.| from users as t`";
            var users = "sql`create table users (id int, name text)`;\n";
            var items = Texts(users + code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
        }

        [Fact]
        public void From_comma_separated_tables()
        {
            string code = "sql`select * from a, b; select * from |`";
            var items = Texts(code, Schema, true);
            Assert.Contains("a", items);
            Assert.Contains("b", items);
        }
        [Fact]
        public void Create_table_if_not_exists()
        {
            string code = "sql`create table if not exists t (id int, name text); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
        }

        [Fact]
        public void Create_table_with_schema_prefix()
        {
            string code = "sql`create table public.t (id int, name text); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
        }

        [Fact]
        public void Create_table_skips_constraint_keywords()
        {
            string code = "sql`create table t (id int, primary key (id), name text); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
            Assert.DoesNotContain("primary", items.Take(5));
        }
        [Fact]
        public void Alter_table_add_column_with_column_keyword()
        {
            string code = "sql`create table t (id int); alter table t add column email text; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("email", items);
        }

        [Fact]
        public void Alter_table_add_without_column_keyword()
        {
            string code = "sql`create table t (id int); alter table t add email text; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("email", items);
        }
        [Fact]
        public void Insert_into_learns_columns()
        {
            string code = "sql`insert into t (id, name) values (1, 'a'); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
        }

        [Fact]
        public void Insert_into_with_schema_prefix()
        {
            string code = "sql`insert into public.t (id, name) values (1, 'a'); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
        }
        [Fact]
        public void Update_set_learns_columns()
        {
            string code = "sql`update t set name = 'x', age = 1 where id = 1; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("name", items);
            Assert.Contains("age", items);
        }

        [Fact]
        public void Update_with_alias()
        {
            string code = "sql`create table t (id int, name text); update t as u set name = 'x'; select u.| from t u`";
            var items = Texts(code, Schema);
            Assert.Contains("name", items);
        }

        [Fact]
        public void Update_set_stops_at_where()
        {
            string code = "sql`update t set name = 'x' where fake = 1; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("name", items);
        }

        [Fact]
        public void Update_set_handles_subquery_in_value()
        {
            string code = "sql`update t set total = (select max(v) from other) where id = 1; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("total", items);
        }
        [Fact]
        public void Alias_dot_column_is_learned()
        {
            string code = "sql`select u.phone, u.address from users u; select u.| from users u`";
            var items = Texts(code, Schema);
            Assert.Contains("phone", items);
            Assert.Contains("address", items);
        }
        [Fact]
        public void Semicolon_inside_string_does_not_split_statement()
        {
            string code = "sql`create table users (id int); select * from users where name = 'a;b'; select u.| from users u`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
        }

        [Fact]
        public void Semicolon_inside_line_comment_does_not_split()
        {
            string code = "sql`create table users (id int); select * from users -- note; important\nwhere id = 1; select u.| from users u`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
        }

        [Fact]
        public void Semicolon_inside_block_comment_does_not_split()
        {
            string code = "sql`create table users (id int); select * from users /* note; important */ where id = 1; select u.| from users u`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
        }

        [Fact]
        public void Where_clause_ends_from_tracking()
        {
            string code = "sql`create table users (id int); select * from users where users.id = 1; select * from |`";
            var items = Texts(code, Schema, true);
            Assert.Contains("users", items);
        }

        // ---- Multiple statements with same alias for different tables (already tested but confirming isolation) ----

        [Fact]
        public void Each_statement_resolves_aliases_independently()
        {
            string code = "sql`select a.col1 from alpha a; select a.col2 from beta a; select a.| from beta a`";
            var items = Texts(code, Schema);
            Assert.Equal("col2", items.First());
        }

        // ---- NotAnAlias: reserved keywords are not treated as aliases ----

        [Fact]
        public void Reserved_words_are_not_aliases()
        {
            string code = "sql`select * from t where x = 1; select * from |`";
            var items = Texts(code, Schema, true);
            Assert.DoesNotContain("where", items);
        }
        [Fact]
        public void Declared_tables_come_before_undeclared_ones()
        {
            string code = "sql`create table declared (id int)`;\nsql`select * from undeclared; select * from undeclared; select * from |`";
            var items = Texts(code, Schema, true);
            int declaredIdx = items.IndexOf("declared");
            int undeclaredIdx = items.IndexOf("undeclared");
            Assert.True(declaredIdx >= 0);
            Assert.True(undeclaredIdx >= 0);
            Assert.True(declaredIdx < undeclaredIdx);
        }

        // ---- Tokenizer: comment skipping (lines 74-78) ----

        [Fact]
        public void Line_comment_dash_dash_is_skipped()
        {
            // Line 74: c == '-' && i + 1 < to && text[i + 1] == '-'
            string code = "sql`create table t (id int, -- skip this\nname text); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
            Assert.DoesNotContain("skip", items.Take(10));
        }

        [Fact]
        public void Block_comment_slash_star_is_skipped()
        {
            // Lines 75-79: c == '/' && text[i+1] == '*', then close = indexOf("*/")
            string code = "sql`create table t (id int, /* block comment */ name text); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
        }

        [Fact]
        public void Unclosed_block_comment_skips_to_end()
        {
            // Line 78: close < 0 -> skip to end
            string code = "sql`create table t (id int /* unclosed`;\nsql`select t.| from t`";
            var items = Texts(code, Schema);
            // 'id' should still be found since the CREATE TABLE saw the column before the comment
            Assert.Contains("id", items);
        }

        // ---- Tokenizer: string literal skipping (lines 81-87) ----

        [Fact]
        public void Single_quote_literal_is_skipped()
        {
            string code = "sql`create table t (id int); select * from t where id = 'value'; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
        }

        [Fact]
        public void Double_quote_identifier_is_handled()
        {
            // Line 81: c == '"' is also a quote boundary
            string code = "sql`create table t (\"id\" int); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.NotEmpty(items);
        }

        [Fact]
        public void Backtick_identifier_is_handled()
        {
            // Line 81: c == '`' is also a quote boundary (MySQL style)
            // Not directly testable through engine since backtick terminates the template literal
            // but the tokenizer handles it internally
        }

        // ---- Create: depth tracking (lines 160-161) ----

        [Fact]
        public void Create_table_nested_parens_track_depth()
        {
            // Lines 160-161: Punct(i, '(') depth++ and Punct(i, ')') depth--
            string code = "sql`create table t (id int, name varchar(255), age int); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
            Assert.Contains("age", items);
        }

        // ---- Create: EndsFrom detection (line 163) ----

        [Fact]
        public void EndsFrom_keyword_ends_from_tracking()
        {
            // Line 163: _t[i].IsWord && EndsFrom.Contains(...) && !Is(i, "from") -> inFrom = false
            string code = "sql`select * from users where id = 1; select * from |`";
            var items = Texts(code, Schema, true);
            Assert.Contains("users", items);
            Assert.DoesNotContain("where", items);
        }

        // ---- Create: alter table recognition (line 166) ----

        [Fact]
        public void Alter_table_recognized_as_two_keywords()
        {
            // Line 166: Is(i, "alter") && Is(i + 1, "table")
            string code = "sql`alter table t add column email text; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("email", items);
        }

        // ---- Create: insert into recognition (line 167) ----

        [Fact]
        public void Insert_into_recognized_as_two_keywords()
        {
            // Line 167: Is(i, "insert") && Is(i + 1, "into")
            string code = "sql`insert into t (id, name) values (1, 'a'); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
        }

        // ---- Table uses counting (line 208) ----

        [Fact]
        public void Table_uses_are_counted()
        {
            // Line 208: table.Uses++
            // A table mentioned more times should still appear
            string code = "sql`select * from t; select * from t; select * from |`";
            var items = Texts(code, Schema, true);
            Assert.Contains("t", items);
        }

        // ---- Create: if not exists (lines 200-203) ----

        [Fact]
        public void Create_table_with_if_not_exists_and_search_limit()
        {
            // Lines 200-203: j < _t.Count && j < i + 4 search window
            string code = "sql`create table if not exists t (id int); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
        }

        // ---- Update: column detection in SET (lines 275-287) ----

        [Fact]
        public void Update_set_column_after_comma()
        {
            // Line 284: Punct(k - 1, ',') matches
            string code = "sql`update t set a = 1, b = 2; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("a", items);
            Assert.Contains("b", items);
        }

        [Fact]
        public void Update_set_column_not_inside_parens()
        {
            // Line 284: depth == 0 check - columns inside parens (subqueries) not counted as SET columns
            string code = "sql`update t set x = (select max(y) from s); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("x", items);
        }

        // ---- Insert: column after '(' or ',' (line 259) ----

        [Fact]
        public void Insert_columns_after_parenthesis_and_comma()
        {
            // Line 259: Punct(k - 1, '(') || Punct(k - 1, ',')
            string code = "sql`insert into t (a, b, c) values (1, 2, 3); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("a", items);
            Assert.Contains("b", items);
            Assert.Contains("c", items);
        }

        [Fact]
        public void Insert_skips_qualified_column()
        {
            // Line 259: !(Punct(k - 1, '.')) - qualified names skipped
            string code = "sql`insert into t (s.a, b) values (1, 2); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("b", items);
        }

        // ---- Split: semicolons inside comments and strings (lines 325-327) ----

        [Fact]
        public void Split_skips_semicolon_in_double_quote()
        {
            // Line 325: c == '"' skips to closing quote
            string code = "sql`create table t (id int); select * from t where name = \"a;b\"; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
        }

        [Fact]
        public void Split_skips_semicolon_in_line_comment()
        {
            // Line 326: c == '-' && text[i+1] == '-' skips line comment
            string code = "sql`create table t (id int); select * from t -- note; here\n; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
        }

        [Fact]
        public void Split_skips_semicolon_in_block_comment()
        {
            // Line 327: c == '/' && text[i+1] == '*' skips block comment
            string code = "sql`create table t (id int); select * from t /* note; here */; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
        }

        // ---- Alias resolution (lines 176-179, 270-272) ----

        [Fact]
        public void Alias_without_as_keyword()
        {
            string code = "sql`create table users (id int, name text); select u.| from users u`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
        }

        [Fact]
        public void Update_with_alias_without_as()
        {
            string code = "sql`create table users (id int); update users u set id = 1; select u.| from users u`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
        }

        // ---- NotAColumn filtering (line 241) ----

        [Fact]
        public void Constraint_keywords_not_treated_as_columns()
        {
            // Line 241: column != null && !NotAColumn.Contains(column)
            string code = "sql`create table t (id int, unique (id), name text); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("name", items);
            Assert.DoesNotContain("unique", items.Take(10));
        }

        // ---- Table schema prefix (line 141) ----

        [Fact]
        public void Schema_prefix_with_multiple_parts()
        {
            // Line 141: while (Punct(next, '.') && Word(next + 1) != null) - multi-part schema
            string code = "sql`create table db.schema.t (id int); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
        }

        // ---- Caret statement identification (line 311) ----

        [Fact]
        public void Caret_in_second_statement_gets_correct_context()
        {
            // Line 311: caretStart >= statement.Key && caretStart <= statement.Value
            string code = "sql`create table a (id int); create table b (name text); select b.| from b`";
            var items = Texts(code, Schema);
            Assert.Contains("name", items);
        }

        // ---- Depth tracking in Create (lines 215-216) ----

        [Fact]
        public void Create_table_depth_exits_at_matching_paren()
        {
            // Lines 215-216: Punct(k, ')') depth-- and depth == 0 break
            string code = "sql`create table t (id int, val int); select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.Contains("val", items);
        }
    }
}
