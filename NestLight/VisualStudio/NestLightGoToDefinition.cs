using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;
using NestLight.Highlighting;

namespace NestLight.VisualStudio
{
    [Export(typeof(IPeekableItemSourceProvider))]
    [Name("NestLight Go-to-Definition")]
    [ContentType("TypeScript")][ContentType("JavaScript")]
    [ContentType("CSharp")][ContentType("Python")][ContentType("C/C++")]
    [SupportsStandaloneFiles(true)]
    internal sealed class NestLightPeekableItemSourceProvider : IPeekableItemSourceProvider
    {
        public IPeekableItemSource TryCreatePeekableItemSource(ITextBuffer textBuffer)
        {
            return null; // reserved for future implementation
        }
    }

    /// <summary>
    /// Navigates from a CSS class name in HTML to its definition in a CSS string in the same document.
    /// This is an <see cref="ITextViewCreationListener"/> that listens for the go-to-definition command
    /// and, when the caret is on a class name inside an HTML string, jumps to the CSS rule that defines it.
    /// </summary>
    [Export(typeof(ITextViewCreationListener))]
    [ContentType("TypeScript")][ContentType("JavaScript")]
    [ContentType("CSharp")][ContentType("Python")][ContentType("C/C++")]
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

        /// <summary>
        /// Tries to navigate to the definition of the CSS class or ID under the caret. Returns true when a definition was found and the caret was moved.
        /// </summary>
        public bool TryNavigate()
        {
            ITextSnapshot snapshot = _textView.TextSnapshot;
            int position = _textView.Caret.Position.BufferPosition.Position;
            string text = snapshot.GetText();

            NestLightBuffer shared;
            if (!_textView.TextBuffer.Properties.TryGetProperty(typeof(NestLightBuffer), out shared))
                return false;

            IReadOnlyList<Token> tokens = shared.Analysis.Highlighter.Highlight(text);

            Token current = null;
            foreach (Token t in tokens)
            {
                if (t.Start <= position && t.End > position) { current = t; break; }
                if (t.Start > position) break;
            }

            if (current == null) return false;

            string target = null;
            string targetType = null;

            if (current.Type == ClassificationNames.AttributeValue)
            {
                int attrStart = -1;
                foreach (Token t in tokens)
                {
                    if (t.End <= current.Start && (t.Type == ClassificationNames.Attribute || t.Type == ClassificationNames.AttributeEvent
                        || t.Type == ClassificationNames.AttributeProperty || t.Type == ClassificationNames.AttributeBoolean))
                        attrStart = t.Start;
                    if (t.Start >= current.Start) break;
                }
                if (attrStart >= 0)
                {
                    string attrName = null;
                    foreach (Token t in tokens)
                    {
                        if (t.Start == attrStart) { attrName = text.Substring(t.Start, t.Length).ToLowerInvariant(); break; }
                    }
                    if (attrName == "class" || attrName == "classname")
                    {
                        string value = text.Substring(current.Start, current.Length).Trim('"', '\'');
                        string[] classes = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        int localOffset = position - current.Start;
                        string unquoted = text.Substring(current.Start, current.Length);
                        if (unquoted.Length > 0 && (unquoted[0] == '"' || unquoted[0] == '\'')) localOffset--;
                        int at = 0;
                        foreach (string cls in classes)
                        {
                            int idx = value.IndexOf(cls, at, StringComparison.Ordinal);
                            if (localOffset >= idx && localOffset < idx + cls.Length)
                            {
                                target = "." + cls;
                                targetType = ClassificationNames.CssSelectorClass;
                                break;
                            }
                            at = idx + cls.Length;
                        }
                    }
                    else if (attrName == "id")
                    {
                        string value = text.Substring(current.Start, current.Length).Trim('"', '\'');
                        target = "#" + value;
                        targetType = ClassificationNames.CssSelectorId;
                    }
                }
            }
            else if (current.Type == ClassificationNames.CssSelectorClass)
            {
                target = text.Substring(current.Start, current.Length);
                targetType = ClassificationNames.CssSelectorClass;
            }
            else if (current.Type == ClassificationNames.CssSelectorId)
            {
                target = text.Substring(current.Start, current.Length);
                targetType = ClassificationNames.CssSelectorId;
            }

            if (target == null) return false;

            foreach (Token t in tokens)
            {
                if (t.Type != targetType) continue;
                if (t.Start == current.Start && t.Length == current.Length) continue;
                string candidate = text.Substring(t.Start, t.Length);
                if (string.Equals(candidate, target, StringComparison.OrdinalIgnoreCase))
                {
                    SnapshotPoint point = new SnapshotPoint(snapshot, t.Start);
                    _textView.Caret.MoveTo(point);
                    _textView.ViewScroller.EnsureSpanVisible(new SnapshotSpan(point, t.Length));
                    return true;
                }
            }

            return false;
        }
    }
}
