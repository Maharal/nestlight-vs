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
| 4 | Avoid copying the whole text on every edit (`GetText()`, Large Object Heap allocation above ~85 KB) | **Tested, rejected for now** | Triggers gen2 collections, but the time saved is under 1 ms for files up to ~800 KB |
| 5 | Language registry built per buffer (`CreateHighlighter`) | **Tested, rejected** | 45 µs and 11 KB per buffer |
| 6 | `Highlight` itself allocates ~2x the text size per edit; reduce those allocations | Not tested (found while testing 4) | n/a |

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

## Hypothesis 4: the text copy (`GetText()`) on every edit

**Idea.** On every new snapshot the classifier copies the whole text into a string. Above ~85 KB (about 42k characters) that string lands on the Large Object Heap, which is only reclaimed by gen2 collections.

**Test.** Simulated typing on a synthetic C# file: 1000 edits (300 for the largest file), each one doing what the classifier does, in three variants: copy + `Highlight` (current), copy only, and `Highlight` only on an existing string. We measured time per edit, bytes allocated and GC collections. Same setup and caveats as the baseline.

| File | Copy + highlight | Copy only | Highlight only | Gen2 collections (copy + highlight / highlight only) |
|---|---:|---:|---:|---:|
| 78 KB | 0.32 ms | 0.01 ms | 0.24 ms | 0 / 0 |
| 195 KB | 0.63 ms | 0.24 ms | 0.39 ms | 62 / 0 |
| 781 KB | 2.5 ms | 0.19 ms | 1.5 ms | 208 / 0 |
| 1.9 MB | 6.2 ms | 0.67 ms | 4.0 ms | 447 / 1 |
| 7.8 MB (300 edits) | 22 ms | 3.4 ms | 18 ms | 148 / 55 |

**What it shows.**
- The mechanism is real: once the copy goes to the LOH, a gen2 collection happens every 2 to 5 edits, against almost none without it.
- The time cost is small. The copy itself takes 0.2 to 0.7 ms up to 2 MB, and removing it would save at most 0.1 to 1 ms per edit up to ~800 KB (about 25 to 40%), 2.2 ms at 1.9 MB. That is an upper bound: scanning directly over `ITextSnapshot` instead of a `string` would make every character access slower, so part of the gain would be lost.
- Typical large files (under 200 KB) spend 0.6 ms per edit in total.

**Decision: rejected for now.** The change touches every scanner and its tests for a gain that is below a millisecond on files of normal size.

**Caveat.** The benchmark process has a tiny heap. A gen2 collection in Visual Studio walks a heap of hundreds of megabytes, so each one costs more there than here, and the real penalty of the copy may be larger. Only a measurement inside Visual Studio (gen2 count and pause time while typing in a large file) can settle that.

## Hypothesis 5: the registry built per buffer

**Idea.** `CreateHighlighter` builds all the tokenizers again for every open file.

**Test.** 200 calls to `CreateLanguages()` (time and bytes), and 100 consecutive `CreateHighlighter` calls for each host.

**Result.** `CreateLanguages()` takes 45 µs (p99: 64 µs) and allocates 11 KB. A hundred buffers cost 3 to 6 ms and about 1.1 MB in total.

**Decision: rejected.** The cost per open file is too small to justify a shared registry.

## Hypothesis 6 (new): allocations inside `Highlight`

The test for hypothesis 4 showed that `Highlight` allocates about twice the size of the text per edit (1.5 MB for a 781 KB file), more than the copy itself. These are small objects, so the collections are mostly gen0 and cheap, but it is the largest allocation source in the pipeline. Candidates: one `Substring` per comment in `MarkerTracker.Comment`, the token and embedded-string lists, the `Decode` buffers.
- Test: an allocation profile of one `Highlight` call on a large file, to see which objects dominate before changing anything.
- Expectation: a modest gain. Worth it only if a few call sites account for most of the bytes.

## Hypotheses still to test

These two need a measurement inside Visual Studio, with a large real file and continuous typing: the isolated scan does not capture time spent waiting on the UI thread.

**2. Background re-analysis.** Today the analysis runs inside `GetClassificationSpans`, on the UI thread. Proposal: return the tokens of the previous snapshot and raise `ClassificationChanged` when the new analysis finishes.
- Test: typing latency (per keystroke) on files of 10k, 50k and 100k lines, before and after.
- Risk: briefly stale colors, and more concurrency complexity.

**3. Incremental analysis.** Re-analyze only the strings hit by the edit and reuse the tokens of the rest.
- Test: cost of a one-character edit in the middle of the file, compared to the full scan. Only worth it if (2) is not enough.
- Risk: the largest. Escapes, nested interpolations and unterminated strings shift everything after the edit point.
