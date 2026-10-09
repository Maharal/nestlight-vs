using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>Regular expressions: groups, classes, quantifiers, escapes, anchors.</summary>
    public class RegexTokenizerTests
    {
        private static string[] R(string regex) { return Lexer.Language("regex", regex); }

        [Fact]
        public void Anchors_groups_alternation_and_quantifier()
        {
            Assert.Equal(new[]
            {
                "regex.anchor|^", "regex.group|(", "regex.group||", "regex.group|)", "regex.quantifier|+", "regex.anchor|$"
            }, R("^(a|b)+$"));
        }

        [Theory]
        [InlineData("\\d")]
        [InlineData("\\w")]
        [InlineData("\\S")]
        [InlineData("\\.")]
        [InlineData("\\\\")]
        [InlineData("\\n")]
        [InlineData("\\/")]
        public void Escapes(string escape)
        {
            Assert.Equal(new[] { "regex.escape|" + escape }, R(escape));
        }

        [Theory]
        [InlineData("\\b")]
        [InlineData("\\B")]
        [InlineData("\\A")]
        [InlineData("\\Z")]
        [InlineData("\\z")]
        [InlineData("\\G")]
        public void Anchor_escapes(string escape)
        {
            Assert.Equal(new[] { "regex.anchor|" + escape }, R(escape));
        }

        [Theory]
        [InlineData("\\x41")]
        [InlineData("\\u00e9")]
        [InlineData("\\p{L}")]
        [InlineData("\\P{Lu}")]
        [InlineData("\\k<name>")]
        [InlineData("\\12")]
        public void Escapes_that_take_arguments(string escape)
        {
            Assert.Equal(new[] { "regex.escape|" + escape }, R(escape));
        }

        [Fact]
        public void Escape_takes_only_as_many_hex_digits_as_it_needs()
        {
            Assert.Equal(new[] { "regex.escape|\\x41" }, R("\\x41zz"));
            Assert.Equal(new[] { "regex.escape|\\x4" }, R("\\x4"));
        }

        [Fact]
        public void A_trailing_backslash_is_an_escape_of_nothing()
        {
            // a backslash at the end of a JavaScript template would escape the backtick, so the host here is C++
            Assert.Equal(new[] { "regex.escape|\\" }, Lexer.Seq(HostLanguage.Cpp, "// regex\nauto a = R\"(\\)\";"));
        }

        [Theory]
        [InlineData("[abc]")]
        [InlineData("[a-z]")]
        [InlineData("[^a-z]")]
        [InlineData("[\\]x]")]
        [InlineData("[]a]")]
        [InlineData("[^]a]")]
        [InlineData("[[:alpha:]]")]
        [InlineData("[.*+]")]
        public void Character_classes_are_one_token(string cls)
        {
            Assert.Equal(new[] { "regex.class|" + cls }, R(cls));
        }

        [Fact]
        public void Unclosed_class_runs_to_the_end()
        {
            Assert.Equal(new[] { "regex.class|[abc" }, R("[abc"));
        }

        [Fact]
        public void The_dot_is_a_class()
        {
            Assert.Equal(new[] { "regex.class|." }, R("."));
        }

        [Theory]
        [InlineData("(?:")]
        [InlineData("(?=")]
        [InlineData("(?!")]
        [InlineData("(?<=")]
        [InlineData("(?<!")]
        [InlineData("(?>")]
        [InlineData("(?<name>")]
        [InlineData("(?P<name>")]
        [InlineData("(?'name'")]
        [InlineData("(?i)")]
        [InlineData("(?-i)")]
        [InlineData("(?i:")]
        [InlineData("(")]
        public void Group_openings_include_their_construct(string opening)
        {
            Assert.Equal(new[] { "regex.group|" + opening }, R(opening));
        }

        [Fact]
        public void Group_text_is_left_alone()
        {
            Assert.Equal(new[] { "regex.group|(?<year>", "regex.escape|\\d", "regex.quantifier|{4}", "regex.group|)" }, R("(?<year>\\d{4})"));
        }

        [Theory]
        [InlineData("*")]
        [InlineData("+")]
        [InlineData("?")]
        [InlineData("*?")]
        [InlineData("+?")]
        [InlineData("??")]
        [InlineData("*+")]
        [InlineData("{3}")]
        [InlineData("{3,}")]
        [InlineData("{3,5}")]
        [InlineData("{,5}")]
        [InlineData("{3,5}?")]
        public void Quantifiers_with_lazy_and_possessive_suffixes(string q)
        {
            Assert.Equal(new[] { "regex.quantifier|" + q }, R(q));
        }

        [Theory]
        [InlineData("{")]
        [InlineData("{}")]
        [InlineData("{x}")]
        [InlineData("{,}")]
        [InlineData("{3")]
        public void A_brace_that_is_not_a_repeat_is_a_literal(string text)
        {
            Assert.Empty(R(text));
        }

        [Fact]
        public void Literal_characters_are_not_colored()
        {
            Assert.Empty(R("abc 123 -_@"));
        }

        [Fact]
        public void Realistic_email_pattern()
        {
            Assert.Equal(new[]
            {
                "regex.anchor|^", "regex.class|[\\w.+-]", "regex.quantifier|+", "regex.class|[\\w-]", "regex.quantifier|+",
                "regex.escape|\\.", "regex.class|[a-z]", "regex.quantifier|{2,}", "regex.anchor|$"
            }, R("^[\\w.+-]+@[\\w-]+\\.[a-z]{2,}$"));
        }

        [Fact]
        public void Interpolations_are_neutral()
        {
            Assert.Equal(new[] { "regex.anchor|^", "regex.group|(", "regex.group|)", "regex.quantifier|+" }, R("^(${alt})+"));
        }
    }
}
