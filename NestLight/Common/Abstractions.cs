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
