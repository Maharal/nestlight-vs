using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA33_IncrementalLocate : Experiment
    {
        public override string Id { get { return "EA33"; } }
        public override string Title { get { return "Scanning only the lines around the edit"; } }
        public override string Hypothesis { get { return "After EA32's alternative was dropped, what is left of a request is `Locate`, which reads the whole text for the embedded strings after every keystroke. The strings before the edit are the same and the ones after it are the same moved, so only the lines around the edit need to be scanned again, from a place where the scanner depends on nothing before it, until a place after the edit where the old scan was in the same state."; } }
        public override string Method { get { return "EA10's files (typical code with a marked string every ten units, and one new word per line; 1,200, 12,000 and 60,000 lines), in the four hosts. A person typing `comp000` letter by letter: seven texts, one character apart, each a new string instance, with the marked SQL string at the start, in the middle and at the end of the file (an edit at the start moves every string after it). `Locate` for each text, through the cache of the scan that the plugin uses: once as it was (the whole text is scanned after each edit) and once resuming from the scan of the text before. Per request: the time, the memory allocated and the collections of generation 2 in the whole run. The strings found and the site are compared request by request."; } }
        public override string Criterion { get { return "`Locate` at least three times faster than scanning the whole text at 60,000 lines, in every host, shape and place of the edit; with the same strings and the same site at every request of every case; and without pressure on the collector: no more collections of generation 2, and at most 85 KB more allocated per request (from 85 KB an array goes to the large object heap, which is what a list of the size of the file would have done)."; } }
        public override string IfMet { get { return "The incremental scan stays: the plugin is already built with it, since the cache of the scan uses it whenever the scanner can resume."; } }
        public override string IfNotMet { get { return "Scan the whole text after every edit, as before: put a scanner that cannot resume (`IHostScanner` only) in the cache of the scan."; } }

        /// <summary>From this size an array goes to the large object heap.</summary>
        private const double LargeObjectKb = 85;

        private static readonly string[] Typed = { "c", "co", "com", "comp", "comp0", "comp00", "comp000" };
        private static readonly string[] Places = { "start", "middle", "end" };

        private static string Wrap(HostLanguage host, string word)
        {
            switch (host)
            {
                case HostLanguage.JavaScript: return "sql`" + word + "`;\n";
                case HostLanguage.CSharp: return "// language=sql\nvar q = \"" + word + "\";\n";
                case HostLanguage.Python: return "# language=sql\nq = \"" + word + "\"\n";
                default: return "// language=sql\nauto q = R\"(" + word + ")\";\n";
            }
        }

        private static string DistinctWords(HostLanguage host, int lines)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < lines; i++)
                sb.Append(host == HostLanguage.Python ? "comp" : "int comp").Append(i.ToString("D6")).Append(host == HostLanguage.Python ? " = 1\n" : " = 1;\n");
            return sb.ToString();
        }

        private sealed class Session
        {
            public string[] Texts;
            public int[] Carets;
        }

        private static Session Typing(HostLanguage host, string body, int place)
        {
            int cut = place == 0 ? 0 : place == 1 ? body.IndexOf('\n', body.Length / 2) + 1 : body.Length;
            string before = body.Substring(0, cut), after = body.Substring(cut);
            var session = new Session { Texts = new string[Typed.Length], Carets = new int[Typed.Length] };
            for (int i = 0; i < Typed.Length; i++)
            {
                string wrapped = Wrap(host, Typed[i]);
                session.Texts[i] = before + wrapped + after;
                session.Carets[i] = before.Length + wrapped.IndexOf(Typed[i], StringComparison.Ordinal) + Typed[i].Length;
            }
            return session;
        }

        /// <summary>A scanner that can only scan whole texts: the cache has nothing to resume with.</summary>
        private sealed class WholeTextOnly : IHostScanner
        {
            private readonly IHostScanner _inner;
            public WholeTextOnly(IHostScanner inner) { _inner = inner; }
            public IReadOnlyList<EmbeddedString> Scan(string text) { return _inner.Scan(text); }
        }

        private static CompletionEngine Engine(HostLanguage host, bool resume)
        {
            IHostScanner scanner = NestLightComposition.CreateScanner(host, NestLightComposition.CreateEmbeddedLanguages());
            return new CompletionEngine(new CachingHostScanner(resume ? scanner : new WholeTextOnly(scanner)));
        }

        private static string Describe(IReadOnlyList<EmbeddedString> strings)
        {
            return string.Join(";", strings.Select(s => s.EmbeddedLanguageId + "," + s.OuterStart + "," + s.Start + "," + s.End + "," + s.OuterEnd + "," + s.Interpolations.Count + "," + s.Escapes.Count));
        }

        private static string Describe(CompletionSite site)
        {
            return site == null ? "none" : site.EmbeddedLanguageId + "," + site.Start + "," + site.Caret + "," + site.End + "," + site.OwnerStart;
        }

        private struct Measurement
        {
            public double Ms, AllocKb;
            public int Gen2;
        }

        /// <summary>The session over and over: the time and the memory of a request, and the collections of generation 2 during all of them.</summary>
        private static Measurement Measured(CompletionEngine engine, Session session, int warmup, int runs, out double[] medians)
        {
            for (int r = 0; r < warmup; r++)
                for (int i = 0; i < session.Texts.Length; i++) engine.Locate(session.Texts[i], session.Carets[i]);
            Measure.Collect();
            int gen2 = GC.CollectionCount(2);
            var times = new List<double>();
            long bytes = 0;
            for (int r = 0; r < runs; r++)
                for (int i = 0; i < session.Texts.Length; i++)
                {
                    long before = Measure.AllocatedBytes();
                    var sw = Stopwatch.StartNew();
                    engine.Locate(session.Texts[i], session.Carets[i]);
                    times.Add(sw.Elapsed.TotalMilliseconds);
                    bytes += Measure.AllocatedBytes() - before;
                }
            times.Sort();
            medians = null;
            return new Measurement { Ms = times[times.Count / 2], AllocKb = bytes / 1024.0 / times.Count, Gen2 = GC.CollectionCount(2) - gen2 };
        }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var table = new Table("`Locate` while typing (median of a request)", "Host", "Shape", "Lines", "Edit at", "Characters", "Whole text", "Resuming", "Faster", "Allocated, whole", "Allocated, resuming", "Gen 2, whole", "Gen 2, resuming");
            int largest = settings.Lines.Max();
            bool allSame = true, memoryOk = true;
            double slowestGain = double.MaxValue;
            string slowestCase = "";
            string firstDifference = null, firstMemory = null;
            var gains = new List<double>();
            double extraAlloc = 0;

            foreach (HostLanguage host in SyntheticCode.Hosts)
                foreach (bool distinct in new[] { false, true })
                    foreach (int lines in settings.Lines)
                        for (int place = 0; place < Places.Length; place++)
                        {
                            string body = distinct ? DistinctWords(host, lines) : SyntheticCode.ByLines(host, lines, 10);
                            Session session = Typing(host, body, place);
                            string shape = distinct ? "distinct words" : "typical code";

                            // the same strings and the same site at every request
                            CompletionEngine whole = Engine(host, false), resuming = Engine(host, true);
                            for (int i = 0; i < Typed.Length; i++)
                            {
                                CompletionSite a = whole.Locate(session.Texts[i], session.Carets[i]), b = resuming.Locate(session.Texts[i], session.Carets[i]);
                                string sa = Describe(NestLightComposition.CreateScanner(host, NestLightComposition.CreateEmbeddedLanguages()).Scan(session.Texts[i]));
                                string sb = Describe(new CachingHostScanner(NestLightComposition.CreateScanner(host, NestLightComposition.CreateEmbeddedLanguages())).Scan(session.Texts[i]));
                                if (Describe(a) != Describe(b) || sa != sb) { allSame = false; if (firstDifference == null) firstDifference = host + ", " + shape + ", " + lines + " lines, edit at the " + Places[place] + ", after typing '" + Typed[i] + "'"; }
                            }
                            // the strings the resuming cache gives after a whole sequence of edits (not only after the first scan)
                            var chain = new CachingHostScanner(NestLightComposition.CreateScanner(host, NestLightComposition.CreateEmbeddedLanguages()));
                            var plain = NestLightComposition.CreateScanner(host, NestLightComposition.CreateEmbeddedLanguages());
                            for (int r = 0; r < 2; r++)
                                for (int i = 0; i < Typed.Length; i++)
                                    if (Describe(chain.Scan(session.Texts[i])) != Describe(plain.Scan(session.Texts[i])))
                                    {
                                        allSame = false;
                                        if (firstDifference == null) firstDifference = host + ", " + shape + ", " + lines + " lines, edit at the " + Places[place] + ", scan " + i + " of a chain";
                                    }

                            double[] ignored;
                            CompletionEngine w = Engine(host, false), x = Engine(host, true);
                            Measurement runWhole = Measured(w, session, settings.Warmup, settings.Runs, out ignored);
                            Measurement runResuming = Measured(x, session, settings.Warmup, settings.Runs, out ignored);
                            double gain = runWhole.Ms / runResuming.Ms;
                            table.Add(host, shape, lines, Places[place], session.Texts[0].Length, Measure.Ms(runWhole.Ms), Measure.Ms(runResuming.Ms), Measure.Ratio(gain),
                                Measure.Kb(runWhole.AllocKb), Measure.Kb(runResuming.AllocKb), runWhole.Gen2, runResuming.Gen2);
                            if (lines == largest)
                            {
                                if (gain < slowestGain) { slowestGain = gain; slowestCase = host + ", " + shape + ", edit at the " + Places[place]; }
                                gains.Add(gain);
                                extraAlloc = Math.Max(extraAlloc, runResuming.AllocKb - runWhole.AllocKb);
                                if ((runResuming.AllocKb > runWhole.AllocKb + LargeObjectKb || runResuming.Gen2 > runWhole.Gen2) && firstMemory == null)
                                { memoryOk = false; firstMemory = host + ", " + shape + ", edit at the " + Places[place]; }
                            }
                        }

            outcome.Tables.Add(table);
            outcome.CriterionMet = allSame && memoryOk && slowestGain >= 3;
            outcome.Headline = (allSame ? "same strings" : "DIFFERENT strings") + "; least gain " + Measure.Ratio(slowestGain) + " at " + largest + " lines";
            outcome.Analysis.Add(allSame
                ? "The two ways found the same strings and the same site at every request of every case, and the cache that resumes found the same strings as a plain scan after a whole sequence of edits."
                : "The strings or the site differ, first in: " + firstDifference + ". The incremental scan is wrong until this is fixed.");
            outcome.Analysis.Add("The smallest gain at " + largest + " lines is " + Measure.Ratio(slowestGain) + " (" + slowestCase + "); the criterion is 3x. The median of the gains at that size is " + Measure.Ratio(Median(gains)) + ".");
            outcome.Analysis.Add(memoryOk
                ? "Resuming allocated at most " + LargeObjectKb + " KB more per request than scanning the whole text (" + Measure.Kb(extraAlloc) + " at most), and caused no more collections of generation 2."
                : "Resuming allocated more than " + LargeObjectKb + " KB more per request, or caused more collections of generation 2, than scanning the whole text, first in: " + firstMemory + ".");
            outcome.Analysis.Add("An edit at the start moves every string after it (each is copied with its new offsets), so it is the case with the most work left; an edit at the end moves none.");
            return outcome;
        }

        private static double Median(List<double> values)
        {
            values.Sort();
            return values.Count == 0 ? 0 : values[values.Count / 2];
        }
    }
}
