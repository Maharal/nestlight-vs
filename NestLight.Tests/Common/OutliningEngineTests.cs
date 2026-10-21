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
            OutlineRegion r = Regions(code).Single();
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
        public void A_windows_line_break_is_hidden_whole()
        {
            const string code = "const q = /*html*/`\r\n<div>\r\n</div>\r\n`;";
            Assert.Equal(code.IndexOf('\r'), Regions(code).Single().Start);
        }
    }
}
