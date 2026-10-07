using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NestLight.Experiments
{
    /// <summary>Where and on what code a run happened: without this, a result cannot be compared with another one.</summary>
    internal sealed class RunEnvironment
    {
        public DateTime Utc = DateTime.UtcNow;
        public string Commit = "unknown";
        public bool Dirty;
        public string Runtime = RuntimeInformation.FrameworkDescription;
        public string Os = RuntimeInformation.OSDescription;
        public string Machine;
        public bool Release = true;

        public static RunEnvironment Detect(string repoRoot)
        {
            var env = new RunEnvironment();
#if DEBUG
            env.Release = false;
#endif
            string cpu = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");
            env.Machine = (string.IsNullOrEmpty(cpu) ? "CPU not identified" : cpu) + ", " + Environment.ProcessorCount + " logical cores";

            string sha = Environment.GetEnvironmentVariable("GITHUB_SHA");
            if (!string.IsNullOrEmpty(sha)) env.Commit = sha.Substring(0, Math.Min(7, sha.Length));
            else
            {
                string head = Git(repoRoot, "rev-parse --short=7 HEAD");
                if (!string.IsNullOrEmpty(head)) env.Commit = head;
            }
            string status = Git(repoRoot, "status --porcelain");
            env.Dirty = !string.IsNullOrEmpty(status);
            return env;
        }

        private static string Git(string workingDirectory, string arguments)
        {
            try
            {
                var info = new ProcessStartInfo("git", arguments)
                {
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                using (Process p = Process.Start(info))
                {
                    string output = p.StandardOutput.ReadToEnd().Trim();
                    p.WaitForExit();
                    return p.ExitCode == 0 ? output : null;
                }
            }
            catch (Exception)
            {
                return null; // no git on the machine: the commit stays unknown
            }
        }
    }
}
