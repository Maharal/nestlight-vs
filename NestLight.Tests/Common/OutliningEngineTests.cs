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

        private static string[] Hidden(string code)
        {
            return Regions(code).Select(r => code.Substring(r.Start, r.End - r.Start)).ToArray();
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
            string[] hidden = Hidden(code);
            Assert.Contains("\n  <li>a</li>\n  <li>\n    b\n  </li>\n", hidden); // the <ul>
            Assert.Contains("\n    b\n  ", hidden);                              // the second <li>
            Assert.Equal(3, hidden.Length);                                       // and the string; the one-line <li> is left alone
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
        public void A_css_rule_collapses_between_its_braces_and_braces_in_strings_and_comments_are_ignored()
        {
            const string code = "const q = /*css*/`\n.a {\n  content: \"}\";\n  /* } */\n  color: red;\n}\n`;";
            Assert.Contains("\n  content: \"}\";\n  /* } */\n  color: red;\n", Hidden(code));
        }

        [Fact]
        public void Json_objects_and_arrays_collapse()
        {
            const string code = "const q = /*json*/`{\n  \"a\": [\n    1,\n    2\n  ],\n  \"b\": 1\n}`;";
            Assert.Equal(3, Regions(code).Length); // the string, the object, the array
        }

        [Fact]
        public void A_sql_subquery_collapses_between_its_parentheses()
        {
            const string code = "const q = /*sql*/`select * from (\n  select 1\n  from t\n) x`;";
            Assert.Contains("\n  select 1\n  from t\n", Hidden(code));
        }

        [Fact]
        public void A_yaml_key_holds_the_lines_indented_below_it()
        {
            const string code = "const q = /*yaml*/`\na:\n  b: 1\n  c:\n    d: 2\nz: 3\n`;";
            string[] hidden = Hidden(code);
            Assert.Contains("\n  b: 1\n  c:\n    d: 2", hidden);
            Assert.Contains("\n    d: 2", hidden);
        }

        [Fact]
        public void A_markdown_heading_holds_its_section_up_to_the_next_heading_of_the_same_level()
        {
            const string code = "const q = /*md*/`\n# A\ntext\n## B\nmore\n\n# C\nend\n`;";
            string[] hidden = Hidden(code);
            Assert.Contains("\ntext\n## B\nmore", hidden);
            Assert.Contains("\nmore", hidden);
        }

        [Fact]
        public void A_toml_table_holds_its_keys()
        {
            const string code = "const q = /*toml*/`\n[a]\nx = 1\ny = 2\n[b]\nz = 3\n`;";
            Assert.Contains("\nx = 1\ny = 2", Hidden(code));
        }

        [Fact]
        public void A_windows_line_break_is_hidden_whole()
        {
            const string code = "const q = /*html*/`\r\n<div>\r\n</div>\r\n`;";
            Assert.Equal(code.IndexOf('\r'), Regions(code).First().Start);
        }
    }
}
