using System;
using System.IO;
using System.Linq;
using NestLight.Detection;
using Xunit;

namespace NestLight.Tests
{
    public class DetectionOptionsTests
    {
        private static string TempFile()
        {
            return Path.Combine(Path.GetTempPath(), "nestlight-test-" + Guid.NewGuid().ToString("N"), "detection.txt");
        }

        [Fact]
        public void It_is_off_by_default_with_every_language_chosen()
        {
            var options = new DetectionOptions();
            Assert.False(options.Enabled);
            Assert.Equal(DetectionOptions.Available.OrderBy(x => x), options.Languages.OrderBy(x => x));
            Assert.False(options.IsOn("sql"));
        }

        [Fact]
        public void A_language_is_on_only_when_the_detection_is_on_and_it_is_chosen()
        {
            var options = new DetectionOptions();
            options.Set(true, new[] { "sql", "css" });
            Assert.True(options.IsOn("sql"));
            Assert.True(options.IsOn("css"));
            Assert.False(options.IsOn("html"));
            Assert.False(options.IsOn(null));
            options.Set(false, new[] { "sql", "css" });
            Assert.False(options.IsOn("sql"));
        }

        [Fact]
        public void Unknown_ids_are_ignored_and_ids_are_case_insensitive()
        {
            var options = new DetectionOptions();
            options.Set(true, new[] { "SQL", "klingon", null });
            Assert.Equal(new[] { "sql" }, options.Languages.ToArray());
        }

        [Fact]
        public void A_change_bumps_the_version_and_raises_the_event_but_the_same_values_do_not()
        {
            var options = new DetectionOptions();
            int raised = 0;
            options.Changed += (s, e) => raised++;
            int v0 = options.Version;
            options.Set(true, DetectionOptions.Available);
            Assert.Equal(v0 + 1, options.Version);
            Assert.Equal(1, raised);
            options.Set(true, DetectionOptions.Available.Reverse());
            Assert.Equal(v0 + 1, options.Version);
            Assert.Equal(1, raised);
        }

        [Fact]
        public void The_file_keeps_the_options()
        {
            string path = TempFile();
            try
            {
                var options = new DetectionOptions();
                options.Set(true, new[] { "json", "graphql" });
                Assert.True(DetectionOptionsFile.Save(path, options));

                DetectionOptions loaded = DetectionOptionsFile.Load(path);
                Assert.True(loaded.Enabled);
                Assert.Equal(new[] { "graphql", "json" }, loaded.Languages.OrderBy(x => x).ToArray());
            }
            finally { Directory.Delete(Path.GetDirectoryName(path), true); }
        }

        [Fact]
        public void A_file_with_no_language_chooses_none()
        {
            string path = TempFile();
            try
            {
                var options = new DetectionOptions();
                options.Set(true, new string[0]);
                DetectionOptionsFile.Save(path, options);
                DetectionOptions loaded = DetectionOptionsFile.Load(path);
                Assert.True(loaded.Enabled);
                Assert.Empty(loaded.Languages);
            }
            finally { Directory.Delete(Path.GetDirectoryName(path), true); }
        }

        [Fact]
        public void A_missing_or_damaged_file_gives_the_defaults()
        {
            string path = TempFile();
            Assert.False(DetectionOptionsFile.Load(path).Enabled);

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            try
            {
                File.WriteAllBytes(path, new byte[] { 0, 255, 254, 13, 10, 61, 61, 0 });
                DetectionOptions loaded = DetectionOptionsFile.Load(path);
                Assert.False(loaded.Enabled);
                Assert.Equal(DetectionOptions.Available.Count, loaded.Languages.Count);
            }
            finally { Directory.Delete(Path.GetDirectoryName(path), true); }
        }

        [Fact]
        public void A_file_that_cannot_be_written_is_reported_not_thrown()
        {
            string path = Path.Combine(Path.GetTempPath(), "nestlight-test-" + Guid.NewGuid().ToString("N"), "x");
            Directory.CreateDirectory(path); // a folder where the file should be
            try { Assert.False(DetectionOptionsFile.Save(path, new DetectionOptions())); }
            finally { Directory.Delete(path, true); Directory.Delete(Path.GetDirectoryName(path)); }
        }
    }
}
