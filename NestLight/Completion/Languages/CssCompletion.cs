using System;
using System.Collections.Generic;
using System.Linq;

namespace NestLight.Completion
{
    /// <summary>CSS: words with dashes, numbers that are followed by a unit, a grammar for the place of the caret, and the properties, values, functions and at-rules it offers.</summary>
    internal sealed class CssCompletion : CompletionLanguage
    {
        /// <summary>The id of the language: HTML names it for the CSS it holds.</summary>
        public const string Id = "css";

        public CssCompletion() : base(new[] { Id }, CssWords) { }

        public override bool IsExtraWordChar(char c) { return c == '-'; }

        public override bool TryAdjustWordStart(string text, int ownerStart, ref int start, int caret)
        {
            if (start >= caret) return true;
            // 10p| and -1.5r| are a number followed by the start of a unit; a hex color (#1a2b3c) is not
            int digits = start;
            if (text[digits] == '-') digits++;
            int unit = digits;
            while (unit < caret && char.IsDigit(text[unit])) unit++;
            if (unit == digits) return true;
            if (start > ownerStart && text[start - 1] == '#') return false;
            for (int k = unit; k < caret; k++) if (!char.IsLetter(text[k])) return false;
            start = unit;
            return true;
        }

        // ---- the grammar --------------------------------------------------------------------------------------

        private enum CssBlock { Rules, Keyframes, Declarations }

        private static readonly string[] CssRuleAtRules = { "@media", "@supports", "@layer", "@container", "@document" };

        protected override Position ReadPosition(string text, int floor, CompletionSite site)
        {
            var blocks = new List<CssBlock>(); // innermost last; none: the top level
            if (site.InlineDeclarations) blocks.Add(CssBlock.Declarations); // the value of a style attribute starts inside a rule
            int declarationStart = floor, colon = -1, parens = 0, brackets = 0;
            int i = floor;
            while (i < site.Start)
            {
                char c = text[i];
                if (c == '/' && i + 1 < site.Start && text[i + 1] == '*')
                {
                    int close = text.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    if (close < 0 || close >= site.Start) return new Position("css:comment", onlyWords: true);
                    i = close + 2;
                    continue;
                }
                if (c == '"' || c == '\'')
                {
                    int close = text.IndexOf(c, i + 1);
                    if (close < 0 || close >= site.Start) return new Position("css:string", onlyWords: true);
                    i = close + 1;
                    continue;
                }
                if (c == '(') parens++;
                else if (c == ')') { if (parens > 0) parens--; }
                else if (c == '[') brackets++;
                else if (c == ']') { if (brackets > 0) brackets--; }
                else if (parens == 0 && brackets == 0)
                {
                    CssBlock top = blocks.Count > 0 ? blocks[blocks.Count - 1] : CssBlock.Rules;
                    if (c == '{')
                    {
                        string prelude = text.Substring(declarationStart, i - declarationStart).Trim();
                        CssBlock kind = CssBlock.Declarations;
                        if (prelude.StartsWith("@", StringComparison.Ordinal))
                        {
                            if (prelude.IndexOf("keyframes", StringComparison.OrdinalIgnoreCase) >= 0) kind = CssBlock.Keyframes;
                            else if (CssRuleAtRules.Any(a => prelude.StartsWith(a, StringComparison.OrdinalIgnoreCase))) kind = CssBlock.Rules;
                        }
                        blocks.Add(kind); declarationStart = i + 1; colon = -1;
                    }
                    else if (c == '}') { if (blocks.Count > 0) blocks.RemoveAt(blocks.Count - 1); declarationStart = i + 1; colon = -1; }
                    else if (c == ';') { declarationStart = i + 1; colon = -1; }
                    else if (c == ':' && top == CssBlock.Declarations && colon < 0) colon = i;
                }
                i++;
            }

            CssBlock inside = blocks.Count > 0 ? blocks[blocks.Count - 1] : CssBlock.Rules;
            string pending = text.Substring(declarationStart, site.Start - declarationStart).TrimStart();

            if (parens > 0)
            {
                // the condition of a media query; anywhere else (url( ... ), calc( ... )) the place says nothing
                if (pending.StartsWith("@", StringComparison.Ordinal))
                {
                    if (char.IsDigit(site.Start > floor ? text[site.Start - 1] : ' ')) return new Position("css:unit", Units, onlyWords: true);
                    if (pending.StartsWith("@supports", StringComparison.OrdinalIgnoreCase)) return new Position("css:supports-feature", Properties);
                    if (pending.StartsWith("@container", StringComparison.OrdinalIgnoreCase)) return new Position("css:container-feature", ContainerFeatures);
                    return new Position("css:media-feature", MediaFeatures);
                }
                if (char.IsDigit(site.Start > floor ? text[site.Start - 1] : ' '))
                    return inside == CssBlock.Declarations ? new Position("css:unit", Units, onlyWords: true) : null; // rotate(45d|), not 2|n in :nth-child(2n)
                return null;
            }
            if (brackets > 0) return new Position("css:attribute-selector", SelectorAttributes, onlyWords: true);

            if (char.IsDigit(site.Start > floor ? text[site.Start - 1] : ' ') && inside != CssBlock.Declarations) return null; // 50|% in a keyframe, 2|n in a selector: no word to complete
            if (inside == CssBlock.Keyframes)
                return new Position("css:keyframe-selector", new[] { "from", "to" }, onlyWords: true);

            if (pending.StartsWith("@", StringComparison.Ordinal))
            {
                int space = pending.IndexOfAny(new[] { ' ', '\t', '\n', '\r' });
                string name = space < 0 ? pending : pending.Substring(0, space);
                if (space < 0) return new Position("css:at-rule", AtRules, onlyWords: true); // the name of the at-rule itself
                if (name.IndexOf("keyframes", StringComparison.OrdinalIgnoreCase) >= 0) return new Position("css:keyframes-name", onlyWords: true);
                if (name.Equals("@media", StringComparison.OrdinalIgnoreCase)) return new Position("css:media-query", MediaTypes);
                return null;
            }

            char before = site.Start > floor ? text[site.Start - 1] : ' ';
            if (char.IsDigit(before) && inside == CssBlock.Declarations) return new Position("css:unit", Units, onlyWords: true); // 10px: the unit after a number
            if (inside == CssBlock.Rules)
            {
                // a selector: a class or an id after '.' or '#', a pseudo-class after ':', otherwise an element
                if (before == '.' || before == '#') return new Position("css:class", onlyWords: true);
                if (before == ':')
                {
                    bool element = site.Start - 2 >= floor && text[site.Start - 2] == ':';
                    return element
                        ? new Position("css:pseudo-element", PseudoElements, onlyWords: true)
                        : new Position("css:pseudo-class", PseudoClasses, onlyWords: true);
                }
                return new Position("css:selector", HtmlCompletion.Tags, unlikely: w => IsKeyword(w) && !HtmlCompletion.IsTag(w));
            }

            if (colon < 0)
                return new Position("css:property", Properties, unlikely: w => IsValueOnly(w), priorOrder: true);

            if (before == '#') return new Position("css:hex", onlyWords: true);
            if (before == '!') return new Position("css:important", new[] { "important" }, onlyWords: true);

            // a value: the property is the word before the colon
            int end = colon;
            while (end > declarationStart && char.IsWhiteSpace(text[end - 1])) end--;
            int start = end;
            while (start > declarationStart && (IsWordChar(text[start - 1]) || text[start - 1] == '-')) start--;
            string property = text.Substring(start, end - start).ToLowerInvariant();
            IReadOnlyList<string> values = ValuesOf(property);
            return new Position("css:value:" + property, values, unlikely: w => !IsValueOnly(w) && !values.Contains(w, StringComparer.OrdinalIgnoreCase),
                secondary: FunctionsFor(property));
        }

        // ---- the words ----------------------------------------------------------------------------------------

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

        // ---- places in the grammar: what belongs where (see ReadPosition) ---------------------------------------------------

        private static readonly HashSet<string> CssPropertySet = new HashSet<string>(CssPropertyWords, StringComparer.OrdinalIgnoreCase);
        private static readonly HashSet<string> CssValueSet = new HashSet<string>(CssValueWords, StringComparer.OrdinalIgnoreCase);

        /// <summary>The properties of CSS, sorted.</summary>
        public static IReadOnlyList<string> Properties { get { return CssPropertyWords; } }

        /// <summary>Whether the word is a value and not also a property (<c>none</c>, but not <c>flex</c>).</summary>
        public static bool IsValueOnly(string word)
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
        public static IReadOnlyList<string> ValuesOf(string property)
        {
            IReadOnlyList<string> values;
            return property != null && CssValues.TryGetValue(property, out values) ? values : CssValueWords;
        }

        /// <summary>The conditions of a media query.</summary>
        public static readonly IReadOnlyList<string> MediaFeatures = Words(
            "width height min-width max-width min-height max-height orientation aspect-ratio resolution prefers-color-scheme prefers-reduced-motion hover pointer display-mode");

        public static readonly IReadOnlyList<string> ContainerFeatures = Words("width height inline-size block-size aspect-ratio orientation");

        public static readonly IReadOnlyList<string> AtRules = Words(
            "media supports keyframes font-face import layer container charset namespace property page counter-style font-feature-values scope starting-style position-try");

        public static readonly IReadOnlyList<string> Units = Words(
            "px rem em vh vw vmin vmax ch ex lh fr s ms deg rad turn grad svh dvh lvh cqw cqh dpi dppx cm mm in pt pc Q");

        /// <summary>The attributes a selector tests most: <c>input[type=...]</c>, <c>a[href^=...]</c>.</summary>
        public static readonly IReadOnlyList<string> SelectorAttributes = Words(
            "type href src name value disabled checked placeholder role target rel lang class id title for alt hidden readonly required selected open data-id aria-label aria-hidden aria-expanded");

        private static readonly HashSet<string> CssColorShorthands = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "background", "border", "border-top", "border-right", "border-bottom", "border-left", "outline", "box-shadow", "text-shadow", "text-decoration", "column-rule", "border-block", "border-inline" };

        public static readonly IReadOnlyList<string> MediaTypes = Words("screen print all and not only");

        public static readonly IReadOnlyList<string> PseudoClasses = Words(
            @"hover focus active visited focus-visible focus-within first-child last-child nth-child only-child first-of-type last-of-type nth-of-type
              not is where has checked disabled enabled required optional empty root target placeholder-shown read-only link");

        public static readonly IReadOnlyList<string> PseudoElements = Words("before after first-line first-letter placeholder selection marker backdrop");

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
        public static IReadOnlyList<string> FunctionsFor(string property)
        {
            IReadOnlyList<string> functions;
            bool colors = property != null && CssColorShorthands.Contains(property);
            if (property != null && CssFunctions.TryGetValue(property, out functions)) return colors ? functions.Concat(CssWithColors).Distinct().ToList() : functions;
            return colors ? CssWithColors : CssCommonFunctions;
        }

        private static readonly IReadOnlyList<string> CssWithColors = CssCommonFunctions.Concat(new[] { "transparent", "currentColor" }).Concat(CssNamedColors).ToList();

        private static readonly HashSet<string> KeywordSet = new HashSet<string>(CssWords, StringComparer.OrdinalIgnoreCase);

        /// <summary>Whether the word is a word of CSS (a property, a value), whatever its case.</summary>
        public static bool IsKeyword(string word)
        {
            return KeywordSet.Contains(word);
        }
    }
}
