using System.Linq;
using NestLight.EmbeddedLanguages;

namespace NestLight.Completion
{
    /// <summary>Shell/Bash: keywords, builtins, common commands, and a grammar for the place of the caret.</summary>
    internal sealed class ShellCompletion : CompletionLanguage
    {
        public ShellCompletion() : base(new[] { "shell", "bash", "sh", "zsh" },
            ShellTokenizer.Keywords.Concat(ShellTokenizer.Builtins),
            ShellTokenizer.CommonCommands.OrderBy(w => w).ToArray()) { }

        public override bool IsExtraWordChar(char c) { return c == '-'; }

        protected override Position ReadPosition(string text, int floor, CompletionSite site)
        {
            int lineStart = LineStart(text, site.Start, floor);
            string line = text.Substring(lineStart, site.Start - lineStart).TrimStart();

            if (line.Length == 0) return new Position("shell:command");

            if (line.StartsWith("#", System.StringComparison.Ordinal))
                return new Position("shell:comment", onlyWords: true);

            if (line.EndsWith("| ", System.StringComparison.Ordinal) || line.EndsWith("&& ", System.StringComparison.Ordinal)
                || line.EndsWith("|| ", System.StringComparison.Ordinal) || line.EndsWith("; ", System.StringComparison.Ordinal))
                return new Position("shell:command");

            char before = site.Start > floor ? text[site.Start - 1] : ' ';
            if (before == '$') return new Position("shell:variable", onlyWords: true);

            if (line.EndsWith("- ", System.StringComparison.Ordinal) || (before == '-' && line.Length > 0))
                return new Position("shell:flag", onlyWords: true);

            return null;
        }
    }
}
