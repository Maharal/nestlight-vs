using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The words that a review of 800 suggestions found missing.</summary>
    public class MissingVocabularyTests
    {
        private static readonly CompletionFeatures Grammar = new CompletionFeatures(grammar: true);

        private static CompletionEngine Engine(CompletionFeatures features)
        {
            return new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000, CompletionEngine.DefaultMinWordLength, features: features);
        }

        private static string Name(string codeWithCaret)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            CompletionSite site = Engine(Grammar).Locate(code, caret);
            Position position = Positions.At(code, site);
            return position == null ? null : position.Name;
        }

        private static List<Suggestion> Items(string codeWithCaret, CompletionFeatures features = null)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            CompletionEngine engine = Engine(features ?? Grammar);
            return engine.Suggest(code, engine.Locate(code, caret)).ToList();
        }

        private static List<string> Texts(string codeWithCaret, CompletionFeatures features = null) { return Items(codeWithCaret, features).Select(s => s.Text).ToList(); }

        [Fact]
        public void The_words_that_are_offered_but_not_colored_stay_out_of_the_vocabulary_the_tokenizers_share()
        {
            Assert.DoesNotContain("main", Vocabularies.For("glsl"));
            Assert.DoesNotContain("gl_FragColor", Vocabularies.For("glsl"));
            Assert.Contains("main", Vocabularies.ForCompletion("glsl"));
            Assert.Contains("gl_FragColor", Vocabularies.ForCompletion("glsl"));
            Assert.Contains("main", Vocabularies.ForCompletion("wgsl"));
            Assert.Equal(Vocabularies.For("sql"), Vocabularies.ForCompletion("sql")); // a language without extras is unchanged
            Assert.Equal("gl_FragColor", Vocabularies.FindInCompletion("GLSL", "GL_FRAGCOLOR"));
            Assert.Null(Vocabularies.Find("glsl", "main"));
            Assert.Equal(Vocabularies.ForCompletion("glsl").OrderBy(w => w, System.StringComparer.OrdinalIgnoreCase), Vocabularies.ForCompletion("glsl"));
        }

        [Fact]
        public void Glsl_offers_main_and_its_variables()
        {
            Assert.Contains("main", Texts("glsl`void ma|`"));
            Assert.Contains("gl_FragColor", Texts("glsl`void main() { gl_Fr|`"));
            Assert.Contains("gl_Position", Texts("glsl`void main() { gl_Po|`"));
        }

        [Fact]
        public void Glsl_directives_and_the_version_profile()
        {
            Assert.Equal("glsl:directive", Name("glsl`#ver|`"));
            Assert.Equal("version", Texts("glsl`#ver|`")[0]);
            Assert.Equal("define", Texts("glsl`#de|`")[0]);
            Assert.Equal("glsl:version", Name("glsl`#version 300 |`"));
            Assert.Equal("es", Texts("glsl`#version 300 |`")[0]);
            Assert.Null(Name("glsl`float a = 1.0; // #ver|`".Replace("#ver", "ver")));
            Assert.DoesNotContain("define", Texts("glsl`float x;\\n  def|`")); // a directive only after '#'
        }

        [Fact]
        public void Wgsl_attributes_and_the_values_inside_them()
        {
            Assert.Equal("wgsl:attribute", Name("wgsl`@|`"));
            Assert.Equal("builtin", Texts("wgsl`@bui|`")[0]);
            Assert.Equal("vertex", Texts("wgsl`@ver|`")[0]);
            Assert.Equal("workgroup_size", Texts("wgsl`@compute @work|`")[0]);
            Assert.Equal("wgsl:builtin-value", Name("wgsl`struct A { @builtin(|`"));
            Assert.Equal("position", Texts("wgsl`struct A { @builtin(pos|`")[0]);
            Assert.Equal("global_invocation_id", Texts("wgsl`fn f(@builtin(glo|`")[0]);
            Assert.Equal("wgsl:interpolate-type", Name("wgsl`@interpolate(|`"));
            Assert.Equal("wgsl:interpolate-sampling", Name("wgsl`@interpolate(flat, |`"));
            Assert.Null(Name("wgsl`@builtin(position) x : vec4<f32>, @location(0) y|`".Replace("y|", "|")));
        }

        [Fact]
        public void Wgsl_address_spaces_and_access_modes()
        {
            Assert.Equal("wgsl:address-space", Name("wgsl`@group(0) @binding(0) var<|`"));
            Assert.Equal("uniform", Texts("wgsl`@group(0) @binding(0) var<un|`")[0]);
            Assert.Equal("storage", Texts("wgsl`var<sto|`")[0]);
            Assert.Equal("wgsl:access-mode", Name("wgsl`var<storage, |`"));
            Assert.Equal("read_write", Texts("wgsl`var<storage, read_|`")[0]);
            Assert.Equal("read", Texts("wgsl`var<storage, rea|`")[0]);
            Assert.Null(Name("wgsl`var<uniform> u : f|`".Replace("f|", "|")));
            Assert.Contains("main", Texts("wgsl`fn ma|`"));
        }

        [Fact]
        public void Html_roles_aria_values_and_colors_of_svg()
        {
            Assert.Equal("region", Texts("html`<section role='reg|'>`")[0]);
            Assert.Equal("navigation", Texts("html`<div role='navi|'>`")[0]);
            Assert.Equal("true", Texts("html`<button aria-pressed='tr|'>`")[0]);
            Assert.Equal("polite", Texts("html`<div aria-live='po|'>`")[0]);
            Assert.Equal("currentColor", Texts("html`<svg><path stroke='cur|'/></svg>`")[0]);
            Assert.Equal("round", Texts("html`<svg><path stroke-linecap='ro|'/></svg>`")[0]);
            Assert.Equal("lazy", Texts("html`<img loading='la|'>`")[0]);
            Assert.Equal("multipart/form-data", Texts("html`<form enctype='mult|'>`")[0]);
            Assert.Equal("viewport", Texts("html`<meta name='view|'>`")[0]);
            Assert.Equal("module", Texts("html`<script type='mod|'>`")[0]);
            Assert.Equal("new-password", Texts("html`<input autocomplete='new-|'>`")[0]);
        }

        [Fact]
        public void More_values_of_css_properties()
        {
            Assert.Equal("sans-serif", Texts("css`.a { font-family: sans|}`")[0]);
            Assert.Equal("cover", Texts("css`.a { background-size: co|}`")[0]);
            Assert.Equal("no-repeat", Texts("css`.a { background-repeat: no-|}`")[0]);
            Assert.Equal("smooth", Texts("css`.a { scroll-behavior: sm|}`")[0]);
            Assert.Equal("multiply", Texts("css`.a { mix-blend-mode: mul|}`")[0]);
            Assert.Equal("collapse", Texts("css`.a { border-collapse: col|}`")[0]);
        }
    }
}
