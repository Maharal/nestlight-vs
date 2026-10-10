using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>SQL: keywords, identifiers, literals, operators, comments.</summary>
    public class SqlTokenizerTests
    {
        private static string[] S(string sql) { return Lexer.Language("sql", sql); }

        [Fact]
        public void Keywords_and_identifiers_are_told_apart()
        {
            Assert.Equal(new[]
            {
                "sql.keyword|SELECT", "sql.identifier|name", "sql.keyword|FROM", "sql.identifier|users",
                "sql.keyword|WHERE", "sql.identifier|id", "sql.operator|=", "sql.number|1"
            }, S("SELECT name FROM users WHERE id = 1"));
        }

        [Theory]
        [InlineData("select")]
        [InlineData("SELECT")]
        [InlineData("SeLeCt")]
        public void Keywords_are_case_insensitive(string word)
        {
            Assert.Equal(new[] { "sql.keyword|" + word }, S(word));
        }

        [Fact]
        public void Longer_words_that_start_with_a_keyword_are_identifiers()
        {
            Assert.Equal(new[] { "sql.identifier|selection", "sql.identifier|order_id", "sql.identifier|from1" },
                S("selection order_id from1"));
        }

        [Theory]
        [InlineData("'abc'")]
        [InlineData("''")]
        [InlineData("'it''s'")]
        [InlineData("'multi\nline'")]
        public void Single_quotes_make_a_string(string literal)
        {
            Assert.Equal(new[] { "sql.string|" + literal }, S(literal));
        }

        [Fact]
        public void Unterminated_string_runs_to_the_end()
        {
            Assert.Equal(new[] { "sql.keyword|select", "sql.string|'abc" }, S("select 'abc"));
        }

        [Theory]
        [InlineData("\"my col\"")]
        [InlineData("[my col2]")]
        [InlineData("[col]")]
        [InlineData("\"a\"\"b\"")]
        public void Quoted_identifiers(string quoted)
        {
            Assert.Equal(new[] { "sql.identifier|" + quoted }, S(quoted));
        }

        [Fact]
        public void Backtick_quoted_identifiers()
        {
            // a backtick cannot be written inside a JavaScript template, so the host here is Python
            Assert.Equal(new[] { "sql.identifier|`my col`" }, Lexer.Seq(HostLanguage.Python, "# sql\nx = '''`my col`'''"));
        }

        [Fact]
        public void Array_subscripts_are_not_bracket_identifiers()
        {
            Assert.Equal(new[] { "sql.identifier|arr", "sql.number|1" }, S("arr[1]"));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("42")]
        [InlineData("3.14")]
        [InlineData(".5")]
        [InlineData("1e5")]
        [InlineData("1.5E-3")]
        [InlineData("2e+8")]
        [InlineData("0xFF")]
        public void Numbers(string number)
        {
            Assert.Equal(new[] { "sql.number|" + number }, S(number));
        }

        [Theory]
        [InlineData("=")]
        [InlineData("<>")]
        [InlineData("<=")]
        [InlineData(">=")]
        [InlineData("!=")]
        [InlineData("||")]
        [InlineData("+")]
        [InlineData("*")]
        [InlineData("::")]
        public void Operators(string op)
        {
            Assert.Equal(new[] { "sql.operator|" + op }, S(op));
        }

        [Fact]
        public void Punctuation_is_not_colored()
        {
            Assert.Equal(new[] { "sql.identifier|f", "sql.identifier|a", "sql.identifier|b" }, S("f(a, b);"));
        }

        [Fact]
        public void Line_comment_ends_at_the_line_break()
        {
            Assert.Equal(new[] { "sql.keyword|select", "sql.comment|-- a b", "sql.number|1" }, S("select -- a b\n1"));
        }

        [Fact]
        public void Block_comment_spans_lines_and_may_be_unterminated()
        {
            Assert.Equal(new[] { "sql.comment|/* a\nb */", "sql.number|1" }, S("/* a\nb */ 1"));
            Assert.Equal(new[] { "sql.comment|/* open" }, S("/* open"));
        }

        [Fact]
        public void Keywords_inside_strings_and_comments_stay_in_them()
        {
            Assert.Equal(new[] { "sql.string|'select from'", "sql.comment|-- where" }, S("'select from' -- where"));
        }

        [Theory]
        [InlineData("@id")]
        [InlineData(":name")]
        [InlineData("$1")]
        public void Parameters_are_identifiers(string parameter)
        {
            Assert.Equal(new[] { "sql.parameter|" + parameter }, S(parameter));
        }

        [Fact]
        public void Postgres_cast_is_an_operator_not_a_parameter()
        {
            Assert.Equal(new[] { "sql.identifier|x", "sql.operator|::", "sql.keyword|int" }, S("x::int"));
        }

        [Fact]
        public void Interpolations_are_not_colored_and_do_not_break_the_neighbors()
        {
            Assert.Equal(new[]
            {
                "sql.keyword|select", "sql.operator|*", "sql.keyword|from",
                "sql.keyword|where", "sql.identifier|id", "sql.operator|="
            }, S("select * from ${table} where id = ${id}"));
        }

        [Fact]
        public void Interpolation_inside_a_string_stays_inside_the_string()
        {
            var seq = S("select '${a}b'");
            Assert.Equal(new[] { "sql.keyword|select", "sql.string|'", "sql.string|b'" }, seq);
        }

        [Fact]
        public void Empty_and_whitespace_only_code_gives_no_tokens()
        {
            Assert.Empty(S(""));
            Assert.Empty(S("  \n\t "));
        }
    }
}
