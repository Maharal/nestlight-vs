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
            if (string.Equals(languageId, "sql", StringComparison.OrdinalIgnoreCase))
                return CheckSql(text, start, end);
            if (string.Equals(languageId, "css", StringComparison.OrdinalIgnoreCase))
                return CheckCss(text, start, end);
            if (string.Equals(languageId, "yaml", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(languageId, "yml", StringComparison.OrdinalIgnoreCase))
                return CheckYaml(text, start, end);
            if (string.Equals(languageId, "graphql", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(languageId, "gql", StringComparison.OrdinalIgnoreCase))
                return CheckGraphQl(text, start, end);
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
                        if (closePos >= 0)
                        {
                            int closeGt = text.IndexOf('>', closePos + closeTag.Length, end - closePos - closeTag.Length);
                            i = closeGt >= 0 ? closeGt + 1 : closePos + closeTag.Length;
                            continue;
                        }
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

        internal static IEnumerable<DiagnosticResult> CheckSql(string text, int start, int end)
        {
            int parens = 0;
            bool inString = false;
            char quote = '\0';
            for (int i = start; i < end; i++)
            {
                char c = text[i];
                if (inString)
                {
                    if (c == quote)
                    {
                        if (i + 1 < end && text[i + 1] == quote) i++;
                        else inString = false;
                    }
                    continue;
                }
                if (c == '-' && i + 1 < end && text[i + 1] == '-')
                {
                    int nl = text.IndexOf('\n', i + 2, end - i - 2);
                    i = nl < 0 ? end - 1 : nl;
                    continue;
                }
                if (c == '\'' || c == '"') { inString = true; quote = c; continue; }
                if (c == '(') parens++;
                else if (c == ')')
                {
                    parens--;
                    if (parens < 0)
                        yield return new DiagnosticResult(i, i + 1, "Unexpected closing ')'");
                }
            }
            if (inString) yield return new DiagnosticResult(end - 1, end, "Unterminated SQL string");
            if (parens > 0) yield return new DiagnosticResult(start, Math.Min(start + 1, end), "Unclosed '('");
        }

        internal static IEnumerable<DiagnosticResult> CheckCss(string text, int start, int end)
        {
            int braces = 0;
            int parens = 0;
            int brackets = 0;
            bool inString = false;
            char quote = '\0';
            bool inComment = false;
            for (int i = start; i < end; i++)
            {
                char c = text[i];
                if (inComment)
                {
                    if (c == '*' && i + 1 < end && text[i + 1] == '/')
                    {
                        inComment = false;
                        i++;
                    }
                    continue;
                }
                if (inString)
                {
                    if (c == '\\') i++;
                    else if (c == quote) inString = false;
                    continue;
                }
                if (c == '/' && i + 1 < end && text[i + 1] == '*') { inComment = true; i++; continue; }
                if (c == '\'' || c == '"') { inString = true; quote = c; continue; }
                if (c == '{') braces++;
                else if (c == '}')
                {
                    braces--;
                    if (braces < 0) yield return new DiagnosticResult(i, i + 1, "Unexpected closing '}'");
                }
                else if (c == '(') parens++;
                else if (c == ')')
                {
                    parens--;
                    if (parens < 0) yield return new DiagnosticResult(i, i + 1, "Unexpected closing ')'");
                }
                else if (c == '[') brackets++;
                else if (c == ']')
                {
                    brackets--;
                    if (brackets < 0) yield return new DiagnosticResult(i, i + 1, "Unexpected closing ']'");
                }
            }
            if (inComment) yield return new DiagnosticResult(end - 1, end, "Unterminated CSS comment");
            if (inString) yield return new DiagnosticResult(end - 1, end, "Unterminated CSS string");
            if (braces > 0) yield return new DiagnosticResult(start, Math.Min(start + 1, end), "Unclosed '{'");
            if (parens > 0) yield return new DiagnosticResult(start, Math.Min(start + 1, end), "Unclosed '('");
            if (brackets > 0) yield return new DiagnosticResult(start, Math.Min(start + 1, end), "Unclosed '['");
        }

        internal static IEnumerable<DiagnosticResult> CheckYaml(string text, int start, int end)
        {
            bool prevWasSpace = false;
            bool prevWasTab = false;
            int lineStart = start;
            int lineNumber = 0;
            for (int i = start; i <= end; i++)
            {
                bool atEnd = i == end;
                char c = atEnd ? '\n' : text[i];
                if (c == '\n' || atEnd)
                {
                    lineNumber++;
                    lineStart = i + 1;
                    prevWasSpace = false;
                    prevWasTab = false;
                    continue;
                }
                if (i == lineStart)
                {
                    prevWasSpace = c == ' ';
                    prevWasTab = c == '\t';
                }
                else if (i > lineStart && (prevWasSpace || prevWasTab) && (c == ' ' || c == '\t'))
                {
                    if ((prevWasSpace && c == '\t') || (prevWasTab && c == ' '))
                    {
                        yield return new DiagnosticResult(lineStart, Math.Min(i + 1, end), "Mixed tabs and spaces in indentation");
                        for (int skip = i + 1; skip < end && text[skip] != '\n'; skip++) i = skip;
                        continue;
                    }
                    prevWasSpace = c == ' ';
                    prevWasTab = c == '\t';
                }
                else
                {
                    prevWasSpace = false;
                    prevWasTab = false;
                }
            }

            int duplicateStart = start;
            while (duplicateStart < end)
            {
                int lineEnd = text.IndexOf('\n', duplicateStart, end - duplicateStart);
                if (lineEnd < 0) lineEnd = end;
                int p = duplicateStart;
                while (p < lineEnd && (text[p] == ' ' || text[p] == '\t')) p++;
                if (p == duplicateStart && p < lineEnd && text[p] != '#' && text[p] != '-')
                {
                    int colon = text.IndexOf(':', p, lineEnd - p);
                    if (colon > p)
                    {
                        string key = text.Substring(p, colon - p).TrimEnd();
                        int nextLine = lineEnd + 1;
                        while (nextLine < end)
                        {
                            int nextEnd = text.IndexOf('\n', nextLine, end - nextLine);
                            if (nextEnd < 0) nextEnd = end;
                            int np = nextLine;
                            while (np < nextEnd && (text[np] == ' ' || text[np] == '\t')) np++;
                            if (np > nextLine || np >= nextEnd) { nextLine = nextEnd + 1; continue; }
                            if (text[np] == '#') { nextLine = nextEnd + 1; continue; }
                            int nc = text.IndexOf(':', np, nextEnd - np);
                            if (nc > np)
                            {
                                string nextKey = text.Substring(np, nc - np).TrimEnd();
                                if (np == nextLine && string.Equals(key, nextKey, StringComparison.Ordinal))
                                    yield return new DiagnosticResult(np, nc, "Duplicate YAML key '" + key + "'");
                            }
                            nextLine = nextEnd + 1;
                        }
                    }
                }
                duplicateStart = lineEnd + 1;
            }
        }

        internal static IEnumerable<DiagnosticResult> CheckGraphQl(string text, int start, int end)
        {
            int braces = 0;
            int parens = 0;
            int brackets = 0;
            bool inString = false;
            bool inComment = false;
            for (int i = start; i < end; i++)
            {
                char c = text[i];
                if (inComment) { if (c == '\n') inComment = false; continue; }
                if (inString) { if (c == '\\') i++; else if (c == '"') inString = false; continue; }
                if (c == '#') { inComment = true; continue; }
                if (c == '"') { inString = true; continue; }
                if (c == '{') braces++;
                else if (c == '}')
                {
                    braces--;
                    if (braces < 0) yield return new DiagnosticResult(i, i + 1, "Unexpected closing '}'");
                }
                else if (c == '(') parens++;
                else if (c == ')')
                {
                    parens--;
                    if (parens < 0) yield return new DiagnosticResult(i, i + 1, "Unexpected closing ')'");
                }
                else if (c == '[') brackets++;
                else if (c == ']')
                {
                    brackets--;
                    if (brackets < 0) yield return new DiagnosticResult(i, i + 1, "Unexpected closing ']'");
                }
            }
            if (inString) yield return new DiagnosticResult(end - 1, end, "Unterminated GraphQL string");
            if (braces > 0) yield return new DiagnosticResult(start, Math.Min(start + 1, end), "Unclosed '{'");
            if (parens > 0) yield return new DiagnosticResult(start, Math.Min(start + 1, end), "Unclosed '('");
            if (brackets > 0) yield return new DiagnosticResult(start, Math.Min(start + 1, end), "Unclosed '['");
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
