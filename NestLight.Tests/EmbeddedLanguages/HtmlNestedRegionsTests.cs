using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Completion;
using NestLight.EmbeddedLanguages;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>
    /// Where the CSS is inside HTML has one definition, the one of the tokenizer: the regions it hands to the tokenizer of CSS are the ones the
    /// completion completes as CSS.
    /// </summary>
    public class HtmlNestedRegionsTests
    {
        private static IReadOnlyList<NestedRegion> Regions(string html)
        {
            return HtmlTokenizer.Regions(html.ToCharArray(), 0, html.Length);
        }

        private static string Text(string html, NestedRegion r) { return html.Substring(r.Start, r.End - r.Start); }

        // ---- the regions -------------------------------------------------------------------------------------------------

        [Fact]
        public void A_style_element_holds_a_style_sheet()
        {
            const string html = "<style>.a { margin: 0 }</style><p>x</p>";
            NestedRegion r = Regions(html).Single();
            Assert.Equal("css", r.EmbeddedLanguageId);
            Assert.False(r.InlineDeclarations);
            Assert.Equal(".a { margin: 0 }", Text(html, r));
        }

        [Theory]
        [InlineData("<div style=\"color: red\">")]
        [InlineData("<div style='color: red'>")]
        [InlineData("<div class=a STYLE = \"color: red\">")]
        [InlineData("<div style=\"color: red\"/>")]
        public void A_style_attribute_holds_a_list_of_declarations(string html)
        {
            NestedRegion r = Regions(html).Single();
            Assert.Equal("css", r.EmbeddedLanguageId);
            Assert.True(r.InlineDeclarations);
            Assert.Equal("color: red", Text(html, r));
        }

        [Fact]
        public void The_regions_come_in_order_of_appearance()
        {
            const string html = "<div style=\"a: b\"><style>.x{}</style><span style='c: d'></span></div>";
            IReadOnlyList<NestedRegion> regions = Regions(html);
            Assert.Equal(new[] { "a: b", ".x{}", "c: d" }, regions.Select(r => Text(html, r)).ToArray());
            Assert.Equal(new[] { true, false, true }, regions.Select(r => r.InlineDeclarations).ToArray());
            for (int i = 1; i < regions.Count; i++) Assert.True(regions[i - 1].End <= regions[i].Start);
        }

        [Fact]
        public void The_name_of_the_element_and_of_the_attribute_are_not_case_sensitive()
        {
            const string html = "<STYLE>.a{}</STYLE ><P STYLE=\"b: c\"></P>";
            Assert.Equal(new[] { ".a{}", "b: c" }, Regions(html).Select(r => Text(html, r)).ToArray());
        }

        [Fact]
        public void An_unclosed_region_runs_to_the_end_while_the_code_is_being_typed()
        {
            const string element = "<p></p><style>.a { mar";
            Assert.Equal(".a { mar", Text(element, Regions(element).Single()));
            const string attribute = "<div style=\"color: re";
            Assert.Equal("color: re", Text(attribute, Regions(attribute).Single()));
            const string unfinishedTag = "<style";
            Assert.Empty(Regions(unfinishedTag));
        }

        [Fact]
        public void An_empty_region_is_still_a_region_so_that_the_completion_can_complete_inside_it()
        {
            const string html = "<div style=\"\"></div><style></style>";
            IReadOnlyList<NestedRegion> regions = Regions(html);
            Assert.Equal(2, regions.Count);
            Assert.All(regions, r => Assert.Equal(r.Start, r.End));
            Assert.Equal(html.IndexOf("\"\"") + 1, regions[0].Start);
        }

        [Fact]
        public void The_places_that_hold_no_css_have_no_region()
        {
            Assert.Empty(Regions("<div class=\"style\">style=\"x\"</div>"));
            Assert.Empty(Regions("<div data-style=\"a: b\"></div>"));
            Assert.Empty(Regions("<!-- <style>.a{}</style> <p style=\"a: b\"> -->"));
            Assert.Empty(Regions("<div style=color:red></div>")); // an unquoted value is a value, not declarations
            Assert.Empty(Regions("<stylesheet>.a{}</stylesheet>"));
            Assert.Empty(Regions(""));
        }

        [Fact]
        public void An_interpolation_is_part_of_the_region_and_cannot_close_it()
        {
            string mask = new string(TextUtil.Mask, 8);
            string attribute = "<div style=\"color: " + mask + "; margin: 0\">";
            Assert.Equal("color: " + mask + "; margin: 0", Text(attribute, Regions(attribute).Single()));
            string element = "<style>.a { color: " + mask + " }</style>";
            Assert.Equal(".a { color: " + mask + " }", Text(element, Regions(element).Single()));
        }

        [Fact]
        public void The_regions_are_searched_only_in_the_range_given()
        {
            const string html = "<p style=\"a: b\"></p><p style=\"c: d\"></p>";
            char[] text = html.ToCharArray();
            int second = html.IndexOf("<p style=\"c");
            NestedRegion r = HtmlTokenizer.Regions(text, second, text.Length).Single();
            Assert.Equal("c: d", html.Substring(r.Start, r.End - r.Start));
        }

        // ---- one definition ------------------------------------------------------------------------------------------------

        [Fact]
        public void Only_the_html_tokenizer_nests_another_language()
        {
            IEmbeddedLanguageRegistry registry = NestLightComposition.CreateEmbeddedLanguages();
            foreach (string id in new[] { "html", "htm", "svg" }) Assert.IsAssignableFrom<INestingTokenizer>(registry.Find(id));
            foreach (string id in new[] { "css", "sql", "json", "graphql", "xml", "markdown", "yaml", "regex", "glsl", "wgsl" })
                Assert.False(registry.Find(id) is INestingTokenizer, id);
        }

        [Fact]
        public void The_instance_and_the_static_method_find_the_same_regions()
        {
            const string html = "<div style=\"a: b\"><style>.x{}</style></div>";
            IEmbeddedLanguageRegistry registry = NestLightComposition.CreateEmbeddedLanguages();
            var nesting = (INestingTokenizer)registry.Find("html");
            IReadOnlyList<NestedRegion> viaInstance = nesting.FindRegions(html.ToCharArray(), 0, html.Length), viaStatic = Regions(html);
            Assert.Equal(viaStatic.Select(r => r.Start + ":" + r.End + ":" + r.InlineDeclarations), viaInstance.Select(r => r.Start + ":" + r.End + ":" + r.InlineDeclarations));
        }

        /// <summary>The tokenizer of CSS in disguise: it only writes down what it was asked to tokenize.</summary>
        private sealed class Recorder : IEmbeddedLanguageTokenizer
        {
            public readonly List<string> Calls = new List<string>();
            public IReadOnlyList<string> Ids { get { return new[] { "css" }; } }
            public void Tokenize(char[] text, int from, int to, TokenSink emit) { Calls.Add(from + ":" + to); }
        }

        private static List<string> Delegated(string html, out List<string> found)
        {
            var recorder = new Recorder();
            var registry = new EmbeddedLanguageRegistry(r => new IEmbeddedLanguageTokenizer[] { new HtmlTokenizer(r), recorder });
            registry.Find("html").Tokenize(html.ToCharArray(), 0, html.Length, (a, b, type) => { });
            // the tokenizer skips the empty ones: there is nothing to color
            found = Regions(html).Where(r => r.End > r.Start).Select(r => r.Start + ":" + r.End).ToList();
            return recorder.Calls;
        }

        private static readonly string[] Samples =
        {
            "<style>.a { margin: 0 }</style><div style=\"color: red\">x</div>",
            "<div class=\"card\" style='a: b; c: d'><p STYLE=\"e: f\"></p><style type=\"text/css\">p { x: y }</style></div>",
            "<!-- <style>.no{}</style> --><section><style>.yes{}</style><span style=\"\"></span><b style=\"k: v\"/></section>",
            "<ul><li style=\"a: \u0001\u0001\u0001\u0001\">one</li><li style=unquoted>two</li></ul><style>.\u0001\u0001 { a: b }</style>",
            "<svg viewBox=\"0 0 10 10\"><circle style=\"fill: red\" r=\"4\"/></svg>"
        };

        [Fact]
        public void What_the_tokenizer_hands_to_the_css_tokenizer_is_what_the_regions_say()
        {
            foreach (string sample in Samples)
            {
                List<string> found;
                List<string> delegated = Delegated(sample, out found);
                Assert.Equal(found, delegated);
                Assert.NotEmpty(delegated);
            }
        }

        [Fact]
        public void The_two_agree_on_every_prefix_of_every_sample()
        {
            // while the code is typed the tag, the quote or the element may be unfinished anywhere
            foreach (string sample in Samples)
                for (int cut = 0; cut <= sample.Length; cut++)
                {
                    string html = sample.Substring(0, cut);
                    List<string> found;
                    List<string> delegated = Delegated(html, out found);
                    Assert.True(found.SequenceEqual(delegated), "cut " + cut + " of " + sample + ": " + string.Join(",", found) + " vs " + string.Join(",", delegated));
                }
        }

        // ---- the completion asks the same tokenizer --------------------------------------------------------------------------

        [Fact]
        public void The_completion_finds_the_regions_in_the_coordinates_of_the_document_with_the_interpolations_masked()
        {
            const string code = "const pad = 'xxxxxxxxxxxxxxxxxxxxxxxx';\nconst tpl = html`<div style=\"color: ${c}; margin: 0\"><style>.a { top: ${t}px }</style></div>`;\n";
            EmbeddedString owner = Pipeline.Scanner(HostLanguage.JavaScript).Scan(code).Single();
            var html = (INestedLanguages)CompletionLanguages.Default.Find("html");

            IReadOnlyList<NestedRegion> regions = html.RegionsIn(code, owner);
            Assert.Equal(2, regions.Count);
            Assert.Equal("color: ${c}; margin: 0", code.Substring(regions[0].Start, regions[0].End - regions[0].Start));
            Assert.Equal(".a { top: ${t}px }", code.Substring(regions[1].Start, regions[1].End - regions[1].Start));
            Assert.True(regions[0].InlineDeclarations);
            Assert.False(regions[1].InlineDeclarations);

            NestedRegion at;
            Assert.True(html.TryRegionAt(code, owner, code.IndexOf("margin"), out at));
            Assert.Equal(regions[0].Start, at.Start);
            Assert.True(html.TryRegionAt(code, owner, regions[0].End, out at));     // right before the closing quote
            Assert.Equal(regions[0].Start, at.Start);
            Assert.False(html.TryRegionAt(code, owner, regions[0].End + 1, out at)); // the tag itself
            Assert.True(html.TryRegionAt(code, owner, code.IndexOf("top"), out at));
            Assert.Equal(regions[1].Start, at.Start);
        }

        [Fact]
        public void A_quote_inside_an_interpolation_does_not_end_the_attribute()
        {
            const string code = "html`<div style=\"color: ${ pick(\"a\") }; margin: 0\"></div>`";
            EmbeddedString owner = Pipeline.Scanner(HostLanguage.JavaScript).Scan(code).Single();
            NestedRegion region = ((INestedLanguages)CompletionLanguages.Default.Find("html")).RegionsIn(code, owner).Single();
            Assert.Equal("color: ${ pick(\"a\") }; margin: 0", code.Substring(region.Start, region.End - region.Start));
        }

        [Fact]
        public void A_string_that_the_text_cut_short_still_has_its_regions()
        {
            const string code = "html`<p style=\"color: re";
            EmbeddedString owner = Pipeline.Scanner(HostLanguage.JavaScript).Scan(code).Single();
            NestedRegion region = ((INestedLanguages)CompletionLanguages.Default.Find("html")).RegionsIn(code, owner).Single();
            Assert.Equal("color: re", code.Substring(region.Start, region.End - region.Start));
            Assert.Equal(code.Length, region.End);
        }

        [Fact]
        public void The_css_of_a_style_attribute_is_completed_as_css_and_the_html_around_it_as_html()
        {
            const string css = "const t = html`<div style=\"disp|\">`;";
            const string markup = "const t = html`<div cl|>`;";
            var features = CompletionFeatures.Default;
            foreach (var test in new[] { new { Code = css, Expected = "display" }, new { Code = markup, Expected = "class" } })
            {
                int caret = test.Code.IndexOf('|');
                string code = test.Code.Remove(caret, 1);
                var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), features: features);
                CompletionSite site = engine.Locate(code, caret);
                Assert.Contains(test.Expected, engine.Suggest(code, site).Select(s => s.Text));
            }
        }
    }
}
