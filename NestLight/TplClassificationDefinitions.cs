using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace NestLight
{
    // ---- Classification types ------------------------------------------------
    internal static class TplClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.Tag)]
        internal static ClassificationTypeDefinition Tag;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.Delimiter)]
        internal static ClassificationTypeDefinition Delimiter;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.Attribute)]
        internal static ClassificationTypeDefinition Attribute;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.AttributeEvent)]
        internal static ClassificationTypeDefinition AttributeEvent;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.AttributeProperty)]
        internal static ClassificationTypeDefinition AttributeProperty;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.AttributeBoolean)]
        internal static ClassificationTypeDefinition AttributeBoolean;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.AttributeValue)]
        internal static ClassificationTypeDefinition AttributeValue;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.Comment)]
        internal static ClassificationTypeDefinition Comment;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.ExprDelimiter)]
        internal static ClassificationTypeDefinition ExprDelimiter;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.Expression)]
        [BaseDefinition("identifier")]
        internal static ClassificationTypeDefinition Expression;
    }

    // ---- Formats (default colors; editable in Tools > Options > Environment > Fonts and Colors) ----
    internal abstract class TplFormat : ClassificationFormatDefinition
    {
        protected TplFormat(string display, byte r, byte g, byte b, bool italic = false)
        {
            DisplayName = display;
            ForegroundColor = Color.FromRgb(r, g, b);
            if (italic) IsItalic = true;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.Tag)]
    [Name("Template HTML Tag Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplTagFormat : TplFormat { public TplTagFormat() : base("Template HTML Tag", 0x4A, 0x90, 0xD9) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.Delimiter)]
    [Name("Template HTML Delimiter Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplDelimiterFormat : TplFormat { public TplDelimiterFormat() : base("Template HTML Delimiter", 0x90, 0x90, 0x90) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.Attribute)]
    [Name("Template HTML Attribute Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplAttributeFormat : TplFormat { public TplAttributeFormat() : base("Template HTML Attribute", 0x56, 0xB0, 0xC8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.AttributeEvent)]
    [Name("Template HTML Event Binding Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplEventFormat : TplFormat { public TplEventFormat() : base("Template HTML Event Binding (@event)", 0xB2, 0x61, 0xC9) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.AttributeProperty)]
    [Name("Template HTML Property Binding Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplPropertyFormat : TplFormat { public TplPropertyFormat() : base("Template HTML Property Binding (.prop)", 0x2F, 0xA6, 0x7A) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.AttributeBoolean)]
    [Name("Template HTML Boolean Binding Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplBooleanFormat : TplFormat { public TplBooleanFormat() : base("Template HTML Boolean Binding (?attr)", 0xC9, 0xA2, 0x27) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.AttributeValue)]
    [Name("Template HTML Attribute Value Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplValueFormat : TplFormat { public TplValueFormat() : base("Template HTML Attribute Value", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.Comment)]
    [Name("Template HTML Comment Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCommentFormat : TplFormat { public TplCommentFormat() : base("Template HTML Comment", 0x6A, 0x99, 0x55, italic: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.ExprDelimiter)]
    [Name("Template Expression Delimiter Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplExprDelimiterFormat : TplFormat { public TplExprDelimiterFormat() : base("Template Expression Delimiter (${ })", 0xD6, 0x7B, 0x3B) { } }

    // No color of its own: inherits from "Identifier" to "undo" the string color inside ${ }.
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.Expression)]
    [Name("Template Expression Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplExpressionFormat : ClassificationFormatDefinition
    {
        public TplExpressionFormat() { DisplayName = "Template Expression"; }
    }
}
