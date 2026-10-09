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
    internal sealed class EA32_WordIndex : Experiment
    {
        public override string Id { get { return "EA32"; } }
        public override string Title { get { return "Keeping the words of the document in memory"; } }
        public override string Hypothesis { get { return "Every request for suggestions reads the whole text for the words that start with what was typed. The words could be kept in memory (where each one starts and how long it is, in order) and brought up to date after each keystroke by reading again only the words around the edit, so the request only looks at the words that start with the typed letter."; } }
        public override string Method { get { return "EA10's files (typical code and one new word per line, 12,000 and 60,000 lines), with the marked SQL string in the middle of the file, as an edit is. The session of a person typing `comp000` letter by letter: seven texts, one character apart, each a new string instance, and `Locate` + `Suggest` for each (the scan of the host is shared by both, as in the plugin). Three ways to answer: reading the text at every request (as it is), the index built again at every request, and the index updated from the one of the text before. Time of `Suggest` alone, per request, after the same warm-up. The suggestions of the three are compared request by request. Also the same text asked twice (a second Ctrl+Space), and the memory of the index."; } }
        public override string Criterion { get { return "With the index updated after each edit, `Suggest` is at least twice as fast as reading the text, at the largest size, in every host and shape; and the three ways give the same suggestions at every request in every case."; } }
        public override string IfMet { get { return "The words can be kept: turn on `CompletionFeatures.WordIndex` in the default features. What is left of a request is the scan of the host (`Locate`)."; } }
        public override string IfNotMet { get { return "Leave the feature off: reading the text is fast enough that the index does not pay for the memory and the code it needs."; } }

        private static readonly string[] Typed = { "c", "co", "com", "comp", "comp0", "comp00", "comp000" };

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

        /// <summary>The texts of the session: the same file with the word typed so far in the middle, and where the caret is in each.</summary>
        private sealed class Session
        {
            public string[] Texts;
            public int[] Carets;
        }

        private static Session Typing(HostLanguage host, string body)
        {
            int middle = body.IndexOf('\n', body.Length / 2) + 1;
            string before = body.Substring(0, middle), after = body.Substring(middle);
            var session = new Session { Texts = new string[Typed.Length], Carets = new int[Typed.Length] };
            for (int i = 0; i < Typed.Length; i++)
            {
                string wrapped = Wrap(host, Typed[i]);
                session.Texts[i] = before + wrapped + after;
                session.Carets[i] = before.Length + wrapped.IndexOf(Typed[i], StringComparison.Ordinal) + Typed[i].Length;
            }
            return session;
        }

        private static CompletionEngine Engine(HostLanguage host, bool index)
        {
            IHostScanner scanner = new CachingHostScanner(NestLightComposition.CreateScanner(host, NestLightComposition.CreateEmbeddedLanguages()));
            var features = new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true, schema: true, ranker: WordRankers.Blend(0.5),
                wordsBeforeKeywords: true, keywordPriority: true, headKeywords: 12, shortWordsLast: true, wordIndex: index);
            return new CompletionEngine(scanner, matcher: new BandedPrefixMatcher(), features: features);
        }

        private enum Way { Scan, IndexBuiltAgain, IndexUpdated }

        /// <summary>One run of the session: the time of every Suggest, summed, and the suggestions of each request.</summary>
        private static double Run(Way way, HostLanguage host, Session session, ref CompletionEngine engine, List<string> answers, out double locateMs)
        {
            double suggestMs = 0;
            locateMs = 0;
            for (int i = 0; i < session.Texts.Length; i++)
            {
                if (way == Way.IndexBuiltAgain) engine = Engine(host, true);
                var sw = Stopwatch.StartNew();
                CompletionSite site = engine.Locate(session.Texts[i], session.Carets[i]);
                locateMs += sw.Elapsed.TotalMilliseconds;
                sw.Restart();
                IReadOnlyList<Suggestion> items = engine.Suggest(session.Texts[i], site);
                suggestMs += sw.Elapsed.TotalMilliseconds;
                if (answers != null) answers.Add(string.Join("|", items.Select(s => s.Kind + ":" + s.Text)));
            }
            return suggestMs;
        }

        private static double Median(List<double> values)
        {
            values.Sort();
            return values[values.Count / 2];
        }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var typing = new Table("One request while typing, `Suggest` alone (median)", "Host", "Shape", "Lines", "Characters", "Index used", "Locate", "Reading the text", "Index built again", "Index updated", "Updated vs reading", "Index memory");
            var cost = new Table("What the index costs (median)", "Host", "Shape", "Lines", "Built once", "Updated after one character", "Memory");
            var again = new Table("The same text asked again, `Suggest` alone (median)", "Host", "Shape", "Lines", "Reading the text", "Index kept", "Kept vs reading");
            int largest = settings.Lines.Max();
            bool allSame = true;
            double slowestGain = double.MaxValue;
            string slowestCase = "";
            string firstDifference = null;
            int outsideWindow = 0;
            var servedGains = new List<double>();

            foreach (HostLanguage host in SyntheticCode.Hosts)
                foreach (bool distinct in new[] { false, true })
                    foreach (int lines in settings.Lines)
                    {
                        string body = distinct ? DistinctWords(host, lines) : SyntheticCode.ByLines(host, lines, 10);
                        Session session = Typing(host, body);
                        string shape = distinct ? "distinct words" : "typical code";

                        // the same suggestions at every request, whichever way they are made
                        var answers = new Dictionary<Way, List<string>>();
                        foreach (Way way in Enum.GetValues(typeof(Way)))
                        {
                            var list = new List<string>();
                            CompletionEngine e = Engine(host, way != Way.Scan);
                            double ignored;
                            Run(way, host, session, ref e, list, out ignored);
                            answers[way] = list;
                        }
                        for (int i = 0; i < Typed.Length; i++)
                            if (answers[Way.IndexBuiltAgain][i] != answers[Way.Scan][i] || answers[Way.IndexUpdated][i] != answers[Way.Scan][i])
                            {
                                allSame = false;
                                if (firstDifference == null) firstDifference = host + ", " + shape + ", " + lines + " lines, after typing '" + Typed[i] + "'";
                            }

                        var times = new Dictionary<Way, List<double>>();
                        var locates = new List<double>();
                        foreach (Way way in Enum.GetValues(typeof(Way)))
                        {
                            CompletionEngine engine = Engine(host, way != Way.Scan);
                            var list = new List<double>();
                            for (int r = 0; r < settings.Warmup + settings.Runs; r++)
                            {
                                double locate;
                                double suggest = Run(way, host, session, ref engine, null, out locate);
                                if (r < settings.Warmup) continue;
                                list.Add(suggest / Typed.Length * 1000);
                                if (way == Way.Scan) locates.Add(locate / Typed.Length * 1000);
                            }
                            times[way] = list;
                        }
                        double scan = Median(times[Way.Scan]) / 1000, built = Median(times[Way.IndexBuiltAgain]) / 1000, updated = Median(times[Way.IndexUpdated]) / 1000, locateMs = Median(locates) / 1000;
                        double gain = scan / updated;
                        long bytes = WordIndex.Build(session.Texts[Typed.Length - 1], false).Bytes;
                        // the engine reads the words from the index only when the window of the word scan covers the whole text
                        int lastCaret = session.Carets[Typed.Length - 1], lastLength = session.Texts[Typed.Length - 1].Length;
                        bool served = lastCaret <= CompletionEngine.WordScanWindow && lastLength - lastCaret <= CompletionEngine.WordScanWindow;
                        typing.Add(host, shape, lines, session.Texts[0].Length, served ? "yes" : "no: the text is larger than the window", Measure.Ms(locateMs), Measure.Ms(scan), Measure.Ms(built), Measure.Ms(updated), Measure.Ratio(gain), Measure.Kb(bytes / 1024.0));
                        if (lines == largest && gain < slowestGain) { slowestGain = gain; slowestCase = host + ", " + shape; }
                        if (lines == largest && !served) outsideWindow++;
                        if (served && lines >= 12000) { servedGains.Add(gain); }

                        string once = session.Texts[Typed.Length - 2], twice = session.Texts[Typed.Length - 1];
                        WordIndex made = WordIndex.Build(once, false);
                        Sample build = Measure.Run(() => WordIndex.Build(twice, false), settings.Warmup, settings.Runs);
                        Sample update = Measure.Run(() => made.Update(once, twice), settings.Warmup, settings.Runs);
                        cost.Add(host, shape, lines, Measure.Ms(build.Ms), Measure.Ms(update.Ms), Measure.Kb(bytes / 1024.0));

                        // a second request for the same text (the index is kept as it is)
                        CompletionEngine scanning = Engine(host, false), keeping = Engine(host, true);
                        string last = session.Texts[Typed.Length - 1];
                        int caret = session.Carets[Typed.Length - 1];
                        CompletionSite site1 = scanning.Locate(last, caret), site2 = keeping.Locate(last, caret);
                        Sample reading = Measure.Run(() => scanning.Suggest(last, site1), settings.Warmup, settings.Runs);
                        Sample kept = Measure.Run(() => keeping.Suggest(last, site2), settings.Warmup, settings.Runs);
                        again.Add(host, shape, lines, Measure.Ms(reading.Ms), Measure.Ms(kept.Ms), Measure.Ratio(reading.Ms / kept.Ms));
                    }

            outcome.Tables.Add(typing);
            outcome.Tables.Add(again);
            outcome.Tables.Add(cost);
            outcome.CriterionMet = allSame && slowestGain >= 2;
            outcome.Headline = (allSame ? "same suggestions" : "DIFFERENT suggestions") + "; least gain " + Measure.Ratio(slowestGain) + " at " + largest + " lines";
            outcome.Analysis.Add(allSame
                ? "The three ways gave the same suggestions at every request of every case."
                : "The suggestions differ, first in: " + firstDifference + ". The index is wrong until this is fixed.");
            outcome.Analysis.Add("The smallest gain of the updated index over reading the text at " + largest + " lines is " + Measure.Ratio(slowestGain) + " (" + slowestCase + "); the criterion is 2x.");
            if (outsideWindow > 0)
                outcome.Analysis.Add("In " + outsideWindow + " of the cases at " + largest + " lines the text is larger than the window of the word scan (" + CompletionEngine.WordScanWindow + " characters on each side of the caret), where the engine reads the text as before: there the two ways are the same code and the gain is 1x by construction.");
            if (servedGains.Count > 0)
                outcome.Analysis.Add("Where the index serves the request and the file has at least 12,000 lines, the updated index made `Suggest` " + Measure.Ratio(servedGains.Min()) + " to " + Measure.Ratio(servedGains.Max()) + " as fast; the median is " + Measure.Ratio(Median(servedGains)) + ".");
            outcome.Analysis.Add("What stays in a request with the index is `Locate` (the scan of the host, which this experiment does not change) and the work on the words that match; compare the `Locate` column with the others.");
            outcome.Analysis.Add("The index built again at every request pays for reading the text once and for storing it: it only helps when it is kept and updated.");
            return outcome;
        }
    }
}
