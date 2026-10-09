# Tests (xUnit)

They cover the whole pipeline without depending on the Visual Studio SDK.

| Folder / file | What it verifies |
|---|---|
| `Common/MarkerCommentTests.cs` | marker comments (`id`, `language=id`, `lang=id`), tags, the same-line / line-above rule, the languages a host accepts |
| `Hosts/JavaScriptHostScannerTests.cs`, `JavaScriptMarkersTests.cs` | which template literals are embedded code, `//` and `/* */` markers, every documented id, nesting |
| `Hosts/CSharpHostScannerTests.cs` | regular, verbatim and raw strings, `$` / `$$` interpolation, escapes, format specifiers, the `json` / `regex` exclusion |
| `Hosts/PythonHostScannerTests.cs` | quote styles and prefixes, f-strings and t-strings, replacement fields, escapes |
| `Hosts/CppHostScannerTests.cs` | raw string literals, delimiters and prefixes, digit separators |
| `EmbeddedLanguages/*TokenizerTests.cs` | the tokens of each embedded language (HTML, CSS, SQL, JSON, GraphQL, XML, Markdown, YAML, regex, shaders) |
| `Highlighting/HighlightEngineTests.cs` | the engine against fake scanners and tokenizers (decoding, masking, mapping back), interpolation tokens per host, nesting at every level |
| `Highlighting/SnapshotTokenCacheTests.cs` | caching per snapshot, span queries, failures never escaping |
| `Completion/CompletionEngineTests.cs` | where completion applies (marked strings only, never inside an interpolation, innermost string), the keywords of each language, SQL case, and the words of the document (ranking, no self-suggestion, limits, large files) |
| `Completion/WordRankerTests.cs` | the rankers of the words of the document (nearest, frequency, blend) on their own, and a ranker the engine has never heard of plugged in behind it |
| `Completion/KeywordOrderTests.cs` | the order of the keywords by use, owned by each language (aliases, a language without a list, the features that turn it on), the words before the keywords and the words of two letters |
| `Highlighting/IncrementalHostScanTests.cs` | the scan of an edited text made from the scan of the text before: equal to a whole scan (strings, interpolations, escapes and safe points) after random chains of edits in every host and at three densities of safe points, edits that change what follows, the strings before the edit kept and the ones after it moved, and the cache that uses it |
| `Completion/SuggestionIconTests.cs` | the icon of each suggestion (keyword, word of the document, and the small one of a similar word), and, in `Highlighting/CompositionTests.cs`, that the Visual Studio code gives every kind of icon an image of the catalog |
| `Completion/SqlContinuationTests.cs` | what continues each clause of SQL (the table of clauses) |
| `EmbeddedLanguages/HtmlNestedRegionsTests.cs` | where the CSS of HTML is (`<style>`, `style=""`), that the tokenizer and the completion agree on every prefix of the samples, and the regions in the coordinates of the document |
| `Highlighting/CompositionTests.cs` | the registry, the composition root, every documented id, and that every classification name has a type and a default format in `VisualStudio/` |
| `Highlighting/RobustnessTests.cs`, `AllHostsRobustnessTests.cs` | every prefix / suffix / single-character deletion of real samples, random noise, deep nesting and performance, for every host and language |

## How to run
- Visual Studio: *Test > Test Explorer > Run All* (restore the NuGet packages first).
- Command line, in this project's folder: `dotnet test`

The project targets `net48`, like the extension. The source folders `Common`, `Hosts`, `EmbeddedLanguages` and `Highlighting` of `../NestLight` are linked into
the test project, so the tests always run against the extension's real code. Keep the two folders side by side.

## How to add a case
- A host: `Lexer.Scan(HostLanguage.X, code)` returns the embedded strings (range, language, interpolations, escapes).
- A language: `Lexer.Language("sql", code)` wraps the code in a JavaScript template and returns `type|text` for each token (backticks and a trailing backslash
  cannot be written there: use `Lexer.Seq(HostLanguage.Python, ...)` instead).
- The whole pipeline: `Lexer.Lex(host, code)` / `Lexer.Texts(host, code, type)`.

## Experiments
The experiments are not unit tests: they ask how well the plugin performs, not whether it is correct. They are split by folder into automatic (the code decides) and manual (a person reads what the plugin did on generated code), and a code generator writes a sample for every host x embedded language combination: see [docs/experiments.md](../docs/experiments.md#automatic-and-manual-experiments).
