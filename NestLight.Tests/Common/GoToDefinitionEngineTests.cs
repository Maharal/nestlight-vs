using System.Collections.Generic;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class GoToDefinitionEngineTests
    {
        [Fact]
        public void Finds_css_class_definition_from_html_attribute()
        {
            string code = "const x = html`<div class=\"foo\"></div>`; const y = css`.foo { color: red }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int classValuePos = code.IndexOf("\"foo\"") + 1;
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens, code, classValuePos);

            Assert.True(result.HasValue);
            string definitionText = code.Substring(result.Value.Start, result.Value.Length);
            Assert.Equal(".foo", definitionText);
        }

        [Fact]
        public void Returns_null_when_no_definition_exists()
        {
            string code = "const x = html`<div class=\"bar\"></div>`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int classValuePos = code.IndexOf("\"bar\"") + 1;
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens, code, classValuePos);

            Assert.Null(result);
        }

        [Fact]
        public void Returns_null_for_position_outside_tokens()
        {
            string code = "const x = html`<div></div>`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            Assert.Null(GoToDefinitionEngine.FindDefinition(tokens, code, 0));
        }

        [Fact]
        public void Returns_null_for_non_navigable_token()
        {
            string code = "const x = html`<div></div>`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int divPos = code.IndexOf("div");
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens, code, divPos);

            Assert.Null(result);
        }

        [Fact]
        public void Navigates_from_css_class_selector_to_another_occurrence()
        {
            string code = "const x = css`.btn { } .btn:hover { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int firstBtn = code.IndexOf(".btn");
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens, code, firstBtn);

            Assert.True(result.HasValue);
            Assert.NotEqual(firstBtn, result.Value.Start);
        }

        [Fact]
        public void Finds_id_definition_from_html_attribute()
        {
            string code = "const x = html`<div id=\"main\"></div>`; const y = css`#main { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int idValuePos = code.IndexOf("\"main\"") + 1;
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens, code, idValuePos);

            Assert.True(result.HasValue);
            string definitionText = code.Substring(result.Value.Start, result.Value.Length);
            Assert.Equal("#main", definitionText);
        }

        [Fact]
        public void Returns_null_for_empty_tokens()
        {
            Assert.Null(GoToDefinitionEngine.FindDefinition(new List<Token>(), "text", 0));
        }
    }
}
