using NestLight.Detection;
using Xunit;

namespace NestLight.Tests
{
    public class DetectorsMutationTests
    {
        private static int Score<T>(string text) where T : ILanguageDetector, new()
        {
            var d = new T();
            return d.Score(text, 0, text.Length);
        }

        // ---- SqlDetector: mutations at L18 (CanStartWith equality checks), L24 (logical on punctuation), L27 (negation), L29 (negate Score)

        [Theory]
        [InlineData('s', true)]
        [InlineData('S', true)]
        [InlineData('i', true)]
        [InlineData('I', true)]
        [InlineData('u', true)]
        [InlineData('U', true)]
        [InlineData('d', true)]
        [InlineData('D', true)]
        [InlineData('c', true)]
        [InlineData('C', true)]
        [InlineData('a', true)]
        [InlineData('A', true)]
        [InlineData('w', true)]
        [InlineData('W', true)]
        [InlineData('x', false)]
        [InlineData('f', false)]
        public void SqlDetector_CanStartWith(char first, bool expected)
        {
            Assert.Equal(expected, new SqlDetector().CanStartWith(first));
        }

        [Fact]
        public void SqlDetector_rejects_text_ending_with_period()
        {
            Assert.Equal(0, Score<SqlDetector>("select a from b."));
        }

        [Fact]
        public void SqlDetector_rejects_text_ending_with_question_mark()
        {
            Assert.Equal(0, Score<SqlDetector>("select a from b?"));
        }

        [Fact]
        public void SqlDetector_rejects_text_ending_with_exclamation()
        {
            Assert.Equal(0, Score<SqlDetector>("select a from b!"));
        }

        [Fact]
        public void SqlDetector_rejects_prose_with_article_after_verb()
        {
            Assert.Equal(0, Score<SqlDetector>("select an option from the list"));
            Assert.Equal(0, Score<SqlDetector>("delete your account from settings"));
        }

        [Fact]
        public void SqlDetector_scores_3_for_real_sql()
        {
            Assert.Equal(3, Score<SqlDetector>("select a from t where x = 1"));
        }

        [Fact]
        public void SqlDetector_scores_0_for_verb_without_follower()
        {
            Assert.Equal(0, Score<SqlDetector>("select something interesting"));
        }

        // ---- HtmlDetector: mutations at L57 (equality on length), L59 (equality on next char), L61 (logical, string mutations)

        [Theory]
        [InlineData('<', true)]
        [InlineData('a', false)]
        public void HtmlDetector_CanStartWith(char first, bool expected)
        {
            Assert.Equal(expected, new HtmlDetector().CanStartWith(first));
        }

        [Fact]
        public void HtmlDetector_rejects_text_shorter_than_3()
        {
            Assert.Equal(0, Score<HtmlDetector>("<>"));
        }

        [Fact]
        public void HtmlDetector_rejects_when_second_char_is_not_letter_or_bang_or_slash()
        {
            Assert.Equal(0, Score<HtmlDetector>("<1x>"));
        }

        [Fact]
        public void HtmlDetector_rejects_when_last_char_is_not_closing_bracket()
        {
            Assert.Equal(0, Score<HtmlDetector>("<div>x"));
        }

        [Fact]
        public void HtmlDetector_scores_3_with_closing_tag()
        {
            Assert.Equal(3, Score<HtmlDetector>("<div></div>"));
        }

        [Fact]
        public void HtmlDetector_scores_3_with_self_closing()
        {
            Assert.Equal(3, Score<HtmlDetector>("<img src=\"x\" />"));
        }

        [Fact]
        public void HtmlDetector_scores_2_without_closing_tag()
        {
            Assert.Equal(2, Score<HtmlDetector>("<div class=\"a\">"));
        }

        // ---- JsonDetector: mutations at L78 (equality checks for opening/closing chars)

        [Theory]
        [InlineData('{', true)]
        [InlineData('[', true)]
        [InlineData('a', false)]
        public void JsonDetector_CanStartWith(char first, bool expected)
        {
            Assert.Equal(expected, new JsonDetector().CanStartWith(first));
        }

        [Fact]
        public void JsonDetector_scores_3_for_object()
        {
            Assert.Equal(3, Score<JsonDetector>("{\"key\": \"value\"}"));
        }

        [Fact]
        public void JsonDetector_scores_0_for_mismatched_braces()
        {
            Assert.Equal(0, Score<JsonDetector>("{\"key\": 1]"));
        }

        [Fact]
        public void JsonDetector_scores_2_for_array_with_numbers()
        {
            Assert.Equal(2, Score<JsonDetector>("[1, 2, 3]"));
        }

        [Fact]
        public void JsonDetector_scores_2_for_array_with_objects()
        {
            Assert.Equal(2, Score<JsonDetector>("[{\"a\": 1}]"));
        }

        [Fact]
        public void JsonDetector_scores_2_for_array_with_negative_number()
        {
            Assert.Equal(2, Score<JsonDetector>("[-1, -2, -3]"));
        }

        [Fact]
        public void JsonDetector_scores_0_for_array_with_letter_first()
        {
            Assert.Equal(0, Score<JsonDetector>("[abc, def]"));
        }

        [Fact]
        public void JsonDetector_scores_3_for_empty_object()
        {
            Assert.Equal(3, Score<JsonDetector>("{}"));
        }

        [Fact]
        public void JsonDetector_scores_2_for_empty_array()
        {
            Assert.Equal(2, Score<JsonDetector>("[]"));
        }

        [Fact]
        public void JsonDetector_scores_2_for_nested_array()
        {
            Assert.Equal(2, Score<JsonDetector>("[[1, 2], [3]]"));
        }

        // ---- CssDetector: mutations at L93-L99

        [Fact]
        public void CssDetector_rejects_without_closing_brace()
        {
            Assert.Equal(0, Score<CssDetector>(".a { color: red; "));
        }

        [Fact]
        public void CssDetector_rejects_hash_followed_by_space()
        {
            Assert.Equal(0, Score<CssDetector>("# heading { color: red; }"));
        }

        [Fact]
        public void CssDetector_rejects_hash_followed_by_tab()
        {
            Assert.Equal(0, Score<CssDetector>("#\theading { color: red; }"));
        }

        [Fact]
        public void CssDetector_rejects_without_open_brace()
        {
            Assert.Equal(0, Score<CssDetector>("color: red; }"));
        }

        [Fact]
        public void CssDetector_rejects_without_colon()
        {
            Assert.Equal(0, Score<CssDetector>(".a { red; }"));
        }

        [Fact]
        public void CssDetector_scores_3_with_semicolon()
        {
            Assert.Equal(3, Score<CssDetector>(".a { color: red; }"));
        }

        [Fact]
        public void CssDetector_scores_2_without_semicolon()
        {
            Assert.Equal(2, Score<CssDetector>("a:hover { text-decoration: underline }"));
        }

        // ---- GraphQlDetector: mutations at L107 (CanStartWith), L118 (equality), L125-L127

        [Theory]
        [InlineData('q', true)]
        [InlineData('m', true)]
        [InlineData('s', true)]
        [InlineData('f', true)]
        [InlineData('{', true)]
        [InlineData('#', true)]
        [InlineData('a', false)]
        public void GraphQlDetector_CanStartWith(char first, bool expected)
        {
            Assert.Equal(expected, new GraphQlDetector().CanStartWith(first));
        }

        [Fact]
        public void GraphQlDetector_rejects_without_closing_brace()
        {
            Assert.Equal(0, Score<GraphQlDetector>("query Q { user { name "));
        }

        [Fact]
        public void GraphQlDetector_scores_2_for_anonymous_query()
        {
            Assert.Equal(2, Score<GraphQlDetector>("{ viewer { login } }"));
        }

        [Fact]
        public void GraphQlDetector_rejects_brace_then_non_letter()
        {
            Assert.Equal(0, Score<GraphQlDetector>("{ 123 }"));
        }

        [Fact]
        public void GraphQlDetector_rejects_multiple_top_level_objects()
        {
            Assert.Equal(0, Score<GraphQlDetector>("{ a } ; { b }"));
        }

        [Fact]
        public void GraphQlDetector_scores_3_for_named_query()
        {
            Assert.Equal(3, Score<GraphQlDetector>("query Users { user { name } }"));
        }

        [Fact]
        public void GraphQlDetector_scores_3_for_mutation()
        {
            Assert.Equal(3, Score<GraphQlDetector>("mutation Create { addUser { id } }"));
        }

        [Fact]
        public void GraphQlDetector_scores_3_for_subscription()
        {
            Assert.Equal(3, Score<GraphQlDetector>("subscription OnMsg { messageAdded { text } }"));
        }

        [Fact]
        public void GraphQlDetector_scores_3_for_fragment()
        {
            Assert.Equal(3, Score<GraphQlDetector>("fragment F on User { name email }"));
        }

        [Fact]
        public void GraphQlDetector_scores_0_for_keyword_without_brace()
        {
            Assert.Equal(0, Score<GraphQlDetector>("query without brace"));
        }

        // ---- LanguageDetector context: mutations at L168 (equality MinLength), L177 (equality bestScore)

        [Fact]
        public void Detector_rejects_text_shorter_than_MinLength()
        {
            Assert.Equal(8, LanguageDetector.MinLength);
            Assert.Null(LanguageDetector.Create(true).Detect("sel fm ", 0, 7));
            Assert.Null(LanguageDetector.Create(true).Detect("sel fm", 0, 6));
        }

        [Fact]
        public void Detector_trims_leading_and_trailing_whitespace()
        {
            Assert.Equal("sql", LanguageDetector.Create(true).Detect("  SELECT a FROM b WHERE c = 1  ", 0, 31));
        }

        [Fact]
        public void Detector_with_gate_off_still_detects()
        {
            Assert.Equal("sql", LanguageDetector.Create(false).Detect("SELECT a FROM b WHERE c = 1", 0, 27));
        }
    }
}
