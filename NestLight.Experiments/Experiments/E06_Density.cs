using System;
using NestLight.Common;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class E06_Density : Experiment
    {
        public override string Id { get { return "E06"; } }
        public override string Title { get { return "Number of marked strings"; } }
        public override string Hypothesis { get { return "The cost grows only linearly with the number of embedded strings: nothing in the engine (a sort, a nested scan) grows faster than the number of strings it has to handle."; } }
        public override string Method { get { return "A file where every unit carries a marked string, at 1x, 2x, 4x and 8x a base size, for each host. The number of embedded strings grows with the file, and so should the time."; } }
        public override string Criterion { get { return "8 times more embedded strings cost less than 10 times the time (linear is 8x, quadratic is 64x), for every host."; } }
        public override string IfMet { get { return "The cost is linear in the number of embedded strings, so files full of embedded code are not a special case."; } }
        public override string IfNotMet { get { return "The cost grows faster than the number of embedded strings: look for a quadratic step in the engine."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var table = new Table("Time by number of embedded strings", "Host", "Characters", "Embedded strings", "Time", "Per embedded string", "Gen2 collections");
            int baseChars = settings.Chars[0];
            bool met = true;
            double worstGrowth = 0;

            foreach (HostLanguage host in SyntheticCode.Hosts)
            {
                double first = 0, last = 0;
                for (int factor = 1; factor <= 8; factor *= 2)
                {
                    string text = SyntheticCode.ByChars(host, baseChars * factor, 1);
                    IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
                    IHostScanner scanner = NestLightComposition.CreateScanner(host, languages);
                    var engine = new HighlightEngine(scanner, languages);
                    int count = scanner.Scan(text).Count;
                    Sample s = Measure.Run(() => engine.Highlight(text), settings.Warmup, settings.Runs);
                    table.Add(host, text.Length, count, Measure.Ms(s.Ms), Measure.Ms(s.Ms / count), s.Gen2 + " in " + settings.Runs + " runs");
                    if (factor == 1) first = s.Ms;
                    last = s.Ms;
                }
                double growth = last / first;
                worstGrowth = Math.Max(worstGrowth, growth);
                met &= growth < 10;
                outcome.Analysis.Add(host + ": 8x the embedded strings multiplied the time by " + Measure.Ratio(growth) + ".");
            }

            outcome.Tables.Add(table);
            outcome.CriterionMet = met;
            outcome.Headline = "worst growth " + Measure.Ratio(worstGrowth) + " for 8x the strings";
            return outcome;
        }
    }
}
