using System;
using System.Collections.Generic;
using System.Linq;

namespace NestLight.Experiments
{
    /// <summary>
    /// A random snippet of each embedded language, from a seed: the same seed gives the same snippet, so a case that looks wrong
    /// can be written down by its seed and read again. Like <see cref="LanguageSamples"/>, a snippet carries <see cref="LanguageSamples.Slot"/>
    /// where the host puts an interpolation, and avoids triple quotes and <c>)x"</c> so that every host can carry it.
    /// </summary>
    internal static class RandomSnippets
    {
        private static readonly string[] Nouns = { "user", "order", "item", "invoice", "account", "product", "session", "report", "team", "event" };
        private static readonly string[] Fields = { "id", "name", "email", "total", "status", "created", "price", "count", "title", "kind" };
        private static readonly string[] Colors = { "red", "teal", "#336699", "#fa0", "rgb(10, 20, 30)", "hsl(200 50% 40%)", "currentColor" };

        public static string For(string language, int seed)
        {
            var d = new Dice(seed * 31 + language.Length * 7 + language[0]);
            switch (language)
            {
                case "html": return Html(d);
                case "css": return Css(d);
                case "sql": return Sql(d);
                case "json": return Json(d);
                case "graphql": return GraphQl(d);
                case "xml": return Xml(d);
                case "markdown": return Markdown(d);
                case "yaml": return Yaml(d);
                case "regex": return Regex(d);
                case "glsl": return Glsl(d);
                case "wgsl": return Wgsl(d);
                default: throw new ArgumentException("unknown language " + language);
            }
        }

        private static string Noun(Dice d) { return d.Pick(Nouns); }
        private static string Field(Dice d) { return d.Pick(Fields); }
        private const string Slot = LanguageSamples.Slot;

        private static string Html(Dice d)
        {
            string tag = d.Pick(new[] { "div", "section", "article", "nav", "ul", "form" });
            string child = tag == "ul" ? "li" : d.Pick(new[] { "p", "span", "a", "button" });
            var lines = new List<string> { "<" + tag + " class=\"" + Noun(d) + "-" + d.Pick(new[] { "box", "list", "card" }) + "\"" + (d.Chance(0.5) ? " id=\"" + Noun(d) + d.Next(9) + "\"" : "") + (d.Chance(0.3) ? " hidden" : "") + ">" };
            for (int i = 0; i < d.Between(2, 4); i++)
                lines.Add("  <" + child + " class=\"item\" data-" + Field(d) + "=\"" + d.Next(100) + "\"" + (d.Chance(0.3) ? " onclick=\"go()\"" : "") + ">" + (i == 0 ? Slot : Noun(d)) + "</" + child + ">");
            if (d.Chance(0.5)) lines.Add("  <!-- " + Noun(d) + " -->");
            if (d.Chance(0.4)) lines.Add("  <img src=\"/" + Noun(d) + ".png\" alt=\"" + Noun(d) + "\">");
            lines.Add("</" + tag + ">");
            return string.Join("\n", lines);
        }

        private static string Css(Dice d)
        {
            var sb = new List<string>();
            for (int rule = 0; rule < d.Between(1, 3); rule++)
            {
                string selector = d.Pick(new[] { "." + Noun(d), "#" + Noun(d), Noun(d) + " > a", "." + Noun(d) + ":hover", "a[href^=\"http\"]", "li:nth-child(2n+1)" });
                var props = new List<string>();
                props.Add("  color: " + (rule == 0 ? Slot : d.Pick(Colors)) + ";");
                props.Add("  margin: " + d.Between(0, 24) + "px " + d.Pick(new[] { "auto", "0", "1.5em", "2rem" }) + ";");
                if (d.Chance(0.5)) props.Add("  --" + Field(d) + "-gap: calc(" + d.Between(1, 9) + "px + " + d.Between(1, 3) + "em);");
                if (d.Chance(0.4)) props.Add("  background: url(\"/img/" + Noun(d) + ".png\") no-repeat !important;");
                if (d.Chance(0.3)) props.Add("  transition: opacity " + d.Between(1, 9) / 10.0 + "s ease-in-out;");
                sb.Add(selector + " {\n" + string.Join("\n", props) + "\n}");
            }
            if (d.Chance(0.4)) sb.Add("@media (max-width: " + d.Between(320, 1200) + "px) {\n  ." + Noun(d) + " { display: none; }\n}");
            if (d.Chance(0.3)) sb.Insert(0, "/* " + Noun(d) + " styles */");
            return string.Join("\n", sb);
        }

        private static string Sql(Dice d)
        {
            string table = Noun(d) + "s", alias = table.Substring(0, 1);
            var cols = d.Some(Fields, d.Between(2, 4)).Select(c => alias + "." + c).ToList();
            string select = d.Chance(0.4) ? "SELECT " + string.Join(", ", cols) + ", COUNT(*) AS n" : "SELECT " + string.Join(", ", cols);
            var sb = new List<string> { select, "FROM " + table + " " + alias };
            if (d.Chance(0.5)) sb.Add(d.Pick(new[] { "INNER JOIN", "LEFT JOIN" }) + " " + Noun(d) + "s j ON j." + Noun(d) + "_id = " + alias + ".id");
            sb.Add("WHERE " + alias + ".id = " + Slot + " AND " + alias + "." + Field(d) + " " + d.Pick(new[] { "LIKE 'a%'", "> " + d.Next(100), "IS NOT NULL", "IN (1, 2, 3)" }) + (d.Chance(0.3) ? " -- " + Noun(d) : ""));
            if (d.Chance(0.4)) sb.Add("GROUP BY " + alias + ".id");
            if (d.Chance(0.5)) sb.Add("ORDER BY " + alias + "." + Field(d) + " " + d.Pick(new[] { "ASC", "DESC" }));
            if (d.Chance(0.3)) sb.Add("LIMIT " + d.Between(1, 50));
            return string.Join("\n", sb) + ";";
        }

        private static string Json(Dice d)
        {
            var members = new List<string> { "\"id\": " + Slot };
            foreach (string f in d.Some(Fields.Skip(1).ToList(), d.Between(2, 4)))
                members.Add("\"" + f + "\": " + d.Pick(new[] { "\"" + Noun(d) + "\"", d.Between(0, 999).ToString(), (d.Next(1000) / 10.0).ToString("0.0", System.Globalization.CultureInfo.InvariantCulture), "true", "false", "null", "-1.5e3", "[1, 2, 3]", "{\"a\": \"b\"}", "\"line\\n\\u00e9\"" }));
            return "{" + string.Join(", ", members) + "}";
        }

        private static string GraphQl(Dice d)
        {
            string op = d.Pick(new[] { "query", "mutation" });
            string name = Noun(d);
            var lines = new List<string> { op + " " + char.ToUpper(name[0]) + name.Substring(1) + "($first: Int = " + d.Between(1, 20) + ") {" };
            lines.Add("  " + name + "(id: " + Slot + (d.Chance(0.5) ? ", first: $first" : "") + ")" + (d.Chance(0.3) ? " @include(if: true)" : "") + " {");
            foreach (string f in d.Some(Fields, d.Between(2, 4))) lines.Add("    " + f);
            if (d.Chance(0.5)) lines.Add("    ... on " + char.ToUpper(Noun(d)[0]) + Noun(d).Substring(1) + " {\n      " + Field(d) + "\n    }");
            lines.Add("  }");
            lines.Add("}");
            if (d.Chance(0.3)) lines.Insert(0, "# " + Noun(d));
            return string.Join("\n", lines);
        }

        private static string Xml(Dice d)
        {
            string root = Noun(d);
            var lines = new List<string> { "<" + root + " id=\"" + d.Next(100) + "\" xmlns:x=\"urn:" + Noun(d) + "\">" };
            lines.Add("  <" + Field(d) + ">" + Slot + "</" + Field(d) + ">");
            for (int i = 0; i < d.Between(1, 3); i++) lines.Add("  <" + Field(d) + " " + Field(d) + "=\"" + Noun(d) + "\"/>");
            if (d.Chance(0.4)) lines.Add("  <![CDATA[" + Noun(d) + " < " + Noun(d) + "]]>");
            if (d.Chance(0.4)) lines.Add("  <!-- " + Noun(d) + " -->");
            lines.Add("</" + root + ">");
            return string.Join("\n", lines);
        }

        private static string Markdown(Dice d)
        {
            var lines = new List<string> { "# " + Noun(d) + " " + Noun(d), "" };
            lines.Add("Some *" + Noun(d) + "* and **" + Noun(d) + "** with [a link](http://example.com/" + Noun(d) + ") and " + Slot + ".");
            lines.Add("");
            for (int i = 0; i < d.Between(2, 4); i++) lines.Add((d.Chance(0.5) ? "- " : (i + 1) + ". ") + Noun(d) + " " + Field(d));
            if (d.Chance(0.4)) lines.Add("\n> " + Noun(d));
            if (d.Chance(0.4)) lines.Add("\n## " + Field(d));
            return string.Join("\n", lines);
        }

        private static string Yaml(Dice d)
        {
            var lines = new List<string> { "name: " + Slot };
            foreach (string f in d.Some(Fields.Skip(1).ToList(), d.Between(1, 3)))
                lines.Add(f + ": " + d.Pick(new[] { Noun(d), d.Between(0, 99).ToString(), "true", "null", "\"" + Noun(d) + "\"", "[x, y]" }) + (d.Chance(0.3) ? " # " + Noun(d) : ""));
            lines.Add("items:");
            for (int i = 0; i < d.Between(2, 3); i++) lines.Add("  - " + (i == 0 && d.Chance(0.5) ? "&" + Noun(d) + " " : "") + Noun(d));
            return string.Join("\n", lines);
        }

        private static string Regex(Dice d)
        {
            var parts = new List<string> { "^" };
            for (int i = 0; i < d.Between(2, 4); i++)
                parts.Add(d.Pick(new[] { "\\d{" + d.Between(1, 4) + "}", "[a-z]+", "(?<" + Field(d) + ">\\w+)", "(a|b)", "\\s*", ".?", "[^0-9]", "\\.", "(?:" + Noun(d) + ")", "\\b" }));
            parts.Add(Slot);
            parts.Add("$");
            return string.Join("", parts);
        }

        private static string Glsl(Dice d)
        {
            var lines = new List<string> { "#version 300 es", "precision mediump float;" };
            lines.Add("uniform " + d.Pick(new[] { "vec3", "vec2", "float" }) + " u_" + Field(d) + ";");
            lines.Add("in vec2 v_uv;");
            lines.Add("out vec4 fragColor;");
            lines.Add("void main() {");
            lines.Add("  float " + Field(d) + " = " + d.Between(0, 9) + "." + d.Between(0, 9) + ";");
            lines.Add("  fragColor = vec4(" + Slot + ", v_uv, " + d.Pick(new[] { "1.0", "0.5" }) + ");");
            if (d.Chance(0.4)) lines.Add("  // " + Noun(d));
            lines.Add("}");
            return string.Join("\n", lines);
        }

        private static string Wgsl(Dice d)
        {
            var lines = new List<string>();
            lines.Add("struct " + char.ToUpper(Noun(d)[0]) + "Params {\n  " + Field(d) + " : f32,\n};");
            lines.Add("@group(0) @binding(0) var<uniform> params : " + char.ToUpper(Noun(d)[0]) + "Params;");
            lines.Add("@fragment");
            lines.Add("fn main(@location(0) uv : vec2<f32>) -> @location(0) vec4<f32> {");
            lines.Add("  let " + Field(d) + " = " + d.Between(0, 9) + "." + d.Between(0, 9) + ";");
            lines.Add("  return vec4<f32>(" + Slot + ", uv, 1.0);");
            lines.Add("}");
            return string.Join("\n", lines);
        }
    }
}
