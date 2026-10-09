using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA22_SimilarRecovery : Experiment
    {
        public override string Id { get { return "EA22"; } }
        public override string Title { get { return "Does the second stage recover the word after one mistake, and which tie-break works?"; } }
        public override string Hypothesis { get { return "When the typed text has one edit (an extra letter, a missing one, a wrong one, two swapped) in a prefix of 4 to 8 letters, the word the user meant is among the first 5 suggestions in most cases. Among the words that are the same number of edits away, the nearest to the caret is no worse a tie-break than the most frequent."; } }
        public override string Method { get { return "50 generated files (EA18's corpus, locality 0.5). A sample of the words of the embedded strings, 5 or more letters long, is typed as a prefix of 4 to 8 letters with one edit of each kind, at a place picked at random over the whole prefix (the first letter included). Only the cases where the word exists elsewhere in the document, or is a keyword, count (reachable). The second stage is on, with the defaults, and the list is also reordered in three ways inside each group of the same kind and distance: nearest to the caret first (the engine), most frequent first, most frequent and then nearest."; } }
        public override string Criterion { get { return "The meant word is within the first 5 in at least 70% of the reachable cases, with the best of the three tie-breaks. If more than one reaches 70%, the one with the best result enters; if they tie, the nearest to the caret stays (it is the first stage's order)."; } }
        public override string IfMet { get { return "Keep the second stage as designed, with the tie-break that won."; } }
        public override string IfNotMet { get { return "Look at the table by kind and by place of the mistake: a mistake in the first letter is out of reach by design; a kind that fails alone points at the distance or at the tolerance."; } }

        private sealed class Variant
        {
            public string Name;
            public Func<List<Suggestion>, Dictionary<string, int>, Dictionary<string, int>, List<string>> Order;
        }

        private static int Spelling(string a, string b)
        {
            int c = string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
            return c != 0 ? c : string.CompareOrdinal(a, b);
        }

        private static List<string> Reorder(List<Suggestion> items, Func<Suggestion, IComparable[]> keys)
        {
            var exact = items.Where(s => s.Distance == 0).Select(s => s.Text);
            var similar = items.Where(s => s.Distance > 0).OrderBy(s => s, Comparer<Suggestion>.Create((a, b) =>
            {
                IComparable[] x = keys(a), y = keys(b);
                for (int i = 0; i < x.Length; i++) { int c = x[i].CompareTo(y[i]); if (c != 0) return c; }
                return Spelling(a.Text, b.Text);
            })).Select(s => s.Text);
            return exact.Concat(similar).ToList();
        }

        private static readonly Variant[] Variants =
        {
            new Variant { Name = "nearest to the caret (the engine)", Order = (items, counts, near) => items.Select(s => s.Text).ToList() },
            new Variant { Name = "most frequent", Order = (items, counts, near) => Reorder(items, s => new IComparable[] { s.Kind == SuggestionKind.Keyword ? 0 : 1, s.Distance, -Count(counts, s) }) },
            new Variant { Name = "most frequent, then nearest", Order = (items, counts, near) => Reorder(items, s => new IComparable[] { s.Kind == SuggestionKind.Keyword ? 0 : 1, s.Distance, -Count(counts, s), Near(near, s) }) },
        };

        private static int Count(Dictionary<string, int> counts, Suggestion s) { int c; return s.Kind == SuggestionKind.Word && counts.TryGetValue(s.Text, out c) ? c : 0; }
        private static int Near(Dictionary<string, int> near, Suggestion s) { int d; return s.Kind == SuggestionKind.Word && near.TryGetValue(s.Text, out d) ? d : 0; }

        private sealed class Tally
        {
            public int Cases, First, Five, Ten;
            public void Add(int rank) { Cases++; if (rank == 0) First++; if (rank >= 0 && rank < 5) Five++; if (rank >= 0 && rank < 10) Ten++; }
            public string FiveRate { get { return Cases == 0 ? "-" : (100.0 * Five / Cases).ToString("F1") + "%"; } }
        }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            int files = settings.Quick ? 8 : 50, perFile = settings.Quick ? 20 : 60;
            var corpus = SyntheticCorpus.Generate(files, 1234, 0.5);

            var engines = new Dictionary<HostLanguage, CompletionEngine>();
            foreach (HostLanguage h in SyntheticCode.Hosts) engines[h] = CompletionLab.Engine(h, CompletionLab.Unlimited, CompletionEngine.DefaultMinWordLength, true, CompletionEngine.DefaultFuzzyBelow, 1000);

            var overall = Variants.Select(v => new Tally()).ToArray();
            var byKind = new Dictionary<string, Tally>();
            var byLength = new Dictionary<int, Tally>();
            var byPlace = new Dictionary<string, Tally>();
            int cases = 0, unreachable = 0, answeredByFirstStage = 0, similarRan = 0;

            foreach (CompletionLab.Typing t in CompletionLab.OneEditPrefixes(corpus, perFile, 99))
            {
                cases++;
                CompletionEngine engine = engines[t.Host];
                CompletionSite site = engine.Locate(t.Text, t.Caret);
                if (site == null) { unreachable++; continue; }

                Dictionary<string, int> counts, near;
                CompletionLab.WordFacts(t.Text, t.Caret, site.EmbeddedLanguageId, out counts, out near);
                bool reachable = counts.ContainsKey(t.Word) || CompletionLanguages.Default.Find(site.EmbeddedLanguageId).Keywords.Contains(t.Word, StringComparer.OrdinalIgnoreCase);
                if (!reachable) { unreachable++; continue; }

                List<Suggestion> items = engine.Suggest(t.Text, site).ToList();
                if (items.Any(s => s.Distance == 0)) answeredByFirstStage++; else if (items.Count > 0) similarRan++;

                for (int v = 0; v < Variants.Length; v++)
                {
                    int rank = CompletionLab.RankOf(Variants[v].Order(items, counts, near), t.Word);
                    overall[v].Add(rank);
                    if (v != 0) continue;
                    Tally a, b, c;
                    if (!byKind.TryGetValue(t.Kind, out a)) byKind[t.Kind] = a = new Tally();
                    if (!byLength.TryGetValue(t.Typed.Length, out b)) byLength[t.Typed.Length] = b = new Tally();
                    string place = t.EditAt == 0 ? "the first letter" : "a later letter";
                    if (!byPlace.TryGetValue(place, out c)) byPlace[place] = c = new Tally();
                    a.Add(rank); b.Add(rank); c.Add(rank);
                }
            }

            int reached = overall[0].Cases;
            var table = new Table("One mistake in the prefix: " + files + " files, " + cases + " cases, " + reached + " reachable (" + Percent(reached, cases) + ")",
                "Tie-break", "Meant word first", "Within the first 5", "Within the first 10");
            for (int v = 0; v < Variants.Length; v++)
                table.Add(Variants[v].Name, Percent(overall[v].First, reached), Percent(overall[v].Five, reached), Percent(overall[v].Ten, reached));
            outcome.Tables.Add(table);

            var kinds = new Table("By kind of mistake (the engine's order)", "Mistake", "Cases", "Within the first 5");
            foreach (var pair in byKind.OrderBy(p => p.Key)) kinds.Add(pair.Key, pair.Value.Cases, pair.Value.FiveRate);
            outcome.Tables.Add(kinds);
            var lengths = new Table("By length of what was typed (the engine's order)", "Typed letters", "Cases", "Within the first 5");
            foreach (var pair in byLength.OrderBy(p => p.Key)) lengths.Add(pair.Key, pair.Value.Cases, pair.Value.FiveRate);
            outcome.Tables.Add(lengths);
            var places = new Table("By place of the mistake (the engine's order)", "Place", "Cases", "Within the first 5");
            foreach (var pair in byPlace.OrderBy(p => p.Key)) places.Add(pair.Key, pair.Value.Cases, pair.Value.FiveRate);
            outcome.Tables.Add(places);

            double best = Variants.Select((v, i) => overall[i].Cases == 0 ? 0 : 100.0 * overall[i].Five / overall[i].Cases).Max();
            int winner = Enumerable.Range(0, Variants.Length).OrderByDescending(i => overall[i].Five).ThenBy(i => i).First();
            outcome.CriterionMet = best >= 70;
            outcome.Headline = "best tie-break (" + Variants[winner].Name + ") " + best.ToString("F1") + "% within the first 5";
            outcome.Analysis.Add("In " + answeredByFirstStage + " of the " + reached + " reachable cases the mistake happened to leave an exact prefix of another word, so the first stage answered and the second did not run; in " + similarRan + " the second stage ran and found something.");
            outcome.Analysis.Add("A mistake in the first letter cannot be recovered with the first letter required (a design decision); it is in the table by place. The cases not reachable (" + unreachable + ") are words that exist nowhere else in the document.");
            outcome.Analysis.Add("The corpus is generated and its names repeat by construction; the mistakes are uniform over the prefix, not the way people really mistype.");
            return outcome;
        }

        private static string Percent(int part, int whole)
        {
            return whole == 0 ? "-" : (100.0 * part / whole).ToString("F1") + "%";
        }
    }
}
