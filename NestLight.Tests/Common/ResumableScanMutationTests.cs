using System;
using NestLight.Common;
using Xunit;

namespace NestLight.Tests
{
    public class SafePointsMutationTests
    {
        // SafePoints.Reached: mutations at L78 (equality gap check) and L79 (equality _knownSafe/minStop)

        [Fact]
        public void Points_closer_than_the_gap_are_skipped()
        {
            var sp = new SafePoints(gap: 100);
            Assert.False(sp.Reached(0));
            Assert.False(sp.Reached(50));
            Assert.False(sp.Reached(99));
            Assert.False(sp.Reached(100));
            var arr = sp.ToArray();
            Assert.Equal(2, arr.Length);
            Assert.Equal(0, arr[0]);
            Assert.Equal(100, arr[1]);
        }

        [Fact]
        public void Point_at_exactly_the_gap_is_accepted()
        {
            var sp = new SafePoints(gap: 10);
            Assert.False(sp.Reached(0));
            Assert.False(sp.Reached(9));
            var arr = sp.ToArray();
            Assert.Single(arr);
            Assert.Equal(0, arr[0]);
            Assert.False(sp.Reached(10));
            arr = sp.ToArray();
            Assert.Equal(2, arr.Length);
            Assert.Equal(10, arr[1]);
        }

        [Fact]
        public void Stops_when_knownSafe_returns_true_at_or_past_minStop()
        {
            var sp = new SafePoints(minStop: 50, knownSafe: pos => pos >= 100, gap: 10);
            Assert.False(sp.Reached(0));
            Assert.False(sp.Reached(20));
            Assert.False(sp.Reached(40));
            Assert.False(sp.Reached(60));
            Assert.False(sp.Reached(80));
            Assert.True(sp.Reached(100));
            Assert.Equal(100, sp.StoppedAt);
        }

        [Fact]
        public void Does_not_stop_before_minStop_even_if_knownSafe_says_true()
        {
            var sp = new SafePoints(minStop: 200, knownSafe: pos => true, gap: 10);
            Assert.False(sp.Reached(0));
            Assert.False(sp.Reached(50));
            Assert.False(sp.Reached(100));
            Assert.False(sp.Reached(150));
            Assert.True(sp.Reached(200));
        }

        [Fact]
        public void Without_knownSafe_it_never_stops()
        {
            var sp = new SafePoints(gap: 10);
            for (int i = 0; i < 1000; i += 20)
                Assert.False(sp.Reached(i));
        }
    }

    public class TextDiffMutationTests
    {
        // TextDiff.CommonPrefix/CommonSuffix: mutations at L101 (equality), L110 (equality/arithmetic)

        [Fact]
        public void CommonPrefix_returns_zero_for_different_starts()
        {
            Assert.Equal(0, TextDiff.CommonPrefix("abc", "xyz"));
        }

        [Fact]
        public void CommonPrefix_returns_full_length_for_identical_strings()
        {
            Assert.Equal(5, TextDiff.CommonPrefix("hello", "hello"));
        }

        [Fact]
        public void CommonPrefix_handles_one_shorter_than_the_other()
        {
            Assert.Equal(3, TextDiff.CommonPrefix("abc", "abcdef"));
            Assert.Equal(3, TextDiff.CommonPrefix("abcdef", "abc"));
        }

        [Fact]
        public void CommonPrefix_handles_strings_longer_than_the_chunk_size()
        {
            string common = new string('a', 300);
            Assert.Equal(300, TextDiff.CommonPrefix(common + "x", common + "y"));
        }

        [Fact]
        public void CommonSuffix_returns_zero_for_different_ends()
        {
            Assert.Equal(0, TextDiff.CommonSuffix("abc", "xyz", 0));
        }

        [Fact]
        public void CommonSuffix_returns_full_length_for_identical_strings()
        {
            Assert.Equal(5, TextDiff.CommonSuffix("hello", "hello", 0));
        }

        [Fact]
        public void CommonSuffix_respects_skip()
        {
            Assert.Equal(3, TextDiff.CommonSuffix("XXXabc", "YYYabc", 3));
        }

        [Fact]
        public void CommonSuffix_handles_strings_longer_than_the_chunk_size()
        {
            string common = new string('z', 300);
            Assert.Equal(300, TextDiff.CommonSuffix("x" + common, "y" + common, 0));
        }
    }
}
