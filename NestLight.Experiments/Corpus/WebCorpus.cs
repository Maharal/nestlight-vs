using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NestLight.Experiments
{
    /// <summary>Style sheets and templates the way front-end code has them.</summary>
    internal static class WebCorpus
    {
        private static readonly string[] Blocks =
            { "card", "button", "navbar", "sidebar", "modal", "table", "form", "hero", "footer", "badge", "tooltip", "menu", "tabs", "list", "toolbar", "avatar", "banner", "dropdown", "pagination", "gallery" };
        private static readonly string[] Elements = { "title", "body", "header", "footer", "icon", "item", "label", "link", "content", "actions", "meta", "image" };
        private static readonly string[] Modifiers = { "primary", "secondary", "large", "small", "active", "disabled", "dark", "outline", "compact", "wide" };
        private static readonly string[] Colors = { "#fff", "#111", "#0070f3", "#e5e7eb", "#f3f4f6", "rgba(0, 0, 0, 0.5)", "var(--color-primary)", "var(--color-text)", "transparent", "currentColor", "hsl(220, 14%, 96%)" };
        private static readonly string[] Lengths = { "0", "4px", "8px", "12px", "16px", "24px", "32px", "1rem", "1.5rem", "2rem", "100%", "50%", "auto", "min(100%, 40rem)", "calc(100% - 2rem)" };

        // ---- CSS ------------------------------------------------------------------------------------------------------

        public static CorpusDocument Css(Dice d, int count)
        {
            var sb = new StringBuilder("import { css } from './styled';\n\n");
            for (int n = 0; n < count; n++)
            {
                string block = d.Weighted(Blocks);
                sb.Append("export const ").Append(block).Append("Styles").Append(n).Append(" = css`\n").Append(Indent(CssRules(d, block))).Append("\n`;\n\n");
            }
            return new CorpusDocument { Language = "css", Text = sb.ToString(), Snippets = count };
        }

        private static string Indent(string text) { return string.Join("\n", text.Split('\n').Select(l => l.Length == 0 ? l : "  " + l)); }

        private static string Declarations(Dice d, params string[] groups)
        {
            var lines = new List<string>();
            foreach (string group in groups) lines.AddRange(Group(d, group));
            return string.Join("\n", lines.Select(l => "  " + l));
        }

        private static IEnumerable<string> Group(Dice d, string kind)
        {
            switch (kind)
            {
                case "layout":
                    if (d.Chance(0.55))
                    {
                        yield return "display: flex;";
                        if (d.Chance(0.7)) yield return "flex-direction: " + d.Pick(new[] { "row", "column", "row-reverse" }) + ";";
                        if (d.Chance(0.7)) yield return "align-items: " + d.Pick(new[] { "center", "flex-start", "flex-end", "stretch", "baseline" }) + ";";
                        if (d.Chance(0.7)) yield return "justify-content: " + d.Pick(new[] { "space-between", "center", "flex-start", "flex-end", "space-around" }) + ";";
                        if (d.Chance(0.5)) yield return "gap: " + d.Pick(Lengths) + ";";
                        if (d.Chance(0.3)) yield return "flex-wrap: wrap;";
                    }
                    else
                    {
                        yield return "display: grid;";
                        yield return "grid-template-columns: " + d.Pick(new[] { "repeat(auto-fill, minmax(200px, 1fr))", "1fr 2fr", "repeat(3, 1fr)", "240px 1fr" }) + ";";
                        yield return "gap: " + d.Pick(Lengths) + ";";
                        if (d.Chance(0.3)) yield return "place-items: center;";
                    }
                    break;
                case "box":
                    yield return "padding: " + d.Pick(Lengths) + (d.Chance(0.4) ? " " + d.Pick(Lengths) : "") + ";";
                    if (d.Chance(0.6)) yield return "margin: " + d.Pick(Lengths) + (d.Chance(0.3) ? " auto" : "") + ";";
                    if (d.Chance(0.5)) yield return "border: 1px solid " + d.Pick(Colors) + ";";
                    if (d.Chance(0.6)) yield return "border-radius: " + d.Pick(new[] { "4px", "8px", "12px", "50%", "9999px" }) + ";";
                    if (d.Chance(0.4)) yield return "box-shadow: 0 " + d.Pick(new[] { "1px", "2px", "4px", "10px" }) + " " + d.Pick(new[] { "3px", "6px", "20px" }) + " rgba(0, 0, 0, 0.15);";
                    if (d.Chance(0.4)) yield return "width: " + d.Pick(Lengths) + ";";
                    if (d.Chance(0.3)) yield return "max-width: " + d.Pick(new[] { "40rem", "1200px", "100%", "320px" }) + ";";
                    if (d.Chance(0.2)) yield return "min-height: " + d.Pick(new[] { "100vh", "48px", "200px" }) + ";";
                    if (d.Chance(0.2)) yield return "overflow: " + d.Pick(new[] { "hidden", "auto", "scroll" }) + ";";
                    if (d.Chance(0.15)) yield return "box-sizing: border-box;";
                    break;
                case "paint":
                    yield return "background-color: " + d.Pick(Colors) + ";";
                    yield return "color: " + d.Pick(Colors) + ";";
                    if (d.Chance(0.2)) yield return "opacity: " + d.Pick(new[] { "0.5", "0.8", "1" }) + ";";
                    if (d.Chance(0.15)) yield return "background-image: linear-gradient(to bottom, " + d.Pick(Colors) + ", " + d.Pick(Colors) + ");";
                    break;
                case "text":
                    yield return "font-size: " + d.Pick(new[] { "0.875rem", "1rem", "1.25rem", "2rem", "14px", "18px" }) + ";";
                    if (d.Chance(0.6)) yield return "font-weight: " + d.Pick(new[] { "400", "500", "600", "700", "bold", "normal" }) + ";";
                    if (d.Chance(0.4)) yield return "line-height: " + d.Pick(new[] { "1.2", "1.5", "1.75", "24px" }) + ";";
                    if (d.Chance(0.3)) yield return "text-align: " + d.Pick(new[] { "center", "left", "right" }) + ";";
                    if (d.Chance(0.2)) yield return "text-transform: " + d.Pick(new[] { "uppercase", "capitalize", "none" }) + ";";
                    if (d.Chance(0.2)) yield return "letter-spacing: " + d.Pick(new[] { "0.05em", "0.1em", "-0.01em" }) + ";";
                    if (d.Chance(0.15)) yield return "text-decoration: " + d.Pick(new[] { "none", "underline" }) + ";";
                    if (d.Chance(0.12)) yield return "white-space: nowrap;";
                    if (d.Chance(0.12)) yield return "text-overflow: ellipsis;";
                    if (d.Chance(0.2)) yield return "font-family: " + d.Pick(new[] { "system-ui, sans-serif", "'Inter', sans-serif", "monospace", "var(--font-body)" }) + ";";
                    break;
                case "position":
                    yield return "position: " + d.Pick(new[] { "absolute", "relative", "fixed", "sticky" }) + ";";
                    if (d.Chance(0.6)) yield return "top: " + d.Pick(Lengths) + ";";
                    if (d.Chance(0.5)) yield return "left: " + d.Pick(Lengths) + ";";
                    if (d.Chance(0.3)) yield return "right: " + d.Pick(Lengths) + ";";
                    if (d.Chance(0.3)) yield return "bottom: " + d.Pick(Lengths) + ";";
                    if (d.Chance(0.5)) yield return "z-index: " + d.Pick(new[] { "1", "10", "100", "1000" }) + ";";
                    if (d.Chance(0.2)) yield return "transform: " + d.Pick(new[] { "translate(-50%, -50%)", "translateY(-2px)", "scale(1.05)", "rotate(45deg)" }) + ";";
                    break;
                case "motion":
                    yield return "transition: " + d.Pick(new[] { "all", "background-color", "opacity", "transform", "color", "box-shadow" }) + " " + d.Pick(new[] { "0.2s", "150ms", "0.3s", "0.5s" }) + " " + d.Pick(new[] { "ease", "ease-in-out", "ease-out", "linear" }) + ";";
                    if (d.Chance(0.4)) yield return "cursor: " + d.Pick(new[] { "pointer", "default", "not-allowed", "grab" }) + ";";
                    if (d.Chance(0.2)) yield return "pointer-events: " + d.Pick(new[] { "none", "auto" }) + ";";
                    if (d.Chance(0.15)) yield return "user-select: none;";
                    if (d.Chance(0.15)) yield return "will-change: transform;";
                    break;
            }
        }

        private static string CssRules(Dice d, string block)
        {
            var sb = new StringBuilder();
            string root = "." + block + (d.Chance(0.2) ? "--" + d.Pick(Modifiers) : "");
            var groups = new List<string> { "layout", "box", "paint", "text", "position", "motion" };
            sb.Append(root).Append(" {\n").Append(Declarations(d, d.Some(groups, d.Between(2, 4)).ToArray())).Append("\n}");
            for (int i = d.Between(1, 3); i > 0; i--)
            {
                switch (d.Next(8))
                {
                    case 0: sb.Append("\n\n").Append(root).Append(":hover {\n").Append(Declarations(d, "paint")).Append("\n}"); break;
                    case 1: sb.Append("\n\n").Append(root).Append(":focus-visible {\n  outline: 2px solid ").Append(d.Pick(Colors)).Append(";\n  outline-offset: 2px;\n}"); break;
                    case 2: sb.Append("\n\n").Append(root).Append(":disabled {\n  opacity: 0.5;\n  cursor: not-allowed;\n}"); break;
                    case 3: sb.Append("\n\n").Append(root).Append("__").Append(d.Pick(Elements)).Append(" {\n").Append(Declarations(d, "text", "box")).Append("\n}"); break;
                    case 4: sb.Append("\n\n@media (max-width: ").Append(d.Pick(new[] { "480px", "600px", "768px", "1024px" })).Append(") {\n  ").Append(root).Append(" {\n").Append(Indent(Declarations(d, "layout", "box"))).Append("\n  }\n}"); break;
                    case 5:
                        string name = d.Pick(new[] { "fade-in", "slide-up", "spin", "pulse", "shake" });
                        sb.Append("\n\n@keyframes ").Append(name).Append(" {\n  from { opacity: 0; transform: translateY(8px); }\n  to { opacity: 1; transform: translateY(0); }\n}\n\n").Append(root).Append(" {\n  animation: ").Append(name).Append(" ").Append(d.Pick(new[] { "0.3s", "1s", "200ms" })).Append(" ").Append(d.Pick(new[] { "ease-out", "linear", "ease-in-out" })).Append(d.Chance(0.3) ? " infinite" : "").Append(";\n}");
                        break;
                    case 6: sb.Append("\n\n").Append(root).Append("::").Append(d.Pick(new[] { "before", "after" })).Append(" {\n  content: '';\n  display: block;\n").Append(Declarations(d, "position")).Append("\n}"); break;
                    default: sb.Append("\n\n").Append(root).Append(" > ").Append(d.Pick(new[] { "a", "span", "li", "img", "p", "button" })).Append(" {\n").Append(Declarations(d, "text", "paint")).Append("\n}"); break;
                }
            }
            return sb.ToString();
        }

        // ---- HTML -----------------------------------------------------------------------------------------------------

        public static CorpusDocument Html(Dice d, int count)
        {
            var sb = new StringBuilder("import { html } from './template';\n\n");
            for (int n = 0; n < count; n++)
            {
                string block = d.Weighted(Blocks);
                sb.Append("export const ").Append(block).Append("View").Append(n).Append(" = (data) => html`\n").Append(Indent(HtmlBlock(d, block))).Append("\n`;\n\n");
            }
            return new CorpusDocument { Language = "html", Text = sb.ToString(), Snippets = count };
        }

        private static string Cls(Dice d, string block, string element = null)
        {
            if (d.Chance(0.35)) return d.Pick(new[] { "flex items-center gap-2", "p-4 rounded-lg shadow", "text-sm text-gray-500", "container mx-auto", "grid grid-cols-3 gap-4", "mt-2 font-bold", "w-full px-3 py-2 border" });
            return block + (element == null ? "" : "__" + element) + (d.Chance(0.25) ? " " + block + (element == null ? "" : "__" + element) + "--" + d.Pick(Modifiers) : "");
        }

        private static string HtmlBlock(Dice d, string block)
        {
            switch (d.Next(9))
            {
                case 0:
                    return "<form class=\"" + Cls(d, block) + "\" action=\"/" + block + "\" method=\"post\">\n"
                        + Fields(d, block) + "  <button type=\"submit\" class=\"btn btn-primary\"" + (d.Chance(0.2) ? " disabled" : "") + ">Save</button>\n</form>";
                case 1:
                    return "<table class=\"" + Cls(d, block) + "\">\n  <thead>\n    <tr><th scope=\"col\">Name</th><th scope=\"col\">Status</th><th scope=\"col\">Created</th></tr>\n  </thead>\n  <tbody>\n    ${data.rows.map(r => html`<tr><td>${r.name}</td><td>${r.status}</td><td>${r.created}</td></tr>`)}\n  </tbody>\n</table>";
                case 2:
                    return "<nav class=\"" + Cls(d, block) + "\" aria-label=\"" + d.Pick(new[] { "Main", "Breadcrumb", "Pagination" }) + "\">\n  <ul class=\"" + Cls(d, block, "list") + "\">\n"
                        + string.Join("\n", Enumerable.Range(0, d.Between(2, 4)).Select(i => "    <li class=\"" + Cls(d, block, "item") + "\"><a href=\"/" + d.Pick(new[] { "docs", "blog", "pricing", "about", "contact" }) + "\"" + (d.Chance(0.3) ? " class=\"active\"" : "") + ">" + d.Pick(new[] { "Docs", "Blog", "Pricing", "About", "Contact" }) + "</a></li>")) + "\n  </ul>\n</nav>";
                case 3:
                    return "<article class=\"" + Cls(d, block) + "\" id=\"" + block + "-" + d.Next(100) + "\">\n  <img class=\"" + Cls(d, block, "image") + "\" src=\"${data.image}\" alt=\"${data.title}\" width=\"320\" height=\"180\" loading=\"lazy\">\n  <h3 class=\"" + Cls(d, block, "title") + "\">${data.title}</h3>\n  <p class=\"" + Cls(d, block, "body") + "\">${data.text}</p>\n  <a href=\"${data.url}\" class=\"" + Cls(d, block, "link") + "\" target=\"_blank\" rel=\"noopener\">Read more</a>\n</article>";
                case 4:
                    return "<dialog class=\"" + Cls(d, block) + "\" id=\"" + block + "\" aria-labelledby=\"" + block + "-title\">\n  <h2 id=\"" + block + "-title\" class=\"" + Cls(d, block, "title") + "\">${data.title}</h2>\n  <div class=\"" + Cls(d, block, "content") + "\">${data.content}</div>\n  <div class=\"" + Cls(d, block, "actions") + "\">\n    <button type=\"button\" class=\"btn\" data-action=\"cancel\">Cancel</button>\n    <button type=\"button\" class=\"btn btn-primary\" data-action=\"confirm\">OK</button>\n  </div>\n</dialog>";
                case 5:
                    return "<section class=\"" + Cls(d, block) + "\" role=\"" + d.Pick(new[] { "region", "list", "banner", "navigation" }) + "\">\n  <header class=\"" + Cls(d, block, "header") + "\"><h2>${data.heading}</h2></header>\n  <ul>\n    ${data.items.map(i => html`<li class=\"" + Cls(d, block, "item") + "\">${i.label}</li>`)}\n  </ul>\n  <footer class=\"" + Cls(d, block, "footer") + "\"><small>${data.note}</small></footer>\n</section>";
                case 6:
                    return "<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 24 24\" width=\"24\" height=\"24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\">\n  <circle cx=\"12\" cy=\"12\" r=\"10\"/>\n  <path d=\"M12 6v6l4 2\"/>\n  <line x1=\"4\" y1=\"4\" x2=\"20\" y2=\"20\"/>\n</svg>";
                case 7:
                    return "<div class=\"" + Cls(d, block) + "\" data-id=\"${data.id}\" tabindex=\"0\" role=\"button\" aria-pressed=\"false\">\n  <span class=\"" + Cls(d, block, "icon") + "\" aria-hidden=\"true\"></span>\n  <span class=\"" + Cls(d, block, "label") + "\">${data.label}</span>\n  <select name=\"" + d.Pick(new[] { "country", "role", "size" }) + "\" class=\"" + Cls(d, block, "select") + "\">\n    <option value=\"\" selected>Choose</option>\n    <option value=\"a\">A</option>\n    <option value=\"b\">B</option>\n  </select>\n</div>";
                default:
                    return "<footer class=\"" + Cls(d, block) + "\">\n  <p class=\"" + Cls(d, block, "meta") + "\">&copy; ${data.year} ${data.company}</p>\n  <video src=\"${data.video}\" controls autoplay muted loop poster=\"${data.poster}\"></video>\n  <textarea name=\"message\" rows=\"3\" cols=\"40\" placeholder=\"Say something\" maxlength=\"200\"></textarea>\n</footer>";
            }
        }

        private static string Fields(Dice d, string block)
        {
            var sb = new StringBuilder();
            string[][] fields =
            {
                new[] { "email", "email", "Email" }, new[] { "password", "password", "Password" }, new[] { "name", "text", "Name" },
                new[] { "age", "number", "Age" }, new[] { "birthday", "date", "Birthday" }, new[] { "phone", "tel", "Phone" }, new[] { "website", "url", "Website" },
            };
            foreach (string[] f in d.Some(fields, d.Between(2, 4)))
                sb.Append("  <label for=\"" + f[0] + "\" class=\"" + Cls(d, block, "label") + "\">" + f[2] + "</label>\n  <input type=\"" + f[1] + "\" id=\"" + f[0] + "\" name=\"" + f[0] + "\" class=\"" + Cls(d, block, "input") + "\"" + (d.Chance(0.5) ? " required" : "") + (d.Chance(0.4) ? " placeholder=\"" + f[2] + "\"" : "") + (d.Chance(0.2) ? " autocomplete=\"" + f[0] + "\"" : "") + ">\n");
            return sb.ToString();
        }
    }
}
