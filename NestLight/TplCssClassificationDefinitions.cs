using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace NestLight
{
    // ---- Classification types (CSS) -----------------------------------------
    internal static class TplCssClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.CssSelector)]
        internal static ClassificationTypeDefinition Selector;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.CssSelectorClass)]
        internal static ClassificationTypeDefinition SelectorClass;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.CssPseudo)]
        internal static ClassificationTypeDefinition Pseudo;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.CssProperty)]
        internal static ClassificationTypeDefinition Property;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.CssCustomProperty)]
        internal static ClassificationTypeDefinition CustomProperty;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.CssValue)]
        internal static ClassificationTypeDefinition Value;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.CssNumber)]
        internal static ClassificationTypeDefinition Number;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.CssFunction)]
        internal static ClassificationTypeDefinition Function;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.CssAtRule)]
        internal static ClassificationTypeDefinition AtRule;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.CssString)]
        internal static ClassificationTypeDefinition String;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.CssPunct)]
        internal static ClassificationTypeDefinition Punct;

        [Export(typeof(ClassificationTypeDefinition))][Name(TplNames.CssComment)]
        internal static ClassificationTypeDefinition Comment;
    }

    // ---- Formats (default colors; editable in Tools > Options > Environment > Fonts and Colors) ----
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.CssSelector)]
    [Name("Template CSS Selector Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssSelectorFormat : TplFormat { public TplCssSelectorFormat() : base("Template CSS Selector", 0xD6, 0xA2, 0x45) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.CssSelectorClass)]
    [Name("Template CSS Class/Id Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssSelectorClassFormat : TplFormat { public TplCssSelectorClassFormat() : base("Template CSS Class / Id", 0xC8, 0x6F, 0x8A) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.CssPseudo)]
    [Name("Template CSS Pseudo Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssPseudoFormat : TplFormat { public TplCssPseudoFormat() : base("Template CSS Pseudo-class / element", 0xB2, 0x61, 0xC9) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.CssProperty)]
    [Name("Template CSS Property Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssPropertyFormat : TplFormat { public TplCssPropertyFormat() : base("Template CSS Property", 0x56, 0xB6, 0xC2) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.CssCustomProperty)]
    [Name("Template CSS Custom Property Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssCustomPropertyFormat : TplFormat { public TplCssCustomPropertyFormat() : base("Template CSS Custom Property (--x)", 0x7A, 0x9E, 0xE8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.CssValue)]
    [Name("Template CSS Value Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssValueFormat : TplFormat { public TplCssValueFormat() : base("Template CSS Value", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.CssNumber)]
    [Name("Template CSS Number Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssNumberFormat : TplFormat { public TplCssNumberFormat() : base("Template CSS Number / Color", 0x6C, 0xB8, 0x6C) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.CssFunction)]
    [Name("Template CSS Function Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssFunctionFormat : TplFormat { public TplCssFunctionFormat() : base("Template CSS Function", 0xC2, 0xA3, 0x3A) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.CssAtRule)]
    [Name("Template CSS At-rule Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssAtRuleFormat : TplFormat { public TplCssAtRuleFormat() : base("Template CSS At-rule / !important", 0xD1, 0x6D, 0xA8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.CssString)]
    [Name("Template CSS String Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssStringFormat : TplFormat { public TplCssStringFormat() : base("Template CSS String", 0xB5, 0x9A, 0x6A) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.CssPunct)]
    [Name("Template CSS Punctuation Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssPunctFormat : TplFormat { public TplCssPunctFormat() : base("Template CSS Punctuation", 0x90, 0x90, 0x90) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = TplNames.CssComment)]
    [Name("Template CSS Comment Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class TplCssCommentFormat : TplFormat { public TplCssCommentFormat() : base("Template CSS Comment", 0x6A, 0x99, 0x55, italic: true) { } }
}
