using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class E18_CompletionRobustness : Experiment
    {
        public override string Id { get { return "E18"; } }
        public override string Title { get { return "Completion on incomplete and cut code"; } }
        public override string Hypothesis { get { return "Completion runs while the code is being typed, so it sees unterminated strings, half-written interpolations and carets anywhere. For every text and every caret, Locate and Suggest must not throw, the site must lie inside the text and cover only word characters, and the suggestions must start with what was typed, without repeats."; } }
        public override string Method { get { return "50 generated files (E20's generator, 4 hosts). For each file: every prefix cut at a stride, and every single-character deletion at a stride; in each text, the caret at the end, at the start, and at 5 seeded random positions. 6 invariants checked on every result."; } }
        public override string Criterion { get { return "Zero violations."; } }
        public override string IfMet { get { return "Completion can be triggered anywhere in a file being edited."; } }
        public override string IfNotMet { get { return "The violations are listed: each is an input that would throw in the editor or offer a wrong replacement."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var corpus = SyntheticCorpus.Generate(settings.Quick ? 8 : 50, 42, 0.5, blocksPerFile: 4);
            var violations = new Dictionary<string, int>();
            var examples = new List<string>();
            long texts = 0, carets = 0, sites = 0;
            var random = new Random(7);

            foreach (CorpusFile file in corpus)
            {
                CompletionEngine engine = CompletionLab.Engine(file.Host, CompletionEngine.DefaultMaxItems);
                int length = file.Text.Length;
                int stride = Math.Max(1, length / (settings.Quick ? 30 : 120));
                var variants = new List<string>();
                for (int cut = 0; cut <= length; cut += stride) variants.Add(file.Text.Substring(0, cut));
                for (int at = 0; at < length; at += stride) variants.Add(file.Text.Remove(at, 1));

                foreach (string text in variants)
                {
                    texts++;
                    var positions = new List<int> { 0, text.Length };
                    for (int i = 0; i < 5 && text.Length > 0; i++) positions.Add(random.Next(text.Length + 1));
                    foreach (int caret in positions)
                    {
                        carets++;
                        try
                        {
                            CompletionSite site = engine.Locate(text, caret);
                            if (site == null) continue;
                            sites++;
                            if (!(0 <= site.Start && site.Start <= site.Caret && site.Caret == caret && site.Caret <= site.End && site.End <= text.Length))
                                Violation(violations, examples, "site outside the text or not around the caret", file, text, caret);
                            else
                            {
                                string typed = text.Substring(site.Start, site.PrefixLength);
                                if (typed.Any(c => !CompletionLab.IsWordChar(site.LanguageId, c)))
                                    Violation(violations, examples, "the prefix holds a character that is not a word character", file, text, caret);

                                IReadOnlyList<Suggestion> items = engine.Suggest(text, site);
                                if (items.Count > CompletionEngine.DefaultMaxItems) Violation(violations, examples, "more suggestions than the limit", file, text, caret);
                                if (items.Any(s => !s.Text.StartsWith(typed, StringComparison.OrdinalIgnoreCase)))
                                    Violation(violations, examples, "a suggestion does not start with the prefix", file, text, caret);
                                if (items.Any(s => s.Text.Length == typed.Length))
                                    Violation(violations, examples, "a suggestion adds nothing to the prefix", file, text, caret);
                                if (items.Select(s => s.Text.ToLowerInvariant()).Distinct().Count() != items.Count)
                                    Violation(violations, examples, "a suggestion is repeated", file, text, caret);
                            }
                        }
                        catch (Exception ex)
                        {
                            Violation(violations, examples, "threw " + ex.GetType().Name, file, text, caret);
                        }
                    }
                }
            }

            int total = violations.Values.Sum();
            var table = new Table("Checks", "Check", "Value");
            table.Add("Files", corpus.Count);
            table.Add("Texts (cuts and deletions)", texts);
            table.Add("Carets tried", carets);
            table.Add("Carets inside embedded code", sites);
            table.Add("Violations", total);
            outcome.Tables.Add(table);
            if (total > 0)
            {
                var kinds = new Table("Violations by kind", "Kind", "Count");
                foreach (var pair in violations.OrderByDescending(p => p.Value)) kinds.Add(pair.Key, pair.Value);
                outcome.Tables.Add(kinds);
                foreach (string example in examples.Take(5)) outcome.Analysis.Add("Example: " + example);
            }
            outcome.CriterionMet = total == 0;
            outcome.Headline = total + " violations in " + carets + " carets";
            outcome.Analysis.Add(sites + " of the " + carets + " carets were inside the code of an embedded string and went through Suggest.");
            return outcome;
        }

        private static void Violation(Dictionary<string, int> counts, List<string> examples, string kind, CorpusFile file, string text, int caret)
        {
            int n;
            counts[kind] = counts.TryGetValue(kind, out n) ? n + 1 : 1;
            if (n == 0 && examples.Count < 20)
            {
                int from = Math.Max(0, caret - 25);
                examples.Add(kind + " (" + file.Host + ", caret " + caret + "): `" + text.Substring(from, Math.Min(text.Length, caret + 15) - from).Replace("\n", "\\n") + "`");
            }
        }
    }
}
