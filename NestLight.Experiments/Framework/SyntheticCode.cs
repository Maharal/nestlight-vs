using System.Text;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    /// <summary>
    /// The synthetic files of the experiments. A file repeats one unit of typical code of the host (comments, plain strings,
    /// interpolation). The unit is part of every experiment that uses it: changing it changes the experiment.
    /// </summary>
    internal static class SyntheticCode
    {
        public static readonly HostLanguage[] Hosts =
            { HostLanguage.JavaScript, HostLanguage.CSharp, HostLanguage.Python, HostLanguage.Cpp };

        /// <param name="markedEvery">0: no marked string; n: one unit in n has a marked string.</param>
        public static string Unit(HostLanguage host, bool marked)
        {
            switch (host)
            {
                case HostLanguage.CSharp:
                    return "// compute the total for the user\npublic int Compute(string name, int count)\n{\n"
                        + "    var message = \"hello \" + name + $\" {count} items\";\n"
                        + "    if (count > 10) { return count * 2; } // double it\n"
                        + "    var text = @\"verbatim \"\"x\"\"\";\n"
                        + (marked ? "    // language=sql\n    var q = \"select * from t where a = 1\";\n" : "    var q = \"select\";\n")
                        + "    return 0;\n}\n";
                case HostLanguage.JavaScript:
                    return "// compute the total for the user\nexport function compute(name, count) {\n"
                        + "  const message = `hello ${name} ${count} items`;\n"
                        + "  if (count > 10) { return count * 2; } // double it\n"
                        + "  const t = 'plain' + \"strings\";\n"
                        + (marked ? "  const v = html`<li class=\"a\">${name}</li>`;\n" : "  const v = `<li>${name}</li>`;\n")
                        + "  return JSON.stringify(message);\n}\n";
                case HostLanguage.Python:
                    return "# compute the total for the user\ndef compute(name, count):\n"
                        + "    message = f\"hello {name} {count} items\"\n"
                        + "    if count > 10:\n        return count * 2  # double it\n"
                        + "    t = 'plain' + \"strings\"\n"
                        + (marked ? "    # language=sql\n    q = \"select * from t where a = 1\"\n" : "    q = \"select\"\n")
                        + "    return 0\n\n";
                default:
                    return "// compute the total for the user\nint compute(const std::string& name, int count)\n{\n"
                        + "    auto message = \"hello \" + name;\n"
                        + "    if (count > 10) { return count * 2; } // double it\n"
                        + (marked ? "    // language=sql\n    auto q = R\"(select * from t)\";\n" : "    auto q = R\"(select)\";\n")
                        + "    return 0;\n}\n";
            }
        }

        public static string ByLines(HostLanguage host, int lines, int markedEvery)
        {
            return Build(host, markedEvery, (chars, n) => n >= lines);
        }

        public static string ByChars(HostLanguage host, int chars, int markedEvery)
        {
            return Build(host, markedEvery, (n, lines) => n >= chars);
        }

        /// <param name="done">Receives the characters and the lines built so far.</param>
        private static string Build(HostLanguage host, int markedEvery, System.Func<int, int, bool> done)
        {
            var sb = new StringBuilder();
            string plain = Unit(host, false), marked = Unit(host, true);
            int lines = 0;
            for (int i = 0; ; i++)
            {
                string unit = markedEvery > 0 && i % markedEvery == 0 ? marked : plain;
                sb.Append(unit);
                lines += Count(unit);
                if (done(sb.Length, lines)) return sb.ToString();
            }
        }

        private static int Count(string s)
        {
            int n = 0;
            foreach (char c in s) if (c == '\n') n++;
            return n;
        }

        public static int Lines(string text) { return Count(text); }
    }
}
