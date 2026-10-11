using NestLight.Common;
using Xunit;

namespace NestLight.Tests
{
    public class TomlTokenizerTests
    {
        [Fact]
        public void Tables_are_classified()
        {
            var tokens = Lexer.Language("toml", "[package]\nname = \"test\"");
            Assert.Contains("toml.table|[package]", tokens);
        }

        [Fact]
        public void Array_tables_are_classified()
        {
            var tokens = Lexer.Language("toml", "[[dependencies]]\nname = \"dep\"");
            Assert.Contains("toml.table|[[dependencies]]", tokens);
        }

        [Fact]
        public void Keys_are_classified()
        {
            var tokens = Lexer.Language("toml", "name = \"test\"");
            Assert.Contains("toml.key|name", tokens);
        }

        [Fact]
        public void Strings_are_classified()
        {
            var tokens = Lexer.Language("toml", "name = \"test\"");
            Assert.Contains("toml.string|\"test\"", tokens);
        }

        [Fact]
        public void Numbers_are_classified()
        {
            var tokens = Lexer.Language("toml", "version = 42");
            Assert.Contains("toml.number|42", tokens);
        }

        [Fact]
        public void Booleans_are_classified()
        {
            var tokens = Lexer.Language("toml", "enabled = true");
            Assert.Contains("toml.literal|true", tokens);
        }

        [Fact]
        public void Dates_are_classified()
        {
            var tokens = Lexer.Language("toml", "date = 2024-01-15");
            Assert.Contains("toml.date|2024-01-15", tokens);
        }

        [Fact]
        public void Comments_are_classified()
        {
            var tokens = Lexer.Language("toml", "# this is a comment\nname = \"test\"");
            Assert.Contains("toml.comment|# this is a comment", tokens);
        }

        [Fact]
        public void Inline_comments_are_classified()
        {
            var tokens = Lexer.Language("toml", "[package] # comment\nname = \"test\"");
            Assert.Contains("toml.comment|# comment", tokens);
        }

        [Fact]
        public void Punctuation_is_classified()
        {
            var tokens = Lexer.Language("toml", "name = \"test\"");
            Assert.Contains("toml.punctuation|=", tokens);
        }

        [Fact]
        public void Hex_numbers_are_classified()
        {
            var tokens = Lexer.Language("toml", "color = 0xFF00FF");
            Assert.Contains("toml.number|0xFF00FF", tokens);
        }

        [Fact]
        public void Float_numbers_are_classified()
        {
            var tokens = Lexer.Language("toml", "pi = 3.14");
            Assert.Contains("toml.number|3.14", tokens);
        }

        [Fact]
        public void No_tokens_overlap()
        {
            Assert.False(Lexer.HasOverlap(Lexer.Lex(Lexer.In("toml", "[package]\nname = \"test\"\nversion = 1\nenabled = true\n# comment"))));
        }

        [Fact]
        public void Array_values_are_parsed()
        {
            var tokens = Lexer.Language("toml", "deps = [\"a\", \"b\"]");
            Assert.Contains("toml.string|\"a\"", tokens);
            Assert.Contains("toml.string|\"b\"", tokens);
            Assert.Contains("toml.punctuation|[", tokens);
            Assert.Contains("toml.punctuation|]", tokens);
        }
    }
}
