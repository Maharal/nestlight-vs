using NestLight.Common;
using System.Linq;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>CSS tokenization rules inside css`...`.</summary>
    public class CssTokenizerTests
    {
        private static string[] T(string css, string type) { return Lexer.Texts(Lexer.C(css), type); }

        // ---- structure ------------------------------------------------------------

        [Fact]
        public void Rule_produces_selector_property_value_and_punctuation()
        {
            Assert.Equal(new[]
            {
                "css.selector|a", "css.punctuation|{",
                "css.property|color", "css.punctuation|:", "css.value|red", "css.punctuation|;",
                "css.punctuation|}"
            }, Lexer.Seq(Lexer.C("a { color: red; }")));
        }

        [Fact]
        public void Empty_css_template_has_no_tokens()
        {
            Assert.Empty(Lexer.Lex(Lexer.C("")));
        }

        [Fact]
        public void Several_declarations_on_one_line_without_final_semicolon()
        {
            Assert.Equal(new[] { "b", "c", "d" }, T("a { b: 1; c: 2; d: 3 }", ClassificationNames.CssProperty));
        }

        [Fact]
        public void Marker_comment_enables_css_for_a_plain_template()
        {
            Assert.Equal(new[] { "b" }, Lexer.Texts("const x = /* css */ `a { b: c }`;", ClassificationNames.CssProperty));
        }

        // ---- selectors -------------------------------------------------------------

        [Fact]
        public void Selector_parts_are_classified_separately()
        {
            string css = ".card#main:hover::before > p { }";
            Assert.Equal(new[] { ".card", "#main" }, T(css, ClassificationNames.CssSelectorClass));
            Assert.Equal(new[] { ":hover", "::before" }, T(css, ClassificationNames.CssPseudo));
            Assert.Equal(new[] { "p" }, T(css, ClassificationNames.CssSelector));
            Assert.Contains(">", T(css, ClassificationNames.CssPunct)); // a combinator is punctuation, not a name
        }

        [Fact]
        public void Host_pseudo_class()
        {
            Assert.Equal(new[] { ":host" }, T(":host { }", ClassificationNames.CssPseudo));
            Assert.Empty(T(":host { }", ClassificationNames.CssSelector));
        }

        [Fact]
        public void Functional_pseudo_class_exposes_the_inner_selector()
        {
            string css = ":not(.x) { }";
            Assert.Equal(new[] { ":not" }, T(css, ClassificationNames.CssPseudo));
            Assert.Equal(new[] { ".x" }, T(css, ClassificationNames.CssSelectorClass));
        }

        [Fact]
        public void Attribute_selector_string_is_a_string_token()
        {
            Assert.Equal(new[] { "\"x\"" }, T("a[href=\"x\"] { }", ClassificationNames.CssString));
        }

        [Fact]
        public void Selector_list_and_combinators()
        {
            string css = "a, b > c + d ~ e { }";
            Assert.Equal(new[] { "a", "b", "c", "d", "e" }, T(css, ClassificationNames.CssSelector));
            Assert.Equal(new[] { ",", ">", "+", "~", "{", "}" }, T(css, ClassificationNames.CssPunct));
        }

        [Fact]
        public void Nesting_with_ampersand()
        {
            string css = "a { &:hover { color: red } }";
            Assert.Equal(new[] { "a", "&" }, T(css, ClassificationNames.CssSelector));
            Assert.Equal(new[] { ":hover" }, T(css, ClassificationNames.CssPseudo));
            Assert.Equal(new[] { "color" }, T(css, ClassificationNames.CssProperty));
        }

        // ---- values ----------------------------------------------------------------

        [Fact]
        public void Custom_properties_in_declarations_and_in_var()
        {
            string css = "a { --gap: 8px; color: var(--text, #333); }";
            Assert.Equal(new[] { "--gap", "--text" }, T(css, ClassificationNames.CssCustomProperty));
            Assert.Equal(new[] { "color" }, T(css, ClassificationNames.CssProperty));
            Assert.Equal(new[] { "var" }, T(css, ClassificationNames.CssFunction));
            Assert.Equal(new[] { "8px", "#333" }, T(css, ClassificationNames.CssNumber));
        }

        [Fact]
        public void Numbers_with_units_signs_and_decimals()
        {
            string css = "a { b: 0 1.5rem -2px .5em +3% 10 }";
            Assert.Equal(new[] { "0", "1.5rem", "-2px", ".5em", "+3%", "10" }, T(css, ClassificationNames.CssNumber));
            Assert.Empty(T(css, ClassificationNames.CssValue));
        }

        [Fact]
        public void Hex_colors_are_numbers()
        {
            Assert.Equal(new[] { "#fff", "#00ff00" }, T("a { c: #fff #00ff00 }", ClassificationNames.CssNumber));
        }

        [Fact]
        public void Important_flag_with_and_without_space()
        {
            Assert.Equal(new[] { "!important", "! important" },
                T("a { b: c !important; d: e ! important }", ClassificationNames.CssAtRule));
        }

        [Fact]
        public void Hyphenated_identifiers_are_single_values()
        {
            Assert.Equal(new[] { "-webkit-box", "sans-serif" }, T("a { b: -webkit-box sans-serif }", ClassificationNames.CssValue));
        }

        [Fact]
        public void Minus_operator_between_spaces_is_punctuation()
        {
            string css = "a { w: calc(100% - 2rem) }";
            Assert.Equal(new[] { "calc" }, T(css, ClassificationNames.CssFunction));
            Assert.Equal(new[] { "100%", "2rem" }, T(css, ClassificationNames.CssNumber));
            Assert.Contains("-", T(css, ClassificationNames.CssPunct));
        }

        [Fact]
        public void Quoted_strings_in_values()
        {
            string css = "a { f: \"Open Sans\", sans-serif; g: 'x' }";
            Assert.Equal(new[] { "\"Open Sans\"", "'x'" }, T(css, ClassificationNames.CssString));
            Assert.Equal(new[] { "sans-serif" }, T(css, ClassificationNames.CssValue));
        }

        [Fact]
        public void Unquoted_url_content_is_a_string()
        {
            string css = "a { b: url(img/a.png) }";
            Assert.Equal(new[] { "url" }, T(css, ClassificationNames.CssFunction));
            Assert.Equal(new[] { "img/a.png" }, T(css, ClassificationNames.CssString));
        }

        [Fact]
        public void Quoted_url_keeps_the_quotes_in_the_string_token()
        {
            Assert.Equal(new[] { "\"a.png\"" }, T("a { b: url(\"a.png\") }", ClassificationNames.CssString));
        }

        [Fact]
        public void Semicolon_inside_parentheses_does_not_end_the_declaration()
        {
            string css = "a { b: url(data:image/png;base64,AAA); c: d }";
            Assert.Equal(new[] { "b", "c" }, T(css, ClassificationNames.CssProperty));
            Assert.Equal(new[] { "data:image/png;base64,AAA" }, T(css, ClassificationNames.CssString));
        }

        // ---- at-rules ---------------------------------------------------------------

        [Fact]
        public void At_rule_with_block_and_nested_rule()
        {
            string css = "@media (min-width: 600px) { .a { width: 1px } }";
            Assert.Equal(new[] { "@media" }, T(css, ClassificationNames.CssAtRule));
            Assert.Equal(new[] { "min-width", "width" }, T(css, ClassificationNames.CssProperty)); // the media feature is named like a property
            Assert.Empty(T(css, ClassificationNames.CssValue));
            Assert.Equal(new[] { "600px", "1px" }, T(css, ClassificationNames.CssNumber));
            Assert.Equal(new[] { ".a" }, T(css, ClassificationNames.CssSelectorClass));
        }

        [Fact]
        public void At_rule_without_block_ends_at_semicolon()
        {
            string css = "@import url(x.css); a { b: c }";
            Assert.Equal(new[] { "@import" }, T(css, ClassificationNames.CssAtRule));
            Assert.Equal(new[] { "x.css" }, T(css, ClassificationNames.CssString));
            Assert.Equal(new[] { "a" }, T(css, ClassificationNames.CssSelector));
            Assert.Equal(new[] { "b" }, T(css, ClassificationNames.CssProperty));
        }

        [Fact]
        public void Font_face_block_contains_declarations()
        {
            string css = "@font-face { font-family: x; }";
            Assert.Equal(new[] { "@font-face" }, T(css, ClassificationNames.CssAtRule));
            Assert.Equal(new[] { "font-family" }, T(css, ClassificationNames.CssProperty));
        }

        [Fact]
        public void Keyframes_contain_selector_like_rules()
        {
            string css = "@keyframes spin { from { a: 0 } to { a: 1 } }";
            Assert.Equal(new[] { "@keyframes" }, T(css, ClassificationNames.CssAtRule));
            Assert.Equal(new[] { "spin" }, T(css, ClassificationNames.CssValue));
            Assert.Equal(new[] { "from", "to" }, T(css, ClassificationNames.CssSelector));
            Assert.Equal(new[] { "a", "a" }, T(css, ClassificationNames.CssProperty));
        }

        // ---- comments --------------------------------------------------------------

        [Fact]
        public void Comment_before_a_rule()
        {
            Assert.Equal(new[] { "/* c */" }, T("/* c */ a { b: c }", ClassificationNames.CssComment));
        }

        [Fact]
        public void Comment_inside_a_value()
        {
            string css = "a { b: /* x */ red }";
            Assert.Equal(new[] { "/* x */" }, T(css, ClassificationNames.CssComment));
            Assert.Equal(new[] { "red" }, T(css, ClassificationNames.CssValue));
        }

        [Fact]
        public void Comment_hides_structural_characters()
        {
            string css = "a { /* x; y } */ b: c }";
            Assert.Equal(new[] { "/* x; y } */" }, T(css, ClassificationNames.CssComment));
            Assert.Equal(new[] { "b" }, T(css, ClassificationNames.CssProperty));
        }

        [Fact]
        public void Unterminated_comment_runs_to_the_end()
        {
            Assert.Equal(new[] { "/* x" }, T("a { b: c /* x", ClassificationNames.CssComment));
        }

        // ---- incomplete code (typing) ---------------------------------------------------

        [Fact]
        public void Property_name_without_colon_is_still_a_property()
        {
            Assert.Equal(new[] { "col" }, T("a { col", ClassificationNames.CssProperty));
        }

        [Fact]
        public void Property_with_colon_and_no_value()
        {
            string css = "a { color:";
            Assert.Equal(new[] { "color" }, T(css, ClassificationNames.CssProperty));
            Assert.Contains(":", T(css, ClassificationNames.CssPunct));
        }

        [Fact]
        public void Stray_closing_brace_does_not_break_following_rules()
        {
            string css = "} a { b: c }";
            Assert.Equal(new[] { "a" }, T(css, ClassificationNames.CssSelector));
            Assert.Equal(new[] { "b" }, T(css, ClassificationNames.CssProperty));
        }

        [Fact]
        public void Unterminated_string_runs_to_the_end()
        {
            Assert.Equal(new[] { "\"abc }" }, T("a { content: \"abc }", ClassificationNames.CssString));
        }

        // ---- ${...} expressions ------------------------------------------------------------------

        [Fact]
        public void Expression_followed_by_unit_is_a_number()
        {
            string css = "a { p: ${g}px 1rem }";
            Assert.Equal(new[] { "px", "1rem" }, T(css, ClassificationNames.CssNumber));
            Assert.Equal(new[] { "g" }, T(css, ClassificationNames.Expression));
            Assert.Empty(T(css, ClassificationNames.CssValue));
        }

        [Fact]
        public void Expression_as_the_whole_value_leaves_no_value_token()
        {
            string css = "a { color: ${c}; }";
            Assert.Equal(new[] { "color" }, T(css, ClassificationNames.CssProperty));
            Assert.Empty(T(css, ClassificationNames.CssValue));
            Assert.Empty(T(css, ClassificationNames.CssNumber));
            Assert.Equal(new[] { "c" }, T(css, ClassificationNames.Expression));
        }

        [Fact]
        public void Expression_as_property_name_leaves_no_property_token()
        {
            string css = "a { ${p}: red }";
            Assert.Empty(T(css, ClassificationNames.CssProperty));
            Assert.Equal(new[] { "red" }, T(css, ClassificationNames.CssValue));
        }

        [Fact]
        public void Mixin_expression_before_a_declaration_is_skipped_cleanly()
        {
            // no extra whitespace in the token: "b", not " b"
            Assert.Equal(new[] { "b" }, T("a { ${mixin} b: c }", ClassificationNames.CssProperty));
        }

        [Fact]
        public void Expression_before_a_rule_does_not_become_part_of_the_selector()
        {
            string css = "${shared}\na { b: c }";
            Assert.Equal(new[] { "a" }, T(css, ClassificationNames.CssSelector));
            Assert.Equal(new[] { "b" }, T(css, ClassificationNames.CssProperty));
        }

        [Fact]
        public void Trailing_expression_statement_produces_no_css_tokens()
        {
            string css = "a { b: c }\n${shared}";
            Assert.Equal(new[] { "b" }, T(css, ClassificationNames.CssProperty));
            Assert.Equal(new[] { "c" }, T(css, ClassificationNames.CssValue));
            Assert.Equal(new[] { "shared" }, T(css, ClassificationNames.Expression));
        }

        // ---- selectors with arguments, attribute selectors, keyframes -----------------------------------------------

        [Fact]
        public void Functional_pseudo_classes_hold_selectors_and_formulas()
        {
            string css = "a:not(.b, #c) > li:nth-child(2n+1):is(.d, p) { }";
            Assert.Equal(new[] { ":not", ":nth-child", ":is" }, T(css, ClassificationNames.CssPseudo));
            Assert.Equal(new[] { ".b", "#c", ".d" }, T(css, ClassificationNames.CssSelectorClass));
            Assert.Equal(new[] { "a", "li", "p" }, T(css, ClassificationNames.CssSelector));
            Assert.Equal(new[] { "2n", "+1" }, T(css, ClassificationNames.CssNumber));
            Assert.DoesNotContain("(", T(css, ClassificationNames.CssSelector));
            Assert.DoesNotContain(")", T(css, ClassificationNames.CssSelector));
        }

        [Fact]
        public void An_attribute_selector_has_a_name_an_operator_and_a_value()
        {
            string css = "input[type=\"text\" i], a[href^=http], [disabled] { }";
            Assert.Equal(new[] { "type", "href", "disabled" }, T(css, ClassificationNames.CssProperty));
            Assert.Equal(new[] { "\"text\"" }, T(css, ClassificationNames.CssString));
            Assert.Equal(new[] { "i", "http" }, T(css, ClassificationNames.CssValue));
            Assert.Equal(new[] { "input", "a" }, T(css, ClassificationNames.CssSelector));
            Assert.Contains("[", T(css, ClassificationNames.CssPunct));
            Assert.Contains("^", T(css, ClassificationNames.CssPunct));
        }

        [Fact]
        public void Keyframe_percentages_are_numbers_and_from_to_are_names()
        {
            string css = "@keyframes spin { from { opacity: 0 } 50% { opacity: .5 } to { opacity: 1 } }";
            Assert.Equal(new[] { "from", "to" }, T(css, ClassificationNames.CssSelector));
            Assert.Contains("50%", T(css, ClassificationNames.CssNumber));
        }

        // ---- the condition of an at-rule -----------------------------------------------------------------------------

        [Fact]
        public void A_media_query_names_its_features_like_properties()
        {
            string css = "@media screen and (min-width: 600px) and (hover: hover) and (color) { }";
            Assert.Equal(new[] { "min-width", "hover", "color" }, T(css, ClassificationNames.CssProperty));
            Assert.Equal(new[] { "screen", "and", "and", "hover", "and" }, T(css, ClassificationNames.CssValue));
            Assert.Equal(new[] { "600px" }, T(css, ClassificationNames.CssNumber));
        }

        [Fact]
        public void The_range_syntax_of_a_media_query_and_a_container_and_a_supports_test()
        {
            Assert.Equal(new[] { "width" }, T("@media (width > 600px) { }", ClassificationNames.CssProperty));
            Assert.Equal(new[] { "min-width" }, T("@container card (min-width: 400px) { }", ClassificationNames.CssProperty));
            Assert.Equal(new[] { "display", "display" }, T("@supports (display: grid) and not (display: inline-grid) { }", ClassificationNames.CssProperty));
            Assert.Equal(new[] { "base", "components" }, T("@layer base, components;", ClassificationNames.CssValue));
        }

        [Fact]
        public void A_function_in_a_prelude_is_still_a_function()
        {
            string css = "@import url('x.css') layer(base) screen and (min-width: 1px);";
            Assert.Equal(new[] { "url", "layer" }, T(css, ClassificationNames.CssFunction));
            Assert.Equal(new[] { "min-width" }, T(css, ClassificationNames.CssProperty));
            Assert.Equal(new[] { "'x.css'" }, T(css, ClassificationNames.CssString));
        }

        [Fact]
        public void A_number_with_an_exponent_is_one_number()
        {
            Assert.Equal(new[] { "1e3ms", "2.5E-2", "1em" }, T("a { transition-delay: 1e3ms; opacity: 2.5E-2; width: 1em }", ClassificationNames.CssNumber));
        }
    }
}
