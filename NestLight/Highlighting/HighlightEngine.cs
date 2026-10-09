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
        private readonly IHostScanner _scanner;
        private readonly IEmbeddedLanguageRegistry _languages;

        public HighlightEngine(IHostScanner scanner, IEmbeddedLanguageRegistry languages)
        {
            if (scanner == null) throw new ArgumentNullException("scanner");
            if (languages == null) throw new ArgumentNullException("languages");
            _scanner = scanner;
            _languages = languages;
        }

        public IReadOnlyList<Token> Highlight(string text)
        {
            var tokens = new List<Token>();
            IReadOnlyList<EmbeddedString> strings = _scanner.Scan(text);
            foreach (EmbeddedString s in strings)
            {
                IEmbeddedLanguageTokenizer tokenizer = _languages.Find(s.EmbeddedLanguageId);
                if (tokenizer == null) continue;
                EmitInterpolations(s, strings, tokens);
                EmitLanguage(text, s, tokenizer, tokens);
            }
            tokens.Sort((a, b) => a.Start.CompareTo(b.Start));
            return tokens;
        }

        // ---- the code of the embedded language --------------------------------------------------------------

        private static void EmitLanguage(string text, EmbeddedString s, IEmbeddedLanguageTokenizer tokenizer, List<Token> output)
        {
            int[] map;
            char[] decoded = Decode(text, s, out map);
            if (decoded.Length == 0) return;

            int count = decoded.Length;
            tokenizer.Tokenize(decoded, 0, count, (a, b, type) =>
            {
                if (a < 0) a = 0;
                if (b > count) b = count;
                if (b > a) AddClipped(map[a], map[b], type, s.Interpolations, output);
            });
        }

        /// <summary>
        /// The text of the string as the embedded language sees it. map[k] is the source offset of character k,
        /// and map[length] the end of the content.
        /// </summary>
        private static char[] Decode(string text, EmbeddedString s, out int[] map)
        {
            int start = s.Start, end = Math.Min(s.End, text.Length);
            var chars = new List<char>(Math.Max(0, end - start));
            var offsets = new List<int>(Math.Max(0, end - start) + 1);

            int interpolation = 0, escape = 0;
            int p = start;
            while (p < end)
            {
                while (interpolation < s.Interpolations.Count && s.Interpolations[interpolation].End <= p) interpolation++;
                if (interpolation < s.Interpolations.Count && s.Interpolations[interpolation].Start <= p)
                {
                    chars.Add(TextUtil.Mask);
                    offsets.Add(p);
                    p++;
                    continue;
                }

                while (escape < s.Escapes.Count && s.Escapes[escape].Start < p) escape++;
                if (escape < s.Escapes.Count && s.Escapes[escape].Start == p && p + s.Escapes[escape].Length <= end)
                {
                    chars.Add(s.Escapes[escape].Value);
                    offsets.Add(p);
                    p += s.Escapes[escape].Length;
                    continue;
                }

                chars.Add(text[p]);
                offsets.Add(p);
                p++;
            }
            offsets.Add(Math.Max(start, end));
            map = offsets.ToArray();
            return chars.ToArray();
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
                    // starts inside the expression (even if still unclosed, while typing)
                    if (nested.OuterStart > cursor) output.Add(new Token(cursor, nested.OuterStart - cursor, ClassificationNames.Expression));
                    cursor = Math.Max(cursor, Math.Min(nested.OuterEnd, close));
                }
                if (cursor < close) output.Add(new Token(cursor, close - cursor, ClassificationNames.Expression));
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
