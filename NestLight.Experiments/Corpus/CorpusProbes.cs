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
            ILanguageRegistry languages = NestLightComposition.CreateLanguages();
            IHostScanner scanner = NestLightComposition.CreateScanner(HostLanguage.JavaScript, languages);
            bool dash = Vocabularies.IsExtraWordChar(language, '-');
            foreach (CorpusDocument doc in documents)
            {
                string text = doc.Text;
                foreach (EmbeddedString s in scanner.Scan(text))
                {
                    if (!Vocabularies.SameLanguage(s.LanguageId, language) && !(language == "html" && s.LanguageId == "svg")) continue;
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
                foreach (int prefix in prefixes)
                {
                    int p = Math.Min(prefix, o.Word.Length - 1);
                    string typed = o.Word.Substring(0, p);
                    string text = o.Document.Text.Remove(o.Start, o.Word.Length).Insert(o.Start, typed);
                    bool reachable = Vocabularies.Find(language, o.Word) != null
                        || Regex.Matches(text, "(?<![\\p{L}\\p{N}_-])" + Regex.Escape(o.Word) + "(?![\\p{L}\\p{N}_-])", RegexOptions.IgnoreCase).Cast<Match>().Any(m => m.Index != o.Start);
                    probes.Add(new CorpusProbe { Language = language, Text = text, Caret = o.Start + p, Word = o.Word, Prefix = p, Reachable = reachable });
                }
            }
            return probes;
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
