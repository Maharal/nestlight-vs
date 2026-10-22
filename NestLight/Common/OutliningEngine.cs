using System;
using System.Collections.Generic;

namespace NestLight.Common
{
    /// <summary>A range of text the programmer can collapse: [Start, End).</summary>
    internal struct OutlineRegion
    {
        public readonly int Start;
        public readonly int End;

        public OutlineRegion(int start, int end)
        {
            Start = start; End = end;
        }
    }

    /// <summary>
    /// Decides what can be collapsed in a host file: the content of each embedded string that takes several lines, and, inside markup
    /// (html, svg, xml), each element that takes several lines. The line of the opening quote or tag stays visible (it says what is
    /// hidden) and so does the closing one.
    /// </summary>
    internal static class OutliningEngine
    {
        /// <summary>The content must have at least this many line breaks, or collapsing it would hide almost nothing.</summary>
        private const int MinLineBreaks = 2;

        private static readonly HashSet<string> VoidElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr"
        };

        /// <summary>Ordered by start (the outer one first). Strings nobody recognized (no language) are not outlined.</summary>
        public static IReadOnlyList<OutlineRegion> Regions(IReadOnlyList<EmbeddedString> strings, string text)
        {
            var result = new List<OutlineRegion>();
            foreach (EmbeddedString s in strings)
            {
                if (s.EmbeddedLanguageId == null) continue;
                int end = s.End < text.Length ? s.End : text.Length;
                int breaks = 0;
                for (int i = s.Start; i < end; i++)
                    if (text[i] == (char)10) breaks++;
                if (breaks >= MinLineBreaks) result.Add(new OutlineRegion(s.Start, end));
                if (IsMarkup(s.EmbeddedLanguageId)) Elements(text, s.Start, end, result);
                else OutliningStructures.Regions(s.EmbeddedLanguageId, text, s.Start, end, result);
            }
            return Nested(result);
        }

        /// <summary>
        /// Keeps the regions that nest properly: the editor cannot lay out two that start at the same place or that cross each
        /// other, so of such a pair the outer (or the first) one stays. Ordered by start, the outer one first.
        /// </summary>
        private static IReadOnlyList<OutlineRegion> Nested(List<OutlineRegion> regions)
        {
            regions.Sort((a, b) => a.Start != b.Start ? a.Start.CompareTo(b.Start) : b.End.CompareTo(a.End));
            var kept = new List<OutlineRegion>();
            var enclosing = new List<OutlineRegion>();
            foreach (OutlineRegion r in regions)
            {
                while (enclosing.Count > 0 && enclosing[enclosing.Count - 1].End <= r.Start) enclosing.RemoveAt(enclosing.Count - 1);
                if (enclosing.Count > 0)
                {
                    OutlineRegion top = enclosing[enclosing.Count - 1];
                    if (r.Start == top.Start || r.End > top.End) continue;
                }
                kept.Add(r);
                enclosing.Add(r);
            }
            return kept;
        }

        private static bool IsMarkup(string id)
        {
            return id == "html" || id == "htm" || id == "svg" || id == "xml";
        }

        /// <summary>
        /// The elements of markup in text[from, to) that take several lines: what is between the opening tag and the closing one can be
        /// collapsed, and the tags stay (<c>&lt;div&gt;...&lt;/div&gt;</c>). A rough pass, not a parse: a closing tag closes the nearest open
        /// element of its name (the ones in between are dropped), and one with no open element is ignored.
        /// </summary>
        private static void Elements(string text, int from, int to, List<OutlineRegion> result)
        {
            var open = new List<KeyValuePair<string, int>>(); // name, index right after its opening tag
            int i = from;
            while (i < to)
            {
                int lt = text.IndexOf('<', i, to - i);
                if (lt < 0) break;
                if (StartsWith(text, lt, to, "<!--")) { i = Skip(text, lt + 4, to, "-->"); continue; }
                if (StartsWith(text, lt, to, "<![CDATA[")) { i = Skip(text, lt + 9, to, "]]>"); continue; }
                bool closing = lt + 1 < to && text[lt + 1] == '/';
                int n = lt + (closing ? 2 : 1);
                if (n >= to || !char.IsLetter(text[n])) { i = lt + 1; continue; }
                int nameEnd = n;
                while (nameEnd < to && (char.IsLetterOrDigit(text[nameEnd]) || text[nameEnd] == '-' || text[nameEnd] == ':' || text[nameEnd] == '_' || text[nameEnd] == '.')) nameEnd++;
                string name = text.Substring(n, nameEnd - n);
                int gt = TagEnd(text, nameEnd, to);
                if (gt < 0) break;
                i = gt + 1;
                if (closing)
                {
                    for (int k = open.Count - 1; k >= 0; k--)
                    {
                        if (!string.Equals(open[k].Key, name, StringComparison.OrdinalIgnoreCase)) continue;
                        AddIfMultiline(text, open[k].Value, lt, result);
                        open.RemoveRange(k, open.Count - k);
                        break;
                    }
                    continue;
                }
                if (text[gt - 1] == '/' || VoidElements.Contains(name)) continue;
                if (name.Equals("script", StringComparison.OrdinalIgnoreCase) || name.Equals("style", StringComparison.OrdinalIgnoreCase))
                {
                    // The content is code of another language: no tags in it.
                    int close = text.IndexOf("</" + name, i, to - i, StringComparison.OrdinalIgnoreCase);
                    if (close < 0) break;
                    AddIfMultiline(text, i, close, result);
                    i = close;
                    continue;
                }
                open.Add(new KeyValuePair<string, int>(name, i));
            }
        }

        private static void AddIfMultiline(string text, int start, int end, List<OutlineRegion> result)
        {
            if (end > start && text.IndexOf('\n', start, end - start) >= 0) result.Add(new OutlineRegion(start, end));
        }

        private static bool StartsWith(string text, int at, int to, string prefix)
        {
            return at + prefix.Length <= to && string.CompareOrdinal(text, at, prefix, 0, prefix.Length) == 0;
        }

        private static int Skip(string text, int from, int to, string end)
        {
            int at = from >= to ? -1 : text.IndexOf(end, from, to - from, StringComparison.Ordinal);
            return at < 0 ? to : at + end.Length;
        }

        /// <summary>The index of the <c>&gt;</c> that ends the tag, skipping the ones inside quoted attribute values; -1 when the tag is not closed.</summary>
        private static int TagEnd(string text, int from, int to)
        {
            char quote = '\0';
            for (int i = from; i < to; i++)
            {
                char c = text[i];
                if (quote != '\0') { if (c == quote) quote = '\0'; }
                else if (c == '"' || c == '\'') quote = c;
                else if (c == '>') return i;
            }
            return -1;
        }
    }
}
