using System.Linq;
using NestLight.Completion;

namespace NestLight.Experiments
{
    internal sealed class EA20_PreviousWord : Experiment
    {
        public override string Id { get { return "EA20"; } }
        public override string Title { get { return "Does the word before the caret help to rank the suggestions?"; } }
        public override string Hypothesis { get { return "The words that already followed the same word (with the same punctuation) elsewhere in the document are the likely ones: after `from ` the word that followed `from` before, after `display: ` the value that followed `display:`. Putting them first puts the word the user means among the first 5 more often than the order by distance alone, and one more pass over the window still fits in a frame."; } }
        public override string Method { get { return "50 generated files with structure (SQL over a schema with a CREATE TABLE for each table, CSS whose values belong to the property, HTML whose attributes belong to the tag; locality 0.5). A sample of the words of the embedded strings, 4 or more letters long, is typed as a prefix of 1, 2 and 3 letters, with the rest of the word removed. Only the cases where the word exists elsewhere in the document or is a keyword count (reachable). The list is the one the editor gets (100 items, the second stage on). Two variants: the order by distance alone, and the previous word first. Also the start of a session (Locate twice and Suggest, the scan shared) on files of 1,200 to 60,000 lines."; } }
        public override string Criterion { get { return "Over all the reachable cases the word is within the first 5 in at least 3 percentage points more cases with the previous word than without; in no language it falls by more than 1 point; and the session with the previous word stays under 16 ms at 60,000 lines, in every host."; } }
        public override string IfMet { get { return "Keep the previous word on."; } }
        public override string IfNotMet { get { return "Look at the table by the word before: a gain in some contexts and a loss in others points at which contexts to keep; no gain anywhere says the corpus has nothing to find, or the idea is not worth its pass."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var corpus = SyntheticCorpus.Generate(settings.Quick ? 8 : 50, 4321, 0.5, 40, true);
            var probes = ContextLab.Probes(corpus, settings.Quick ? 20 : 60, 3);
            ContextLab.MarkReachable(probes);

            var names = new[] { "By distance alone", "Previous word first" };
            var features = new[] { CompletionFeatures.None, new CompletionFeatures(previousWord: true) };
            var factories = features.Select(ContextLab.Plugin).ToArray();
            ContextLab.Comparison c = ContextLab.Compare(probes, names, factories);
            ContextLab.AddTables(outcome, c, "The word typed with 1 to 3 letters");

            double worst; string worstCase;
            outcome.Tables.Add(ContextLab.Latency(settings, names, factories, "comp", out worst, out worstCase));

            double gain = c.Rate(1, p => true, 5) - c.Rate(0, p => true, 5);
            double worstLanguage = c.Probes.Where(p => p.Reachable).Select(p => p.EmbeddedLanguageId).Distinct()
                .Select(id => c.Rate(1, p => p.EmbeddedLanguageId == id, 5) - c.Rate(0, p => p.EmbeddedLanguageId == id, 5)).Min();
            outcome.CriterionMet = gain >= 3 && worstLanguage >= -1 && worst < 16;
            outcome.Headline = "within the first 5: " + ContextLab.Pct(c.Rate(0, p => true, 5)) + " to " + ContextLab.Pct(c.Rate(1, p => true, 5)) + " (" + ContextLab.Signed(gain) + " points); worst session " + Measure.Ms(worst);
            outcome.Analysis.Add("The gain over all the reachable cases is " + ContextLab.Signed(gain) + " points within the first 5; the worst language changes by " + ContextLab.Signed(worstLanguage) + " points.");
            outcome.Analysis.Add("The slowest session with the previous word at the largest size is " + worstCase + ": " + Measure.Ms(worst) + " (the frame budget is 16 ms).");
            outcome.Analysis.Add("The corpus is generated. It has the structure the idea looks for (the values of a property, the table after `from`), because the generator was written with it; real code may repeat its structure more or less than that.");
            return outcome;
        }
    }
}
