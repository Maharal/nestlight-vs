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
    internal sealed class TplCssSelectorFormat : TplFormat { public TplCssSelectorFormat() : base("Template CSS Selector", 0xD6, 0xA2, 0x45) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssSelectorClass)]
    [Name("Template CSS Class/Id Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssSelectorClassFormat : TplFormat { public TplCssSelectorClassFormat() : base("Template CSS Class / Id", 0xC8, 0x6F, 0x8A) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssPseudo)]
    [Name("Template CSS Pseudo Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssPseudoFormat : TplFormat { public TplCssPseudoFormat() : base("Template CSS Pseudo-class / element", 0xB2, 0x61, 0xC9) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssProperty)]
    [Name("Template CSS Property Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssPropertyFormat : TplFormat { public TplCssPropertyFormat() : base("Template CSS Property", 0x56, 0xB6, 0xC2) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssCustomProperty)]
    [Name("Template CSS Custom Property Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssCustomPropertyFormat : TplFormat { public TplCssCustomPropertyFormat() : base("Template CSS Custom Property (--x)", 0x7A, 0x9E, 0xE8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssValue)]
    [Name("Template CSS Value Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssValueFormat : TplFormat { public TplCssValueFormat() : base("Template CSS Value", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssNumber)]
    [Name("Template CSS Number Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssNumberFormat : TplFormat { public TplCssNumberFormat() : base("Template CSS Number / Color", 0x6C, 0xB8, 0x6C) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssFunction)]
    [Name("Template CSS Function Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssFunctionFormat : TplFormat { public TplCssFunctionFormat() : base("Template CSS Function", 0xC2, 0xA3, 0x3A) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssAtRule)]
    [Name("Template CSS At-rule Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssAtRuleFormat : TplFormat { public TplCssAtRuleFormat() : base("Template CSS At-rule / !important", 0xD1, 0x6D, 0xA8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssString)]
    [Name("Template CSS String Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssStringFormat : TplFormat { public TplCssStringFormat() : base("Template CSS String", 0xB5, 0x9A, 0x6A) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssPunct)]
    [Name("Template CSS Punctuation Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssPunctFormat : TplFormat { public TplCssPunctFormat() : base("Template CSS Punctuation", 0x90, 0x90, 0x90) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.CssComment)]
    [Name("Template CSS Comment Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssCommentFormat : TplFormat { public TplCssCommentFormat() : base("Template CSS Comment", 0x6A, 0x99, 0x55, italic: true) { } }
}
