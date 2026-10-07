using NestLight.Common;
using System;
using System.Text;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class E07_Interpolations : Experiment
    {
        public override string Id { get { return "E07"; } }
        public override string Title { get { return "Many interpolations in one string"; } }
        public override string Hypothesis { get { return "The engine clips every token against the interpolations of its string starting from the first one, so a single large template with many interpolations costs O(tokens x interpolations). Suspected from reading HighlightEngine.AddClipped, not measured before."; } }
        public override string Method { get { return "One marked HTML string with 100, 1,000 and 10,000 interpolations, in JavaScript, C# and Python. Time against the number of interpolations."; } }
        public override string Criterion { get { return "Ten times more interpolations cost less than 20 times the time (linear is 10x, quadratic is 100x), for every host."; } }
        public override string IfMet { get { return "Large templates scale roughly linearly."; } }
        public override string IfNotMet { get { return "A string with many interpolations is quadratic: the clipping needs to resume from where the previous token ended."; } }

        private static string Build(HostLanguage host, int count)
        {
            var sb = new StringBuilder();
            switch (host)
            {
                case HostLanguage.JavaScript:
                    sb.Append("const s = html`");
                    for (int i = 0; i < count; i++) sb.Append("<li class=\"a\">${x}</li>");
                    sb.Append("`;");
                    break;
                case HostLanguage.CSharp:
                    sb.Append("// language=html\nvar s = $\"\"\"");
                    for (int i = 0; i < count; i++) sb.Append("<li class='a'>{x}</li>");
                    sb.Append("\"\"\";");
                    break;
                default:
                    sb.Append("# language=html\ns = f\"\"\"");
                    for (int i = 0; i < count; i++) sb.Append("<li class='a'>{x}</li>");
                    sb.Append("\"\"\"");
                    break;
            }
            return sb.ToString();
        }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var table = new Table("Time by number of interpolations", "Host", "Interpolations", "Time", "Per interpolation", "Tokens");
            int[] counts = settings.Quick ? new[] { 100, 1000 } : new[] { 100, 1000, 10000 };
            bool met = true;
            double worstGrowth = 0;

            foreach (HostLanguage host in new[] { HostLanguage.JavaScript, HostLanguage.CSharp, HostLanguage.Python })
            {
                double previous = 0;
                foreach (int count in counts)
                {
                    string text = Build(host, count);
                    IHighlighter highlighter = NestLightComposition.CreateHighlighter(host);
                    int tokens = 0;
                    Sample s = Measure.Run(() => tokens = highlighter.Highlight(text).Count, 1, settings.SlowRuns);
                    table.Add(host, count, Measure.Ms(s.Ms), Measure.Ms(s.Ms / count), tokens);
                    if (previous > 0)
                    {
                        double growth = s.Ms / previous;
                        worstGrowth = Math.Max(worstGrowth, growth);
                        met &= growth < 20;
                        outcome.Analysis.Add(host + ": " + counts[Array.IndexOf(counts, count) - 1] + " to " + count + " interpolations multiplied the time by " + Measure.Ratio(growth) + ".");
                    }
                    previous = s.Ms;
                }
            }

            outcome.Tables.Add(table);
            outcome.CriterionMet = met;
            outcome.Headline = "worst growth " + Measure.Ratio(worstGrowth) + " for 10x interpolations";
            return outcome;
        }
    }
}
