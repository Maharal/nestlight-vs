using System;
using System.IO;
using System.Linq;
using NestLight.Detection;
using Xunit;

namespace NestLight.Tests
{
    public class DetectionOptionsMutationTests
    {
        // L72: boolean mutation (recognized = false -> true)

        [Fact]
        public void A_file_without_enabled_key_does_not_enable_detection()
        {
            string path = Path.Combine(Path.GetTempPath(), "nestlight-mut-" + Guid.NewGuid().ToString("N"), "detection.txt");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, "language=sql\nlanguage=css\n");
                var loaded = DetectionOptionsFile.Load(path);
                Assert.False(loaded.Enabled);
            }
            finally { Directory.Delete(Path.GetDirectoryName(path), true); }
        }

        // L77: equality mutation (eq <= 0 vs eq < 0)

        [Fact]
        public void Lines_with_equals_at_position_zero_are_ignored()
        {
            string path = Path.Combine(Path.GetTempPath(), "nestlight-mut-" + Guid.NewGuid().ToString("N"), "detection.txt");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, "enabled=true\n=invalid\nlanguage=sql\n");
                var loaded = DetectionOptionsFile.Load(path);
                Assert.True(loaded.Enabled);
                Assert.Contains("sql", loaded.Languages);
            }
            finally { Directory.Delete(Path.GetDirectoryName(path), true); }
        }

        // L97: conditional mutation on enabled ternary, string mutation on "true"/"false"

        [Fact]
        public void Save_writes_enabled_true()
        {
            string path = Path.Combine(Path.GetTempPath(), "nestlight-mut-" + Guid.NewGuid().ToString("N"), "detection.txt");
            try
            {
                var options = new DetectionOptions();
                options.Set(true, new[] { "sql" });
                DetectionOptionsFile.Save(path, options);
                string content = File.ReadAllText(path);
                Assert.Contains("enabled=true", content);
            }
            finally { Directory.Delete(Path.GetDirectoryName(path), true); }
        }

        [Fact]
        public void Save_writes_enabled_false()
        {
            string path = Path.Combine(Path.GetTempPath(), "nestlight-mut-" + Guid.NewGuid().ToString("N"), "detection.txt");
            try
            {
                var options = new DetectionOptions();
                DetectionOptionsFile.Save(path, options);
                string content = File.ReadAllText(path);
                Assert.Contains("enabled=false", content);
            }
            finally { Directory.Delete(Path.GetDirectoryName(path), true); }
        }

        // L98: OrderBy to OrderByDescending mutation

        [Fact]
        public void Languages_are_saved_in_alphabetical_order()
        {
            string path = Path.Combine(Path.GetTempPath(), "nestlight-mut-" + Guid.NewGuid().ToString("N"), "detection.txt");
            try
            {
                var options = new DetectionOptions();
                options.Set(true, new[] { "sql", "css", "html" });
                DetectionOptionsFile.Save(path, options);
                var lines = File.ReadAllLines(path).Where(l => l.StartsWith("language=")).Select(l => l.Substring("language=".Length)).ToArray();
                Assert.Equal(new[] { "css", "html", "sql" }, lines);
            }
            finally { Directory.Delete(Path.GetDirectoryName(path), true); }
        }
    }
}
