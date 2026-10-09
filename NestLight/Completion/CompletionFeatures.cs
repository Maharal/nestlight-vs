using System;
using System.Collections.Generic;
using System.Linq;

namespace NestLight.Completion
{
    /// <summary>How the words of the document are ordered among themselves.</summary>
    internal enum WordOrder
    {
        /// <summary>The nearest occurrence to the caret first.</summary>
        Nearest,
        /// <summary>The most frequent first, the nearest among equals.</summary>
        Frequency,
        /// <summary><c>ln(1 + count) - weight * ln(1 + distance)</c>: a word used often counts, a word used far away counts less.</summary>
        Blend
    }

    /// <summary>
    /// The context-aware ways of ranking the suggestions, each one on its own so that an experiment can turn it on or off. With
    /// none of them the engine ranks only by what was typed: the keywords, then the words of the document nearest to the caret first.
    /// </summary>
    internal sealed class CompletionFeatures
    {
        /// <summary>No feature: the ranking of the first stage.</summary>
        public static readonly CompletionFeatures None = new CompletionFeatures();

        /// <summary>The features the plugin runs with: the ones whose experiment met its criterion.</summary>
        public static readonly CompletionFeatures Default = new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true,
            wordsBeforeKeywords: true, keywordPriors: KeywordUse.Default, headKeywords: 12, shortWordsLast: true);

        /// <summary>
        /// The words that already followed the same word (and the same punctuation) elsewhere in the document come first:
        /// after <c>FROM </c> the word that followed <c>FROM</c> before, after <c>display: </c> the value used after <c>display:</c>.
        /// </summary>
        public readonly bool PreviousWord;

        /// <summary>
        /// The words found in the code of embedded strings of the same language as the caret's come before the other words of the
        /// document (the host code, the strings of other languages): a column name written in some other SQL string is likelier than
        /// a variable that happens to start with the same letters.
        /// </summary>
        public readonly bool SameLanguageWords;

        /// <summary>
        /// The place of the caret in the grammar decides what comes first: after <c>FROM</c> a table, after <c>display:</c> its values,
        /// inside <c>&lt;button </c> its attributes, and the words that do not belong there come last (see <see cref="Positions"/>).
        /// </summary>
        public readonly bool Grammar;

        /// <summary>
        /// The tables and columns the SQL of the document talks about (a <c>CREATE TABLE</c>, the <c>FROM</c> of the statement, an alias) tell
        /// which table to offer after <c>FROM</c> and which columns after <c>u.</c> or in the select list. It needs the place of the caret,
        /// which is computed even when <see cref="Grammar"/> is off.
        /// </summary>
        public readonly bool Schema;

        /// <summary>The order of the words of the document among themselves (and of the words that followed the context).</summary>
        public readonly WordOrder Order;

        /// <summary>For <see cref="WordOrder.Blend"/>: how much the distance weighs against the count (0: the count alone).</summary>
        public readonly double BlendWeight;

        /// <summary>
        /// Where the place of the caret says nothing (a language without rules, or a place the rules do not know), the words of the
        /// document come before the keywords of the language.
        /// </summary>
        public readonly bool WordsBeforeKeywords;

        /// <summary>
        /// For each language, its keywords from the most used to the least used. The keywords are offered in this order instead of the
        /// alphabetical one; the ones that are not in the list follow, alphabetically. Null: alphabetical.
        /// </summary>
        public readonly IReadOnlyDictionary<string, IReadOnlyList<string>> KeywordPriors;

        /// <summary>
        /// With <see cref="WordsBeforeKeywords"/>, how many of the most used keywords (see <see cref="KeywordPriors"/>) still come before the
        /// words of the document: <c>true</c>, <c>false</c> and <c>null</c> in JSON, <c>float</c> and <c>uniform</c> in a shader.
        /// </summary>
        public readonly int HeadKeywords;

        /// <summary>Words of two letters (<c>id</c>, <c>db</c>, <c>in</c>) are offered too, after all the longer words.</summary>
        public readonly bool ShortWordsLast;

        /// <summary>The shortest prefix the similar words are looked for with (<see cref="CompletionEngine.FuzzyMinPrefix"/> by default).</summary>
        public readonly int FuzzyMinPrefix;

        /// <summary>For a prefix of 3 letters or less, the most similar words offered; 0: as many as for the longer ones.</summary>
        public readonly int ShortSimilarCap;

        /// <summary>For a prefix of 3 letters or less, similar words come from the document only, not from the keywords of the language.</summary>
        public readonly bool ShortSimilarFromFileOnly;

        private readonly Dictionary<string, IReadOnlyList<string>> _ordered = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>The keywords of the language in the order they are offered.</summary>
        public IReadOnlyList<string> OrderKeywords(string languageId, IReadOnlyList<string> alphabetical)
        {
            IReadOnlyList<string> prior;
            if (KeywordPriors == null || languageId == null || !KeywordPriors.TryGetValue(languageId, out prior)) return alphabetical;
            lock (_ordered)
            {
                IReadOnlyList<string> ordered;
                if (_ordered.TryGetValue(languageId, out ordered)) return ordered;
                var set = new HashSet<string>(alphabetical, StringComparer.OrdinalIgnoreCase);
                var list = new List<string>();
                var placed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string word in prior) if (set.Contains(word) && placed.Add(word)) list.Add(alphabetical.First(w => string.Equals(w, word, StringComparison.OrdinalIgnoreCase)));
                foreach (string word in alphabetical) if (placed.Add(word)) list.Add(word);
                _ordered[languageId] = list;
                return list;
            }
        }

        private readonly Dictionary<IReadOnlyList<string>, IReadOnlyList<string>> _byUse = new Dictionary<IReadOnlyList<string>, IReadOnlyList<string>>();

        /// <summary>A list of words of the language (the properties of CSS) with the ones in <see cref="KeywordPriors"/> first, in that order; the rest keep their order.</summary>
        public IReadOnlyList<string> OrderByUse(string languageId, IReadOnlyList<string> words)
        {
            IReadOnlyList<string> prior;
            if (KeywordPriors == null || languageId == null || !KeywordPriors.TryGetValue(languageId, out prior)) return words;
            lock (_byUse)
            {
                IReadOnlyList<string> ordered;
                if (_byUse.TryGetValue(words, out ordered)) return ordered;
                var set = new HashSet<string>(words, StringComparer.OrdinalIgnoreCase);
                var list = new List<string>();
                var placed = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (string word in prior) if (set.Contains(word) && placed.Add(word)) list.Add(words.First(w => string.Equals(w, word, StringComparison.OrdinalIgnoreCase)));
                foreach (string word in words) if (placed.Add(word)) list.Add(word);
                _byUse[words] = list;
                return list;
            }
        }

        public CompletionFeatures(bool previousWord = false, bool sameLanguageWords = false, bool grammar = false, bool schema = false,
            WordOrder order = WordOrder.Nearest, double blendWeight = 0.5, bool wordsBeforeKeywords = false,
            IReadOnlyDictionary<string, IReadOnlyList<string>> keywordPriors = null, int headKeywords = 0,
            bool shortWordsLast = false, int fuzzyMinPrefix = 3, int shortSimilarCap = 0, bool shortSimilarFromFileOnly = false)
        {
            ShortWordsLast = shortWordsLast;
            FuzzyMinPrefix = fuzzyMinPrefix;
            ShortSimilarCap = shortSimilarCap;
            ShortSimilarFromFileOnly = shortSimilarFromFileOnly;
            HeadKeywords = headKeywords;
            WordsBeforeKeywords = wordsBeforeKeywords;
            KeywordPriors = keywordPriors;
            Order = order;
            BlendWeight = blendWeight;
            PreviousWord = previousWord;
            SameLanguageWords = sameLanguageWords;
            Grammar = grammar;
            Schema = schema;
        }
    }
}
