using System.Linq;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>Template location rules: which backticks are HTML/CSS and where they start/end.</summary>
    public class ScannerTests
    {
        private static string Body(string code, TemplateInfo t) { return code.Substring(t.Start, t.End - t.Start); }

        // ---- recognized tags ----------------------------------------------------

        [Theory]
        [InlineData("html")]
        [InlineData("svg")]
        [InlineData("htm")]
        [InlineData("HTML")]
        [InlineData("Html")]
        [InlineData("lit.html")]
        [InlineData("const t = html")]
        public void Html_tags_are_detected(string tag)
        {
            string code = tag + "`<b></b>`";
            var templates = TplScanner.FindTemplates(code);
            Assert.Single(templates);
            Assert.Equal("Html", templates[0].Kind.ToString());
            Assert.Equal("<b></b>", Body(code, templates[0]));
        }

        [Theory]
        [InlineData("css")]
        [InlineData("CSS")]
        [InlineData("lit.css")]
        public void Css_tags_are_detected(string tag)
        {
            string code = tag + "`a { b: c }`";
            var templates = TplScanner.FindTemplates(code);
            Assert.Single(templates);
            Assert.Equal("Css", templates[0].Kind.ToString());
            Assert.Equal("a { b: c }", Body(code, templates[0]));
        }

        [Theory]
        [InlineData("/* html */ ", "Html")]
        [InlineData("/*html*/", "Html")]
        [InlineData("/* HTML */ ", "Html")]
        [InlineData("/* svg */ ", "Html")]
        [InlineData("/* htm */ ", "Html")]
        [InlineData("/* language=html */ ", "Html")]
        [InlineData("/* css */ ", "Css")]
        [InlineData("/*css*/", "Css")]
        [InlineData("/* language=css */ ", "Css")]
        public void Marker_comments_are_detected(string marker, string kind)
        {
            string code = "const a = " + marker + "`x`;";
            var templates = TplScanner.FindTemplates(code);
            Assert.Single(templates);
            Assert.Equal(kind, templates[0].Kind.ToString());
            Assert.Equal("x", Body(code, templates[0]));
        }

        [Theory]
        [InlineData("sql`x`")]
        [InlineData("myhtml`x`")]
        [InlineData("htmlx`x`")]
        [InlineData("String.raw`x`")]
        [InlineData("`x`")]
        [InlineData("/* sql */ `x`")]
        [InlineData("/* html */ 5 `x`")]
        [InlineData("html `x`")] // the tag must be attached to the backtick
        public void Unknown_tags_and_markers_are_ignored(string code)
        {
            Assert.Empty(TplScanner.FindTemplates(code));
        }

        // ---- code that must not be interpreted -------------------------------------

        [Theory]
        [InlineData("// html`<b></b>`\n")]
        [InlineData("/* html`<b></b>` */")]
        [InlineData("var s = \"html`<b></b>`\";")]
        [InlineData("var s = 'html`<b></b>`';")]
        [InlineData("var s = 'it\\'s html`<b></b>`';")]
        public void Not_detected_inside_comments_or_strings(string code)
        {
            Assert.Empty(TplScanner.FindTemplates(code));
        }

        [Fact]
        public void Text_without_backtick_has_no_templates()
        {
            Assert.Empty(TplScanner.FindTemplates("const a = 1; // nothing here"));
        }

        // ---- template boundaries -----------------------------------------------------

        [Fact]
        public void Escaped_backtick_does_not_end_the_template()
        {
            string code = "html`a \\` b`";
            var templates = TplScanner.FindTemplates(code);
            Assert.Single(templates);
            Assert.Equal("a \\` b", Body(code, templates[0]));
        }

        [Fact]
        public void Escaped_dollar_does_not_start_an_expression()
        {
            var templates = TplScanner.FindTemplates("html`\\${x}`");
            Assert.Single(templates);
            Assert.Empty(templates[0].Expressions);
        }

        [Fact]
        public void Empty_template_has_empty_body()
        {
            string code = "html``";
            var templates = TplScanner.FindTemplates(code);
            Assert.Single(templates);
            Assert.Equal(templates[0].Start, templates[0].End);
        }

        [Fact]
        public void Unterminated_template_extends_to_end_of_text()
        {
            string code = "html`<b>";
            var templates = TplScanner.FindTemplates(code);
            Assert.Single(templates);
            Assert.Equal(5, templates[0].Start);
            Assert.Equal(code.Length, templates[0].End);
        }

        [Fact]
        public void Multiple_templates_are_returned_in_source_order()
        {
            string code = "const a = html`1`; const b = css`2`; const c = svg`3`;";
            var templates = TplScanner.FindTemplates(code);
            Assert.Equal(new[] { "Html", "Css", "Html" }, templates.Select(t => t.Kind.ToString()).ToArray());
            Assert.Equal(new[] { "1", "2", "3" }, templates.Select(t => Body(code, t)).ToArray());
        }

        // ---- ${...} expressions -----------------------------------------------------

        [Fact]
        public void Expression_ranges_are_recorded()
        {
            string code = "html`a ${b} c ${d.e}`";
            var t = TplScanner.FindTemplates(code).Single();
            Assert.Equal(new[] { "${b}", "${d.e}" }, t.Expressions.Select(e => code.Substring(e.Start, e.End - e.Start)).ToArray());
            Assert.True(t.Expressions.All(e => e.Closed));
        }

        [Fact]
        public void Expression_with_nested_braces_ends_at_the_matching_brace()
        {
            string code = "html`${ {a: 1}.a } x`";
            var t = TplScanner.FindTemplates(code).Single();
            var e = t.Expressions.Single();
            Assert.Equal("${ {a: 1}.a }", code.Substring(e.Start, e.End - e.Start));
            Assert.True(e.Closed);
        }

        [Fact]
        public void Brace_inside_a_string_does_not_end_the_expression()
        {
            string code = "html`${ '}' } x`";
            var e = TplScanner.FindTemplates(code).Single().Expressions.Single();
            Assert.Equal("${ '}' }", code.Substring(e.Start, e.End - e.Start));
        }

        [Fact]
        public void Unterminated_expression_is_marked_as_open()
        {
            string code = "html`<b>${x";
            var t = TplScanner.FindTemplates(code).Single();
            Assert.Equal(code.Length, t.End);
            Assert.False(t.Expressions.Single().Closed);
        }

        // ---- nested templates ---------------------------------------------------

        [Fact]
        public void Html_template_nested_in_an_expression_is_found()
        {
            string code = "html`${ ok ? html`<b></b>` : '' }`";
            var templates = TplScanner.FindTemplates(code);
            Assert.Equal(2, templates.Count);
            Assert.True(templates[0].Start < templates[1].Start);
            Assert.Equal("<b></b>", Body(code, templates[1]));
            Assert.Single(templates[0].Expressions);
        }

        [Fact]
        public void Template_inside_an_untagged_template_expression_is_found()
        {
            string code = "`a ${html`<i></i>`}`";
            var templates = TplScanner.FindTemplates(code);
            Assert.Single(templates);
            Assert.Equal("Html", templates[0].Kind.ToString());
            Assert.Equal("<i></i>", Body(code, templates[0]));
        }

        [Fact]
        public void Css_nested_inside_html_expression_keeps_its_own_kind()
        {
            var templates = TplScanner.FindTemplates("html`${css`a{}`}`");
            Assert.Equal(new[] { "Html", "Css" }, templates.Select(t => t.Kind.ToString()).ToArray());
        }
    }
}
