using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;

namespace NestLight.VisualStudio
{
    internal static class RegexClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.RegexGroup)]
        internal static ClassificationTypeDefinition Group;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.RegexClass)]
        internal static ClassificationTypeDefinition Class;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.RegexQuantifier)]
        internal static ClassificationTypeDefinition Quantifier;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.RegexEscape)]
        internal static ClassificationTypeDefinition Escape;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.RegexAnchor)]
        internal static ClassificationTypeDefinition Anchor;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.RegexGroup)]
    [Name("Template Regex Group Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class RegexGroupFormat : TplFormat { public RegexGroupFormat() : base("Template Regex Group", 0x56, 0x9C, 0xD6) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.RegexClass)]
    [Name("Template Regex Character Class Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class RegexClassFormat : TplFormat { public RegexClassFormat() : base("Template Regex Character Class", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.RegexQuantifier)]
    [Name("Template Regex Quantifier Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class RegexQuantifierFormat : TplFormat { public RegexQuantifierFormat() : base("Template Regex Quantifier", 0xC5, 0x86, 0xC0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.RegexEscape)]
    [Name("Template Regex Escape Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class RegexEscapeFormat : TplFormat { public RegexEscapeFormat() : base("Template Regex Escape", 0xD7, 0xBA, 0x7D) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.RegexAnchor)]
    [Name("Template Regex Anchor Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class RegexAnchorFormat : TplFormat { public RegexAnchorFormat() : base("Template Regex Anchor", 0xD6, 0x7B, 0x3B) { } }
}
