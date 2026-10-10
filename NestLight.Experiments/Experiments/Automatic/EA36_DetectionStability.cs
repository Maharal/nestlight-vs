using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Detection;

namespace NestLight.Experiments
{
    internal sealed class EA36_DetectionStability : Experiment
    {
        public override string Id { get { return "EA36"; } }
        public override string Title { get { return "Does the language of a string flicker while it is typed?"; } }
        public override string Hypothesis { get { return "The detector is asked again at every edit. While a person types a string one character at a time, the answer should settle: nothing, then the language, and nothing again only if the string is broken. If it flips back and forth, the color of the string blinks at every key."; } }
        public override string Method { get { return "For 100 random snippets of each language the detector has a rule for (seeds 1 to 100, written on several lines), the detector is asked about every prefix of the snippet, as if the string were typed from its first character. The number of changes of the answer along the prefixes is counted (nothing to a language, a language to nothing and a language to another all count), and so is the length at which the right language is first recognized, as a share of the whole snippet."; } }
        public override string Criterion { get { return "For every language, at least 95% of the snippets change their answer at most twice (nothing, the language, nothing) and none is ever recognized as another language."; } }
        public override string IfMet { get { return "The color of a string settles as it is typed and does not blink."; } }
        public override string IfNotMet { get { return "The languages listed flip while typed: the heuristic needs a rule that holds on a prefix (for example, to ask for the closing character only when the string is finished)."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            LanguageDetector detector = LanguageDetector.Create(true);
            int seeds = settings.Quick ? 20 : 100;
            var table = new Table("Changes of the answer while typing", "Language", "Snippets", "Never recognized", "At most 2 changes", "Most changes", "Median length when first recognized", "Recognized as another language");
            bool met = true;
            string worst = null;
            int worstChanges = 0;
            foreach (string language in LanguageDetector.Languages)
            {
                int never = 0, calm = 0, most = 0, other = 0;
                var firsts = new List<double>();
                for (int seed = 1; seed <= seeds; seed++)
                {
                    string text = EA35_DetectionAccuracy.Snippet(language, seed, false);
                    string previous = null;
                    int changes = 0, first = -1;
                    bool another = false;
                    for (int length = 1; length <= text.Length; length++)
                    {
                        string found = detector.Detect(text, 0, length);
                        if (found != previous) changes++;
                        if (found == language && first < 0) first = length;
                        if (found != null && found != language) another = true;
                        previous = found;
                    }
                    if (first < 0) never++; else firsts.Add(100.0 * first / text.Length);
                    if (changes <= 2) calm++;
                    if (another) other++;
                    if (changes > most) most = changes;
                    if (changes > worstChanges) { worstChanges = changes; worst = language + " seed " + seed; }
                }
                firsts.Sort();
                table.Add(language, seeds, never, (100.0 * calm / seeds).ToString("0", System.Globalization.CultureInfo.InvariantCulture) + "%", most,
                    firsts.Count == 0 ? "-" : firsts[firsts.Count / 2].ToString("0", System.Globalization.CultureInfo.InvariantCulture) + "%", other);
                if (calm * 100 < 95 * seeds || other > 0) met = false;
            }
            outcome.Tables.Add(table);
            outcome.CriterionMet = met;
            outcome.Headline = met ? "the answer settles for every language" : "some languages flip while typed (the worst: " + worst + ", " + worstChanges + " changes)";
            outcome.Analysis.Add("A language that is \"never recognized\" is a snippet the detector never accepts, as in EA35. A language recognized only at the end (a median near 100%) cannot color the string while it is typed.");
            return outcome;
        }
    }
}
