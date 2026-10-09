using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    /// <summary>
    /// Types words of hand-written files, with a prefix of 0 to 3 letters (or with a mistake), asks the completion of the plugin (every
    /// feature it runs with) and writes the input and the top of the list as JSON, for a person to read and judge.
    /// </summary>
    internal static class ReviewRunner
    {
        public const int PerLanguage = 100;
        public const int Top = 20;

        private sealed class Occurrence
        {
            public int Doc, Start;
            public string Word;
            public EmbeddedString Owner;
        }

        public static void Run(string outDir)
        {
            Directory.CreateDirectory(outDir);
            var json = new StringBuilder("{\n");
            bool firstLanguage = true;
            foreach (string language in ReviewDocuments.Languages)
            {
                string[] documents = ReviewDocuments.ByLanguage[language];
                if (!firstLanguage) json.Append(",\n");
                firstLanguage = false;
                json.Append("  ").Append(Quote(language)).Append(": {\n    \"documents\": [");
                json.Append(string.Join(", ", documents.Select(d => Quote(d.Trim('\r', '\n')))));
                json.Append("],\n    \"cases\": [\n");

                List<string> cases = Cases(language, documents);
                json.Append(string.Join(",\n", cases));
                json.Append("\n    ]\n  }");
                Console.WriteLine(language + ": " + cases.Count + " cases");
            }
            json.Append("\n}\n");
            File.WriteAllText(Path.Combine(outDir, "cases.json"), json.ToString(), new UTF8Encoding(false));
        }

        /// <summary>Writes the generated corpus (all the documents of each language) and tells how many embedded strings the scanner finds in it.</summary>
        public static void DumpCorpus(string outDir)
        {
            Directory.CreateDirectory(outDir);
            IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
            IHostScanner scanner = NestLightComposition.CreateScanner(HostLanguage.JavaScript, languages);
            foreach (string language in RealisticCorpus.Languages)
            {
                var documents = RealisticCorpus.Documents(language, RealisticCorpus.SnippetsPerLanguage, 1);
                int strings = 0, wrong = 0;
                var text = new StringBuilder();
                for (int i = 0; i < documents.Count; i++)
                {
                    foreach (EmbeddedString s in scanner.Scan(documents[i].Text))
                    {
                        strings++;
                        if (!Vocabularies.SameLanguage(s.EmbeddedLanguageId, language) && !(language == "html" && s.EmbeddedLanguageId == "svg")) wrong++;
                    }
                    text.Append("// ---- ").Append(language).Append(" file ").Append(i + 1).Append(documents[i].Train ? " (train)" : " (test)").Append('\n').Append(documents[i].Text).Append('\n');
                }
                File.WriteAllText(Path.Combine(outDir, language + ".txt"), text.ToString(), new UTF8Encoding(false));
                Console.WriteLine(language + ": " + documents.Count + " files, " + documents.Sum(x => x.Snippets) + " snippets, " + strings + " strings found, " + wrong + " of another language, " + text.Length / 1024 + " KB");
            }
        }

        /// <summary>Writes the order of the keywords by use, learned from the generated corpus, as the C# file the plugin compiles.</summary>
        public static void WritePriors(string path)
        {
            var sb = new StringBuilder();
            sb.Append("using System.Collections.Generic;\n\nnamespace NestLight.Completion\n{\n");
            sb.Append("    /// <summary>\n    /// The keywords of each language from the most used to the least used, learned from the corpus of generated code of the experiments\n");
            sb.Append("    /// (NestLight.Experiments, <c>--priors</c>): a starting point, not what real projects use. Only the first ones matter: the rest are alphabetical.\n    /// </summary>\n");
            sb.Append("    internal static class KeywordUse\n    {\n        public static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> Default = new Dictionary<string, IReadOnlyList<string>>(System.StringComparer.OrdinalIgnoreCase)\n        {\n");
            foreach (string language in RealisticCorpus.Languages)
            {
                var documents = RealisticCorpus.Documents(language, RealisticCorpus.SnippetsPerLanguage, 1);
                var priors = CorpusProbes.Priors(documents, language).Take(80).ToList();
                sb.Append("            { \"").Append(language).Append("\", new[] { ").Append(string.Join(", ", priors.Select(w => "\"" + w + "\""))).Append(" } },\n");
                if (language == "html") sb.Append("            { \"svg\", new[] { ").Append(string.Join(", ", priors.Select(w => "\"" + w + "\""))).Append(" } },\n");
            }
            sb.Append("        };\n    }\n}\n");
            File.WriteAllText(path, sb.ToString(), new UTF8Encoding(false));
            Console.WriteLine("Wrote " + path);
        }

        private static List<string> Cases(string language, string[] documents)
        {
            IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
            IHostScanner scanner = NestLightComposition.CreateScanner(HostLanguage.JavaScript, languages);

            var occurrences = new List<Occurrence>();
            for (int d = 0; d < documents.Length; d++)
            {
                string text = documents[d].Trim('\r', '\n');
                foreach (EmbeddedString s in scanner.Scan(text))
                {
                    if (!Vocabularies.SameLanguage(s.EmbeddedLanguageId, language) && !(language == "html" && s.EmbeddedLanguageId == "svg")) continue;
                    int i = s.Start, end = Math.Min(s.End, text.Length);
                    bool dash = Vocabularies.IsExtraWordChar(language, '-');
                    while (i < end)
                    {
                        if (!(char.IsLetter(text[i]) || text[i] == '_')) { i++; continue; }
                        int start = i;
                        while (i < end && (char.IsLetterOrDigit(text[i]) || text[i] == '_' || (dash && text[i] == '-'))) i++;
                        bool inInterpolation = s.Interpolations.Any(x => x.Start <= start && start < x.End);
                        char before = start > 0 ? text[start - 1] : ' ';
                        if (inInterpolation || before == '#' || before == '$' || before == '@' && language != "wgsl" || char.IsDigit(before) || before == '.' && start > 1 && char.IsDigit(text[start - 2]) || i - start < 2) continue;
                        occurrences.Add(new Occurrence { Doc = d, Start = start, Word = text.Substring(start, i - start), Owner = s });
                    }
                }
            }

            // spread evenly over all the occurrences; the prefix cycles through 1, 2, 3 and 0 letters, and every tenth case has a mistake
            var picked = new List<string>();
            double step = Math.Max(1.0, occurrences.Count / (double)PerLanguage);
            for (int n = 0; n < PerLanguage; n++)
            {
                Occurrence o = occurrences[(int)((n * step) % occurrences.Count)];
                int[] cycle = { 1, 2, 3, 0 };
                int prefix = Math.Min(cycle[n % 4], o.Word.Length - 1);
                bool typo = n % 10 == 9 && o.Word.Length >= 5;
                picked.Add(OneCase(language, documents, n + 1, o, prefix, typo));
            }
            return picked;
        }

        private static string OneCase(string language, string[] documents, int id, Occurrence o, int prefix, bool typo)
        {
            string original = documents[o.Doc].Trim('\r', '\n');
            string typed;
            string category;
            if (typo)
            {
                var chars = o.Word.Substring(0, Math.Min(o.Word.Length - 1, 6)).ToCharArray();
                if (id % 20 < 10) { char t = chars[1]; chars[1] = chars[2]; chars[2] = t; category = "mistake: two letters swapped"; }
                else { typed = new string(chars); chars = typed.Remove(2, 1).ToCharArray(); category = "mistake: a letter missing"; }
                typed = new string(chars);
            }
            else
            {
                typed = o.Word.Substring(0, prefix);
                category = prefix == 0 ? "explicit request (Ctrl+Space), nothing typed" : "typing: " + prefix + " letter" + (prefix == 1 ? "" : "s");
            }
            string text = original.Remove(o.Start, o.Word.Length).Insert(o.Start, typed);
            int caret = o.Start + typed.Length;

            BufferAnalysis analysis = NestLightComposition.CreateForBuffer(HostLanguage.JavaScript);
            CompletionSite site = analysis.Completion.Locate(text, caret);
            var top = new List<string>();
            int rank = 0;
            string place = "(none)";
            int total = 0;
            if (site != null)
            {
                Position position = Positions.At(text, site);
                place = position == null ? "(no rule)" : position.Name;
                IReadOnlyList<Suggestion> items = analysis.Completion.Suggest(text, site);
                total = items.Count;
                for (int i = 0; i < items.Count; i++)
                    if (rank == 0 && string.Equals(items[i].Text, o.Word, StringComparison.OrdinalIgnoreCase)) rank = i + 1;
                foreach (Suggestion s in items.Take(Top))
                    top.Add("{\"text\":" + Quote(s.Text) + ",\"kind\":" + Quote(s.Distance > 0 ? "similar" : s.Kind == SuggestionKind.Keyword ? "keyword" : "word") + "}");
            }

            // whether anything could offer the word: it is a keyword of the language, or it is written elsewhere in the file
            bool reachable = Vocabularies.Find(language, o.Word) != null
                || System.Text.RegularExpressions.Regex.Matches(text, "(?<![\\p{L}\\p{N}_-])" + System.Text.RegularExpressions.Regex.Escape(o.Word) + "(?![\\p{L}\\p{N}_-])", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                    .Cast<System.Text.RegularExpressions.Match>().Any(m => m.Index != o.Start);

            // the code of the string the caret is in, with the caret marked
            EmbeddedString owner = null;
            foreach (EmbeddedString s in NestLightComposition.CreateScanner(HostLanguage.JavaScript, NestLightComposition.CreateEmbeddedLanguages()).Scan(text))
                if (s.Start <= caret && caret <= Math.Min(s.End, text.Length)) owner = s;
            string shown = owner == null ? text : text.Substring(owner.Start, Math.Min(owner.End, text.Length) - owner.Start);
            int markAt = owner == null ? caret : caret - owner.Start;
            shown = shown.Insert(markAt, "▮");
            int line = text.Take(caret).Count(c => c == '\n') + 1;

            var sb = new StringBuilder();
            sb.Append("      {\"id\":").Append(id)
              .Append(",\"doc\":").Append(o.Doc + 1)
              .Append(",\"line\":").Append(line)
              .Append(",\"category\":").Append(Quote(category))
              .Append(",\"typed\":").Append(Quote(typed))
              .Append(",\"intended\":").Append(Quote(o.Word))
              .Append(",\"place\":").Append(Quote(place))
              .Append(",\"input\":").Append(Quote(shown))
              .Append(",\"reachable\":").Append(reachable ? "true" : "false")
              .Append(",\"total\":").Append(total)
              .Append(",\"rank\":").Append(rank)
              .Append(",\"top\":[").Append(string.Join(",", top)).Append("]}");
            return sb.ToString();
        }

        private static string Quote(string s)
        {
            var sb = new StringBuilder("\"");
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20) sb.Append("\\u").Append(((int)c).ToString("x4"));
                        else sb.Append(c);
                        break;
                }
            }
            return sb.Append('"').ToString();
        }
    }
}
