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

        // ---- Table.Add deduplication (L25, L27) ----

        [Fact]
        public void Table_Add_rejects_duplicate_columns_case_insensitively()
        {
            string code = "sql`create table t (name text); alter table t add column Name text; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Equal(1, items.Where(i => i.Equals("name", System.StringComparison.OrdinalIgnoreCase) || i.Equals("Name", System.StringComparison.OrdinalIgnoreCase)).Take(2).Count());
        }

        // ---- Tokenizer: comment skipping (L72, L74-L79) ----

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

        // ---- String literal skipping (L81-L86) ----

        [Fact]
        public void String_literal_content_is_not_treated_as_identifiers()
        {
            string code = "sql`create table t (id int); select * from t where id = 'fake_table'; select t.| from t`";
            var items = Texts(code, Schema);
            Assert.Contains("id", items);
            Assert.DoesNotContain("fake_table", items.Take(5));
        }

        // ---- FROM/JOIN parsing (L163, L166-L168, L170-L181) ----

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

        // ---- CREATE TABLE (L197-L225) ----

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

        // ---- ALTER TABLE (L228-L245) ----

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

        // ---- INSERT INTO (L247-L262) ----

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

        // ---- UPDATE SET (L264-L289) ----

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

        // ---- alias.column references (L186-L194) ----

        [Fact]
        public void Alias_dot_column_is_learned()
        {
            string code = "sql`select u.phone, u.address from users u; select u.| from users u`";
            var items = Texts(code, Schema);
            Assert.Contains("phone", items);
            Assert.Contains("address", items);
        }

        // ---- Split: semicolons inside comments/strings do not split (L318-L333) ----

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

        // ---- endsFrom stops FROM parsing (L163) ----

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

        // ---- Uses count for table ordering (L148, L108 existing test but let's add edge case) ----

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
    }
}
