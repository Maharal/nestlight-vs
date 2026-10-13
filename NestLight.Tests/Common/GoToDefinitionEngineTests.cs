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

        [Fact]
        public void Finds_definition_in_external_file()
        {
            string file1 = "const x = html`<div class=\"foo\"></div>`;";
            string file2 = "const y = css`.foo { color: red }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens1 = highlighter.Highlight(file1);
            IReadOnlyList<Token> tokens2 = highlighter.Highlight(file2);

            int classValuePos = file1.IndexOf("\"foo\"") + 1;
            var externals = new List<ExternalFile> { new ExternalFile(tokens2, file2) };
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens1, file1, classValuePos, externals);

            Assert.True(result.HasValue);
            Assert.Equal(0, result.Value.FileIndex);
            string definitionText = file2.Substring(result.Value.Start, result.Value.Length);
            Assert.Equal(".foo", definitionText);
        }

        [Fact]
        public void Same_file_definition_takes_priority_over_external()
        {
            string code = "const x = html`<div class=\"foo\"></div>`; const y = css`.foo { color: red }`;";
            string file2 = "const z = css`.foo { display: block }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);
            IReadOnlyList<Token> tokens2 = highlighter.Highlight(file2);

            int classValuePos = code.IndexOf("\"foo\"") + 1;
            var externals = new List<ExternalFile> { new ExternalFile(tokens2, file2) };
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens, code, classValuePos, externals);

            Assert.True(result.HasValue);
            Assert.Equal(-1, result.Value.FileIndex);
        }

        // --- Mutation killing tests ---

        // L47: "" instead of "classname" string mutation
        [Fact]
        public void Finds_definition_from_classname_attribute()
        {
            string code = "const x = html`<div classname=\"foo\"></div>`; const y = css`.foo { color: red }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int classValuePos = code.IndexOf("\"foo\"") + 1;
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens, code, classValuePos);

            // classname should work like class
            Assert.True(result.HasValue);
        }

        // L53: unquoted.Length > 0 && (unquoted[0] == '"' || unquoted[0] == '\'')
        // Mutations: || to &&, < 0, >= 0, != '"', localOffset-- vs ++
        [Fact]
        public void Class_attribute_with_multiple_classes_finds_correct_one()
        {
            string code = "const x = html`<div class=\"foo bar baz\"></div>`; const y = css`.bar { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            // Position on "bar" within the class attribute
            int barPos = code.IndexOf("bar") ;
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens, code, barPos);
            Assert.True(result.HasValue);
            string defText = code.Substring(result.Value.Start, result.Value.Length);
            Assert.Equal(".bar", defText);
        }

        // L58: localOffset >= idx || localOffset < idx + cls.Length mutations
        [Fact]
        public void Class_attribute_position_at_boundary_of_class_name()
        {
            string code = "const x = html`<div class=\"ab cd\"></div>`; const y = css`.ab { } .cd { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            // Position at "cd" start
            int cdStart = code.IndexOf("cd");
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens, code, cdStart);
            Assert.True(result.HasValue);
            string defText = code.Substring(result.Value.Start, result.Value.Length);
            Assert.Equal(".cd", defText);
        }

        // L80: current.Type != CssSelectorId (should be ==)
        [Fact]
        public void Navigates_from_css_id_selector()
        {
            string code = "const x = css`#main { } #main:hover { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens = highlighter.Highlight(code);

            int mainPos = code.IndexOf("#main");
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens, code, mainPos);
            Assert.True(result.HasValue);
            Assert.NotEqual(mainPos, result.Value.Start);
        }

        // L94: f <= externalFiles.Count / f-- mutations
        [Fact]
        public void Searches_all_external_files()
        {
            string file1 = "const x = html`<div class=\"foo\"></div>`;";
            string file2 = "const y = css`.bar { }`;";
            string file3 = "const z = css`.foo { }`;";
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            IReadOnlyList<Token> tokens1 = highlighter.Highlight(file1);
            IReadOnlyList<Token> tokens2 = highlighter.Highlight(file2);
            IReadOnlyList<Token> tokens3 = highlighter.Highlight(file3);

            int classValuePos = file1.IndexOf("\"foo\"") + 1;
            var externals = new List<ExternalFile>
            {
                new ExternalFile(tokens2, file2),
                new ExternalFile(tokens3, file3)
            };
            DefinitionResult? result = GoToDefinitionEngine.FindDefinition(tokens1, file1, classValuePos, externals);

            Assert.True(result.HasValue);
            Assert.Equal(1, result.Value.FileIndex); // found in second external file
        }
    }
}
