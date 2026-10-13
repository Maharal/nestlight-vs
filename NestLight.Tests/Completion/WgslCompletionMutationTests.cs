using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class WgslCompletionMutationTests
    {
        private static CompletionEngine Engine()
        {
            return new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000, CompletionEngine.DefaultMinWordLength,
                new BandedPrefixMatcher(), features: CompletionFeatures.Default);
        }

        private static string Name(string codeWithCaret)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var site = Engine().Locate(code, caret);
            if (site == null) return "(no site)";
            Position position = CompletionLanguages.Default.Find(site.EmbeddedLanguageId).PositionAt(code, site);
            return position == null ? null : position.Name;
        }

        private static List<string> Texts(string codeWithCaret)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = Engine();
            var site = engine.Locate(code, caret);
            if (site == null) return new List<string>();
            return engine.Suggest(code, site).Select(s => s.Text).ToList();
        }

        // ---- Line 22: @attribute detection ----

        [Fact]
        public void Attribute_after_at_sign()
        {
            // site.Start > floor && text[site.Start - 1] == '@'
            Assert.Equal("wgsl:attribute", Name("wgsl`@|`"));
        }

        [Fact]
        public void Attribute_suggests_known_attributes()
        {
            var items = Texts("wgsl`@loc|`");
            Assert.Contains("location", items);
        }

        [Fact]
        public void No_attribute_when_at_start()
        {
            // site.Start == floor, so no '@' check
            Assert.Null(Name("wgsl`|`"));
        }

        // ---- Line 27: @builtin( ) detection ----

        [Fact]
        public void Builtin_value_inside_builtin_parens()
        {
            // builtin >= 0 && line.IndexOf(')', builtin) < 0
            Assert.Equal("wgsl:builtin-value", Name("wgsl`@builtin(|`"));
        }

        [Fact]
        public void Builtin_value_suggests_builtin_names()
        {
            var items = Texts("wgsl`@builtin(pos|`");
            Assert.Contains("position", items);
        }

        [Fact]
        public void Builtin_closed_returns_null()
        {
            // line.IndexOf(')', builtin) >= 0 means builtin is closed
            Assert.Null(Name("wgsl`@builtin(position) |`"));
        }

        // ---- Lines 29-32: @interpolate( ) detection ----

        [Fact]
        public void Interpolate_type_before_comma()
        {
            // line.IndexOf(',', interpolate) < 0 -> type
            Assert.Equal("wgsl:interpolate-type", Name("wgsl`@interpolate(|`"));
            var items = Texts("wgsl`@interpolate(per|`");
            Assert.Contains("perspective", items);
        }

        [Fact]
        public void Interpolate_sampling_after_comma()
        {
            // line.IndexOf(',', interpolate) >= 0 -> sampling
            Assert.Equal("wgsl:interpolate-sampling", Name("wgsl`@interpolate(perspective, |`"));
            var items = Texts("wgsl`@interpolate(perspective, cen|`");
            Assert.Contains("center", items);
        }

        // ---- Lines 34-38: var< > and ptr< > detection ----

        [Fact]
        public void Address_space_inside_var_angle_brackets()
        {
            // declaration >= 0 && line.IndexOf('>', declaration + 4) < 0
            // line.IndexOf(',', declaration) < 0 -> address space
            Assert.Equal("wgsl:address-space", Name("wgsl`var<|`"));
            var items = Texts("wgsl`var<uni|`");
            Assert.Contains("uniform", items);
        }

        [Fact]
        public void Access_mode_after_comma_in_var()
        {
            // line.IndexOf(',', declaration) >= 0 -> access mode
            Assert.Equal("wgsl:access-mode", Name("wgsl`var<storage, |`"));
            var items = Texts("wgsl`var<storage, rea|`");
            Assert.Contains("read", items);
        }

        [Fact]
        public void Ptr_angle_brackets_detected()
        {
            Assert.Equal("wgsl:address-space", Name("wgsl`ptr<|`"));
        }

        [Fact]
        public void Var_closed_angle_bracket_returns_null()
        {
            // line.IndexOf('>', declaration + 4) >= 0 means closed
            Assert.Null(Name("wgsl`var<uniform> |`"));
        }
    }
}
