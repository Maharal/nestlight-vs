using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;

namespace NestLight.VisualStudio
{
    internal static class YAMLClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.YamlKey)]
        internal static ClassificationTypeDefinition Key;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.YamlString)]
        internal static ClassificationTypeDefinition String;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.YamlScalar)]
        internal static ClassificationTypeDefinition Scalar;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.YamlAnchor)]
        internal static ClassificationTypeDefinition Anchor;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.YamlComment)]
        internal static ClassificationTypeDefinition Comment;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.YamlPunct)]
        internal static ClassificationTypeDefinition Punctuation;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.YamlKey)]
    [Name("Template YAML Key Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class YAMLKeyFormat : TplFormat { public YAMLKeyFormat() : base("Template YAML Key", 0x9C, 0xDC, 0xFE) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.YamlString)]
    [Name("Template YAML String Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class YAMLStringFormat : TplFormat { public YAMLStringFormat() : base("Template YAML String", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.YamlScalar)]
    [Name("Template YAML Scalar (number / bool / null) Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class YAMLScalarFormat : TplFormat { public YAMLScalarFormat() : base("Template YAML Scalar (number / bool / null)", 0xB5, 0xCE, 0xA8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.YamlAnchor)]
    [Name("Template YAML Anchor / Alias / Tag Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class YAMLAnchorFormat : TplFormat { public YAMLAnchorFormat() : base("Template YAML Anchor / Alias / Tag", 0xC5, 0x86, 0xC0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.YamlComment)]
    [Name("Template YAML Comment Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class YAMLCommentFormat : TplFormat { public YAMLCommentFormat() : base("Template YAML Comment", 0x6A, 0x99, 0x55, italic: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.YamlPunct)]
    [Name("Template YAML Punctuation Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class YAMLPunctuationFormat : TplFormat { public YAMLPunctuationFormat() : base("Template YAML Punctuation", 0x90, 0x90, 0x90) { } }
}
