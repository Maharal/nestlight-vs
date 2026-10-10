using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NestLight.Common;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    /// <summary>How the string says what language it holds.</summary>
    internal enum MarkerStyle
    {
        /// <summary>A tag that touches the backtick: <c>html`...`</c>. JavaScript only.</summary>
        Tag,
        /// <summary>A comment with the bare id: <c>// html</c>.</summary>
        Id,
        /// <summary>A comment with <c>language=id</c>.</summary>
        LanguageEquals
    }

    /// <summary>One point of the matrix: a host, an embedded language, a way to mark the string and whether it has an interpolation.</summary>
    internal sealed class Combination
    {
        public HostLanguage Host;
        public string Language;
        public MarkerStyle Marker;
        public bool Interpolated;
        /// <summary>Null when the plugin is expected to color this combination; otherwise why it is not (the combination is listed, not generated).</summary>
        public string NotApplicable;

        public bool Applicable { get { return NotApplicable == null; } }
        public string Name { get { return Host + "/" + Language + "/" + Marker + (Interpolated ? "/interpolated" : ""); } }
        public string FileName
        {
            get
            {
                return CombinationGenerator.Folder(Host) + "/" + Language + "-" + Marker.ToString().ToLowerInvariant()
                    + (Interpolated ? "-interpolated" : "") + CombinationGenerator.Extension(Host);
            }
        }
    }

    /// <summary>What the plugin has to find in a generated file.</summary>
    internal sealed class Expectation
    {
        public int Strings;
        public int Interpolations;
    }

    /// <summary>
    /// Generates source code for every combination of host and embedded language, so that an experiment (automatic or manual) can
    /// run on each of them without anyone writing the sample by hand. A file holds <c>repeat</c> copies of the same sample, each
    /// in its own variable, so the same generator makes a file to look at (repeat 1) and a file to measure (repeat 1,000).
    /// </summary>
    internal static class CombinationGenerator
    {
        private const string Name = "name";

        public static readonly HostLanguage[] Hosts = SyntheticCode.Hosts;

        public static string Folder(HostLanguage host)
        {
            return host.ToString().ToLowerInvariant();
        }

        public static string Extension(HostLanguage host)
        {
            switch (host)
            {
                case HostLanguage.JavaScript: return ".js";
                case HostLanguage.CSharp: return ".cs";
                case HostLanguage.Python: return ".py";
                default: return ".cpp";
            }
        }

        /// <summary>Every combination, the ones the plugin does not color included (with the reason).</summary>
        public static List<Combination> All()
        {
            var all = new List<Combination>();
            foreach (HostLanguage host in Hosts)
                foreach (string language in LanguageSamples.Languages)
                    foreach (MarkerStyle marker in Enum.GetValues(typeof(MarkerStyle)))
                        foreach (bool interpolated in new[] { false, true })
                            all.Add(new Combination { Host = host, Language = language, Marker = marker, Interpolated = interpolated, NotApplicable = Reason(host, language, marker, interpolated) });
            return all;
        }

        public static List<Combination> Applicable()
        {
            return All().FindAll(c => c.Applicable);
        }

        private static string Reason(HostLanguage host, string language, MarkerStyle marker, bool interpolated)
        {
            if (marker == MarkerStyle.Tag && host != HostLanguage.JavaScript) return "only JavaScript has tags";
            if (interpolated && host == HostLanguage.Cpp) return "C++ raw strings have no interpolation";
            if (host == HostLanguage.CSharp && (language == "json" || language == "regex")) return "json and regex in C# are left to Visual Studio";
            return null;
        }

        public static Expectation Expect(Combination c, int repeat)
        {
            return new Expectation { Strings = repeat, Interpolations = c.Interpolated ? repeat : 0 };
        }

        /// <summary>The source of a file with <paramref name="repeat"/> copies of the sample of the combination.</summary>
        public static string Generate(Combination c, int repeat)
        {
            return Generate(c, repeat, LanguageSamples.For(c.Language));
        }

        /// <summary>Like <see cref="Generate(Combination, int)"/>, with the sample given (it carries <see cref="LanguageSamples.Slot"/> where the interpolation goes).</summary>
        public static string Generate(Combination c, int repeat, string sample)
        {
            if (!c.Applicable) throw new ArgumentException(c.Name + " is not applicable: " + c.NotApplicable);
            var sb = new StringBuilder();
            for (int i = 0; i < repeat; i++)
            {
                switch (c.Host)
                {
                    case HostLanguage.JavaScript: JavaScript(sb, c, sample, i); break;
                    case HostLanguage.CSharp: CSharp(sb, c, sample, i); break;
                    case HostLanguage.Python: Python(sb, c, sample, i); break;
                    default: Cpp(sb, c, sample, i); break;
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }

        /// <summary>Writes one file per applicable combination under <paramref name="dir"/> and returns the combinations written.</summary>
        public static List<Combination> WriteAll(string dir, int repeat, Func<Combination, bool> filter = null)
        {
            var written = new List<Combination>();
            foreach (Combination c in Applicable())
            {
                if (filter != null && !filter(c)) continue;
                string path = Path.Combine(dir, c.FileName.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, Generate(c, repeat), new UTF8Encoding(false));
                written.Add(c);
            }
            return written;
        }

        // ---- the hosts --------------------------------------------------------------------------------------

        private static string Fill(string sample, string slot)
        {
            return sample.Replace(LanguageSamples.Slot, slot);
        }

        private static string Escape(string text, params string[][] pairs)
        {
            var sb = new StringBuilder();
            foreach (char ch in text)
            {
                string replacement = null;
                foreach (string[] p in pairs) if (p[0][0] == ch) { replacement = p[1]; break; }
                sb.Append(replacement ?? ch.ToString());
            }
            return sb.ToString();
        }

        private static void JavaScript(StringBuilder sb, Combination c, string sample, int i)
        {
            // the slot is split off before escaping: ${name} must not be escaped, the text around it must
            string[] parts = sample.Split(new[] { LanguageSamples.Slot }, StringSplitOptions.None);
            var body = new StringBuilder();
            for (int k = 0; k < parts.Length; k++)
            {
                if (k > 0) body.Append(c.Interpolated ? "${" + Name + "}" : Name);
                body.Append(Escape(parts[k], new[] { "\\", "\\\\" }, new[] { "`", "\\`" }));
            }
            string literal = "`" + body + "`";
            switch (c.Marker)
            {
                case MarkerStyle.Tag: sb.Append("const sample").Append(i).Append(" = ").Append(c.Language).Append(literal).Append(";\n"); break;
                case MarkerStyle.Id: sb.Append("const sample").Append(i).Append(" = /* ").Append(c.Language).Append(" */ ").Append(literal).Append(";\n"); break;
                default: sb.Append("// language=").Append(c.Language).Append("\nconst sample").Append(i).Append(" = ").Append(literal).Append(";\n"); break;
            }
        }

        private static void CSharp(StringBuilder sb, Combination c, string sample, int i)
        {
            sb.Append(c.Marker == MarkerStyle.Id ? "// " + c.Language : "// language=" + c.Language).Append('\n');
            // $$""" takes {{expr}} and leaves single braces alone, which CSS, GraphQL and the shaders are full of
            string text = Fill(sample, c.Interpolated ? "{{" + Name + "}}" : Name);
            string quotes = c.Interpolated ? "$$\"\"\"" : "\"\"\"";
            sb.Append("var sample").Append(i).Append(" = ").Append(quotes).Append('\n');
            foreach (string line in text.Split('\n')) sb.Append("    ").Append(line).Append('\n');
            sb.Append("    \"\"\";\n");
        }

        private static void Python(StringBuilder sb, Combination c, string sample, int i)
        {
            sb.Append(c.Marker == MarkerStyle.Id ? "# " + c.Language : "# language=" + c.Language).Append('\n');
            string[] parts = sample.Split(new[] { LanguageSamples.Slot }, StringSplitOptions.None);
            var body = new StringBuilder();
            for (int k = 0; k < parts.Length; k++)
            {
                if (k > 0) body.Append(c.Interpolated ? "{" + Name + "}" : Name);
                // an f-string doubles its literal braces
                body.Append(c.Interpolated ? Escape(parts[k], new[] { "{", "{{" }, new[] { "}", "}}" }) : parts[k]);
            }
            sb.Append("sample").Append(i).Append(" = ").Append(c.Interpolated ? "rf" : "r").Append("\"\"\"").Append(body).Append("\"\"\"\n");
        }

        private static void Cpp(StringBuilder sb, Combination c, string sample, int i)
        {
            sb.Append(c.Marker == MarkerStyle.Id ? "// " + c.Language : "// language=" + c.Language).Append('\n');
            sb.Append("auto sample").Append(i).Append(" = R\"x(").Append(Fill(sample, Name)).Append(")x\";\n");
        }
    }
}
