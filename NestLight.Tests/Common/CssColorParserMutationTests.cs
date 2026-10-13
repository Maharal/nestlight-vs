using NestLight.Common;
using Xunit;

namespace NestLight.Tests
{
    public class CssColorParserMutationTests
    {
        // --- ParseHex mutations ---

        // L36: r < 0 || g < 0 || b < 0 boundary mutations (<=, &&)
        [Fact]
        public void Hex_short_with_zero_channel_is_valid()
        {
            // #000 => all channels are 0. If mutated to <= 0, this would return null.
            var c = CssColorParser.TryParse("#000");
            Assert.True(c.HasValue);
            Assert.Equal(0, c.Value.R);
            Assert.Equal(0, c.Value.G);
            Assert.Equal(0, c.Value.B);
        }

        [Fact]
        public void Hex_short_invalid_char_returns_null()
        {
            // 'g' is not a valid hex char => HexVal returns -1
            Assert.Null(CssColorParser.TryParse("#ggg"));
            Assert.Null(CssColorParser.TryParse("#0g0"));
            Assert.Null(CssColorParser.TryParse("#00g"));
        }

        // L48: g > 0, b > 0 mutations (should be < 0)
        [Fact]
        public void Hex_long_with_zero_channel_is_valid()
        {
            var c = CssColorParser.TryParse("#000000");
            Assert.True(c.HasValue);
            Assert.Equal(0, c.Value.R);
            Assert.Equal(0, c.Value.G);
            Assert.Equal(0, c.Value.B);
        }

        [Fact]
        public void Hex_long_invalid_second_byte_returns_null()
        {
            Assert.Null(CssColorParser.TryParse("#00gg00"));
        }

        [Fact]
        public void Hex_long_invalid_third_byte_returns_null()
        {
            Assert.Null(CssColorParser.TryParse("#0000gg"));
        }

        // L48: 5-char hex (#RGBA)
        [Fact]
        public void Hex_five_char_with_alpha_is_valid()
        {
            var c = CssColorParser.TryParse("#f00f");
            Assert.True(c.HasValue);
            Assert.Equal(255, c.Value.R);
            Assert.Equal(0, c.Value.G);
            Assert.Equal(0, c.Value.B);
            Assert.Equal(255, c.Value.A);
        }

        [Fact]
        public void Hex_five_char_zero_alpha_is_valid()
        {
            var c = CssColorParser.TryParse("#f000");
            Assert.True(c.HasValue);
            Assert.Equal(0, c.Value.A);
        }

        // L73: 9-char hex (#RRGGBBAA)
        [Fact]
        public void Hex_nine_char_with_alpha_is_valid()
        {
            var c = CssColorParser.TryParse("#ff000080");
            Assert.True(c.HasValue);
            Assert.Equal(255, c.Value.R);
            Assert.Equal(0, c.Value.G);
            Assert.Equal(0, c.Value.B);
            Assert.Equal(128, c.Value.A);
        }

        [Fact]
        public void Hex_nine_char_invalid_alpha_returns_null()
        {
            Assert.Null(CssColorParser.TryParse("#ff0000gg"));
        }

        // --- ParseRgb mutations ---

        // L64: open < 0 || close < 0 || close <= open (boundary checks)
        [Fact]
        public void Rgb_no_parens_returns_null()
        {
            Assert.Null(CssColorParser.TryParse("rgb 128, 0, 255"));
        }

        [Fact]
        public void Rgb_close_before_open_returns_null()
        {
            Assert.Null(CssColorParser.TryParse("rgb)(128, 0, 255"));
        }

        [Fact]
        public void Rgb_close_equals_open_returns_null()
        {
            // This shouldn't parse since there's nothing between parens
            Assert.Null(CssColorParser.TryParse("rgb()"));
        }

        // L73: all four channel checks < 0
        [Fact]
        public void Rgb_with_zero_channels_is_valid()
        {
            var c = CssColorParser.TryParse("rgb(0, 0, 0)");
            Assert.True(c.HasValue);
            Assert.Equal(0, c.Value.R);
            Assert.Equal(0, c.Value.G);
            Assert.Equal(0, c.Value.B);
        }

        // --- ParseHsl mutations ---

        // L81: same paren checks as rgb
        [Fact]
        public void Hsl_no_close_paren_returns_null()
        {
            Assert.Null(CssColorParser.TryParse("hsl(0, 100%, 50%"));
        }

        // L92: a <= 0 boundary
        [Fact]
        public void Hsl_with_zero_alpha_via_slash()
        {
            var c = CssColorParser.TryParse("hsl(0, 100%, 50% / 0)");
            Assert.True(c.HasValue);
            Assert.Equal(0, c.Value.A);
        }

        // L94: h % 360 arithmetic mutations
        [Fact]
        public void Hsl_negative_hue_wraps_correctly()
        {
            // hsl(-120, 100%, 50%) should be same as hsl(240, 100%, 50%)
            var c1 = CssColorParser.TryParse("hsl(-120, 100%, 50%)");
            var c2 = CssColorParser.TryParse("hsl(240, 100%, 50%)");
            Assert.True(c1.HasValue);
            Assert.True(c2.HasValue);
            Assert.Equal(c2.Value.R, c1.Value.R);
            Assert.Equal(c2.Value.G, c1.Value.G);
            Assert.Equal(c2.Value.B, c1.Value.B);
        }

        // L96: h / 360.0 arithmetic (h * 360.0 would be wrong)
        [Fact]
        public void Hsl_hue_360_wraps_to_zero()
        {
            var c1 = CssColorParser.TryParse("hsl(0, 100%, 50%)");
            var c2 = CssColorParser.TryParse("hsl(360, 100%, 50%)");
            Assert.True(c1.HasValue);
            Assert.True(c2.HasValue);
            Assert.Equal(c1.Value.R, c2.Value.R);
            Assert.Equal(c1.Value.G, c2.Value.G);
            Assert.Equal(c1.Value.B, c2.Value.B);
        }

        // --- SplitFunctionArgs mutations ---

        // L108: slash > 0 boundary (should be >= 0)
        [Fact]
        public void Rgb_modern_syntax_with_slash_alpha()
        {
            var c = CssColorParser.TryParse("rgb(255 128 0 / 0.5)");
            Assert.True(c.HasValue);
            Assert.Equal(255, c.Value.R);
            Assert.Equal(128, c.Value.G);
            Assert.Equal(0, c.Value.B);
            Assert.Equal(128, c.Value.A);
        }

        // L116: comma detection (IndexOf(',') >= 0)
        [Fact]
        public void Rgb_space_separated_no_commas()
        {
            var c = CssColorParser.TryParse("rgb(128 64 32)");
            Assert.True(c.HasValue);
            Assert.Equal(128, c.Value.R);
            Assert.Equal(64, c.Value.G);
            Assert.Equal(32, c.Value.B);
        }

        // L123: t.Length >= 0 (should be > 0) - empty parts filtered
        [Fact]
        public void Rgb_with_extra_commas_filters_empty_parts()
        {
            // "128,,0,,255" has empty parts between commas
            Assert.Null(CssColorParser.TryParse("rgb(128,,255)"));
        }

        // L125: slash boundary and list.Count >= 4 with alpha in 4th comma-separated arg
        [Fact]
        public void Rgba_legacy_comma_syntax_with_alpha()
        {
            var c = CssColorParser.TryParse("rgba(255, 0, 0, 0.5)");
            Assert.True(c.HasValue);
            Assert.Equal(255, c.Value.R);
            Assert.Equal(0, c.Value.G);
            Assert.Equal(0, c.Value.B);
            Assert.Equal(128, c.Value.A);
        }

        // --- HslToRgb mutations ---

        // L155-167: arithmetic mutations in HslToRgb
        [Fact]
        public void Hsl_green_120_degrees()
        {
            var c = CssColorParser.TryParse("hsl(120, 100%, 50%)");
            Assert.True(c.HasValue);
            Assert.Equal(0, c.Value.R);
            Assert.Equal(255, c.Value.G);
            Assert.Equal(0, c.Value.B);
        }

        [Fact]
        public void Hsl_blue_240_degrees()
        {
            var c = CssColorParser.TryParse("hsl(240, 100%, 50%)");
            Assert.True(c.HasValue);
            Assert.Equal(0, c.Value.R);
            Assert.Equal(0, c.Value.G);
            Assert.Equal(255, c.Value.B);
        }

        [Fact]
        public void Hsl_cyan_180_degrees()
        {
            var c = CssColorParser.TryParse("hsl(180, 100%, 50%)");
            Assert.True(c.HasValue);
            Assert.Equal(0, c.Value.R);
            Assert.Equal(255, c.Value.G);
            Assert.Equal(255, c.Value.B);
        }

        [Fact]
        public void Hsl_yellow_60_degrees()
        {
            var c = CssColorParser.TryParse("hsl(60, 100%, 50%)");
            Assert.True(c.HasValue);
            Assert.Equal(255, c.Value.R);
            Assert.Equal(255, c.Value.G);
            Assert.Equal(0, c.Value.B);
        }

        [Fact]
        public void Hsl_magenta_300_degrees()
        {
            var c = CssColorParser.TryParse("hsl(300, 100%, 50%)");
            Assert.True(c.HasValue);
            Assert.Equal(255, c.Value.R);
            Assert.Equal(0, c.Value.G);
            Assert.Equal(255, c.Value.B);
        }

        // L159: h < 1.0/6 boundary: hue exactly at boundary
        [Fact]
        public void Hsl_at_hue_sector_boundaries()
        {
            // 30 degrees = 1/12 of 360, well inside first sector
            var c30 = CssColorParser.TryParse("hsl(30, 100%, 50%)");
            Assert.True(c30.HasValue);
            Assert.Equal(255, c30.Value.R);
            Assert.True(c30.Value.G > 0);
            Assert.Equal(0, c30.Value.B);

            // 90 degrees, second sector
            var c90 = CssColorParser.TryParse("hsl(90, 100%, 50%)");
            Assert.True(c90.HasValue);
            Assert.True(c90.Value.R > 0 && c90.Value.R < 255);
            Assert.Equal(255, c90.Value.G);
            Assert.Equal(0, c90.Value.B);

            // 270 degrees (5th sector h >= 4/6 and < 5/6)
            var c270 = CssColorParser.TryParse("hsl(270, 100%, 50%)");
            Assert.True(c270.HasValue);
            Assert.True(c270.Value.R > 0);
            Assert.Equal(0, c270.Value.G);
            Assert.Equal(255, c270.Value.B);

            // 330 degrees (6th sector h >= 5/6)
            var c330 = CssColorParser.TryParse("hsl(330, 100%, 50%)");
            Assert.True(c330.HasValue);
            Assert.Equal(255, c330.Value.R);
            Assert.Equal(0, c330.Value.G);
            Assert.True(c330.Value.B > 0);
        }

        // L165-167: r1 - m / g1 - m / b1 - m vs + m, and * 255 vs / 255
        [Fact]
        public void Hsl_with_lightness_produces_correct_rgb()
        {
            // hsl(0, 0%, 50%) => gray (128, 128, 128)
            var c = CssColorParser.TryParse("hsl(0, 0%, 50%)");
            Assert.True(c.HasValue);
            Assert.Equal(128, c.Value.R);
            Assert.Equal(128, c.Value.G);
            Assert.Equal(128, c.Value.B);
        }

        // --- HexVal mutations ---

        // L199: c < '9' boundary (should be <=)
        [Fact]
        public void HexVal_nine_is_valid()
        {
            var c = CssColorParser.TryParse("#999");
            Assert.True(c.HasValue);
            Assert.Equal(153, c.Value.R);
        }

        // L200: c >= 'a' || c <= 'f' mutations
        [Fact]
        public void HexVal_lowercase_f_is_valid()
        {
            var c = CssColorParser.TryParse("#f0f");
            Assert.True(c.HasValue);
            Assert.Equal(255, c.Value.R);
            Assert.Equal(0, c.Value.G);
            Assert.Equal(255, c.Value.B);
        }

        [Fact]
        public void HexVal_lowercase_a_is_valid()
        {
            var c = CssColorParser.TryParse("#a0a");
            Assert.True(c.HasValue);
            Assert.Equal(170, c.Value.R);
        }

        // L208: hi < 0 && lo < 0 (should be ||)
        [Fact]
        public void HexByte_one_invalid_nibble_returns_null()
        {
            // First nibble valid, second invalid
            Assert.Null(CssColorParser.TryParse("#0g0000"));
            // First nibble invalid, second valid
            Assert.Null(CssColorParser.TryParse("#g00000"));
        }

        // L143: string mutation on "%"
        [Fact]
        public void TryParsePercent_requires_percent_sign()
        {
            // "50" without % should not be parsed as percent
            Assert.Null(CssColorParser.TryParse("hsl(0, 50, 50%)"));
        }

        // Invalid hex lengths
        [Fact]
        public void Hex_invalid_lengths_return_null()
        {
            Assert.Null(CssColorParser.TryParse("#ff"));
            Assert.Null(CssColorParser.TryParse("#ff00f"));  // 6 chars (5 hex digits + #)
            Assert.Null(CssColorParser.TryParse("#ff0000ff0")); // 10 chars
        }

        // Hsl with deg suffix
        [Fact]
        public void Hsl_with_deg_suffix()
        {
            var c = CssColorParser.TryParse("hsl(120deg, 100%, 50%)");
            Assert.True(c.HasValue);
            Assert.Equal(0, c.Value.R);
            Assert.Equal(255, c.Value.G);
        }

        // Percentage components
        [Fact]
        public void Rgb_with_percentage_components()
        {
            var c = CssColorParser.TryParse("rgb(100%, 0%, 50%)");
            Assert.True(c.HasValue);
            Assert.Equal(255, c.Value.R);
            Assert.Equal(0, c.Value.G);
            Assert.Equal(128, c.Value.B);
        }

        // Alpha as percentage
        [Fact]
        public void Rgb_alpha_as_percentage()
        {
            var c = CssColorParser.TryParse("rgba(255, 0, 0, 50%)");
            Assert.True(c.HasValue);
            Assert.Equal(128, c.Value.A);
        }

        // Alpha out of range
        [Fact]
        public void Rgb_alpha_above_one_returns_null()
        {
            Assert.Null(CssColorParser.TryParse("rgba(255, 0, 0, 2.0)"));
        }
    }
}
