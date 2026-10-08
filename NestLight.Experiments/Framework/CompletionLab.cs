using System;
using System.Collections.Generic;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    /// <summary>One simulated keystroke sequence: a word of an embedded string is typed up to <see cref="PrefixLength"/> characters.</summary>
    internal sealed class ProbeResult
    {
        public HostLanguage Host;
        public string LanguageId;
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

        public static CompletionEngine Engine(HostLanguage host, int maxItems = Unlimited, int minWordLength = CompletionEngine.DefaultMinWordLength)
        {
            return new CompletionEngine(NestLightComposition.CreateScanner(host, NestLightComposition.CreateLanguages()), maxItems, minWordLength);
        }

        public static bool IsWordChar(string languageId, char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || Vocabularies.IsExtraWordChar(languageId, c);
        }

        private static bool IsWordStart(string languageId, char c)
        {
            return char.IsLetter(c) || c == '_' || Vocabularies.IsExtraWordChar(languageId, c);
        }

        /// <summary>The words of text[from, to): where they start and what they are.</summary>
        public static List<KeyValuePair<int, string>> WordsIn(string text, int from, int to, string languageId)
        {
            var words = new List<KeyValuePair<int, string>>();
            int i = from;
            while (i < to)
            {
                if (!IsWordStart(languageId, text[i])) { i++; continue; }
                int start = i;
                while (i < to && IsWordChar(languageId, text[i])) i++;
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
                IHostScanner scanner = NestLightComposition.CreateScanner(file.Host, NestLightComposition.CreateLanguages());

                var occurrences = new List<KeyValuePair<int, string>>();
                var languages = new List<string>();
                foreach (EmbeddedString s in scanner.Scan(file.Text))
                    foreach (var w in WordsIn(file.Text, s.Start, Math.Min(s.End, file.Text.Length), s.LanguageId))
                        if (w.Value.Length >= 4 && !InInterpolation(s, w.Key)) { occurrences.Add(w); languages.Add(s.LanguageId); }

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

                        var result = new ProbeResult { Host = file.Host, LanguageId = site.LanguageId, Word = word, PrefixLength = k };
                        foreach (Suggestion s in engine.Suggest(text, site))
                            (s.Kind == SuggestionKind.Keyword ? result.Keywords : result.Words).Add(s.Text);

                        result.Stats = new Dictionary<string, int[]>(StringComparer.Ordinal);
                        foreach (var w in WordsIn(text, 0, text.Length, site.LanguageId))
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
                                foreach (var w in WordsIn(text, s.Start, Math.Min(s.End, text.Length), s.LanguageId))
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
    }
}
