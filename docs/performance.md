# Performance: hypotheses and tests

Starting question: does NestLight slow Visual Studio down because it supports many languages?
Reading the code ([NestLightClassifier](../NestLight/VisualStudio/NestLightClassifier.cs), [SnapshotTokenCache](../NestLight/Highlighting/SnapshotTokenCache.cs)):
the cost of having 11 registered languages is negligible (the tokenizers only run on marked strings).
What grows is the cost **per edit**, proportional to the file size: on every new snapshot the whole text is copied (`GetText()`) and scanned, on the UI thread.

| # | Hypothesis | Status | Result |
|---|---|---|---|
| 1 | Skip the scan when the text contains no language id | **Tested, rejected** | No relevant gain; slower for files with markers |
| 2 | Re-analyze in the background and answer with the previous tokens (debounce) | Not tested | n/a |
| 3 | Incremental analysis: re-analyze only the strings touched by the edit | Not tested | n/a |
| 4 | Avoid copying the whole text on every edit (`GetText()`, Large Object Heap allocation above ~85 KB) | Not tested | n/a |
| 5 | Language registry built per buffer (`CreateHighlighter`) | Not tested (cost expected to be irrelevant) | n/a |

## Baseline: the scan is already cheap

We measured `Highlight(text)` (host scan + tokenization) on synthetic files of the 4 hosts, without and with marked strings.

- **Method:** the test project in Release, on a modern runtime (faster than the `net48` of Visual Studio: the numbers are for comparing versions, not absolute times).
  Each case has 5 warm-up runs and 25 measured runs; the median is reported.
- **Files:** 1.2k, 12k and 60k lines, repeating a unit of typical host code (comments, plain strings, interpolation).
  In the "with marker" cases, 1 unit in 10 has a marked string (`// language=sql`, `html\`...\``, `R"(...)"`).
- **Limit:** synthetic code, not real files.

| Host | 1.2k lines | 12k lines | 60k lines |
|---|---:|---:|---:|
| JavaScript, no marker | 0.20 ms | 2.1 ms | 8.7 ms |
| C#, no marker | 0.22 ms | 2.2 ms | 6.7 ms |
| Python, no marker | 0.55 ms | 5.8 ms | 8.3 ms |
| C++, no marker | 0.24 ms | 2.1 ms | 4.7 ms |
| C#, with marker | 0.13 ms | 1.9 ms | 8.5 ms |
| Python, with marker | 0.18 ms | 2.1 ms | 10.2 ms |

Conclusion: tokenizing is not the bottleneck. Any gain has to come from something that costs more than this, or from outside the scan.

## Hypothesis 1: shortcut "no language id in the text, no scan"

**Idea.** A string is only embedded code when a tag or comment carries a known id (`html`, `sql`, `json`...). If the text contains none of the 17 ids, the scan could not find anything. JavaScript and C++ already have similar shortcuts (no `` ` `` / no `R"`); C# and Python have none.

**Test.** Prototype in `HighlightEngine.Highlight`: before the scan, `text.IndexOf(id, OrdinalIgnoreCase)` for each id; if none appears, return no tokens. Same benchmark as the baseline, before and after.

**Result (median):**

| Case | Baseline | With shortcut |
|---|---:|---:|
| C#, 1.3k lines, no marker | 0.22 ms | 0.04 ms |
| C#, 13.5k lines, no marker | 2.2 ms | 0.45 ms |
| C#, 67k lines, no marker | 6.7 ms | 3.9 ms |
| JS, no marker (the code uses `JSON.stringify`) | 0.20 ms | 0.25 ms |
| C#, 1.3k lines, **with** marker | 0.13 ms | 0.33 ms |
| C#, 13.6k lines, **with** marker | 1.9 ms | 2.8 ms |
| Python, 1.4k lines, **with** marker | 0.18 ms | 0.70 ms |
| Python, 13.6k lines, **with** marker | 2.1 ms | 5.2 ms |

**Decision: rejected.**
- The gain shows up where the scan already costs a fraction of a millisecond.
- With markers, the file pays for the 17 searches and then for the full scan (up to 3.9x slower).
- Short ids (`json`, `md`, `xml`) show up in ordinary code and cancel the shortcut, as in the JS case.

## Hypotheses to test

For the first three, a meaningful measurement only exists inside Visual Studio, with a large real file (generated code, a JS bundle) and continuous typing. The isolated scan does not capture allocation cost or time spent waiting on the UI thread.

**2. Background re-analysis.** Today the analysis runs inside `GetClassificationSpans`, on the UI thread. Proposal: return the tokens of the previous snapshot and raise `ClassificationChanged` when the new analysis finishes.
- Test: typing latency (per keystroke) on files of 10k, 50k and 100k lines, before and after.
- Risk: briefly stale colors, and more concurrency complexity.

**3. Incremental analysis.** Re-analyze only the strings hit by the edit and reuse the tokens of the rest.
- Test: cost of a one-character edit in the middle of the file, compared to the full scan. Only worth it if (2) is not enough.
- Risk: the largest. Escapes, nested interpolations and unterminated strings shift everything after the edit point.

**4. Text copy (`GetText()`).** On files above ~85 KB the copy lands on the Large Object Heap on every keystroke.
- Test: count gen2 GC collections and bytes allocated per edit on a 1 MB file, with and without the copy. One variant is scanning directly over the `ITextSnapshot`, without materializing the string.
- Risk: the scanners currently take a `string`; changing that touches every host and its tests.

**5. Registry per buffer.** `CreateHighlighter` builds ~11 tokenizers for every open file.
- Test: time and memory of `CreateLanguages()` and of opening 100 buffers.
- Expectation: irrelevant. If confirmed, the registry can become a singleton at no maintenance cost.
