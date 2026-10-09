using System;
using System.Collections.Generic;
using System.Linq;

namespace NestLight.Completion
{
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
            wordsBeforeKeywords: true, keywordPriority: true, headKeywords: 12, shortWordsLast: true);

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
        /// inside <c>&lt;button </c> its attributes, and the words that do not belong there come last (see <see cref="ICompletionLanguage.PositionAt"/>).
        /// </summary>
        public readonly bool Grammar;

        /// <summary>
        /// The tables and columns the SQL of the document talks about (a <c>CREATE TABLE</c>, the <c>FROM</c> of the statement, an alias) tell
        /// which table to offer after <c>FROM</c> and which columns after <c>u.</c> or in the select list. It needs the place of the caret,
        /// which is computed even when <see cref="Grammar"/> is off.
        /// </summary>
        public readonly bool Schema;

        /// <summary>The order of the words of the document among themselves (and of the words that followed the context); see <see cref="WordRankers"/>.</summary>
        public readonly IWordRanker Ranker;

        /// <summary>
        /// Where the place of the caret says nothing (a language without rules, or a place the rules do not know), the words of the
        /// document come before the keywords of the language.
        /// </summary>
        public readonly bool WordsBeforeKeywords;

        /// <summary>
        /// The keywords are offered in the order each language knows them by use (<see cref="ICompletionLanguage.CompletionWordsByUse"/>)
        /// instead of the alphabetical one; the ones the language has no use for follow, alphabetically.
        /// </summary>
        public readonly bool KeywordPriority;

        /// <summary>
        /// With <see cref="WordsBeforeKeywords"/>, how many of the most used keywords (see <see cref="KeywordPriority"/>) still come before the
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

        /// <summary>The keywords of the language in the order they are offered.</summary>
        public IReadOnlyList<string> OrderKeywords(ICompletionLanguage language)
        {
            return KeywordPriority ? language.CompletionWordsByUse : language.CompletionWords;
        }

        /// <summary>A list of words of the language (the properties of CSS) with the most used first, when <see cref="KeywordPriority"/> is on; the rest keep their order.</summary>
        public IReadOnlyList<string> OrderByUse(ICompletionLanguage language, IReadOnlyList<string> words)
        {
            return KeywordPriority ? language.OrderByUse(words) : words;
        }

        public CompletionFeatures(bool previousWord = false, bool sameLanguageWords = false, bool grammar = false, bool schema = false,
            IWordRanker ranker = null, bool wordsBeforeKeywords = false,
            bool keywordPriority = false, int headKeywords = 0,
            bool shortWordsLast = false, int fuzzyMinPrefix = 3, int shortSimilarCap = 0, bool shortSimilarFromFileOnly = false)
        {
            ShortWordsLast = shortWordsLast;
            FuzzyMinPrefix = fuzzyMinPrefix;
            ShortSimilarCap = shortSimilarCap;
            ShortSimilarFromFileOnly = shortSimilarFromFileOnly;
            HeadKeywords = headKeywords;
            WordsBeforeKeywords = wordsBeforeKeywords;
            KeywordPriority = keywordPriority;
            Ranker = ranker ?? WordRankers.Nearest;
            PreviousWord = previousWord;
            SameLanguageWords = sameLanguageWords;
            Grammar = grammar;
            Schema = schema;
        }
    }
}
