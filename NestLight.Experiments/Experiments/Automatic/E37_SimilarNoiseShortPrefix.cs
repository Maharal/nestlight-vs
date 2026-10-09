using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class E37_SimilarNoiseShortPrefix : Experiment
    {
        public override string Id { get { return "E37"; } }
        public override string Title { get { return "Does the similar-words stage make noise with short prefixes, and what removes it?"; } }
        public override string Hypothesis { get { return "A review of 800 suggestions found the stage that corrects mistakes inventing suggestions with no relation when the person is typing a new word with 3 letters (`fir` offers `fragment`, `scr` offers `src`). With 3 letters one edit is a third of the word, so almost any word is \"similar\". Looking for similar words only from 4 letters, or showing fewer of them, or only the words of the file at 3 letters, removes most of that noise and keeps most of the recovery of real mistakes."; } }
        public override string Method { get { return "The corpus of 500 snippets for each of the 8 languages (the odd files) and the hand-written files of the review. Recovery: words typed with one mistake (a swap, a missing letter, a wrong one, an extra one, never in the first letter) in a prefix that leaves 3, 4, 5 or 6 letters typed; only words that exist elsewhere in the file or are keywords; the meant word within the first 5. Noise: words written once in the file, not keywords, typed with a correct prefix of 3, 4 and 5 letters (a new word: nothing similar is wanted); how often any similar item is shown. Five variants: as it is; similar words only from 4 letters; at most 3 similar words with 3 letters; only words of the file as similar words with 3 letters; both of the last two."; } }
        public override string Criterion { get { return "A variant keeps at least 85% of the recovery of the current engine with 3 letters typed (relative) and shows noise in at most half as many cases with a correct 3-letter prefix; recovery with 4 and 5 letters stays within 1 point of the current engine; and the hand-written files go the same way (noise lower, recovery with 3 letters at least 85% of the current)."; } }
        public override string IfMet { get { return "Adopt the variant."; } }
        public override string IfNotMet { get { return "Keep the stage as it is and say what it costs."; } }

        public static readonly string[] Names = { "As it is", "From 4 letters", "At most 3 items at 3 letters", "File words only at 3 letters", "Both of the last two" };

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            int words = settings.Quick ? 60 : 500;
            var mistakes = new List<CorpusProbes.MistakeProbe>();
            var handMistakes = new List<CorpusProbes.MistakeProbe>();
            var fresh = new List<CorpusProbe>();
            var handFresh = new List<CorpusProbe>();
            foreach (string language in RealisticCorpus.Languages)
            {
                var documents = RealisticCorpus.Documents(language, settings.Quick ? 100 : RealisticCorpus.SnippetsPerLanguage, 1);
                var test = documents.Where(x => !x.Train).ToList();
                mistakes.AddRange(CorpusProbes.Mistakes(test, language, words, 5));
                fresh.AddRange(CorpusProbes.NewWords(test, language, words));
                var written = ReviewDocuments.ByLanguage[language].Select(t => new CorpusDocument { Language = language, Text = t.Trim('\r', '\n'), Snippets = 1 }).ToList();
                handMistakes.AddRange(CorpusProbes.Mistakes(written, language, 400, 5));
                handFresh.AddRange(CorpusProbes.NewWords(written, language, 400));
            }

            Func<CompletionFeatures, Func<IHostScanner, CompletionEngine>> make = f => scanner => new CompletionEngine(scanner, matcher: new BandedPrefixMatcher(), features: f);
            CompletionFeatures D(int minPrefix = 3, int cap = 0, bool fileOnly = false)
            {
                return new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true, wordsBeforeKeywords: true, keywordPriors: KeywordUse.Default, headKeywords: 12,
                    fuzzyMinPrefix: minPrefix, shortSimilarCap: cap, shortSimilarFromFileOnly: fileOnly);
            }
            var variants = new[] { make(D()), make(D(minPrefix: 4)), make(D(cap: 3)), make(D(fileOnly: true)), make(D(cap: 3, fileOnly: true)) };

            double[][] recovery = Recovery(mistakes, variants), handRecovery = Recovery(handMistakes, variants);
            double[][] noise = Noise(fresh, variants), handNoise = Noise(handFresh, variants);

            outcome.Tables.Add(RecoveryTable("Recovery of a mistake, by letters typed: the meant word within the first 5 (test files)", recovery, mistakes));
            outcome.Tables.Add(NoiseTable("Noise on a new word, by letters typed: how often a similar item is shown (test files)", noise, fresh));
            outcome.Tables.Add(RecoveryTable("Recovery of a mistake (hand-written files)", handRecovery, handMistakes));
            outcome.Tables.Add(NoiseTable("Noise on a new word (hand-written files)", handNoise, handFresh));

            int best = -1; double bestScore = double.MinValue;
            for (int v = 1; v < variants.Length; v++)
            {
                bool keeps = recovery[v][0] >= 0.85 * recovery[0][0];
                bool quieter = noise[v][0] <= 0.5 * noise[0][0];
                bool longer = Math.Abs(recovery[v][1] - recovery[0][1]) <= 1 && Math.Abs(recovery[v][2] - recovery[0][2]) <= 1;
                bool hand = handNoise[v][0] <= handNoise[0][0] && handRecovery[v][0] >= 0.85 * handRecovery[0][0];
                outcome.Analysis.Add(Names[v] + ": recovery with 3 letters " + ContextLab.Pct(recovery[v][0]) + " against " + ContextLab.Pct(recovery[0][0]) + ", noise with 3 letters " + ContextLab.Pct(noise[v][0]) + " against " + ContextLab.Pct(noise[0][0]) + (keeps && quieter && longer && hand ? " (meets)" : " (does not meet)"));
                if (keeps && quieter && longer && hand && recovery[v][0] - noise[v][0] > bestScore) { best = v; bestScore = recovery[v][0] - noise[v][0]; }
            }
            outcome.CriterionMet = best >= 0;
            outcome.Headline = best >= 0 ? "adopt: " + Names[best] : "no variant meets the criterion";
            outcome.Analysis.Add("The mistakes are made by the experiment (uniform over the letters but the first), not the way people mistype; and a new word that the generator writes once is a word that is rare in the corpus. Both numbers are about the corpus.");
            return outcome;
        }

        private static double[][] Recovery(List<CorpusProbes.MistakeProbe> probes, Func<IHostScanner, CompletionEngine>[] variants)
        {
            var result = new double[variants.Length][];
            for (int v = 0; v < variants.Length; v++)
            {
                result[v] = new double[4];
                var counts = new int[4]; var hits = new int[4];
                var engines = new Dictionary<string, CompletionEngine>();
                foreach (var m in probes)
                {
                    CompletionEngine engine;
                    if (!engines.TryGetValue("x", out engine)) engines["x"] = engine = variants[v](NestLightComposition.CreateScanner(HostLanguage.JavaScript, NestLightComposition.CreateEmbeddedLanguages()));
                    int bucket = Math.Min(m.Typed.Length, 6) - 3;
                    counts[bucket]++;
                    int rank = CorpusProbes.Rank(engine, m.Probe);
                    if (rank >= 0 && rank < 5) hits[bucket]++;
                }
                for (int b = 0; b < 4; b++) result[v][b] = counts[b] == 0 ? 0 : 100.0 * hits[b] / counts[b];
            }
            return result;
        }

        private static double[][] Noise(List<CorpusProbe> probes, Func<IHostScanner, CompletionEngine>[] variants)
        {
            var result = new double[variants.Length][];
            for (int v = 0; v < variants.Length; v++)
            {
                result[v] = new double[3];
                var counts = new int[3]; var noisy = new int[3];
                CompletionEngine engine = variants[v](NestLightComposition.CreateScanner(HostLanguage.JavaScript, NestLightComposition.CreateEmbeddedLanguages()));
                foreach (CorpusProbe p in probes)
                {
                    CompletionSite site = engine.Locate(p.Text, p.Caret);
                    if (site == null) continue;
                    int bucket = p.Prefix - 3;
                    counts[bucket]++;
                    if (engine.Suggest(p.Text, site).Any(s => s.Distance > 0)) noisy[bucket]++;
                }
                for (int b = 0; b < 3; b++) result[v][b] = counts[b] == 0 ? 0 : 100.0 * noisy[b] / counts[b];
            }
            return result;
        }

        private static Table RecoveryTable(string title, double[][] recovery, List<CorpusProbes.MistakeProbe> probes)
        {
            var table = new Table(title + ", " + probes.Count + " mistakes", "Variant", "3 letters", "4 letters", "5 letters", "6 letters");
            for (int v = 0; v < Names.Length; v++) table.Add(Names[v], ContextLab.Pct(recovery[v][0]), ContextLab.Pct(recovery[v][1]), ContextLab.Pct(recovery[v][2]), ContextLab.Pct(recovery[v][3]));
            return table;
        }

        private static Table NoiseTable(string title, double[][] noise, List<CorpusProbe> probes)
        {
            var table = new Table(title + ", " + probes.Count / 3 + " new words", "Variant", "3 letters", "4 letters", "5 letters");
            for (int v = 0; v < Names.Length; v++) table.Add(Names[v], ContextLab.Pct(noise[v][0]), ContextLab.Pct(noise[v][1]), ContextLab.Pct(noise[v][2]));
            return table;
        }
    }
}
