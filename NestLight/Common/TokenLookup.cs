using System.Collections.Generic;

namespace NestLight.Common
{
    internal static class TokenLookup
    {
        /// <summary>Finds the token that contains <paramref name="position"/>, or null. Uses binary search.</summary>
        public static Token AtPosition(IReadOnlyList<Token> tokens, int position)
        {
            int lo = 0, hi = tokens.Count;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (tokens[mid].End <= position) lo = mid + 1;
                else hi = mid;
            }
            if (lo < tokens.Count && tokens[lo].Start <= position && tokens[lo].End > position)
                return tokens[lo];
            return null;
        }

        /// <summary>Finds the last token before <paramref name="position"/> whose type matches one of the given types.</summary>
        public static Token PrecedingOfType(IReadOnlyList<Token> tokens, int position, params string[] types)
        {
            int lo = 0, hi = tokens.Count;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (tokens[mid].End <= position) lo = mid + 1;
                else hi = mid;
            }
            for (int k = lo - 1; k >= 0; k--)
            {
                if (tokens[k].End > position) continue;
                for (int t = 0; t < types.Length; t++)
                    if (tokens[k].Type == types[t]) return tokens[k];
                break;
            }
            return null;
        }

        /// <summary>Finds the first token after <paramref name="current"/> that has the given type and text (case-insensitive).</summary>
        public static Token FindDefinition(IReadOnlyList<Token> tokens, string text, Token current, string targetType, string sourceText)
        {
            for (int k = 0; k < tokens.Count; k++)
            {
                Token t = tokens[k];
                if (t.Type != targetType) continue;
                if (t.Start == current.Start && t.Length == current.Length) continue;
                if (t.Start + t.Length <= sourceText.Length)
                {
                    string candidate = sourceText.Substring(t.Start, t.Length);
                    if (string.Equals(candidate, text, System.StringComparison.OrdinalIgnoreCase))
                        return t;
                }
            }
            return null;
        }
    }
}
