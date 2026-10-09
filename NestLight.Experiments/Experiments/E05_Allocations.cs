using System;
using NestLight.Common;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class E05_Allocations : Experiment
    {
        public override string Id { get { return "E05"; } }
        public override string Title { get { return "Where Highlight allocates"; } }
        public override string Hypothesis { get { return "Highlight allocates about twice the size of the text per call (seen while running E03), and one of its two stages, the host scan or the rest (decoding, tokenizing, mapping back), accounts for most of it."; } }
        public override string Method { get { return "Bytes allocated by the host scan alone and by the whole Highlight call, on the same file; the rest is the difference. With and without marked strings, for the 4 hosts, at the second size."; } }
        public override string Criterion { get { return "On every host, with marked strings, one stage accounts for at least 70% of the bytes."; } }
        public override string IfMet { get { return "There is one place to look for allocations; a profile of that stage can find the call sites."; } }
        public override string IfNotMet { get { return "The allocations are spread over both stages: reducing them would take several separate changes."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var table = new Table("Allocation per call", "Host", "Marked strings", "Text", "Scan", "Whole Highlight", "Highlight / text", "Scan share", "Rest share");
            int lines = settings.Lines.Length > 1 ? settings.Lines[1] : settings.Lines[0];
            bool met = true;
            double worstDominance = 1, totalRatio = 0;
            int cases = 0;

            foreach (HostLanguage host in SyntheticCode.Hosts)
                foreach (int markedEvery in new[] { 0, 10 })
                {
                    string text = SyntheticCode.ByLines(host, lines, markedEvery);
                    IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
                    IHostScanner scanner = NestLightComposition.CreateScanner(host, languages);
                    var engine = new HighlightEngine(scanner, languages);

                    Sample scan = Measure.Run(() => scanner.Scan(text), settings.Warmup, settings.Runs);
                    Sample whole = Measure.Run(() => engine.Highlight(text), settings.Warmup, settings.Runs);
                    double textKb = text.Length * 2 / 1024.0;
                    double scanShare = Math.Min(1, scan.AllocKb / whole.AllocKb), restShare = 1 - scanShare;
                    table.Add(host, markedEvery == 0 ? "none" : "1 unit in 10", Measure.Kb(textKb), Measure.Kb(scan.AllocKb), Measure.Kb(whole.AllocKb),
                        Measure.Ratio(whole.AllocKb / textKb), (100 * scanShare).ToString("F0") + "%", (100 * restShare).ToString("F0") + "%");

                    if (markedEvery > 0)
                    {
                        double dominant = Math.Max(scanShare, restShare);
                        met &= dominant >= 0.7;
                        worstDominance = Math.Min(worstDominance, dominant);
                        totalRatio += whole.AllocKb / textKb;
                        cases++;
                    }
                }

            outcome.Tables.Add(table);
            outcome.CriterionMet = met;
            outcome.Headline = "Highlight allocates " + Measure.Ratio(totalRatio / cases) + " the text on average; weakest dominant stage " + (100 * worstDominance).ToString("F0") + "%";
            outcome.Analysis.Add("With marked strings, Highlight allocates " + Measure.Ratio(totalRatio / cases) + " the size of the text on average (UTF-16, 2 bytes per character).");
            outcome.Analysis.Add("The stage that dominates the least accounts for " + (100 * worstDominance).ToString("F0") + "% of the bytes of its host.");
            outcome.Analysis.Add("This splits the allocation by stage only; finding the call sites needs an allocation profile of the stage that dominates.");
            return outcome;
        }
    }
}
