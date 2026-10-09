using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal class EA29_ShortWords : Experiment
    {
        public override string Id { get { return "EA29"; } }
        public override string Title { get { return "Should words of two letters be offered?"; } }
        public override string Hypothesis { get { return "The completion skips the words under 3 letters, so `id`, `db`, `in`, `uv` and `if` are never offered, and they are among the most written words of SQL, YAML and shaders. Offering them, after all the longer words, lets a person who types `i` find `id` without crowding the list for the person who wants a longer word."; } }
        public override string Method { get { return "The corpus of 500 snippets for each of the 8 languages (the odd files, for measuring) and the hand-written files of the review. 600 words per language typed with 1 or 2 letters, only words that exist elsewhere in the file or are keywords. Three variants of the engine the plugin runs: as it is (minimum length 3); the minimum length lowered to 2 for every word; and the two-letter words in a tier after all the others. Reported for the words of 2 letters and for the longer ones."; } }
        public override string Criterion { get { return "A variant raises the words of 2 letters by at least 15 points within the first 5; it lowers the longer words by no more than 0.5 point overall and no more than 1 point in any language; and on the hand-written files the words of 2 letters gain at least 10 points while the longer ones lose no more than 1."; } }
        public override string IfMet { get { return "Offer the two-letter words in the way of the variant that meets it."; } }
        public override string IfNotMet { get { return "Offer them only where the place says a short word is likely (after a dot, after a word that was followed by one before)."; } }

        public static readonly string[] Names = { "Minimum 3 (as it is)", "Minimum 2", "Two-letter words last" };

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            int words = settings.Quick ? 60 : 600;
            var test = new Dictionary<string, List<CorpusProbe>>();
            var hand = new Dictionary<string, List<CorpusProbe>>();
            foreach (string language in RealisticCorpus.Languages)
            {
                var documents = RealisticCorpus.Documents(language, settings.Quick ? 100 : RealisticCorpus.SnippetsPerLanguage, 1);
                test[language] = CorpusProbes.Probes(documents.Where(x => !x.Train).ToList(), language, words, new[] { 1, 2 });
                var written = ReviewDocuments.ByLanguage[language].Select(t => new CorpusDocument { Language = language, Text = t.Trim('\r', '\n'), Snippets = 1 }).ToList();
                hand[language] = CorpusProbes.Probes(written, language, 400, new[] { 1, 2 });
            }

            // the engine as it was when the experiment was written: the plugin default now has the two-letter tier on
            var features = new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true, wordsBeforeKeywords: true, keywordPriors: KeywordUse.Default, headKeywords: 12);
            var shortLast = new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true, wordsBeforeKeywords: true, keywordPriors: KeywordUse.Default, headKeywords: 12, shortWordsLast: true);
            var variants = new Func<IHostScanner, CompletionEngine>[]
            {
                scanner => new CompletionEngine(scanner, matcher: new BandedPrefixMatcher(), features: features),
                scanner => new CompletionEngine(scanner, minWordLength: 2, matcher: new BandedPrefixMatcher(), features: features),
                scanner => new CompletionEngine(scanner, matcher: new BandedPrefixMatcher(), features: shortLast),
            };
            var t = EA27_KeywordOrder.Evaluate(test, variants);
            var h = EA27_KeywordOrder.Evaluate(hand, variants);

            outcome.Tables.Add(Table("The test files", t));
            outcome.Tables.Add(ByLanguage("Within the first 5, by language, test files", t));
            outcome.Tables.Add(Table("The hand-written files of the review (another source)", h));
            outcome.Tables.Add(ByLanguage("Within the first 5, by language, hand-written files", h));

            Func<EA27_KeywordOrder.Scored, bool> two = s => s.Probe.Word.Length == 2, longer = s => s.Probe.Word.Length > 2;
            int best = -1; double bestGain = double.MinValue, bestHarm = double.MinValue;
            for (int v = 1; v < variants.Length; v++)
            {
                double baseline = EA27_KeywordOrder.Rate(t, 0, two), handBaseline = EA27_KeywordOrder.Rate(h, 0, two);
                double gain = EA27_KeywordOrder.Rate(t, v, two) - baseline;
                double harm = EA27_KeywordOrder.Rate(t, v, longer) - EA27_KeywordOrder.Rate(t, 0, longer);
                double worstLanguage = RealisticCorpus.Languages.Select(l => EA27_KeywordOrder.Rate(t, v, s => longer(s) && s.Probe.Language == l) - EA27_KeywordOrder.Rate(t, 0, s => longer(s) && s.Probe.Language == l)).Min();
                double handGain = EA27_KeywordOrder.Rate(h, v, two) - EA27_KeywordOrder.Rate(h, 0, two);
                double handHarm = EA27_KeywordOrder.Rate(h, v, longer) - EA27_KeywordOrder.Rate(h, 0, longer);
                bool meets = Meets(gain, baseline, harm, worstLanguage, handGain, handBaseline, handHarm);
                outcome.Analysis.Add(Names[v] + ": the words of 2 letters " + ContextLab.Signed(gain) + " points, the longer ones " + ContextLab.Signed(harm) + " (worst language " + ContextLab.Signed(worstLanguage) + "); hand-written: " + ContextLab.Signed(handGain) + " and " + ContextLab.Signed(handHarm) + (meets ? " (meets)" : " (does not meet)"));
                if (meets && Better(gain, harm, bestGain, bestHarm)) { best = v; bestGain = gain; bestHarm = harm; }
            }
            outcome.CriterionMet = best >= 0;
            outcome.Headline = best >= 0 ? "adopt: " + Names[best] + ", the words of 2 letters " + ContextLab.Signed(bestGain) + " points" : "no variant meets the criterion";
            outcome.Analysis.Add("The corpus is generated; 2-letter words in it are the ones the generators write (`id`, `db`, `as`, `on`, `uv`...).");
            return outcome;
        }

        /// <summary>The criterion as written for EA29: gains of 15 and 10 points for the words of 2 letters, no loss for the longer ones.</summary>
        protected virtual bool Meets(double gain, double baseline, double harm, double worstLanguage, double handGain, double handBaseline, double handHarm)
        {
            return gain >= 15 && harm >= -0.5 && worstLanguage >= -1 && handGain >= 10 && handHarm >= -1;
        }

        /// <summary>Among the variants that meet it, the one with the larger gain.</summary>
        protected virtual bool Better(double gain, double harm, double bestGain, double bestHarm) { return gain > bestGain; }

        private static Table Table(string title, List<EA27_KeywordOrder.Scored> scored)
        {
            var table = new Table(title, "Variant", "2-letter words: cases", "2-letter: first", "2-letter: first 5", "Longer: cases", "Longer: first", "Longer: first 5");
            Func<EA27_KeywordOrder.Scored, bool> two = s => s.Probe.Word.Length == 2, longer = s => s.Probe.Word.Length > 2;
            for (int v = 0; v < Names.Length; v++)
                table.Add(Names[v], scored.Count(s => s.Probe.Reachable && two(s)) / Names.Length, ContextLab.Pct(EA27_KeywordOrder.Rate(scored, v, two, 1)), ContextLab.Pct(EA27_KeywordOrder.Rate(scored, v, two)),
                    scored.Count(s => s.Probe.Reachable && longer(s)) / Names.Length, ContextLab.Pct(EA27_KeywordOrder.Rate(scored, v, longer, 1)), ContextLab.Pct(EA27_KeywordOrder.Rate(scored, v, longer)));
            return table;
        }

        private static Table ByLanguage(string title, List<EA27_KeywordOrder.Scored> scored)
        {
            var table = new Table(title, new[] { "Language", "2-letter cases" }.Concat(Names.Select(n => "2-letter: " + n)).Concat(Names.Select(n => "Longer: " + n)).ToArray());
            foreach (string language in RealisticCorpus.Languages)
            {
                string l = language;
                Func<EA27_KeywordOrder.Scored, bool> two = s => s.Probe.Word.Length == 2 && s.Probe.Language == l, longer = s => s.Probe.Word.Length > 2 && s.Probe.Language == l;
                table.Add(new object[] { language, scored.Count(s => s.Probe.Reachable && two(s)) / Names.Length }
                    .Concat(Enumerable.Range(0, Names.Length).Select(v => (object)ContextLab.Pct(EA27_KeywordOrder.Rate(scored, v, two))))
                    .Concat(Enumerable.Range(0, Names.Length).Select(v => (object)ContextLab.Pct(EA27_KeywordOrder.Rate(scored, v, longer)))).ToArray());
            }
            return table;
        }
    }
}
