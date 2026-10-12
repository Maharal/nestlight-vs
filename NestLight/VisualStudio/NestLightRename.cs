using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    internal sealed class NestLightRenameListener : ITextViewCreationListener
    {
        public void TextViewCreated(ITextView textView)
        {
            textView.Properties.GetOrCreateSingletonProperty(
                typeof(NestLightRenameHandler),
                () => new NestLightRenameHandler(textView));
        }
    }

    internal sealed class NestLightRenameHandler
    {
        private readonly ITextView _textView;

        public NestLightRenameHandler(ITextView textView)
        {
            _textView = textView;
        }

        public bool TryRename(string newName)
        {
            ITextSnapshot snapshot = _textView.TextSnapshot;
            int position = _textView.Caret.Position.BufferPosition.Position;
            string text = snapshot.GetText();

            NestLightBuffer shared;
            if (!_textView.TextBuffer.Properties.TryGetProperty(typeof(NestLightBuffer), out shared))
                return false;

            IReadOnlyList<Token> tokens = shared.Analysis.Highlighter.Highlight(text);
            IReadOnlyList<RenameReference> references = RenameEngine.FindReferences(tokens, text, position);
            if (references == null || references.Count == 0) return false;

            using (var edit = _textView.TextBuffer.CreateEdit())
            {
                for (int i = references.Count - 1; i >= 0; i--)
                {
                    var r = references[i];
                    edit.Replace(r.Start, r.Length, newName);
                }
                edit.Apply();
            }
            return true;
        }

        public IReadOnlyList<RenameReference> FindReferences()
        {
            ITextSnapshot snapshot = _textView.TextSnapshot;
            int position = _textView.Caret.Position.BufferPosition.Position;
            string text = snapshot.GetText();

            NestLightBuffer shared;
            if (!_textView.TextBuffer.Properties.TryGetProperty(typeof(NestLightBuffer), out shared))
                return null;

            IReadOnlyList<Token> tokens = shared.Analysis.Highlighter.Highlight(text);
            return RenameEngine.FindReferences(tokens, text, position);
        }
    }
}
