using System.Collections.Generic;

namespace NestLight.Common
{
    internal struct QuickInfoResult
    {
        public readonly int Start;
        public readonly int Length;
        public readonly string Description;

        public QuickInfoResult(int start, int length, string description) { Start = start; Length = length; Description = description; }
    }

    internal static class QuickInfoEngine
    {
        public static QuickInfoResult? GetInfo(IReadOnlyList<Token> tokens, int position)
        {
            Token token = TokenLookup.AtPosition(tokens, position);
            if (token == null) return null;

            string description = DescriptionOf(token.Type);
            if (description == null) return null;

            return new QuickInfoResult(token.Start, token.Length, description);
        }

        internal static string DescriptionOf(string classificationType)
        {
            if (classificationType == null) return null;

            if (classificationType == ClassificationNames.SqlKeyword) return "SQL keyword";
            if (classificationType == ClassificationNames.SqlParameter) return "SQL parameter";
            if (classificationType == ClassificationNames.SqlIdentifier) return "SQL identifier";
            if (classificationType == ClassificationNames.SqlString) return "SQL string literal";
            if (classificationType == ClassificationNames.SqlNumber) return "SQL number";
            if (classificationType == ClassificationNames.SqlOperator) return "SQL operator";
            if (classificationType == ClassificationNames.SqlComment) return "SQL comment";

            if (classificationType == ClassificationNames.Tag) return "HTML tag";
            if (classificationType == ClassificationNames.Attribute) return "HTML attribute";
            if (classificationType == ClassificationNames.AttributeEvent) return "HTML event handler attribute";
            if (classificationType == ClassificationNames.AttributeProperty) return "HTML property binding";
            if (classificationType == ClassificationNames.AttributeBoolean) return "HTML boolean attribute";
            if (classificationType == ClassificationNames.AttributeValue) return "HTML attribute value";
            if (classificationType == ClassificationNames.Comment) return "HTML comment";

            if (classificationType == ClassificationNames.CssProperty) return "CSS property";
            if (classificationType == ClassificationNames.CssCustomProperty) return "CSS custom property (variable)";
            if (classificationType == ClassificationNames.CssSelector) return "CSS selector";
            if (classificationType == ClassificationNames.CssSelectorClass) return "CSS class selector";
            if (classificationType == ClassificationNames.CssSelectorId) return "CSS ID selector";
            if (classificationType == ClassificationNames.CssAtRule) return "CSS at-rule";
            if (classificationType == ClassificationNames.CssFunction) return "CSS function";
            if (classificationType == ClassificationNames.CssValue) return "CSS value";
            if (classificationType == ClassificationNames.CssUnit) return "CSS unit";
            if (classificationType == ClassificationNames.CssNumber) return "CSS number";
            if (classificationType == ClassificationNames.CssPseudo) return "CSS pseudo-class / pseudo-element";
            if (classificationType == ClassificationNames.CssComment) return "CSS comment";

            if (classificationType == ClassificationNames.JsonKey) return "JSON key";
            if (classificationType == ClassificationNames.JsonString) return "JSON string";
            if (classificationType == ClassificationNames.JsonNumber) return "JSON number";
            if (classificationType == ClassificationNames.JsonLiteral) return "JSON literal (true / false / null)";

            if (classificationType == ClassificationNames.GqlOperation) return "GraphQL operation";
            if (classificationType == ClassificationNames.GqlKeyword) return "GraphQL keyword";
            if (classificationType == ClassificationNames.GqlType) return "GraphQL type";
            if (classificationType == ClassificationNames.GqlDirective) return "GraphQL directive";
            if (classificationType == ClassificationNames.GqlVariable) return "GraphQL variable";

            if (classificationType == ClassificationNames.XmlTag) return "XML element";
            if (classificationType == ClassificationNames.XmlAttribute) return "XML attribute";
            if (classificationType == ClassificationNames.XmlCData) return "XML CDATA section";
            if (classificationType == ClassificationNames.XmlComment) return "XML comment";

            if (classificationType == ClassificationNames.YamlKey) return "YAML key";
            if (classificationType == ClassificationNames.YamlScalar) return "YAML scalar (number / bool / null)";
            if (classificationType == ClassificationNames.YamlAnchor) return "YAML anchor / alias / tag";
            if (classificationType == ClassificationNames.YamlComment) return "YAML comment";

            if (classificationType == ClassificationNames.MdHeading) return "Markdown heading";
            if (classificationType == ClassificationNames.MdCode) return "Markdown code span";
            if (classificationType == ClassificationNames.MdLink) return "Markdown link";
            if (classificationType == ClassificationNames.MdMath) return "Markdown math expression";

            if (classificationType == ClassificationNames.RegexGroup) return "Regex group";
            if (classificationType == ClassificationNames.RegexClass) return "Regex character class";
            if (classificationType == ClassificationNames.RegexQuantifier) return "Regex quantifier";
            if (classificationType == ClassificationNames.RegexEscape) return "Regex escape sequence";
            if (classificationType == ClassificationNames.RegexAnchor) return "Regex anchor";

            if (classificationType == ClassificationNames.ShaderKeyword) return "Shader keyword";
            if (classificationType == ClassificationNames.ShaderType) return "Shader type";
            if (classificationType == ClassificationNames.ShaderBuiltin) return "Shader built-in function";

            if (classificationType == ClassificationNames.ShellKeyword) return "Shell keyword";
            if (classificationType == ClassificationNames.ShellBuiltin) return "Shell built-in command";
            if (classificationType == ClassificationNames.ShellCommand) return "Shell command";
            if (classificationType == ClassificationNames.ShellVariable) return "Shell variable";
            if (classificationType == ClassificationNames.ShellFlag) return "Shell command flag";
            if (classificationType == ClassificationNames.ShellComment) return "Shell comment";

            if (classificationType == ClassificationNames.TomlTable) return "TOML table header";
            if (classificationType == ClassificationNames.TomlKey) return "TOML key";
            if (classificationType == ClassificationNames.TomlLiteral) return "TOML literal (true / false)";
            if (classificationType == ClassificationNames.TomlDate) return "TOML date / time";
            if (classificationType == ClassificationNames.TomlComment) return "TOML comment";

            return null;
        }
    }
}
