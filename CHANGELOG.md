# Changelog

All notable changes to this project are documented here.
The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Context ranking: the words that already followed the same word in the document come first (`FROM` then the table that followed `FROM` before, `display:` then its value). Experiment EA25: the word is within the first 5 in 85.5% of the cases instead of 72.5% (generated code).
- Context ranking: the words found in the code of strings of the same language come before the words of the host code and of other languages, and the previous-word context is taken from them (experiment EA26, +2.6 points within the first 5 on top of EA25).
- Context ranking by the place in the grammar of SQL, CSS and HTML: a table after `FROM`, `BY` after `GROUP`, the values of a property after its colon, the attributes of a tag inside `<button `; what does not belong comes last, nothing is dropped (experiment EA27, 88.1% to 97.2% within the first 5 on generated code). Selectors in CSS now offer the HTML tags.
- A reader of the schema of the SQL in the document (`CREATE TABLE`, `FROM` / `JOIN` and aliases, `INSERT` and `UPDATE` columns) behind `CompletionFeatures.Schema`, **off**: experiment EA28 gave +0.3 points within the first 5 on top of the rest and a session of 6-16 ms at 60,000 lines, so it did not meet its criterion.
- An order of the words by count and distance (`WordOrder.Frequency` and `WordOrder.Blend`), **off**: experiment EA29 found no gain over the nearest first.
- Similar words in the completion: when nothing starts with what was typed (3 or more letters), the keywords and the words of the document that are one or two edits away are offered after the exact ones (`SELCT` offers `SELECT`). Experiments EA21 to EA24; EA24 replaces EA16.
- Completion inside embedded strings: keywords of the language of the string (SQL, CSS, HTML / SVG, GraphQL, JSON, YAML, GLSL, WGSL) and words that already exist in the document, nearest to the caret first.
- Completion of the CSS inside HTML and SVG strings: the content of `<style>` and the value of `style="..."` are completed as CSS (properties, values, pseudo-classes, `@media`, the words of every style in the file).
- Vocabulary the review of 800 suggestions found missing: `main` and the `gl_` variables of GLSL; directives and the `#version` profile of GLSL; the attributes, builtin values, interpolation, address spaces and access modes of WGSL; the ARIA roles and the values of more attributes in HTML and SVG; more values of CSS properties.

### Changed
- CSS, in general: the tokenizer colors the punctuation of a selector as punctuation, reads `:not()` / `:is()` / `:nth-child()` and attribute selectors in parts, names the features of `@media` / `@supports` / `@container` like properties and reads `1e3ms` as one number. The completion offers at-rule names, `!important`, units after a number, the named colors, 200 more properties (the properties come in the order of use) and the attributes of an attribute selector.
- CSS colors: `#id`, the attribute of an attribute selector, the unit of a number (`px`, `rem`, `%`) and `!important` have their own classifications (user-adjustable in Fonts and Colors), and the CSS palette keeps at least 3.5:1 of contrast on both a white and a dark editor (before, the colors were meant for the dark theme only and some were below 3:1 on white). On the dark theme the colors are a little darker. Other languages keep their palette.
- The stage that corrects mistakes no longer offers keywords or tag names where none belongs (text, attribute values, JSON and YAML keys, literals and comments): it offers only words of the file there.
- Words of two letters (`id`, `db`, `in`, `uv`) are offered, after all the longer words (experiments EA33 and EA35). On the 800 hand-written examples the words of 2 letters within the first 5 go from 77.8% to 97.4%, and the longer words do not change. The similar-words stage keeps looking from 3 letters: no gate that was tried (EA34) removes its noise with a new 3-letter word (31% of cases in the corpus) without losing most of its recovery of mistakes.
- Where the place of the caret has no rule (GLSL, WGSL, GraphQL, and the places the rules do not know) the words of the file come before the keywords, and the keywords are offered in the order of how much code uses them, with the 12 most used still in front (experiment EA32; hand-written examples: 58.3% to 69.4% within the first 5). The order is learned from a generated corpus (`KeywordUse`).
- Corrections found by a review of 800 suggestions (`docs/suggestion-review`): after `GROUP BY` the next clauses; the clause of a parenthesis is its own; nothing but words of the file inside literals, comments, class names, JSON keys and strings, YAML keys, attribute values and text; columns, types and constraints in `CREATE TABLE`; pseudo-classes, at-rules, `transition`/`animation` values and functions in CSS; the open element after `</`; attributes the tag already has are not offered again; the word already after the caret no longer counts as following the context. On the same 800 examples the word is first in 303 cases instead of 281.
- The classifier and the completion share one scan per snapshot, and the completion creates a word only when it offers it: the start of a session on a 60,000-line file falls from 12-32 ms to under 3 ms (EA20).

### Experiments
- A generated corpus of 500 snippets for each of the 8 languages (`--corpus`), EA31 and EA32 for the order of the keywords, and EA25 to EA30 for the context rankings (previous word, language, grammar, SQL schema, count and distance, robustness) and a generated corpus with structure.
- EA14 to EA20 for the completion (latency, limits, robustness, vocabulary against tokenizers, order and scope of the words) and a seeded generator of code for them.

## [0.1.0] - 2026-10-06

First build of the extension, published as a pre-release.

### Added
- Highlighting of languages embedded in strings: HTML, CSS, SQL, JSON, GraphQL, XML, Markdown, YAML, regular expressions and shaders (GLSL, WGSL).
- Hosts: JavaScript / TypeScript, C#, Python and C++.
- Strings are marked by a tag or a marker comment; interpolations are left to the host code.
- CI that runs the tests and builds the VSIX with a test report.

[Unreleased]: https://github.com/Maharal/nestlight-vs/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/Maharal/nestlight-vs/releases/tag/v0.1.0
