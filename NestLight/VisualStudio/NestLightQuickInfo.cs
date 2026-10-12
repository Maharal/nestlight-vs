using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;
using NestLight.Highlighting;

namespace NestLight.VisualStudio
{
    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("NestLight quick info (JavaScript / TypeScript)")]
    [ContentType("TypeScript")][ContentType("JavaScript")]
    internal sealed class JavaScriptQuickInfoSourceProvider : NestLightQuickInfoSourceProvider
    {
        public JavaScriptQuickInfoSourceProvider() : base(HostLanguage.JavaScript) { }
    }

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("NestLight quick info (C#)")]
    [ContentType("CSharp")]
    internal sealed class CSharpQuickInfoSourceProvider : NestLightQuickInfoSourceProvider
    {
        public CSharpQuickInfoSourceProvider() : base(HostLanguage.CSharp) { }
    }

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("NestLight quick info (Python)")]
    [ContentType("Python")]
    internal sealed class PythonQuickInfoSourceProvider : NestLightQuickInfoSourceProvider
    {
        public PythonQuickInfoSourceProvider() : base(HostLanguage.Python) { }
    }

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("NestLight quick info (C++)")]
    [ContentType("C/C++")]
    internal sealed class CppQuickInfoSourceProvider : NestLightQuickInfoSourceProvider
    {
        public CppQuickInfoSourceProvider() : base(HostLanguage.Cpp) { }
    }

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("NestLight quick info (Rust)")]
    [ContentType("Rust")]
    internal sealed class RustQuickInfoSourceProvider : NestLightQuickInfoSourceProvider
    {
        public RustQuickInfoSourceProvider() : base(HostLanguage.Rust) { }
    }

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("NestLight quick info (Go)")]
    [ContentType("Go")]
    internal sealed class GoQuickInfoSourceProvider : NestLightQuickInfoSourceProvider
    {
        public GoQuickInfoSourceProvider() : base(HostLanguage.Go) { }
    }

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("NestLight quick info (Java)")]
    [ContentType("Java")]
    internal sealed class JavaQuickInfoSourceProvider : NestLightQuickInfoSourceProvider
    {
        public JavaQuickInfoSourceProvider() : base(HostLanguage.Java) { }
    }

    internal abstract class NestLightQuickInfoSourceProvider : IAsyncQuickInfoSourceProvider
    {
        private readonly HostLanguage _host;

        protected NestLightQuickInfoSourceProvider(HostLanguage host) { _host = host; }

        public IAsyncQuickInfoSource CreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(
                typeof(NestLightQuickInfoSource),
                () => new NestLightQuickInfoSource(textBuffer, NestLightBuffer.Get(textBuffer, _host)));
        }
    }

    internal sealed class NestLightQuickInfoSource : IAsyncQuickInfoSource
    {
        private readonly ITextBuffer _buffer;
        private readonly NestLightBuffer _shared;
        private readonly SnapshotTokenCache<ITextSnapshot> _cache;

        public NestLightQuickInfoSource(ITextBuffer buffer, NestLightBuffer shared)
        {
            _buffer = buffer;
            _shared = shared;
            _cache = new SnapshotTokenCache<ITextSnapshot>(shared.Analysis.Highlighter, shared.Text.Of);
        }

        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            SnapshotPoint? triggerPoint = session.GetTriggerPoint(_buffer.CurrentSnapshot);
            if (!triggerPoint.HasValue) return Task.FromResult<QuickInfoItem>(null);

            ITextSnapshot snapshot = triggerPoint.Value.Snapshot;
            int position = triggerPoint.Value.Position;
            string text = _shared.Text.Of(snapshot);

            var tokens = _cache.TokensIn(snapshot, 0, snapshot.Length);
            QuickInfoResult? result = QuickInfoEngine.GetInfo(tokens as System.Collections.Generic.IReadOnlyList<Token> ?? new System.Collections.Generic.List<Token>(tokens), position, text);

            if (!result.HasValue) return Task.FromResult<QuickInfoItem>(null);

            var info = result.Value;
            if (info.Start + info.Length > snapshot.Length) return Task.FromResult<QuickInfoItem>(null);

            var span = snapshot.CreateTrackingSpan(info.Start, info.Length, SpanTrackingMode.EdgeInclusive);
            object element;
            if (info.Color.HasValue)
            {
                var c = info.Color.Value;
                var swatch = new Border
                {
                    Width = 14, Height = 14,
                    Background = new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B)),
                    BorderBrush = new SolidColorBrush(Colors.Gray),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(2),
                    Margin = new Thickness(0, 0, 6, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };
                element = new ContainerElement(ContainerElementStyle.Wrapped,
                    swatch,
                    new ClassifiedTextElement(new ClassifiedTextRun("keyword", info.Description)));
            }
            else
            {
                element = new ContainerElement(ContainerElementStyle.Stacked,
                    new ClassifiedTextElement(new ClassifiedTextRun("keyword", info.Description)));
            }
            return Task.FromResult(new QuickInfoItem(span, element));
        }

        public void Dispose() { }
    }
}
