using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;

namespace NestLight.VisualStudio
{
    internal static class ShaderClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShaderKeyword)]
        internal static ClassificationTypeDefinition Keyword;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShaderType)]
        internal static ClassificationTypeDefinition Type;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShaderBuiltin)]
        internal static ClassificationTypeDefinition Builtin;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShaderNumber)]
        internal static ClassificationTypeDefinition Number;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.ShaderComment)]
        internal static ClassificationTypeDefinition Comment;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShaderKeyword)]
    [Name("Template Shader Keyword (GLSL / WGSL) Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShaderKeywordFormat : TplFormat { public ShaderKeywordFormat() : base("Template Shader Keyword (GLSL / WGSL)", 0x56, 0x9C, 0xD6) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShaderType)]
    [Name("Template Shader Type (GLSL / WGSL) Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShaderTypeFormat : TplFormat { public ShaderTypeFormat() : base("Template Shader Type (GLSL / WGSL)", 0x4E, 0xC9, 0xB0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShaderBuiltin)]
    [Name("Template Shader Built-in (GLSL / WGSL) Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShaderBuiltinFormat : TplFormat { public ShaderBuiltinFormat() : base("Template Shader Built-in (GLSL / WGSL)", 0xDC, 0xDC, 0xAA) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShaderNumber)]
    [Name("Template Shader Number (GLSL / WGSL) Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShaderNumberFormat : TplFormat { public ShaderNumberFormat() : base("Template Shader Number (GLSL / WGSL)", 0xB5, 0xCE, 0xA8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.ShaderComment)]
    [Name("Template Shader Comment (GLSL / WGSL) Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class ShaderCommentFormat : TplFormat { public ShaderCommentFormat() : base("Template Shader Comment (GLSL / WGSL)", 0x6A, 0x99, 0x55, italic: true) { } }

    // ---- Shell / Bash ----
}
