using System;

namespace NestLight.Completion
{
    /// <summary>
    /// Tells how far a typed prefix is from a candidate word. Implementations keep no state shared between calls, so the completion
    /// can use one from any thread.
    /// </summary>
    internal interface IApproximateMatcher
    {
        /// <summary>
        /// The smallest edit distance between typed[typedStart, typedStart + typedLength) and any prefix of
        /// candidate[candidateStart, candidateStart + candidateLength) (an insertion, a deletion, a substitution and the swap of two
        /// neighbouring letters cost 1 each, and case is ignored), or -1 when it is larger than <paramref name="maxDistance"/>.
        /// </summary>
        int Distance(string typed, int typedStart, int typedLength, string candidate, int candidateStart, int candidateLength, int maxDistance);
    }

    /// <summary>
    /// The distance of "optimal string alignment" (Damerau-Levenshtein without edits inside a swapped pair) between what was typed and
    /// the closest prefix of the candidate, since the user has not finished the word.
    /// </summary>
    /// <remarks>
    /// Only the 2k+1 cells around the diagonal can hold a value of at most k, so the cost is O(k * n) per candidate, and the matrix is
    /// three rolling rows of at most 2k+1 cells: nothing is allocated per call (the rows are kept per thread). The comparison of
    /// letters is by their upper-case form in the invariant culture, which is what the ordinal comparison without case that the
    /// rest of the engine uses amounts to.
    /// </remarks>
    internal sealed class BandedPrefixMatcher : IApproximateMatcher
    {
        /// <summary>The largest <c>maxDistance</c> the buffers hold.</summary>
        public const int MaxK = 4;
        private const int Width = 2 * MaxK + 1;

        [ThreadStatic] private static int[] _rows;

        public int Distance(string typed, int typedStart, int typedLength, string candidate, int candidateStart, int candidateLength, int maxDistance)
        {
            if (typed == null) throw new ArgumentNullException("typed");
            if (candidate == null) throw new ArgumentNullException("candidate");
            if (typedStart < 0 || typedLength < 0 || typedStart + typedLength > typed.Length) throw new ArgumentOutOfRangeException("typedLength");
            if (candidateStart < 0 || candidateLength < 0 || candidateStart + candidateLength > candidate.Length) throw new ArgumentOutOfRangeException("candidateLength");
            if (maxDistance < 0 || maxDistance > MaxK) throw new ArgumentOutOfRangeException("maxDistance");

            int k = maxDistance, n = typedLength;
            int inf = k + 1;
            int[] buffer = _rows ?? (_rows = new int[3 * Width]);
            int cur = 0, one = Width, two = 2 * Width; // offsets of row m, m-1 and m-2

            // row 0: D[0][j] = j
            for (int d = 0; d <= 2 * k; d++) buffer[cur + d] = inf;
            for (int j = 0; j <= k && j <= n; j++) buffer[cur + (j + k)] = j;

            int best = n <= k ? n : inf; // the empty prefix of the candidate: delete everything that was typed
            int previousMin = 0; // the minimum of row m-1 (row 0 holds 0)
            int maxRows = Math.Min(candidateLength, n + k);
            for (int m = 1; m <= maxRows; m++)
            {
                // rotate: the old row m-2 is reused for row m
                int free = two; two = one; one = cur; cur = free;
                for (int d = 0; d <= 2 * k; d++) buffer[cur + d] = inf;

                char b = candidate[candidateStart + m - 1];
                int rowMin = inf;
                int jFrom = Math.Max(0, m - k), jTo = Math.Min(n, m + k);
                for (int j = jFrom; j <= jTo; j++)
                {
                    int d = j - m + k;
                    int value;
                    if (j == 0) value = m;
                    else
                    {
                        char a = typed[typedStart + j - 1];
                        // substitution or match, from D[m-1][j-1] (the same diagonal)
                        value = buffer[one + d] + (Same(a, b) ? 0 : 1);
                        // a character of the candidate that was not typed: D[m-1][j] (one diagonal over)
                        if (d + 1 <= 2 * k) value = Math.Min(value, buffer[one + d + 1] + 1);
                        // a typed character that the candidate does not have: D[m][j-1]
                        if (d - 1 >= 0) value = Math.Min(value, buffer[cur + d - 1] + 1);
                        // two neighbouring letters swapped: D[m-2][j-2]
                        if (m > 1 && j > 1 && Same(b, typed[typedStart + j - 2]) && Same(candidate[candidateStart + m - 2], a))
                            value = Math.Min(value, buffer[two + d] + 1);
                    }
                    if (value > inf) value = inf;
                    buffer[cur + d] = value;
                    if (value < rowMin) rowMin = value;
                }

                if (n >= m - k && n <= m + k)
                {
                    int atEnd = buffer[cur + (n - m + k)];
                    if (atEnd < best) best = atEnd;
                }
                // a swap looks two rows back, so only two rows in a row above k close the case
                if (rowMin > k && previousMin > k) break;
                previousMin = rowMin;
            }
            return best <= k ? best : -1;
        }

        private static bool Same(char x, char y)
        {
            return x == y || char.ToUpperInvariant(x) == char.ToUpperInvariant(y);
        }
    }
}
