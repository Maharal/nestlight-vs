using System;

namespace NestLight
{
    internal static class TplNames
    {
        // HTML
        public const string Tag = "template.html.tag";
        public const string Delimiter = "template.html.delimiter";
        public const string Attribute = "template.html.attribute";
        public const string AttributeEvent = "template.html.attribute.event";
        public const string AttributeProperty = "template.html.attribute.property";
        public const string AttributeBoolean = "template.html.attribute.boolean";
        public const string AttributeValue = "template.html.attribute.value";
        public const string Comment = "template.html.comment";
        public const string ExprDelimiter = "template.html.expression.delimiter";
        public const string Expression = "template.html.expression";

        // CSS
        public const string CssSelector = "template.css.selector";
        public const string CssSelectorClass = "template.css.selector.class";
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
    }

    internal enum TemplateKind { Html, Css }

    internal struct TextRange
    {
        public int Start;
        public int End;
        /// <summary>For ${...} expressions: true if the closing '}' exists.</summary>
        public bool Closed;
        public TextRange(int start, int end) { Start = start; End = end; Closed = false; }
    }

    internal sealed class TemplateInfo
    {
        public TemplateKind Kind;
        /// <summary>Index right after the opening backtick.</summary>
        public int Start;
        /// <summary>Index of the closing backtick (or end of text if unclosed).</summary>
        public int End;
        /// <summary>${ ... } ranges (includes "${" and "}").</summary>
        public System.Collections.Generic.List<TextRange> Expressions = new System.Collections.Generic.List<TextRange>();
    }

    internal sealed class TplToken
    {
        public readonly int Start;
        public readonly int Length;
        public readonly string Type;
        public int End { get { return Start + Length; } }
        public TplToken(int start, int length, string type) { Start = start; Length = length; Type = type; }
    }
}
