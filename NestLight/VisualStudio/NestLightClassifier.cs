using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;
using NestLight.Detection;
using NestLight.Highlighting;

namespace NestLight.VisualStudio
{
    // One provider per host language. They only differ in the content types they serve and in the host they ask for.

    [Export(typeof(IClassifierProvider))]
    [ContentType("TypeScript")]
    [ContentType("JavaScript")]
    internal sealed class JavaScriptClassifierProvider : NestLightClassifierProvider
    {
        public JavaScriptClassifierProvider() : base(HostLanguage.JavaScript) { }
    }

    [Export(typeof(IClassifierProvider))]
    [ContentType("CSharp")]
    internal sealed class CSharpClassifierProvider : NestLightClassifierProvider
    {
        public CSharpClassifierProvider() : base(HostLanguage.CSharp) { }
    }

    [Export(typeof(IClassifierProvider))]
    [ContentType("Python")]
    internal sealed class PythonClassifierProvider : NestLightClassifierProvider
    {
        public PythonClassifierProvider() : base(HostLanguage.Python) { }
    }

    [Export(typeof(IClassifierProvider))]
    [ContentType("C/C++")]
    internal sealed class CppClassifierProvider : NestLightClassifierProvider
    {
        public CppClassifierProvider() : base(HostLanguage.Cpp) { }
    }

    internal abstract class NestLightClassifierProvider : IClassifierProvider
    {
        private readonly HostLanguage _host;

        protected NestLightClassifierProvider(HostLanguage host)
        {
            _host = host;
        }

        [Import]
        internal IClassificationTypeRegistryService Registry { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(
                typeof(NestLightClassifier),
                () => new NestLightClassifier(buffer, Registry, NestLightBuffer.Get(buffer, _host)));
        }
    }

    /// <summary>The adapter between the editor and the highlighter: it only converts tokens to classification spans.</summary>
    internal sealed class NestLightClassifier : IClassifier
    {
        private readonly IClassificationTypeRegistryService _registry;
        private readonly SnapshotTokenCache<ITextSnapshot> _cache;
        private readonly Dictionary<string, IClassificationType> _types = new Dictionary<string, IClassificationType>();

        private readonly ITextBuffer _buffer;

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public NestLightClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry, NestLightBuffer shared)
        {
            _registry = registry;
            _buffer = buffer;
            DetectionOptions options = shared.Options;
            _cache = new SnapshotTokenCache<ITextSnapshot>(shared.Analysis.Highlighter, shared.Text.Of, () => options.Version);
            buffer.ChangedLowPriority += OnBufferChanged;
            new WeakOptionsListener(options, this);
        }

        /// <summary>The detection options changed (from the options page): everything in the buffer may have another color now.</summary>
        private void OnOptionsChanged()
        {
            var handler = ClassificationChanged;
            ITextSnapshot snapshot = _buffer.CurrentSnapshot;
            if (handler != null)
                handler(this, new ClassificationChangedEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
        }

        /// <summary>
        /// The options outlive every buffer. A listener that held the classifier would keep a closed document alive, so it holds it weakly
        /// and unsubscribes itself the first time the classifier is gone.
        /// </summary>
        private sealed class WeakOptionsListener
        {
            private readonly DetectionOptions _options;
            private readonly WeakReference<NestLightClassifier> _target;

            public WeakOptionsListener(DetectionOptions options, NestLightClassifier target)
            {
                _options = options;
                _target = new WeakReference<NestLightClassifier>(target);
                options.Changed += OnChanged;
            }

            private void OnChanged(object sender, EventArgs e)
            {
                NestLightClassifier classifier;
                if (_target.TryGetTarget(out classifier)) classifier.OnOptionsChanged();
                else _options.Changed -= OnChanged;
            }
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            var handler = ClassificationChanged;
            if (handler != null)
                handler(this, new ClassificationChangedEventArgs(new SnapshotSpan(e.After, 0, e.After.Length)));
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>();
            ITextSnapshot snapshot = span.Snapshot;
            foreach (Token token in _cache.TokensIn(snapshot, span.Start, span.End))
            {
                if (token.End > snapshot.Length) continue;
                IClassificationType type = TypeOf(token.Type);
                if (type != null) result.Add(new ClassificationSpan(new SnapshotSpan(snapshot, token.Start, token.Length), type));
            }
            return result;
        }

        private IClassificationType TypeOf(string name)
        {
            IClassificationType type;
            if (!_types.TryGetValue(name, out type))
            {
                type = _registry.GetClassificationType(name);
                _types[name] = type;
            }
            return type;
        }
    }

    /// <summary>
    /// What the classifier and the completion of a buffer share: one scan and one copy of the text per snapshot. The first of the two
    /// that needs the snapshot pays for them, the other finds them done.
    /// </summary>
    internal sealed class NestLightBuffer
    {
        private NestLightBuffer(HostLanguage host)
        {
            Options = NestLightOptions.Detection;
            Analysis = NestLightComposition.CreateForBuffer(host, Options);
            Text = new SnapshotTextCache<ITextSnapshot>(snapshot => snapshot.GetText());
        }

        public DetectionOptions Options { get; private set; }
        public BufferAnalysis Analysis { get; private set; }
        public SnapshotTextCache<ITextSnapshot> Text { get; private set; }

        public static NestLightBuffer Get(ITextBuffer buffer, HostLanguage host)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(typeof(NestLightBuffer), () => new NestLightBuffer(host));
        }
    }
}
