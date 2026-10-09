using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>C++: raw string literals, delimiters and prefixes, `//` and block markers.</summary>
    public class CppHostScannerTests
    {
        private static IReadOnlyList<EmbeddedString> Scan(string code)
        {
            return Lexer.Scan(HostLanguage.Cpp, code);
        }

        private static string Body(string code, EmbeddedString s) { return code.Substring(s.Start, s.End - s.Start); }

        [Theory]
        [InlineData("R\"(<b/>)\"", "<b/>")]
        [InlineData("R\"x(<b/>)x\"", "<b/>")]
        [InlineData("R\"delim(<b/>)delim\"", "<b/>")]
        [InlineData("u8R\"(<b/>)\"", "<b/>")]
        [InlineData("uR\"(<b/>)\"", "<b/>")]
        [InlineData("UR\"(<b/>)\"", "<b/>")]
        [InlineData("LR\"(<b/>)\"", "<b/>")]
        [InlineData("R\"(a\nb)\"", "a\nb")]
        [InlineData("R\"()\"", "")]
        public void Raw_strings_with_any_prefix_and_delimiter_are_found(string literal, string body)
        {
            string code = "// language=html\nauto a = " + literal + ";";
            var s = Scan(code).Single();
            Assert.Equal("html", s.EmbeddedLanguageId);
            Assert.Equal(body, Body(code, s));
            Assert.Equal(literal, code.Substring(s.OuterStart, s.OuterEnd - s.OuterStart));
        }

        [Fact]
        public void Raw_string_ends_only_at_its_own_delimiter()
        {
            string code = "// html\nauto a = R\"x(a)\" b)\"; )x\";";
            Assert.Equal("a)\" b)\"; ", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Raw_strings_have_no_interpolation_and_no_escapes()
        {
            string code = "// html\nauto a = R\"({x} ${y} \\n)\";";
            var s = Scan(code).Single();
            Assert.Empty(s.Interpolations);
            Assert.Empty(s.Escapes);
        }

        [Theory]
        [InlineData("// html\nauto a = R\"(x)\";", "html")]
        [InlineData("// lang=sql\nauto a = R\"(x)\";", "sql")]
        [InlineData("/* css */ auto a = R\"(x)\";", "css")]
        [InlineData("auto a = /* json */ R\"(x)\";", "json")]
        [InlineData("auto a = R\"(x)\";", null)]
        [InlineData("// html\n\nauto a = R\"(x)\";", null)]
        [InlineData("# html\nauto a = R\"(x)\";", null)]
        public void Markers_use_slash_comments(string code, string id)
        {
            var s = Scan(code);
            if (id == null) Assert.Empty(s);
            else Assert.Equal(id, s.Single().EmbeddedLanguageId);
        }

        [Fact]
        public void Ordinary_strings_are_never_embedded()
        {
            Assert.Empty(Scan("// html\nauto a = \"<b/>\";"));
            Assert.Empty(Scan("// html\nauto a = u8\"<b/>\";"));
        }

        [Fact]
        public void Strings_and_comments_hide_raw_literals()
        {
            Assert.Empty(Scan("// html\n// R\"(x)\"\nauto a = \"R\\\"(x)\\\"\";"));
            Assert.Empty(Scan("/* R\"(x)\" */"));
        }

        [Fact]
        public void Identifiers_ending_in_R_are_not_raw_prefixes()
        {
            Assert.Empty(Scan("// html\nFOOR\"(x)\";"));
        }

        [Fact]
        public void Digit_separators_do_not_start_character_literals()
        {
            string code = "int n = 1'000'000; // html\nauto a = R\"(<b/>)\";";
            Assert.Single(Scan(code));
        }

        [Fact]
        public void Character_literal_with_a_quote_is_skipped()
        {
            string code = "char q = '\"'; // html\nauto a = R\"(<b/>)\";";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Invalid_delimiter_is_not_a_raw_string()
        {
            Assert.Empty(Scan("// html\nauto a = R\"abc def(x)abc def\";"));
            Assert.Empty(Scan("// html\nauto a = R\"01234567890123456789(x)01234567890123456789\";"));
        }

        [Fact]
        public void Unterminated_raw_string_extends_to_the_end()
        {
            string code = "// html\nauto a = R\"(<b>";
            var s = Scan(code).Single();
            Assert.Equal(code.Length, s.End);
            Assert.Equal(code.Length, s.OuterEnd);
        }

        [Fact]
        public void Several_raw_strings_are_returned_in_order()
        {
            string code = "// html\nauto a = R\"(1)\";\n// css\nauto b = R\"(2)\";";
            Assert.Equal(new[] { "1", "2" }, Scan(code).Select(s => Body(code, s)).ToArray());
        }
    }
}
