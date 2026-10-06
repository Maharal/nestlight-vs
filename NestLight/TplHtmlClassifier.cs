using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace NestLight
{
    [Export(typeof(IClassifierProvider))]
    [ContentType("TypeScript")]
    [ContentType("JavaScript")]
    internal sealed class TplHtmlClassifierProvider : IClassifierProvider
    {
        [Import]
        internal IClassificationTypeRegistryService Registry { get; set; }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return buffer.Properties.GetOrCreateSingletonProperty(
                () => new TplHtmlClassifier(buffer, Registry));
        }
    }

    internal sealed class TplHtmlClassifier : IClassifier
    {
        private readonly ITextBuffer _buffer;
        private readonly IClassificationTypeRegistryService _registry;
        private readonly Dictionary<string, IClassificationType> _types = new Dictionary<string, IClassificationType>();
        private readonly object _gate = new object();

        private ITextSnapshot _snapshot;
        private List<TplToken> _tokens = new List<TplToken>();

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

        public TplHtmlClassifier(ITextBuffer buffer, IClassificationTypeRegistryService registry)
        {
            _buffer = buffer;
            _registry = registry;
            _buffer.ChangedLowPriority += OnBufferChanged;
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            var snap = e.After;
            var handler = ClassificationChanged;
            if (handler != null)
                handler(this, new ClassificationChangedEventArgs(new SnapshotSpan(snap, 0, snap.Length)));
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            var result = new List<ClassificationSpan>();
            var snap = span.Snapshot;

            List<TplToken> tokens;
            lock (_gate)
            {
                if (!ReferenceEquals(_snapshot, snap))
                {
                    _tokens = Analyze(snap);
                    _snapshot = snap;
                }
                tokens = _tokens;
            }

            foreach (var t in tokens)
            {
                if (t.End <= span.Start) continue;
                if (t.Start >= span.End) break;
                if (t.Length <= 0 || t.End > snap.Length) continue;
                result.Add(new ClassificationSpan(new SnapshotSpan(snap, t.Start, t.Length), GetType(t.Type)));
            }
            return result;
        }

        private IClassificationType GetType(string name)
        {
            IClassificationType type;
            if (!_types.TryGetValue(name, out type))
            {
                type = _registry.GetClassificationType(name);
                _types[name] = type;
            }
            return type;
        }

        private static List<TplToken> Analyze(ITextSnapshot snapshot)
        {
            var tokens = new List<TplToken>();
            try
            {
                string text = snapshot.GetText();
                var templates = TplScanner.FindTemplates(text);
                foreach (var tpl in templates)
                    TplHtmlTokenizer.Tokenize(text, tpl, templates, tokens);
                tokens.Sort((a, b) => a.Start.CompareTo(b.Start));
            }
            catch (Exception)
            {
                // Never take down the editor because of colorization.
                tokens.Clear();
            }
            return tokens;
        }
    }
}
