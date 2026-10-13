using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class JsonCompletionMutationTests
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

        // ---- Line 16: inString = false outside string ----

        [Fact]
        public void Value_outside_string_returns_null()
        {
            // Line 16: inString starts false -> null outside strings
            Assert.Null(Name("json`{\"key\": |}`"));
        }

        // ---- Line 27: stack tracking and isKey determination ----

        [Fact]
        public void Key_position_inside_object_string()
        {
            // stack.Count > 0 && stack[last] == '{' && (previous == '{' || previous == ',')
            Assert.Equal("json:key", Name("json`{\"| `"));
        }

        [Fact]
        public void String_position_for_value()
        {
            // Inside string after colon: not a key
            Assert.Equal("json:string", Name("json`{\"key\": \"| `"));
        }

        // ---- Lines 29-30: bracket stack ----

        [Fact]
        public void Nested_objects_track_correctly()
        {
            // c == '{' pushes, c == '}' pops
            Assert.Equal("json:key", Name("json`{\"a\": {\"| `"));
        }

        [Fact]
        public void Array_bracket_tracking()
        {
            // c == '[' pushes, c == ']' pops
            Assert.Equal("json:string", Name("json`{\"a\": [\"| `"));
        }

        [Fact]
        public void After_comma_in_object_is_key()
        {
            // previous == ',' with object on stack
            Assert.Equal("json:key", Name("json`{\"a\": 1, \"| `"));
        }

        [Fact]
        public void Escape_in_string_skips_next_char()
        {
            // Line 23: c == '\\' -> i++ (skip escaped char)
            Assert.Equal("json:key", Name("json`{\"a\\\"b\": 1, \"| `"));
        }

        [Fact]
        public void Whitespace_is_skipped()
        {
            // Line 28: char.IsWhiteSpace(c) continue
            Assert.Equal("json:key", Name("json`{   \n   \"| `"));
        }
    }
}
