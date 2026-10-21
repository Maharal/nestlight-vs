using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class OutliningEngineTests
    {
        private static OutlineRegion[] Regions(string code)
        {
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            return OutliningEngine.Regions(highlighter.Scanner.Scan(code), code).ToArray();
        }

        [Fact]
        public void A_marked_string_of_several_lines_can_be_collapsed_after_its_first_line()
        {
            const string code = "const q = /*html*/`\n  <div>\n    <p>x</p>\n  </div>\n`;";
            OutlineRegion r = Regions(code).First();
            Assert.Equal(code.IndexOf('\n'), r.Start);
            Assert.Equal(code.LastIndexOf('`'), r.End);
        }

        [Fact]
        public void A_string_of_one_line_is_left_alone()
        {
            Assert.Empty(Regions("const q = /*html*/`<div>x</div>`;"));
        }

        [Fact]
        public void A_string_with_no_language_is_left_alone()
        {
            Assert.Empty(Regions("const q = `a\nb\nc\nd`;"));
        }

        [Fact]
        public void Each_element_of_html_that_takes_several_lines_can_be_collapsed_between_its_tags()
        {
            const string code = "const q = /*html*/`<ul>\n  <li>a</li>\n  <li>\n    b\n  </li>\n</ul>`;";
            OutlineRegion[] regions = Regions(code);
            string[] hidden = regions.Select(r => code.Substring(r.Start, r.End - r.Start)).ToArray();
            Assert.Contains("\n  <li>a</li>\n  <li>\n    b\n  </li>\n", hidden); // the <ul>
            Assert.Contains("\n    b\n  ", hidden);                              // the second <li>
            Assert.DoesNotContain("a", hidden);                                   // the one-line <li> is left alone
            Assert.Equal(3, regions.Length);                                      // the string, the <ul>, the <li>
        }

        [Fact]
        public void Void_elements_and_comments_do_not_open_an_element()
        {
            const string code = "const q = /*html*/`<div>\n  <br>\n  <!-- <p> -->\n  <img src=\"a>b\">\n</div>`;";
            Assert.Equal(2, Regions(code).Length); // the string and the <div>
        }

        [Fact]
        public void A_style_element_collapses_whole()
        {
            const string code = "const q = /*html*/`<style>\n  a { b: c }\n  d { e: f }\n</style>`;";
            Assert.Equal(2, Regions(code).Length);
        }

        [Fact]
        public void A_windows_line_break_is_hidden_whole()
        {
            const string code = "const q = /*html*/`\r\n<div>\r\n</div>\r\n`;";
            Assert.Equal(code.IndexOf('\r'), Regions(code).First().Start);
        }
    }
}
