using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Completion
{
    /// <summary>
    /// What the completion knows about one embedded language: its words, what counts as a word, and what the grammar says about the place
    /// of the caret. The engine asks the strategy of the language of the string and never looks at the id, so a new language is a new class
    /// and the engine does not change. A language without a grammar (GraphQL, XML, Markdown, regular expressions) offers its words and the
    /// words of the document and nothing else.
    /// </summary>
    internal interface ICompletionLanguage
    {
        /// <summary>Lower-case ids and aliases that select this language: the same as the ones of its tokenizer.</summary>
        IReadOnlyList<string> Ids { get; }

        /// <summary>The words the tokenizer colors, sorted; empty when the language has no closed vocabulary. The same list every time.</summary>
        IReadOnlyList<string> Keywords { get; }

        /// <summary>The words the completion offers: <see cref="Keywords"/> and the ones the tokenizer does not color (<c>main</c>), sorted.</summary>
        IReadOnlyList<string> CompletionWords { get; }

        /// <summary>
        /// <see cref="CompletionWords"/> from the most used to the least used in the code people write in the language, the rest alphabetically
        /// (see <see cref="KeywordUse"/>). The same list as <see cref="CompletionWords"/> when the language has no such order. The same list every time.
        /// </summary>
        IReadOnlyList<string> CompletionWordsByUse { get; }

        /// <summary>
        /// A list of words of the language that is long and alphabetical (the properties of CSS) with the most used first and the rest in
        /// their order. The words that are not in the use order of the language stay where they are; a repeated word is placed once.
        /// </summary>
        IReadOnlyList<string> OrderByUse(IReadOnlyList<string> words);

        /// <summary>The keyword spelled like the word, ignoring case, in the spelling of the vocabulary; null when there is none.</summary>
        string FindKeyword(string word);

        /// <summary>Like <see cref="FindKeyword"/>, over <see cref="CompletionWords"/>.</summary>
        string FindCompletionWord(string word);

        /// <summary>The language does not tell cases apart (SQL), so a keyword follows the case the user is typing.</summary>
        bool KeywordsFollowTypedCase { get; }

        /// <summary>Whether the character, besides letters, digits and the underscore, belongs to a word of the language (the dash of CSS).</summary>
        bool IsExtraWordChar(char c);

        /// <summary>
        /// Lets the language correct where the word under the caret starts (the unit that follows a number in CSS), or refuse the place
        /// (a hex color). False: nothing is completed here.
        /// </summary>
        bool TryAdjustWordStart(string text, int ownerStart, ref int start, int caret);

        /// <summary>What the grammar says about the place of the caret, or null when the language has no grammar here.</summary>
        Position PositionAt(string text, CompletionSite site);
    }

    /// <summary>A language that has the schema of the document in its code, which tells which names exist (the tables and columns of SQL).</summary>
    internal interface ISchemaCompletion
    {
        /// <summary>
        /// The names the document says belong at the place, in order; null when it knows nothing for this place.
        /// <paramref name="ranges"/> are the sorted, disjoint [start, end) pairs of the code of the language, flat.
        /// </summary>
        List<string> SchemaWords(string text, CompletionSite site, Position place, int[] ranges, int from, int to);
    }

    /// <summary>A language whose strings hold code of another language (the CSS of a <c>style</c> attribute in HTML).</summary>
    internal interface INestedLanguages
    {
        /// <summary>The nested region the caret is in, or false when it is in the code of the language itself.</summary>
        bool TryRegionAt(string text, EmbeddedString owner, int caret, out NestedRegion region);

        /// <summary>The nested regions of the string, in order of appearance.</summary>
        IReadOnlyList<NestedRegion> RegionsIn(string text, EmbeddedString owner);
    }

    /// <summary>The languages the completion knows, by id.</summary>
    internal interface ICompletionLanguages
    {
        /// <summary>The strategy for the id; a language with nothing special for an id the completion does not know. Never null.</summary>
        ICompletionLanguage Find(string embeddedLanguageId);

        /// <summary>Whether two ids name the same language (<c>html</c> and <c>svg</c> are one, <c>yaml</c> and <c>yml</c> too).</summary>
        bool Same(string a, string b);
    }
}
