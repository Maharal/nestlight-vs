using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;

namespace NestLight.Experiments
{
    internal sealed class E35_HeadKeywords : Experiment
    {
        public override string Id { get { return "E35"; } }
        public override string Title { get { return "Do a few keywords still come before the words of the file?"; } }
        public override string Hypothesis { get { return "E34 found that the words of the file first and the keywords by use gain in GLSL, WGSL and HTML but lose in JSON and GraphQL: in JSON the words of the file pushed `true`, `false` and `null` down, and they are the few keywords that are always right in a value. Putting only the few most used keywords of the language before the words of the file, and the others after them, keeps that gain and the loss goes away."; } }
        public override string Method { get { return "E34's files, words and priors (the corpus of 500 snippets for each language, the even files for the order of the keywords, the odd files for measuring, and the hand-written files of the review as a second test). Variants: the engine as it is; words of the file first with the keywords by use (E34's best, 0 keywords in front); and the same with the 3, 6 and 12 most used keywords of the language in front of the words."; } }
        public override string Criterion { get { return "The best of the variants with keywords in front is at least 3 points better than the engine as it is within the first 5, over the words typed with 0 or 1 letter on the test files; no language is worse by more than 1 point; and on the hand-written files it is not worse than the engine as it is."; } }
        public override string IfMet { get { return "Adopt the variant: the words of the file first where no rule decides, with that many keywords in front, in the order of use."; } }
        public override string IfNotMet { get { return "Keep the order as it is for the languages that lose (JSON, GraphQL) and apply the variant only where it gains."; } }

        public static readonly string[] Names = { "As it is", "Words first, 0 in front", "3 keywords in front", "6 keywords in front", "12 keywords in front" };

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            int words = settings.Quick ? 60 : 600;
            var perLanguage = new Dictionary<string, List<CorpusProbe>>();
            var handWritten = new Dictionary<string, List<CorpusProbe>>();
            var priors = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (string language in RealisticCorpus.Languages)
            {
                var documents = RealisticCorpus.Documents(language, settings.Quick ? 100 : RealisticCorpus.SnippetsPerLanguage, 1);
                priors[language] = CorpusProbes.Priors(documents.Where(x => x.Train).ToList(), language);
                perLanguage[language] = CorpusProbes.Probes(documents.Where(x => !x.Train).ToList(), language, words, new[] { 0, 1 });
                var hand = ReviewDocuments.ByLanguage[language].Select(t => new CorpusDocument { Language = language, Text = t.Trim('\r', '\n'), Snippets = 1 }).ToList();
                handWritten[language] = CorpusProbes.Probes(hand, language, 400, new[] { 0, 1 });
            }
            var asymmetric = priors.ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase);
            var variants = new[]
            {
                E34_KeywordOrder.Variant(false, null),
                E34_KeywordOrder.Variant(true, asymmetric, 0),
                E34_KeywordOrder.Variant(true, asymmetric, 3),
                E34_KeywordOrder.Variant(true, asymmetric, 6),
                E34_KeywordOrder.Variant(true, asymmetric, 12),
            };

            var test = E34_KeywordOrder.Evaluate(perLanguage, variants);
            var hand2 = E34_KeywordOrder.Evaluate(handWritten, variants);
            outcome.Tables.Add(E34_KeywordOrder.Overall("The test files", test, Names));
            outcome.Tables.Add(E34_KeywordOrder.ByLanguage("Within the first 5, by language, test files", test, Names));
            outcome.Tables.Add(E34_KeywordOrder.Overall("The hand-written files of the review (another source)", hand2, Names));
            outcome.Tables.Add(E34_KeywordOrder.ByLanguage("Within the first 5, by language, hand-written files", hand2, Names));

            double baseline = E34_KeywordOrder.Rate(test, 0, p => true);
            int best = Enumerable.Range(2, 3).OrderByDescending(v => E34_KeywordOrder.Rate(test, v, p => true)).First();
            double gain = E34_KeywordOrder.Rate(test, best, p => true) - baseline;
            double worstLanguage = RealisticCorpus.Languages.Select(l => E34_KeywordOrder.Rate(test, best, p => p.Probe.Language == l) - E34_KeywordOrder.Rate(test, 0, p => p.Probe.Language == l)).Min();
            double handGain = E34_KeywordOrder.Rate(hand2, best, p => true) - E34_KeywordOrder.Rate(hand2, 0, p => true);
            outcome.CriterionMet = gain >= 3 && worstLanguage >= -1 && handGain >= 0;
            outcome.Headline = "best: " + Names[best] + " within the first 5 " + ContextLab.Pct(baseline) + " to " + ContextLab.Pct(E34_KeywordOrder.Rate(test, best, p => true)) + " (" + ContextLab.Signed(gain) + " points); hand-written " + ContextLab.Signed(handGain);
            outcome.Analysis.Add("On the test files the best variant is " + Names[best] + ": " + ContextLab.Signed(gain) + " points within the first 5; the worst language changes by " + ContextLab.Signed(worstLanguage) + " points; on the hand-written files " + ContextLab.Signed(handGain) + " points.");
            outcome.Analysis.Add("E35 was written after seeing E34, whose criterion was not met: the variants here are a reaction to its tables, not a prediction made before them. The same test files are used, so a gain here is partly fitted to them; the hand-written files are the check.");
            return outcome;
        }
    }
}
