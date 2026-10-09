using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal class E27_SimilarRobustness : Experiment
    {
        public override string Id { get { return "E27"; } }
        public override string Title { get { return "Completion with similar words on incomplete and cut code"; } }
        public override string Hypothesis { get { return "This replaces E18, whose invariant (every suggestion starts with what was typed) no longer holds for similar words. Completion still runs while the code is being typed: for every text and every caret, Suggest must not throw; the site must lie inside the text and cover only word characters; the exact suggestions must start with the typed text; the similar ones must be at the distance they claim (as the definition computes it) between 1 and the tolerance, with the first letter typed, and come after the exact ones; nothing repeats; and the limits hold."; } }
        public override string Method { get { return "50 generated files (E20's generator, 4 hosts). For each file: every prefix cut at a stride, and every single-character deletion at a stride; in each text, the caret at the end, at the start, and at 5 seeded random positions. In every position inside embedded code, also the same text with a one-letter mistake put in the word under the caret, so that the second stage runs often. The distance of every similar item is recomputed with the whole matrix of the definition."; } }
        public override string Criterion { get { return "Zero violations."; } }
        public override string IfMet { get { return "Completion can be triggered anywhere in a file being edited, with the second stage on."; } }
        public override string IfNotMet { get { return "The violations are listed: each is an input that would throw in the editor or offer a wrong replacement."; } }

        /// <summary>The context features the engine runs with; null: none.</summary>
        protected virtual CompletionFeatures Features { get { return null; } }
        /// <summary>Whether the files are the structured ones (SQL over a schema, CSS values, HTML attributes).</summary>
        protected virtual bool Structured { get { return false; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var corpus = SyntheticCorpus.Generate(settings.Quick ? 8 : 50, 42, 0.5, blocksPerFile: 4, structured: Structured);
            var violations = new Dictionary<string, int>();
            var examples = new List<string>();
            long texts = 0, carets = 0, sites = 0, damaged = 0, similarItems = 0;
            var random = new Random(7);

            foreach (CorpusFile file in corpus)
            {
                CompletionEngine engine = CompletionLab.Engine(file.Host, CompletionEngine.DefaultMaxItems, CompletionEngine.DefaultMinWordLength, true, features: Features);
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
                        sites += Check(engine, file, text, caret, violations, examples, ref similarItems);

                        // the same position with a mistake in the word being typed
                        CompletionSite site = null;
                        try { site = engine.Locate(text, caret); } catch (Exception) { }
                        if (site != null && site.PrefixLength >= 3)
                        {
                            var chars = text.ToCharArray();
                            int at = site.Start + random.Next(site.PrefixLength);
                            chars[at] = chars[at] == 'q' ? 'w' : 'q';
                            damaged++;
                            Check(engine, file, new string(chars), caret, violations, examples, ref similarItems);
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
            table.Add("Of them, with a mistake put in the word", damaged);
            table.Add("Similar items checked against the definition", similarItems);
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
            outcome.Headline = total + " violations in " + carets + " carets, " + similarItems + " similar items checked";
            outcome.Analysis.Add(sites + " of the " + carets + " carets were inside the code of an embedded string and went through Suggest; " + damaged + " more with a mistake in the word.");
            return outcome;
        }

        /// <returns>1 when the caret was inside embedded code.</returns>
        private static int Check(CompletionEngine engine, CorpusFile file, string text, int caret, Dictionary<string, int> violations, List<string> examples, ref long similarItems)
        {
            try
            {
                CompletionSite site = engine.Locate(text, caret);
                if (site == null) return 0;
                if (!(0 <= site.Start && site.Start <= site.Caret && site.Caret == caret && site.Caret <= site.End && site.End <= text.Length))
                {
                    Violation(violations, examples, "site outside the text or not around the caret", file, text, caret);
                    return 1;
                }
                string typed = text.Substring(site.Start, site.PrefixLength);
                if (typed.Any(c => !CompletionLab.IsWordChar(site.LanguageId, c)))
                    Violation(violations, examples, "the prefix holds a character that is not a word character", file, text, caret);

                IReadOnlyList<Suggestion> items = engine.Suggest(text, site);
                if (items.Count > CompletionEngine.DefaultMaxItems) Violation(violations, examples, "more suggestions than the limit", file, text, caret);
                if (items.Select(s => s.Text.ToLowerInvariant()).Distinct().Count() != items.Count) Violation(violations, examples, "a suggestion is repeated", file, text, caret);

                int k = CompletionEngine.ToleranceFor(typed.Length);
                bool similarSeen = false;
                int similarCount = 0;
                Suggestion previous = null;
                foreach (Suggestion s in items)
                {
                    if (s.Distance == 0)
                    {
                        if (!s.Text.StartsWith(typed, StringComparison.OrdinalIgnoreCase)) Violation(violations, examples, "an exact suggestion does not start with the prefix", file, text, caret);
                        if (s.Text.Length == typed.Length) Violation(violations, examples, "a suggestion adds nothing to the prefix", file, text, caret);
                        if (similarSeen) Violation(violations, examples, "an exact suggestion comes after a similar one", file, text, caret);
                    }
                    else
                    {
                        similarSeen = true; similarCount++; similarItems++;
                        if (typed.Length < CompletionEngine.FuzzyMinPrefix) Violation(violations, examples, "similar items for a prefix that is too short", file, text, caret);
                        if (s.Distance > k) Violation(violations, examples, "a similar item farther than the tolerance", file, text, caret);
                        if (CompletionLab.ReferenceDistance(typed, s.Text, k) != s.Distance) Violation(violations, examples, "the distance of a similar item is not the one of the definition", file, text, caret);
                        if (char.ToUpperInvariant(typed[0]) != char.ToUpperInvariant(s.Text[0])) Violation(violations, examples, "a similar item starts with another letter", file, text, caret);
                        if (previous != null && previous.Distance > 0)
                        {
                            bool keywordAfterWord = previous.Kind == SuggestionKind.Word && s.Kind == SuggestionKind.Keyword;
                            bool fartherFirst = previous.Kind == s.Kind && previous.Distance > s.Distance;
                            if (keywordAfterWord || fartherFirst) Violation(violations, examples, "similar items out of order", file, text, caret);
                        }
                    }
                    previous = s;
                }
                if (similarCount > CompletionEngine.DefaultFuzzyMaxItems) Violation(violations, examples, "more similar items than the limit", file, text, caret);
                return 1;
            }
            catch (Exception ex)
            {
                Violation(violations, examples, "threw " + ex.GetType().Name, file, text, caret);
                return 1;
            }
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
