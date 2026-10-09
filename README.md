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

A string is embedded code only when it is marked. Nestlight never guesses from the content.

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

## Embedded languages

| Language | Ids | Highlighted |
|---|---|---|
| HTML | `html`, `htm`, `svg` | tags, attributes, comments, `@event` / `.property` / `?boolean` bindings, CSS in `<style>` and `style="..."` |
| CSS | `css` | selectors, properties, `--custom-properties`, values, units, colors, functions, at-rules, `!important`, nesting |
| SQL | `sql` | keywords, identifiers, literals, operators, comments |
| JSON | `json` | keys, strings, numbers, literals, punctuation |
| GraphQL | `graphql`, `gql` | operations, fields, arguments, variables, types, directives |
| XML | `xml` | tags, attributes, comments, CDATA |
| Markdown | `markdown`, `md` | headings, emphasis, code spans and fences, links, lists |
| YAML | `yaml`, `yml` | keys, scalars, anchors, comments |
| Regular expressions | `regex`, `regexp` | groups, classes, quantifiers, escapes, anchors |
| Shaders | `glsl`, `wgsl` | keywords, types, built-ins, numbers, comments |

In C#, strings marked `json` or `regex` are left to Visual Studio's built-in support.

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
| `Completion` | The completion engine and the distance used for similar words; in `Completion/Languages`, one strategy per embedded language (its words, what counts as a word, the grammar of the place of the caret) |
| `Common` | The model (embedded string, interpolation, token), the interfaces everything else depends on, marker rules and text helpers |
| `Hosts` | One scanner per host language |
| `EmbeddedLanguages` | One tokenizer per embedded language |
| `Highlighting` | The engine, the registry, the snapshot and scan caches (the classifier and the completion of a buffer share one scan per snapshot) and the composition root, the only place that knows the concrete classes |
| `VisualStudio` | The only code that touches the editor: classifier and completion providers, and the classification types and default colors |

Everything but `VisualStudio` is free of the Visual Studio SDK, so the tests run it as is.

Adding a host means writing a scanner and a provider. Adding a language means writing a tokenizer, adding it to the composition root and declaring its classification types in `VisualStudio`, and, to complete it, writing a completion strategy in `Completion/Languages` (a class that derives from `CompletionLanguage` and overrides only what is different about the language).

## Experiments

Beyond the unit tests, which check what the plugin colors, a suite of **experiments** checks how well it performs. Each experiment is a hypothesis (for example, "the text copy on every edit is worth removing") with a test and a criterion written in advance. They live in their own project, `NestLight.Experiments`, and are not part of `dotnet test`. There are two kinds:

- **Automatic** (`EAnn`): the code measures and decides whether the criterion held.
- **Manual** (`EMnn`): the code runs the plugin over generated code, one file per host x embedded language combination, and a person reads the files case by case. It has no criterion; its result is a list of findings.

```
dotnet run -c Release --project NestLight.Experiments                      # the automatic experiments
dotnet run -c Release --project NestLight.Experiments -- --manual out      # the manual review: files to read, in out/
dotnet run -c Release --project NestLight.Experiments -- --generate out    # only the generated code for every combination
```

The automatic run executes every automatic experiment and writes a Markdown report with the tables, the analysis and whether each criterion held to `reports/experiments-<date>-<commit>.md` (not versioned: each report is valid only for its commit and machine). Options: `--only EA03,EA05`, `--list`, `--quick` (a smoke run, numbers not worth keeping) and `--out <dir>`.

The experiments are defined in [docs/experiments.md](docs/experiments.md). The *Experiments* workflow runs the suite on Windows, on the same runtime as Visual Studio (`net48`), and uploads the report.

## Limitations

- Code inside an interpolation gets a neutral color, not host-language highlighting.
- The JavaScript / TypeScript scanner does not understand regex literals, so one containing a quote or a backtick can derail the scan for the rest of the file.
- Python implicit string concatenation is not joined: each literal is analyzed on its own.
- `<script>` content, SCSS / LESS and JSX / TSX are not highlighted.

## License

[MIT](LICENSE.txt). Copyright (c) 2026 Raphael Augusto Teixeira Silva.