using System;
using System.IO;
using System.Linq;
using NestLight.Detection;
using Xunit;

namespace NestLight.Tests
{
    public class DetectionOptionsMutationTests
    {

        [Fact]
        public void A_file_without_enabled_key_leaves_detection_on()
        {
            string path = Path.Combine(Path.GetTempPath(), "nestlight-mut-" + Guid.NewGuid().ToString("N"), "detection.txt");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, "language=sql\nlanguage=css\n");
                var loaded = DetectionOptionsFile.Load(path);
                Assert.True(loaded.Enabled);
            }
            finally { Directory.Delete(Path.GetDirectoryName(path), true); }
        }
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
                var options = new DetectionOptions(); options.Set(false, DetectionOptions.Available);
                DetectionOptionsFile.Save(path, options);
                string content = File.ReadAllText(path);
                Assert.Contains("enabled=false", content);
            }
            finally { Directory.Delete(Path.GetDirectoryName(path), true); }
        }
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

        // --- Kills: line 72 Boolean mutation: recognized = false → recognized = true ---
        [Fact]
        public void A_file_without_enabled_key_keeps_all_languages()
        {
            // If recognized defaults to true (mutation), Set() would be called with the
            // file's languages (empty list), replacing the default full set.
            string path = Path.Combine(Path.GetTempPath(), "nestlight-mut-" + Guid.NewGuid().ToString("N"), "detection.txt");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, "somethingelse=value\n");
                var loaded = DetectionOptionsFile.Load(path);
                // Default languages should be ALL available (not empty/changed)
                Assert.Equal(DetectionOptions.Available.Count, loaded.Languages.Count);
            }
            finally { Directory.Delete(Path.GetDirectoryName(path), true); }
        }

        // --- Kills: line 77 eq <= 0 → eq < 0 ---
        [Fact]
        public void Lines_with_equals_at_position_zero_are_skipped()
        {
            // "=true" has eq==0, with <= 0 it's skipped, with < 0 it would be processed
            // key="" and value="true" - key "" != "enabled" and != "language" so no effect
            // Actually this mutant might be equivalent since key="" matches nothing.
            // But let's make sure the behavior is correct:
            string path = Path.Combine(Path.GetTempPath(), "nestlight-mut-" + Guid.NewGuid().ToString("N"), "detection.txt");
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                File.WriteAllText(path, "enabled=true\n=sql\nlanguage=css\n");
                var loaded = DetectionOptionsFile.Load(path);
                Assert.True(loaded.Enabled);
                // "=sql" should be ignored, only "language=css" counted
                Assert.Contains("css", loaded.Languages);
                Assert.DoesNotContain("sql", loaded.Languages);
            }
            finally { Directory.Delete(Path.GetDirectoryName(path), true); }
        }
    }
}
