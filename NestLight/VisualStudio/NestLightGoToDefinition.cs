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
        public void TextViewCreated(ITextView textView)
        {
            textView.Properties.GetOrCreateSingletonProperty(
                typeof(NestLightGoToDefinitionHandler),
                () => new NestLightGoToDefinitionHandler(textView));
        }
    }

    internal sealed class NestLightGoToDefinitionHandler
    {
        private readonly ITextView _textView;

        public NestLightGoToDefinitionHandler(ITextView textView)
        {
            _textView = textView;
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
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens, text, position);
            if (!result.HasValue) return false;

            var def = result.Value;
            SnapshotPoint point = new SnapshotPoint(snapshot, def.Start);
            _textView.Caret.MoveTo(point);
            _textView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(point, def.Length));
            return true;
        }
    }
}
