using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;

namespace NestLight.VisualStudio
{
    // Classification types and default colors of the embedded languages besides HTML and CSS.
    // Colors are editable in Tools > Options > Environment > Fonts and Colors > Text Editor,
    // in the items named "Template <Language> ...".
    // MEF reads these attributes statically, so every item is declared here.
    // ---- SQL ----
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

    // ---- JSON ----
    internal static class JSONClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.JsonKey)]
        internal static ClassificationTypeDefinition Key;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.JsonString)]
        internal static ClassificationTypeDefinition String;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.JsonNumber)]
        internal static ClassificationTypeDefinition Number;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.JsonLiteral)]
        internal static ClassificationTypeDefinition Literal;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.JsonPunct)]
        internal static ClassificationTypeDefinition Punctuation;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.JsonKey)]
    [Name("Template JSON Key Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class JSONKeyFormat : TplFormat { public JSONKeyFormat() : base("Template JSON Key", 0x9C, 0xDC, 0xFE) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.JsonString)]
    [Name("Template JSON String Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class JSONStringFormat : TplFormat { public JSONStringFormat() : base("Template JSON String", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.JsonNumber)]
    [Name("Template JSON Number Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class JSONNumberFormat : TplFormat { public JSONNumberFormat() : base("Template JSON Number", 0xB5, 0xCE, 0xA8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.JsonLiteral)]
    [Name("Template JSON Literal (true / false / null) Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class JSONLiteralFormat : TplFormat { public JSONLiteralFormat() : base("Template JSON Literal (true / false / null)", 0x56, 0x9C, 0xD6) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.JsonPunct)]
    [Name("Template JSON Punctuation Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class JSONPunctuationFormat : TplFormat { public JSONPunctuationFormat() : base("Template JSON Punctuation", 0x90, 0x90, 0x90) { } }

    // ---- GraphQL ----
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

    // ---- XML ----
    internal static class XMLClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.XmlTag)]
        internal static ClassificationTypeDefinition Tag;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.XmlDelimiter)]
        internal static ClassificationTypeDefinition Delimiter;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.XmlAttribute)]
        internal static ClassificationTypeDefinition Attribute;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.XmlValue)]
        internal static ClassificationTypeDefinition AttributeValue;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.XmlComment)]
        internal static ClassificationTypeDefinition Comment;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.XmlCData)]
        internal static ClassificationTypeDefinition CData;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.XmlTag)]
    [Name("Template XML Tag Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class XMLTagFormat : TplFormat { public XMLTagFormat() : base("Template XML Tag", 0x4A, 0x90, 0xD9) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.XmlDelimiter)]
    [Name("Template XML Delimiter Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class XMLDelimiterFormat : TplFormat { public XMLDelimiterFormat() : base("Template XML Delimiter", 0x90, 0x90, 0x90) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.XmlAttribute)]
    [Name("Template XML Attribute Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class XMLAttributeFormat : TplFormat { public XMLAttributeFormat() : base("Template XML Attribute", 0x56, 0xB0, 0xC8) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.XmlValue)]
    [Name("Template XML Attribute Value Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class XMLAttributeValueFormat : TplFormat { public XMLAttributeValueFormat() : base("Template XML Attribute Value", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.XmlComment)]
    [Name("Template XML Comment Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class XMLCommentFormat : TplFormat { public XMLCommentFormat() : base("Template XML Comment", 0x6A, 0x99, 0x55, italic: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.XmlCData)]
    [Name("Template XML CDATA Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class XMLCDataFormat : TplFormat { public XMLCDataFormat() : base("Template XML CDATA", 0xB5, 0x9A, 0x6A) { } }

    // ---- Markdown ----
    internal static class MarkdownClassificationTypes
    {
        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdHeading)]
        internal static ClassificationTypeDefinition Heading;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdEmphasis)]
        internal static ClassificationTypeDefinition Emphasis;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdStrong)]
        internal static ClassificationTypeDefinition Strong;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdCode)]
        internal static ClassificationTypeDefinition Code;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdLink)]
        internal static ClassificationTypeDefinition Link;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdList)]
        internal static ClassificationTypeDefinition List;

        [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.MdMath)]
        internal static ClassificationTypeDefinition Math;
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdHeading)]
    [Name("Template Markdown Heading Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownHeadingFormat : TplFormat { public MarkdownHeadingFormat() : base("Template Markdown Heading", 0x56, 0x9C, 0xD6, bold: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdEmphasis)]
    [Name("Template Markdown Emphasis Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownEmphasisFormat : TplFormat { public MarkdownEmphasisFormat() : base("Template Markdown Emphasis", 0x9C, 0xDC, 0xFE, italic: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdStrong)]
    [Name("Template Markdown Strong Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownStrongFormat : TplFormat { public MarkdownStrongFormat() : base("Template Markdown Strong", 0x9C, 0xDC, 0xFE, bold: true) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdCode)]
    [Name("Template Markdown Code Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownCodeFormat : TplFormat { public MarkdownCodeFormat() : base("Template Markdown Code", 0xCE, 0x91, 0x78) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdLink)]
    [Name("Template Markdown Link Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownLinkFormat : TplFormat { public MarkdownLinkFormat() : base("Template Markdown Link", 0x4F, 0xA3, 0xE0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdList)]
    [Name("Template Markdown List Marker Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownListFormat : TplFormat { public MarkdownListFormat() : base("Template Markdown List Marker", 0xC5, 0x86, 0xC0) { } }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationNames.MdMath)]
    [Name("Template Markdown Math Format")][UserVisible(true)][Order(After = Priority.High)]
    internal sealed class MarkdownMathFormat : TplFormat { public MarkdownMathFormat() : base("Template Markdown Math", 0xB5, 0xCE, 0xA8) { } }

    // ---- YAML ----
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

    // ---- Regex ----
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

    // ---- Shader ----
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
}
