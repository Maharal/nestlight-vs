using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA_02_SkipScan : Experiment
    {
        public override string Id { get { return "EA_02"; } }
        public override string Title { get { return "Skip the scan when the text has no language id"; } }
        public override string Hypothesis { get { return "A string is only embedded code when a tag or comment carries a known language id. If the text contains none of them, the scan could not find anything, so skipping it saves time on C# and Python, which have no shortcut of their own."; } }
        public override string Method { get { return "A decorator around the real highlighter looks for each language id with IndexOf(OrdinalIgnoreCase) and returns no tokens when none appears. Same files, with and without the decorator."; } }
        public override string Criterion { get { return "At least 2x faster on every file without marked strings, and no more than 1.1x slower on every file with them."; } }
        public override string IfMet { get { return "The shortcut is worth adding to the engine."; } }
        public override string IfNotMet { get { return "The shortcut is not worth adding: it either gains too little where the scan is already cheap or penalizes the files that use the plugin."; } }

        private sealed class Prechecked : IHighlighter
        {
            private readonly IHighlighter _inner;
            private readonly string[] _ids;
            private static readonly IReadOnlyList<Token> None = new Token[0];

            public Prechecked(IHighlighter inner, string[] ids) { _inner = inner; _ids = ids; }

            public IReadOnlyList<Token> Highlight(string text)
            {
                foreach (string id in _ids)
                    if (text.IndexOf(id, StringComparison.OrdinalIgnoreCase) >= 0) return _inner.Highlight(text);
                return None;
            }
        }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var table = new Table("Median time, current engine against the decorated one", "Host", "Marked strings", "Lines", "Current", "With shortcut", "Speed-up");
            string[] ids = ((EmbeddedLanguageRegistry)NestLightComposition.CreateEmbeddedLanguages()).Ids.ToArray();
            bool met = true;
            double bestUnmarked = 0, worstMarked = double.MaxValue;

            foreach (HostLanguage host in new[] { HostLanguage.CSharp, HostLanguage.Python })
                foreach (int markedEvery in new[] { 0, 10 })
                    foreach (int lines in settings.Lines)
                    {
                        string text = SyntheticCode.ByLines(host, lines, markedEvery);
                        IHighlighter current = NestLightComposition.CreateHighlighter(host);
                        IHighlighter shortcut = new Prechecked(NestLightComposition.CreateHighlighter(host), ids);
                        Sample a = Measure.Run(() => current.Highlight(text), settings.Warmup, settings.Runs);
                        Sample b = Measure.Run(() => shortcut.Highlight(text), settings.Warmup, settings.Runs);
                        double speedup = a.Ms / b.Ms;
                        table.Add(host, markedEvery == 0 ? "none" : "1 unit in 10", lines, Measure.Ms(a.Ms), Measure.Ms(b.Ms), Measure.Ratio(speedup));
                        if (markedEvery == 0) { met &= speedup >= 2; bestUnmarked = Math.Max(bestUnmarked, speedup); }
                        else { met &= speedup >= 1 / 1.1; worstMarked = Math.Min(worstMarked, speedup); }
                    }

            outcome.Tables.Add(table);
            outcome.CriterionMet = met;
            outcome.Headline = "best " + Measure.Ratio(bestUnmarked) + " without markers, worst " + Measure.Ratio(worstMarked) + " with them";
            outcome.Analysis.Add("Best speed-up on files without markers: " + Measure.Ratio(bestUnmarked) + ". Worst result on files with markers: " + Measure.Ratio(worstMarked) + " (below 1x means slower).");
            return outcome;
        }
    }
}
