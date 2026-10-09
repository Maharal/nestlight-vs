using System;
using System.Linq;
using NestLight.EmbeddedLanguages;

namespace NestLight.Completion
{
    /// <summary>WGSL: keywords, types and built-ins, attributes, address spaces, and a grammar for the place of the caret.</summary>
    internal sealed class WgslCompletion : CompletionLanguage
    {
        public WgslCompletion() : base(new[] { "wgsl" }, ShaderVocabulary.WgslKeywords.Concat(ShaderVocabulary.WgslTypes).Concat(ShaderVocabulary.WgslBuiltins), new[] { "main" }) { }

        // ---- the grammar --------------------------------------------------------------------------------------

        private static readonly string[] WgslAttributes = { "location", "builtin", "group", "binding", "vertex", "fragment", "compute", "workgroup_size", "size", "align", "id", "interpolate", "invariant", "must_use", "diagnostic" };
        private static readonly string[] WgslBuiltinValues =
            { "position", "vertex_index", "instance_index", "front_facing", "frag_depth", "local_invocation_id", "local_invocation_index", "global_invocation_id", "workgroup_id", "num_workgroups", "sample_index", "sample_mask" };
        private static readonly string[] WgslAddressSpaces = { "function", "private", "workgroup", "uniform", "storage", "handle" };
        private static readonly string[] WgslAccessModes = { "read", "write", "read_write" };

        protected override Position ReadPosition(string text, int floor, CompletionSite site)
        {
            if (site.Start > floor && text[site.Start - 1] == '@') return new Position("wgsl:attribute", WgslAttributes, onlyWords: true);
            int lineStart = LineStart(text, site.Start, floor);
            string line = text.Substring(lineStart, site.Start - lineStart);

            int builtin = line.LastIndexOf("@builtin(", StringComparison.Ordinal);
            if (builtin >= 0 && line.IndexOf(')', builtin) < 0) return new Position("wgsl:builtin-value", WgslBuiltinValues, onlyWords: true);
            int interpolate = line.LastIndexOf("@interpolate(", StringComparison.Ordinal);
            if (interpolate >= 0 && line.IndexOf(')', interpolate) < 0)
                return line.IndexOf(',', interpolate) < 0
                    ? new Position("wgsl:interpolate-type", new[] { "perspective", "linear", "flat" }, onlyWords: true)
                    : new Position("wgsl:interpolate-sampling", new[] { "center", "centroid", "sample", "first", "either" }, onlyWords: true);

            int declaration = Math.Max(line.LastIndexOf("var<", StringComparison.Ordinal), line.LastIndexOf("ptr<", StringComparison.Ordinal));
            if (declaration >= 0 && line.IndexOf('>', declaration + 4) < 0)
                return line.IndexOf(',', declaration) < 0
                    ? new Position("wgsl:address-space", WgslAddressSpaces, onlyWords: true)
                    : new Position("wgsl:access-mode", WgslAccessModes, onlyWords: true);
            return null;
        }
    }
}
