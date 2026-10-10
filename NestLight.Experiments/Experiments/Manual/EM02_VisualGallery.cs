using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NestLight.Common;
using NestLight.Detection;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    /// <summary>
    /// A manual experiment: it has no criterion. For every host x embedded language x marker x interpolation combination it writes
    /// random snippets (seeded) inside the host, runs the real highlighter over them and paints the result in HTML, with
    /// the colors of an editor, to be opened in any browser. The same snippets, with the marker removed, run again with the automatic
    /// detection on, to see what it recognizes. A person or an AI agent looks at the pages and judges the colors; the findings file lists the
    /// cases worth looking at first.
    /// </summary>
    internal static class EM02_VisualGallery
    {
        public const string Id = "EM02";
        public const string Title = "Visual gallery of random snippets";

        public static int Run(string outDir, Func<Combination, bool> filter, int samples = 3)
        {
            Directory.CreateDirectory(outDir);
            var pages = new List<Page>();
            var findings = new List<string>();
            IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();

            foreach (var group in CombinationGenerator.Applicable().Where(c => filter == null || filter(c)).GroupBy(c => new { c.Host, c.Language }))
            {
                var page = new Page { Host = group.Key.Host, Language = group.Key.Language };
                foreach (Combination c in group)
                {
                    for (int seed = 1; seed <= samples; seed++)
                    {
                        string text = CombinationGenerator.Generate(c, 1, RandomSnippets.For(c.Language, seed));
                        Card card = Render(c.Host, text, null, languages);
                        card.Title = c.Marker + (c.Interpolated ? ", interpolated" : "") + ", seed " + seed;
                        page.Marked.Add(card);
                        page.Strings += card.Strings;
                        page.Tokens += card.Tokens;
                        string problem = Problem(card, c.Language);
                        if (problem != null) { card.Problem = problem; findings.Add("- " + c.Host + "/" + c.Language + " (" + card.Title + "): " + problem); }
                    }
                }
                // the same snippets with no marker, the detection on: the first marker style of each interpolation, one per seed
                foreach (Combination c in group.Where(x => !x.Interpolated && x.Marker == MarkerStyle.Id))
                {
                    var options = new DetectionOptions();
                    options.Set(true, DetectionOptions.Available);
                    for (int seed = 1; seed <= samples; seed++)
                    {
                        string text = Unmarked(CombinationGenerator.Generate(c, 1, RandomSnippets.For(c.Language, seed)), c);
                        Card card = Render(c.Host, text, options, languages);
                        card.Title = "no marker, seed " + seed;
                        card.Expected = c.Language;
                        page.Detected.Add(card);
                        page.AutoTotal++;
                        if (card.Languages.Contains(Canonical(languages, c.Language))) page.AutoHits++;
                    }
                }
                pages.Add(page);
                string path = Path.Combine(outDir, CombinationGenerator.Folder(page.Host), page.Language + ".html");
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, PageHtml(page), new UTF8Encoding(false));
            }

            File.WriteAllText(Path.Combine(outDir, "index.html"), IndexHtml(pages), new UTF8Encoding(false));
            File.WriteAllText(Path.Combine(outDir, "FINDINGS.md"), "# " + Id + ": cases worth looking at first\n\n"
                + (findings.Count == 0 ? "None: every marked snippet gave one string with tokens.\n" : string.Join("\n", findings) + "\n"), new UTF8Encoding(false));
            Console.WriteLine(pages.Count + " pages in " + outDir + " (open index.html), " + findings.Count + " cases flagged");
            return pages.Count == 0 ? 2 : 0;
        }

        private static string Canonical(IEmbeddedLanguageRegistry languages, string id)
        {
            return languages.Find(id).Ids[0];
        }

        private static string Problem(Card card, string language)
        {
            if (card.Strings != 1) return card.Strings + " strings found, 1 expected";
            if (card.Tokens == 0) return "the string got no token";
            return null;
        }

        /// <summary>The source without the marker: the tag, the bare id or the <c>language=</c> comment.</summary>
        private static string Unmarked(string text, Combination c)
        {
            text = Regex.Replace(text, @"^(//|#) (language=)?" + c.Language + "\n", "", RegexOptions.Multiline);
            text = text.Replace("/* " + c.Language + " */ ", "");
            return text;
        }

        // ---- running the plugin -----------------------------------------------------------------------------

        private sealed class Card
        {
            public string Title, Html, Source, Problem, Expected;
            public int Strings, Tokens;
            public List<string> Languages = new List<string>();
            public List<string> Roles = new List<string>();
        }

        private sealed class Page
        {
            public HostLanguage Host;
            public string Language;
            public List<Card> Marked = new List<Card>(), Detected = new List<Card>();
            public int Strings, Tokens, AutoHits, AutoTotal;
        }

        private static Card Render(HostLanguage host, string text, DetectionOptions detection, IEmbeddedLanguageRegistry languages)
        {
            IHostScanner scanner = NestLightComposition.CreateScanner(host, languages, detection);
            IReadOnlyList<EmbeddedString> strings = scanner.Scan(text);
            IReadOnlyList<Token> tokens = NestLightComposition.CreateHighlighter(host, detection).Highlight(text);

            // a class for each character: -1 host code, -2 inside an embedded string with no token, >= 0 the index of the role
            var roles = tokens.Select(t => t.Type).Distinct().OrderBy(r => r, StringComparer.Ordinal).ToList();
            var kind = new int[text.Length];
            for (int i = 0; i < kind.Length; i++) kind[i] = -1;
            foreach (EmbeddedString s in strings)
                for (int i = Math.Max(0, s.Start); i < Math.Min(s.End, text.Length); i++) kind[i] = char.IsWhiteSpace(text[i]) ? -1 : -2; // blanks with no token are not a gap
            foreach (Token t in tokens)
                for (int i = Math.Max(0, t.Start); i < Math.Min(t.End, text.Length); i++) kind[i] = roles.IndexOf(t.Type);

            var sb = new StringBuilder();
            int at = 0;
            while (at < text.Length)
            {
                int end = at;
                while (end < text.Length && kind[end] == kind[at]) end++;
                string piece = Esc(text.Substring(at, end - at));
                if (kind[at] == -1) sb.Append(piece);
                else if (kind[at] == -2) sb.Append("<span class=\"plain\" title=\"inside the string, no token\">").Append(piece).Append("</span>");
                else sb.Append("<span class=\"").Append(RoleClass(roles[kind[at]])).Append("\" title=\"").Append(Esc(roles[kind[at]])).Append("\">").Append(piece).Append("</span>");
                at = end;
            }
            return new Card
            {
                Html = sb.ToString(),
                Source = text,
                Strings = strings.Count,
                Tokens = tokens.Count,
                Roles = roles,
                Languages = strings.Select(s => s.EmbeddedLanguageId).Distinct().ToList()
            };
        }

        /// <summary>A role is painted by what it is, not by its language, so the same kind of piece has the same color everywhere (the editor does the same by default).</summary>
        internal static string RoleClass(string type)
        {
            string t = type.ToLowerInvariant();
            if (t.Contains("comment")) return "c-comment";
            if (t.Contains("expression")) return "c-expr";
            if (t.Contains("number") || t.Contains("unit") || t.Contains("literal")) return "c-number";
            if (t.Contains("string") || t.Contains("value") || t.Contains("cdata") || t.Contains("code")) return "c-string";
            if (t.Contains("punct") || t.Contains("delimiter") || t.Contains("operator") || t.Contains("regex.") && (t.Contains("anchor") || t.Contains("quantifier"))) return "c-punct";
            if (t.Contains("keyword") || t.EndsWith(".tag") || t.Contains("operation") || t.Contains("atrule") || t.Contains("heading") || t.Contains("directive") || t.Contains("important") || t.Contains("strong")) return "c-keyword";
            if (t.Contains("function") || t.Contains(".type") || t.Contains("class") || t.Contains("builtin") || t.Contains("emphasis")) return "c-type";
            if (t.Contains("attribute") || t.Contains("property") || t.Contains("key") || t.Contains("field") || t.Contains("argument") || t.Contains("variable")) return "c-attr";
            return "c-other";
        }

        // ---- the pages --------------------------------------------------------------------------------------

        private const string Style = @"
:root{--bg:#1e1e1e;--fg:#d4d4d4;--card:#252526;--line:#3c3c3c;--muted:#8a8a8a;--bad:#f48771;
--c-comment:#6a9955;--c-expr:#c586c0;--c-number:#b5cea8;--c-string:#ce9178;--c-punct:#9a9a9a;--c-keyword:#569cd6;--c-type:#4ec9b0;--c-attr:#9cdcfe;--c-other:#dcdcaa}
:root[data-theme=light]{--bg:#fff;--fg:#222;--card:#f6f6f6;--line:#ddd;--muted:#666;--bad:#c0392b;
--c-comment:#008000;--c-expr:#af00db;--c-number:#098658;--c-string:#a31515;--c-punct:#666;--c-keyword:#0000ff;--c-type:#267f99;--c-attr:#001080;--c-other:#795e26}
body{font:14px/1.5 system-ui,sans-serif;margin:16px;background:var(--bg);color:var(--fg)}a{color:var(--c-keyword)}
h1,h2{font-weight:600}h2{margin-top:28px;border-bottom:1px solid var(--line)}
.grid{display:grid;grid-template-columns:repeat(auto-fill,minmax(420px,1fr));gap:12px}
.card{background:var(--card);border:1px solid var(--line);border-radius:6px;padding:8px 10px;min-width:0}.card.bad{border-color:var(--bad)}
.card h3{margin:0 0 6px;font-size:12px;color:var(--muted);font-weight:500}.card .note{color:var(--bad);font-size:12px}
pre{font:13px/1.45 Consolas,Menlo,monospace;margin:0;overflow:auto;white-space:pre;tab-size:4}
.plain{background:rgba(128,128,128,.22);border-radius:2px}
.c-comment{color:var(--c-comment);font-style:italic}.c-expr{color:var(--c-expr);background:rgba(197,134,192,.15)}.c-number{color:var(--c-number)}
.c-string{color:var(--c-string)}.c-punct{color:var(--c-punct)}.c-keyword{color:var(--c-keyword)}.c-type{color:var(--c-type)}.c-attr{color:var(--c-attr)}.c-other{color:var(--c-other)}
.legend{font-size:12px;color:var(--muted)}.legend span{margin-right:10px}
button{background:var(--card);color:var(--fg);border:1px solid var(--line);border-radius:4px;padding:3px 10px;cursor:pointer}
table{border-collapse:collapse}td,th{border:1px solid var(--line);padding:4px 10px;text-align:left}td.num{text-align:right}
details{margin-top:6px;font-size:12px;color:var(--muted)}
";

        private const string Script = "<script>(function(){var r=document.documentElement,k='nestlight-gallery-theme';try{var s=localStorage.getItem(k);if(s)r.dataset.theme=s}catch(e){}"
            + "document.getElementById('theme').onclick=function(){r.dataset.theme=r.dataset.theme==='light'?'dark':'light';try{localStorage.setItem(k,r.dataset.theme)}catch(e){}}})()</script>";

        private static string Head(string title, string up)
        {
            return "<!doctype html><html lang=\"en\" data-theme=\"dark\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>" + Esc(title) + "</title><style>" + Style + "</style></head><body>"
                + "<p>" + (up != null ? "<a href=\"" + up + "\">&larr; index</a> " : "") + "<button id=\"theme\">light / dark</button></p>";
        }

        private static string PageHtml(Page page)
        {
            string title = page.Host + " / " + page.Language;
            var sb = new StringBuilder(Head(title, "../index.html"));
            sb.Append("<h1>").Append(Esc(title)).Append("</h1>");
            sb.Append("<p class=\"legend\">Painted by what each piece is. <span class=\"plain\">gray background</span> = inside the string, no token (plain text, or a gap). Hover a piece to see its token type. ")
              .Append("<span class=\"c-comment\">comment</span><span class=\"c-keyword\">keyword / tag</span><span class=\"c-attr\">attribute / key</span><span class=\"c-string\">string / value</span><span class=\"c-number\">number</span><span class=\"c-type\">type / function</span><span class=\"c-punct\">punctuation</span><span class=\"c-expr\">interpolation</span></p>");
            sb.Append("<h2>With the string marked</h2><div class=\"grid\">");
            foreach (Card c in page.Marked) sb.Append(CardHtml(c));
            sb.Append("</div>");
            if (page.Detected.Count > 0)
            {
                sb.Append("<h2>With no marker, automatic detection on (")
                  .Append(DetectionOptions.Available.Contains(page.Language) ? page.AutoHits + " of " + page.AutoTotal + " recognized as " + Esc(page.Language) : "the detector has no rule for " + Esc(page.Language) + ": nothing is expected")
                  .Append(")</h2><div class=\"grid\">");
                foreach (Card c in page.Detected) sb.Append(CardHtml(c));
                sb.Append("</div>");
            }
            return sb.Append(Script).Append("</body></html>").ToString();
        }

        private static string CardHtml(Card c)
        {
            var sb = new StringBuilder("<div class=\"card" + (c.Problem != null ? " bad" : "") + "\"><h3>" + Esc(c.Title) + " &middot; " + c.Strings + " string(s), " + c.Tokens + " token(s)");
            if (c.Expected != null) sb.Append(" &middot; detected: ").Append(c.Languages.Count == 0 ? "<b>nothing</b>" : Esc(string.Join(", ", c.Languages)));
            sb.Append("</h3>");
            if (c.Problem != null) sb.Append("<div class=\"note\">").Append(Esc(c.Problem)).Append("</div>");
            sb.Append("<pre>").Append(c.Html).Append("</pre>");
            return sb.Append("</div>").ToString();
        }

        private static string IndexHtml(List<Page> pages)
        {
            var sb = new StringBuilder(Head(Id + " gallery", null));
            sb.Append("<h1>").Append(Id).Append(": ").Append(Esc(Title)).Append("</h1>");
            sb.Append("<p>One page per host and language. Each has random snippets in every way to mark the string, and the same snippets with no marker and the automatic detection on. The snippets come from a seed, so the same run can be repeated.</p>");
            sb.Append("<table><tr><th>Host</th><th>Language</th><th>Strings</th><th>Tokens</th><th>Detected with no marker</th></tr>");
            foreach (Page p in pages.OrderBy(p => p.Host).ThenBy(p => p.Language, StringComparer.Ordinal))
                sb.Append("<tr><td>").Append(p.Host).Append("</td><td><a href=\"").Append(CombinationGenerator.Folder(p.Host)).Append('/').Append(p.Language).Append(".html\">").Append(p.Language).Append("</a></td><td class=\"num\">")
                  .Append(p.Strings).Append("</td><td class=\"num\">").Append(p.Tokens).Append("</td><td class=\"num\">").Append(DetectionOptions.Available.Contains(p.Language) ? p.AutoHits + " / " + p.AutoTotal : "no detector").Append("</td></tr>");
            return sb.Append("</table>").Append(Script).Append("</body></html>").ToString();
        }

        private static string Esc(string s)
        {
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }
    }
}
