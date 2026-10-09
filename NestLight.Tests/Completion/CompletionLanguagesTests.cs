using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The strategy of each embedded language and the registry that finds it.</summary>
    public class CompletionLanguagesTests
    {
        private static readonly ICompletionLanguages Languages = CompletionLanguages.Default;

        [Fact]
        public void Every_language_with_a_tokenizer_has_a_completion_strategy_with_the_same_ids()
        {
            var tokenizers = NestLightComposition.CreateEmbeddedLanguages();
            var registry = (EmbeddedLanguageRegistry)tokenizers;
            var tokenizerIds = registry.Ids.OrderBy(i => i).ToList();
            var completionIds = CompletionLanguages.Default.Ids.OrderBy(i => i).ToList();
            Assert.Equal(tokenizerIds, completionIds);
        }

        [Theory]
        [InlineData("html", typeof(HtmlCompletion))]
        [InlineData("svg", typeof(HtmlCompletion))]
        [InlineData("htm", typeof(HtmlCompletion))]
        [InlineData("css", typeof(CssCompletion))]
        [InlineData("SQL", typeof(SqlCompletion))]
        [InlineData("gql", typeof(GraphQlCompletion))]
        [InlineData("graphql", typeof(GraphQlCompletion))]
        [InlineData("json", typeof(JsonCompletion))]
        [InlineData("yml", typeof(YamlCompletion))]
        [InlineData("yaml", typeof(YamlCompletion))]
        [InlineData("glsl", typeof(GlslCompletion))]
        [InlineData("wgsl", typeof(WgslCompletion))]
        [InlineData("xml", typeof(XmlCompletion))]
        [InlineData("md", typeof(MarkdownCompletion))]
        [InlineData("markdown", typeof(MarkdownCompletion))]
        [InlineData("regex", typeof(RegexCompletion))]
        [InlineData("regexp", typeof(RegexCompletion))]
        public void An_id_finds_the_strategy_of_its_language(string id, Type strategy)
        {
            Assert.IsType(strategy, Languages.Find(id));
        }

        [Fact]
        public void An_unknown_id_finds_a_language_with_nothing_special_and_never_null()
        {
            foreach (string id in new[] { "klingon", "", null })
            {
                ICompletionLanguage language = Languages.Find(id);
                Assert.NotNull(language);
                Assert.Empty(language.Keywords);
                Assert.Empty(language.CompletionWords);
                Assert.False(language.IsExtraWordChar('-'));
                Assert.False(language.KeywordsFollowTypedCase);
                Assert.Null(language.PositionAt("abc", new CompletionSite("klingon", 0, 1, 1)));
            }
        }

        [Fact]
        public void Aliases_are_one_language()
        {
            Assert.True(Languages.Same("html", "svg"));
            Assert.True(Languages.Same("yaml", "YML"));
            Assert.True(Languages.Same("md", "markdown"));
            Assert.True(Languages.Same("regex", "regexp"));
            Assert.True(Languages.Same("gql", "graphql"));
            Assert.True(Languages.Same("klingon", "KLINGON"));
            Assert.False(Languages.Same("html", "css"));
            Assert.False(Languages.Same("xml", "html"));
            Assert.False(Languages.Same("klingon", "vulcan"));
        }

        [Fact]
        public void The_same_id_in_another_case_is_the_same_language_object()
        {
            Assert.Same(Languages.Find("css"), Languages.Find("CSS"));
            Assert.Same(Languages.Find("html"), Languages.Find("svg"));
        }

        [Fact]
        public void Registering_an_id_twice_is_refused()
        {
            Assert.Throws<InvalidOperationException>(() => new CompletionLanguages(new ICompletionLanguage[] { new JsonCompletion(), new JsonCompletion() }));
            Assert.Throws<ArgumentNullException>(() => new CompletionLanguages(null));
        }

        [Fact]
        public void The_vocabulary_is_sorted_and_the_same_list_every_time()
        {
            foreach (string id in new[] { "sql", "css", "html", "graphql", "json", "yaml", "glsl", "wgsl" })
            {
                ICompletionLanguage language = Languages.Find(id);
                Assert.NotEmpty(language.Keywords);
                Assert.Same(language.Keywords, language.Keywords);
                Assert.Same(language.CompletionWords, language.CompletionWords);
                Assert.Equal(language.Keywords.OrderBy(w => w, StringComparer.OrdinalIgnoreCase).ToList(), language.Keywords);
                Assert.Equal(language.Keywords.Count, language.Keywords.Distinct().Count());
            }
        }

        [Fact]
        public void The_words_the_tokenizer_does_not_color_are_offered_but_are_not_keywords()
        {
            ICompletionLanguage glsl = Languages.Find("glsl");
            Assert.DoesNotContain("main", glsl.Keywords);
            Assert.Contains("main", glsl.CompletionWords);
            Assert.Contains("gl_Position", glsl.CompletionWords);
            Assert.Null(glsl.FindKeyword("main"));
            Assert.Equal("main", glsl.FindCompletionWord("MAIN"));
            Assert.Equal("vec3", glsl.FindCompletionWord("VEC3"));
            Assert.Same(Languages.Find("sql").Keywords, Languages.Find("sql").CompletionWords);
        }

        [Fact]
        public void A_keyword_is_found_in_the_spelling_of_the_vocabulary_whatever_the_case()
        {
            Assert.Equal("select", Languages.Find("sql").FindKeyword("SeLeCt"));
            Assert.Null(Languages.Find("sql").FindKeyword("selec"));
            Assert.Equal("Int", Languages.Find("graphql").FindKeyword("INT"));
        }

        [Fact]
        public void Only_sql_follows_the_case_typed()
        {
            foreach (string id in new[] { "css", "html", "graphql", "json", "yaml", "glsl", "wgsl", "xml", "markdown", "regex" })
                Assert.False(Languages.Find(id).KeywordsFollowTypedCase, id);
            Assert.True(Languages.Find("sql").KeywordsFollowTypedCase);
        }

        [Fact]
        public void A_dash_is_part_of_a_word_in_html_css_and_yaml_only()
        {
            foreach (string id in new[] { "css", "html", "svg", "yaml", "yml" }) Assert.True(Languages.Find(id).IsExtraWordChar('-'), id);
            foreach (string id in new[] { "sql", "graphql", "json", "glsl", "wgsl", "xml", "markdown", "regex" }) Assert.False(Languages.Find(id).IsExtraWordChar('-'), id);
            Assert.False(Languages.Find("css").IsExtraWordChar('_'));
            Assert.False(Languages.Find("css").IsExtraWordChar('.'));
        }

        [Fact]
        public void The_languages_with_a_grammar_and_the_ones_without()
        {
            // a place the grammar knows: after FROM in SQL, a value in CSS, a tag in HTML, a key in JSON, a key in YAML, a type in a shader
            Assert.NotNull(PositionOf("sql", "select * from "));
            Assert.NotNull(PositionOf("css", ".a { display: "));
            Assert.NotNull(PositionOf("html", "<div "));
            Assert.NotNull(PositionOf("json", "{\"na")); // inside a key
            // languages with no grammar say nothing about the place
            Assert.Null(PositionOf("graphql", "query { "));
            Assert.Null(PositionOf("xml", "<a "));
            Assert.Null(PositionOf("markdown", "# "));
            Assert.Null(PositionOf("regex", "^("));
        }

        private static Position PositionOf(string id, string beforeCaret)
        {
            var site = new CompletionSite(id, beforeCaret.Length, beforeCaret.Length, beforeCaret.Length, 0, beforeCaret.Length);
            return Languages.Find(id).PositionAt(beforeCaret, site);
        }

        [Fact]
        public void A_position_is_read_only_inside_the_text()
        {
            Assert.Null(Languages.Find("sql").PositionAt(null, new CompletionSite("sql", 0, 0, 0)));
            Assert.Null(Languages.Find("sql").PositionAt("abc", null));
            Assert.Null(Languages.Find("sql").PositionAt("abc", new CompletionSite("sql", 99, 99, 99)));
        }

        [Fact]
        public void Only_html_nests_another_language_and_only_sql_has_a_schema()
        {
            foreach (string id in new[] { "sql", "css", "graphql", "json", "yaml", "glsl", "wgsl", "xml", "markdown", "regex" })
                Assert.False(Languages.Find(id) is INestedLanguages, id);
            Assert.True(Languages.Find("html") is INestedLanguages);
            foreach (string id in new[] { "css", "html", "graphql", "json", "yaml", "glsl", "wgsl", "xml", "markdown", "regex" })
                Assert.False(Languages.Find(id) is ISchemaCompletion, id);
            Assert.True(Languages.Find("sql") is ISchemaCompletion);
        }

        [Fact]
        public void Html_finds_the_css_of_a_style_element_and_of_a_style_attribute()
        {
            const string code = "html`<div style=\"color: red\"><style>.a { margin: 0 }</style></div>`";
            EmbeddedString owner = Pipeline.Scanner(HostLanguage.JavaScript).Scan(code).Single();
            var html = (INestedLanguages)Languages.Find("html");

            IReadOnlyList<NestedRegion> regions = html.RegionsIn(code, owner);
            Assert.Equal(2, regions.Count);
            Assert.All(regions, r => Assert.Equal("css", r.EmbeddedLanguageId));
            Assert.True(regions[0].InlineDeclarations);   // the attribute
            Assert.False(regions[1].InlineDeclarations);  // the element

            NestedRegion at;
            Assert.True(html.TryRegionAt(code, owner, code.IndexOf("red"), out at));
            Assert.Equal(regions[0].Start, at.Start);
            Assert.False(html.TryRegionAt(code, owner, code.IndexOf("<div") + 2, out at));
        }

        [Fact]
        public void Css_corrects_the_start_of_a_word_that_follows_a_number()
        {
            ICompletionLanguage css = Languages.Find("css");
            // 10px: the unit starts after the digits
            string text = "a { width: 10p";
            int start = text.IndexOf("10p");
            Assert.True(css.TryAdjustWordStart(text, 0, ref start, text.Length));
            Assert.Equal(text.IndexOf("10p") + 2, start);
            // a hex color is not a number followed by a unit
            text = "a { color: #1a2b3c";
            start = text.IndexOf("1a2b3c");
            Assert.False(css.TryAdjustWordStart(text, 0, ref start, text.Length));
            // a plain word is left alone
            text = "a { color: re";
            start = text.IndexOf("re");
            Assert.True(css.TryAdjustWordStart(text, 0, ref start, text.Length));
            Assert.Equal(text.IndexOf("re"), start);
        }

        [Fact]
        public void The_other_languages_do_not_touch_the_start_of_a_word()
        {
            foreach (string id in new[] { "sql", "html", "graphql", "json", "yaml", "glsl", "wgsl", "xml", "markdown", "regex" })
            {
                string text = "10p";
                int start = 0;
                Assert.True(Languages.Find(id).TryAdjustWordStart(text, 0, ref start, text.Length), id);
                Assert.Equal(0, start);
            }
        }

        [Fact]
        public void The_engine_uses_the_languages_it_is_given()
        {
            // a language that offers one word and nothing else, for the id "toy"
            var toy = new ToyCompletion();
            var registry = new CompletionLanguages(new ICompletionLanguage[] { toy });
            var engine = new CompletionEngine(new FixedScanner("toy", 0, 4), languages: registry);
            string text = "xx-pl";
            CompletionSite site = engine.Locate(text, text.Length);
            Assert.NotNull(site);
            Assert.Equal("xx-pl".Length, site.PrefixLength); // the toy language's words contain dashes
            Assert.Contains("xx-plugin", engine.Suggest(text, site).Select(s => s.Text));
        }

        private sealed class ToyCompletion : CompletionLanguage
        {
            public ToyCompletion() : base(new[] { "toy" }, new[] { "xx-plugin", "xx-play" }) { }
            public override bool IsExtraWordChar(char c) { return c == '-'; }
        }

        private sealed class FixedScanner : IHostScanner
        {
            private readonly string _id; private readonly int _start, _end;
            public FixedScanner(string id, int start, int end) { _id = id; _start = start; _end = end; }
            public IReadOnlyList<EmbeddedString> Scan(string text)
            {
                var s = new EmbeddedString(_id) { OuterStart = _start, Start = _start, End = text.Length, OuterEnd = text.Length };
                return new[] { s };
            }
        }

        [Fact]
        public void Words_of_a_string_in_an_alias_count_as_words_of_the_same_language()
        {
            // md and markdown name one language: a word typed in the first string is offered in the second
            const string code = "const a = md`# Changelog`;\nconst b = markdown`# Chan`;";
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100, 3, features: CompletionFeatures.Default);
            int caret = code.IndexOf("Chan`") + 4;
            CompletionSite site = engine.Locate(code, caret);
            Assert.NotNull(site);
            Assert.Contains("Changelog", engine.Suggest(code, site).Select(s => s.Text));
        }
    }
}
