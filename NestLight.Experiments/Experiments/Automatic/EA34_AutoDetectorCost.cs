using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Experiments.Detection;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA34_AutoDetectorCost : Experiment
    {
        public override string Id { get { return "EA34"; } }
        public override string Title { get { return "What an automatic language detector costs"; } }
        public override string Hypothesis { get { return "An automatic detector that guesses the language of the strings nobody marked, with one cheap heuristic per language behind a common interface (the strategy pattern), makes Highlight at most 1.5x slower on files where one string in four looks like code, allocates almost nothing for the strings it asks about, and keeps almost nothing in memory."; } }
        public override string Method { get { return "A prototype in this project (Detection/): five strategies (SQL, HTML, JSON, CSS, GraphQL), each reading the text where it is, and a context that asks them and keeps the highest score. The files are EA05's, with 8 unmarked strings added to each unit of code (6 ordinary, 2 of code), in the 4 hosts at two sizes. Measured on the same file, in the same process: the current Highlight (A); finding the unmarked strings (B, which the host scanner would already do in the pass it makes); B plus asking the detector (C, with and without a check of the first character before asking a strategy); and the whole thing, Highlight plus detection plus tokenizing the strings found (D). Detection alone is C minus B. Retained memory is what the detector keeps alive once built. A table of the right and wrong answers on the pools of strings, and of the cost of each strategy, goes with it."; } }
        public override string Criterion { get { return "At the larger size, in every host: D at most 1.5x the time of A; the detection allocates at most 100 bytes per string it asks about; and the detector keeps at most 100 KB."; } }
        public override string IfMet { get { return "Detecting the language of the unmarked strings is cheap enough to run on every edit; the cost is not a reason against the feature."; } }
        public override string IfNotMet { get { return "Detection has a price that shows in the editor: it needs a shortcut (a cache per string, or running it only where it is asked for, as a suggestion) before it can run on every edit."; } }

        private static readonly TokenSink Discard = (a, b, t) => { };

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var time = new Table("Median time per call", "Host", "Lines", "Unmarked strings", "Found as code", "A: Highlight", "B: find strings", "Detection, gated", "Detection, every strategy", "Tokenizing what was found", "D: everything", "D / A");
            var memory = new Table("Memory per call", "Host", "Lines", "A: Highlight", "B: find strings", "Detection, gated", "Detection per string", "D: everything", "D / A", "Gen 0 of D");
            bool met = true;
            double worstRatio = 0, worstBytes = 0;
            int[] sizes = settings.Lines.Length > 1 ? new[] { settings.Lines[1], settings.Lines[settings.Lines.Length - 1] } : settings.Lines;
            int larger = sizes[sizes.Length - 1];
            string sampleText = null;

            foreach (HostLanguage host in SyntheticCode.Hosts)
                foreach (int lines in sizes)
                {
                    string text = DetectionCorpus.Build(host, lines);
                    if (host == HostLanguage.JavaScript && lines == larger) sampleText = text;
                    IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
                    var engine = new HighlightEngine(NestLightComposition.CreateScanner(host, languages), languages);
                    LanguageDetector gated = LanguageDetector.Create(true), every = LanguageDetector.Create(false);
                    var bounds = new List<int>(1 << 16);
                    LiteralFinder.Find(host, text, bounds);
                    int literals = bounds.Count / 2, found = 0;
                    for (int k = 0; k < bounds.Count; k += 2) if (gated.Detect(text, bounds[k], bounds[k + 1]) != null) found++;

                    Sample a = Measure.Run(() => engine.Highlight(text), settings.Warmup, settings.Runs);
                    Sample b = Measure.Run(() => LiteralFinder.Find(host, text, bounds), settings.Warmup, settings.Runs);
                    Sample c = Measure.Run(() => { LiteralFinder.Find(host, text, bounds); AskAll(gated, text, bounds); }, settings.Warmup, settings.Runs);
                    Sample u = Measure.Run(() => { LiteralFinder.Find(host, text, bounds); AskAll(every, text, bounds); }, settings.Warmup, settings.Runs);
                    Sample d = Measure.Run(() =>
                    {
                        engine.Highlight(text);
                        LiteralFinder.Find(host, text, bounds);
                        TokenizeFound(gated, languages, text, bounds);
                    }, settings.Warmup, settings.Runs);

                    double detection = Math.Max(0, c.Ms - b.Ms), detectionEvery = Math.Max(0, u.Ms - b.Ms);
                    double ratio = d.Ms / a.Ms;
                    double tokenizing = Math.Max(0, d.Ms - a.Ms - c.Ms);
                    double perString = Math.Max(0, c.AllocKb - b.AllocKb) * 1024 / Math.Max(1, literals);
                    time.Add(host, lines, literals, found, Measure.Ms(a.Ms), Measure.Ms(b.Ms), Measure.Ms(detection), Measure.Ms(detectionEvery), Measure.Ms(tokenizing), Measure.Ms(d.Ms), Measure.Ratio(ratio));
                    memory.Add(host, lines, Measure.Kb(a.AllocKb), Measure.Kb(b.AllocKb), Measure.Kb(Math.Max(0, c.AllocKb - b.AllocKb)), perString.ToString("F0") + " B", Measure.Kb(d.AllocKb), Measure.Ratio(d.AllocKb / a.AllocKb), d.Gen0);
                    if (lines == larger)
                    {
                        worstRatio = Math.Max(worstRatio, ratio);
                        worstBytes = Math.Max(worstBytes, perString);
                        met &= ratio <= 1.5 && perString <= 100;
                    }
                }

            long retained = Retained();
            met &= retained <= 100 * 1024;

            outcome.Tables.Add(time);
            outcome.Tables.Add(memory);
            outcome.Tables.Add(StrategyCosts(settings, sampleText));
            outcome.Tables.Add(Accuracy(outcome));
            outcome.CriterionMet = met;
            outcome.Headline = "everything " + Measure.Ratio(worstRatio) + " of Highlight at worst, " + worstBytes.ToString("F0") + " B per string asked, " + retained + " B retained";
            outcome.Analysis.Insert(0, "Worst case at the larger size: Highlight plus detection takes " + Measure.Ratio(worstRatio) + " the time of Highlight alone. The detection allocates at most " + worstBytes.ToString("F0") + " bytes per string it asks about, and a built detector keeps " + retained + " bytes alive (the tables of the strategies are static and shared).");
            outcome.Analysis.Add("What D is made of: Highlight (A), finding the strings (B, which the host scanner would already do), asking the detector, and tokenizing what the detector found. The last is the cost of coloring those strings, the same work a marked string costs today; it grows with how many strings are code, and in these files one string in four is.");
            outcome.Analysis.Add("The prototype lives in this project only; nothing in the plugin changed. The finder of unmarked strings is a simplified lexer: in the plugin the host scanner would hand the strings over in the pass it already makes, so column B is an upper bound of that part, not a cost the detector adds.");
            outcome.Analysis.Add("Cost is not the whole question: a detector also colors strings by a guess. The accuracy table is on 20 ordinary strings (the last " + DetectionCorpus.HardOrdinary + " on purpose hard) and 16 of code; it says how the heuristics fare, not how they would fare on real code.");
            return outcome;
        }

        private static void AskAll(LanguageDetector detector, string text, List<int> bounds)
        {
            for (int k = 0; k < bounds.Count; k += 2) detector.Detect(text, bounds[k], bounds[k + 1]);
        }

        private static void TokenizeFound(LanguageDetector detector, IEmbeddedLanguageRegistry languages, string text, List<int> bounds)
        {
            for (int k = 0; k < bounds.Count; k += 2)
            {
                string id = detector.Detect(text, bounds[k], bounds[k + 1]);
                if (id == null) continue;
                IEmbeddedLanguageTokenizer tokenizer = languages.Find(id);
                if (tokenizer == null) continue;
                char[] chars = text.ToCharArray(bounds[k], bounds[k + 1] - bounds[k]);
                tokenizer.Tokenize(chars, 0, chars.Length, Discard);
            }
        }

        /// <summary>What one built detector keeps alive, in bytes: many are built and held, so that the size is above the resolution of the measure.</summary>
        private static long Retained()
        {
            const int count = 1000;
            Measure.Collect();
            long before = GC.GetTotalMemory(true);
            var held = new LanguageDetector[count];
            for (int i = 0; i < count; i++) held[i] = LanguageDetector.Create(true);
            long after = GC.GetTotalMemory(true);
            GC.KeepAlive(held);
            return Math.Max(0, (after - before) / count);
        }

        private static Table StrategyCosts(Settings settings, string text)
        {
            var table = new Table("Cost of each strategy alone (JavaScript, larger size)", "Strategy", "Strings asked", "Time per string", "Answers");
            var bounds = new List<int>();
            LiteralFinder.Find(HostLanguage.JavaScript, text, bounds);
            int literals = bounds.Count / 2;
            foreach (ILanguageDetector strategy in LanguageDetector.Strategies())
            {
                var alone = new LanguageDetector(new[] { strategy }, true);
                int answers = 0;
                for (int k = 0; k < bounds.Count; k += 2) if (alone.Detect(text, bounds[k], bounds[k + 1]) != null) answers++;
                Sample s = Measure.Run(() => AskAll(alone, text, bounds), settings.Warmup, settings.Runs);
                table.Add(strategy.Id, literals, (s.Ms * 1000000 / literals).ToString("F0") + " ns", answers);
            }
            return table;
        }

        private static Table Accuracy(Outcome outcome)
        {
            var table = new Table("Answers of the detector on the pools of strings", "Language", "Strings", "Right", "Wrong language", "Missed");
            LanguageDetector detector = LanguageDetector.Create(true);
            foreach (string language in DetectionCorpus.Code.Select(p => p.Key).Distinct())
            {
                int total = 0, right = 0, wrong = 0, missed = 0;
                foreach (var p in DetectionCorpus.Code.Where(p => p.Key == language))
                {
                    total++;
                    string id = detector.Detect(p.Value, 0, p.Value.Length);
                    if (id == language) right++; else if (id == null) missed++; else wrong++;
                }
                table.Add(language, total, right, wrong, missed);
            }

            var false_ = new List<string>();
            foreach (string s in DetectionCorpus.Ordinary)
                if (detector.Detect(s, 0, s.Length) != null) false_.Add(s);
            table.Add("(ordinary, wrongly taken for code)", DetectionCorpus.Ordinary.Length, DetectionCorpus.Ordinary.Length - false_.Count, false_.Count, 0);
            if (false_.Count > 0) outcome.Analysis.Add("Ordinary strings taken for code: " + string.Join("; ", false_.Select(s => "`" + s + "`")) + ".");
            return table;
        }
    }
}
