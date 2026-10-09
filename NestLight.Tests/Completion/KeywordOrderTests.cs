using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The order of the keywords and the words where no rule decides the place.</summary>
    public class KeywordOrderTests
    {
        private static List<Suggestion> Items(string codeWithCaret, CompletionFeatures features, int maxItems = 100000)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), maxItems, CompletionEngine.DefaultMinWordLength, features: features);
            return engine.Suggest(code, engine.Locate(code, caret)).ToList();
        }

        private static List<string> Texts(string codeWithCaret, CompletionFeatures features) { return Items(codeWithCaret, features).Select(s => s.Text).ToList(); }

        private static readonly CompletionFeatures WordsFirst = new CompletionFeatures(grammar: true, wordsBeforeKeywords: true);

        [Fact]
        public void The_order_of_keywords_puts_the_ones_in_the_list_first_and_the_others_alphabetically()
        {
            var features = new CompletionFeatures(keywordPriors: new Dictionary<string, IReadOnlyList<string>> { { "x", new[] { "b", "zzz", "B" } } });
            IReadOnlyList<string> ordered = features.OrderKeywords("x", new[] { "a", "b", "c" });
            Assert.Equal(new[] { "b", "a", "c" }, ordered.ToArray()); // a word that is not a keyword is ignored, a repeated one is placed once
            Assert.Equal(new[] { "a", "b", "c" }, features.OrderKeywords("y", new[] { "a", "b", "c" }).ToArray()); // another language: unchanged
            Assert.Equal(new[] { "a", "b", "c" }, CompletionFeatures.None.OrderKeywords("x", new[] { "a", "b", "c" }).ToArray());
        }

        [Fact]
        public void Where_no_rule_decides_the_words_of_the_file_come_before_the_keywords()
        {
            const string code = "glsl`void main() { float texel = 1.0; float t|`";
            List<string> without = Texts(code, new CompletionFeatures(grammar: true)), with = Texts(code, WordsFirst);
            Assert.True(without.IndexOf("texel") > without.IndexOf("tan"));
            Assert.Equal("texel", with[0]);
            Assert.Equal(without.OrderBy(w => w), with.OrderBy(w => w)); // only the order changes
        }

        [Fact]
        public void The_most_used_keywords_still_come_first_and_the_rest_after_the_words()
        {
            var priors = new Dictionary<string, IReadOnlyList<string>> { { "glsl", new[] { "texture", "tan" } } };
            const string code = "glsl`void main() { float texel = 1.0; float t|`";
            List<string> head = Texts(code, new CompletionFeatures(grammar: true, wordsBeforeKeywords: true, keywordPriors: priors, headKeywords: 1));
            Assert.Equal(new[] { "texture", "texel" }, head.Take(2).ToArray());
            Assert.True(head.IndexOf("tan") > head.IndexOf("texel"));

            List<string> none = Texts(code, new CompletionFeatures(grammar: true, wordsBeforeKeywords: true, keywordPriors: priors, headKeywords: 0));
            Assert.Equal("texel", none[0]);
            List<string> two = Texts(code, new CompletionFeatures(grammar: true, wordsBeforeKeywords: true, keywordPriors: priors, headKeywords: 2));
            Assert.Equal(new[] { "texture", "tan", "texel" }, two.Take(3).ToArray());
        }

        [Fact]
        public void In_json_true_false_and_null_still_come_first_in_a_value()
        {
            List<string> items = Texts("json`{ \"title\": 1, \"total\": 2, \"flag\": t| }`", CompletionFeatures.Default);
            Assert.Equal("true", items[0]);
        }

        [Fact]
        public void The_places_that_have_a_rule_keep_their_order()
        {
            // SQL decides by the place; the words-first order is for the places without a rule
            List<string> sql = Texts("const orders = 1;\nsql`select a from ord|`", CompletionFeatures.Default);
            Assert.Equal("orders", sql[0]);
            List<string> css = Texts("css`.a { disp| }`", CompletionFeatures.Default);
            Assert.Equal("display", css[0]);
        }

        [Fact]
        public void The_plugin_default_has_a_list_for_each_language_with_a_vocabulary()
        {
            foreach (string language in new[] { "sql", "css", "html", "svg", "graphql", "json", "yaml", "glsl", "wgsl" })
            {
                IReadOnlyList<string> list;
                Assert.True(KeywordUse.Default.TryGetValue(language, out list), language);
                Assert.NotEmpty(list);
                Assert.All(list.Take(20), w => Assert.NotNull(Vocabularies.Find(language, w))); // each one is a keyword of the language
            }
        }

        [Fact]
        public void Every_cut_of_a_shader_gives_a_list_without_errors_and_each_word_once()
        {
            const string sample = "glsl`#version 300 es\nprecision highp float;\nuniform float uTime;\nin vec2 vUv;\nout vec4 fragColor;\nvoid main() {\n  float pulse = sin(uTime);\n  fragColor = vec4(vUv, pulse, 1.0);\n}`";
            for (int cut = 5; cut <= sample.Length; cut++)
            {
                string text = sample.Substring(0, cut);
                var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), features: CompletionFeatures.Default);
                CompletionSite site = engine.Locate(text, text.Length);
                if (site == null) continue;
                var texts = engine.Suggest(text, site).Select(s => s.Text.ToLowerInvariant()).ToList();
                Assert.Equal(texts.Count, texts.Distinct().Count());
            }
        }
    }
}
