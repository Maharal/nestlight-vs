using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;
using NestLight.Highlighting;

namespace NestLight.VisualStudio
{
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("TypeScript")][ContentType("JavaScript")]
    internal sealed class JavaScriptOutliningTaggerProvider : NestLightOutliningTaggerProvider
    {
        public JavaScriptOutliningTaggerProvider() : base(HostLanguage.JavaScript) { }
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("CSharp")]
    internal sealed class CSharpOutliningTaggerProvider : NestLightOutliningTaggerProvider
    {
        public CSharpOutliningTaggerProvider() : base(HostLanguage.CSharp) { }
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("Python")]
    internal sealed class PythonOutliningTaggerProvider : NestLightOutliningTaggerProvider
    {
        public PythonOutliningTaggerProvider() : base(HostLanguage.Python) { }
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("C/C++")]
    internal sealed class CppOutliningTaggerProvider : NestLightOutliningTaggerProvider
    {
        public CppOutliningTaggerProvider() : base(HostLanguage.Cpp) { }
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("Rust")]
    internal sealed class RustOutliningTaggerProvider : NestLightOutliningTaggerProvider
    {
        public RustOutliningTaggerProvider() : base(HostLanguage.Rust) { }
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("Go")]
    internal sealed class GoOutliningTaggerProvider : NestLightOutliningTaggerProvider
    {
        public GoOutliningTaggerProvider() : base(HostLanguage.Go) { }
    }

    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IOutliningRegionTag))]
    [ContentType("Java")]
    internal sealed class JavaOutliningTaggerProvider : NestLightOutliningTaggerProvider
    {
        public JavaOutliningTaggerProvider() : base(HostLanguage.Java) { }
    }

    internal abstract class NestLightOutliningTaggerProvider : ITaggerProvider
    {
        private readonly HostLanguage _host;

        protected NestLightOutliningTaggerProvider(HostLanguage host) { _host = host; }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            // The buffer may match several providers (JavaScript and TypeScript share content types); one tagger is enough,
            // otherwise every region is offered twice.
            if (buffer.Properties.ContainsProperty(typeof(NestLightOutliningTagger))) return null;
            return buffer.Properties.GetOrCreateSingletonProperty(
                typeof(NestLightOutliningTagger),
                () => new NestLightOutliningTagger(buffer, NestLightBuffer.Get(buffer, _host))) as ITagger<T>;
        }
    }

    /// <summary>The adapter between the editor and <see cref="OutliningEngine"/>: it only converts regions to outlining tags.</summary>
    internal sealed class NestLightOutliningTagger : ITagger<IOutliningRegionTag>
    {
        /// <summary>Longest preview shown when the mouse is over a collapsed region.</summary>
        private const int HintLength = 2000;

        private readonly NestLightBuffer _shared;

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public NestLightOutliningTagger(ITextBuffer buffer, NestLightBuffer shared)
        {
            _shared = shared;
            buffer.ChangedLowPriority += OnBufferChanged;
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            var handler = TagsChanged;
            if (handler != null) handler(this, new SnapshotSpanEventArgs(new SnapshotSpan(e.After, 0, e.After.Length)));
        }

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var result = new List<ITagSpan<IOutliningRegionTag>>();
            if (spans.Count == 0) return result;
            ITextSnapshot snapshot = spans[0].Snapshot;
            IHostScanner scanner = _shared.Analysis.Highlighter.Scanner;
            if (scanner == null) return result;
            string text = _shared.Text.Of(snapshot);

            foreach (OutlineRegion region in OutliningEngine.Regions(scanner.Scan(text), text))
            {
                if (region.End > snapshot.Length) continue;
                var span = new SnapshotSpan(snapshot, region.Start, region.End - region.Start);
                if (!Intersects(spans, span)) continue;
                string hint = text.Substring(region.Start, Math.Min(region.End - region.Start, HintLength)).TrimStart('\r', '\n');
                result.Add(new TagSpan<IOutliningRegionTag>(span, new OutliningRegionTag(false, false, "...", hint)));
            }
            return result;
        }

        private static bool Intersects(NormalizedSnapshotSpanCollection spans, SnapshotSpan span)
        {
            foreach (SnapshotSpan s in spans)
                if (s.OverlapsWith(span)) return true;
            return false;
        }
    }
}
