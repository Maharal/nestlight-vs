using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class YamlCompletionMutationTests
    {
        private static string Name(string codeWithCaret)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000,
                CompletionEngine.DefaultMinWordLength, features: CompletionFeatures.Default);
            var site = engine.Locate(code, caret);
            if (site == null) return "(no site)";
            Position position = CompletionLanguages.Default.Find(site.EmbeddedLanguageId).PositionAt(code, site);
            return position == null ? null : position.Name;
        }

        // ---- lineStart boundary (line 19) ----

        [Fact]
        public void LineStart_stops_at_floor()
        {
            // lineStart > floor && text[lineStart - 1] != '\n'
            Assert.Equal("yaml:key", Name("yaml`|`"));
        }

        // ---- Colon detection (lines 30-31) ----

        [Fact]
        public void Key_position_before_colon()
        {
            // Before any colon: it's a key
            Assert.Equal("yaml:key", Name("yaml`na|`"));
        }

        [Fact]
        public void Value_position_after_colon_space()
        {
            // Line 31: c == ':' && (i + 1 == line.Length || char.IsWhiteSpace(line[i + 1]))
            // After "key: ", value is true -> return null (values are not constrained)
            Assert.Null(Name("yaml`name: |`"));
        }

        [Fact]
        public void Colon_without_space_is_not_separator()
        {
            // Line 31: colon not followed by whitespace or end-of-line is not a separator
            Assert.Equal("yaml:key", Name("yaml`http://|`"));
        }

        [Fact]
        public void Colon_at_end_of_line_is_separator()
        {
            // Line 31: i + 1 == line.Length
            // After "name:\n", the next line starts fresh, and with no content it's a key position
            Assert.Equal("yaml:key", Name("yaml`name:\n|`"));
        }

        // ---- Comment detection (line 30) ----

        [Fact]
        public void Comment_position_after_hash()
        {
            // Line 30: c == '#' && (i == 0 || char.IsWhiteSpace(line[i - 1]))
            Assert.Equal("yaml:comment", Name("yaml`# |`"));
        }

        [Fact]
        public void Hash_after_space_is_comment()
        {
            Assert.Equal("yaml:comment", Name("yaml`key: value # |`"));
        }

        [Fact]
        public void Hash_without_space_is_not_comment()
        {
            // '#' at i != 0 with non-whitespace before it is not a comment
            // "key: value#" has '#' without preceding whitespace, so it's not a comment
            // But the colon+space makes it a value, so it's null (value position)
            var name = Name("yaml`key: value#|`");
            Assert.True(name == null || name != "yaml:comment");
        }

        // ---- Quote handling (lines 28-29) ----

        [Fact]
        public void Inside_double_quote_is_string()
        {
            Assert.Equal("yaml:string", Name("yaml`name: \"|`"));
        }

        [Fact]
        public void Inside_single_quote_is_string()
        {
            Assert.Equal("yaml:string", Name("yaml`name: '|`"));
        }

        [Fact]
        public void Closed_quote_continues_normally()
        {
            Assert.Null(Name("yaml`name: \"value\" |`"));
        }

        // ---- List item prefix (line 21) ----

        [Fact]
        public void List_item_dash_prefix_is_stripped()
        {
            // Line 21: while (line.StartsWith("- ", ...)) strips "- "
            Assert.Equal("yaml:key", Name("yaml`- |`"));
        }

        [Fact]
        public void Nested_list_items()
        {
            Assert.Equal("yaml:key", Name("yaml`- - |`"));
        }
    }
}
