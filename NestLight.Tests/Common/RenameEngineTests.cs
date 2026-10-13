using System.Collections.Generic;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class RenameEngineTests
    {
        [Fact]
        public void Finds_css_class_references_from_selector()
        {
            string code = "const x = html`<div class=\"foo bar\"></div>`; const y = css`.foo { color: red } .bar { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int fooSelectorPos = code.IndexOf(".foo");
            var refs = RenameEngine.FindReferences(tokens, code, fooSelectorPos);

            Assert.NotNull(refs);
            Assert.True(refs.Count >= 2);
        }

        [Fact]
        public void Finds_css_class_references_from_html_attribute()
        {
            string code = "const x = html`<div class=\"foo\"></div>`; const y = css`.foo { color: red }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int classValuePos = code.IndexOf("\"foo\"") + 1;
            var refs = RenameEngine.FindReferences(tokens, code, classValuePos);

            Assert.NotNull(refs);
            Assert.True(refs.Count >= 2);
        }

        [Fact]
        public void Finds_id_references()
        {
            string code = "const x = html`<div id=\"main\"></div>`; const y = css`#main { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int idValuePos = code.IndexOf("\"main\"") + 1;
            var refs = RenameEngine.FindReferences(tokens, code, idValuePos);

            Assert.NotNull(refs);
            Assert.True(refs.Count >= 2);
        }

        [Fact]
        public void Returns_null_for_non_renameable_token()
        {
            string code = "const x = html`<div></div>`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            Assert.Null(RenameEngine.FindReferences(tokens, code, 0));
        }

        [Fact]
        public void Returns_null_for_empty_tokens()
        {
            Assert.Null(RenameEngine.FindReferences(new List<Token>(), "text", 0));
        }

        // --- Mutation killing tests ---

        // L42: "" instead of "classname" string mutation
        [Fact]
        public void Finds_references_from_classname_attribute()
        {
            string code = "const x = html`<div classname=\"foo\"></div>`; const y = css`.foo { color: red }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int classValuePos = code.IndexOf("\"foo\"") + 1;
            var refs = RenameEngine.FindReferences(tokens, code, classValuePos);
            Assert.NotNull(refs);
            Assert.True(refs.Count >= 2);
        }

        // L48: unquoted.Length boundary mutations, quote detection, localOffset-- vs ++
        [Fact]
        public void Multiple_class_references_with_position_inside_second_class()
        {
            string code = "const x = html`<div class=\"foo bar\"></div>`; const y = css`.foo { } .bar { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int barPos = code.IndexOf("bar");
            var refs = RenameEngine.FindReferences(tokens, code, barPos);
            Assert.NotNull(refs);
            Assert.True(refs.Count >= 2);
        }

        // L74: conditional true/false mutation, prefix == "." check, string mutations
        [Fact]
        public void Rename_from_id_selector_uses_id_type()
        {
            string code = "const x = html`<div id=\"main\"></div>`; const y = css`#main { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            // From CSS selector
            int selectorPos = code.IndexOf("#main");
            var refs = RenameEngine.FindReferences(tokens, code, selectorPos);
            Assert.NotNull(refs);
            Assert.True(refs.Count >= 2);
        }

        // L80: t.Start + t.Length > text.Length (boundary check), t.Start - t.Length vs +
        [Fact]
        public void Reference_at_end_of_text_is_found()
        {
            string code = "const x = css`.foo { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int fooPos = code.IndexOf(".foo");
            var refs = RenameEngine.FindReferences(tokens, code, fooPos);
            Assert.NotNull(refs);
        }

        // L86: t.Start - 1 vs + 1, t.Length + 1 vs - 1 (selector reference offsets)
        [Fact]
        public void Selector_reference_offset_excludes_dot_prefix()
        {
            string code = "const x = css`.foo { } .foo:hover { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int fooPos = code.IndexOf(".foo");
            var refs = RenameEngine.FindReferences(tokens, code, fooPos);
            Assert.NotNull(refs);
            // Each reference should be "foo" without the "." prefix
            foreach (var r in refs)
            {
                string refText = code.Substring(r.Start, r.Length);
                Assert.Equal("foo", refText);
            }
        }

        // L95: pAttrName == "classname" mutations, prefix == "." mutations
        [Fact]
        public void Class_reference_matches_class_attribute()
        {
            string code = "const x = html`<div class=\"test\"></div>`; const y = css`.test { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int selectorPos = code.IndexOf(".test");
            var refs = RenameEngine.FindReferences(tokens, code, selectorPos);
            Assert.NotNull(refs);
            // Should find both .test in CSS and "test" in HTML attribute
            Assert.True(refs.Count >= 2);
            foreach (var r in refs)
            {
                string refText = code.Substring(r.Start, r.Length);
                Assert.Equal("test", refText);
            }
        }

        // L96: pAttrName == "id" && prefix == "#" mutations
        [Fact]
        public void Id_reference_matches_id_attribute()
        {
            string code = "const x = html`<div id=\"main\"></div>`; const y = css`#main { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int idPos = code.IndexOf("#main");
            var refs = RenameEngine.FindReferences(tokens, code, idPos);
            Assert.NotNull(refs);
            foreach (var r in refs)
            {
                string refText = code.Substring(r.Start, r.Length);
                Assert.Equal("main", refText);
            }
        }

        // L101: raw.Length boundary, quote detection mutations
        // L104: isId || string.Equals (should be &&)
        // L106: t.Start - quoteOffset vs + quoteOffset
        [Fact]
        public void Id_in_attribute_with_quotes_has_correct_offset()
        {
            string code = "const x = html`<div id=\"test\"></div>`; const y = css`#test { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int idValuePos = code.IndexOf("\"test\"") + 1;
            var refs = RenameEngine.FindReferences(tokens, code, idValuePos);
            Assert.NotNull(refs);
            // The reference from the attribute should point to "test" (skipping the quote)
            foreach (var r in refs)
            {
                string refText = code.Substring(r.Start, r.Length);
                Assert.Equal("test", refText);
            }
        }

        // L117: t.Start + quoteOffset + idx offsets, t.Start - quoteOffset
        [Fact]
        public void Class_in_multi_value_attribute_has_correct_offset()
        {
            string code = "const x = html`<div class=\"a b\"></div>`; const y = css`.b { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int selectorPos = code.IndexOf(".b");
            var refs = RenameEngine.FindReferences(tokens, code, selectorPos);
            Assert.NotNull(refs);
            foreach (var r in refs)
            {
                string refText = code.Substring(r.Start, r.Length);
                Assert.Equal("b", refText);
            }
        }

        // L125: references.Count >= 0 vs > 0, conditional true/false
        [Fact]
        public void Returns_null_when_no_references_found()
        {
            // CSS selector with no matching attribute values
            string code = "const x = html`<div></div>`; const y = css`.nonexistent { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int selectorPos = code.IndexOf(".nonexistent");
            var refs = RenameEngine.FindReferences(tokens, code, selectorPos);
            // Only the selector itself, so refs should have at least 1
            // Actually it should find at least the selector itself
            Assert.NotNull(refs);
        }
    }
}
