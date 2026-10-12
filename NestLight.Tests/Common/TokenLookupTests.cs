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
    }
}
