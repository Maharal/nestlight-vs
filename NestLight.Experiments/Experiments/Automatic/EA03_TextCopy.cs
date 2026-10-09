using NestLight.Common;
using System.Linq;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA03_TextCopy : Experiment
    {
        public override string Id { get { return "EA03"; } }
        public override string Title { get { return "The text copy on every edit"; } }
        public override string Hypothesis { get { return "On every new snapshot the classifier copies the whole text into a string. Above ~85 KB (about 42k characters) that string lands on the Large Object Heap, which only gen2 collections reclaim: the copy costs time, and the collections cost more."; } }
        public override string Method { get { return "Simulated typing on a synthetic C# file: each edit does what the classifier does (copy the text into a new string, then Highlight it). Three variants: copy + Highlight (current), copy only, Highlight only on an existing string. The time is the mean per edit, so that GC pauses are counted."; } }
        public override string Criterion { get { return "Removing the copy would save at least 1 ms per edit on the 400,000-character file (about 12k lines, a large but ordinary file)."; } }
        public override string IfMet { get { return "The copy is worth removing, for example by scanning over the snapshot instead of a string."; } }
        public override string IfNotMet { get { return "The copy is not worth removing: touching every scanner would buy less than a millisecond on files of ordinary size."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var table = new Table("Mean time per edit", "File", "Copy + highlight", "Copy only", "Highlight only", "Possible saving", "Gen2 collections (copy + highlight / highlight only)", "Allocated per edit (copy + highlight)");
            double saving400k = 0;
            long gen2WithCopy = 0, gen2Without = 0;

            foreach (int chars in settings.Chars)
            {
                string text = SyntheticCode.ByChars(HostLanguage.CSharp, chars, 10);
                char[] source = text.ToCharArray();
                IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.CSharp);

                Sample both = Measure.Run(() => highlighter.Highlight(new string(source)), settings.Warmup, settings.Edits);
                Sample copy = Measure.Run(() => new string(source), settings.Warmup, settings.Edits);
                Sample only = Measure.Run(() => highlighter.Highlight(text), settings.Warmup, settings.Edits);

                double saving = both.MeanMs - only.MeanMs;
                table.Add(Measure.Kb(text.Length * 2 / 1024.0), Measure.Ms(both.MeanMs), Measure.Ms(copy.MeanMs), Measure.Ms(only.MeanMs),
                    Measure.Ms(saving) + " (" + (100 * saving / both.MeanMs).ToString("F0") + "%)", both.Gen2 + " / " + only.Gen2, Measure.Kb(both.AllocKb));
                if (chars == 400000) saving400k = saving;
                gen2WithCopy += both.Gen2; gen2Without += only.Gen2;
            }

            outcome.Tables.Add(table);
            outcome.CriterionMet = saving400k >= 1;
            outcome.Headline = "possible saving " + Measure.Ms(saving400k) + " per edit at 400k characters";
            outcome.Analysis.Add("At 400,000 characters, removing the copy would save at most " + Measure.Ms(saving400k) + " per edit (the \"highlight only\" column is the best case: reading from a snapshot instead of a string would be slower).");
            outcome.Analysis.Add("Gen2 collections over the " + settings.Edits + " edits of every size: " + gen2WithCopy + " with the copy, " + gen2Without + " without it.");
            outcome.Analysis.Add("The benchmark process has a tiny heap; a gen2 collection in Visual Studio walks a much larger one, so the real cost of the collections is probably higher (see EM01).");
            return outcome;
        }
    }
}
