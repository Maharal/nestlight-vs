using Xunit;

namespace NestLight.Tests
{
    /// <summary>JSON: keys, strings, numbers, literals, punctuation.</summary>
    public class JsonTokenizerTests
    {
        private static string[] J(string json) { return Lexer.Language("json", json); }

        [Fact]
        public void Object_with_every_value_kind()
        {
            Assert.Equal(new[]
            {
                "json.punctuation|{", "json.key|\"a\"", "json.punctuation|:", "json.string|\"s\"", "json.punctuation|,",
                "json.key|\"b\"", "json.punctuation|:", "json.number|1", "json.punctuation|,",
                "json.key|\"c\"", "json.punctuation|:", "json.literal|true", "json.punctuation|}"
            }, J("{\"a\":\"s\",\"b\":1,\"c\":true}"));
        }

        [Fact]
        public void A_string_is_a_key_only_when_a_colon_follows()
        {
            Assert.Equal(new[] { "json.string|\"x\"", "json.punctuation|," }, J("\"x\","));
            Assert.Equal(new[] { "json.key|\"x\"", "json.punctuation|:" }, J("\"x\" :"));
            Assert.Equal(new[] { "json.key|\"x\"", "json.punctuation|:" }, J("\"x\"\n  :"));
        }

        [Theory]
        [InlineData("0")]
        [InlineData("-1")]
        [InlineData("12.5")]
        [InlineData("-0.5")]
        [InlineData("1e10")]
        [InlineData("1E+10")]
        [InlineData("-2.5e-3")]
        public void Numbers(string number)
        {
            Assert.Equal(new[] { "json.number|" + number }, J(number));
        }

        [Theory]
        [InlineData("true")]
        [InlineData("false")]
        [InlineData("null")]
        public void Literals(string literal)
        {
            Assert.Equal(new[] { "json.literal|" + literal }, J(literal));
        }

        [Fact]
        public void Other_words_are_not_literals()
        {
            Assert.Empty(J("True nullable maybe"));
        }

        [Fact]
        public void Arrays_and_nesting()
        {
            Assert.Equal(new[]
            {
                "json.punctuation|[", "json.number|1", "json.punctuation|,", "json.punctuation|[",
                "json.literal|null", "json.punctuation|]", "json.punctuation|]"
            }, J("[1,[null]]"));
        }

        [Fact]
        public void Escaped_quote_does_not_end_a_string()
        {
            Assert.Equal(new[] { "json.string|\"a\\\"b\"" }, J("\"a\\\"b\""));
        }

        [Fact]
        public void Unterminated_string_stops_at_the_line_break()
        {
            Assert.Equal(new[] { "json.string|\"abc", "json.number|1" }, J("\"abc\n1"));
        }

        [Fact]
        public void A_lone_minus_is_not_a_number()
        {
            Assert.Empty(J("-"));
        }

        [Fact]
        public void Interpolations_work_as_keys_values_and_inside_strings()
        {
            Assert.Equal(new[]
            {
                "json.punctuation|{", "json.punctuation|:", "json.punctuation|,",
                "json.key|\"k\"", "json.punctuation|:", "json.string|\"v-", "json.string|\"", "json.punctuation|}"
            }, J("{${key}: ${value}, \"k\": \"v-${x}\"}"));
        }
    }
}
