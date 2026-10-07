# Experiments

A log of performance questions about NestLight. Each one is a hypothesis, an automated test that can answer it, a criterion written before the run, and what was observed.

The tests live in [NestLight.Experiments](../NestLight.Experiments), one class per experiment in `Experiments/`. A run writes a Markdown report with the tables and the analysis to `docs/reports/`; this file holds the definitions and the decisions.

The main question behind all of them: does NestLight slow Visual Studio down? Reading the code
([NestLightClassifier](../NestLight/VisualStudio/NestLightClassifier.cs), [SnapshotTokenCache](../NestLight/Highlighting/SnapshotTokenCache.cs)):
the number of languages is not the problem, because the tokenizers only run on marked strings.
What grows is the cost **per edit**, proportional to the file size: every new snapshot is copied (`GetText()`) and scanned, on the UI thread.

## How it works

```
dotnet run -c Release --project NestLight.Experiments            # all experiments, writes docs/reports/experiments-<date>-<commit>.md
dotnet run -c Release --project NestLight.Experiments -- --only E03,E07
dotnet run -c Release --project NestLight.Experiments -- --list
dotnet run -c Release --project NestLight.Experiments -- --quick  # smoke run, numbers not worth keeping
```

The *Experiments* workflow runs the same suite on Windows, on `net48` (the runtime of Visual Studio), and uploads the report.

## How to read this file

- A **result is a snapshot**, valid for one commit, one runtime and one machine. The code changes, so results go stale: a past run is never edited, a new run adds a new report.
- **Absolute times move between sessions** even on the same machine. The numbers below are ratios and orders of magnitude from runs on one machine; compare numbers only inside one report.
- **Run at least three times** before trusting a verdict. A verdict that flips between identical runs sits at the edge of the noise (E09 does), and the report of a single run cannot show that.
- The **hypothesis, the test and the criterion** are the stable part of an experiment. If one of them changes, the experiment is replaced: the old id is closed and a new one opened. (E03's criterion was restated when it was automated, and E06 was rewritten twice before any of its results was recorded; see their notes.)
- A **decision** says why, and a **Revisit when** line says what would make the old answer wrong: a code change, a bigger file, a new host.
- *Criterion met* is the statement of the experiment's own criterion, not a grade. For an optimization, met means it is worth doing; for a risk (E06 to E09), met means the plugin is healthy.
- Statuses: `Planned`, `Done`, `Closed`. Decisions: `Adopted`, `Rejected`, `Inconclusive`.

All the criteria below were written before the run that decided them, except those of E01 to E04, which were first measured with throwaway code and then restated when the experiments were automated.

## Index

| Id | Question | Test | Status | Decision |
|---|---|---|---|---|
| [E01](#e01-baseline-cost-of-one-highlight-call) | How much does one `Highlight` call cost? | Automated | Done | Baseline |
| [E02](#e02-skip-the-scan-when-the-text-has-no-language-id) | Does skipping the scan without language ids help? | Automated | Done | Rejected |
| [E03](#e03-the-text-copy-on-every-edit) | How much does the `GetText()` copy cost? | Automated | Done | Rejected for now |
| [E04](#e04-the-language-registry-built-per-buffer) | Does building the registry per buffer matter? | Automated | Done | Rejected |
| [E05](#e05-where-highlight-allocates) | Where do the allocations of `Highlight` come from? | Automated | Done | Points at the scan |
| [E06](#e06-number-of-marked-strings) | Does the cost stay linear as the strings multiply? | Automated | Done | Not linear above ~400k characters |
| [E07](#e07-many-interpolations-in-one-string) | Does one string with many interpolations scale? | Automated | Done | **Quadratic** |
| [E08](#e08-throughput-of-each-embedded-language) | Is any tokenizer much slower than the others? | Automated | Done | No outlier; all superlinear on large strings |
| [E09](#e09-malformed-and-pathological-input) | Does bad input make the cost explode? | Automated | Done | Inconclusive (borderline) |
| [E10](#e10-typing-latency-and-gc-inside-visual-studio) | What does the user feel while typing in a large file? | Manual | Planned | |
| [E11](#e11-background-re-analysis) | Does analyzing off the UI thread improve typing latency? | Manual | Planned | |
| [E12](#e12-incremental-analysis) | Is re-analyzing only the edited strings worth it? | Both | Planned | |
| [E13](#e13-clip-tokens-without-rescanning-the-interpolations) | Does fixing the quadratic clipping make E07 linear? | Automated | Planned | |
| [E14](#e14-fewer-allocations-in-the-scan) | How much does the scan's allocation fall if comments stop allocating? | Automated | Planned | |

## Latest runs

Three consecutive full runs on commit `6a7f9ff`, 2026-10-07, .NET 10 in Release (tiered compilation off, 16 logical cores, a development machine). No run of the suite on `net48` yet: the first one comes from the *Experiments* workflow.

| Id | Run 1 | Run 2 | Run 3 | Criterion |
|---|---|---|---|---|
| E01 | 10.1 ms | 10.1 ms | 10.4 ms | Met in all |
| E02 | 2.63x / 0.83x | 2.63x / 0.85x | 2.68x / 0.82x | Not met in all |
| E03 | 361 µs | 348 µs | 336 µs | Not met in all |
| E04 | 15 µs, 11 KB | 15 µs, 11 KB | 15 µs, 11 KB | Met in all |
| E05 | 1.74x text, 85% | 1.74x text, 85% | 1.74x text, 85% | Met in all |
| E06 | 18.3x | 16.4x | 18.7x | Not met in all |
| E07 | 84.8x | 81.9x | 97.5x | Not met in all |
| E08 | 22.1x | 21.1x | 23.4x | Not met in all |
| E09 | 2.41x | 2.45x | 2.58x | **Met, met, not met** |

(E01: worst case at 60k lines. E02: best speed-up without markers / worst result with them. E03: time the copy could save per edit at 400k characters. E05: allocation as a multiple of the text, and the share of the dominant stage. E06: time for 8x the strings. E07: time for 10x the interpolations. E08: worst time for 10x the text. E09: worst time for 2x the input.)

## E01: baseline cost of one `Highlight` call

**Status:** Done · **Decision:** Baseline

**Hypothesis.** Tokenizing is cheap enough that the number of languages and the scan of the host are not what could make the editor slow.

**Test.** `Highlight(text)` (host scan + tokenization) for the 4 hosts, with no marked string and with one unit in 10 carrying a marked string, at 1.2k, 12k and 60k lines.

**Criterion.** Every case stays under 16 ms (one frame) at 60k lines.

**Result.** Met: the worst case is 10 ms at 60k lines, a file far larger than the ordinary. At 12k lines the cost is 1 to 2 ms.

**Decision.** One `Highlight` call is not what makes typing slow on files of ordinary size. Anything that matters has to come from the very large files or from outside the scan.

**Revisit when.** A host or a tokenizer is added, or the scan changes shape.

## E02: skip the scan when the text has no language id

**Status:** Done · **Decision:** Rejected

**Hypothesis.** A string is only embedded code when a tag or comment carries a known language id. If the text contains none of the 17 ids, the scan could not find anything, so skipping it saves time on C# and Python, which have no shortcut of their own.

**Test.** A decorator around the real highlighter looks for each id with `IndexOf(OrdinalIgnoreCase)` and returns no tokens when none appears. Same files, with and without the decorator, measured in the same process.

**Criterion.** At least 2x faster on every file without marked strings, and no more than 1.1x slower on every file with them.

**Result.** Not met in the three runs. Python without markers: 2.6x faster. C# without markers: only 1.3x to 1.5x. With markers: 0.82x to 0.93x, that is, 7% to 18% slower, because the file pays for the searches and then for the full scan.

**Decision.** Rejected. The gain exists only where the scan already takes a fraction of a millisecond, and the cost lands on the files that use the plugin.

**Correction.** The first measurement of this hypothesis compared two separate processes and reported up to 3.9x slower with markers. The same-process comparison used here shows 1.1x to 1.2x. The conclusion stands; the size of the penalty did not.

**Revisit when.** The scan becomes much more expensive per character, or the ids are searched in a single pass.

## E03: the text copy on every edit

**Status:** Done · **Decision:** Rejected for now

**Hypothesis.** On every new snapshot the classifier copies the whole text into a string. Above ~85 KB (about 42k characters) that string lands on the Large Object Heap, which only gen2 collections reclaim: the copy costs time, and the collections cost more.

**Test.** Simulated typing on a synthetic C# file, 300 edits per size. Each edit copies the text into a new string and highlights it. Three variants: copy + `Highlight` (current), copy only, `Highlight` only on an existing string. Mean time per edit, so that GC pauses count.

**Criterion.** Removing the copy would save at least 1 ms per edit on the 400,000-character file. *(Restated when automated: the first wording said "1 ms or 30%" and mixed the two.)*

**Result.** Not met: the possible saving is 336 to 361 µs per edit at 400k characters (21%). It reaches 1.2 ms at 1.9 MB (24%) and 3 ms at 7.6 MB (15%). The mechanism is real: with the copy, 73 gen2 collections in 300 edits at 781 KB, against none without it.

**Decision.** Rejected for now: the saving is below a millisecond on files of ordinary size, and it is an upper bound, since scanning over `ITextSnapshot` instead of a `string` would make every character access slower.

**Caveat.** The process has a tiny heap. A gen2 collection in Visual Studio walks a far larger one, so the real cost of the collections is probably higher. E10 measures that.

**Revisit when.** E10 shows gen2 pauses while typing, or the scanners are rewritten to take a span or a snapshot.

## E04: the language registry built per buffer

**Status:** Done · **Decision:** Rejected

**Hypothesis.** `CreateHighlighter` builds all the tokenizers again for every open file, and that cost adds up.

**Test.** `CreateLanguages()` 200 times, and 100 consecutive `CreateHighlighter` calls for each host.

**Criterion.** Opening a buffer costs less than 1 ms and 1 MB, for every host.

**Result.** Met: 15 µs and 11 KB per buffer.

**Decision.** Rejected: a shared registry would save nothing noticeable.

**Revisit when.** The tokenizers multiply, or one starts building large tables in its constructor.

## E05: where `Highlight` allocates

**Status:** Done · **Decision:** Points at the scan

**Hypothesis.** `Highlight` allocates about twice the size of the text per call, and one of its two stages, the host scan or the rest (decoding, tokenizing, mapping back), accounts for most of it.

**Test.** Bytes allocated by the host scan alone and by the whole call on the same file, with and without marked strings, for the 4 hosts at 12k lines.

**Criterion.** On every host, with marked strings, one stage accounts for at least 70% of the bytes.

**Result.** Met, identically in the three runs. With marked strings `Highlight` allocates 1.2x to 2.3x the size of the text (1.74x on average), and the scan accounts for 85% to 88% of it. The scan allocates more than the text itself even **without any marked string** (C#: 1.2 MB for a 680 KB file), so what it allocates is not tied to the embedded code it finds.

**Decision.** The allocation is in the scan, and it is independent of what the file contains. The stage split is as far as this test goes: the call sites need an allocation profile. The comment handling is the first suspect (a `Substring`, a `Trim` and a `ToLowerInvariant` for each comment): see E14.

**Revisit when.** E14 is run.

## E06: number of marked strings

**Status:** Done · **Decision:** Not linear above ~400k characters

**Hypothesis.** The cost grows only linearly with the number of embedded strings: nothing in the engine (a sort, a nested scan) grows faster than the number of strings it has to handle.

**Test.** A file where every unit carries a marked string, at 100k, 200k, 400k and 800k characters, for each host. Time and gen2 collections.

**Criterion.** 8 times more embedded strings cost less than 10 times the time (linear is 8x, quadratic is 64x), for every host.

**Result.** Not met in the three runs: 16x to 19x for 8x the strings. Up to 200k characters the cost is linear (about 2 µs per string). At 400k it starts to rise and at 800k it is about 4 µs per string. The gen2 collections follow the same curve: none at 100k and 200k, 1 to 5 in 25 runs at 400k and 800k. That is a correlation, not a proof of cause.

**Decision.** The engine is not quadratic in the number of strings, but a file of 800k characters (about 25k lines) costs twice as much per string as one of 200k. That does not hurt files of ordinary size.

**Note.** The first version of this experiment (a fixed file with a growing density of marked strings) was replaced before any result was recorded: it measured a difference between two noisy times of about 1 ms and its verdict changed between identical runs. A later version also gave an optimistic result when run alone, because the first experiments ran on code the JIT had not optimized yet; tiered compilation is now off, which also matches `net48`.

**Revisit when.** E14 reduces the allocations: if the gen2 collections and the extra cost go away together, the cause was the garbage collector.

## E07: many interpolations in one string

**Status:** Done · **Decision:** Quadratic

**Hypothesis.** The engine clips every token against the interpolations of its string starting from the first one, so a single large template with many interpolations costs O(tokens x interpolations). Suspected from reading `HighlightEngine.AddClipped`.

**Test.** One marked HTML string with 100, 1,000 and 10,000 interpolations, in JavaScript, C# and Python.

**Criterion.** Ten times more interpolations cost less than 20 times the time (linear is 10x, quadratic is 100x), for every host.

**Result.** Not met, and the clearest finding of the suite: 10x the interpolations cost 82x to 98x the time, the same in the three hosts. 100 interpolations take 0.16 ms, 1,000 take 7 ms and 10,000 take 550 to 600 ms.

**Decision.** The cost is quadratic in the number of interpolations of one string. It is invisible below a few hundred interpolations and a freeze of half a second at 10,000. The experiment does not isolate the cause: `AddClipped` is the suspect, not a proven culprit. E13 tests the fix.

**Revisit when.** E13 is run.

## E08: throughput of each embedded language

**Status:** Done · **Decision:** No outlier; all superlinear on large strings

**Hypothesis.** All the tokenizers run at a similar speed in characters per millisecond, and none gets slower per character on larger inputs.

**Test.** One marked string per language, in a JavaScript host, of 100,000 and of 1,000,000 characters made by repeating a snippet of typical code of that language.

**Criterion.** The slowest language is less than 5 times slower than the median one, and every language takes less than 12 times longer on 10 times the text.

**Result.** Not met in the three runs, but only the second half. The slowest language (SQL or YAML) is 1.5x to 1.6x below the median, so there is no outlier. All 11 languages take 13.6x to 23x longer on 10x the text, not just one: Markdown, GraphQL and GLSL sit at the low end, SQL, JSON and XML at the high end.

**Decision.** No tokenizer is an outlier. The superlinear growth is common to all of them, which points to the shared engine (large buffers, the garbage collector) more than to any tokenizer, as in E06. The test does not separate the two.

**Revisit when.** E14 reduces the allocations.

## E09: malformed and pathological input

**Status:** Done · **Decision:** Inconclusive (borderline)

**Hypothesis.** Code being typed is often malformed. An unterminated string or comment early in a large file, deep nesting, or a very long line can make the scan or a tokenizer quadratic.

**Test.** For each host: an unterminated marked string and an unterminated block comment at the top of a file of 250,000 characters, deep nesting (200 and 400 levels), and one line of 250,000 characters. Each case at N and at 2N. Cases under 5 ms at 2N are ignored.

**Criterion.** Doubling the input multiplies the time by less than 2.5 (linear is 2, quadratic is 4) in every case.

**Result.** Met, met, not met (worst growth 2.41x, 2.45x and 2.58x). Only one case matters: an unterminated marked string at the top, where the whole file becomes one embedded string of 250k to 500k characters. Everything else is linear, and the block comment, the long line and the nesting are too fast to measure. That case grows 1.9x to 2.6x depending on the host and the run, with 6 gen2 collections at N and 8 to 9 at 2N.

**Decision.** Inconclusive: the verdict flips on noise, so there is no quadratic here, but a mild superlinear effect cannot be ruled out. The size of an unterminated string is the same situation as E06 and E08.

**Revisit when.** E14 reduces the allocations. A criterion of 2.5 is too close to the noise to separate a mild effect: a larger N or more runs would help.

## E10: typing latency and GC inside Visual Studio

**Status:** Planned

**Hypothesis.** In a real session, the gen2 collections caused by the text copy (E03) and the analysis on the UI thread are enough to make typing in a large file noticeable.

**Test (manual).** Open a large real file (generated code, a JS bundle of 5k to 50k lines), type continuously, and record the per-keystroke latency and the gen2 count and pause time with and without the extension. A fixed file and script, kept with the result, so runs can be repeated.

**Criterion.** A p95 latency increase under 8 ms, and no gen2 pause above 20 ms.

## E11: background re-analysis

**Status:** Planned (depends on E10 showing a problem)

**Hypothesis.** Returning the tokens of the previous snapshot and raising `ClassificationChanged` when the new analysis finishes takes the scan off the UI thread and improves typing latency.

**Test (manual).** The E10 session before and after the change.

**Criterion.** The p95 latency in E10 goes back to within 2 ms of the baseline without the extension.

**Risks.** Colors stale for an instant, and more concurrency complexity.

## E12: incremental analysis

**Status:** Planned (depends on E11 not being enough)

**Hypothesis.** Re-analyzing only the strings hit by the edit and reusing the tokens of the rest is much cheaper than the full scan.

**Test.** Automated: the cost of a one-character edit in the middle of a file against the full scan, over the E01 sizes. Manual: the E10 session. Plus a differential test: after any sequence of edits, the incremental tokens must equal the tokens of a full scan.

**Criterion.** At least 5x cheaper per edit at 60k lines, and the differential test passes on the existing samples and on random edits.

**Risks.** The largest of all: escapes, nested interpolations and unterminated strings shift everything after the edit point.

## E13: clip tokens without rescanning the interpolations

**Status:** Planned (follows E07)

**Hypothesis.** E07 is quadratic because `AddClipped` walks the interpolations of the string from the first one for every token. Resuming from where the previous token ended makes the cost linear.

**Test.** Change the clipping, then run E07 again, and run the unit tests of the engine and the robustness tests to check that the tokens are exactly the same.

**Criterion.** E07 criterion met (10x the interpolations cost less than 20x the time), no token changes in the existing tests, and no slowdown above 5% in E01.

## E14: fewer allocations in the scan

**Status:** Planned (follows E05)

**Hypothesis.** The scan allocates more than the size of the text even when there is nothing to find, mostly in the handling of comments (a `Substring`, a `Trim` and a `ToLowerInvariant` for each one). Parsing the marker on the text itself, without creating strings, removes most of it. If E06, E08 and E09 are limited by the garbage collector, they improve together.

**Test.** An allocation profile of the scan on a large file to confirm the call sites, then the change, then E05 again, and E06, E08 and E09 to see whether the superlinear growth goes away.

**Criterion.** E05: the scan allocates less than 0.5x the size of the text without marked strings, and no token changes in the existing tests.
