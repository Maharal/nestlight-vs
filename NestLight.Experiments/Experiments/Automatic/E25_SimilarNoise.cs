using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class E25_SimilarNoise : Experiment
    {
        public override string Id { get { return "E25"; } }
        public override string Title { get { return "Does the second stage get in the way when the prefix is right?"; } }
        public override string Hypothesis { get { return "If the second stage ran whenever the first one found few items, a correct prefix would often get a list full of words that are only similar by chance. Running it only when nothing matched (FuzzyBelow = 1) keeps that noise rare."; } }
        public override string Method { get { return "The corpus of E20 (50 generated files). A sample of the words of the embedded strings is typed correctly, from 3 to 8 letters, with the rest of the word removed. The engine runs with FuzzyBelow = 1, 3 and 5, and the cases where it adds similar items are counted, apart for those where the first stage found something and those where it found nothing."; } }
        public override string Criterion { get { return "With FuzzyBelow = 1, the second stage adds items in at most 5% of the cases. The default is then chosen among the values that meet it."; } }
        public override string IfMet { get { return "Keep FuzzyBelow = 1 (the second stage as a fallback)."; } }
        public override string IfNotMet { get { return "Require more letters, a shorter list or a stricter filter before the second stage runs."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            int files = settings.Quick ? 8 : 50, perFile = settings.Quick ? 20 : 60;
            var corpus = SyntheticCorpus.Generate(files, 1234, 0.5);
            var typings = CompletionLab.CorrectPrefixes(corpus, perFile, 3, 8).ToList();

            var below = new[] { 1, 3, 5 };
            var engines = below.Select(b => SyntheticCode.Hosts.ToDictionary(h => h, h => CompletionLab.Engine(h, CompletionEngine.DefaultMaxItems, CompletionEngine.DefaultMinWordLength, true, b, CompletionEngine.DefaultFuzzyMaxItems))).ToArray();
            var plain = SyntheticCode.Hosts.ToDictionary(h => h, h => CompletionLab.Engine(h, CompletionEngine.DefaultMaxItems));

            var added = new int[below.Length];
            var addedWhenNothing = new int[below.Length];
            var addedWhenSomething = new int[below.Length];
            var similarItems = new int[below.Length];
            int cases = 0, nothing = 0, something = 0;

            foreach (CompletionLab.Typing t in typings)
            {
                CompletionSite site = plain[t.Host].Locate(t.Text, t.Caret);
                if (site == null) continue;
                cases++;
                bool exactFound = plain[t.Host].Suggest(t.Text, site).Count > 0;
                if (exactFound) something++; else nothing++;

                for (int b = 0; b < below.Length; b++)
                {
                    int similar = engines[b][t.Host].Suggest(t.Text, site).Count(s => s.Distance > 0);
                    if (similar == 0) continue;
                    added[b]++;
                    similarItems[b] += similar;
                    if (exactFound) addedWhenSomething[b]++; else addedWhenNothing[b]++;
                }
            }

            var table = new Table("Correct prefixes of 3 to 8 letters: " + files + " files, " + cases + " cases (" + nothing + " with no exact match, " + something + " with one or more)",
                "FuzzyBelow", "Cases with similar items added", "of which the first stage found nothing", "of which it found something", "Similar items per case that has them");
            for (int b = 0; b < below.Length; b++)
                table.Add(below[b], Percent(added[b], cases), addedWhenNothing[b], addedWhenSomething[b], added[b] == 0 ? "-" : (similarItems[b] / (double)added[b]).ToString("F1"));
            outcome.Tables.Add(table);

            double rate = cases == 0 ? 0 : 100.0 * added[0] / cases;
            outcome.CriterionMet = rate <= 5;
            outcome.Headline = "with FuzzyBelow = 1 similar items are added in " + rate.ToString("F1") + "% of the cases";
            outcome.Analysis.Add("With FuzzyBelow = 1 the second stage can only run when the first found nothing: " + Percent(nothing, cases) + " of the cases here (the words that exist nowhere else in the document). Those are not mistakes of the user, but the list gains similar words in some of them.");
            outcome.Analysis.Add("The corpus is generated, so how often a word has no twin in the document is a property of the generator.");
            return outcome;
        }

        private static string Percent(int part, int whole)
        {
            return whole == 0 ? "-" : (100.0 * part / whole).ToString("F1") + "%";
        }
    }
}
