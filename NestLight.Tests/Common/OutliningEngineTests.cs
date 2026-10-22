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
        public void A_marked_string_of_several_lines_can_be_collapsed_between_its_quotes()
        {
            const string code = "const q = /*html*/`\n  <div>\n    <p>x</p>\n  </div>\n`;";
            OutlineRegion r = Regions(code).First();
            Assert.Equal(code.IndexOf('`') + 1, r.Start);
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
        public void Regions_never_share_a_start_and_never_cross()
        {
            string[] codes =
            {
                "const q = /*html*/`<ul>\n  <li>a</li>\n  <li>\n    b\n  </li>\n</ul>`;",
                "const q = /*md*/`# A\ntext\n## B\nmore`;",
                "const q = /*json*/`{\n\"a\": [\n1\n]\n}`;",
                "const q = /*yaml*/`a:\n  b:\n    c: 1\n`;",
                "const q = /*css*/`a{\nb{\n}\n}`;",
                "const q = /*html*/`\r\n<div>\r\n<p>\r\nx\r\n</p>\r\n</div>\r\n`;"
            };
            foreach (string code in codes)
            {
                OutlineRegion[] r = Regions(code);
                Assert.NotEmpty(r);
                for (int i = 0; i < r.Length; i++)
                    for (int j = i + 1; j < r.Length; j++)
                    {
                        bool disjoint = r[i].End <= r[j].Start;
                        bool inside = r[j].Start > r[i].Start && r[j].End <= r[i].End;
                        Assert.True(disjoint || inside, code);
                    }
            }
        }
    }
}
