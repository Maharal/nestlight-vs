using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The engine against fakes: it decodes, masks and maps back, whatever the host and the language are.</summary>
    public class HighlightEngineTests
    {
        private sealed class FakeScanner : IHostScanner
        {
            private readonly EmbeddedString[] _strings;
            public FakeScanner(params EmbeddedString[] strings) { _strings = strings; }
            public IReadOnlyList<EmbeddedString> Scan(string text) { return _strings; }
        }

        private sealed class FakeTokenizer : ILanguageTokenizer
        {
            public readonly List<string> Received = new List<string>();
            public Action<char[], int, int, TokenSink> Behavior;
            public IReadOnlyList<string> Ids { get { return new[] { "fake" }; } }

            public void Tokenize(char[] text, int from, int to, TokenSink emit)
            {
                Received.Add(new string(text, from, to - from));
                if (Behavior != null) Behavior(text, from, to, emit);
            }
        }

        private sealed class FakeRegistry : ILanguageRegistry
        {
            private readonly ILanguageTokenizer _tokenizer;
            public FakeRegistry(ILanguageTokenizer tokenizer) { _tokenizer = tokenizer; }
            public bool IsKnown(string id) { return id == "fake"; }
            public ILanguageTokenizer Find(string id) { return id == "fake" ? _tokenizer : null; }
        }

        private static EmbeddedString Fake(int start, int end)
        {
            return new EmbeddedString("fake") { OuterStart = start - 1, Start = start, End = end, OuterEnd = end + 1 };
        }

        private static IReadOnlyList<Token> Run(string text, EmbeddedString s, FakeTokenizer tokenizer)
        {
            return new HighlightEngine(new FakeScanner(s), new FakeRegistry(tokenizer)).Highlight(text);
        }

        // ---- what the tokenizer receives ---------------------------------------------------------------------

        [Fact]
        public void The_tokenizer_receives_the_content_with_interpolations_masked_and_escapes_decoded()
        {
            string text = "'a\"\"b{x}c'";
            var s = Fake(1, text.Length - 1);
            s.Escapes.Add(new EscapeSequence(2, 2, '"'));
            s.Interpolations.Add(new Interpolation(5, 8, 1, 1));
            var tokenizer = new FakeTokenizer();

            Run(text, s, tokenizer);

            Assert.Equal(new[] { "a\"b" + new string(TextUtil.Mask, 3) + "c" }, tokenizer.Received.ToArray());
        }

        [Fact]
        public void The_tokenizer_does_not_run_on_empty_content()
        {
            var tokenizer = new FakeTokenizer();
            Run("''", Fake(1, 1), tokenizer);
            Assert.Empty(tokenizer.Received);
        }

        [Fact]
        public void Strings_of_unknown_languages_are_skipped()
        {
            var tokenizer = new FakeTokenizer();
            var s = new EmbeddedString("banana") { Start = 1, End = 3, OuterStart = 0, OuterEnd = 4 };
            Assert.Empty(Run("'ab'", s, tokenizer));
            Assert.Empty(tokenizer.Received);
        }

        // ---- what comes back -------------------------------------------------------------------------------

        [Fact]
        public void Tokens_are_mapped_back_to_the_source_and_clipped_around_interpolations()
        {
            string text = "'a\"\"b{x}c'";
            var s = Fake(1, text.Length - 1);
            s.Escapes.Add(new EscapeSequence(2, 2, '"'));
            s.Interpolations.Add(new Interpolation(5, 8, 1, 1));
            var tokenizer = new FakeTokenizer { Behavior = (t, from, to, emit) => emit(from, to, "t") };

            var tokens = Run(text, s, tokenizer).Where(t => t.Type == "t").ToList();

            // decoded "a" + '"' + "b" cover source [1,6) minus nothing; the hole [5,8) is cut out; "c" is [8,9)
            Assert.Equal(new[] { Tuple.Create(1, 4), Tuple.Create(8, 1) }, tokens.Select(t => Tuple.Create(t.Start, t.Length)).ToArray());
        }

        [Fact]
        public void A_token_over_an_escape_covers_the_whole_escape_sequence()
        {
            string text = "'a\"\"b'";
            var s = Fake(1, text.Length - 1);
            s.Escapes.Add(new EscapeSequence(2, 2, '"'));
            var tokenizer = new FakeTokenizer { Behavior = (t, from, to, emit) => emit(1, 2, "q") };

            var token = Run(text, s, tokenizer).Single(t => t.Type == "q");

            Assert.Equal("\"\"", text.Substring(token.Start, token.Length));
        }

        [Fact]
        public void Out_of_range_tokens_from_a_tokenizer_are_clamped()
        {
            string text = "'abc'";
            var tokenizer = new FakeTokenizer { Behavior = (t, from, to, emit) => { emit(-5, 99, "t"); emit(7, 9, "far"); emit(2, 2, "empty"); } };

            var tokens = Run(text, Fake(1, 4), tokenizer);

            Assert.Equal(new[] { Tuple.Create(1, 3) }, tokens.Select(t => Tuple.Create(t.Start, t.Length)).ToArray());
        }

        [Fact]
        public void Interpolations_yield_delimiters_and_a_neutral_expression()
        {
            string text = "'a{xy}b'";
            var s = Fake(1, text.Length - 1);
            s.Interpolations.Add(new Interpolation(2, 6, 1, 1));

            var tokens = Run(text, s, new FakeTokenizer());

            Assert.Equal(new[]
            {
                ClassificationNames.ExprDelimiter + "|{",
                ClassificationNames.Expression + "|xy",
                ClassificationNames.ExprDelimiter + "|}"
            }, tokens.Select(t => t.Type + "|" + text.Substring(t.Start, t.Length)).ToArray());
        }

        [Fact]
        public void Unclosed_interpolation_has_only_an_opening_delimiter()
        {
            string text = "'a{xy";
            var s = Fake(1, text.Length);
            s.Interpolations.Add(new Interpolation(2, text.Length, 1, 0));

            var tokens = Run(text, s, new FakeTokenizer());

            Assert.Equal(new[] { "{", "xy" }, tokens.Select(t => text.Substring(t.Start, t.Length)).ToArray());
        }

        [Fact]
        public void Tokens_are_sorted_by_position()
        {
            string text = "'ab{x}'";
            var s = Fake(1, text.Length - 1);
            s.Interpolations.Add(new Interpolation(3, 6, 1, 1));
            var tokenizer = new FakeTokenizer { Behavior = (t, from, to, emit) => emit(0, 2, "t") };

            var tokens = Run(text, s, tokenizer);

            Assert.Equal(tokens.OrderBy(t => t.Start).Select(t => t.Start).ToArray(), tokens.Select(t => t.Start).ToArray());
        }

        // ---- real hosts and languages ------------------------------------------------------------------

        [Fact]
        public void Csharp_interpolation_delimiters_follow_the_dollar_count()
        {
            string single = "// html\nvar a = $\"<b>{name}</b>\";";
            string raw = "// html\nvar a = $$\"\"\"<b>{{name}}</b>\"\"\";";

            Assert.Equal(new[] { "{", "}" }, Lexer.Texts(HostLanguage.CSharp, single, ClassificationNames.ExprDelimiter));
            Assert.Equal(new[] { "{{", "}}" }, Lexer.Texts(HostLanguage.CSharp, raw, ClassificationNames.ExprDelimiter));
            Assert.Equal(new[] { "name" }, Lexer.Texts(HostLanguage.CSharp, raw, ClassificationNames.Expression));
        }

        [Fact]
        public void Python_replacement_fields_are_interpolations()
        {
            string code = "# html\nx = f'<b>{name!r}</b>'";
            Assert.Equal(new[] { "{", "}" }, Lexer.Texts(HostLanguage.Python, code, ClassificationNames.ExprDelimiter));
            Assert.Equal(new[] { "name!r" }, Lexer.Texts(HostLanguage.Python, code, ClassificationNames.Expression));
        }

        [Fact]
        public void Cpp_raw_strings_are_highlighted_without_interpolations()
        {
            string code = "// html\nauto a = R\"(<b class=\"x\">{y}</b>)\";";
            Assert.Equal(new[] { "b", "b" }, Lexer.Texts(HostLanguage.Cpp, code, ClassificationNames.Tag));
            Assert.Empty(Lexer.Texts(HostLanguage.Cpp, code, ClassificationNames.Expression));
        }

        [Fact]
        public void Csharp_doubled_quotes_are_one_character_for_the_embedded_language()
        {
            // CSS sees  a { content: "x" }  so the string token is the quotes with the x, doubled quotes included
            string code = "// css\nvar a = @\"a { content: \"\"x\"\" }\";";
            Assert.Equal(new[] { "\"\"x\"\"" }, Lexer.Texts(HostLanguage.CSharp, code, ClassificationNames.CssString));
        }

        [Fact]
        public void Csharp_backslash_quotes_are_decoded_too()
        {
            string code = "// css\nvar a = \"a { content: \\\"x\\\" }\";";
            Assert.Equal(new[] { "\\\"x\\\"" }, Lexer.Texts(HostLanguage.CSharp, code, ClassificationNames.CssString));
        }

        [Fact]
        public void Doubled_braces_are_one_brace_for_the_embedded_language()
        {
            string csharp = "// css\nvar a = $\"a {{ color: red }}\";";
            string python = "# css\nx = f'a {{ color: red }}'";

            foreach (var tokens in new[] { Lexer.Lex(HostLanguage.CSharp, csharp), Lexer.Lex(HostLanguage.Python, python) })
            {
                Assert.Equal(new[] { "{{", ":", "}}" }, Lexer.Texts(tokens, ClassificationNames.CssPunct));
                Assert.Equal(new[] { "color" }, Lexer.Texts(tokens, ClassificationNames.CssProperty));
                Assert.Equal(new[] { "red" }, Lexer.Texts(tokens, ClassificationNames.CssValue));
            }
        }

        [Fact]
        public void Python_backslash_pairs_are_one_character_for_the_embedded_language()
        {
            // "\\d+" in the source is \d+ for the regex language: one escape token that covers three source characters
            string code = "# regex\nx = \"\\\\d+\"";
            Assert.Equal(new[] { "\\\\d" }, Lexer.Texts(HostLanguage.Python, code, ClassificationNames.RegexEscape));
        }

        [Fact]
        public void Python_raw_strings_keep_backslashes_as_written()
        {
            string code = "# regex\nx = r\"\\d+\"";
            Assert.Equal(new[] { "\\d" }, Lexer.Texts(HostLanguage.Python, code, ClassificationNames.RegexEscape));
        }

        // ---- nesting ------------------------------------------------------------------------------------------

        [Fact]
        public void Templates_nested_in_interpolations_are_highlighted_at_every_level_in_javascript()
        {
            string code = "html`<a>${ html`<b>${ css`c { d: e }` }</b>` }</a>`";
            var toks = Lexer.Lex(code);
            Assert.Equal(new[] { "a", "a", "b", "b" }, Lexer.Texts(toks, ClassificationNames.Tag).OrderBy(x => x).ToArray());
            Assert.Equal(new[] { "d" }, Lexer.Texts(toks, ClassificationNames.CssProperty));
            Assert.False(Lexer.HasOverlap(toks));
        }

        [Fact]
        public void Templates_nested_in_interpolations_are_highlighted_in_csharp()
        {
            string code = "// html\nvar a = $\"<ul>{ Join(items.Select(i => /* html */ $\"<li>{i}</li>\")) }</ul>\";";
            var toks = Lexer.Lex(HostLanguage.CSharp, code);
            Assert.Equal(new[] { "li", "li", "ul", "ul" }, Lexer.Texts(toks, ClassificationNames.Tag).OrderBy(x => x).ToArray());
            Assert.False(Lexer.HasOverlap(toks));
        }

        [Fact]
        public void Templates_nested_in_interpolations_are_highlighted_in_python()
        {
            string code = "# html\nx = f\"\"\"<ul>{ ''.join(\n  # html\n  f'<li>{i}</li>' for i in xs) }</ul>\"\"\"";
            var toks = Lexer.Lex(HostLanguage.Python, code);
            Assert.Equal(new[] { "li", "li", "ul", "ul" }, Lexer.Texts(toks, ClassificationNames.Tag).OrderBy(x => x).ToArray());
            Assert.False(Lexer.HasOverlap(toks));
        }

        [Fact]
        public void Expression_text_leaves_a_hole_where_a_nested_string_lives()
        {
            string code = "html`${ a ? html`<i></i>` : b }`";
            var expression = Lexer.Texts(code, ClassificationNames.Expression);
            // the tag (html) is host code: it stays in the expression, only the backtick string is a hole
            Assert.Equal(new[] { " a ? html", " : b " }, expression);
        }

        [Theory]
        [InlineData(HostLanguage.JavaScript, "html`<b>${zz}</b>`")]
        [InlineData(HostLanguage.CSharp, "// html\nvar a = $\"<b>{zz}</b>\";")]
        [InlineData(HostLanguage.Python, "# html\nx = f'<b>{zz}</b>'")]
        public void Interpolations_are_never_colored_as_embedded_code(HostLanguage host, string code)
        {
            foreach (var t in Lexer.Lex(host, code))
            {
                if (t.Type.StartsWith("template.expression")) continue;
                Assert.DoesNotContain("zz", t.Text);
            }
        }
    }
}
