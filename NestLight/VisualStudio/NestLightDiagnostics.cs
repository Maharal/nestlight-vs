using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;
using NestLight.Highlighting;

namespace NestLight.VisualStudio
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IErrorTag))]
    [ContentType("TypeScript")][ContentType("JavaScript")]
    internal sealed class JavaScriptDiagnosticsProvider : NestLightDiagnosticsProvider
    {
        public JavaScriptDiagnosticsProvider() : base(HostLanguage.JavaScript) { }
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IErrorTag))]
    [ContentType("CSharp")]
    internal sealed class CSharpDiagnosticsProvider : NestLightDiagnosticsProvider
    {
        public CSharpDiagnosticsProvider() : base(HostLanguage.CSharp) { }
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IErrorTag))]
    [ContentType("Python")]
    internal sealed class PythonDiagnosticsProvider : NestLightDiagnosticsProvider
    {
        public PythonDiagnosticsProvider() : base(HostLanguage.Python) { }
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IErrorTag))]
    [ContentType("C/C++")]
    internal sealed class CppDiagnosticsProvider : NestLightDiagnosticsProvider
    {
        public CppDiagnosticsProvider() : base(HostLanguage.Cpp) { }
    }

    internal abstract class NestLightDiagnosticsProvider : ITaggerProvider
    {
        private readonly HostLanguage _host;

        protected NestLightDiagnosticsProvider(HostLanguage host) { _host = host; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty(
                typeof(NestLightDiagnosticsTagger),
                () => new NestLightDiagnosticsTagger(buffer, NestLightBuffer.Get(buffer, _host))) as ITagger<T>;
        }
    }

    internal sealed class NestLightDiagnosticsTagger : ITagger<IErrorTag>
    {
        private readonly ITextBuffer _buffer;
        private readonly NestLightBuffer _shared;

        public NestLightDiagnosticsTagger(ITextBuffer buffer, NestLightBuffer shared)
        {
            _buffer = buffer;
            _shared = shared;
            buffer.ChangedLowPriority += (s, e) =>
            {
                var handler = TagsChanged;
                if (handler != null)
                    handler(this, new SnapshotSpanEventArgs(new SnapshotSpan(e.After, 0, e.After.Length)));
            };
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0) yield break;

            ITextSnapshot snapshot = spans[0].Snapshot;
            string text = _shared.Text.Of(snapshot);

            foreach (EmbeddedString s in _shared.Analysis.Highlighter is HighlightEngine engine
                ? ((IHostScanner)typeof(HighlightEngine).GetField("_scanner", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(engine)).Scan(text)
                : new EmbeddedString[0])
            {
                if (s.End > snapshot.Length) continue;
                string id = s.EmbeddedLanguageId;
                if (id == null) continue;

                int start = s.Start, end = Math.Min(s.End, text.Length);
                if (end <= start) continue;

                foreach (var diag in CheckEmbeddedCode(id, text, start, end))
                {
                    if (diag.Start >= snapshot.Length || diag.End > snapshot.Length) continue;
                    var span = new SnapshotSpan(snapshot, diag.Start, diag.End - diag.Start);
                    if (spans.IntersectsWith(span))
                        yield return new TagSpan<IErrorTag>(span, new ErrorTag(PredefinedErrorTypeNames.Warning, diag.Message));
                }
            }
        }

        private static IEnumerable<Diagnostic> CheckEmbeddedCode(string languageId, string text, int start, int end)
        {
            if (string.Equals(languageId, "html", StringComparison.OrdinalIgnoreCase) || string.Equals(languageId, "htm", StringComparison.OrdinalIgnoreCase) || string.Equals(languageId, "svg", StringComparison.OrdinalIgnoreCase))
                return CheckHtml(text, start, end);
            if (string.Equals(languageId, "json", StringComparison.OrdinalIgnoreCase))
                return CheckJson(text, start, end);
            if (string.Equals(languageId, "xml", StringComparison.OrdinalIgnoreCase))
                return CheckXml(text, start, end);
            return Enumerable.Empty<Diagnostic>();
        }

        private static IEnumerable<Diagnostic> CheckHtml(string text, int start, int end)
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
                        yield return new Diagnostic(lt, Math.Min(lt + 4, end), "Unclosed HTML comment");
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
                        yield return new Diagnostic(lt, gt + 1, "Closing tag </" + tag + "> has no matching opening tag");
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
                yield return new Diagnostic(open.Start, open.End, "Unclosed HTML tag <" + open.Name + ">");
        }

        private static IEnumerable<Diagnostic> CheckJson(string text, int start, int end)
        {
            int braces = 0, brackets = 0;
            bool inString = false;
            for (int i = start; i < end; i++)
            {
                char c = text[i];
                if (inString) { if (c == '\\') i++; else if (c == '"') inString = false; continue; }
                if (c == '"') { inString = true; continue; }
                if (c == '{') braces++;
                else if (c == '}') { braces--; if (braces < 0) yield return new Diagnostic(i, i + 1, "Unexpected closing '}'"); }
                else if (c == '[') brackets++;
                else if (c == ']') { brackets--; if (brackets < 0) yield return new Diagnostic(i, i + 1, "Unexpected closing ']'"); }
            }
            if (inString) yield return new Diagnostic(end - 1, end, "Unterminated JSON string");
            if (braces > 0) yield return new Diagnostic(start, start + 1, "Unclosed '{'");
            if (brackets > 0) yield return new Diagnostic(start, start + 1, "Unclosed '['");
        }

        private static IEnumerable<Diagnostic> CheckXml(string text, int start, int end)
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
                        yield return new Diagnostic(lt, gt + 1, "Closing tag </" + tag + "> has no matching opening tag");
                    else
                        openTags.RemoveRange(found, openTags.Count - found);
                }
                else if (!selfClosing) openTags.Add(new TagInfo(tag, lt, gt + 1));
                i = gt + 1;
            }
            foreach (TagInfo open in openTags)
                yield return new Diagnostic(open.Start, open.End, "Unclosed XML tag <" + open.Name + ">");
        }

        private static readonly HashSet<string> VoidElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "source", "track", "wbr",
            "path", "circle", "rect", "line", "ellipse", "polygon", "polyline", "stop", "use"
        };

        private static bool IsVoidElement(string tag) { return VoidElements.Contains(tag); }

        private struct Diagnostic
        {
            public int Start, End;
            public string Message;
            public Diagnostic(int start, int end, string message) { Start = start; End = end; Message = message; }
        }

        private struct TagInfo
        {
            public string Name;
            public int Start, End;
            public TagInfo(string name, int start, int end) { Name = name; Start = start; End = end; }
        }
    }
}
