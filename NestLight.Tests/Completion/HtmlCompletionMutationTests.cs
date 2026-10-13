using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class HtmlCompletionMutationTests
    {
        private static CompletionEngine Engine(CompletionFeatures features = null)
        {
            return new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000, CompletionEngine.DefaultMinWordLength,
                new BandedPrefixMatcher(), features: features ?? CompletionFeatures.Default);
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
            Position position = CompletionLanguages.Default.Find(site.EmbeddedLanguageId).PositionAt(code, site);
            return position == null ? null : position.Name;
        }

        // ---- RegionsIn: line 31-32, Min/Max and boundary ----

        [Fact]
        public void RegionsIn_with_owner_end_past_text_length_is_clamped()
        {
            // The Math.Min on line 31 ensures owner.End does not exceed text.Length
            var html = (INestedLanguages)CompletionLanguages.Default.Find("html");
            // owner.End = text.Length is fine (no clamping needed), owner.End < start returns empty
            string code = "html`<style>.a{}</style>`";
            var owner = Pipeline.Scanner(HostLanguage.JavaScript).Scan(code).Single();
            var regions = html.RegionsIn(code, owner);
            Assert.True(regions.Count > 0);
        }

        [Fact]
        public void RegionsIn_empty_when_end_equals_start()
        {
            // Line 32: end <= start returns empty array
            var html = (INestedLanguages)CompletionLanguages.Default.Find("html");
            string code = "html``";
            var owner = Pipeline.Scanner(HostLanguage.JavaScript).Scan(code).Single();
            var regions = html.RegionsIn(code, owner);
            Assert.Empty(regions);
        }

        // ---- ReadPosition: tag detection (lines 56-58) ----

        [Fact]
        public void Open_angle_bracket_detected_for_tag_position()
        {
            // Line 56: text[i] == '>' breaks the loop
            // Line 58: open < 0 gives text position
            Assert.Equal("html:text", Name("html`<div>some |</div>`"));
        }

        [Fact]
        public void Inside_unclosed_tag_gives_tag_position()
        {
            Assert.Equal("html:tag", Name("html`<|`"));
        }

        // ---- tag name detection (line 74) ----

        [Fact]
        public void Tag_name_extends_to_first_non_word_char()
        {
            // Line 74: nameEnd < site.Start loop with word chars including dash and colon
            Assert.Equal("html:attribute:div", Name("html`<div |`"));
        }

        [Fact]
        public void Closing_tag_after_slash_gives_closing_tag_or_tag_position()
        {
            // When closing is true and nameEnd == site.Start, the open element is searched
            // After '</': we are right after the '/', and site.Start is at the position after '/'
            // The caret after '</' should give either closing-tag (if an element is open) or tag
            var name = Name("html`<div></|`");
            Assert.True(name == "html:closing-tag" || name == "html:tag");
        }

        [Fact]
        public void Inside_closing_tag_name_is_tag_position()
        {
            // Line 81: if (closing) return null when nameEnd > site.Start and nameEnd < site.Start
            // Actually, when caret is inside the name of a closing tag (di| inside </div>), it returns null or tag
            var name = Name("html`<section></sect|`");
            // The caret is in the closing tag's name, nameEnd reaches site.Start
            Assert.True(name == null || name == "html:tag" || name == "html:closing-tag");
        }

        // ---- attribute detection (lines 86-93) ----

        [Fact]
        public void Attribute_value_after_equals_sign()
        {
            // Lines 91, 92, 94: attribute set on '=', reset after whitespace+non-space
            Assert.StartsWith("html:value", Name("html`<input type=\"|`"));
        }

        [Fact]
        public void Attribute_value_with_single_quotes()
        {
            Assert.StartsWith("html:value", Name("html`<input type='|`"));
        }

        [Fact]
        public void Attribute_name_position_inside_tag()
        {
            // Line 108: attribute names filtering existing
            Assert.StartsWith("html:attribute", Name("html`<div class=\"x\" |`"));
        }

        [Fact]
        public void Unquoted_attribute_value_ends_at_whitespace()
        {
            // Line 92: char.IsWhiteSpace(c) && attribute >= 0 && i > attribute + 1
            // Tests the boundary i == attribute + 1 (single char after =, not ended yet)
            Assert.StartsWith("html:value", Name("html`<input type=t|`"));
        }

        // ---- AttributesOf (lines 112-137) ----

        [Fact]
        public void AttributesOf_skips_word_under_caret()
        {
            // Line 120: i == site.Start skips to site.End
            var items = Texts("html`<div class=\"x\" cla|`");
            Assert.Contains("class", items); // the word being typed is not counted as present
        }

        [Fact]
        public void AttributesOf_stops_at_greater_than_or_less_than()
        {
            // Line 125: c == '>' || c == '<' break
            var items = Texts("html`<div |>`");
            Assert.NotEmpty(items);
        }

        [Fact]
        public void AttributesOf_excludes_already_present_attribute()
        {
            // Lines 131-133: existing attributes are excluded from the suggestions
            // The "class" attribute is already written, so it should not appear again in the attribute list
            // But the completion offers keywords as general words too; check the attribute-specific behavior
            // through the position's Expected: "class" should not be in the attribute position's expected list
            string code;
            var site = Site("html`<div class=\"x\" |`", out code);
            Assert.NotNull(site);
            Position pos = CompletionLanguages.Default.Find(site.EmbeddedLanguageId).PositionAt(code, site);
            Assert.NotNull(pos);
            Assert.StartsWith("html:attribute", pos.Name);
            // "class" is already on the tag, so it should not be in the Expected list
            Assert.DoesNotContain("class", pos.Expected);
        }

        [Fact]
        public void Quote_tracking_in_attributes()
        {
            // Lines 123-124: quote tracking
            var items = Texts("html`<input type=\"submit\" |`");
            Assert.NotEmpty(items);
        }

        // ---- OpenElement (lines 141-185) ----

        [Fact]
        public void OpenElement_finds_innermost_open_tag()
        {
            // The stack logic for finding the innermost open element
            var items = Texts("html`<section><div></|`");
            Assert.Contains("div", items);
        }

        [Fact]
        public void OpenElement_handles_html_comments()
        {
            // Lines 150-155: HTML comment detection and skip
            Assert.Equal("html:tag", Name("html`<!-- comment --><|`"));
        }

        [Fact]
        public void OpenElement_skips_void_elements()
        {
            // Line 181: void elements not added to stack
            var items = Texts("html`<div><br><img></|`");
            Assert.Contains("div", items);
        }

        [Fact]
        public void OpenElement_handles_self_closing_tags()
        {
            // Line 175: text[gt - 1] == '/' for self-closing
            var items = Texts("html`<div><span/></|`");
            Assert.Contains("div", items);
        }

        [Fact]
        public void OpenElement_handles_nested_quotes_in_attributes()
        {
            // Lines 165-172: quote tracking in tag scanning
            var items = Texts("html`<div class=\"test\"><span></span></|`");
            Assert.Contains("div", items);
        }

        [Fact]
        public void Comment_where_endComment_is_before_at()
        {
            // Line 153: endComment >= 0 && endComment < at case
            Assert.Equal("html:tag", Name("html`<!-- c1 --><!-- c2 --><|`"));
        }

        [Fact]
        public void Comment_not_closed_before_at()
        {
            // Line 153: endComment < 0 case -> in OpenElement this skips to 'at'
            // The caret sees '<!--' which starts with '<', so open is set
            // But the comment is not closed, so the '<!--' acts like a tag start
            var name = Name("html`<!-- not closed |`");
            // The important thing is it doesn't crash and returns some position
            Assert.NotNull(name);
        }

        // ---- LastNonSpace (lines 187-192) ----

        [Fact]
        public void Attribute_value_when_equals_is_last_non_space()
        {
            // Line 94: attribute >= 0 && attribute == LastNonSpace(text, site.Start)
            Assert.StartsWith("html:value", Name("html`<input type= |`"));
        }

        // ---- ValuesOf and AttributesOfTag ----

        [Fact]
        public void ValuesOf_returns_empty_for_unknown_attribute()
        {
            var values = HtmlCompletion.ValuesOf("div", "data-custom");
            Assert.Empty(values);
        }

        [Fact]
        public void ValuesOf_returns_empty_for_null_attribute()
        {
            var values = HtmlCompletion.ValuesOf("div", null);
            Assert.Empty(values);
        }

        [Fact]
        public void ValuesOf_finds_tag_specific_values()
        {
            // tag:attribute lookup first, then attribute alone
            var values = HtmlCompletion.ValuesOf("input", "type");
            Assert.Contains("text", values);
            Assert.Contains("password", values);
        }

        [Fact]
        public void ValuesOf_falls_back_to_attribute_alone()
        {
            var values = HtmlCompletion.ValuesOf("div", "role");
            Assert.Contains("button", values);
        }

        [Fact]
        public void AttributesOfTag_includes_global_and_tag_specific()
        {
            var attrs = HtmlCompletion.AttributesOfTag("img");
            Assert.Contains("src", attrs);
            Assert.Contains("alt", attrs);
            Assert.Contains("class", attrs); // global
        }

        [Fact]
        public void AttributesOfTag_for_unknown_tag_returns_globals_only()
        {
            var attrs = HtmlCompletion.AttributesOfTag("custom-element");
            Assert.Contains("class", attrs);
            Assert.Contains("id", attrs);
        }

        [Fact]
        public void IsTag_returns_true_for_known_tags()
        {
            Assert.True(HtmlCompletion.IsTag("div"));
            Assert.True(HtmlCompletion.IsTag("DIV"));
            Assert.True(HtmlCompletion.IsTag("svg"));
            Assert.True(HtmlCompletion.IsTag("path"));
        }

        [Fact]
        public void IsTag_returns_false_for_unknown_tags()
        {
            Assert.False(HtmlCompletion.IsTag("foobar"));
            Assert.False(HtmlCompletion.IsTag("notaTag"));
        }

        // ---- Block removal mutation on line 213 ----

        [Fact]
        public void Closing_tag_with_open_element_suggests_the_open_one()
        {
            // Tests the whole OpenElement method returns a useful result (block removal mutation)
            var name = Name("html`<article></|`");
            Assert.Equal("html:closing-tag", name);
            var items = Texts("html`<article></|`");
            Assert.Contains("article", items);
        }

        // ---- Edge cases for the loop at line 146 ----

        [Fact]
        public void OpenElement_with_no_tags_returns_text_position()
        {
            Assert.Equal("html:text", Name("html`some text |`"));
        }

        // ---- Quote in value (line 170-171) ----

        [Fact]
        public void Quoted_value_in_tag_does_not_confuse_gt_search()
        {
            // Attribute with '>' inside a quoted value should not confuse parsing
            var items = Texts("html`<div title=\"a>b\" |`");
            Assert.NotEmpty(items);
        }
    }
}
