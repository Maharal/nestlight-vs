using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NestLight.Common;

namespace NestLight.Completion
{
    internal sealed partial class CompletionEngine
    {
        // ---- the second stage: similar words ------------------------------------------------------------------

        /// <summary>The number of edits tolerated for a prefix of this length.</summary>
        public static int ToleranceFor(int prefixLength)
        {
            return prefixLength >= 6 ? 2 : 1;
        }

        /// <summary>A candidate that is close enough: a keyword (a string of the vocabulary) or a word of the text (a range of it).</summary>
        private sealed class Similar
        {
            public string Source;
            public int Start, Length;
            public bool Keyword;
            public int Distance;
            /// <summary>For a word, the distance from the caret to its nearest occurrence.</summary>
            public int Near;
            /// <summary>Same hash, different text (a collision): the chain of the words seen with this hash.</summary>
            public Similar Next;
            /// <summary>The word is too far: it is remembered only to skip the next occurrences.</summary>
            public bool Rejected;
        }

        private void AddSimilar(ICompletionLanguage language, string text, CompletionSite site, string prefix, bool upper, HashSet<string> seen, List<Suggestion> result, CancellationToken cancellation, bool keywords)
        {
            cancellation.ThrowIfCancellationRequested();
            int k = ToleranceFor(prefix.Length), n = prefix.Length;
            char first = char.ToUpperInvariant(prefix[0]);
            int examined = 0;

            // Only the best few are kept, so thousands of similar words cost a comparison each, not a sort.
            bool shortPrefix = n <= 3;
            int limit = shortPrefix && _features.ShortSimilarCap > 0 ? Math.Min(_fuzzyMaxItems, _features.ShortSimilarCap) : _fuzzyMaxItems;
            int capacity = limit + result.Count;
            var best = new List<Similar>(capacity + 1);

            foreach (string word in !keywords || shortPrefix && _features.ShortSimilarFromFileOnly ? new string[0] : language.CompletionWords)
            {
                if (++examined % CancellationStride == 0) cancellation.ThrowIfCancellationRequested();
                if (word.Length < n - k) continue;
                if (_fuzzyFirstLetter && char.ToUpperInvariant(word[0]) != first) continue;
                int distance = _matcher.Distance(text, site.Start, n, word, 0, word.Length, k);
                if (distance <= 0) continue; // too far, or an exact prefix (the first stage's business)
                Keep(best, capacity, new Similar { Source = word, Start = 0, Length = word.Length, Keyword = true, Distance = distance }, text);
            }

            // the words of the document: one pass, the same window and the same word rules as the first stage
            int from = Math.Max(0, site.Caret - WordScanWindow);
            int to = Math.Min(text.Length, site.Caret + WordScanWindow);
            bool dash = language.IsExtraWordChar('-');
            var known = new Dictionary<int, Similar>();
            int i = from;
            while (i < to)
            {
                char c = text[i];
                if (!(char.IsLetter(c) || c == '_' || (dash && c == '-'))) { i++; continue; }
                int start = i;
                while (i < to && (char.IsLetterOrDigit(text[i]) || text[i] == '_' || (dash && text[i] == '-'))) i++;
                int length = i - start;

                if (length < _minWordLength || length > MaxWordLength) continue;
                if (start <= site.Caret && site.Caret <= i) continue; // the word under the caret
                if (length < n - k) continue;
                if (_fuzzyFirstLetter && char.ToUpperInvariant(text[start]) != first) continue;

                int near = start > site.Caret ? start - site.Caret : site.Caret - i;
                int hash = WordText.Hash(text, start, length);
                Similar entry;
                known.TryGetValue(hash, out entry);
                Similar same = entry;
                while (same != null && !(same.Length == length && string.CompareOrdinal(text, same.Start, text, start, length) == 0)) same = same.Next;
                if (same != null) { if (near < same.Near) same.Near = near; continue; }

                if (++examined % CancellationStride == 0) cancellation.ThrowIfCancellationRequested();
                int distance = _matcher.Distance(text, site.Start, n, text, start, length, k);
                known[hash] = new Similar { Start = start, Length = length, Distance = distance, Near = near, Rejected = distance <= 0, Next = entry };
            }

            foreach (Similar candidate in known.Values)
                for (Similar s = candidate; s != null; s = s.Next)
                    if (!s.Rejected) Keep(best, capacity, s, text);

            cancellation.ThrowIfCancellationRequested();
            int added = 0;
            foreach (Similar s in best)
            {
                if (added >= limit || result.Count >= _maxItems) break;
                string word = s.Keyword ? s.Source : text.Substring(s.Start, s.Length);
                if (!seen.Add(word)) continue;
                result.Add(new Suggestion(s.Keyword && upper ? word.ToUpperInvariant() : word, s.Keyword ? SuggestionKind.Keyword : SuggestionKind.Word, s.Distance));
                added++;
            }
        }

        /// <summary>Inserts the candidate in the sorted list if it is among the first <paramref name="capacity"/>.</summary>
        private static void Keep(List<Similar> best, int capacity, Similar candidate, string text)
        {
            int lo = 0, hi = best.Count;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (Compare(best[mid], candidate, text) <= 0) lo = mid + 1; else hi = mid;
            }
            if (lo >= capacity) return;
            best.Insert(lo, candidate);
            if (best.Count > capacity) best.RemoveAt(best.Count - 1);
        }

        /// <summary>Keywords before words, then the fewer edits, then the nearer to the caret, then the spelling (so that the order is the same every time).</summary>
        private static int Compare(Similar a, Similar b, string text)
        {
            if (a.Keyword != b.Keyword) return a.Keyword ? -1 : 1;
            if (a.Distance != b.Distance) return a.Distance < b.Distance ? -1 : 1;
            if (a.Near != b.Near) return a.Near < b.Near ? -1 : 1;
            string sa = a.Keyword ? a.Source : text, sb = b.Keyword ? b.Source : text;
            int common = Math.Min(a.Length, b.Length);
            int c = string.Compare(sa, a.Start, sb, b.Start, common, StringComparison.OrdinalIgnoreCase);
            if (c != 0) return c;
            if (a.Length != b.Length) return a.Length < b.Length ? -1 : 1;
            return string.CompareOrdinal(sa, a.Start, sb, b.Start, a.Length);
        }

        /// <summary>The matches of one pass over the window of the document.</summary>
        private sealed class WordScan
        {
            public readonly List<WordMatch> Before = new List<WordMatch>();
            public readonly List<WordMatch> After = new List<WordMatch>();
            /// <summary>With <see cref="CompletionFeatures.SameLanguageWords"/>, the matches outside the code of the strings of the language; they come after the others.</summary>
            public readonly List<WordMatch> OtherBefore = new List<WordMatch>();
            public readonly List<WordMatch> OtherAfter = new List<WordMatch>();
            /// <summary>With <see cref="CompletionFeatures.ShortWordsLast"/>, the words under the minimum length; they come after all the others.</summary>
            public readonly List<WordMatch> ShortBefore = new List<WordMatch>();
            public readonly List<WordMatch> ShortAfter = new List<WordMatch>();
            /// <summary>The matches that follow the same word and punctuation as the caret does (only with <see cref="CompletionFeatures.PreviousWord"/>).</summary>
            public readonly List<WordMatch> Follows;
            public WordScan(bool context) { if (context) Follows = new List<WordMatch>(); }
        }

        /// <summary>The word before the caret and the punctuation between them: what the words that follow it elsewhere have in common.</summary>
        private struct PreviousContext
        {
            public bool Has;
            public int Start, Length;
            /// <summary>The characters between the two words, without blanks and at most <see cref="MaxSeparator"/> of them.</summary>
            public string Separator;
        }

        private const int MaxSeparator = 3;
        /// <summary>The shortest word the previous-word ranking offers.</summary>
        private const int FollowMinWordLength = 2;
        private const int ContextReach = 200;

        private static PreviousContext ContextBefore(ICompletionLanguage language, string text, CompletionSite site)
        {
            int floor = Math.Max(site.OwnerStart >= 0 ? site.OwnerStart : 0, site.Start - ContextReach);
            int i = site.Start;
            var separator = new char[MaxSeparator];
            int count = 0;
            while (i > floor && !IsWordChar(language, text[i - 1]))
            {
                char c = text[i - 1];
                if (!char.IsWhiteSpace(c))
                {
                    if (count == MaxSeparator) return default(PreviousContext);
                    separator[MaxSeparator - 1 - count++] = c;
                }
                i--;
            }
            int end = i;
            while (i > floor && IsWordChar(language, text[i - 1])) i--;
            if (end == i || !IsWordStart(language, text[i])) return default(PreviousContext);
            return new PreviousContext { Has = true, Start = i, Length = end - i, Separator = new string(separator, MaxSeparator - count, count) };
        }

        /// <summary>Whether the words at <paramref name="previousStart"/> and then <paramref name="start"/> are the ones of the context, with the same punctuation between.</summary>
        private static bool Follows(string text, PreviousContext context, int previousStart, int previousLength, int start)
        {
            if (previousLength != context.Length) return false;
            int between = previousStart + previousLength;
            if (start - between > ContextReach) return false;
            int k = 0;
            for (int j = between; j < start; j++)
            {
                char c = text[j];
                if (char.IsWhiteSpace(c)) continue;
                if (k >= context.Separator.Length || context.Separator[k] != c) return false;
                k++;
            }
            if (k != context.Separator.Length) return false;
            return string.Compare(text, previousStart, text, context.Start, previousLength, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>The part of [from, to) that is outside the interpolations of the string.</summary>
        private static void AddPieces(List<KeyValuePair<int, int>> pieces, EmbeddedString s, int from, int to)
        {
            int at = from;
            foreach (Interpolation x in s.Interpolations)
            {
                if (x.End <= at || x.Start >= to) continue;
                if (x.Start > at) pieces.Add(new KeyValuePair<int, int>(at, x.Start));
                at = Math.Max(at, x.End);
            }
            if (at < to) pieces.Add(new KeyValuePair<int, int>(at, to));
        }

        /// <summary>
        /// The code of the strings of the caret's language as sorted, disjoint [start, end) pairs, flat: what is inside a string of the
        /// language and outside its interpolations.
        /// </summary>
        private int[] CodeRanges(string text, CompletionSite site)
        {
            var pieces = new List<KeyValuePair<int, int>>();
            foreach (EmbeddedString s in _scanner.Scan(text))
            {
                if (_languages.Same(s.EmbeddedLanguageId, site.EmbeddedLanguageId)) AddPieces(pieces, s, s.Start, Math.Min(s.End, text.Length));
                else
                {
                    // the code of this language inside a string of another (the CSS inside HTML)
                    var nesting = _languages.Find(s.EmbeddedLanguageId) as INestedLanguages;
                    if (nesting != null)
                        foreach (NestedRegion r in nesting.RegionsIn(text, s))
                            if (_languages.Same(r.EmbeddedLanguageId, site.EmbeddedLanguageId)) AddPieces(pieces, s, r.Start, r.End);
                }
            }
            pieces.Sort((a, b) => a.Key.CompareTo(b.Key));

            var merged = new List<int>(pieces.Count * 2);
            foreach (var piece in pieces)
            {
                if (merged.Count > 0 && piece.Key <= merged[merged.Count - 1]) merged[merged.Count - 1] = Math.Max(merged[merged.Count - 1], piece.Value);
                else { merged.Add(piece.Key); merged.Add(piece.Value); }
            }
            return merged.ToArray();
        }

        /// <summary>
        /// One pass over the window finds where the words that start with the prefix are, without creating a string.
        /// The word being typed is not a candidate for itself.
        /// </summary>
        private WordScan ScanWords(ICompletionLanguage language, string text, CompletionSite site, string prefix, PreviousContext context, int[] scope)
        {
            var scan = new WordScan(context.Has);
            int from = Math.Max(0, site.Caret - WordScanWindow);
            int to = Math.Min(text.Length, site.Caret + WordScanWindow);
            bool dash = language.IsExtraWordChar('-');

            // a short word is only worth offering where the context says it belongs (the BY after GROUP)
            int shortest = context.Has || _features.ShortWordsLast ? Math.Min(_minWordLength, FollowMinWordLength) : _minWordLength;
            int previousStart = -1, previousLength = 0;
            bool nextSeen = false; // the first word after the caret is what the text already says comes next, not a word that followed the context
            int range = 0; // the matches come in order, so a pointer into the ranges of scope is enough
            int i = from;
            while (i < to)
            {
                char first = text[i];
                if (!(char.IsLetter(first) || first == '_' || (dash && first == '-'))) { i++; continue; }
                int start = i;
                while (i < to && (char.IsLetterOrDigit(text[i]) || text[i] == '_' || (dash && text[i] == '-'))) i++;
                int length = i - start;
                int beforeStart = previousStart, beforeLength = previousLength;
                previousStart = start; previousLength = length;
                bool isNext = !nextSeen && start >= site.End;
                if (isNext) nextSeen = true;

                if (length < shortest || length > MaxWordLength) continue;
                if (start <= site.Caret && site.Caret <= i) continue; // the word under the caret
                if (length == prefix.Length) continue; // nothing to add
                if (string.Compare(text, start, prefix, 0, prefix.Length, StringComparison.OrdinalIgnoreCase) != 0) continue;
                var match = new WordMatch(start, length);
                bool inScope = true;
                if (scope != null)
                {
                    while (range < scope.Length && scope[range + 1] <= start) range += 2;
                    inScope = range < scope.Length && scope[range] <= start;
                }
                bool follows = inScope && !isNext && context.Has && beforeStart >= 0 && Follows(text, context, beforeStart, beforeLength, start);
                if (follows) scan.Follows.Add(match);
                if (length < _minWordLength)
                {
                    if (_features.ShortWordsLast) (start > site.Caret ? scan.ShortAfter : scan.ShortBefore).Add(match);
                    continue;
                }
                if (start > site.Caret) (inScope ? scan.After : scan.OtherAfter).Add(match);
                else (inScope ? scan.Before : scan.OtherBefore).Add(match);
            }
            return scan;
        }

        /// <summary>The words that followed the same word before, the nearest occurrence first; a keyword of the language keeps its own spelling.</summary>
        private static void AddFollowing(ICompletionLanguage language, string text, CompletionSite site, WordScan scan, bool upper, HashSet<string> seen, List<Suggestion> result, IWordRanker ranker)
        {
            foreach (WordMatch m in ranker.RankFollowing(text, site.Caret, scan.Follows))
            {
                string word = text.Substring(m.Start, m.Length);
                if (!seen.Add(word)) continue;
                string keyword = language.FindCompletionWord(word);
                result.Add(keyword != null
                    ? new Suggestion(upper ? keyword.ToUpperInvariant() : keyword, SuggestionKind.Keyword)
                    : new Suggestion(word, SuggestionKind.Word));
            }
        }

        /// <summary>
        /// The words of the scan in the order of the ranker (the words of the code of the language first, then the others, then the short
        /// ones) and each one once. A word is only created when it is about to be offered: one that was already offered is recognized from
        /// its hash, so a caller that stops after the first N words never pays for the other thousands.
        /// </summary>
        private IEnumerable<string> OrderedWords(string text, CompletionSite site, WordScan scan)
        {
            var emitted = new HashSet<string>(StringComparer.Ordinal);
            var byHash = new Dictionary<int, string>();
            for (int tier = 0; tier < 3; tier++)
            {
                List<WordMatch> before = tier == 0 ? scan.Before : tier == 1 ? scan.OtherBefore : scan.ShortBefore, after = tier == 0 ? scan.After : tier == 1 ? scan.OtherAfter : scan.ShortAfter;
                foreach (WordMatch m in _features.Ranker.Rank(text, site.Caret, before, after))
                {
                    int hash = WordText.Hash(text, m.Start, m.Length);
                    string known;
                    if (byHash.TryGetValue(hash, out known) && known.Length == m.Length && string.CompareOrdinal(text, m.Start, known, 0, m.Length) == 0) continue;
                    string word = text.Substring(m.Start, m.Length);
                    if (!emitted.Add(word)) continue;
                    if (known == null) byHash[hash] = word;
                    yield return word;
                }
            }
        }
    }
}
