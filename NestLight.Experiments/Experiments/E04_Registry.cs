using System;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class E04_Registry : Experiment
    {
        public override string Id { get { return "E04"; } }
        public override string Title { get { return "The language registry built per buffer"; } }
        public override string Hypothesis { get { return "CreateHighlighter builds all the tokenizers again for every open file, and that cost adds up when many files are open."; } }
        public override string Method { get { return "Time and allocation of CreateLanguages(), and of 100 consecutive CreateHighlighter calls (100 buffers) for each host."; } }
        public override string Criterion { get { return "Opening a buffer costs less than 1 ms and 1 MB, for every host."; } }
        public override string IfMet { get { return "The registry per buffer is not a cost worth removing."; } }
        public override string IfNotMet { get { return "A shared registry (or lazily built tokenizers) would pay off."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            Sample languages = Measure.Run(() => NestLightComposition.CreateLanguages(), 50, 200);
            var table = new Table("Cost of opening one buffer", "Host", "Time per buffer", "Allocated per buffer");
            table.Add("CreateLanguages() alone", Measure.Ms(languages.Ms), Measure.Kb(languages.AllocKb));

            bool met = languages.Ms < 1 && languages.AllocKb < 1024;
            double worstMs = languages.Ms, worstKb = languages.AllocKb;
            foreach (HostLanguage host in SyntheticCode.Hosts)
            {
                var keep = new object[100];
                Sample s = Measure.Run(() => { for (int i = 0; i < keep.Length; i++) keep[i] = NestLightComposition.CreateHighlighter(host); }, 3, 20);
                double ms = s.Ms / keep.Length, kb = s.AllocKb / keep.Length;
                table.Add("CreateHighlighter(" + host + ")", Measure.Ms(ms), Measure.Kb(kb));
                met &= ms < 1 && kb < 1024;
                worstMs = Math.Max(worstMs, ms); worstKb = Math.Max(worstKb, kb);
            }

            outcome.Tables.Add(table);
            outcome.CriterionMet = met;
            outcome.Headline = "worst " + Measure.Ms(worstMs) + " and " + Measure.Kb(worstKb) + " per buffer";
            outcome.Analysis.Add("Opening a buffer costs at most " + Measure.Ms(worstMs) + " and " + Measure.Kb(worstKb) + " across the hosts.");
            return outcome;
        }
    }
}
