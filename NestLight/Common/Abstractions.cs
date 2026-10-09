using System.Collections.Generic;

namespace NestLight.Common
{
    /// <summary>Receives a token [start, end) of a given classification type.</summary>
    internal delegate void TokenSink(int start, int end, string type);

    /// <summary>Finds the embedded strings of a host language (JavaScript, C#, Python, C++...).</summary>
    internal interface IHostScanner
    {
        /// <summary>Strings marked as embedded code, ordered by <see cref="EmbeddedString.OuterStart"/>. Never throws on incomplete code.</summary>
        IReadOnlyList<EmbeddedString> Scan(string text);
    }

    /// <summary>Tokenizes the code of an embedded language.</summary>
    internal interface IEmbeddedLanguageTokenizer
    {
        /// <summary>Lower-case ids and aliases that select this language.</summary>
        IReadOnlyList<string> Ids { get; }

        /// <summary>
        /// Tokenizes text[from, to). Interpolations were already replaced by <see cref="TextUtil.Mask"/>
        /// and escape sequences decoded; the tokens come back in the coordinates of <paramref name="text"/>.
        /// </summary>
        void Tokenize(char[] text, int from, int to, TokenSink emit);
    }

    /// <summary>A part of the code of an embedded language that is code of another embedded language (the CSS of a <c>style</c> attribute in HTML).</summary>
    internal struct NestedRegion
    {
        public string EmbeddedLanguageId;
        public int Start, End;
        /// <summary>The region is a list of declarations, not a whole style sheet (a <c>style</c> attribute).</summary>
        public bool InlineDeclarations;
    }

    /// <summary>Receives a region of code of another language.</summary>
    internal delegate void RegionSink(NestedRegion region);

    /// <summary>
    /// A tokenizer whose language holds code of other languages. The place where that code is has one definition, in the tokenizer: the
    /// tokenizer hands each region to the tokenizer of its language, and whoever else needs to know where the regions are (the completion,
    /// to complete them as the language they are) asks the same tokenizer, so the two cannot disagree.
    /// </summary>
    internal interface INestingTokenizer : IEmbeddedLanguageTokenizer
    {
        /// <summary>
        /// The regions of other languages in text[from, to), in order of appearance, found by the same rules as <see cref="IEmbeddedLanguageTokenizer.Tokenize"/>
        /// (interpolations masked): the ones <c>Tokenize</c> delegates, and the empty ones too. An unclosed region runs to <paramref name="to"/>.
        /// </summary>
        IReadOnlyList<NestedRegion> FindRegions(char[] text, int from, int to);
    }

    /// <summary>Maps language ids and aliases to tokenizers.</summary>
    internal interface IEmbeddedLanguageRegistry
    {
        bool IsKnown(string id);

        /// <summary>The tokenizer for the id, or null when unknown. Ids are case-insensitive.</summary>
        IEmbeddedLanguageTokenizer Find(string id);
    }

    /// <summary>Turns a text into classification tokens.</summary>
    internal interface IHighlighter
    {
        IReadOnlyList<Token> Highlight(string text);
    }
}
