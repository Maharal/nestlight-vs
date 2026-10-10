using System.Linq;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>Markdown: headings, emphasis, code, links, lists.</summary>
    public class MarkdownTokenizerTests
    {
        // Markdown is full of backticks, which cannot be written in a JavaScript template: the host here is Python.
        private static string[] M(string md)
        {
            return Lexer.Seq(HostLanguage.Python, "# markdown\nx = '''" + md + "'''");
        }

        // ---- headings -----------------------------------------------------------------------------------

        [Theory]
        [InlineData("# Title")]
        [InlineData("## Title")]
        [InlineData("###### Title")]
        [InlineData("   # Title")]
        [InlineData("#")]
        public void Headings_cover_the_whole_line(string line)
        {
            Assert.Equal(new[] { "markdown.heading|" + line.TrimStart() }, M(line));
        }

        [Theory]
        [InlineData("#Title")]
        [InlineData("####### seven")]
        [InlineData("    # indented code")]
        public void Not_headings(string line)
        {
            Assert.DoesNotContain("markdown.heading|", string.Join("", M(line)));
        }

        [Fact]
        public void Inline_markup_inside_a_heading_is_not_split()
        {
            Assert.Equal(new[] { "markdown.heading|# A *b* `c`" }, M("# A *b* `c`"));
        }

        // ---- emphasis ------------------------------------------------------------------------------------

        [Theory]
        [InlineData("*em*", "markdown.emphasis")]
        [InlineData("_em_", "markdown.emphasis")]
        [InlineData("**strong**", "markdown.strong")]
        [InlineData("__strong__", "markdown.strong")]
        [InlineData("***both***", "markdown.strong")]
        public void Emphasis_and_strong(string text, string type)
        {
            Assert.Equal(new[] { type + "|" + text }, M(text));
        }

        [Fact]
        public void Emphasis_inside_a_sentence()
        {
            Assert.Equal(new[] { "markdown.emphasis|*b*", "markdown.strong|**d**" }, M("a *b* c **d** e"));
        }

        [Theory]
        [InlineData("snake_case_word")]
        [InlineData("2 * 3 * 4")]
        [InlineData("a * b")]
        [InlineData("*unclosed")]
        [InlineData("**")]
        public void Things_that_look_like_emphasis_but_are_not(string text)
        {
            Assert.Empty(M(text));
        }

        // ---- paragraphs: inline markup crosses line breaks ----------------------------------------------

        [Fact]
        public void Emphasis_and_strong_cross_the_line_breaks_of_a_paragraph()
        {
            Assert.Equal(new[] { "markdown.emphasis|*a\nb*" }, M("*a\nb*"));
            Assert.Equal(new[] { "markdown.strong|**a\nb\nc**" }, M("**a\nb\nc**"));
        }

        [Fact]
        public void A_code_span_and_a_link_cross_the_line_breaks_of_a_paragraph()
        {
            Assert.Equal(new[] { "markdown.code|`a\nb`" }, M("`a\nb`"));
            Assert.Equal(new[] { "markdown.link|[text\nmore](http://x)" }, M("[text\nmore](http://x)"));
        }

        [Fact]
        public void A_blank_line_ends_the_paragraph()
        {
            Assert.Empty(M("*a\n\nb*"));
            Assert.Empty(M("`a\n\nb`"));
        }

        [Theory]
        [InlineData("*a\n# Title\nb*")]
        [InlineData("*a\n- item\nb*")]
        [InlineData("*a\n```\nb*")]
        public void A_block_that_starts_ends_the_paragraph(string md)
        {
            Assert.DoesNotContain("markdown.emphasis", string.Join("", M(md)));
        }

        [Fact]
        public void The_lines_after_a_list_marker_belong_to_the_item()
        {
            Assert.Equal(new[] { "markdown.list|-", "markdown.strong|**one\n  two**", "markdown.list|-", "markdown.code|`x`" }, M("- **one\n  two**\n- `x`"));
        }

        [Fact]
        public void An_opener_with_no_closer_does_not_take_the_rest_of_the_paragraph_with_it()
        {
            // the closer is farther than an inline rule looks, so it is text, and the rest of the paragraph is still read
            string md = "*" + new string('a', 2500) + "* then `code`";
            Assert.Equal(new[] { "markdown.code|`code`" }, M(md));
        }

        // ---- the indentation of the code around the string --------------------------------------------

        [Fact]
        public void Indentation_shared_by_every_line_is_not_markdown_indentation()
        {
            string md = "\n        # Title\n\n        Some *text*\n        - item\n        ```\n        x\n        ```\n    ";
            Assert.Equal(new[]
            {
                "markdown.heading|# Title", "markdown.emphasis|*text*", "markdown.list|-",
                "markdown.code|        ```", "markdown.code|        x", "markdown.code|        ```"
            }, M(md));
        }

        [Fact]
        public void Four_columns_past_the_shared_indentation_is_still_indented_code()
        {
            string md = "\n    # Title\n\n        # not a heading\n    - item";
            Assert.Equal(new[] { "markdown.heading|# Title", "markdown.list|-" }, M(md));
        }

        [Fact]
        public void The_first_line_does_not_count_when_the_indentation_is_worked_out()
        {
            // the first line starts at the quote, wherever the code around is indented
            Assert.Equal(new[] { "markdown.heading|# Title", "markdown.list|-" }, M("# Title\n    - item"));
        }

        [Fact]
        public void A_single_indented_line_is_indented_code_as_before()
        {
            Assert.DoesNotContain("markdown.heading", string.Join("", M("    # indented code")));
        }

        [Fact]
        public void Tabs_count_as_four_columns()
        {
            Assert.Equal(new[] { "markdown.heading|# Title", "markdown.list|-" }, M("\n\t# Title\n\t- item"));
            Assert.Equal(new[] { "markdown.heading|# Title" }, M("\n\t# Title\n\t\t# deeper"));
        }

        [Fact]
        public void Windows_line_breaks_with_indentation()
        {
            Assert.Equal(new[] { "markdown.heading|# T", "markdown.list|-", "markdown.strong|**a**" }, M("\r\n    # T\r\n    - **a**\r\n    "));
        }

        // ---- math ----------------------------------------------------------------------------------------------

        [Theory]
        [InlineData("$x^2$")]
        [InlineData("$a_b + c_d$")]
        [InlineData("$$x^2$$")]
        [InlineData("$$ x = \\frac{a}{b} $$")]
        public void Inline_math(string math)
        {
            Assert.Equal(new[] { "markdown.math|" + math }, M("text " + math + " more"));
        }

        [Fact]
        public void Math_hides_the_markup_inside_it()
        {
            Assert.Equal(new[] { "markdown.math|$a*b*c$" }, M("$a*b*c$"));
            Assert.Equal(new[] { "markdown.math|$a_b_c$" }, M("$a_b_c$"));
        }

        [Fact]
        public void A_dollar_sign_is_not_math_when_it_cannot_be()
        {
            Assert.Empty(M("it costs $5 and $6 in all"));
            Assert.Empty(M("a $ b $ c"));
            Assert.Empty(M("$ not math$"));
            Assert.Empty(M("unclosed $x"));
            Assert.Empty(M("$$$"));
            Assert.Empty(M("\\$x\\$"));
        }

        [Fact]
        public void A_dollar_sign_inside_a_code_span_is_code()
        {
            Assert.Equal(new[] { "markdown.code|`$x$`" }, M("`$x$`"));
        }

        [Fact]
        public void Display_math_on_lines_of_its_own()
        {
            Assert.Equal(new[] { "markdown.math|$$", "markdown.math|E = mc^2", "markdown.math|$$", "markdown.emphasis|*after*" },
                M("$$\nE = mc^2\n$$\n*after*"));
        }

        [Fact]
        public void Display_math_can_close_at_the_end_of_a_line_of_content()
        {
            Assert.Equal(new[] { "markdown.math|$$", "markdown.math|a + b $$", "markdown.strong|**c**" }, M("$$\na + b $$\n**c**"));
        }

        [Fact]
        public void An_unclosed_math_block_runs_to_the_end_like_a_fence()
        {
            Assert.Equal(new[] { "markdown.math|$$", "markdown.math|*not em*" }, M("$$\n*not em*"));
        }

        [Fact]
        public void Math_blocks_do_not_start_inside_a_fence()
        {
            Assert.Equal(new[] { "markdown.code|```", "markdown.code|$$", "markdown.code|```" }, M("```\n$$\n```"));
        }

        [Fact]
        public void Display_math_spans_the_lines_of_a_paragraph()
        {
            Assert.Equal(new[] { "markdown.math|$$x\ny$$" }, M("see $$x\ny$$"));
        }

        // ---- inside a JavaScript template: the backtick is written \` ------------------------------------

        private static string[] J(string md)
        {
            return Lexer.Seq(HostLanguage.JavaScript, "const a = md`" + md + "`;");
        }

        [Fact]
        public void Code_spans_with_escaped_backticks_in_a_javascript_template()
        {
            Assert.Equal(new[] { "markdown.code|\\`code\\`" }, J("Use \\`code\\` here"));
        }

        [Fact]
        public void Fenced_blocks_with_escaped_backticks_in_a_javascript_template()
        {
            Assert.Equal(new[] { "markdown.code|\\`\\`\\`js", "markdown.code|let a = *1*;", "markdown.code|\\`\\`\\`", "markdown.emphasis|*after*" },
                J("\\`\\`\\`js\nlet a = *1*;\n\\`\\`\\`\n*after*"));
        }

        [Fact]
        public void A_double_backtick_span_in_a_javascript_template()
        {
            Assert.Equal(new[] { "markdown.code|\\`\\`co\\`de\\`\\`" }, J("\\`\\`co\\`de\\`\\`"));
        }

        [Fact]
        public void An_indented_document_in_a_javascript_template()
        {
            string md = "\n    # Title\n\n    Some *emphasis* that\n    spans **two\n    lines**.\n\n    - item with \\`code\\`\n    ";
            Assert.Equal(new[]
            {
                "markdown.heading|# Title", "markdown.emphasis|*emphasis*", "markdown.strong|**two\n    lines**", "markdown.list|-", "markdown.code|\\`code\\`"
            }, J(md));
        }

        [Fact]
        public void Math_in_a_javascript_template()
        {
            Assert.Equal(new[] { "markdown.math|$x^2$", "markdown.math|$$", "markdown.math|\\sum_i x_i", "markdown.math|$$" }, J("Energy $x^2$ and:\n\n$$\n\\sum_i x_i\n$$"));
        }

        [Fact]
        public void An_interpolation_inside_the_markdown_is_plain_text()
        {
            var seq = Lexer.Seq(HostLanguage.JavaScript, "const a = md`# T ${t}\nthe *b* and \\`${c}\\``;").Where(t => !t.StartsWith("expression")).ToArray();
            Assert.Contains("markdown.emphasis|*b*", seq);
            Assert.Contains("markdown.heading|# T ", seq);
        }

        [Fact]
        public void Escaped_markers_are_text()
        {
            Assert.Empty(M("\\*a\\*"));
        }

        // ---- code ------------------------------------------------------------------------------------------

        [Theory]
        [InlineData("`code`")]
        [InlineData("``co`de``")]
        [InlineData("`*not em*`")]
        public void Code_spans(string span)
        {
            Assert.Equal(new[] { "markdown.code|" + span }, M(span));
        }

        [Fact]
        public void Unclosed_code_span_is_text()
        {
            Assert.Empty(M("`open"));
        }

        [Fact]
        public void Code_span_inside_emphasis_keeps_the_emphasis_whole()
        {
            Assert.Equal(new[] { "markdown.emphasis|*a `*` b*" }, M("*a `*` b*"));
        }

        [Theory]
        [InlineData("```", "```")]
        [InlineData("~~~", "~~~")]
        [InlineData("````", "````")]
        public void Fenced_blocks_are_code_from_fence_to_fence(string open, string close)
        {
            string md = open + "js\nlet a = *b*;\n" + close + "\nafter *em*";
            Assert.Equal(new[]
            {
                "markdown.code|" + open + "js", "markdown.code|let a = *b*;", "markdown.code|" + close, "markdown.emphasis|*em*"
            }, M(md));
        }

        [Fact]
        public void A_shorter_or_different_fence_does_not_close_the_block()
        {
            Assert.Equal(new[] { "markdown.code|````", "markdown.code|```", "markdown.code|~~~~", "markdown.code|````" },
                M("````\n```\n~~~~\n````"));
        }

        [Fact]
        public void Unclosed_fence_runs_to_the_end()
        {
            Assert.Equal(new[] { "markdown.code|```", "markdown.code|# not a heading" }, M("```\n# not a heading"));
        }

        // ---- links -----------------------------------------------------------------------------------------

        [Theory]
        [InlineData("[text](http://x.y)")]
        [InlineData("[text](http://x.y/a_(b))")]
        [InlineData("[a [b] c](u)")]
        [InlineData("![alt](img.png)")]
        [InlineData("<https://x.y/z>")]
        [InlineData("<mailto:a@b.c>")]
        public void Links_images_and_autolinks(string link)
        {
            Assert.Equal(new[] { "markdown.link|" + link }, M(link));
        }

        [Theory]
        [InlineData("[text]")]
        [InlineData("[text] (u)")]
        [InlineData("[text](unclosed")]
        [InlineData("<b>")]
        [InlineData("a < b > c")]
        public void Not_links(string text)
        {
            Assert.DoesNotContain("markdown.link", string.Join("", M(text)));
        }

        [Fact]
        public void A_link_hides_the_emphasis_in_its_text()
        {
            Assert.Equal(new[] { "markdown.link|[*a*](u)" }, M("[*a*](u)"));
        }

        // ---- lists -----------------------------------------------------------------------------------------

        [Theory]
        [InlineData("- item", "-")]
        [InlineData("* item", "*")]
        [InlineData("+ item", "+")]
        [InlineData("1. item", "1.")]
        [InlineData("12) item", "12)")]
        [InlineData("  - nested", "-")]
        public void List_markers(string line, string marker)
        {
            Assert.Equal(new[] { "markdown.list|" + marker }, M(line));
        }

        [Theory]
        [InlineData("-item")]
        [InlineData("1.item")]
        [InlineData("- ")]
        [InlineData("1234567890. too long")]
        [InlineData("a. letter")]
        public void Not_list_items(string line)
        {
            var seq = M(line);
            if (line == "- ") Assert.Equal(new[] { "markdown.list|-" }, seq);
            else Assert.DoesNotContain("markdown.list", string.Join("", seq));
        }

        [Fact]
        public void Inline_markup_continues_after_the_marker()
        {
            Assert.Equal(new[] { "markdown.list|-", "markdown.strong|**a**", "markdown.code|`b`" }, M("- **a** `b`"));
        }

        [Fact]
        public void Bold_at_the_start_of_a_line_is_not_a_list()
        {
            Assert.Equal(new[] { "markdown.strong|**a**" }, M("**a**"));
        }

        // ---- the whole thing ------------------------------------------------------------------------------

        [Fact]
        public void A_small_document()
        {
            string md = "# T\n\nSome *text* with `code`.\n\n- one\n- [two](u)\n\n```\nx\n```\n";
            Assert.Equal(new[]
            {
                "markdown.heading|# T", "markdown.emphasis|*text*", "markdown.code|`code`",
                "markdown.list|-", "markdown.list|-", "markdown.link|[two](u)",
                "markdown.code|```", "markdown.code|x", "markdown.code|```"
            }, M(md));
        }

        [Fact]
        public void Windows_line_breaks()
        {
            Assert.Equal(new[] { "markdown.heading|# T", "markdown.list|-" }, M("# T\r\n- a\r\n"));
        }

        [Fact]
        public void Interpolations_are_plain_text()
        {
            var seq = Lexer.Seq(HostLanguage.Python, "# markdown\nx = f'''# T {t}\n*a{b}*'''")
                .Where(t => !t.StartsWith("expression")).ToArray();
            Assert.Equal(new[] { "markdown.heading|# T ", "markdown.emphasis|*a", "markdown.emphasis|*" }, seq);
        }

        [Fact]
        public void Thousands_of_openers_with_no_closer_in_one_paragraph_stay_fast()
        {
            // every opener looks ahead a bounded distance, so the cost grows with the length of the paragraph and not with its square
            string md = string.Concat(Enumerable.Repeat("*a $b [c `d _e ", 8000));
            var watch = System.Diagnostics.Stopwatch.StartNew();
            M(md);
            watch.Stop();
            Assert.True(watch.ElapsedMilliseconds < 4000, "took " + watch.ElapsedMilliseconds + " ms");
        }
    }
}
