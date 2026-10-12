using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>
    /// Boundary and mutation-killing tests for all host scanners. Each test targets a specific
    /// mutation a tool could make (off-by-one, swapped condition, removed branch).
    /// </summary>
    public class HostScannerMutationTests
    {
        private static IReadOnlyList<EmbeddedString> Scan(HostLanguage host, string code)
        {
            return Lexer.Scan(host, code);
        }

        private static string Body(string code, EmbeddedString s) { return code.Substring(s.Start, s.End - s.Start); }

        // ---- boundary: empty input -------------------------------------------------------------

        [Theory]
        [InlineData(HostLanguage.JavaScript)]
        [InlineData(HostLanguage.CSharp)]
        [InlineData(HostLanguage.Python)]
        [InlineData(HostLanguage.Cpp)]
        [InlineData(HostLanguage.Rust)]
        [InlineData(HostLanguage.Go)]
        [InlineData(HostLanguage.Java)]
        public void Empty_input_returns_no_strings(HostLanguage host)
        {
            Assert.Empty(Scan(host, ""));
        }

        // ---- boundary: only whitespace ---------------------------------------------------------

        [Theory]
        [InlineData(HostLanguage.JavaScript)]
        [InlineData(HostLanguage.CSharp)]
        [InlineData(HostLanguage.Python)]
        [InlineData(HostLanguage.Cpp)]
        [InlineData(HostLanguage.Rust)]
        [InlineData(HostLanguage.Go)]
        [InlineData(HostLanguage.Java)]
        public void Whitespace_only_returns_no_strings(HostLanguage host)
        {
            Assert.Empty(Scan(host, "   \n\t\n   "));
        }

        // ---- boundary: code with no strings at all ---------------------------------------------

        [Theory]
        [InlineData(HostLanguage.JavaScript, "var x = 42;")]
        [InlineData(HostLanguage.CSharp, "int x = 42;")]
        [InlineData(HostLanguage.Python, "x = 42")]
        [InlineData(HostLanguage.Cpp, "int x = 42;")]
        [InlineData(HostLanguage.Rust, "let x = 42;")]
        [InlineData(HostLanguage.Go, "var x = 42")]
        [InlineData(HostLanguage.Java, "int x = 42;")]
        public void Code_without_strings_returns_nothing(HostLanguage host, string code)
        {
            Assert.Empty(Scan(host, code));
        }

        // ---- boundary: marker followed by no string (marker must expire) -----------------------

        [Theory]
        [InlineData(HostLanguage.CSharp, "// html\nint x = 42;")]
        [InlineData(HostLanguage.Cpp, "// html\nint x = 42;")]
        [InlineData(HostLanguage.Rust, "// html\nlet x = 42;")]
        [InlineData(HostLanguage.Go, "// html\nvar x = 42")]
        [InlineData(HostLanguage.Java, "// html\nint x = 42;")]
        public void Marker_with_no_string_on_next_line_produces_nothing(HostLanguage host, string code)
        {
            Assert.Empty(Scan(host, code));
        }

        // ---- boundary: marker two lines above the string (too far) -----------------------------

        [Theory]
        [InlineData(HostLanguage.CSharp, "// html\n\nvar a = \"x\";")]
        [InlineData(HostLanguage.Cpp, "// html\n\nauto a = R\"(x)\";")]
        [InlineData(HostLanguage.Rust, "// html\n\nlet a = r\"x\";")]
        [InlineData(HostLanguage.Go, "// html\n\nvar a = `x`")]
        [InlineData(HostLanguage.Java, "// html\n\nString a = \"x\";")]
        public void Marker_two_lines_above_does_not_mark(HostLanguage host, string code)
        {
            Assert.Empty(Scan(host, code));
        }

        // ---- boundary: block comment marker on same line ---------------------------------------

        [Theory]
        [InlineData(HostLanguage.CSharp, "var a = /* html */ \"<b/>\";")]
        [InlineData(HostLanguage.Cpp, "auto a = /* html */ R\"(<b/>)\";")]
        [InlineData(HostLanguage.Rust, "let a = /* html */ r\"<b/>\";")]
        [InlineData(HostLanguage.Go, "var a = /* html */ `<b/>`")]
        [InlineData(HostLanguage.Java, "String a = /* html */ \"<b/>\";")]
        public void Block_comment_marker_on_same_line_marks(HostLanguage host, string code)
        {
            Assert.Equal("html", Scan(host, code).Single().EmbeddedLanguageId);
        }

        // ---- mutation: unclosed block comment eats the rest ------------------------------------

        [Theory]
        [InlineData(HostLanguage.CSharp, "/* unclosed\nvar a = \"x\";")]
        [InlineData(HostLanguage.Cpp, "/* unclosed\nauto a = R\"(x)\";")]
        [InlineData(HostLanguage.Rust, "/* unclosed\nlet a = r\"x\";")]
        [InlineData(HostLanguage.Go, "/* unclosed\nvar a = `x`")]
        [InlineData(HostLanguage.Java, "/* unclosed\nString a = \"x\";")]
        public void Unclosed_block_comment_hides_everything_after(HostLanguage host, string code)
        {
            Assert.Empty(Scan(host, code));
        }

        // ---- mutation: line comment hides rest of line -----------------------------------------

        [Theory]
        [InlineData(HostLanguage.CSharp, "// var a = \"x\";")]
        [InlineData(HostLanguage.Cpp, "// auto a = R\"(x)\";")]
        [InlineData(HostLanguage.Rust, "// let a = r\"x\";")]
        [InlineData(HostLanguage.Go, "// var a = `x`")]
        [InlineData(HostLanguage.Java, "// String a = \"x\";")]
        public void Line_comment_hides_strings_on_same_line(HostLanguage host, string code)
        {
            Assert.Empty(Scan(host, code));
        }

        // ---- C# specific mutations ------------------------------------------------------------

        [Fact]
        public void CSharp_char_literal_with_escaped_quote_does_not_eat_next_string()
        {
            string code = "char c = '\"'; // html\nvar a = \"<b/>\";";
            Assert.Equal("<b/>", Body(code, Scan(HostLanguage.CSharp, code).Single()));
        }

        [Fact]
        public void CSharp_dollar_alone_is_not_a_string()
        {
            Assert.Empty(Scan(HostLanguage.CSharp, "// html\nvar a = $;"));
        }

        [Fact]
        public void CSharp_triple_dollar_without_raw_quotes_is_rejected()
        {
            Assert.Empty(Scan(HostLanguage.CSharp, "// html\nvar a = $$$x;"));
        }

        [Fact]
        public void CSharp_interpolation_close_brace_must_match_dollar_count()
        {
            string code = "// html\nvar a = $$\"\"\"<b>{{x}}</b>\"\"\";";
            var s = Scan(HostLanguage.CSharp, code).Single();
            Assert.Single(s.Interpolations);
        }

        // ---- JavaScript specific mutations -----------------------------------------------------

        [Fact]
        public void JS_template_with_nested_template_in_interpolation()
        {
            string code = "html`<div>${css`color:red`}</div>`";
            var strings = Scan(HostLanguage.JavaScript, code);
            Assert.True(strings.Count >= 2);
        }

        [Fact]
        public void JS_escaped_backtick_does_not_end_template()
        {
            string code = "html`a\\`b`";
            var s = Scan(HostLanguage.JavaScript, code).Single();
            Assert.Equal("a\\`b", Body(code, s));
        }

        // ---- Python specific mutations ---------------------------------------------------------

        [Fact]
        public void Python_triple_quote_is_found()
        {
            string code = "# html\na = \"\"\"<b/>\n\"\"\"";
            Assert.Equal("<b/>\n", Body(code, Scan(HostLanguage.Python, code).Single()));
        }

        [Fact]
        public void Python_fstring_interpolation_is_found()
        {
            string code = "# html\na = f\"<b>{x}</b>\"";
            var s = Scan(HostLanguage.Python, code).Single();
            Assert.Single(s.Interpolations);
        }

        // ---- Rust specific mutations -----------------------------------------------------------

        [Fact]
        public void Rust_r_without_quote_is_not_a_raw_string()
        {
            Assert.Empty(Scan(HostLanguage.Rust, "// html\nlet a = rx;"));
        }

        [Fact]
        public void Rust_hash_mismatch_does_not_end_raw_string()
        {
            string code = "// html\nlet a = r##\"a\"#b\"##;";
            Assert.Equal("a\"#b", Body(code, Scan(HostLanguage.Rust, code).Single()));
        }

        // ---- Go specific mutations -------------------------------------------------------------

        [Fact]
        public void Go_backtick_with_backslash_is_literal()
        {
            string code = "// html\nvar a = `\\n\\t`";
            var s = Scan(HostLanguage.Go, code).Single();
            Assert.Equal("\\n\\t", Body(code, s));
            Assert.Empty(s.Escapes);
        }

        // ---- Java specific mutations -----------------------------------------------------------

        [Fact]
        public void Java_text_block_single_quote_in_content()
        {
            string code = "// html\nString a = \"\"\"\na\"b\n\"\"\";";
            Assert.Contains("a\"b", Body(code, Scan(HostLanguage.Java, code).Single()));
        }

        [Fact]
        public void Java_two_quotes_is_a_regular_string_not_text_block()
        {
            string code = "// html\nString a = \"\";";
            var s = Scan(HostLanguage.Java, code).Single();
            Assert.Equal("", Body(code, s));
        }
    }

    /// <summary>Tests for the new SQL and CSS diagnostic checks.</summary>
    public class DiagnosticCheckerSqlCssTests
    {
        // ---- SQL ----

        [Fact]
        public void Valid_sql_produces_no_diagnostics()
        {
            string text = "SELECT id FROM users WHERE name = 'test'";
            Assert.Empty(DiagnosticChecker.CheckSql(text, 0, text.Length));
        }

        [Fact]
        public void Unmatched_paren_in_sql_is_reported()
        {
            string text = "SELECT COUNT(id FROM users";
            var diags = DiagnosticChecker.CheckSql(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unclosed '('", diags[0].Message);
        }

        [Fact]
        public void Extra_closing_paren_in_sql_is_reported()
        {
            string text = "SELECT id) FROM users";
            var diags = DiagnosticChecker.CheckSql(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unexpected closing ')'", diags[0].Message);
        }

        [Fact]
        public void Unterminated_sql_string_is_reported()
        {
            string text = "SELECT * FROM t WHERE x = 'abc";
            var diags = DiagnosticChecker.CheckSql(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unterminated SQL string", diags[0].Message);
        }

        [Fact]
        public void Sql_comment_is_skipped()
        {
            string text = "SELECT 1 -- ) unclosed paren in comment";
            Assert.Empty(DiagnosticChecker.CheckSql(text, 0, text.Length));
        }

        [Fact]
        public void Sql_escaped_single_quote_is_not_end_of_string()
        {
            string text = "SELECT 'it''s fine'";
            Assert.Empty(DiagnosticChecker.CheckSql(text, 0, text.Length));
        }

        // ---- CSS ----

        [Fact]
        public void Valid_css_produces_no_diagnostics()
        {
            string text = ".item { color: red; }";
            Assert.Empty(DiagnosticChecker.CheckCss(text, 0, text.Length));
        }

        [Fact]
        public void Unmatched_brace_in_css_is_reported()
        {
            string text = ".item { color: red;";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unclosed '{'", diags[0].Message);
        }

        [Fact]
        public void Extra_closing_brace_in_css_is_reported()
        {
            string text = "color: red; }";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unexpected closing '}'", diags[0].Message);
        }

        [Fact]
        public void Unmatched_paren_in_css_is_reported()
        {
            string text = ".item { background: url(foo; }";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unclosed '('"));
        }

        [Fact]
        public void Unterminated_css_comment_is_reported()
        {
            string text = ".item { /* unclosed";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unterminated CSS comment"));
        }

        [Fact]
        public void Unterminated_css_string_is_reported()
        {
            string text = ".item { content: \"hello; }";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unterminated CSS string"));
        }

        [Fact]
        public void Css_comment_hides_braces()
        {
            string text = "/* { } */ .item { color: red; }";
            Assert.Empty(DiagnosticChecker.CheckCss(text, 0, text.Length));
        }

        [Fact]
        public void Css_string_hides_braces()
        {
            string text = ".item { content: \"}\"; }";
            Assert.Empty(DiagnosticChecker.CheckCss(text, 0, text.Length));
        }

        [Fact]
        public void Css_escaped_char_in_string_does_not_end_it()
        {
            string text = ".item { content: \"hello\\\"\"; }";
            Assert.Empty(DiagnosticChecker.CheckCss(text, 0, text.Length));
        }

        [Fact]
        public void Unmatched_bracket_in_css_is_reported()
        {
            string text = "[class { }";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unclosed '['"));
        }
    }
}
