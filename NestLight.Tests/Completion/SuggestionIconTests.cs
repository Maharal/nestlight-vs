using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The icon of each suggestion in the list: what the engine says it is decides which icon it gets.</summary>
    public class SuggestionIconTests
    {
        [Fact]
        public void A_keyword_has_the_keyword_icon_and_a_word_of_the_document_the_word_icon()
        {
            Assert.Equal(SuggestionIcon.Keyword, SuggestionIcons.Of(new Suggestion("select", SuggestionKind.Keyword)));
            Assert.Equal(SuggestionIcon.Word, SuggestionIcons.Of(new Suggestion("orders", SuggestionKind.Word)));
        }

        [Fact]
        public void Only_a_suggestion_that_is_not_a_prefix_of_what_was_typed_is_marked_as_similar()
        {
            Assert.False(SuggestionIcons.IsSimilar(new Suggestion("select", SuggestionKind.Keyword)));
            Assert.True(SuggestionIcons.IsSimilar(new Suggestion("select", SuggestionKind.Keyword, 1)));
            Assert.True(SuggestionIcons.IsSimilar(new Suggestion("orders", SuggestionKind.Word, 2)));
        }

        private static List<Suggestion> Items(string codeWithCaret)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), matcher: new BandedPrefixMatcher(), features: CompletionFeatures.Default);
            return engine.Suggest(code, engine.Locate(code, caret)).ToList();
        }

        [Fact]
        public void The_suggestions_of_the_engine_get_the_icon_of_what_they_are()
        {
            List<Suggestion> items = Items("const orders_total = 1;\nsql`select a from orders where or|`");
            Suggestion keyword = items.First(s => s.Text.ToLowerInvariant() == "order"), word = items.First(s => s.Text == "orders_total");
            Assert.Equal(SuggestionIcon.Keyword, SuggestionIcons.Of(keyword));
            Assert.Equal(SuggestionIcon.Word, SuggestionIcons.Of(word));
            Assert.All(items, s => Assert.False(SuggestionIcons.IsSimilar(s))); // everything here starts with what was typed
        }

        [Fact]
        public void A_misspelled_word_gets_similar_suggestions_with_the_small_icon()
        {
            List<Suggestion> items = Items("sql`selct|`");
            Assert.NotEmpty(items);
            Suggestion select = items.First(s => s.Text.ToLowerInvariant() == "select");
            Assert.True(SuggestionIcons.IsSimilar(select));
            Assert.Equal(SuggestionIcon.Keyword, SuggestionIcons.Of(select)); // still a keyword: the small icon is added to the main one
        }
    }
}
