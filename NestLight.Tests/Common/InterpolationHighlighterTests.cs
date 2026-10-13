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

        // --- Mutation killing tests ---

        // Mutations L34: c == '/' && i + 1 < end (comment detection)
        // Tests: || vs &&, > vs <, i-1 vs i+1
        [Fact]
        public void Slash_at_end_of_range_does_not_crash()
        {
            // A single "/" at the end: i+1 < end is false so no comment
            string text = "42 /";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            Assert.Single(tokens); // just the number 42
        }

        // Mutation L36: i - 1 vs i + 1 (skip line comment via //)
        [Fact]
        public void Line_comment_skips_to_end_of_line()
        {
            // Everything after // to newline should be skipped
            string text = "42 // comment\n99";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            var numbers = tokens.Where(t => t.Type == ClassificationNames.ExprNumber)
                .Select(t => text.Substring(t.Start, t.Length)).ToList();
            Assert.Contains("42", numbers);
            Assert.Contains("99", numbers);
        }

        // Mutations L39: remaining boundary checks, i-2 vs i+2
        [Fact]
        public void Line_comment_with_no_newline_goes_to_end()
        {
            string text = "42 // comment no newline";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            Assert.Single(tokens);
            Assert.Equal("42", text.Substring(tokens[0].Start, tokens[0].Length));
        }

        // Mutations L46-48: block comment close detection, i-2 vs i+2, close-2 vs close+2
        [Fact]
        public void Block_comment_is_skipped_and_continues_after()
        {
            string text = "42 /* block */ 99";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            var numbers = tokens.Where(t => t.Type == ClassificationNames.ExprNumber)
                .Select(t => text.Substring(t.Start, t.Length)).ToList();
            Assert.Equal(2, numbers.Count);
            Assert.Contains("42", numbers);
            Assert.Contains("99", numbers);
        }

        // Mutation L46: "*/" string mutation to ""
        [Fact]
        public void Block_comment_without_close_goes_to_end()
        {
            string text = "42 /* unclosed block comment";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            Assert.Single(tokens);
        }

        // Mutation L57: Math.Min replaced by Math.Max (escape handling)
        [Fact]
        public void String_with_escape_at_end_does_not_overflow()
        {
            // Escape at very end of range: strEnd+2 should be clamped with Min(strEnd+2, end)
            string text = "'a\\'";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            Assert.Single(tokens);
            Assert.True(tokens[0].End <= text.Length);
        }

        // Mutation L61: strEnd > end => strEnd >= end or removed
        [Fact]
        public void String_end_boundary_is_clamped()
        {
            // Unclosed string: should still produce a token within bounds
            string text = "\"unterminated";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            Assert.Single(tokens);
            Assert.True(tokens[0].End <= text.Length);
        }

        // Mutations L67: c == '.' && i + 1 < end, number starting with dot
        [Fact]
        public void Number_starting_with_dot_is_highlighted()
        {
            string text = ".5 + 1";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            var numbers = tokens.Where(t => t.Type == ClassificationNames.ExprNumber).ToList();
            Assert.Equal(2, numbers.Count);
            Assert.Equal(".5", text.Substring(numbers[0].Start, numbers[0].Length));
        }

        // Mutation L67: c != '.' (negate), i-1 vs i+1
        [Fact]
        public void Dot_not_followed_by_digit_is_not_number()
        {
            string text = ". x";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            Assert.Empty(tokens.Where(t => t.Type == ClassificationNames.ExprNumber));
        }

        // Mutation L67: dot at very end (i+1 < end is false)
        [Fact]
        public void Dot_at_end_is_not_number()
        {
            string text = "x.";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            Assert.Empty(tokens.Where(t => t.Type == ClassificationNames.ExprNumber));
        }

        // Mutation L73: statement mutation (i = wordEnd removed)
        [Fact]
        public void Non_keyword_identifier_advances_past_word()
        {
            // "myVar" is not a keyword; the parser should still advance past it
            string text = "myVar + 1";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            Assert.Single(tokens); // only the number 1
        }

        // Mutation L76: c != '_', c != '$'
        [Fact]
        public void Underscore_and_dollar_start_identifiers()
        {
            string text = "_val + $var";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            // Neither _val nor $var is a keyword, so no keyword tokens
            Assert.Empty(tokens.Where(t => t.Type == ClassificationNames.ExprKeyword));
        }

        // Mutation L84: statement mutation (i = wordEnd removed would cause infinite loop)
        [Fact]
        public void Keyword_followed_by_non_keyword_both_handled()
        {
            string text = "const myVar";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            Assert.Single(tokens);
            Assert.Equal(ClassificationNames.ExprKeyword, tokens[0].Type);
            Assert.Equal("const", text.Substring(tokens[0].Start, tokens[0].Length));
        }

        // Mutation L40: nl < 0 => nl <= 0 (line comment with immediate newline)
        [Fact]
        public void Line_comment_immediately_followed_by_newline()
        {
            string text = "//\n42";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            Assert.Single(tokens);
            Assert.Equal("42", text.Substring(tokens[0].Start, tokens[0].Length));
        }

        // Mutation L47: close < 0 => close <= 0 (block comment close at position 0-ish)
        [Fact]
        public void Block_comment_close_at_minimum_distance()
        {
            // "/**/" is the shortest block comment
            string text = "/**/42";
            var tokens = new List<Token>();
            InterpolationHighlighter.Highlight(text, 0, text.Length, tokens);
            Assert.Single(tokens);
            Assert.Equal("42", text.Substring(tokens[0].Start, tokens[0].Length));
        }
    }
}
