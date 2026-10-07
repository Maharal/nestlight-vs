using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Highlighting
{
    /// <summary>
    /// Keeps the tokens of the last text snapshot, so the editor can ask for any span without a new analysis.
    /// The snapshot type is a parameter (an <c>ITextSnapshot</c> in Visual Studio), which keeps the cache free of the editor.
    /// A failure in the highlighter never escapes: the text just gets no embedded colors.
    /// </summary>
    internal sealed class SnapshotTokenCache<TSnapshot> where TSnapshot : class
    {
        private static readonly IReadOnlyList<Token> NoTokens = new Token[0];

        private readonly IHighlighter _highlighter;
        private readonly Func<TSnapshot, string> _getText;
        private readonly object _gate = new object();
        private TSnapshot _snapshot;
        private IReadOnlyList<Token> _tokens = NoTokens;

        public SnapshotTokenCache(IHighlighter highlighter, Func<TSnapshot, string> getText)
        {
            if (highlighter == null) throw new ArgumentNullException("highlighter");
            if (getText == null) throw new ArgumentNullException("getText");
            _highlighter = highlighter;
            _getText = getText;
        }

        /// <summary>The tokens that intersect [start, end), in order.</summary>
        public IList<Token> TokensIn(TSnapshot snapshot, int start, int end)
        {
            IReadOnlyList<Token> tokens = Analyze(snapshot);
            var result = new List<Token>();

            // first token that ends after the start (tokens are ordered and do not overlap)
            int lo = 0, hi = tokens.Count;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (tokens[mid].End <= start) lo = mid + 1; else hi = mid;
            }
            for (int k = lo; k < tokens.Count && tokens[k].Start < end; k++)
                if (tokens[k].Length > 0) result.Add(tokens[k]);
            return result;
        }

        private IReadOnlyList<Token> Analyze(TSnapshot snapshot)
        {
            lock (_gate)
            {
                if (!ReferenceEquals(_snapshot, snapshot))
                {
                    _tokens = Run(snapshot);
                    _snapshot = snapshot;
                }
                return _tokens;
            }
        }

        private IReadOnlyList<Token> Run(TSnapshot snapshot)
        {
            try
            {
                return _highlighter.Highlight(_getText(snapshot));
            }
            catch (Exception)
            {
                // Never take down the editor because of colorization.
                return NoTokens;
            }
        }
    }
}
