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

        [Fact]
        public void Emphasis_does_not_cross_lines()
        {
            Assert.Empty(M("*a\nb*"));
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
    }
}
