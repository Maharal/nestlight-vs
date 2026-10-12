using System;
using System.Collections.Generic;

namespace NestLight.Common
{
    internal struct CssColor
    {
        public readonly byte R, G, B, A;
        public CssColor(byte r, byte g, byte b, byte a = 255) { R = r; G = g; B = b; A = a; }
    }

    internal static class CssColorParser
    {
        public static CssColor? TryParse(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            text = text.Trim();

            if (text[0] == '#') return ParseHex(text);

            string lower = text.ToLowerInvariant();
            if (lower.StartsWith("rgb")) return ParseRgb(lower);
            if (lower.StartsWith("hsl")) return ParseHsl(lower);

            CssColor c;
            if (NamedColors.TryGetValue(lower, out c)) return c;

            return null;
        }

        private static CssColor? ParseHex(string hex)
        {
            if (hex.Length == 4)
            {
                int r = HexVal(hex[1]); int g = HexVal(hex[2]); int b = HexVal(hex[3]);
                if (r < 0 || g < 0 || b < 0) return null;
                return new CssColor((byte)(r * 17), (byte)(g * 17), (byte)(b * 17));
            }
            if (hex.Length == 5)
            {
                int r = HexVal(hex[1]); int g = HexVal(hex[2]); int b = HexVal(hex[3]); int a = HexVal(hex[4]);
                if (r < 0 || g < 0 || b < 0 || a < 0) return null;
                return new CssColor((byte)(r * 17), (byte)(g * 17), (byte)(b * 17), (byte)(a * 17));
            }
            if (hex.Length == 7)
            {
                int r = HexByte(hex, 1); int g = HexByte(hex, 3); int b = HexByte(hex, 5);
                if (r < 0 || g < 0 || b < 0) return null;
                return new CssColor((byte)r, (byte)g, (byte)b);
            }
            if (hex.Length == 9)
            {
                int r = HexByte(hex, 1); int g = HexByte(hex, 3); int b = HexByte(hex, 5); int a = HexByte(hex, 7);
                if (r < 0 || g < 0 || b < 0 || a < 0) return null;
                return new CssColor((byte)r, (byte)g, (byte)b, (byte)a);
            }
            return null;
        }

        private static CssColor? ParseRgb(string text)
        {
            int open = text.IndexOf('(');
            int close = text.LastIndexOf(')');
            if (open < 0 || close < 0 || close <= open) return null;
            string inner = text.Substring(open + 1, close - open - 1);
            string[] parts = inner.Split(new[] { ',', '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) return null;
            int r = ParseComponent(parts[0].Trim(), 255);
            int g = ParseComponent(parts[1].Trim(), 255);
            int b = ParseComponent(parts[2].Trim(), 255);
            int a = parts.Length >= 4 ? ParseAlpha(parts[3].Trim()) : 255;
            if (r < 0 || g < 0 || b < 0 || a < 0) return null;
            return new CssColor((byte)r, (byte)g, (byte)b, (byte)a);
        }

        private static CssColor? ParseHsl(string text)
        {
            int open = text.IndexOf('(');
            int close = text.LastIndexOf(')');
            if (open < 0 || close < 0 || close <= open) return null;
            string inner = text.Substring(open + 1, close - open - 1);
            string[] parts = inner.Split(new[] { ',', '/', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 3) return null;
            double h, s, l;
            string hStr = parts[0].Trim().TrimEnd(new[] { 'd', 'e', 'g' });
            if (!double.TryParse(hStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out h)) return null;
            if (!TryParsePercent(parts[1].Trim(), out s)) return null;
            if (!TryParsePercent(parts[2].Trim(), out l)) return null;
            int a = parts.Length >= 4 ? ParseAlpha(parts[3].Trim()) : 255;
            if (a < 0) return null;

            h = ((h % 360) + 360) % 360;
            byte r, g2, b2;
            HslToRgb(h / 360.0, s, l, out r, out g2, out b2);
            return new CssColor(r, g2, b2, (byte)a);
        }

        private static bool TryParsePercent(string s, out double value)
        {
            value = 0;
            if (s.EndsWith("%"))
            {
                double v;
                if (!double.TryParse(s.TrimEnd('%'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) return false;
                value = Math.Max(0, Math.Min(1, v / 100.0));
                return true;
            }
            return false;
        }

        private static void HslToRgb(double h, double s, double l, out byte r, out byte g, out byte b)
        {
            double c = (1 - Math.Abs(2 * l - 1)) * s;
            double x = c * (1 - Math.Abs((h * 6) % 2 - 1));
            double m = l - c / 2;
            double r1, g1, b1;
            if (h < 1.0 / 6) { r1 = c; g1 = x; b1 = 0; }
            else if (h < 2.0 / 6) { r1 = x; g1 = c; b1 = 0; }
            else if (h < 3.0 / 6) { r1 = 0; g1 = c; b1 = x; }
            else if (h < 4.0 / 6) { r1 = 0; g1 = x; b1 = c; }
            else if (h < 5.0 / 6) { r1 = x; g1 = 0; b1 = c; }
            else { r1 = c; g1 = 0; b1 = x; }
            r = (byte)Math.Round((r1 + m) * 255);
            g = (byte)Math.Round((g1 + m) * 255);
            b = (byte)Math.Round((b1 + m) * 255);
        }

        private static int ParseComponent(string s, int max)
        {
            if (s.EndsWith("%"))
            {
                double v;
                if (!double.TryParse(s.TrimEnd('%'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) return -1;
                return (int)Math.Round(Math.Max(0, Math.Min(max, v * max / 100.0)));
            }
            int n;
            if (!int.TryParse(s, out n)) return -1;
            return Math.Max(0, Math.Min(max, n));
        }

        private static int ParseAlpha(string s)
        {
            if (s.EndsWith("%"))
            {
                double v;
                if (!double.TryParse(s.TrimEnd('%'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out v)) return -1;
                return (int)Math.Round(Math.Max(0, Math.Min(255, v * 255.0 / 100.0)));
            }
            double a;
            if (!double.TryParse(s, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out a)) return -1;
            if (a >= 0 && a <= 1) return (int)Math.Round(a * 255);
            return -1;
        }

        private static int HexVal(char c)
        {
            if (c >= '0' && c <= '9') return c - '0';
            if (c >= 'a' && c <= 'f') return c - 'a' + 10;
            if (c >= 'A' && c <= 'F') return c - 'A' + 10;
            return -1;
        }

        private static int HexByte(string s, int index)
        {
            int hi = HexVal(s[index]), lo = HexVal(s[index + 1]);
            if (hi < 0 || lo < 0) return -1;
            return hi * 16 + lo;
        }

        private static readonly Dictionary<string, CssColor> NamedColors = new Dictionary<string, CssColor>(StringComparer.OrdinalIgnoreCase)
        {
            { "black", new CssColor(0, 0, 0) },
            { "white", new CssColor(255, 255, 255) },
            { "red", new CssColor(255, 0, 0) },
            { "green", new CssColor(0, 128, 0) },
            { "blue", new CssColor(0, 0, 255) },
            { "yellow", new CssColor(255, 255, 0) },
            { "cyan", new CssColor(0, 255, 255) },
            { "magenta", new CssColor(255, 0, 255) },
            { "orange", new CssColor(255, 165, 0) },
            { "purple", new CssColor(128, 0, 128) },
            { "pink", new CssColor(255, 192, 203) },
            { "gray", new CssColor(128, 128, 128) },
            { "grey", new CssColor(128, 128, 128) },
            { "brown", new CssColor(165, 42, 42) },
            { "navy", new CssColor(0, 0, 128) },
            { "teal", new CssColor(0, 128, 128) },
            { "olive", new CssColor(128, 128, 0) },
            { "maroon", new CssColor(128, 0, 0) },
            { "aqua", new CssColor(0, 255, 255) },
            { "lime", new CssColor(0, 255, 0) },
            { "silver", new CssColor(192, 192, 192) },
            { "fuchsia", new CssColor(255, 0, 255) },
            { "coral", new CssColor(255, 127, 80) },
            { "salmon", new CssColor(250, 128, 114) },
            { "tomato", new CssColor(255, 99, 71) },
            { "gold", new CssColor(255, 215, 0) },
            { "khaki", new CssColor(240, 230, 140) },
            { "violet", new CssColor(238, 130, 238) },
            { "indigo", new CssColor(75, 0, 130) },
            { "crimson", new CssColor(220, 20, 60) },
            { "plum", new CssColor(221, 160, 221) },
            { "sienna", new CssColor(160, 82, 45) },
            { "tan", new CssColor(210, 180, 140) },
            { "turquoise", new CssColor(64, 224, 208) },
            { "skyblue", new CssColor(135, 206, 235) },
            { "steelblue", new CssColor(70, 130, 180) },
            { "slategray", new CssColor(112, 128, 144) },
            { "darkblue", new CssColor(0, 0, 139) },
            { "darkgreen", new CssColor(0, 100, 0) },
            { "darkred", new CssColor(139, 0, 0) },
            { "darkgray", new CssColor(169, 169, 169) },
            { "dimgray", new CssColor(105, 105, 105) },
            { "lightgray", new CssColor(211, 211, 211) },
            { "lightblue", new CssColor(173, 216, 230) },
            { "lightgreen", new CssColor(144, 238, 144) },
            { "lightyellow", new CssColor(255, 255, 224) },
            { "transparent", new CssColor(0, 0, 0, 0) },
        };
    }
}
