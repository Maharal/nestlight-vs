using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
            Assert.Equal("sql", site.EmbeddedLanguageId);
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
            bool dash = Vocabularies.IsExtraWordChar(site.EmbeddedLanguageId, '-');
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
                            .Where(w => !Vocabularies.For(site.EmbeddedLanguageId).Contains(w, StringComparer.OrdinalIgnoreCase)).ToList();
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

        // ---- similar words (the second stage) --------------------------------------------------------------------------

        private static CompletionEngine WithMatcher(HostLanguage host, int maxItems = CompletionEngine.DefaultMaxItems, int fuzzyBelow = 1,
            int fuzzyMaxItems = CompletionEngine.DefaultFuzzyMaxItems, bool firstLetter = true)
        {
            return new CompletionEngine(Pipeline.Scanner(host), maxItems, CompletionEngine.DefaultMinWordLength, new BandedPrefixMatcher(), fuzzyBelow, fuzzyMaxItems, firstLetter);
        }

        private static List<Suggestion> Similar(string codeWithCaret, HostLanguage host = HostLanguage.JavaScript, int fuzzyBelow = 1,
            int fuzzyMaxItems = CompletionEngine.DefaultFuzzyMaxItems, bool firstLetter = true, int maxItems = CompletionEngine.DefaultMaxItems)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            CompletionEngine engine = WithMatcher(host, maxItems, fuzzyBelow, fuzzyMaxItems, firstLetter);
            CompletionSite site = engine.Locate(code, caret);
            return site == null ? null : engine.Suggest(code, site).ToList();
        }

        private static List<string> Texts(List<Suggestion> items) { return items.Select(s => s.Text).ToList(); }

        [Fact]
        public void A_missing_letter_in_a_keyword_still_finds_it()
        {
            Assert.Contains("select", Texts(Similar("sql`selct|`")));
        }

        [Fact]
        public void The_similar_keyword_follows_the_case_that_was_typed()
        {
            Assert.Contains("SELECT", Texts(Similar("sql`SELCT|`")));
        }

        [Fact]
        public void Swapped_neighbours_in_a_tag_still_find_it()
        {
            Assert.Contains("div", Texts(Similar("html`<dvi|`")));
        }

        [Fact]
        public void A_wrong_letter_in_a_css_property_still_finds_it()
        {
            Assert.Contains("color", Texts(Similar("css`.a { colr| }`")));
        }

        [Fact]
        public void A_word_of_the_document_is_found_with_two_edits_allowed_for_a_long_prefix()
        {
            List<Suggestion> items = Similar("const customerName = 1;\nsql`select custmer|`");
            Assert.Contains("customerName", Texts(items));
            Assert.Equal(1, items.First(s => s.Text == "customerName").Distance);
        }

        [Fact]
        public void The_first_letter_has_to_be_the_one_typed_unless_the_option_is_off()
        {
            Assert.DoesNotContain("customerName", Texts(Similar("const customerName = 1;\nsql`select xustomer|`")));
            Assert.Contains("customerName", Texts(Similar("const customerName = 1;\nsql`select xustomer|`", firstLetter: false)));
        }

        [Fact]
        public void With_fewer_than_three_letters_nothing_similar_is_offered()
        {
            Assert.Empty(Similar("sql`sl|`"));
            // the exact word is still offered; only the similar ones need three letters
            Assert.Empty(Similar("const slot = 1;\nsql`sl|`").Where(s => s.Distance > 0));
            Assert.Empty(Similar("const select = 1;\nsql`sx|`"));
        }

        [Fact]
        public void When_the_prefix_matches_the_result_is_the_one_without_the_second_stage()
        {
            const string code = "const selection = 1;\nsql`sel|`";
            int caret = code.IndexOf('|');
            string text = code.Remove(caret, 1);
            var plain = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript));
            var similar = WithMatcher(HostLanguage.JavaScript);
            Assert.Equal(plain.Suggest(text, plain.Locate(text, caret)).Select(s => s.Text + "|" + s.Kind).ToList(),
                         similar.Suggest(text, similar.Locate(text, caret)).Select(s => s.Text + "|" + s.Kind).ToList());
        }

        [Fact]
        public void Without_a_matcher_the_engine_is_the_first_stage()
        {
            Assert.Empty(At(HostLanguage.JavaScript, "sql`selct|`"));
        }

        [Fact]
        public void Exact_suggestions_have_distance_zero_and_similar_ones_at_least_one()
        {
            Assert.All(Similar("const selection = 1;\nsql`sel|`"), s => Assert.Equal(0, s.Distance));
            Assert.All(Similar("sql`selct|`"), s => Assert.InRange(s.Distance, 1, 1));
            Assert.All(Similar("sql`selcct|`"), s => Assert.InRange(s.Distance, 1, 1));
            Assert.All(Similar("sql`seleect|`"), s => Assert.InRange(s.Distance, 1, 2));
        }

        [Fact]
        public void The_second_stage_can_also_run_below_a_larger_threshold()
        {
            // "selctor" starts with what was typed, so the first stage has one item; select is one edit away
            const string code = "const selctor = 1;\nsql`selct|`";
            Assert.Equal(new[] { "selctor" }, Texts(Similar(code)));                    // the default: only when nothing matched
            Assert.Equal(new[] { "selctor" }, Texts(Similar(code, fuzzyBelow: 1)));
            List<Suggestion> more = Similar(code, fuzzyBelow: 2);
            Assert.Equal("selctor", more[0].Text);                                       // the exact one is never displaced
            Assert.Equal(0, more[0].Distance);
            Assert.Contains("select", Texts(more));
        }

        [Fact]
        public void No_more_than_the_limit_of_similar_items_and_the_limit_in_all()
        {
            string words = string.Join(" ", Enumerable.Range(0, 26).Select(i => "customer" + (char)('A' + i)).Concat(Enumerable.Range(0, 4).Select(i => "customer" + i)));
            List<Suggestion> items = Similar("const " + words.Replace(" ", ", ") + ";\nsql`select custmer|`");
            Assert.Equal(CompletionEngine.DefaultFuzzyMaxItems, items.Count(s => s.Distance > 0));
            Assert.Equal(3, Similar("const " + words.Replace(" ", ", ") + ";\nsql`select custmer|`", maxItems: 3).Count);
            Assert.Equal(4, Similar("const " + words.Replace(" ", ", ") + ";\nsql`select custmer|`", fuzzyMaxItems: 4).Count);
        }

        [Fact]
        public void A_similar_word_that_is_also_a_keyword_is_offered_once_as_the_keyword()
        {
            List<Suggestion> items = Similar("const select = 1;\nsql`selct|`");
            Assert.Equal(1, items.Count(s => string.Equals(s.Text, "select", StringComparison.OrdinalIgnoreCase)));
            Assert.Equal(SuggestionKind.Keyword, items.First(s => string.Equals(s.Text, "select", StringComparison.OrdinalIgnoreCase)).Kind);
        }

        [Fact]
        public void The_word_being_typed_is_not_similar_to_itself()
        {
            Assert.Empty(Similar("sql`zzzqqq|`"));
            Assert.DoesNotContain("custmer", Texts(Similar("sql`custmer|`")));
        }

        [Fact]
        public void Similar_words_come_ordered_keywords_then_fewer_edits_then_nearest_then_alphabetical()
        {
            // two edits for the first, one for the others; zebra and apple are as far from the caret as each other in the text
            List<Suggestion> items = Similar("const customer = 1; const customerZ = 1; const customerA = 1; const customers = 1;\nsql`custmer|`");
            var similar = items.Where(s => s.Distance > 0).ToList();
            Assert.True(similar.Count >= 3);
            for (int i = 1; i < similar.Count; i++)
            {
                Suggestion a = similar[i - 1], b = similar[i];
                if (a.Kind == b.Kind) Assert.True(a.Distance <= b.Distance, a.Text + " before " + b.Text);
                else Assert.Equal(SuggestionKind.Keyword, a.Kind);
            }
            // the same text and the same options give the same list
            Assert.Equal(Texts(items), Texts(Similar("const customer = 1; const customerZ = 1; const customerA = 1; const customers = 1;\nsql`custmer|`")));
        }

        [Fact]
        public void A_cancelled_request_throws_and_returns_no_partial_list()
        {
            const string code = "const customerName = 1;\nsql`select custmer`";
            CompletionEngine engine = WithMatcher(HostLanguage.JavaScript);
            CompletionSite site = engine.Locate(code, code.Length - 1);
            var cancelled = new CancellationTokenSource();
            cancelled.Cancel();
            Assert.Throws<OperationCanceledException>(() => engine.Suggest(code, site, cancelled.Token));
            // when the second stage does not run, the first stage has no reason to look at the token
            const string exact = "const selection = 1;\nsql`sel`";
            CompletionSite exactSite = engine.Locate(exact, exact.Length - 1);
            Assert.NotEmpty(engine.Suggest(exact, exactSite, cancelled.Token));
        }

        [Fact]
        public void The_second_stage_gives_the_same_answers_from_many_threads()
        {
            string code = CorpusLike(HostLanguage.JavaScript, 0.5).Replace("select cust ", "select custmer ");
            CompletionEngine engine = WithMatcher(HostLanguage.JavaScript);
            var random = new Random(5);
            var carets = Enumerable.Range(0, 300).Select(_ => random.Next(code.Length + 1)).ToList();
            Func<int, string> run = caret =>
            {
                CompletionSite site = engine.Locate(code, caret);
                return site == null ? "-" : string.Join(",", engine.Suggest(code, site).Select(s => s.Text + ":" + s.Distance));
            };
            List<string> sequential = carets.Select(run).ToList();
            List<string> parallel = carets.AsParallel().AsOrdered().WithDegreeOfParallelism(8).Select(run).ToList();
            Assert.Equal(sequential, parallel);
        }

        [Fact]
        public void The_first_stage_of_the_engine_with_a_matcher_is_always_the_engine_without_one()
        {
            int compared = 0;
            foreach (HostLanguage host in new[] { HostLanguage.JavaScript, HostLanguage.CSharp, HostLanguage.Python, HostLanguage.Cpp })
            {
                string code = CorpusLike(host, 0.5);
                var plain = new CompletionEngine(Pipeline.Scanner(host));
                CompletionEngine similar = WithMatcher(host);
                var random = new Random(11);
                for (int attempt = 0; attempt < 500; attempt++)
                {
                    int caret = random.Next(code.Length + 1);
                    CompletionSite site = plain.Locate(code, caret);
                    if (site == null) continue;
                    var expected = plain.Suggest(code, site).Select(s => s.Text + "|" + s.Kind).ToList();
                    var actual = similar.Suggest(code, similar.Locate(code, caret)).ToList();
                    Assert.Equal(expected, actual.Where(s => s.Distance == 0).Select(s => s.Text + "|" + s.Kind).ToList());
                    if (expected.Count > 0) Assert.Equal(expected.Count, actual.Count); // something matched: nothing similar is added
                    compared++;
                }
            }
            Assert.True(compared > 200);
        }

        [Fact]
        public void Similar_words_respect_the_invariants_on_cut_and_damaged_code()
        {
            int sites = 0, similarItems = 0;
            foreach (HostLanguage host in new[] { HostLanguage.JavaScript, HostLanguage.CSharp, HostLanguage.Python, HostLanguage.Cpp })
            {
                string source = CorpusLike(host, 0.5).Replace("select cust", "selct custmer");
                CompletionEngine engine = WithMatcher(host);
                var random = new Random(21);
                var texts = new List<string>();
                for (int cut = 0; cut <= source.Length; cut += 53) texts.Add(source.Substring(0, cut));
                for (int at = 0; at < source.Length; at += 53) texts.Add(source.Remove(at, 1));
                foreach (string text in texts)
                    for (int attempt = 0; attempt < 6 && text.Length > 0; attempt++)
                    {
                        int caret = attempt == 0 ? text.Length : random.Next(text.Length + 1);
                        CompletionSite site = engine.Locate(text, caret);
                        if (site == null) continue;
                        sites++;
                        string typed = text.Substring(site.Start, site.PrefixLength);
                        IReadOnlyList<Suggestion> items = engine.Suggest(text, site);
                        Assert.True(items.Count <= CompletionEngine.DefaultMaxItems);
                        Assert.Equal(items.Count, items.Select(s => s.Text.ToLowerInvariant()).Distinct().Count());
                        foreach (Suggestion s in items)
                        {
                            if (s.Distance == 0) Assert.StartsWith(typed, s.Text, StringComparison.OrdinalIgnoreCase);
                            else
                            {
                                similarItems++;
                                Assert.InRange(s.Distance, 1, CompletionEngine.ToleranceFor(typed.Length));
                                Assert.True(typed.Length >= CompletionEngine.FuzzyMinPrefix);
                                Assert.Equal(char.ToUpperInvariant(typed[0]), char.ToUpperInvariant(s.Text[0]));
                            }
                        }
                        Assert.True(items.Count(s => s.Distance > 0) <= CompletionEngine.DefaultFuzzyMaxItems);
                    }
            }
            Assert.True(sites > 200 && similarItems > 20, sites + " sites, " + similarItems + " similar items");
        }
    }
}
