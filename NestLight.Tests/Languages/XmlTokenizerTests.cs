using System.Linq;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>XML: tags, attributes, comments, CDATA, processing instructions.</summary>
    public class XmlTokenizerTests
    {
        private static string[] X(string xml) { return Lexer.Language("xml", xml); }

        [Fact]
        public void Element_with_attributes_and_text()
        {
            Assert.Equal(new[]
            {
                "xml.delimiter|<", "xml.tag|a", "xml.attribute|b", "xml.attribute.value|\"c\"", "xml.attribute|d",
                "xml.attribute.value|'e'", "xml.delimiter|>", "xml.delimiter|</", "xml.tag|a", "xml.delimiter|>"
            }, X("<a b=\"c\" d='e'>text</a>"));
        }

        [Fact]
        public void Self_closing_tag()
        {
            Assert.Equal(new[] { "xml.delimiter|<", "xml.tag|br", "xml.delimiter|/>" }, X("<br/>"));
            Assert.Equal(new[] { "xml.delimiter|<", "xml.tag|br", "xml.delimiter|/>" }, X("<br />"));
        }

        [Theory]
        [InlineData("ns:item")]
        [InlineData("my-element")]
        [InlineData("a.b")]
        [InlineData("_x")]
        [InlineData("Item1")]
        public void Names_may_have_prefixes_dots_and_hyphens(string name)
        {
            Assert.Equal(new[] { "xml.delimiter|<", "xml.tag|" + name, "xml.delimiter|/>" }, X("<" + name + "/>"));
        }

        [Fact]
        public void Tags_keep_their_case()
        {
            Assert.Equal(new[] { "xml.delimiter|<", "xml.tag|Item", "xml.delimiter|/>" }, X("<Item/>"));
        }

        [Fact]
        public void Namespace_attributes()
        {
            Assert.Equal(new[] { "xml.attribute|xmlns:x", "xml.attribute.value|\"u\"" }, X("<a xmlns:x=\"u\"/>").Skip(2).Take(2).ToArray());
        }

        [Fact]
        public void Attribute_value_may_span_lines()
        {
            Assert.Contains("xml.attribute.value|\"a\nb\"", X("<a b=\"a\nb\"/>"));
        }

        [Fact]
        public void Unquoted_value_is_tolerated()
        {
            Assert.Contains("xml.attribute.value|5", X("<a b=5>"));
        }

        [Fact]
        public void Comments()
        {
            Assert.Equal(new[] { "xml.comment|<!-- a <b> -->", "xml.delimiter|<", "xml.tag|c", "xml.delimiter|/>" }, X("<!-- a <b> --><c/>"));
            Assert.Equal(new[] { "xml.comment|<!-- open" }, X("<!-- open"));
        }

        [Fact]
        public void Cdata_sections_hide_markup()
        {
            Assert.Equal(new[]
            {
                "xml.delimiter|<", "xml.tag|a", "xml.delimiter|>", "xml.cdata|<![CDATA[ <b>x</b> ]]>",
                "xml.delimiter|</", "xml.tag|a", "xml.delimiter|>"
            }, X("<a><![CDATA[ <b>x</b> ]]></a>"));
            Assert.Equal(new[] { "xml.cdata|<![CDATA[ open" }, X("<![CDATA[ open"));
        }

        [Fact]
        public void Processing_instruction()
        {
            Assert.Equal(new[]
            {
                "xml.delimiter|<?", "xml.tag|xml", "xml.attribute|version", "xml.attribute.value|\"1.0\"", "xml.delimiter|?>"
            }, X("<?xml version=\"1.0\"?>"));
        }

        [Fact]
        public void Doctype()
        {
            var seq = X("<!DOCTYPE note>");
            Assert.Equal("xml.delimiter|<!", seq[0]);
            Assert.Equal("xml.tag|DOCTYPE", seq[1]);
        }

        [Fact]
        public void Style_blocks_are_not_css_in_xml()
        {
            Assert.DoesNotContain(X("<style>a { b: c }</style>"), t => t.StartsWith("css."));
        }

        [Theory]
        [InlineData("a < b")]
        [InlineData("1 <2")]
        [InlineData("<")]
        [InlineData("<>")]
        public void A_lone_less_than_is_text(string xml)
        {
            Assert.Empty(X(xml));
        }

        [Fact]
        public void Malformed_tag_does_not_swallow_the_next_one()
        {
            Assert.Equal(new[] { "b", "b" }, X("<a <b></b>").Where(t => t.StartsWith("xml.tag|") && t != "xml.tag|a").Select(t => t.Substring(8)).ToArray());
        }

        [Fact]
        public void Interpolations_work_in_names_attributes_and_values()
        {
            var seq = X("<${tag} ${attrs} a=\"x${v}y\">${text}</${tag}>");
            Assert.Contains("xml.attribute|a", seq);
            Assert.Contains("xml.attribute.value|\"x", seq);
            Assert.Contains("xml.attribute.value|y\"", seq);
        }

        [Fact]
        public void Unterminated_attribute_value_runs_to_the_end()
        {
            Assert.Equal(new[] { "xml.delimiter|<", "xml.tag|a", "xml.attribute|b", "xml.attribute.value|\"x>" }, X("<a b=\"x>"));
        }
    }
}
