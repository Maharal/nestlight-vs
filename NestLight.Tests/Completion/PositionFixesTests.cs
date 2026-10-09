using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The places of the grammar that a review of 800 suggestions found wrong or missing.</summary>
    public class PositionFixesTests
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

        private static List<Suggestion> Items(string codeWithCaret, CompletionFeatures features = null)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            CompletionEngine engine = Engine(features ?? Grammar);
            return engine.Suggest(code, engine.Locate(code, caret)).ToList();
        }

        private static List<string> Texts(string codeWithCaret, CompletionFeatures features = null) { return Items(codeWithCaret, features).Select(s => s.Text).ToList(); }

        // ---- SQL ------------------------------------------------------------------------------------------------------

        [Fact]
        public void After_group_by_a_column_the_next_clauses_come_first()
        {
            Assert.Equal("sql:continue-group", Name("sql`select a from t group by a |`"));
            Assert.Equal(new[] { "having", "order", "limit" }, Texts("sql`select a from t group by a |`").Take(3).ToArray());
        }

        [Fact]
        public void The_clause_inside_parentheses_is_not_the_clause_after_them()
        {
            Assert.Equal("sql:continue-select", Name("sql`select rank() over (partition by m order by t desc) |`"));
            Assert.Equal("from", Texts("sql`select rank() over (partition by m order by t desc) |`")[0]); // the clause of the select list, not the ORDER BY inside the OVER( )
            Assert.Contains("as", Texts("sql`select rank() over (partition by m order by t desc) a|`"));
            Assert.DoesNotContain("asc", Texts("sql`select rank() over (partition by m order by t desc) a|`").Take(1));
            Assert.Equal("sql:continue-from", Name("sql`select a from (select b from c) x |`"));
        }

        [Fact]
        public void Inside_a_literal_or_a_comment_no_keyword_is_offered()
        {
            Assert.Equal("sql:literal", Name("sql`select 'ab|'`"));
            Assert.Equal("sql:comment", Name("sql`select a -- fr|`"));
            Assert.Equal("sql:comment", Name("sql`select a /* fr|`"));
            Assert.All(Items("sql`select a, 'from x' as b, 'fr|'`"), s => Assert.Equal(SuggestionKind.Word, s.Kind));
            Assert.Equal(new[] { "days" }, Texts("sql`select ${d} from t where x < interval '${days} day|'`").ToArray().Where(w => w == "days").ToArray());
        }

        [Fact]
        public void In_the_definition_of_a_table_a_name_then_a_type_then_the_constraints()
        {
            Assert.Equal("sql:column-name", Name("sql`create table t (|`"));
            Assert.Equal("sql:column-type", Name("sql`create table t (id |`"));
            Assert.Equal("int", Texts("sql`create table t (id |`")[0]);
            Assert.Equal("sql:column-constraint", Name("sql`create table t (id int |`"));
            Assert.Equal("not", Texts("sql`create table t (id int |`")[0]);
            Assert.Equal("sql:column-name", Name("sql`create table t (id int, |`"));
            Assert.Equal("sql:column-type", Name("sql`create table t (id int primary key, name |`"));
            Assert.Equal("sql:column-type", Name("sql`create table t (id varchar(32) not null, name |`")); // the parentheses of a type do not break the entry
            Assert.All(Items("sql`create table t (|`"), s => Assert.Equal(SuggestionKind.Word, s.Kind));
        }

        [Fact]
        public void Other_places_of_SQL_that_the_review_found()
        {
            Assert.Equal("conflict", Texts("sql`insert into t (a) values (1) on |`")[0]);
            Assert.Equal("add", Texts("sql`alter table t |`")[0]);
            Assert.Equal("column", Texts("sql`alter table t drop |`")[0]);
            Assert.Equal("when", Texts("sql`select case |`")[0]);
            Assert.Equal("nothing", Texts("sql`insert into t (a) values (1) on conflict (a) do |`")[0]);
        }

        [Fact]
        public void The_functions_of_SQL_are_offered_after_the_columns()
        {
            List<string> items = Texts("sql`select coun|`");
            Assert.Contains("count", items);
            List<string> columns = Texts("const countryName = 1;\nsql`select coun|`");
            Assert.True(columns.IndexOf("countryName") < columns.IndexOf("count")); // the column is the word of the file, the function comes after it
            Assert.Contains("sum", Texts("sql`select a, su|`"));
            Assert.Contains("coalesce", Texts("sql`select a from t where x = coal|`"));
        }

        // ---- CSS ------------------------------------------------------------------------------------------------------

        [Fact]
        public void After_a_dot_or_a_hash_only_names_of_the_file_are_offered()
        {
            Assert.Equal("css:class", Name("css`.|`"));
            Assert.Equal("css:class", Name("css`#|`"));
            List<Suggestion> items = Items("css`.button { color: red } .bu|`");
            Assert.Equal(new[] { "button" }, items.Select(s => s.Text).ToArray());
            Assert.All(Items("css`.b|`"), s => Assert.Equal(SuggestionKind.Word, s.Kind));
            Assert.Contains("blockquote", Texts("css`bl| {}`")); // an element selector keeps the tags
        }

        [Fact]
        public void After_a_colon_in_a_selector_the_pseudo_classes_and_elements()
        {
            Assert.Equal("css:pseudo-class", Name("css`a:|`"));
            Assert.Equal("hover", Texts("css`a:ho|`")[0]);
            Assert.Equal("css:pseudo-element", Name("css`a::|`"));
            Assert.Equal("before", Texts("css`a::be|`")[0]);
            Assert.Equal("css:value:color", Name("css`.a { color: |}`")); // the colon of a declaration is not a pseudo-class
        }

        [Fact]
        public void At_rules_hold_selectors_and_keyframes_not_declarations()
        {
            Assert.Equal("css:selector", Name("css`@media screen { b| }`"));
            Assert.Equal("css:class", Name("css`@media screen { .| }`"));
            Assert.Equal("css:property", Name("css`@media screen { .a { col| } }`"));
            Assert.Equal("css:media-feature", Name("css`@media (|`"));
            Assert.Equal("max-width", Texts("css`@media (max-w|`")[0]);
            Assert.Equal("css:keyframes-name", Name("css`@keyframes |`"));
            Assert.Equal("css:keyframe-selector", Name("css`@keyframes x { | }`"));
            Assert.Equal("from", Texts("css`@keyframes x { fr| }`")[0]);
            Assert.Equal("css:property", Name("css`@keyframes x { from { | } }`"));
            Assert.Equal("css:selector", Name("css`@media screen { .a { color: red } | }`"));
        }

        [Fact]
        public void Transition_and_animation_values_and_the_functions_of_a_value()
        {
            Assert.Equal("opacity", Texts("css`.a { transition: opa|`")[0]);
            Assert.Equal("ease", Texts("css`.a { transition: opacity 1s ea|`")[0]);
            Assert.Equal("infinite", Texts("css`.a { animation: spin 1s inf|`")[0]);
            Assert.Contains("translate", Texts("css`.a { transform: trans|`"));
            Assert.Contains("repeat", Texts("css`.a { grid-template-columns: rep|`"));
            Assert.Contains("var", Texts("css`.a { color: va|`"));
        }

        // ---- HTML -----------------------------------------------------------------------------------------------------

        [Fact]
        public void A_closing_tag_offers_the_element_that_is_open()
        {
            Assert.Equal("html:closing-tag", Name("html`<ul><li>x</|`"));
            Assert.Equal("li", Texts("html`<ul><li>x</|`")[0]);
            Assert.Equal("ul", Texts("html`<ul><li>x</li></|`")[0]);
            Assert.Equal("div", Texts("html`<div><br><img src='a'></|`")[0]);
            Assert.Equal("div", Texts("html`<div><!-- <b> --></|`")[0]);
            Assert.Equal("svg", Texts("html`<svg><circle r='1'/><path d='M0 0'/></|`")[0]);
            Assert.Equal("div", Texts("html`<div><p>a</p><p>b</p></|`")[0]);
            Assert.Equal("span", Texts("html`<div><span class='a>b'>x</|`")[0]); // a '>' inside a quoted value does not end the tag
            Assert.Equal("span", Texts("html`<div><span>x</sp|`")[0]);
        }

        [Fact]
        public void An_attribute_the_tag_already_has_is_not_offered_again()
        {
            // the attributes of the table (keywords); the words of the file are another list
            Func<string, List<string>> attributes = code => Items(code).Where(s => s.Kind == SuggestionKind.Keyword).Select(s => s.Text).ToList();
            List<string> items = attributes("html`<input type='text' name='a' |`");
            Assert.DoesNotContain("type", items);
            Assert.DoesNotContain("name", items);
            Assert.Contains("placeholder", items);
            Assert.DoesNotContain("type", attributes("html`<input | type='text'>`")); // nor one that comes after the caret
            Assert.Contains("type", attributes("html`<input ty| value='x'>`"));
            Assert.Contains("value", attributes("html`<option va|>`"));
        }

        [Fact]
        public void In_a_value_or_a_text_no_tag_is_offered()
        {
            Assert.DoesNotContain("div", Texts("html`<p class='dish'>x</p><p class='di|'>`"));
            Assert.DoesNotContain("main", Texts("html`<nav aria-label='ma|'>`"));
            Assert.DoesNotContain("header", Texts("html`<svg xmlns='http://www.w3.org/2000/svg' x='he|'>`"));
            Assert.Equal("text", Texts("html`<input type='te|'>`")[0]);
        }

        // ---- JSON and YAML -----------------------------------------------------------------------------------------------

        [Fact]
        public void In_json_a_key_and_a_string_get_no_keyword_and_a_value_gets_true_false_null()
        {
            Assert.Equal("json:key", Name("json`{ \"na|\": 1 }`"));
            Assert.Equal("json:key", Name("json`{ \"a\": 1, \"b|\": 2 }`"));
            Assert.Equal("json:key", Name("json`[ { \"x|\": 1 } ]`"));
            Assert.Equal("json:string", Name("json`{ \"a\": \"tr|\" }`"));
            Assert.Equal("json:string", Name("json`[ \"a\", \"tr|\" ]`"));
            Assert.Null(Name("json`{ \"a\": tr| }`"));
            Assert.Null(Name("json`[ 1, tr| ]`"));
            Assert.Contains("true", Texts("json`{ \"a\": tr| }`"));
            Assert.DoesNotContain("true", Texts("json`{ \"tr|\": 1 }`"));
            Assert.DoesNotContain("null", Texts("json`{ \"a\": \"nu|\" }`"));
            Assert.Equal("json:key", Name("json`{ \"a\\\"b\": 1, \"c|\": 2 }`")); // an escaped quote does not end the string
        }

        [Fact]
        public void In_yaml_a_key_gets_no_keyword_and_a_value_gets_the_booleans()
        {
            Assert.Equal("yaml:key", Name("yaml`ap|: 1`"));
            Assert.Equal("yaml:key", Name("yaml`a:\n  - na|`"));
            Assert.Equal("yaml:key", Name("yaml`a:\n  - name: x\n    ru|`"));
            Assert.Null(Name("yaml`a: tr|`"));
            Assert.Null(Name("yaml`a:\n  - b: tr|`"));
            Assert.Equal("yaml:comment", Name("yaml`a: 1 # tr|`"));
            Assert.Equal("yaml:string", Name("yaml`a: \"tr|\"`"));
            Assert.Contains("true", Texts("yaml`a: tr|`"));
            Assert.DoesNotContain("true", Texts("yaml`tr|: 1`"));
        }

        // ---- the word after the caret --------------------------------------------------------------------------------------

        [Fact]
        public void The_word_that_already_follows_the_caret_is_not_a_word_that_followed_the_context()
        {
            var previous = new CompletionFeatures(previousWord: true);
            List<string> items = Texts("sql`select a from alpha; select b from | beta`", previous);
            Assert.Equal("alpha", items[0]);
            Assert.Contains("beta", items); // still a word of the file
        }
    }
}
