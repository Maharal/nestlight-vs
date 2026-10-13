using System.Collections.Generic;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class QuickInfoEngineTests
    {
        [Fact]
        public void Returns_description_for_known_token()
        {
            var tokens = new List<Token> { new Token(5, 3, ClassificationNames.SqlKeyword) };
            QuickInfoResult? result = QuickInfoEngine.GetInfo(tokens, 6);

            Assert.True(result.HasValue);
            Assert.Equal("SQL keyword", result.Value.Description);
            Assert.Equal(5, result.Value.Start);
            Assert.Equal(3, result.Value.Length);
        }

        [Fact]
        public void Returns_null_for_position_outside_tokens()
        {
            var tokens = new List<Token> { new Token(5, 3, ClassificationNames.SqlKeyword) };
            Assert.Null(QuickInfoEngine.GetInfo(tokens, 0));
            Assert.Null(QuickInfoEngine.GetInfo(tokens, 10));
        }

        [Fact]
        public void Returns_null_for_token_with_no_description()
        {
            var tokens = new List<Token> { new Token(0, 5, "unknown.type") };
            Assert.Null(QuickInfoEngine.GetInfo(tokens, 2));
        }

        [Fact]
        public void Returns_null_for_empty_tokens()
        {
            Assert.Null(QuickInfoEngine.GetInfo(new List<Token>(), 0));
        }

        [Theory]
        [InlineData(ClassificationNames.Tag, "HTML tag")]
        [InlineData(ClassificationNames.CssProperty, "CSS property")]
        [InlineData(ClassificationNames.JsonKey, "JSON key")]
        [InlineData(ClassificationNames.GqlOperation, "GraphQL operation")]
        [InlineData(ClassificationNames.XmlTag, "XML element")]
        [InlineData(ClassificationNames.YamlKey, "YAML key")]
        [InlineData(ClassificationNames.MdHeading, "Markdown heading")]
        [InlineData(ClassificationNames.RegexGroup, "Regex group")]
        [InlineData(ClassificationNames.ShaderKeyword, "Shader keyword")]
        [InlineData(ClassificationNames.ShellKeyword, "Shell keyword")]
        [InlineData(ClassificationNames.TomlTable, "TOML table header")]
        public void Covers_all_language_families(string type, string expectedDescription)
        {
            var tokens = new List<Token> { new Token(0, 5, type) };
            QuickInfoResult? result = QuickInfoEngine.GetInfo(tokens, 2);

            Assert.True(result.HasValue);
            Assert.Equal(expectedDescription, result.Value.Description);
        }

        [Fact]
        public void End_to_end_with_real_pipeline()
        {
            string code = "const x = html`<div></div>`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int divPos = code.IndexOf("div");
            QuickInfoResult? result = QuickInfoEngine.GetInfo(tokens, divPos);

            Assert.True(result.HasValue);
            Assert.Equal("HTML tag", result.Value.Description);
        }

        [Theory]
        [InlineData("const q = sql`SELECT * FROM t`;", "SELECT", "Retrieves rows")]
        [InlineData("const q = sql`SELECT * FROM t`;", "FROM", "table, view or subquery")]
        [InlineData("const x = html`<ul><li>a</li></ul>`;", "ul", "unordered")]
        [InlineData("const x = html`<img src=\"a.png\">`;", "img", "image")]
        [InlineData("const x = css`.a { display: flex; }`;", "display", "kind of box")]
        public void Detail_is_specific_to_the_word(string code, string word, string expected)
        {
            IReadOnlyList<Token> tokens = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript).Highlight(code);
            QuickInfoResult? result = QuickInfoEngine.GetInfo(tokens, code.IndexOf(word), code);

            Assert.True(result.HasValue);
            Assert.Contains(expected, result.Value.Detail);
            Assert.StartsWith(word, result.Value.Title);
        }

        [Fact]
        public void Detail_falls_back_to_the_kind_of_token()
        {
            var tokens = new List<Token> { new Token(0, 5, ClassificationNames.Tag) };
            QuickInfoResult? result = QuickInfoEngine.GetInfo(tokens, 2, "zzzzz");

            Assert.Equal("Defines an HTML element and the kind of content it represents.", result.Value.Detail);
        }

        [Fact]
        public void Css_hex_color_is_parsed()
        {
            string code = "const x = css`.a { color: #ff0000; }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int colorPos = code.IndexOf("#ff0000");
            QuickInfoResult? result = QuickInfoEngine.GetInfo(tokens, colorPos, code);

            Assert.True(result.HasValue);
            Assert.True(result.Value.Color.HasValue);
            Assert.Equal(255, result.Value.Color.Value.R);
            Assert.Equal(0, result.Value.Color.Value.G);
            Assert.Equal(0, result.Value.Color.Value.B);
        }

        [Fact]
        public void Css_named_color_is_parsed()
        {
            string code = "const x = css`.a { color: red; }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int colorPos = code.IndexOf("red");
            QuickInfoResult? result = QuickInfoEngine.GetInfo(tokens, colorPos, code);

            Assert.True(result.HasValue);
            Assert.True(result.Value.Color.HasValue);
            Assert.Equal(255, result.Value.Color.Value.R);
        }

        [Fact]
        public void Non_color_css_value_has_no_color()
        {
            var tokens = new List<Token> { new Token(0, 5, ClassificationNames.CssProperty) };
            QuickInfoResult? result = QuickInfoEngine.GetInfo(tokens, 2, "color");

            Assert.True(result.HasValue);
            Assert.False(result.Value.Color.HasValue);
        }

        // Mutation L31: token.Start + token.Length < text.Length (should be <=)
        // Mutation L31: token.Start - token.Length (should be +)
        [Fact]
        public void Color_parsed_when_token_spans_exact_text_length()
        {
            // Token covers the entire text: Start=0, Length=7, text.Length=7
            // token.Start + token.Length == text.Length => boundary case for <=
            string text = "#ff0000";
            var tokens = new List<Token> { new Token(0, 7, ClassificationNames.CssNumber) };
            QuickInfoResult? result = QuickInfoEngine.GetInfo(tokens, 3, text);

            Assert.True(result.HasValue);
            Assert.True(result.Value.Color.HasValue);
            Assert.Equal(255, result.Value.Color.Value.R);
        }

        // Mutation L68: string "" instead of "CSS value"
        [Fact]
        public void CssValue_description_is_not_empty()
        {
            var result = QuickInfoEngine.DescriptionOf(ClassificationNames.CssValue);
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal("CSS value", result);
        }

        // Mutation L70: string "" instead of "CSS number"
        [Fact]
        public void CssNumber_description_is_not_empty()
        {
            var result = QuickInfoEngine.DescriptionOf(ClassificationNames.CssNumber);
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal("CSS number", result);
        }

        [Fact]
        public void Color_parser_handles_short_hex()
        {
            var c = CssColorParser.TryParse("#fff");
            Assert.True(c.HasValue);
            Assert.Equal(255, c.Value.R);
            Assert.Equal(255, c.Value.G);
            Assert.Equal(255, c.Value.B);
        }

        [Fact]
        public void Color_parser_handles_rgb()
        {
            var c = CssColorParser.TryParse("rgb(128, 0, 255)");
            Assert.True(c.HasValue);
            Assert.Equal(128, c.Value.R);
            Assert.Equal(0, c.Value.G);
            Assert.Equal(255, c.Value.B);
        }

        [Fact]
        public void Color_parser_handles_hsl()
        {
            var c = CssColorParser.TryParse("hsl(0, 100%, 50%)");
            Assert.True(c.HasValue);
            Assert.Equal(255, c.Value.R);
            Assert.Equal(0, c.Value.G);
            Assert.Equal(0, c.Value.B);
        }

        [Fact]
        public void Color_parser_returns_null_for_non_color()
        {
            Assert.Null(CssColorParser.TryParse("block"));
            Assert.Null(CssColorParser.TryParse("10px"));
            Assert.Null(CssColorParser.TryParse(null));
        }
    }
}
