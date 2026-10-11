# NestLight (Visual Studio 2022/2026)

The extension project. See the [main README](../README.md) for what it highlights and how to use it.

## Layout
- `Common/`: model, interfaces, marker rules, text helpers
- `Hosts/`: scanners for JavaScript / TypeScript, C#, Python and C++
- `Detection/`: the optional automatic detector of the language of unmarked strings (one `ILanguageDetector` strategy per language) and its options
- `EmbeddedLanguages/`: tokenizers for HTML, CSS, SQL, JSON, GraphQL, XML, Markdown, YAML, regular expressions, shaders (GLSL, WGSL), Shell/Bash and TOML
- `Highlighting/`: engine, language registry, snapshot cache and the composition root (`NestLightComposition`)
- `VisualStudio/`: MEF classifier providers, classification types and default formats, quick-info tooltips, diagnostics (error squiggles) and go-to-definition

Dependencies are passed through constructors. Only `VisualStudio/` references the Visual Studio SDK.

## Build
1. Open `NestLight.csproj` (workload "Visual Studio extension development").
2. F5 opens the experimental instance.
3. Release build -> `bin\Release\NestLight.vsix`.

## Colors
Tools > Options > Environment > Fonts and Colors > "Text Editor" > items "Template <Language> ...".

## Adding an embedded language

An embedded language is a language that appears inside string literals of a host language (SQL inside a JavaScript template literal, HTML inside a C# raw string, etc.). Adding one touches these files, in order:

### 1. Tokenizer — `EmbeddedLanguages/<Language>/<Language>Tokenizer.cs`

Create a class that implements `IEmbeddedLanguageTokenizer` (defined in `Common/Abstractions.cs`):

```csharp
internal sealed class FooTokenizer : IEmbeddedLanguageTokenizer
{
    private static readonly string[] FooIds = { "foo" }; // lower-case ids and aliases

    public IReadOnlyList<string> Ids => FooIds;

    public void Tokenize(char[] text, int from, int to, TokenSink emit)
    {
        // text[from..to) is the embedded code, with interpolations already masked
        // (replaced by spaces) and escape sequences decoded.
        // Call emit(start, end, ClassificationNames.FooXxx) for each token.
    }
}
```

- `Ids`: the tags and marker ids that select this language (e.g. `{ "shell", "bash", "sh", "zsh" }`). Case-insensitive. The first id is the canonical one.
- `Tokenize`: receives a `char[]` (not a `string`), a range `[from, to)`, and a `TokenSink` delegate. Emit `(start, end, type)` triples. The type strings come from `ClassificationNames` (step 2). Never throw on malformed input.
- If the language holds code of another language (HTML holds CSS in `<style>` and `style=""`), implement `INestingTokenizer` instead, which adds `FindRegions`.

Register it in `Highlighting/NestLightComposition.cs` → `CreateEmbeddedLanguages()`:
```csharp
new FooTokenizer()
```

### 2. Classification names — `Common/ClassificationNames.cs`

Add a constant per token type. The naming convention is `template.<language>.<role>`:

```csharp
// Foo
public const string FooKeyword = "template.foo.keyword";
public const string FooComment = "template.foo.comment";
```

These names are the contract between the tokenizer and the editor. A test (`VisualStudioContractTests`) fails if any name is missing a type definition or a default format in `VisualStudio/`.

### 3. Classification types and colors — `VisualStudio/EmbeddedLanguageClassificationDefinitions.cs`

For each name from step 2, add **two** things:

**a) A type definition** (a static field with MEF attributes):
```csharp
internal static class FooClassificationTypes
{
    [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.FooKeyword)]
    internal static ClassificationTypeDefinition Keyword;

    [Export(typeof(ClassificationTypeDefinition))][Name(ClassificationNames.FooComment)]
    internal static ClassificationTypeDefinition Comment;
}
```

**b) A format definition** (a class per type, with a display name and default color):
```csharp
[Export(typeof(EditorFormatDefinition))]
[ClassificationType(ClassificationTypeNames = ClassificationNames.FooKeyword)]
[Name("Template Foo Keyword Format")][UserVisible(true)][Order(After = Priority.High)]
internal sealed class FooKeywordFormat : TplFormat
{
    public FooKeywordFormat() : base("Template Foo Keyword", 0x56, 0x9C, 0xD6) { }
}
```

The display name (`"Template Foo Keyword"`) is what appears under Tools > Options > Fonts and Colors. `TplFormat` is a base class that sets the foreground from RGB bytes and optionally `italic: true` or `bold: true`.

### 4. Completion strategy — `Completion/Languages/FooCompletion.cs`

Derive from `CompletionLanguage` and give it the **same ids** as the tokenizer:

```csharp
internal sealed class FooCompletion : CompletionLanguage
{
    public FooCompletion() : base(
        new[] { "foo" },
        keywords: new[] { "keyword1", "keyword2" },  // sorted; offered as completions
        extraWords: new[] { "extra1" })               // offered but not colored by the tokenizer
    { }

    // Optional overrides:
    // KeywordsFollowTypedCase — true if "sel" → "SELECT" (SQL does this)
    // IsExtraWordChar(char) — return true for chars beyond [a-zA-Z0-9_] that are part of a word (e.g. '-' in CSS/TOML)
    // ReadPosition(text, floor, site) — grammar-aware completion: return a Position that says what belongs here
}
```

Register it in `Completion/Languages/CompletionLanguages.cs` → `CreateStandard()`:
```csharp
new FooCompletion()
```

A test (`CompositionTests`) fails until every tokenizer id has a matching completion strategy with the same ids.

### 5. Quick-info descriptions — `VisualStudio/NestLightQuickInfo.cs`

In the `DescriptionOf(string)` method, add entries for the new classification names so the tooltip knows what to say:

```csharp
if (classificationType == ClassificationNames.FooKeyword) return "Foo keyword";
if (classificationType == ClassificationNames.FooComment) return "Foo comment";
```

Types not listed here show no tooltip on hover. This is optional but recommended.

### 6. Tests

- `EmbeddedLanguages/FooTokenizerTests.cs`: test the tokens of each syntactic form, a no-overlap test, and edge cases.
- `Highlighting/CompositionTests.cs` → `DocumentedIds()`: add the new ids. Update the tokenizer count in `Ten_documented_languages_are_served_by_eleven_tokenizers`. Add a sample string to the "every token type" test.

### 7. Documentation

Update the "Embedded languages" table in the root `README.md` and the "Completion" section if the language has grammar-aware completion.

### Optional: automatic detection

To let NestLight guess the language of unmarked strings:

1. Write an `ILanguageDetector` in `Detection/Detectors.cs`. It has three members:
   - `Id`: the same id as the tokenizer (e.g. `"foo"`).
   - `CanStartWith(char)`: a cheap gate — return false if the first character rules out this language without reading the rest.
   - `Score(text, start, end)`: return 0 (not this language) or a positive integer (the higher, the surer). Read `text[start..end)` in place — no copies, no allocations. Use the `Probe` helpers.

2. Add it to `LanguageDetector.Strategies()` in the same file.

3. Add a display name to `NestLightPackage.cs` → `DetectionOptionsPanel.Names` dictionary:
   ```csharp
   { "foo", "Foo" }
   ```

The options page and the persisted file follow the `Strategies()` list automatically. A test fails if a detectable language has no tokenizer.

### Optional: diagnostics

To show warning squiggles for structural errors, add a `Check<Language>` method to `VisualStudio/NestLightDiagnostics.cs` and wire it from `CheckEmbeddedCode`.

---

## Adding a host language

A host language is a programming language whose strings can contain embedded code (JavaScript, C#, Python, C++). Adding one touches more files because every MEF provider that talks to the editor needs a concrete class per host content type.

### 1. Scanner — `Hosts/<Language>HostScanner.cs`

Create a class that implements `IHostScanner` (or `IResumableHostScanner` for incremental scanning):

```csharp
internal sealed class RubyHostScanner : IHostScanner
{
    private readonly AcceptedEmbeddedLanguages _languages;

    public RubyHostScanner(AcceptedEmbeddedLanguages languages)
    {
        _languages = languages ?? throw new ArgumentNullException("languages");
    }

    public IReadOnlyList<EmbeddedString> Scan(string text)
    {
        // Find all string literals in the host code.
        // For each string:
        //   1. Check if it has a marker comment (tag or // language=id) → _languages.Identify(id)
        //   2. Find the body of the string (after quotes/delimiters)
        //   3. Find interpolation ranges (e.g. ${...} in JS, {expr} in C#)
        //   4. Return an EmbeddedString with OuterStart/OuterEnd, Start/End (body), EmbeddedLanguageId, and Interpolations
        // Return the list ordered by OuterStart.
    }
}
```

The scanner must handle:
- Every string form of the host (regular, raw, verbatim, template, multiline, f-strings, etc.)
- Marker comments (`// language=id`, `/* id */`, `# id`) on the same line or the line above
- Tags (in JavaScript: `` html`...` ``)
- Interpolation ranges (the engine masks them before passing to the tokenizer)
- Escape sequences (the engine decodes them)
- Incomplete/malformed code without throwing

### 2. Composition root — `Highlighting/NestLightComposition.cs`

Add a value to the `HostLanguage` enum:
```csharp
public enum HostLanguage { JavaScript, CSharp, Python, Cpp, Ruby }
```

Add a case to `CreateScanner`:
```csharp
case HostLanguage.Ruby: return new RubyHostScanner(new AcceptedEmbeddedLanguages(languages, null, guesser));
```

The second argument to `AcceptedEmbeddedLanguages` is an exclusion list — ids left to VS's built-in support (C# excludes `json` and `regex`). Pass `null` if none.

### 3. MEF providers — `VisualStudio/` (one concrete class per host for each provider)

Every MEF component that serves a content type needs a concrete subclass with the `[ContentType]` and `[Export]` attributes. The content type string is what VS uses (e.g. `"Ruby"` — check VS's content type registry for the exact name).

**a) Classifier** — `VisualStudio/NestLightClassifier.cs`:
```csharp
[Export(typeof(IClassifierProvider))]
[ContentType("Ruby")]
internal sealed class RubyClassifierProvider : NestLightClassifierProvider
{
    public RubyClassifierProvider() : base(HostLanguage.Ruby) { }
}
```

**b) Completion source** — `VisualStudio/NestLightCompletionSource.cs`:
```csharp
[Export(typeof(IAsyncCompletionSourceProvider))]
[ContentType("Ruby")]
[Name("NestLight completion source (Ruby)")]
internal sealed class RubyCompletionSourceProvider : NestLightCompletionSourceProvider
{
    public RubyCompletionSourceProvider() : base(HostLanguage.Ruby) { }
}
```

**c) Quick-info** — `VisualStudio/NestLightQuickInfo.cs`:
```csharp
[Export(typeof(IAsyncQuickInfoSourceProvider))]
[Name("NestLight quick info (Ruby)")]
[ContentType("Ruby")]
internal sealed class RubyQuickInfoSourceProvider : NestLightQuickInfoSourceProvider
{
    public RubyQuickInfoSourceProvider() : base(HostLanguage.Ruby) { }
}
```

**d) Diagnostics** — `VisualStudio/NestLightDiagnostics.cs`:
```csharp
[Export(typeof(ITaggerProvider))]
[TagType(typeof(IErrorTag))]
[ContentType("Ruby")]
internal sealed class RubyDiagnosticsProvider : NestLightDiagnosticsProvider
{
    public RubyDiagnosticsProvider() : base(HostLanguage.Ruby) { }
}
```

**e) Go-to-definition** — `VisualStudio/NestLightGoToDefinition.cs`: add the `[ContentType("Ruby")]` attribute to the existing `NestLightGoToDefinitionListener` and `NestLightPeekableItemSourceProvider` classes.

### 4. Tests

- `Hosts/<Language>HostScannerTests.cs`: test every string form, markers, interpolations, escapes, incomplete code, and the content of the `EmbeddedString` fields.
- `Highlighting/CompositionTests.cs`: the `Every_host_gets_a_working_highlighter` test picks up the new `HostLanguage` value automatically.
- `Highlighting/IncrementalHostScanTests.cs`: add the host to the `Hosts` member data if the scanner implements `IResumableHostScanner`.
- `Highlighting/RobustnessTests.cs` / `AllHostsRobustnessTests.cs`: add the host so it is fuzz-tested.

### 5. Documentation

Update the "Hosts" table in the root `README.md` (string forms, interpolation, marker comments) and the layout list in this file.

---

## Checklist summary

### Embedded language (minimum)

| Step | File | What |
|---|---|---|
| 1 | `EmbeddedLanguages/<Lang>/<Lang>Tokenizer.cs` | Implement `IEmbeddedLanguageTokenizer` |
| 2 | `Highlighting/NestLightComposition.cs` | Register in `CreateEmbeddedLanguages()` |
| 3 | `Common/ClassificationNames.cs` | Add `template.<lang>.<role>` constants |
| 4 | `VisualStudio/EmbeddedLanguageClassificationDefinitions.cs` | Add type + format definitions |
| 5 | `Completion/Languages/<Lang>Completion.cs` | Derive from `CompletionLanguage` |
| 6 | `Completion/Languages/CompletionLanguages.cs` | Register in `CreateStandard()` |
| 7 | `VisualStudio/NestLightQuickInfo.cs` | Add descriptions in `DescriptionOf()` |
| 8 | Tests + docs | See above |

### Host language (minimum)

| Step | File | What |
|---|---|---|
| 1 | `Hosts/<Lang>HostScanner.cs` | Implement `IHostScanner` |
| 2 | `Highlighting/NestLightComposition.cs` | Add enum value + `CreateScanner` case |
| 3 | `VisualStudio/NestLightClassifier.cs` | Classifier provider with `[ContentType]` |
| 4 | `VisualStudio/NestLightCompletionSource.cs` | Completion provider with `[ContentType]` |
| 5 | `VisualStudio/NestLightQuickInfo.cs` | Quick-info provider with `[ContentType]` |
| 6 | `VisualStudio/NestLightDiagnostics.cs` | Diagnostics provider with `[ContentType]` |
| 7 | `VisualStudio/NestLightGoToDefinition.cs` | Add `[ContentType]` to listener + peek provider |
| 8 | Tests + docs | See above |

## License
MIT - see LICENSE.txt. Copyright (c) 2026 Raphael Augusto Teixeira Silva.
