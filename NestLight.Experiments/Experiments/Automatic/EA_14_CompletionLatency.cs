using System;
using System.Linq;
using System.Text;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA_14_CompletionLatency : Experiment
    {
        public override string Id { get { return "EA_14"; } }
        public override string Title { get { return "Completion latency against the size of the file"; } }
        public override string Hypothesis { get { return "Every keystroke in an embedded string scans the host to find the string (Locate) and the whole text for words (Suggest). Those two scans, plus a dictionary of the matching words, are cheap on a normal file but could be noticeable on a large one, mostly when thousands of distinct words share the prefix."; } }
        public override string Method { get { return "For each host, a file of increasing size with the caret at the end of an open `comp` in a marked SQL string. Two shapes: typical code (EA_01's file, a few distinct words) and a file whose every line declares a new `compNNNNN` identifier (thousands of distinct matches). Time of Locate alone and of Locate + Suggest, without the text copy of the editor (EA_03)."; } }
        public override string Criterion { get { return "Locate + Suggest stay under 16 ms (one frame) at the largest size, in every host and shape."; } }
        public override string IfMet { get { return "Completion is not a latency risk; the cache of the scan between the classifier and the completion is not needed."; } }
        public override string IfNotMet { get { return "Reuse the scan of the classifier, lower the window of the word scan, or index the words incrementally."; } }

        private static string Tail(HostLanguage host)
        {
            switch (host)
            {
                case HostLanguage.JavaScript: return "sql`comp|`;\n";
                case HostLanguage.CSharp: return "// language=sql\nvar q = \"comp|\";\n";
                case HostLanguage.Python: return "# language=sql\nq = \"comp|\"\n";
                default: return "// language=sql\nauto q = R\"(comp|)\";\n";
            }
        }

        private static string DistinctWords(HostLanguage host, int lines)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < lines; i++)
                sb.Append(host == HostLanguage.Python ? "comp" : "int comp").Append(i.ToString("D6")).Append(host == HostLanguage.Python ? " = 1\n" : " = 1;\n");
            return sb.ToString();
        }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var table = new Table("Median time of one completion", "Host", "Shape", "Lines", "Characters", "Locate", "Locate + Suggest", "Suggestions");
            int largest = settings.Lines.Max();
            double worst = 0;
            string worstCase = "";

            foreach (HostLanguage host in SyntheticCode.Hosts)
                foreach (bool distinct in new[] { false, true })
                    foreach (int lines in settings.Lines)
                    {
                        string body = distinct ? DistinctWords(host, lines) : SyntheticCode.ByLines(host, lines, 10);
                        string tail = Tail(host);
                        int caret = body.Length + tail.IndexOf('|');
                        string text = body + tail.Remove(tail.IndexOf('|'), 1);

                        CompletionEngine engine = CompletionLab.Engine(host, CompletionEngine.DefaultMaxItems);
                        CompletionSite site = null;
                        int count = 0;
                        Sample locate = Measure.Run(() => site = engine.Locate(text, caret), settings.Warmup, settings.Runs);
                        Sample both = Measure.Run(() => { site = engine.Locate(text, caret); count = engine.Suggest(text, site).Count; }, settings.Warmup, settings.Runs);
                        table.Add(host, distinct ? "distinct words" : "typical code", lines, text.Length, Measure.Ms(locate.Ms), Measure.Ms(both.Ms), count);
                        if (lines == largest && both.Ms > worst) { worst = both.Ms; worstCase = host + ", " + (distinct ? "distinct words" : "typical code"); }
                    }

            outcome.Tables.Add(table);
            outcome.CriterionMet = worst < 16;
            outcome.Headline = "worst case " + Measure.Ms(worst) + " at " + largest + " lines";
            outcome.Analysis.Add("The slowest case at " + largest + " lines is " + worstCase + ": " + Measure.Ms(worst) + " (the frame budget is 16 ms).");
            outcome.Analysis.Add("The word scan only looks at 500,000 characters on each side of the caret, so beyond about 1 million characters the cost no longer depends on the size of the file; Locate always scans all of it.");
            return outcome;
        }
    }
}
