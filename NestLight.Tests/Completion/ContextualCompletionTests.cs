using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The context-aware rankings of the completion, one feature at a time.</summary>
    public class ContextualCompletionTests
    {
        private static List<Suggestion> Run(string codeWithCaret, CompletionFeatures features, int maxItems = CompletionEngine.DefaultMaxItems)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), maxItems, CompletionEngine.DefaultMinWordLength,
                features: features);
            CompletionSite site = engine.Locate(code, caret);
            return site == null ? null : engine.Suggest(code, site).ToList();
        }

        private static List<string> Texts(List<Suggestion> items) { return items.Select(s => s.Text).ToList(); }

        private static readonly CompletionFeatures PreviousWord = new CompletionFeatures(previousWord: true);

        // ---- the previous word --------------------------------------------------------------------------------------------

        [Fact]
        public void The_word_that_followed_the_same_word_before_comes_first()
        {
            const string code = "const orders = 1, users = 2;\nsql`select a from orders; select b from users; select c from |`";
            List<string> without = Texts(Run(code, CompletionFeatures.None, 100000));
            List<string> with = Texts(Run(code, PreviousWord, 100000));

            Assert.Equal(new[] { "users", "orders" }, with.Take(2).ToArray());
            // the words of the document come after the keywords without the feature, and the feature does not drop anything
            Assert.True(without.IndexOf("users") >= Vocabularies.For("sql").Count);
            Assert.Equal(without.OrderBy(w => w), with.OrderBy(w => w));
        }

        [Fact]
        public void The_nearest_occurrence_of_the_context_wins_and_the_prefix_still_filters()
        {
            const string code = "sql`select a from users; select b from usage_log; select c from u|`";
            Assert.Equal(new[] { "usage_log", "users" }, Texts(Run(code, PreviousWord)).Take(2).ToArray());
            Assert.DoesNotContain("orders", Texts(Run("sql`select a from orders; select c from u|`", PreviousWord)));
        }

        [Fact]
        public void The_punctuation_between_the_words_is_part_of_the_context()
        {
            const string code = "sql`select u.email, v email from users u, vendors v; select u.|`";
            List<string> with = Texts(Run(code, PreviousWord));
            Assert.Equal("email", with[0]);

            // 'v email' (a blank) is another context than 'u.email': after "v " the word 'email' is not promoted by 'u.'
            const string other = "sql`select u.email, v nickname from users u, vendors v; select v |`";
            Assert.Equal("nickname", Texts(Run(other, PreviousWord))[0]);
            Assert.NotEqual("email", Texts(Run("sql`select u.email, v nickname from users u, vendors v; select u |`", PreviousWord))[0]);
        }

        [Fact]
        public void The_value_that_followed_a_property_is_offered_after_it()
        {
            const string code = "css`.a { display: block; } .b { display: flex; color: red; } .c { display: |}`";
            List<string> with = Texts(Run(code, PreviousWord));
            Assert.Equal(new[] { "flex", "block" }, with.Take(2).ToArray());
            Assert.DoesNotContain("red", with.Take(2));
        }

        [Fact]
        public void A_keyword_that_followed_the_word_keeps_the_spelling_of_the_vocabulary_and_is_a_keyword()
        {
            const string code = "sql`SELECT a FROM t GROUP BY a; select b from t group |`";
            List<Suggestion> with = Run(code, PreviousWord);
            Assert.Equal("by", with[0].Text);
            Assert.Equal(SuggestionKind.Keyword, with[0].Kind);

            List<Suggestion> upper = Run("sql`SELECT a FROM t GROUP BY a; SELECT b FROM t GROUP B|`", PreviousWord);
            Assert.Equal("BY", upper[0].Text);
        }

        [Fact]
        public void Each_word_is_offered_once_even_when_it_follows_the_context_several_times()
        {
            const string code = "sql`select a from users; select b from users; select c from users; select d from |`";
            Assert.Equal(1, Texts(Run(code, PreviousWord)).Count(w => w == "users"));
        }

        [Fact]
        public void Without_a_word_before_the_caret_or_with_far_punctuation_the_ranking_is_the_default()
        {
            const string atStart = "const users = 1;\nsql`|`";
            Assert.Equal(Texts(Run(atStart, CompletionFeatures.None)), Texts(Run(atStart, PreviousWord)));

            const string farPunctuation = "const users = 1;\nsql`select a from users; select b ((((|`";
            Assert.Equal(Texts(Run(farPunctuation, CompletionFeatures.None)), Texts(Run(farPunctuation, PreviousWord)));
        }

        [Fact]
        public void The_feature_does_not_change_the_suggestions_it_only_reorders_them()
        {
            const string code = "const orders = 1, users = 2;\nsql`select a from orders; select b from users; select c from o|`";
            Assert.Equal(Texts(Run(code, CompletionFeatures.None)).OrderBy(w => w), Texts(Run(code, PreviousWord)).OrderBy(w => w));
        }

        [Fact]
        public void The_limit_of_items_still_holds_with_the_feature()
        {
            const string code = "sql`select a from users; select b from orders; select c from |`";
            Assert.Equal(3, Run(code, PreviousWord, maxItems: 3).Count);
            Assert.Equal(new[] { "orders", "users" }, Texts(Run(code, PreviousWord, maxItems: 3)).Take(2).ToArray());
        }

        // ---- the words of the language of the string ----------------------------------------------------------------------

        private static readonly CompletionFeatures Scope = new CompletionFeatures(sameLanguageWords: true);
        private static readonly CompletionFeatures Both = new CompletionFeatures(previousWord: true, sameLanguageWords: true);

        private static List<string> Words(string code, CompletionFeatures features)
        {
            return Run(code, features, 100000).Where(s => s.Kind == SuggestionKind.Word).Select(s => s.Text).ToList();
        }

        [Fact]
        public void A_word_of_another_sql_string_comes_before_a_nearer_variable_of_the_host()
        {
            const string code = "sql`select a from customers`;\nconst customerId = 1;\nsql`select cu|`";
            Assert.Equal(new[] { "customerId", "customers" }, Words(code, CompletionFeatures.None).ToArray());
            Assert.Equal(new[] { "customers", "customerId" }, Words(code, Scope).ToArray());
        }

        [Fact]
        public void An_interpolation_is_host_code_and_not_the_language_of_the_string()
        {
            const string code = "sql`select customers from ${customerExpr}; select cu|`";
            Assert.Equal(new[] { "customerExpr", "customers" }, Words(code, CompletionFeatures.None).ToArray());
            Assert.Equal(new[] { "customers", "customerExpr" }, Words(code, Scope).ToArray());
        }

        [Fact]
        public void A_string_of_another_language_is_not_the_language_of_the_caret()
        {
            const string code = "sql`select customers`;\ncss`.customer { color: red }`;\nsql`select cu|`";
            Assert.Equal(new[] { "customer", "customers" }, Words(code, CompletionFeatures.None).ToArray());
            Assert.Equal(new[] { "customers", "customer" }, Words(code, Scope).ToArray());
        }

        [Fact]
        public void Aliases_of_a_language_are_the_same_language()
        {
            Assert.True(Vocabularies.SameLanguage("html", "svg"));
            Assert.True(Vocabularies.SameLanguage("YAML", "yml"));
            Assert.True(Vocabularies.SameLanguage("xml", "XML"));
            Assert.False(Vocabularies.SameLanguage("sql", "css"));
            Assert.False(Vocabularies.SameLanguage(null, "sql"));

            const string code = "svg`<g class='shape'>`;\nconst shapeId = 1;\nhtml`<div class='sh|'>`";
            Assert.Equal(new[] { "shape", "shapeId" }, Words(code, Scope).ToArray());
        }

        [Fact]
        public void The_scope_only_reorders_what_is_offered()
        {
            const string code = "sql`select a from customers`;\nconst customerId = 1;\ncss`.customer {}`;\nsql`select cu|`";
            Assert.Equal(Words(code, CompletionFeatures.None).OrderBy(w => w), Words(code, Scope).OrderBy(w => w));
        }

        [Fact]
        public void The_context_of_the_previous_word_is_taken_from_the_code_of_the_language_only()
        {
            const string code = "sql`select a from users`;\n// import x from vendors\nsql`select b from |`";
            Assert.Equal("vendors", Texts(Run(code, PreviousWord))[0]);
            Assert.Equal("users", Texts(Run(code, Both))[0]);
        }

        [Fact]
        public void With_the_scope_many_strings_and_interpolations_are_handled()
        {
            string code = string.Concat(Enumerable.Range(0, 300).Select(i => "const v" + i + " = sql`select col" + i + " from ${t" + i + "} where x = 1`;\n")) + "sql`select co|`";
            List<string> words = Words(code, Scope);
            Assert.Equal(300, words.Count(w => w.StartsWith("col")));
            Assert.DoesNotContain("t5", words);
        }
    }
}
