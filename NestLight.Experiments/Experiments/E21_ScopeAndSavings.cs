using System;
using System.Collections.Generic;
using System.Linq;

namespace NestLight.Experiments
{
    internal sealed class E21_ScopeAndSavings : Experiment
    {
        public override string Id { get { return "E21"; } }
        public override string Title { get { return "Where the words come from, and how many keystrokes completion saves"; } }
        public override string Hypothesis { get { return "Offering the words of the whole document (the host code included, as Visual Studio Code does) saves more keystrokes than offering only the words that appear inside embedded strings, or only the words of the string being typed. The names of the host code pollute the list less than they help."; } }
        public override string Method { get { return "The corpus of E20 (50 generated files, locality 0.5). A sample of the words of the embedded strings is typed one character at a time, up to 5, with the rest removed. The completion is accepted at the first prefix where the right word is within the first 5 suggestions; the saving is the length of the word minus the characters typed minus one for the accepting key. Four scopes: keywords only, the string being typed, all embedded strings, the whole document (always with the keywords)."; } }
        public override string Criterion { get { return "The saving of the whole-document scope is within 2 percentage points of the best narrower scope, or above it."; } }
        public override string IfMet { get { return "The whole document is a good scope: nothing to gain from restricting it."; } }
        public override string IfNotMet { get { return "Restrict the words to the embedded strings (or to the current one)."; } }

        private sealed class Scope
        {
            public string Name;
            public Func<ProbeResult, List<string>> List;
        }

        private static readonly Scope[] Scopes =
        {
            new Scope { Name = "keywords only", List = p => p.Keywords },
            new Scope { Name = "the string being typed", List = p => p.Keywords.Concat(p.Words.Where(w => p.WordsInOwner.Contains(w))).ToList() },
            new Scope { Name = "all embedded strings", List = p => p.Keywords.Concat(p.Words.Where(w => p.WordsInEmbedded.Contains(w))).ToList() },
            new Scope { Name = "the whole document", List = p => p.Keywords.Concat(p.Words).ToList() },
        };

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            int files = settings.Quick ? 8 : 50, perFile = settings.Quick ? 20 : 60;
            var corpus = SyntheticCorpus.Generate(files, 1234, 0.5);
            var probes = CompletionLab.Probe(corpus, 5, perFile, scopes: true).ToList();

            // one group per word typed: its probes, from 1 character up
            var words = new List<List<ProbeResult>>();
            ProbeResult previous = null;
            List<ProbeResult> current = null;
            foreach (ProbeResult p in probes)
            {
                if (previous == null || p.PrefixLength <= previous.PrefixLength || p.Word != previous.Word) { current = new List<ProbeResult>(); words.Add(current); }
                current.Add(p);
                previous = p;
            }

            var table = new Table("Hit rate and saving by scope (" + words.Count + " words typed, " + probes.Count + " cases)",
                "Scope", "Within the first 5 after 1 character", "after 2", "after 3", "Characters saved");
            double whole = 0, bestNarrow = 0;
            string bestName = "";
            foreach (Scope scope in Scopes)
            {
                var hits = new int[4];
                var totals = new int[4];
                double typed = 0, saved = 0;
                foreach (List<ProbeResult> typing in words)
                {
                    int length = typing[0].Word.Length;
                    typed += length;
                    bool accepted = false;
                    foreach (ProbeResult p in typing)
                    {
                        int rank = CompletionLab.RankOf(scope.List(p), p.Word);
                        bool hit = rank >= 0 && rank < 5;
                        if (p.PrefixLength <= 3) { totals[p.PrefixLength]++; if (hit) hits[p.PrefixLength]++; }
                        if (hit && !accepted) { saved += Math.Max(0, length - p.PrefixLength - 1); accepted = true; }
                    }
                }
                double saving = 100.0 * saved / Math.Max(1, typed);
                table.Add(scope.Name, Percent(hits[1], totals[1]), Percent(hits[2], totals[2]), Percent(hits[3], totals[3]), saving.ToString("F1") + "%");
                if (scope.Name == "the whole document") whole = saving;
                else if (scope.Name != "keywords only" && saving > bestNarrow) { bestNarrow = saving; bestName = scope.Name; }
            }

            outcome.Tables.Add(table);
            outcome.CriterionMet = whole >= bestNarrow - 2;
            outcome.Headline = "whole document saves " + whole.ToString("F1") + "% of the characters; best narrower scope (" + bestName + ") " + bestNarrow.ToString("F1") + "%";
            outcome.Analysis.Add("Saving = characters of the words that completion would have typed, over all the characters of those words.");
            outcome.Analysis.Add("The corpus is generated, and the words that are typed come from the embedded strings only. That favors the narrower scopes: a word that a user types in a string and that exists only in host code (for example the name of a variable) is never a target here.");
            return outcome;
        }

        private static string Percent(int part, int whole)
        {
            return whole == 0 ? "-" : (100.0 * part / whole).ToString("F1") + "%";
        }
    }
}
