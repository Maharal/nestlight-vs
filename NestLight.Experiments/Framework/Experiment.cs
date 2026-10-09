using System;
using System.Collections.Generic;

namespace NestLight.Experiments
{
    /// <summary>Knobs shared by all the experiments. <see cref="Quick"/> is a smoke run: small sizes, few repetitions, numbers not worth keeping.</summary>
    internal sealed class Settings
    {
        public bool Quick;
        public int Warmup { get { return Quick ? 2 : 10; } }
        public int Runs { get { return Quick ? 5 : 25; } }
        /// <summary>For the experiments whose single run is slow.</summary>
        public int SlowRuns { get { return Quick ? 3 : 7; } }
        public int Edits { get { return Quick ? 30 : 300; } }
        public int[] Chars { get { return Quick ? new[] { 100000, 400000 } : new[] { 100000, 400000, 1000000, 4000000 }; } }
        /// <summary>Where the manual experiments write their kits.</summary>
        public string ManualDir = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "nestlight-manual");
        public int[] Lines { get { return Quick ? new[] { 300, 3000 } : new[] { 1200, 12000, 60000 }; } }
    }

    /// <summary>A table of results; cells are already formatted.</summary>
    internal sealed class Table
    {
        public string Title;
        public string[] Headers;
        public List<string[]> Rows = new List<string[]>();

        public Table(string title, params string[] headers)
        {
            Title = title;
            Headers = headers;
        }

        public void Add(params object[] cells)
        {
            var row = new string[cells.Length];
            for (int i = 0; i < cells.Length; i++) row[i] = Convert.ToString(cells[i], System.Globalization.CultureInfo.InvariantCulture);
            Rows.Add(row);
        }
    }

    /// <summary>What an experiment found: tables, the analysis in words, and whether its criterion held.</summary>
    internal sealed class Outcome
    {
        public bool? CriterionMet;
        public string Headline = "";
        public List<Table> Tables = new List<Table>();
        public List<string> Analysis = new List<string>();
    }

    /// <summary>
    /// Who runs the test. An automatic experiment is code that measures and decides by itself; a manual one needs a person in
    /// Visual Studio, so the harness only prepares what the person needs and cannot decide the criterion.
    /// </summary>
    internal enum ExperimentKind { Automatic, Manual }

    /// <summary>
    /// One hypothesis with a test. By default the test is automatic (see <see cref="ExperimentKind"/>). The text fields are the stable part (they are written before the first run and go into
    /// the report as they are); <see cref="Run"/> is the test.
    /// </summary>
    internal abstract class Experiment
    {
        public virtual ExperimentKind Kind { get { return ExperimentKind.Automatic; } }
        public abstract string Id { get; }
        public abstract string Title { get; }
        public abstract string Hypothesis { get; }
        public abstract string Method { get; }
        public abstract string Criterion { get; }
        /// <summary>What it means for the plugin when the criterion is met.</summary>
        public abstract string IfMet { get; }
        /// <summary>What it means when it is not.</summary>
        public abstract string IfNotMet { get; }

        public abstract Outcome Run(Settings settings);
    }

    /// <summary>
    /// An experiment that a person runs in Visual Studio. <see cref="Run"/> does not measure: it writes the kit (the files to open
    /// and the protocol to follow) and reports it, with the criterion left undecided until the person records the result.
    /// </summary>
    internal abstract class ManualExperiment : Experiment
    {
        public sealed override ExperimentKind Kind { get { return ExperimentKind.Manual; } }

        /// <summary>What the person does, in order.</summary>
        public abstract IList<string> Steps { get; }

        /// <summary>Writes the files the steps refer to under <paramref name="dir"/> and returns how many.</summary>
        protected abstract int Prepare(Settings settings, string dir);

        public sealed override Outcome Run(Settings settings)
        {
            string dir = System.IO.Path.Combine(settings.ManualDir, Id);
            System.IO.Directory.CreateDirectory(dir);
            int files = Prepare(settings, dir);
            System.IO.File.WriteAllText(System.IO.Path.Combine(dir, "PROTOCOL.md"), Protocol(files), new System.Text.UTF8Encoding(false));

            var outcome = new Outcome { CriterionMet = null, Headline = "Manual: kit of " + files + " files written to " + dir + "; waiting for a person" };
            var table = new Table("Steps", "#", "Step");
            for (int i = 0; i < Steps.Count; i++) table.Add(i + 1, Steps[i]);
            outcome.Tables.Add(table);
            outcome.Analysis.Add("A person runs this one: open the files in " + dir + ", follow PROTOCOL.md and write what was observed in docs/experiments.md.");
            return outcome;
        }

        private string Protocol(int files)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("# " + Id + ": " + Title).AppendLine();
            sb.AppendLine("**Hypothesis.** " + Hypothesis).AppendLine();
            sb.AppendLine("**Method.** " + Method).AppendLine();
            sb.AppendLine("**Criterion.** " + Criterion).AppendLine();
            sb.AppendLine("The kit has " + files + " files next to this one.").AppendLine();
            sb.AppendLine("## Steps").AppendLine();
            for (int i = 0; i < Steps.Count; i++) sb.AppendLine((i + 1) + ". " + Steps[i]);
            sb.AppendLine().AppendLine("## Record the result").AppendLine();
            sb.AppendLine("Commit: `__________`   Visual Studio: `__________`   Date: `__________`   Criterion met: `yes / no`").AppendLine();
            return sb.ToString();
        }
    }
}
