using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>What the completion of CSS offers beyond the properties and their values.</summary>
    public class CssCompletionTests
    {
        private static CompletionEngine Engine(CompletionFeatures features = null)
        {
            return new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000, CompletionEngine.DefaultMinWordLength, new BandedPrefixMatcher(), features: features ?? CompletionFeatures.Default);
        }

        private static CompletionSite Site(string codeWithCaret, out string code)
        {
            int caret = codeWithCaret.IndexOf('|');
            code = codeWithCaret.Remove(caret, 1);
            return Engine().Locate(code, caret);
        }

        private static List<string> Texts(string codeWithCaret)
        {
            string code;
            CompletionSite site = Site(codeWithCaret, out code);
            return Engine().Suggest(code, site).Select(s => s.Text).ToList();
        }

        private static string Name(string codeWithCaret)
        {
            string code;
            CompletionSite site = Site(codeWithCaret, out code);
            if (site == null) return "(no site)";
            Position position = Positions.At(code, site);
            return position == null ? null : position.Name;
        }

        [Fact]
        public void An_at_rule_name_after_the_at_sign()
        {
            Assert.Equal("css:at-rule", Name("css`@|`"));
            Assert.Equal("media", Texts("css`@med|`")[0]);
            Assert.Equal("keyframes", Texts("css`.a {} @key|`")[0]);
            Assert.Equal("font-face", Texts("css`@font-|`")[0]);
            Assert.Equal("container", Texts("css`@conta|`")[0]);
            Assert.All(Engine().Suggest("@med", Engine().Locate("@med", 4) ?? Site("css`@med|`", out string c)), s => { });
        }

        [Fact]
        public void The_condition_of_each_at_rule_has_its_own_words()
        {
            Assert.Equal("css:media-feature", Name("css`@media (|`"));
            Assert.Equal("css:supports-feature", Name("css`@supports (|`"));
            Assert.Equal("display", Texts("css`@supports (displ|`")[0]);
            Assert.Equal("css:container-feature", Name("css`@container card (|`"));
            Assert.Equal("inline-size", Texts("css`@container (inline-|`")[0]);
            Assert.Equal("css:media-query", Name("css`@media |`"));
            Assert.Equal("screen", Texts("css`@media scr|`")[0]);
        }

        [Fact]
        public void After_an_exclamation_mark_important()
        {
            Assert.Equal("css:important", Name("css`.a { color: red !|}`"));
            Assert.Equal("important", Texts("css`.a { color: red !imp|}`")[0]);
        }

        [Fact]
        public void After_a_number_a_unit()
        {
            Assert.Equal("css:unit", Name("css`.a { width: 10|}`"));
            Assert.Equal("px", Texts("css`.a { width: 10|}`")[0]);
            Assert.Equal("rem", Texts("css`.a { width: 10r|}`")[0]);
            Assert.Equal("vh", Texts("css`.a { height: 100v|}`")[0]);
            Assert.Equal("ms", Texts("css`.a { transition: opacity 200m|}`")[0]);
            Assert.Equal("deg", Texts("css`.a { transform: rotate(45d|)}`")[0]);
            Assert.Equal("rem", Texts("css`.a { margin: -1.5r|}`")[0]);
            Assert.Equal("css:unit", Name("css`@media (min-width: 60|`"));
            Assert.Equal("px", Texts("css`@media (min-width: 600p|`")[0]);
        }

        [Fact]
        public void A_hex_color_a_keyframe_percentage_and_a_selector_number_are_not_units()
        {
            Assert.Equal("(no site)", Name("css`.a { color: #123|}`"));
            Assert.Equal("css:hex", Name("css`.a { color: #ab|}`"));
            Assert.Null(Name("css`@keyframes x { 50|% { top: 0 } }`"));
            Assert.Null(Name("css`li:nth-child(2|n) {}`"));
        }

        [Fact]
        public void All_the_named_colors_for_the_properties_that_take_a_color()
        {
            Assert.Equal("rebeccapurple", Texts("css`.a { color: rebecca|}`")[0]);
            Assert.Equal("tomato", Texts("css`.a { background-color: tom|}`")[0]);
            Assert.Equal("cornflowerblue", Texts("css`.a { border-color: cornf|}`")[0]);
            Assert.Equal("red", Texts("css`.a { color: re|}`")[0]); // the basic ones keep the front
            Assert.Equal("lightgoldenrodyellow", Texts("css`.a { fill: lightgold|}`")[0]);
        }

        [Fact]
        public void Named_colors_in_the_shorthands_that_take_one_after_the_values_of_the_property()
        {
            List<string> border = Texts("css`.a { border: 1px solid salm|}`");
            Assert.Equal("salmon", border[0]);
            Assert.Contains("steelblue", Texts("css`.a { box-shadow: 0 0 4px steel|}`"));
            Assert.Contains("navy", Texts("css`.a { background: nav|}`"));
        }

        [Fact]
        public void Modern_properties()
        {
            Assert.Equal("margin-inline", Texts("css`.a { margin-in|}`")[0]);
            Assert.Equal("padding-block", Texts("css`.a { padding-bl|}`")[0]);
            Assert.Equal("container-type", Texts("css`.a { container-t|}`")[0]);
            Assert.Equal("accent-color", Texts("css`.a { accent|}`")[0]);
            Assert.Equal("scrollbar-gutter", Texts("css`.a { scrollbar-g|}`")[0]);
            Assert.Equal("text-wrap", Texts("css`.a { text-wra|}`")[0]);
            Assert.Equal("stroke-linecap", Texts("css`.a { stroke-linec|}`")[0]);
            Assert.True(Vocabularies.CssProperties.Count > 300);
            Assert.Equal(Vocabularies.CssProperties.OrderBy(w => w, System.StringComparer.OrdinalIgnoreCase), Vocabularies.CssProperties);
        }

        [Fact]
        public void An_attribute_selector_offers_the_attributes_that_are_tested_most()
        {
            Assert.Equal("css:attribute-selector", Name("css`input[|`"));
            Assert.Equal("type", Texts("css`input[ty|`")[0]);
            Assert.Equal("href", Texts("css`a[hr|`")[0]);
            Assert.Equal("disabled", Texts("css`button[disa|`")[0]);
        }

        [Fact]
        public void The_properties_come_in_the_order_of_use_and_the_rest_alphabetically()
        {
            List<string> items = Texts("css`.a { | }`");
            IReadOnlyList<string> prior = KeywordUse.Default["css"];
            string[] firstProperties = prior.Where(w => Vocabularies.CssProperties.Contains(w, System.StringComparer.OrdinalIgnoreCase)).Take(5).ToArray();
            Assert.Equal(firstProperties, items.Take(5).ToArray());
            Assert.True(items.IndexOf("zoom") > items.IndexOf("display")); // one nobody has written comes last
            List<string> a = Texts("css`.a { bo| }`");
            Assert.Equal(a.OrderBy(w => w).Count(), a.Count);
            Assert.Equal(Texts("css`.a { bo| }`"), Texts("css`.a { bo| }`")); // the same every time (the order is cached)
        }
    }
}
