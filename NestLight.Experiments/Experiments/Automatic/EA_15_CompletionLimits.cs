using System;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA_15_CompletionLimits : Experiment
    {
        public override string Id { get { return "EA_15"; } }
        public override string Title { get { return "Do the limits of the completion change its cost?"; } }
        public override string Hypothesis { get { return "The limit on the number of suggestions (100) and the minimum word length (3) were picked without measuring. If the cost is in the scan, no value of them is a performance lever and they can be chosen for quality alone; if it is in the dictionary and the sort, they are."; } }
        public override string Method { get { return "The two shapes of EA_14 at the second size (JavaScript), with the maximum number of suggestions at 10, 100, 1,000 and 10,000 and the minimum word length at 1, 3 and 5, one knob at a time from the default (100, 3). Median of Locate + Suggest, against the faster of two measurements of the default (first and last)."; } }
        public override string Criterion { get { return "Every combination is within 25% of the default, in both shapes."; } }
        public override string IfMet { get { return "The limits are free to tune for the quality of the list."; } }
        public override string IfNotMet { get { return "The limits matter: the defaults need a justification, or the dictionary needs to be cheaper."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var table = new Table("Median time of Locate + Suggest (JavaScript)", "Shape", "Max suggestions", "Min word length", "Time", "Against the default", "Suggestions");
            int lines = settings.Lines[settings.Lines.Length > 1 ? 1 : 0];
            bool met = true;
            double worst = 1;
            string worstCase = "";

            foreach (bool distinct in new[] { false, true })
            {
                string body = distinct ? Distinct(lines) : SyntheticCode.ByLines(HostLanguage.JavaScript, lines, 10);
                string text = body + "sql`comp`;\n";
                int caret = text.Length - 3;

                // The default is measured again at the end: the first measurement of a run is often slower (cold caches), and a
                // baseline taken once, first, would make every other row look faster than it is.
                var combos = new[] { new[] { 100, 3 }, new[] { 10, 3 }, new[] { 1000, 3 }, new[] { 10000, 3 }, new[] { 100, 1 }, new[] { 100, 5 }, new[] { 100, 3 } };
                var times = new double[combos.Length];
                var counts = new int[combos.Length];
                for (int c = 0; c < combos.Length; c++)
                {
                    CompletionEngine engine = CompletionLab.Engine(HostLanguage.JavaScript, combos[c][0], combos[c][1]);
                    int count = 0;
                    Measure.Collect();
                    times[c] = Measure.Run(() => count = engine.Suggest(text, engine.Locate(text, caret)).Count, settings.Warmup * 2, settings.Runs * 4).Ms;
                    counts[c] = count;
                }
                double baseline = Math.Min(times[0], times[combos.Length - 1]);
                for (int c = 0; c < combos.Length; c++)
                {
                    double ratio = times[c] / baseline;
                    table.Add(distinct ? "distinct words" : "typical code", combos[c][0], combos[c][1] + (c == combos.Length - 1 ? " (default again)" : ""), Measure.Ms(times[c]), Measure.Ratio(ratio), counts[c]);
                    if (Math.Abs(ratio - 1) > 0.25) met = false;
                    if (Math.Abs(ratio - 1) > Math.Abs(worst - 1)) { worst = ratio; worstCase = (distinct ? "distinct words" : "typical code") + ", max " + combos[c][0] + ", min length " + combos[c][1]; }
                }
            }

            outcome.Tables.Add(table);
            outcome.CriterionMet = met;
            outcome.Headline = worstCase == "" ? "no difference" : "largest difference " + Measure.Ratio(worst) + " (" + worstCase + ")";
            outcome.Analysis.Add("The largest difference from the default is " + Measure.Ratio(worst) + (worstCase == "" ? "." : " (" + worstCase + ")."));
            return outcome;
        }

        private static string Distinct(int lines)
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < lines; i++) sb.Append("int comp").Append(i.ToString("D6")).Append(" = 1;\n");
            return sb.ToString();
        }
    }
}
