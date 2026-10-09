using System.Collections.Generic;

namespace NestLight.Completion
{
    /// <summary>
    /// The keywords of each language from the most used to the least used, learned from the corpus of generated code of the experiments
    /// (NestLight.Experiments, <c>--priors</c>): a starting point, not what real projects use. Only the first ones matter: the rest are alphabetical.
    /// This is only the data. A language reads its own list (see <see cref="ICompletionLanguage.CompletionWordsByUse"/>), under the first of its ids
    /// that has one, so an alias or the id of a sibling (<c>svg</c> for <c>html</c>) needs no entry.
    /// </summary>
    internal static class KeywordUse
    {
        public static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> Default = new Dictionary<string, IReadOnlyList<string>>(System.StringComparer.OrdinalIgnoreCase)
        {
            { "sql", new[] { "from", "select", "where", "and", "by", "order", "null", "not", "as", "limit", "is", "in", "desc", "between", "like", "or", "on", "set", "update", "join", "offset", "key", "table", "insert", "into", "values", "then", "timestamp", "when", "create", "group", "integer", "delete", "to", "over", "partition", "returning", "with", "default", "exists", "primary", "serial", "all", "left", "union", "foreign", "references", "alter", "column", "having", "case", "else", "end", "false", "unique", "inner", "bigint", "numeric", "true", "conflict", "do", "text", "boolean", "add", "date", "index", "jsonb", "if", "uuid", "varchar", "rename", "drop" } },
            { "css", new[] { "background-color", "color", "font-size", "opacity", "display", "padding", "transform", "solid", "position", "auto", "font-weight", "border-radius", "gap", "margin", "max-width", "box-shadow", "border", "transition", "cursor", "flex", "left", "line-height", "top", "z-index", "bottom", "width", "grid", "grid-template-columns", "flex-direction", "right", "justify-content", "center", "align-items", "text-align", "transparent", "none", "currentColor", "outline", "outline-offset", "font-family", "animation", "block", "content", "letter-spacing", "overflow", "absolute", "min-height", "sticky", "text-transform", "relative", "text-decoration", "fixed", "background-image", "box-sizing", "flex-wrap", "white-space", "pointer-events", "text-overflow", "place-items", "all", "user-select", "will-change", "hidden", "scale", "translate", "rotate" } },
            { "html", new[] { "button", "li", "div", "label", "td", "th", "option", "footer", "h2", "ul", "tr", "span", "input", "col", "header", "section", "small", "form", "svg", "article", "h3", "textarea", "video", "table", "dialog", "tbody", "thead", "select", "nav", "img", "circle", "line", "path", "text", "menu", "main" } },
            { "graphql", new[] { "ID", "Int", "String", "input", "query", "Boolean", "Float", "mutation", "enum", "type", "subscription", "fragment", "on", "implements" } },
            { "json", new[] { "true", "false", "null" } },
            { "yaml", new[] { "true", "on", "false" } },
            { "glsl", new[] { "float", "vec3", "uniform", "vec4", "in", "dot", "max", "normalize", "precision", "void", "vec2", "mat4", "varying", "sin", "mix", "out", "smoothstep", "clamp", "highp", "return", "pow", "mediump", "length", "discard", "if", "sampler2D", "texture", "texture2D" } },
            { "wgsl", new[] { "f32", "let", "vec3", "var", "vec4", "fn", "return", "u32", "struct", "vec2", "array", "clamp", "sin", "cos", "arrayLength", "const", "for", "if", "dot", "mat4x4", "max", "normalize", "sampler", "textureSample", "texture_2d", "sqrt", "abs" } },
        };
    }
}
