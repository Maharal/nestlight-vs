using NestLight.Common;
using Xunit;

namespace NestLight.Tests
{
    public class EmbeddedLanguageMarkersMutationTests
    {
        // MarkerComment.Parse(text, from, to): L17 string mutation on StringComparison, L30 equality mutation on key.Length comparison

        [Fact]
        public void Parse_range_language_equals_html_is_case_insensitive()
        {
            Assert.Equal("html", MarkerComment.Parse("LANGUAGE=HTML", 0, "LANGUAGE=HTML".Length));
            Assert.Equal("html", MarkerComment.Parse("Language=Html", 0, "Language=Html".Length));
        }

        [Fact]
        public void Parse_range_lang_equals_sql_is_case_insensitive()
        {
            Assert.Equal("sql", MarkerComment.Parse("LANG=SQL", 0, "LANG=SQL".Length));
        }

        [Fact]
        public void Parse_range_compares_key_length_correctly()
        {
            Assert.Equal("html", MarkerComment.Parse("language=html", 0, "language=html".Length));
            Assert.Equal("sql", MarkerComment.Parse("lang=sql", 0, "lang=sql".Length));
        }

        [Fact]
        public void Parse_range_body_shorter_than_key_is_treated_as_id()
        {
            Assert.Equal("lan", MarkerComment.Parse("lan", 0, 3));
            Assert.Equal("lan", MarkerComment.Parse("lan"));
        }

        // MarkerComment.IsId: ensure special characters are accepted/rejected

        [Fact]
        public void Parse_accepts_ids_with_special_chars()
        {
            Assert.Equal("c++", MarkerComment.Parse("c++"));
            Assert.Equal("c-sharp", MarkerComment.Parse("c-sharp"));
            Assert.Equal("c_plus", MarkerComment.Parse("c_plus"));
            Assert.Equal("c#", MarkerComment.Parse("c#"));
        }

        [Fact]
        public void Parse_rejects_ids_with_spaces()
        {
            Assert.Null(MarkerComment.Parse("c plus"));
        }

        [Fact]
        public void Parse_rejects_ids_with_slash()
        {
            Assert.Null(MarkerComment.Parse("a/b"));
        }

        [Fact]
        public void Parse_rejects_empty_id()
        {
            Assert.Null(MarkerComment.Parse(""));
            Assert.Null(MarkerComment.Parse("   "));
        }

        // Escapes.TryDecode: mutations at L159-L164 (boolean mutations flipping return true to false)

        [Theory]
        [InlineData('"', '"')]
        [InlineData('\'', '\'')]
        [InlineData('\\', '\\')]
        [InlineData('n', '\n')]
        [InlineData('r', '\r')]
        [InlineData('t', '\t')]
        public void Known_escapes_return_true_and_their_value(char next, char expected)
        {
            char value;
            Assert.True(Escapes.TryDecode(next, out value));
            Assert.Equal(expected, value);
        }

        [Theory]
        [InlineData('a')]
        [InlineData('0')]
        [InlineData('x')]
        [InlineData('u')]
        public void Unknown_escapes_return_false(char next)
        {
            char value;
            Assert.False(Escapes.TryDecode(next, out value));
            Assert.Equal('\0', value);
        }
    }
}
