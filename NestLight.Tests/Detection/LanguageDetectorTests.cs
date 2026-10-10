using System.Linq;
using NestLight.Detection;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The strategies of the automatic detector and the context that asks them.</summary>
    public class LanguageDetectorTests
    {
        private static string Detect(string text, bool gate = true)
        {
            return LanguageDetector.Create(gate).Detect(text, 0, text.Length);
        }

        [Theory]
        [InlineData("SELECT id, name FROM customers WHERE active = 1", "sql")]
        [InlineData("insert into orders (customer_id, total) values (1, 2)", "sql")]
        [InlineData("  UPDATE products SET price = 1 WHERE id = 2  ", "sql")]
        [InlineData("<div class=\"card\"><p>Body</p></div>", "html")]
        [InlineData("<img src=\"logo.png\" alt=\"Logo\" />", "html")]
        [InlineData("{\"name\": \"Ada\", \"age\": 36}", "json")]
        [InlineData("[1, 2, 3, 4, 5]", "json")]
        [InlineData(".card { color: red; margin: 0 auto; }", "css")]
        [InlineData("a:hover { text-decoration: underline }", "css")]
        [InlineData("query Users($id: ID!) { user(id: $id) { name } }", "graphql")]
        [InlineData("{ viewer { login repositories(first: 5) { totalCount } } }", "graphql")]
        [InlineData("SELECT a\nFROM t\nWHERE a = 1", "sql")]
        [InlineData("SELECT id\n  FROM users\n  WHERE active = 1", "sql")]
        [InlineData("/* reset */ body { color: red; }", "css")]
        [InlineData("/* vars */\n:root { --c: red; }", "css")]
        [InlineData("#main { display: flex; }", "css")]
        [InlineData("# comment\nquery Q { user { name } }", "graphql")]
        [InlineData("# comment\n{ viewer { login } }", "graphql")]
        public void Code_is_recognized(string text, string language)
        {
            Assert.Equal(language, Detect(text));
        }

        [Theory]
        [InlineData("")]
        [InlineData("hello world")]
        [InlineData("Error: file not found")]
        [InlineData("https://example.com/api/v1/users")]
        [InlineData("C:\\temp\\output.txt")]
        [InlineData("Delete the file from disk?")]
        [InlineData("select")]
        [InlineData("<b>")]
        [InlineData("a < b and b > c")]
        [InlineData("{0} items selected")]
        [InlineData("application/json")]
        [InlineData("Select an option from the list")]
        [InlineData("Delete your account from settings")]
        [InlineData("{name} has joined {room}")]
        public void Ordinary_text_is_left_alone(string text)
        {
            Assert.Null(Detect(text));
        }

        [Fact]
        public void Gating_does_not_change_the_answer()
        {
            foreach (string s in new[] { "SELECT a FROM b WHERE c = 1", "<p>x</p>", "{\"a\": 1}", ".a { b: c; }", "query Q { a { b } }", "plain text here" })
                Assert.Equal(Detect(s, false), Detect(s, true));
        }

        [Fact]
        public void A_range_of_a_larger_text_is_read_where_it_is()
        {
            const string text = "var q = \"SELECT a FROM b WHERE c = 1\";";
            Assert.Equal("sql", LanguageDetector.Create(true).Detect(text, 9, text.Length - 2));
        }

        [Fact]
        public void The_strategies_have_distinct_ids_and_are_what_the_options_offer()
        {
            string[] ids = LanguageDetector.Strategies().Select(s => s.Id).ToArray();
            Assert.Equal(ids.Length, ids.Distinct().Count());
            Assert.Equal(ids, DetectionOptions.Available.ToArray());
        }

        [Fact]
        public void Every_language_it_can_detect_has_a_tokenizer()
        {
            foreach (string id in DetectionOptions.Available)
                Assert.True(Pipeline.Languages.IsKnown(id), id);
        }

        [Theory]
        [InlineData("\u0000")]
        [InlineData("{")]
        [InlineData("<")]
        [InlineData("[")]
        [InlineData("{\"")]
        [InlineData("<a")]
        [InlineData("select from")]
        [InlineData("query {")]
        [InlineData("{ }}}}}}}}}}")]
        [InlineData("<<<<<<<<<<")]
        public void Odd_text_never_throws(string text)
        {
            Detect(text);
            for (int n = 0; n <= text.Length; n++) Detect(text.Substring(0, n));
        }
    }
}
