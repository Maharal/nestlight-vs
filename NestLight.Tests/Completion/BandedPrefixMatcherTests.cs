using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NestLight.Completion;
using Xunit;

namespace NestLight.Tests
{
    public class BandedPrefixMatcherTests
    {
        private static readonly IApproximateMatcher Matcher = new BandedPrefixMatcher();

        private static int D(string typed, string candidate, int k)
        {
            return Matcher.Distance(typed, 0, typed.Length, candidate, 0, candidate.Length, k);
        }

        /// <summary>The definition, written the straightforward way: the whole matrix of "optimal string alignment", then the closest prefix.</summary>
        private static int Reference(string a, string b, int k)
        {
            int n = a.Length, m = b.Length;
            var d = new int[m + 1, n + 1];
            for (int j = 0; j <= n; j++) d[0, j] = j;
            for (int i = 1; i <= m; i++)
            {
                d[i, 0] = i;
                for (int j = 1; j <= n; j++)
                {
                    bool same = char.ToUpperInvariant(b[i - 1]) == char.ToUpperInvariant(a[j - 1]);
                    int v = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + (same ? 0 : 1));
                    if (i > 1 && j > 1 && char.ToUpperInvariant(b[i - 1]) == char.ToUpperInvariant(a[j - 2]) && char.ToUpperInvariant(b[i - 2]) == char.ToUpperInvariant(a[j - 1]))
                        v = Math.Min(v, d[i - 2, j - 2] + 1);
                    d[i, j] = v;
                }
            }
            int best = int.MaxValue;
            for (int i = 0; i <= m; i++) best = Math.Min(best, d[i, n]);
            return best <= k ? best : -1;
        }

        [Fact]
        public void A_prefix_is_at_distance_zero()
        {
            Assert.Equal(0, D("sel", "select", 2));
            Assert.Equal(0, D("select", "select", 2));
            Assert.Equal(0, D("", "select", 2));
        }

        [Theory]
        [InlineData("selct", "select")]      // a missing letter
        [InlineData("seleect", "select")]    // an extra letter
        [InlineData("selact", "select")]     // a wrong letter
        [InlineData("slect", "select")]      // a missing letter near the start
        [InlineData("selcet", "select")]     // two neighbouring letters swapped
        public void One_edit_costs_one(string typed, string candidate)
        {
            Assert.Equal(1, D(typed, candidate, 2));
        }

        [Fact]
        public void The_swap_of_two_neighbours_is_one_edit_and_not_two()
        {
            Assert.Equal(1, D("dvi", "div", 1));
            Assert.Equal(1, D("form", "from", 1));
            Assert.Equal(1, D("abxd", "abcdef", 3)); // a plain substitution: abxd against abcd
        }

        [Fact]
        public void The_comparison_ignores_case()
        {
            Assert.Equal(1, D("SELCT", "select", 1));
            Assert.Equal(0, D("SELECT", "select", 1));
            Assert.Equal(1, D("custmer", "CustomerName", 1));
        }

        [Fact]
        public void Too_far_is_minus_one()
        {
            Assert.Equal(-1, D("xyz", "select", 1));
            Assert.Equal(-1, D("seleccct", "select", 1));
            Assert.Equal(-1, D("custmr", "customer", 0)); // no edit allowed, and it is not a prefix
        }

        [Fact]
        public void A_typed_text_no_longer_than_the_tolerance_matches_anything_as_the_empty_prefix()
        {
            Assert.Equal(2, D("zq", "select", 2));
            Assert.Equal(0, D("se", "select", 2));
        }

        [Fact]
        public void Ranges_of_larger_strings_are_read_as_such()
        {
            const string typed = "xxx selct yyy", candidate = "zz select zz";
            Assert.Equal(1, Matcher.Distance(typed, 4, 5, candidate, 3, 6, 1));
        }

        [Fact]
        public void Wrong_arguments_are_refused()
        {
            Assert.Throws<ArgumentNullException>(() => Matcher.Distance(null, 0, 0, "a", 0, 1, 1));
            Assert.Throws<ArgumentNullException>(() => Matcher.Distance("a", 0, 1, null, 0, 0, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Matcher.Distance("a", 0, 5, "a", 0, 1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Matcher.Distance("a", 0, 1, "a", 0, 5, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Matcher.Distance("a", 0, 1, "a", 0, 1, -1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Matcher.Distance("a", 0, 1, "a", 0, 1, BandedPrefixMatcher.MaxK + 1));
        }

        // ---- against the definition ------------------------------------------------------------------------------------

        private static string Random(Random random, string alphabet, int maxLength)
        {
            int length = random.Next(maxLength + 1);
            var chars = new char[length];
            for (int i = 0; i < length; i++) chars[i] = alphabet[random.Next(alphabet.Length)];
            return new string(chars);
        }

        // few letters, so that the same ones collide often; letters whose case changes in surprising ways (dotless i, long s, sharp s)
        private const string SmallAlphabet = "abAB_c-";
        private const string OddAlphabet = "iIıİsSſßkK";

        private static int Mismatches(int seed, int pairs, string alphabet, int maxTyped, int maxCandidate)
        {
            var random = new Random(seed);
            int mismatches = 0;
            for (int p = 0; p < pairs; p++)
            {
                string typed = Random(random, alphabet, maxTyped), candidate = Random(random, alphabet, maxCandidate);
                int k = random.Next(BandedPrefixMatcher.MaxK + 1);
                // put both in a longer string, at an offset, to check the ranges
                string padA = Random(random, "xyz", 3), padB = Random(random, "xyz", 3);
                int actual = Matcher.Distance(padA + typed + padB, padA.Length, typed.Length, padB + candidate + padA, padB.Length, candidate.Length, k);
                if (actual != Reference(typed, candidate, k)) mismatches++;
            }
            return mismatches;
        }

        [Fact]
        public void The_band_gives_the_same_distance_as_the_whole_matrix_in_200000_random_pairs()
        {
            Assert.Equal(0, Mismatches(1, 150000, SmallAlphabet, 8, 12));
            Assert.Equal(0, Mismatches(2, 50000, OddAlphabet, 8, 12));
        }

        [Fact]
        public void The_band_agrees_on_the_real_vocabulary()
        {
            var random = new Random(3);
            var words = CompletionLanguages.Default.Find("css").Keywords.Concat(CompletionLanguages.Default.Find("sql").Keywords).Concat(CompletionLanguages.Default.Find("glsl").Keywords).ToList();
            for (int p = 0; p < 20000; p++)
            {
                string candidate = words[random.Next(words.Count)];
                var typed = candidate.Substring(0, random.Next(1, candidate.Length + 1)).ToCharArray();
                if (typed.Length > 1 && random.Next(2) == 0) { int at = random.Next(typed.Length - 1); char t = typed[at]; typed[at] = typed[at + 1]; typed[at + 1] = t; }
                if (random.Next(3) == 0) typed[random.Next(typed.Length)] = (char)('a' + random.Next(26));
                string text = new string(typed);
                int k = 1 + random.Next(2);
                Assert.Equal(Reference(text, candidate, k), D(text, candidate, k));
            }
        }

        [Fact]
        public void Many_threads_get_the_same_answers()
        {
            // the matcher keeps no state shared between calls: its rows belong to each thread
            int[] mismatches = Enumerable.Range(10, 8).AsParallel().WithDegreeOfParallelism(8).Select(seed => Mismatches(seed, 20000, SmallAlphabet, 8, 12)).ToArray();
            Assert.All(mismatches, m => Assert.Equal(0, m));
        }

        [Fact]
        public void One_matcher_serves_tasks_that_run_at_the_same_time()
        {
            var tasks = Enumerable.Range(0, 6).Select(i => Task.Run(() => Mismatches(100 + i, 15000, SmallAlphabet, 8, 12))).ToArray();
            Task.WaitAll(tasks);
            Assert.All(tasks, t => Assert.Equal(0, t.Result));
        }
    }
}
