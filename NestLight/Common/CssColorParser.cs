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
            string inner = text.Substring(open + 1, close - open - 1).Trim();
            string[] channels;
            int a = 255;
            if (!SplitFunctionArgs(inner, out channels, ref a)) return null;
            if (channels.Length != 3) return null;
            int r = ParseComponent(channels[0], 255);
            int g = ParseComponent(channels[1], 255);
            int b = ParseComponent(channels[2], 255);
            if (r < 0 || g < 0 || b < 0 || a < 0) return null;
            return new CssColor((byte)r, (byte)g, (byte)b, (byte)a);
        }

        private static CssColor? ParseHsl(string text)
        {
            int open = text.IndexOf('(');
            int close = text.LastIndexOf(')');
            if (open < 0 || close < 0 || close <= open) return null;
            string inner = text.Substring(open + 1, close - open - 1).Trim();
            string[] channels;
            int a = 255;
            if (!SplitFunctionArgs(inner, out channels, ref a)) return null;
            if (channels.Length != 3) return null;
            double h, s, l;
            string hStr = channels[0].TrimEnd('d', 'e', 'g');
            if (!double.TryParse(hStr, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out h)) return null;
            if (!TryParsePercent(channels[1], out s)) return null;
            if (!TryParsePercent(channels[2], out l)) return null;
            if (a < 0) return null;

            h = ((h % 360) + 360) % 360;
            byte r, g2, b2;
            HslToRgb(h / 360.0, s, l, out r, out g2, out b2);
            return new CssColor(r, g2, b2, (byte)a);
        }

        /// <summary>
        /// Splits the inside of rgb()/hsl() into channel values and an optional alpha.
        /// Handles both legacy comma syntax and modern space-separated syntax with optional "/ alpha".
        /// </summary>
        private static bool SplitFunctionArgs(string inner, out string[] channels, ref int alpha)
        {
            channels = null;
            int slash = inner.IndexOf('/');
            if (slash >= 0)
            {
                string alphaStr = inner.Substring(slash + 1).Trim();
                inner = inner.Substring(0, slash).Trim();
                alpha = ParseAlpha(alphaStr);
                if (alpha < 0) return false;
            }

            if (inner.IndexOf(',') >= 0)
            {
                string[] parts = inner.Split(',');
                var list = new List<string>(parts.Length);
                foreach (string p in parts)
                {
                    string t = p.Trim();
                    if (t.Length > 0) list.Add(t);
                }
                if (slash < 0 && list.Count >= 4)
                {
                    alpha = ParseAlpha(list[list.Count - 1]);
                    list.RemoveAt(list.Count - 1);
                }
                channels = list.ToArray();
            }
            else
            {
                string[] parts = inner.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                channels = parts;
            }
            return channels.Length >= 3;
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
            { "aliceblue", new CssColor(240, 248, 255) },
            { "antiquewhite", new CssColor(250, 235, 215) },
            { "aqua", new CssColor(0, 255, 255) },
            { "aquamarine", new CssColor(127, 255, 212) },
            { "azure", new CssColor(240, 255, 255) },
            { "beige", new CssColor(245, 245, 220) },
            { "bisque", new CssColor(255, 228, 196) },
            { "black", new CssColor(0, 0, 0) },
            { "blanchedalmond", new CssColor(255, 235, 205) },
            { "blue", new CssColor(0, 0, 255) },
            { "blueviolet", new CssColor(138, 43, 226) },
            { "brown", new CssColor(165, 42, 42) },
            { "burlywood", new CssColor(222, 184, 135) },
            { "cadetblue", new CssColor(95, 158, 160) },
            { "chartreuse", new CssColor(127, 255, 0) },
            { "chocolate", new CssColor(210, 105, 30) },
            { "coral", new CssColor(255, 127, 80) },
            { "cornflowerblue", new CssColor(100, 149, 237) },
            { "cornsilk", new CssColor(255, 248, 220) },
            { "crimson", new CssColor(220, 20, 60) },
            { "cyan", new CssColor(0, 255, 255) },
            { "darkblue", new CssColor(0, 0, 139) },
            { "darkcyan", new CssColor(0, 139, 139) },
            { "darkgoldenrod", new CssColor(184, 134, 11) },
            { "darkgray", new CssColor(169, 169, 169) },
            { "darkgreen", new CssColor(0, 100, 0) },
            { "darkgrey", new CssColor(169, 169, 169) },
            { "darkkhaki", new CssColor(189, 183, 107) },
            { "darkmagenta", new CssColor(139, 0, 139) },
            { "darkolivegreen", new CssColor(85, 107, 47) },
            { "darkorange", new CssColor(255, 140, 0) },
            { "darkorchid", new CssColor(153, 50, 204) },
            { "darkred", new CssColor(139, 0, 0) },
            { "darksalmon", new CssColor(233, 150, 122) },
            { "darkseagreen", new CssColor(143, 188, 143) },
            { "darkslateblue", new CssColor(72, 61, 139) },
            { "darkslategray", new CssColor(47, 79, 79) },
            { "darkslategrey", new CssColor(47, 79, 79) },
            { "darkturquoise", new CssColor(0, 206, 209) },
            { "darkviolet", new CssColor(148, 0, 211) },
            { "deeppink", new CssColor(255, 20, 147) },
            { "deepskyblue", new CssColor(0, 191, 255) },
            { "dimgray", new CssColor(105, 105, 105) },
            { "dimgrey", new CssColor(105, 105, 105) },
            { "dodgerblue", new CssColor(30, 144, 255) },
            { "firebrick", new CssColor(178, 34, 34) },
            { "floralwhite", new CssColor(255, 250, 240) },
            { "forestgreen", new CssColor(34, 139, 34) },
            { "fuchsia", new CssColor(255, 0, 255) },
            { "gainsboro", new CssColor(220, 220, 220) },
            { "ghostwhite", new CssColor(248, 248, 255) },
            { "gold", new CssColor(255, 215, 0) },
            { "goldenrod", new CssColor(218, 165, 32) },
            { "gray", new CssColor(128, 128, 128) },
            { "green", new CssColor(0, 128, 0) },
            { "greenyellow", new CssColor(173, 255, 47) },
            { "grey", new CssColor(128, 128, 128) },
            { "honeydew", new CssColor(240, 255, 240) },
            { "hotpink", new CssColor(255, 105, 180) },
            { "indianred", new CssColor(205, 92, 92) },
            { "indigo", new CssColor(75, 0, 130) },
            { "ivory", new CssColor(255, 255, 240) },
            { "khaki", new CssColor(240, 230, 140) },
            { "lavender", new CssColor(230, 230, 250) },
            { "lavenderblush", new CssColor(255, 240, 245) },
            { "lawngreen", new CssColor(124, 252, 0) },
            { "lemonchiffon", new CssColor(255, 250, 205) },
            { "lightblue", new CssColor(173, 216, 230) },
            { "lightcoral", new CssColor(240, 128, 128) },
            { "lightcyan", new CssColor(224, 255, 255) },
            { "lightgoldenrodyellow", new CssColor(250, 250, 210) },
            { "lightgray", new CssColor(211, 211, 211) },
            { "lightgreen", new CssColor(144, 238, 144) },
            { "lightgrey", new CssColor(211, 211, 211) },
            { "lightpink", new CssColor(255, 182, 193) },
            { "lightsalmon", new CssColor(255, 160, 122) },
            { "lightseagreen", new CssColor(32, 178, 170) },
            { "lightskyblue", new CssColor(135, 206, 250) },
            { "lightslategray", new CssColor(119, 136, 153) },
            { "lightslategrey", new CssColor(119, 136, 153) },
            { "lightsteelblue", new CssColor(176, 196, 222) },
            { "lightyellow", new CssColor(255, 255, 224) },
            { "lime", new CssColor(0, 255, 0) },
            { "limegreen", new CssColor(50, 205, 50) },
            { "linen", new CssColor(250, 240, 230) },
            { "magenta", new CssColor(255, 0, 255) },
            { "maroon", new CssColor(128, 0, 0) },
            { "mediumaquamarine", new CssColor(102, 205, 170) },
            { "mediumblue", new CssColor(0, 0, 205) },
            { "mediumorchid", new CssColor(186, 85, 211) },
            { "mediumpurple", new CssColor(147, 112, 219) },
            { "mediumseagreen", new CssColor(60, 179, 113) },
            { "mediumslateblue", new CssColor(123, 104, 238) },
            { "mediumspringgreen", new CssColor(0, 250, 154) },
            { "mediumturquoise", new CssColor(72, 209, 204) },
            { "mediumvioletred", new CssColor(199, 21, 133) },
            { "midnightblue", new CssColor(25, 25, 112) },
            { "mintcream", new CssColor(245, 255, 250) },
            { "mistyrose", new CssColor(255, 228, 225) },
            { "moccasin", new CssColor(255, 228, 181) },
            { "navajowhite", new CssColor(255, 222, 173) },
            { "navy", new CssColor(0, 0, 128) },
            { "oldlace", new CssColor(253, 245, 230) },
            { "olive", new CssColor(128, 128, 0) },
            { "olivedrab", new CssColor(107, 142, 35) },
            { "orange", new CssColor(255, 165, 0) },
            { "orangered", new CssColor(255, 69, 0) },
            { "orchid", new CssColor(218, 112, 214) },
            { "palegoldenrod", new CssColor(238, 232, 170) },
            { "palegreen", new CssColor(152, 251, 152) },
            { "paleturquoise", new CssColor(175, 238, 238) },
            { "palevioletred", new CssColor(219, 112, 147) },
            { "papayawhip", new CssColor(255, 239, 213) },
            { "peachpuff", new CssColor(255, 218, 185) },
            { "peru", new CssColor(205, 133, 63) },
            { "pink", new CssColor(255, 192, 203) },
            { "plum", new CssColor(221, 160, 221) },
            { "powderblue", new CssColor(176, 224, 230) },
            { "purple", new CssColor(128, 0, 128) },
            { "rebeccapurple", new CssColor(102, 51, 153) },
            { "red", new CssColor(255, 0, 0) },
            { "rosybrown", new CssColor(188, 143, 143) },
            { "royalblue", new CssColor(65, 105, 225) },
            { "saddlebrown", new CssColor(139, 69, 19) },
            { "salmon", new CssColor(250, 128, 114) },
            { "sandybrown", new CssColor(244, 164, 96) },
            { "seagreen", new CssColor(46, 139, 87) },
            { "seashell", new CssColor(255, 245, 238) },
            { "sienna", new CssColor(160, 82, 45) },
            { "silver", new CssColor(192, 192, 192) },
            { "skyblue", new CssColor(135, 206, 235) },
            { "slateblue", new CssColor(106, 90, 205) },
            { "slategray", new CssColor(112, 128, 144) },
            { "slategrey", new CssColor(112, 128, 144) },
            { "snow", new CssColor(255, 250, 250) },
            { "springgreen", new CssColor(0, 255, 127) },
            { "steelblue", new CssColor(70, 130, 180) },
            { "tan", new CssColor(210, 180, 140) },
            { "teal", new CssColor(0, 128, 128) },
            { "thistle", new CssColor(216, 191, 216) },
            { "tomato", new CssColor(255, 99, 71) },
            { "transparent", new CssColor(0, 0, 0, 0) },
            { "turquoise", new CssColor(64, 224, 208) },
            { "violet", new CssColor(238, 130, 238) },
            { "wheat", new CssColor(245, 222, 179) },
            { "white", new CssColor(255, 255, 255) },
            { "whitesmoke", new CssColor(245, 245, 245) },
            { "yellow", new CssColor(255, 255, 0) },
            { "yellowgreen", new CssColor(154, 205, 50) },
        };
    }
}
