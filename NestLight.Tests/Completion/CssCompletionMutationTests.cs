using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class CssCompletionMutationTests
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
            if (site == null) return new List<string>();
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

        // ---- TryAdjustWordStart: line 22 digits++ and boundary ----

        [Fact]
        public void Adjust_word_start_negative_number_with_unit()
        {
            // Line 22: text[digits] == '-' causes digits++ (not digits--)
            ICompletionLanguage css = CompletionLanguages.Default.Find("css");
            string text = "a { margin: -10p";
            int start = text.IndexOf("-10p");
            Assert.True(css.TryAdjustWordStart(text, 0, ref start, text.Length));
            Assert.Equal(text.IndexOf("p"), start);
        }

        [Fact]
        public void Adjust_word_start_no_digits_returns_true()
        {
            // Line 25: unit == digits means no digits, returns true immediately
            ICompletionLanguage css = CompletionLanguages.Default.Find("css");
            string text = "a { color: red";
            int start = text.IndexOf("red");
            Assert.True(css.TryAdjustWordStart(text, 0, ref start, text.Length));
            Assert.Equal(text.IndexOf("red"), start);
        }

        [Fact]
        public void Adjust_word_start_boundary_start_equals_ownerStart()
        {
            // Line 26: start > ownerStart with # test
            ICompletionLanguage css = CompletionLanguages.Default.Find("css");
            // When start == ownerStart, text[start-1] check is skipped
            string text = "10px";
            int start = 0;
            Assert.True(css.TryAdjustWordStart(text, 0, ref start, text.Length));
            Assert.Equal(2, start); // adjusted past digits
        }

        [Fact]
        public void Adjust_word_start_non_letter_in_suffix_returns_false()
        {
            // Line 27: non-letter in suffix after digits
            ICompletionLanguage css = CompletionLanguages.Default.Find("css");
            string text = "a { width: 10p.x";
            int start = text.IndexOf("10p");
            Assert.False(css.TryAdjustWordStart(text, 0, ref start, text.Length));
        }

        // ---- ReadPosition: comment detection (lines 47-53) ----

        [Fact]
        public void Comment_inside_css_gives_comment_position()
        {
            Assert.Equal("css:comment", Name("css`.a { /* |`"));
        }

        [Fact]
        public void Comment_closed_before_caret_continues_parsing()
        {
            // Line 49-51: close >= 0 && close < site.Start
            Assert.NotEqual("css:comment", Name("css`.a { /* c */ color: |}`"));
        }

        // ---- String detection (lines 54-59) ----

        [Fact]
        public void Double_quoted_string_gives_string_position()
        {
            Assert.Equal("css:string", Name("css`.a { content: \"|`"));
        }

        [Fact]
        public void Single_quoted_string_gives_string_position()
        {
            Assert.Equal("css:string", Name("css`.a { content: '|`"));
        }

        // ---- Parentheses and brackets (lines 61-64) ----

        [Fact]
        public void Parens_tracking_for_media_query()
        {
            // Line 61: c == '(' increments parens
            // Line 62: c == ')' decrements parens when > 0
            Assert.Equal("css:media-feature", Name("css`@media (|`"));
        }

        [Fact]
        public void Brackets_give_attribute_selector()
        {
            // Line 63-64: bracket tracking
            Assert.Equal("css:attribute-selector", Name("css`input[|`"));
        }

        // ---- Block detection: keyframes (lines 72-77) ----

        [Fact]
        public void Keyframes_block_gives_keyframe_selector()
        {
            // Line 74: prelude contains "keyframes" -> CssBlock.Keyframes
            Assert.Equal("css:keyframe-selector", Name("css`@keyframes fade { |`"));
            var items = Texts("css`@keyframes fade { fro|`");
            Assert.Contains("from", items);
        }

        [Fact]
        public void Nested_media_rule_block()
        {
            // Line 75: CssRuleAtRules match -> CssBlock.Rules
            Assert.Equal("css:property", Name("css`@media screen { .a { | } }`"));
        }

        // ---- Block closing (line 79) ----

        [Fact]
        public void After_closing_brace_new_block_starts()
        {
            // Line 79: '}' pops block and resets declarationStart
            Assert.Equal("css:selector", Name("css`.a { color: red } .b |`"));
        }

        // ---- Semicolon resets (line 80) ----

        [Fact]
        public void Semicolon_resets_colon_tracking()
        {
            // Line 80: ';' resets declarationStart and colon
            Assert.Equal("css:property", Name("css`.a { color: red; |}`"));
        }

        // ---- Colon detection (line 81) ----

        [Fact]
        public void First_colon_in_declarations_is_tracked()
        {
            // Line 81: colon < 0 means first colon is tracked
            Assert.StartsWith("css:value:", Name("css`.a { display: |}`"));
        }

        // ---- Parentheses with digits (lines 94, 99-100) ----

        [Fact]
        public void Digit_in_parens_in_declarations_gives_unit()
        {
            // Line 99-100: inside declarations, digit gives unit; outside doesn't
            Assert.Equal("css:unit", Name("css`.a { transform: rotate(45|)}`"));
        }

        [Fact]
        public void Digit_in_parens_in_selector_gives_null()
        {
            // Line 100: outside declarations, returns null for nth-child(2n)
            Assert.Null(Name("css`li:nth-child(2|n) {}`"));
        }

        // ---- At-rules (lines 109-116) ----

        [Fact]
        public void At_rule_with_space_gives_content_position()
        {
            // Line 112-113: space < 0 gives at-rule name, space >= 0 gives content
            Assert.Equal("css:at-rule", Name("css`@|`"));
            Assert.Equal("css:media-query", Name("css`@media |`"));
        }

        [Fact]
        public void Keyframes_name_position()
        {
            // Line 114: keyframes gives keyframes-name position after the name
            Assert.Equal("css:keyframes-name", Name("css`@keyframes |`"));
        }

        // ---- Selector positions (lines 121-132) ----

        [Fact]
        public void Selector_position_for_element()
        {
            // Line 132: default selector position
            Assert.Equal("css:selector", Name("css`|`"));
        }

        [Fact]
        public void Pseudo_class_after_single_colon()
        {
            // Line 125-130: ':' gives pseudo-class
            Assert.Equal("css:pseudo-class", Name("css`div:|`"));
        }

        [Fact]
        public void Pseudo_element_after_double_colon()
        {
            // Line 127-129: '::' gives pseudo-element
            Assert.Equal("css:pseudo-element", Name("css`div::|`"));
        }

        [Fact]
        public void Class_selector_after_dot()
        {
            // Line 124: '.' gives class position
            Assert.Equal("css:class", Name("css`.|`"));
        }

        [Fact]
        public void Id_selector_after_hash()
        {
            // Line 124: '#' gives class position (id)
            Assert.Equal("css:class", Name("css`#|`"));
        }

        // ---- Property position (line 135-136) ----

        [Fact]
        public void Property_position_when_no_colon()
        {
            // Line 135: colon < 0 gives property position
            Assert.Equal("css:property", Name("css`.a { |}`"));
        }

        // ---- Hex color (line 138) ----

        [Fact]
        public void Hex_color_position_in_value()
        {
            Assert.Equal("css:hex", Name("css`.a { color: #|}`"));
        }

        // ---- Important (line 139) ----

        [Fact]
        public void Important_after_exclamation()
        {
            Assert.Equal("css:important", Name("css`.a { color: red !|}`"));
        }

        // ---- Value position with property extraction (lines 142-149) ----

        [Fact]
        public void Value_position_includes_property_name()
        {
            // Lines 143-146: extracting property before colon
            Assert.Equal("css:value:display", Name("css`.a { display: |}`"));
            Assert.Equal("css:value:color", Name("css`.a { color: |}`"));
        }

        [Fact]
        public void Value_position_filters_unlikely_words()
        {
            // Line 148: unlikely filter: !IsValueOnly(w) && !values.Contains(w)
            var features = new CompletionFeatures(grammar: true);
            int caret = "css`.a { display: ".Length;
            string code = "css`.a { display: }`";
            code = code.Remove(caret, 0);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000,
                CompletionEngine.DefaultMinWordLength, features: features);
            var site = engine.Locate(code, caret);
            Assert.NotNull(site);
        }

        // ---- IsValueOnly and IsKeyword (lines 228-231, 388-391) ----

        [Fact]
        public void IsValueOnly_true_for_value_not_property()
        {
            // Line 230: CssValueSet.Contains(word) && !CssPropertySet.Contains(word)
            Assert.True(CssCompletion.IsValueOnly("inherit"));
            Assert.True(CssCompletion.IsValueOnly("initial"));
            Assert.True(CssCompletion.IsValueOnly("unset"));
        }

        [Fact]
        public void IsValueOnly_false_for_word_that_is_also_property()
        {
            // "flex" is both a value and a property
            Assert.False(CssCompletion.IsValueOnly("flex"));
            Assert.False(CssCompletion.IsValueOnly("grid"));
        }

        [Fact]
        public void IsValueOnly_false_for_property_not_value()
        {
            Assert.False(CssCompletion.IsValueOnly("margin"));
            Assert.False(CssCompletion.IsValueOnly("padding"));
        }

        [Fact]
        public void IsKeyword_true_for_properties_and_values()
        {
            Assert.True(CssCompletion.IsKeyword("display"));
            Assert.True(CssCompletion.IsKeyword("inline"));
            Assert.True(CssCompletion.IsKeyword("inherit"));
        }

        [Fact]
        public void IsKeyword_false_for_unknown_words()
        {
            Assert.False(CssCompletion.IsKeyword("foobar"));
            Assert.False(CssCompletion.IsKeyword("notacssprop"));
        }

        // ---- Block removal mutation on line 389 ----

        [Fact]
        public void IsKeyword_handles_case_insensitively()
        {
            Assert.True(CssCompletion.IsKeyword("DISPLAY"));
            Assert.True(CssCompletion.IsKeyword("Display"));
        }

        // ---- Selector unlikely filter (line 132) ----

        [Fact]
        public void Selector_position_filters_css_keywords_that_are_not_tags()
        {
            // Line 132: unlikely: w => IsKeyword(w) && !HtmlCompletion.IsTag(w)
            // "display" is a CSS keyword but not an HTML tag, so it should be unlikely
            // "div" is both a CSS keyword (no) and an HTML tag, so it should not be unlikely
            var features = new CompletionFeatures(grammar: true);
            string codeWithCaret = "css`d|`";
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000,
                CompletionEngine.DefaultMinWordLength, features: features);
            var site = engine.Locate(code, caret);
            var items = engine.Suggest(code, site).ToList();
            // Tags like "div" should appear before CSS-only keywords like "display"
            int divIdx = items.FindIndex(s => s.Text == "div");
            int displayIdx = items.FindIndex(s => s.Text == "display");
            Assert.True(divIdx >= 0);
            Assert.True(displayIdx >= 0);
            Assert.True(divIdx < displayIdx);
        }

        // ---- ValuesOf (lines 323-328) ----

        [Fact]
        public void ValuesOf_returns_specific_values_for_known_property()
        {
            var values = CssCompletion.ValuesOf("display");
            Assert.Contains("block", values);
            Assert.Contains("flex", values);
        }

        [Fact]
        public void ValuesOf_returns_general_values_for_unknown_property()
        {
            var values = CssCompletion.ValuesOf("unknown-prop");
            Assert.Contains("inherit", values);
        }

        [Fact]
        public void ValuesOf_returns_general_values_for_null()
        {
            var values = CssCompletion.ValuesOf(null);
            Assert.Contains("inherit", values);
        }

        // ---- FunctionsFor (lines 375-381) ----

        [Fact]
        public void FunctionsFor_returns_specific_functions_for_transform()
        {
            var functions = CssCompletion.FunctionsFor("transform");
            Assert.Contains("translate", functions);
            Assert.Contains("rotate", functions);
        }

        [Fact]
        public void FunctionsFor_includes_colors_for_shorthand_properties()
        {
            // Line 378-379: CssColorShorthands adds color functions
            var functions = CssCompletion.FunctionsFor("background");
            Assert.Contains("linear-gradient", functions);
        }

        [Fact]
        public void FunctionsFor_returns_common_functions_for_unknown()
        {
            var functions = CssCompletion.FunctionsFor("z-index");
            Assert.Contains("var", functions);
            Assert.Contains("calc", functions);
        }

        // ---- Inline declarations (line 41, site.InlineDeclarations) ----

        [Fact]
        public void Inline_declarations_start_inside_declarations()
        {
            // The style attribute's CSS is inline declarations: property position right away
            Assert.Equal("css:property", Name("html`<div style=\"|\">`"));
        }

        [Fact]
        public void Inline_declarations_value_after_colon()
        {
            Assert.Equal("css:value:color", Name("html`<div style=\"color: |\">`"));
        }
    }
}
