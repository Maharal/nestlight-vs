using NestLight.Common;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>How a comment or a tag marks a string: <c>id</c>, <c>language=id</c> and <c>lang=id</c>.</summary>
    public class MarkerCommentTests
    {
        [Theory]
        [InlineData("html", "html")]
        [InlineData(" html ", "html")]
        [InlineData("HTML", "html")]
        [InlineData("language=html", "html")]
        [InlineData(" language=HTML ", "html")]
        [InlineData("lang=sql", "sql")]
        [InlineData("language = css", "css")]
        [InlineData("Lang=Json", "json")]
        public void Marker_comments_yield_a_lower_case_id(string body, string id)
        {
            Assert.Equal(id, MarkerComment.Parse(body));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("html is great")]
        [InlineData("TODO: html")]
        [InlineData("language=")]
        [InlineData("language=html css")]
        [InlineData("a/b")]
        public void Comments_that_are_not_markers_yield_null(string body)
        {
            Assert.Null(MarkerComment.Parse(body));
        }

        [Theory]
        [InlineData("html")]
        [InlineData(" html ")]
        [InlineData("HTML")]
        [InlineData("language=html")]
        [InlineData(" language=HTML ")]
        [InlineData("lang=sql")]
        [InlineData("language = css")]
        [InlineData("Lang=Json")]
        [InlineData("language")]
        [InlineData("lang")]
        [InlineData("languag")]
        [InlineData("language html")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("html is great")]
        [InlineData("TODO: html")]
        [InlineData("language=")]
        [InlineData("language=html css")]
        [InlineData("a/b")]
        public void Reading_a_range_of_the_text_gives_the_same_result_as_reading_the_comment_alone(string body)
        {
            string text = "x // " + body + " tail";
            Assert.Equal(MarkerComment.Parse(body), MarkerComment.Parse(text, 5, 5 + body.Length));
        }

        [Fact]
        public void Language_alone_is_an_id_not_a_key()
        {
            // "language" without "=" is just a word; unknown languages are filtered later by the host
            Assert.Equal("language", MarkerComment.Parse("language"));
        }

        [Theory]
        [InlineData("html`x`", 4, "html")]
        [InlineData("ui.html`x`", 7, "html")]
        [InlineData("HTML`x`", 4, "html")]
        [InlineData("const a = sql`x`", 13, "sql")]
        [InlineData("a_b$c`x`", 5, "a_b$c")]
        public void Tag_before_a_backtick_is_read_back_to_the_previous_non_word_character(string text, int backtick, string id)
        {
            Assert.Equal(id, MarkerComment.IdBefore(text, backtick));
        }

        [Fact]
        public void Tag_must_touch_the_backtick()
        {
            Assert.Null(MarkerComment.IdBefore("html `x`", 5));
            Assert.Null(MarkerComment.IdBefore("`x`", 0));
        }
    }

    public class MarkerTrackerTests
    {
        private static MarkerTracker Tracker()
        {
            return new MarkerTracker(new AcceptedEmbeddedLanguages(Pipeline.Languages));
        }

        private static string Take(string text, string comment, int stringStart)
        {
            var tracker = Tracker();
            int commentStart = text.IndexOf(comment);
            tracker.Comment(text, commentStart + 2, commentStart + comment.Length - (comment.StartsWith("/*") ? 2 : 0),
                            commentStart + comment.Length);
            return tracker.Take(text, stringStart);
        }

        [Fact]
        public void Marks_a_string_on_the_same_line()
        {
            string text = "x = /* html */ \"a\"";
            Assert.Equal("html", Take(text, "/* html */", text.IndexOf('"')));
        }

        [Fact]
        public void Marks_a_string_on_the_next_line_even_after_a_declaration()
        {
            string text = "// html\nvar page = \"a\"";
            Assert.Equal("html", Take(text, "// html", text.IndexOf('"')));
        }

        [Fact]
        public void Does_not_reach_two_lines_down()
        {
            string text = "// html\n\nvar page = \"a\"";
            Assert.Null(Take(text, "// html", text.IndexOf('"')));
        }

        [Theory]
        [InlineData("// html\nfoo(); \"a\"")]
        [InlineData("// html\nif (x) { \"a\"")]
        [InlineData("// html\n} \"a\"")]
        public void A_statement_boundary_cancels_the_marker(string text)
        {
            Assert.Null(Take(text, "// html", text.IndexOf('"')));
        }

        [Fact]
        public void Unknown_languages_are_not_markers()
        {
            string text = "// banana\n\"a\"";
            Assert.Null(Take(text, "// banana", text.IndexOf('"')));
        }

        [Fact]
        public void A_marker_is_used_once()
        {
            string text = "// html\n\"a\" \"b\"";
            var tracker = Tracker();
            tracker.Comment(text, 2, 7, 7);
            Assert.Equal("html", tracker.Take(text, 8));
            Assert.Null(tracker.Take(text, 12));
        }

        [Fact]
        public void A_following_ordinary_comment_cancels_the_marker()
        {
            string text = "// html\n// note\n\"a\"";
            var tracker = Tracker();
            tracker.Comment(text, 2, 7, 7);
            tracker.Comment(text, 10, 15, 15);
            Assert.Null(tracker.Take(text, 16));
        }
    }

    public class AcceptedEmbeddedLanguagesTests
    {
        [Fact]
        public void Accepts_known_ids_case_insensitively()
        {
            var languages = new AcceptedEmbeddedLanguages(Pipeline.Languages);
            Assert.True(languages.Accepts("html"));
            Assert.True(languages.Accepts("SQL"));
            Assert.False(languages.Accepts("banana"));
            Assert.False(languages.Accepts(null));
            Assert.False(languages.Accepts(""));
        }

        [Fact]
        public void Excluded_ids_are_rejected_even_when_known()
        {
            var languages = new AcceptedEmbeddedLanguages(Pipeline.Languages, new[] { "json", "regex" });
            Assert.False(languages.Accepts("json"));
            Assert.False(languages.Accepts("JSON"));
            Assert.True(languages.Accepts("sql"));
        }
    }
}
