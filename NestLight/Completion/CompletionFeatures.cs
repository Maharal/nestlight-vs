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
        public static readonly CompletionFeatures Default = new CompletionFeatures(previousWord: true);

        /// <summary>
        /// The words that already followed the same word (and the same punctuation) elsewhere in the document come first:
        /// after <c>FROM </c> the word that followed <c>FROM</c> before, after <c>display: </c> the value used after <c>display:</c>.
        /// </summary>
        public readonly bool PreviousWord;

        public CompletionFeatures(bool previousWord = false)
        {
            PreviousWord = previousWord;
        }
    }
}
