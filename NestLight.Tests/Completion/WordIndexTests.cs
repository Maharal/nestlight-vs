using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The words of the document kept in memory: the same words as reading the text, brought up to date after an edit without reading it all again.</summary>
    public class WordIndexTests
    {
        /// <summary>The straightforward way: read the text, character by character, with the rule of the scan of the engine.</summary>
        private static List<string> Reference(string text, bool dash)
        {
            var words = new List<string>();
            int i = 0;
            while (i < text.Length)
            {
                char c = text[i];
                if (!(char.IsLetter(c) || c == '_' || (dash && c == '-'))) { i++; continue; }
                int start = i;
                while (i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '_' || (dash && text[i] == '-'))) i++;
                words.Add(start + ":" + (i - start));
            }
            return words;
        }

        private static List<string> Describe(WordIndex index)
        {
            var words = new List<string>();
            for (int k = 0; k < index.Count; k++) words.Add(index.Start(k) + ":" + index.Length(k));
            return words;
        }

        private static void AssertSame(string text, WordIndex index, bool dash)
        {
            Assert.Equal(Reference(text, dash), Describe(index));
            for (int k = 0; k < index.Count; k++) Assert.Equal(char.ToUpperInvariant(text[index.Start(k)]), index.Key(k));
        }

        // ---- the words ----------------------------------------------------------------------------------------------------

        [Theory]
        [InlineData("")]
        [InlineData("   \n\t")]
        [InlineData("select name from users where id = 7")]
        [InlineData("9abc 0xFF x9 _a __ 1_2 3d4")]
        [InlineData("margin-top: 10px; -webkit-box a-b- -- -")]
        [InlineData("naïve café Ünïcode ñandú 日本語 ǅ ß ı İ")]
        [InlineData("one")]
        [InlineData("a")]
        [InlineData("1")]
        public void The_index_holds_the_words_of_the_text_by_the_rule_of_the_scan(string text)
        {
            foreach (bool dash in new[] { false, true })
                AssertSame(text, WordIndex.Build(text, dash), dash);
        }

        [Fact]
        public void The_dash_makes_a_difference_only_when_the_language_says_so()
        {
            const string text = "margin-top margin";
            Assert.Equal(new[] { "0:6", "7:3", "11:6" }, Describe(WordIndex.Build(text, false)).ToArray());
            Assert.Equal(new[] { "0:10", "11:6" }, Describe(WordIndex.Build(text, true)).ToArray());
            Assert.True(WordIndex.Build(text, true).Dash);
        }

        [Fact]
        public void The_first_word_at_or_after_a_position_is_found_by_search()
        {
            WordIndex index = WordIndex.Build("ab cd  ef", false); // ab 0, cd 3, ef 7
            Assert.Equal(0, index.FirstAtOrAfter(0));
            Assert.Equal(1, index.FirstAtOrAfter(1));
            Assert.Equal(1, index.FirstAtOrAfter(3));
            Assert.Equal(2, index.FirstAtOrAfter(4));
            Assert.Equal(2, index.FirstAtOrAfter(7));
            Assert.Equal(3, index.FirstAtOrAfter(8));
            Assert.Equal(0, WordIndex.Build("", false).FirstAtOrAfter(5));
        }

        [Fact]
        public void An_index_reports_how_much_memory_it_holds()
        {
            WordIndex index = WordIndex.Build(string.Concat(Enumerable.Repeat("alpha beta ", 1000)), false);
            Assert.Equal(2000, index.Count);
            Assert.Equal(2000L * 10, index.Bytes);
        }

        // ---- after an edit --------------------------------------------------------------------------------------------------

        [Theory]
        [InlineData("select a from t", "select ab from t")]            // a letter makes a word longer
        [InlineData("select a from t", "select  from t")]              // a word disappears
        [InlineData("select a from t", "select a from")]               // the last word
        [InlineData("select a from t", "select a fromt")]              // two words become one
        [InlineData("select a from t", "select a fr om t")]            // one word becomes two
        [InlineData("select a from t", "9select a from t")]            // a digit in front: the word is the same
        [InlineData("9select a from t", "select a from t")]
        [InlineData("a9b", "a b")]
        [InlineData("a b", "a9b")]
        [InlineData("x 9ab", "x ab")]                                    // the digits that skip the start of a word
        [InlineData("x ab", "x 9ab")]
        [InlineData("", "hello")]
        [InlineData("hello", "")]
        [InlineData("hello", "hello")]
        [InlineData("ab cd", "cd ab")]
        [InlineData("ab cd ef", "ab XX ef")]
        [InlineData("ab cd ef", "XXXXXXXXXX")]
        public void The_updated_index_is_the_index_of_the_new_text(string before, string after)
        {
            foreach (bool dash in new[] { false, true })
            {
                WordIndex updated = WordIndex.Build(before, dash).Update(before, after);
                AssertSame(after, updated, dash);
                Assert.Equal(Describe(WordIndex.Build(after, dash)), Describe(updated));
            }
        }

        [Fact]
        public void An_update_with_nothing_changed_is_the_same_index()
        {
            const string text = "select a from t";
            WordIndex index = WordIndex.Build(text, false);
            Assert.Same(index, index.Update(text, text));
            Assert.Same(index, index.Update(text, new string(text.ToCharArray()))); // another string, the same text
        }

        [Fact]
        public void An_index_is_not_changed_by_the_update_that_it_made()
        {
            const string before = "alpha beta gamma", after = "alpha betaX gamma delta";
            WordIndex old = WordIndex.Build(before, false);
            WordIndex updated = old.Update(before, after);
            AssertSame(before, old, false);   // the request that is still running with the old one is not affected
            AssertSame(after, updated, false);
        }

        [Fact]
        public void Random_edits_of_random_texts_always_give_the_index_of_the_new_text()
        {
            // letters, digits, the underscore, the dash, blanks and a few non-ASCII letters: every way for a word to start, grow, split and join
            const string alphabet = "abcXYZ_-9 \n.é日";
            var random = new Random(20260610);
            for (int round = 0; round < 3000; round++)
            {
                bool dash = round % 2 == 0;
                string text = Chars(random, alphabet, random.Next(0, 80));
                WordIndex index = WordIndex.Build(text, dash);
                // a chain of edits: the index of each text is made from the one before
                for (int step = 0; step < 6; step++)
                {
                    string edited = Edit(random, alphabet, text);
                    index = index.Update(text, edited);
                    text = edited;
                    AssertSame(text, index, dash);
                }
            }
        }

        [Fact]
        public void Edits_in_a_long_text_are_found_by_the_comparison_in_blocks()
        {
            var random = new Random(7);
            var sb = new StringBuilder();
            for (int i = 0; i < 4000; i++) sb.Append(Chars(random, "abcdefgh_ 9", random.Next(1, 9))).Append(i % 7 == 0 ? "\n" : " ");
            string text = sb.ToString();
            Assert.True(text.Length > 5000);
            WordIndex index = WordIndex.Build(text, false);
            foreach (int at in new[] { 0, 1, 255, 256, 257, text.Length / 2, text.Length - 257, text.Length - 256, text.Length - 1, text.Length })
            {
                string edited = text.Insert(at, "q");
                AssertSame(edited, index.Update(text, edited), false);
                if (at < text.Length)
                {
                    string cut = text.Remove(at, 1);
                    AssertSame(cut, index.Update(text, cut), false);
                }
            }
        }

        [Theory]
        [InlineData("", "", 0)]
        [InlineData("abc", "abd", 2)]
        [InlineData("abc", "abc", 3)]
        [InlineData("abc", "abcdef", 3)]
        [InlineData("xyz", "abc", 0)]
        public void The_common_prefix_is_found(string a, string b, int expected)
        {
            Assert.Equal(expected, WordIndex.CommonPrefix(a, b));
        }

        [Fact]
        public void The_common_suffix_never_overlaps_the_prefix()
        {
            Assert.Equal(0, WordIndex.CommonSuffix("aaa", "aa", 2));
            Assert.Equal(1, WordIndex.CommonSuffix("abXc", "abYc", 2));
            string a = new string('x', 1000) + "A" + new string('y', 1000), b = new string('x', 1000) + "B" + new string('y', 1000);
            Assert.Equal(1000, WordIndex.CommonPrefix(a, b));
            Assert.Equal(1000, WordIndex.CommonSuffix(a, b, 1000));
        }

        private static string Chars(Random random, string alphabet, int length)
        {
            var chars = new char[length];
            for (int i = 0; i < length; i++) chars[i] = alphabet[random.Next(alphabet.Length)];
            return new string(chars);
        }

        private static string Edit(Random random, string alphabet, string text)
        {
            int at = random.Next(0, text.Length + 1);
            switch (random.Next(4))
            {
                case 0: return text.Insert(at, Chars(random, alphabet, random.Next(1, 4)));
                case 1: return text.Remove(at, Math.Min(random.Next(1, 4), text.Length - at));
                case 2:
                {
                    int cut = Math.Min(random.Next(1, 6), text.Length - at);
                    return text.Remove(at, cut).Insert(at, Chars(random, alphabet, random.Next(0, 6)));
                }
                default: return Chars(random, alphabet, random.Next(0, 60)); // another text altogether
            }
        }

        // ---- behind the engine ----------------------------------------------------------------------------------------------

        private static CompletionEngine Engine(CompletionFeatures features)
        {
            return new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), features: features);
        }

        private static CompletionFeatures With(bool index, bool previous = true, bool sameLanguage = true, bool grammar = true, bool shortLast = true)
        {
            return new CompletionFeatures(previousWord: previous, sameLanguageWords: sameLanguage, grammar: grammar, schema: grammar,
                ranker: WordRankers.Blend(0.5), wordsBeforeKeywords: true, keywordPriority: true, headKeywords: 12, shortWordsLast: shortLast, wordIndex: index);
        }

        private static string Answer(CompletionEngine engine, string text, int caret)
        {
            CompletionSite site = engine.Locate(text, caret);
            if (site == null) return "no site";
            return string.Join("|", engine.Suggest(text, site).Select(s => s.Kind + ":" + s.Text + ":" + s.Distance));
        }

        private const string Document =
            "const orders = 1, users = 2, userCount = 3;\n" +
            "const html = html`<div style=\"margin-top: 4px; color: ${tone}\" class=\"card-item\"><style>.card-item { padding-left: 2px; padding-top: 1px }</style></div>`;\n" +
            "const q1 = sql`select u.id, u.name, orders.total from users u join orders o on o.user_id = u.id where u.id = 1 group by u.name`;\n" +
            "const q2 = sql`select count(*) from orders where total > 10 order by total`;\n" +
            "const g = glsl`void main() { float intensity = 1.0; float inten2 = intensity * 2.0; }`;\n" +
            "const j = json`{ \"title\": 1, \"total\": 2, \"flag\": true }`;\n" +
            "function userNames(users) { return users.map(u => u.name); }\n";

        [Fact]
        public void The_engine_suggests_the_same_with_the_index_at_every_place_of_the_document()
        {
            CompletionEngine scanned = Engine(With(false)), indexed = Engine(With(true));
            int checkedPlaces = 0;
            for (int caret = 0; caret <= Document.Length; caret += 3)
            {
                Assert.Equal(Answer(scanned, Document, caret), Answer(indexed, Document, caret));
                checkedPlaces++;
            }
            Assert.True(checkedPlaces > 100);
        }

        [Fact]
        public void The_engine_suggests_the_same_with_each_feature_on_its_own()
        {
            foreach (CompletionFeatures[] pair in new[]
            {
                new[] { With(false, previous: false, sameLanguage: false, grammar: false, shortLast: false), With(true, previous: false, sameLanguage: false, grammar: false, shortLast: false) },
                new[] { With(false, previous: true, sameLanguage: false, grammar: false, shortLast: false), With(true, previous: true, sameLanguage: false, grammar: false, shortLast: false) },
                new[] { With(false, previous: false, sameLanguage: true, grammar: false, shortLast: false), With(true, previous: false, sameLanguage: true, grammar: false, shortLast: false) },
                new[] { With(false, previous: false, sameLanguage: false, grammar: true, shortLast: false), With(true, previous: false, sameLanguage: false, grammar: true, shortLast: false) },
                new[] { With(false, previous: false, sameLanguage: false, grammar: false, shortLast: true), With(true, previous: false, sameLanguage: false, grammar: false, shortLast: true) },
                new[] { new CompletionFeatures(), new CompletionFeatures(wordIndex: true) }
            })
            {
                CompletionEngine scanned = Engine(pair[0]), indexed = Engine(pair[1]);
                for (int caret = 1; caret <= Document.Length; caret += 5)
                    Assert.Equal(Answer(scanned, Document, caret), Answer(indexed, Document, caret));
            }
        }

        [Fact]
        public void The_engine_suggests_the_same_while_a_word_is_typed_letter_by_letter_in_the_middle_of_the_document()
        {
            // the index of each text is made from the one before: the way a person types
            const string tail = "const last = sql`select 1`;\nconst userLast = 1;\n";
            CompletionEngine scanned = Engine(With(false)), indexed = Engine(With(true));
            string typed = "";
            foreach (char c in "userCoun")
            {
                typed += c;
                string text = Document + "const x = sql`select " + typed + "`;\n" + tail;
                int caret = Document.Length + "const x = sql`select ".Length + typed.Length;
                Assert.Equal(Answer(scanned, text, caret), Answer(indexed, text, caret));
            }
            // and deleted again
            while (typed.Length > 1)
            {
                typed = typed.Substring(0, typed.Length - 1);
                string text = Document + "const x = sql`select " + typed + "`;\n" + tail;
                int caret = Document.Length + "const x = sql`select ".Length + typed.Length;
                Assert.Equal(Answer(scanned, text, caret), Answer(indexed, text, caret));
            }
        }

        [Fact]
        public void The_engine_suggests_the_same_after_random_edits_of_the_document()
        {
            var random = new Random(99);
            CompletionEngine scanned = Engine(With(false)), indexed = Engine(With(true));
            string text = Document;
            for (int step = 0; step < 150; step++)
            {
                int at = random.Next(0, text.Length + 1);
                text = random.Next(3) == 0 && text.Length > 10 ? text.Remove(Math.Min(at, text.Length - 1), 1) : text.Insert(at, "abcdefg_ ou\n`;9"[random.Next(15)].ToString());
                int caret = random.Next(0, text.Length + 1);
                Assert.Equal(Answer(scanned, text, caret), Answer(indexed, text, caret));
            }
        }

        [Fact]
        public void The_engine_suggests_the_same_in_a_text_larger_than_the_window_of_the_scan()
        {
            // beyond 500,000 characters on a side the scan reads only around the caret: the index is not used there, the result is the same
            var sb = new StringBuilder();
            for (int i = 0; sb.Length < 1200000; i++) sb.Append("const customer").Append(i % 977).Append(" = ").Append(i).Append(";\n");
            sb.Insert(600000, "sql`select cust|`;\n");
            string marked = sb.ToString();
            int caret = marked.IndexOf('|');
            string text = marked.Remove(caret, 1);
            Assert.Equal(Answer(Engine(With(false)), text, caret), Answer(Engine(With(true)), text, caret));
        }

        [Fact]
        public void A_text_that_fits_in_the_window_is_read_from_the_index_and_the_answer_is_the_same_as_for_the_scan()
        {
            var sb = new StringBuilder();
            for (int i = 0; sb.Length < 300000; i++) sb.Append("const customer").Append(i % 977).Append(" = ").Append(i).Append(";\n");
            sb.Insert(150000, "sql`select cust|`;\n");
            string marked = sb.ToString();
            int caret = marked.IndexOf('|');
            string text = marked.Remove(caret, 1);
            Assert.Equal(Answer(Engine(With(false)), text, caret), Answer(Engine(With(true)), text, caret));
        }

        [Fact]
        public void Requests_from_several_threads_on_the_texts_of_one_session_get_the_answers_of_each_text()
        {
            CompletionEngine scanned = Engine(With(false)), indexed = Engine(With(true));
            var texts = new List<string>();
            for (int i = 0; i < 6; i++) texts.Add(Document + "const x = sql`select user" + new string('a', i) + "`;\n");
            var expected = texts.Select(t => Answer(scanned, t, t.IndexOf("`;\n", t.IndexOf("select user", StringComparison.Ordinal) + 5, StringComparison.Ordinal))).ToList();

            var failures = new System.Collections.Concurrent.ConcurrentBag<string>();
            Parallel.For(0, 400, n =>
            {
                int t = n % texts.Count;
                string text = texts[t];
                int caret = text.IndexOf("`;\n", text.IndexOf("select user", StringComparison.Ordinal) + 5, StringComparison.Ordinal);
                string got = Answer(indexed, text, caret);
                if (got != expected[t]) failures.Add("text " + t);
            });
            Assert.Empty(failures);
        }

        [Fact]
        public void The_feature_is_off_unless_asked_for()
        {
            Assert.False(CompletionFeatures.None.WordIndex);
            Assert.False(new CompletionFeatures().WordIndex);
            Assert.True(new CompletionFeatures(wordIndex: true).WordIndex);
        }
    }
}
