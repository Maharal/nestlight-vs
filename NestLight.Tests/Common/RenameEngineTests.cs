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
    }
}
