using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The ways of ordering the words of the document (<see cref="IWordRanker"/>), on their own and behind the engine.</summary>
    public class WordRankerTests
    {
        /// <summary>A text with a caret at <c>|</c>: every word of it, split into the ones before and the ones after the caret, as the engine's scan hands them over.</summary>
        private sealed class Scene
        {
            public string Text;
            public int Caret;
            public List<WordMatch> Before = new List<WordMatch>(), After = new List<WordMatch>();

            public Scene(string codeWithCaret)
            {
                Caret = codeWithCaret.IndexOf('|');
                Text = codeWithCaret.Remove(Caret, 1);
                foreach (Match m in Regex.Matches(Text, @"[A-Za-z_]\w*"))
                    (m.Index < Caret ? Before : After).Add(new WordMatch(m.Index, m.Length));
            }

            public string Word(WordMatch m) { return Text.Substring(m.Start, m.Length); }

            public string[] Rank(IWordRanker ranker)
            {
                return ranker.Rank(Text, Caret, Before, After).Select(Word).ToArray();
            }

            public string[] Distinct(IWordRanker ranker) { return Rank(ranker).Distinct().ToArray(); }
        }

        // ---- nearest ------------------------------------------------------------------------------------------------------

        [Fact]
        public void The_nearest_ranker_merges_the_two_sides_by_distance()
        {
            var scene = new Scene("alpha beta |gamma delta");
            // gamma touches the caret, beta is one blank away, then alpha and delta are as far from it (alphabetical)
            Assert.Equal(new[] { "gamma", "beta", "alpha", "delta" }, scene.Rank(WordRankers.Nearest));
        }

        [Fact]
        public void The_same_distance_on_both_sides_puts_the_alphabetically_first_word_first()
        {
            Assert.Equal(new[] { "aa", "zz" }, new Scene("zz | aa").Rank(WordRankers.Nearest));
            Assert.Equal(new[] { "aa", "zz" }, new Scene("aa | zz").Rank(WordRankers.Nearest));
        }

        [Fact]
        public void The_nearest_ranker_offers_every_occurrence_and_leaves_the_repeated_ones_to_the_engine()
        {
            var scene = new Scene("one two one |two");
            Assert.Equal(new[] { "two", "one", "two", "one" }, scene.Rank(WordRankers.Nearest));
        }

        [Fact]
        public void The_nearest_ranker_handles_an_empty_side()
        {
            Assert.Equal(new[] { "beta", "alpha" }, new Scene("alpha beta |").Rank(WordRankers.Nearest));
            Assert.Equal(new[] { "alpha", "beta" }, new Scene("| alpha beta").Rank(WordRankers.Nearest));
            Assert.Empty(new Scene("|").Rank(WordRankers.Nearest));
        }

        [Fact]
        public void The_words_that_followed_the_context_come_nearest_first_and_an_equal_distance_goes_to_the_earlier()
        {
            var scene = new Scene("far near |");
            var follows = new List<WordMatch>(scene.Before);
            Assert.Equal(new[] { "near", "far" }, WordRankers.Nearest.RankFollowing(scene.Text, scene.Caret, follows).Select(scene.Word).ToArray());

            var tied = new Scene("aa | bb");
            var both = new List<WordMatch>(tied.Before.Concat(tied.After));
            Assert.Equal(new[] { "aa", "bb" }, WordRankers.Nearest.RankFollowing(tied.Text, tied.Caret, both).Select(tied.Word).ToArray());
        }

        // ---- frequency ----------------------------------------------------------------------------------------------------

        [Fact]
        public void The_frequency_ranker_puts_the_most_used_first_and_then_the_nearer_one()
        {
            var scene = new Scene("aa bb bb cc cc cc |dd");
            // cc three times, bb twice, and aa and dd once: dd touches the caret
            Assert.Equal(new[] { "cc", "bb", "dd", "aa" }, scene.Rank(WordRankers.Frequency));
        }

        [Fact]
        public void The_ranked_order_offers_each_word_once_with_its_nearest_occurrence()
        {
            var scene = new Scene("aa bb aa |cc aa");
            WordMatch[] ranked = WordRankers.Frequency.Rank(scene.Text, scene.Caret, scene.Before, scene.After).ToArray();
            Assert.Equal(new[] { "aa", "cc", "bb" }, ranked.Select(scene.Word).ToArray());
            Assert.Equal(scene.Text.IndexOf("cc"), ranked[1].Start);
            // the nearest of the three aa to the caret is the one right before it
            Assert.Equal(6, ranked[0].Start);
        }

        [Fact]
        public void The_case_of_a_word_matters_when_the_occurrences_are_counted()
        {
            var scene = new Scene("Total total total Total |x");
            string[] words = scene.Distinct(WordRankers.Frequency);
            Assert.Equal(3, words.Length);
            Assert.Contains("Total", words);
            Assert.Contains("total", words);
        }

        [Fact]
        public void The_frequency_ranker_does_not_confuse_two_words_with_the_same_hash_and_length()
        {
            // not a proof of a collision, but the comparison after the hash is what keeps two different words apart: many near words of one length
            string words = string.Join(" ", Enumerable.Range(0, 2000).Select(i => "w" + i.ToString("D5")));
            var scene = new Scene(words + " |");
            Assert.Equal(2000, scene.Distinct(WordRankers.Frequency).Length);
        }

        [Fact]
        public void The_words_that_followed_the_context_are_counted_too()
        {
            var scene = new Scene("rare usual usual usual rare2 |");
            var follows = new List<WordMatch>(scene.Before);
            Assert.Equal("usual", WordRankers.Frequency.RankFollowing(scene.Text, scene.Caret, follows).Select(scene.Word).First());
            Assert.Equal("rare2", WordRankers.Nearest.RankFollowing(scene.Text, scene.Caret, new List<WordMatch>(scene.Before)).Select(scene.Word).First());
        }

        // ---- blend --------------------------------------------------------------------------------------------------------

        [Fact]
        public void The_blend_goes_from_the_count_alone_to_the_distance_alone_as_the_weight_grows()
        {
            var scene = new Scene("cc cc cc " + new string('.', 60) + " bb |");
            Assert.Equal(new[] { "cc", "bb" }, scene.Distinct(WordRankers.Blend(0)));
            Assert.Equal(new[] { "cc", "bb" }, scene.Distinct(WordRankers.Frequency)); // a weight of zero is the frequency
            Assert.Equal(new[] { "bb", "cc" }, scene.Distinct(WordRankers.Blend(5)));
        }

        [Theory]
        [InlineData(-1.0)]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        public void The_weight_of_the_blend_has_to_be_a_finite_number_that_is_not_negative(double weight)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => WordRankers.Blend(weight));
        }

        [Fact]
        public void A_blend_with_a_weight_keeps_it()
        {
            Assert.Equal(0.25, ((BlendWordRanker)WordRankers.Blend(0.25)).Weight);
        }

        // ---- the features ----------------------------------------------------------------------------------------------------

        [Fact]
        public void Without_a_ranker_the_features_rank_by_distance()
        {
            Assert.Same(WordRankers.Nearest, CompletionFeatures.None.Ranker);
            Assert.Same(WordRankers.Nearest, CompletionFeatures.Default.Ranker);
            Assert.Same(WordRankers.Frequency, new CompletionFeatures(ranker: WordRankers.Frequency).Ranker);
        }

        // ---- behind the engine -----------------------------------------------------------------------------------------------

        /// <summary>A ranker the engine has never heard of: the farthest first, and every occurrence offered twice.</summary>
        private sealed class FarthestFirst : IWordRanker
        {
            public int Calls;
            public int FollowingCalls;

            public IEnumerable<WordMatch> Rank(string text, int caret, List<WordMatch> before, List<WordMatch> after)
            {
                Calls++;
                foreach (WordMatch m in before.Concat(after).OrderByDescending(x => WordText.Distance(x, caret)))
                {
                    yield return m;
                    yield return m;
                }
            }

            public IEnumerable<WordMatch> RankFollowing(string text, int caret, List<WordMatch> follows)
            {
                FollowingCalls++;
                return follows.OrderByDescending(x => WordText.Distance(x, caret));
            }
        }

        private static List<string> Words(string codeWithCaret, CompletionFeatures features)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), CompletionEngine.DefaultMaxItems, CompletionEngine.DefaultMinWordLength, features: features);
            return engine.Suggest(code, engine.Locate(code, caret)).Where(s => s.Kind == SuggestionKind.Word).Select(s => s.Text).ToList();
        }

        [Fact]
        public void A_new_ranker_orders_the_words_without_the_engine_knowing_it()
        {
            const string code = "const usersFar = 1;\nconst usersNear = 1;\nsql`select us|`";
            Assert.Equal(new[] { "usersNear", "usersFar" }, Words(code, CompletionFeatures.None));

            var custom = new FarthestFirst();
            // a word the ranker offers twice is offered once
            Assert.Equal(new[] { "usersFar", "usersNear" }, Words(code, new CompletionFeatures(ranker: custom)));
            Assert.True(custom.Calls >= 1);
        }

        [Fact]
        public void The_ranker_orders_the_words_that_followed_the_context_too()
        {
            const string code = "sql`select a from tbl_old; select b from tbl_new; select c from |`";
            var custom = new FarthestFirst();
            Assert.Equal(new[] { "tbl_old", "tbl_new" }, Words(code, new CompletionFeatures(previousWord: true, ranker: custom)).Take(2).ToArray());
            Assert.True(custom.FollowingCalls >= 1);
            Assert.Equal(new[] { "tbl_new", "tbl_old" }, Words(code, new CompletionFeatures(previousWord: true)).Take(2).ToArray());
        }

        [Fact]
        public void The_words_of_the_language_still_come_before_the_others_with_a_custom_ranker()
        {
            const string code = "sql`select usersList from t`;\nconst usersAdmin = 1;\nsql`select us|`";
            var features = new CompletionFeatures(sameLanguageWords: true, ranker: new FarthestFirst());
            Assert.Equal(new[] { "usersList", "usersAdmin" }, Words(code, features).Take(2).ToArray());
        }

        [Fact]
        public void The_ranker_changes_the_order_and_never_the_set_of_suggestions()
        {
            const string code = "const usersList = 1; usersList; usersList;\nconst usersAdmin = 1;\nsql`select us|`";
            var expected = Words(code, CompletionFeatures.None).OrderBy(w => w).ToList();
            foreach (IWordRanker ranker in new IWordRanker[] { WordRankers.Nearest, WordRankers.Frequency, WordRankers.Blend(0.5), WordRankers.Blend(3), new FarthestFirst() })
                Assert.Equal(expected, Words(code, new CompletionFeatures(ranker: ranker)).OrderBy(w => w).ToList());
        }
    }
}
