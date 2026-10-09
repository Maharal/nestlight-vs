using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The place of the caret in the grammar of SQL, CSS and HTML.</summary>
    public class PositionTests
    {
        private static readonly CompletionFeatures Grammar = new CompletionFeatures(grammar: true);

        private static CompletionEngine Engine(CompletionFeatures features, int maxItems = 100000)
        {
            return new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), maxItems, CompletionEngine.DefaultMinWordLength, features: features);
        }

        private static string Name(string codeWithCaret)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            CompletionSite site = Engine(Grammar).Locate(code, caret);
            Position position = Positions.At(code, site);
            return position == null ? null : position.Name;
        }

        private static List<string> Texts(string codeWithCaret, CompletionFeatures features)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            CompletionEngine engine = Engine(features);
            return engine.Suggest(code, engine.Locate(code, caret)).Select(s => s.Text).ToList();
        }

        // ---- SQL ------------------------------------------------------------------------------------------------------

        [Theory]
        [InlineData("sql`|`", "sql:statement")]
        [InlineData("sql`select 1; |`", "sql:statement")]
        [InlineData("sql`select a from |`", "sql:table")]
        [InlineData("sql`select a from t join |`", "sql:table")]
        [InlineData("sql`insert into |`", "sql:table")]
        [InlineData("sql`update |`", "sql:table")]
        [InlineData("sql`select u.|`", "sql:member")]
        [InlineData("sql`select a, |`", "sql:expression")]
        [InlineData("sql`select a from t where x = |`", "sql:expression")]
        [InlineData("sql`select a from t where |`", "sql:expression")]
        [InlineData("sql`select count(|`", "sql:expression")]
        [InlineData("sql`select a from t group |`", "sql:after-group")]
        [InlineData("sql`insert |`", "sql:after-insert")]
        [InlineData("sql`select a |`", "sql:continue-select")]
        [InlineData("sql`select a from t |`", "sql:continue-from")]
        [InlineData("sql`select a from t where b = 1 |`", "sql:continue-where")]
        [InlineData("sql`update t set a = 1 |`", "sql:continue-set")]
        [InlineData("sql`select 'a from' |`", "sql:continue-select")]
        [InlineData("sql`select a /* from */ |`", "sql:continue-select")]
        [InlineData("sql`select a -- from\n |`", "sql:continue-select")]
        public void Sql_places(string code, string expected)
        {
            Assert.Equal(expected, Name(code));
        }

        [Fact]
        public void After_from_a_table_of_the_document_comes_before_the_keywords_that_start_alike()
        {
            const string code = "const orders = 1;\nsql`select * from ord|`";
            List<string> without = Texts(code, CompletionFeatures.None), with = Texts(code, Grammar);
            Assert.Equal("order", without[0]);
            Assert.Equal("orders", with[0]);
            Assert.Contains("order", with); // demoted, not dropped
            Assert.Equal(without.OrderBy(w => w), with.OrderBy(w => w));
        }

        [Fact]
        public void After_group_the_word_by_comes_first_and_after_a_column_the_next_clause()
        {
            Assert.Equal("by", Texts("sql`select a from t group |`", Grammar)[0]);
            Assert.Equal("from", Texts("sql`select a |`", Grammar)[0]);
            Assert.Equal("where", Texts("sql`select a from t |`", Grammar)[0]);
            Assert.Equal("FROM", Texts("sql`SELECT a F|`", Grammar)[0]); // the case that is being typed
        }

        [Fact]
        public void A_statement_starts_with_a_statement_keyword()
        {
            List<string> items = Texts("sql`|`", Grammar);
            Assert.Equal(new[] { "select", "insert", "update", "delete" }, items.Take(4).ToArray());
        }

        // ---- CSS ------------------------------------------------------------------------------------------------------

        [Theory]
        [InlineData("css`|`", "css:selector")]
        [InlineData("css`.a, .b |`", "css:selector")]
        [InlineData("css`.a { |}`", "css:property")]
        [InlineData("css`.a { color: red; |}`", "css:property")]
        [InlineData("css`.a { display: |}`", "css:value:display")]
        [InlineData("css`.a { Display : |}`", "css:value:display")]
        [InlineData("css`@media screen { .a { color: | } }`", "css:value:color")]
        [InlineData("css`.a { } .b { |}`", "css:property")]
        [InlineData("css`/* a: */ .a { |}`", "css:property")]
        [InlineData("css`.a { content: \"x:\"; |}`", "css:property")]
        public void Css_places(string code, string expected)
        {
            Assert.Equal(expected, Name(code));
        }

        [Fact]
        public void Inside_parentheses_there_is_no_place()
        {
            Assert.Null(Name("css`.a { background: url(|) }`"));
        }

        [Fact]
        public void In_a_value_the_values_of_the_property_come_first_and_the_properties_last()
        {
            List<string> display = Texts("css`.a { display: |}`", Grammar);
            Assert.Equal(new[] { "block", "inline", "inline-block", "flex" }, display.Take(4).ToArray());
            Assert.Equal("none", Texts("css`.a { display: no|}`", Grammar)[0]);
            Assert.Equal("center", Texts("css`.a { text-align: ce|}`", Grammar)[0]);

            List<string> all = Texts("css`.a { display: |}`", Grammar);
            Assert.True(all.IndexOf("color") > all.IndexOf("inline-grid")); // a property after the values
        }

        [Fact]
        public void In_a_property_position_the_properties_come_first_and_the_values_last()
        {
            List<string> items = Texts("css`.a { c| }`", Grammar);
            Assert.StartsWith("c", items[0]);
            Assert.True(items.IndexOf("color") < items.IndexOf("center"));
            Assert.Equal(Texts("css`.a { c| }`", CompletionFeatures.None).OrderBy(w => w), items.OrderBy(w => w));
        }

        [Fact]
        public void In_a_selector_the_tags_of_html_are_offered()
        {
            List<string> items = Texts("css`bu| {}`", Grammar);
            Assert.Equal("button", items[0]);
            Assert.DoesNotContain("button", Texts("css`bu| {}`", CompletionFeatures.None));
        }

        // ---- HTML -----------------------------------------------------------------------------------------------------

        [Theory]
        [InlineData("html`<|`", "html:tag")]
        [InlineData("html`<di|`", "html:tag")]
        [InlineData("html`</di|`", "html:tag")]
        [InlineData("html`<p>hello |</p>`", "html:text")]
        [InlineData("html`|`", "html:text")]
        [InlineData("html`<button |`", "html:attribute:button")]
        [InlineData("html`<a href='x' |`", "html:attribute:a")]
        [InlineData("html`<input type=\"text\" na|`", "html:attribute:input")]
        [InlineData("html`<input type=\"|\"`", "html:value:type")]
        [InlineData("html`<input type=|`", "html:value:type")]
        [InlineData("html`<div class='a b |'`", "html:value:class")]
        [InlineData("svg`<g |`", "html:attribute:g")]
        public void Html_places(string code, string expected)
        {
            Assert.Equal(expected, Name(code));
        }

        [Fact]
        public void In_a_tag_the_attributes_of_the_tag_are_offered_and_no_tag()
        {
            List<string> items = Texts("html`<button t|`", Grammar);
            Assert.Equal("type", items[0]);
            Assert.Contains("title", items);
            Assert.DoesNotContain("table", items); // a tag name never belongs where an attribute goes
            Assert.Equal("href", Texts("html`<a h|`", Grammar)[0]);
            Assert.Equal("viewBox", Texts("svg`<svg v|`", Grammar)[0]);
        }

        [Fact]
        public void In_the_value_of_an_attribute_its_values_come_first()
        {
            Assert.Equal("checkbox", Texts("html`<input type=\"ch|\"`", Grammar)[0]);
            Assert.Equal("_blank", Texts("html`<a target='_|'`", Grammar)[0]);
        }

        [Fact]
        public void In_a_class_the_classes_of_the_document_come_first()
        {
            List<string> items = Texts("html`<div class='card-title'></div><div class='ca|'`", Grammar);
            Assert.Equal("card-title", items[0]);
        }

        [Fact]
        public void In_text_a_tag_name_is_not_offered()
        {
            List<string> items = Texts("html`<p>dish</p><p>di|</p>`", Grammar);
            Assert.Equal("dish", items[0]);
            Assert.DoesNotContain("div", items); // no tag in a text node
        }

        [Fact]
        public void A_language_without_grammar_has_no_place()
        {
            Assert.Null(Name("json`{ \"a\": tr| }`"));
            Assert.Null(Name("graphql`query { us| }`"));
            Assert.Null(Name("yaml`a: tr|`"));
        }

        [Fact]
        public void The_place_is_found_in_every_cut_of_a_text_without_errors()
        {
            string[] samples =
            {
                "sql`select a, b from t join u on t.id = u.id where x in (1, 2) group by a order by b; -- c\n/* d */ insert into t (a) values ('x');`",
                "css`@media (min-width: 1px) { .a:hover > b { color: red; background: url(x.png); content: \"{\"; } }`",
                "html`<div class=\"a b\" id='c' data-x=y><input type=text disabled /><!-- <x --></div>`",
            };
            foreach (string sample in samples)
                for (int cut = 0; cut <= sample.Length; cut++)
                {
                    string text = sample.Substring(0, cut);
                    CompletionEngine engine = Engine(Grammar, 100);
                    CompletionSite site = engine.Locate(text, text.Length);
                    if (site == null) continue;
                    Position position = Positions.At(text, site);
                    Assert.NotNull(engine.Suggest(text, site));
                }
        }
    }
}
