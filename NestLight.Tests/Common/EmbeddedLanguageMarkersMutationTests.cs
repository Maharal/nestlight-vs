using NestLight.Common;
using Xunit;

namespace NestLight.Tests
{
    public class EmbeddedLanguageMarkersMutationTests
    {

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

        // --- Mutation killing tests for EmbeddedLanguageMarkers.cs ---

        // Mutation L17: "" instead of "registry" (ArgumentNullException paramName)
        [Fact]
        public void AcceptedEmbeddedLanguages_null_registry_throws_ArgumentNullException()
        {
            var ex = Assert.Throws<System.ArgumentNullException>(() =>
                new AcceptedEmbeddedLanguages(null));
            Assert.Equal("registry", ex.ParamName);
        }

        // Mutation L30: s.End >= s.Start (should be > s.Start for non-empty string)
        [Fact]
        public void Resolve_empty_string_range_does_not_guess()
        {
            var registry = new StubRegistry("sql");
            var guesser = new StubGuesser("sql");
            var accepted = new AcceptedEmbeddedLanguages(registry, null, guesser);
            // s.End == s.Start means empty string, so guessing should be skipped
            var s = new EmbeddedString(null);
            s.Start = 5;
            s.End = 5; // empty
            Assert.False(accepted.Resolve("some text here", s));
        }

        // Mutation L34: s.Escapes.Count < 0 / >= 0 / negation
        [Fact]
        public void Resolve_string_with_escapes_decodes_before_guessing()
        {
            var registry = new StubRegistry("sql");
            var guesser = new StubGuesser("sql");
            var accepted = new AcceptedEmbeddedLanguages(registry, null, guesser);
            var s = new EmbeddedString(null);
            s.Start = 0;
            s.End = 10;
            s.Escapes.Add(new EscapeSequence(2, 2, 'X')); // has escapes
            Assert.True(accepted.Resolve("0123456789", s));
            Assert.Equal("sql", s.EmbeddedLanguageId);
        }

        [Fact]
        public void Resolve_string_without_escapes_guesses_directly()
        {
            var registry = new StubRegistry("sql");
            var guesser = new StubGuesser("sql");
            var accepted = new AcceptedEmbeddedLanguages(registry, null, guesser);
            var s = new EmbeddedString(null);
            s.Start = 0;
            s.End = 5;
            // No escapes
            Assert.True(accepted.Resolve("hello", s));
            Assert.Equal("sql", s.EmbeddedLanguageId);
        }

        // Mutation L50: end + start => end - start
        // Mutation L54-57: loop boundary mutations in DecodeForDetection
        [Fact]
        public void Resolve_with_escape_at_end_of_string()
        {
            var registry = new StubRegistry("sql");
            var guesser = new StubGuesser("sql");
            var accepted = new AcceptedEmbeddedLanguages(registry, null, guesser);
            var s = new EmbeddedString(null);
            s.Start = 0;
            s.End = 6;
            // Escape at position 4, length 2 (e.g. \n)
            s.Escapes.Add(new EscapeSequence(4, 2, '\n'));
            Assert.True(accepted.Resolve("abcd\\n", s));
        }

        // Mutation L55: p + escapes[esc].Length > end / < end (should be <=)
        [Fact]
        public void Resolve_with_escape_spanning_exact_end()
        {
            var registry = new StubRegistry("sql");
            var guesser = new StubGuesser("sql");
            var accepted = new AcceptedEmbeddedLanguages(registry, null, guesser);
            var s = new EmbeddedString(null);
            s.Start = 0;
            s.End = 4;
            // Escape at position 2, length 2 => ends exactly at 4 == end
            s.Escapes.Add(new EscapeSequence(2, 2, 'X'));
            Assert.True(accepted.Resolve("ab\\n", s));
        }

        // Stubs for testing
        private class StubRegistry : IEmbeddedLanguageRegistry
        {
            private readonly string _known;
            public StubRegistry(string known) { _known = known; }
            public bool IsKnown(string id) { return string.Equals(id, _known, System.StringComparison.OrdinalIgnoreCase); }
            public IEmbeddedLanguageTokenizer Find(string id) { return null; }
        }

        private class StubGuesser : ILanguageGuesser
        {
            private readonly string _result;
            public StubGuesser(string result) { _result = result; }
            public string Guess(string text, int start, int end) { return _result; }
        }
    }
}
