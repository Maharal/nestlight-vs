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

            IHostScanner scanner = _shared.Analysis.Highlighter.Scanner;
            if (scanner == null) yield break;

            foreach (EmbeddedString s in scanner.Scan(text))
            {
                if (s.End > snapshot.Length) continue;
                string id = s.EmbeddedLanguageId;
                if (id == null) continue;

                int start = s.Start, end = Math.Min(s.End, text.Length);
                if (end <= start) continue;

                foreach (var diag in DiagnosticChecker.Check(id, text, start, end))
                {
                    if (diag.Start >= snapshot.Length || diag.End > snapshot.Length) continue;
                    var span = new SnapshotSpan(snapshot, diag.Start, diag.End - diag.Start);
                    if (spans.IntersectsWith(span))
                        yield return new TagSpan<IErrorTag>(span, new ErrorTag(PredefinedErrorTypeNames.Warning, diag.Message));
                }
            }
        }
    }
}
