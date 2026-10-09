using NestLight.Common;
using System.Collections.Generic;
using System.Linq;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA31_CombinationMatrix : Experiment
    {
        public override string Id { get { return "EA31"; } }
        public override string Title { get { return "Every host with every embedded language"; } }
        public override string Hypothesis { get { return "The unit tests try each host with some languages and each language mostly in a JavaScript host. A combination nobody wrote a test for (an interpolation in a YAML string in Python, a WGSL string marked with language= in C#) may be missed or read as another language."; } }
        public override string Method { get { return "CombinationGenerator writes a file for each host (JavaScript, C#, Python, C++), embedded language (11), way to mark the string (tag, bare id, language=) and with or without an interpolation, skipping the combinations the plugin does not color (tags outside JavaScript, interpolation in C++, json and regex in C#). The file is run through the scan and the highlighter at 1 copy of the sample and at 200, and compared with what the generator put there: number of strings, number of interpolations, the language, tokens inside the strings. Then the time of the highlight of the 200 copies."; } }
        public override string Criterion { get { return "Every applicable combination passes at both sizes."; } }
        public override string IfMet { get { return "The plugin colors every supported combination of host and language, written in every supported way."; } }
        public override string IfNotMet { get { return "The combinations listed as failing are not covered by the tests; each is a missing case, or a bug."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            int big = settings.Quick ? 20 : 200;
            List<Combination> all = CombinationGenerator.All();
            List<Combination> applicable = all.Where(c => c.Applicable).ToList();

            var failures = new Table("Combinations that failed", "Combination", "Copies", "Problems");
            var times = new Table("Highlight time of " + big + " copies, by host and language (ms, median)", "Language", "JavaScript", "C#", "Python", "C++");
            var timing = new Dictionary<string, Dictionary<HostLanguage, double>>();
            int failed = 0;

            foreach (Combination c in applicable)
            {
                foreach (int copies in new[] { 1, big })
                {
                    CombinationResult r = CombinationCheck.Check(c, copies);
                    if (!r.Passed) { failed++; failures.Add(c.Name, copies, string.Join("; ", r.Problems)); }
                }

                // the time is taken on the plainest form of each pair, so each cell is one comparable file
                if (c.Marker == MarkerStyle.LanguageEquals && c.Interpolated == (c.Host != HostLanguage.Cpp))
                {
                    string text = CombinationGenerator.Generate(c, big);
                    IHighlighter highlighter = NestLightComposition.CreateHighlighter(c.Host);
                    Sample s = Measure.Run(() => highlighter.Highlight(text), settings.Warmup, settings.SlowRuns);
                    if (!timing.ContainsKey(c.Language)) timing[c.Language] = new Dictionary<HostLanguage, double>();
                    timing[c.Language][c.Host] = s.Ms;
                }
            }

            foreach (string language in LanguageSamples.Languages)
            {
                Dictionary<HostLanguage, double> row;
                timing.TryGetValue(language, out row);
                Table t = times;
                t.Add(language, Cell(row, HostLanguage.JavaScript), Cell(row, HostLanguage.CSharp), Cell(row, HostLanguage.Python), Cell(row, HostLanguage.Cpp));
            }

            var skipped = new Table("Combinations left out", "Reason", "Combinations");
            foreach (var group in all.Where(c => !c.Applicable).GroupBy(c => c.NotApplicable))
                skipped.Add(group.Key, group.Count());

            if (failed > 0) outcome.Tables.Add(failures);
            outcome.Tables.Add(times);
            outcome.Tables.Add(skipped);
            outcome.CriterionMet = failed == 0;
            outcome.Headline = applicable.Count + " combinations of " + all.Count + ", " + failed + " failing checks";
            outcome.Analysis.Add(applicable.Count + " applicable combinations were generated and run at 1 and " + big + " copies; " + (all.Count - applicable.Count) + " were left out for the reasons above.");
            outcome.Analysis.Add("The generator is the same one that writes the files of the manual experiments (--generate), so what the person sees in Visual Studio is what this experiment checked.");
            return outcome;
        }

        private static string Cell(Dictionary<HostLanguage, double> row, HostLanguage host)
        {
            double ms;
            return row != null && row.TryGetValue(host, out ms) ? Measure.Ms(ms) : "-";
        }
    }
}
