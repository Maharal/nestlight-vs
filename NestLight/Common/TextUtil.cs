using System;

namespace NestLight.Common
{
    /// <summary>Character helpers shared by scanners and tokenizers.</summary>
    internal static class TextUtil
    {
        /// <summary>Stands in for the characters of an interpolation while a tokenizer runs.</summary>
        public const char Mask = '\u0001';

        public static bool IsWordStart(char c)
        {
            return char.IsLetter(c) || c == '_' || c == Mask;
        }

        public static bool IsWordChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == Mask;
        }

        public static bool StartsWith(char[] m, int i, int end, string s)
        {
            if (i < 0 || i + s.Length > end) return false;
            for (int k = 0; k < s.Length; k++) if (m[i + k] != s[k]) return false;
            return true;
        }

        public static bool StartsWithIgnoreCase(char[] m, int i, int end, string lowerCase)
        {
            if (i < 0 || i + lowerCase.Length > end) return false;
            for (int k = 0; k < lowerCase.Length; k++)
                if (char.ToLowerInvariant(m[i + k]) != lowerCase[k]) return false;
            return true;
        }

        /// <summary>Index of <paramref name="s"/> in m[from, end), or -1.</summary>
        public static int IndexOf(char[] m, string s, int from, int end)
        {
            for (int i = from; i + s.Length <= end; i++) if (StartsWith(m, i, end, s)) return i;
            return -1;
        }

        public static int IndexOfIgnoreCase(char[] m, string lowerCase, int from, int end)
        {
            for (int i = from; i + lowerCase.Length <= end; i++)
                if (StartsWithIgnoreCase(m, i, end, lowerCase)) return i;
            return -1;
        }

        /// <summary>Index of the line break that ends the line of <paramref name="i"/>, or <paramref name="end"/>.</summary>
        public static int LineEnd(char[] m, int i, int end)
        {
            while (i < end && m[i] != '\n' && m[i] != '\r') i++;
            return i;
        }

        public static int SkipBlanks(char[] m, int i, int end)
        {
            while (i < end && (m[i] == ' ' || m[i] == '\t')) i++;
            return i;
        }

        public static bool EqualsIgnoreCase(char[] m, int start, int end, string s)
        {
            if (end - start != s.Length) return false;
            for (int k = 0; k < s.Length; k++)
                if (char.ToLowerInvariant(m[start + k]) != char.ToLowerInvariant(s[k])) return false;
            return true;
        }

        public static string Substring(char[] m, int start, int end)
        {
            return new string(m, start, Math.Max(0, end - start));
        }
    }
}
