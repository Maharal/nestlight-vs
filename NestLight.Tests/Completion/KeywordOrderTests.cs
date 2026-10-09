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
        private static List<Suggestion> Items(string codeWithCaret, CompletionFeatures features, int maxItems = 100000, ICompletionLanguages languages = null)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), maxItems, CompletionEngine.DefaultMinWordLength, features: features, languages: languages);
            return engine.Suggest(code, engine.Locate(code, caret)).ToList();
        }

        private static List<string> Texts(string codeWithCaret, CompletionFeatures features, ICompletionLanguages languages = null) { return Items(codeWithCaret, features, languages: languages).Select(s => s.Text).ToList(); }

        private static readonly CompletionFeatures WordsFirst = new CompletionFeatures(grammar: true, wordsBeforeKeywords: true);

        /// <summary>A language that says for itself which of its words are used most.</summary>
        private sealed class UsedLanguage : CompletionLanguage
        {
            private readonly IReadOnlyList<string> _use;
            public UsedLanguage(string id, string[] keywords, string[] use, string[] extraWords = null) : base(new[] { id }, keywords, extraWords) { _use = use; }
            protected override IReadOnlyList<string> UseOrder { get { return _use; } }
        }

        [Fact]
        public void The_order_of_keywords_puts_the_ones_in_the_list_first_and_the_others_alphabetically()
        {
            var language = new UsedLanguage("x", new[] { "a", "b", "c" }, new[] { "b", "zzz", "B" });
            Assert.Equal(new[] { "b", "a", "c" }, language.CompletionWordsByUse.ToArray()); // a word that is not a keyword is ignored, a repeated one is placed once
            Assert.Equal(new[] { "a", "b", "c" }, language.CompletionWords.ToArray());      // the alphabetical list is still there

            var other = new UsedLanguage("y", new[] { "a", "b", "c" }, new string[0]);
            Assert.Equal(new[] { "a", "b", "c" }, other.CompletionWordsByUse.ToArray());    // a language without a list: unchanged
            Assert.Same(other.CompletionWords, other.CompletionWordsByUse);
            Assert.Same(language.CompletionWordsByUse, language.CompletionWordsByUse);       // computed once
        }

        [Fact]
        public void The_words_that_are_offered_but_not_colored_take_part_in_the_order_by_use()
        {
            var language = new UsedLanguage("x", new[] { "a", "c" }, new[] { "main", "c" }, extraWords: new[] { "main" });
            Assert.Equal(new[] { "main", "c", "a" }, language.CompletionWordsByUse.ToArray());
        }

        [Fact]
        public void A_list_of_the_grammar_is_ordered_by_the_use_of_its_language_and_the_order_is_cached_by_list()
        {
            var language = new UsedLanguage("x", new[] { "a" }, new[] { "z", "m", "missing" });
            IReadOnlyList<string> properties = new[] { "a", "m", "z" };
            IReadOnlyList<string> ordered = language.OrderByUse(properties);
            Assert.Equal(new[] { "z", "m", "a" }, ordered.ToArray());
            Assert.Same(ordered, language.OrderByUse(properties));
            Assert.Equal(new[] { "a", "m", "z" }, properties.ToArray()); // the list that came in is not touched

            var none = new UsedLanguage("y", new[] { "a" }, new string[0]);
            Assert.Same(properties, none.OrderByUse(properties));
            Assert.Throws<System.ArgumentNullException>(() => language.OrderByUse(null));
        }

        [Fact]
        public void The_features_decide_whether_the_use_order_applies_not_which_order_it_is()
        {
            var language = new UsedLanguage("x", new[] { "a", "b", "c" }, new[] { "c" });
            Assert.Equal(new[] { "c", "a", "b" }, new CompletionFeatures(keywordPriority: true).OrderKeywords(language).ToArray());
            Assert.Equal(new[] { "a", "b", "c" }, CompletionFeatures.None.OrderKeywords(language).ToArray());
            IReadOnlyList<string> properties = new[] { "a", "c" };
            Assert.Equal(new[] { "c", "a" }, new CompletionFeatures(keywordPriority: true).OrderByUse(language, properties).ToArray());
            Assert.Same(properties, CompletionFeatures.None.OrderByUse(language, properties));
        }

        [Fact]
        public void A_language_reads_its_order_under_any_of_its_ids_so_an_alias_needs_no_entry()
        {
            // "htm" and "svg" are not keys of KeywordUse.Default: the html language finds its list under "html"
            Assert.False(KeywordUse.Default.ContainsKey("htm"));
            Assert.False(KeywordUse.Default.ContainsKey("svg"));
            IReadOnlyList<string> html = CompletionLanguages.Default.Find("html").CompletionWordsByUse;
            Assert.Same(html, CompletionLanguages.Default.Find("htm").CompletionWordsByUse);
            Assert.Same(html, CompletionLanguages.Default.Find("svg").CompletionWordsByUse);
            Assert.Equal(KeywordUse.Default["html"][0], html[0]);
            // yml is the alias of yaml
            Assert.Equal("true", CompletionLanguages.Default.Find("yml").CompletionWordsByUse[0]);
        }

        [Fact]
        public void A_language_without_a_list_offers_its_words_alphabetically_even_with_the_priority_on()
        {
            foreach (string id in new[] { "xml", "markdown", "regex" })
            {
                ICompletionLanguage language = CompletionLanguages.Default.Find(id);
                Assert.Same(language.CompletionWords, language.CompletionWordsByUse);
            }
            Assert.Empty(CompletionLanguages.Default.Find("unknown-language").CompletionWordsByUse);
        }

        [Fact]
        public void An_order_learned_elsewhere_replaces_the_one_of_the_instance_only()
        {
            ICompletionLanguage shipped = CompletionLanguages.Default.Find("json");
            ICompletionLanguage copy = null;
            foreach (ICompletionLanguage l in CompletionLanguages.CreateStandard()) if (l.Ids.Contains("json")) copy = l;
            ((CompletionLanguage)copy).LearnUseOrder(new[] { "null", "false" });
            Assert.Equal(new[] { "null", "false", "true" }, copy.CompletionWordsByUse.ToArray());
            Assert.Equal(new[] { "true", "false", "null" }, shipped.CompletionWordsByUse.ToArray()); // the shared one is untouched
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
            // the glsl of the plugin, with an order by use of its own
            var glsl = CompletionLanguages.CreateStandard().Single(l => l.Ids.Contains("glsl"));
            ((CompletionLanguage)glsl).LearnUseOrder(new[] { "texture", "tan" });
            var languages = new CompletionLanguages(CompletionLanguages.CreateStandard().Where(l => !l.Ids.Contains("glsl")).Concat(new[] { glsl }));

            const string code = "glsl`void main() { float texel = 1.0; float t|`";
            List<string> head = Texts(code, new CompletionFeatures(grammar: true, wordsBeforeKeywords: true, keywordPriority: true, headKeywords: 1), languages);
            Assert.Equal(new[] { "texture", "texel" }, head.Take(2).ToArray());
            Assert.True(head.IndexOf("tan") > head.IndexOf("texel"));

            List<string> none = Texts(code, new CompletionFeatures(grammar: true, wordsBeforeKeywords: true, keywordPriority: true, headKeywords: 0), languages);
            Assert.Equal("texel", none[0]);
            List<string> two = Texts(code, new CompletionFeatures(grammar: true, wordsBeforeKeywords: true, keywordPriority: true, headKeywords: 2), languages);
            Assert.Equal(new[] { "texture", "tan", "texel" }, two.Take(3).ToArray());

            // with the priority off the order of the language is not used at all
            List<string> off = Texts(code, new CompletionFeatures(grammar: true, wordsBeforeKeywords: true, headKeywords: 2), languages);
            Assert.True(off.IndexOf("tan") < off.IndexOf("texture"));
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
            foreach (string id in new[] { "sql", "css", "html", "svg", "graphql", "json", "yaml", "glsl", "wgsl" })
            {
                ICompletionLanguage language = CompletionLanguages.Default.Find(id);
                IReadOnlyList<string> byUse = language.CompletionWordsByUse;
                Assert.NotSame(language.CompletionWords, byUse);
                // the same words (a word spelled in several cases is one word here, as it always was)
                Assert.Equal(language.CompletionWords.Select(w => w.ToLowerInvariant()).Distinct().OrderBy(w => w, System.StringComparer.Ordinal), byUse.Select(w => w.ToLowerInvariant()).Distinct().OrderBy(w => w, System.StringComparer.Ordinal));
                Assert.Equal(byUse.Count, byUse.Distinct(System.StringComparer.OrdinalIgnoreCase).Count());
                Assert.All(KeywordUse.Default[id == "svg" ? "html" : id].Take(20), w => Assert.NotNull(language.FindKeyword(w))); // each one is a keyword of the language
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

        // ---- words of two letters ---------------------------------------------------------------------------------------

        [Fact]
        public void Words_of_two_letters_are_offered_after_all_the_longer_ones()
        {
            const string code = "const idle = 1, ident = 2;\nsql`select id, ok from t where id = 1 and i|`";
            var shortLast = new CompletionFeatures(grammar: true, shortWordsLast: true);
            List<string> without = Texts(code, new CompletionFeatures(grammar: true)), with = Texts(code, shortLast);
            Assert.DoesNotContain("id", without);
            Assert.True(with.IndexOf("id") > with.IndexOf("ident"));
            Assert.True(with.IndexOf("id") > with.IndexOf("idle"));
            Assert.Equal(without.OrderBy(w => w).Concat(new[] { "id" }).OrderBy(w => w), with.OrderBy(w => w)); // only the two-letter words are added
        }

        [Fact]
        public void A_word_of_two_letters_is_not_offered_when_it_is_the_whole_prefix_and_one_letter_words_never_are()
        {
            var shortLast = new CompletionFeatures(grammar: true, shortWordsLast: true);
            Assert.DoesNotContain("id", Texts("sql`select id from t where id|`", shortLast)); // nothing to add to what was typed
            Assert.DoesNotContain(Texts("sql`select a, b from t where x = 1 and |`", shortLast), w => w.Length == 1);
        }

        [Fact]
        public void The_plugin_default_offers_the_two_letter_words()
        {
            Assert.Contains("id", Texts("const idle = 1;\nsql`select p.id, p.name from products p where p.i|`", CompletionFeatures.Default));
        }
    }
}
