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

        // --- Mutant-killing tests ---

        [Fact]
        public void Blank_lines_are_skipped()
        {
            // Kills: L24 statement mutation (skip blank lines)
            var tokens = Lexer.Language("toml", "\n\nname = \"test\"");
            Assert.Contains("toml.key|name", tokens);
        }

        [Fact]
        public void Array_table_boundary_check()
        {
            // Kills: L38 boundary mutations on p+1 < end, L41 boundary on close < end
            var tokens = Lexer.Language("toml", "[[a]]");
            Assert.Contains("toml.table|[[a]]", tokens);
            // Single bracket table with inline comment
            tokens = Lexer.Language("toml", "[a] # note");
            Assert.Contains("toml.table|[a]", tokens);
            Assert.Contains("toml.comment|# note", tokens);
        }

        [Fact]
        public void Comment_on_table_line()
        {
            // Kills: L44 boundary on p < end and m[p] == '#'
            var tokens = Lexer.Language("toml", "[pkg] # c");
            Assert.Contains("toml.comment|# c", tokens);
        }

        [Fact]
        public void Key_with_equals_sign()
        {
            // Kills: L50 keyEnd > p, L54 boundary on p < end and m[p] == '='
            var tokens = Lexer.Language("toml", "k = 1");
            Assert.Contains("toml.key|k", tokens);
            Assert.Contains("toml.punctuation|=", tokens);
            Assert.Contains("toml.number|1", tokens);
        }

        [Fact]
        public void Equals_sign_spacing()
        {
            // Kills: L57 arithmetic mutation p - 1
            var tokens = Lexer.Language("toml", "k = \"v\"");
            Assert.Contains("toml.punctuation|=", tokens);
            Assert.Contains("toml.string|\"v\"", tokens);
        }

        [Fact]
        public void Value_parsing_loop()
        {
            // Kills: L68 boundary on p < end
            var tokens = Lexer.Language("toml", "k = 42");
            Assert.Contains("toml.number|42", tokens);
        }

        [Fact]
        public void Comment_in_value_area()
        {
            // Kills: L76-77 statement mutations
            var tokens = Lexer.Language("toml", "k = 1 # remark");
            Assert.Contains("toml.comment|# remark", tokens);
            Assert.Contains("toml.number|1", tokens);
        }

        [Fact]
        public void Triple_quoted_strings()
        {
            // Kills: L82 triple string detection and L83 conditional
            var tokens = Lexer.Language("toml", "k = \"\"\"hello\"\"\"");
            Assert.Contains("toml.string|\"\"\"hello\"\"\"", tokens);
            // Single-quoted triple
            tokens = Lexer.Language("toml", "k = '''world'''");
            Assert.Contains("toml.string|'''world'''", tokens);
        }

        [Fact]
        public void False_literal_is_classified()
        {
            // Kills: L95 c == 'f' check
            var tokens = Lexer.Language("toml", "x = false");
            Assert.Contains("toml.literal|false", tokens);
        }

        [Fact]
        public void Inline_table_punctuation()
        {
            // Kills: L112, L116, L118 statement mutations for ], }, ,
            var tokens = Lexer.Language("toml", "k = {a = 1, b = 2}");
            Assert.Contains("toml.punctuation|{", tokens);
            Assert.Contains("toml.punctuation|}", tokens);
            Assert.Contains("toml.punctuation|,", tokens);
        }

        [Fact]
        public void Inline_array_punctuation()
        {
            // Kills: L112, L126 statement mutations for [ and ]
            var tokens = Lexer.Language("toml", "k = [1, 2]");
            Assert.Contains("toml.punctuation|[", tokens);
            Assert.Contains("toml.punctuation|]", tokens);
        }

        [Fact]
        public void Signed_numbers()
        {
            // Kills: L129 mutations on c == '+' and c == '-', p+1 boundary, digit/inf/nan checks
            var tokens = Lexer.Language("toml", "a = +42");
            Assert.Contains("toml.number|+42", tokens);
            tokens = Lexer.Language("toml", "a = -5");
            Assert.Contains("toml.number|-5", tokens);
        }

        [Fact]
        public void Signed_inf_and_nan()
        {
            // Kills: L129 mutations on m[p+1]=='i' and m[p+1]=='n'
            var tokens = Lexer.Language("toml", "a = +inf");
            Assert.Contains("toml.number|+inf", tokens);
            tokens = Lexer.Language("toml", "a = -nan");
            Assert.Contains("toml.number|-nan", tokens);
        }

        [Fact]
        public void Bare_inf_and_nan()
        {
            // Kills: L137 logical mutation on inf/nan
            var tokens = Lexer.Language("toml", "a = inf");
            Assert.Contains("toml.number|inf", tokens);
            tokens = Lexer.Language("toml", "a = nan");
            Assert.Contains("toml.number|nan", tokens);
        }

        [Fact]
        public void Word_starting_value_is_consumed()
        {
            // Kills: L144 negate IsWordStart, L147 boundary, L147 IsWordChar || m[wEnd]=='-'
            var tokens = Lexer.Language("toml", "k = some-word");
            // "some-word" is not a known value type, so it gets consumed but not emitted as a specific token
            // But the key should still be found
            Assert.Contains("toml.key|k", tokens);
        }

        [Fact]
        public void Quoted_key_and_dotted_key()
        {
            // Kills: L162 c == '"' || c == '\'', L149 statement mutation
            var tokens = Lexer.Language("toml", "\"key\" = 1");
            Assert.Contains("toml.key|\"key\"", tokens);
            tokens = Lexer.Language("toml", "a.b = 1");
            Assert.Contains("toml.key|a.b", tokens);
        }

        [Fact]
        public void Key_trimming_strips_trailing_blanks_and_dots()
        {
            // Kills: L180 boundary checks and L180 logical mutations on key trailing chars
            var tokens = Lexer.Language("toml", "k  = 1");
            Assert.Contains("toml.key|k", tokens);
        }

        [Fact]
        public void Key_detection_requires_equals_sign_ahead()
        {
            // Kills: L181 conditional and logical mutations
            var tokens = Lexer.Language("toml", "k = 1");
            Assert.Contains("toml.key|k", tokens);
            Assert.Contains("toml.punctuation|=", tokens);
        }

        [Fact]
        public void Single_string_with_escape()
        {
            // Kills: L189 quote == '"' && m[q] == '\\' (double-quoted escape)
            var tokens = Lexer.Language("toml", "k = \"a\\\"b\"");
            Assert.Contains("toml.string|\"a\\\"b\"", tokens);
        }

        [Fact]
        public void Single_quoted_string_no_escape()
        {
            // Kills: L189 — single quotes do NOT support escapes
            var tokens = Lexer.Language("toml", "k = 'hello'");
            Assert.Contains("toml.string|'hello'", tokens);
        }

        [Fact]
        public void Triple_string_with_escape()
        {
            // Kills: L200-204 triple string end detection, L202 escape handling
            var tokens = Lexer.Language("toml", "k = \"\"\"a\\\"b\"\"\"");
            Assert.Contains("toml.string|\"\"\"a\\\"b\"\"\"", tokens);
        }

        [Fact]
        public void DateTime_detection()
        {
            // Kills: L211-214 IsDateTimeStart digit checks
            var tokens = Lexer.Language("toml", "d = 2024-01-15T12:30:00Z");
            Assert.Contains("toml.date|2024-01-15T12:30:00Z", tokens);
        }

        [Fact]
        public void DateTime_not_detected_for_short_input()
        {
            // Kills: L211 p + 10 > end boundary
            var tokens = Lexer.Language("toml", "k = 2024-01");
            // Too short for datetime, should be treated as number
            Assert.DoesNotContain("toml.date|2024-01", tokens);
        }

        [Fact]
        public void DateTime_end_includes_timezone_and_fractions()
        {
            // Kills: L220-221 DateTimeEnd character loop
            var tokens = Lexer.Language("toml", "d = 2024-01-15T12:30:00.123+05:30");
            Assert.Contains("toml.date|2024-01-15T12:30:00.123+05:30", tokens);
        }

        [Fact]
        public void Number_with_sign_prefix()
        {
            // Kills: L228 m[q] == '+' || m[q] == '-'
            var tokens = Lexer.Language("toml", "a = +1_000");
            Assert.Contains("toml.number|+1_000", tokens);
            tokens = Lexer.Language("toml", "a = -1_000");
            Assert.Contains("toml.number|-1_000", tokens);
        }

        [Fact]
        public void Hex_prefix_case_insensitive()
        {
            // Kills: L232 prefix == 'X'
            var tokens = Lexer.Language("toml", "a = 0xAB");
            Assert.Contains("toml.number|0xAB", tokens);
            tokens = Lexer.Language("toml", "a = 0XAB");
            Assert.Contains("toml.number|0XAB", tokens);
        }

        [Fact]
        public void Hex_number_with_underscores()
        {
            // Kills: L235 boundary and m[q] == '_'
            var tokens = Lexer.Language("toml", "a = 0xFF_00");
            Assert.Contains("toml.number|0xFF_00", tokens);
        }

        [Fact]
        public void Octal_numbers()
        {
            // Kills: L238 prefix == 'o' || prefix == 'O'
            var tokens = Lexer.Language("toml", "a = 0o755");
            Assert.Contains("toml.number|0o755", tokens);
            tokens = Lexer.Language("toml", "a = 0O755");
            Assert.Contains("toml.number|0O755", tokens);
        }

        [Fact]
        public void Binary_numbers()
        {
            // Kills: L244 prefix == 'b' || prefix == 'B', L247 binary digit loop
            var tokens = Lexer.Language("toml", "a = 0b1010");
            Assert.Contains("toml.number|0b1010", tokens);
            tokens = Lexer.Language("toml", "a = 0B1010");
            Assert.Contains("toml.number|0B1010", tokens);
        }

        [Fact]
        public void Binary_with_underscores()
        {
            // Kills: L247 m[q] == '0' || m[q] == '1' || m[q] == '_'
            var tokens = Lexer.Language("toml", "a = 0b10_10");
            Assert.Contains("toml.number|0b10_10", tokens);
        }

        [Fact]
        public void Integer_with_underscores()
        {
            // Kills: L251-253 integer/decimal digit loops with underscore
            var tokens = Lexer.Language("toml", "a = 1_000");
            Assert.Contains("toml.number|1_000", tokens);
        }

        [Fact]
        public void Decimal_with_fractional_part()
        {
            // Kills: L253-257 underscore in fractional part
            var tokens = Lexer.Language("toml", "a = 3.14_15");
            Assert.Contains("toml.number|3.14_15", tokens);
        }

        [Fact]
        public void Exponent_notation()
        {
            // Kills: L259 m[q] == 'e' || m[q] == 'E'
            var tokens = Lexer.Language("toml", "a = 1e10");
            Assert.Contains("toml.number|1e10", tokens);
            tokens = Lexer.Language("toml", "a = 1E10");
            Assert.Contains("toml.number|1E10", tokens);
        }

        [Fact]
        public void No_tokens_overlap_complex()
        {
            Assert.False(Lexer.HasOverlap(Lexer.Lex(Lexer.In("toml",
                "[[deps]]\nname = \"a\"\nversion = 0xFF\ndate = 2024-01-15\n# comment\nval = +inf\nbin = 0b1010\noct = 0o777"))));
        }
    }
}
