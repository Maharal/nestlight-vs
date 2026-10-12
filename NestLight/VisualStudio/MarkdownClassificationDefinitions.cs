using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;

namespace NestLight.VisualStudio
{
    internal static class MarkdownClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdHeading)]
        internal static ClassificationTypeDefinition Heading;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdEmphasis)]
        internal static ClassificationTypeDefinition Emphasis;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdStrong)]
        internal static ClassificationTypeDefinition Strong;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdCode)]
        internal static ClassificationTypeDefinition Code;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdLink)]
        internal static ClassificationTypeDefinition Link;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdList)]
        internal static ClassificationTypeDefinition List;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdMath)]
        internal static ClassificationTypeDefinition Math;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdHeading)]
    [Name("Template Markdown Heading Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownHeadingFormat : TplFormat { public MarkdownHeadingFormat() : base("Template Markdown Heading", 0x56, 0x9C, 0xD6, bold: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdEmphasis)]
    [Name("Template Markdown Emphasis Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownEmphasisFormat : TplFormat { public MarkdownEmphasisFormat() : base("Template Markdown Emphasis", 0x9C, 0xDC, 0xFE, italic: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdStrong)]
    [Name("Template Markdown Strong Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownStrongFormat : TplFormat { public MarkdownStrongFormat() : base("Template Markdown Strong", 0x9C, 0xDC, 0xFE, bold: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdCode)]
    [Name("Template Markdown Code Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownCodeFormat : TplFormat { public MarkdownCodeFormat() : base("Template Markdown Code", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdLink)]
    [Name("Template Markdown Link Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownLinkFormat : TplFormat { public MarkdownLinkFormat() : base("Template Markdown Link", 0x4F, 0xA3, 0xE0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdList)]
    [Name("Template Markdown List Marker Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownListFormat : TplFormat { public MarkdownListFormat() : base("Template Markdown List Marker", 0xC5, 0x86, 0xC0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdMath)]
    [Name("Template Markdown Math Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownMathFormat : TplFormat { public MarkdownMathFormat() : base("Template Markdown Math", 0xB5, 0xCE, 0xA8) { } }
}
