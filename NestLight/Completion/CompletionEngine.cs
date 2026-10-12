using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NestLight.Common;

namespace NestLight.Completion
{
    /// <summary>
    /// Completion inside embedded strings, from two sources: the keywords of the language of the string
    /// (<see cref="ICompletionLanguage"/>) and the words that already exist in the document, nearest to the caret first
    /// (what Visual Studio Code calls word-based suggestions).
    /// </summary>
    /// <remarks>
    /// When a matcher is given and nothing (fewer than <c>fuzzyBelow</c> items) starts with what was typed, a second stage offers
    /// the keywords and the words of the document that are a few edits away from it, after the exact ones. With no matcher the
    /// engine is exactly the first stage.
    /// </remarks>
    internal sealed partial class CompletionEngine : ICompletionProvider
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
                AddFollowing(language, text, site, scan, upper, seen, result, _features.Ranker);
            }
            if (schema != null && place.Role == PlaceRole.Table) AddCandidates(schema, prefix, seen, result);

            // what the grammar expects at the caret, then (where the place says so) the words of the document, then the other keywords
            if (position != null) AddExpected(language, position.PriorOrder ? _features.OrderByUse(language, position.Expected) : position.Expected, prefix, upper, seen, result);
            bool wordsFirst = position != null ? position.WordsFirst : _features.WordsBeforeKeywords;
            if (wordsFirst && position == null && _features.HeadKeywords > 0)
            {
                // the most used keywords still go first where the words of the file do
                foreach (string word in _features.OrderKeywords(language).Take(_features.HeadKeywords))
                {
                    if (result.Count >= _maxItems) return result;
                    if (!StartsWithIgnoreCase(word, prefix) || word.Length == prefix.Length) continue;
                    if (seen.Add(word)) result.Add(new Suggestion(upper ? word.ToUpperInvariant() : word, SuggestionKind.Keyword));
                }
            }
            if (wordsFirst) AddWords(language, text, site, prefix, context, scope, ref scan, seen, result);
            if (position != null && position.Secondary.Count > 0) AddExpected(language, position.Secondary, prefix, upper, seen, result);

            List<string> unlikely = null;
            foreach (string word in position != null && position.OnlyWords ? new string[0] : _features.OrderKeywords(language))
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
