using System;
using System.Collections.Generic;

namespace NestLight.Common
{
    /// <summary>
    /// What can be collapsed in the languages that are not markup: the blocks of a language with brackets (CSS, JSON, GraphQL, shaders, SQL),
    /// and the sections of the ones with indentation or headings (YAML, TOML, Markdown). Rough passes over text[from, to), not parses: they
    /// only have to find places that are worth collapsing, and never throw on incomplete code.
    /// </summary>
    internal static class OutliningStructures
    {
        public static void Regions(string id, string text, int from, int to, List<OutlineRegion> result)
        {
            string brackets = Brackets(id);
            if (brackets != null) ByBrackets(text, from, to, brackets, id, result);
            else if (IsYaml(id)) ByIndentation(text, from, to, result);
            else if (IsToml(id)) BySections(text, from, to, line => line.StartsWith("[", StringComparison.Ordinal) ? 1 : 0, result);
            else if (IsMarkdown(id)) BySections(text, from, to, HeadingLevel, result);
        }

        private static bool IsYaml(string id) { return id == "yaml" || id == "yml"; }
        private static bool IsToml(string id) { return id == "toml"; }
        private static bool IsMarkdown(string id) { return id == "markdown" || id == "md"; }

        /// <summary>The opening brackets that make a block in the language, or null when it has none worth collapsing.</summary>
        private static string Brackets(string id)
        {
            switch (id)
            {
                case "css": case "glsl": case "wgsl": return "{";
                case "json": case "graphql": case "gql": return "{[";
                case "sql": return "(";
                default: return null;
            }
        }

        // ---- brackets -------------------------------------------------------------------------------------------------

        private static void ByBrackets(string text, int from, int to, string openers, string id, List<OutlineRegion> result)
        {
            bool slashComments = id == "css" || id == "glsl" || id == "wgsl";
            bool lineSlashes = id == "glsl" || id == "wgsl";
            bool hashComments = id == "graphql" || id == "gql";
            bool dashComments = id == "sql";
            var open = new List<KeyValuePair<char, int>>(); // the opening bracket, the index right after it
            int i = from;
            while (i < to)
            {
                char c = text[i];
                if (slashComments && c == '/' && i + 1 < to && text[i + 1] == '*') { i = Skip(text, i + 2, to, "*/"); continue; }
                if (lineSlashes && c == '/' && i + 1 < to && text[i + 1] == '/') { i = LineEnd(text, i, to); continue; }
                if (hashComments && c == '#') { i = LineEnd(text, i, to); continue; }
                if (dashComments && c == '-' && i + 1 < to && text[i + 1] == '-') { i = LineEnd(text, i, to); continue; }
                if (dashComments && c == '/' && i + 1 < to && text[i + 1] == '*') { i = Skip(text, i + 2, to, "*/"); continue; }
                if (c == '"' || (c == '\'' && id != "wgsl" && id != "glsl"))
                {
                    i = EndOfQuoted(text, i, to, c);
                    continue;
                }
                if (openers.IndexOf(c) >= 0) open.Add(new KeyValuePair<char, int>(c, i + 1));
                else if (c == '}' || c == ']' || c == ')')
                {
                    char want = c == '}' ? '{' : c == ']' ? '[' : '(';
                    for (int k = open.Count - 1; k >= 0; k--)
                    {
                        if (open[k].Key != want) continue;
                        Add(text, open[k].Value, i, result);
                        open.RemoveRange(k, open.Count - k);
                        break;
                    }
                }
                i++;
            }
        }

        // ---- indentation (YAML) ---------------------------------------------------------------------------------------

        /// <summary>A line that ends with a colon (a key whose value is the lines below) holds the lines indented deeper than it.</summary>
        private static void ByIndentation(string text, int from, int to, List<OutlineRegion> result)
        {
            List<Line> lines = Lines(text, from, to);
            for (int n = 0; n < lines.Count; n++)
            {
                Line head = lines[n];
                if (head.Blank || !EndsWithColon(text, head)) continue;
                int last = -1;
                for (int m = n + 1; m < lines.Count; m++)
                {
                    if (lines[m].Blank) continue;
                    if (lines[m].Indent <= head.Indent) break;
                    last = m;
                }
                if (last > n) Add(text, head.End, lines[last].End, result);
            }
        }

        private static bool EndsWithColon(string text, Line line)
        {
            int e = line.End;
            while (e > line.Start && (text[e - 1] == ' ' || text[e - 1] == '\t')) e--;
            return e > line.Start && text[e - 1] == ':';
        }

        // ---- sections (TOML tables, Markdown headings) ----------------------------------------------------------------

        /// <summary>
        /// <paramref name="level"/> gives the level of a header line (0: not a header; a smaller number is a larger section). A section
        /// goes from the end of its header to the end of the last non-blank line before the next header of the same or a larger level.
        /// </summary>
        private static void BySections(string text, int from, int to, Func<string, int> level, List<OutlineRegion> result)
        {
            List<Line> lines = Lines(text, from, to);
            var levels = new int[lines.Count];
            bool fenced = false;
            for (int n = 0; n < lines.Count; n++)
            {
                string line = text.Substring(lines[n].Start, lines[n].End - lines[n].Start).TrimStart();
                if (line.StartsWith("```", StringComparison.Ordinal) || line.StartsWith("~~~", StringComparison.Ordinal)) { fenced = !fenced; continue; }
                if (!fenced) levels[n] = level(line);
            }
            for (int n = 0; n < lines.Count; n++)
            {
                if (levels[n] == 0) continue;
                int last = n;
                for (int m = n + 1; m < lines.Count; m++)
                {
                    if (levels[m] != 0 && levels[m] <= levels[n]) break;
                    if (!lines[m].Blank) last = m;
                }
                if (last > n) Add(text, lines[n].End, lines[last].End, result);
            }
        }

        private static int HeadingLevel(string line)
        {
            int n = 0;
            while (n < line.Length && line[n] == '#') n++;
            return n > 0 && n <= 6 && n < line.Length && line[n] == ' ' ? n : 0;
        }

        // ---- helpers --------------------------------------------------------------------------------------------------

        private struct Line
        {
            public int Start, End, Indent;
            public bool Blank;
        }

        /// <summary>The lines of text[from, to): End is before the line break (and before its "\r").</summary>
        private static List<Line> Lines(string text, int from, int to)
        {
            var lines = new List<Line>();
            int start = from;
            while (start <= to)
            {
                int nl = start < to ? text.IndexOf('\n', start, to - start) : -1;
                int end = nl < 0 ? to : nl;
                int visibleEnd = end > start && text[end - 1] == '\r' ? end - 1 : end;
                int indent = 0;
                while (start + indent < visibleEnd && (text[start + indent] == ' ' || text[start + indent] == '\t')) indent++;
                lines.Add(new Line { Start = start, End = visibleEnd, Indent = indent, Blank = start + indent >= visibleEnd });
                if (nl < 0) break;
                start = nl + 1;
            }
            return lines;
        }

        private static void Add(string text, int start, int end, List<OutlineRegion> result)
        {
            if (end > start && text.IndexOf('\n', start, end - start) >= 0) result.Add(new OutlineRegion(start, end));
        }

        private static int Skip(string text, int from, int to, string end)
        {
            int at = from >= to ? -1 : text.IndexOf(end, from, to - from, StringComparison.Ordinal);
            return at < 0 ? to : at + end.Length;
        }

        private static int LineEnd(string text, int from, int to)
        {
            int nl = text.IndexOf('\n', from, to - from);
            return nl < 0 ? to : nl;
        }

        /// <summary>The index after the closing quote; a quoted piece that is not closed on its line ends with the line.</summary>
        private static int EndOfQuoted(string text, int at, int to, char quote)
        {
            for (int i = at + 1; i < to; i++)
            {
                if (text[i] == '\\') { i++; continue; }
                if (text[i] == quote) return i + 1;
                if (text[i] == '\n') return i;
            }
            return to;
        }
    }
}
