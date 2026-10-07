using System;
using System.Collections.Generic;
using System.Text;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class CorpusFile
    {
        public HostLanguage Host;
        public string Text;
    }

    /// <summary>
    /// A seeded generator of code that looks a little like real code: functions that use a small pool of domain names (the
    /// same names as variables in the host and as columns, classes and fields in the embedded strings), with SQL, HTML, CSS and
    /// GraphQL strings marked the way the plugin expects. It is not real code. How often a name comes back, and how close to its
    /// last use, is set by the generator, so the experiments that depend on it say what they would find only under that setting.
    /// </summary>
    internal static class SyntheticCorpus
    {
        private static readonly string[] Nouns =
        {
            "customer", "order", "invoice", "product", "account", "payment", "address", "shipment", "cart", "item",
            "user", "session", "ticket", "report", "budget", "project", "team", "message", "comment", "review",
            "coupon", "vendor", "warehouse", "employee", "department", "contract", "booking", "schedule", "balance", "profile"
        };

        private static readonly string[] Suffixes = { "Id", "Name", "Total", "Count", "Date", "Status", "Price", "Email", "Type", "Code", "Level", "Note" };
        private static readonly string[] Verbs = { "load", "save", "update", "remove", "find", "build", "render", "check", "merge", "export" };
        private static readonly string[] Properties = { "color", "margin", "padding", "display", "background", "border", "font-size", "width", "height", "opacity" };
        private static readonly string[] Values = { "red", "none", "auto", "block", "flex", "0", "1px", "100%", "inherit", "solid" };
        private static readonly string[] Tags = { "div", "span", "li", "ul", "p", "button", "section", "h2", "label", "td" };

        /// <param name="files">Spread over the four hosts.</param>
        /// <param name="locality">0: a name is picked at random from the pool; 1: always one of the last few used.</param>
        public static List<CorpusFile> Generate(int files, int seed, double locality, int blocksPerFile = 40)
        {
            var result = new List<CorpusFile>();
            for (int i = 0; i < files; i++)
            {
                HostLanguage host = SyntheticCode.Hosts[i % SyntheticCode.Hosts.Length];
                result.Add(new CorpusFile { Host = host, Text = new Writer(host, new Random(seed * 1009 + i), locality).File(blocksPerFile) });
            }
            return result;
        }

        private struct Concept
        {
            public string Noun, Suffix;
            public string Camel { get { return Noun + Suffix; } }
            public string Snake { get { return Noun + "_" + Suffix.ToLowerInvariant(); } }
            public string Kebab { get { return Noun + "-" + Suffix.ToLowerInvariant(); } }
        }

        private sealed class Writer
        {
            private readonly HostLanguage _host;
            private readonly Random _rng;
            private readonly double _locality;
            private readonly List<Concept> _pool = new List<Concept>();
            private readonly List<int> _recent = new List<int>();

            public Writer(HostLanguage host, Random rng, double locality)
            {
                _host = host; _rng = rng; _locality = locality;
                while (_pool.Count < 40)
                {
                    var c = new Concept { Noun = Nouns[rng.Next(Nouns.Length)], Suffix = Suffixes[rng.Next(Suffixes.Length)] };
                    if (!_pool.Exists(x => x.Camel == c.Camel)) _pool.Add(c);
                }
            }

            private Concept Pick()
            {
                int index;
                if (_recent.Count > 0 && _rng.NextDouble() < _locality) index = _recent[_rng.Next(_recent.Count)];
                else { double r = _rng.NextDouble(); index = (int)(_pool.Count * r * r); } // skewed: a few names are used a lot
                _recent.Add(index);
                if (_recent.Count > 8) _recent.RemoveAt(0);
                return _pool[index];
            }

            private string Var(Concept c) { return _host == HostLanguage.Python ? c.Snake : c.Camel; }
            private string Fn()
            {
                Concept c = Pick();
                string verb = Verbs[_rng.Next(Verbs.Length)];
                return _host == HostLanguage.Python ? verb + "_" + c.Snake : verb + char.ToUpperInvariant(c.Camel[0]) + c.Camel.Substring(1);
            }

            // ---- the embedded languages -------------------------------------------------------------------------

            private string Sql(string param)
            {
                Concept a = Pick(), b = Pick(), c = Pick(), d = Pick();
                string text = "select " + a.Snake + ", " + b.Snake + " from " + a.Noun + "s where " + c.Snake + " = " + Interp(param);
                if (_rng.Next(3) == 0) text += " and " + d.Snake + " is not null";
                if (_rng.Next(2) == 0) text += " order by " + d.Snake + " limit 10";
                return text;
            }

            private string Html(string param)
            {
                string tag = Tags[_rng.Next(Tags.Length)];
                return "<" + tag + " class='" + Pick().Kebab + "'><span>" + Interp(param) + "</span></" + tag + ">";
            }

            private string Css()
            {
                var sb = new StringBuilder("." + Pick().Kebab + " {");
                for (int i = 0, n = 2 + _rng.Next(3); i < n; i++)
                    sb.Append(' ').Append(Properties[_rng.Next(Properties.Length)]).Append(": ").Append(Values[_rng.Next(Values.Length)]).Append(';');
                return sb.Append(" }").ToString();
            }

            private string GraphQl()
            {
                return "query { " + Pick().Camel + "(first: 10) { " + Pick().Camel + " " + Pick().Camel + " } }";
            }

            private string Interp(string expr)
            {
                switch (_host)
                {
                    case HostLanguage.JavaScript: return "${" + expr + "}";
                    case HostLanguage.Cpp: return "1";
                    default: return "{" + expr + "}";
                }
            }

            // ---- the host ---------------------------------------------------------------------------------------

            private string Embed(string id, string body, bool interpolated)
            {
                switch (_host)
                {
                    case HostLanguage.JavaScript: return "  const " + Var(Pick()) + " = " + id + "`" + body + "`;\n";
                    case HostLanguage.CSharp:
                        return "        // language=" + id + "\n        var " + Var(Pick()) + " = " + (interpolated ? "$" : "") + "\"" + body + "\";\n";
                    case HostLanguage.Python:
                        return "    # language=" + id + "\n    " + Var(Pick()) + " = " + (interpolated ? "f" : "") + "\"" + body + "\"\n";
                    default:
                        return "    // language=" + id + "\n    auto " + Var(Pick()) + " = R\"(" + body + ")\";\n";
                }
            }

            private string Statement()
            {
                Concept a = Pick(), b = Pick();
                switch (_host)
                {
                    case HostLanguage.Python: return "    " + Var(a) + " = " + Var(b) + " + 1  # " + a.Noun + "\n";
                    case HostLanguage.JavaScript: return "  let " + Var(a) + " = " + Var(b) + " + 1; // " + a.Noun + "\n";
                    case HostLanguage.CSharp: return "        var " + Var(a) + " = " + Var(b) + " + 1; // " + a.Noun + "\n";
                    default: return "    int " + Var(a) + " = " + Var(b) + " + 1; // " + a.Noun + "\n";
                }
            }

            private string Block()
            {
                Concept p1 = Pick(), p2 = Pick();
                string a = Var(p1), b = Var(p2);
                var sb = new StringBuilder();
                switch (_host)
                {
                    case HostLanguage.JavaScript: sb.Append("export function ").Append(Fn()).Append('(').Append(a).Append(", ").Append(b).Append(") {\n"); break;
                    case HostLanguage.CSharp: sb.Append("    public void ").Append(Fn()).Append("(int ").Append(a).Append(", int ").Append(b).Append(")\n    {\n"); break;
                    case HostLanguage.Python: sb.Append("def ").Append(Fn()).Append('(').Append(a).Append(", ").Append(b).Append("):\n"); break;
                    default: sb.Append("void ").Append(Fn()).Append("(int ").Append(a).Append(", int ").Append(b).Append(")\n{\n"); break;
                }
                for (int i = 0, n = 2 + _rng.Next(3); i < n; i++) sb.Append(Statement());
                bool interp = _host != HostLanguage.Cpp;
                sb.Append(Embed("sql", Sql(a), interp));
                int more = _rng.Next(4);
                if (more == 0) sb.Append(Embed("html", Html(b), interp));
                else if (more == 1) sb.Append(Embed("css", Css(), false));
                else if (more == 2) sb.Append(Embed("graphql", GraphQl(), false));
                sb.Append(Statement());
                switch (_host)
                {
                    case HostLanguage.Python: sb.Append("    return ").Append(a).Append("\n\n"); break;
                    case HostLanguage.JavaScript: sb.Append("  return ").Append(a).Append(";\n}\n\n"); break;
                    case HostLanguage.CSharp: sb.Append("        return;\n    }\n\n"); break;
                    default: sb.Append("    return;\n}\n\n"); break;
                }
                return sb.ToString();
            }

            public string File(int blocks)
            {
                var sb = new StringBuilder();
                if (_host == HostLanguage.CSharp) sb.Append("class Generated\n{\n");
                for (int i = 0; i < blocks; i++) sb.Append(Block());
                if (_host == HostLanguage.CSharp) sb.Append("}\n");
                return sb.ToString();
            }
        }
    }
}
