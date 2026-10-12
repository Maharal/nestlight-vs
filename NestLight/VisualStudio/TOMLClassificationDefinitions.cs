using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;

namespace NestLight.VisualStudio
{
    internal static class TOMLClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.TomlTable)]
        internal static ClassificationTypeDefinition Table;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.TomlKey)]
        internal static ClassificationTypeDefinition Key;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.TomlString)]
        internal static ClassificationTypeDefinition String;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.TomlNumber)]
        internal static ClassificationTypeDefinition Number;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.TomlLiteral)]
        internal static ClassificationTypeDefinition Literal;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.TomlDate)]
        internal static ClassificationTypeDefinition Date;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.TomlComment)]
        internal static ClassificationTypeDefinition Comment;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.TomlPunct)]
        internal static ClassificationTypeDefinition Punctuation;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.TomlTable)]
    [Name("Template TOML Table Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TOMLTableFormat : TplFormat { public TOMLTableFormat() : base("Template TOML Table", 0x56, 0x9C, 0xD6, bold: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.TomlKey)]
    [Name("Template TOML Key Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TOMLKeyFormat : TplFormat { public TOMLKeyFormat() : base("Template TOML Key", 0x9C, 0xDC, 0xFE) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.TomlString)]
    [Name("Template TOML String Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TOMLStringFormat : TplFormat { public TOMLStringFormat() : base("Template TOML String", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.TomlNumber)]
    [Name("Template TOML Number Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TOMLNumberFormat : TplFormat { public TOMLNumberFormat() : base("Template TOML Number", 0xB5, 0xCE, 0xA8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.TomlLiteral)]
    [Name("Template TOML Literal (true / false) Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TOMLLiteralFormat : TplFormat { public TOMLLiteralFormat() : base("Template TOML Literal (true / false)", 0x56, 0x9C, 0xD6) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.TomlDate)]
    [Name("Template TOML Date Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TOMLDateFormat : TplFormat { public TOMLDateFormat() : base("Template TOML Date", 0xB5, 0x9A, 0x6A) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.TomlComment)]
    [Name("Template TOML Comment Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TOMLCommentFormat : TplFormat { public TOMLCommentFormat() : base("Template TOML Comment", 0x6A, 0x99, 0x55, italic: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.TomlPunct)]
    [Name("Template TOML Punctuation Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TOMLPunctuationFormat : TplFormat { public TOMLPunctuationFormat() : base("Template TOML Punctuation", 0x90, 0x90, 0x90) { } }
}
