namespace NestLight.Completion
{
    /// <summary>TOML: its literals and a grammar that tells tables, keys and values apart.</summary>
    internal sealed class TomlCompletion : CompletionLanguage
    {
        public TomlCompletion() : base(new[] { "toml" }, new[] { "false", "inf", "nan", "true" }) { }

        public override bool IsExtraWordChar(char c) { return c == '-'; }

        protected override Position ReadPosition(string text, int floor, CompletionSite site)
        {
            int lineStart = LineStart(text, site.Start, floor);
            string line = text.Substring(lineStart, site.Start - lineStart).TrimStart();

            if (line.StartsWith("#", System.StringComparison.Ordinal))
                return new Position("toml:comment", onlyWords: true);

            if (line.StartsWith("[", System.StringComparison.Ordinal))
                return new Position("toml:table", onlyWords: true);

            if (line.IndexOf('=') >= 0)
                return new Position("toml:value", new[] { "true", "false" });

            return new Position("toml:key", onlyWords: true);
        }
    }
}
