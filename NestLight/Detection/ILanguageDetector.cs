using System;

namespace NestLight.Detection
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

        public static bool ContainsWord(string t, int start, int end, string word)
        {
            int pos = start;
            while (pos < end)
            {
                int found = t.IndexOf(word, pos, end - pos, StringComparison.OrdinalIgnoreCase);
                if (found < 0) return false;
                bool leftOk = found == start || t[found - 1] == ' ' || t[found - 1] == '\n' || t[found - 1] == '\r' || t[found - 1] == '\t' || t[found - 1] == ',';
                int after = found + word.Length;
                bool rightOk = after == end || t[after] == ' ' || t[after] == '\n' || t[after] == '\r' || t[after] == '\t' || t[after] == ',';
                if (leftOk && rightOk) return true;
                pos = found + 1;
            }
            return false;
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

        public static int SkipCStyleComments(string t, int start, int end)
        {
            int i = start;
            while (i + 1 < end)
            {
                if (char.IsWhiteSpace(t[i])) { i++; continue; }
                if (t[i] == '/' && t[i + 1] == '*')
                {
                    int close = t.IndexOf("*/", i + 2, end - i - 2, StringComparison.Ordinal);
                    if (close < 0) return end;
                    i = close + 2;
                    continue;
                }
                break;
            }
            return i;
        }

        public static int SkipLineComments(string t, int start, int end, char marker)
        {
            int i = start;
            while (i < end)
            {
                if (char.IsWhiteSpace(t[i])) { i++; continue; }
                if (t[i] == marker)
                {
                    int nl = t.IndexOf('\n', i, end - i);
                    i = nl < 0 ? end : nl + 1;
                    continue;
                }
                break;
            }
            return i;
        }
    }
}
