using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using Xunit;

namespace NestLight.Tests
{
    public class InterpolationHighlighterTests
    {
        [Fact]
        public void Keywords_are_highlighted()
        {
            string text = "this.items.length > 0 ? true : false";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);

            var keywords = tokens.Where(t => t.Type == ClassificationNames.ExprKeyword)
                .Select(t => text.Substring(t.Start, t.Length)).ToList();
            Assert.Contains("this", keywords);
            Assert.Contains("true", keywords);
            Assert.Contains("false", keywords);
        }

        [Fact]
        public void Numbers_are_highlighted()
        {
            string text = "count + 42";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);

            var numbers = tokens.Where(t => t.Type == ClassificationNames.ExprNumber).ToList();
            Assert.Single(numbers);
            Assert.Equal("42", text.Substring(numbers[0].Start, numbers[0].Length));
        }

        [Fact]
        public void Strings_are_highlighted()
        {
            string text = "\"hello\" + 'world'";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);

            var strings = tokens.Where(t => t.Type == ClassificationNames.ExprString).ToList();
            Assert.Equal(2, strings.Count);
        }

        [Fact]
        public void Parses_keywords_from_multiple_languages()
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
        public void Handles_strings_and_comments()
        {
            string text = "\"hello\" + /* comment */ 42 // line";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);

            Assert.Single(tokens.Where(t => t.Type == ClassificationNames.ExprString));
            Assert.Single(tokens.Where(t => t.Type == ClassificationNames.ExprNumber));
        }

        [Fact]
        public void Tokens_do_not_overlap()
        {
            string text = "const obj = {\"key\": true, count: 42}";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            tokens.Sort((a, b) => a.Start.CompareTo(b.Start));

            for (int i = 1; i < tokens.Count; i++)
                Assert.True(tokens[i].Start >= tokens[i - 1].End,
                    "overlapping at " + i + ": " + tokens[i - 1].End + " > " + tokens[i].Start);
        }

        [Fact]
        public void Respects_start_end_boundaries()
        {
            string text = "xxxconst y = 42zzz";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 3, 15, tokens);

            foreach (var t in tokens)
            {
                Assert.True(t.Start >= 3, "token starts before range");
                Assert.True(t.End <= 15, "token ends after range");
            }
        }

        [Fact]
        public void Escaped_strings_do_not_overflow()
        {
            string text = "\"ab\\\"";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);

            Assert.Single(tokens);
            Assert.True(tokens[0].End <= text.Length);
        }
    }
}
