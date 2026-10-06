# Parser tests (xUnit)

They cover the parser rules, without depending on the Visual Studio SDK:

| File | What it verifies |
|---|---|
| `ScannerTests.cs` | which backticks become templates (html/svg/htm/css, `/* html */`, `/* css */`), ignored comments and strings, escapes, nested `${}`, templates inside expressions, unclosed templates/expressions |
| `HtmlTokenizerTests.cs` | tags, attributes, `@event` / `.prop` / `?bool`, expressions in any position, comments, `<style>` and `style="..."` as CSS |
| `CssTokenizerTests.cs` | selectors, pseudo, nesting, properties, `--custom`, numbers/units/colors, functions, `url()`, `!important`, at-rules, comments, incomplete code, expressions |
| `RobustnessTests.cs` | every prefix/suffix of a file without exceptions or overlaps, tokens inside templates, performance, unique classification names |

## How to run
- Visual Studio: *Test > Test Explorer > Run All* (restore the NuGet packages first).
- Command line, in this project's folder: `dotnet test`

The `Tpl*.cs` files from the `../NestLight` folder are linked into the test project
(`TplNames`, `TplScanner`, `TplHtmlTokenizer`, `TplCssTokenizer`), so the tests always run
against the extension's real code. Keep the two folders side by side.

## How to add a case
Use `Lexer.H("<html here>")` / `Lexer.C("css here")` to wrap the text in a template and
`Lexer.Texts(code, TplNames.Type)` to get the ranges of each classification, or
`Lexer.Seq(code)` for the full `type|text` sequence.
