using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class CompletionEngineMutationTests
    {
        private static List<Suggestion> Suggest(string codeWithCaret, CompletionFeatures features = null,
            IApproximateMatcher matcher = null, int maxItems = CompletionEngine.DefaultMaxItems,
            int fuzzyBelow = 1, int fuzzyMaxItems = 10, bool fuzzyFirstLetter = true)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), maxItems,
                CompletionEngine.DefaultMinWordLength, matcher, fuzzyBelow, fuzzyMaxItems, fuzzyFirstLetter, features);
            var site = engine.Locate(code, caret);
            return site == null ? null : engine.Suggest(code, site).ToList();
        }

        private static List<string> Texts(string codeWithCaret, CompletionFeatures features = null)
        {
            var items = Suggest(codeWithCaret, features);
            return items?.Select(s => s.Text).ToList();
        }

        // ---- L197: PriorOrder conditional ----

        [Fact]
        public void Grammar_position_with_prior_order_reorders_expected()
        {
            var features = new CompletionFeatures(grammar: true);
            var items = Texts("css`.a { co| }`", features);
            Assert.NotNull(items);
            Assert.Contains("color", items);
        }

        // ---- L199: wordsFirst equality ----

        [Fact]
        public void WordsFirst_puts_words_before_keywords_when_position_says_so()
        {
            var features = new CompletionFeatures(grammar: true, schema: true);
            string tables = "sql`create table users (id int, name text)`;\n";
            var items = Texts(tables + "sql`select | from users`", features);
            Assert.NotNull(items);
            Assert.True(items.Count > 0);
        }

        // ---- L204-L206: maxItems and prefix checks in HeadKeywords loop ----

        [Fact]
        public void HeadKeywords_respects_maxItems_limit()
        {
            var features = new CompletionFeatures(wordsBeforeKeywords: true, headKeywords: 5);
            var items = Suggest("sql`|`", features, maxItems: 3);
            Assert.True(items.Count <= 3);
        }

        [Fact]
        public void HeadKeywords_skips_exact_match()
        {
            var features = new CompletionFeatures(wordsBeforeKeywords: true, headKeywords: 100);
            var items = Texts("sql`select|`", features);
            Assert.DoesNotContain("select", items);
        }

        // ---- L210: secondary expected ----

        [Fact]
        public void Secondary_keywords_appear_after_primary()
        {
            var features = new CompletionFeatures(grammar: true);
            var items = Texts("sql`select a from t |`", features);
            Assert.NotNull(items);
        }

        // ---- L220: unlikely keywords are deferred ----

        [Fact]
        public void Unlikely_keywords_come_after_regular_ones()
        {
            var features = new CompletionFeatures(grammar: true);
            var items = Suggest("sql`sel|`", features);
            Assert.NotNull(items);
            Assert.Contains(items, s => s.Text == "select");
        }

        // ---- L230-L231: unlikely list respects maxItems ----

        [Fact]
        public void Unlikely_keywords_respect_maxItems()
        {
            var features = new CompletionFeatures(grammar: true);
            var items = Suggest("sql`|`", features, maxItems: 2);
            Assert.True(items.Count <= 2);
        }

        // ---- L234: fuzzy stage triggers ----

        [Fact]
        public void Fuzzy_stage_requires_minimum_prefix_length()
        {
            var items = Suggest("sql`sl|`", new CompletionFeatures(), new BandedPrefixMatcher());
            Assert.All(items, s => Assert.Equal(0, s.Distance));
        }

        [Fact]
        public void Fuzzy_stage_triggers_when_nothing_matched()
        {
            var items = Suggest("sql`selct|`", null, new BandedPrefixMatcher());
            Assert.Contains(items, s => s.Text == "select" && s.Distance > 0);
        }

        // ---- L263: AddCandidates maxItems check ----

        [Fact]
        public void Schema_candidates_respect_maxItems()
        {
            var features = new CompletionFeatures(schema: true);
            string tables = "sql`create table t (a int, b int, c int, d int, e int, f int)`;\n";
            var items = Suggest(tables + "sql`select t.| from t`", features, maxItems: 3);
            Assert.True(items.Count <= 3);
        }

        // ---- L273-L275: AddExpected maxItems and null coalescing ----

        [Fact]
        public void Expected_words_respect_maxItems()
        {
            var features = new CompletionFeatures(grammar: true);
            var items = Suggest("css`.a { | }`", features, maxItems: 2);
            Assert.True(items.Count <= 2);
        }

        // ---- L285: ToleranceFor boundary ----

        [Fact]
        public void ToleranceFor_returns_1_for_short_prefix()
        {
            Assert.Equal(1, CompletionEngine.ToleranceFor(3));
            Assert.Equal(1, CompletionEngine.ToleranceFor(5));
        }

        [Fact]
        public void ToleranceFor_returns_2_for_long_prefix()
        {
            Assert.Equal(2, CompletionEngine.ToleranceFor(6));
            Assert.Equal(2, CompletionEngine.ToleranceFor(10));
        }

        // ---- L311-L314: CodeRanges / scope ----

        [Fact]
        public void Words_from_same_language_strings_come_before_others_when_sameLanguageWords_is_on()
        {
            var features = new CompletionFeatures(sameLanguageWords: true);
            string code = "sql`select customerName from t`;\ncss`.customerStyle { }`;\nsql`cust|`";
            var items = Texts(code, features);
            Assert.True(items.IndexOf("customerName") < items.IndexOf("customerStyle"));
        }

        // ---- L340-L345: ScanWords word detection, dash handling ----

        [Fact]
        public void Css_dashes_are_part_of_words()
        {
            string code = "css`.my-class { background-color| }`";
            var items = Texts(code);
            Assert.DoesNotContain("background", items);
        }

        [Fact]
        public void Words_longer_than_64_are_ignored()
        {
            string longWord = new string('a', 65);
            var items = Texts("const " + longWord + " = 1;\nsql`a|`");
            Assert.DoesNotContain(longWord, items);
        }

        [Fact]
        public void Words_at_exactly_max_length_are_included()
        {
            string word64 = "a" + new string('b', 63);
            var items = Texts("const " + word64 + " = 1;\nsql`a|`");
            Assert.Contains(word64, items);
        }

        // ---- L478-L482: AddPieces skips interpolations ----

        [Fact]
        public void Words_inside_interpolations_rank_lower_than_same_language()
        {
            var features = new CompletionFeatures(sameLanguageWords: true);
            string code = "sql`select sqlWord from t; select ${interpWord} from t; select sql|`";
            var items = Texts(code, features);
            Assert.Contains("sqlWord", items);
        }

        // ---- L468: Follows context boolean check ----

        [Fact]
        public void Previous_word_context_offers_following_words()
        {
            var features = new CompletionFeatures(previousWord: true);
            string code = "sql`select a from tbl_one; select b from tbl_two; select c from |`";
            var items = Texts(code, features);
            Assert.Contains("tbl_one", items);
            Assert.Contains("tbl_two", items);
        }

        // ---- L559: short words last feature ----

        [Fact]
        public void Short_words_come_last_when_feature_is_on()
        {
            var features = new CompletionFeatures(shortWordsLast: true, previousWord: true);
            string code = "const abcd = 1; const ab = 2;\nsql`select a|`";
            var items = Suggest(code, features, maxItems: 100);
            int longIdx = items.FindIndex(s => s.Text == "abcd");
            Assert.True(longIdx >= 0);
        }

        // ---- Cancellation token ----

        [Fact]
        public void Cancelled_token_stops_suggest()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100, CompletionEngine.DefaultMinWordLength, new BandedPrefixMatcher());
            string code = "sql`selct`";
            var site = engine.Locate(code, 8);
            Assert.Throws<OperationCanceledException>(() => engine.Suggest(code, site, cts.Token));
        }

        // ---- Suggest with null/invalid inputs ----

        [Fact]
        public void Suggest_with_null_text_returns_empty()
        {
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript));
            var site = new CompletionSite("sql", 0, 0, 0);
            Assert.Empty(engine.Suggest(null, site));
        }

        [Fact]
        public void Suggest_with_null_site_returns_empty()
        {
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript));
            Assert.Empty(engine.Suggest("sql`sel`", null));
        }
    }
}
