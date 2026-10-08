# Changelog

All notable changes to this project are documented here.
The format follows [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and the project follows [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
- Context ranking: the words that already followed the same word in the document come first (`FROM` then the table that followed `FROM` before, `display:` then its value). Experiment E28: the word is within the first 5 in 85.5% of the cases instead of 72.5% (generated code).
- Similar words in the completion: when nothing starts with what was typed (3 or more letters), the keywords and the words of the document that are one or two edits away are offered after the exact ones (`SELCT` offers `SELECT`). Experiments E23 to E27; E27 replaces E18.
- Completion inside embedded strings: keywords of the language of the string (SQL, CSS, HTML / SVG, GraphQL, JSON, YAML, GLSL, WGSL) and words that already exist in the document, nearest to the caret first.

### Changed
- The classifier and the completion share one scan per snapshot, and the completion creates a word only when it offers it: the start of a session on a 60,000-line file falls from 12-32 ms to under 3 ms (E22).

### Experiments
- E16 to E22 for the completion (latency, limits, robustness, vocabulary against tokenizers, order and scope of the words) and a seeded generator of code for them.

## [0.1.0] - 2026-10-06

First build of the extension, published as a pre-release.

### Added
- Highlighting of languages embedded in strings: HTML, CSS, SQL, JSON, GraphQL, XML, Markdown, YAML, regular expressions and shaders (GLSL, WGSL).
- Hosts: JavaScript / TypeScript, C#, Python and C++.
- Strings are marked by a tag or a marker comment; interpolations are left to the host code.
- CI that runs the tests and builds the VSIX with a test report.

[Unreleased]: https://github.com/Maharal/nestlight-vs/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/Maharal/nestlight-vs/releases/tag/v0.1.0
