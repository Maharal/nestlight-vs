using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The tables and columns read from the SQL of the document, and what they put first.</summary>
    public class SqlSchemaTests
    {
        private static readonly CompletionFeatures Schema = new CompletionFeatures(schema: true);
        private static readonly CompletionFeatures SchemaAndGrammar = new CompletionFeatures(grammar: true, schema: true);

        private const string Tables = "sql`create table users (id int primary key, name text, email text, primary key (id))`;\n"
                                    + "sql`create table orders (id int, user_id int, total int, status text, foreign key (user_id) references users (id))`;\n";

        private static List<string> Texts(string codeWithCaret, CompletionFeatures features, bool onlyWords = false)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000, CompletionEngine.DefaultMinWordLength, features: features);
            CompletionSite site = engine.Locate(code, caret);
            return engine.Suggest(code, site).Where(s => !onlyWords || s.Kind == SuggestionKind.Word).Select(s => s.Text).ToList();
        }

        // ---- columns -------------------------------------------------------------------------------------------------

        [Fact]
        public void After_an_alias_and_a_dot_the_columns_of_its_table_come_first()
        {
            Assert.Equal(new[] { "id", "name", "email" }, Texts(Tables + "sql`select u.| from users u`", Schema).Take(3).ToArray());
            Assert.Equal("name", Texts(Tables + "sql`select u.na| from users u`", Schema)[0]);
            Assert.Equal("status", Texts(Tables + "sql`select o.s| from users u join orders o on o.user_id = u.id`", Schema)[0]);
        }

        [Fact]
        public void The_columns_of_the_other_table_do_not_come_first()
        {
            List<string> items = Texts(Tables + "sql`select o.| from users u join orders o on o.user_id = u.id`", Schema);
            Assert.Equal(new[] { "id", "status", "total", "user_id" }, items.Take(4).OrderBy(x => x).ToArray());
            Assert.True(!items.Contains("email") || items.IndexOf("total") < items.IndexOf("email"));
        }

        [Fact]
        public void An_alias_means_what_its_own_statement_says()
        {
            const string code = "sql`select t.a1 from alpha t`;\nsql`select t.b1 from beta t`;\nsql`select t.| from beta t`";
            Assert.Equal("b1", Texts(code, Schema)[0]);
            Assert.DoesNotContain("a1", Texts(code, Schema).Take(1));
        }

        [Fact]
        public void A_table_name_works_as_its_own_qualifier()
        {
            Assert.Equal("total", Texts(Tables + "sql`select orders.to| from orders`", Schema)[0]);
        }

        [Fact]
        public void Columns_seen_only_in_queries_are_known_too()
        {
            const string code = "sql`select u.email, u.phone from users u`;\nsql`select u.| from users u`";
            Assert.Equal(new[] { "email", "phone" }, Texts(code, Schema).Take(2).ToArray());
        }

        [Fact]
        public void In_the_select_list_the_columns_of_the_tables_of_the_statement_come_first_even_when_the_from_is_after_the_caret()
        {
            Assert.Equal(new[] { "id", "name", "email" }, Texts(Tables + "sql`select | from users`", Schema).Take(3).ToArray());
            Assert.Equal("total", Texts(Tables + "sql`select a from orders where to|`", Schema)[0]);
            Assert.Equal("email", Texts(Tables + "sql`select a from users where id = 1 and em|`", Schema)[0]);
        }

        [Fact]
        public void In_an_insert_and_an_update_the_columns_of_the_table_come_first()
        {
            Assert.Equal(new[] { "id", "name", "email" }, Texts(Tables + "sql`insert into users (|`", Schema).Take(3).ToArray());
            Assert.Equal("status", Texts(Tables + "sql`update orders set st|`", Schema)[0]);
            Assert.Equal("total", Texts(Tables + "sql`update orders o set o.to|`", Schema)[0]);
        }

        [Fact]
        public void Columns_of_alter_table_and_insert_lists_are_learned()
        {
            string code = Tables + "sql`alter table users add column nickname text`;\nsql`insert into users (id, birthday) values (1, 2)`;\nsql`select u.| from users u`";
            List<string> items = Texts(code, Schema);
            Assert.Equal(new[] { "id", "name", "email", "nickname", "birthday" }, items.Take(5).ToArray()); // declared first, then learned
        }

        // ---- tables --------------------------------------------------------------------------------------------------

        [Fact]
        public void After_from_the_tables_of_the_document_are_offered()
        {
            List<string> items = Texts(Tables + "sql`select * from |`", Schema);
            Assert.Equal(new[] { "orders", "users" }, items.Take(2).OrderBy(x => x).ToArray());
            Assert.Equal("users", Texts(Tables + "sql`select * from us|`", Schema, true)[0]);
        }

        [Fact]
        public void The_word_being_typed_is_not_a_table()
        {
            Assert.DoesNotContain("us", Texts("sql`select * from us|`", Schema));
            Assert.DoesNotContain("zz", Texts("sql`select a from users zz| `", Schema));
        }

        [Fact]
        public void A_table_used_in_more_places_comes_before_one_used_less_when_neither_was_declared()
        {
            const string code = "sql`select a from rare`;\nsql`select a from common`;\nsql`select b from common`;\nsql`select * from |`";
            Assert.Equal(new[] { "common", "rare" }, Texts(code, Schema, true).Take(2).ToArray());
        }

        // ---- when it knows nothing -----------------------------------------------------------------------------------

        [Fact]
        public void Without_tables_the_ranking_is_the_one_without_the_schema()
        {
            const string code = "const alpha = 1;\nsql`select | `";
            Assert.Equal(Texts(code, CompletionFeatures.None), Texts(code, Schema));
            const string member = "sql`select zz.| from t`";
            Assert.Equal(Texts(member, CompletionFeatures.None), Texts(member, Schema));
        }

        [Fact]
        public void Other_languages_are_left_alone()
        {
            const string code = Tables + "css`.a { col| }`";
            Assert.Equal(Texts(code, CompletionFeatures.None), Texts(code, Schema));
        }

        [Fact]
        public void The_schema_does_not_take_away_what_the_grammar_offers()
        {
            const string code = Tables + "sql`select u.| from users u`";
            List<string> with = Texts(code, SchemaAndGrammar), without = Texts(code, new CompletionFeatures(grammar: true));
            Assert.Empty(without.Except(with));
            Assert.Equal(with.Count, with.Distinct().Count());
        }

        [Fact]
        public void Statements_it_does_not_understand_and_every_cut_of_a_text_do_not_throw()
        {
            string[] samples =
            {
                "sql`create table (; alter table ; insert into (( update set from join , ) ) select . from . a.b.c.d`",
                "sql`create table t (a int, b varchar(10), constraint c primary key (a, b)); select t.a, x.* from t, (select 1) x where t.a = x.b; -- from z\n/* join q */ update t as u set a = 1, b = (select max(c) from d) where u.a = 2;`",
                "sql`select 'it''s from fake f', \"from\" from real r join other o on o.id = r.id and o.x in (select y from z) group by r.a order by 1 limit 5 offset 2`",
            };
            foreach (string sample in samples)
                for (int cut = 0; cut <= sample.Length; cut++)
                {
                    string text = sample.Substring(0, cut);
                    var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100, CompletionEngine.DefaultMinWordLength, features: SchemaAndGrammar);
                    CompletionSite site = engine.Locate(text, text.Length);
                    if (site != null) Assert.NotNull(engine.Suggest(text, site));
                    for (int caret = 0; caret <= text.Length; caret += 7)
                    {
                        site = engine.Locate(text, caret);
                        if (site != null) Assert.NotNull(engine.Suggest(text, site));
                    }
                }
        }
    }
}
