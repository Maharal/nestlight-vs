using System;
using System.Linq;
using System.Text;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA17_SimilarLatency : Experiment
    {
        public override string Id { get { return "EA17"; } }
        public override string Title { get { return "Does the second stage of the completion fit in a frame?"; } }
        public override string Hypothesis { get { return "The second stage compares what was typed with every distinct word of the document that passes two cheap filters, and keeps the best few. Even forced to run in every session, with thousands of words that are one edit away, one session still fits in the 16 ms of a frame, as EA16 showed for the first stage."; } }
        public override string Method { get { return "EA16's files and session (Locate, Locate again and Suggest, over a new text instance, the classifier having highlighted it first), 4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines, with the second stage forced to run (it is asked for below any number of items). Two things typed: the exact prefix `comp` (the second stage compares everything and finds nothing new) and `cmop`, two letters swapped (in the distinct-words file, every one of the words is one edit away). Also the same session with the second stage off."; } }
        public override string Criterion { get { return "The session with the second stage forced stays under 16 ms at 60,000 lines, in every host, shape and typed text."; } }
        public override string IfMet { get { return "The second stage needs no cache of the distinct words and no index."; } }
        public override string IfNotMet { get { return "Cache the distinct words of a text (as CachingHostScanner does with the scan), shrink the window, and only then think of an index."; } }

        private static string Tail(HostLanguage host, string typed)
        {
            switch (host)
            {
                case HostLanguage.JavaScript: return "sql`" + typed + "|`;\n";
                case HostLanguage.CSharp: return "// language=sql\nvar q = \"" + typed + "|\";\n";
                case HostLanguage.Python: return "# language=sql\nq = \"" + typed + "|\"\n";
                default: return "// language=sql\nauto q = R\"(" + typed + "|)\";\n";
            }
        }

        private static string DistinctWords(HostLanguage host, int lines)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < lines; i++)
                sb.Append(host == HostLanguage.Python ? "comp" : "int comp").Append(i.ToString("D6")).Append(host == HostLanguage.Python ? " = 1\n" : " = 1;\n");
            return sb.ToString();
        }

        private static double Session(CompletionEngine engine, IHighlighter highlighter, string[] texts, int caret, int warmup, out int similar, out double allocatedKb)
        {
            var times = new double[texts.Length];
            var sw = new System.Diagnostics.Stopwatch();
            similar = 0;
            long bytes = 0;
            for (int i = 0; i < texts.Length; i++)
            {
                highlighter.Highlight(texts[i]); // the classifier, not timed
                long before = Measure.AllocatedBytes();
                sw.Restart();
                engine.Locate(texts[i], caret);
                CompletionSite site = engine.Locate(texts[i], caret);
                var items = engine.Suggest(texts[i], site);
                sw.Stop();
                if (i >= warmup) bytes += Measure.AllocatedBytes() - before;
                times[i] = sw.Elapsed.TotalMilliseconds;
                similar = items.Count(s => s.Distance > 0);
            }
            allocatedKb = bytes / 1024.0 / Math.Max(1, texts.Length - warmup);
            var measured = times.Skip(warmup).OrderBy(t => t).ToArray();
            return measured[measured.Length / 2];
        }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var table = new Table("Median time of the start of one completion session (the scan shared)", "Host", "Shape", "Typed", "Lines", "Characters", "Second stage off", "Second stage forced", "Allocated off", "Allocated forced", "Similar items");
            int largest = settings.Lines.Max();
            double worst = 0;
            string worstCase = "";

            foreach (HostLanguage host in SyntheticCode.Hosts)
                foreach (bool distinct in new[] { false, true })
                    foreach (string typed in new[] { "comp", "cmop" })
                        foreach (int lines in settings.Lines)
                        {
                            string body = distinct ? DistinctWords(host, lines) : SyntheticCode.ByLines(host, lines, 10);
                            string tail = Tail(host, typed);
                            int caret = body.Length + tail.IndexOf('|');
                            string baseText = body + tail.Remove(tail.IndexOf('|'), 1);
                            int copies = settings.Warmup + settings.Runs;
                            var texts = new string[copies];
                            for (int i = 0; i < copies; i++) texts[i] = new string(baseText.ToCharArray());

                            double off = 0, forced = 0, offKb = 0, forcedKb = 0;
                            int similar = 0;
                            foreach (bool on in new[] { false, true })
                            {
                                IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
                                var scanner = new CachingHostScanner(NestLightComposition.CreateScanner(host, languages));
                                var highlighter = new HighlightEngine(scanner, languages);
                                var engine = new CompletionEngine(scanner, CompletionEngine.DefaultMaxItems, CompletionEngine.DefaultMinWordLength,
                                    on ? new BandedPrefixMatcher() : null, int.MaxValue);
                                int found;
                                double kb;
                                double ms = Session(engine, highlighter, texts, caret, settings.Warmup, out found, out kb);
                                if (on) { forced = ms; forcedKb = kb; similar = found; } else { off = ms; offKb = kb; }
                            }

                            table.Add(host, distinct ? "distinct words" : "typical code", typed, lines, baseText.Length, Measure.Ms(off), Measure.Ms(forced), Measure.Kb(offKb), Measure.Kb(forcedKb), similar);
                            if (lines == largest && forced > worst) { worst = forced; worstCase = host + ", " + (distinct ? "distinct words" : "typical code") + ", typed " + typed; }
                        }

            outcome.Tables.Add(table);
            outcome.CriterionMet = worst < 16;
            outcome.Headline = "worst case with the second stage forced " + Measure.Ms(worst) + " at " + largest + " lines";
            outcome.Analysis.Add("The slowest session with the second stage forced at " + largest + " lines is " + worstCase + ": " + Measure.Ms(worst) + " (the frame budget is 16 ms).");
            outcome.Analysis.Add("Allocated is what the session allocates (the same meaning as EA05): the difference between forced and off is the cost of the second stage; no string is created for a word that is too far, only a small record to skip its next occurrences.");
            outcome.Analysis.Add("In the typical file there are few distinct words and nearly all of them fail the first-letter filter; in the file of distinct words every word is a candidate and, for `cmop`, every one is one edit away.");
            return outcome;
        }
    }
}
