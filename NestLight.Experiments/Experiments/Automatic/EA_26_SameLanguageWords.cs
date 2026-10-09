using System.Linq;
using NestLight.Completion;

namespace NestLight.Experiments
{
    internal sealed class EA_26_SameLanguageWords : Experiment
    {
        public override string Id { get { return "EA_26"; } }
        public override string Title { get { return "Do the words of the same language come first?"; } }
        public override string Hypothesis { get { return "A word written in the code of another string of the same language (a column in another SQL string) is likelier than a word of the host code or of a string of another language that happens to start with the same letters, even when the other one is nearer to the caret. Putting the words of the language first, and taking the context of the previous word from them only, puts the meant word in the first 5 more often."; } }
        public override string Method { get { return "EA_25's probes (50 generated files with structure, words typed with 1 to 3 letters, the list the editor gets). Four variants: the order by distance alone; the words of the language first; the previous word; both. Interpolations are host code. Also the start of a session on files of 1,200 to 60,000 lines, with the scan shared (the scope needs the strings, which the classifier already found)."; } }
        public override string Criterion { get { return "Adding the words of the language to the previous word is at least 2 points better within the first 5 over all the reachable cases; in no language it falls by more than 1 point; and the session with both stays under 16 ms at 60,000 lines, in every host."; } }
        public override string IfMet { get { return "Keep the words of the language first."; } }
        public override string IfNotMet { get { return "Look at the table by language: a language that loses points should not use the scope (a language with few strings has nothing to gain)."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var corpus = SyntheticCorpus.Generate(settings.Quick ? 8 : 50, 4321, 0.5, 40, true);
            var probes = ContextLab.Probes(corpus, settings.Quick ? 20 : 60, 3);
            ContextLab.MarkReachable(probes);

            var names = new[] { "By distance alone", "Words of the language", "Previous word", "Previous word and language" };
            var features = new[]
            {
                CompletionFeatures.None,
                new CompletionFeatures(sameLanguageWords: true),
                new CompletionFeatures(previousWord: true),
                new CompletionFeatures(previousWord: true, sameLanguageWords: true),
            };
            var factories = features.Select(ContextLab.Plugin).ToArray();
            ContextLab.Comparison c = ContextLab.Compare(probes, names, factories);
            ContextLab.AddTables(outcome, c, "The word typed with 1 to 3 letters");

            double worst; string worstCase;
            outcome.Tables.Add(ContextLab.Latency(settings, names, factories, "comp", out worst, out worstCase));

            double gain = c.Rate(3, p => true, 5) - c.Rate(2, p => true, 5);
            double worstLanguage = c.Probes.Where(p => p.Reachable).Select(p => p.EmbeddedLanguageId).Distinct()
                .Select(id => c.Rate(3, p => p.EmbeddedLanguageId == id, 5) - c.Rate(2, p => p.EmbeddedLanguageId == id, 5)).Min();
            outcome.CriterionMet = gain >= 2 && worstLanguage >= -1 && worst < 16;
            outcome.Headline = "within the first 5: previous word " + ContextLab.Pct(c.Rate(2, p => true, 5)) + ", with the language " + ContextLab.Pct(c.Rate(3, p => true, 5)) + " (" + ContextLab.Signed(gain) + " points); worst session " + Measure.Ms(worst);
            outcome.Analysis.Add("The words of the language alone move the share within the first 5 from " + ContextLab.Pct(c.Rate(0, p => true, 5)) + " to " + ContextLab.Pct(c.Rate(1, p => true, 5)) + "; added to the previous word, " + ContextLab.Signed(gain) + " points; the worst language changes by " + ContextLab.Signed(worstLanguage) + " points.");
            outcome.Analysis.Add("The slowest session with both at the largest size is " + worstCase + ": " + Measure.Ms(worst) + " (the frame budget is 16 ms).");
            outcome.Analysis.Add("The corpus is generated: its host variables are named after the same nouns as the tables, so a host word with the same first letters is common by construction. How often real code has that is not measured.");
            return outcome;
        }
    }
}
