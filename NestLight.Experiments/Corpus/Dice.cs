using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NestLight.Experiments
{
    /// <summary>Seeded choices for the generators of code.</summary>
    internal sealed class Dice
    {
        private readonly Random _random;

        public Dice(int seed) { _random = new Random(seed); }

        public int Next(int n) { return _random.Next(n); }
        public int Between(int low, int high) { return low + _random.Next(high - low + 1); }
        public bool Chance(double p) { return _random.NextDouble() < p; }
        public T Pick<T>(IList<T> items) { return items[_random.Next(items.Count)]; }

        /// <summary>Picks <paramref name="count"/> different items, in a random order.</summary>
        public List<T> Some<T>(IList<T> items, int count)
        {
            var pool = items.ToList();
            var result = new List<T>();
            for (int i = 0; i < count && pool.Count > 0; i++)
            {
                int k = _random.Next(pool.Count);
                result.Add(pool[k]);
                pool.RemoveAt(k);
            }
            return result;
        }

        /// <summary>A name written the way a developer would: one of the options, picked by weight (the first ones are the more common).</summary>
        public T Weighted<T>(IList<T> items)
        {
            double r = _random.NextDouble();
            int index = (int)(items.Count * r * r);
            return items[Math.Min(index, items.Count - 1)];
        }
    }

    /// <summary>One file of a corpus: host code with a few strings of one embedded language.</summary>
    internal sealed class CorpusDocument
    {
        public string Language;
        public string Text;
        public int Snippets;
        /// <summary>The files are split in two halves: one to learn from, one to measure on.</summary>
        public bool Train;
    }
}
