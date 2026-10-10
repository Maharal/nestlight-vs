using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using Xunit;

namespace NestLight.Tests
{
    public class WordRankingMutationTests
    {
        // WordText.Distance: mutations at L58 (equality on Start > caret, arithmetic)

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

        // CountingWordRanker.Collect: mutation at L158 (logical), L166 (equality on near < c.Near)

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

        // BlendWordRanker: mutation at L184 (string mutation on "weight" parameter name)

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

        // NearestWordRanker.RankFollowing: mutation at L97 conditional

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
    }
}
