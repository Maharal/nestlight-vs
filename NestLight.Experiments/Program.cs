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
  --kind <kind>    auto (default) or manual. Automatic experiments measure and decide; a manual one only writes its kit
                   (the files to open in Visual Studio and a protocol) under --manual-dir
  --manual-dir <d> where the kits of the manual experiments go (default: the temp folder)
  --generate <dir> write the code generated for every host x embedded language combination, one file per combination, and exit
                   (--host, --language and --repeat narrow or enlarge it; E39 and E40 use the same generator)
  --host h         with --generate: javascript, csharp, python or cpp
  --language l     with --generate: html, css, sql, json, graphql, xml, markdown, yaml, regex, glsl or wgsl
  --repeat n       with --generate: copies of the sample in each file (default 3)
  --list           list the experiments and their kind, and exit
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
            string only = null, outDir = null, kind = "auto", generate = null, host = null, language = null;
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
                    case "--kind" when i + 1 < args.Length: kind = args[++i].ToLowerInvariant(); break;
                    case "--manual-dir" when i + 1 < args.Length: settings.ManualDir = args[++i]; break;
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

            if (generate != null)
            {
                var written = CombinationGenerator.WriteAll(generate, repeat, c =>
                    (host == null || string.Equals(CombinationGenerator.Folder(c.Host), host, StringComparison.OrdinalIgnoreCase))
                    && (language == null || string.Equals(c.Language, language, StringComparison.OrdinalIgnoreCase)));
                Console.WriteLine(written.Count + " files written to " + generate);
                return written.Count == 0 ? 2 : 0;
            }

            if (kind != "auto" && kind != "manual") { Console.Error.WriteLine("--kind is auto or manual"); return 2; }
            ExperimentKind wanted = kind == "manual" ? ExperimentKind.Manual : ExperimentKind.Automatic;
            IList<Experiment> all = Catalog.All();
            if (list)
            {
                foreach (Experiment e in all) Console.WriteLine(e.Id + "  " + (e.Kind == ExperimentKind.Manual ? "manual " : "auto   ") + e.Title);
                return 0;
            }
            all = all.Where(e => e.Kind == wanted).ToList();

            IList<Experiment> selected = all;
            if (only != null)
            {
                var ids = new HashSet<string>(only.Split(',').Select(s => s.Trim()), StringComparer.OrdinalIgnoreCase);
                selected = all.Where(e => ids.Contains(e.Id)).ToList();
                if (selected.Count == 0) { Console.Error.WriteLine("No " + kind + " experiment matches " + only); return 2; }
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
