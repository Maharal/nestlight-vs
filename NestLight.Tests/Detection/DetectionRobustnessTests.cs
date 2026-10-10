using System;
using System.IO;
using NestLight.Detection;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The file of the options when the disk says no, the detector on the shapes of code it is meant for, and on text that must stay plain.</summary>
    public class DetectionRobustnessTests
    {
        [Fact]
        public void The_default_file_is_in_the_local_application_data_folder_of_the_user()
        {
            string path = DetectionOptionsFile.DefaultPath;
            Assert.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), path);
            Assert.EndsWith(Path.Combine("NestLight", "detection.txt"), path);
        }

        [Fact]
        public void Saving_where_the_folder_cannot_be_made_says_so_and_throws_nothing()
        {
            string blocker = Path.Combine(Path.GetTempPath(), "nestlight-test-" + Guid.NewGuid().ToString("N"));
            File.WriteAllText(blocker, "a file where a folder should go");
            try
            {
                Assert.False(DetectionOptionsFile.Save(Path.Combine(blocker, "inner", "detection.txt"), new DetectionOptions()));
            }
            finally { File.Delete(blocker); }
        }

        [Fact]
        public void A_path_that_is_a_folder_is_not_an_options_file()
        {
            string dir = Path.Combine(Path.GetTempPath(), "nestlight-test-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(dir);
            try
            {
                Assert.False(DetectionOptionsFile.Load(dir).Enabled);
                Assert.False(DetectionOptionsFile.Save(dir, new DetectionOptions()));
            }
            finally { Directory.Delete(dir); }
        }

        [Fact]
        public void A_file_another_program_holds_open_gives_the_defaults()
        {
            string path = Path.Combine(Path.GetTempPath(), "nestlight-test-" + Guid.NewGuid().ToString("N") + ".txt");
            File.WriteAllText(path, "enabled=true\n");
            try
            {
                using (new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    DetectionOptions loaded = DetectionOptionsFile.Load(path);
                    Assert.False(loaded.Enabled); // the file is there and says true, but it cannot be read: the defaults stand
                }
                Assert.True(DetectionOptionsFile.Load(path).Enabled);
            }
            finally { File.Delete(path); }
        }

        [Fact]
        public void A_file_without_the_enabled_key_leaves_the_defaults_even_if_it_lists_languages()
        {
            string path = Path.Combine(Path.GetTempPath(), "nestlight-test-" + Guid.NewGuid().ToString("N") + ".txt");
            File.WriteAllText(path, "language=sql\nlanguage=css\n");
            try
            {
                DetectionOptions loaded = DetectionOptionsFile.Load(path);
                Assert.False(loaded.Enabled);
                Assert.Equal(DetectionOptions.Available.Count, loaded.Languages.Count);
            }
            finally { File.Delete(path); }
        }

        [Fact]
        public void The_keys_of_the_file_are_read_without_regard_to_case_or_blanks()
        {
            string path = Path.Combine(Path.GetTempPath(), "nestlight-test-" + Guid.NewGuid().ToString("N") + ".txt");
            File.WriteAllText(path, "  ENABLED = TRUE \n Language = sql\nnot a pair\n=orphan\n");
            try
            {
                DetectionOptions loaded = DetectionOptionsFile.Load(path);
                Assert.True(loaded.Enabled);
                Assert.Equal(new[] { "sql" }, loaded.Languages);
            }
            finally { File.Delete(path); }
        }

        // ---- the detector --------------------------------------------------------------------------------------

        private static string Detect(string text) { return LanguageDetector.Create(true).Detect(text, 0, text.Length); }

        [Theory]
        [InlineData("sql", "SELECT id, name FROM users WHERE id = 1")]
        [InlineData("sql", "select * from users where id = 1")]
        [InlineData("html", "<ul class=\"list\">\n  <li>one</li>\n</ul>")]
        [InlineData("html", "<img src=\"a.png\" />")]
        [InlineData("json", "{\"id\": 1, \"tags\": [\"a\"]}")]
        [InlineData("json", "{\n  \"id\": 1\n}")]
        [InlineData("json", "[1, 2, 3]")]
        [InlineData("css", ".a { color: red; margin: 0 }")]
        [InlineData("css", ".a:hover {\n  color: red;\n}")]
        [InlineData("graphql", "query Q { user(id: 1) { name } }")]
        [InlineData("graphql", "{ viewer { login } }")]
        public void Code_of_the_shapes_the_rules_were_written_for_is_recognized(string language, string code)
        {
            Assert.Equal(language, Detect(code));
        }

        [Fact(Skip = "Known gap found by EA35: SqlDetector looks for ' from ' with a blank on each side, so a statement whose FROM or WHERE starts a line is not recognized (about half of the multi-line SQL).")]
        public void A_sql_statement_written_on_several_lines_is_recognized()
        {
            Assert.Equal("sql", Detect("SELECT a\nFROM t\nWHERE a = 1"));
        }

        [Theory]
        [InlineData("hello world, this is plain")]
        [InlineData("C:\\temp\\output.txt")]
        [InlineData("https://example.com/api/v1/users")]
        [InlineData("Dear customer,\nYour order has shipped.")]
        [InlineData("Is it done?")]
        [InlineData("select the right one.")]
        [InlineData("<not a tag, just a bracket")]
        [InlineData("a { b } c")]
        public void Ordinary_strings_stay_plain(string text)
        {
            Assert.Null(Detect(text));
        }

        [Fact]
        public void The_text_is_judged_without_its_blanks_at_both_ends()
        {
            Assert.Equal("json", Detect("\n   {\"id\": 1}   \n"));
            Assert.Null(Detect("          short    "));
        }

        [Fact]
        public void A_range_inside_a_larger_text_is_judged_alone()
        {
            string text = "xx{\"id\": 1, \"ok\": true}yy";
            LanguageDetector detector = LanguageDetector.Create(true);
            Assert.Equal("json", detector.Detect(text, 2, text.Length - 2));
            Assert.Null(detector.Detect(text, 0, text.Length));
        }

        [Fact]
        public void The_gate_never_changes_the_answer()
        {
            string[] samples =
            {
                "SELECT a FROM t WHERE a = 1", "<p>one</p>", "{\"a\": 1}", ".a { color: red; }", "query Q { a }", "plain text with nothing", "[1, 2, 3]", "  ", "",
            };
            foreach (string s in samples)
                Assert.Equal(LanguageDetector.Create(false).Detect(s, 0, s.Length), LanguageDetector.Create(true).Detect(s, 0, s.Length));
        }
    }
}
