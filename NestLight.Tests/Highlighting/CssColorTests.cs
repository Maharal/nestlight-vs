using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>
    /// The default colors of the CSS (the formats of CssClassificationDefinitions.cs), read from the source: one palette serves the dark and the
    /// light themes of Visual Studio, so each color has to be legible on both backgrounds, and two kinds of token that share a color have to differ in style.
    /// </summary>
    public class CssColorTests
    {
        private sealed class Format
        {
            public string Name;
            public int R, G, B;
            public bool Bold, Italic;
            public double Luminance;
        }

        private static List<Format> Formats()
        {
            string dir = AppContext.BaseDirectory;
            while (dir != null && !File.Exists(Path.Combine(dir, "NestLight", "VisualStudio", "CssClassificationDefinitions.cs"))) dir = Path.GetDirectoryName(dir);
            Assert.NotNull(dir);
            string source = File.ReadAllText(Path.Combine(dir, "NestLight", "VisualStudio", "CssClassificationDefinitions.cs"));
            var formats = new List<Format>();
            foreach (Match m in Regex.Matches(source, "base\\(\"([^\"]+)\",\\s*0x([0-9A-Fa-f]{2}),\\s*0x([0-9A-Fa-f]{2}),\\s*0x([0-9A-Fa-f]{2})([^)]*)\\)"))
            {
                var f = new Format
                {
                    Name = m.Groups[1].Value,
                    R = Convert.ToInt32(m.Groups[2].Value, 16), G = Convert.ToInt32(m.Groups[3].Value, 16), B = Convert.ToInt32(m.Groups[4].Value, 16),
                    Bold = m.Groups[5].Value.Contains("bold: true"), Italic = m.Groups[5].Value.Contains("italic: true"),
                };
                f.Luminance = 0.2126 * Channel(f.R) + 0.7152 * Channel(f.G) + 0.0722 * Channel(f.B);
                formats.Add(f);
            }
            return formats;
        }

        private static double Channel(int value)
        {
            double c = value / 255.0;
            return c <= 0.03928 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
        }

        private static double Contrast(double a, double b)
        {
            return (Math.Max(a, b) + 0.05) / (Math.Min(a, b) + 0.05);
        }

        private static readonly double White = 1.0, DarkEditor = 0.0129; // #FFFFFF and #1E1E1E

        [Fact]
        public void Every_css_classification_has_a_format()
        {
            Assert.Equal(16, Formats().Count);
        }

        [Fact]
        public void Each_color_is_legible_on_the_light_and_on_the_dark_editor()
        {
            foreach (Format f in Formats())
            {
                Assert.True(Contrast(f.Luminance, White) >= 3.5, f.Name + " on white: " + Contrast(f.Luminance, White).ToString("F1"));
                Assert.True(Contrast(f.Luminance, DarkEditor) >= 3.5, f.Name + " on #1E1E1E: " + Contrast(f.Luminance, DarkEditor).ToString("F1"));
            }
        }

        [Fact]
        public void Two_kinds_of_token_with_the_same_color_differ_in_style()
        {
            List<Format> formats = Formats();
            foreach (var group in formats.GroupBy(f => f.R * 65536 + f.G * 256 + f.B).Where(g => g.Count() > 1))
            {
                var styles = group.Select(f => (f.Bold ? "b" : "") + (f.Italic ? "i" : "")).ToList();
                Assert.True(styles.Distinct().Count() == styles.Count, "the same color and style: " + string.Join(", ", group.Select(f => f.Name)));
            }
        }

        [Fact]
        public void The_colors_that_are_not_the_same_are_far_enough_apart_to_tell()
        {
            List<Format> formats = Formats();
            for (int i = 0; i < formats.Count; i++)
                for (int j = i + 1; j < formats.Count; j++)
                {
                    Format a = formats[i], b = formats[j];
                    if (a.R == b.R && a.G == b.G && a.B == b.B) continue;
                    double distance = Math.Sqrt(Math.Pow(a.R - b.R, 2) + Math.Pow(a.G - b.G, 2) + Math.Pow(a.B - b.B, 2));
                    Assert.True(distance >= 40, a.Name + " and " + b.Name + " are only " + distance.ToString("F0") + " apart");
                }
        }
    }
}
