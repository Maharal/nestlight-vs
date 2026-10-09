using System;
using System.Linq;
using NestLight.EmbeddedLanguages;

namespace NestLight.Completion
{
    /// <summary>YAML: words with dashes, its literals, and a grammar that tells keys from values.</summary>
    internal sealed class YamlCompletion : CompletionLanguage
    {
        public YamlCompletion() : base(new[] { "yaml", "yml" }, YamlTokenizer.Literals.Where(w => w != "~")) { }

        public override bool IsExtraWordChar(char c) { return c == '-'; }

        // ---- the grammar --------------------------------------------------------------------------------------

        protected override Position ReadPosition(string text, int floor, CompletionSite site)
        {
            int lineStart = site.Start;
            while (lineStart > floor && text[lineStart - 1] != '\n') lineStart--;
            string line = text.Substring(lineStart, site.Start - lineStart).TrimStart();
            while (line.StartsWith("- ", StringComparison.Ordinal)) line = line.Substring(2).TrimStart();

            char quote = '\0';
            bool value = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (quote != '\0') { if (c == quote) quote = '\0'; continue; }
                if (c == '"' || c == '\'') quote = c;
                else if (c == '#' && (i == 0 || char.IsWhiteSpace(line[i - 1]))) return new Position("yaml:comment", onlyWords: true);
                else if (c == ':' && (i + 1 == line.Length || char.IsWhiteSpace(line[i + 1]))) value = true; // after "key: ": true, false, yes, no are possible
            }
            if (quote != '\0') return new Position("yaml:string", onlyWords: true);
            return value ? null : new Position("yaml:key", onlyWords: true);
        }
    }
}
