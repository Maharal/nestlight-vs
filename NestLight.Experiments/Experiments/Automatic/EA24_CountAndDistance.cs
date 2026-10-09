using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;

namespace NestLight.Experiments
{
    internal sealed class EA24_CountAndDistance : Experiment
    {
        public override string Id { get { return "EA24"; } }
        public override string Title { get { return "Do the words used most often come before the nearest ones?"; } }
        public override string Hypothesis { get { return "The order by distance alone sends to the end a word that is used all over the file and is not close to the caret, while the nearest word may have been used once. A word used often counts, a word used far away counts less: ln(1 + count) - weight * ln(1 + distance). The edit history is not available to the engine, so how near an occurrence is to the caret stands for how recently the word was used. A blend puts the meant word in the first 5 more often than distance alone, whatever the locality of the code."; } }
        public override string Method { get { return "EA20's probes (words typed with 1 to 3 letters, the list the editor gets, the previous word, the language and the grammar on) on files of three localities (0: a name is picked at random; 0.5; 0.9: almost always one of the last few used), 50 files each. Five orders of the words of the document and of the words that followed the context: distance alone (what ran before), the count alone (the nearest among equals), and the blend with a weight of 1, 0.5 and 0.25 for the distance. Also the start of a session on files of 1,200 to 60,000 lines."; } }
        public override string Criterion { get { return "The best of the four other orders is at least 1.5 points better within the first 5 than distance alone at locality 0.5; at no locality it is worse than distance alone by more than 1 point; no language falls by more than 1 point at locality 0.5; and the session stays under 16 ms at 60,000 lines in every host."; } }
        public override string IfMet { get { return "Order the words by the best blend."; } }
        public override string IfNotMet { get { return "Keep the nearest first. If the gain depends on the locality, say so: the generator sets how often a name comes back."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var names = new[] { "Distance alone", "Count alone", "Blend, weight 1", "Blend, weight 0.5", "Blend, weight 0.25" };
            var orders = new[]
            {
                new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true),
                new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true, order: WordOrder.Frequency),
                new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true, order: WordOrder.Blend, blendWeight: 1),
                new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true, order: WordOrder.Blend, blendWeight: 0.5),
                new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true, order: WordOrder.Blend, blendWeight: 0.25),
            };
            var factories = orders.Select(ContextLab.Plugin).ToArray();

            var byLocality = new Table("Within the first 5, by locality of the code (how often a name is one of the last few used)", new[] { "Locality", "Cases" }.Concat(names).ToArray());
            ContextLab.Comparison middle = null;
            double worstLocality = 0;
            foreach (double locality in new[] { 0.0, 0.5, 0.9 })
            {
                var corpus = SyntheticCorpus.Generate(settings.Quick ? 8 : 50, 4321, locality, 40, true);
                var probes = ContextLab.Probes(corpus, settings.Quick ? 20 : 60, 3);
                ContextLab.MarkReachable(probes);
                ContextLab.Comparison c = ContextLab.Compare(probes, names, factories);
                byLocality.Add(new object[] { locality.ToString("0.0"), c.Cases }.Concat(Enumerable.Range(0, names.Length).Select(v => (object)ContextLab.Pct(c.Rate(v, p => true, 5)))).ToArray());
                double best = Enumerable.Range(1, 4).Max(v => c.Rate(v, p => true, 5));
                worstLocality = locality == 0.0 ? best - c.Rate(0, p => true, 5) : System.Math.Min(worstLocality, best - c.Rate(0, p => true, 5));
                if (locality == 0.5) middle = c;
            }
            outcome.Tables.Add(byLocality);
            ContextLab.AddTables(outcome, middle, "Locality 0.5: the word typed with 1 to 3 letters", 4);

            // not part of the criterion: the same orders with no other feature, where EA13 found the order by distance 8 points ahead of the alphabetical one
            var alone = new[] { CompletionFeatures.None, new CompletionFeatures(order: WordOrder.Frequency), new CompletionFeatures(order: WordOrder.Blend, blendWeight: 1),
                                new CompletionFeatures(order: WordOrder.Blend, blendWeight: 0.5), new CompletionFeatures(order: WordOrder.Blend, blendWeight: 0.25) };
            ContextLab.Comparison bare = ContextLab.Compare(middle.Probes, names, alone.Select(ContextLab.Plugin).ToArray());
            var bareTable = new Table("The same orders with no other feature (locality 0.5), within the first 5", "Order", "Within the first 5", "Mean reciprocal rank");
            for (int v = 0; v < names.Length; v++) bareTable.Add(names[v], ContextLab.Pct(bare.Rate(v, p => true, 5)), bare.Mrr(v, p => true).ToString("F3"));
            outcome.Tables.Add(bareTable);

            double worst; string worstCase;
            outcome.Tables.Add(ContextLab.Latency(settings, names, factories, "comp", out worst, out worstCase));

            int winner = Enumerable.Range(1, 4).OrderByDescending(v => middle.Rate(v, p => true, 5)).ThenBy(v => v).First();
            double gain = middle.Rate(winner, p => true, 5) - middle.Rate(0, p => true, 5);
            double worstLanguage = middle.Probes.Where(p => p.Reachable).Select(p => p.EmbeddedLanguageId).Distinct()
                .Select(id => middle.Rate(winner, p => p.EmbeddedLanguageId == id, 5) - middle.Rate(0, p => p.EmbeddedLanguageId == id, 5)).Min();
            outcome.CriterionMet = gain >= 1.5 && worstLocality >= -1 && worstLanguage >= -1 && worst < 16;
            outcome.Headline = "best order (" + names[winner] + ") within the first 5: " + ContextLab.Pct(middle.Rate(0, p => true, 5)) + " to " + ContextLab.Pct(middle.Rate(winner, p => true, 5)) + " (" + ContextLab.Signed(gain) + " points) at locality 0.5; worst session " + Measure.Ms(worst);
            outcome.Analysis.Add("At locality 0.5 the best order is " + names[winner] + ": " + ContextLab.Signed(gain) + " points within the first 5; the worst language changes by " + ContextLab.Signed(worstLanguage) + " points; across the three localities the smallest advantage of the best order over distance alone is " + ContextLab.Signed(worstLocality) + " points.");
            outcome.Analysis.Add("The slowest session with the last order at the largest size is " + worstCase + ": " + Measure.Ms(worst) + " (the frame budget is 16 ms). Ranking by count has to look at every occurrence in the window, not only at the first words it offers.");
            outcome.Analysis.Add("The generator sets how often a name comes back and how close to its last use (the locality); the gain of a blend depends on both, and no real file was measured. Recency here is the distance in the text, not the order of the edits.");
            return outcome;
        }
    }
}
