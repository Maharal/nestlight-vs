namespace NestLight.Common
{
    /// <summary>
    /// Names of the classification types. Each name is "template.&lt;language&gt;.&lt;role&gt;" and is the
    /// contract between the tokenizers (which produce them) and the editor (which colors them).
    /// </summary>
    internal static class ClassificationNames
    {
        // Interpolations (shared by every host)
        public const string ExprDelimiter = "template.expression.delimiter";
        public const string Expression = "template.expression";

        // HTML
        public const string Tag = "template.html.tag";
        public const string Delimiter = "template.html.delimiter";
        public const string Attribute = "template.html.attribute";
        public const string AttributeEvent = "template.html.attribute.event";
        public const string AttributeProperty = "template.html.attribute.property";
        public const string AttributeBoolean = "template.html.attribute.boolean";
        public const string AttributeValue = "template.html.attribute.value";
        public const string Comment = "template.html.comment";

        // CSS
        public const string CssSelector = "template.css.selector";
        public const string CssSelectorClass = "template.css.selector.class";
        public const string CssSelectorId = "template.css.selector.id";
        public const string CssAttribute = "template.css.attribute";
        public const string CssUnit = "template.css.unit";
        public const string CssImportant = "template.css.important";
        public const string CssPseudo = "template.css.pseudo";
        public const string CssProperty = "template.css.property";
        public const string CssCustomProperty = "template.css.property.custom";
        public const string CssValue = "template.css.value";
        public const string CssNumber = "template.css.number";
        public const string CssFunction = "template.css.function";
        public const string CssAtRule = "template.css.atrule";
        public const string CssString = "template.css.string";
        public const string CssPunct = "template.css.punctuation";
        public const string CssComment = "template.css.comment";

        // SQL
        public const string SqlKeyword = "template.sql.keyword";
        public const string SqlIdentifier = "template.sql.identifier";
        public const string SqlString = "template.sql.string";
        public const string SqlNumber = "template.sql.number";
        public const string SqlOperator = "template.sql.operator";
        public const string SqlParameter = "template.sql.parameter";
        public const string SqlComment = "template.sql.comment";

        // JSON
        public const string JsonKey = "template.json.key";
        public const string JsonString = "template.json.string";
        public const string JsonNumber = "template.json.number";
        public const string JsonLiteral = "template.json.literal";
        public const string JsonPunct = "template.json.punctuation";

        // GraphQL
        public const string GqlOperation = "template.graphql.operation";
        public const string GqlKeyword = "template.graphql.keyword";
        public const string GqlField = "template.graphql.field";
        public const string GqlArgument = "template.graphql.argument";
        public const string GqlVariable = "template.graphql.variable";
        public const string GqlType = "template.graphql.type";
        public const string GqlDirective = "template.graphql.directive";
        public const string GqlString = "template.graphql.string";
        public const string GqlNumber = "template.graphql.number";
        public const string GqlComment = "template.graphql.comment";

        // XML
        public const string XmlTag = "template.xml.tag";
        public const string XmlDelimiter = "template.xml.delimiter";
        public const string XmlAttribute = "template.xml.attribute";
        public const string XmlValue = "template.xml.attribute.value";
        public const string XmlComment = "template.xml.comment";
        public const string XmlCData = "template.xml.cdata";

        // Markdown
        public const string MdHeading = "template.markdown.heading";
        public const string MdEmphasis = "template.markdown.emphasis";
        public const string MdStrong = "template.markdown.strong";
        public const string MdCode = "template.markdown.code";
        public const string MdLink = "template.markdown.link";
        public const string MdList = "template.markdown.list";
        public const string MdMath = "template.markdown.math";

        // YAML
        public const string YamlKey = "template.yaml.key";
        public const string YamlString = "template.yaml.string";
        public const string YamlScalar = "template.yaml.scalar";
        public const string YamlAnchor = "template.yaml.anchor";
        public const string YamlComment = "template.yaml.comment";
        public const string YamlPunct = "template.yaml.punctuation";

        // Regular expressions
        public const string RegexGroup = "template.regex.group";
        public const string RegexClass = "template.regex.class";
        public const string RegexQuantifier = "template.regex.quantifier";
        public const string RegexEscape = "template.regex.escape";
        public const string RegexAnchor = "template.regex.anchor";

        // Shaders (GLSL and WGSL)
        public const string ShaderKeyword = "template.shader.keyword";
        public const string ShaderType = "template.shader.type";
        public const string ShaderBuiltin = "template.shader.builtin";
        public const string ShaderNumber = "template.shader.number";
        public const string ShaderComment = "template.shader.comment";
    }
}
