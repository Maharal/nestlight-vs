using System;
using System.Collections.Generic;

namespace NestLight.Common
{
    /// <summary>
    /// What a scan of a text found: the embedded strings, and the places where a scan of an edited version of the text can pick the
    /// work up instead of starting over.
    /// </summary>
    internal sealed class HostScan
    {
        public HostScan(IReadOnlyList<EmbeddedString> strings, int[] safePoints, int stoppedAt = -1)
        {
            Strings = strings;
            SafePoints = safePoints;
            StoppedAt = stoppedAt;
        }

        public IReadOnlyList<EmbeddedString> Strings { get; private set; }

        /// <summary>
        /// Sorted offsets of line breaks that the scan reached in the plain code of the host: not in a string, a comment or an
        /// interpolation, and with no marker comment waiting for its string. From such a place the scan depends on nothing before it, so
        /// a scan can start there, and two scans of texts that are the same from there on find the same things from there on.
        /// </summary>
        public int[] SafePoints { get; private set; }

        /// <summary>For a scan that resumed another: the safe point where it stopped because the rest was already known; -1 when it read to the end.</summary>
        public int StoppedAt { get; private set; }
    }

    /// <summary>A host scanner that can start in the middle of a text and stop when it meets work that is already done.</summary>
    internal interface IResumableHostScanner : IHostScanner
    {
        /// <summary>The scan of the whole text, with its safe points.</summary>
        HostScan ScanAll(string text);

        /// <summary>
        /// Scans text[from, ...) as if it had read the text up to <paramref name="from"/> (a safe point, or 0). At a safe point at or after
        /// <paramref name="minStop"/> for which <paramref name="knownSafe"/> says the same is true of the text it replaces, it stops. The
        /// strings come back in order; those that start before <paramref name="from"/> or at the stop are not in it.
        /// </summary>
        HostScan Resume(string text, int from, int minStop, Func<int, bool> knownSafe);
    }

    /// <summary>Collects the safe points of a scan, and tells it when to stop.</summary>
    /// <remarks>
    /// Only the first line break at least <c>gap</c> characters after the one before is kept, so the list is a few thousand numbers for a
    /// large file, not one per line: it is copied at every edit, and a list of a few hundred kilobytes would be allocated on the large
    /// object heap each time (EA33). The choice depends only on the text, so a scan that resumes makes the same list as a whole scan.
    /// </remarks>
    internal sealed class SafePoints
    {
        /// <summary>The distance between two safe points the plugin keeps.</summary>
        public const int DefaultGap = 512;

        private readonly List<int> _points = new List<int>();
        private readonly int _minStop;
        private readonly Func<int, bool> _knownSafe;
        private readonly int _gap;
        private int _last = int.MinValue / 2;

        /// <summary>For a scan of a whole text: no stop.</summary>
        public SafePoints(int gap = DefaultGap) { _minStop = int.MaxValue; _gap = gap; }

        public SafePoints(int minStop, Func<int, bool> knownSafe, int gap = DefaultGap)
        {
            _minStop = minStop;
            _knownSafe = knownSafe;
            _gap = gap;
        }

        public int StoppedAt = -1;

        /// <summary>The scan reached the line break at the position in plain code with nothing pending. True: stop here.</summary>
        public bool Reached(int position)
        {
            if (position - _last < _gap) return false;
            if (_knownSafe != null && position >= _minStop && _knownSafe(position))
            {
                StoppedAt = position;
                return true;
            }
            _points.Add(position);
            _last = position;
            return false;
        }

        public int[] ToArray() { return _points.ToArray(); }
    }

    /// <summary>Where two texts differ.</summary>
    internal static class TextDiff
    {
        private const int Chunk = 256;

        /// <summary>The number of characters both texts start with.</summary>
        public static int CommonPrefix(string a, string b)
        {
            int n = Math.Min(a.Length, b.Length), i = 0;
            while (i + Chunk <= n && string.CompareOrdinal(a, i, b, i, Chunk) == 0) i += Chunk;
            while (i < n && a[i] == b[i]) i++;
            return i;
        }

        /// <summary>The number of characters both texts end with, not counting the first <paramref name="skip"/> (the common prefix).</summary>
        public static int CommonSuffix(string a, string b, int skip)
        {
            int n = Math.Min(a.Length, b.Length) - skip, i = 0;
            while (i + Chunk <= n && string.CompareOrdinal(a, a.Length - i - Chunk, b, b.Length - i - Chunk, Chunk) == 0) i += Chunk;
            while (i < n && a[a.Length - 1 - i] == b[b.Length - 1 - i]) i++;
            return i;
        }
    }
}
