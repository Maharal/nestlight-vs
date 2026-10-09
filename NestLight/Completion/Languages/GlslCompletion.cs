using System;
using System.Linq;
using NestLight.EmbeddedLanguages;

namespace NestLight.Completion
{
    /// <summary>GLSL: keywords, types and built-ins, the <c>gl_</c> variables, and a grammar for directives and qualifiers.</summary>
    internal sealed class GlslCompletion : CompletionLanguage
    {
        public GlslCompletion() : base(new[] { "glsl" }, ShaderVocabulary.GlslKeywords.Concat(ShaderVocabulary.GlslTypes).Concat(ShaderVocabulary.GlslBuiltins), new[] { "main", "gl_FragColor", "gl_FragData", "gl_FragCoord", "gl_FragDepth", "gl_FrontFacing", "gl_PointCoord", "gl_PointSize", "gl_Position", "gl_VertexID", "gl_InstanceID" }) { }

        // ---- the grammar --------------------------------------------------------------------------------------

        private static readonly string[] GlslDirectives = { "version", "define", "ifdef", "ifndef", "if", "else", "elif", "endif", "extension", "pragma", "undef", "include" };

        protected override Position ReadPosition(string text, int floor, CompletionSite site)
        {
            int lineStart = LineStart(text, site.Start, floor);
            string line = text.Substring(lineStart, site.Start - lineStart).TrimStart();
            if (line.StartsWith("#", StringComparison.Ordinal))
            {
                string rest = line.Substring(1).TrimStart();
                if (rest.IndexOf(' ') < 0 && rest.IndexOf('\t') < 0) return new Position("glsl:directive", GlslDirectives, onlyWords: true);
                if (rest.StartsWith("version", StringComparison.Ordinal)) return new Position("glsl:version", new[] { "es", "core", "compatibility" }, onlyWords: true);
            }
            return null;
        }
    }
}
