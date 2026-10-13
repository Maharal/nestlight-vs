using System.Collections.Generic;

namespace NestLight.Common
{
    internal struct QuickInfoResult
    {
        public readonly int Start;
        public readonly int Length;
        public readonly string Description;
        public readonly CssColor? Color;
        public readonly string Title;
        public readonly string Detail;

        public QuickInfoResult(int start, int length, string description, CssColor? color = null, string title = null, string detail = null)
        {
            Start = start; Length = length; Description = description; Color = color;
            Title = title ?? description;
            Detail = detail;
        }
    }

    internal static class QuickInfoEngine
    {
        public static QuickInfoResult? GetInfo(IReadOnlyList<Token> tokens, int position)
        {
            return GetInfo(tokens, position, null);
        }

        public static QuickInfoResult? GetInfo(IReadOnlyList<Token> tokens, int position, string text)
        {
            Token token = TokenLookup.AtPosition(tokens, position);
            if (token == null) return null;

            string description = DescriptionOf(token.Type);
            if (description == null) return null;

            CssColor? color = null;
            string title = description;
            string detail = DetailOf(token.Type);
            if (text != null && token.Start >= 0 && token.Start + token.Length <= text.Length)
            {
                string tokenText = text.Substring(token.Start, token.Length);
                string name = tokenText.Trim();
                detail = TokenDescriptions.Of(token.Type, name) ?? detail;
                if (name.Length > 40) name = name.Substring(0, 37) + "...";
                if (name.Length > 0 && !IsLiteralOrComment(token.Type) && name.IndexOf('\n') < 0 && name.IndexOf('\r') < 0) title = name + " \u2014 " + description;
                if (token.Type == ClassificationNames.CssNumber || token.Type == ClassificationNames.CssValue)
                    color = CssColorParser.TryParse(tokenText);
            }

            return new QuickInfoResult(token.Start, token.Length, description, color, title, detail);
        }

        /// <summary>Whether the text of the token is content (a value, a comment) that the title should not repeat.</summary>
        private static bool IsLiteralOrComment(string t)
        {
            return t == ClassificationNames.AttributeValue || t == ClassificationNames.SqlString || t == ClassificationNames.JsonString
                || t == ClassificationNames.ExprString || t == ClassificationNames.XmlCData || t == ClassificationNames.MdCode
                || t == ClassificationNames.MdLink || t == ClassificationNames.MdMath
                || t == ClassificationNames.Comment || t == ClassificationNames.CssComment || t == ClassificationNames.SqlComment
                || t == ClassificationNames.XmlComment || t == ClassificationNames.YamlComment || t == ClassificationNames.ShellComment
                || t == ClassificationNames.TomlComment;
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
            if (classificationType == ClassificationNames.Mustache) return "Mustache tag";

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

            if (classificationType == ClassificationNames.ExprKeyword) return "Expression keyword";
            if (classificationType == ClassificationNames.ExprString) return "Expression string";
            if (classificationType == ClassificationNames.ExprNumber) return "Expression number";

            return null;
        }

        internal static string DetailOf(string classificationType)
        {
            if (classificationType == null) return null;

            if (classificationType == ClassificationNames.SqlKeyword) return "A reserved word of the SQL language that defines the structure of a statement.";
            if (classificationType == ClassificationNames.SqlParameter) return "A placeholder whose value is supplied when the query is executed.";
            if (classificationType == ClassificationNames.SqlIdentifier) return "The name of a table, column, alias or other database object.";
            if (classificationType == ClassificationNames.SqlString) return "A literal text value enclosed in quotes.";
            if (classificationType == ClassificationNames.SqlNumber) return "A numeric literal.";
            if (classificationType == ClassificationNames.SqlOperator) return "Combines or compares values, such as =, <> or +.";
            if (classificationType == ClassificationNames.SqlComment) return "Text ignored by the database, used for notes.";
            if (classificationType == ClassificationNames.Tag) return "Defines an HTML element and the kind of content it represents.";
            if (classificationType == ClassificationNames.Attribute) return "Provides additional information or configuration for an HTML element.";
            if (classificationType == ClassificationNames.AttributeEvent) return "Binds a handler to a DOM event, such as a click or input.";
            if (classificationType == ClassificationNames.AttributeProperty) return "Binds a value to a DOM property of the element instead of an HTML attribute.";
            if (classificationType == ClassificationNames.AttributeBoolean) return "An attribute that enables a behavior simply by being present on the element.";
            if (classificationType == ClassificationNames.AttributeValue) return "The value assigned to an HTML attribute.";
            if (classificationType == ClassificationNames.Comment) return "Text ignored by the browser, used for notes.";
            if (classificationType == ClassificationNames.Mustache) return "A template placeholder, {{value}} or {{{value}}}, that a template engine replaces with data. Three braces insert the value without HTML escaping.";
            if (classificationType == ClassificationNames.CssProperty) return "Sets a style aspect of the selected elements, such as color, size or layout.";
            if (classificationType == ClassificationNames.CssCustomProperty) return "A user-defined CSS variable, read elsewhere with var().";
            if (classificationType == ClassificationNames.CssSelector) return "Chooses which elements the style rules apply to.";
            if (classificationType == ClassificationNames.CssSelectorClass) return "Matches every element that has this class name.";
            if (classificationType == ClassificationNames.CssSelectorId) return "Matches the single element with this id.";
            if (classificationType == ClassificationNames.CssAtRule) return "A CSS statement that starts with @, such as @media or @keyframes.";
            if (classificationType == ClassificationNames.CssFunction) return "A CSS function that computes a value, such as calc() or var().";
            if (classificationType == ClassificationNames.CssValue) return "The value assigned to a CSS property.";
            if (classificationType == ClassificationNames.CssUnit) return "The unit that gives a CSS number its meaning, such as px, em or %.";
            if (classificationType == ClassificationNames.CssNumber) return "A numeric value in a CSS declaration.";
            if (classificationType == ClassificationNames.CssPseudo) return "Selects an element by its state or a part of it, such as :hover or ::before.";
            if (classificationType == ClassificationNames.CssComment) return "Text ignored by the browser, used for notes.";
            if (classificationType == ClassificationNames.JsonKey) return "The name of a property in a JSON object.";
            if (classificationType == ClassificationNames.JsonString) return "A text value enclosed in double quotes.";
            if (classificationType == ClassificationNames.JsonNumber) return "A numeric value.";
            if (classificationType == ClassificationNames.JsonLiteral) return "One of the fixed JSON values: true, false or null.";
            if (classificationType == ClassificationNames.GqlOperation) return "Declares the kind of GraphQL request: query, mutation or subscription.";
            if (classificationType == ClassificationNames.GqlKeyword) return "A reserved word of the GraphQL language.";
            if (classificationType == ClassificationNames.GqlType) return "The name of a GraphQL type.";
            if (classificationType == ClassificationNames.GqlDirective) return "Annotation that changes how a field or fragment is executed.";
            if (classificationType == ClassificationNames.GqlVariable) return "A named value passed to the operation, prefixed with $.";
            if (classificationType == ClassificationNames.XmlTag) return "Defines an XML element.";
            if (classificationType == ClassificationNames.XmlAttribute) return "Provides additional information for an XML element.";
            if (classificationType == ClassificationNames.XmlCData) return "A block of text that the XML parser does not interpret as markup.";
            if (classificationType == ClassificationNames.XmlComment) return "Text ignored by the XML parser, used for notes.";
            if (classificationType == ClassificationNames.YamlKey) return "The name of a mapping entry in YAML.";
            if (classificationType == ClassificationNames.YamlScalar) return "A plain YAML value interpreted as a number, boolean or null.";
            if (classificationType == ClassificationNames.YamlAnchor) return "Marks a node for reuse (&anchor), references it (*alias) or sets its type (!tag).";
            if (classificationType == ClassificationNames.YamlComment) return "Text ignored by the YAML parser, used for notes.";
            if (classificationType == ClassificationNames.MdHeading) return "A section title in Markdown.";
            if (classificationType == ClassificationNames.MdCode) return "Text displayed in a monospaced font as code.";
            if (classificationType == ClassificationNames.MdLink) return "A hyperlink to another location.";
            if (classificationType == ClassificationNames.MdMath) return "A mathematical expression written in LaTeX notation.";
            if (classificationType == ClassificationNames.RegexGroup) return "Groups part of the pattern so it can be captured or quantified.";
            if (classificationType == ClassificationNames.RegexClass) return "Matches one character out of a set.";
            if (classificationType == ClassificationNames.RegexQuantifier) return "Controls how many times the preceding element may repeat.";
            if (classificationType == ClassificationNames.RegexEscape) return "A backslash sequence that matches a special character or a character class.";
            if (classificationType == ClassificationNames.RegexAnchor) return "Matches a position, such as the start or end of the text, instead of a character.";
            if (classificationType == ClassificationNames.ShaderKeyword) return "A reserved word of the shader language.";
            if (classificationType == ClassificationNames.ShaderType) return "A built-in data type of the shader language, such as a vector or a matrix.";
            if (classificationType == ClassificationNames.ShaderBuiltin) return "A function provided by the shader language.";
            if (classificationType == ClassificationNames.ShellKeyword) return "A reserved word that controls the flow of the script.";
            if (classificationType == ClassificationNames.ShellBuiltin) return "A command implemented by the shell itself.";
            if (classificationType == ClassificationNames.ShellCommand) return "The program or command being run.";
            if (classificationType == ClassificationNames.ShellVariable) return "A named value expanded by the shell.";
            if (classificationType == ClassificationNames.ShellFlag) return "An option that changes the behavior of a command.";
            if (classificationType == ClassificationNames.ShellComment) return "Text ignored by the shell, used for notes.";
            if (classificationType == ClassificationNames.TomlTable) return "Starts a group of key/value pairs.";
            if (classificationType == ClassificationNames.TomlKey) return "The name of a TOML setting.";
            if (classificationType == ClassificationNames.TomlLiteral) return "A boolean value: true or false.";
            if (classificationType == ClassificationNames.TomlDate) return "A date and/or time value.";
            if (classificationType == ClassificationNames.TomlComment) return "Text ignored by the parser, used for notes.";
            if (classificationType == ClassificationNames.ExprKeyword) return "A reserved word of the template expression language.";
            if (classificationType == ClassificationNames.ExprString) return "A literal text value inside an expression.";
            if (classificationType == ClassificationNames.ExprNumber) return "A numeric literal inside an expression.";

            return null;
        }
    }
}
