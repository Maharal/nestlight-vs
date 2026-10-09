using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        /// <summary>0 when what was typed is a prefix of the suggestion; otherwise the number of edits that separate them.</summary>
        public readonly int Distance;

        public Suggestion(string text, SuggestionKind kind, int distance = 0)
        {
            Text = text; Kind = kind; Distance = distance;
        }
    }

    /// <summary>Where a completion applies: the word under the caret inside an embedded string.</summary>
    internal sealed class CompletionSite
    {
        public CompletionSite(string embeddedLanguageId, int start, int caret, int end, int ownerStart = -1, int ownerEnd = -1, bool inlineDeclarations = false)
        {
            EmbeddedLanguageId = embeddedLanguageId; Start = start; Caret = caret; End = end; OwnerStart = ownerStart; OwnerEnd = ownerEnd; InlineDeclarations = inlineDeclarations;
        }

        /// <summary>Lower-case id or alias of the embedded language.</summary>
        public string EmbeddedLanguageId { get; private set; }
        /// <summary>First character of the word (the replaced span is [Start, End)).</summary>
        public int Start { get; private set; }
        public int Caret { get; private set; }
        /// <summary>Index right after the last character of the word, including what follows the caret.</summary>
        public int End { get; private set; }
        /// <summary>The code of the embedded string the caret is in: [OwnerStart, OwnerEnd). -1 when unknown.</summary>
        public int OwnerStart { get; private set; }
        public int OwnerEnd { get; private set; }
        /// <summary>The code is the value of a style attribute: a list of declarations, with no selectors.</summary>
        public bool InlineDeclarations { get; private set; }

        /// <summary>What was typed so far: the text of [Start, Caret).</summary>
        public int PrefixLength { get { return Caret - Start; } }
    }

    internal interface ICompletionProvider
    {
        /// <summary>The site of the caret, or null when it is not inside the code of an embedded string.</summary>
        CompletionSite Locate(string text, int caret);

        /// <summary>
        /// The suggestions for the site, best first. Never throws on incomplete code; the only exception is the cancellation, which
        /// returns no partial list.
        /// </summary>
        IReadOnlyList<Suggestion> Suggest(string text, CompletionSite site, CancellationToken cancellation = default(CancellationToken));
    }

    /// <summary>
    /// Completion inside embedded strings, from two sources: the keywords of the language of the string
    /// (<see cref="Vocabularies"/>) and the words that already exist in the document, nearest to the caret first
    /// (what Visual Studio Code calls word-based suggestions).
    /// </summary>
    /// <remarks>
    /// When a matcher is given and nothing (fewer than <c>fuzzyBelow</c> items) starts with what was typed, a second stage offers
    /// the keywords and the words of the document that are a few edits away from it, after the exact ones. With no matcher the
    /// engine is exactly the first stage.
    /// </remarks>
    internal sealed class CompletionEngine : ICompletionProvider
    {
        public const int DefaultMaxItems = 100;
        public const int DefaultMinWordLength = 3;
        public const int DefaultFuzzyBelow = 1;
        public const int DefaultFuzzyMaxItems = 10;
        /// <summary>The shortest prefix the second stage looks at: with fewer letters nearly every word is "a few edits away".</summary>
        public const int FuzzyMinPrefix = 3;
        private const int CancellationStride = 256;
        private const int MaxWordLength = 64;
        /// <summary>Documents larger than this are only scanned for words around the caret.</summary>
        private const int WordScanWindow = 500000;

        private readonly IHostScanner _scanner;
        private readonly int _maxItems;
        private readonly int _minWordLength;
        private readonly IApproximateMatcher _matcher;
        private readonly int _fuzzyBelow;
        private readonly int _fuzzyMaxItems;
        private readonly bool _fuzzyFirstLetter;
        private readonly CompletionFeatures _features;
        private readonly ICompletionLanguages _languages;

        /// <param name="matcher">Enables the second stage when not null.</param>
        /// <param name="fuzzyBelow">The second stage runs only when the first one returned fewer items than this (1: only when nothing matched).</param>
        /// <param name="fuzzyMaxItems">The most items the second stage adds.</param>
        /// <param name="fuzzyFirstLetter">The first letter of a similar word has to be the one that was typed.</param>
        /// <param name="features">The context-aware rankings that are on; null: none.</param>
        /// <param name="languages">What the completion knows about each embedded language; null: the languages of the plugin.</param>
        public CompletionEngine(IHostScanner scanner, int maxItems = DefaultMaxItems, int minWordLength = DefaultMinWordLength,
            IApproximateMatcher matcher = null, int fuzzyBelow = DefaultFuzzyBelow, int fuzzyMaxItems = DefaultFuzzyMaxItems, bool fuzzyFirstLetter = true,
            CompletionFeatures features = null, ICompletionLanguages languages = null)
        {
            if (scanner == null) throw new ArgumentNullException("scanner");
            if (maxItems < 1) throw new ArgumentOutOfRangeException("maxItems");
            if (minWordLength < 1) throw new ArgumentOutOfRangeException("minWordLength");
            if (fuzzyBelow < 1) throw new ArgumentOutOfRangeException("fuzzyBelow");
            if (fuzzyMaxItems < 1) throw new ArgumentOutOfRangeException("fuzzyMaxItems");
            _scanner = scanner;
            _matcher = matcher;
            _fuzzyBelow = fuzzyBelow;
            _fuzzyMaxItems = fuzzyMaxItems;
            _fuzzyFirstLetter = fuzzyFirstLetter;
            _features = features ?? CompletionFeatures.None;
            _languages = languages ?? CompletionLanguages.Default;
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

            string id = owner.EmbeddedLanguageId;
            ICompletionLanguage language = _languages.Find(id);
            int ownerStart = owner.Start, limit = Math.Min(owner.End, text.Length);
            bool inline = false;
            var nesting = language as INestedLanguages;
            NestedRegion nested;
            if (nesting != null && nesting.TryRegionAt(text, owner, caret, out nested))
            {
                // the code of another language inside the string (the CSS of a <style> element or of a style attribute) is completed as that language
                id = nested.EmbeddedLanguageId; ownerStart = nested.Start; limit = nested.End; inline = nested.InlineDeclarations;
                language = _languages.Find(id);
            }
            int start = caret;
            while (start > ownerStart && IsWordChar(language, text[start - 1])) start--;
            int end = caret;
            while (end < limit && IsWordChar(language, text[end])) end++;

            if (!language.TryAdjustWordStart(text, ownerStart, ref start, caret)) return null;
            if (start < caret && !IsWordStart(language, text[start])) return null; // numbers, not words
            return new CompletionSite(id, start, caret, end, ownerStart, limit, inline);
        }

        // ---- what -----------------------------------------------------------------------------------------------

        public IReadOnlyList<Suggestion> Suggest(string text, CompletionSite site, CancellationToken cancellation = default(CancellationToken))
        {
            var result = new List<Suggestion>();
            if (text == null || site == null || site.Caret > text.Length) return result;

            string prefix = text.Substring(site.Start, site.PrefixLength);
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            ICompletionLanguage language = _languages.Find(site.EmbeddedLanguageId);
            bool upper = language.KeywordsFollowTypedCase && prefix.Length > 0 && !HasLower(prefix);

            // the place of the caret and the schema of the document come first, then the words that followed the same word before;
            // with the features off nothing is scanned before the keywords
            int[] scope = _features.SameLanguageWords ? CodeRanges(text, site) : null;
            Position place = _features.Grammar || _features.Schema ? language.PositionAt(text, site) : null;
            Position position = _features.Grammar ? place : null;
            List<string> schema = _features.Schema && place != null && place.Role != PlaceRole.None ? SchemaWords(language, text, site, place, scope) : null;
            if (schema != null && place.Role != PlaceRole.Table) AddCandidates(schema, prefix, seen, result);

            PreviousContext context = _features.PreviousWord ? ContextBefore(language, text, site) : default(PreviousContext);
            WordScan scan = null;
            if (context.Has)
            {
                scan = ScanWords(language, text, site, prefix, context, scope);
                AddFollowing(language, text, site, scan, upper, seen, result, _features.Order, _features.BlendWeight);
            }
            if (schema != null && place.Role == PlaceRole.Table) AddCandidates(schema, prefix, seen, result);

            // what the grammar expects at the caret, then (where the place says so) the words of the document, then the other keywords
            if (position != null) AddExpected(language, position.PriorOrder ? _features.OrderByUse(site.EmbeddedLanguageId, position.Expected) : position.Expected, prefix, upper, seen, result);
            bool wordsFirst = position != null ? position.WordsFirst : _features.WordsBeforeKeywords;
            if (wordsFirst && position == null && _features.HeadKeywords > 0)
            {
                // the most used keywords still go first where the words of the file do
                foreach (string word in _features.OrderKeywords(site.EmbeddedLanguageId, language.CompletionWords).Take(_features.HeadKeywords))
                {
                    if (result.Count >= _maxItems) return result;
                    if (!StartsWithIgnoreCase(word, prefix) || word.Length == prefix.Length) continue;
                    if (seen.Add(word)) result.Add(new Suggestion(upper ? word.ToUpperInvariant() : word, SuggestionKind.Keyword));
                }
            }
            if (wordsFirst) AddWords(language, text, site, prefix, context, scope, ref scan, seen, result);
            if (position != null && position.Secondary.Count > 0) AddExpected(language, position.Secondary, prefix, upper, seen, result);

            List<string> unlikely = null;
            foreach (string word in position != null && position.OnlyWords ? new string[0] : _features.OrderKeywords(site.EmbeddedLanguageId, language.CompletionWords))
            {
                if (result.Count >= _maxItems) return result;
                if (!StartsWithIgnoreCase(word, prefix) || word.Length == prefix.Length) continue;
                if (position != null && position.Unlikely != null && position.Unlikely(word))
                {
                    (unlikely ?? (unlikely = new List<string>())).Add(word);
                    continue;
                }
                if (seen.Add(word)) result.Add(new Suggestion(upper ? word.ToUpperInvariant() : word, SuggestionKind.Keyword));
            }

            if (!wordsFirst) AddWords(language, text, site, prefix, context, scope, ref scan, seen, result);

            if (unlikely != null)
                foreach (string word in unlikely)
                {
                    if (result.Count >= _maxItems) break;
                    if (seen.Add(word)) result.Add(new Suggestion(upper ? word.ToUpperInvariant() : word, SuggestionKind.Keyword));
                }

            if (_matcher != null && prefix.Length >= _features.FuzzyMinPrefix && result.Count < _fuzzyBelow && result.Count < _maxItems)
                AddSimilar(language, text, site, prefix, upper, seen, result, cancellation, position == null || !position.OnlyWords);
            return result;
        }

        private void AddWords(ICompletionLanguage language, string text, CompletionSite site, string prefix, PreviousContext context, int[] scope, ref WordScan scan,
            HashSet<string> seen, List<Suggestion> result)
        {
            if (scan == null) scan = ScanWords(language, text, site, prefix, context, scope);
            foreach (string word in OrderedWords(text, site, scan))
            {
                if (result.Count >= _maxItems) break;
                if (seen.Add(word)) result.Add(new Suggestion(word, SuggestionKind.Word));
            }
        }

        /// <summary>The words that the schema of the document says belong here, in order; null when the language has no schema or it knows nothing for this place.</summary>
        private List<string> SchemaWords(ICompletionLanguage language, string text, CompletionSite site, Position place, int[] scope)
        {
            var schema = language as ISchemaCompletion;
            if (schema == null) return null;
            int[] ranges = scope ?? CodeRanges(text, site);
            return schema.SchemaWords(text, site, place, ranges, Math.Max(0, site.Caret - WordScanWindow), Math.Min(text.Length, site.Caret + WordScanWindow));
        }

        private void AddCandidates(List<string> words, string prefix, HashSet<string> seen, List<Suggestion> result)
        {
            foreach (string word in words)
            {
                if (result.Count >= _maxItems) return;
                if (!StartsWithIgnoreCase(word, prefix) || word.Length == prefix.Length) continue;
                if (seen.Add(word)) result.Add(new Suggestion(word, SuggestionKind.Word));
            }
        }

        private void AddExpected(ICompletionLanguage language, IReadOnlyList<string> expectedWords, string prefix, bool upper, HashSet<string> seen, List<Suggestion> result)
        {
            foreach (string expected in expectedWords)
            {
                if (result.Count >= _maxItems) return;
                if (!StartsWithIgnoreCase(expected, prefix) || expected.Length == prefix.Length) continue;
                string word = language.FindCompletionWord(expected) ?? expected;
                if (seen.Add(word)) result.Add(new Suggestion(upper ? word.ToUpperInvariant() : word, SuggestionKind.Keyword));
            }
        }

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
                int hash = Hash(text, start, length);
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
            public readonly List<Match> Before = new List<Match>();
            public readonly List<Match> After = new List<Match>();
            /// <summary>With <see cref="CompletionFeatures.SameLanguageWords"/>, the matches outside the code of the strings of the language; they come after the others.</summary>
            public readonly List<Match> OtherBefore = new List<Match>();
            public readonly List<Match> OtherAfter = new List<Match>();
            /// <summary>With <see cref="CompletionFeatures.ShortWordsLast"/>, the words under the minimum length; they come after all the others.</summary>
            public readonly List<Match> ShortBefore = new List<Match>();
            public readonly List<Match> ShortAfter = new List<Match>();
            /// <summary>The matches that follow the same word and punctuation as the caret does (only with <see cref="CompletionFeatures.PreviousWord"/>).</summary>
            public readonly List<Match> Follows;
            public WordScan(bool context) { if (context) Follows = new List<Match>(); }
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
                var match = new Match(start, length);
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
        private static void AddFollowing(ICompletionLanguage language, string text, CompletionSite site, WordScan scan, bool upper, HashSet<string> seen, List<Suggestion> result, WordOrder order, double blendWeight)
        {
            int caret = site.Caret;
            IEnumerable<Match> ordered;
            if (order == WordOrder.Nearest)
            {
                scan.Follows.Sort((x, y) =>
                {
                    int nx = x.Start > caret ? x.Start - caret : caret - (x.Start + x.Length), ny = y.Start > caret ? y.Start - caret : caret - (y.Start + y.Length);
                    return nx != ny ? nx.CompareTo(ny) : x.Start.CompareTo(y.Start);
                });
                ordered = scan.Follows;
            }
            else ordered = Ranked(text, caret, new[] { scan.Follows }, order, blendWeight);
            foreach (Match m in ordered)
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
        /// The words of the scan, nearest to the caret first and each one once. The matches before and after the caret are already
        /// in order, so merging them gives the order by distance, and a word is only created when it is about to be offered: a caller
        /// that stops after the first N words never pays for the other thousands.
        /// </summary>
        private IEnumerable<string> OrderedWords(string text, CompletionSite site, WordScan scan)
        {
            var emitted = new HashSet<string>(StringComparer.Ordinal);
            var byHash = new Dictionary<int, string>();
            if (_features.Order != WordOrder.Nearest)
            {
                // by count and distance: one representative occurrence of each distinct word, in rank order, tier by tier
                for (int tier = 0; tier < 3; tier++)
                    foreach (Match m in Ranked(text, site.Caret, tier == 0 ? new[] { scan.Before, scan.After } : tier == 1 ? new[] { scan.OtherBefore, scan.OtherAfter } : new[] { scan.ShortBefore, scan.ShortAfter }, _features.Order, _features.BlendWeight))
                    {
                        string word = text.Substring(m.Start, m.Length);
                        if (emitted.Add(word)) yield return word;
                    }
                yield break;
            }

            // the words of the code of the language, then the others (empty without the feature)
            for (int tier = 0; tier < 3; tier++)
            {
                List<Match> before = tier == 0 ? scan.Before : tier == 1 ? scan.OtherBefore : scan.ShortBefore, after = tier == 0 ? scan.After : tier == 1 ? scan.OtherAfter : scan.ShortAfter;
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
        }

        private sealed class Counted
        {
            public Match First;
            public int Count, Near;
            public Counted Next;
            public double Score;
        }

        /// <summary>
        /// One occurrence of each distinct word of the lists (case matters), in the order of <paramref name="order"/>: the number of times
        /// it occurs and the distance from the caret to the nearest one decide. Ties go to the nearer, then to the earlier in the text,
        /// so that the order is the same every time.
        /// </summary>
        private static IEnumerable<Match> Ranked(string text, int caret, IEnumerable<List<Match>> lists, WordOrder order, double blendWeight)
        {
            var byHash = new Dictionary<int, Counted>();
            var all = new List<Counted>();
            foreach (List<Match> list in lists)
                foreach (Match m in list)
                {
                    int near = m.Start > caret ? m.Start - caret : caret - (m.Start + m.Length);
                    int hash = Hash(text, m.Start, m.Length);
                    Counted head;
                    byHash.TryGetValue(hash, out head);
                    Counted c = head;
                    while (c != null && !(c.First.Length == m.Length && string.CompareOrdinal(text, c.First.Start, text, m.Start, m.Length) == 0)) c = c.Next;
                    if (c == null)
                    {
                        c = new Counted { First = m, Near = near, Next = head };
                        byHash[hash] = c;
                        all.Add(c);
                    }
                    c.Count++;
                    if (near < c.Near) { c.Near = near; c.First = m; }
                }

            if (order == WordOrder.Blend)
                foreach (Counted c in all) c.Score = Math.Log(1 + c.Count) - blendWeight * Math.Log(1 + c.Near);
            all.Sort((x, y) =>
            {
                if (order == WordOrder.Frequency) { if (x.Count != y.Count) return y.Count.CompareTo(x.Count); }
                else if (x.Score != y.Score) return y.Score.CompareTo(x.Score);
                if (x.Near != y.Near) return x.Near.CompareTo(y.Near);
                return x.First.Start.CompareTo(y.First.Start);
            });
            foreach (Counted c in all) yield return c.First;
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

        private static bool IsWordStart(ICompletionLanguage language, char c)
        {
            return char.IsLetter(c) || c == '_' || language.IsExtraWordChar(c);
        }

        private static bool IsWordChar(ICompletionLanguage language, char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || language.IsExtraWordChar(c);
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
