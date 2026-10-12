using System;
using NestLight.Common;

namespace NestLight.Hosts
{
    /// <summary>The <c>//</c> and <c>/* */</c> comments that C#, C++, Go, Java and Rust share: they are where a marker can be.</summary>
    internal static class CStyleComments
    {
        /// <summary>
        /// When a comment starts at <paramref name="i"/>, tells the markers about it, moves <paramref name="i"/> past it and returns true.
        /// An unterminated block comment runs to the end of the text and is never a marker.
        /// </summary>
        public static bool TrySkip(string t, ref int i, MarkerTracker markers)
        {
            if (t[i] != '/' || i + 1 >= t.Length) return false;
            if (t[i + 1] == '/')
            {
                int e = t.IndexOf('\n', i);
                int end = e < 0 ? t.Length : e;
                markers.Comment(t, i + 2, end, end);
                i = end;
                return true;
            }
            if (t[i + 1] == '*')
            {
                int e = t.IndexOf("*/", i + 2, StringComparison.Ordinal);
                if (e < 0) { markers.Comment(t, 0, 0, t.Length); i = t.Length; }
                else { markers.Comment(t, i + 2, e, e + 2); i = e + 2; }
                return true;
            }
            return false;
        }
    }
}
