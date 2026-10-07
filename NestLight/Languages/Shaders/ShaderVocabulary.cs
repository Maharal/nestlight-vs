using System.Collections.Generic;
using System.Linq;

namespace NestLight.Languages
{
    /// <summary>The word lists of GLSL and WGSL.</summary>
    internal static class ShaderVocabulary
    {
        private static string[] Words(string text)
        {
            return text.Split(new[] { ' ', '\n', '\r', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
        }

        // ---- GLSL -----------------------------------------------------------------------------

        public static readonly IReadOnlyList<string> GlslKeywords = Words(
            @"const uniform in out inout attribute varying layout flat smooth noperspective centroid sample patch
              precision highp mediump lowp if else for while do switch case default break continue return discard
              struct true false invariant precise buffer shared coherent volatile restrict readonly writeonly subroutine");

        public static readonly IReadOnlyList<string> GlslTypes = Words(
            @"void bool int uint float double
              vec2 vec3 vec4 ivec2 ivec3 ivec4 uvec2 uvec3 uvec4 bvec2 bvec3 bvec4 dvec2 dvec3 dvec4
              mat2 mat3 mat4 mat2x2 mat2x3 mat2x4 mat3x2 mat3x3 mat3x4 mat4x2 mat4x3 mat4x4
              dmat2 dmat3 dmat4 atomic_uint")
            .Concat(new[] { "", "i", "u" }.SelectMany(prefix => new[]
                { "sampler1D", "sampler2D", "sampler3D", "samplerCube", "sampler2DArray", "sampler2DMS", "sampler2DRect",
                  "samplerBuffer", "samplerCubeArray", "sampler1DArray", "image2D", "image3D", "imageCube" }
                .Select(s => prefix + s)))
            .Concat(new[] { "sampler1DShadow", "sampler2DShadow", "samplerCubeShadow", "sampler2DArrayShadow" })
            .ToList();

        public static readonly IReadOnlyList<string> GlslBuiltins = Words(
            @"radians degrees sin cos tan asin acos atan sinh cosh tanh asinh acosh atanh pow exp log exp2 log2 sqrt
              inversesqrt abs sign floor ceil trunc round roundEven fract mod modf min max clamp mix step smoothstep
              isnan isinf length distance dot cross normalize reflect refract faceforward matrixCompMult outerProduct
              transpose determinant inverse lessThan lessThanEqual greaterThan greaterThanEqual equal notEqual any all not
              texture texture2D texture3D textureCube textureLod textureGrad textureProj textureOffset texelFetch
              textureSize textureGather dFdx dFdy fwidth floatBitsToInt floatBitsToUint intBitsToFloat uintBitsToFloat
              packUnorm4x8 unpackUnorm4x8 bitCount findLSB findMSB EmitVertex EndPrimitive barrier
              imageLoad imageStore atomicAdd atomicMin atomicMax");

        public const string GlslBuiltinPrefix = "gl_";

        // ---- WGSL -----------------------------------------------------------------------------

        public static readonly IReadOnlyList<string> WgslKeywords = Words(
            @"fn let var const override struct alias if else for while loop switch case default break continue
              continuing return discard true false enable requires diagnostic const_assert");

        public static readonly IReadOnlyList<string> WgslTypes = Words(
            @"bool i32 u32 f32 f16 array atomic ptr sampler sampler_comparison
              texture_1d texture_2d texture_2d_array texture_3d texture_cube texture_cube_array texture_multisampled_2d
              texture_depth_2d texture_depth_2d_array texture_depth_cube texture_depth_cube_array texture_depth_multisampled_2d
              texture_storage_1d texture_storage_2d texture_storage_2d_array texture_storage_3d texture_external")
            .Concat(new[] { "vec2", "vec3", "vec4" }.SelectMany(v => new[] { "", "f", "i", "u", "h" }.Select(s => v + s)))
            .Concat(new[] { "mat2x2", "mat2x3", "mat2x4", "mat3x2", "mat3x3", "mat3x4", "mat4x2", "mat4x3", "mat4x4" }
                .SelectMany(v => new[] { "", "f", "h" }.Select(s => v + s)))
            .ToList();

        public static readonly IReadOnlyList<string> WgslBuiltins = Words(
            @"abs acos acosh all any arrayLength asin asinh atan atan2 atanh bitcast ceil clamp cos cosh cross degrees
              determinant distance dot dpdx dpdy exp exp2 floor fma fract fwidth inverseSqrt length log log2 max min mix
              modf normalize pow radians reflect refract round saturate select sign sin sinh smoothstep sqrt step tan tanh
              transpose trunc textureSample textureSampleLevel textureSampleBias textureSampleCompare textureLoad
              textureStore textureDimensions workgroupBarrier storageBarrier atomicAdd atomicLoad atomicStore");
    }
}
