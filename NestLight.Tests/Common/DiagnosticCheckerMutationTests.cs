using System.Linq;
using NestLight.Common;
using Xunit;

namespace NestLight.Tests
{
    public class DiagnosticCheckerMutationTests
    {
        // ---- Check dispatch mutations (L23-L39) ----
        // String mutations: each language string mutated to ""
        // Logical mutations: || to &&, negation

        [Fact]
        public void Html_dispatches_correctly_not_as_empty_string()
        {
            // If "html" mutated to "", then Check("html",...) would not dispatch to CheckHtml
            string text = "<div>unclosed";
            var diags = DiagnosticChecker.Check("html", text, 0, text.Length).ToList();
            Assert.NotEmpty(diags);
        }

        [Fact]
        public void Htm_dispatches_correctly()
        {
            string text = "<div>unclosed";
            var diags = DiagnosticChecker.Check("htm", text, 0, text.Length).ToList();
            Assert.NotEmpty(diags);
        }

        [Fact]
        public void Svg_dispatches_correctly()
        {
            string text = "<div>unclosed";
            var diags = DiagnosticChecker.Check("svg", text, 0, text.Length).ToList();
            Assert.NotEmpty(diags);
        }

        [Fact]
        public void Json_dispatches_and_finds_error()
        {
            string text = "{unclosed";
            var diags = DiagnosticChecker.Check("json", text, 0, text.Length).ToList();
            Assert.NotEmpty(diags);
        }

        [Fact]
        public void Xml_dispatches_and_finds_error()
        {
            string text = "<root>unclosed";
            var diags = DiagnosticChecker.Check("xml", text, 0, text.Length).ToList();
            Assert.NotEmpty(diags);
        }

        [Fact]
        public void Sql_dispatches_and_finds_error()
        {
            string text = "SELECT (";
            var diags = DiagnosticChecker.Check("sql", text, 0, text.Length).ToList();
            Assert.NotEmpty(diags);
        }

        [Fact]
        public void Css_dispatches_and_finds_error()
        {
            string text = ".x {";
            var diags = DiagnosticChecker.Check("css", text, 0, text.Length).ToList();
            Assert.NotEmpty(diags);
        }

        [Fact]
        public void Yaml_dispatches_and_finds_error()
        {
            string text = "name: first\nname: second";
            var diags = DiagnosticChecker.Check("yaml", text, 0, text.Length).ToList();
            Assert.NotEmpty(diags);
        }

        [Fact]
        public void Yml_dispatches_and_finds_error()
        {
            string text = "name: first\nname: second";
            var diags = DiagnosticChecker.Check("yml", text, 0, text.Length).ToList();
            Assert.NotEmpty(diags);
        }

        [Fact]
        public void Graphql_dispatches_and_finds_error()
        {
            string text = "{ unclosed";
            var diags = DiagnosticChecker.Check("graphql", text, 0, text.Length).ToList();
            Assert.NotEmpty(diags);
        }

        [Fact]
        public void Gql_dispatches_and_finds_error()
        {
            string text = "{ unclosed";
            var diags = DiagnosticChecker.Check("gql", text, 0, text.Length).ToList();
            Assert.NotEmpty(diags);
        }

        // ---- CheckHtml mutations ----

        // L48: i <= end boundary
        [Fact]
        public void Html_loop_boundary_at_exact_end()
        {
            // Text with < at exact end-1: should not be found as tag
            string text = "<div></div><";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            // The trailing < is not a complete tag, no crash expected
            Assert.Empty(diags);
        }

        // L53: lt + 3 <= end / lt - 3 (comment detection)
        [Fact]
        public void Html_comment_marker_at_boundary()
        {
            // "<!--" exactly fills the text (lt=0, lt+3=3, end=4): exactly 4 chars
            string text = "<!--";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unclosed HTML comment", diags[0].Message);
        }

        // L55: end + lt vs end - lt in IndexOf
        // L56: closeComment <= 0 boundary
        [Fact]
        public void Html_comment_closed_right_after_opening()
        {
            string text = "<!---->";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Empty(diags);
        }

        // L58: Math.Min vs Math.Max; lt + 4 vs lt - 4
        [Fact]
        public void Html_unclosed_comment_diagnostic_spans_correctly()
        {
            string text = "<!--";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Equal(0, diags[0].Start);
            Assert.True(diags[0].End <= text.Length);
            Assert.True(diags[0].End > diags[0].Start);
        }

        // L61: closeComment - 3 vs + 3
        [Fact]
        public void Html_comment_resume_position_after_close()
        {
            string text = "<!-- comment --><div></div>";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Empty(diags);
        }

        // L66: gt <= 0 boundary
        [Fact]
        public void Html_no_closing_gt_skips_to_next()
        {
            string text = "<div><  <span></span>";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unclosed"));
        }

        // L69: nameStart <= gt
        // L72: nameEnd < gt with char checks (- and :)
        [Fact]
        public void Html_tag_with_hyphen_and_colon()
        {
            string text = "<my-tag></my-tag><ns:tag></ns:tag>";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Empty(diags);
        }

        // L72: text[nameEnd] != '-', text[nameEnd] != ':'
        [Fact]
        public void Html_tag_name_stops_at_space()
        {
            string text = "<div class=\"a\"></div>";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Empty(diags);
        }

        // L81: k++ vs k-- (closing tag search)
        [Fact]
        public void Html_nested_closing_tags_match_correctly()
        {
            string text = "<div><span></span></div>";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Empty(diags);
        }

        // L86: gt + 1 vs gt - 1, "Closing tag" message string
        [Fact]
        public void Html_closing_tag_diagnostic_position()
        {
            string text = "</orphan>";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Equal(0, diags[0].Start);
            Assert.Equal(text.Length, diags[0].End);
        }

        // L92: style/script string mutations, || vs &&
        [Fact]
        public void Html_style_tag_skips_inner_tags()
        {
            string text = "<style><div>not real</div></style>";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Empty(diags);
        }

        [Fact]
        public void Html_script_tag_skips_inner_tags()
        {
            string text = "<script><div>not real</div></script>";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Empty(diags);
        }

        // L94: "</" string mutation
        [Fact]
        public void Html_style_with_no_close_is_tracked()
        {
            string text = "<style>.x { }";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.NotEmpty(diags);
            Assert.Contains(diags, d => d.Message.Contains("Unclosed HTML tag <style>"));
        }

        // L95-99: closePos, closeGt arithmetic mutations
        [Fact]
        public void Html_style_close_tag_with_attributes()
        {
            // Unusual but the parser should handle it
            string text = "<style>.x{}</style >";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Empty(diags);
        }

        // L99: closeGt < 0 / > 0, closeGt - 1 vs + 1
        [Fact]
        public void Html_script_without_gt_on_close()
        {
            // "</script" without ">" at the end of range
            string text = "<script>x</script";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            // Script close found but no >, parser uses fallback position
            Assert.Empty(diags);
        }

        // L103: gt + 1 vs gt - 1, L106: gt + 1 vs gt - 1
        [Fact]
        public void Html_open_tag_advances_past_gt()
        {
            string text = "<div><p>text</p></div>";
            var diags = DiagnosticChecker.CheckHtml(text, 0, text.Length).ToList();
            Assert.Empty(diags);
        }

        // ---- CheckJson mutations (L121-L129) ----

        // L121: i++ (statement mutation)
        [Fact]
        public void Json_escaped_quote_inside_string()
        {
            string text = "{\"key\\\"value\": 1}";
            Assert.Empty(DiagnosticChecker.CheckJson(text, 0, text.Length));
        }

        // L123: i + 1 vs i - 1
        [Fact]
        public void Json_brace_positions_reported_correctly()
        {
            string text = "  }";
            var diags = DiagnosticChecker.CheckJson(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Equal(2, diags[0].Start);
            Assert.Equal(3, diags[0].End);
        }

        // L125: i + 1 vs i - 1
        [Fact]
        public void Json_bracket_positions_reported_correctly()
        {
            string text = "  ]";
            var diags = DiagnosticChecker.CheckJson(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Equal(2, diags[0].Start);
            Assert.Equal(3, diags[0].End);
        }

        // L127-129: end - 1 vs end + 1, start + 1 vs start - 1
        [Fact]
        public void Json_unterminated_string_diagnostic_position()
        {
            string text = "\"abc";
            var diags = DiagnosticChecker.CheckJson(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unterminated JSON string"));
            var sd = diags.First(d => d.Message.Contains("Unterminated"));
            Assert.Equal(text.Length - 1, sd.Start);
            Assert.Equal(text.Length, sd.End);
        }

        [Fact]
        public void Json_unclosed_brace_diagnostic_position()
        {
            string text = "{\"a\": 1";
            var diags = DiagnosticChecker.CheckJson(text, 0, text.Length).ToList();
            var bd = diags.First(d => d.Message.Contains("Unclosed '{'"));
            Assert.Equal(0, bd.Start);
            Assert.Equal(1, bd.End);
        }

        // ---- CheckXml mutations (L136-L166) ----

        // L136: i <= end
        [Fact]
        public void Xml_loop_handles_end_correctly()
        {
            string text = "<a></a> trailing text";
            Assert.Empty(DiagnosticChecker.CheckXml(text, 0, text.Length));
        }

        // L141: gt <= 0
        [Fact]
        public void Xml_tag_without_gt()
        {
            string text = "<a";
            var diags = DiagnosticChecker.CheckXml(text, 0, text.Length).ToList();
            // No '>' found, parser skips
            Assert.Empty(diags);
        }

        // L144: nameStart < gt handling for '?' and '!' prefixes
        [Fact]
        public void Xml_doctype_is_skipped()
        {
            string text = "<!DOCTYPE html><root></root>";
            Assert.Empty(DiagnosticChecker.CheckXml(text, 0, text.Length));
        }

        // L145-146: nameStart > gt, <= gt, < gt boundaries
        [Fact]
        public void Xml_closing_tag_with_slash()
        {
            string text = "<root><child></child></root>";
            Assert.Empty(DiagnosticChecker.CheckXml(text, 0, text.Length));
        }

        // L149: nameEnd boundary mutations and text[nameEnd] comparisons
        [Fact]
        public void Xml_tag_with_attributes()
        {
            string text = "<root attr=\"val\"><child/></root>";
            Assert.Empty(DiagnosticChecker.CheckXml(text, 0, text.Length));
        }

        // L159: statement mutation on continue
        [Fact]
        public void Xml_matching_close_removes_open_tags()
        {
            string text = "<a><b></b></a>";
            Assert.Empty(DiagnosticChecker.CheckXml(text, 0, text.Length));
        }

        // L161: gt + 1 vs gt - 1, tag name in error message
        [Fact]
        public void Xml_closing_without_opening_message()
        {
            string text = "</orphan>";
            var diags = DiagnosticChecker.CheckXml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("orphan", diags[0].Message);
            Assert.Equal(0, diags[0].Start);
            Assert.Equal(text.Length, diags[0].End);
        }

        // L165: gt + 1 vs gt - 1 (open tag advance), L166: gt + 1
        [Fact]
        public void Xml_self_closing_does_not_push_stack()
        {
            string text = "<a/><b/><c/>";
            Assert.Empty(DiagnosticChecker.CheckXml(text, 0, text.Length));
        }

        // ---- CheckSql mutations (L172-L206) ----

        // L184: i + 1 > end vs < end, text[i+1] != quote, i - 1 vs i + 1
        [Fact]
        public void Sql_escaped_quote_in_string()
        {
            // SQL escapes quotes by doubling: 'it''s'
            string text = "SELECT 'it''s fine'";
            Assert.Empty(DiagnosticChecker.CheckSql(text, 0, text.Length));
        }

        [Fact]
        public void Sql_quote_at_end_of_range()
        {
            // String that ends at range boundary
            string text = "'abc'";
            Assert.Empty(DiagnosticChecker.CheckSql(text, 0, text.Length));
        }

        // L189: c == '-' && i + 1 < end && text[i+1] == '-'
        [Fact]
        public void Sql_line_comment_skips_rest_of_line()
        {
            string text = "SELECT 1 -- comment\nFROM t";
            Assert.Empty(DiagnosticChecker.CheckSql(text, 0, text.Length));
        }

        [Fact]
        public void Sql_single_dash_is_not_comment()
        {
            string text = "SELECT 1-2";
            Assert.Empty(DiagnosticChecker.CheckSql(text, 0, text.Length));
        }

        [Fact]
        public void Sql_dash_at_end_is_not_comment()
        {
            string text = "SELECT 1 -";
            Assert.Empty(DiagnosticChecker.CheckSql(text, 0, text.Length));
        }

        // L191: i - 2 vs i + 2, L192: nl < 0 / end - 1 vs end + 1
        [Fact]
        public void Sql_comment_with_no_newline_goes_to_end()
        {
            string text = "-- comment no newline";
            Assert.Empty(DiagnosticChecker.CheckSql(text, 0, text.Length));
        }

        // L192: conditional true/false mutation
        [Fact]
        public void Sql_comment_with_newline_continues_after()
        {
            string text = "-- comment\n(";
            var diags = DiagnosticChecker.CheckSql(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unclosed '('", diags[0].Message);
        }

        // L200: parens <= 0 boundary
        [Fact]
        public void Sql_unexpected_close_paren()
        {
            string text = "SELECT )";
            var diags = DiagnosticChecker.CheckSql(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unexpected closing ')'", diags[0].Message);
        }

        // L201: i + 1 vs i - 1 (paren diagnostic position)
        [Fact]
        public void Sql_close_paren_diagnostic_position()
        {
            string text = ")";
            var diags = DiagnosticChecker.CheckSql(text, 0, text.Length).ToList();
            Assert.Equal(0, diags[0].Start);
            Assert.Equal(1, diags[0].End);
        }

        // L204-205: end - 1, start + 1 vs start - 1, Math.Min vs Max
        [Fact]
        public void Sql_unterminated_string_diagnostic()
        {
            string text = "SELECT 'abc";
            var diags = DiagnosticChecker.CheckSql(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unterminated SQL string"));
        }

        [Fact]
        public void Sql_unclosed_paren_diagnostic()
        {
            string text = "SELECT (1, 2";
            var diags = DiagnosticChecker.CheckSql(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unclosed '('"));
            var pd = diags.First(d => d.Message.Contains("Unclosed '('"));
            Assert.True(pd.End <= text.Length);
        }

        // L195: quote detection
        [Fact]
        public void Sql_double_quoted_string()
        {
            string text = "SELECT \"column\"";
            Assert.Empty(DiagnosticChecker.CheckSql(text, 0, text.Length));
        }

        // ---- CheckCss mutations (L208-L260) ----

        // L221: c == '*' && i + 1 < end, text[i+1] == '/'
        [Fact]
        public void Css_block_comment_is_handled()
        {
            string text = "/* comment */ .a { }";
            Assert.Empty(DiagnosticChecker.CheckCss(text, 0, text.Length));
        }

        [Fact]
        public void Css_star_at_end_is_not_comment_close()
        {
            string text = "/* comment *";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unterminated CSS comment"));
        }

        // L224: i++ vs i-- (in comment: skip both * and /)
        [Fact]
        public void Css_comment_close_advances_correctly()
        {
            string text = "/* comment */.a { }";
            Assert.Empty(DiagnosticChecker.CheckCss(text, 0, text.Length));
        }

        // L234: c == '/' && i + 1 < end && text[i+1] == '*'
        // L234: i++ vs i-- (in comment open: skip both / and *)
        [Fact]
        public void Css_comment_open_detection()
        {
            string text = ".a { /* open */ color: red; }";
            Assert.Empty(DiagnosticChecker.CheckCss(text, 0, text.Length));
        }

        [Fact]
        public void Css_slash_at_end_is_not_comment_open()
        {
            string text = ".a { color: red; } /";
            Assert.Empty(DiagnosticChecker.CheckCss(text, 0, text.Length));
        }

        // L230: c == '\\' in string: i++ (escape handling)
        [Fact]
        public void Css_string_with_escaped_quote()
        {
            string text = ".a::after { content: \"hello \\\" world\"; }";
            Assert.Empty(DiagnosticChecker.CheckCss(text, 0, text.Length));
        }

        // L240: i + 1 vs i - 1 for paren
        [Fact]
        public void Css_parens_balanced()
        {
            string text = ".a { background: url('test'); }";
            Assert.Empty(DiagnosticChecker.CheckCss(text, 0, text.Length));
        }

        // L255-259: end - 1, start + 1, Math.Min vs Math.Max
        [Fact]
        public void Css_unterminated_comment_diagnostic()
        {
            string text = "/* unclosed";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unterminated CSS comment"));
            var cd = diags.First(d => d.Message.Contains("Unterminated"));
            Assert.Equal(text.Length - 1, cd.Start);
            Assert.Equal(text.Length, cd.End);
        }

        [Fact]
        public void Css_unterminated_string_diagnostic()
        {
            string text = ".a { content: \"abc; }";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unterminated CSS string"));
        }

        [Fact]
        public void Css_unclosed_brace_diagnostic()
        {
            string text = ".a {";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unclosed '{'"));
            var bd = diags.First(d => d.Message.Contains("Unclosed '{'"));
            Assert.True(bd.End <= text.Length);
        }

        [Fact]
        public void Css_unclosed_paren_diagnostic()
        {
            string text = ".a { background: url(";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unclosed '('"));
        }

        [Fact]
        public void Css_unclosed_bracket_diagnostic()
        {
            string text = ".a[href";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unclosed '['"));
        }

        [Fact]
        public void Css_unexpected_close_brace()
        {
            string text = "}";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unexpected closing '}'", diags[0].Message);
        }

        [Fact]
        public void Css_unexpected_close_paren()
        {
            string text = ")";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unexpected closing ')'", diags[0].Message);
        }

        [Fact]
        public void Css_unexpected_close_bracket()
        {
            string text = "]";
            var diags = DiagnosticChecker.CheckCss(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unexpected closing ']'", diags[0].Message);
        }

        // L264-265: boolean mutations (false to true for prevWasSpace/prevWasTab)
        // L268: i < end vs i <= end
        // L274: lineNumber-- vs ++ (statement mutation)
        // L276-278: boolean mutations

        // ---- CheckYaml mutations ----

        [Fact]
        public void Yaml_tabs_only_indentation_is_fine()
        {
            string text = "root:\n\tindented: true";
            Assert.Empty(DiagnosticChecker.CheckYaml(text, 0, text.Length));
        }

        [Fact]
        public void Yaml_spaces_only_indentation_is_fine()
        {
            string text = "root:\n  indented: true";
            Assert.Empty(DiagnosticChecker.CheckYaml(text, 0, text.Length));
        }

        // L285: i > lineStart vs >= lineStart
        [Fact]
        public void Yaml_mixed_indent_at_column_one()
        {
            // Space then tab starting at lineStart
            string text = "root:\n \tchild: 1";
            var diags = DiagnosticChecker.CheckYaml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Mixed tabs and spaces", diags[0].Message);
        }

        // L287: prevWasSpace || c == '\t' vs &&
        [Fact]
        public void Yaml_tab_then_space_is_mixed()
        {
            string text = "root:\n\t child: 1";
            var diags = DiagnosticChecker.CheckYaml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Mixed tabs and spaces", diags[0].Message);
        }

        // L289: Math.Min vs Math.Max, i + 1 vs i - 1
        [Fact]
        public void Yaml_mixed_indent_diagnostic_position()
        {
            string text = "root:\n \tindented: true";
            var diags = DiagnosticChecker.CheckYaml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.True(diags[0].Start >= 0);
            Assert.True(diags[0].End <= text.Length);
            Assert.True(diags[0].End > diags[0].Start);
        }

        // L290: skip < end / text[skip] == '\n'
        [Fact]
        public void Yaml_mixed_indent_skips_rest_of_line()
        {
            // After reporting mixed indent, should skip to next line
            string text = "root:\n \tmixed: bad\n \tother: also";
            var diags = DiagnosticChecker.CheckYaml(text, 0, text.Length).ToList();
            Assert.Equal(2, diags.Count); // One per line with mixed indentation
        }

        // L298: prevWasSpace = false (boolean mutation)
        [Fact]
        public void Yaml_non_indent_chars_reset_indent_tracking()
        {
            string text = "a b c:\n  val: 1";
            Assert.Empty(DiagnosticChecker.CheckYaml(text, 0, text.Length));
        }

        // L304: duplicateStart <= end
        // L307: lineEnd <= 0
        // L309: p > lineEnd / p <= lineEnd, text[p] == ' ' && text[p] == '\t'
        [Fact]
        public void Yaml_duplicate_key_with_indent()
        {
            // Indented keys should not be detected as duplicates of top-level keys
            string text = "name: first\n  name: second";
            Assert.Empty(DiagnosticChecker.CheckYaml(text, 0, text.Length));
        }

        // L310: compound condition: p == duplicateStart && p < lineEnd && text[p] != '#' && text[p] != '-'
        [Fact]
        public void Yaml_dashed_line_is_not_duplicate_key()
        {
            string text = "items:\n- first\n- second";
            Assert.Empty(DiagnosticChecker.CheckYaml(text, 0, text.Length));
        }

        // L313: colon >= p vs > p
        [Fact]
        public void Yaml_line_without_colon_is_skipped()
        {
            string text = "just a value\nname: test";
            Assert.Empty(DiagnosticChecker.CheckYaml(text, 0, text.Length));
        }

        // L316: lineEnd - 1 vs + 1
        // L317: nextLine <= end
        // L320: nextEnd <= 0
        [Fact]
        public void Yaml_duplicate_check_across_multiple_lines()
        {
            string text = "a: 1\nb: 2\na: 3";
            var diags = DiagnosticChecker.CheckYaml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Duplicate YAML key 'a'", diags[0].Message);
        }

        // L322-326: nextLine indent checks
        [Fact]
        public void Yaml_indented_next_line_not_compared()
        {
            string text = "name: first\n  name: nested";
            Assert.Empty(DiagnosticChecker.CheckYaml(text, 0, text.Length));
        }

        // L323: np > nextLine && np >= nextEnd
        [Fact]
        public void Yaml_blank_lines_between_duplicates()
        {
            string text = "name: first\n\nname: second";
            var diags = DiagnosticChecker.CheckYaml(text, 0, text.Length).ToList();
            // Blank lines are skipped (np >= nextEnd), but the second "name:" should still be found
            Assert.Single(diags);
        }

        // L330: "" instead of "'" string mutation
        [Fact]
        public void Yaml_duplicate_message_contains_key_name()
        {
            string text = "mykey: 1\nmykey: 2";
            var diags = DiagnosticChecker.CheckYaml(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("'mykey'", diags[0].Message);
        }

        // ---- CheckGraphQl mutations (L340-L377) ----

        // L350: c != '\n' (comment end detection), boolean true
        [Fact]
        public void Graphql_comment_ends_at_newline()
        {
            string text = "# comment\n{ user }";
            Assert.Empty(DiagnosticChecker.CheckGraphQl(text, 0, text.Length));
        }

        // L351: c != '\\' (escape in string)
        [Fact]
        public void Graphql_escaped_quote_in_string()
        {
            string text = "{ field(arg: \"test \\\" end\") }";
            Assert.Empty(DiagnosticChecker.CheckGraphQl(text, 0, text.Length));
        }

        // L352: false instead of true for inComment
        [Fact]
        public void Graphql_hash_starts_comment()
        {
            string text = "# comment with { brace\n{ user }";
            Assert.Empty(DiagnosticChecker.CheckGraphQl(text, 0, text.Length));
        }

        // L353: inString = true (statement mutation)
        [Fact]
        public void Graphql_string_hides_braces()
        {
            string text = "{ field(arg: \"}\") }";
            Assert.Empty(DiagnosticChecker.CheckGraphQl(text, 0, text.Length));
        }

        // L358: i + 1 vs i - 1 for brace diagnostic
        [Fact]
        public void Graphql_unexpected_close_brace_position()
        {
            string text = "  }";
            var diags = DiagnosticChecker.CheckGraphQl(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Equal(2, diags[0].Start);
            Assert.Equal(3, diags[0].End);
        }

        // L360: parens++ vs -- (statement mutation)
        [Fact]
        public void Graphql_balanced_parens()
        {
            string text = "{ field(a: 1, b: 2) }";
            Assert.Empty(DiagnosticChecker.CheckGraphQl(text, 0, text.Length));
        }

        // L373-376: end diagnostic positions
        [Fact]
        public void Graphql_unterminated_string_position()
        {
            string text = "{ \"unclosed";
            var diags = DiagnosticChecker.CheckGraphQl(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unterminated GraphQL string"));
            var sd = diags.First(d => d.Message.Contains("Unterminated"));
            Assert.Equal(text.Length - 1, sd.Start);
            Assert.Equal(text.Length, sd.End);
        }

        [Fact]
        public void Graphql_unclosed_paren()
        {
            string text = "{ field(";
            var diags = DiagnosticChecker.CheckGraphQl(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unclosed '('"));
        }

        [Fact]
        public void Graphql_unclosed_bracket()
        {
            string text = "{ field [";
            var diags = DiagnosticChecker.CheckGraphQl(text, 0, text.Length).ToList();
            Assert.Contains(diags, d => d.Message.Contains("Unclosed '['"));
        }

        [Fact]
        public void Graphql_unexpected_close_paren()
        {
            string text = ")";
            var diags = DiagnosticChecker.CheckGraphQl(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unexpected closing ')'", diags[0].Message);
        }

        [Fact]
        public void Graphql_unexpected_close_bracket()
        {
            string text = "]";
            var diags = DiagnosticChecker.CheckGraphQl(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unexpected closing ']'", diags[0].Message);
        }

        // L375: parens < 0 instead of > 0, string mutation on "Unclosed '('"
        [Fact]
        public void Graphql_unclosed_paren_message_and_position()
        {
            string text = "(";
            var diags = DiagnosticChecker.CheckGraphQl(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unclosed '('", diags[0].Message);
            Assert.True(diags[0].End <= text.Length);
        }

        // L376: brackets < 0 instead of > 0
        [Fact]
        public void Graphql_unclosed_bracket_message_and_position()
        {
            string text = "[";
            var diags = DiagnosticChecker.CheckGraphQl(text, 0, text.Length).ToList();
            Assert.Single(diags);
            Assert.Contains("Unclosed '['", diags[0].Message);
            Assert.True(diags[0].End <= text.Length);
        }

        // Substring range tests for all languages
        [Fact]
        public void Html_respects_start_offset()
        {
            string text = "XXX<div>XXX";
            var diags = DiagnosticChecker.CheckHtml(text, 3, 8).ToList();
            Assert.NotEmpty(diags);
            Assert.Contains("Unclosed HTML tag <div>", diags[0].Message);
        }

        [Fact]
        public void Css_respects_start_offset()
        {
            string text = "XXX{ XXX";
            var diags = DiagnosticChecker.CheckCss(text, 3, 4).ToList();
            Assert.NotEmpty(diags);
        }
    }
}
