using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace NestLight.Experiments
{
    internal static class Program
    {
        private const string Usage = @"NestLight experiments: hypotheses about the performance of the plugin, tested automatically.

  dotnet run -c Release --project NestLight.Experiments -- [options]

  --only E01,E05   run only these experiments
  --manual <dir>   the manual experiments (Experiments/Manual): generate the code of every host x embedded language combination,
                   run the plugin over it and write a file per combination to read and judge (--host and --language narrow it)
  --generate <dir> write that code, one file per combination, and exit (--host, --language and --repeat narrow or enlarge it)
  --host h         with --generate or --manual: javascript, csharp, python or cpp
  --language l     with --generate or --manual: html, css, sql, json, graphql, xml, markdown, yaml, regex, glsl or wgsl
  --repeat n       with --generate: copies of the sample in each file (default 3)
  --list           list the experiments and exit
  --quick          smoke run with small sizes (numbers are not worth keeping)
  --out <dir>      where to write the report (default: docs/reports; with --quick: the temp folder)
  --corpus <dir>   write the generated corpus of 500 snippets for each language and count the strings the scanner finds in it
  --priors <file>  write NestLight/Completion/KeywordUse.cs from the generated corpus
  --review <dir>   type words of hand-written files, run the completion of the plugin and write the inputs and the top 20 as JSON, to be read
";

        private static int Main(string[] args)
        {
            // reports are versioned: the same text on every machine
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            var settings = new Settings();
            string only = null, outDir = null, generate = null, review = null, host = null, language = null;
            int repeat = 3;
            bool list = false;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--quick": settings.Quick = true; break;
                    case "--list": list = true; break;
                    case "--only" when i + 1 < args.Length: only = args[++i]; break;
                    case "--out" when i + 1 < args.Length: outDir = args[++i]; break;
                    case "--manual" when i + 1 < args.Length: review = args[++i]; break;
                    case "--generate" when i + 1 < args.Length: generate = args[++i]; break;
                    case "--host" when i + 1 < args.Length: host = args[++i]; break;
                    case "--language" when i + 1 < args.Length: language = args[++i]; break;
                    case "--repeat" when i + 1 < args.Length && int.TryParse(args[i + 1], out repeat) && repeat > 0: i++; break;
                    case "--corpus" when i + 1 < args.Length: ReviewRunner.DumpCorpus(args[++i]); return 0;
                    case "--priors" when i + 1 < args.Length: ReviewRunner.WritePriors(args[++i]); return 0;
                    case "--review" when i + 1 < args.Length: ReviewRunner.Run(args[++i]); return 0;
                    default: Console.Error.WriteLine(Usage); return 2;
                }
            }

            Func<Combination, bool> filter = c =>
                (host == null || string.Equals(CombinationGenerator.Folder(c.Host), host, StringComparison.OrdinalIgnoreCase))
                && (language == null || string.Equals(c.Language, language, StringComparison.OrdinalIgnoreCase));
            if (review != null) return CombinationReview.Run(review, filter);
            if (generate != null)
            {
                var written = CombinationGenerator.WriteAll(generate, repeat, filter);
                Console.WriteLine(written.Count + " files written to " + generate);
                return written.Count == 0 ? 2 : 0;
            }

            IList<Experiment> all = Catalog.Automatic();
            if (list)
            {
                foreach (Experiment e in all) Console.WriteLine(e.Id + "  " + e.Title);
                return 0;
            }

            IList<Experiment> selected = all;
            if (only != null)
            {
                var ids = new HashSet<string>(only.Split(',').Select(s => s.Trim()), StringComparer.OrdinalIgnoreCase);
                selected = all.Where(e => ids.Contains(e.Id)).ToList();
                if (selected.Count == 0) { Console.Error.WriteLine("No experiment matches " + only); return 2; }
            }

            string repoRoot = FindRepoRoot();
            RunEnvironment env = RunEnvironment.Detect(repoRoot);
            if (outDir == null)
                outDir = settings.Quick ? Path.Combine(Path.GetTempPath(), "nestlight-experiments") : Path.Combine(repoRoot, "docs", "reports");

            Measure.EnableAllocationTracking();
            var results = new List<Result>();
            foreach (Experiment e in selected)
            {
                Console.Write(e.Id + " " + e.Title + " ... ");
                var result = new Result { Experiment = e };
                results.Add(result);
                var sw = Stopwatch.StartNew();
                try { result.Outcome = e.Run(settings); }
                catch (Exception ex) { result.Error = ex.GetType().Name + ": " + ex.Message; }
                result.Seconds = sw.Elapsed.TotalSeconds;
                Console.WriteLine(result.Outcome != null ? (result.Outcome.CriterionMet == true ? "criterion met" : result.Outcome.CriterionMet == false ? "criterion not met" : "info") : "FAILED: " + result.Error);
            }

            string report = ReportWriter.Write(results, env, settings);
            Directory.CreateDirectory(outDir);
            string name = string.Format("experiments-{0:yyyy-MM-dd}-{1}{2}{3}.md", env.Utc, env.Commit, env.Dirty ? "-dirty" : "", settings.Quick ? "-quick" : "");
            string path = Path.Combine(outDir, name);
            File.WriteAllText(path, report, new UTF8Encoding(false));
            Console.WriteLine("Report: " + path);
            return results.Any(r => r.Outcome == null) ? 1 : 0;
        }

        /// <summary>The folder with the .git entry, looking up from the current folder; the current folder if there is none.</summary>
        private static string FindRepoRoot()
        {
            for (var dir = new DirectoryInfo(Directory.GetCurrentDirectory()); dir != null; dir = dir.Parent)
                if (Directory.Exists(Path.Combine(dir.FullName, ".git")) || File.Exists(Path.Combine(dir.FullName, ".git")))
                    return dir.FullName;
            return Directory.GetCurrentDirectory();
        }
    }
}
