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
    /// A manual experiment: it has no criterion and decides nothing. It generates the code of every host x embedded language
    /// combination, runs the plugin over it and writes, for each combination, a file with everything the plugin did (the source,
    /// each token with its type, the words of the string that got no color, the completion at the words typed with a prefix, and
    /// the carets that must get nothing). A person or an AI agent reads the files one by one, without Visual Studio, and judges each case; the index lists
    /// what is worth reading first. "Manual" is for whoever reads: running it is one command.
    /// </summary>
    internal static class CombinationReview
    {
        public const string Id = "EM01";
        public const string Title = "Review of every host with every embedded language";

        private const int Top = 10;
        private const int MaxWords = 8;

        public static int Run(string outDir, Func<Combination, bool> filter)
        {
            Directory.CreateDirectory(outDir);
            var index = new StringBuilder();
            index.AppendLine("# " + Id + ": " + Title).AppendLine();
            index.AppendLine("One file per combination: read each and judge the colors and the completion. `Gaps` = words inside the string with no token; `Misses` = completion cases where the intended word is not in the top " + Top + ".").AppendLine();
            index.AppendLine("| Combination | Strings | Tokens | Gaps | Misses | Wrong sites |").AppendLine("|---|---|---|---|---|---|");

            int files = 0;
            foreach (Combination c in CombinationGenerator.Applicable())
            {
                if (filter != null && !filter(c)) continue;
                var summary = new Summary();
                string html;
                string report = Review(c, summary, out html);
                string rel = c.FileName.Replace(".js", ".md").Replace(".cs", ".md").Replace(".py", ".md").Replace(".cpp", ".md");
                string path = Path.Combine(outDir, rel.Replace('/', Path.DirectorySeparatorChar));
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, report, new UTF8Encoding(false));
                File.WriteAllText(Path.ChangeExtension(path, ".html"), html, new UTF8Encoding(false));
                index.AppendLine("| [" + c.Name + "](" + rel + ") | " + summary.Strings + " | " + summary.Tokens + " | " + summary.Gaps + " | " + summary.Misses + " | " + summary.WrongSites + " |");
                files++;
            }
            File.WriteAllText(Path.Combine(outDir, "INDEX.md"), index.ToString(), new UTF8Encoding(false));
            Console.WriteLine(files + " combinations reviewed in " + outDir);
            return files == 0 ? 2 : 0;
        }

        private sealed class Summary { public int Strings, Tokens, Gaps, Misses, WrongSites; }

        private static string Review(Combination c, Summary summary, out string html)
        {
            string text = CombinationGenerator.Generate(c, 1);
            IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
            IHostScanner scanner = NestLightComposition.CreateScanner(c.Host, languages);
            IReadOnlyList<EmbeddedString> strings = scanner.Scan(text);
            IReadOnlyList<Token> tokens = new HighlightEngine(scanner, languages).Highlight(text);
            summary.Strings = strings.Count;
            summary.Tokens = tokens.Count;

            var sb = new StringBuilder();
            sb.AppendLine("# " + c.Name).AppendLine();
            sb.AppendLine("## Source").AppendLine().AppendLine("```").Append(text).AppendLine("```").AppendLine();
            sb.AppendLine("## Final view").AppendLine();
            sb.AppendLine("What the editor would show, as text: every token is written `⟦text|role⟧`, so a reader sees the color each piece gets. The same view, painted, is in [" + Path.GetFileNameWithoutExtension(c.FileName) + ".html](" + Path.GetFileNameWithoutExtension(c.FileName) + ".html).").AppendLine();
            sb.AppendLine("```").Append(Annotate(text, tokens)).AppendLine("```").AppendLine();
            html = Paint(c, text, tokens);

            sb.AppendLine("## Strings found").AppendLine();
            foreach (EmbeddedString s in strings)
                sb.AppendLine("- language `" + s.EmbeddedLanguageId + "`, " + Where(text, s.Start) + " to " + Where(text, s.End) + ", " + s.Interpolations.Count + " interpolation(s)"
                    + string.Concat(s.Interpolations.Select(i => " `" + Cut(text, i.Start, i.End) + "`")));
            sb.AppendLine();

            sb.AppendLine("## Tokens").AppendLine().AppendLine("| Position | Text | Type |").AppendLine("|---|---|---|");
            foreach (Token t in tokens)
                sb.AppendLine("| " + Where(text, t.Start) + " | `" + Cut(text, t.Start, t.End).Replace("|", "\\|") + "` | " + t.Type + " |");
            sb.AppendLine();

            // words inside the string that no token covers: plain text, or a gap in the tokenizer
            sb.AppendLine("## Words with no token").AppendLine();
            var gaps = new List<string>();
            foreach (EmbeddedString s in strings)
                foreach (Word w in Words(text, s))
                    if (!tokens.Any(t => t.Start <= w.Start && w.End <= t.End)) gaps.Add("`" + w.Text + "` at " + Where(text, w.Start));
            summary.Gaps = gaps.Count;
            sb.AppendLine(gaps.Count == 0 ? "None." : string.Join("\n", gaps.Select(g => "- " + g))).AppendLine();

            sb.AppendLine("## Completion").AppendLine();
            sb.AppendLine("Each word of the string typed with 1 and 2 letters (the rest of the word cut), the list the editor would get. `rank` is the place of the intended word; 0 = absent.").AppendLine();
            foreach (EmbeddedString s in strings)
            {
                var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (Word w in Words(text, s))
                {
                    if (!seen.Add(w.Text) || seen.Count > MaxWords) continue;
                    foreach (int prefix in new[] { 1, 2 })
                        if (w.Text.Length > prefix) Complete(c, text, w, prefix, sb, summary);
                }
            }
            sb.AppendLine();

            sb.AppendLine("## Carets that must get nothing").AppendLine();
            sb.AppendLine("The plugin must stay out of the host code and out of interpolations. Each caret is placed after the first letters of a word of the host code, and after the first letter inside each interpolation.").AppendLine();
            foreach (int caret in HostCarets(text, strings))
            {
                CompletionSite site = NestLightComposition.CreateForBuffer(c.Host).Completion.Locate(text, caret);
                bool wrong = site != null;
                if (wrong) summary.WrongSites++;
                sb.AppendLine("- " + Where(text, caret) + " after `" + Cut(text, Math.Max(0, caret - 6), caret).Replace("\n", "\\n") + "`: " + (wrong ? "**site in `" + site.EmbeddedLanguageId + "` (wrong)**" : "no site"));
            }
            return sb.ToString();
        }

        private static string Role(Token t)
        {
            return t.Type.StartsWith("template.") ? t.Type.Substring("template.".Length) : t.Type;
        }

        private static string Annotate(string text, IReadOnlyList<Token> tokens)
        {
            var sb = new StringBuilder();
            int at = 0;
            foreach (Token t in tokens.OrderBy(x => x.Start))
            {
                if (t.Start < at) continue;
                sb.Append(text, at, t.Start - at).Append("⟦").Append(Cut(text, t.Start, t.End)).Append('|').Append(Role(t)).Append("⟧");
                at = t.End;
            }
            return sb.Append(text, at, text.Length - at).ToString();
        }

        /// <summary>A page with the source painted by role, so that the result can be seen and not only read. Colors are only to tell the roles apart; the legend names them.</summary>
        private static string Paint(Combination c, string text, IReadOnlyList<Token> tokens)
        {
            var roles = tokens.Select(Role).Distinct().OrderBy(r => r).ToList();
            var sb = new StringBuilder("<!doctype html><html><head><meta charset=\"utf-8\"><title>" + c.Name + "</title><style>");
            sb.Append("body{font:14px/1.5 sans-serif;margin:16px;background:#fff;color:#222}pre{font:13px/1.5 Consolas,monospace;background:#f6f6f6;padding:12px;border-radius:6px;overflow:auto}");
            sb.Append(".legend span{margin-right:12px;white-space:nowrap}@media(prefers-color-scheme:dark){body{background:#1e1e1e;color:#ddd}pre{background:#2a2a2a}}");
            for (int i = 0; i < roles.Count; i++)
                sb.Append(".r" + i + "{color:hsl(" + (i * 137 % 360) + ",65%,42%);border-bottom:1px dotted currentColor}");
            sb.Append("</style></head><body><h1>" + Esc(c.Name) + "</h1><p class=\"legend\">");
            for (int i = 0; i < roles.Count; i++) sb.Append("<span class=\"r" + i + "\">" + Esc(roles[i]) + "</span>");
            sb.Append("</p><pre>");
            int at = 0;
            foreach (Token t in tokens.OrderBy(x => x.Start))
            {
                if (t.Start < at) continue;
                sb.Append(Esc(text.Substring(at, t.Start - at)));
                sb.Append("<span class=\"r" + roles.IndexOf(Role(t)) + "\" title=\"" + Esc(Role(t)) + "\">" + Esc(Cut(text, t.Start, t.End)) + "</span>");
                at = t.End;
            }
            sb.Append(Esc(text.Substring(at))).Append("</pre></body></html>");
            return sb.ToString();
        }

        private static string Esc(string s)
        {
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        private static void Complete(Combination c, string original, Word w, int prefix, StringBuilder sb, Summary summary)
        {
            string typed = w.Text.Substring(0, prefix);
            string text = original.Remove(w.Start, w.Text.Length).Insert(w.Start, typed);
            int caret = w.Start + typed.Length;
            BufferAnalysis analysis = NestLightComposition.CreateForBuffer(c.Host);
            CompletionSite site = analysis.Completion.Locate(text, caret);
            sb.Append("- `" + typed + "` for `" + w.Text + "` at " + Where(original, w.Start) + ": ");
            if (site == null) { sb.AppendLine("**no site**"); summary.WrongSites++; return; }
            Position position = CompletionLanguages.Default.Find(site.EmbeddedLanguageId).PositionAt(text, site);
            IReadOnlyList<Suggestion> items = analysis.Completion.Suggest(text, site);
            int rank = 0;
            for (int i = 0; i < items.Count && rank == 0; i++)
                if (string.Equals(items[i].Text, w.Text, StringComparison.OrdinalIgnoreCase)) rank = i + 1;
            if (rank == 0 || rank > Top) summary.Misses++;
            sb.AppendLine("site `" + site.EmbeddedLanguageId + "`, place " + (position == null ? "(no rule)" : position.Name) + ", rank " + rank + " of " + items.Count + ": "
                + string.Join(", ", items.Take(Top).Select(s => s.Text + (s.Distance > 0 ? "~" : s.Kind == SuggestionKind.Keyword ? "*" : ""))));
        }

        private sealed class Word { public string Text; public int Start; public int End { get { return Start + Text.Length; } } }

        /// <summary>The words (letters, digits and _, starting with a letter, at least 3 long) of the string outside its interpolations.</summary>
        private static IEnumerable<Word> Words(string text, EmbeddedString s)
        {
            int i = s.Start, end = Math.Min(s.End, text.Length);
            while (i < end)
            {
                if (!char.IsLetter(text[i])) { i++; continue; }
                int start = i;
                while (i < end && (char.IsLetterOrDigit(text[i]) || text[i] == '_')) i++;
                if (i - start >= 3 && !s.Interpolations.Any(x => x.Start <= start && start < x.End))
                    yield return new Word { Text = text.Substring(start, i - start), Start = start };
            }
        }

        private static IEnumerable<int> HostCarets(string text, IReadOnlyList<EmbeddedString> strings)
        {
            var carets = new List<int>();
            int i = 0;
            while (i < text.Length && carets.Count < 6)
            {
                if (!char.IsLetter(text[i])) { i++; continue; }
                int start = i;
                while (i < text.Length && char.IsLetterOrDigit(text[i])) i++;
                if (i - start >= 3 && !strings.Any(s => s.OuterStart <= start && start < s.OuterEnd)) carets.Add(start + 2);
            }
            foreach (EmbeddedString s in strings)
                foreach (Interpolation x in s.Interpolations)
                    if (x.InnerStart + 1 <= x.InnerEnd) carets.Add(x.InnerStart + 1);
            return carets;
        }

        private static string Where(string text, int offset)
        {
            int line = 1, last = -1;
            for (int i = 0; i < offset && i < text.Length; i++) if (text[i] == '\n') { line++; last = i; }
            return line + ":" + (Math.Min(offset, text.Length) - last);
        }

        private static string Cut(string text, int start, int end)
        {
            start = Math.Max(0, Math.Min(start, text.Length));
            end = Math.Max(start, Math.Min(end, text.Length));
            return text.Substring(start, end - start);
        }
    }
}
