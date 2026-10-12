using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;

namespace NestLight.VisualStudio
{
    internal static class JSONClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.JsonKey)]
        internal static ClassificationTypeDefinition Key;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.JsonString)]
        internal static ClassificationTypeDefinition String;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.JsonNumber)]
        internal static ClassificationTypeDefinition Number;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.JsonLiteral)]
        internal static ClassificationTypeDefinition Literal;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.JsonPunct)]
        internal static ClassificationTypeDefinition Punctuation;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.JsonKey)]
    [Name("Template JSON Key Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class JSONKeyFormat : TplFormat { public JSONKeyFormat() : base("Template JSON Key", 0x9C, 0xDC, 0xFE) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.JsonString)]
    [Name("Template JSON String Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class JSONStringFormat : TplFormat { public JSONStringFormat() : base("Template JSON String", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.JsonNumber)]
    [Name("Template JSON Number Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class JSONNumberFormat : TplFormat { public JSONNumberFormat() : base("Template JSON Number", 0xB5, 0xCE, 0xA8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.JsonLiteral)]
    [Name("Template JSON Literal (true / false / null) Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class JSONLiteralFormat : TplFormat { public JSONLiteralFormat() : base("Template JSON Literal (true / false / null)", 0x56, 0x9C, 0xD6) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.JsonPunct)]
    [Name("Template JSON Punctuation Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class JSONPunctuationFormat : TplFormat { public JSONPunctuationFormat() : base("Template JSON Punctuation", 0x90, 0x90, 0x90) { } }
}
