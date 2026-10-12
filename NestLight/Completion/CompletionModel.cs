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
}
