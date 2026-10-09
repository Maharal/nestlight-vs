using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    /// <summary>One simulated keystroke sequence: a word of an embedded string is typed up to <see cref="PrefixLength"/> characters.</summary>
    internal sealed class ProbeResult
    {
        public HostLanguage Host;
        public string EmbeddedLanguageId;
        public string Word;
        public int PrefixLength;
        /// <summary>Keywords of the language, in the order the engine returns them.</summary>
        public List<string> Keywords = new List<string>();
        /// <summary>Words of the document, in the order the engine returns them (nearest to the caret first).</summary>
        public List<string> Words = new List<string>();
        /// <summary>Occurrences and first position of every word of the text, for the orderings the engine does not use.</summary>
        public Dictionary<string, int[]> Stats;
        /// <summary>The words that appear inside some embedded string / inside the string being typed. Null unless asked for.</summary>
        public HashSet<string> WordsInEmbedded, WordsInOwner;
    }

    internal static class CompletionLab
    {
        public const int Unlimited = 1000000;

        public static CompletionEngine Engine(HostLanguage host, int maxItems = Unlimited, int minWordLength = CompletionEngine.DefaultMinWordLength,
            bool similar = false, int fuzzyBelow = CompletionEngine.DefaultFuzzyBelow, int fuzzyMaxItems = CompletionEngine.DefaultFuzzyMaxItems,
            CompletionFeatures features = null)
        {
            return new CompletionEngine(NestLightComposition.CreateScanner(host, NestLightComposition.CreateEmbeddedLanguages()), maxItems, minWordLength,
                similar ? new BandedPrefixMatcher() : null, fuzzyBelow, fuzzyMaxItems, features: features);
        }

        public static bool IsWordChar(string embeddedLanguageId, char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || CompletionLanguages.Default.Find(embeddedLanguageId).IsExtraWordChar(c);
        }

        private static bool IsWordStart(string embeddedLanguageId, char c)
        {
            return char.IsLetter(c) || c == '_' || CompletionLanguages.Default.Find(embeddedLanguageId).IsExtraWordChar(c);
        }

        /// <summary>The words of text[from, to): where they start and what they are.</summary>
        public static List<KeyValuePair<int, string>> WordsIn(string text, int from, int to, string embeddedLanguageId)
        {
            var words = new List<KeyValuePair<int, string>>();
            int i = from;
            while (i < to)
            {
                if (!IsWordStart(embeddedLanguageId, text[i])) { i++; continue; }
                int start = i;
                while (i < to && IsWordChar(embeddedLanguageId, text[i])) i++;
                words.Add(new KeyValuePair<int, string>(start, text.Substring(start, i - start)));
            }
            return words;
        }

        /// <summary>
        /// Types words of the embedded strings of the corpus, one prefix at a time (1 to <paramref name="maxPrefix"/> characters), with the rest
        /// of the word missing, and records what the engine offers there. At most <paramref name="perFile"/> words per file, spread evenly.
        /// </summary>
        public static IEnumerable<ProbeResult> Probe(List<CorpusFile> corpus, int maxPrefix, int perFile, bool scopes)
        {
            foreach (CorpusFile file in corpus)
            {
                CompletionEngine engine = Engine(file.Host);
                IHostScanner scanner = NestLightComposition.CreateScanner(file.Host, NestLightComposition.CreateEmbeddedLanguages());

                var occurrences = new List<KeyValuePair<int, string>>();
                var languages = new List<string>();
                foreach (EmbeddedString s in scanner.Scan(file.Text))
                    foreach (var w in WordsIn(file.Text, s.Start, Math.Min(s.End, file.Text.Length), s.EmbeddedLanguageId))
                        if (w.Value.Length >= 4 && !InInterpolation(s, w.Key)) { occurrences.Add(w); languages.Add(s.EmbeddedLanguageId); }

                int step = Math.Max(1, occurrences.Count / perFile);
                for (int n = 0; n < occurrences.Count; n += step)
                {
                    int start = occurrences[n].Key;
                    string word = occurrences[n].Value;
                    for (int k = 1; k <= maxPrefix && k < word.Length; k++)
                    {
                        string text = file.Text.Remove(start + k, word.Length - k);
                        CompletionSite site = engine.Locate(text, start + k);
                        if (site == null) continue;

                        var result = new ProbeResult { Host = file.Host, EmbeddedLanguageId = site.EmbeddedLanguageId, Word = word, PrefixLength = k };
                        foreach (Suggestion s in engine.Suggest(text, site))
                            (s.Kind == SuggestionKind.Keyword ? result.Keywords : result.Words).Add(s.Text);

                        result.Stats = new Dictionary<string, int[]>(StringComparer.Ordinal);
                        foreach (var w in WordsIn(text, 0, text.Length, site.EmbeddedLanguageId))
                        {
                            if (w.Key <= site.Caret && site.Caret <= w.Key + w.Value.Length) continue;
                            int[] stat;
                            if (!result.Stats.TryGetValue(w.Value, out stat)) result.Stats[w.Value] = new[] { 1, w.Key };
                            else stat[0]++;
                        }

                        if (scopes)
                        {
                            result.WordsInEmbedded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            result.WordsInOwner = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            foreach (EmbeddedString s in scanner.Scan(text))
                            {
                                bool owner = s.Start <= site.Caret && site.Caret <= Math.Min(s.End, text.Length);
                                foreach (var w in WordsIn(text, s.Start, Math.Min(s.End, text.Length), s.EmbeddedLanguageId))
                                {
                                    if (w.Key <= site.Caret && site.Caret <= w.Key + w.Value.Length) continue;
                                    result.WordsInEmbedded.Add(w.Value);
                                    if (owner) result.WordsInOwner.Add(w.Value);
                                }
                            }
                        }
                        yield return result;
                    }
                }
            }
        }

        private static bool InInterpolation(EmbeddedString s, int position)
        {
            foreach (Interpolation x in s.Interpolations) if (x.Start <= position && position < x.End) return true;
            return false;
        }

        /// <summary>The 0-based position of the word in the ordered list, ignoring case; -1 when it is not there.</summary>
        public static int RankOf(IList<string> ordered, string word)
        {
            for (int i = 0; i < ordered.Count; i++)
                if (string.Equals(ordered[i], word, StringComparison.OrdinalIgnoreCase)) return i;
            return -1;
        }

        // ---- typing with and without mistakes ----------------------------------------------------------------------

        /// <summary>A word of an embedded string typed up to a point: the text around it has the typed text instead of the word.</summary>
        internal sealed class Typing
        {
            public HostLanguage Host;
            public string Text;
            public int Caret;
            /// <summary>The word the user meant.</summary>
            public string Word;
            /// <summary>What was typed (a prefix of the word, or a prefix with one edit).</summary>
            public string Typed;
            /// <summary>"correct", or the kind of the edit: insertion, deletion, substitution, transposition.</summary>
            public string Kind;
            /// <summary>Where in the typed text the edit is (0 for the first letter); -1 when there is no edit.</summary>
            public int EditAt = -1;
        }

        internal static List<KeyValuePair<int, string>> Sample(CorpusFile file, int perFile, int minLength)
        {
            IHostScanner scanner = NestLightComposition.CreateScanner(file.Host, NestLightComposition.CreateEmbeddedLanguages());
            var all = new List<KeyValuePair<int, string>>();
            foreach (EmbeddedString s in scanner.Scan(file.Text))
                foreach (var w in WordsIn(file.Text, s.Start, Math.Min(s.End, file.Text.Length), s.EmbeddedLanguageId))
                    if (w.Value.Length >= minLength && !InInterpolation(s, w.Key)) all.Add(w);
            int step = Math.Max(1, all.Count / perFile);
            var picked = new List<KeyValuePair<int, string>>();
            for (int n = 0; n < all.Count; n += step) picked.Add(all[n]);
            return picked;
        }

        private static Typing Replace(CorpusFile file, int start, string word, string typed, string kind, int editAt)
        {
            return new Typing { Host = file.Host, Text = file.Text.Remove(start, word.Length).Insert(start, typed), Caret = start + typed.Length, Word = word, Typed = typed, Kind = kind, EditAt = editAt };
        }

        /// <summary>The words of the corpus typed correctly, from <paramref name="minPrefix"/> to <paramref name="maxPrefix"/> letters.</summary>
        public static IEnumerable<Typing> CorrectPrefixes(List<CorpusFile> corpus, int perFile, int minPrefix, int maxPrefix)
        {
            foreach (CorpusFile file in corpus)
                foreach (var w in Sample(file, perFile, minPrefix + 1))
                    for (int p = minPrefix; p <= maxPrefix && p < w.Value.Length; p++)
                        yield return Replace(file, w.Key, w.Value, w.Value.Substring(0, p), "correct", -1);
        }

        /// <summary>
        /// The words of the corpus typed with one mistake, one of each kind, in a prefix of 4 to 8 letters (the length cycles through the words).
        /// The place of the mistake is picked at random (seeded) over the whole prefix, the first letter included.
        /// </summary>
        public static IEnumerable<Typing> OneEditPrefixes(List<CorpusFile> corpus, int perFile, int seed)
        {
            var random = new Random(seed);
            int n = 0;
            foreach (CorpusFile file in corpus)
                foreach (var w in Sample(file, perFile, 5))
                {
                    int p = 4 + (n++ % 5);
                    p = Math.Min(p, w.Value.Length - 1);
                    if (p < 4) continue;
                    string prefix = w.Value.Substring(0, p);

                    int at = random.Next(p + 1);
                    char extra = (char)('a' + random.Next(26));
                    yield return Replace(file, w.Key, w.Value, prefix.Insert(at, extra.ToString()), "insertion", at);

                    at = random.Next(p);
                    yield return Replace(file, w.Key, w.Value, prefix.Remove(at, 1), "deletion", at);

                    at = random.Next(p);
                    char other = (char)('a' + random.Next(26));
                    if (char.ToLowerInvariant(other) == char.ToLowerInvariant(prefix[at])) other = other == 'z' ? 'a' : (char)(other + 1);
                    yield return Replace(file, w.Key, w.Value, prefix.Substring(0, at) + other + prefix.Substring(at + 1), "substitution", at);

                    var swappable = Enumerable.Range(0, p - 1).Where(i => char.ToLowerInvariant(prefix[i]) != char.ToLowerInvariant(prefix[i + 1])).ToList();
                    if (swappable.Count > 0)
                    {
                        at = swappable[random.Next(swappable.Count)];
                        var chars = prefix.ToCharArray();
                        char t = chars[at]; chars[at] = chars[at + 1]; chars[at + 1] = t;
                        yield return Replace(file, w.Key, w.Value, new string(chars), "transposition", at);
                    }
                }
        }

        /// <summary>The words of the text, apart from the one the caret touches, with their number of occurrences and the distance of the nearest one to the caret.</summary>
        public static void WordFacts(string text, int caret, string embeddedLanguageId, out Dictionary<string, int> counts, out Dictionary<string, int> near)
        {
            counts = new Dictionary<string, int>(StringComparer.Ordinal);
            near = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var w in WordsIn(text, 0, text.Length, embeddedLanguageId))
            {
                int end = w.Key + w.Value.Length;
                if (w.Key <= caret && caret <= end) continue;
                int distance = w.Key > caret ? w.Key - caret : caret - end;
                int count, nearest;
                counts[w.Value] = counts.TryGetValue(w.Value, out count) ? count + 1 : 1;
                if (!near.TryGetValue(w.Value, out nearest) || distance < nearest) near[w.Value] = distance;
            }
        }

        /// <summary>The definition of the distance, the straightforward way: the whole matrix, then the closest prefix; -1 when it is above <paramref name="k"/>.</summary>
        public static int ReferenceDistance(string typed, string candidate, int k)
        {
            int n = typed.Length, m = candidate.Length;
            var d = new int[m + 1, n + 1];
            for (int j = 0; j <= n; j++) d[0, j] = j;
            for (int i = 1; i <= m; i++)
            {
                d[i, 0] = i;
                for (int j = 1; j <= n; j++)
                {
                    char b = char.ToUpperInvariant(candidate[i - 1]), a = char.ToUpperInvariant(typed[j - 1]);
                    int v = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + (a == b ? 0 : 1));
                    if (i > 1 && j > 1 && b == char.ToUpperInvariant(typed[j - 2]) && char.ToUpperInvariant(candidate[i - 2]) == a) v = Math.Min(v, d[i - 2, j - 2] + 1);
                    d[i, j] = v;
                }
            }
            int best = int.MaxValue;
            for (int i = 0; i <= m; i++) best = Math.Min(best, d[i, n]);
            return best <= k ? best : -1;
        }
    }
}
