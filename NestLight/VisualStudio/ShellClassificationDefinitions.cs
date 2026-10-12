using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;

namespace NestLight.VisualStudio
{
    internal static class ShellClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShellKeyword)]
        internal static ClassificationTypeDefinition Keyword;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShellBuiltin)]
        internal static ClassificationTypeDefinition Builtin;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShellCommand)]
        internal static ClassificationTypeDefinition Command;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShellVariable)]
        internal static ClassificationTypeDefinition Variable;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShellString)]
        internal static ClassificationTypeDefinition String;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShellNumber)]
        internal static ClassificationTypeDefinition Number;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShellOperator)]
        internal static ClassificationTypeDefinition Operator;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShellFlag)]
        internal static ClassificationTypeDefinition Flag;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShellComment)]
        internal static ClassificationTypeDefinition Comment;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShellKeyword)]
    [Name("Template Shell Keyword Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShellKeywordFormat : TplFormat { public ShellKeywordFormat() : base("Template Shell Keyword", 0x56, 0x9C, 0xD6) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShellBuiltin)]
    [Name("Template Shell Builtin Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShellBuiltinFormat : TplFormat { public ShellBuiltinFormat() : base("Template Shell Builtin", 0xDC, 0xDC, 0xAA) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShellCommand)]
    [Name("Template Shell Command Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShellCommandFormat : TplFormat { public ShellCommandFormat() : base("Template Shell Command", 0xDC, 0xDC, 0xAA) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShellVariable)]
    [Name("Template Shell Variable Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShellVariableFormat : TplFormat { public ShellVariableFormat() : base("Template Shell Variable", 0x9C, 0xDC, 0xFE) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShellString)]
    [Name("Template Shell String Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShellStringFormat : TplFormat { public ShellStringFormat() : base("Template Shell String", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShellNumber)]
    [Name("Template Shell Number Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShellNumberFormat : TplFormat { public ShellNumberFormat() : base("Template Shell Number", 0xB5, 0xCE, 0xA8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShellOperator)]
    [Name("Template Shell Operator Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShellOperatorFormat : TplFormat { public ShellOperatorFormat() : base("Template Shell Operator", 0xD4, 0xD4, 0xD4) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShellFlag)]
    [Name("Template Shell Flag Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShellFlagFormat : TplFormat { public ShellFlagFormat() : base("Template Shell Flag", 0x9C, 0x9C, 0xCE) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShellComment)]
    [Name("Template Shell Comment Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShellCommentFormat : TplFormat { public ShellCommentFormat() : base("Template Shell Comment", 0x6A, 0x99, 0x55, italic: true) { } }
}
