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
    }
}
