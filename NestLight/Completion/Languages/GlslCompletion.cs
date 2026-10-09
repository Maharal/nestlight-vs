
namespace NestLight.Completion
{
    /// <summary>GLSL: keywords, types and built-ins, the <c>gl_</c> variables, and a grammar for directives and qualifiers.</summary>
    internal sealed class GlslCompletion : CompletionLanguage
    {
        public GlslCompletion() : base(new[] { "glsl" }, Vocabularies.For("glsl"), Vocabularies.ExtraFor("glsl")) { }

        protected override Position ReadPosition(string text, int floor, CompletionSite site) { return Positions.Glsl(text, floor, site); }
    }
}
