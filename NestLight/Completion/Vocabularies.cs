using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.EmbeddedLanguages;

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
        public static IReadOnlyList<string> For(string embeddedLanguageId)
        {
            IReadOnlyList<string> words;
            return embeddedLanguageId != null && ById.TryGetValue(embeddedLanguageId, out words) ? words : new string[0];
        }

        // words that are offered but not colored: the tokenizers know them as plain identifiers or by a prefix, so they are not in For()
        private static readonly Dictionary<string, string[]> Extra = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "glsl", new[] { "main", "gl_FragColor", "gl_FragData", "gl_FragCoord", "gl_FragDepth", "gl_FrontFacing", "gl_PointCoord", "gl_PointSize", "gl_Position", "gl_VertexID", "gl_InstanceID" } },
            { "wgsl", new[] { "main" } },
        };

        private static readonly Dictionary<string, IReadOnlyList<string>> Merged = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The words the completion offers for the language: the vocabulary and the words that the tokenizers do not color (<c>main</c>, the
        /// <c>gl_</c> variables of GLSL), sorted. <see cref="For"/> stays the colored vocabulary.
        /// </summary>
        public static IReadOnlyList<string> ForCompletion(string embeddedLanguageId)
        {
            IReadOnlyList<string> words = For(embeddedLanguageId);
            string[] extra;
            if (embeddedLanguageId == null || !Extra.TryGetValue(embeddedLanguageId, out extra)) return words;
            lock (Merged)
            {
                IReadOnlyList<string> merged;
                if (Merged.TryGetValue(embeddedLanguageId.ToLowerInvariant(), out merged)) return merged;
                var list = new List<string>(words);
                foreach (string w in extra) if (!list.Contains(w, StringComparer.OrdinalIgnoreCase)) list.Add(w);
                list.Sort(StringComparer.OrdinalIgnoreCase);
                Merged[embeddedLanguageId.ToLowerInvariant()] = list;
                return list;
            }
        }

        /// <summary>Like <see cref="Find"/>, over the words the completion offers.</summary>
        public static string FindInCompletion(string embeddedLanguageId, string word)
        {
            string colored = Find(embeddedLanguageId, word);
            if (colored != null) return colored;
            string[] extra;
            if (embeddedLanguageId == null || !Extra.TryGetValue(embeddedLanguageId, out extra)) return null;
            return extra.FirstOrDefault(w => string.Equals(w, word, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Whether two ids name the same language (<c>html</c> and <c>svg</c> are one, <c>yaml</c> and <c>yml</c> too).</summary>
        public static bool SameLanguage(string a, string b)
        {
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) return true;
            IReadOnlyList<string> x, y;
            return a != null && b != null && ById.TryGetValue(a, out x) && ById.TryGetValue(b, out y) && ReferenceEquals(x, y);
        }

        /// <summary>The keyword of the language that is spelled like the word, ignoring case, in the spelling of the vocabulary; null when there is none.</summary>
        public static string Find(string embeddedLanguageId, string word)
        {
            IReadOnlyList<string> words;
            if (embeddedLanguageId == null || !ById.TryGetValue(embeddedLanguageId, out words)) return null;
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
        public static bool FollowsTypedCase(string embeddedLanguageId)
        {
            return string.Equals(embeddedLanguageId, "sql", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Characters, besides letters, digits and the underscore, that belong to a word of the language.</summary>
        public static bool IsExtraWordChar(string embeddedLanguageId, char c)
        {
            if (c != '-') return false;
            switch ((embeddedLanguageId ?? "").ToLowerInvariant())
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

        private static readonly IReadOnlyList<string> CssPropertyWords = Sorted(Words(
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
              width will-change word-break word-spacing word-wrap writing-mode z-index
              accent-color animation-composition animation-range animation-timeline background-position-x background-position-y block-size
              border-block border-block-color border-block-end border-block-end-color border-block-end-style border-block-end-width border-block-start
              border-block-start-color border-block-start-style border-block-start-width border-block-style border-block-width border-end-end-radius
              border-end-start-radius border-inline border-inline-color border-inline-end border-inline-end-color border-inline-end-style
              border-inline-end-width border-inline-start border-inline-start-color border-inline-start-style border-inline-start-width
              border-inline-style border-inline-width border-start-end-radius border-start-start-radius box-decoration-break break-after break-before
              break-inside clip color-scheme column-fill column-rule column-rule-color column-rule-style column-rule-width column-span column-width
              contain contain-intrinsic-size container container-name container-type content-visibility counter-set field-sizing font-display
              font-optical-sizing font-synthesis font-variant-caps font-variant-east-asian font-variant-ligatures font-variant-numeric
              font-variation-settings forced-color-adjust hanging-punctuation image-rendering inline-size inset-block inset-block-end inset-block-start
              inset-inline inset-inline-end inset-inline-start interpolate-size line-break margin-block margin-block-end margin-block-start
              margin-inline margin-inline-end margin-inline-start mask-clip mask-composite mask-image mask-mode mask-origin mask-position mask-repeat
              mask-size mask-type max-block-size max-inline-size min-block-size min-inline-size offset offset-anchor offset-distance offset-path
              offset-position offset-rotate orphans overflow-anchor overflow-block overflow-clip-margin overflow-inline overscroll-behavior
              overscroll-behavior-block overscroll-behavior-inline overscroll-behavior-x overscroll-behavior-y padding-block padding-block-end
              padding-block-start padding-inline padding-inline-end padding-inline-start page paint-order perspective-origin print-color-adjust
              scroll-margin scroll-margin-block scroll-margin-bottom scroll-margin-inline scroll-margin-left scroll-margin-right scroll-margin-top
              scroll-padding scroll-padding-block scroll-padding-bottom scroll-padding-inline scroll-padding-left scroll-padding-right
              scroll-padding-top scroll-snap-stop scrollbar-color scrollbar-gutter scrollbar-width shape-image-threshold shape-margin shape-outside
              text-combine-upright text-decoration-skip-ink text-decoration-thickness text-emphasis text-emphasis-color text-emphasis-position
              text-emphasis-style text-justify text-orientation text-rendering text-underline-offset text-underline-position text-wrap text-wrap-mode
              text-wrap-style transform-box transition-behavior view-transition-name white-space-collapse zoom
              alignment-baseline clip-rule dominant-baseline fill-opacity fill-rule flood-color flood-opacity lighting-color marker marker-end
              marker-mid marker-start shape-rendering stop-color stop-opacity stroke-dasharray stroke-dashoffset stroke-linecap stroke-linejoin
              stroke-miterlimit stroke-opacity stroke-width text-anchor vector-effect"));

        private static IReadOnlyList<string> Sorted(IReadOnlyList<string> words)
        {
            return words.Distinct(StringComparer.Ordinal).OrderBy(w => w, StringComparer.OrdinalIgnoreCase).ToList();
        }

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

        private static readonly IReadOnlyList<string> CssNamedColors = Words(
            @"aliceblue antiquewhite aqua aquamarine azure beige bisque black blanchedalmond blue blueviolet brown burlywood cadetblue chartreuse
              chocolate coral cornflowerblue cornsilk crimson cyan darkblue darkcyan darkgoldenrod darkgray darkgreen darkgrey darkkhaki darkmagenta
              darkolivegreen darkorange darkorchid darkred darksalmon darkseagreen darkslateblue darkslategray darkslategrey darkturquoise darkviolet
              deeppink deepskyblue dimgray dimgrey dodgerblue firebrick floralwhite forestgreen fuchsia gainsboro ghostwhite gold goldenrod gray green
              greenyellow grey honeydew hotpink indianred indigo ivory khaki lavender lavenderblush lawngreen lemonchiffon lightblue lightcoral
              lightcyan lightgoldenrodyellow lightgray lightgreen lightgrey lightpink lightsalmon lightseagreen lightskyblue lightslategray
              lightslategrey lightsteelblue lightyellow lime limegreen linen magenta maroon mediumaquamarine mediumblue mediumorchid mediumpurple
              mediumseagreen mediumslateblue mediumspringgreen mediumturquoise mediumvioletred midnightblue mintcream mistyrose moccasin navajowhite
              navy oldlace olive olivedrab orange orangered orchid palegoldenrod palegreen paleturquoise palevioletred papayawhip peachpuff peru pink
              plum powderblue purple rebeccapurple red rosybrown royalblue saddlebrown salmon sandybrown seagreen seashell sienna silver skyblue
              slateblue slategray slategrey snow springgreen steelblue tan teal thistle tomato turquoise violet wheat white whitesmoke yellow yellowgreen");

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
            add("color background-color border-color outline-color fill stroke caret-color accent-color text-decoration-color column-rule-color stop-color flood-color lighting-color text-emphasis-color border-top-color border-right-color border-bottom-color border-left-color",
                ColorNames + " " + string.Join(" ", CssNamedColors));
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
            add("transition", "all " + string.Join(" ", CssPropertyWords) + " ease ease-in ease-out ease-in-out linear step-start step-end");
            add("transition-timing-function animation-timing-function", "ease ease-in ease-out ease-in-out linear step-start step-end");
            add("animation", "ease ease-in ease-out ease-in-out linear infinite normal reverse alternate alternate-reverse forwards backwards both paused running none");
            add("animation-direction", "normal reverse alternate alternate-reverse");
            add("animation-fill-mode", "none forwards backwards both");
            add("text-overflow", "clip ellipsis");
            add("transform", "none");
            add("font-family", "sans-serif serif monospace system-ui cursive fantasy inherit");
            add("background-size", "cover contain auto");
            add("background-repeat", "no-repeat repeat repeat-x repeat-y space round");
            add("background-position", "center top bottom left right");
            add("background-attachment", "scroll fixed local");
            add("border-width", "thin medium thick");
            add("border-collapse", "collapse separate");
            add("justify-items justify-self", "start end center stretch baseline");
            add("place-items place-content place-self", "center start end stretch space-between space-around");
            add("mix-blend-mode", "normal multiply screen overlay darken lighten color-dodge color-burn difference exclusion");
            add("overflow-wrap word-wrap", "normal break-word anywhere");
            add("scroll-behavior", "auto smooth");
            add("appearance", "none auto");
            add("backface-visibility", "visible hidden");
            add("content", "none normal");
            add("list-style", "none disc circle square decimal inside outside");
            add("object-position", "center top bottom left right");
            add("text-decoration-line", "none underline overline line-through");
            add("text-decoration-style", "solid double dotted dashed wavy");
            add("table-layout", "auto fixed");
            add("direction", "ltr rtl");
            add("writing-mode", "horizontal-tb vertical-rl vertical-lr");
            add("aspect-ratio", "auto");
            add("touch-action", "auto none manipulation pan-x pan-y pinch-zoom");
            add("isolation", "auto isolate");
            add("hyphens", "none manual auto");
            add("outline-offset border-radius", "0");
            return map;
        }

        /// <summary>The values that belong to the property; the values of CSS in general when the property is not known.</summary>
        public static IReadOnlyList<string> CssValuesOf(string property)
        {
            IReadOnlyList<string> values;
            return property != null && CssValues.TryGetValue(property, out values) ? values : CssValueWords;
        }

        /// <summary>The conditions of a media query.</summary>
        public static readonly IReadOnlyList<string> CssMediaFeatures = Words(
            "width height min-width max-width min-height max-height orientation aspect-ratio resolution prefers-color-scheme prefers-reduced-motion hover pointer display-mode");

        public static readonly IReadOnlyList<string> CssContainerFeatures = Words("width height inline-size block-size aspect-ratio orientation");

        public static readonly IReadOnlyList<string> CssAtRules = Words(
            "media supports keyframes font-face import layer container charset namespace property page counter-style font-feature-values scope starting-style position-try");

        public static readonly IReadOnlyList<string> CssUnits = Words(
            "px rem em vh vw vmin vmax ch ex lh fr s ms deg rad turn grad svh dvh lvh cqw cqh dpi dppx cm mm in pt pc Q");

        /// <summary>The attributes a selector tests most: <c>input[type=...]</c>, <c>a[href^=...]</c>.</summary>
        public static readonly IReadOnlyList<string> CssSelectorAttributes = Words(
            "type href src name value disabled checked placeholder role target rel lang class id title for alt hidden readonly required selected open data-id aria-label aria-hidden aria-expanded");

        private static readonly HashSet<string> CssColorShorthands = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "background", "border", "border-top", "border-right", "border-bottom", "border-left", "outline", "box-shadow", "text-shadow", "text-decoration", "column-rule", "border-block", "border-inline" };

        public static readonly IReadOnlyList<string> CssMediaTypes = Words("screen print all and not only");

        public static readonly IReadOnlyList<string> CssPseudoClasses = Words(
            @"hover focus active visited focus-visible focus-within first-child last-child nth-child only-child first-of-type last-of-type nth-of-type
              not is where has checked disabled enabled required optional empty root target placeholder-shown read-only link");

        public static readonly IReadOnlyList<string> CssPseudoElements = Words("before after first-line first-letter placeholder selection marker backdrop");

        private static readonly IReadOnlyList<string> CssCommonFunctions = Words("var calc url rgb rgba hsl hsla min max clamp");

        private static readonly Dictionary<string, IReadOnlyList<string>> CssFunctions = BuildCssFunctions();

        private static Dictionary<string, IReadOnlyList<string>> BuildCssFunctions()
        {
            var map = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);
            Action<string, string> add = (properties, values) => { foreach (string p in Words(properties)) map[p] = Words(values); };
            add("transform", "translate translateX translateY translateZ rotate rotateX rotateY scale scaleX scaleY skew skewX skewY matrix perspective var calc");
            add("grid-template-columns grid-template-rows grid-auto-columns grid-auto-rows", "repeat minmax fit-content auto-fill auto-fit var calc");
            add("background background-image", "linear-gradient radial-gradient conic-gradient url var rgba");
            add("filter backdrop-filter", "blur brightness contrast grayscale hue-rotate invert opacity saturate sepia drop-shadow");
            add("transition animation", "cubic-bezier steps var");
            add("clip-path", "circle ellipse inset polygon path");
            return map;
        }

        /// <summary>The functions that belong in the value of the property.</summary>
        public static IReadOnlyList<string> CssFunctionsFor(string property)
        {
            IReadOnlyList<string> functions;
            bool colors = property != null && CssColorShorthands.Contains(property);
            if (property != null && CssFunctions.TryGetValue(property, out functions)) return colors ? functions.Concat(CssWithColors).Distinct().ToList() : functions;
            return colors ? CssWithColors : CssCommonFunctions;
        }

        private static readonly IReadOnlyList<string> CssWithColors = CssCommonFunctions.Concat(new[] { "transparent", "currentColor" }).Concat(CssNamedColors).ToList();

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
            { "role", Words(@"alert alertdialog application article banner button cell checkbox columnheader combobox complementary contentinfo definition dialog directory document feed figure form grid gridcell group heading img link list listbox listitem log main marquee math menu menubar menuitem navigation none note option presentation progressbar radio radiogroup region row rowgroup rowheader scrollbar search searchbox separator slider spinbutton status switch tab table tablist tabpanel term textbox timer toolbar tooltip tree treegrid treeitem") },
            { "aria-hidden", Words("true false") },
            { "aria-expanded", Words("true false") },
            { "aria-pressed", Words("true false mixed") },
            { "aria-checked", Words("true false mixed") },
            { "aria-selected", Words("true false") },
            { "aria-disabled", Words("true false") },
            { "aria-live", Words("polite assertive off") },
            { "aria-current", Words("page step location date time true false") },
            { "aria-haspopup", Words("true false menu listbox tree grid dialog") },
            { "aria-autocomplete", Words("none inline list both") },
            { "aria-orientation", Words("horizontal vertical") },
            { "fill", Words("none currentColor transparent white black red green blue gray") },
            { "stroke", Words("none currentColor transparent white black red green blue gray") },
            { "stroke-linecap", Words("butt round square") },
            { "stroke-linejoin", Words("miter round bevel") },
            { "fill-rule", Words("nonzero evenodd") },
            { "preserveAspectRatio", Words("none xMinYMin xMidYMid xMaxYMax meet slice") },
            { "autocomplete", Words("on off name email username new-password current-password one-time-code tel street-address postal-code country cc-number") },
            { "enctype", Words("application/x-www-form-urlencoded multipart/form-data text/plain") },
            { "preload", Words("none metadata auto") },
            { "crossorigin", Words("anonymous use-credentials") },
            { "decoding", Words("sync async auto") },
            { "referrerpolicy", Words("no-referrer origin same-origin strict-origin strict-origin-when-cross-origin unsafe-url") },
            { "inputmode", Words("none text decimal numeric tel search email url") },
            { "scope", Words("col row colgroup rowgroup") },
            { "wrap", Words("soft hard off") },
            { "spellcheck", Words("true false") },
            { "contenteditable", Words("true false plaintext-only") },
            { "draggable", Words("true false") },
            { "translate", Words("yes no") },
            { "align", Words("left center right justify") },
            { "meta:name", Words("viewport description keywords author robots theme-color") },
            { "link:rel", Words("stylesheet icon preload prefetch canonical manifest alternate") },
            { "script:type", Words("module text/javascript application/json importmap") },
            { "input:autocomplete", Words("on off name email username new-password current-password one-time-code tel") },
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
