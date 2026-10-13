using NestLight.Common;
using Xunit;

namespace NestLight.Tests
{
    public class TextUtilMutationTests
    {

        [Fact]
        public void StartsWithIgnoreCase_returns_false_when_index_is_negative()
        {
            var m = "Hello".ToCharArray();
            Assert.False(TextUtil.StartsWithIgnoreCase(m, -1, 5, "hel"));
        }

        [Fact]
        public void StartsWithIgnoreCase_returns_false_when_string_extends_past_end()
        {
            var m = "Hello".ToCharArray();
            Assert.False(TextUtil.StartsWithIgnoreCase(m, 3, 5, "lo!"));
        }

        [Fact]
        public void StartsWithIgnoreCase_returns_true_when_exact_fit_at_end()
        {
            var m = "Hello".ToCharArray();
            Assert.True(TextUtil.StartsWithIgnoreCase(m, 3, 5, "lo"));
        }

        [Fact]
        public void StartsWithIgnoreCase_returns_false_on_mismatch()
        {
            var m = "Hello".ToCharArray();
            Assert.False(TextUtil.StartsWithIgnoreCase(m, 0, 5, "hax"));
        }

        [Fact]
        public void StartsWithIgnoreCase_is_case_insensitive()
        {
            var m = "HELLO".ToCharArray();
            Assert.True(TextUtil.StartsWithIgnoreCase(m, 0, 5, "hello"));
        }
        [Fact]
        public void IndexOf_finds_string_at_very_end()
        {
            var m = "abcdef".ToCharArray();
            Assert.Equal(3, TextUtil.IndexOf(m, "def", 0, 6));
        }

        [Fact]
        public void IndexOf_returns_minus_one_when_not_found()
        {
            var m = "abcdef".ToCharArray();
            Assert.Equal(-1, TextUtil.IndexOf(m, "xyz", 0, 6));
        }

        [Fact]
        public void IndexOf_finds_string_at_start()
        {
            var m = "abcdef".ToCharArray();
            Assert.Equal(0, TextUtil.IndexOf(m, "abc", 0, 6));
        }

        [Fact]
        public void IndexOf_respects_from_parameter()
        {
            var m = "abcabc".ToCharArray();
            Assert.Equal(3, TextUtil.IndexOf(m, "abc", 1, 6));
        }
        [Fact]
        public void IndexOfIgnoreCase_finds_string_at_very_end()
        {
            var m = "abcDEF".ToCharArray();
            Assert.Equal(3, TextUtil.IndexOfIgnoreCase(m, "def", 0, 6));
        }

        [Fact]
        public void IndexOfIgnoreCase_returns_minus_one_when_not_found()
        {
            var m = "abcdef".ToCharArray();
            Assert.Equal(-1, TextUtil.IndexOfIgnoreCase(m, "xyz", 0, 6));
        }

        [Fact]
        public void IndexOfIgnoreCase_respects_from_and_end()
        {
            var m = "abcABC".ToCharArray();
            Assert.Equal(3, TextUtil.IndexOfIgnoreCase(m, "abc", 1, 6));
            Assert.Equal(-1, TextUtil.IndexOfIgnoreCase(m, "abc", 0, 2));
        }

        // Mutation: i - lowerCase.Length => i + lowerCase.Length (L45 arithmetic)
        [Fact]
        public void IndexOfIgnoreCase_finds_at_exact_boundary()
        {
            // "abc" at index 0, search from 0: the loop condition is i + lowerCase.Length <= end
            // With end=3, the string "abc" fits exactly at i=0 but NOT at i=1
            var m = "abc".ToCharArray();
            Assert.Equal(0, TextUtil.IndexOfIgnoreCase(m, "abc", 0, 3));
            // If mutated to i - lowerCase.Length <= end, it would find it even at positions past the boundary
            Assert.Equal(-1, TextUtil.IndexOfIgnoreCase(m, "abc", 1, 3));
        }
    }
}
