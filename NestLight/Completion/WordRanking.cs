using System;
using System.Collections.Generic;

namespace NestLight.Completion
{
    /// <summary>Where a word of the document is: <c>text[Start, Start + Length)</c>. The scan finds them without creating a string.</summary>
    internal struct WordMatch
    {
        public readonly int Start, Length;
        public WordMatch(int start, int length) { Start = start; Length = length; }
    }

    /// <summary>
    /// How the words of the document are ordered among themselves. The engine finds where the words are and asks the ranker for the order;
    /// a new way of ranking is a new class and the engine does not change. A ranker keeps no state between calls, so one instance is shared.
    /// </summary>
    internal interface IWordRanker
    {
        /// <summary>
        /// The occurrences found before and after the caret (each list in the order of the text) in the order they are offered. A word
        /// may come out more than once: the engine keeps the first occurrence of each.
        /// </summary>
        IEnumerable<WordMatch> Rank(string text, int caret, List<WordMatch> before, List<WordMatch> after);

        /// <summary>The occurrences of the words that already followed the same word as the caret's (see <see cref="CompletionFeatures.PreviousWord"/>), in the order they are offered.</summary>
        IEnumerable<WordMatch> RankFollowing(string text, int caret, List<WordMatch> follows);
    }

    /// <summary>The rankers the plugin has.</summary>
    internal static class WordRankers
    {
        /// <summary>The nearest occurrence to the caret first.</summary>
        public static readonly IWordRanker Nearest = new NearestWordRanker();

        /// <summary>The most frequent first, the nearest among equals.</summary>
        public static readonly IWordRanker Frequency = new FrequencyWordRanker();

        /// <summary><c>ln(1 + count) - weight * ln(1 + distance)</c>: a word used often counts, a word used far away counts less (0: the count alone).</summary>
        public static IWordRanker Blend(double weight) { return new BlendWordRanker(weight); }
    }

    /// <summary>What the rankers share.</summary>
    internal static class WordText
    {
        public static int Hash(string text, int start, int length)
        {
            unchecked
            {
                int h = (int)2166136261;
                for (int k = 0; k < length; k++) h = (h ^ text[start + k]) * 16777619;
                return h;
            }
        }

        /// <summary>The distance from the caret to the nearest edge of the word.</summary>
        public static int Distance(WordMatch m, int caret)
        {
            return m.Start > caret ? m.Start - caret : caret - (m.Start + m.Length);
        }
    }

    /// <summary>
    /// The words nearest to the caret first. The occurrences before and after the caret are already in order, so merging the two lists
    /// gives the order by distance, and nothing is sorted or grouped: a caller that stops after the first N words never pays for the
    /// other thousands.
    /// </summary>
    internal sealed class NearestWordRanker : IWordRanker
    {
        public IEnumerable<WordMatch> Rank(string text, int caret, List<WordMatch> before, List<WordMatch> after)
        {
            int b = before.Count - 1, a = 0;
            while (b >= 0 || a < after.Count)
            {
                if (a >= after.Count) yield return before[b--];
                else if (b < 0) yield return after[a++];
                else
                {
                    int behind = caret - (before[b].Start + before[b].Length), ahead = after[a].Start - caret;
                    if (behind < ahead) yield return before[b--];
                    else if (ahead < behind) yield return after[a++];
                    else
                    {
                        // the same distance on both sides: alphabetical, so that the order does not depend on the side
                        WordMatch left = before[b], right = after[a];
                        bool leftFirst = string.CompareOrdinal(text.Substring(left.Start, left.Length), text.Substring(right.Start, right.Length)) <= 0;
                        yield return leftFirst ? before[b--] : after[a++];
                    }
                }
            }
        }

        public IEnumerable<WordMatch> RankFollowing(string text, int caret, List<WordMatch> follows)
        {
            follows.Sort((x, y) =>
            {
                int nx = WordText.Distance(x, caret), ny = WordText.Distance(y, caret);
                return nx != ny ? nx.CompareTo(ny) : x.Start.CompareTo(y.Start);
            });
            return follows;
        }
    }

    /// <summary>
    /// A ranker that looks at how many times each word occurs: it groups the occurrences of the same word (case matters), gives each word
    /// a <see cref="Score"/>, and offers one occurrence of each, the highest score first. Ties go to the nearer word, then to the earlier
    /// in the text, so that the order is the same every time.
    /// </summary>
    internal abstract class CountingWordRanker : IWordRanker
    {
        /// <summary>One distinct word: its nearest occurrence, how many there are and how far the nearest one is.</summary>
        protected sealed class Occurrences
        {
            public WordMatch First;
            public int Count, Near;
            public Occurrences Next;
            public double Score;
        }

        /// <summary>The higher the score, the sooner the word is offered.</summary>
        protected abstract double Score(int count, int distance);

        public IEnumerable<WordMatch> Rank(string text, int caret, List<WordMatch> before, List<WordMatch> after)
        {
            return Ranked(text, caret, before, after);
        }

        public IEnumerable<WordMatch> RankFollowing(string text, int caret, List<WordMatch> follows)
        {
            return Ranked(text, caret, follows, null);
        }

        private IEnumerable<WordMatch> Ranked(string text, int caret, List<WordMatch> first, List<WordMatch> second)
        {
            var byHash = new Dictionary<int, Occurrences>();
            var all = new List<Occurrences>();
            Collect(text, caret, first, byHash, all);
            if (second != null) Collect(text, caret, second, byHash, all);

            foreach (Occurrences c in all) c.Score = Score(c.Count, c.Near);
            all.Sort((x, y) =>
            {
                if (x.Score != y.Score) return y.Score.CompareTo(x.Score);
                if (x.Near != y.Near) return x.Near.CompareTo(y.Near);
                return x.First.Start.CompareTo(y.First.Start);
            });
            foreach (Occurrences c in all) yield return c.First;
        }

        private static void Collect(string text, int caret, List<WordMatch> list, Dictionary<int, Occurrences> byHash, List<Occurrences> all)
        {
            foreach (WordMatch m in list)
            {
                int near = WordText.Distance(m, caret);
                int hash = WordText.Hash(text, m.Start, m.Length);
                Occurrences head;
                byHash.TryGetValue(hash, out head);
                Occurrences c = head;
                while (c != null && !(c.First.Length == m.Length && string.CompareOrdinal(text, c.First.Start, text, m.Start, m.Length) == 0)) c = c.Next;
                if (c == null)
                {
                    c = new Occurrences { First = m, Near = near, Next = head };
                    byHash[hash] = c;
                    all.Add(c);
                }
                c.Count++;
                if (near < c.Near) { c.Near = near; c.First = m; }
            }
        }
    }

    /// <summary>The most frequent words first.</summary>
    internal sealed class FrequencyWordRanker : CountingWordRanker
    {
        protected override double Score(int count, int distance) { return count; }
    }

    /// <summary>The count and the distance together: <c>ln(1 + count) - weight * ln(1 + distance)</c>.</summary>
    internal sealed class BlendWordRanker : CountingWordRanker
    {
        private readonly double _weight;

        public BlendWordRanker(double weight)
        {
            if (weight < 0 || double.IsNaN(weight) || double.IsInfinity(weight)) throw new ArgumentOutOfRangeException("weight");
            _weight = weight;
        }

        public double Weight { get { return _weight; } }

        protected override double Score(int count, int distance) { return Math.Log(1 + count) - _weight * Math.Log(1 + distance); }
    }
}
