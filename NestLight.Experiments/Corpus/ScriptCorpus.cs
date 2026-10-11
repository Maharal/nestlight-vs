using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NestLight.Experiments
{
    internal static class ScriptCorpus
    {
        private static readonly string[] Commands = { "grep", "find", "sed", "awk", "sort", "uniq", "wc", "cut", "head", "tail", "xargs", "curl", "tar", "rsync" };
        private static readonly string[] Builtins = { "echo", "printf", "read", "cd", "export", "source", "test", "eval" };
        private static readonly string[] Dirs = { "/var/log", "/tmp", "/opt", "$HOME", "/etc", "/usr/local/bin", "/srv" };
        private static readonly string[] Exts = { "log", "txt", "csv", "json", "xml", "conf", "sh", "pid" };
        private static readonly string[] Nouns = { "deploy", "backup", "sync", "report", "build", "clean", "check", "init", "migrate", "rotate" };
        private static readonly string[] Keys = { "name", "version", "port", "host", "timeout", "workers", "level", "path", "enabled", "format" };
        private static readonly string[] Sections = { "server", "database", "logging", "cache", "auth", "metrics", "storage", "proxy" };

        private static string Indent(string text, string prefix) { return string.Join("\n", text.Split('\n').Select(l => l.Length == 0 ? l : prefix + l)); }

        public static CorpusDocument Shell(Dice d, int count)
        {
            var sb = new StringBuilder("const scripts = {\n");
            for (int n = 0; n < count; n++)
                sb.Append("  ").Append(d.Pick(Nouns)).Append(n).Append(": shell`\n").Append(Indent(ShellBody(d), "    ")).Append("\n  `,\n");
            sb.Append("};\n");
            return new CorpusDocument { Language = "shell", Text = sb.ToString(), Snippets = count };
        }

        private static string ShellBody(Dice d)
        {
            var lines = new List<string>();
            if (d.Chance(0.7)) lines.Add("#!/bin/bash");
            lines.Add("set -" + d.Pick(new[] { "e", "eu", "euo pipefail" }));
            if (d.Chance(0.3)) lines.Add("# " + d.Pick(Nouns) + " script");
            string var1 = d.Pick(Nouns).ToUpperInvariant() + "_DIR";
            string var2 = d.Pick(Nouns).ToUpperInvariant() + "_FILE";
            lines.Add(var1 + "=\"" + d.Pick(Dirs) + "/" + d.Pick(Nouns) + "\"");
            lines.Add(var2 + "=\"$" + var1 + "/" + d.Pick(Nouns) + "." + d.Pick(Exts) + "\"");
            for (int i = 0; i < d.Between(2, 5); i++)
            {
                string cmd = d.Pick(Commands.Concat(Builtins).ToArray());
                string flags = d.Chance(0.5) ? " " + d.Pick(new[] { "-r", "-v", "-n", "-f", "--quiet", "-l", "-i", "--recursive" }) : "";
                string arg = d.Pick(new[] { "$" + var1, "$" + var2, "\"$1\"", "*." + d.Pick(Exts), d.Pick(Dirs) });
                string pipe = d.Chance(0.4) ? " | " + d.Pick(Commands) + (d.Chance(0.5) ? " -" + d.Pick(new[] { "c", "l", "n", "r" }) : "") : "";
                string redir = d.Chance(0.2) ? " >> $" + var2 : d.Chance(0.1) ? " 2>&1" : "";
                lines.Add(cmd + flags + " " + arg + pipe + redir);
            }
            if (d.Chance(0.5))
            {
                lines.Add("if [ -d \"$" + var1 + "\" ]; then");
                lines.Add("  " + d.Pick(Builtins) + " \"$" + var1 + "\"");
                lines.Add("else");
                lines.Add("  mkdir -p \"$" + var1 + "\"");
                lines.Add("fi");
            }
            if (d.Chance(0.4))
            {
                lines.Add("for f in \"$" + var1 + "\"/*." + d.Pick(Exts) + "; do");
                lines.Add("  " + d.Pick(Commands) + " \"$f\" " + (d.Chance(0.5) ? "| " + d.Pick(Commands) : ""));
                lines.Add("done");
            }
            if (d.Chance(0.3)) lines.Add("echo \"Done: $(date)\"");
            return string.Join("\n", lines);
        }

        public static CorpusDocument Toml(Dice d, int count)
        {
            var sb = new StringBuilder("const configs = {\n");
            for (int n = 0; n < count; n++)
                sb.Append("  config").Append(n).Append(": toml`\n").Append(Indent(TomlBody(d), "    ")).Append("\n  `,\n");
            sb.Append("};\n");
            return new CorpusDocument { Language = "toml", Text = sb.ToString(), Snippets = count };
        }

        private static string TomlBody(Dice d)
        {
            var lines = new List<string>();
            if (d.Chance(0.3)) lines.Add("# " + d.Pick(Nouns) + " configuration");
            int sections = d.Between(2, 4);
            for (int s = 0; s < sections; s++)
            {
                if (s > 0) lines.Add("");
                string section = d.Pick(Sections);
                if (d.Chance(0.2))
                {
                    lines.Add("[[" + section + "]]");
                }
                else
                {
                    lines.Add("[" + section + "]");
                }
                int keys = d.Between(2, 5);
                for (int k = 0; k < keys; k++)
                {
                    string key = d.Pick(Keys);
                    string value;
                    switch (d.Between(0, 6))
                    {
                        case 0: value = "\"" + d.Pick(Nouns) + "-" + d.Pick(Nouns) + "\""; break;
                        case 1: value = d.Between(0, 9999).ToString(); break;
                        case 2: value = d.Between(0, 9) + "." + d.Between(0, 99); break;
                        case 3: value = d.Chance(0.5) ? "true" : "false"; break;
                        case 4: value = (2020 + d.Between(0, 6)) + "-" + d.Between(1, 12).ToString("D2") + "-" + d.Between(1, 28).ToString("D2"); break;
                        case 5: value = "[\"" + d.Pick(Nouns) + "\", \"" + d.Pick(Nouns) + "\"]"; break;
                        default: value = "'" + d.Pick(Dirs) + "/" + d.Pick(Nouns) + "'"; break;
                    }
                    string comment = d.Chance(0.15) ? " # " + d.Pick(Nouns) : "";
                    lines.Add(key + " = " + value + comment);
                }
            }
            return string.Join("\n", lines);
        }
    }
}
