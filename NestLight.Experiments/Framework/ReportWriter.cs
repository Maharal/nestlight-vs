using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NestLight.Experiments
{
    internal sealed class Result
    {
        public Experiment Experiment;
        public Outcome Outcome;     // null when the experiment failed to run
        public string Error;
        public double Seconds;
    }

    /// <summary>Turns the results of a run into the Markdown report that gets versioned.</summary>
    internal static class ReportWriter
    {
        public static string Write(IList<Result> results, RunEnvironment env, Settings settings)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# NestLight experiments report");
            sb.AppendLine();
            sb.AppendLine("| | |");
            sb.AppendLine("|---|---|");
            sb.AppendLine("| Date | " + env.Utc.ToString("yyyy-MM-dd HH:mm 'UTC'", CultureInfo.InvariantCulture) + " |");
            sb.AppendLine("| Commit | `" + env.Commit + "`" + (env.Dirty ? " (**uncommitted changes**: this run cannot be reproduced from the commit)" : "") + " |");
            sb.AppendLine("| Runtime | " + env.Runtime + (env.Release ? ", Release" : ", **Debug (numbers are not valid)**") + " |");
            sb.AppendLine("| Machine | " + env.Machine + " |");
            sb.AppendLine("| OS | " + env.Os + " |");
            sb.AppendLine("| Mode | " + (settings.Quick ? "**quick (smoke run, numbers are not worth keeping)**" : "full") + " |");
            sb.AppendLine();
            sb.AppendLine("Definitions of the experiments: [docs/experiments.md](../experiments.md). Each result is valid only for the commit and the machine above.");
            sb.AppendLine();

            sb.AppendLine("## Summary");
            sb.AppendLine();
            sb.AppendLine("| Id | Experiment | Criterion | Finding |");
            sb.AppendLine("|---|---|---|---|");
            foreach (Result r in results)
                sb.AppendLine("| " + r.Experiment.Id + " | " + r.Experiment.Title + " | " + Verdict(r) + " | " + (r.Outcome != null ? r.Outcome.Headline : r.Error) + " |");
            sb.AppendLine();
            sb.AppendLine("*Criterion met* is the statement of each experiment's own criterion, not a good/bad grade: see *If met* and *If not met* under each experiment.");

            foreach (Result r in results)
            {
                Experiment e = r.Experiment;
                sb.AppendLine();
                sb.AppendLine("## " + e.Id + ": " + e.Title);
                sb.AppendLine();
                sb.AppendLine("**Hypothesis.** " + e.Hypothesis);
                sb.AppendLine();
                sb.AppendLine("**Method.** " + e.Method);
                sb.AppendLine();
                sb.AppendLine("**Criterion.** " + e.Criterion);
                sb.AppendLine();
                if (r.Outcome == null)
                {
                    sb.AppendLine("**The experiment failed to run:** `" + r.Error + "`");
                    continue;
                }

                foreach (Table t in r.Outcome.Tables)
                {
                    if (!string.IsNullOrEmpty(t.Title)) { sb.AppendLine("**" + t.Title + "**"); sb.AppendLine(); }
                    sb.AppendLine("| " + string.Join(" | ", t.Headers) + " |");
                    sb.AppendLine("|" + string.Join("|", Array.ConvertAll(t.Headers, h => "---")) + "|");
                    foreach (string[] row in t.Rows) sb.AppendLine("| " + string.Join(" | ", row) + " |");
                    sb.AppendLine();
                }

                sb.AppendLine("**Analysis.**");
                sb.AppendLine();
                foreach (string line in r.Outcome.Analysis) sb.AppendLine("- " + line);
                sb.AppendLine();
                sb.AppendLine("**Criterion " + Verdict(r).ToLowerInvariant() + ".** " + (r.Outcome.CriterionMet == true ? e.IfMet : r.Outcome.CriterionMet == false ? e.IfNotMet : ""));
                sb.AppendLine();
                sb.AppendLine("*Ran in " + r.Seconds.ToString("F1", CultureInfo.InvariantCulture) + " s.*");
            }

            sb.AppendLine();
            sb.AppendLine("## Limits");
            sb.AppendLine();
            sb.AppendLine("- Synthetic code, not real files.");
            sb.AppendLine("- The experiments run in a console process with a small heap, away from the Visual Studio UI thread and its GC pressure. They compare versions of the code: they are not times the user will see.");
            sb.AppendLine("- Times are medians of repeated runs on one machine; differences of a few percent are noise.");
            return sb.ToString();
        }

        private static string Verdict(Result r)
        {
            if (r.Outcome == null) return "Error";
            return r.Outcome.CriterionMet == true ? "Met" : r.Outcome.CriterionMet == false ? "Not met" : "Info";
        }
    }
}
