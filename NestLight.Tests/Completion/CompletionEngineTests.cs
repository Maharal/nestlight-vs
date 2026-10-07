using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class CompletionEngineTests
    {
        /// <summary>The code with a '|' where the caret is; returns the suggestions there, or null outside embedded code.</summary>
        private static List<string> At(HostLanguage host, string codeWithCaret, int minWordLength = CompletionEngine.DefaultMinWordLength)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(host), CompletionEngine.DefaultMaxItems, minWordLength);
            CompletionSite site = engine.Locate(code, caret);
            return site == null ? null : engine.Suggest(code, site).Select(s => s.Text).ToList();
        }

        private static List<string> Js(string codeWithCaret) { return At(HostLanguage.JavaScript, codeWithCaret); }

        // ---- where it applies -------------------------------------------------------------------------------------

        [Fact]
        public void Outside_embedded_strings_there_is_no_site()
        {
            Assert.Null(Js("const a = sel|"));
            Assert.Null(Js("const a = `sel|`;")); // untagged
            Assert.Null(Js("const a = \"sel|\";"));
        }

        [Fact]
        public void Inside_a_marked_string_there_is_a_site()
        {
            Assert.NotNull(Js("sql`sel|`"));
            Assert.NotNull(Js("/* sql */ `sel|`"));
            Assert.NotNull(At(HostLanguage.CSharp, "// language=sql\nvar q = \"sel|\";"));
            Assert.NotNull(At(HostLanguage.Python, "# language=sql\nq = \"sel|\""));
            Assert.NotNull(At(HostLanguage.Cpp, "// language=sql\nauto q = R\"(sel|)\";"));
        }

        [Fact]
        public void Not_inside_an_interpolation()
        {
            Assert.Null(Js("sql`select ${sel|} from t`"));
            Assert.NotNull(Js("sql`select ${x} sel| from t`"));
        }

        [Fact]
        public void The_innermost_string_wins_when_templates_are_nested()
        {
            List<string> inner = Js("html`<div>${ sql`sel|` }</div>`");
            Assert.NotNull(inner);
            Assert.Contains("select", inner);
        }

        [Fact]
        public void The_site_covers_the_whole_word_and_remembers_the_prefix()
        {
            const string code = "sql`select fro_m x`";
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript));
            CompletionSite site = engine.Locate(code, code.IndexOf("_m"));
            Assert.Equal(code.IndexOf("fro_m"), site.Start);
            Assert.Equal(code.IndexOf(" x"), site.End);
            Assert.Equal(3, site.PrefixLength);
            Assert.Equal("sql", site.LanguageId);
        }

        [Fact]
        public void Invalid_positions_and_incomplete_code_do_not_throw()
        {
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript));
            Assert.Null(engine.Locate(null, 0));
            Assert.Null(engine.Locate("sql`x`", -1));
            Assert.Null(engine.Locate("sql`x`", 99));
            Assert.NotNull(Js("sql`select * fro|")); // unclosed
            Assert.NotNull(Js("sql`select ${ fro|")); // unclosed interpolation outside of it
        }

        [Fact]
        public void Numbers_are_not_words()
        {
            Assert.Null(Js("sql`select 12|`"));
        }

        // ---- keywords ---------------------------------------------------------------------------------------------

        [Fact]
        public void Keywords_of_the_language_that_start_with_the_prefix()
        {
            List<string> items = Js("sql`sel|`");
            Assert.Contains("select", items);
            Assert.DoesNotContain("from", items);
        }

        [Theory]
        [InlineData("css", "backgr", "background-color")]
        [InlineData("html", "bu", "button")]
        [InlineData("graphql", "mut", "mutation")]
        [InlineData("gql", "frag", "fragment")]
        [InlineData("json", "tr", "true")]
        [InlineData("yaml", "fal", "false")]
        [InlineData("glsl", "smoothst", "smoothstep")]
        [InlineData("wgsl", "textu", "texture_2d")]
        public void Every_language_with_a_vocabulary_offers_it(string id, string prefix, string expected)
        {
            Assert.Contains(expected, Js(id + "`" + prefix + "|`"));
        }

        [Fact]
        public void Languages_without_a_vocabulary_offer_only_words()
        {
            foreach (string id in new[] { "xml", "markdown", "regex" })
                Assert.DoesNotContain("select", Js(id + "`sel|`"));
            Assert.Empty(Vocabularies.For("xml"));
            Assert.Empty(Vocabularies.For(null));
        }

        [Fact]
        public void Keywords_are_matched_without_regard_to_case_and_sorted()
        {
            List<string> items = Js("sql`SEL|`");
            Assert.Contains("SELECT", items);
            List<string> lower = Js("sql`in|`");
            var keywords = lower.Where(w => Vocabularies.For("sql").Contains(w)).ToList();
            Assert.True(keywords.Count > 1);
            Assert.Equal(keywords.OrderBy(w => w, System.StringComparer.OrdinalIgnoreCase).ToList(), keywords);
        }

        [Fact]
        public void Sql_keywords_follow_the_case_being_typed()
        {
            Assert.Contains("select", Js("sql`sel|`"));
            Assert.Contains("SELECT", Js("sql`SEL|`"));
            Assert.DoesNotContain("select", Js("sql`SEL|`"));
        }

        [Fact]
        public void Other_languages_keep_their_own_case()
        {
            Assert.Contains("mutation", Js("graphql`MUT|`"));
        }

        [Fact]
        public void A_keyword_already_typed_in_full_is_not_offered_again()
        {
            Assert.DoesNotContain("select", Js("sql`select|`"));
        }

        [Fact]
        public void Dashed_names_are_one_word_in_css_html_and_yaml()
        {
            Assert.Contains("background-color", Js("css`.a { background-c| }`"));
            Assert.Contains("background-color", Js("css`.a { background-| }`"));
        }

        [Fact]
        public void Each_language_is_taken_from_the_marker_not_from_the_host()
        {
            List<string> items = Js("css`.a { sel| }`");
            Assert.DoesNotContain("select", items);
        }

        // ---- words of the document --------------------------------------------------------------------------------

        [Fact]
        public void Words_that_already_exist_in_the_document_are_offered()
        {
            List<string> items = Js("const customerName = 1;\nsql`select cust|`");
            Assert.Contains("customerName", items);
        }

        [Fact]
        public void The_word_being_typed_is_not_offered_to_itself()
        {
            List<string> items = Js("sql`select cust|`");
            Assert.DoesNotContain("cust", items);
            // …even when the word continues after the caret
            Assert.DoesNotContain("customer", Js("sql`select cust|omer`"));
        }

        [Fact]
        public void Words_nearest_to_the_caret_come_first()
        {
            List<string> items = Js("const prefixFar = 1;\n" + new string(' ', 10) + "const prefixNear = 2;\nsql`pre|`");
            Assert.True(items.IndexOf("prefixNear") < items.IndexOf("prefixFar"));
        }

        [Fact]
        public void Words_after_the_caret_count_too()
        {
            Assert.Contains("lateWord", Js("sql`late|`;\nconst lateWord = 1;"));
        }

        [Fact]
        public void Short_words_and_numbers_are_ignored()
        {
            Assert.DoesNotContain("ab", Js("const ab = 1;\nsql`a|`"));
            List<string> items = At(HostLanguage.JavaScript, "const ab = 1;\nsql`a|`", minWordLength: 2);
            Assert.Contains("ab", items);
            Assert.DoesNotContain("1abc", Js("const x = 1abc;\nsql`1|`") ?? new List<string>());
        }

        [Fact]
        public void Words_are_matched_without_regard_to_case_and_not_repeated()
        {
            List<string> items = Js("const UserId = 1; const UserId2 = UserId;\nsql`user|`");
            Assert.Equal(1, items.Count(w => w == "UserId"));
            Assert.Contains("UserId2", items);
        }

        [Fact]
        public void A_word_that_is_also_a_keyword_is_offered_once_as_the_keyword()
        {
            List<string> items = Js("const select = 1;\nsql`sel|`");
            Assert.Equal(1, items.Count(w => string.Equals(w, "select", System.StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public void Keywords_come_before_words()
        {
            List<string> items = Js("const selection = 1;\nsql`sel|`");
            Assert.True(items.IndexOf("select") < items.IndexOf("selection"));
        }

        [Fact]
        public void Words_come_from_every_host()
        {
            Assert.Contains("customerName", At(HostLanguage.CSharp, "var customerName = 1;\n// language=sql\nvar q = \"cust|\";"));
            Assert.Contains("customer_name", At(HostLanguage.Python, "customer_name = 1\n# language=sql\nq = \"cust|\""));
            Assert.Contains("customerName", At(HostLanguage.Cpp, "int customerName = 1;\n// language=sql\nauto q = R\"(cust|)\";"));
        }

        [Fact]
        public void The_number_of_suggestions_is_limited()
        {
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), maxItems: 5);
            string code = string.Join(" ", Enumerable.Range(0, 50).Select(i => "word" + i)) + "\nsql`wo`";
            int caret = code.Length - 2;
            Assert.Equal(5, engine.Suggest(code, engine.Locate(code, caret)).Count);
            string keywords = "sql``";
            Assert.Equal(5, engine.Suggest(keywords, engine.Locate(keywords, 4)).Count);
        }

        [Fact]
        public void Suggestions_are_marked_as_keyword_or_word()
        {
            const string code = "const selection = 1;\nsql`sel`";
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript));
            List<Suggestion> items = engine.Suggest(code, engine.Locate(code, code.Length - 2)).ToList();
            Assert.Equal(SuggestionKind.Keyword, items.First(s => s.Text == "select").Kind);
            Assert.Equal(SuggestionKind.Word, items.First(s => s.Text == "selection").Kind);
        }

        [Fact]
        public void A_large_document_is_scanned_in_a_reasonable_time()
        {
            string filler = string.Concat(Enumerable.Repeat("const identifierNumber = computeValue(argument);\n", 40000));
            string code = filler + "sql`ident|`";
            var watch = System.Diagnostics.Stopwatch.StartNew();
            List<string> items = Js(code);
            watch.Stop();
            Assert.Contains("identifierNumber", items);
            Assert.True(watch.ElapsedMilliseconds < 2000, "took " + watch.ElapsedMilliseconds + " ms");
        }

        // ---- the order of the words, against the straightforward implementation ---------------------------------------

        /// <summary>The words of the document as the first version of the engine produced them: every match in a dictionary, then a sort.</summary>
        private static List<string> ReferenceWords(string text, CompletionSite site, int minWordLength)
        {
            string prefix = text.Substring(site.Start, site.PrefixLength);
            bool dash = Vocabularies.IsExtraWordChar(site.LanguageId, '-');
            Func<char, bool> isStart = c => char.IsLetter(c) || c == '_' || (dash && c == '-');
            Func<char, bool> isChar = c => char.IsLetterOrDigit(c) || c == '_' || (dash && c == '-');
            int from = Math.Max(0, site.Caret - 500000), to = Math.Min(text.Length, site.Caret + 500000);

            var nearest = new Dictionary<string, int>(StringComparer.Ordinal);
            int i = from;
            while (i < to)
            {
                if (!isStart(text[i])) { i++; continue; }
                int start = i;
                while (i < to && isChar(text[i])) i++;
                int length = i - start;
                if (length < minWordLength || length > 64) continue;
                if (start <= site.Caret && site.Caret <= i) continue;
                if (length == prefix.Length) continue;
                if (string.Compare(text, start, prefix, 0, prefix.Length, StringComparison.OrdinalIgnoreCase) != 0) continue;
                string word = text.Substring(start, length);
                int distance = start >= site.Caret ? start - site.Caret : site.Caret - i;
                int known;
                if (!nearest.TryGetValue(word, out known) || distance < known) nearest[word] = distance;
            }
            var ordered = nearest.OrderBy(p => p.Value).ThenBy(p => p.Key, StringComparer.Ordinal).Select(p => p.Key);
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var result = new List<string>();
            foreach (string word in ordered) if (seen.Add(word)) result.Add(word);
            return result;
        }

        [Fact]
        public void The_words_come_in_the_same_order_as_the_straightforward_implementation()
        {
            int compared = 0;
            foreach (double locality in new[] { 0.0, 0.8 })
            {
                var random = new Random(99);
                foreach (HostLanguage host in new[] { HostLanguage.JavaScript, HostLanguage.CSharp, HostLanguage.Python, HostLanguage.Cpp })
                {
                    string code = CorpusLike(host, locality);
                    var engine = new CompletionEngine(Pipeline.Scanner(host), maxItems: 100000);
                    for (int attempt = 0; attempt < 600; attempt++)
                    {
                        int caret = random.Next(code.Length + 1);
                        CompletionSite site = engine.Locate(code, caret);
                        if (site == null) continue;
                        List<string> actual = engine.Suggest(code, site).Where(s => s.Kind == SuggestionKind.Word).Select(s => s.Text).ToList();
                        List<string> expected = ReferenceWords(code, site, CompletionEngine.DefaultMinWordLength)
                            .Where(w => !Vocabularies.For(site.LanguageId).Contains(w, StringComparer.OrdinalIgnoreCase)).ToList();
                        Assert.Equal(expected, actual);
                        compared++;
                    }
                }
            }
            Assert.True(compared > 200, "only " + compared + " carets were inside embedded code");
        }

        /// <summary>A small program that reuses a handful of names, with a marked string per function.</summary>
        private static string CorpusLike(HostLanguage host, double locality)
        {
            var random = new Random((int)(locality * 10) + 5);
            string[] names = { "customerName", "customerId", "orderTotal", "orderCount", "order_total", "Order-Total", "invoiceDate", "invoice_date", "select_all", "item" };
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < 60; i++)
            {
                string a = names[random.Next(names.Length)], b = names[random.Next(names.Length)], c = names[random.Next(names.Length)];
                switch (host)
                {
                    case HostLanguage.JavaScript: sb.Append("const " + a + " = " + b + ";\nsql`select " + c + " from t where " + a + " = ${" + b + "}`;\n"); break;
                    case HostLanguage.CSharp: sb.Append("var " + a + " = " + b + ";\n// language=sql\nvar q = $\"select " + c + " from t where " + a + " = {" + b + "}\";\n"); break;
                    case HostLanguage.Python: sb.Append(a + " = " + b + "\n# language=sql\nq = f\"select " + c + " from t where " + a + " = {" + b + "}\"\n"); break;
                    default: sb.Append("int " + a + " = " + b + ";\n// language=sql\nauto q = R\"(select " + c + " from t where " + a + " = 1)\";\n"); break;
                }
            }
            return sb.ToString();
        }

        [Theory]
        [InlineData("zetaX", "zetaA")]
        [InlineData("zetaA", "zetaX")]
        public void Two_words_at_the_same_distance_on_both_sides_come_in_alphabetical_order(string beforeCaret, string afterCaret)
        {
            // ";sql`zet" is 8 characters before the caret and "`" plus 7 blanks 8 after it
            List<string> items = Js("const " + beforeCaret + ";sql`zet|`       " + afterCaret + " = 2;");
            Assert.True(items.IndexOf("zetaA") >= 0 && items.IndexOf("zetaA") < items.IndexOf("zetaX"), string.Join(",", items));
        }

        [Fact]
        public void A_repeated_word_is_offered_once_and_a_case_variant_is_not_a_second_suggestion()
        {
            string code = string.Concat(Enumerable.Repeat("const userName = 1; ", 500)) + "const UserName = 2;\nsql`user|`";
            List<string> items = Js(code);
            Assert.Equal(1, items.Count(w => string.Equals(w, "userName", StringComparison.OrdinalIgnoreCase)));
        }
    }
}
