namespace NestLight.Completion
{
    /// <summary>CSS: words with dashes, numbers that are followed by a unit, a grammar for the place of the caret.</summary>
    internal sealed class CssCompletion : CompletionLanguage
    {
        public CssCompletion() : base(new[] { "css" }, Vocabularies.For("css")) { }

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

        protected override Position ReadPosition(string text, int floor, CompletionSite site) { return Positions.Css(text, floor, site); }
    }
}
