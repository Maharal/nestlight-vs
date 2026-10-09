
namespace NestLight.Completion
{
    /// <summary>YAML: words with dashes, its literals, and a grammar that tells keys from values.</summary>
    internal sealed class YamlCompletion : CompletionLanguage
    {
        public YamlCompletion() : base(new[] { "yaml", "yml" }, Vocabularies.For("yaml")) { }

        public override bool IsExtraWordChar(char c) { return c == '-'; }

        protected override Position ReadPosition(string text, int floor, CompletionSite site) { return Positions.Yaml(text, floor, site); }
    }
}
