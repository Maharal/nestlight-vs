using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;
using NestLight.Highlighting;

namespace NestLight.VisualStudio
{
    [Export(typeof(ITextViewCreationListener))]
    [ContentType("TypeScript")][ContentType("JavaScript")]
    [ContentType("CSharp")][ContentType("Python")][ContentType("C/C++")]
    [ContentType("Rust")][ContentType("Go")][ContentType("Java")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class NestLightGoToDefinitionListener : ITextViewCreationListener
    {
        [Import]
        internal ITextDocumentFactoryService DocumentFactory { get; set; }

        [Import]
        internal ITextBufferFactoryService BufferFactory { get; set; }

        public void TextViewCreated(ITextView textView)
        {
            textView.Properties.GetOrCreateSingletonProperty(
                typeof(NestLightGoToDefinitionHandler),
                () => new NestLightGoToDefinitionHandler(textView, DocumentFactory));
        }
    }

    internal sealed class NestLightGoToDefinitionHandler
    {
        private readonly ITextView _textView;
        private readonly ITextDocumentFactoryService _documentFactory;

        public NestLightGoToDefinitionHandler(ITextView textView, ITextDocumentFactoryService documentFactory)
        {
            _textView = textView;
            _documentFactory = documentFactory;
        }

        public bool TryNavigate()
        {
            ITextSnapshot snapshot = _textView.TextSnapshot;
            int position = _textView.Caret.Position.BufferPosition.Position;
            string text = snapshot.GetText();

            NestLightBuffer shared;
            if (!_textView.TextBuffer.Properties.TryGetProperty(typeof(NestLightBuffer), out shared))
                return false;

            IReadOnlyList<Token> tokens = shared.Analysis.Highlighter.Highlight(text);

            var externalFiles = new List<ExternalFile>();
            var externalBuffers = new List<ITextBuffer>();
            CollectExternalBuffers(externalBuffers, shared, tokens, text);

            for (int i = 0; i < externalBuffers.Count; i++)
            {
                NestLightBuffer extShared;
                if (!externalBuffers[i].Properties.TryGetProperty(typeof(NestLightBuffer), out extShared))
                    continue;
                string extText = externalBuffers[i].CurrentSnapshot.GetText();
                IReadOnlyList<Token> extTokens = extShared.Analysis.Highlighter.Highlight(extText);
                externalFiles.Add(new ExternalFile(extTokens, extText));
            }

            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens, text, position, externalFiles.Count > 0 ? externalFiles : null);
            if (!result.HasValue) return false;

            var def = result.Value;
            if (def.FileIndex >= 0 && def.FileIndex < externalBuffers.Count)
            {
                ITextBuffer targetBuffer = externalBuffers[def.FileIndex];
                ITextSnapshot targetSnapshot = targetBuffer.CurrentSnapshot;
                if (def.Start + def.Length > targetSnapshot.Length) return false;

                ITextDocument document;
                if (_documentFactory != null && _documentFactory.TryGetTextDocument(targetBuffer, out document))
                {
                    try
                    {
                        var dte = (EnvDTE.DTE)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(EnvDTE.DTE));
                        if (dte != null)
                        {
                            EnvDTE.Window window = dte.ItemOperations.OpenFile(document.FilePath);
                            var targetView = GetActiveTextView();
                            if (targetView != null)
                            {
                                SnapshotPoint point = new SnapshotPoint(targetView.TextSnapshot, Math.Min(def.Start, targetView.TextSnapshot.Length));
                                targetView.Caret.MoveTo(point);
                                targetView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(point, Math.Min(def.Length, targetView.TextSnapshot.Length - point.Position)));
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                return true;
            }

            SnapshotPoint localPoint = new SnapshotPoint(snapshot, def.Start);
            _textView.Caret.MoveTo(localPoint);
            _textView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(localPoint, def.Length));
            return true;
        }

        private void CollectExternalBuffers(List<ITextBuffer> buffers, NestLightBuffer currentShared, IReadOnlyList<Token> tokens, string text)
        {
            try
            {
                var dte = (EnvDTE.DTE)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(EnvDTE.DTE));
                if (dte == null) return;
                foreach (EnvDTE.Document doc in dte.Documents)
                {
                    try
                    {
                        if (doc.FullName == null) continue;
                        ITextDocument currentDoc;
                        if (_documentFactory != null && _documentFactory.TryGetTextDocument(_textView.TextBuffer, out currentDoc))
                        {
                            if (string.Equals(currentDoc.FilePath, doc.FullName, StringComparison.OrdinalIgnoreCase))
                                continue;
                        }

                        var textDoc = doc.Object("TextDocument") as EnvDTE.TextDocument;
                        if (textDoc == null) continue;
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }

        private ITextView GetActiveTextView()
        {
            try
            {
                var componentModel = (Microsoft.VisualStudio.ComponentModelHost.IComponentModel)
                    Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.ComponentModelHost.SComponentModel));
                if (componentModel == null) return null;
                var editorAdaptersFactory = componentModel.GetService<Microsoft.VisualStudio.Editor.IVsEditorAdaptersFactoryService>();
                if (editorAdaptersFactory == null) return null;
                var textManager = (Microsoft.VisualStudio.TextManager.Interop.IVsTextManager)
                    Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(Microsoft.VisualStudio.TextManager.Interop.SVsTextManager));
                if (textManager == null) return null;
                Microsoft.VisualStudio.TextManager.Interop.IVsTextView activeView;
                textManager.GetActiveView(1, null, out activeView);
                if (activeView == null) return null;
                return editorAdaptersFactory.GetWpfTextView(activeView);
            }
            catch
            {
                return null;
            }
        }
    }
}
