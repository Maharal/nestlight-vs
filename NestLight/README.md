# NestLight (Visual Studio 2022/2026)

The extension project. See the [main README](../README.md) for what it highlights and how to use it.

## Layout
- `Common/`: model, interfaces, marker rules, text helpers
- `Hosts/`: scanners for JavaScript / TypeScript, C#, Python and C++
- `EmbeddedLanguages/`: tokenizers for HTML, CSS, SQL, JSON, GraphQL, XML, Markdown, YAML, regular expressions and shaders (GLSL, WGSL)
- `Highlighting/`: engine, language registry, snapshot cache and the composition root (`NestLightComposition`)
- `VisualStudio/`: MEF classifier providers, classification types and default formats

Dependencies are passed through constructors. Only `VisualStudio/` references the Visual Studio SDK.

## Build
1. Open `NestLight.csproj` (workload "Visual Studio extension development").
2. F5 opens the experimental instance.
3. Release build -> `bin\Release\NestLight.vsix`.

## Colors
Tools > Options > Environment > Fonts and Colors > "Text Editor" > items "Template <Language> ...".

## Adding a language
1. Write a tokenizer in `EmbeddedLanguages/` (`IEmbeddedLanguageTokenizer`) and add it to `NestLightComposition.CreateEmbeddedLanguages`.
2. Add its names to `Common/ClassificationNames.cs` and declare the types and default colors in `VisualStudio/`.
   A test fails until every name has both.

## Adding a host
1. Write a scanner in `Hosts/` (`IHostScanner`) and add a `HostLanguage` value and a case in `NestLightComposition.CreateScanner`.
2. Add a classifier provider in `VisualStudio/NestLightClassifier.cs` with the host's content type.

## License
MIT - see LICENSE.txt. Copyright (c) 2026 Raphael Augusto Teixeira Silva.
