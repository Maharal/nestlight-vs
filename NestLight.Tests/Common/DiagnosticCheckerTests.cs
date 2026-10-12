using System.Linq;
using NestLight.Common;
using Xunit;

namespace NestLight.Tests
{
    public class DiagnosticCheckerTests
    {
        // ---- HTML ----

        [Fact]
        public void Valid_html_produces_no_diagnostics()
        {
            string text = "<div><p>hello</p></div>";
            Assert.Empty(DiagnosticChecker.CheckHtml(text, 0, text.Length));
        }

        [Fact]
        public void Unclosed_html_tag_is_reported()
        {
            string text = "<div><p>hello</div>";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unclosed HTML tag <p>", diags[0].Message);
        }

        [Fact]
        public void Closing_tag_without_opening_is_reported()
        {
            string text = "</span>";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("no matching opening tag", diags[0].Message);
        }

        [Fact]
        public void Self_closing_tags_are_not_flagged()
        {
            string text = "<br/><img/>";
            Assert.Empty(DiagnosticChecker.CheckHtml(text, 0, text.Length));
        }

        [Fact]
        public void Void_elements_are_not_flagged()
        {
            string text = "<br><img><input>";
            Assert.Empty(DiagnosticChecker.CheckHtml(text, 0, text.Length));
        }

        [Fact]
        public void Unclosed_html_comment_is_reported()
        {
            string text = "<!-- incomplete";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unclosed HTML comment", diags[0].Message);
        }

        [Fact]
        public void Closed_html_comment_is_fine()
        {
            string text = "<!-- complete --><div></div>";
            Assert.Empty(DiagnosticChecker.CheckHtml(text, 0, text.Length));
        }

        [Fact]
        public void Style_and_script_content_is_skipped()
        {
            string text = "<style>.x { } </style><script>var x = 1;</script>";
            Assert.Empty(DiagnosticChecker.CheckHtml(text, 0, text.Length));
        }

        [Fact]
        public void SVG_void_elements_are_recognized()
        {
            string text = "<svg><path d=\"M0 0\"><circle r=\"5\"></svg>";
            Assert.Empty(DiagnosticChecker.CheckHtml(text, 0, text.Length));
        }

        [Fact]
        public void Substring_range_is_respected()
        {
            string text = "xxxxx<div></div>xxxxx";
            Assert.Empty(DiagnosticChecker.CheckHtml(text, 5, 16));
        }

        // ---- JSON ----

        [Fact]
        public void Valid_json_produces_no_diagnostics()
        {
            string text = "{\"key\": [1, 2, 3]}";
            Assert.Empty(DiagnosticChecker.CheckJson(text, 0, text.Length));
        }

        [Fact]
        public void Unclosed_brace_is_reported()
        {
            string text = "{\"key\": 1";
            var diags = DiagnosticChecker.CheckJson(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unclosed '{'", diags[0].Message);
        }

        [Fact]
        public void Unclosed_bracket_is_reported()
        {
            string text = "[1, 2";
            var diags = DiagnosticChecker.CheckJson(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unclosed '['", diags[0].Message);
        }

        [Fact]
        public void Unexpected_closing_brace_is_reported()
        {
            string text = "}";
            var diags = DiagnosticChecker.CheckJson(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unexpected closing '}'", diags[0].Message);
        }

        [Fact]
        public void Unexpected_closing_bracket_is_reported()
        {
            string text = "]";
            var diags = DiagnosticChecker.CheckJson(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unexpected closing ']'", diags[0].Message);
        }

        [Fact]
        public void Unterminated_json_string_is_reported()
        {
            string text = "{\"key";
            var diags = DiagnosticChecker.CheckJson(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unterminated JSON string"));
        }

        [Fact]
        public void Escaped_quotes_in_json_strings_are_handled()
        {
            string text = "{\"key\": \"value with \\\" quote\"}";
            Assert.Empty(DiagnosticChecker.CheckJson(text, 0, text.Length));
        }

        // ---- XML ----

        [Fact]
        public void Valid_xml_produces_no_diagnostics()
        {
            string text = "<root><child>text</child></root>";
            Assert.Empty(DiagnosticChecker.CheckXml(text, 0, text.Length));
        }

        [Fact]
        public void Unclosed_xml_tag_is_reported()
        {
            string text = "<root><child>text</root>";
            var diags = DiagnosticChecker.CheckXml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unclosed XML tag <child>", diags[0].Message);
        }

        [Fact]
        public void Xml_closing_without_opening_is_reported()
        {
            string text = "</orphan>";
            var diags = DiagnosticChecker.CheckXml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("no matching opening tag", diags[0].Message);
        }

        [Fact]
        public void Xml_self_closing_is_fine()
        {
            string text = "<item/>";
            Assert.Empty(DiagnosticChecker.CheckXml(text, 0, text.Length));
        }

        [Fact]
        public void Xml_processing_instructions_are_skipped()
        {
            string text = "<?xml version=\"1.0\"?><root></root>";
            Assert.Empty(DiagnosticChecker.CheckXml(text, 0, text.Length));
        }

        [Fact]
        public void Xml_case_sensitive_tag_matching()
        {
            string text = "<Root></root>";
            var diags = DiagnosticChecker.CheckXml(text, 0, text.Length).ToList();
            Assert.True(diags.Count >= 1);
        }

        // ---- Check dispatch ----

        [Theory]
        [InlineData("html")]
        [InlineData("htm")]
        [InlineData("svg")]
        [InlineData("HTML")]
        public void Check_dispatches_html(string id)
        {
            string text = "<div></div>";
            Assert.Empty(DiagnosticChecker.Check(id, text, 0, text.Length));
        }

        [Theory]
        [InlineData("json")]
        [InlineData("JSON")]
        public void Check_dispatches_json(string id)
        {
            string text = "{}";
            Assert.Empty(DiagnosticChecker.Check(id, text, 0, text.Length));
        }

        [Theory]
        [InlineData("xml")]
        [InlineData("XML")]
        public void Check_dispatches_xml(string id)
        {
            string text = "<a></a>";
            Assert.Empty(DiagnosticChecker.Check(id, text, 0, text.Length));
        }

        [Fact]
        public void Unknown_language_produces_no_diagnostics()
        {
            Assert.Empty(DiagnosticChecker.Check("sql", "SELECT *", 0, 8));
        }
    }
}
