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
    /// One hypothesis with an automated test. The text fields are the stable part (they are written before the first run and go into
    /// the report as they are); <see cref="Run"/> is the test.
    /// </summary>
    internal abstract class Experiment
    {
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
}
