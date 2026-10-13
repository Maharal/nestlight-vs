using NestLight.Common;
using Xunit;

namespace NestLight.Tests
{
    public class TokenLookupTests
    {
        private static readonly Token[] Tokens = new[]
        {
            new Token(0, 3, "a"),   // [0, 3)
            new Token(5, 2, "b"),   // [5, 7)
            new Token(10, 4, "c"),  // [10, 14)
            new Token(20, 1, "d"),  // [20, 21)
        };

        [Theory]
        [InlineData(0, "a")]
        [InlineData(2, "a")]
        [InlineData(5, "b")]
        [InlineData(6, "b")]
        [InlineData(12, "c")]
        [InlineData(20, "d")]
        public void AtPosition_finds_containing_token(int position, string expectedType)
        {
            Token result = TokenLookup.AtPosition(Tokens, position);
            Assert.NotNull(result);
            Assert.Equal(expectedType, result.Type);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(7)]
        [InlineData(15)]
        [InlineData(21)]
        [InlineData(100)]
        public void AtPosition_returns_null_for_gaps(int position)
        {
            Assert.Null(TokenLookup.AtPosition(Tokens, position));
        }

        [Fact]
        public void AtPosition_returns_null_for_empty_list()
        {
            Assert.Null(TokenLookup.AtPosition(new Token[0], 5));
        }

        [Fact]
        public void PrecedingOfType_finds_nearest_match()
        {
            Token result = TokenLookup.PrecedingOfType(Tokens, 10, "a", "b");
            Assert.NotNull(result);
            Assert.Equal("b", result.Type);
        }

        [Fact]
        public void PrecedingOfType_returns_null_when_no_match()
        {
            Token result = TokenLookup.PrecedingOfType(Tokens, 10, "z");
            Assert.Null(result);
        }

        [Fact]
        public void PrecedingOfType_returns_null_at_start()
        {
            Token result = TokenLookup.PrecedingOfType(Tokens, 0, "a");
            Assert.Null(result);
        }

        [Fact]
        public void FindDefinition_locates_matching_token()
        {
            string text = ".foo  .bar  .foo";
            var tokens = new[]
            {
                new Token(0, 4, "css.class"),
                new Token(6, 4, "css.class"),
                new Token(12, 4, "css.class"),
            };

            Token result = TokenLookup.FindDefinition(tokens, ".foo", tokens[0], "css.class", text);
            Assert.NotNull(result);
            Assert.Equal(12, result.Start);
        }

        [Fact]
        public void FindDefinition_returns_null_when_no_other_match()
        {
            string text = ".foo  .bar";
            var tokens = new[]
            {
                new Token(0, 4, "css.class"),
                new Token(6, 4, "css.class"),
            };

            Token result = TokenLookup.FindDefinition(tokens, ".foo", tokens[0], "css.class", text);
            Assert.Null(result);
        }

        // Mutation L17: tokens[lo].End >= position (should be > position)
        // At the boundary: position equals End exactly => should NOT match
        [Fact]
        public void AtPosition_boundary_end_equals_position_returns_null()
        {
            // Token [0,3) has End=3. Position=3 should NOT be found.
            Assert.Null(TokenLookup.AtPosition(Tokens, 3));
            // Token [5,7) has End=7. Position=7 should NOT be found.
            Assert.Null(TokenLookup.AtPosition(Tokens, 7));
        }

        // Mutation L29: tokens[mid].End > position or < position (should be <=)
        // Binary search in PrecedingOfType: if End <= position, go right
        [Fact]
        public void PrecedingOfType_at_exact_end_boundary()
        {
            // Position=7 is exactly the End of token "b" [5,7)
            // PrecedingOfType should find "b" as preceding (its End <= 7)
            Token result = TokenLookup.PrecedingOfType(Tokens, 7, "b");
            Assert.NotNull(result);
            Assert.Equal("b", result.Type);
        }

        // Mutation L32: lo + 1 => lo - 1 (k starts from wrong place)
        // Mutation L34: tokens[k].End >= position (should be > position)
        [Fact]
        public void PrecedingOfType_skips_token_overlapping_position()
        {
            // Position=6 is inside token "b" [5,7), so PrecedingOfType should skip "b"
            // and find "a" [0,3)
            Token result = TokenLookup.PrecedingOfType(Tokens, 6, "a", "b");
            Assert.NotNull(result);
            Assert.Equal("a", result.Type);
        }

        // Mutation L32: k >= 0 => k > 0 (off-by-one, would miss index 0)
        [Fact]
        public void PrecedingOfType_finds_token_at_index_zero()
        {
            // Position=5, look for type "a". Token "a" is at index 0 in the list.
            Token result = TokenLookup.PrecedingOfType(Tokens, 5, "a");
            Assert.NotNull(result);
            Assert.Equal("a", result.Type);
        }

        // Mutation L37: return null => removed (statement mutation)
        [Fact]
        public void PrecedingOfType_returns_null_when_preceding_is_wrong_type()
        {
            // Position=10 (token "c"), look for type "z" which doesn't exist
            Token result = TokenLookup.PrecedingOfType(Tokens, 10, "z");
            Assert.Null(result);
        }

        // Mutation L48: statement mutation on continue => removed
        [Fact]
        public void FindDefinition_skips_wrong_type()
        {
            string text = ".foo  .foo";
            var tokens = new[]
            {
                new Token(0, 4, "css.class"),
                new Token(6, 4, "other"),
            };
            // Looking for type "css.class": should not find token at index 6 which is type "other"
            Token result = TokenLookup.FindDefinition(tokens, ".foo", tokens[0], "css.class", text);
            Assert.Null(result);
        }

        // Mutation L50: t.Start - t.Length (should be + t.Length)
        [Fact]
        public void FindDefinition_checks_bounds_correctly()
        {
            // Token that would go past sourceText.Length if bounds check fails
            string text = ".foo";
            var tokens = new[]
            {
                new Token(0, 4, "css.class"),
                new Token(2, 10, "css.class"), // extends past text length
            };
            // Should not crash and should not match the out-of-bounds token
            Token result = TokenLookup.FindDefinition(tokens, ".foo", tokens[0], "css.class", text);
            Assert.Null(result);
        }
    }
}
