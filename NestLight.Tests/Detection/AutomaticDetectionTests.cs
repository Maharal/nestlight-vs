using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Detection;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The scanners and the pipeline with the detection on: which unmarked strings become embedded code.</summary>
    public class AutomaticDetectionTests
    {
        private const string Sql = "SELECT id, name FROM customers WHERE active = 1";

        private static DetectionOptions On(params string[] languages)
        {
            var options = new DetectionOptions();
            options.Set(true, languages.Length == 0 ? DetectionOptions.Available : languages);
            return options;
        }

        private static IReadOnlyList<EmbeddedString> Scan(HostLanguage host, string code, DetectionOptions options)
        {
            return NestLightComposition.CreateScanner(host, Pipeline.Languages, options).Scan(code);
        }

        public static IEnumerable<object[]> UnmarkedSql()
        {
            yield return new object[] { HostLanguage.JavaScript, "const q = `" + Sql + "`;" };
            yield return new object[] { HostLanguage.CSharp, "var q = \"" + Sql + "\";" };
            yield return new object[] { HostLanguage.CSharp, "var q = @\"" + Sql + "\";" };
            yield return new object[] { HostLanguage.CSharp, "var q = $\"" + Sql + "\";" };
            yield return new object[] { HostLanguage.CSharp, "var q = \"\"\"" + Sql + "\"\"\";" };
            yield return new object[] { HostLanguage.Python, "q = \"" + Sql + "\"" };
            yield return new object[] { HostLanguage.Python, "q = f'''" + Sql + "'''" };
            yield return new object[] { HostLanguage.Cpp, "auto q = R\"(" + Sql + ")\";" };
        }

        [Theory]
        [MemberData(nameof(UnmarkedSql))]
        public void An_unmarked_string_is_embedded_code_when_the_detection_is_on(HostLanguage host, string code)
        {
            EmbeddedString s = Scan(host, code, On()).Single();
            Assert.Equal("sql", s.EmbeddedLanguageId);
            Assert.Equal(Sql, code.Substring(s.Start, s.End - s.Start));
        }

        [Theory]
        [MemberData(nameof(UnmarkedSql))]
        public void Nothing_is_guessed_when_the_detection_is_off_or_absent(HostLanguage host, string code)
        {
            Assert.Empty(Scan(host, code, new DetectionOptions()));
            Assert.Empty(Scan(host, code, null));
        }

        [Theory]
        [MemberData(nameof(UnmarkedSql))]
        public void A_language_that_is_not_chosen_is_not_guessed(HostLanguage host, string code)
        {
            Assert.Empty(Scan(host, code, On("html", "css")));
            Assert.Single(Scan(host, code, On("sql")));
        }

        [Fact]
        public void A_mark_wins_over_the_content()
        {
            const string code = "const q = css`" + Sql + "`;";
            EmbeddedString s = Scan(HostLanguage.JavaScript, code, On()).Single();
            Assert.Equal("css", s.EmbeddedLanguageId);

            const string comment = "// language=yaml\nvar q = \"" + Sql + "\";";
            Assert.Equal("yaml", Scan(HostLanguage.CSharp, comment, On()).Single().EmbeddedLanguageId);
        }

        [Fact]
        public void Ordinary_strings_and_ordinary_javascript_strings_stay_plain()
        {
            Assert.Empty(Scan(HostLanguage.JavaScript, "const a = `Hello ${name}, welcome back`; const b = \"" + Sql + "\";", On()));
            Assert.Empty(Scan(HostLanguage.CSharp, "var a = \"Error: file not found\"; var b = \"Delete the file from disk?\";", On()));
            Assert.Empty(Scan(HostLanguage.Python, "a = 'hello world'\nb = \"https://example.com/a\"", On()));
        }

        [Fact]
        public void Json_in_csharp_is_left_to_visual_studio_even_when_it_is_detected()
        {
            const string code = "var j = @\"{\"\"name\"\": \"\"Ada\"\", \"\"age\"\": 36}\";";
            Assert.Empty(Scan(HostLanguage.CSharp, code, On()));
            Assert.Single(Scan(HostLanguage.Python, "j = '{\"name\": \"Ada\", \"age\": 36}'", On()));
        }

        [Fact]
        public void Interpolations_inside_a_detected_string_are_kept()
        {
            const string code = "const q = `SELECT * FROM users WHERE id = ${id}`;";
            EmbeddedString s = Scan(HostLanguage.JavaScript, code, On()).Single();
            Assert.Single(s.Interpolations);
        }

        [Fact]
        public void A_detected_string_is_colored_as_its_language()
        {
            var options = On();
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.CSharp, options);
            string code = "var q = \"" + Sql + "\";";
            var types = highlighter.Highlight(code).Select(t => t.Type).Distinct().ToList();
            Assert.Contains(types, t => t.Contains("sql"));

            Assert.Empty(NestLightComposition.CreateHighlighter(HostLanguage.CSharp, new DetectionOptions()).Highlight(code));
        }

        [Fact]
        public void The_cached_scan_follows_a_change_of_the_options()
        {
            var options = new DetectionOptions();
            BufferAnalysis analysis = NestLightComposition.CreateForBuffer(HostLanguage.CSharp, options);
            string code = "var q = \"" + Sql + "\";";

            Assert.Empty(analysis.Highlighter.Highlight(code));
            options.Set(true, DetectionOptions.Available);
            Assert.NotEmpty(analysis.Highlighter.Highlight(code));   // same text instance: the old scan must not be served
            options.Set(true, new[] { "html" });
            Assert.Empty(analysis.Highlighter.Highlight(code));
            options.Set(false, new[] { "html" });
            Assert.Empty(analysis.Highlighter.Highlight(code));
        }

        [Fact]
        public void An_edit_after_a_change_of_the_options_is_scanned_under_the_new_options()
        {
            var options = new DetectionOptions();
            IHostScanner scanner = new CachingHostScanner(NestLightComposition.CreateScanner(HostLanguage.Python, Pipeline.Languages, options), () => options.Version);
            string lines = string.Concat(Enumerable.Range(0, 200).Select(i => "x" + i + " = 1\n"));
            string a = lines + "q = \"" + Sql + "\"\n" + lines;
            Assert.Empty(scanner.Scan(a));

            options.Set(true, DetectionOptions.Available);
            string b = a.Insert(5, "y");
            Assert.Single(scanner.Scan(b));
        }

        [Fact]
        public void Token_cache_is_recomputed_when_the_version_changes()
        {
            var options = new DetectionOptions();
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.CSharp, options);
            string code = "var q = \"" + Sql + "\";";
            var cache = new SnapshotTokenCache<string>(highlighter, s => s, () => options.Version);
            Assert.Empty(cache.TokensIn(code, 0, code.Length));
            options.Set(true, DetectionOptions.Available);
            Assert.NotEmpty(cache.TokensIn(code, 0, code.Length));
        }

        [Theory]
        [MemberData(nameof(AllHostsRobustnessTests.Samples), MemberType = typeof(AllHostsRobustnessTests))]
        public void Every_prefix_and_suffix_is_analyzed_without_errors_with_the_detection_on(HostLanguage host, string sample)
        {
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(host, On());
            for (int n = 0; n <= sample.Length; n += 3)
            {
                Check(highlighter.Highlight(sample.Substring(0, n)), n);
                Check(highlighter.Highlight(sample.Substring(n)), n);
            }
        }

        private static void Check(IReadOnlyList<Token> tokens, int where)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                Assert.True(tokens[i].Start >= 0 && tokens[i].Length > 0, "bad token at " + where);
                Assert.True(i == 0 || tokens[i - 1].End <= tokens[i].Start, "overlapping tokens at " + where);
            }
        }
    }
}
