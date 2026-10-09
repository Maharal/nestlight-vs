using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The CSS inside an HTML string: a style element and a style attribute.</summary>
    public class NestedCssTests
    {
        private static CompletionEngine Engine(CompletionFeatures features = null)
        {
            return new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000, CompletionEngine.DefaultMinWordLength, new BandedPrefixMatcher(), features: features ?? CompletionFeatures.Default);
        }

        private static CompletionSite Site(string codeWithCaret, out string code, CompletionFeatures features = null)
        {
            int caret = codeWithCaret.IndexOf('|');
            code = codeWithCaret.Remove(caret, 1);
            return Engine(features).Locate(code, caret);
        }

        private static List<string> Texts(string codeWithCaret, CompletionFeatures features = null)
        {
            string code;
            CompletionSite site = Site(codeWithCaret, out code, features);
            return Engine(features).Suggest(code, site).Select(s => s.Text).ToList();
        }

        private static string Language(string codeWithCaret)
        {
            string code;
            CompletionSite site = Site(codeWithCaret, out code);
            return site == null ? null : site.LanguageId;
        }

        private static string Place(string codeWithCaret)
        {
            string code;
            CompletionSite site = Site(codeWithCaret, out code);
            Position position = Positions.At(code, site);
            return position == null ? null : position.Name;
        }

        // ---- where the CSS is ----------------------------------------------------------------------------------------

        [Fact]
        public void The_content_of_a_style_element_and_the_value_of_a_style_attribute_are_css()
        {
            Assert.Equal("css", Language("html`<style>.a { col| }</style>`"));
            Assert.Equal("css", Language("html`<div style=\"col|\">x</div>`"));
            Assert.Equal("css", Language("html`<div style='color: red; mar|'>x</div>`"));
            Assert.Equal("css", Language("svg`<svg><style>.a { fil| }</style></svg>`"));
            Assert.Equal("css", Language("html`<STYLE media='print'>.a { col| }</STYLE>`"));
            Assert.Equal("css", Language("html`<div STYLE=\"col|\">`"));
        }

        [Fact]
        public void The_html_around_it_is_still_html()
        {
            Assert.Equal("html", Language("html`<style>.a { color: red }</style><di|`"));
            Assert.Equal("html", Language("html`<div cla| style=\"color: red\">`"));
            Assert.Equal("html", Language("html`<div class=\"col|\" style=\"color: red\">`"));
            Assert.Equal("html", Language("html`<p>col|</p><style>.a {}</style>`"));
            Assert.Equal("html", Language("html`<sty|`"));
            Assert.Equal("html", Language("html`<div data-style=\"col|\">`"));
            Assert.Equal("html", Language("html`<style>.a {}</style> te|`"));
        }

        [Fact]
        public void An_unfinished_style_is_css_to_the_end_of_the_string()
        {
            Assert.Equal("css", Language("html`<style>.a { col|`"));
            Assert.Equal("css", Language("html`<div style=\"col|`"));
            Assert.Equal("css", Language("html`<style>|`"));
            Assert.Equal("css", Language("html`<div style=\"|\">`"));
        }

        [Fact]
        public void An_interpolation_does_not_end_a_tag_or_a_quote()
        {
            Assert.Equal("css", Language("html`<div class=${a => a > 1} style=\"mar|\">`"));
            Assert.Equal("css", Language("html`<div style=\"color: ${c ? \"red\" : \"blue\"}; mar|\">`"));
            Assert.Equal("css", Language("html`<style>.a { color: ${c}; mar| }</style>`"));
            Assert.Null(Language("html`<div style=\"color: ${co|}\">`")); // the host owns the expression
        }

        [Fact]
        public void A_comment_with_a_style_in_it_is_not_css()
        {
            Assert.Equal("html", Language("html`<!-- <style> --><p>x|</p>`"));
            Assert.Equal("html", Language("html`<!-- <div style=\"a:b\"> -->te|`"));
        }

        // ---- what is offered ----------------------------------------------------------------------------------------

        [Fact]
        public void A_style_element_gets_the_suggestions_of_css()
        {
            Assert.Equal("css:property", Place("html`<style>.a { col| }</style>`"));
            Assert.Equal("color", Texts("html`<style>.a { col| }</style>`")[0]);
            Assert.Equal("flex", Texts("html`<style>.a { display: fl| }</style>`")[0]);
            Assert.Equal("css:class", Place("html`<style>.a {} .|</style>`"));
            Assert.Equal("hover", Texts("html`<style>a:ho| {}</style>`")[0]);
            Assert.Equal("css:value:display", Place("html`<style>@media screen { .a { display: | } }</style>`"));
            Assert.Equal("css:keyframe-selector", Place("html`<style>@keyframes x { | }</style>`"));
        }

        [Fact]
        public void A_style_attribute_is_a_list_of_declarations()
        {
            Assert.Equal("css:property", Place("html`<div style=\"|\">`"));
            Assert.Equal("color", Texts("html`<div style=\"col|\">`")[0]);
            Assert.Equal("css:value:display", Place("html`<div style=\"display: |\">`"));
            Assert.Equal("flex", Texts("html`<div style=\"display: fl|\">`")[0]);
            Assert.Equal("margin", Texts("html`<div style='color: red; mar|'>`")[0]);
            Assert.Equal("css:property", Place("html`<div style=\"color: red; |\">`")); // after the semicolon, a property again
            Assert.Equal("css:value:margin", Place("html`<div style=\"color: red; margin: |\">`"));
            Assert.Equal("--accent", Texts("html`<div style=\"--accent: red\"></div><p style=\"color: var(--ac|)\">`").FirstOrDefault(w => w == "--accent"));
        }

        [Fact]
        public void The_words_of_the_css_are_the_words_of_every_style_in_the_file_and_not_the_html()
        {
            const string code = "html`<style>.cardTitle { color: red }</style><p class=\"cardBody\" style=\"color: blue\">x</p>`;\ncss`.cardFooter { color: green }`;\nhtml`<div style=\"car|\">`";
            var scoped = new CompletionFeatures(sameLanguageWords: true);
            List<string> items = Texts(code, scoped);
            // no selector word is offered in a declaration list: the property position has only properties and the words of css
            Assert.DoesNotContain("cardBody", items.Take(3));
            string code2 = "html`<style>.cardTitle { color: red }</style><p class=\"cardBody\">x</p>`;\ncss`.cardFooter { color: green }`;\nhtml`<style>.|`";
            List<string> classes = Texts(code2, scoped).Where(w => w.StartsWith("card")).ToList();
            Assert.Equal(new[] { "cardFooter", "cardTitle", "cardBody" }, classes.ToArray()); // the words of the css first, the class of the html after them
        }

        [Fact]
        public void A_mistake_in_a_style_offers_css_and_no_tag()
        {
            Assert.Contains("margin", Texts("html`<div style=\"margn|\">`"));
            Assert.DoesNotContain("main", Texts("html`<div style=\"mar|\">`")); // a tag is not what a style attribute needs
            Assert.DoesNotContain(Texts("html`<div style=\"zzzcolr|\">`"), w => w == "code" || w == "clipPath");
        }

        // ---- the noise the review found ---------------------------------------------------------------------------------

        [Fact]
        public void Where_no_keyword_belongs_the_similar_words_do_not_bring_them_back()
        {
            // text between tags and the value of an attribute: only words of the file, also for a mistake
            Assert.DoesNotContain("clipPath", Texts("html`<p>colx|</p>`"));
            Assert.DoesNotContain("main", Texts("html`<nav aria-label=\"mar|\">`"));
            Assert.All(Items("html`<p class=\"zzzq|\">`"), s => Assert.Equal(SuggestionKind.Word, s.Kind));
            Assert.All(Items("sql`select 'frmo|'`"), s => Assert.Equal(SuggestionKind.Word, s.Kind));
        }

        private static List<Suggestion> Items(string codeWithCaret)
        {
            string code;
            CompletionSite site = Site(codeWithCaret, out code);
            return Engine().Suggest(code, site).ToList();
        }

        [Fact]
        public void Every_cut_of_an_html_with_styles_gives_a_site_and_a_list_without_errors()
        {
            const string sample = "html`<style media=\"print\">.a:hover { color: red; margin: 0 auto } @media (max-width: 600px) { .b { display: none } }</style><div style=\"color: ${c}; --gap: 4px\" class='x'><!-- <b style='a:b'> --><p>t</p></div>`";
            for (int cut = 5; cut <= sample.Length; cut++)
            {
                string text = sample.Substring(0, cut);
                CompletionEngine engine = Engine();
                CompletionSite site = engine.Locate(text, text.Length);
                if (site == null) continue;
                Assert.NotNull(engine.Suggest(text, site));
                for (int caret = 0; caret <= text.Length; caret += 9)
                {
                    site = engine.Locate(text, caret);
                    if (site != null) Assert.NotNull(engine.Suggest(text, site));
                }
            }
        }
    }
}
