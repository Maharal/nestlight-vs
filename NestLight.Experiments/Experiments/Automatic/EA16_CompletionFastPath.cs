using System.Linq;
using System.Text;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA16_CompletionFastPath : Experiment
    {
        public override string Id { get { return "EA16"; } }
        public override string Title { get { return "Sharing the scan and not creating the words of the completion"; } }
        public override string Hypothesis { get { return "EA10 found that completion takes more than a frame above ~1 million characters, and that most of it is the scan of the host. In Visual Studio the session of a completion scans the text twice (once to decide whether to open, once to fill the list) after the classifier already did it for the same snapshot, and the word pass created a string and a sort entry for every match. Sharing one scan between the classifier and the completion, and creating a word only when it is offered, brings the worst case under a frame."; } }
        public override string Method { get { return "The files of EA10 (4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines). The cost of the start of one completion session, as the editor does it: Locate, Locate again and Suggest, over a new text instance each time. Two cases: nothing shared (a plain scanner, as in EA10) and the scan shared (the classifier has already highlighted that text, through a CachingHostScanner). Also Suggest alone."; } }
        public override string Criterion { get { return "The session with the scan shared stays under 16 ms at 60,000 lines, in every host and shape."; } }
        public override string IfMet { get { return "Completion fits in a frame on any file the classifier can handle."; } }
        public override string IfNotMet { get { return "The scan of the host itself (EA05) is the limit; it would have to be made incremental."; } }

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
            var table = new Table("Median time of the start of one completion session", "Host", "Shape", "Lines", "Characters", "Nothing shared", "Scan shared", "Suggest alone", "Suggestions");
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
                        string baseText = body + tail.Remove(tail.IndexOf('|'), 1);
                        int copies = settings.Warmup + settings.Runs;

                        // a new text instance for every run, made before the clock starts: the editor reads a new one for every snapshot
                        var texts = new string[copies];
                        for (int i = 0; i < copies; i++) texts[i] = new string(baseText.ToCharArray());

                        // nothing shared: a plain scanner, as in EA10
                        CompletionEngine plain = CompletionLab.Engine(host, CompletionEngine.DefaultMaxItems);
                        int n = 0;
                        Sample cold = Measure.Run(() =>
                        {
                            string text = texts[n++ % copies];
                            plain.Locate(text, caret);
                            plain.Suggest(text, plain.Locate(text, caret));
                        }, settings.Warmup, settings.Runs);

                        // the scan shared: the classifier highlights the text first (not timed), then the session starts
                        IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
                        var scanner = new CachingHostScanner(NestLightComposition.CreateScanner(host, languages));
                        var highlighter = new HighlightEngine(scanner, languages);
                        var shared = new CompletionEngine(scanner);
                        int count = 0;
                        var times = new double[copies];
                        // The classifier runs first and is not timed, so the clock is read by hand, text by text.
                        var sw = new System.Diagnostics.Stopwatch();
                        for (int i = 0; i < copies; i++)
                        {
                            string text = texts[i];
                            highlighter.Highlight(text);
                            sw.Restart();
                            shared.Locate(text, caret);
                            CompletionSite site = shared.Locate(text, caret);
                            count = shared.Suggest(text, site).Count;
                            sw.Stop();
                            times[i] = sw.Elapsed.TotalMilliseconds;
                        }
                        var measured = times.Skip(settings.Warmup).OrderBy(t => t).ToArray();
                        double sharedMs = measured[measured.Length / 2];

                        CompletionSite fixedSite = plain.Locate(baseText, caret);
                        Sample suggest = Measure.Run(() => plain.Suggest(baseText, fixedSite), settings.Warmup, settings.Runs);

                        table.Add(host, distinct ? "distinct words" : "typical code", lines, baseText.Length, Measure.Ms(cold.Ms), Measure.Ms(sharedMs), Measure.Ms(suggest.Ms), count);
                        if (lines == largest && sharedMs > worst) { worst = sharedMs; worstCase = host + ", " + (distinct ? "distinct words" : "typical code"); }
                    }

            outcome.Tables.Add(table);
            outcome.CriterionMet = worst < 16;
            outcome.Headline = "worst case with the scan shared " + Measure.Ms(worst) + " at " + largest + " lines";
            outcome.Analysis.Add("The slowest session with the scan shared at " + largest + " lines is " + worstCase + ": " + Measure.Ms(worst) + " (the frame budget is 16 ms).");
            outcome.Analysis.Add("\"Nothing shared\" is the start of a session as the editor makes it without the shared scan: Locate twice and Suggest. EA10 measured one Locate and Suggest.");
            return outcome;
        }
    }
}
