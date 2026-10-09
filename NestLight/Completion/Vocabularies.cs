using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Languages;

namespace NestLight.Completion
{
    /// <summary>The keywords each embedded language offers, for the languages that have a closed vocabulary.</summary>
    internal static class Vocabularies
    {
        private static string[] Words(string text)
        {
            return text.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static readonly Dictionary<string, IReadOnlyList<string>> ById =
            new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);

        static Vocabularies()
        {
            Add(new[] { "html", "htm", "svg" }, HtmlTags);
            Add(new[] { "css" }, CssWords);
            Add(new[] { "sql" }, SqlTokenizer.Keywords);
            Add(new[] { "graphql", "gql" }, GraphQlWords);
            Add(new[] { "json" }, new[] { "true", "false", "null" });
            Add(new[] { "yaml", "yml" }, YamlTokenizer.Literals.Where(w => w != "~"));
            Add(new[] { "glsl" }, ShaderVocabulary.GlslKeywords.Concat(ShaderVocabulary.GlslTypes).Concat(ShaderVocabulary.GlslBuiltins));
            Add(new[] { "wgsl" }, ShaderVocabulary.WgslKeywords.Concat(ShaderVocabulary.WgslTypes).Concat(ShaderVocabulary.WgslBuiltins));
        }

        private static void Add(string[] ids, IEnumerable<string> words)
        {
            IReadOnlyList<string> list = words.Distinct(StringComparer.Ordinal).OrderBy(w => w, StringComparer.OrdinalIgnoreCase).ToList();
            foreach (string id in ids) ById[id] = list;
        }

        /// <summary>The keywords of the language, sorted; empty when it has no closed vocabulary (XML, Markdown, regex).</summary>
        public static IReadOnlyList<string> For(string languageId)
        {
            IReadOnlyList<string> words;
            return languageId != null && ById.TryGetValue(languageId, out words) ? words : new string[0];
        }

        /// <summary>Whether two ids name the same language (<c>html</c> and <c>svg</c> are one, <c>yaml</c> and <c>yml</c> too).</summary>
        public static bool SameLanguage(string a, string b)
        {
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) return true;
            IReadOnlyList<string> x, y;
            return a != null && b != null && ById.TryGetValue(a, out x) && ById.TryGetValue(b, out y) && ReferenceEquals(x, y);
        }

        /// <summary>The keyword of the language that is spelled like the word, ignoring case, in the spelling of the vocabulary; null when there is none.</summary>
        public static string Find(string languageId, string word)
        {
            IReadOnlyList<string> words;
            if (languageId == null || !ById.TryGetValue(languageId, out words)) return null;
            Dictionary<string, string> index;
            lock (Indexes)
            {
                if (!Indexes.TryGetValue(words, out index))
                {
                    index = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (string w in words) if (!index.ContainsKey(w)) index[w] = w;
                    Indexes[words] = index;
                }
            }
            string found;
            return index.TryGetValue(word, out found) ? found : null;
        }

        private static readonly Dictionary<IReadOnlyList<string>, Dictionary<string, string>> Indexes =
            new Dictionary<IReadOnlyList<string>, Dictionary<string, string>>();

        /// <summary>SQL is case-insensitive, so the keyword follows the case the user is typing.</summary>
        public static bool FollowsTypedCase(string languageId)
        {
            return string.Equals(languageId, "sql", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Characters, besides letters, digits and the underscore, that belong to a word of the language.</summary>
        public static bool IsExtraWordChar(string languageId, char c)
        {
            if (c != '-') return false;
            switch ((languageId ?? "").ToLowerInvariant())
            {
                case "html": case "htm": case "svg": case "css": case "yaml": case "yml": return true;
                default: return false;
            }
        }

        // ---- word lists ---------------------------------------------------------------------------------------

        private static readonly IReadOnlyList<string> GraphQlWords = GraphQlTokenizer.Operations
            .Concat(GraphQlTokenizer.TypeDefinitions)
            .Concat(GraphQlTokenizer.OtherKeywords)
            .Concat(GraphQlTokenizer.Literals)
            .Concat(Words("on Int Float String Boolean ID skip include deprecated specifiedBy"))
            .ToList();

        private static readonly IReadOnlyList<string> HtmlTags = Words(
            @"a abbr address area article aside audio b base blockquote body br button canvas caption cite code col colgroup
              data datalist dd del details dfn dialog div dl dt em embed fieldset figcaption figure footer form h1 h2 h3 h4 h5 h6
              head header hgroup hr html i iframe img input ins kbd label legend li link main map mark menu meta meter nav
              noscript object ol optgroup option output p picture pre progress q rp rt ruby s samp script search section select
              slot small source span strong style sub summary sup table tbody td template textarea tfoot th thead time title tr
              track u ul var video wbr
              svg path circle rect line polyline polygon ellipse g defs use symbol text tspan linearGradient radialGradient stop
              clipPath mask filter");

        private static readonly IReadOnlyList<string> CssPropertyWords = Words(
            @"align-content align-items align-self all animation animation-delay animation-direction animation-duration
              animation-fill-mode animation-iteration-count animation-name animation-play-state animation-timing-function
              aspect-ratio backdrop-filter backface-visibility background background-attachment background-blend-mode
              background-clip background-color background-image background-origin background-position background-repeat
              background-size border border-bottom border-bottom-color border-bottom-left-radius border-bottom-right-radius
              border-bottom-style border-bottom-width border-collapse border-color border-image border-left border-left-color
              border-left-style border-left-width border-radius border-right border-right-color border-right-style
              border-right-width border-spacing border-style border-top border-top-color border-top-left-radius
              border-top-right-radius border-top-style border-top-width border-width bottom box-shadow box-sizing
              caption-side caret-color clear clip-path color column-count column-gap columns content counter-increment
              counter-reset cursor direction display empty-cells fill filter flex flex-basis flex-direction flex-flow
              flex-grow flex-shrink flex-wrap float font font-family font-feature-settings font-kerning font-size
              font-size-adjust font-stretch font-style font-variant font-weight gap grid grid-area grid-auto-columns
              grid-auto-flow grid-auto-rows grid-column grid-column-end grid-column-start grid-gap grid-row grid-row-end
              grid-row-start grid-template grid-template-areas grid-template-columns grid-template-rows height hyphens
              inset isolation justify-content justify-items justify-self left letter-spacing line-height list-style
              list-style-image list-style-position list-style-type margin margin-bottom margin-left margin-right margin-top
              mask max-height max-width min-height min-width mix-blend-mode object-fit object-position opacity order
              outline outline-color outline-offset outline-style outline-width overflow overflow-wrap overflow-x overflow-y
              padding padding-bottom padding-left padding-right padding-top page-break-after page-break-before
              page-break-inside perspective place-content place-items place-self pointer-events position quotes resize
              right rotate row-gap scale scroll-behavior scroll-snap-align scroll-snap-type tab-size table-layout text-align
              text-align-last text-decoration text-decoration-color text-decoration-line text-decoration-style
              text-indent text-overflow text-shadow text-transform top touch-action transform transform-origin
              transform-style transition transition-delay transition-duration transition-property
              transition-timing-function translate unicode-bidi user-select vertical-align visibility white-space widows
              width will-change word-break word-spacing word-wrap writing-mode z-index");

        private static readonly IReadOnlyList<string> CssValueWords = Words(
            @"inherit initial unset revert none auto block inline inline-block flex grid absolute relative fixed sticky
              hidden visible solid dashed dotted center transparent currentColor");

        private static readonly IReadOnlyList<string> CssWords = CssPropertyWords.Concat(CssValueWords).ToList();

        // ---- places in the grammar: what belongs where (see Positions) ---------------------------------------------------

        private static readonly HashSet<string> CssPropertySet = new HashSet<string>(CssPropertyWords, StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> CssValueSet = new HashSet<string>(CssValueWords, StringComparer.OrdinalIgnoreCase);

        /// <summary>The properties of CSS, sorted.</summary>
        public static IReadOnlyList<string> CssProperties { get { return CssPropertyWords; } }

        /// <summary>Whether the word is a value and not also a property (<c>none</c>, but not <c>flex</c>).</summary>
        public static bool IsCssValueOnly(string word)
        {
            return CssValueSet.Contains(word) && !CssPropertySet.Contains(word);
        }

        private const string ColorNames = "transparent currentColor red green blue white black gray grey yellow orange purple pink brown";

        private static readonly Dictionary<string, IReadOnlyList<string>> CssValues = BuildCssValues();

        private static Dictionary<string, IReadOnlyList<string>> BuildCssValues()
        {
            var map = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);
            Action<string, string> add = (properties, values) => { foreach (string p in Words(properties)) map[p] = Words(values); };
            add("display", "block inline inline-block flex inline-flex grid inline-grid none contents table");
            add("position", "static relative absolute fixed sticky");
            add("overflow overflow-x overflow-y", "visible hidden scroll auto clip");
            add("cursor", "pointer default text move wait not-allowed grab crosshair help");
            add("text-align", "left right center justify start end");
            add("flex-direction", "row row-reverse column column-reverse");
            add("flex-wrap", "nowrap wrap wrap-reverse");
            add("justify-content", "flex-start flex-end center space-between space-around space-evenly start end");
            add("align-items align-self align-content", "stretch flex-start flex-end center baseline start end");
            add("color background-color border-color outline-color fill stroke caret-color", ColorNames);
            add("background", "none " + ColorNames);
            add("border-style outline-style border-top-style border-bottom-style border-left-style border-right-style", "none solid dashed dotted double groove ridge inset outset hidden");
            add("visibility", "visible hidden collapse");
            add("float", "left right none");
            add("clear", "both left right none");
            add("white-space", "normal nowrap pre pre-wrap pre-line");
            add("font-weight", "normal bold bolder lighter");
            add("font-style", "normal italic oblique");
            add("text-decoration", "none underline overline line-through");
            add("text-transform", "none uppercase lowercase capitalize");
            add("box-sizing", "content-box border-box");
            add("object-fit", "fill contain cover none scale-down");
            add("pointer-events", "auto none");
            add("user-select", "none auto text all");
            add("list-style-type", "none disc circle square decimal");
            add("resize", "none both horizontal vertical");
            add("word-break", "normal break-all keep-all break-word");
            add("flex", "none auto initial");
            add("grid-auto-flow", "row column dense");
            add("vertical-align", "baseline top middle bottom text-top text-bottom sub super");
            add("transition-property will-change", string.Join(" ", CssPropertyWords));
            return map;
        }

        /// <summary>The values that belong to the property; the values of CSS in general when the property is not known.</summary>
        public static IReadOnlyList<string> CssValuesOf(string property)
        {
            IReadOnlyList<string> values;
            return property != null && CssValues.TryGetValue(property, out values) ? values : CssValueWords;
        }

        private static readonly string[] HtmlGlobalAttributes = Words(
            @"class id style title lang dir hidden tabindex role draggable contenteditable accesskey slot spellcheck translate
              aria-label aria-hidden aria-expanded aria-labelledby aria-describedby aria-live");

        private static readonly Dictionary<string, string[]> HtmlAttributes = BuildHtmlAttributes();

        private static Dictionary<string, string[]> BuildHtmlAttributes()
        {
            var map = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            Action<string, string> add = (tags, attributes) => { foreach (string t in Words(tags)) map[t] = Words(attributes); };
            add("a", "href target rel download hreflang type");
            add("img", "src alt width height loading srcset sizes");
            add("input", "type name value placeholder required disabled checked readonly min max step pattern maxlength autocomplete autofocus");
            add("button", "type disabled name value form onclick");
            add("form", "action method enctype novalidate target autocomplete");
            add("label", "for form");
            add("select", "name multiple required disabled size");
            add("option", "value selected disabled label");
            add("textarea", "name rows cols placeholder required readonly disabled maxlength");
            add("td th", "colspan rowspan headers scope align");
            add("tr", "align");
            add("link", "rel href type media as crossorigin");
            add("meta", "name content charset http-equiv");
            add("script", "src type async defer crossorigin integrity");
            add("iframe", "src width height allow sandbox loading");
            add("video", "src controls autoplay loop muted poster preload width height");
            add("audio", "src controls autoplay loop muted preload");
            add("source", "src type srcset media sizes");
            add("ol", "start reversed type");
            add("li", "value");
            add("svg", "viewBox xmlns width height fill stroke preserveAspectRatio");
            add("path", "d fill stroke stroke-width");
            add("circle", "cx cy r fill stroke");
            add("rect", "x y width height rx ry fill stroke");
            add("line", "x1 y1 x2 y2 stroke");
            add("g", "transform fill stroke");
            add("text", "x y fill font-size");
            return map;
        }

        /// <summary>The attributes of the tag first, then the ones every tag has.</summary>
        public static IReadOnlyList<string> HtmlAttributesOf(string tag)
        {
            string[] own;
            var list = new List<string>();
            if (tag != null && HtmlAttributes.TryGetValue(tag, out own)) list.AddRange(own);
            foreach (string g in HtmlGlobalAttributes) if (!list.Contains(g)) list.Add(g);
            return list;
        }

        private static readonly Dictionary<string, string[]> HtmlValues = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "input:type", Words("text password email number checkbox radio submit button reset file date hidden search tel url") },
            { "button:type", Words("submit button reset") },
            { "target", Words("_blank _self _parent _top") },
            { "rel", Words("stylesheet noopener noreferrer nofollow icon preload") },
            { "method", Words("get post") },
            { "loading", Words("lazy eager") },
            { "dir", Words("ltr rtl auto") },
        };

        /// <summary>The values that belong to the attribute of the tag; empty when they are not a closed list (a class, an id).</summary>
        public static IReadOnlyList<string> HtmlValuesOf(string tag, string attribute)
        {
            string[] values;
            if (attribute == null) return new string[0];
            if (HtmlValues.TryGetValue(tag + ":" + attribute, out values) || HtmlValues.TryGetValue(attribute, out values)) return values;
            return new string[0];
        }
    }
}
