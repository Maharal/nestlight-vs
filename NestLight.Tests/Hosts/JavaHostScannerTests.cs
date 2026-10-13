using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class JavaHostScannerTests
    {
        private static IReadOnlyList<EmbeddedString> Scan(string code)
        {
            return Lexer.Scan(HostLanguage.Java, code);
        }

        private static string Body(string code, EmbeddedString s) { return code.Substring(s.Start, s.End - s.Start); }
        private static string Outer(string code, EmbeddedString s) { return code.Substring(s.OuterStart, s.OuterEnd - s.OuterStart); }

        // ---- text blocks -----------------------------------------------------------------------

        [Fact]
        public void Text_block_is_found()
        {
            string code = "// html\nString a = \"\"\"\n<b/>\n\"\"\";";
            var s = Scan(code).Single();
            Assert.Equal("html", s.EmbeddedLanguageId);
            Assert.Equal("<b/>\n", Body(code, s));
        }

        [Fact]
        public void Text_block_content_starts_after_first_line_break()
        {
            string code = "// html\nString a = \"\"\"\n    <li>x</li>\n    \"\"\";";
            var s = Scan(code).Single();
            Assert.Equal("    <li>x</li>\n    ", Body(code, s));
        }

        [Fact]
        public void Text_block_spans_multiple_lines()
        {
            string code = "// html\nString a = \"\"\"\n<div>\n  <b/>\n</div>\n\"\"\";";
            Assert.Equal("<div>\n  <b/>\n</div>\n", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Text_block_has_no_interpolation()
        {
            string code = "// html\nString a = \"\"\"\n{x} ${y}\n\"\"\";";
            var s = Scan(code).Single();
            Assert.Empty(s.Interpolations);
        }

        [Fact]
        public void Embedded_quote_pair_inside_text_block()
        {
            string code = "// html\nString a = \"\"\"\n<b class=\"x\">\n\"\"\";";
            Assert.Contains("class=\"x\"", Body(code, Scan(code).Single()));
        }

        // ---- regular strings -------------------------------------------------------------------

        [Fact]
        public void Regular_string_is_found()
        {
            string code = "// html\nString a = \"<b/>\";";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Escape_in_regular_string_is_recorded()
        {
            string code = "// html\nString a = \"<b\\n/>\";";
            var s = Scan(code).Single();
            Assert.Single(s.Escapes);
            Assert.Equal('\n', s.Escapes[0].Value);
        }

        [Fact]
        public void Regular_string_ends_at_newline()
        {
            string code = "// html\nString a = \"<b>\nString b = \"\";";
            var s = Scan(code).Single();
            Assert.Equal("<b>", Body(code, s));
        }

        // ---- markers ---------------------------------------------------------------------------

        [Theory]
        [InlineData("// html\nString a = \"x\";", "html")]
        [InlineData("// lang=sql\nString a = \"x\";", "sql")]
        [InlineData("/* css */ String a = \"x\";", "css")]
        [InlineData("String a = /* json */ \"x\";", "json")]
        [InlineData("String a = \"x\";", null)]
        [InlineData("// html\n\nString a = \"x\";", null)]
        public void Markers_use_slash_comments(string code, string id)
        {
            var s = Scan(code);
            if (id == null) Assert.Empty(s);
            else Assert.Equal(id, s.Single().EmbeddedLanguageId);
        }

        [Fact]
        public void Marker_works_with_text_block()
        {
            string code = "// language=sql\nString q = \"\"\"\nSELECT 1\n\"\"\";";
            Assert.Equal("sql", Scan(code).Single().EmbeddedLanguageId);
        }

        // ---- edge cases ------------------------------------------------------------------------

        [Fact]
        public void Strings_and_comments_hide_text_blocks()
        {
            Assert.Empty(Scan("// \"\"\"\nx\n\"\"\"\nint a = 1;"));
            Assert.Empty(Scan("/* \"\"\" */"));
        }

        [Fact]
        public void Character_literal_with_a_quote_is_skipped()
        {
            string code = "char q = '\"'; // html\nString a = \"<b/>\";";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Unterminated_text_block_extends_to_the_end()
        {
            string code = "// html\nString a = \"\"\"\n<b>";
            var s = Scan(code).Single();
            Assert.Equal(code.Length, s.End);
        }

        [Fact]
        public void Unterminated_regular_string_extends_to_end_of_line()
        {
            string code = "// html\nString a = \"<b>";
            var s = Scan(code).Single();
            Assert.Equal("<b>", Body(code, s));
        }

        [Fact]
        public void Several_strings_are_returned_in_order()
        {
            string code = "// html\nString a = \"1\";\n// css\nString b = \"2\";";
            Assert.Equal(new[] { "1", "2" }, Scan(code).Select(s => Body(code, s)).ToArray());
        }

        // ---- mutation-killing tests ----------------------------------------------------------------

        [Fact]
        public void Escape_at_end_of_string_advances_by_one_not_two()
        {
            // Line 100: _i += _i + 1 < _t.Length ? 2 : 1
            // If mutated to always 2 (true ? 2 : 1), a backslash at the very end of the text
            // would skip past the end. With the correct code, it advances by 1.
            string code = "// html\nString a = \"<b>\\";
            var s = Scan(code).Single();
            Assert.Equal("<b>\\", Body(code, s));
        }

        [Fact]
        public void Char_literal_with_escape_and_newline_before_close_stops_early()
        {
            // Line 151: nl >= 0 && nl < e -- if mutated to nl > e, the char skip would
            // consume past the newline into the next line
            string code = "char q = '\\x\n// html\nString a = \"<b/>\";";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Char_literal_with_escape_followed_by_closing_quote()
        {
            // Line 151: e + 1 vs e - 1 -- after finding the closing quote,
            // _i should advance past it (e + 1)
            string code = "char q = '\\n'; // html\nString a = \"<b/>\";";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }
    }
}
