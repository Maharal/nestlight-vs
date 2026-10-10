# Nestlight for Visual Studio

> Syntax highlighting for languages embedded in strings.
> For **Visual Studio 2022 / 2026**. Not for VS Code.

Nestlight recognizes code embedded in string literals from a tag or a marker comment, colors it as its own language, and leaves interpolated expressions to the host code. It colors the embedded code and completes it as you type; it never changes your code on its own.

<!-- TODO: add a screenshot, e.g. docs/screenshot.png -->

## Hosts

| Host | String forms | Interpolation | Marker comments |
|---|---|---|---|
| JavaScript / TypeScript | template literals, tagged or untagged | `${expr}` | `/* */`, `//` |
| C# | regular, verbatim (`@"..."`), raw (`"""..."""`) and their interpolated forms (`$`, `$$`) | `{expr}`, or `{{expr}}` with `$$` (the brace count equals the `$` count) | `/* */`, `//` |
| Python | single, double and triple-quoted strings, including f-strings and t-strings | `{expr}` in f-strings and t-strings | `#` |
| C++ | raw string literals (`R"delim(...)delim"`) | none | `/* */`, `//` |

## Marking a string

A string is embedded code when it is marked. Nestlight guesses from the content only if you turn on [automatic detection](#automatic-detection), which is off by default.

- **Tag** (JavaScript / TypeScript): `` html`...` ``. The tag is a language id or alias and touches the backtick. Member access works (`` ui.html`...` ``).
- **Marker comment** (every host): a comment immediately before the string, on the same line or the line above, containing `id`, `language=id` or `lang=id`.

Ids and markers are case-insensitive.

```ts
const page = html`<li class="item">${name}</li>`;
const sheet = /* css */ `.item { color: ${color}; }`;
```

```csharp
// language=html
var page = $"""<li class="item">{name}</li>""";
```

```python
# language=html
page = f"""<li class="item">{name}</li>"""
```

```cpp
// language=html
auto page = R"(<li class="item">name</li>)";
```

## Automatic detection

Off by default. Under **Tools > Options > NestLight > Automatic detection** you can turn on the guess of the language of strings you did not mark, and choose which languages it may guess: SQL, HTML, CSS, JSON and GraphQL (the list follows the detectors the plugin has). A string that is guessed is colored and completed like a marked one.

It is a heuristic and can be wrong: a string that only looks like code is colored as code (`"Update the set of items"` can pass for SQL). A mark always wins over the guess, so `// language=yaml` or a tag settles any doubt, and a language you turn off is never guessed. It reads only the content of the string, cheaply: [EA34](docs/experiments.md) measures the cost. In C#, `json` stays with Visual Studio, as for marked strings. The choice is kept in `%LOCALAPPDATA%\NestLight\detection.txt`.

## Embedded languages

| Language | Ids | Highlighted |
|---|---|---|
| HTML | `html`, `htm`, `svg` | tags, attributes, comments, `@event` / `.property` / `?boolean` bindings, CSS in `<style>` and `style="..."` |
| CSS | `css` | selectors, properties, `--custom-properties`, values, units, colors, functions, at-rules, `!important`, nesting |
| SQL | `sql` | keywords, identifiers, literals, operators, comments |
| JSON | `json` | keys, strings, numbers, literals, punctuation |
| GraphQL | `graphql`, `gql` | operations, fields, arguments, variables, types, directives |
| XML | `xml` | tags, attributes, comments, CDATA |
| Markdown | `markdown`, `md` | headings, emphasis (also across line breaks), code spans and fences, links, lists, math (`$x$`, `$$x$$` and `$$` blocks) |
| YAML | `yaml`, `yml` | keys, scalars, anchors, comments |
| Regular expressions | `regex`, `regexp` | groups, classes, quantifiers, escapes, anchors |
| Shaders | `glsl`, `wgsl` | keywords, types, built-ins, numbers, comments |

In C#, strings marked `json` or `regex` are left to Visual Studio's built-in support.

In a JavaScript template the backtick of a Markdown code span or fence is written `\``; Nestlight reads it as the backtick it stands for. The indentation that every line of the string shares is taken as the indentation of the code around it, so a Markdown document indented inside a function is colored like one at the left margin.

Interpolations are never colored as embedded code in any language. Templates nested in an interpolation are highlighted at every level, and incomplete code never breaks the editor.

## Completion

Inside the code of a marked string, Nestlight adds two kinds of suggestions to the list Visual Studio shows while you type (or on **Ctrl+Space**). Outside embedded code the host language keeps its own completion, and nothing is offered inside an interpolation.

- **Keywords of the language**: SQL keywords, CSS properties and values, HTML and SVG tags, GraphQL keywords, `true` / `false` / `null` in JSON and YAML, and the keywords, types and built-ins of GLSL and WGSL. SQL keywords follow the case you type (`SEL` offers `SELECT`).
- **Words already in the document**, as Visual Studio Code does: identifiers of at least 3 characters that start with what you typed, nearest to the caret first. They come from the whole file, host code included, so a column or class name you already wrote is one keystroke away.

XML, Markdown and regular expressions have no closed vocabulary, so they only get words from the document. Keywords come first in the list.

**Misspelled words.** When nothing starts with what you typed (3 or more letters), Nestlight offers up to 10 similar keywords and words of the document, after any exact ones: one edit away for 3 to 5 letters, two for 6 or more (an extra letter, a missing one, a wrong one, or two swapped), with the same first letter. `SELCT` offers `SELECT`, `<dvi` offers `div`, `custmer` offers `customerName`. Press **Ctrl+Space** after the mistake: a list opened by typing starts after one letter. When the prefix is right the list is exactly the one above.

**Context.** The words that already followed the same word in the document come first: after `FROM ` the table that followed `FROM` before, after `display: ` the value used after `display:`. A short word such as `BY` is offered where it followed (`GROUP BY`). The words written in other strings of the same language come before the variables of the host code and the words of other languages, even when those are nearer to the caret.

**Place in the grammar.** In SQL, CSS and HTML the place of the caret decides what comes first: a table of the document after `FROM` / `JOIN` / `INTO`, `BY` after `GROUP` / `ORDER`, the next clause after a column, a column after `u.`; in CSS the properties inside the braces, the values of the property after its colon (`display: ` offers `block`, `flex`, `grid`) and the HTML tags in a selector; in HTML the attributes of the tag inside `<button `, the values of an attribute (`type="` offers the input types) and the classes of the document inside `class="`. What does not belong there is moved to the end of the list, not removed.

## Install

Download the `.vsix` from [Releases](../../releases), close Visual Studio and double-click the file.

To build from source, open `NestLight/NestLight.csproj` with the **Visual Studio extension development** workload and press F5.

To run the tests, use **Test > Run All Tests** in Visual Studio or `dotnet test NestLight.Tests` from a terminal.

Releases are built by CI (VSIX plus a test report) and follow [Semantic Versioning](https://semver.org); see [RELEASING.md](RELEASING.md).

Colors are under **Tools > Options > Environment > Fonts and Colors > Text Editor**, in the items named **Template &lt;Language&gt; ...**.

## How it works

Nestlight is a MEF `IClassifier` that layers its colors over the editor's string color.

1. **Host scanner** (one per host) finds marked strings, the range of every interpolation and the escape sequences, skipping comments and unmarked strings.
2. **Language tokenizer** (one per embedded language) receives the text with interpolations masked and escapes decoded, tokenizes it and never sees the host code.
3. **Registry** maps tags, markers and ids to tokenizers.
4. **Engine** runs the pipeline and maps the tokens back to the source, clipping them so they never overlap an interpolation.
5. **Classifier** turns tokens into classification spans, cached per text snapshot.

The code is split by responsibility, and every dependency is injected through a constructor:

| Folder | Holds |
|---|---|
| `Completion` | The completion engine, the distance used for similar words and the rankers of the words of the document (`IWordRanker`: nearest, frequency, blend); in `Completion/Languages`, one strategy per embedded language (its words and the order they are used in, what counts as a word, the grammar of the place of the caret) |
| `Common` | The model (embedded string, interpolation, token), the interfaces everything else depends on, marker rules and text helpers |
| `Hosts` | One scanner per host language |
| `Detection` | The automatic detector (one strategy per language it can guess, behind `ILanguageDetector`) and the options that turn it and each language on and off |
| `EmbeddedLanguages` | One tokenizer per embedded language |
| `Highlighting` | The engine, the registry, the snapshot and scan caches (the classifier and the completion of a buffer share one scan per snapshot) and the composition root, the only place that knows the concrete classes |
| `VisualStudio` | The only code that touches the editor: classifier and completion providers, and the classification types and default colors |

Everything but `VisualStudio` is free of the Visual Studio SDK, so the tests run it as is.

Adding a host means writing a scanner and a provider. Adding a language means writing a tokenizer, adding it to the composition root and declaring its classification types in `VisualStudio`, and, to complete it, writing a completion strategy in `Completion/Languages` (a class that derives from `CompletionLanguage` and overrides only what is different about the language). A language that holds code of another (HTML holds CSS) has one definition of where that code is, in its tokenizer (`INestingTokenizer`); the completion asks the same tokenizer, so the two cannot disagree. A new way of ordering the words of the document is a new `IWordRanker`; the engine does not change.

## Experiments

Beyond the unit tests, which check what the plugin colors, a suite of **experiments** checks how well it performs. Each experiment is a hypothesis (for example, "the text copy on every edit is worth removing") with a test and a criterion written in advance. They live in their own project, `NestLight.Experiments`, and are not part of `dotnet test`. There are two kinds:

- **Automatic** (`EAnn`): the code measures and decides whether the criterion held.
- **Manual** (`EMnn`): a qualitative test without Visual Studio. The code runs the plugin over generated code and writes artifacts (the source, the final view of the colors as text and as an HTML page, the completion, case by case), and a person or an AI agent reads them and judges the quality. It has no criterion; its result is a list of findings. What is tried by hand inside Visual Studio is not documented as an experiment.

```
dotnet run -c Release --project NestLight.Experiments                      # the automatic experiments
dotnet run -c Release --project NestLight.Experiments -- --manual          # the manual review: artifacts to read, in artifacts/EM01/<time>/
dotnet run -c Release --project NestLight.Experiments -- --generate        # only the generated code for every combination, in artifacts/generated/<time>/
```

The automatic run executes every automatic experiment and writes a Markdown report with the tables, the analysis and whether each criterion held to `reports/<time>-<commit>.md`. Options: `--only EA03,EA05`, `--list`, `--quick` (a smoke run, numbers not worth keeping) and `--out <dir>`.

Reports (`reports/`) and artifacts (`artifacts/`) are named by the time of the run and are not versioned: only timeless documentation goes to GitHub. The experiments are defined in [docs/experiments.md](docs/experiments.md), and each one has a document beside its source (`NestLight.Experiments/Experiments/`). The *Experiments* workflow runs the suite on Windows, on the same runtime as Visual Studio (`net48`), and uploads the report.

## Limitations

- Code inside an interpolation gets a neutral color, not host-language highlighting.
- Python implicit string concatenation is not joined: each literal is analyzed on its own.
- Automatic detection reads the string as written, with escapes and interpolations undecoded, so JSON inside a C# regular string (`\"`) is not recognized. In JavaScript only template literals are considered.
- `<script>` content, SCSS / LESS and JSX / TSX are not highlighted.

## License

[MIT](LICENSE.txt). Copyright (c) 2026 Raphael Augusto Teixeira Silva.