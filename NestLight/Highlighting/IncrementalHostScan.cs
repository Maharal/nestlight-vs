using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Highlighting
{
    /// <summary>
    /// The scan of an edited text made from the scan of the text before it: the strings before the edit are the same, the ones after it
    /// are the same moved by the length of the edit, and only the lines around the edit are scanned again.
    /// </summary>
    /// <remarks>
    /// The scan starts at a safe point before the edit (see <see cref="HostScan.SafePoints"/>), one more back than the last one, and goes on
    /// until it reaches a safe point after the edit that was a safe point of the old text too: from there the text is the same and so is
    /// everything the scanner finds in it. If it never does, it reads to the end, which is a whole scan.
    /// </remarks>
    internal static class IncrementalHostScan
    {
        public static HostScan Update(IResumableHostScanner scanner, HostScan previous, string oldText, string text)
        {
            int prefix = TextDiff.CommonPrefix(oldText, text);
            if (prefix == oldText.Length && prefix == text.Length) return previous; // another instance of the same text

            int suffix = TextDiff.CommonSuffix(oldText, text, prefix);
            int editEnd = text.Length - suffix, delta = text.Length - oldText.Length;
            int[] safe = previous.SafePoints;

            // the scan starts one safe point before the last one that is not after the edit
            int notAfter = UpperBound(safe, prefix);
            int firstSafe = notAfter - 2;
            int from = firstSafe >= 0 ? safe[firstSafe] : 0;
            int kept = Math.Max(0, firstSafe);

            HostScan middle = scanner.Resume(text, from, editEnd, position => Contains(safe, position - delta));

            IReadOnlyList<EmbeddedString> old = previous.Strings;
            int before = LowerBound(old, from);
            int resumedAt = middle.StoppedAt >= 0 ? middle.StoppedAt - delta : -1;
            int after = resumedAt >= 0 ? LowerBound(old, resumedAt) : old.Count;

            var strings = new List<EmbeddedString>(before + middle.Strings.Count + (old.Count - after));
            for (int k = 0; k < before; k++) strings.Add(old[k]);
            strings.AddRange(middle.Strings);
            for (int k = after; k < old.Count; k++) strings.Add(Shift(old[k], delta));

            int firstAfterSafe = resumedAt >= 0 ? LowerBound(safe, resumedAt) : safe.Length;
            var points = new int[kept + middle.SafePoints.Length + (safe.Length - firstAfterSafe)];
            Array.Copy(safe, 0, points, 0, kept);
            Array.Copy(middle.SafePoints, 0, points, kept, middle.SafePoints.Length);
            int tail = kept + middle.SafePoints.Length;
            for (int k = firstAfterSafe, j = tail; k < safe.Length; k++, j++) points[j] = safe[k] + delta;

            return new HostScan(strings, points);
        }

        private static EmbeddedString Shift(EmbeddedString s, int delta)
        {
            if (delta == 0) return s;
            var moved = new EmbeddedString(s.EmbeddedLanguageId)
            {
                OuterStart = s.OuterStart + delta, Start = s.Start + delta, End = s.End + delta, OuterEnd = s.OuterEnd + delta
            };
            foreach (Interpolation x in s.Interpolations) moved.Interpolations.Add(new Interpolation(x.Start + delta, x.End + delta, x.OpenLength, x.CloseLength));
            foreach (EscapeSequence e in s.Escapes) moved.Escapes.Add(new EscapeSequence(e.Start + delta, e.Length, e.Value));
            return moved;
        }

        /// <summary>The number of values less than or equal to the position.</summary>
        private static int UpperBound(int[] sorted, int position)
        {
            int lo = 0, hi = sorted.Length;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (sorted[mid] <= position) lo = mid + 1; else hi = mid;
            }
            return lo;
        }

        /// <summary>The index of the first value at or after the position.</summary>
        private static int LowerBound(int[] sorted, int position)
        {
            int lo = 0, hi = sorted.Length;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (sorted[mid] < position) lo = mid + 1; else hi = mid;
            }
            return lo;
        }

        private static bool Contains(int[] sorted, int position)
        {
            int k = LowerBound(sorted, position);
            return k < sorted.Length && sorted[k] == position;
        }

        /// <summary>The index of the first string that starts at or after the position (the list is in order of start).</summary>
        private static int LowerBound(IReadOnlyList<EmbeddedString> strings, int position)
        {
            int lo = 0, hi = strings.Count;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (strings[mid].OuterStart < position) lo = mid + 1; else hi = mid;
            }
            return lo;
        }
    }
}
