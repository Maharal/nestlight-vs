using System;
using System.Collections.Generic;
using System.Linq;

namespace NestLight.Experiments
{
    internal sealed class EA13_RankingQuality : Experiment
    {
        public override string Id { get { return "EA13"; } }
        public override string Title { get { return "Order of the words of the document"; } }
        public override string Hypothesis { get { return "Listing the words that already exist in the document nearest to the caret first puts the word the user wants among the first five more often than listing them alphabetically, by frequency or by first appearance."; } }
        public override string Method { get { return "50 generated files (SyntheticCorpus: 4 hosts, SQL / HTML / CSS / GraphQL strings, a pool of 40 names reused as variables, columns, classes and fields). A sample of the words of the embedded strings is typed 1, 2 and 3 characters at a time, with the rest of the word removed, and the position of the right word in each ordering is recorded. Only the cases where the word still exists in the document or in the vocabulary (reachable) count. The same 50 files are generated with 3 settings of locality: how much the generator reuses the names it used last."; } }
        public override string Criterion { get { return "With medium locality (0.5), the nearest-first order has a hit rate within the first 5 at least 5 percentage points above the alphabetical order."; } }
        public override string IfMet { get { return "Keep the order by distance."; } }
        public override string IfNotMet { get { return "Nearest-first is not worth its extra code: sort alphabetically, or rank by frequency."; } }

        private sealed class Ordering
        {
            public string Name;
            public Func<ProbeResult, List<string>> Order;
        }

        private static readonly Ordering[] Orderings =
        {
            new Ordering { Name = "keywords, then words nearest first (the engine)", Order = p => p.Keywords.Concat(p.Words).ToList() },
            new Ordering { Name = "keywords, then words alphabetical", Order = p => p.Keywords.Concat(p.Words.OrderBy(w => w, StringComparer.OrdinalIgnoreCase)).ToList() },
            new Ordering { Name = "keywords, then words by frequency", Order = p => p.Keywords.Concat(p.Words.OrderByDescending(w => Count(p, w)).ThenBy(w => w, StringComparer.Ordinal)).ToList() },
            new Ordering { Name = "keywords, then words by first appearance", Order = p => p.Keywords.Concat(p.Words.OrderBy(w => First(p, w))).ToList() },
            new Ordering { Name = "words nearest first, then keywords", Order = p => p.Words.Concat(p.Keywords).ToList() },
        };

        private static int Count(ProbeResult p, string word) { int[] s; return p.Stats.TryGetValue(word, out s) ? s[0] : 0; }
        private static int First(ProbeResult p, string word) { int[] s; return p.Stats.TryGetValue(word, out s) ? s[1] : int.MaxValue; }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            int files = settings.Quick ? 8 : 50, perFile = settings.Quick ? 20 : 60;
            double nearest = 0, alphabetical = 0;

            foreach (double locality in new[] { 0.5, 0.0, 0.9 })
            {
                var corpus = SyntheticCorpus.Generate(files, 1234, locality);
                var probes = CompletionLab.Probe(corpus, 3, perFile, scopes: false).ToList();
                var reachable = probes.Where(p => CompletionLab.RankOf(Orderings[0].Order(p), p.Word) >= 0).ToList();

                var table = new Table("Locality " + locality.ToString("F1") + ": " + files + " files, " + probes.Count + " cases, " + reachable.Count + " reachable (" + Percent(reachable.Count, probes.Count) + ")",
                    "Order", "Right word first", "Within the first 5", "Within the first 10", "Mean reciprocal rank");
                foreach (Ordering ordering in Orderings)
                {
                    int first = 0, five = 0, ten = 0;
                    double reciprocal = 0;
                    foreach (ProbeResult p in reachable)
                    {
                        int rank = CompletionLab.RankOf(ordering.Order(p), p.Word);
                        if (rank < 0) continue;
                        if (rank == 0) first++;
                        if (rank < 5) five++;
                        if (rank < 10) ten++;
                        reciprocal += 1.0 / (rank + 1);
                    }
                    int n = Math.Max(1, reachable.Count);
                    table.Add(ordering.Name, Percent(first, n), Percent(five, n), Percent(ten, n), (reciprocal / n).ToString("F3"));
                    if (locality == 0.5 && ordering == Orderings[0]) nearest = 100.0 * five / n;
                    if (locality == 0.5 && ordering == Orderings[1]) alphabetical = 100.0 * five / n;
                }
                outcome.Tables.Add(table);
            }

            outcome.CriterionMet = nearest - alphabetical >= 5;
            outcome.Headline = "nearest " + nearest.ToString("F1") + "% against alphabetical " + alphabetical.ToString("F1") + "% within the first 5 (locality 0.5)";
            outcome.Analysis.Add("At medium locality, the right word is within the first 5 in " + nearest.ToString("F1") + "% of the cases with the nearest-first order and " + alphabetical.ToString("F1") + "% with the alphabetical order.");
            outcome.Analysis.Add("The corpus is generated. The advantage of nearest-first comes from how the generator reuses names (locality), so the table for locality 0.0 is the fairest comparison and the one for 0.9 the most favorable. Real code was not measured.");
            return outcome;
        }

        private static string Percent(int part, int whole)
        {
            return whole == 0 ? "-" : (100.0 * part / whole).ToString("F1") + "%";
        }
    }
}
