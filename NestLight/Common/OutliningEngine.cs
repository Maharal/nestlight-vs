using System.Collections.Generic;

namespace NestLight.Common
{
    /// <summary>A range of text the programmer can collapse: [Start, End).</summary>
    internal struct OutlineRegion
    {
        public readonly int Start;
        public readonly int End;

        public OutlineRegion(int start, int end)
        {
            Start = start; End = end;
        }
    }

    /// <summary>
    /// Decides what can be collapsed in a host file: the content of each embedded string that takes several lines. The line of the opening
    /// quote stays visible (it says what the string is, like the header of a method) and so does the closing quote.
    /// </summary>
    internal static class OutliningEngine
    {
        /// <summary>The content must have at least this many line breaks, or collapsing it would hide almost nothing.</summary>
        private const int MinLineBreaks = 2;

        /// <summary>In source order, as the strings are. Strings nobody recognized (no language) are not outlined.</summary>
        public static IReadOnlyList<OutlineRegion> Regions(IReadOnlyList<EmbeddedString> strings, string text)
        {
            var result = new List<OutlineRegion>();
            foreach (EmbeddedString s in strings)
            {
                if (s.EmbeddedLanguageId == null) continue;
                int end = s.End < text.Length ? s.End : text.Length;
                int firstBreak = -1, breaks = 0;
                for (int i = s.Start; i < end; i++)
                {
                    if (text[i] != '\n') continue;
                    if (firstBreak < 0) firstBreak = i;
                    breaks++;
                }
                if (breaks < MinLineBreaks) continue;
                // A "\r\n" break is hidden whole: the range starts before its "\r".
                int start = firstBreak > s.Start && text[firstBreak - 1] == '\r' ? firstBreak - 1 : firstBreak;
                result.Add(new OutlineRegion(start, end));
            }
            return result;
        }
    }
}
