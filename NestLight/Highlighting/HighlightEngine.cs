using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Highlighting
{
    /// <summary>
    /// The whole pipeline for one host: finds the embedded strings, prepares their text (interpolations masked,
    /// escapes decoded), runs the tokenizer of each language and maps the tokens back to the source.
    /// </summary>
    internal sealed class HighlightEngine : IHighlighter
    {
        private static readonly Comparer<Token> ByStart = Comparer<Token>.Create((a, b) => a.Start.CompareTo(b.Start));

        private readonly IHostScanner _scanner;
        private readonly IEmbeddedLanguageRegistry _languages;
        private char[] _decodedBuf;
        private int[] _mapBuf;

        public HighlightEngine(IHostScanner scanner, IEmbeddedLanguageRegistry languages)
        {
            if (scanner == null) throw new ArgumentNullException("scanner");
            if (languages == null) throw new ArgumentNullException("languages");
            _scanner = scanner;
            _languages = languages;
        }

        public IHostScanner Scanner { get { return _scanner; } }

        public IReadOnlyList<Token> Highlight(string text)
        {
            IReadOnlyList<EmbeddedString> strings = _scanner.Scan(text);
            var tokens = new List<Token>(strings.Count * 6);
            bool nested = false;
            char[] decodedBuf = _decodedBuf;
            int[] mapBuf = _mapBuf;
            foreach (EmbeddedString s in strings)
            {
                IEmbeddedLanguageTokenizer tokenizer = _languages.Find(s.EmbeddedLanguageId);
                if (tokenizer == null) continue;
                int mark = tokens.Count;
                EmitInterpolations(s, strings, tokens);
                int maxLen = Math.Max(0, Math.Min(s.End, text.Length) - s.Start);
                if (decodedBuf == null || decodedBuf.Length < maxLen)
                    decodedBuf = new char[Math.Max(maxLen, 256)];
                if (mapBuf == null || mapBuf.Length < maxLen + 1)
                    mapBuf = new int[Math.Max(maxLen + 1, 257)];
                int decodedLen = Decode(text, s, decodedBuf, mapBuf);
                EmitLanguage(decodedBuf, decodedLen, mapBuf, s, tokenizer, tokens);
                int added = tokens.Count - mark;
                if (added > 1 && s.Interpolations.Count > 0)
                    tokens.Sort(mark, added, ByStart);
                if (!nested && mark > 0 && added > 0 && tokens[mark].Start < tokens[mark - 1].End)
                    nested = true;
            }
            _decodedBuf = decodedBuf;
            _mapBuf = mapBuf;
            if (nested) tokens.Sort(ByStart);
            return tokens;
        }

        // ---- the code of the embedded language --------------------------------------------------------------

        private static void EmitLanguage(char[] decoded, int count, int[] map, EmbeddedString s, IEmbeddedLanguageTokenizer tokenizer, List<Token> output)
        {
            if (count == 0) return;
            tokenizer.Tokenize(decoded, 0, count, (a, b, type) =>
            {
                if (a < 0) a = 0;
                if (b > count) b = count;
                if (b > a) AddClipped(map[a], map[b], type, s.Interpolations, output);
            });
        }

        private static int Decode(string text, EmbeddedString s, char[] chars, int[] offsets)
        {
            int start = s.Start, end = Math.Min(s.End, text.Length);
            int n = 0;
            int interpolation = 0, escape = 0;
            int p = start;
            while (p < end)
            {
                while (interpolation < s.Interpolations.Count && s.Interpolations[interpolation].End <= p) interpolation++;
                if (interpolation < s.Interpolations.Count && s.Interpolations[interpolation].Start <= p)
                {
                    chars[n] = TextUtil.Mask;
                    offsets[n] = p;
                    n++;
                    p++;
                    continue;
                }

                while (escape < s.Escapes.Count && s.Escapes[escape].Start < p) escape++;
                if (escape < s.Escapes.Count && s.Escapes[escape].Start == p && p + s.Escapes[escape].Length <= end)
                {
                    chars[n] = s.Escapes[escape].Value;
                    offsets[n] = p;
                    n++;
                    p += s.Escapes[escape].Length;
                    continue;
                }

                chars[n] = text[p];
                offsets[n] = p;
                n++;
                p++;
            }
            offsets[n] = Math.Max(start, end);
            return n;
        }

        /// <summary>
        /// Adds the token [start, end) without the parts that fall inside an interpolation.
        /// The interpolations are ordered and do not overlap, so the first one that can matter is found by binary search.
        /// </summary>
        private static void AddClipped(int start, int end, string type, List<Interpolation> holes, List<Token> output)
        {
            int lo = 0, hi = holes.Count;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (holes[mid].End <= start) lo = mid + 1; else hi = mid;
            }
            for (int k = lo; k < holes.Count; k++)
            {
                Interpolation hole = holes[k];
                if (hole.Start >= end) break;
                if (hole.Start > start) output.Add(new Token(start, hole.Start - start, type));
                start = Math.Max(start, hole.End);
                if (start >= end) return;
            }
            if (end > start) output.Add(new Token(start, end - start, type));
        }

        // ---- the interpolations --------------------------------------------------------------------------------

        /// <summary>
        /// Delimiters and neutral expression text. Strings nested in the expression are left out: they color themselves.
        /// </summary>
        private static void EmitInterpolations(EmbeddedString owner, IReadOnlyList<EmbeddedString> all, List<Token> output)
        {
            foreach (Interpolation x in owner.Interpolations)
            {
                int open = Math.Min(x.InnerStart, x.End);
                int close = x.InnerEnd;
                output.Add(new Token(x.Start, open - x.Start, ClassificationNames.ExprDelimiter));
                if (x.Closed) output.Add(new Token(close, x.CloseLength, ClassificationNames.ExprDelimiter));

                int cursor = open;
                for (int k = FirstStartingAtOrAfter(all, open); k < all.Count; k++)
                {
                    EmbeddedString nested = all[k];
                    if (nested.OuterStart >= close) break;
                    if (ReferenceEquals(nested, owner)) continue;
                    if (nested.OuterStart > cursor)
                        output.Add(new Token(cursor, nested.OuterStart - cursor, ClassificationNames.Expression));
                    cursor = Math.Max(cursor, Math.Min(nested.OuterEnd, close));
                }
                if (cursor < close)
                    output.Add(new Token(cursor, close - cursor, ClassificationNames.Expression));
            }
        }

        /// <summary>Binary search over strings ordered by OuterStart.</summary>
        private static int FirstStartingAtOrAfter(IReadOnlyList<EmbeddedString> all, int position)
        {
            int lo = 0, hi = all.Count;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;
                if (all[mid].OuterStart < position) lo = mid + 1; else hi = mid;
            }
            return lo;
        }
    }
}
