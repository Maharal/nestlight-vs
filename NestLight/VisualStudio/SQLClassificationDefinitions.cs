using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;

namespace NestLight.VisualStudio
{
    // Classification types and default colors of the embedded languages besides HTML and CSS.
    // Colors are editable in Tools > Options > Environment > Fonts and Colors > Text Editor,
    // in the items named "Template <Language> ...".
    // MEF reads these attributes statically, so every item is declared; one file per language.
    internal static class SQLClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.SqlKeyword)]
        internal static ClassificationTypeDefinition Keyword;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.SqlIdentifier)]
        internal static ClassificationTypeDefinition Identifier;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.SqlParameter)]
        internal static ClassificationTypeDefinition Parameter;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.SqlString)]
        internal static ClassificationTypeDefinition String;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.SqlNumber)]
        internal static ClassificationTypeDefinition Number;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.SqlOperator)]
        internal static ClassificationTypeDefinition Operator;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.SqlComment)]
        internal static ClassificationTypeDefinition Comment;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.SqlKeyword)]
    [Name("Template SQL Keyword Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class SQLKeywordFormat : TplFormat { public SQLKeywordFormat() : base("Template SQL Keyword", 0x56, 0x9C, 0xD6) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.SqlIdentifier)]
    [Name("Template SQL Identifier Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class SQLIdentifierFormat : TplFormat { public SQLIdentifierFormat() : base("Template SQL Identifier", 0x9C, 0xDC, 0xFE) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.SqlParameter)]
    [Name("Template SQL Parameter Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class SQLParameterFormat : TplFormat { public SQLParameterFormat() : base("Template SQL Parameter", 0xE0, 0x6C, 0xC0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.SqlString)]
    [Name("Template SQL String Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class SQLStringFormat : TplFormat { public SQLStringFormat() : base("Template SQL String", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.SqlNumber)]
    [Name("Template SQL Number Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class SQLNumberFormat : TplFormat { public SQLNumberFormat() : base("Template SQL Number", 0xB5, 0xCE, 0xA8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.SqlOperator)]
    [Name("Template SQL Operator Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class SQLOperatorFormat : TplFormat { public SQLOperatorFormat() : base("Template SQL Operator", 0xD4, 0xD4, 0xD4) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.SqlComment)]
    [Name("Template SQL Comment Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class SQLCommentFormat : TplFormat { public SQLCommentFormat() : base("Template SQL Comment", 0x6A, 0x99, 0x55, italic: true) { } }
}
