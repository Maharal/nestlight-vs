using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
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
        private const string DistanceKey = "NestLight.Distance";

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

            IReadOnlyList<Suggestion> suggestions;
            try
            {
                suggestions = _completion.Suggest(text, site, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                return Task.FromCanceled<CompletionContext>(token);
            }

            string typed = text.Substring(site.Start, site.PrefixLength);
            var items = ImmutableArray.CreateBuilder<CompletionItem>(suggestions.Count);
            int rank = 0;
            foreach (Suggestion suggestion in suggestions)
            {
                // The editor sorts the list by the sort text, which is the text of the item unless told otherwise: without the rank the
                // order of the engine (the context first, then the nearest word) would be lost. A similar word does not start with what
                // was typed; the editor filters the list against the text of the span at every key, so the item is filtered by what was
                // typed, while it shows and inserts the word it suggests.
                string sortText = (rank++).ToString("D5", CultureInfo.InvariantCulture);
                string filterText = suggestion.Distance == 0 ? suggestion.Text : typed;
                CompletionItem item = new CompletionItem(suggestion.Text, this, default(ImageElement), ImmutableArray<CompletionFilter>.Empty, string.Empty,
                    suggestion.Text, sortText, filterText, ImmutableArray<ImageElement>.Empty);
                item.Properties.AddProperty(KindKey, suggestion.Kind);
                item.Properties.AddProperty(LanguageKey, site.EmbeddedLanguageId);
                item.Properties.AddProperty(DistanceKey, suggestion.Distance);
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

            int distance;
            if (!item.Properties.TryGetProperty(DistanceKey, out distance)) distance = 0;

            string description = kind == SuggestionKind.Keyword
                ? (distance == 0 ? language + " keyword" : "Similar " + language + " keyword")
                : (distance == 0 ? "Word in the document" : "Similar word in the document");
            return Task.FromResult<object>(description);
        }
    }
}
