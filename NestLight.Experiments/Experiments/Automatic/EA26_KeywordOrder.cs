using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA26_KeywordOrder : Experiment
    {
        public override string Id { get { return "EA26"; } }
        public override string Title { get { return "Where the place of the caret says nothing, do the words of the file and the most used keywords come first?"; } }
        public override string Hypothesis { get { return "A review of 800 suggestions found that, without a rule for the place, the list is the vocabulary in alphabetical order and cut at 100, with the words of the file after it: with nothing or one letter typed the word the person wants is often out of the first five (or out of the list). Putting the words of the file first, and the keywords in the order of how much code uses them, puts it among the first five more often."; } }
        public override string Method { get { return "A corpus of 500 snippets for each of the 8 languages, written by generators with the idioms of application code (SQL over a schema, CSS components, HTML templates, GraphQL, JSON and YAML configuration, GLSL and WGSL shaders), in 50 files of 10 snippets each. The even files are for learning the order of the keywords (how many times each is used), the odd files for measuring: 600 words per language typed with no letter (a request with Ctrl+Space) and with one letter. A second test on the hand-written files of the review of 800 suggestions, which come from another source. Four variants of the engine the plugin runs: as it is; the words of the file before the keywords; the keywords by use; both."; } }
        public override string Criterion { get { return "Over the words typed with 0 or 1 letter, the best of the three variants is at least 3 points better than the current engine within the first 5, on the test files; no language is worse by more than 1 point; and on the hand-written files it is not worse than the current engine."; } }
        public override string IfMet { get { return "Adopt the order of the best variant. The order of keywords by use is a prior learned from generated code: it is a starting point to be replaced by what the files of the user say."; } }
        public override string IfNotMet { get { return "Look at the tables by language: a language that gains and another that loses suggests an order for each."; } }

        public static readonly string[] Names = { "As it is", "Words of the file first", "Keywords by use", "Both" };

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            int words = settings.Quick ? 60 : 600;
            var currentFeatures = CompletionFeatures.Default;
            var perLanguage = new Dictionary<string, List<CorpusProbe>>();
            var handWritten = new Dictionary<string, List<CorpusProbe>>();
            var priors = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (string language in RealisticCorpus.Languages)
            {
                var documents = RealisticCorpus.Documents(language, settings.Quick ? 100 : RealisticCorpus.SnippetsPerLanguage, 1);
                var train = documents.Where(x => x.Train).ToList();
                var test = documents.Where(x => !x.Train).ToList();
                priors[language] = CorpusProbes.Priors(train, language);
                perLanguage[language] = CorpusProbes.Probes(test, language, words, new[] { 0, 1 });
                var hand = ReviewDocuments.ByLanguage[language].Select(t => new CorpusDocument { Language = language, Text = t.Trim('\r', '\n'), Snippets = 1 }).ToList();
                handWritten[language] = CorpusProbes.Probes(hand, language, 400, new[] { 0, 1 });
            }

            var variants = new[]
            {
                Variant(false, null),
                Variant(true, null),
                Variant(false, priors),
                Variant(true, priors),
            };

            var test2 = Evaluate(perLanguage, variants);
            var hand2 = Evaluate(handWritten, variants);

            outcome.Tables.Add(Overall("The test files: " + test2.Count(p => p.Probe.Reachable && p.Probe.Prefix == 0) / variants.Length + " requests with no letter and " + test2.Count(p => p.Probe.Reachable && p.Probe.Prefix == 1) / variants.Length + " with one letter (words that exist in the file or are keywords)", test2));
            outcome.Tables.Add(ByLanguage("Within the first 5, by language, test files", test2));
            outcome.Tables.Add(Overall("The hand-written files of the review (another source)", hand2));
            outcome.Tables.Add(ByLanguage("Within the first 5, by language, hand-written files", hand2));

            double baseline = Rate(test2, 0, p => true);
            int best = Enumerable.Range(1, 3).OrderByDescending(v => Rate(test2, v, p => true)).First();
            double gain = Rate(test2, best, p => true) - baseline;
            double worstLanguage = RealisticCorpus.Languages.Select(l => Rate(test2, best, p => p.Probe.Language == l) - Rate(test2, 0, p => p.Probe.Language == l)).Min();
            double handGain = Rate(hand2, best, p => true) - Rate(hand2, 0, p => true);
            outcome.CriterionMet = gain >= 3 && worstLanguage >= -1 && handGain >= 0;
            outcome.Headline = "best: " + Names[best] + " within the first 5 " + ContextLab.Pct(baseline) + " to " + ContextLab.Pct(Rate(test2, best, p => true)) + " (" + ContextLab.Signed(gain) + " points); hand-written " + ContextLab.Signed(handGain);
            outcome.Analysis.Add("On the test files the best variant is " + Names[best] + ": " + ContextLab.Signed(gain) + " points within the first 5; the worst language changes by " + ContextLab.Signed(worstLanguage) + " points; on the hand-written files " + ContextLab.Signed(handGain) + " points.");
            outcome.Analysis.Add("The corpus is made by generators written by the same person who reads the result, with the idioms they know. Training and test files come from the same generators, so the order of the keywords by use is learned from the same distribution it is measured on; the hand-written files are the check that is not.");
            outcome.Analysis.Add("A prior learned from generated code says which keywords this corpus uses, not which ones real projects use. The plugin could learn the order from the files of the user.");
            return outcome;
        }

        internal sealed class Scored { public CorpusProbe Probe; public int[] Ranks; }

        internal static Func<IHostScanner, CompletionEngine> Variant(bool wordsFirst, Dictionary<string, IReadOnlyList<string>> priors, int head = 0)
        {
            var features = new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true, wordsBeforeKeywords: wordsFirst, keywordPriors: priors, headKeywords: head);
            return scanner => new CompletionEngine(scanner, matcher: new BandedPrefixMatcher(), features: features);
        }

        internal static List<Scored> Evaluate(Dictionary<string, List<CorpusProbe>> probes, Func<IHostScanner, CompletionEngine>[] variants)
        {
            var scored = new List<Scored>();
            foreach (var pair in probes)
            {
                var engines = variants.Select(v => v(NestLightComposition.CreateScanner(HostLanguage.JavaScript, NestLightComposition.CreateEmbeddedLanguages()))).ToArray();
                foreach (CorpusProbe probe in pair.Value)
                    scored.Add(new Scored { Probe = probe, Ranks = engines.Select(e => CorpusProbes.Rank(e, probe)).ToArray() });
            }
            return scored;
        }

        internal static double Rate(List<Scored> scored, int variant, Func<Scored, bool> where, int top = 5)
        {
            var cases = scored.Where(s => s.Probe.Reachable && where(s)).ToList();
            return cases.Count == 0 ? 0 : 100.0 * cases.Count(s => s.Ranks[variant] >= 0 && s.Ranks[variant] < top) / cases.Count;
        }

        internal static double Mrr(List<Scored> scored, int variant, Func<Scored, bool> where)
        {
            var cases = scored.Where(s => s.Probe.Reachable && where(s)).ToList();
            return cases.Count == 0 ? 0 : cases.Sum(s => s.Ranks[variant] >= 0 ? 1.0 / (s.Ranks[variant] + 1) : 0) / cases.Count;
        }

        internal static Table Overall(string title, List<Scored> scored, string[] names = null)
        {
            names = names ?? Names;
            var table = new Table(title, "Variant", "No letter: first", "No letter: first 5", "One letter: first", "One letter: first 5", "Both: first 5", "Mean reciprocal rank");
            for (int v = 0; v < names.Length; v++)
                table.Add(names[v], ContextLab.Pct(Rate(scored, v, s => s.Probe.Prefix == 0, 1)), ContextLab.Pct(Rate(scored, v, s => s.Probe.Prefix == 0)),
                    ContextLab.Pct(Rate(scored, v, s => s.Probe.Prefix == 1, 1)), ContextLab.Pct(Rate(scored, v, s => s.Probe.Prefix == 1)), ContextLab.Pct(Rate(scored, v, s => true)), Mrr(scored, v, s => true).ToString("F3"));
            return table;
        }

        internal static Table ByLanguage(string title, List<Scored> scored, string[] names = null)
        {
            names = names ?? Names;
            var table = new Table(title, new[] { "Language", "Cases" }.Concat(names).ToArray());
            foreach (string language in RealisticCorpus.Languages)
            {
                string l = language;
                table.Add(new object[] { language, scored.Count(s => s.Probe.Reachable && s.Probe.Language == l) }.Concat(Enumerable.Range(0, names.Length).Select(v => (object)ContextLab.Pct(Rate(scored, v, s => s.Probe.Language == l)))).ToArray());
            }
            return table;
        }
    }
}
