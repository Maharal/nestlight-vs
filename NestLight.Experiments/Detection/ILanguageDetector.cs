using System;

namespace NestLight.Experiments.Detection
{
    /// <summary>
    /// One strategy of the automatic detector: how sure it is that a string holds code of one language. It reads the text where it is
    /// (no copy, no allocation), so the cost of asking is the cost of the heuristic alone.
    /// </summary>
    internal interface ILanguageDetector
    {
        /// <summary>The language id the strategy answers for (the id of its tokenizer).</summary>
        string Id { get; }

        /// <summary>A cheap look at the first character: false rules the language out without reading the rest.</summary>
        bool CanStartWith(char first);

        /// <summary>0: not this language. The higher, the surer. The text is trimmed and not shorter than the detector's minimum.</summary>
        int Score(string text, int start, int end);
    }

    /// <summary>Allocation-free reading helpers shared by the strategies.</summary>
    internal static class Probe
    {
        public static bool StartsWithWord(string t, int start, int end, string word)
        {
            if (end - start <= word.Length) return false;
            if (string.Compare(t, start, word, 0, word.Length, StringComparison.OrdinalIgnoreCase) != 0) return false;
            return !char.IsLetterOrDigit(t[start + word.Length]) && t[start + word.Length] != '_';
        }

        public static bool Contains(string t, int start, int end, string needle)
        {
            return t.IndexOf(needle, start, end - start, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static int IndexOf(string t, int start, int end, char c)
        {
            return t.IndexOf(c, start, end - start);
        }

        public static char NextSignificant(string t, int from, int end)
        {
            for (int i = from; i < end; i++) if (!char.IsWhiteSpace(t[i])) return t[i];
            return '\0';
        }
    }
}
