
namespace NestLight.Completion
{
    /// <summary>WGSL: keywords, types and built-ins, attributes, address spaces, and a grammar for the place of the caret.</summary>
    internal sealed class WgslCompletion : CompletionLanguage
    {
        public WgslCompletion() : base(new[] { "wgsl" }, Vocabularies.For("wgsl"), Vocabularies.ExtraFor("wgsl")) { }

        protected override Position ReadPosition(string text, int floor, CompletionSite site) { return Positions.Wgsl(text, floor, site); }
    }
}
