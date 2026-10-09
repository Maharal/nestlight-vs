using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Completion
{
    /// <summary>HTML and SVG: tags and attributes with dashes, a grammar for the place of the caret, and the CSS of <c>&lt;style&gt;</c> and <c>style=""</c>.</summary>
    internal sealed class HtmlCompletion : CompletionLanguage, INestedLanguages
    {
        private readonly ICompletionLanguage _css;

        public HtmlCompletion(ICompletionLanguage css) : base(new[] { "html", "htm", "svg" }, Vocabularies.For("html"))
        {
            if (css == null) throw new ArgumentNullException("css");
            _css = css;
        }

        public override bool IsExtraWordChar(char c) { return c == '-'; }

        protected override Position ReadPosition(string text, int floor, CompletionSite site) { return Positions.Html(text, floor, site); }

        public bool TryRegionAt(string text, EmbeddedString owner, int caret, out NestedRegion region)
        {
            NestedLanguages.CssRange css;
            if (NestedLanguages.TryCssAt(text, owner, caret, out css))
            {
                region = Region(css);
                return true;
            }
            region = default(NestedRegion);
            return false;
        }

        public IReadOnlyList<NestedRegion> RegionsIn(string text, EmbeddedString owner)
        {
            var regions = new List<NestedRegion>();
            foreach (NestedLanguages.CssRange r in NestedLanguages.CssIn(text, owner)) regions.Add(Region(r));
            return regions;
        }

        private NestedRegion Region(NestedLanguages.CssRange css)
        {
            return new NestedRegion { EmbeddedLanguageId = _css.Ids[0], Start = css.Start, End = css.End, InlineDeclarations = css.Attribute };
        }
    }
}
