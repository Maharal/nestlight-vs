using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using NestLight.Hosts;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>
    /// The scan of an edited text made from the scan of the text before it must be the scan of the edited text: the same strings, with the same
    /// interpolations and escapes, and the same safe points to resume from next time. Whatever the edit, in whatever host.
    /// </summary>
    public class IncrementalHostScanTests
    {
        /// <summary>The scanner of the host, keeping a safe point every <paramref name="gap"/> characters (0: at every line break it reaches).</summary>
        private static IResumableHostScanner Scanner(HostLanguage host, int gap = 0)
        {
            IEmbeddedLanguageRegistry registry = NestLightComposition.CreateEmbeddedLanguages();
            switch (host)
            {
                case HostLanguage.JavaScript: return new JavaScriptHostScanner(new AcceptedEmbeddedLanguages(registry), gap);
                case HostLanguage.CSharp: return new CSharpHostScanner(new AcceptedEmbeddedLanguages(registry, new[] { "json", "regex", "regexp" }), gap);
                case HostLanguage.Python: return new PythonHostScanner(new AcceptedEmbeddedLanguages(registry), gap);
                default: return new CppHostScanner(new AcceptedEmbeddedLanguages(registry), gap);
            }
        }

        /// <summary>Everything a scan found, as text: two scans are the same when their dumps are.</summary>
        private static string Dump(HostScan scan, bool safePoints = true)
        {
            var lines = scan.Strings.Select(s =>
                s.EmbeddedLanguageId + "|" + s.OuterStart + "," + s.Start + "," + s.End + "," + s.OuterEnd +
                "|I:" + string.Join(";", s.Interpolations.Select(x => x.Start + "," + x.End + "," + x.OpenLength + "," + x.CloseLength)) +
                "|E:" + string.Join(";", s.Escapes.Select(e => e.Start + "," + e.Length + "," + (int)e.Value)));
            return string.Join("\n", lines) + (safePoints ? "\nSAFE:" + string.Join(",", scan.SafePoints) : "");
        }

        /// <summary>
        /// A text with nothing the host could mark (no backtick in JavaScript, no <c>R"</c> in C++) is not read at all by a whole scan, which has no safe
        /// points to give; a scan that resumes reads it and has some, which are as good. Everything else must be the same, safe points too.
        /// </summary>
        private static void AssertSameScan(HostLanguage host, string before, string after, HostScan expected, HostScan updated)
        {
            bool unread = host == HostLanguage.JavaScript ? after.IndexOf('`') < 0 : host == HostLanguage.Cpp && after.IndexOf("R\"", StringComparison.Ordinal) < 0;
            string e = Dump(expected, !unread), u = Dump(updated, !unread);
            if (e != u)
                Assert.True(false, host + "\nbefore: " + Show(before) + "\nafter:  " + Show(after) + "\nexpected:\n" + e + "\nupdated:\n" + u);
            for (int k = 0; k < updated.SafePoints.Length; k++)
            {
                Assert.Equal('\n', after[updated.SafePoints[k]]);
                if (k > 0) Assert.True(updated.SafePoints[k - 1] < updated.SafePoints[k]);
            }
        }

        // ---- the pieces of code each host is made of ---------------------------------------------------------------------------

        private static readonly string[] Common =
        {
            "\n", "\n", "\n", " ", "  ", "x", "name", "9", ";", ",", "=", "{", "}", "(", ")", "[", "]", "\\", "\"", "'", ":", ".", "<b>", "</b>", "-", "_"
        };

        private static readonly Dictionary<HostLanguage, string[]> Pieces = new Dictionary<HostLanguage, string[]>
        {
            { HostLanguage.JavaScript, new[] {
                "`", "`", "sql`", "html`", "css`", "${", "}", "${x}", "$", "/*", "*/", "//", "// language=sql\n", "/* html */ ", "/* css */", "const a = ", "function f() {", "`select 1`", "html`<p>${a}</p>`",
                "'it`s'", "\"say `hi`\"", "\\`", "tag`", "json`{}`", "// not a marker\n" } },
            { HostLanguage.CSharp, new[] {
                "\"", "\"", "$\"", "@\"", "$@\"", "\"\"\"", "$\"\"\"", "$$\"\"\"", "{{", "}}", "{x}", "{{x}}", "/*", "*/", "//", "// language=sql\n", "/* html */ ", "var q = ", "@\"a\"\"b\"", "$\"{a}:{b}\"",
                "'\"'", "'\\''", "// language=css\n", "\"\"\"\nselect 1\n\"\"\"" } },
            { HostLanguage.Python, new[] {
                "\"", "\"\"\"", "'''", "'", "f\"", "rf'", "f\"\"\"", "{x}", "{{", "}}", "{x!r:>{w}}", "#", "# language=sql\n", "# language=html\n", "q = ", "\\\n", "b\"x\"", "f'{a['k']}'", "# not a marker\n", "print(" } },
            { HostLanguage.Cpp, new[] {
                "R\"(", ")\"", "R\"x(", ")x\"", "u8R\"(", "LR\"(", "\"", "'", "/*", "*/", "//", "// language=sql\n", "/* html */ ", "auto q = ", "1'000", "\"R\\\"(\"", "// language=css\n", "R\"(select 1)\";" } },
        };

        private static string Random(Random random, string[] pieces, int count)
        {
            var parts = new List<string>();
            for (int i = 0; i < count; i++) parts.Add(random.Next(3) == 0 ? Common[random.Next(Common.Length)] : pieces[random.Next(pieces.Length)]);
            return string.Concat(parts);
        }

        private static string Edit(Random random, string[] pieces, string text)
        {
            int at = random.Next(0, text.Length + 1);
            switch (random.Next(5))
            {
                case 0: return text.Insert(at, Random(random, pieces, random.Next(1, 3)));
                case 1: return text.Remove(at, Math.Min(random.Next(1, 8), text.Length - at));
                case 2:
                {
                    int cut = Math.Min(random.Next(1, 10), text.Length - at);
                    return text.Remove(at, cut).Insert(at, Random(random, pieces, random.Next(0, 3)));
                }
                case 3: return text.Insert(at, Common[random.Next(Common.Length)]);
                default: return text.Remove(at, Math.Min(1, text.Length - at)); // a character deleted
            }
        }

        /// <summary>Every host with a safe point at every line break, one every 30 characters, and one every 512, which is what the plugin keeps.</summary>
        public static IEnumerable<object[]> Hosts()
        {
            foreach (HostLanguage host in Enum.GetValues(typeof(HostLanguage)))
                foreach (int gap in new[] { 0, 30, SafePoints.DefaultGap })
                    yield return new object[] { host, gap };
        }

        // ---- the same scan ------------------------------------------------------------------------------------------------

        [Theory]
        [MemberData("Hosts")]
        public void An_update_after_any_edit_is_the_scan_of_the_edited_text(HostLanguage host, int gap)
        {
            IResumableHostScanner scanner = Scanner(host, gap);
            string[] pieces = Pieces[host];
            var random = new Random(1000 + (int)host);
            for (int round = 0; round < 700; round++)
            {
                string text = Random(random, pieces, random.Next(0, 60));
                HostScan scan = scanner.ScanAll(text);
                // a chain: each scan is made from the one before, so a mistake would pile up
                for (int step = 0; step < 8; step++)
                {
                    string edited = Edit(random, pieces, text);
                    HostScan updated = IncrementalHostScan.Update(scanner, scan, text, edited);
                    AssertSameScan(host, text, edited, scanner.ScanAll(edited), updated);
                    text = edited;
                    scan = updated;
                }
            }
        }

        private static string Show(string text) { return text.Replace("\n", "\\n"); }

        [Theory]
        [MemberData("Hosts")]
        public void The_same_holds_in_a_file_of_many_lines_with_the_edits_in_the_middle(HostLanguage host, int gap)
        {
            IResumableHostScanner scanner = Scanner(host, gap);
            string[] pieces = Pieces[host];
            var random = new Random(77 + (int)host);
            for (int round = 0; round < 25; round++)
            {
                var lines = new List<string>();
                for (int i = 0; i < 300; i++) lines.Add(Random(random, pieces, random.Next(1, 6)) + "\n");
                string text = string.Concat(lines);
                HostScan scan = scanner.ScanAll(text);
                for (int step = 0; step < 20; step++)
                {
                    string edited = Edit(random, pieces, text);
                    scan = IncrementalHostScan.Update(scanner, scan, text, edited);
                    AssertSameScan(host, text, edited, scanner.ScanAll(edited), scan);
                    text = edited;
                }
            }
        }

        [Theory]
        [MemberData("Hosts")]
        public void Typing_a_marked_string_letter_by_letter_gives_the_scan_of_each_text(HostLanguage host, int gap)
        {
            string open, close;
            switch (host)
            {
                case HostLanguage.JavaScript: open = "const first = 1;\nconst q = sql`"; close = "`;\nconst last = 2;\n"; break;
                case HostLanguage.CSharp: open = "int first = 1;\n// language=sql\nvar q = \""; close = "\";\nint last = 2;\n"; break;
                case HostLanguage.Python: open = "first = 1\n# language=sql\nq = \""; close = "\"\nlast = 2\n"; break;
                default: open = "int first = 1;\n// language=sql\nauto q = R\"("; close = ")\";\nint last = 2;\n"; break;
            }
            string padding = string.Concat(Enumerable.Repeat("int pad = 1;\n", 200));
            IResumableHostScanner scanner = Scanner(host, gap);
            string word = "select name from users where id = 1";
            string text = padding + open + close + padding;
            HostScan scan = scanner.ScanAll(text);
            for (int i = 1; i <= word.Length; i++)
            {
                string edited = padding + open + word.Substring(0, i) + close + padding;
                scan = IncrementalHostScan.Update(scanner, scan, text, edited);
                AssertSameScan(host, text, edited, scanner.ScanAll(edited), scan);
                Assert.Equal(1, scan.Strings.Count);
                text = edited;
            }
            while (word.Length > 0)
            {
                word = word.Substring(0, word.Length - 1);
                string edited = padding + open + word + close + padding;
                scan = IncrementalHostScan.Update(scanner, scan, text, edited);
                AssertSameScan(host, text, edited, scanner.ScanAll(edited), scan);
                text = edited;
            }
        }

        // ---- the edits that change what the rest of the text means ------------------------------------------------------------------

        [Theory]
        [InlineData("const a = sql`select 1`;\nconst b = html`<p>x</p>`;\n", "const a = sql`select 1;\nconst b = html`<p>x</p>`;\n")]       // the closing backtick goes: the strings swap sides
        [InlineData("const a = 1;\nconst b = html`<p>x</p>`;\n", "const a = `1;\nconst b = html`<p>x</p>`;\n")]                                // an opening backtick swallows the rest
        [InlineData("// language=sql\nconst q = `select 1`;\n", "// language=sqx\nconst q = `select 1`;\n")]                                  // the marker stops being one
        [InlineData("const q = `select 1`;\n", "// language=sql\nconst q = `select 1`;\n")]                                                       // a marker appears
        [InlineData("/* c */ const q = html`<p>x</p>`;\n", "/* c const q = html`<p>x</p>`;\n")]                                                  // a comment is left open
        [InlineData("const q = html`<p>${a}</p>`;\nconst r = css`a{}`;\n", "const q = html`<p>${a</p>`;\nconst r = css`a{}`;\n")]          // an interpolation is left open
        [InlineData("", "const q = html`<p>x</p>`;\n")]
        [InlineData("const q = html`<p>x</p>`;\n", "")]
        [InlineData("const q = html`<p>x</p>`;\n", "const q = html`<p>x</p>`;\n")]
        [InlineData("a\nb\nc\n", "a\nb\nc\nhtml`<p></p>`")]
        public void An_edit_that_changes_the_meaning_of_what_follows_is_followed_in_javascript(string before, string after)
        {
            IResumableHostScanner scanner = Scanner(HostLanguage.JavaScript);
            AssertSameScan(HostLanguage.JavaScript, before, after, scanner.ScanAll(after), IncrementalHostScan.Update(scanner, scanner.ScanAll(before), before, after));
        }

        [Fact]
        public void The_strings_before_the_edit_are_kept_and_the_ones_after_it_are_moved()
        {
            IResumableHostScanner scanner = Scanner(HostLanguage.JavaScript);
            string padding = string.Concat(Enumerable.Repeat("const pad = 1;\n", 100));
            string first = "const a = html`<p>${x}</p>`;\n", middle = "const q = sql`select`;\n", last = "const z = css`a{}`;\n";
            string before = padding + first + padding + middle + padding + last;
            string after = padding + first + padding + middle.Replace("select", "select name") + padding + last;
            HostScan old = scanner.ScanAll(before);
            HostScan updated = IncrementalHostScan.Update(scanner, old, before, after);

            Assert.Same(old.Strings[0], updated.Strings[0]);              // before the edit: the very same object, not scanned again
            Assert.NotSame(old.Strings[2], updated.Strings[2]);           // after it: moved by the 5 characters that were added
            Assert.Equal(old.Strings[2].OuterStart + 5, updated.Strings[2].OuterStart);
            AssertSameScan(HostLanguage.JavaScript, before, after, scanner.ScanAll(after), updated);
        }

        [Fact]
        public void An_edit_that_changes_nothing_after_it_does_not_move_the_strings()
        {
            IResumableHostScanner scanner = Scanner(HostLanguage.JavaScript);
            const string before = "const a = 1;\nconst b = html`<p>x</p>`;\nconst c = 1;\n";
            string after = before.Replace("const a = 1;", "const a = 2;");
            HostScan old = scanner.ScanAll(before);
            HostScan updated = IncrementalHostScan.Update(scanner, old, before, after);
            Assert.Same(old.Strings[0], updated.Strings[0]);
        }

        [Fact]
        public void The_same_text_in_another_instance_is_the_same_scan()
        {
            IResumableHostScanner scanner = Scanner(HostLanguage.JavaScript);
            string before = "const q = html`<p>x</p>`;\n", copy = new string(before.ToCharArray());
            HostScan old = scanner.ScanAll(before);
            Assert.Same(old, IncrementalHostScan.Update(scanner, old, before, copy));
        }

        [Fact]
        public void A_safe_point_is_a_line_break_and_the_safe_points_are_in_order()
        {
            foreach (HostLanguage host in Enum.GetValues(typeof(HostLanguage)))
            {
                string text = Random(new Random(5), Pieces[host], 400);
                int[] safe = Scanner(host).ScanAll(text).SafePoints;
                for (int k = 0; k < safe.Length; k++)
                {
                    Assert.Equal('\n', text[safe[k]]);
                    if (k > 0) Assert.True(safe[k - 1] < safe[k]);
                }
            }
        }

        [Fact]
        public void No_safe_point_is_inside_a_string_a_comment_or_waiting_for_the_string_a_marker_marks()
        {
            IResumableHostScanner scanner = Scanner(HostLanguage.JavaScript);
            const string text = "a\n`x\ny`\n/* c\nd */\nb\n// language=sql\n`select\n1`\nc\n";
            int[] expected =
            {
                text.IndexOf("a\n") + 1,       // after a
                text.IndexOf("y`\n") + 2,      // after the template (not the break inside it)
                text.IndexOf("*/\n") + 2,      // after the comment (not the break inside it)
                text.IndexOf("b\n") + 1,       // after b
                                               // not the break after the marker: it still waits for its string, and not the one inside that string
                text.IndexOf("1`\n") + 2,      // after the marked string
                text.LastIndexOf("c\n") + 1    // after c
            };
            Assert.Equal(expected, scanner.ScanAll(text).SafePoints);
        }

        [Theory]
        [InlineData(HostLanguage.JavaScript)]
        [InlineData(HostLanguage.CSharp)]
        [InlineData(HostLanguage.Python)]
        [InlineData(HostLanguage.Cpp)]
        public void The_safe_points_are_far_apart_so_that_the_list_of_a_large_file_is_small(HostLanguage host)
        {
            // thousands of lines, so the list would have thousands of numbers if it had one for each line
            string marked = host == HostLanguage.JavaScript ? "sql`select 1`;\n" : host == HostLanguage.CSharp ? "// language=sql\nvar q = \"select 1\";\n"
                : host == HostLanguage.Python ? "# language=sql\nq = \"select 1\"\n" : "// language=sql\nauto q = R\"(select 1)\";\n";
            string text = string.Concat(Enumerable.Repeat("x = 1\nint pad = 1;\n", 12000)) + marked; // the string makes the host read the text at all
            IResumableHostScanner scanner = Scanner(host, SafePoints.DefaultGap);
            int[] safe = scanner.ScanAll(text).SafePoints;
            Assert.NotEmpty(safe);
            Assert.True(safe.Length <= text.Length / SafePoints.DefaultGap + 1, safe.Length + " safe points in " + text.Length + " characters");
            Assert.True(safe.Length * sizeof(int) < 85000, "the list would be on the large object heap");
            for (int k = 1; k < safe.Length; k++) Assert.True(safe[k] - safe[k - 1] >= SafePoints.DefaultGap);
            // a long run of plain code, with a string in the middle of it, can still be edited
            int at = text.Length / 2;
            string edited = text.Insert(at, "x");
            Assert.Equal(Dump(scanner.ScanAll(edited), false), Dump(IncrementalHostScan.Update(scanner, scanner.ScanAll(text), text, edited), false));
        }

        // ---- the cache that uses it -------------------------------------------------------------------------------------------

        [Fact]
        public void The_caching_scanner_updates_the_scan_of_the_text_before_when_it_is_still_alive()
        {
            var cache = new CachingHostScanner(NestLightComposition.CreateScanner(HostLanguage.JavaScript, NestLightComposition.CreateEmbeddedLanguages()));
            string padding = string.Concat(Enumerable.Repeat("const pad = 1;\n", 50));
            string first = padding + "const a = html`<p>x</p>`;\n" + padding;
            string second = padding + "const a = html`<p>xy</p>`;\n" + padding; // the text before is held by this variable
            IReadOnlyList<EmbeddedString> one = cache.Scan(first);
            IReadOnlyList<EmbeddedString> two = cache.Scan(second);
            Assert.NotSame(one, two);
            Assert.Equal(1, two.Count);
            Assert.Equal(second.IndexOf("`<p>") + 1, two[0].Start);
            GC.KeepAlive(first);

            var fresh = Scanner(HostLanguage.JavaScript).ScanAll(second);
            Assert.Equal(Dump(fresh), Dump(new HostScan(two, fresh.SafePoints)));
        }

        [Fact]
        public void The_caching_scanner_serves_the_same_instance_of_the_text_from_memory_as_before()
        {
            var cache = new CachingHostScanner(NestLightComposition.CreateScanner(HostLanguage.Python, NestLightComposition.CreateEmbeddedLanguages()));
            string text = "# language=sql\nq = \"select 1\"\n";
            Assert.Same(cache.Scan(text), cache.Scan(text));
        }

        [Fact]
        public void The_caching_scanner_follows_a_chain_of_edits_in_every_host()
        {
            foreach (HostLanguage host in Enum.GetValues(typeof(HostLanguage)))
            {
                var cache = new CachingHostScanner(NestLightComposition.CreateScanner(host, NestLightComposition.CreateEmbeddedLanguages()));
                IResumableHostScanner reference = Scanner(host);
                var random = new Random(31 + (int)host);
                string text = Random(random, Pieces[host], 80);
                for (int step = 0; step < 300; step++)
                {
                    text = Edit(random, Pieces[host], text);
                    IReadOnlyList<EmbeddedString> got = cache.Scan(text);
                    HostScan expected = reference.ScanAll(text);
                    Assert.Equal(Dump(expected, false), Dump(new HostScan(got, expected.SafePoints), false));
                }
            }
        }
    }
}
