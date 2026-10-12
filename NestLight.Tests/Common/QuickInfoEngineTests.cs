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
    }
}
