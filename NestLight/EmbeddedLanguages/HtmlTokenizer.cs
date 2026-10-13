using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.EmbeddedLanguages
{
    /// <summary>
    /// Error-tolerant HTML tokenizer. Interpolations were already masked, so they work in any position:
    /// text, tag name, attribute name, value... CSS in &lt;style&gt; blocks and style="..." attributes is
    /// delegated to the CSS tokenizer of the registry.
    /// </summary>
    internal sealed class HtmlTokenizer : INestingTokenizer
    {
        private const char Mask = TextUtil.Mask;
        private static readonly string[] HtmlIds = { "html", "htm", "svg" };

        /// <summary>The language of the content of a <c>&lt;style&gt;</c> element and of the value of a <c>style</c> attribute.</summary>
        public const string StyleLanguageId = "css";

        private readonly IEmbeddedLanguageRegistry _languages;

        public HtmlTokenizer(IEmbeddedLanguageRegistry languages)
        {
            if (languages == null) throw new ArgumentNullException("languages");
            _languages = languages;
        }

        public IReadOnlyList<string> Ids { get { return HtmlIds; } }

        public void Tokenize(char[] m, int from, int to, TokenSink add)
        {
            Parse(m, from, to, add, region => Embedded(region.EmbeddedLanguageId, m, region.Start, region.End, add));
        }

        public IReadOnlyList<NestedRegion> FindRegions(char[] m, int from, int to)
        {
            return Regions(m, from, to);
        }

        private static readonly TokenSink Ignore = (a, b, type) => { };

        /// <summary>
        /// <see cref="FindRegions"/> without a registry: finding the regions does not need the tokenizers of the languages inside, only the
        /// rules of HTML that say where they are.
        /// </summary>
        public static IReadOnlyList<NestedRegion> Regions(char[] m, int from, int to)
        {
            var regions = new List<NestedRegion>();
            Parse(m, from, to, Ignore, regions.Add);
            return regions;
        }

        /// <summary>The one pass over the HTML: the tokens of the markup go to <paramref name="add"/> and the code of other languages to <paramref name="nested"/>.</summary>
        private static void Parse(char[] m, int from, int to, TokenSink add, RegionSink nested)
        {
            int n = to;
            int i = from;
            while (i < n)
            {
                if (m[i] == '{')
                {
                    int mustacheEnd = MustacheEnd(m, i, n);
                    if (mustacheEnd > 0) { add(i, mustacheEnd, ClassificationNames.Mustache); i = mustacheEnd; continue; }
                }
                if (m[i] != '<') { i++; continue; }

                // Comment <!-- ... -->
                if (TextUtil.StartsWith(m, i, n, "<!--"))
                {
                    int end = TextUtil.IndexOf(m, "-->", i + 4, n);
                    int stop = end < 0 ? n : end + 3;
                    add(i, stop, ClassificationNames.Comment);
                    i = stop;
                    continue;
                }

                int p = i + 1;
                bool closing = false;
                if (p < n && m[p] == '/') { closing = true; p++; }
                if (p >= n || !(char.IsLetter(m[p]) || m[p] == Mask)) { i++; continue; }

                add(i, p, ClassificationNames.Delimiter);          // "<" or "</"
                int nameStart = p;
                while (p < n && IsNameChar(m[p])) p++;
                add(nameStart, p, ClassificationNames.Tag);
                string tagName = new string(m, nameStart, p - nameStart).ToLowerInvariant();
                bool opened = false; // reached the '>' (not self-closing)

                // tag body: attributes
                while (p < n)
                {
                    char c = m[p];
                    if (char.IsWhiteSpace(c)) { p++; continue; }
                    if (c == '>') { add(p, p + 1, ClassificationNames.Delimiter); p++; opened = true; break; }
                    if (c == '/' && p + 1 < n && m[p + 1] == '>') { add(p, p + 2, ClassificationNames.Delimiter); p += 2; break; }
                    if (c == '<') break; // malformed tag; restart analysis

                    int a = p;
                    while (p < n && !char.IsWhiteSpace(m[p]) && m[p] != '=' && m[p] != '>' &&
                           m[p] != '"' && m[p] != '\'' && m[p] != '<' &&
                           !(m[p] == '/' && p + 1 < n && m[p + 1] == '>'))
                        p++;
                    if (p == a) { p++; continue; } // stray quotes etc.

                    add(a, p, AttributeType(m[a]));
                    bool isStyleAttr = TextUtil.EqualsIgnoreCase(m, a, p, "style");

                    int q = p;
                    while (q < n && char.IsWhiteSpace(m[q])) q++;
                    if (q < n && m[q] == '=')
                    {
                        q++;
                        while (q < n && char.IsWhiteSpace(m[q])) q++;
                        if (q < n && (m[q] == '"' || m[q] == '\''))
                        {
                            char quote = m[q];
                            int v = q;
                            q++;
                            while (q < n && m[q] != quote) q++;
                            bool closed = q < n;
                            if (isStyleAttr)
                            {
                                // style="prop: value; ..." -> CSS between the quotes
                                add(v, v + 1, ClassificationNames.AttributeValue);
                                nested(new NestedRegion { EmbeddedLanguageId = StyleLanguageId, Start = v + 1, End = q, InlineDeclarations = true });
                                if (closed) add(q, q + 1, ClassificationNames.AttributeValue);
                            }
                            else
                            {
                                Value(m, v, closed ? q + 1 : n, add);
                            }
                            q = closed ? q + 1 : n;
                        }
                        else
                        {
                            int v = q;
                            while (q < n && !char.IsWhiteSpace(m[q]) && m[q] != '>' &&
                                   !(m[q] == '/' && q + 1 < n && m[q + 1] == '>'))
                                q++;
                            Value(m, v, q, add);
                        }
                        p = q;
                    }
                }

                // <style> ... </style> -> CSS
                if (opened && !closing && tagName == "style")
                {
                    int close = TextUtil.IndexOfIgnoreCase(m, "</style", p, n);
                    int end = close < 0 ? n : close;
                    nested(new NestedRegion { EmbeddedLanguageId = StyleLanguageId, Start = p, End = end });
                    p = end;
                }
                i = p;
            }
        }

        /// <summary>An attribute value, with the Mustache tags inside it set apart from the rest of the value.</summary>
        private static void Value(char[] m, int from, int to, TokenSink add)
        {
            int seg = from;
            int i = from;
            while (i < to)
            {
                int end = m[i] == '{' ? MustacheEnd(m, i, to) : -1;
                if (end < 0) { i++; continue; }
                if (i > seg) add(seg, i, ClassificationNames.AttributeValue);
                add(i, end, ClassificationNames.Mustache);
                i = seg = end;
            }
            if (to > seg) add(seg, to, ClassificationNames.AttributeValue);
        }

        /// <summary>
        /// The end of the Mustache tag that starts at <paramref name="p"/>: <c>{{...}}</c> or <c>{{{...}}}</c>, the part every Mustache-like
        /// engine shares (Mustache, Handlebars, Vue, Angular...). Returns -1 when there is none or it is not closed.
        /// </summary>
        private static int MustacheEnd(char[] m, int p, int n)
        {
            if (p + 1 >= n || m[p] != '{' || m[p + 1] != '{') return -1;
            bool triple = p + 2 < n && m[p + 2] == '{';
            string close = triple ? "}}}" : "}}";
            int open = p + (triple ? 3 : 2);
            int end = TextUtil.IndexOf(m, close, open, n);
            return end < 0 ? -1 : end + close.Length;
        }

        private void Embedded(string id, char[] m, int from, int to, TokenSink add)
        {
            IEmbeddedLanguageTokenizer tokenizer = _languages.Find(id);
            if (tokenizer != null && to > from) tokenizer.Tokenize(m, from, to, add);
        }

        private static string AttributeType(char first)
        {
            switch (first)
            {
                case '@': return ClassificationNames.AttributeEvent;
                case '.': return ClassificationNames.AttributeProperty;
                case '?': return ClassificationNames.AttributeBoolean;
                default: return ClassificationNames.Attribute;
            }
        }

        private static bool IsNameChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == ':' || c == '.' || c == Mask;
        }
    }
}
