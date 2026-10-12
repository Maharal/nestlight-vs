using System;
using System.Collections.Generic;
using System.Linq;

namespace NestLight.Common
{
    internal struct DiagnosticResult
    {
        public readonly int Start;
        public readonly int End;
        public readonly string Message;

        public DiagnosticResult(int start, int end, string message)
        {
            Start = start; End = end; Message = message;
        }
    }

    internal static class DiagnosticChecker
    {
        public static IEnumerable<DiagnosticResult> Check(string languageId, string text, int start, int end)
        {
            if (string.Equals(languageId, "html", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(languageId, "htm", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(languageId, "svg", StringComparison.OrdinalIgnoreCase))
                return CheckHtml(text, start, end);
            if (string.Equals(languageId, "json", StringComparison.OrdinalIgnoreCase))
                return CheckJson(text, start, end);
            if (string.Equals(languageId, "xml", StringComparison.OrdinalIgnoreCase))
                return CheckXml(text, start, end);
            return Enumerable.Empty<DiagnosticResult>();
        }

        internal static IEnumerable<DiagnosticResult> CheckHtml(string text, int start, int end)
        {
            var openTags = new List<TagInfo>();
            int i = start;
            while (i < end)
            {
                int lt = text.IndexOf('<', i, end - i);
                if (lt < 0) break;

                if (lt + 3 < end && text.Substring(lt, 4) == "<!--")
                {
                    int closeComment = text.IndexOf("-->", lt + 4, end - lt - 4, StringComparison.Ordinal);
                    if (closeComment < 0)
                    {
                        yield return new DiagnosticResult(lt, Math.Min(lt + 4, end), "Unclosed HTML comment");
                        break;
                    }
                    i = closeComment + 3;
                    continue;
                }

                int gt = text.IndexOf('>', lt + 1, end - lt - 1);
                if (gt < 0) { i = lt + 1; continue; }

                int nameStart = lt + 1;
                bool closing = nameStart < gt && text[nameStart] == '/';
                if (closing) nameStart++;
                int nameEnd = nameStart;
                while (nameEnd < gt && (char.IsLetterOrDigit(text[nameEnd]) || text[nameEnd] == '-' || text[nameEnd] == ':')) nameEnd++;
                if (nameEnd == nameStart) { i = gt + 1; continue; }

                string tag = text.Substring(nameStart, nameEnd - nameStart).ToLowerInvariant();
                bool selfClosing = text[gt - 1] == '/';

                if (closing)
                {
                    int found = -1;
                    for (int k = openTags.Count - 1; k >= 0; k--)
                    {
                        if (string.Equals(openTags[k].Name, tag, StringComparison.OrdinalIgnoreCase)) { found = k; break; }
                    }
                    if (found < 0)
                        yield return new DiagnosticResult(lt, gt + 1, "Closing tag </" + tag + "> has no matching opening tag");
                    else
                        openTags.RemoveRange(found, openTags.Count - found);
                }
                else if (!selfClosing && !IsVoidElement(tag))
                {
                    if (string.Equals(tag, "style", StringComparison.OrdinalIgnoreCase) || string.Equals(tag, "script", StringComparison.OrdinalIgnoreCase))
                    {
                        string closeTag = "</" + tag;
                        int closePos = text.IndexOf(closeTag, gt + 1, end - gt - 1, StringComparison.OrdinalIgnoreCase);
                        if (closePos >= 0) { i = closePos; continue; }
                    }
                    openTags.Add(new TagInfo(tag, lt, gt + 1));
                }

                i = gt + 1;
            }

            foreach (TagInfo open in openTags)
                yield return new DiagnosticResult(open.Start, open.End, "Unclosed HTML tag <" + open.Name + ">");
        }

        internal static IEnumerable<DiagnosticResult> CheckJson(string text, int start, int end)
        {
            int braces = 0, brackets = 0;
            bool inString = false;
            for (int i = start; i < end; i++)
            {
                char c = text[i];
                if (inString) { if (c == '\\') i++; else if (c == '"') inString = false; continue; }
                if (c == '"') { inString = true; continue; }
                if (c == '{') braces++;
                else if (c == '}') { braces--; if (braces < 0) yield return new DiagnosticResult(i, i + 1, "Unexpected closing '}'"); }
                else if (c == '[') brackets++;
                else if (c == ']') { brackets--; if (brackets < 0) yield return new DiagnosticResult(i, i + 1, "Unexpected closing ']'"); }
            }
            if (inString) yield return new DiagnosticResult(end - 1, end, "Unterminated JSON string");
            if (braces > 0) yield return new DiagnosticResult(start, start + 1, "Unclosed '{'");
            if (brackets > 0) yield return new DiagnosticResult(start, start + 1, "Unclosed '['");
        }

        internal static IEnumerable<DiagnosticResult> CheckXml(string text, int start, int end)
        {
            var openTags = new List<TagInfo>();
            int i = start;
            while (i < end)
            {
                int lt = text.IndexOf('<', i, end - i);
                if (lt < 0) break;
                int gt = text.IndexOf('>', lt + 1, end - lt - 1);
                if (gt < 0) { i = lt + 1; continue; }

                int nameStart = lt + 1;
                if (nameStart < gt && text[nameStart] == '?') { i = gt + 1; continue; }
                if (nameStart < gt && text[nameStart] == '!') { i = gt + 1; continue; }
                bool closing = nameStart < gt && text[nameStart] == '/';
                if (closing) nameStart++;
                int nameEnd = nameStart;
                while (nameEnd < gt && text[nameEnd] != ' ' && text[nameEnd] != '/' && text[nameEnd] != '>') nameEnd++;
                if (nameEnd == nameStart) { i = gt + 1; continue; }

                string tag = text.Substring(nameStart, nameEnd - nameStart);
                bool selfClosing = text[gt - 1] == '/';

                if (closing)
                {
                    int found = -1;
                    for (int k = openTags.Count - 1; k >= 0; k--)
                        if (string.Equals(openTags[k].Name, tag, StringComparison.Ordinal)) { found = k; break; }
                    if (found < 0)
                        yield return new DiagnosticResult(lt, gt + 1, "Closing tag </" + tag + "> has no matching opening tag");
                    else
                        openTags.RemoveRange(found, openTags.Count - found);
                }
                else if (!selfClosing) openTags.Add(new TagInfo(tag, lt, gt + 1));
                i = gt + 1;
            }
            foreach (TagInfo open in openTags)
                yield return new DiagnosticResult(open.Start, open.End, "Unclosed XML tag <" + open.Name + ">");
        }

        private static readonly HashSet<string> VoidElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "source", "track", "wbr",
            "path", "circle", "rect", "line", "ellipse", "polygon", "polyline", "stop", "use"
        };

        private static bool IsVoidElement(string tag) { return VoidElements.Contains(tag); }

        private struct TagInfo
        {
            public string Name;
            public int Start, End;
            public TagInfo(string name, int start, int end) { Name = name; Start = start; End = end; }
        }
    }
}
