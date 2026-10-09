using NestLight.Completion;

namespace NestLight.Experiments
{
    internal sealed class EA30_ContextRobustness : EA24_SimilarRobustness
    {
        public override string Id { get { return "EA30"; } }
        public override string Title { get { return "Completion with the context rankings on incomplete and cut code"; } }
        public override string Hypothesis { get { return "EA24 again, with every context feature on (the previous word, the words of the language, the grammar of SQL, CSS and HTML, the schema of the SQL, the blend of count and distance) over the structured files, which have the statements, the rules and the tags those features read. The look-behind of the grammar and of the schema, the pointers into the ranges of the strings, the ranked words and the short words offered after a context must not throw on a text cut anywhere, must not offer a word twice, must not break the limit, and the exact suggestions must still start with what was typed."; } }
        public override string Method { get { return "EA24's test (every prefix cut at a stride, every single-character deletion at a stride, the caret at the end, at the start and at 5 random positions, and the same position with a mistake in the word), over 50 structured files, with every feature of CompletionFeatures on."; } }
        public override string IfMet { get { return "Completion can be triggered anywhere in a file being edited, with the context rankings on."; } }

        protected override CompletionFeatures Features { get { return new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true, schema: true, order: WordOrder.Blend, wordsBeforeKeywords: true, keywordPriors: KeywordUse.Default, headKeywords: 12, shortWordsLast: true); } }
        protected override bool Structured { get { return true; } }
    }
}
