using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using NestLight.Common;
using NestLight.Highlighting;
using NestLight.Hosts;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>
    /// Properties that must hold for any text, found by feeding the pipeline random and half-typed code: nothing throws, every range is
    /// inside the text, and an edit never changes what a scan finds. When a property fails, FsCheck prints the smallest text it found and the
    /// seed to replay it.
    /// </summary>
    public class FuzzTests
    {
        private const int Runs = 500;

        /// <summary>Pieces of code (and of broken code) that the languages and the hosts treat specially.</summary>
        private static readonly string[] Pieces =
        {
            "<div class=\"", "</div>", "<!--", "-->", "<![CDATA[", "<style>", "</style>", "<script>", "style=\"color:red\"",
            "{", "}", "(", ")", "[", "]", ";", ":", ",", ".", "#", "@", "$", "${", "{{", "}}", "\\", "\\n", "\\u00", "\\\\",
            "\"", "'", "`", "\"\"\"", "'''", "R\"(", ")\"", "r#\"", "\"#", "/*", "*/", "//", "--", "\n", "\r\n", "\t", " ", "  ",
            "SELECT * FROM t WHERE", "select", "from", "color: ", "#fff", "12px", "rgba(", "url(", "calc(", "@media", "!important",
            "query { a(b: $c) }", "{\"a\": [1, true, null]}", "key: value", "- item", "[table]", "a = 1", "# heading", "**b**", "`code`",
            "echo $HOME | grep -v", "vec4 c = vec3(1.0);", "fn main() {}", "(a|b)+?[^x]{2,3}\\d",
            "html", "css", "sql", "json", "regex", "// html\n", "/* css */", "# language=sql\n", "language=html", "html`", "css`", "sql`",
            "a", "b", "x1", "_", "0", "9", "é", "漢", "\0"
        };

        private static Gen<string> TextGen(int maxPieces)
        {
            return from n in Gen.Choose(0, maxPieces)
                   from parts in Gen.ArrayOf(n, Gen.Elements(Pieces))
                   select string.Concat(parts);
        }

        private static Arbitrary<string> Texts(int maxPieces = 40)
        {
            return Arb.From(TextGen(maxPieces), Shrink);
        }

        private static IEnumerable<string> Shrink(string text)
        {
            if (text.Length == 0) yield break;
            yield return "";
            yield return text.Substring(0, text.Length / 2);
            yield return text.Substring(text.Length / 2);
            if (text.Length > 1) yield return text.Substring(1);
            yield return text.Substring(0, text.Length - 1);
        }

        private static void Check(Property property)
        {
            var configuration = Configuration.QuickThrowOnFailure;
            configuration.MaxNbOfTest = Runs;
            property.Check(configuration);
        }

        private static readonly HostLanguage[] Hosts = (HostLanguage[])Enum.GetValues(typeof(HostLanguage));

        // ---- the tokenizers ----

        /// <remarks>Empty tokens are allowed: the highlighter drops them.</remarks>
        [Fact]
        public void A_tokenizer_never_throws_and_emits_tokens_inside_its_range()
        {
            var tokenizers = ((EmbeddedLanguageRegistry)Pipeline.Languages).Ids.Select(id => Pipeline.Languages.Find(id)).Distinct().ToList();
            Assert.NotEmpty(tokenizers);

            var input = from text in TextGen(40)
                        from a in Gen.Choose(0, text.Length)
                        from b in Gen.Choose(0, text.Length)
                        select Tuple.Create(text, Math.Min(a, b), Math.Max(a, b));

            Check(Prop.ForAll(Arb.From(input), t =>
            {
                char[] chars = t.Item1.ToCharArray();
                foreach (IEmbeddedLanguageTokenizer tokenizer in tokenizers)
                {
                    int from = t.Item2, to = t.Item3;
                    tokenizer.Tokenize(chars, from, to, (start, end, type) =>
                    {
                        if (start < from || end > to || start > end || string.IsNullOrEmpty(type))
                            throw new InvalidOperationException(tokenizer.Ids[0] + " emitted [" + start + ", " + end + ") '" + type + "' outside [" + from + ", " + to + ")");
                    });
                }
                return true;
            }));
        }

        // ---- the hosts ----

        [Fact]
        public void A_scan_finds_ordered_strings_inside_the_text_in_every_host()
        {
            Check(Prop.ForAll(Texts(), text =>
            {
                foreach (HostLanguage host in Hosts)
                {
                    int previous = -1;
                    foreach (EmbeddedString s in Pipeline.Scanner(host).Scan(text))
                    {
                        bool ordered = 0 <= s.OuterStart && s.OuterStart <= s.Start && s.Start <= s.End && s.End <= s.OuterEnd && s.OuterEnd <= text.Length;
                        if (!ordered || s.OuterStart < previous)
                            throw new InvalidOperationException(host + " found a string out of place: " + Describe(s));
                        previous = s.OuterStart;
                        foreach (var i in s.Interpolations)
                            if (i.Start < s.Start || i.End > s.End || i.Start > i.End)
                                throw new InvalidOperationException(host + " found an interpolation outside its string: " + Describe(s));
                    }
                }
                return true;
            }));
        }

        [Fact]
        public void Highlighting_never_throws_and_every_token_is_inside_the_text_in_every_host()
        {
            Check(Prop.ForAll(Texts(), text =>
            {
                foreach (HostLanguage host in Hosts)
                    foreach (Token t in Pipeline.Highlighter(host).Highlight(text))
                        if (t.Start < 0 || t.Length <= 0 || t.Start + t.Length > text.Length)
                            throw new InvalidOperationException(host + " emitted a token out of range: " + t.Start + "+" + t.Length + " of " + text.Length);
                return true;
            }));
        }

        [Fact]
        public void The_embedded_strings_of_a_complete_scan_are_the_ones_of_a_plain_scan()
        {
            Check(Prop.ForAll(Texts(), text =>
            {
                foreach (HostLanguage host in Hosts)
                {
                    IResumableHostScanner scanner = Scanner(host, 0);
                    string whole = Dump(scanner.ScanAll(text).Strings), plain = Dump(scanner.Scan(text));
                    if (whole != plain) throw new InvalidOperationException(host + " scans differ:\n" + whole + "\n" + plain);
                }
                return true;
            }));
        }

        // ---- editing ----

        [Fact]
        public void Updating_a_scan_after_an_edit_gives_the_scan_of_the_edited_text()
        {
            var input = from before in TextGen(40)
                        from at in Gen.Choose(0, before.Length)
                        from removed in Gen.Choose(0, Math.Min(12, before.Length - at))
                        from inserted in TextGen(4)
                        select Tuple.Create(before, at, removed, inserted);

            Check(Prop.ForAll(Arb.From(input), edit =>
            {
                string before = edit.Item1;
                string after = before.Substring(0, edit.Item2) + edit.Item4 + before.Substring(edit.Item2 + edit.Item3);
                foreach (HostLanguage host in Hosts)
                {
                    IResumableHostScanner scanner = Scanner(host, 0);
                    HostScan updated = IncrementalHostScan.Update(scanner, scanner.ScanAll(before), before, after);
                    string expected = Dump(scanner.ScanAll(after).Strings), actual = Dump(updated.Strings);
                    if (expected != actual)
                        throw new InvalidOperationException(host + " update differs.\nexpected:\n" + expected + "\nupdated:\n" + actual);
                }
                return true;
            }));
        }

        // ---- helpers ----

        /// <summary>The scanner of the host, keeping a safe point at every line break it reaches, so that an update has something to resume from.</summary>
        private static IResumableHostScanner Scanner(HostLanguage host, int gap)
        {
            IEmbeddedLanguageRegistry registry = Pipeline.Languages;
            switch (host)
            {
                case HostLanguage.JavaScript: return new JavaScriptHostScanner(new AcceptedEmbeddedLanguages(registry), gap);
                case HostLanguage.CSharp: return new CSharpHostScanner(new AcceptedEmbeddedLanguages(registry, new[] { "json", "regex", "regexp" }), gap);
                case HostLanguage.Python: return new PythonHostScanner(new AcceptedEmbeddedLanguages(registry), gap);
                case HostLanguage.Cpp: return new CppHostScanner(new AcceptedEmbeddedLanguages(registry), gap);
                case HostLanguage.Rust: return new RustHostScanner(new AcceptedEmbeddedLanguages(registry), gap);
                case HostLanguage.Go: return new GoHostScanner(new AcceptedEmbeddedLanguages(registry), gap);
                default: return new JavaHostScanner(new AcceptedEmbeddedLanguages(registry), gap);
            }
        }

        private static string Describe(EmbeddedString s)
        {
            return s.EmbeddedLanguageId + "|" + s.OuterStart + "," + s.Start + "," + s.End + "," + s.OuterEnd;
        }

        private static string Dump(IReadOnlyList<EmbeddedString> strings)
        {
            return string.Join("\n", strings.Select(s =>
                Describe(s) +
                "|I:" + string.Join(";", s.Interpolations.Select(x => x.Start + "," + x.End + "," + x.OpenLength + "," + x.CloseLength)) +
                "|E:" + string.Join(";", s.Escapes.Select(e => e.Start + "," + e.Length + "," + (int)e.Value))));
        }
    }
}
