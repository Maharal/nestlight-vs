using NestLight.Common;
using System.Linq;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class E01_Baseline : Experiment
    {
        public override string Id { get { return "E01"; } }
        public override string Title { get { return "Baseline cost of one Highlight call"; } }
        public override string Hypothesis { get { return "Tokenizing is cheap enough that the number of languages and the scan of the host are not what could make the editor slow."; } }
        public override string Method { get { return "Highlight(text) (host scan + tokenization) for the 4 hosts, with no marked string and with one unit in 10 carrying a marked string, on files of increasing size."; } }
        public override string Criterion { get { return "Every case stays under 16 ms (one frame) at the largest size."; } }
        public override string IfMet { get { return "One Highlight call is not what makes typing slow, even on very large files; look elsewhere (copy, UI thread, GC)."; } }
        public override string IfNotMet { get { return "The analysis itself is too slow on large files: it deserves optimization or running off the UI thread."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var table = new Table("Median time of one Highlight call", "Host", "Marked strings", "Lines", "Characters", "Time", "Tokens");
            double worst = 0;
            string worstCase = "";
            int largest = settings.Lines.Max();

            foreach (HostLanguage host in SyntheticCode.Hosts)
                foreach (int markedEvery in new[] { 0, 10 })
                    foreach (int lines in settings.Lines)
                    {
                        string text = SyntheticCode.ByLines(host, lines, markedEvery);
                        IHighlighter highlighter = NestLightComposition.CreateHighlighter(host);
                        int tokens = 0;
                        Sample s = Measure.Run(() => tokens = highlighter.Highlight(text).Count, settings.Warmup, settings.Runs);
                        table.Add(host, markedEvery == 0 ? "none" : "1 unit in 10", lines, text.Length, Measure.Ms(s.Ms), tokens);
                        if (lines == largest && s.Ms > worst) { worst = s.Ms; worstCase = host + ", " + (markedEvery == 0 ? "no marker" : "with markers"); }
                    }

            outcome.Tables.Add(table);
            outcome.CriterionMet = worst < 16;
            outcome.Headline = "worst case " + Measure.Ms(worst) + " at " + largest + " lines";
            outcome.Analysis.Add("The slowest case at " + largest + " lines is " + worstCase + ": " + Measure.Ms(worst) + " (the frame budget is 16 ms).");
            return outcome;
        }
    }
}
