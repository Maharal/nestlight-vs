using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.EmbeddedLanguages;

namespace NestLight.Completion
{
    /// <summary>HTML and SVG: tags and attributes with dashes, a grammar for the place of the caret, and the CSS of <c>&lt;style&gt;</c> and <c>style=""</c>.</summary>
    internal sealed class HtmlCompletion : CompletionLanguage, INestedLanguages
    {
        public HtmlCompletion() : base(new[] { "html", "htm", "svg" }, Tags) { }

        public override bool IsExtraWordChar(char c) { return c == '-'; }

        // ---- the CSS inside ------------------------------------------------------------------------------------

        // Where the CSS is has one definition: the one of the tokenizer, which colors it. The completion asks it on the text of the string
        // with the interpolations masked, as the highlighter does, so the offsets are the ones of the document.

        public bool TryRegionAt(string text, EmbeddedString owner, int caret, out NestedRegion region)
        {
            foreach (NestedRegion r in RegionsIn(text, owner))
                if (r.Start <= caret && caret <= r.End) { region = r; return true; }
            region = default(NestedRegion);
            return false;
        }

        public IReadOnlyList<NestedRegion> RegionsIn(string text, EmbeddedString owner)
        {
            int start = owner.Start, end = Math.Min(owner.End, text.Length);
            if (end <= start) return new NestedRegion[0];
            var masked = new char[end - start];
            text.CopyTo(start, masked, 0, masked.Length);
            foreach (Interpolation x in owner.Interpolations)
                for (int k = Math.Max(x.Start, start); k < Math.Min(x.End, end); k++) masked[k - start] = TextUtil.Mask;

            IReadOnlyList<NestedRegion> found = HtmlTokenizer.Regions(masked, 0, masked.Length);
            var regions = new List<NestedRegion>(found.Count);
            foreach (NestedRegion r in found) regions.Add(new NestedRegion { EmbeddedLanguageId = r.EmbeddedLanguageId, Start = r.Start + start, End = r.End + start, InlineDeclarations = r.InlineDeclarations });
            return regions;
        }

        // ---- the grammar --------------------------------------------------------------------------------------

        private static readonly HashSet<string> VoidElements = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "source", "track", "wbr", "path", "circle", "rect", "line", "ellipse", "polygon", "polyline", "stop", "use" };

        protected override Position ReadPosition(string text, int floor, CompletionSite site)
        {
            // the last '<' or '>' before the caret says whether it is in a tag
            int open = -1;
            for (int i = site.Start - 1; i >= floor; i--)
            {
                if (text[i] == '>') break;
                if (text[i] == '<') { open = i; break; }
            }
            if (open < 0)
                return new Position("html:text", onlyWords: true); // text between tags: not a tag name unless '<' was typed

            int nameStart = open + 1;
            bool closing = nameStart < site.Start && text[nameStart] == '/';
            if (closing) nameStart++;
            if (nameStart == site.Start)
            {
                // right after '<': the vocabulary is the tags; after '</': the element that is open first
                if (!closing) return new Position("html:tag");
                string openTag = OpenElement(text, floor, open);
                return openTag == null ? new Position("html:tag") : new Position("html:closing-tag", new[] { openTag });
            }

            // inside a tag: its name, then the attribute being written or the value of one
            int nameEnd = nameStart;
            while (nameEnd < site.Start && (IsWordChar(text[nameEnd]) || text[nameEnd] == '-' || text[nameEnd] == ':')) nameEnd++;
            if (nameEnd == site.Start)
            {
                if (!closing) return new Position("html:tag");
                string openTag = OpenElement(text, floor, open);
                return openTag == null ? new Position("html:tag") : new Position("html:closing-tag", new[] { openTag });
            }
            if (closing) return null;
            string tag = text.Substring(nameStart, nameEnd - nameStart).ToLowerInvariant();

            char quote = '\0';
            int attribute = -1;
            for (int i = nameEnd; i < site.Start; i++)
            {
                char c = text[i];
                if (quote != '\0') { if (c == quote) quote = '\0'; continue; }
                if (c == '"' || c == '\'') quote = c;
                else if (c == '=') attribute = i;
                else if (char.IsWhiteSpace(c) && attribute >= 0 && i > attribute + 1) attribute = -1; // an unquoted value ended
            }
            if (quote != '\0' || (attribute >= 0 && attribute == LastNonSpace(text, site.Start)))
            {
                // the value of an attribute: the attribute is the word before '='; a tag name never belongs here
                int end = attribute;
                if (attribute < 0) return new Position("html:value", onlyWords: true);
                while (end > nameEnd && char.IsWhiteSpace(text[end - 1])) end--;
                int start = end;
                while (start > nameEnd && (IsWordChar(text[start - 1]) || text[start - 1] == '-')) start--;
                string name = text.Substring(start, end - start).ToLowerInvariant();
                return new Position("html:value:" + name, ValuesOf(tag, name), wordsFirst: true, onlyWords: true);
            }

            // an attribute name: the ones the tag does not have yet
            HashSet<string> present = AttributesOf(text, nameEnd, site);
            return new Position("html:attribute:" + tag, AttributesOfTag(tag).Where(a => !present.Contains(a)), onlyWords: true);
        }

        /// <summary>The attribute names written in the tag that contains the caret, before it and after it (the word being typed is not one).</summary>
        private static HashSet<string> AttributesOf(string text, int from, CompletionSite site)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            char quote = '\0';
            int i = from;
            int limit = Math.Min(text.Length, site.Start + Reach);
            while (i < limit)
            {
                if (i == site.Start) i = site.End; // skip the word under the caret
                if (i >= limit) break;
                char c = text[i];
                if (quote != '\0') { if (c == quote) quote = '\0'; i++; continue; }
                if (c == '"' || c == '\'') { quote = c; i++; continue; }
                if (c == '>' || c == '<') break;
                if (IsWordChar(c) || c == '-')
                {
                    int start = i;
                    while (i < limit && (IsWordChar(text[i]) || text[i] == '-' || text[i] == ':')) i++;
                    // an attribute name is not the unquoted value that follows '='
                    int p = start - 1;
                    while (p >= from && char.IsWhiteSpace(text[p])) p--;
                    if (!(p >= from && text[p] == '=')) names.Add(text.Substring(start, i - start));
                    continue;
                }
                i++;
            }
            return names;
        }

        /// <summary>The innermost element that is open at <paramref name="at"/>, found by reading the tags back to <paramref name="floor"/>; null when none is.</summary>
        private static string OpenElement(string text, int floor, int at)
        {
            var stack = new List<string>();
            int i = floor;
            while (i < at)
            {
                int lt = text.IndexOf('<', i, at - i);
                if (lt < 0) break;
                if (lt + 3 < text.Length && string.CompareOrdinal(text, lt, "<!--", 0, 4) == 0)
                {
                    int endComment = text.IndexOf("-->", lt + 4, StringComparison.Ordinal);
                    i = endComment < 0 || endComment >= at ? at : endComment + 3;
                    continue;
                }
                int p = lt + 1;
                bool closes = p < at && text[p] == '/';
                if (closes) p++;
                int nameStart = p;
                while (p < at && (IsWordChar(text[p]) || text[p] == '-' || text[p] == ':')) p++;
                if (p == nameStart) { i = lt + 1; continue; }
                string name = text.Substring(nameStart, p - nameStart);

                // the end of the tag, past quoted values
                char quote = '\0';
                int gt = -1;
                for (int k = p; k < at; k++)
                {
                    char c = text[k];
                    if (quote != '\0') { if (c == quote) quote = '\0'; continue; }
                    if (c == '"' || c == '\'') quote = c;
                    else if (c == '>') { gt = k; break; }
                }
                if (gt < 0) break;
                bool selfClosing = text[gt - 1] == '/';
                if (closes)
                {
                    int index = stack.FindLastIndex(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase));
                    if (index >= 0) stack.RemoveRange(index, stack.Count - index);
                }
                else if (!selfClosing && !VoidElements.Contains(name)) stack.Add(name);
                i = gt + 1;
            }
            return stack.Count == 0 ? null : stack[stack.Count - 1];
        }

        private static int LastNonSpace(string text, int before)
        {
            int i = before - 1;
            while (i >= 0 && char.IsWhiteSpace(text[i])) i--;
            return i;
        }

        // ---- the words ----------------------------------------------------------------------------------------

        private static readonly string[] TagWords = Words(
            @"a abbr address area article aside audio b base blockquote body br button canvas caption cite code col colgroup
              data datalist dd del details dfn dialog div dl dt em embed fieldset figcaption figure footer form h1 h2 h3 h4 h5 h6
              head header hgroup hr html i iframe img input ins kbd label legend li link main map mark menu meta meter nav
              noscript object ol optgroup option output p picture pre progress q rp rt ruby s samp script search section select
              slot small source span strong style sub summary sup table tbody td template textarea tfoot th thead time title tr
              track u ul var video wbr
              svg path circle rect line polyline polygon ellipse g defs use symbol text tspan linearGradient radialGradient stop
              clipPath mask filter");

        /// <summary>The tags, sorted and without repeats.</summary>
        public static readonly IReadOnlyList<string> Tags = TagWords.Distinct(StringComparer.Ordinal).OrderBy(w => w, StringComparer.OrdinalIgnoreCase).ToList();

        private static readonly HashSet<string> TagSet = new HashSet<string>(TagWords, StringComparer.OrdinalIgnoreCase);

        /// <summary>Whether the word is the name of an HTML or SVG tag, whatever its case.</summary>
        public static bool IsTag(string word)
        {
            return TagSet.Contains(word);
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
        public static IReadOnlyList<string> AttributesOfTag(string tag)
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
        public static IReadOnlyList<string> ValuesOf(string tag, string attribute)
        {
            string[] values;
            if (attribute == null) return new string[0];
            if (HtmlValues.TryGetValue(tag + ":" + attribute, out values) || HtmlValues.TryGetValue(attribute, out values)) return values;
            return new string[0];
        }
    }
}
