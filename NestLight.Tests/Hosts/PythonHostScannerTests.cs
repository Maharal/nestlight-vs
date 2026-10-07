using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>Python: quote styles, prefixes, f-strings and t-strings, `#` markers.</summary>
    public class PythonHostScannerTests
    {
        private static IReadOnlyList<EmbeddedString> Scan(string code)
        {
            return Lexer.Scan(HostLanguage.Python, code);
        }

        private static string Body(string code, EmbeddedString s) { return code.Substring(s.Start, s.End - s.Start); }
        private static string[] Holes(string code, EmbeddedString s)
        {
            return s.Interpolations.Select(x => code.Substring(x.Start, x.End - x.Start)).ToArray();
        }

        // ---- quote styles and prefixes -----------------------------------------------------------------------

        [Theory]
        [InlineData("'<b/>'")]
        [InlineData("\"<b/>\"")]
        [InlineData("'''<b/>'''")]
        [InlineData("\"\"\"<b/>\"\"\"")]
        [InlineData("r'<b/>'")]
        [InlineData("R\"<b/>\"")]
        [InlineData("u'<b/>'")]
        [InlineData("b'<b/>'")]
        [InlineData("rb'<b/>'")]
        [InlineData("BR'<b/>'")]
        [InlineData("f'<b/>'")]
        [InlineData("F\"<b/>\"")]
        [InlineData("fr'<b/>'")]
        [InlineData("rf'<b/>'")]
        [InlineData("t'<b/>'")]
        [InlineData("tr'<b/>'")]
        [InlineData("f\"\"\"<b/>\"\"\"")]
        public void Every_quote_style_and_prefix_is_found(string literal)
        {
            string code = "# language=html\nx = " + literal;
            var s = Scan(code).Single();
            Assert.Equal("html", s.LanguageId);
            Assert.Equal("<b/>", Body(code, s));
            Assert.Equal(code.IndexOf(literal), s.OuterStart);
            Assert.Equal(code.Length, s.OuterEnd);
        }

        [Theory]
        [InlineData("xr'a'")]
        [InlineData("rr'a'")]
        [InlineData("bf'a'")]
        public void Unknown_prefixes_are_not_strings_but_the_quotes_still_are(string literal)
        {
            // "xr'a'" is the name xr followed by the string 'a'
            string code = "# html\n" + literal;
            var s = Scan(code);
            Assert.Single(s);
            Assert.Equal("a", Body(code, s[0]));
        }

        [Fact]
        public void Identifier_ending_in_a_prefix_letter_is_not_a_prefix()
        {
            string code = "# html\nprint'<b/>'";
            var s = Scan(code).Single();
            Assert.Equal(code.IndexOf('\''), s.OuterStart);
        }

        [Fact]
        public void Triple_quoted_strings_span_lines_and_may_contain_quotes()
        {
            string code = "# html\nx = \"\"\"a \"b\" ''c''\nd\"\"\"";
            Assert.Equal("a \"b\" ''c''\nd", Body(code, Scan(code).Single()));
        }

        // ---- markers --------------------------------------------------------------------------------------

        [Theory]
        [InlineData("# html\nx = '1'", "html")]
        [InlineData("# language=sql\nx = '1'", "sql")]
        [InlineData("# lang=css\nx = '1'", "css")]
        [InlineData("x = '1'", null)]
        [InlineData("# html\n\nx = '1'", null)]
        [InlineData("# html is great\nx = '1'", null)]
        [InlineData("// html\nx = '1'", null)]
        [InlineData("/* html */ x = '1'", null)]
        public void Marker_comments_use_the_hash(string code, string id)
        {
            var s = Scan(code);
            if (id == null) Assert.Empty(s);
            else Assert.Equal(id, s.Single().LanguageId);
        }

        [Fact]
        public void Strings_in_comments_are_ignored()
        {
            Assert.Empty(Scan("# html '<b/>'"));
        }

        [Fact]
        public void Implicit_concatenation_is_not_joined()
        {
            string code = "# html\nx = ('<a>'\n     '</a>')";
            var s = Scan(code);
            Assert.Single(s); // only the first literal is marked
            Assert.Equal("<a>", Body(code, s[0]));
        }

        // ---- escapes ----------------------------------------------------------------------------------------

        [Fact]
        public void Escaped_quote_does_not_end_the_string()
        {
            string code = "# json\nx = \"{\\\"a\\\": 1}\"";
            var s = Scan(code).Single();
            Assert.Equal("{\\\"a\\\": 1}", Body(code, s));
            Assert.Equal(new[] { '"', '"' }, s.Escapes.Select(e => e.Value).ToArray());
        }

        [Fact]
        public void Raw_strings_do_not_decode_escapes_but_still_protect_the_quote()
        {
            string code = "# regex\nx = r\"\\d+\\\"\"";
            var s = Scan(code).Single();
            Assert.Empty(s.Escapes);
            Assert.Equal("\\d+\\\"", Body(code, s));
        }

        // ---- f-strings and t-strings ---------------------------------------------------------------------

        [Theory]
        [InlineData("f")]
        [InlineData("F")]
        [InlineData("t")]
        [InlineData("rf")]
        [InlineData("fr")]
        [InlineData("tr")]
        public void Replacement_fields_exist_in_f_and_t_strings(string prefix)
        {
            string code = "# html\nx = " + prefix + "'<b>{name}</b>{ a + 1 }'";
            Assert.Equal(new[] { "{name}", "{ a + 1 }" }, Holes(code, Scan(code).Single()));
        }

        [Theory]
        [InlineData("")]
        [InlineData("r")]
        [InlineData("b")]
        [InlineData("u")]
        public void Plain_strings_have_no_replacement_fields(string prefix)
        {
            string code = "# html\nx = " + prefix + "'<b>{name}</b>'";
            Assert.Empty(Scan(code).Single().Interpolations);
        }

        [Fact]
        public void Doubled_braces_are_escapes()
        {
            string code = "# css\nx = f'a {{ b: {c} }}'";
            var s = Scan(code).Single();
            Assert.Equal(new[] { "{c}" }, Holes(code, s));
            Assert.Equal(new[] { '{', '}' }, s.Escapes.Select(e => e.Value).ToArray());
        }

        [Fact]
        public void Conversion_format_spec_and_nested_fields_belong_to_the_field()
        {
            string code = "# html\nx = f'{a!r} {b:>10} {c:{w}.{p}f} {d=}'";
            Assert.Equal(new[] { "{a!r}", "{b:>10}", "{c:{w}.{p}f}", "{d=}" }, Holes(code, Scan(code).Single()));
        }

        [Fact]
        public void Dict_literals_and_slices_nest_inside_a_field()
        {
            string code = "# html\nx = f'{ {1: 2}[1] } {a[1:2]} {fn(x, y=1)}'";
            Assert.Equal(new[] { "{ {1: 2}[1] }", "{a[1:2]}", "{fn(x, y=1)}" }, Holes(code, Scan(code).Single()));
        }

        [Fact]
        public void Brace_in_a_nested_string_does_not_close_the_field()
        {
            string code = "# html\nx = f\"<b>{ '}' }</b>\"";
            Assert.Equal(new[] { "{ '}' }" }, Holes(code, Scan(code).Single()));
        }

        [Fact]
        public void Marked_string_nested_in_a_triple_quoted_field_is_found()
        {
            string code = "# html\nx = f\"\"\"<ul>{ \"\".join(\n  # css\n  f'<li>{i}</li>' for i in xs) }</ul>\"\"\"";
            Assert.Equal(new[] { "html", "css" }, Scan(code).Select(s => s.LanguageId).ToArray());
        }

        [Fact]
        public void Unclosed_field_in_a_single_line_string_stops_at_the_line_break()
        {
            string code = "# html\nx = f'<b>{name\ny = 1";
            var s = Scan(code).Single();
            Assert.False(s.Interpolations.Single().Closed);
            Assert.True(s.Interpolations[0].End <= code.IndexOf("\ny = 1"));
        }

        [Fact]
        public void Unclosed_triple_quoted_string_extends_to_the_end()
        {
            string code = "# html\nx = '''<b>";
            Assert.Equal(code.Length, Scan(code).Single().End);
        }

        [Fact]
        public void Unterminated_single_quoted_string_ends_at_the_line_break()
        {
            string code = "# html\nx = '<b>\ny = 1";
            Assert.Equal("<b>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Numbers_and_names_before_quotes_do_not_confuse_the_scan()
        {
            string code = "n = 1_000\n# html\nx = f'<b>{n}</b>'";
            Assert.Single(Scan(code));
        }
    }
}
