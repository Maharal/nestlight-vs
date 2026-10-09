namespace NestLight.Completion
{
    /// <summary>What the icon of a suggestion in the list says it is.</summary>
    internal enum SuggestionIcon
    {
        /// <summary>A word of the language of the string.</summary>
        Keyword,
        /// <summary>A word that is in the document.</summary>
        Word
    }

    /// <summary>Which icon a suggestion has. The editor draws it; this only decides, so that the decision can be tested without the editor.</summary>
    internal static class SuggestionIcons
    {
        public static SuggestionIcon Of(Suggestion suggestion)
        {
            return suggestion.Kind == SuggestionKind.Keyword ? SuggestionIcon.Keyword : SuggestionIcon.Word;
        }

        /// <summary>A suggestion that does not start with what was typed, but is a few edits away from it: it gets a small icon of its own beside the main one.</summary>
        public static bool IsSimilar(Suggestion suggestion)
        {
            return suggestion.Distance > 0;
        }
    }
}
