using Xunit;

namespace NestLight.Tests
{
    /// <summary>YAML: keys, scalars, anchors, comments.</summary>
    public class YamlTokenizerTests
    {
        private static string[] Y(string yaml) { return Lexer.Language("yaml", yaml); }

        [Fact]
        public void Key_and_plain_value()
        {
            Assert.Equal(new[] { "yaml.key|name", "yaml.punctuation|:", "yaml.string|John Smith" }, Y("name: John Smith"));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("-5")]
        [InlineData("3.14")]
        [InlineData("1e3")]
        [InlineData("0x1F")]
        [InlineData("true")]
        [InlineData("False")]
        [InlineData("null")]
        [InlineData("~")]
        [InlineData("yes")]
        [InlineData(".inf")]
        public void Numbers_booleans_and_null_are_scalars(string value)
        {
            Assert.Equal(new[] { "yaml.key|k", "yaml.punctuation|:", "yaml.scalar|" + value }, Y("k: " + value));
        }

        [Theory]
        [InlineData("1.2.3")]
        [InlineData("truthy")]
        [InlineData("abc")]
        [InlineData("-x")]
        public void Other_plain_values_are_strings(string value)
        {
            Assert.Equal(new[] { "yaml.key|k", "yaml.punctuation|:", "yaml.string|" + value }, Y("k: " + value));
        }

        [Theory]
        [InlineData("\"double\"")]
        [InlineData("'single'")]
        [InlineData("\"a \\\" b\"")]
        [InlineData("'it''s'")]
        [InlineData("\"has: colon\"")]
        public void Quoted_scalars_are_strings(string value)
        {
            Assert.Equal(new[] { "yaml.key|k", "yaml.punctuation|:", "yaml.string|" + value }, Y("k: " + value));
        }

        [Fact]
        public void Quoted_keys()
        {
            Assert.Equal(new[] { "yaml.key|\"a b\"", "yaml.punctuation|:", "yaml.scalar|1" }, Y("\"a b\": 1"));
            Assert.Equal(new[] { "yaml.key|'a'", "yaml.punctuation|:", "yaml.scalar|1" }, Y("'a': 1"));
        }

        [Fact]
        public void Nested_mappings_by_indentation()
        {
            Assert.Equal(new[]
            {
                "yaml.key|a", "yaml.punctuation|:", "yaml.key|b", "yaml.punctuation|:", "yaml.scalar|1",
                "yaml.key|c", "yaml.punctuation|:", "yaml.scalar|2"
            }, Y("a:\n  b: 1\nc: 2"));
        }

        [Fact]
        public void Key_with_no_value()
        {
            Assert.Equal(new[] { "yaml.key|k", "yaml.punctuation|:" }, Y("k:"));
            Assert.Equal(new[] { "yaml.key|k", "yaml.punctuation|:" }, Y("k:   "));
        }

        [Fact]
        public void Keys_may_contain_spaces()
        {
            Assert.Equal(new[] { "yaml.key|two words", "yaml.punctuation|:", "yaml.scalar|1" }, Y("two words: 1"));
        }

        // ---- sequences ---------------------------------------------------------------------------------------

        [Fact]
        public void Sequence_entries()
        {
            Assert.Equal(new[] { "yaml.punctuation|-", "yaml.string|a", "yaml.punctuation|-", "yaml.scalar|2" }, Y("- a\n- 2"));
        }

        [Fact]
        public void Mapping_inside_a_sequence_entry()
        {
            Assert.Equal(new[] { "yaml.punctuation|-", "yaml.key|k", "yaml.punctuation|:", "yaml.string|v" }, Y("- k: v"));
        }

        [Fact]
        public void Nested_sequence_dashes()
        {
            Assert.Equal(new[] { "yaml.punctuation|-", "yaml.punctuation|-", "yaml.string|x" }, Y("- - x"));
        }

        [Fact]
        public void A_dash_glued_to_text_is_not_a_sequence_entry()
        {
            Assert.Equal(new[] { "yaml.string|-x" }, Y("-x"));
        }

        // ---- comments --------------------------------------------------------------------------------------

        [Fact]
        public void Full_line_and_trailing_comments()
        {
            Assert.Equal(new[] { "yaml.comment|# top", "yaml.key|a", "yaml.punctuation|:", "yaml.scalar|1", "yaml.comment|# trailing" },
                Y("# top\na: 1 # trailing"));
        }

        [Fact]
        public void Hash_without_a_space_before_it_is_not_a_comment()
        {
            Assert.Equal(new[] { "yaml.key|a", "yaml.punctuation|:", "yaml.string|b#c" }, Y("a: b#c"));
            // with a space before it, it is a comment
            Assert.Equal(new[] { "yaml.key|color", "yaml.punctuation|:", "yaml.comment|#fff" }, Y("color: #fff"));
        }

        [Fact]
        public void Hash_inside_quotes_is_not_a_comment()
        {
            Assert.Equal(new[] { "yaml.key|a", "yaml.punctuation|:", "yaml.string|\"x # y\"" }, Y("a: \"x # y\""));
        }

        [Fact]
        public void Indented_comment()
        {
            Assert.Equal(new[] { "yaml.key|a", "yaml.punctuation|:", "yaml.comment|# c", "yaml.key|b", "yaml.punctuation|:", "yaml.scalar|1" },
                Y("a:\n  # c\n  b: 1"));
        }

        // ---- anchors, aliases and tags ------------------------------------------------------------

        [Fact]
        public void Anchors_aliases_and_tags()
        {
            Assert.Equal(new[] { "yaml.key|a", "yaml.punctuation|:", "yaml.anchor|&x", "yaml.scalar|1" }, Y("a: &x 1"));
            Assert.Equal(new[] { "yaml.key|b", "yaml.punctuation|:", "yaml.anchor|*x" }, Y("b: *x"));
            Assert.Equal(new[] { "yaml.key|c", "yaml.punctuation|:", "yaml.anchor|!!str", "yaml.scalar|1" }, Y("c: !!str 1"));
        }

        [Fact]
        public void Merge_key_with_an_alias()
        {
            Assert.Equal(new[] { "yaml.key|<<", "yaml.punctuation|:", "yaml.anchor|*base" }, Y("<<: *base"));
        }

        // ---- documents and block scalars ---------------------------------------------------------------

        [Fact]
        public void Document_markers()
        {
            Assert.Equal(new[] { "yaml.punctuation|---", "yaml.key|a", "yaml.punctuation|:", "yaml.scalar|1", "yaml.punctuation|..." },
                Y("---\na: 1\n..."));
        }

        [Theory]
        [InlineData("|")]
        [InlineData(">")]
        [InlineData("|-")]
        [InlineData(">+")]
        [InlineData("|2")]
        public void Block_scalar_lines_are_strings_until_the_indentation_returns(string header)
        {
            Assert.Equal(new[]
            {
                "yaml.key|text", "yaml.punctuation|:", "yaml.punctuation|" + header,
                "yaml.string|line: one", "yaml.string|# not a comment", "yaml.key|next", "yaml.punctuation|:", "yaml.scalar|1"
            }, Y("text: " + header + "\n  line: one\n  # not a comment\nnext: 1"));
        }

        [Fact]
        public void Block_scalar_runs_to_the_end_of_the_text()
        {
            Assert.Equal(new[] { "yaml.key|t", "yaml.punctuation|:", "yaml.punctuation||", "yaml.string|a: b" }, Y("t: |\n  a: b"));
        }

        [Fact]
        public void Blank_lines_inside_a_block_scalar_do_not_end_it()
        {
            Assert.Equal(new[] { "yaml.key|t", "yaml.punctuation|:", "yaml.punctuation||", "yaml.string|a", "yaml.string|b" },
                Y("t: |\n  a\n\n  b"));
        }

        // ---- flow collections ----------------------------------------------------------------------------

        [Fact]
        public void Flow_sequence()
        {
            Assert.Equal(new[]
            {
                "yaml.key|k", "yaml.punctuation|:", "yaml.punctuation|[", "yaml.string|a", "yaml.punctuation|,",
                "yaml.scalar|1", "yaml.punctuation|,", "yaml.string|\"q\"", "yaml.punctuation|]"
            }, Y("k: [a, 1, \"q\"]"));
        }

        [Fact]
        public void Flow_mapping()
        {
            Assert.Equal(new[]
            {
                "yaml.key|k", "yaml.punctuation|:", "yaml.punctuation|{", "yaml.key|a", "yaml.punctuation|:", "yaml.scalar|1",
                "yaml.punctuation|,", "yaml.key|b", "yaml.punctuation|:", "yaml.string|x", "yaml.punctuation|}"
            }, Y("k: {a: 1, b: x}"));
        }

        [Fact]
        public void Url_values_are_strings_not_keys()
        {
            Assert.Equal(new[] { "yaml.key|home", "yaml.punctuation|:", "yaml.string|http://x.y/z" }, Y("home: http://x.y/z"));
        }

        [Fact]
        public void Windows_line_breaks()
        {
            Assert.Equal(new[] { "yaml.key|a", "yaml.punctuation|:", "yaml.scalar|1", "yaml.key|b", "yaml.punctuation|:", "yaml.scalar|2" },
                Y("a: 1\r\nb: 2\r\n"));
        }

        [Fact]
        public void Interpolations_as_values_and_inside_strings()
        {
            Assert.Equal(new[] { "yaml.key|a", "yaml.punctuation|:" }, Y("a: ${v}"));
            Assert.Equal(new[] { "yaml.key|a", "yaml.punctuation|:", "yaml.string|\"x", "yaml.string|y\"" }, Y("a: \"x${v}y\""));
        }

        [Fact]
        public void Empty_and_blank_input()
        {
            Assert.Empty(Y(""));
            Assert.Empty(Y("\n\n  \n"));
        }

        // ---- indentation stripping (host code indentation) ---------------------------------------------------

        [Fact]
        public void Indented_key_value_same_as_unindented()
        {
            Assert.Equal(Y("name: John Smith"), Y("\n    name: John Smith"));
        }

        [Fact]
        public void Indented_document_markers_are_recognized()
        {
            Assert.Equal(
                new[] { "yaml.punctuation|---", "yaml.key|a", "yaml.punctuation|:", "yaml.scalar|1", "yaml.punctuation|..." },
                Y("\n    ---\n    a: 1\n    ..."));
        }

        [Fact]
        public void Indented_block_scalar()
        {
            Assert.Equal(
                new[] { "yaml.key|text", "yaml.punctuation|:", "yaml.punctuation||", "yaml.string|hello", "yaml.key|next", "yaml.punctuation|:", "yaml.scalar|1" },
                Y("\n    text: |\n      hello\n    next: 1"));
        }

        [Fact]
        public void Indented_nested_mappings()
        {
            Assert.Equal(
                new[] { "yaml.key|a", "yaml.punctuation|:", "yaml.key|b", "yaml.punctuation|:", "yaml.scalar|1", "yaml.key|c", "yaml.punctuation|:", "yaml.scalar|2" },
                Y("\n    a:\n      b: 1\n    c: 2"));
        }

        [Fact]
        public void Indented_sequence()
        {
            Assert.Equal(
                new[] { "yaml.punctuation|-", "yaml.string|a", "yaml.punctuation|-", "yaml.scalar|2" },
                Y("\n    - a\n    - 2"));
        }

        // --- Mutant-killing tests ---

        [Fact]
        public void Block_parent_reset()
        {
            // Kills: L47 blockParent = -1
            // When indentation goes back, block scalar mode ends
            Assert.Equal(new[]
            {
                "yaml.key|t", "yaml.punctuation|:", "yaml.punctuation||",
                "yaml.string|hello",
                "yaml.key|k", "yaml.punctuation|:", "yaml.scalar|1"
            }, Y("t: |\n  hello\nk: 1"));
        }

        [Fact]
        public void Document_marker_boundary()
        {
            // Kills: L55 boundary checks on p + 3 == end and m[p+3] == ' '
            Assert.Equal(new[] { "yaml.punctuation|---" }, Y("---"));
            Assert.Equal(new[] { "yaml.punctuation|..." }, Y("..."));
            // --- followed by space + value
            Assert.Equal(new[] { "yaml.punctuation|---", "yaml.key|k", "yaml.punctuation|:", "yaml.scalar|1" },
                Y("--- k: 1"));
        }

        [Fact]
        public void Document_marker_with_tab()
        {
            // Kills: L55 m[p+3] != '\t' mutation
            Assert.Equal(new[] { "yaml.punctuation|---" }, Y("---\t"));
        }

        [Fact]
        public void Value_after_document_marker()
        {
            // Kills: L59 p < end && blockParent < 0 boundary
            Assert.Equal(new[] { "yaml.punctuation|---", "yaml.key|a", "yaml.punctuation|:", "yaml.scalar|1" },
                Y("--- a: 1"));
        }

        [Fact]
        public void Sequence_dash_at_end_of_line()
        {
            // Kills: L108 p + 1 == end boundary on "?" standalone
            Assert.Equal(new[] { "yaml.punctuation|?", "yaml.key|k", "yaml.punctuation|:", "yaml.scalar|1" },
                Y("?\n  k: 1"));
        }

        [Fact]
        public void Question_mark_followed_by_space()
        {
            // Kills: L108 p + 1 == end || m[p + 1] == ' '
            Assert.Equal(new[] { "yaml.punctuation|?", "yaml.key|a", "yaml.punctuation|:", "yaml.scalar|1" },
                Y("? a: 1"));
        }

        [Fact]
        public void Emit_and_increment_for_question_mark()
        {
            // Kills: L110 statement mutation, L112 statement mutation
            var result = Y("? key: val");
            Assert.Contains("yaml.punctuation|?", result);
            Assert.Contains("yaml.key|key", result);
        }

        [Fact]
        public void Quoted_key_detection()
        {
            // Kills: L121 after + 1 != end boundary
            Assert.Equal(new[] { "yaml.key|\"k\"", "yaml.punctuation|:", "yaml.scalar|1" }, Y("\"k\": 1"));
        }

        [Fact]
        public void Quoted_key_vs_quoted_value()
        {
            // Kills: L123 conditional true/false mutations (isKey ? after : close)
            Assert.Equal(new[] { "yaml.key|\"k\"", "yaml.punctuation|:", "yaml.string|\"v\"" }, Y("\"k\": \"v\""));
        }

        [Fact]
        public void Comment_preceded_by_tab()
        {
            // Kills: L155 m[p - 1] != '\t'
            Assert.Equal(new[] { "yaml.key|a", "yaml.punctuation|:", "yaml.scalar|1", "yaml.comment|# c" },
                Y("a: 1\t# c"));
        }

        [Fact]
        public void Sequence_dash_boundary_end_of_line()
        {
            // Kills: L166 p + 1 == end (dash at end)
            Assert.Equal(new[] { "yaml.punctuation|-" }, Y("-"));
        }

        [Fact]
        public void Flow_anchor_stop_chars()
        {
            // Kills: L175 boundary checks, char.IsWhiteSpace, ",]}".IndexOf
            Assert.Equal(new[]
            {
                "yaml.key|k", "yaml.punctuation|:", "yaml.punctuation|[",
                "yaml.anchor|&x", "yaml.scalar|1", "yaml.punctuation|,",
                "yaml.anchor|*x", "yaml.punctuation|]"
            }, Y("k: [&x 1, *x]"));
        }

        [Fact]
        public void Flow_emit_and_continue()
        {
            // Kills: L176 statement mutation, L178 statement mutation
            Assert.Contains("yaml.anchor|&a", Y("k: [&a 1]"));
        }

        [Fact]
        public void Plain_end_stop_at_colon_space()
        {
            // Kills: L190 stop + 1 != end, L191 stop < p boundary
            Assert.Equal(new[] { "yaml.key|a b", "yaml.punctuation|:", "yaml.scalar|1" }, Y("a b: 1"));
        }

        [Fact]
        public void Plain_end_trailing_whitespace_trimmed()
        {
            // Kills: L191 m[stop - 1] == ' ' || m[stop - 1] == '\t'
            Assert.Equal(new[] { "yaml.key|k", "yaml.punctuation|:", "yaml.scalar|1" }, Y("k  : 1"));
        }

        [Fact]
        public void Flow_key_detection()
        {
            // Kills: L198 after + 1 != end boundary
            Assert.Equal(new[]
            {
                "yaml.key|k", "yaml.punctuation|:", "yaml.punctuation|{",
                "yaml.key|a", "yaml.punctuation|:", "yaml.scalar|1",
                "yaml.punctuation|}"
            }, Y("k: {a: 1}"));
        }

        [Fact]
        public void Flow_colon_as_separator()
        {
            // Kills: L229 statement mutation
            Assert.Contains("yaml.punctuation|:", Y("k: {a: 1}"));
        }

        [Fact]
        public void Flow_comment_in_collection()
        {
            // Kills: L245 m[q] != '#'
            Assert.Contains("yaml.comment|# c", Y("k: [1 # c\n]"));
        }

        [Fact]
        public void TrimEnd_strips_spaces_and_tabs()
        {
            // Kills: L261 line boundary, m[line] == '\n' and '\r' mutations
            Assert.Equal(new[] { "yaml.key|a", "yaml.punctuation|:", "yaml.scalar|1" }, Y("a: 1   "));
        }

        [Fact]
        public void Scalar_detection_special_values()
        {
            // Kills: L290 ".inf" and ".nan" string mutations
            Assert.Equal(new[] { "yaml.key|k", "yaml.punctuation|:", "yaml.scalar|.inf" }, Y("k: .inf"));
            Assert.Equal(new[] { "yaml.key|k", "yaml.punctuation|:", "yaml.scalar|-.inf" }, Y("k: -.inf"));
            Assert.Equal(new[] { "yaml.key|k", "yaml.punctuation|:", "yaml.scalar|.nan" }, Y("k: .nan"));
        }

        [Fact]
        public void Quoted_end_double_quote_escape()
        {
            // Kills: L232 q + 1 < end boundary for quoted strings
            Assert.Equal(new[] { "yaml.key|k", "yaml.punctuation|:", "yaml.string|\"a\\\"b\"" }, Y("k: \"a\\\"b\""));
        }

        [Fact]
        public void Single_quote_escape_by_doubling()
        {
            // Kills: L232 '' escape for single quotes
            Assert.Equal(new[] { "yaml.key|k", "yaml.punctuation|:", "yaml.string|'it''s'" }, Y("k: 'it''s'"));
        }
    }
}
