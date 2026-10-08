using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Completion
{
    internal enum SuggestionKind
    {
        /// <summary>A keyword, tag, property or built-in of the embedded language.</summary>
        Keyword,
        /// <summary>A word that already appears in the document.</summary>
        Word
    }

    internal sealed class Suggestion
    {
        public readonly string Text;
        public readonly SuggestionKind Kind;

        public Suggestion(string text, SuggestionKind kind)
        {
            Text = text; Kind = kind;
        }
    }

    /// <summary>Where a completion applies: the word under the caret inside an embedded string.</summary>
    internal sealed class CompletionSite
    {
        public CompletionSite(string languageId, int start, int caret, int end)
        {
            LanguageId = languageId; Start = start; Caret = caret; End = end;
        }

        /// <summary>Lower-case id or alias of the embedded language.</summary>
        public string LanguageId { get; private set; }
        /// <summary>First character of the word (the replaced span is [Start, End)).</summary>
        public int Start { get; private set; }
        public int Caret { get; private set; }
        /// <summary>Index right after the last character of the word, including what follows the caret.</summary>
        public int End { get; private set; }

        /// <summary>What was typed so far: the text of [Start, Caret).</summary>
        public int PrefixLength { get { return Caret - Start; } }
    }

    internal interface ICompletionProvider
    {
        /// <summary>The site of the caret, or null when it is not inside the code of an embedded string.</summary>
        CompletionSite Locate(string text, int caret);

        /// <summary>The suggestions for the site, best first. Never throws on incomplete code.</summary>
        IReadOnlyList<Suggestion> Suggest(string text, CompletionSite site);
    }

    /// <summary>
    /// Completion inside embedded strings, from two sources: the keywords of the language of the string
    /// (<see cref="Vocabularies"/>) and the words that already exist in the document, nearest to the caret first
    /// (what Visual Studio Code calls word-based suggestions).
    /// </summary>
    internal sealed class CompletionEngine : ICompletionProvider
    {
        public const int DefaultMaxItems = 100;
        public const int DefaultMinWordLength = 3;
        private const int MaxWordLength = 64;
        /// <summary>Documents larger than this are only scanned for words around the caret.</summary>
        private const int WordScanWindow = 500000;

        private readonly IHostScanner _scanner;
        private readonly int _maxItems;
        private readonly int _minWordLength;

        public CompletionEngine(IHostScanner scanner, int maxItems = DefaultMaxItems, int minWordLength = DefaultMinWordLength)
        {
            if (scanner == null) throw new ArgumentNullException("scanner");
            if (maxItems < 1) throw new ArgumentOutOfRangeException("maxItems");
            if (minWordLength < 1) throw new ArgumentOutOfRangeException("minWordLength");
            _scanner = scanner;
            _maxItems = maxItems;
            _minWordLength = minWordLength;
        }

        // ---- where ----------------------------------------------------------------------------------------------

        public CompletionSite Locate(string text, int caret)
        {
            if (text == null || caret < 0 || caret > text.Length) return null;

            EmbeddedString owner = null;
            foreach (EmbeddedString s in _scanner.Scan(text))
            {
                if (s.OuterStart > caret) break;
                // the innermost string wins when templates are nested in an interpolation
                if (s.Start <= caret && caret <= Math.Min(s.End, text.Length)) owner = s;
            }
            if (owner == null) return null;

            foreach (Interpolation x in owner.Interpolations)
                if (x.Start < caret && caret < x.End) return null; // the host language owns the expression

            string id = owner.LanguageId;
            int start = caret;
            while (start > owner.Start && IsWordChar(id, text[start - 1])) start--;
            int end = caret;
            int limit = Math.Min(owner.End, text.Length);
            while (end < limit && IsWordChar(id, text[end])) end++;

            if (start < caret && !IsWordStart(id, text[start])) return null; // numbers, not words
            return new CompletionSite(id, start, caret, end);
        }

        // ---- what -----------------------------------------------------------------------------------------------

        public IReadOnlyList<Suggestion> Suggest(string text, CompletionSite site)
        {
            var result = new List<Suggestion>();
            if (text == null || site == null || site.Caret > text.Length) return result;

            string prefix = text.Substring(site.Start, site.PrefixLength);
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            bool upper = Vocabularies.FollowsTypedCase(site.LanguageId) && prefix.Length > 0 && !HasLower(prefix);
            foreach (string word in Vocabularies.For(site.LanguageId))
            {
                if (result.Count >= _maxItems) return result;
                if (!StartsWithIgnoreCase(word, prefix) || word.Length == prefix.Length) continue;
                if (seen.Add(word)) result.Add(new Suggestion(upper ? word.ToUpperInvariant() : word, SuggestionKind.Keyword));
            }

            foreach (string word in WordsOfDocument(text, site, prefix))
            {
                if (result.Count >= _maxItems) break;
                if (seen.Add(word)) result.Add(new Suggestion(word, SuggestionKind.Word));
            }
            return result;
        }

        /// <summary>
        /// The words of the document that start with the prefix, nearest to the caret first and each one once. The word being typed
        /// is not a candidate for itself.
        /// </summary>
        /// <remarks>
        /// One pass over the window finds where the matching words are, without creating a string. The matches before and after the
        /// caret are already in order, so merging them gives the order by distance, and a word is only created when it is about to
        /// be offered: a caller that stops after the first N words never pays for the other thousands.
        /// </remarks>
        private IEnumerable<string> WordsOfDocument(string text, CompletionSite site, string prefix)
        {
            int from = Math.Max(0, site.Caret - WordScanWindow);
            int to = Math.Min(text.Length, site.Caret + WordScanWindow);
            bool dash = Vocabularies.IsExtraWordChar(site.LanguageId, '-');

            var before = new List<Match>();
            var after = new List<Match>();
            int i = from;
            while (i < to)
            {
                char first = text[i];
                if (!(char.IsLetter(first) || first == '_' || (dash && first == '-'))) { i++; continue; }
                int start = i;
                while (i < to && (char.IsLetterOrDigit(text[i]) || text[i] == '_' || (dash && text[i] == '-'))) i++;
                int length = i - start;

                if (length < _minWordLength || length > MaxWordLength) continue;
                if (start <= site.Caret && site.Caret <= i) continue; // the word under the caret
                if (length == prefix.Length) continue; // nothing to add
                if (string.Compare(text, start, prefix, 0, prefix.Length, StringComparison.OrdinalIgnoreCase) != 0) continue;
                (start > site.Caret ? after : before).Add(new Match(start, length));
            }

            var emitted = new HashSet<string>(StringComparer.Ordinal);
            var byHash = new Dictionary<int, string>();
            int b = before.Count - 1, a = 0;
            while (b >= 0 || a < after.Count)
            {
                Match m;
                if (a >= after.Count) m = before[b--];
                else if (b < 0) m = after[a++];
                else
                {
                    int behind = site.Caret - (before[b].Start + before[b].Length), ahead = after[a].Start - site.Caret;
                    if (behind < ahead) m = before[b--];
                    else if (ahead < behind) m = after[a++];
                    else
                    {
                        // the same distance on both sides: alphabetical, so that the order does not depend on the side
                        Match left = before[b], right = after[a];
                        bool leftFirst = string.CompareOrdinal(text.Substring(left.Start, left.Length), text.Substring(right.Start, right.Length)) <= 0;
                        m = leftFirst ? before[b--] : after[a++];
                    }
                }

                // a word that was already offered is recognized from its hash, without creating the string again
                int hash = Hash(text, m.Start, m.Length);
                string known;
                if (byHash.TryGetValue(hash, out known) && known.Length == m.Length && string.CompareOrdinal(text, m.Start, known, 0, m.Length) == 0) continue;
                string word = text.Substring(m.Start, m.Length);
                if (!emitted.Add(word)) continue;
                if (known == null) byHash[hash] = word;
                yield return word;
            }
        }

        private struct Match
        {
            public readonly int Start, Length;
            public Match(int start, int length) { Start = start; Length = length; }
        }

        private static int Hash(string text, int start, int length)
        {
            unchecked
            {
                int h = (int)2166136261;
                for (int k = 0; k < length; k++) h = (h ^ text[start + k]) * 16777619;
                return h;
            }
        }

        // ---- characters -----------------------------------------------------------------------------------------

        private static bool IsWordStart(string languageId, char c)
        {
            return char.IsLetter(c) || c == '_' || Vocabularies.IsExtraWordChar(languageId, c);
        }

        private static bool IsWordChar(string languageId, char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || Vocabularies.IsExtraWordChar(languageId, c);
        }

        private static bool HasLower(string s)
        {
            foreach (char c in s) if (char.IsLower(c)) return true;
            return false;
        }

        private static bool StartsWithIgnoreCase(string word, string prefix)
        {
            return word.Length >= prefix.Length && string.Compare(word, 0, prefix, 0, prefix.Length, StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
