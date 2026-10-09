using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;

namespace NestLight.VisualStudio
{
    // ---- Classification types (CSS) -----------------------------------------
    internal static class TplCssClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssSelector)]
        internal static ClassificationTypeDefinition Selector;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssSelectorClass)]
        internal static ClassificationTypeDefinition SelectorClass;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssSelectorId)]
        internal static ClassificationTypeDefinition SelectorId;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssAttribute)]
        internal static ClassificationTypeDefinition Attribute;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssUnit)]
        internal static ClassificationTypeDefinition Unit;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssImportant)]
        internal static ClassificationTypeDefinition Important;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssPseudo)]
        internal static ClassificationTypeDefinition Pseudo;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssProperty)]
        internal static ClassificationTypeDefinition Property;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssCustomProperty)]
        internal static ClassificationTypeDefinition CustomProperty;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssValue)]
        internal static ClassificationTypeDefinition Value;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssNumber)]
        internal static ClassificationTypeDefinition Number;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssFunction)]
        internal static ClassificationTypeDefinition Function;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssAtRule)]
        internal static ClassificationTypeDefinition AtRule;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssString)]
        internal static ClassificationTypeDefinition String;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssPunct)]
        internal static ClassificationTypeDefinition Punct;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.CssComment)]
        internal static ClassificationTypeDefinition Comment;
    }

    // ---- Formats (default colors; editable in Tools > Options > Environment > Fonts and Colors) ----
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssSelector)]
    [Name("Template CSS Selector Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssSelectorFormat : TplFormat { public TplCssSelectorFormat() : base("Template CSS Selector", 0xA6, 0x77, 0x25) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssSelectorClass)]
    [Name("Template CSS Class/Id Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssSelectorClassFormat : TplFormat { public TplCssSelectorClassFormat() : base("Template CSS Class", 0xC2, 0x60, 0x7E) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssPseudo)]
    [Name("Template CSS Pseudo Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssPseudoFormat : TplFormat { public TplCssPseudoFormat() : base("Template CSS Pseudo-class / element", 0xB0, 0x5E, 0xC8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssProperty)]
    [Name("Template CSS Property Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssPropertyFormat : TplFormat { public TplCssPropertyFormat() : base("Template CSS Property", 0x36, 0x8B, 0x96) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssCustomProperty)]
    [Name("Template CSS Custom Property Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssCustomPropertyFormat : TplFormat { public TplCssCustomPropertyFormat() : base("Template CSS Custom Property (--x)", 0x4C, 0x7C, 0xE0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssValue)]
    [Name("Template CSS Value Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssValueFormat : TplFormat { public TplCssValueFormat() : base("Template CSS Value", 0xBD, 0x6A, 0x48) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssNumber)]
    [Name("Template CSS Number Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssNumberFormat : TplFormat { public TplCssNumberFormat() : base("Template CSS Number / Color", 0x30, 0x90, 0x30) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssFunction)]
    [Name("Template CSS Function Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssFunctionFormat : TplFormat { public TplCssFunctionFormat() : base("Template CSS Function", 0x80, 0x80, 0xC0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssAtRule)]
    [Name("Template CSS At-rule Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssAtRuleFormat : TplFormat { public TplCssAtRuleFormat() : base("Template CSS At-rule", 0xD0, 0x40, 0xB0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssString)]
    [Name("Template CSS String Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssStringFormat : TplFormat { public TplCssStringFormat() : base("Template CSS String", 0x97, 0x7C, 0x4B) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssPunct)]
    [Name("Template CSS Punctuation Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssPunctFormat : TplFormat { public TplCssPunctFormat() : base("Template CSS Punctuation", 0x7F, 0x7F, 0x7F) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssComment)]
    [Name("Template CSS Comment Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssCommentFormat : TplFormat { public TplCssCommentFormat() : base("Template CSS Comment", 0x60, 0x80, 0x40, italic: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssSelectorId)]
    [Name("Template CSS Id Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssSelectorIdFormat : TplFormat { public TplCssSelectorIdFormat() : base("Template CSS Id", 0xC2, 0x60, 0x7E, bold: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssAttribute)]
    [Name("Template CSS Attribute Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssAttributeFormat : TplFormat { public TplCssAttributeFormat() : base("Template CSS Attribute (in a selector)", 0x36, 0x8B, 0x96, italic: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssUnit)]
    [Name("Template CSS Unit Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssUnitFormat : TplFormat { public TplCssUnitFormat() : base("Template CSS Unit (px, rem, %)", 0x30, 0x90, 0x60) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssImportant)]
    [Name("Template CSS Important Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssImportantFormat : TplFormat { public TplCssImportantFormat() : base("Template CSS !important", 0xD0, 0x40, 0xB0, bold: true) { } }
}
