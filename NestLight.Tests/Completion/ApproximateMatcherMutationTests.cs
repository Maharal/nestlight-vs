using System;
using NestLight.Completion;
using Xunit;

namespace NestLight.Tests
{
    public class ApproximateMatcherMutationTests
    {
        private static readonly IApproximateMatcher Matcher = new BandedPrefixMatcher();

        // L39-L43: argument validation — mutations change the exception messages (string mutations)
        // and flip the logical conditions (OR to AND)

        [Fact]
        public void Null_typed_throws_ArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Matcher.Distance(null, 0, 0, "a", 0, 1, 1));
            Assert.Equal("typed", ex.ParamName);
        }

        [Fact]
        public void Null_candidate_throws_ArgumentNullException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Matcher.Distance("a", 0, 1, null, 0, 0, 1));
            Assert.Equal("candidate", ex.ParamName);
        }

        [Fact]
        public void Negative_typedStart_throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Matcher.Distance("abc", -1, 1, "abc", 0, 3, 1));
        }

        [Fact]
        public void Negative_typedLength_throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Matcher.Distance("abc", 0, -1, "abc", 0, 3, 1));
        }

        [Fact]
        public void TypedStart_plus_length_past_end_throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Matcher.Distance("abc", 1, 3, "abc", 0, 3, 1));
        }

        [Fact]
        public void Negative_candidateStart_throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Matcher.Distance("abc", 0, 3, "abc", -1, 1, 1));
        }

        [Fact]
        public void Negative_candidateLength_throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Matcher.Distance("abc", 0, 3, "abc", 0, -1, 1));
        }

        [Fact]
        public void CandidateStart_plus_length_past_end_throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Matcher.Distance("abc", 0, 3, "abc", 1, 3, 1));
        }

        [Fact]
        public void Negative_maxDistance_throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Matcher.Distance("abc", 0, 3, "abc", 0, 3, -1));
        }

        [Fact]
        public void MaxDistance_above_MaxK_throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Matcher.Distance("abc", 0, 3, "abc", 0, 3, BandedPrefixMatcher.MaxK + 1));
        }

        // L51-L52: row 0 initialization — mutations change loop bounds (< to <=, <= to <)

        [Fact]
        public void Empty_typed_matches_as_prefix_at_distance_zero()
        {
            Assert.Equal(0, Matcher.Distance("abc", 1, 0, "xyz", 0, 3, 2));
        }

        [Fact]
        public void Short_typed_within_tolerance_matches_anything()
        {
            Assert.Equal(1, Matcher.Distance("z", 0, 1, "abc", 0, 3, 1));
        }

        // L54: conditional mutation on best initialization

        [Fact]
        public void Typed_shorter_than_k_starts_with_correct_best()
        {
            Assert.Equal(2, Matcher.Distance("zq", 0, 2, "abcdef", 0, 6, 2));
            Assert.Equal(0, Matcher.Distance("ab", 0, 2, "abcdef", 0, 6, 2));
        }

        // L61: loop initialization mutations

        [Fact]
        public void Single_char_typed_single_char_candidate()
        {
            Assert.Equal(0, Matcher.Distance("a", 0, 1, "a", 0, 1, 1));
            Assert.Equal(1, Matcher.Distance("a", 0, 1, "b", 0, 1, 1));
            Assert.Equal(-1, Matcher.Distance("a", 0, 1, "b", 0, 1, 0));
        }

        // L79: deletion mutation (d - 1 >= 0 check)

        [Fact]
        public void Insertion_in_typed_is_detected()
        {
            Assert.Equal(1, Matcher.Distance("seleect", 0, 7, "select", 0, 6, 2));
        }

        // L86: rowMin comparison

        [Fact]
        public void Best_distance_is_tracked_across_candidate()
        {
            Assert.Equal(1, Matcher.Distance("marin", 0, 5, "margin-top", 0, 10, 2));
        }

        // L92: atEnd comparison

        [Fact]
        public void Exact_match_at_candidate_end()
        {
            Assert.Equal(0, Matcher.Distance("top", 0, 3, "top", 0, 3, 1));
        }

        // L95: early termination — previousMin > k && rowMin > k

        [Fact]
        public void Completely_different_strings_terminate_early()
        {
            Assert.Equal(-1, Matcher.Distance("xyz", 0, 3, "abcdefghij", 0, 10, 1));
        }
    }
}
