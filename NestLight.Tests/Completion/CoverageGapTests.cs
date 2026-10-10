using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NestLight.Completion;
using NestLight.Detection;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>Cases the coverage report showed no test reaching: a branch of the grammar of CSS, GLSL and HTML, and a tie of the rankers.</summary>
    public class CoverageGapTests
    {
        private static string Place(string codeWithCaret)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000, CompletionEngine.DefaultMinWordLength, features: new CompletionFeatures(grammar: true));
            CompletionSite site = engine.Locate(code, caret);
            Position position = CompletionLanguages.Default.Find(site.EmbeddedLanguageId).PositionAt(code, site);
            return position == null ? null : position.Name;
        }

        [Fact]
        public void An_at_rule_with_no_grammar_of_its_own_has_no_place()
        {
            Assert.Null(Place("css`@import u|`"));
            Assert.Null(Place("css`@charset \"utf-8\"; @font-face fo|`"));
        }

        [Fact]
        public void The_at_rules_that_have_a_grammar_name_their_place()
        {
            Assert.Equal("css:at-rule", Place("css`@me|`"));
            Assert.Equal("css:media-query", Place("css`@media sc|`"));
            Assert.Equal("css:keyframes-name", Place("css`@keyframes sp|`"));
            Assert.Equal("css:keyframes-name", Place("css`@-webkit-keyframes sp|`"));
        }

        [Theory]
        [InlineData("glsl`#|`", "glsl:directive")]
        [InlineData("glsl`  #def|`", "glsl:directive")]
        [InlineData("glsl`#version 300 e|`", "glsl:version")]
        [InlineData("glsl`#define FOO 1 ba|`", null)]
        [InlineData("glsl`vec3 c = ve|`", null)]
        public void A_glsl_preprocessor_line_has_a_place_for_its_directive_and_for_the_version_only(string code, string expected)
        {
            Assert.Equal(expected, Place(code));
        }

        [Fact]
        public void A_closing_tag_that_is_being_written_offers_the_open_element_or_a_tag()
        {
            Assert.Equal("html:closing-tag", Place("html`<div></di|`"));
            Assert.Equal("html:closing-tag", Place("html`<ul><li>one</li></u|`"));
            Assert.Equal("html:tag", Place("html`</di|`"));
            Assert.Equal("html:tag", Place("html`<div></div></di|`"));
        }

        [Fact]
        public void A_closed_tag_has_no_attributes_to_complete()
        {
            Assert.Null(Place("html`<div></div cl|`"));
        }

        [Fact]
        public void Words_that_tie_on_count_and_distance_keep_the_order_of_the_text()
        {
            var text = "zz | aa";
            int caret = text.IndexOf('|');
            string code = text.Remove(caret, 1);
            var before = new List<WordMatch>();
            var after = new List<WordMatch>();
            foreach (Match m in Regex.Matches(code, @"[a-z]+")) (m.Index < caret ? before : after).Add(new WordMatch(m.Index, m.Length));

            string[] ranked = WordRankers.Frequency.Rank(code, caret, before, after).Select(w => code.Substring(w.Start, w.Length)).ToArray();

            Assert.Equal(new[] { "zz", "aa" }, ranked);
        }

        [Fact]
        public void Probe_finds_the_next_character_that_is_not_a_blank_and_is_silent_when_there_is_none()
        {
            Assert.Equal('x', Probe.NextSignificant("  \n x", 0, 5));
            Assert.Equal('\0', Probe.NextSignificant("   \t\n", 0, 5));
            Assert.Equal('\0', Probe.NextSignificant("x", 1, 1));
            Assert.Equal('\0', Probe.NextSignificant("  x", 0, 2)); // the end is exclusive
        }

        [Fact]
        public void An_empty_json_object_or_array_is_json_and_a_bracket_with_nothing_after_it_is_not()
        {
            LanguageDetector detector = LanguageDetector.Create(true);
            Assert.Equal("json", detector.Detect("{          }", 0, 12));
            Assert.Equal("json", detector.Detect("[          ]", 0, 12));
            Assert.Null(detector.Detect("[          x", 0, 12));
            Assert.Null(detector.Detect("{          x", 0, 12));
        }
    }
}
