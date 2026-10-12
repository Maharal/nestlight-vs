using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;

namespace NestLight.VisualStudio
{
    internal static class GraphQLClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.GqlOperation)]
        internal static ClassificationTypeDefinition Operation;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.GqlKeyword)]
        internal static ClassificationTypeDefinition Keyword;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.GqlField)]
        internal static ClassificationTypeDefinition Field;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.GqlArgument)]
        internal static ClassificationTypeDefinition Argument;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.GqlVariable)]
        internal static ClassificationTypeDefinition Variable;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.GqlType)]
        internal static ClassificationTypeDefinition Type;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.GqlDirective)]
        internal static ClassificationTypeDefinition Directive;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.GqlString)]
        internal static ClassificationTypeDefinition String;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.GqlNumber)]
        internal static ClassificationTypeDefinition Number;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.GqlComment)]
        internal static ClassificationTypeDefinition Comment;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.GqlOperation)]
    [Name("Template GraphQL Operation Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class GraphQLOperationFormat : TplFormat { public GraphQLOperationFormat() : base("Template GraphQL Operation", 0x56, 0x9C, 0xD6) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.GqlKeyword)]
    [Name("Template GraphQL Keyword Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class GraphQLKeywordFormat : TplFormat { public GraphQLKeywordFormat() : base("Template GraphQL Keyword", 0xC5, 0x86, 0xC0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.GqlField)]
    [Name("Template GraphQL Field Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class GraphQLFieldFormat : TplFormat { public GraphQLFieldFormat() : base("Template GraphQL Field", 0xDC, 0xDC, 0xAA) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.GqlArgument)]
    [Name("Template GraphQL Argument Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class GraphQLArgumentFormat : TplFormat { public GraphQLArgumentFormat() : base("Template GraphQL Argument", 0x9C, 0xDC, 0xFE) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.GqlVariable)]
    [Name("Template GraphQL Variable Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class GraphQLVariableFormat : TplFormat { public GraphQLVariableFormat() : base("Template GraphQL Variable", 0xE0, 0x8A, 0x4E) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.GqlType)]
    [Name("Template GraphQL Type Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class GraphQLTypeFormat : TplFormat { public GraphQLTypeFormat() : base("Template GraphQL Type", 0x4E, 0xC9, 0xB0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.GqlDirective)]
    [Name("Template GraphQL Directive Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class GraphQLDirectiveFormat : TplFormat { public GraphQLDirectiveFormat() : base("Template GraphQL Directive", 0xC5, 0x86, 0xC0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.GqlString)]
    [Name("Template GraphQL String Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class GraphQLStringFormat : TplFormat { public GraphQLStringFormat() : base("Template GraphQL String", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.GqlNumber)]
    [Name("Template GraphQL Number Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class GraphQLNumberFormat : TplFormat { public GraphQLNumberFormat() : base("Template GraphQL Number", 0xB5, 0xCE, 0xA8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.GqlComment)]
    [Name("Template GraphQL Comment Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class GraphQLCommentFormat : TplFormat { public GraphQLCommentFormat() : base("Template GraphQL Comment", 0x6A, 0x99, 0x55, italic: true) { } }
}
