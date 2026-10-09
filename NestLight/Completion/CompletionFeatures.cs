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
        public static readonly CompletionFeatures Default = new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true);

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

        public CompletionFeatures(bool previousWord = false, bool sameLanguageWords = false, bool grammar = false, bool schema = false,
            WordOrder order = WordOrder.Nearest, double blendWeight = 0.5)
        {
            Order = order;
            BlendWeight = blendWeight;
            PreviousWord = previousWord;
            SameLanguageWords = sameLanguageWords;
            Grammar = grammar;
            Schema = schema;
        }
    }
}
