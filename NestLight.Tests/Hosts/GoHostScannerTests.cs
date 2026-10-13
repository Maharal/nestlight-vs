using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class GoHostScannerTests
    {
        private static IReadOnlyList<EmbeddedString> Scan(string code)
        {
            return Lexer.Scan(HostLanguage.Go, code);
        }

        private static string Body(string code, EmbeddedString s) { return code.Substring(s.Start, s.End - s.Start); }
        private static string Outer(string code, EmbeddedString s) { return code.Substring(s.OuterStart, s.OuterEnd - s.OuterStart); }

        // ---- backtick strings ------------------------------------------------------------------

        [Fact]
        public void Backtick_string_is_found()
        {
            string code = "// html\nvar a = `<b/>`";
            var s = Scan(code).Single();
            Assert.Equal("html", s.EmbeddedLanguageId);
            Assert.Equal("<b/>", Body(code, s));
            Assert.Equal("`<b/>`", Outer(code, s));
        }

        [Fact]
        public void Backtick_string_spans_lines()
        {
            string code = "// html\nvar a = `<div>\n  <b/>\n</div>`";
            Assert.Equal("<div>\n  <b/>\n</div>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Backtick_string_has_no_escapes()
        {
            string code = "// html\nvar a = `\\n`";
            var s = Scan(code).Single();
            Assert.Empty(s.Escapes);
            Assert.Equal("\\n", Body(code, s));
        }

        [Fact]
        public void Backtick_string_has_no_interpolation()
        {
            string code = "// html\nvar a = `{x} ${y}`";
            var s = Scan(code).Single();
            Assert.Empty(s.Interpolations);
        }

        [Fact]
        public void Empty_backtick_string()
        {
            string code = "// html\nvar a = ``";
            var s = Scan(code).Single();
            Assert.Equal("", Body(code, s));
        }

        // ---- regular strings -------------------------------------------------------------------

        [Fact]
        public void Regular_string_is_found()
        {
            string code = "// html\nvar a = \"<b/>\"";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Escape_in_regular_string_is_recorded()
        {
            string code = "// html\nvar a = \"<b\\n/>\"";
            var s = Scan(code).Single();
            Assert.Single(s.Escapes);
            Assert.Equal('\n', s.Escapes[0].Value);
        }

        [Fact]
        public void Regular_string_ends_at_newline()
        {
            string code = "// html\nvar a = \"<b>\nvar b = 1";
            var s = Scan(code).Single();
            Assert.Equal("<b>", Body(code, s));
        }

        // ---- markers ---------------------------------------------------------------------------

        [Theory]
        [InlineData("// html\nvar a = `x`", "html")]
        [InlineData("// lang=sql\nvar a = `x`", "sql")]
        [InlineData("/* css */ var a = `x`", "css")]
        [InlineData("var a = /* json */ `x`", "json")]
        [InlineData("var a = `x`", null)]
        [InlineData("// html\n\nvar a = `x`", null)]
        public void Markers_use_slash_comments(string code, string id)
        {
            var s = Scan(code);
            if (id == null) Assert.Empty(s);
            else Assert.Equal(id, s.Single().EmbeddedLanguageId);
        }

        // ---- edge cases ------------------------------------------------------------------------

        [Fact]
        public void Strings_and_comments_hide_backtick_strings()
        {
            Assert.Empty(Scan("// `x`\nvar a = 1"));
            Assert.Empty(Scan("/* `x` */"));
        }

        [Fact]
        public void Rune_literal_with_a_quote_is_skipped()
        {
            string code = "var q = '\"'\n// html\nvar a = `<b/>`";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Unterminated_backtick_extends_to_the_end()
        {
            string code = "// html\nvar a = `<b>";
            var s = Scan(code).Single();
            Assert.Equal(code.Length, s.End);
        }

        [Fact]
        public void Several_strings_are_returned_in_order()
        {
            string code = "// html\nvar a = `1`\n// css\nvar b = `2`";
            Assert.Equal(new[] { "1", "2" }, Scan(code).Select(s => Body(code, s)).ToArray());
        }

        // ---- mutation-killing tests ----------------------------------------------------------------

        [Fact]
        public void Comment_skipping_continues_loop_without_falling_through()
        {
            // Line 48: if the continue after TrySkip is removed, after skipping a block comment
            // the code falls through and _i++ skips the character right after the comment.
            // Placing a backtick string right after a block comment tests this.
            string code = "/* x *//* html */`<b/>`";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Rune_with_backslash_escape_is_skipped_correctly()
        {
            // Line 144: Math.Min vs Math.Max, and lines 146-148: boundary conditions in SkipRune
            // A rune like '\'' (escaped quote) should not consume the next quote character
            string code = "var q = '\\''\n// html\nvar a = `<b/>`";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Rune_with_escape_terminated_by_newline_does_not_overshoot()
        {
            // Tests the nl >= 0 && nl < e branch: if escaped rune has a newline before the closing quote,
            // it should stop at _i + 1, not consume past the newline
            string code = "var q = '\\x\n// html\nvar a = `<b/>`";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Simple_rune_literal_is_skipped()
        {
            // Tests Next(2) == '\'' branch: a simple rune like 'x' should be skipped as 3 chars
            // If mutated to Next(2) != '\'', it would not skip properly
            string code = "var q = 'x'\n// html\nvar a = `<b/>`";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }

        [Fact]
        public void Rune_with_newline_char_is_not_a_rune()
        {
            // Next(1) != '\n' check: if the character after the quote is a newline,
            // it's not a valid rune and should just advance by 1
            string code = "var q = '\n// html\nvar a = `<b/>`";
            Assert.Equal("<b/>", Body(code, Scan(code).Single()));
        }
    }
}
