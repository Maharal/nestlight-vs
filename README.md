# Nestlight for Visual Studio

> Syntax highlighting for languages embedded in strings.
> For **Visual Studio 2022 / 2026**. Not for VS Code.

Nestlight recognizes code embedded in string literals from a tag or a marker comment, colors it as its own language, and leaves interpolated expressions to the host code. It only provides colors and never edits your code.

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

## Install

Download the `.vsix` from [Releases](../../releases), close Visual Studio and double-click the file.

To build from source, open `NestLight/NestLight.csproj` with the **Visual Studio extension development** workload and press F5.

To run the tests, use **Test > Run All Tests** in Visual Studio or `dotnet test NestLight.Tests` from a terminal (any OS).

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
| `Common` | The model (embedded string, interpolation, token), the interfaces everything else depends on, marker rules and text helpers |
| `Hosts` | One scanner per host language |
| `Languages` | One tokenizer per embedded language |
| `Highlighting` | The engine, the registry, the snapshot cache and the composition root, the only place that knows the concrete classes |
| `VisualStudio` | The only code that touches the editor: classifier providers and the classification types and default colors |

Everything but `VisualStudio` is free of the Visual Studio SDK, so the tests run it as is.

Adding a host means writing a scanner and a provider. Adding a language means writing a tokenizer, adding it to the composition root and declaring its classification types in `VisualStudio`.

## Limitations

- Code inside an interpolation gets a neutral color, not host-language highlighting.
- The JavaScript / TypeScript scanner does not understand regex literals, so one containing a quote or a backtick can derail the scan for the rest of the file.
- Python implicit string concatenation is not joined: each literal is analyzed on its own.
- `<script>` content, SCSS / LESS and JSX / TSX are not highlighted.

## License

[MIT](LICENSE.txt). Copyright (c) 2026 Raphael Augusto Teixeira Silva.