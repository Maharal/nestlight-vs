using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Detection;
using NestLight.Experiments.Detection;

namespace NestLight.Experiments
{
    internal sealed class EA35_DetectionAccuracy : Experiment
    {
        private const int Seeds = 200;

        public override string Id { get { return "EA35"; } }
        public override string Title { get { return "Does the automatic detector recognize the code, and leave the text alone?"; } }
        public override string Hypothesis { get { return "EA34 says what the detector costs, not whether it is right. Its heuristics were tried on a few dozen hand-written strings. On random code of each language, written on one line and on several, it recognizes the code; and on ordinary strings (messages, paths, prose, on purpose some that look like code) it stays silent, because a false positive paints plain text."; } }
        public override string Method { get { return "RandomSnippets writes 200 snippets of each language the detector has a rule for (SQL, HTML, JSON, CSS, GraphQL), from seeds 1 to 200, in two shapes: as written (several lines) and with every line break turned into a space (one line). The detector (the one of the plugin, gated) is asked about each. A hit is the language of the snippet; a miss is nothing; a confusion is another language. The ordinary strings are the pool of EA34 plus some written on several lines and some that start with a word of code. The detector is asked about each: any answer is a false positive."; } }
        public override string Criterion { get { return "For every language and both shapes, at least 90% of the snippets are recognized, none is recognized as another language, and at most 1% of the ordinary strings are recognized as code."; } }
        public override string IfMet { get { return "The detector can be turned on for code written the way it is generated here, with the risk of coloring plain text under 1%."; } }
        public override string IfNotMet { get { return "The languages and shapes listed with a low recall are code the detector leaves plain; each is a heuristic to widen, or a limit to write down in the options page."; } }

        /// <summary>Ordinary strings that are not on EA34's pool: several lines, and words of code at the start.</summary>
        internal static readonly string[] MoreOrdinary =
        {
            "Dear customer,\nYour order has shipped.\nThanks for shopping with us.",
            "Select all the items you want to delete from the list below",
            "select the table from which to read, then press OK",
            "Update the settings where needed and save them",
            "with great power comes great responsibility",
            "drop me a line when you arrive",
            "create a new account to continue",
            "Step 1: open the file\nStep 2: edit it\nStep 3: save it",
            "[Info] 2024-01-01 12:00:00 service started",
            "{name} has joined {room}",
            "<not a tag, just text that starts with a bracket",
            "Total: 3 items, 2 pending, 1 done.",
            "query failed, please try again later",
            "fragment of a longer text that ends with a brace }",
            ".NET Framework 4.8 is required to run this tool",
            "#hashtag and more text after it, all plain",
            "a { b } c",
            "[this, that, and the other]",
        };

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            LanguageDetector detector = LanguageDetector.Create(true);
            int seeds = settings.Quick ? 40 : Seeds;

            var table = new Table("Snippets recognized, by language and shape", "Language", "Shape", "Snippets", "Recognized", "Missed", "Confused with another");
            var worstMisses = new List<string>();
            bool recallMet = true, confusionMet = true;
            foreach (string language in LanguageDetector.Languages)
            {
                foreach (bool oneLine in new[] { false, true })
                {
                    int hit = 0, miss = 0, confused = 0;
                    string firstMiss = null;
                    for (int seed = 1; seed <= seeds; seed++)
                    {
                        string text = Snippet(language, seed, oneLine);
                        string found = detector.Detect(text, 0, text.Length);
                        if (found == language) hit++;
                        else if (found == null) { miss++; if (firstMiss == null) firstMiss = text; }
                        else confused++;
                    }
                    table.Add(language, oneLine ? "one line" : "several lines", seeds, Percent(hit, seeds), miss, confused);
                    if (hit * 100 < 90 * seeds) recallMet = false;
                    if (confused > 0) confusionMet = false;
                    if (firstMiss != null && hit * 100 < 90 * seeds) worstMisses.Add(language + (oneLine ? ", one line" : ", several lines") + ": " + firstMiss.Replace("\n", "\\n"));
                }
            }
            outcome.Tables.Add(table);

            var ordinary = DetectionCorpus.Ordinary.Concat(MoreOrdinary).ToList();
            var falsePositives = new Table("Ordinary strings recognized as code", "String", "Recognized as");
            foreach (string s in ordinary)
            {
                string found = detector.Detect(s, 0, s.Length);
                if (found != null) falsePositives.Add(s.Replace("\n", "\\n"), found);
            }
            bool fpMet = falsePositives.Rows.Count * 100 <= ordinary.Count;
            if (falsePositives.Rows.Count > 0) outcome.Tables.Add(falsePositives);

            outcome.CriterionMet = recallMet && confusionMet && fpMet;
            outcome.Headline = (recallMet ? "recall at least 90%" : "recall under 90% somewhere") + ", " + (confusionMet ? "no confusion" : "confusions") + ", "
                + falsePositives.Rows.Count + " of " + ordinary.Count + " ordinary strings recognized as code";
            if (worstMisses.Count > 0)
            {
                outcome.Analysis.Add("A snippet missed, for each language and shape under 90%:");
                foreach (string m in worstMisses) outcome.Analysis.Add("`" + m + "`");
            }
            outcome.Analysis.Add("The snippets are random but written by us: this says how the heuristics fare on code of this shape, not on every code. The real test is a corpus taken from real projects (see EM02 for the pages to read).");
            return outcome;
        }

        /// <summary>The snippet of the language as the detector would see it in the string: the slot filled, optionally on one line.</summary>
        internal static string Snippet(string language, int seed, bool oneLine)
        {
            string text = RandomSnippets.For(language, seed).Replace(LanguageSamples.Slot, "42");
            return oneLine ? text.Replace("\r", "").Replace('\n', ' ') : text;
        }

        private static string Percent(int part, int whole)
        {
            return (100.0 * part / whole).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "%";
        }
    }
}
