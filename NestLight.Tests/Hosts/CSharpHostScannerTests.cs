using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>C#: regular, verbatim and raw strings, their interpolated forms, markers and exclusions.</summary>
    public class CSharpHostScannerTests
    {
        private static System.Collections.Generic.IReadOnlyList<EmbeddedString> Scan(string code)
        {
            return Lexer.Scan(HostLanguage.CSharp, code);
        }

        private static string Body(string code, EmbeddedString s) { return code.Substring(s.Start, s.End - s.Start); }
        private static string Outer(string code, EmbeddedString s) { return code.Substring(s.OuterStart, s.OuterEnd - s.OuterStart); }
        private static string[] Holes(string code, EmbeddedString s)
        {
            return s.Interpolations.Select(x => code.Substring(x.Start, x.End - x.Start)).ToArray();
        }

        // ---- string forms ----------------------------------------------------------------------------

        [Theory]
        [InlineData("\"<b>x</b>\"", "<b>x</b>")]
        [InlineData("$\"<b>x</b>\"", "<b>x</b>")]
        [InlineData("@\"<b>x</b>\"", "<b>x</b>")]
        [InlineData("$@\"<b>x</b>\"", "<b>x</b>")]
        [InlineData("@$\"<b>x</b>\"", "<b>x</b>")]
        [InlineData("\"\"\"<b>x</b>\"\"\"", "<b>x</b>")]
        [InlineData("$\"\"\"<b>x</b>\"\"\"", "<b>x</b>")]
        [InlineData("$$\"\"\"<b>x</b>\"\"\"", "<b>x</b>")]
        [InlineData("\"\"\"\"<b>x</b>\"\"\"\"", "<b>x</b>")]
        public void Every_string_form_is_found_with_its_content(string literal, string body)
        {
            string code = "// html\nvar a = " + literal + ";";
            var s = Scan(code).Single();
            Assert.Equal("html", s.EmbeddedLanguageId);
            Assert.Equal(body, Body(code, s));
            Assert.Equal(literal, Outer(code, s));
        }

        [Fact]
        public void Raw_string_content_spans_lines()
        {
            string code = "// html\nvar a = \"\"\"\n    <li>x</li>\n    \"\"\";";
            var s = Scan(code).Single();
            Assert.Equal("\n    <li>x</li>\n    ", Body(code, s));
        }

        [Fact]
        public void Raw_string_ends_only_at_a_run_of_the_same_length()
        {
            string code = "// html\nvar a = \"\"\"\"a \"\"\" b\"\"\"\";";
            var s = Scan(code).Single();
            Assert.Equal("a \"\"\" b", Body(code, s));
        }

        [Fact]
        public void Verbatim_string_spans_lines_and_ends_at_a_single_quote()
        {
            string code = "// sql\nvar a = @\"select 1\nfrom t\";";
            Assert.Equal("select 1\nfrom t", Body(code, Scan(code).Single()));
        }

        // ---- escapes --------------------------------------------------------------------------------------

        [Fact]
        public void Doubled_quote_in_a_verbatim_string_is_an_escape_not_the_end()
        {
            string code = "// sql\nvar a = @\"where x = \"\"a\"\"\";";
            var s = Scan(code).Single();
            Assert.Equal("where x = \"\"a\"\"", Body(code, s));
            Assert.Equal(2, s.Escapes.Count);
            Assert.All(s.Escapes, e => Assert.Equal('"', e.Value));
        }

        [Fact]
        public void Backslash_escapes_in_a_regular_string_are_recorded()
        {
            string code = "// sql\nvar a = \"{\\\"a\\\": 1}\";";
            var s = Scan(code).Single();
            Assert.Equal("{\\\"a\\\": 1}", Body(code, s));
            Assert.Equal(new[] { '"', '"' }, s.Escapes.Select(e => e.Value).ToArray());
        }

        [Fact]
        public void Escaped_quote_does_not_end_a_regular_string()
        {
            string code = "// html\nvar a = \"a\\\"b\";";
            Assert.Equal("a\\\"b", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Raw_strings_have_no_escapes()
        {
            string code = "// html\nvar a = \"\"\"a \\n \"\" b\"\"\";";
            Assert.Empty(Scan(code).Single().Escapes);
        }

        [Fact]
        public void Doubled_braces_in_an_interpolated_string_are_escapes_not_holes()
        {
            string code = "// css\nvar a = $\"a {{ b: {c} }}\";";
            var s = Scan(code).Single();
            Assert.Equal(new[] { "{c}" }, Holes(code, s));
            Assert.Equal(new[] { '{', '}' }, s.Escapes.Select(e => e.Value).ToArray());
        }

        // ---- interpolations -------------------------------------------------------------------------------

        [Fact]
        public void Interpolation_ranges_use_one_brace_for_a_single_dollar()
        {
            string code = "// html\nvar a = $\"<b>{name}</b>{ x + 1 }\";";
            var s = Scan(code).Single();
            Assert.Equal(new[] { "{name}", "{ x + 1 }" }, Holes(code, s));
            Assert.All(s.Interpolations, x => { Assert.Equal(1, x.OpenLength); Assert.Equal(1, x.CloseLength); });
        }

        [Fact]
        public void Interpolation_of_a_raw_string_uses_as_many_braces_as_dollars()
        {
            string code = "// html\nvar a = $$\"\"\"<b>{{name}} {x}</b>\"\"\";";
            var s = Scan(code).Single();
            Assert.Equal(new[] { "{{name}}" }, Holes(code, s));
            Assert.Equal(2, s.Interpolations[0].OpenLength);
            Assert.Equal(2, s.Interpolations[0].CloseLength);
        }

        [Fact]
        public void Triple_dollar_needs_three_braces()
        {
            string code = "// html\nvar a = $$$\"\"\"{{a}} {{{b}}}\"\"\";";
            Assert.Equal(new[] { "{{{b}}}" }, Holes(code, Scan(code).Single()));
        }

        [Fact]
        public void Extra_braces_around_a_raw_interpolation_stay_literal()
        {
            string code = "// html\nvar a = $$\"\"\"{{{b}}}\"\"\";";
            // the interpolation is the innermost pair of braces; the outer ones are text
            Assert.Equal(new[] { "{{b}}" }, Holes(code, Scan(code).Single()));
        }

        [Fact]
        public void Fewer_braces_than_dollars_are_text()
        {
            string code = "// html\nvar a = $$\"\"\"{a} {{b}}\"\"\";";
            Assert.Equal(new[] { "{{b}}" }, Holes(code, Scan(code).Single()));
        }

        [Fact]
        public void Format_specifier_and_alignment_belong_to_the_interpolation()
        {
            string code = "// html\nvar a = $\"<b>{price:N2}</b>{x,10}{(a ? b : c)}\";";
            Assert.Equal(new[] { "{price:N2}", "{x,10}", "{(a ? b : c)}" }, Holes(code, Scan(code).Single()));
        }

        [Fact]
        public void Format_specifier_with_a_quote_does_not_start_a_string()
        {
            string code = "// html\nvar a = $\"{d:\\\"x\\\"}\" + \"after\";";
            var s = Scan(code);
            Assert.Single(s);
        }

        [Fact]
        public void Brace_in_a_nested_string_does_not_close_the_interpolation()
        {
            string code = "// html\nvar a = $\"<b>{ \"}\" }</b>\";";
            Assert.Equal(new[] { "{ \"}\" }" }, Holes(code, Scan(code).Single()));
        }

        [Fact]
        public void Object_initializer_braces_nest_inside_an_interpolation()
        {
            string code = "// html\nvar a = $\"{ new { A = 1 }.A }\";";
            Assert.Equal(new[] { "{ new { A = 1 }.A }" }, Holes(code, Scan(code).Single()));
        }

        [Fact]
        public void Verbatim_interpolated_string_has_holes_and_doubled_braces()
        {
            string code = "// html\nvar a = $@\"<b>{{x}} {y}</b>\";";
            var s = Scan(code).Single();
            Assert.Equal(new[] { "{y}" }, Holes(code, s));
            Assert.Equal(2, s.Escapes.Count);
        }

        [Fact]
        public void Unclosed_interpolation_extends_to_the_end_of_the_text()
        {
            string code = "// html\nvar a = $\"<b>{name";
            var s = Scan(code).Single();
            Assert.False(s.Interpolations[0].Closed);
            Assert.Equal(code.Length, s.Interpolations[0].End);
        }

        // ---- nesting ------------------------------------------------------------------------------------

        [Fact]
        public void Marked_string_nested_in_an_interpolation_is_found()
        {
            string code = "// html\nvar a = $\"<ul>{ string.Join(\"\", items.Select(i => /* html */ $\"<li>{i}</li>\")) }</ul>\";";
            var all = Scan(code);
            Assert.Equal(2, all.Count);
            Assert.True(all[0].OuterStart < all[1].OuterStart);
            Assert.Equal("<li>{i}</li>", Body(code, all[1]));
        }

        [Fact]
        public void Marked_string_inside_an_unmarked_interpolated_string_is_found()
        {
            string code = "var a = $\"x {/* css */ \"a{}\"} y\";";
            Assert.Equal(new[] { "css" }, Scan(code).Select(s => s.EmbeddedLanguageId).ToArray());
        }

        // ---- what is not embedded ---------------------------------------------------------------------

        [Fact]
        public void Unmarked_strings_are_not_embedded()
        {
            Assert.Empty(Scan("var a = \"<b>x</b>\"; var b = $\"{x}\"; var c = @\"y\"; var d = \"\"\"z\"\"\";"));
        }

        [Theory]
        [InlineData("// json")]
        [InlineData("// language=json")]
        [InlineData("// lang=regex")]
        [InlineData("// regexp")]
        [InlineData("/* JSON */")]
        public void Json_and_regex_are_left_to_visual_studio(string marker)
        {
            Assert.Empty(Scan(marker + "\nvar a = \"{}\";"));
        }

        [Fact]
        public void Other_languages_are_not_excluded()
        {
            Assert.Single(Scan("// sql\nvar a = \"select 1\";"));
            Assert.Single(Scan("// yaml\nvar a = \"a: 1\";"));
        }

        [Theory]
        [InlineData("// html \"<b></b>\"")]
        [InlineData("/* html \"<b></b>\" */")]
        public void Strings_in_comments_are_ignored(string code)
        {
            Assert.Empty(Scan(code));
        }

        [Fact]
        public void Marker_inside_a_string_is_ignored()
        {
            Assert.Empty(Scan("var a = \"// html\"; var b = \"x\";"));
        }

        [Fact]
        public void Char_literals_do_not_start_strings()
        {
            string code = "var q = '\"'; var e = '\\''; // html\nvar a = \"<b/>\";";
            var s = Scan(code).Single();
            Assert.Equal("<b/>", Body(code, s));
        }

        [Fact]
        public void At_and_dollar_identifiers_are_not_strings()
        {
            string code = "var @class = 1; // html\nvar a = \"<b/>\";";
            Assert.Single(Scan(code));
        }

        // ---- incomplete code ------------------------------------------------------------------------------

        [Fact]
        public void Unterminated_regular_string_ends_at_the_line_break()
        {
            string code = "// html\nvar a = \"<b>\nvar b = 1;";
            var s = Scan(code).Single();
            Assert.Equal("<b>", Body(code, s));
        }

        [Fact]
        public void Unterminated_raw_and_verbatim_strings_extend_to_the_end()
        {
            string raw = "// html\nvar a = \"\"\"<b>";
            string verbatim = "// html\nvar a = @\"<b>";
            Assert.Equal(raw.Length, Scan(raw).Single().End);
            Assert.Equal(verbatim.Length, Scan(verbatim).Single().End);
        }

        [Fact]
        public void Results_are_ordered_by_position()
        {
            string code = "// html\nvar a = \"x\";\n// css\nvar b = \"y\";\n// sql\nvar c = \"z\";";
            var s = Scan(code);
            Assert.Equal(new[] { "html", "css", "sql" }, s.Select(x => x.EmbeddedLanguageId).ToArray());
            Assert.True(s.Zip(s.Skip(1), (a, b) => a.OuterStart < b.OuterStart).All(x => x));
        }
    }
}
