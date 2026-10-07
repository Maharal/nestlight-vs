using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.VisualStudio
{
    // One provider per host language, like the classifiers. They only differ in the content types they serve.

    [Export(typeof(IAsyncCompletionSourceProvider))]
    [Name("NestLight completion (JavaScript / TypeScript)")]
    [ContentType("TypeScript")]
    [ContentType("JavaScript")]
    internal sealed class JavaScriptCompletionSourceProvider : NestLightCompletionSourceProvider
    {
        public JavaScriptCompletionSourceProvider() : base(HostLanguage.JavaScript) { }
    }

    [Export(typeof(IAsyncCompletionSourceProvider))]
    [Name("NestLight completion (C#)")]
    [ContentType("CSharp")]
    internal sealed class CSharpCompletionSourceProvider : NestLightCompletionSourceProvider
    {
        public CSharpCompletionSourceProvider() : base(HostLanguage.CSharp) { }
    }

    [Export(typeof(IAsyncCompletionSourceProvider))]
    [Name("NestLight completion (Python)")]
    [ContentType("Python")]
    internal sealed class PythonCompletionSourceProvider : NestLightCompletionSourceProvider
    {
        public PythonCompletionSourceProvider() : base(HostLanguage.Python) { }
    }

    [Export(typeof(IAsyncCompletionSourceProvider))]
    [Name("NestLight completion (C++)")]
    [ContentType("C/C++")]
    internal sealed class CppCompletionSourceProvider : NestLightCompletionSourceProvider
    {
        public CppCompletionSourceProvider() : base(HostLanguage.Cpp) { }
    }

    internal abstract class NestLightCompletionSourceProvider : IAsyncCompletionSourceProvider
    {
        private readonly HostLanguage _host;

        protected NestLightCompletionSourceProvider(HostLanguage host)
        {
            _host = host;
        }

        public IAsyncCompletionSource GetOrCreate(ITextView textView)
        {
            return textView.Properties.GetOrCreateSingletonProperty(
                typeof(NestLightCompletionSource),
                () => new NestLightCompletionSource(NestLightBuffer.Get(textView.TextBuffer, _host)));
        }
    }

    /// <summary>
    /// The adapter between the editor and the completion engine. It adds its items to the ones other providers offer,
    /// and only inside the code of an embedded string: the host language keeps its own completion everywhere else.
    /// </summary>
    internal sealed class NestLightCompletionSource : IAsyncCompletionSource
    {
        private const string KindKey = "NestLight.Kind";
        private const string LanguageKey = "NestLight.Language";

        private readonly ICompletionProvider _completion;
        private readonly SnapshotTextCache<ITextSnapshot> _text;

        public NestLightCompletionSource(NestLightBuffer shared)
        {
            _completion = shared.Analysis.Completion;
            _text = shared.Text;
        }

        public CompletionStartData InitializeCompletion(CompletionTrigger trigger, SnapshotPoint triggerLocation, CancellationToken token)
        {
            // Typing or the explicit command (Ctrl+Space); deleting characters, for example, does not open the list.
            if (trigger.Reason != CompletionTriggerReason.Insertion && trigger.Reason != CompletionTriggerReason.Invoke
                && trigger.Reason != CompletionTriggerReason.InvokeAndCommitIfUnique)
                return CompletionStartData.DoesNotParticipateInCompletion;

            ITextSnapshot snapshot = triggerLocation.Snapshot;
            CompletionSite site = _completion.Locate(_text.Of(snapshot), triggerLocation.Position);
            if (site == null) return CompletionStartData.DoesNotParticipateInCompletion;

            // While typing, a word starts the list; punctuation and blanks do not.
            bool explicitRequest = trigger.Reason != CompletionTriggerReason.Insertion;
            if (!explicitRequest && site.PrefixLength == 0) return CompletionStartData.DoesNotParticipateInCompletion;

            return new CompletionStartData(CompletionParticipation.ProvidesItems, new SnapshotSpan(snapshot, site.Start, site.End - site.Start));
        }

        public Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger,
            SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token)
        {
            ITextSnapshot snapshot = triggerLocation.Snapshot;
            string text = _text.Of(snapshot);
            CompletionSite site = _completion.Locate(text, triggerLocation.Position);
            if (site == null) return Task.FromResult(CompletionContext.Empty);

            IReadOnlyList<Suggestion> suggestions = _completion.Suggest(text, site);
            var items = ImmutableArray.CreateBuilder<CompletionItem>(suggestions.Count);
            foreach (Suggestion suggestion in suggestions)
            {
                var item = new CompletionItem(suggestion.Text, this);
                item.Properties.AddProperty(KindKey, suggestion.Kind);
                item.Properties.AddProperty(LanguageKey, site.LanguageId);
                items.Add(item);
            }
            return Task.FromResult(new CompletionContext(items.ToImmutable()));
        }

        public Task<object> GetDescriptionAsync(IAsyncCompletionSession session, CompletionItem item, CancellationToken token)
        {
            SuggestionKind kind;
            string language;
            if (!item.Properties.TryGetProperty(KindKey, out kind) || !item.Properties.TryGetProperty(LanguageKey, out language))
                return Task.FromResult<object>(null);

            string description = kind == SuggestionKind.Keyword
                ? language + " keyword"
                : "Word in the document";
            return Task.FromResult<object>(description);
        }
    }
}
