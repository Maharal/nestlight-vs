using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class InterpolationHighlighterTests
    {
        [Fact]
        public void Keywords_inside_interpolation_are_highlighted()
        {
            string code = "const x = html`<div>${this.items.length > 0 ? true : false}</div>`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            var exprKeywords = tokens.Where(t => t.Type == ClassificationNames.ExprKeyword).ToList();
            var keywordTexts = exprKeywords.Select(t => code.Substring(t.Start, t.Length)).ToList();
            Assert.Contains("this", keywordTexts);
            Assert.Contains("true", keywordTexts);
            Assert.Contains("false", keywordTexts);
        }

        [Fact]
        public void Numbers_inside_interpolation_are_highlighted()
        {
            string code = "const x = html`<div>${count + 42}</div>`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            var exprNumbers = tokens.Where(t => t.Type == ClassificationNames.ExprNumber).ToList();
            Assert.Single(exprNumbers);
            Assert.Equal("42", code.Substring(exprNumbers[0].Start, exprNumbers[0].Length));
        }

        [Fact]
        public void Strings_inside_interpolation_are_highlighted()
        {
            string code = "const x = html`<div>${\"hello\"}</div>`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            var exprStrings = tokens.Where(t => t.Type == ClassificationNames.ExprString).ToList();
            Assert.Single(exprStrings);
        }

        [Fact]
        public void Unit_highlighter_parses_keywords()
        {
            string text = "const x = new Array()";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);

            var keywords = tokens.Where(t => t.Type == ClassificationNames.ExprKeyword)
                .Select(t => text.Substring(t.Start, t.Length)).ToList();
            Assert.Contains("const", keywords);
            Assert.Contains("new", keywords);
        }

        [Fact]
        public void Unit_highlighter_handles_strings_and_comments()
        {
            string text = "\"hello\" + /* comment */ 42 // line";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);

            Assert.Single(tokens.Where(t => t.Type == ClassificationNames.ExprString));
            Assert.Single(tokens.Where(t => t.Type == ClassificationNames.ExprNumber));
        }
    }
}
