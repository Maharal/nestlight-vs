using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class RustHostScannerTests
    {
        private static IReadOnlyList<EmbeddedString> Scan(string code)
        {
            return Lexer.Scan(HostLanguage.Rust, code);
        }

        private static string Body(string code, EmbeddedString s) { return code.Substring(s.Start, s.End - s.Start); }
        private static string Outer(string code, EmbeddedString s) { return code.Substring(s.OuterStart, s.OuterEnd - s.OuterStart); }

        // ---- raw strings -----------------------------------------------------------------------

        [Theory]
        [InlineData("r\"<b/>\"", "<b/>")]
        [InlineData("r#\"<b/>\"#", "<b/>")]
        [InlineData("r##\"<b/>\"##", "<b/>")]
        [InlineData("r###\"<b/>\"###", "<b/>")]
        [InlineData("r\"\"", "")]
        public void Raw_strings_with_hashes_are_found(string literal, string body)
        {
            string code = "// language=html\nlet a = " + literal + ";";
            var s = Scan(code).Single();
            Assert.Equal("html", s.EmbeddedLanguageId);
            Assert.Equal(body, Body(code, s));
            Assert.Equal(literal, Outer(code, s));
        }

        [Fact]
        public void Raw_string_ends_only_at_matching_hash_count()
        {
            string code = "// html\nlet a = r##\"a \"# b\"##;";
            Assert.Equal("a \"# b", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Byte_raw_strings_are_found()
        {
            string code = "// html\nlet a = br\"<b/>\";";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Raw_strings_have_no_escapes()
        {
            string code = "// html\nlet a = r\"\\n\";";
            var s = Scan(code).Single();
            Assert.Empty(s.Escapes);
            Assert.Equal("\\n", Body(code, s));
        }

        [Fact]
        public void Raw_strings_have_no_interpolation()
        {
            string code = "// html\nlet a = r\"{x} ${y}\";";
            var s = Scan(code).Single();
            Assert.Empty(s.Interpolations);
        }

        // ---- regular strings -------------------------------------------------------------------

        [Fact]
        public void Regular_string_is_found()
        {
            string code = "// html\nlet a = \"<b/>\";";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Escape_in_regular_string_is_recorded()
        {
            string code = "// html\nlet a = \"<b\\n/>\";";
            var s = Scan(code).Single();
            Assert.Single(s.Escapes);
            Assert.Equal('\n', s.Escapes[0].Value);
        }

        // ---- markers ---------------------------------------------------------------------------

        [Theory]
        [InlineData("// html\nlet a = r\"x\";", "html")]
        [InlineData("// lang=sql\nlet a = r\"x\";", "sql")]
        [InlineData("/* css */ let a = r\"x\";", "css")]
        [InlineData("let a = /* json */ r\"x\";", "json")]
        [InlineData("let a = r\"x\";", null)]
        [InlineData("// html\n\nlet a = r\"x\";", null)]
        public void Markers_use_slash_comments(string code, string id)
        {
            var s = Scan(code);
            if (id == null) Assert.Empty(s);
            else Assert.Equal(id, s.Single().EmbeddedLanguageId);
        }

        // ---- edge cases ------------------------------------------------------------------------

        [Fact]
        public void Strings_and_comments_hide_raw_literals()
        {
            Assert.Empty(Scan("// r\"(x)\"\nlet a = \"r\\\"x\\\"\";"));
            Assert.Empty(Scan("/* r\"x\" */"));
        }

        [Fact]
        public void Character_literal_with_a_quote_is_skipped()
        {
            string code = "let q = '\"'; // html\nlet a = r\"<b/>\";";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Lifetime_does_not_consume_quotes()
        {
            string code = "fn f<'a>() { // html\nlet s = r\"<b/>\";\n}";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Unterminated_raw_string_extends_to_the_end()
        {
            string code = "// html\nlet a = r\"<b>";
            var s = Scan(code).Single();
            Assert.Equal(code.Length, s.End);
        }

        [Fact]
        public void Unterminated_regular_string_extends_to_end_of_line()
        {
            string code = "// html\nlet a = \"<b>";
            var s = Scan(code).Single();
            Assert.Equal("<b>", Body(code, s));
        }

        [Fact]
        public void Several_strings_are_returned_in_order()
        {
            string code = "// html\nlet a = r\"1\";\n// css\nlet b = r\"2\";";
            Assert.Equal(new[] { "1", "2" }, Scan(code).Select(s => Body(code, s)).ToArray());
        }
    }
}
