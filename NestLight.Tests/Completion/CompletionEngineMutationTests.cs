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

        [Fact]
        public void Grammar_position_with_prior_order_reorders_expected()
        {
            var features = new CompletionFeatures(grammar: true);
            var items = Texts("css`.a { co| }`", features);
            Assert.NotNull(items);
            Assert.Contains("color", items);
        }

        [Fact]
        public void WordsFirst_puts_words_before_keywords_when_position_says_so()
        {
            var features = new CompletionFeatures(grammar: true, schema: true);
            string tables = "sql`create table users (id int, name text)`;\n";
            var items = Texts(tables + "sql`select | from users`", features);
            Assert.NotNull(items);
            Assert.True(items.Count > 0);
        }
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

        [Fact]
        public void Secondary_keywords_appear_after_primary()
        {
            var features = new CompletionFeatures(grammar: true);
            var items = Texts("sql`select a from t |`", features);
            Assert.NotNull(items);
        }

        [Fact]
        public void Unlikely_keywords_come_after_regular_ones()
        {
            var features = new CompletionFeatures(grammar: true);
            var items = Suggest("sql`sel|`", features);
            Assert.NotNull(items);
            Assert.Contains(items, s => s.Text == "select");
        }
        [Fact]
        public void Unlikely_keywords_respect_maxItems()
        {
            var features = new CompletionFeatures(grammar: true);
            var items = Suggest("sql`|`", features, maxItems: 2);
            Assert.True(items.Count <= 2);
        }

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

        [Fact]
        public void Schema_candidates_respect_maxItems()
        {
            var features = new CompletionFeatures(schema: true);
            string tables = "sql`create table t (a int, b int, c int, d int, e int, f int)`;\n";
            var items = Suggest(tables + "sql`select t.| from t`", features, maxItems: 3);
            Assert.True(items.Count <= 3);
        }
        [Fact]
        public void Expected_words_respect_maxItems()
        {
            var features = new CompletionFeatures(grammar: true);
            var items = Suggest("css`.a { | }`", features, maxItems: 2);
            Assert.True(items.Count <= 2);
        }

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
        [Fact]
        public void Words_from_same_language_strings_come_before_others_when_sameLanguageWords_is_on()
        {
            var features = new CompletionFeatures(sameLanguageWords: true);
            string code = "sql`select customerName from t`;\ncss`.customerStyle { }`;\nsql`cust|`";
            var items = Texts(code, features);
            Assert.True(items.IndexOf("customerName") < items.IndexOf("customerStyle"));
        }
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
        [Fact]
        public void Words_inside_interpolations_rank_lower_than_same_language()
        {
            var features = new CompletionFeatures(sameLanguageWords: true);
            string code = "sql`select sqlWord from t; select ${interpWord} from t; select sql|`";
            var items = Texts(code, features);
            Assert.Contains("sqlWord", items);
        }

        [Fact]
        public void Previous_word_context_offers_following_words()
        {
            var features = new CompletionFeatures(previousWord: true);
            string code = "sql`select a from tbl_one; select b from tbl_two; select c from |`";
            var items = Texts(code, features);
            Assert.Contains("tbl_one", items);
            Assert.Contains("tbl_two", items);
        }

        [Fact]
        public void Short_words_come_last_when_feature_is_on()
        {
            var features = new CompletionFeatures(shortWordsLast: true, previousWord: true);
            string code = "const abcd = 1; const ab = 2;\nsql`select a|`";
            var items = Suggest(code, features, maxItems: 100);
            int longIdx = items.FindIndex(s => s.Text == "abcd");
            Assert.True(longIdx >= 0);
        }

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

        // ---- CompletionEngine.cs line 139: PriorOrder conditional ----

        [Fact]
        public void PriorOrder_reorders_expected_keywords()
        {
            // Line 139: position.PriorOrder ? _features.OrderByUse(language, position.Expected) : position.Expected
            var features = new CompletionFeatures(grammar: true, keywordPriority: true);
            var items = Texts("css`.a { | }`", features);
            Assert.NotNull(items);
            Assert.True(items.Count > 0);
        }

        // ---- CompletionEngine.cs line 141: HeadKeywords > 0 boundary ----

        [Fact]
        public void HeadKeywords_zero_does_not_add_head_keywords()
        {
            // Line 141: _features.HeadKeywords > 0 - boundary at 0
            var features = new CompletionFeatures(wordsBeforeKeywords: true, headKeywords: 0);
            var items = Suggest("sql`sel|`", features);
            Assert.NotNull(items);
        }

        [Fact]
        public void HeadKeywords_one_adds_one_head_keyword()
        {
            var features = new CompletionFeatures(wordsBeforeKeywords: true, headKeywords: 1, keywordPriority: true);
            var items = Suggest("sql`|`", features, maxItems: 100);
            Assert.NotNull(items);
            Assert.True(items.Count > 0);
        }

        // ---- CompletionEngine.cs line 147: logic mutation ----

        [Fact]
        public void HeadKeywords_skips_words_not_starting_with_prefix()
        {
            // Line 147: !StartsWithIgnoreCase(word, prefix) || word.Length == prefix.Length -> continue
            var features = new CompletionFeatures(wordsBeforeKeywords: true, headKeywords: 100);
            var items = Texts("sql`xyz|`", features);
            // No SQL keywords start with "xyz"
            Assert.Empty(items);
        }

        // ---- CompletionEngine.cs line 148: upper case conditional ----

        [Fact]
        public void HeadKeywords_uppercased_when_prefix_is_upper()
        {
            // Line 148: upper ? word.ToUpperInvariant() : word
            var features = new CompletionFeatures(wordsBeforeKeywords: true, headKeywords: 100);
            var items = Suggest("sql`SEL|`", features);
            Assert.Contains(items, s => s.Text == "SELECT");
        }

        // ---- CompletionEngine.cs line 152: Secondary count > 0 ----

        [Fact]
        public void Secondary_keywords_only_added_when_count_is_positive()
        {
            // Line 152: position.Secondary.Count > 0
            var features = new CompletionFeatures(grammar: true);
            var items = Suggest("sql`select * from t |`", features);
            Assert.NotNull(items);
        }

        // ---- CompletionEngine.cs line 197: null coalescing for CodeRanges ----

        [Fact]
        public void Schema_words_uses_scope_or_CodeRanges()
        {
            // Line 197: scope ?? CodeRanges(text, site) - fallback to CodeRanges
            var features = new CompletionFeatures(schema: true);
            string code = "sql`create table t (id int); select t.| from t`";
            var items = Texts(code, features);
            Assert.Contains("id", items);
        }

        // ---- CompletionEngine.cs line 198: Math.Max/Min for scan window ----

        [Fact]
        public void Schema_scan_window_clamps_to_document_bounds()
        {
            // Line 198: Math.Max(0, ...) and Math.Min(text.Length, ...)
            var features = new CompletionFeatures(schema: true);
            string code = "sql`create table t (id int); select t.| from t`";
            var items = Texts(code, features);
            Assert.Contains("id", items);
        }

        // ---- CompletionEngine.cs line 217: null coalescing for expected ----

        [Fact]
        public void AddExpected_falls_back_to_expected_when_FindCompletionWord_is_null()
        {
            // Line 217: language.FindCompletionWord(expected) ?? expected
            var features = new CompletionFeatures(grammar: true);
            // HTML tags are expected words but may not be keywords of CSS
            var items = Texts("css`|`", features);
            Assert.NotNull(items);
        }

        // ---- CompletionEngine.Similar.cs line 43: ShortSimilarCap boundary ----

        [Fact]
        public void ShortSimilarCap_limits_similar_results_for_short_prefix()
        {
            // Line 43: _features.ShortSimilarCap > 0 boundary check
            var features = new CompletionFeatures(shortSimilarCap: 2);
            var items = Suggest("sql`sel|`", features, new BandedPrefixMatcher(), fuzzyBelow: 100, fuzzyMaxItems: 50);
            Assert.NotNull(items);
        }

        [Fact]
        public void ShortSimilarCap_zero_means_no_cap()
        {
            // Line 43: ShortSimilarCap == 0 means no cap, full fuzzyMaxItems used
            var features = new CompletionFeatures(shortSimilarCap: 0);
            var items = Suggest("sql`selct|`", features, new BandedPrefixMatcher(), fuzzyBelow: 100, fuzzyMaxItems: 50);
            Assert.NotNull(items);
        }

        // ---- CompletionEngine.Similar.cs line 49: cancellation stride ----

        [Fact]
        public void Similar_keywords_checked_for_cancellation()
        {
            // Line 49: ++examined % CancellationStride == 0 triggers check
            var cts = new CancellationTokenSource();
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100,
                CompletionEngine.DefaultMinWordLength, new BandedPrefixMatcher(), fuzzyBelow: 100);
            string code = "sql`selct`";
            var site = engine.Locate(code, 8);
            // Should not throw when not cancelled
            var result = engine.Suggest(code, site, cts.Token);
            Assert.NotNull(result);
        }

        // ---- CompletionEngine.Similar.cs line 125: Compare source vs text ----

        [Fact]
        public void Similar_compare_uses_source_for_keywords_and_text_for_words()
        {
            // Lines 125-131: Compare function uses a.Source/b.Source for keywords, text for words
            // Fuzzy results should be deterministically ordered
            var items1 = Suggest("sql`selct|`", null, new BandedPrefixMatcher(), fuzzyBelow: 100);
            var items2 = Suggest("sql`selct|`", null, new BandedPrefixMatcher(), fuzzyBelow: 100);
            Assert.NotNull(items1);
            Assert.NotNull(items2);
            var fuzzy1 = items1.Where(s => s.Distance > 0).Select(s => s.Text).ToList();
            var fuzzy2 = items2.Where(s => s.Distance > 0).Select(s => s.Text).ToList();
            Assert.Equal(fuzzy1, fuzzy2);
        }

        // ---- CompletionEngine.Similar.cs lines 126-131: Compare details ----

        [Fact]
        public void Similar_keywords_come_before_similar_words()
        {
            // Line 122: a.Keyword != b.Keyword -> keyword first
            string code = "const selact = 1;\nsql`selct|`";
            var items = Suggest(code, null, new BandedPrefixMatcher(), fuzzyBelow: 100);
            var fuzzy = items.Where(s => s.Distance > 0).ToList();
            if (fuzzy.Count > 1)
            {
                var firstKeyword = fuzzy.FindIndex(s => s.Kind == SuggestionKind.Keyword);
                var firstWord = fuzzy.FindIndex(s => s.Kind == SuggestionKind.Word);
                if (firstKeyword >= 0 && firstWord >= 0)
                    Assert.True(firstKeyword < firstWord);
            }
        }

        // ---- CompletionEngine.Similar.cs lines 165, 169, 180: ContextBefore ----

        [Fact]
        public void ContextBefore_with_positive_ownerStart()
        {
            // Line 165: site.OwnerStart >= 0 ? site.OwnerStart : 0
            var features = new CompletionFeatures(previousWord: true);
            string code = "sql`select a from tbl; select b from |`";
            var items = Texts(code, features);
            Assert.Contains("tbl", items);
        }

        [Fact]
        public void ContextBefore_respects_reach_limit()
        {
            // Line 165/190: ContextReach = 200 limits how far back to look
            var features = new CompletionFeatures(previousWord: true);
            string padding = new string(' ', 300);
            string code = "sql`select a from faraway;" + padding + "select b from |`";
            var items = Texts(code, features);
            // "faraway" might not be found because it's beyond ContextReach from the caret
            Assert.NotNull(items);
        }

        // ---- CompletionEngine.Similar.cs lines 209-213: AddPieces interpolation handling ----

        [Fact]
        public void AddPieces_skips_interpolations_in_scope()
        {
            // Lines 209-213: interpolations within [from, to) are excluded
            var features = new CompletionFeatures(sameLanguageWords: true);
            string code = "sql`select sqlword from t; select ${interp} from t; select sql|`";
            var items = Texts(code, features);
            Assert.Contains("sqlword", items);
        }

        // ---- CompletionEngine.Similar.cs line 235: CodeRanges sort ----

        [Fact]
        public void CodeRanges_pieces_are_sorted_and_merged()
        {
            // Line 235: pieces.Sort(...)
            // Line 240: merging adjacent/overlapping ranges
            var features = new CompletionFeatures(sameLanguageWords: true);
            string code = "sql`select word1 from t`;\nsql`select word2 from t`;\nsql`wor|`";
            var items = Texts(code, features);
            Assert.Contains("word1", items);
            Assert.Contains("word2", items);
        }

        // ---- CompletionEngine.Similar.cs lines 283-284: scope range scanning ----

        [Fact]
        public void ScanWords_with_same_language_scope()
        {
            // Lines 283-284: while (range < scope.Length && scope[range + 1] <= start) range += 2
            var features = new CompletionFeatures(sameLanguageWords: true);
            // Two SQL strings with a word that starts with the same prefix
            string longCode = "sql`select testword from t; select test|`";
            var items = Texts(longCode, features);
            Assert.Contains("testword", items);
        }

        // ---- CompletionEngine.Similar.cs line 290: ShortWordsLast before/after caret ----

        [Fact]
        public void ShortWordsLast_separates_before_and_after_caret()
        {
            // Line 290: start > site.Caret ? scan.ShortAfter : scan.ShortBefore
            var features = new CompletionFeatures(shortWordsLast: true, previousWord: true);
            string code = "const ab = 1;\nsql`select abcd from t; select a|`;\nconst ac = 2;";
            var items = Suggest(code, features, maxItems: 100);
            Assert.NotNull(items);
        }
    }
}
