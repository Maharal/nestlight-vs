using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NestLight.Experiments
{
    /// <summary>GLSL and WGSL: vertex, fragment and compute shaders with the names graphics code uses.</summary>
    internal static class ShaderCorpus
    {
        private static readonly string[] Uniforms = { "uTime", "uResolution", "uMouse", "uColor", "uLightDirection", "uModel", "uView", "uProjection", "uOpacity", "uScale", "uNoise" };
        private static readonly string[] Varyings = { "vUv", "vNormal", "vPosition", "vColor", "vViewDirection", "vWorldPos" };
        private static readonly string[] Locals = { "color", "pulse", "light", "diffuse", "specular", "normalized", "mixed", "alpha", "dist", "angle", "noise", "wave", "fresnel" };

        private static string Indent(string text, string prefix) { return string.Join("\n", text.Split('\n').Select(l => l.Length == 0 ? l : prefix + l)); }

        // ---- GLSL -----------------------------------------------------------------------------------------------------

        public static CorpusDocument Glsl(Dice d, int count)
        {
            var sb = new StringBuilder("export const shaders = {\n");
            for (int n = 0; n < count; n++)
                sb.Append("  shader").Append(n).Append(": glsl`\n").Append(Indent(GlslBody(d), "    ")).Append("\n  `,\n");
            sb.Append("};\n");
            return new CorpusDocument { Language = "glsl", Text = sb.ToString(), Snippets = count };
        }

        private static string GlslExpr(Dice d, List<string> floats, List<string> vec3s, int depth)
        {
            string f = floats.Count > 0 ? d.Pick(floats) : "1.0";
            string v = vec3s.Count > 0 ? d.Pick(vec3s) : "vec3(1.0)";
            switch (d.Next(depth > 1 ? 3 : 9))
            {
                case 0: return f + " * " + d.Pick(new[] { "0.5", "2.0", "3.14159", "0.25" });
                case 1: return "sin(" + f + " * " + d.Pick(new[] { "2.0", "6.2831" }) + ")";
                case 2: return f;
                case 3: return "smoothstep(0.0, 1.0, " + GlslExpr(d, floats, vec3s, depth + 1) + ")";
                case 4: return "mix(" + GlslExpr(d, floats, vec3s, depth + 1) + ", " + GlslExpr(d, floats, vec3s, depth + 1) + ", " + f + ")";
                case 5: return "clamp(" + GlslExpr(d, floats, vec3s, depth + 1) + ", 0.0, 1.0)";
                case 6: return "pow(" + GlslExpr(d, floats, vec3s, depth + 1) + ", " + d.Pick(new[] { "2.0", "4.0", "0.5" }) + ")";
                case 7: return "length(" + v + ")";
                default: return "max(dot(" + v + ", " + (vec3s.Count > 0 ? d.Pick(vec3s) : "vec3(1.0)") + "), 0.0)";
            }
        }

        private static bool IsMatrix(string u) { return u == "uModel" || u == "uView" || u == "uProjection"; }
        private static bool IsVec3(string u) { return u == "uColor" || u == "uLightDirection"; }
        private static bool IsVec2(string u) { return u == "uResolution" || u == "uMouse"; }

        private static string GlslBody(Dice d)
        {
            bool es = d.Chance(0.6);
            var sb = new StringBuilder();
            if (es) sb.Append("#version 300 es\n");
            sb.Append(d.Chance(0.5) ? "precision highp float;\n\n" : "precision mediump float;\n\n");
            var uniforms = d.Some(Uniforms, d.Between(2, 4));
            var varyings = d.Some(Varyings, d.Between(1, 3));
            bool texture = d.Chance(0.5) && varyings.Contains("vUv");
            foreach (string u in uniforms) sb.Append("uniform ").Append(IsMatrix(u) ? "mat4" : IsVec3(u) ? "vec3" : IsVec2(u) ? "vec2" : "float").Append(' ').Append(u).Append(";\n");
            if (texture) sb.Append("uniform sampler2D uTexture;\n");
            foreach (string v in varyings) sb.Append(es ? "in " : "varying ").Append(v == "vUv" ? "vec2" : "vec3").Append(' ').Append(v).Append(";\n");
            if (es) sb.Append("out vec4 fragColor;\n");
            sb.Append('\n');
            var floats = new List<string> { "uTime" };
            floats.AddRange(uniforms.Where(u => !IsMatrix(u) && !IsVec3(u) && !IsVec2(u)));
            if (!uniforms.Contains("uTime")) sb.Insert(sb.ToString().IndexOf("\n\n", StringComparison.Ordinal) + 2, "uniform float uTime;\n");
            var vec3s = new List<string>(varyings.Where(v => v != "vUv"));
            vec3s.AddRange(uniforms.Where(IsVec3));
            if (d.Chance(0.5))
                sb.Append("float ").Append(d.Pick(new[] { "diffuse", "falloff", "ripple", "hash" })).Append("(vec3 normal, vec3 light) {\n  return max(dot(normalize(normal), normalize(light)), 0.0);\n}\n\n");
            sb.Append("void main() {\n");
            foreach (string l in d.Some(Locals, d.Between(2, 4)))
            {
                sb.Append("  float ").Append(l).Append(" = ").Append(GlslExpr(d, floats, vec3s, 0)).Append(";\n");
                floats.Add(l);
            }
            if (texture) sb.Append("  vec4 texel = ").Append(es ? "texture" : "texture2D").Append("(uTexture, vUv);\n");
            string rgb = texture ? "mix(texel.rgb, vec3(1.0, 0.4, 0.2), " + floats.Last() + ")" : "vec3(" + floats.Last() + ", " + floats.First() + ", 0.5)";
            if (d.Chance(0.3)) sb.Append("  if (").Append(floats.Last()).Append(" < 0.01) { discard; }\n");
            sb.Append("  ").Append(es ? "fragColor" : "gl_FragColor").Append(" = vec4(").Append(rgb).Append(", 1.0);\n}");
            return sb.ToString();
        }

        // ---- WGSL -----------------------------------------------------------------------------------------------------

        public static CorpusDocument Wgsl(Dice d, int count)
        {
            var sb = new StringBuilder("export const shaders = {\n");
            for (int n = 0; n < count; n++)
                sb.Append("  shader").Append(n).Append(": wgsl`\n").Append(Indent(WgslBody(d), "    ")).Append("\n  `,\n");
            sb.Append("};\n");
            return new CorpusDocument { Language = "wgsl", Text = sb.ToString(), Snippets = count };
        }

        private static string WgslBody(Dice d)
        {
            switch (d.Next(3))
            {
                case 0: return WgslRender(d);
                case 1: return WgslCompute(d);
                default: return WgslHelpers(d);
            }
        }

        private static string WgslRender(Dice d)
        {
            string sampler = d.Pick(new[] { "mySampler", "linearSampler", "texSampler" });
            string texture = d.Pick(new[] { "myTexture", "albedoTexture", "colorTexture" });
            return "struct Uniforms {\n  mvp : mat4x4<f32>,\n  time : f32,\n  tint : vec4<f32>,\n};\n\n"
                + "@group(0) @binding(0) var<uniform> uniforms : Uniforms;\n@group(0) @binding(1) var " + sampler + " : sampler;\n@group(0) @binding(2) var " + texture + " : texture_2d<f32>;\n\n"
                + "struct VertexOutput {\n  @builtin(position) position : vec4<f32>,\n  @location(0) uv : vec2<f32>,\n  @location(1) normal : vec3<f32>,\n};\n\n"
                + "@vertex\nfn vs_main(@location(0) pos : vec3<f32>, @location(1) uv : vec2<f32>, @location(2) normal : vec3<f32>) -> VertexOutput {\n  var out : VertexOutput;\n  out.position = uniforms.mvp * vec4<f32>(pos, 1.0);\n  out.uv = uv;\n  out.normal = normal;\n  return out;\n}\n\n"
                + "@fragment\nfn fs_main(in : VertexOutput) -> @location(0) vec4<f32> {\n  let color = textureSample(" + texture + ", " + sampler + ", in.uv);\n  let light = max(dot(normalize(in.normal), vec3<f32>(0.0, 1.0, 0.0)), 0.0);\n  return vec4<f32>(color.rgb * light * " + d.Pick(new[] { "abs(sin(uniforms.time))", "uniforms.tint.rgb", "0.8" }) + ", color.a);\n}";
        }

        private static string WgslCompute(Dice d)
        {
            string input = d.Pick(new[] { "input", "particles", "values" }), output = d.Pick(new[] { "output", "result", "next" });
            return "@group(0) @binding(0) var<storage, read> " + input + " : array<f32>;\n@group(0) @binding(1) var<storage, read_write> " + output + " : array<f32>;\n\nconst WORKGROUP_SIZE : u32 = " + d.Pick(new[] { "64u", "128u", "256u" }) + ";\n\n"
                + "fn square(value : f32) -> f32 {\n  return value * value;\n}\n\n@compute @workgroup_size(64)\nfn main(@builtin(global_invocation_id) id : vec3<u32>) {\n  let index = id.x;\n  if (index >= arrayLength(&" + input + ")) {\n    return;\n  }\n  var total : f32 = 0.0;\n  for (var i : u32 = 0u; i < 4u; i = i + 1u) {\n    total = total + square(" + input + "[index] + f32(i));\n  }\n  " + output + "[index] = " + d.Pick(new[] { "total", "sqrt(total)", "clamp(total, 0.0, 1.0)" }) + ";\n}";
        }

        private static string WgslHelpers(Dice d)
        {
            string name = d.Pick(new[] { "fresnel", "falloff", "remap", "rotate2d", "hash21" });
            return "fn " + name + "(value : f32, edge0 : f32, edge1 : f32) -> f32 {\n  let t = clamp((value - edge0) / (edge1 - edge0), 0.0, 1.0);\n  return t * t * (3.0 - 2.0 * t);\n}\n\n"
                + "fn palette(t : f32) -> vec3<f32> {\n  let a = vec3<f32>(0.5, 0.5, 0.5);\n  let b = vec3<f32>(0.5, 0.5, 0.5);\n  let c = vec3<f32>(1.0, 1.0, 1.0);\n  let d = vec3<f32>(0.0, 0.33, 0.67);\n  return a + b * cos(6.28318 * (c * t + d));\n}\n\n"
                + "struct Params {\n  resolution : vec2<f32>,\n  time : f32,\n};\n\n@group(0) @binding(0) var<uniform> params : Params;\n\n@fragment\nfn main(@builtin(position) frag : vec4<f32>) -> @location(0) vec4<f32> {\n  let uv = frag.xy / params.resolution;\n  let wave = " + name + "(sin(uv.x * 10.0 + params.time), -1.0, 1.0);\n  return vec4<f32>(palette(wave), 1.0);\n}";
        }
    }

    /// <summary>The corpus of 500 snippets for each language, in files of about ten snippets, half to learn from and half to measure on.</summary>
    internal static class RealisticCorpus
    {
        public const int SnippetsPerLanguage = 500;
        public const int PerFile = 10;

        public static readonly string[] Languages = { "sql", "css", "html", "graphql", "json", "yaml", "glsl", "wgsl" };

        public static List<CorpusDocument> Documents(string language, int snippets, int seed)
        {
            var documents = new List<CorpusDocument>();
            int files = Math.Max(2, snippets / PerFile);
            for (int f = 0; f < files; f++)
            {
                var d = new Dice(seed * 7919 + f * 104729 + language.Length);
                CorpusDocument document;
                switch (language)
                {
                    case "sql": document = SqlCorpus.Document(d, PerFile); break;
                    case "css": document = WebCorpus.Css(d, PerFile); break;
                    case "html": document = WebCorpus.Html(d, PerFile); break;
                    case "graphql": document = DataCorpus.GraphQl(d, PerFile); break;
                    case "json": document = DataCorpus.Json(d, PerFile); break;
                    case "yaml": document = DataCorpus.Yaml(d, PerFile); break;
                    case "glsl": document = ShaderCorpus.Glsl(d, PerFile); break;
                    default: document = ShaderCorpus.Wgsl(d, PerFile); break;
                }
                document.Train = f % 2 == 0;
                documents.Add(document);
            }
            return documents;
        }
    }
}
