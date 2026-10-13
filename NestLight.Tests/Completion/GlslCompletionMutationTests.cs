using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class GlslCompletionMutationTests
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

        private static List<string> Texts(string codeWithCaret)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000,
                CompletionEngine.DefaultMinWordLength, features: CompletionFeatures.Default);
            var site = engine.Locate(code, caret);
            if (site == null) return new List<string>();
            return engine.Suggest(code, site).Select(s => s.Text).ToList();
        }

        // ---- Line 23: directive detection (space and tab checks) ----

        [Fact]
        public void Directive_after_hash_with_no_space_or_tab()
        {
            // rest.IndexOf(' ') < 0 && rest.IndexOf('\t') < 0 -> directive
            Assert.Equal("glsl:directive", Name("glsl`#|`"));
        }

        [Fact]
        public void Directive_suggests_known_directives()
        {
            var items = Texts("glsl`#ver|`");
            Assert.Contains("version", items);
        }

        [Fact]
        public void No_directive_when_word_has_space()
        {
            // rest.IndexOf(' ') >= 0 -> not a bare directive name
            // But if starts with "version", gives version position
            Assert.Equal("glsl:version", Name("glsl`#version |`"));
        }

        [Fact]
        public void Version_position_suggests_profiles()
        {
            // Line 24: rest.StartsWith("version", ...) -> version
            var items = Texts("glsl`#version |`");
            Assert.Contains("es", items);
            Assert.Contains("core", items);
        }

        [Fact]
        public void Non_hash_line_returns_null()
        {
            Assert.Null(Name("glsl`void main() { |`"));
        }

        [Fact]
        public void Directive_with_tab_not_bare()
        {
            // rest.IndexOf('\t') >= 0 means tab present, not bare directive name
            Assert.NotEqual("glsl:directive", Name("glsl`#version\t|`"));
        }
    }
}
