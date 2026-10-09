using System;

namespace NestLight.Completion
{
    /// <summary>
    /// Every word of a text, in order, kept in memory: where it starts, how long it is and its first letter in upper case. The completion
    /// asks it for the words that start with what was typed instead of reading the whole text for them at every request. A word is a
    /// run of letters, digits and underscores that starts with a letter or an underscore (and the dash, for the languages that use it in
    /// words), the same rule the scan of the engine follows.
    /// </summary>
    /// <remarks>
    /// An index never changes after it is built, so a request that is running keeps its index while the next edit makes another one. After
    /// an edit, <see cref="Update"/> finds what changed by comparing the old text with the new one, reads again only the words around the
    /// change and copies the rest, moving the words after the edit by its length.
    /// </remarks>
    internal sealed class WordIndex
    {
        private readonly int[] _start;
        private readonly int[] _length;
        private readonly char[] _key;
        private readonly bool _dash;

        private WordIndex(int[] start, int[] length, char[] key, bool dash)
        {
            _start = start;
            _length = length;
            _key = key;
            _dash = dash;
        }

        public int Count { get { return _start.Length; } }

        /// <summary>Whether the dash belongs to a word (CSS, HTML).</summary>
        public bool Dash { get { return _dash; } }

        /// <summary>The memory the index holds, for the reports.</summary>
        public long Bytes { get { return (long)_start.Length * (sizeof(int) + sizeof(int) + sizeof(char)); } }

        public int Start(int word) { return _start[word]; }
        public int Length(int word) { return _length[word]; }

        /// <summary>The first letter of the word in upper case (the invariant culture): what the comparison without case sees.</summary>
        public char Key(int word) { return _key[word]; }

        /// <summary>The first word that starts at or after the position, or <see cref="Count"/>.</summary>
        public int FirstAtOrAfter(int position)
        {
            int lo = 0, hi = _start.Length;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (_start[mid] < position) lo = mid + 1; else hi = mid;
            }
            return lo;
        }

        // ---- the words of a text -------------------------------------------------------------------------------

        public static bool StartsWord(char c, bool dash) { return char.IsLetter(c) || c == '_' || (dash && c == '-'); }

        public static bool InWord(char c, bool dash) { return char.IsLetterOrDigit(c) || c == '_' || (dash && c == '-'); }

        /// <summary>The next word at or after <paramref name="from"/>: returns where it ends and sets where it starts (-1 when there is none).</summary>
        private static int NextWord(string text, int from, bool dash, out int start)
        {
            int i = from, n = text.Length;
            while (i < n)
            {
                if (!StartsWord(text[i], dash)) { i++; continue; }
                start = i;
                while (i < n && InWord(text[i], dash)) i++;
                return i;
            }
            start = -1;
            return n;
        }

        private sealed class Words
        {
            public int[] Start, Length;
            public char[] Key;
            public int Count;

            public Words(int capacity)
            {
                Start = new int[capacity];
                Length = new int[capacity];
                Key = new char[capacity];
            }

            public void Add(string text, int start, int length)
            {
                if (Count == Start.Length)
                {
                    int capacity = Math.Max(16, Count * 2);
                    Array.Resize(ref Start, capacity);
                    Array.Resize(ref Length, capacity);
                    Array.Resize(ref Key, capacity);
                }
                Start[Count] = start;
                Length[Count] = length;
                Key[Count] = char.ToUpperInvariant(text[start]);
                Count++;
            }
        }

        public static WordIndex Build(string text, bool dash)
        {
            if (text == null) throw new ArgumentNullException("text");
            var words = new Words(Math.Max(16, text.Length / 6));
            int at = 0;
            while (true)
            {
                int start;
                int end = NextWord(text, at, dash, out start);
                if (start < 0) break;
                words.Add(text, start, end - start);
                at = end;
            }
            return Finish(words, dash);
        }

        private static WordIndex Finish(Words words, bool dash)
        {
            // the arrays are kept as they are when nearly full; otherwise they are cut to size, since an index lives as long as the text
            if (words.Count != words.Start.Length)
            {
                Array.Resize(ref words.Start, words.Count);
                Array.Resize(ref words.Length, words.Count);
                Array.Resize(ref words.Key, words.Count);
            }
            return new WordIndex(words.Start, words.Length, words.Key, dash);
        }

        // ---- after an edit -------------------------------------------------------------------------------------

        /// <summary>
        /// The index of <paramref name="newText"/>, given the index of <paramref name="oldText"/> (this one). The words before the change and
        /// the words after it are the ones already known; only the words the change touches are read again.
        /// </summary>
        public WordIndex Update(string oldText, string newText)
        {
            if (oldText == null) throw new ArgumentNullException("oldText");
            if (newText == null) throw new ArgumentNullException("newText");
            int oldLength = oldText.Length, newLength = newText.Length;
            int prefix = CommonPrefix(oldText, newText);
            if (prefix == oldLength && prefix == newLength) return this;
            int suffix = CommonSuffix(oldText, newText, prefix);

            // [prefix, oldLength - suffix) of the old text became [prefix, newLength - suffix) of the new one
            int oldEnd = oldLength - suffix, newEnd = newLength - suffix, delta = newLength - oldLength;

            // The words before the first one that touches the change are the same words. Reading starts where the last of them ends:
            // everything between there and the change is unchanged text that holds no word.
            int first = FirstEndingAtOrAfter(prefix);
            int from = first > 0 ? _start[first - 1] + _length[first - 1] : 0;

            // Reading goes on until a word of the new text starts where a word of the old one started, after the change: from there
            // on the text is the same and so are the words. If that never happens, to the end.
            var read = new Words(16);
            int after = _start.Length;
            int at = from;
            while (true)
            {
                int start;
                int end = NextWord(newText, at, _dash, out start);
                if (start < 0) break;
                if (start >= newEnd)
                {
                    int known = IndexOfStart(start - delta);
                    if (known >= 0) { after = known; break; }
                }
                read.Add(newText, start, end - start);
                at = end;
            }

            int count = first + read.Count + (_start.Length - after);
            var starts = new int[count];
            var lengths = new int[count];
            var keys = new char[count];
            Array.Copy(_start, 0, starts, 0, first);
            Array.Copy(_length, 0, lengths, 0, first);
            Array.Copy(_key, 0, keys, 0, first);
            Array.Copy(read.Start, 0, starts, first, read.Count);
            Array.Copy(read.Length, 0, lengths, first, read.Count);
            Array.Copy(read.Key, 0, keys, first, read.Count);
            int tail = first + read.Count;
            Array.Copy(_length, after, lengths, tail, _start.Length - after);
            Array.Copy(_key, after, keys, tail, _start.Length - after);
            for (int k = after, j = tail; k < _start.Length; k++, j++) starts[j] = _start[k] + delta;
            return new WordIndex(starts, lengths, keys, _dash);
        }

        /// <summary>The first word that ends at or after the position, or <see cref="Count"/>.</summary>
        private int FirstEndingAtOrAfter(int position)
        {
            int lo = 0, hi = _start.Length;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (_start[mid] + _length[mid] >= position) hi = mid; else lo = mid + 1;
            }
            return lo;
        }

        private int IndexOfStart(int position)
        {
            int k = FirstAtOrAfter(position);
            return k < _start.Length && _start[k] == position ? k : -1;
        }

        private const int Chunk = 256;

        /// <summary>The number of characters both texts start with.</summary>
        internal static int CommonPrefix(string a, string b)
        {
            int n = Math.Min(a.Length, b.Length), i = 0;
            while (i + Chunk <= n && string.CompareOrdinal(a, i, b, i, Chunk) == 0) i += Chunk;
            while (i < n && a[i] == b[i]) i++;
            return i;
        }

        /// <summary>The number of characters both texts end with, not counting the first <paramref name="skip"/> (the common prefix).</summary>
        internal static int CommonSuffix(string a, string b, int skip)
        {
            int n = Math.Min(a.Length, b.Length) - skip, i = 0;
            while (i + Chunk <= n && string.CompareOrdinal(a, a.Length - i - Chunk, b, b.Length - i - Chunk, Chunk) == 0) i += Chunk;
            while (i < n && a[a.Length - 1 - i] == b[b.Length - 1 - i]) i++;
            return i;
        }
    }
}
