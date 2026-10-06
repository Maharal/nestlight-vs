using System.Linq;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>HTML tokenization rules inside html`...`.</summary>
    public class HtmlTokenizerTests
    {
        private static string[] T(string html, string type) { return Lexer.Texts(Lexer.H(html), type); }

        // ---- tags ----------------------------------------------------------------

        [Fact]
        public void Element_produces_delimiters_and_tag_names()
        {
            Assert.Equal(new[]
            {
                "html.delimiter|<", "html.tag|div", "html.delimiter|>",
                "html.delimiter|</", "html.tag|div", "html.delimiter|>"
            }, Lexer.Seq(Lexer.H("<div></div>")));
        }

        [Theory]
        [InlineData("<br/>")]
        [InlineData("<br />")]
        public void Self_closing_tags_use_the_slash_gt_delimiter(string html)
        {
            Assert.Equal(new[] { "br" }, T(html, TplNames.Tag));
            Assert.Equal(new[] { "<", "/>" }, T(html, TplNames.Delimiter));
        }

        [Fact]
        public void Custom_element_names_with_hyphen_are_a_single_tag()
        {
            Assert.Equal(new[] { "my-element", "my-element" }, T("<my-element></my-element>", TplNames.Tag));
        }

        [Fact]
        public void Less_than_in_text_is_not_a_tag()
        {
            Assert.Equal(new[] { "p", "p" }, T("<p>1 < 2 &lt; 3</p>", TplNames.Tag));
        }

        [Fact]
        public void Less_than_followed_by_digit_is_not_a_tag()
        {
            Assert.Empty(T("a <3 b", TplNames.Tag));
        }

        [Fact]
        public void Malformed_tag_does_not_swallow_the_next_tag()
        {
            Assert.Equal(new[] { "div", "span", "span" }, T("<div <span></span>", TplNames.Tag));
        }

        [Fact]
        public void Empty_template_has_no_tokens()
        {
            Assert.Empty(Lexer.Lex(Lexer.H("")));
        }

        // ---- atributos -------------------------------------------------------------

        [Fact]
        public void Attributes_with_quoted_unquoted_and_boolean_forms()
        {
            string html = "<a href=\"x\" data-id=5 hidden title='t'>";
            Assert.Equal(new[] { "href", "data-id", "hidden", "title" }, T(html, TplNames.Attribute));
            Assert.Equal(new[] { "\"x\"", "5", "'t'" }, T(html, TplNames.AttributeValue));
        }

        [Fact]
        public void Namespaced_attribute_names_are_kept_whole()
        {
            Assert.Equal(new[] { "xlink:href" }, T("<svg xlink:href=\"#a\"></svg>", TplNames.Attribute));
        }

        [Fact]
        public void Multiline_tags_and_values_are_supported()
        {
            string html = "<div\n  id=\"a\"\n  class=\"b\n c\"\n>";
            Assert.Equal(new[] { "id", "class" }, T(html, TplNames.Attribute));
            Assert.Equal(new[] { "\"a\"", "\"b\n c\"" }, T(html, TplNames.AttributeValue));
        }

        [Fact]
        public void Unterminated_quote_runs_to_the_end_without_failing()
        {
            Assert.Equal(new[] { "\"x>" }, T("<a href=\"x>", TplNames.AttributeValue));
        }

        // ---- Lit binding syntax -----------------------------------------------

        [Fact]
        public void Lit_binding_prefixes_get_their_own_classification()
        {
            string html = "<b @click=${f} .value=${v} ?disabled=${d}></b>";
            Assert.Equal(new[] { "@click" }, T(html, TplNames.AttributeEvent));
            Assert.Equal(new[] { ".value" }, T(html, TplNames.AttributeProperty));
            Assert.Equal(new[] { "?disabled" }, T(html, TplNames.AttributeBoolean));
            Assert.Empty(T(html, TplNames.Attribute));
        }

        // ---- ${...} expressions ---------------------------------------------------------

        [Fact]
        public void Unquoted_expression_value_leaves_no_value_token()
        {
            string html = "<b @click=${f}></b>";
            Assert.Empty(T(html, TplNames.AttributeValue));
            Assert.Equal(new[] { "f" }, T(html, TplNames.Expression));
            Assert.Equal(new[] { "${", "}" }, T(html, TplNames.ExprDelimiter));
        }

        [Fact]
        public void Expression_inside_quoted_value_splits_the_value()
        {
            string html = "<p class=\"a ${b} c\"></p>";
            Assert.Equal(new[] { "\"a ", " c\"" }, T(html, TplNames.AttributeValue));
            Assert.Equal(new[] { "b" }, T(html, TplNames.Expression));
        }

        [Fact]
        public void Expression_in_text_content()
        {
            string html = "<p>Hi ${name}!</p>";
            Assert.Equal(new[] { "name" }, T(html, TplNames.Expression));
            Assert.Equal(new[] { "p", "p" }, T(html, TplNames.Tag));
        }

        [Fact]
        public void Expression_in_attribute_position_is_not_an_attribute()
        {
            string html = "<div ${ref(x)}></div>";
            Assert.Empty(T(html, TplNames.Attribute));
            Assert.Equal(new[] { "ref(x)" }, T(html, TplNames.Expression));
            Assert.Equal(new[] { "div", "div" }, T(html, TplNames.Tag));
        }

        [Fact]
        public void Empty_expression_has_delimiters_only()
        {
            string html = "<p>${}</p>";
            Assert.Equal(new[] { "${", "}" }, T(html, TplNames.ExprDelimiter));
            Assert.Empty(T(html, TplNames.Expression));
        }

        [Fact]
        public void Expression_with_nested_braces_is_a_single_expression()
        {
            string html = "<p>${ {a:1}.a }</p>";
            Assert.Equal(new[] { "${", "}" }, T(html, TplNames.ExprDelimiter));
            Assert.Equal(new[] { " {a:1}.a " }, T(html, TplNames.Expression));
        }

        [Fact]
        public void Unterminated_expression_has_no_closing_delimiter()
        {
            // code being typed: no '}' and no closing backtick
            string code = "html`<b>${x";
            Assert.Equal(new[] { "${" }, Lexer.Texts(code, TplNames.ExprDelimiter));
            Assert.Equal(new[] { "x" }, Lexer.Texts(code, TplNames.Expression));
        }

        [Fact]
        public void Open_outer_expression_does_not_claim_the_inner_closing_brace()
        {
            // the last '}' closes the inner ${i}; the outer expression stays open
            string code = "html`<ul>${items.map(i => html`<li>${i}";
            var toks = Lexer.Lex(code);
            Assert.Equal(new[] { "${", "${", "}" }, Lexer.Texts(toks, TplNames.ExprDelimiter));
            Assert.False(Lexer.HasOverlap(toks));
        }

        [Fact]
        public void Nested_templates_are_tokenized_at_both_levels()
        {
            string code = Lexer.H("<ul>${items.map(i => html`<li>${i}</li>`)}</ul>");
            var toks = Lexer.Lex(code);
            Assert.Equal(new[] { "ul", "li", "li", "ul" }, Lexer.Texts(toks, TplNames.Tag));
            Assert.Equal(new[] { "items.map(i => html", "i", ")" }, Lexer.Texts(toks, TplNames.Expression));
            Assert.False(Lexer.HasOverlap(toks));
        }

        // ---- comments -----------------------------------------------------------------

        [Fact]
        public void Html_comment_is_a_single_token()
        {
            string html = "<!-- hi --><b></b>";
            Assert.Equal(new[] { "<!-- hi -->" }, T(html, TplNames.Comment));
            Assert.Equal(new[] { "b", "b" }, T(html, TplNames.Tag));
        }

        [Fact]
        public void Tags_inside_a_comment_are_not_tokenized()
        {
            string html = "<!-- <b class=\"x\"> -->";
            Assert.Empty(T(html, TplNames.Tag));
            Assert.Equal(new[] { "<!-- <b class=\"x\"> -->" }, T(html, TplNames.Comment));
        }

        [Fact]
        public void Unterminated_comment_runs_to_the_end()
        {
            Assert.Equal(new[] { "<!-- hi" }, T("<!-- hi", TplNames.Comment));
        }

        // ---- <style> ---------------------------------------------------------------------

        [Fact]
        public void Style_element_content_is_tokenized_as_css()
        {
            string html = "<style>p { color: red; }</style>";
            Assert.Equal(new[] { "style", "style" }, T(html, TplNames.Tag));
            Assert.Equal(new[] { "p" }, T(html, TplNames.CssSelector));
            Assert.Equal(new[] { "color" }, T(html, TplNames.CssProperty));
            Assert.Equal(new[] { "red" }, T(html, TplNames.CssValue));
        }

        [Fact]
        public void Style_element_is_case_insensitive()
        {
            Assert.Equal(new[] { "b" }, T("<STYLE>a { b: c }</STYLE>", TplNames.CssProperty));
        }

        [Fact]
        public void Style_element_with_attributes_is_still_css()
        {
            string html = "<style media=\"screen\">a { b: c }</style>";
            Assert.Equal(new[] { "media" }, T(html, TplNames.Attribute));
            Assert.Equal(new[] { "b" }, T(html, TplNames.CssProperty));
        }

        [Fact]
        public void Unclosed_style_runs_to_the_end_of_the_template()
        {
            Assert.Equal(new[] { "b" }, T("<style>a { b: c }", TplNames.CssProperty));
        }

        [Fact]
        public void Self_closing_style_does_not_start_css()
        {
            Assert.Empty(T("<style/><p>a: b</p>", TplNames.CssProperty));
        }

        [Fact]
        public void Closing_style_tag_alone_does_not_start_css()
        {
            Assert.Empty(T("</style>a: b", TplNames.CssProperty));
        }

        [Fact]
        public void Script_content_is_not_css()
        {
            Assert.Empty(T("<script>a { b: c }</script>", TplNames.CssProperty));
        }

        // ---- atributo style="..." -----------------------------------------------------

        [Fact]
        public void Style_attribute_value_is_css()
        {
            string html = "<div style=\"color: blue; margin: 0\"></div>";
            Assert.Equal(new[] { "style" }, T(html, TplNames.Attribute));
            Assert.Equal(new[] { "\"", "\"" }, T(html, TplNames.AttributeValue));
            Assert.Equal(new[] { "color", "margin" }, T(html, TplNames.CssProperty));
            Assert.Equal(new[] { "blue" }, T(html, TplNames.CssValue));
            Assert.Equal(new[] { "0" }, T(html, TplNames.CssNumber));
        }

        [Fact]
        public void Style_attribute_with_single_quotes()
        {
            string html = "<div style='top: 1px'>";
            Assert.Equal(new[] { "top" }, T(html, TplNames.CssProperty));
            Assert.Equal(new[] { "1px" }, T(html, TplNames.CssNumber));
            Assert.Equal(new[] { "'", "'" }, T(html, TplNames.AttributeValue));
        }

        [Fact]
        public void Style_attribute_name_is_case_insensitive()
        {
            Assert.Equal(new[] { "a" }, T("<div STYLE=\"a: b\">", TplNames.CssProperty));
        }

        [Fact]
        public void Style_attribute_with_expression_keeps_the_unit()
        {
            string html = "<div style=\"margin: ${m}px\"></div>";
            Assert.Equal(new[] { "margin" }, T(html, TplNames.CssProperty));
            Assert.Equal(new[] { "px" }, T(html, TplNames.CssNumber));
            Assert.Equal(new[] { "m" }, T(html, TplNames.Expression));
        }

        [Fact]
        public void Other_attributes_are_never_parsed_as_css()
        {
            string html = "<div class=\"color: red\" data-style=\"a: b\">";
            Assert.Empty(T(html, TplNames.CssProperty));
            Assert.Equal(new[] { "\"color: red\"", "\"a: b\"" }, T(html, TplNames.AttributeValue));
        }

        [Fact]
        public void Unquoted_style_value_is_a_plain_attribute_value()
        {
            string html = "<div style=a:b>";
            Assert.Empty(T(html, TplNames.CssProperty));
            Assert.Equal(new[] { "a:b" }, T(html, TplNames.AttributeValue));
        }

        [Fact]
        public void Unterminated_style_attribute_does_not_fail()
        {
            string html = "<div style=\"a: b";
            Assert.Equal(new[] { "a" }, T(html, TplNames.CssProperty));
            Assert.Equal(new[] { "\"" }, T(html, TplNames.AttributeValue));
        }
    }
}
