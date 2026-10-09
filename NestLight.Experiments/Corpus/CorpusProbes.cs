using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    /// <summary>A word of an embedded string, typed with <see cref="Prefix"/> letters (0: the caret is where the word was).</summary>
    internal sealed class CorpusProbe
    {
        public string Language;
        public string Text;
        public int Caret;
        public string Word;
        public int Prefix;
        public bool Reachable;
    }

    internal static class CorpusProbes
    {
        private sealed class Occurrence { public CorpusDocument Document; public int Start; public string Word; }

        private static IEnumerable<Occurrence> Occurrences(IEnumerable<CorpusDocument> documents, string language)
        {
            IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
            IHostScanner scanner = NestLightComposition.CreateScanner(HostLanguage.JavaScript, languages);
            bool dash = Vocabularies.IsExtraWordChar(language, '-');
            foreach (CorpusDocument doc in documents)
            {
                string text = doc.Text;
                foreach (EmbeddedString s in scanner.Scan(text))
                {
                    if (!Vocabularies.SameLanguage(s.EmbeddedLanguageId, language) && !(language == "html" && s.EmbeddedLanguageId == "svg")) continue;
                    int i = s.Start, end = Math.Min(s.End, text.Length);
                    while (i < end)
                    {
                        if (!(char.IsLetter(text[i]) || text[i] == '_')) { i++; continue; }
                        int start = i;
                        while (i < end && (char.IsLetterOrDigit(text[i]) || text[i] == '_' || (dash && text[i] == '-'))) i++;
                        char before = start > 0 ? text[start - 1] : ' ';
                        if (s.Interpolations.Any(x => x.Start <= start && start < x.End) || before == '#' || before == '$' || before == '@' && language != "wgsl" || char.IsDigit(before) || i - start < 2) continue;
                        yield return new Occurrence { Document = doc, Start = start, Word = text.Substring(start, i - start) };
                    }
                }
            }
        }

        /// <summary>Words spread evenly over the documents, each typed with every prefix length asked for.</summary>
        public static List<CorpusProbe> Probes(IEnumerable<CorpusDocument> documents, string language, int words, int[] prefixes)
        {
            var all = Occurrences(documents, language).ToList();
            var probes = new List<CorpusProbe>();
            if (all.Count == 0) return probes;
            double step = Math.Max(1.0, all.Count / (double)words);
            for (int n = 0; n < words && (int)(n * step) < all.Count; n++)
            {
                Occurrence o = all[(int)(n * step)];
                var done = new HashSet<int>();
                foreach (int prefix in prefixes)
                {
                    int p = Math.Min(prefix, o.Word.Length - 1);
                    if (!done.Add(p)) continue;
                    string typed = o.Word.Substring(0, p);
                    string text = o.Document.Text.Remove(o.Start, o.Word.Length).Insert(o.Start, typed);
                    bool reachable = Vocabularies.Find(language, o.Word) != null
                        || Regex.Matches(text, "(?<![\\p{L}\\p{N}_-])" + Regex.Escape(o.Word) + "(?![\\p{L}\\p{N}_-])", RegexOptions.IgnoreCase).Cast<Match>().Any(m => m.Index != o.Start);
                    probes.Add(new CorpusProbe { Language = language, Text = text, Caret = o.Start + p, Word = o.Word, Prefix = p, Reachable = reachable });
                }
            }
            return probes;
        }

        /// <summary>A probe whose typed text is not a prefix of the word: <see cref="Mistake"/> says what was done to it.</summary>
        public sealed class MistakeProbe
        {
            public CorpusProbe Probe;
            public string Typed;
            public string Kind;
        }

        private static int CountOf(string text, string word)
        {
            return Regex.Matches(text, "(?<![\\p{L}\\p{N}_-])" + Regex.Escape(word) + "(?![\\p{L}\\p{N}_-])", RegexOptions.IgnoreCase).Count;
        }

        /// <summary>
        /// Words typed with one mistake (a swap, a missing letter, a wrong letter, an extra one) in a prefix of 3 to 5 letters, never in the first
        /// letter; only words that exist elsewhere in the file or are keywords.
        /// </summary>
        public static List<MistakeProbe> Mistakes(IEnumerable<CorpusDocument> documents, string language, int words, int seed)
        {
            var random = new Random(seed);
            var all = Occurrences(documents, language).Where(o => o.Word.Length >= 5).ToList();
            var result = new List<MistakeProbe>();
            if (all.Count == 0) return result;
            double step = Math.Max(1.0, all.Count / (double)words);
            for (int n = 0; n < words && (int)(n * step) < all.Count; n++)
            {
                Occurrence o = all[(int)(n * step)];
                bool reachable = Vocabularies.Find(language, o.Word) != null || CountOf(o.Document.Text, o.Word) > 1;
                if (!reachable) continue;
                foreach (string kind in new[] { "swap", "missing", "wrong", "extra" })
                {
                    int b = 3 + random.Next(3);                 // the letters of the word the mistake is made in
                    b = Math.Min(b, o.Word.Length - 1);
                    string prefix = o.Word.Substring(0, b);
                    string typed;
                    int at = 1 + random.Next(b - 1);            // never the first letter
                    switch (kind)
                    {
                        case "swap":
                            if (at + 1 >= b || char.ToLowerInvariant(prefix[at]) == char.ToLowerInvariant(prefix[at + 1])) continue;
                            typed = prefix.Substring(0, at) + prefix[at + 1] + prefix[at] + prefix.Substring(at + 2); break;
                        case "missing": typed = prefix.Remove(at, 1); break;
                        case "wrong":
                            char other = (char)('a' + random.Next(26));
                            if (char.ToLowerInvariant(other) == char.ToLowerInvariant(prefix[at])) continue;
                            typed = prefix.Substring(0, at) + other + prefix.Substring(at + 1); break;
                        default: typed = prefix.Insert(at, ((char)('a' + random.Next(26))).ToString()); break;
                    }
                    if (typed.Length < 3 || typed.Length > 6) continue;
                    string text = o.Document.Text.Remove(o.Start, o.Word.Length).Insert(o.Start, typed);
                    result.Add(new MistakeProbe { Kind = kind, Typed = typed, Probe = new CorpusProbe { Language = language, Text = text, Caret = o.Start + typed.Length, Word = o.Word, Prefix = typed.Length, Reachable = true } });
                }
            }
            return result;
        }

        /// <summary>
        /// Words that are written once in the file and are not keywords, typed with a correct prefix of 3 to 5 letters: a new word. Nothing
        /// similar can be what the person wants.
        /// </summary>
        public static List<CorpusProbe> NewWords(IEnumerable<CorpusDocument> documents, string language, int words)
        {
            var all = Occurrences(documents, language).Where(o => o.Word.Length >= 6 && Vocabularies.Find(language, o.Word) == null && CountOf(o.Document.Text, o.Word) == 1).ToList();
            var result = new List<CorpusProbe>();
            if (all.Count == 0) return result;
            double step = Math.Max(1.0, all.Count / (double)words);
            for (int n = 0; n < words && (int)(n * step) < all.Count; n++)
            {
                Occurrence o = all[(int)(n * step)];
                foreach (int p in new[] { 3, 4, 5 })
                {
                    string typed = o.Word.Substring(0, p);
                    result.Add(new CorpusProbe { Language = language, Text = o.Document.Text.Remove(o.Start, o.Word.Length).Insert(o.Start, typed), Caret = o.Start + p, Word = o.Word, Prefix = p, Reachable = false });
                }
            }
            return result;
        }

        /// <summary>The keywords of the language by how often the documents use them, the most used first.</summary>
        public static List<string> Priors(IEnumerable<CorpusDocument> documents, string language)
        {
            var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (Occurrence o in Occurrences(documents, language))
            {
                string keyword = Vocabularies.Find(language, o.Word);
                if (keyword == null) continue;
                int c;
                counts[keyword] = counts.TryGetValue(keyword, out c) ? c + 1 : 1;
            }
            return counts.OrderByDescending(p => p.Value).ThenBy(p => p.Key, StringComparer.OrdinalIgnoreCase).Select(p => p.Key).ToList();
        }

        /// <summary>The 0-based place of the word in the list the engine gives for the probe; -1 when it is not in it.</summary>
        public static int Rank(CompletionEngine engine, CorpusProbe probe)
        {
            CompletionSite site = engine.Locate(probe.Text, probe.Caret);
            if (site == null) return -1;
            return CompletionLab.RankOf(engine.Suggest(probe.Text, site).Select(s => s.Text).ToList(), probe.Word);
        }
    }
}
