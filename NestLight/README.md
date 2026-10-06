# NestLight (Visual Studio 2022/2026)

Highlights HTML and CSS inside template strings in .ts/.js files.

## What is recognized
**HTML:** `html\`...\``, `svg\`...\``, `htm\`...\``, `/* html */ \`...\`` (and `/* language=html */`)
- tags, attributes, `@event`, `.property`, `?boolean`, comments and `${...}` expressions
- CSS inside `<style>...</style>` and `style="..."`

**CSS:** `css\`...\``, `/* css */ \`...\`` (and `/* language=css */`)
- selectors, classes/ids, pseudo-classes, properties, `--custom-properties`, values, numbers/colors,
  functions (`var()`, `calc()`...), `@media`/`@keyframes`..., `!important`, strings, comments
- CSS nesting (`&:hover { }`)

Works with Lit, uhtml, htm and similar libraries. The extension only reads the buffer (classification);
it does not edit code.

## Build
1. Open `NestLight.csproj` (workload "Visual Studio extension development").
2. F5 opens the experimental instance; open `sample.ts`.
3. Release build -> `bin\Release\NestLight.vsix`.

## Colors
Tools > Options > Environment > Fonts and Colors > "Text Editor" > items "Template HTML ..." and "Template CSS ...".

## Adding other tags
In `TplScanner.cs`, method `KindFromName`, add the tag name (for example `styled`) to the desired group.

## License
MIT - see LICENSE.txt. Copyright (c) 2026 Raphael Augusto Teixeira Silva.
