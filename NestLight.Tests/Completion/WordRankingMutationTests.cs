using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using Xunit;

namespace NestLight.Tests
{
    public class WordRankingMutationTests
    {

        [Fact]
        public void Distance_for_word_after_caret()
        {
            var m = new WordMatch(10, 5);
            Assert.Equal(5, WordText.Distance(m, 5));
        }

        [Fact]
        public void Distance_for_word_before_caret()
        {
            var m = new WordMatch(0, 5);
            Assert.Equal(5, WordText.Distance(m, 10));
        }

        [Fact]
        public void Distance_for_word_touching_caret_before()
        {
            var m = new WordMatch(0, 5);
            Assert.Equal(0, WordText.Distance(m, 5));
        }

        [Fact]
        public void Distance_for_word_starting_right_after_caret()
        {
            var m = new WordMatch(6, 3);
            Assert.Equal(1, WordText.Distance(m, 5));
        }

        // WordText.Hash: ensure different strings produce different hashes (not always, but for coverage)

        [Fact]
        public void Hash_is_consistent()
        {
            const string text = "abcdef";
            Assert.Equal(WordText.Hash(text, 0, 3), WordText.Hash(text, 0, 3));
        }

        [Fact]
        public void Hash_differs_for_different_words()
        {
            const string text = "abcxyz";
            Assert.NotEqual(WordText.Hash(text, 0, 3), WordText.Hash(text, 3, 3));
        }
        [Fact]
        public void Frequency_ranker_nearest_occurrence_wins_when_count_is_same()
        {
            string text = "alpha " + new string('.', 50) + " alpha |";
            int caret = text.IndexOf('|');
            text = text.Remove(caret, 1);
            var before = new List<WordMatch>();
            foreach (var match in System.Text.RegularExpressions.Regex.Matches(text, @"alpha"))
            {
                var m = (System.Text.RegularExpressions.Match)match;
                before.Add(new WordMatch(m.Index, m.Length));
            }
            var ranked = WordRankers.Frequency.Rank(text, caret, before, new List<WordMatch>()).ToArray();
            Assert.Single(ranked);
            Assert.True(ranked[0].Start > 10);
        }
        [Fact]
        public void Blend_throws_for_NaN()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => WordRankers.Blend(double.NaN));
            Assert.Equal("weight", ex.ParamName);
        }

        [Fact]
        public void Blend_throws_for_infinity()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => WordRankers.Blend(double.PositiveInfinity));
            Assert.Equal("weight", ex.ParamName);
        }

        [Fact]
        public void Blend_throws_for_negative()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => WordRankers.Blend(-0.1));
            Assert.Equal("weight", ex.ParamName);
        }
        [Fact]
        public void RankFollowing_sorts_by_distance_then_start()
        {
            string text = "aa bb cc";
            int caret = 4;
            var follows = new List<WordMatch>
            {
                new WordMatch(0, 2),
                new WordMatch(3, 2),
                new WordMatch(6, 2),
            };
            var result = WordRankers.Nearest.RankFollowing(text, caret, follows).ToArray();
            Assert.Equal("bb", text.Substring(result[0].Start, result[0].Length));
            Assert.Equal("aa", text.Substring(result[1].Start, result[1].Length));
            Assert.Equal("cc", text.Substring(result[2].Start, result[2].Length));
        }

        // ---- WordText.Distance boundary: m.Start >= caret (line 58) ----

        [Fact]
        public void Distance_word_starting_at_caret()
        {
            // Boundary: m.Start == caret -> m.Start > caret is false, so caret - (m.Start + m.Length) = 5 - 8 = -3
            // This means the caret is inside the word
            var m = new WordMatch(5, 3);
            Assert.Equal(-3, WordText.Distance(m, 5));
        }

        [Fact]
        public void Distance_word_just_after_caret()
        {
            // Boundary: m.Start > caret -> m.Start - caret
            var m = new WordMatch(6, 3);
            Assert.Equal(1, WordText.Distance(m, 5));
        }

        [Fact]
        public void Distance_word_ending_before_caret()
        {
            // m.Start + m.Length < caret
            var m = new WordMatch(0, 3);
            Assert.Equal(2, WordText.Distance(m, 5));
        }

        // ---- RankFollowing: nx.CompareTo(ny) vs x.Start.CompareTo(y.Start) (line 97) ----

        [Fact]
        public void RankFollowing_same_distance_uses_start_position()
        {
            // Line 97: nx != ny ? nx.CompareTo(ny) : x.Start.CompareTo(y.Start)
            string text = "aa .. bb .. cc";
            int caret = 7; // middle
            var follows = new List<WordMatch>
            {
                new WordMatch(12, 2), // cc, distance 5
                new WordMatch(0, 2),  // aa, distance 5
            };
            var result = WordRankers.Nearest.RankFollowing(text, caret, follows).ToArray();
            // Same distance (5), so sorted by Start: aa (0) before cc (12)
            Assert.Equal(0, result[0].Start);
            Assert.Equal(12, result[1].Start);
        }

        // ---- CountingWordRanker: Collect with hash collisions (line 158) ----

        [Fact]
        public void Frequency_ranker_counts_duplicates()
        {
            string text = "abc abc abc def def";
            int caret = text.Length;
            var before = new List<WordMatch>
            {
                new WordMatch(0, 3),
                new WordMatch(4, 3),
                new WordMatch(8, 3),
                new WordMatch(12, 3),
                new WordMatch(16, 3),
            };
            var ranked = WordRankers.Frequency.Rank(text, caret, before, new List<WordMatch>()).ToArray();
            // "abc" (count 3) should come before "def" (count 2)
            Assert.Equal("abc", text.Substring(ranked[0].Start, ranked[0].Length));
            Assert.Equal("def", text.Substring(ranked[1].Start, ranked[1].Length));
        }

        // ---- Nearest ranker: same distance alphabetical order (line 166) ----

        [Fact]
        public void Nearest_ranker_same_distance_alphabetical()
        {
            string text = "beta .... alpha";
            int caret = 7;
            var before = new List<WordMatch> { new WordMatch(0, 4) }; // "beta"
            var after = new List<WordMatch> { new WordMatch(10, 5) }; // "alpha"
            // beta is 3 from caret, alpha is 3 from caret
            var ranked = WordRankers.Nearest.Rank(text, caret, before, after).ToArray();
            // alphabetical: "alpha" before "beta"
            Assert.Equal("alpha", text.Substring(ranked[0].Start, ranked[0].Length));
        }

        // ---- CountingWordRanker: near update (line 166) ----

        [Fact]
        public void Frequency_ranker_nearest_occurrence_used_for_same_word()
        {
            // Line 166: if (near < c.Near) { c.Near = near; c.First = m; }
            string text = "abc ............ abc";
            int caret = text.Length;
            var before = new List<WordMatch>
            {
                new WordMatch(0, 3),
                new WordMatch(17, 3),
            };
            var ranked = WordRankers.Frequency.Rank(text, caret, before, new List<WordMatch>()).ToArray();
            Assert.Single(ranked);
            Assert.Equal(17, ranked[0].Start); // nearer occurrence
        }
    }
}
