using Xunit;

namespace NestLight.Tests
{
    /// <summary>GLSL and WGSL: keywords, types, built-ins, numbers, comments.</summary>
    public class ShaderTokenizerTests
    {
        private static string[] Glsl(string code) { return Lexer.Language("glsl", code); }
        private static string[] Wgsl(string code) { return Lexer.Language("wgsl", code); }

        // ---- GLSL ------------------------------------------------------------------------------------

        [Fact]
        public void Glsl_declaration()
        {
            Assert.Equal(new[] { "shader.keyword|uniform", "shader.type|vec3", "shader.number|1.0" }, Glsl("uniform vec3 color = 1.0;"));
        }

        [Theory]
        [InlineData("uniform")]
        [InlineData("varying")]
        [InlineData("attribute")]
        [InlineData("layout")]
        [InlineData("highp")]
        [InlineData("precision")]
        [InlineData("discard")]
        [InlineData("struct")]
        [InlineData("if")]
        [InlineData("for")]
        [InlineData("return")]
        public void Glsl_keywords(string word)
        {
            Assert.Equal(new[] { "shader.keyword|" + word }, Glsl(word));
        }

        [Theory]
        [InlineData("void")]
        [InlineData("float")]
        [InlineData("int")]
        [InlineData("uint")]
        [InlineData("vec2")]
        [InlineData("vec4")]
        [InlineData("ivec3")]
        [InlineData("mat4")]
        [InlineData("mat3x2")]
        [InlineData("sampler2D")]
        [InlineData("samplerCube")]
        [InlineData("isampler2D")]
        [InlineData("usampler2D")]
        [InlineData("sampler2DShadow")]
        public void Glsl_types(string word)
        {
            Assert.Equal(new[] { "shader.type|" + word }, Glsl(word));
        }

        [Theory]
        [InlineData("normalize")]
        [InlineData("mix")]
        [InlineData("texture")]
        [InlineData("texture2D")]
        [InlineData("dot")]
        [InlineData("gl_Position")]
        [InlineData("gl_FragCoord")]
        [InlineData("gl_AnythingElse")]
        public void Glsl_builtins(string word)
        {
            Assert.Equal(new[] { "shader.builtin|" + word }, Glsl(word));
        }

        [Fact]
        public void Glsl_preprocessor_directives_are_keywords()
        {
            Assert.Equal(new[] { "shader.keyword|#version", "shader.number|300", "shader.keyword|#define", "shader.number|1" },
                Glsl("#version 300 es\n  #define X 1"));
        }

        [Fact]
        public void A_hash_in_the_middle_of_a_line_is_not_a_directive()
        {
            Assert.Empty(Glsl("a # b"));
        }

        [Fact]
        public void Glsl_does_not_know_wgsl_words_and_the_other_way_around()
        {
            Assert.Empty(Glsl("fn let var"));
            Assert.Empty(Wgsl("uniform varying attribute"));
        }

        [Fact]
        public void Glsl_function()
        {
            Assert.Equal(new[]
            {
                "shader.type|void", "shader.keyword|if", "shader.builtin|dot", "shader.number|0.0",
                "shader.builtin|gl_FragColor", "shader.type|vec4", "shader.number|1.0", "shader.keyword|else", "shader.keyword|discard"
            }, Glsl("void main() { if (dot(a, b) > 0.0) gl_FragColor = vec4(1.0); else discard; }"));
        }

        // ---- numbers ------------------------------------------------------------------------------------

        [Theory]
        [InlineData("1")]
        [InlineData("1.0")]
        [InlineData(".5")]
        [InlineData("5.")]
        [InlineData("1e3")]
        [InlineData("1.5e-3")]
        [InlineData("2E+2")]
        [InlineData("0x1F")]
        [InlineData("1.0f")]
        [InlineData("1u")]
        [InlineData("1.0lf")]
        public void Numbers_with_exponents_hex_and_suffixes(string number)
        {
            Assert.Equal(new[] { "shader.number|" + number }, Glsl(number));
        }

        [Fact]
        public void Digits_inside_names_are_not_numbers()
        {
            Assert.Empty(Glsl("a1 b2c3"));
        }

        // ---- comments -------------------------------------------------------------------------------------

        [Fact]
        public void Line_and_block_comments_hide_their_words()
        {
            Assert.Equal(new[] { "shader.comment|// uniform", "shader.keyword|if", "shader.comment|/* vec3\nfloat */", "shader.keyword|for" },
                Glsl("// uniform\nif /* vec3\nfloat */ for"));
        }

        [Fact]
        public void Unterminated_block_comment_runs_to_the_end()
        {
            Assert.Equal(new[] { "shader.comment|/* open" }, Glsl("/* open"));
        }

        // ---- WGSL ------------------------------------------------------------------------------------------

        [Fact]
        public void Wgsl_entry_point()
        {
            Assert.Equal(new[]
            {
                "shader.keyword|@vertex", "shader.keyword|fn", "shader.keyword|@builtin", "shader.type|vec4f",
                "shader.keyword|return", "shader.type|vec4f", "shader.number|1.0"
            }, Wgsl("@vertex fn main() -> @builtin(position) vec4f { return vec4f(1.0); }"));
        }

        [Theory]
        [InlineData("fn")]
        [InlineData("let")]
        [InlineData("var")]
        [InlineData("const")]
        [InlineData("struct")]
        [InlineData("override")]
        [InlineData("loop")]
        [InlineData("continuing")]
        [InlineData("switch")]
        public void Wgsl_keywords(string word)
        {
            Assert.Equal(new[] { "shader.keyword|" + word }, Wgsl(word));
        }

        [Theory]
        [InlineData("f32")]
        [InlineData("i32")]
        [InlineData("u32")]
        [InlineData("bool")]
        [InlineData("vec3f")]
        [InlineData("vec2")]
        [InlineData("vec4u")]
        [InlineData("mat4x4f")]
        [InlineData("array")]
        [InlineData("texture_2d")]
        [InlineData("sampler")]
        public void Wgsl_types(string word)
        {
            Assert.Equal(new[] { "shader.type|" + word }, Wgsl(word));
        }

        [Theory]
        [InlineData("textureSample")]
        [InlineData("textureLoad")]
        [InlineData("saturate")]
        [InlineData("arrayLength")]
        [InlineData("workgroupBarrier")]
        public void Wgsl_builtins(string word)
        {
            Assert.Equal(new[] { "shader.builtin|" + word }, Wgsl(word));
        }

        [Fact]
        public void Wgsl_generic_types_color_both_names()
        {
            Assert.Equal(new[] { "shader.keyword|var", "shader.type|array", "shader.type|vec3", "shader.type|f32", "shader.number|4" },
                Wgsl("var x: array<vec3<f32>, 4>"));
        }

        [Fact]
        public void Wgsl_attributes_use_the_at_sign()
        {
            Assert.Equal(new[] { "shader.keyword|@group", "shader.number|0", "shader.keyword|@binding", "shader.number|1" }, Wgsl("@group(0) @binding(1)"));
        }

        [Fact]
        public void Interpolations_are_neutral()
        {
            Assert.Equal(new[] { "shader.type|vec3", "shader.number|1.0" }, Glsl("vec3 ${name} = 1.0"));
            Assert.Equal(new[] { "shader.keyword|let" }, Wgsl("let ${name}"));
        }

        [Fact]
        public void Empty_input()
        {
            Assert.Empty(Glsl(""));
            Assert.Empty(Wgsl("   "));
        }
    }
}
