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
| [E07](#e07-many-interpolations-in-one-string) | Does one string with many interpolations scale? | Automated | Done | **Quadratic**, fixed by E13 |
| [E08](#e08-throughput-of-each-embedded-language) | Is any tokenizer much slower than the others? | Automated | Done | No outlier; all superlinear on large strings |
| [E09](#e09-malformed-and-pathological-input) | Does bad input make the cost explode? | Automated | Done | Inconclusive (borderline) |
| [E10](#e10-typing-latency-and-gc-inside-visual-studio) | What does the user feel while typing in a large file? | Manual | Planned | |
| [E11](#e11-background-re-analysis) | Does analyzing off the UI thread improve typing latency? | Manual | Planned | |
| [E12](#e12-incremental-analysis) | Is re-analyzing only the edited strings worth it? | Both | Planned | |
| [E13](#e13-clip-tokens-without-rescanning-the-interpolations) | Does fixing the quadratic clipping make E07 linear? | Automated | Done | Adopt: criterion met |
| [E14](#e14-fewer-allocations-in-the-scan) | How much does the scan's allocation fall if comments stop allocating? | Automated | Done | Modest gain; criterion not met |
| [E15](#e15-why-the-cost-grows-faster-than-the-text-above-400k-characters) | What makes the cost superlinear on very large strings? | Automated | Planned | |
| [E16](#e16-completion-latency-against-the-size-of-the-file) | Is completion fast on large files? | Automated | Done | Not met above ~1M characters; no change |
| [E17](#e17-do-the-limits-of-the-completion-change-its-cost) | Do the limits of the completion change its cost? | Automated | Done | Only 10,000 suggestions cost more; defaults kept |
| [E18](#e18-completion-on-incomplete-and-cut-code) | Can completion be triggered anywhere in a file being edited? | Automated | Closed | Met: 0 violations; replaced by E27 |
| [E19](#e19-does-the-tokenizer-agree-with-the-vocabulary) | Does the tokenizer agree with the vocabulary? | Automated | Done | Met |
| [E20](#e20-order-of-the-words-of-the-document) | Is nearest-first the best order for the words of the document? | Automated, generated code | Done | Kept; the advantage depends on the generator |
| [E21](#e21-where-the-words-come-from-and-how-many-keystrokes-completion-saves) | Which words should completion offer? | Automated, generated code | Done | Inconclusive |
| [E22](#e22-sharing-the-scan-and-not-creating-the-words-of-the-completion) | Does sharing the scan and not creating the words bring completion under a frame? | Automated | Done | Adopt: criterion met |
| [E23](#e23-does-the-second-stage-of-the-completion-fit-in-a-frame) | Does the second stage of the completion fit in a frame? | Automated | Done | Inconclusive (borderline in one extreme case) |
| [E24](#e24-does-the-second-stage-recover-the-word-after-one-mistake-and-which-tie-break-works) | Does the second stage recover the word after one mistake? | Automated, generated code | Planned | |
| [E25](#e25-does-the-second-stage-get-in-the-way-when-the-prefix-is-right) | Does it get in the way when the prefix is right? | Automated, generated code | Planned | |
| [E26](#e26-do-the-similar-suggestions-show-up-in-visual-studio) | Do the similar suggestions show up in Visual Studio? | Manual | Planned (needs a build) | |
| [E27](#e27-completion-with-similar-words-on-incomplete-and-cut-code) | Is completion still robust with the second stage? | Automated | Done | Met: 0 violations |
| [E28](#e28-does-the-word-before-the-caret-help-to-rank-the-suggestions) | Does the word before the caret help to rank the suggestions? | Automated, generated code | Done | Adopted: criterion met |
| [E29](#e29-do-the-words-of-the-same-language-come-first) | Do the words of the same language come first? | Automated, generated code | Done | Adopted: criterion met |
| [E30](#e30-does-the-place-in-the-grammar-help-to-rank-the-suggestions) | Does the place in the grammar help to rank the suggestions? | Automated, generated code | Done | Adopted: criterion met |
| [E31](#e31-does-the-schema-read-from-the-sql-of-the-document-help) | Does the schema read from the SQL of the document help? | Automated, generated code | Done | Not adopted: criterion not met |
| [E32](#e32-do-the-words-used-most-often-come-before-the-nearest-ones) | Do the words used most often come before the nearest ones? | Automated, generated code | Done | Not adopted: criterion not met |
| [E33](#e33-completion-with-the-context-rankings-on-incomplete-and-cut-code) | Is completion robust with the context rankings on? | Automated | Done | Met: 0 violations |
| [E34](#e34-where-the-place-of-the-caret-says-nothing-do-the-words-of-the-file-and-the-most-used-keywords-come-first) | Do the words of the file and the most used keywords come first where no rule decides? | Automated, generated code | Done | Not met (+2.2); E35 refines it |
| [E35](#e35-do-a-few-keywords-still-come-before-the-words-of-the-file) | Do a few keywords still come before the words of the file? | Automated, generated code | Done | Adopted: criterion met by a hair (+3.0) |
| [E36](#e36-should-words-of-two-letters-be-offered) | Should words of two letters be offered? | Automated, generated code | Closed | Not met: the criterion could not be met; E38 |
| [E37](#e37-does-the-similar-words-stage-make-noise-with-short-prefixes-and-what-removes-it) | Does the similar-words stage make noise with short prefixes? | Automated, generated code | Done | Not met; kept as it is |
| [E38](#e38-should-words-of-two-letters-be-offered-e36-with-a-criterion-that-can-be-met) | E36 with a criterion that can be met | Automated, generated code | Done | Adopted: two-letter words last |

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

### The two changes, measured against the code they change

Same session, three runs each, on the code of the experiments (`6797746`) and on each change on top of it. The changes are commit `5796fe6` (E13) and commit `3380d4b` (E14). Together, on one build: E01 9.0 ms, E05 1.18x, E07 13.8x.

| Id | Before (3 runs) | After E13 | After E14 |
|---|---|---|---|
| E01 | 9.9 / 9.9 / 10.1 ms | 9.9 / 10.3 / 10.0 ms | 9.3 / 9.1 / 9.5 ms |
| E05 (Highlight / text, with markers) | 1.74x | not run | 1.18x |
| E06 | 18.2x / 18.5x / 17.9x | not run | 16.9x / 21.9x / 17.4x |
| E07 | 87.4x / 86.8x / 82.3x | **13.6x / 13.8x / 14.6x** | 82.1x / 87.8x / 82.0x |
| E08 | 22.0x / 21.6x / 22.9x | not run | 22.4x / 23.7x / 22.5x |
| E09 | 2.94x / 2.75x / 3.08x | not run | 2.69x / 2.56x / 2.62x |

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

**Update.** E14 ran: the scan now allocates 27% to 60% less and `Highlight` 1.74x to 1.18x the text. The stage split is now more even (69% to 84% in the scan), which is why this experiment's criterion reads "not met" on the new code.

**Revisit when.** The scanners create the embedded string lazily (see E14).

## E06: number of marked strings

**Status:** Done · **Decision:** Not linear above ~400k characters

**Hypothesis.** The cost grows only linearly with the number of embedded strings: nothing in the engine (a sort, a nested scan) grows faster than the number of strings it has to handle.

**Test.** A file where every unit carries a marked string, at 100k, 200k, 400k and 800k characters, for each host. Time and gen2 collections.

**Criterion.** 8 times more embedded strings cost less than 10 times the time (linear is 8x, quadratic is 64x), for every host.

**Result.** Not met in the three runs: 16x to 19x for 8x the strings. Up to 200k characters the cost is linear (about 2 µs per string). At 400k it starts to rise and at 800k it is about 4 µs per string. The gen2 collections follow the same curve: none at 100k and 200k, 1 to 5 in 25 runs at 400k and 800k. That is a correlation, not a proof of cause.

**Decision.** The engine is not quadratic in the number of strings, but a file of 800k characters (about 25k lines) costs twice as much per string as one of 200k. That does not hurt files of ordinary size.

**Note.** The first version of this experiment (a fixed file with a growing density of marked strings) was replaced before any result was recorded: it measured a difference between two noisy times of about 1 ms and its verdict changed between identical runs. A later version also gave an optimistic result when run alone, because the first experiments ran on code the JIT had not optimized yet; tiered compilation is now off, which also matches `net48`.

**Update.** E14 reduced the allocations of the scan and neither the growth (16.9x to 21.9x) nor the gen2 collections changed. The scan's allocation is not the cause; see E15.

**Revisit when.** E15 attributes the growth.

## E07: many interpolations in one string

**Status:** Done · **Decision:** Quadratic

**Hypothesis.** The engine clips every token against the interpolations of its string starting from the first one, so a single large template with many interpolations costs O(tokens x interpolations). Suspected from reading `HighlightEngine.AddClipped`.

**Test.** One marked HTML string with 100, 1,000 and 10,000 interpolations, in JavaScript, C# and Python.

**Criterion.** Ten times more interpolations cost less than 20 times the time (linear is 10x, quadratic is 100x), for every host.

**Result.** Not met, and the clearest finding of the suite: 10x the interpolations cost 82x to 98x the time, the same in the three hosts. 100 interpolations take 0.16 ms, 1,000 take 7 ms and 10,000 take 550 to 600 ms.

**Decision.** The cost is quadratic in the number of interpolations of one string. It is invisible below a few hundred interpolations and a freeze of half a second at 10,000. The experiment does not isolate the cause: `AddClipped` is the suspect, not a proven culprit. E13 tests the fix.

**Update.** E13 replaced the walk with a binary search: 10x the interpolations now cost 13.6x to 14.6x the time, 10,000 interpolations take 22 ms instead of 550 to 600 ms, and the tokens are identical. The remaining 14x (against 10x for linear) is the same superlinear effect as in E06 and E08.

**Revisit when.** The clipping changes again.

## E08: throughput of each embedded language

**Status:** Done · **Decision:** No outlier; all superlinear on large strings

**Hypothesis.** All the tokenizers run at a similar speed in characters per millisecond, and none gets slower per character on larger inputs.

**Test.** One marked string per language, in a JavaScript host, of 100,000 and of 1,000,000 characters made by repeating a snippet of typical code of that language.

**Criterion.** The slowest language is less than 5 times slower than the median one, and every language takes less than 12 times longer on 10 times the text.

**Result.** Not met in the three runs, but only the second half. The slowest language (SQL or YAML) is 1.5x to 1.6x below the median, so there is no outlier. All 11 languages take 13.6x to 23x longer on 10x the text, not just one: Markdown, GraphQL and GLSL sit at the low end, SQL, JSON and XML at the high end.

**Decision.** No tokenizer is an outlier. The superlinear growth is common to all of them, which points to the shared engine (large buffers, the garbage collector) more than to any tokenizer, as in E06. The test does not separate the two.

**Update.** E14 did not change the growth (22.4x to 23.7x). The cause is not the scan's allocation; see E15.

**Revisit when.** E15 attributes the growth.

## E09: malformed and pathological input

**Status:** Done · **Decision:** Inconclusive (borderline)

**Hypothesis.** Code being typed is often malformed. An unterminated string or comment early in a large file, deep nesting, or a very long line can make the scan or a tokenizer quadratic.

**Test.** For each host: an unterminated marked string and an unterminated block comment at the top of a file of 250,000 characters, deep nesting (200 and 400 levels), and one line of 250,000 characters. Each case at N and at 2N. Cases under 5 ms at 2N are ignored.

**Criterion.** Doubling the input multiplies the time by less than 2.5 (linear is 2, quadratic is 4) in every case.

**Result.** Met, met, not met (worst growth 2.41x, 2.45x and 2.58x). Only one case matters: an unterminated marked string at the top, where the whole file becomes one embedded string of 250k to 500k characters. Everything else is linear, and the block comment, the long line and the nesting are too fast to measure. That case grows 1.9x to 2.6x depending on the host and the run, with 6 gen2 collections at N and 8 to 9 at 2N.

**Decision.** Inconclusive: the verdict flips on noise, so there is no quadratic here, but a mild superlinear effect cannot be ruled out. The size of an unterminated string is the same situation as E06 and E08.

**Update.** E14 left the case where it was (2.56x to 2.69x, over the limit in the three runs). **Revisit when.** E15 attributes the growth. A criterion of 2.5 is too close to the noise to separate a mild effect: a larger N or more runs would help.

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

**Status:** Done · **Decision:** Adopt (`5796fe6`)

**Hypothesis.** E07 is quadratic because `AddClipped` walks the interpolations of the string from the first one for every token. Starting from the first one that can matter, found by binary search, makes the cost linear.

**Test.** Change the clipping and run E07 again. Check that the tokens are exactly the same: the unit tests, plus a differential run of 3,000 random templates (all 4 hosts, random interpolations, nested templates, text cut at any point) whose tokens are compared before and after.

**Criterion.** E07 criterion met (10x the interpolations cost less than 20x the time), no token changes, and no slowdown above 5% in E01.

**Result.** Met. E07: 13.6x, 13.8x and 14.6x, against 82x to 87x before. 10,000 interpolations: 22 ms, against 550 to 600 ms. The 829 unit tests pass, and the 5.6 MB of tokens of the 3,000 random templates are byte for byte identical. E01: 9.9 to 10.3 ms against 9.9 to 10.1 ms (+1.6% at most).

**Decision.** Adopt: a change of ten lines in one private method, with a large effect where the bug was and none elsewhere.

**Limit.** Only the HTML inside JavaScript, C# and Python templates was measured for the speed; the differential run also covers CSS, SQL and JSON.

## E14: fewer allocations in the scan

**Status:** Done · **Decision:** Modest gain; criterion not met, adopted (`3380d4b`)

**Hypothesis.** The scan allocates more than the size of the text even when there is nothing to find, mostly in the handling of comments (a `Substring`, a `Trim` and a `ToLowerInvariant` for each one, plus an array of keys per call). Parsing the marker on the text itself, without creating strings, removes most of it. If E06, E08 and E09 are limited by the garbage collector, they improve together.

**Test.** Change `MarkerComment.Parse` to read a range of the text, then run E05 again, and E06, E08 and E09 to see whether the superlinear growth goes away. Check that nothing changes: the unit tests (19 new ones compare the two overloads), a differential fuzz of 200,000 random comments (including Unicode that changes case in unexpected ways) against the old implementation, and the 3,000 random templates of E13.

**Criterion.** E05: the scan allocates less than 0.5x the size of the text without marked strings, and no token changes.

**Result.** Not met, but the change is real.
- **Allocation.** The scan without marked strings went from 1.01x-2.01x the size of the text to 0.41x-1.41x (JavaScript 996 to 610 KB, C# 1.2 MB to 834 KB, Python 1.1 MB to 803 KB, C++ 645 to 258 KB). Only C++ is under 0.5x. `Highlight` as a whole: 1.74x to 1.18x the text, on average with marked strings.
- **Time.** E01 improved by 11.5% on average at 12k and 60k lines (from -2% to -18% by case; worst case 60k lines: 9.9 to 9.3 ms).
- **No change in behavior.** 848 unit tests pass, the fuzz found no difference and the 5.6 MB of tokens are identical.
- **E06, E08 and E09 did not move.** E06: 16.9x to 21.9x (before 17.9x to 18.5x). E08: 22.4x to 23.7x (before 21.6x to 22.9x). E09: 2.56x to 2.69x (before 2.75x to 3.08x, the same borderline case). The gen2 collections of E06 at 800k characters did not fall either (4 to 5 in 25 runs, before 3).

**Decision.** The change is safe, small and worth merging for the 11% and the lower allocation. The part of the hypothesis that said the garbage collector explains the superlinear growth is **refuted** for the allocations of the scan: removing a third of them changed nothing. The cause of the growth above ~400k characters is still unknown (see E15).

**Remaining allocation.** What is left is probably an `EmbeddedString` (with its lists) and an id string created for every string literal of the host, marked or not. Allocating them only when the string is marked would remove it, but the scanners need the object while they read the interpolations, so it is a larger change than this one.

**Revisit when.** The scanners are changed to create the embedded string lazily.

## E15: why the cost grows faster than the text above ~400k characters

**Status:** Planned (follows E06, E08, E09 and E14)

**Hypothesis.** When one file or one string is large, the cost per character grows even without the scan's allocations. Candidates: the decoding buffers (`List<char>` and `List<int>` of 12 bytes per character of the string, which land on the Large Object Heap and double as they grow), the list of tokens and its sort, and the processor cache.

**Test.** For one marked string of 100k, 400k and 800k characters, time the stages separately (decode, tokenize, map back, sort) and record gen2 collections and bytes allocated per stage. A variant presizes the buffers.

**Criterion.** One stage accounts for the extra cost, so that the superlinear growth can be attributed. Success is a cause, not a speed-up.

## Completion (E16 to E27)

The completion ([NestLight/Completion](../NestLight/Completion)) offers the keywords of the language of the string and the words that already exist in the document. E16 to E25 and E27 test it without Visual Studio (E26 is manual), on the same shared source as the other experiments. E20 and E21 run on **generated code** (`SyntheticCorpus`: 50 files, 4 hosts, SQL / HTML / CSS / GraphQL strings, a pool of 40 names reused as variables, columns, classes and fields, seeded so a run can be repeated). It is not real code: how often a name comes back, and how close to its last use, is set by the generator (`locality`), so those two experiments only say what would happen under that setting. The first full run was on commit `791df35`, .NET 8 in Release on a 4-core Linux machine, three runs; the quality experiments (E18 to E21) give the same numbers in every run. The second stage (E23 to E27) is described after E22.

## E16: completion latency against the size of the file

**Status:** Done · **Decision:** Not met above about 1 million characters; no change for now

**Hypothesis.** Every keystroke in an embedded string scans the host to find the string (`Locate`) and the whole text for words (`Suggest`). That is cheap on a normal file but could be noticeable on a large one, mostly when thousands of distinct words share the prefix.

**Test.** For each host, a file of 1,200, 12,000 and 60,000 lines with the caret at the end of an open `comp` in a marked SQL string. Two shapes: E01's file (typical code) and a file where every line declares a new `compNNNNN` identifier. Time of `Locate` alone and of `Locate` + `Suggest`, without the text copy of the editor (E03).

**Criterion.** `Locate` + `Suggest` under 16 ms at the largest size, in every host and shape.

**Result.** Not met in the three runs, only at the largest size.
- **60,000 lines (0.9 to 2.1 million characters):** 10 to 22 ms. Python is the slowest in both shapes (17.8 to 22.1 ms); C++ the fastest (9.8 to 15.3 ms). C# with distinct words sits on the line (14.7, 15.9 and 16.1 ms).
- **12,000 lines (0.2 to 0.4 million characters):** every case 3 to 8 ms.
- **Where the time goes.** `Locate` alone is a quarter to four fifths of the total at 60,000 lines (Python, typical code: 14.5 to 16.4 ms of 17.8 to 19.2; JavaScript with distinct words: 3.3 to 3.5 ms of 12.6 to 14.2). The word scan is limited to 500,000 characters on each side of the caret, so it stops growing; the scan of the host does not.

**Decision.** At 12,000 lines completion is well inside a frame. Above a million characters it takes more than a frame in some hosts, and the scan of the host (the same work the classifier already did for that snapshot) is the larger part. No change now.

**Revisit when.** E10 shows that typing in large files is already tight, or the classifier exposes the strings of its last scan: then `Locate` can reuse them and the cost falls to the word scan.

**Update (E22, commit `1fa1fbe`).** The classifier and the completion now share one scan per snapshot, and the word pass no longer creates a string per match. E16, unchanged, measures one `Locate` + `Suggest` over a plain scanner: with distinct words the 60,000-line case fell from 12.6 to 16.1 ms (C#) and 14.1 to 22.1 ms (Python) to 5.7 to 9.5 ms; with typical code nothing moved (9.6 to 13.6 ms, and Python 16.9 to 17.6 ms, still above a frame), because there the cost is the scan of the host, which E16 does not share. The criterion is still not met in the three runs, only by Python with typical code. The real session, with the scan shared, is E22.

## E17: do the limits of the completion change its cost?

**Status:** Done · **Decision:** Defaults kept (100 suggestions, 3 characters)

**Hypothesis.** The limit on the number of suggestions (100) and the minimum word length (3) were picked without measuring. If the cost is in the scan, no value of them is a performance lever.

**Test.** The two shapes of E16 at 12,000 lines (JavaScript), with the maximum number of suggestions at 10, 100, 1,000 and 10,000 and the minimum word length at 1, 3 and 5, one knob at a time from the default. Median of 100 runs, against the faster of two measurements of the default (first and last).

**Criterion.** Every combination within 25% of the default, in both shapes.

**Result.** Not met, in all three runs, by one combination: 10,000 suggestions with thousands of distinct matches costs 1.47x to 1.56x (about 2 ms more: it builds ten thousand objects). Everything else is within 10% of the default in most measurements, but single ones jump: in one run the default measured again came out at 1.34x, and in the run of the committed report a minimum length of 1 came out at 1.49x, which no other run repeats. The noise at this scale is occasionally +-30 to 50%. Only the 10,000 limit shows up in every run.

**How the measurement was fixed.** The first version compared everything against a baseline measured once, first. That measurement was often 40 to 50% slower than the same setting measured later, which made the other rows look 30% faster and flipped the verdict between runs. The default is now measured twice (first and last).

**Update (E22).** After the word pass stopped creating a string per match, the default of 100 costs about 2.5 ms and 10,000 suggestions about 7 ms (2.7x in the three runs, against 1.5x before). The absolute cost of 10,000 did not change; the default got cheaper. The verdict is the same and clearer: only a limit that makes the engine create thousands of words costs anything.

**Decision.** The scan is the cost, not the limits; the one setting that matters (10,000) is far from the default of 100. Keep both defaults. The verdict of this experiment is real but narrow, and the noise means a 25% criterion is tight for 4 ms measurements.

## E18: completion on incomplete and cut code

**Status:** Closed (replaced by [E27](#e27-completion-with-similar-words-on-incomplete-and-cut-code)) · **Decision:** Met

**Hypothesis.** Completion runs while code is being typed, so it sees unterminated strings, half-written interpolations and carets anywhere. For every text and caret, `Locate` and `Suggest` must not throw, the site must lie inside the text around the caret with only word characters, and the suggestions must start with the typed prefix, add something to it and not repeat.

**Test.** 50 generated files. Every prefix cut and every single-character deletion at a stride (12,450 texts), the caret at the start, at the end and at 5 seeded random positions (86,900 carets, 16,374 of them inside the code of an embedded string). 6 invariants on each result.

**Criterion.** Zero violations.

**Result.** Met: 0 violations.

**Limit.** The cuts and deletions are at a stride, not at every position, and the files are generated: the real forms of unfinished code (a comment opened in the middle of a template, for example) are only as varied as the generator.

**Closed.** The invariant "every suggestion starts with what was typed" no longer holds once similar words are offered, so the hypothesis changed and, by the rule of this file, the experiment was replaced. The run above stays as the record of the completion without the second stage; the class was removed from the suite.

## E19: does the tokenizer agree with the vocabulary?

**Status:** Done · **Decision:** Met

**Hypothesis.** The words the completion offers and the words the tokenizers color live in two places. SQL, GraphQL, YAML and shader lists reuse the tokenizers' sets, but HTML tags and CSS properties were written by hand. A word the completion offers and the tokenizer then splits, or colors as plain text, shows the plugin does not know what it just offered.

**Test.** Every word of every vocabulary (SQL 154, GraphQL 28, GLSL 215, WGSL 162, JSON 3, YAML 17, HTML 130, CSS 227) in one or more contexts of its language, through the real tokenizer. It checks that one token covers exactly the word and that its type is the expected one (for SQL and YAML: different from the type of an unknown word in the same context).

**Criterion.** Every word is one token, and at least 95% of the words of each language are classified as expected.

**Result.** Met: 100% in every language.

**What the method got wrong the first time.** The first quick run flagged 10 of the 28 GraphQL words (`Int`, `Float`, `ID`, `Boolean`, `on`, `skip`, `include`, `deprecated`...). That was the check, not the tokenizer: GraphQL colors any name after a colon as a type and any `@name` as a directive, so a built-in and an unknown word look the same. The GraphQL check now lists the accepted types and has a `fragment F on Foo` context. The criterion did not change.

**Limit.** For CSS and HTML the second check is weak: any word in a property position is a property to the tokenizer. What they really verify is that no word is split, which is the check that would have caught a typo in the lists.

## E20: order of the words of the document

**Status:** Done · **Decision:** Kept (nearest to the caret first)

**Hypothesis.** Listing the words that already exist in the document nearest to the caret first puts the right word among the first five more often than alphabetical, by frequency or by first appearance.

**Test.** 50 generated files, typed 1, 2 and 3 characters of a sample of the words of the embedded strings (9,651 cases; 95% are reachable: the word exists elsewhere or is a keyword). The position of the right word in five orderings. The same files are generated with locality 0.0, 0.5 and 0.9.

**Criterion.** At locality 0.5, nearest-first within the first 5 at least 5 percentage points above alphabetical.

**Result.** Met. Within the first 5 (keywords first in all rows except the last):

| Order | Locality 0.0 | Locality 0.5 | Locality 0.9 |
|---|---|---|---|
| nearest first (the engine) | 68.9% | 72.1% | 76.3% |
| alphabetical | 64.3% | 64.0% | 65.5% |
| by frequency | 69.1% | 69.6% | 70.7% |
| by first appearance | 69.4% | 69.8% | 69.3% |
| words nearest first, then keywords | 64.7% | 69.2% | 78.4% |

**What it says, and what it does not.**
- The margin over alphabetical grows with locality (4.6 points at 0.0, 8.1 at 0.5, 10.8 at 0.9). Part of that is built into the generator, which reuses recent names on purpose.
- With no locality, frequency and first appearance are as good as nearest-first (69.1% and 69.4% against 68.9%). If real code is closer to that than to the generator, nearest-first is not worth more than the simpler orders. Real code was not measured.
- Putting the words before the keywords is worse with no locality (64.7%) and better with a lot (78.4% within 5; 45.9% against 38.3% for the right word first). It depends on how much the user repeats what they just typed.

**Decision.** Keep the current order. The experiment cannot say it is the best order for real code, only that it is not worse than the others on this generator.

**Revisit when.** There are real files to run it on, or there is a way to see which suggestion users accept.

## E21: where the words come from, and how many keystrokes completion saves

**Status:** Done · **Decision:** Inconclusive

**Hypothesis.** Offering the words of the whole document (host code included, as Visual Studio Code does) saves more keystrokes than offering only the words inside embedded strings, or only those of the string being typed.

**Test.** The corpus of E20 at locality 0.5. 3,217 words typed one character at a time, up to 5. The completion is accepted at the first prefix where the right word is within the first 5; saving = length of the word - characters typed - 1 for the accepting key. Four scopes, always with the keywords except the first.

**Criterion.** The saving of the whole document within 2 percentage points of the best narrower scope, or above it.

**Result.** Not met, by 2.7 points, in the three runs.

| Scope | Right word within 5 after 1 / 2 / 3 characters | Characters saved |
|---|---|---|
| keywords only | 35.6% / 49.7% / 49.7% | 17.8% |
| the string being typed | 39.1% / 60.6% / 62.8% | 32.8% |
| all embedded strings | 42.8% / 79.9% / 90.1% | **58.0%** |
| the whole document | 42.1% / 77.2% / 86.7% | 55.3% |

**What it says, and what it does not.** The words are drawn from the embedded strings only, so a word that a user types in a string and that exists only in host code (for example a variable inside an interpolation) is never a target. That favors the narrower scopes and is a bias of the test, not a property of completion. Even so, the two best scopes are within 3 points, and both are far above the string being typed alone (32.8%) and the keywords (17.8%): most of the benefit comes from looking beyond the current string, whichever way it is bounded.

**Decision.** Inconclusive: no change to the scope. A test that also types the names of the host would be fairer, and needs real code.

**Revisit when.** There are real files, or the test types words from interpolations too.

## E22: sharing the scan and not creating the words of the completion

**Status:** Done · **Decision:** Adopt (`1fa1fbe`)

**Hypothesis.** E16 found that completion takes more than a frame above ~1 million characters, mostly the scan of the host. In Visual Studio a session scans the text twice (once to decide whether to open, once to fill the list) after the classifier already did it for the same snapshot, and the word pass created a string and a sort entry for every match. Sharing one scan between the classifier and the completion, and creating a word only when it is offered, brings the worst case under a frame.

**Change.** `CachingHostScanner` remembers the last scan, compared by reference to the text, held weakly. `SnapshotTextCache` gives everything that reads one snapshot the same string instance. The classifier and the completion of a buffer are built over one scanner (`NestLightComposition.CreateForBuffer`). The word pass finds the matches as positions, in one pass and without strings, and merges the matches before and after the caret in order of distance, creating a word (and checking it against the ones already offered by hash) only when it is about to be offered.

**Test.** E16's files. The start of one completion session, as the editor makes it (`Locate`, `Locate` again, `Suggest`) over a new text instance each time, in two cases: nothing shared, and the scan shared (the classifier has already highlighted that text, not timed). Plus `Suggest` alone. Check that nothing changes: 897 unit tests, among them a comparison of the order of the words with the first, straightforward implementation (dictionary and sort) on 4,800 random carets of small generated programs (those inside embedded strings are compared), and a test of two words at the same distance on both sides.

**Criterion.** The session with the scan shared stays under 16 ms at 60,000 lines, in every host and shape.

**Result.** Met in three runs: 2.4 to 2.8 ms in every host and shape at 60,000 lines (0.2 to 2 million characters), the same for typical code and distinct words. Nothing shared: 12 to 32 ms (C++ distinct words 12.1 ms, Python typical code 31.5 to 31.8 ms), the cost of the old wiring, which E16 understated because it counted one `Locate`. `Suggest` alone: 2.2 to 2.6 ms. E20 and E21 give the same numbers as before the change.

**Decision.** Adopt. The change is small, the order of the suggestions is the same, and the session start fell from 12 to 32 ms to under 3 ms at the largest size.

**Limits.**
- The 2.5 ms assumes the classifier ran on the same snapshot first (or that completion's scan seeds the classifier). The classifier's own cost, about 10 ms at this size (E01), is not new: it was already paid at every edit.
- A hit needs the same text instance. If a gen2 collection frees the text between the two uses, the next one scans again: slower, never wrong.
- The Visual Studio part (`NestLightBuffer`, the providers) was written without the VS SDK here and was not compiled or run; the test is the same code over a fake scanner and the real one.
- Generated files, one machine, .NET 8 (not net48).

**Revisit when.** E10 measures typing in Visual Studio; or the scan itself becomes incremental (E12), which would help the classifier too.

## Second stage: similar words (E23 to E27)

The second stage ([BandedPrefixMatcher](../NestLight/Completion/ApproximateMatcher.cs) and the end of [CompletionEngine](../NestLight/Completion/CompletionEngine.cs)) runs only when fewer than `FuzzyBelow` (1) items start with what was typed, the typed text has 3 or more letters, and a matcher was given. It offers the keywords and the words of the document that are at most 1 edit away (3 to 5 letters) or 2 (6 or more) from some prefix of them: an extra letter, a missing one, a wrong one or two swapped neighbours. Only the first `FuzzyMaxItems` (10) are added, after the exact ones. The first letter has to be the one typed. `SELCT` offers `SELECT`, `<dvi` offers `div`, `custmer` offers `customerName`.

The first runs were on commit `70e9ffd` (.NET 8 in Release, a 4-core Linux machine), three runs for the experiments that were run. **E24 and E25 have not been run in full**: their criteria are proposals that were left to be confirmed before the first run, and a run fixes them. A smoke run (`--quick`, numbers not kept) only checked that they work.

## E23: does the second stage of the completion fit in a frame?

**Status:** Done · **Decision:** Inconclusive (borderline in one extreme case)

**Hypothesis.** The second stage compares what was typed with every distinct word of the document that passes two cheap filters (the first letter, the length) and keeps the best few. Even forced to run in every session, with thousands of words one edit away, a session still fits in the 16 ms of a frame.

**Test.** E22's files and session (the scan shared, a new text instance each time), 4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines, with the second stage forced (asked for below any number of items). Two typed texts: `comp` (everything is compared, nothing new is found) and `cmop` (in the distinct-words file all 60,000 words are one edit away). Also the same session with the second stage off, and the bytes allocated.

**Criterion.** Under 16 ms at 60,000 lines in every host, shape and typed text.

**Result.** Not met in two runs and met in one, so the verdict flips (the same behavior as E09).
- **One case decides it:** Python, distinct words, `cmop`: 18.7, 15.4 and 16.1 ms. The other distinct-words cases with `cmop` are 11.6 to 13.9 ms in every run.
- **Typical code:** 4.1 to 4.6 ms forced, against 2.1 to 2.9 ms with the stage off. Typing `comp` costs nothing in the distinct-words file (2.2 to 2.6 ms, as with the stage off): no word is one edit away that is not already a prefix match.
- **Allocation:** in typical code the stage allocates 1 KB more. In the worst case it allocates 3.2 to 3.7 MB: one small record per distinct word that passed the filters (60,000 of them), and the 10 strings that are offered. No string is created for a word that is too far or too similar to matter.

**What it says.** The worst case is artificial: 60,000 distinct words that all start with the same letter and are all one edit from the typed text. With real names the first-letter filter removes most of them. In that case the cost is the comparison of each word (about 60,000 small matrices) plus the dictionary of distinct words.

**Decision.** Not changed. The stage keeps the order of remedies that the design set: a cache of the distinct words per text (as `CachingHostScanner` does for the scan) and a smaller window first, and a trie only if those do not fit. Neither helps the first request on a new text, which is the case measured here.

**Revisit when.** E26 shows the feature is used on large files, or real files have as many near-identical distinct words as this one.

## E24: does the second stage recover the word after one mistake, and which tie-break works?

**Status:** Planned (the criterion is a proposal to confirm before the first run)

**Hypothesis.** When the typed text has one edit (an extra letter, a missing one, a wrong one, two swapped) in a prefix of 4 to 8 letters, the meant word is among the first 5 suggestions in most cases. Among words the same number of edits away, the nearest to the caret is no worse a tie-break than the most frequent.

**Test.** The corpus of E20 (50 generated files, locality 0.5). A sample of the words of the embedded strings is typed as a prefix of 4 to 8 letters with one edit of each kind at a random place over the prefix (the first letter included). Only the reachable cases count (the word exists elsewhere in the document or is a keyword). The list is also reordered inside each group of the same kind and distance: nearest first (the engine), most frequent first, most frequent and then nearest.

**Criterion.** The meant word within the first 5 in at least 70% of the reachable cases with the best of the three tie-breaks; if more than one reaches 70%, the best result enters, and on a tie the nearest stays.

**Limits already known.** A mistake in the first letter cannot be recovered with the first letter required; with the mistake uniform over the prefix that is about one case in five or six, which caps the result well under 100%. The corpus is generated and the mistakes are not the way people mistype.

## E25: does the second stage get in the way when the prefix is right?

**Status:** Planned (the criterion is a proposal to confirm before the first run)

**Hypothesis.** If the second stage ran whenever the first found few items, a correct prefix would often get a list with words that are only similar by chance. Running it only when nothing matched (`FuzzyBelow` = 1) keeps that rare.

**Test.** The corpus of E20. A sample of the words of the embedded strings typed correctly, 3 to 8 letters, with the rest of the word removed. The engine with `FuzzyBelow` = 1, 3 and 5; the cases where similar items are added are counted, apart for those where the first stage found something and those where it found nothing.

**Criterion.** With `FuzzyBelow` = 1, similar items are added in at most 5% of the cases. The default is chosen among the values that meet it.

## E26: do the similar suggestions show up in Visual Studio?

**Status:** Planned (manual, needs a build in Visual Studio 2022 and 2026)

**Hypothesis.** The item manager of the editor filters the list at every key against the text of the span. A similar item is created with a filter text equal to what was typed, and shows and inserts the word it suggests, so the manager keeps it. The documentation does not say whether the default filter tolerates this.

**Test (manual).** In a marked string, type a word of 6 letters with a mistake, press Ctrl+Space, and keep typing.

**Criterion.** The meant word is in the list at the first step.

**If not met.** Limit the feature to the explicit invocation and say so in the README. Nothing of the Visual Studio side of this feature (`NestLightCompletionSource`) has been compiled or run in this work.

## E27: completion with similar words on incomplete and cut code

**Status:** Done · **Decision:** Met (replaces E18)

**Hypothesis.** Completion runs while code is being typed. For every text and caret, `Suggest` must not throw; the site must lie inside the text with only word characters; the exact suggestions must start with the typed text; the similar ones must be at the distance they claim (as the definition computes it), between 1 and the tolerance, with the first letter typed, after the exact ones, keywords before words and fewer edits first; nothing repeats; and the limits hold.

**Test.** E18's: 50 generated files, every prefix cut and every single-character deletion at a stride (12,450 texts), the caret at the start, at the end and at 5 random places (86,900 carets, about 16,000 of them inside embedded code); plus, at each of those, the same text with a one-letter mistake put in the word under the caret. The distance of every similar item is recomputed with the whole matrix.

**Criterion.** Zero violations.

**Result.** Met in the three runs: 0 violations in 86,900 carets, 21,275 similar items checked against the definition.

## Context ranking (E28 to E38)

Until here the list depended only on what was typed: the keywords, then the words of the document nearest to the caret first. These experiments ask whether the list gets better when it also looks at where the caret is. Each idea is a flag of [CompletionFeatures](../NestLight/Completion/CompletionFeatures.cs) and an experiment decides whether it is on in the plugin (`CompletionFeatures.Default`).

They share one harness ([ContextLab](../NestLight.Experiments/Framework/ContextLab.cs)) and one corpus: `SyntheticCorpus.Generate(..., structured: true)`, where SQL strings follow a schema (a `CREATE TABLE` for each table, columns that belong to their table, joins on foreign keys), CSS values belong to their property and HTML attributes belong to their tag. **The structure is put there by the generator.** What these experiments show is that an idea works when code has that structure, not how much real code has it; E20 and E21 have the same limit, and no experiment here ran on real files. A word is typed with 1, 2 or 3 letters, the list is the one the editor gets (100 items, the second stage on), and the measure is the share of cases where the word is within the first 5, over the cases where anything could offer it.

The editor sorts the list by the sort text of each item, which is the text unless told otherwise, so the order of the engine did not reach the screen before: [NestLightCompletionSource](../NestLight/VisualStudio/NestLightCompletionSource.cs) now gives every item its position as sort text. That code has not been compiled or run in this work (it needs the Visual Studio SDK).

## E28: does the word before the caret help to rank the suggestions?

**Status:** Done · **Decision:** Adopted (`CompletionFeatures.PreviousWord`)

**Hypothesis.** The words that already followed the same word (with the same punctuation) elsewhere in the document are the likely ones: after `from ` the word that followed `from` before, after `display: ` the value that followed `display:`, after `group ` the `BY`. Putting them first puts the meant word in the first 5 more often than the order by distance alone.

**Test.** 50 generated files with structure, a sample of the words of the embedded strings typed with 1 to 3 letters (9,564 prefixes, 9,162 reachable), the list the editor gets. The order by distance alone against the previous word first; the same words ordered by the nearest occurrence of the context. Also the start of a session on files of 1,200 to 60,000 lines.

**Criterion.** At least 3 points more within the first 5 over all the reachable cases; no language more than 1 point worse; the session under 16 ms at 60,000 lines in every host.

**Result.** Met. Within the first 5: 72.5% to 85.5% (+13.0 points); the word first: 36.8% to 68.5%; mean reciprocal rank 0.528 to 0.764. By language: SQL 72.7% to 88.1%, HTML 61.1% to 70.7%, CSS 74.3% to 80.7%, GraphQL 88.7% to 90.1%. The gain is largest with one letter typed (39.1% to 67.5%) and fades with three (95.0% to 97.0%). After `from` 39.4% to 86.4%, after `into` 37.8% to 82.2%, after `update` 42.1% to 81.8%. The slowest session is 4.07 ms at 60,000 lines (Python), the same as without the feature.

**What it says.** With one or two letters typed, a keyword or a nearby word usually fills the first places, and what the document did after the same word is a better guess than what is nearest. With three letters the prefix has already narrowed the list, and there is little left to gain.

**Cost.** One more comparison per candidate that passes the prefix test, and a record of the previous word during the pass. The hot path with the features off was measured two ways. In one process, alternating the engine of `main` and this one on the same 60,000-line Python text, 40 calls each, four rounds: 0.87 to 1.07 ms against 0.87 to 1.05 ms, no difference. E22 run on `main` and on each commit of this work, three runs each: 1.20 ms on `main`, then 1.44, 1.65, 1.62 and 1.60 ms, a rise of about a third that the first measure does not show; a pass restored to the original loop did not change it, and the cause was not found. Taken together: the code of the first stage costs the same, and the E22 harness reads about 0.4 ms more on these builds for a reason outside the engine (the cell is a median of 25 calls in a process that has also loaded the new experiments). With the three features of the plugin on, the start of a session at 60,000 lines is about 5 ms against about 4 ms for distance alone (E28 to E30).

**Limits.** The words after the same word are found in the whole window, strings and host code alike; E29 puts the words of the language first. A word shorter than 3 letters is only offered when it follows the context (`BY`).

**Revisit when.** E26 shows how the order looks in the editor, or real files show that the previous word predicts worse than here.

## E29: do the words of the same language come first?

**Status:** Done · **Decision:** Adopted (`CompletionFeatures.SameLanguageWords`)

**Hypothesis.** A word written in the code of another string of the same language (a column in another SQL string) is likelier than a word of the host code or of a string of another language that happens to start with the same letters, even when the other one is nearer to the caret. Putting the words of the language first, and taking the context of the previous word from them only, puts the meant word in the first 5 more often. It is E21's question (where should the words come from) asked as a ranking and not as a filter: nothing is dropped, the other words come after.

**Test.** E28's probes, four variants: the order by distance alone; the words of the language first; the previous word; both. Interpolations are host code. Also the start of a session on files of 1,200 to 60,000 lines with the scan shared.

**Criterion.** Adding the words of the language to the previous word is at least 2 points better within the first 5; no language more than 1 point worse; the session under 16 ms at 60,000 lines in every host.

**Result.** Met. Within the first 5: 72.5% (distance alone), 77.3% (the language), 85.5% (previous word), **88.1%** (both): +2.6 points on top of the previous word. By language, with both against the previous word alone: HTML 70.7% to 77.4%, GraphQL 90.1% to 96.0%, SQL 88.1% to 89.8%, CSS 80.7% to 81.8%; none is worse. The slowest session is 5.11 ms at 60,000 lines (Python), against 4.13 ms without the scope.

**What it says.** The scope helps most where the host shares names with the strings (HTML classes against variables, GraphQL fields), and least where the previous word already did the work (SQL). The gain is modest next to E28's; the generator names host variables after the same nouns as the tables, which makes the collision common.

**Cost.** About 1 ms at 60,000 lines (the strings of the language are taken from the shared scan and merged into ranges, and the matches go into two lists). With the features off nothing of it runs.

**Limits.** The second stage (similar words) does not use the scope. A string whose language the plugin does not know has no words of its own language to prefer, and its words are ranked as before.

## E30: does the place in the grammar help to rank the suggestions?

**Status:** Done · **Decision:** Adopted (`CompletionFeatures.Grammar`)

**Hypothesis.** What belongs at the caret can be told from a few characters of look-behind ([Positions](../NestLight/Completion/Positions.cs), no parser): a table after `FROM`, a column after `SELECT`, `BY` after `GROUP`, the properties inside the braces of CSS and the values of the property after its colon, the attributes of the tag inside `<button `. Putting what belongs first and what does not last (nothing is dropped) puts the meant word in the first 5 more often than the previous word and the language alone, and fits in a frame.

**Test.** E28's probes, four variants: the order by distance alone; the grammar; the previous word and the language (what the plugin ran with before); all three. Reported by the place of the caret. Also the start of a session on files of 1,200 to 60,000 lines.

**Criterion.** Adding the grammar to the previous word and the language is at least 2 points better within the first 5; SQL, CSS and HTML each do not fall; the session under 16 ms at 60,000 lines in every host.

**Result.** Met. Within the first 5: 72.5% (distance alone), 87.8% (grammar alone), 88.1% (previous word and language), **97.2%** (all three): +9.2 points. By language with all three: SQL 99.4% (from 89.8%), HTML 91.2% (77.4%), CSS 88.0% (81.8%), GraphQL unchanged at 96.0% (it has no grammar). The word first: 70.6% to 83.0%. The slowest session is 5.15 ms at 60,000 lines, the same as without the grammar (it reads at most 4,000 characters behind the caret).
- **Where it is weak:** `css:selector` 50.6% and `html:tag` 82.0% (tag names are a closed list and the words typed there are classes and ids that the rules do not cover), `css:property` 86.5%.
- **Where the two ideas overlap:** after `from` the previous word already reached 92.0%; the grammar adds the cases the document has not seen yet (a first `JOIN`, a `GROUP BY` never written).

**What it says.** The grammar gives a large gain, but read the corpus before the number: the same person wrote the generator and the tables of attributes, values and continuations, and the generator follows them. 97% is what the rules do on code that obeys them. Real code has attributes, values and clauses that the tables do not know; the rules then put nothing first and the list is the one of E29, so the cost of an incomplete table is a smaller gain, not a wrong order. Where the rules say something wrong (`Unlikely` words pushed after the words of the document) is not measured by this corpus.

**Limits.** The grammar covers SQL, CSS and HTML / SVG. A place the rules do not recognize (inside `url(`, a language without rules) is ranked as before. The tables are short: about 150 attributes, 40 properties with values, 60 SQL continuations.

**Revisit when.** Real files give a hit rate to compare with this one, or a table turns out to hide a word people use (watch `Unlikely`).

## E31: does the schema read from the SQL of the document help?

**Status:** Done · **Decision:** Not adopted (`CompletionFeatures.Schema` stays in the code, off)

**Hypothesis.** The tables and columns the SQL of the file talks about ([SqlSchema](../NestLight/Completion/SqlSchema.cs): a `CREATE TABLE`, the `FROM` and `JOIN` of the statement, the aliases, the column list of an `INSERT`, the `SET` of an `UPDATE`) tell which table to offer after `FROM` and which columns belong after `u.` or in the select list. That is more precise than the previous word, because an alias means a different table in every statement. The reader is a scan of the common shapes, not a parser.

**Test.** E28's probes reported by the place of the caret: the order by distance alone; the previous word, the language and the grammar (what the plugin ran with); the same plus the schema; the schema alone. Files with a `CREATE TABLE` for each table and joins with the aliases `t` and `o` reused for a different table in every statement. Also, apart from the criterion, 200 short files (4 functions each), where the previous word has little history. Also the start of a session with `select u.comp| from users u` typed at the end of files of 1,200 to 60,000 lines, where every SQL string in the window is read.

**Criterion.** At least 1 point better within the first 5 over all the reachable cases; the places that need a table or a column (`sql:table`, `sql:member`, `sql:expression`) do not fall; the session under 16 ms at 60,000 lines in every host.

**Result.** Not met. Within the first 5: 97.2% before, 97.5% with the schema (+0.3 points); the schema alone gets 84.6% against 72.5% for distance alone. By place: `sql:table` 98.4% to 100%, `sql:member` 99.2% to 99.8%, `sql:expression` 98.8% to 99.1%; none falls. The time is the problem: the session goes from about 5 ms to 6.4-7.6 ms at 60,000 lines in three hosts, and one run measured 16.08 ms in the fourth (C++; the other runs of the same case were 6.5 and 8.7 ms in other sessions), above the frame.
- **Short files:** where the document gives the previous word little to learn from, the schema pays more: `sql:table` 93.8% to 99.5%, `sql:member` 97.5% to 100%; over all the reachable prefixes 98.5% to 98.8%.
- **Why so little overall:** the previous word and the grammar already put the right table or column in the first 5 in 97% of the cases, so there are 2.8 points left to gain.

**What it says.** On a corpus built around a schema, with the previous word and the grammar in front, the schema fixes the last few misses and costs more than the rest of the ranking together: every request reads every SQL string around the caret. The criterion was written before the run and it is not met, so the plugin does not turn it on.

**Decision.** Off in `CompletionFeatures.Default`. The code and its 15 tests stay, because the short-files table says where it would pay.

**Revisit when.** The schema of a text is kept between requests (a cache next to the scan, so a keystroke does not read the file again), or real files turn out to have few repeated queries and a `CREATE TABLE` close by.

## E32: do the words used most often come before the nearest ones?

**Status:** Done · **Decision:** Not adopted (`WordOrder` stays in the code, `Nearest` is the default)

**Hypothesis.** The order by distance alone sends to the end a word that is used all over the file and is not close to the caret, while the nearest word may have been used once. A blend, `ln(1 + count) - weight * ln(1 + distance)`, puts the meant word in the first 5 more often, whatever the locality of the code. The engine has no edit history, so how near an occurrence is to the caret stands for how recently the word was used; real recency (the words accepted or typed last) would need the editor to tell the engine and is not done here.

**Test.** E28's probes on files of three localities (0, 0.5, 0.9), 50 each, with the previous word, the language and the grammar on. Five orders of the words of the document and of the words that followed the context: distance alone, count alone, and the blend with a weight of 1, 0.5 and 0.25. Also, apart from the criterion, the same orders with no other feature, and the start of a session.

**Criterion.** The best of the four other orders at least 1.5 points better within the first 5 than distance alone at locality 0.5; at no locality worse by more than 1 point; no language worse by more than 1 point; the session under 16 ms at 60,000 lines.

**Result.** Not met: no order is better. At locality 0.5: distance alone 97.2%, count alone 97.0%, the blends 97.2%, 97.2% and 97.1%. At locality 0 the five orders are within 0.1 points (96.7% to 96.8%), at 0.9 distance alone is the best (97.5% against 97.2% to 97.4%). With no other feature the picture is the same: distance alone 72.5%, count alone 71.4%, the blends 72.5%, 72.3% and 71.9%; counting without looking at the distance is the only order clearly behind (1.1 points). The session costs the same with every order (about 5 ms at 60,000 lines).

**What it says.** E20 had already found the nearest first better than the alphabetical order; this finds that adding the count does not improve on it, in code that repeats names by construction. With the previous word, the language and the grammar in front, a word that the context does not settle is a tie between a few near candidates, and the count does not break it better than the distance. It is not a result about real code, where a name used 30 times in the file is probably a better guess than one used once next to the caret; the generator draws its names from a pool of 40 and cannot say.

**Decision.** `Nearest` stays the default. The blend is kept as an option with its 7 tests, because it costs nothing when off and the answer may change with real files.

**Revisit when.** There are real files to measure on, or the editor can tell the engine which words the user accepted last.

## E33: completion with the context rankings on incomplete and cut code

**Status:** Done · **Decision:** Met

**Hypothesis.** E27 again with every context feature on (the previous word, the words of the language, the grammar of SQL, CSS and HTML, the schema of the SQL, the blend of count and distance), over the structured files that have the statements, rules and tags those features read. The look-behind of the grammar and of the schema, the pointers into the ranges of the strings, the ranked words and the short words offered after a context must not throw on a text cut anywhere, repeat a word, break the limits, or offer an exact suggestion that does not start with what was typed.

**Test.** E27's: every prefix cut at a stride and every single-character deletion at a stride (12,269 texts), the caret at the start, at the end and at 5 random places (85,633 carets, 35,116 of them inside embedded code), and the same position with a mistake in the word (16,841), with every feature of `CompletionFeatures` on, schema and blend included.

**Criterion.** Zero violations.

**Result.** Met: 0 violations, 29,786 similar items checked against the definition.

## E34: where the place of the caret says nothing, do the words of the file and the most used keywords come first?

**Status:** Done · **Decision:** Not met as written; E35 refines it

**Hypothesis.** The review of 800 suggestions (`docs/suggestion-review`) found that, without a rule for the place, the list is the vocabulary in alphabetical order, cut at 100, with the words of the file after it: with nothing or one letter typed the word that is wanted is often out of the first five or out of the list (GLSL, WGSL, GraphQL, and the keyword soup in the others). The words of the file first, and the keywords in the order of how much code uses them, should put it among the first five more often.

**Test.** A corpus of 500 snippets for each of the 8 languages ([the generators](../NestLight.Experiments/Corpus), 50 files of 10 snippets each; `--corpus <dir>` writes it). The even files are used to learn how often each keyword is used, the odd files to measure: 600 words per language, typed with no letter (a request with Ctrl+Space) and with one letter. A second test on the hand-written files of the review, which come from another source. Four variants of the engine the plugin runs: as it is; the words of the file before the keywords; the keywords by use; both.

**Criterion.** Over the words typed with 0 or 1 letter, the best of the three variants at least 3 points better within the first 5 on the test files; no language more than 1 point worse; not worse on the hand-written files.

**Result.** Not met. On the test files "both" gives 73.9% to 76.1% within the first 5 (+2.2 points); the words of the file first alone 75.6%, the keywords by use alone 76.0%. By language the gain is large in GLSL (71.8% to 82.0%) and WGSL (82.8% to 87.7%) and HTML (85.9% to 88.6%), nothing in CSS, and it **loses** in JSON (75.2% to 73.2%) and GraphQL (60.9% to 59.8%), so the worst language is -2.0. On the hand-written files it gains everywhere but JSON: 58.3% to 68.2% (+9.9), GLSL 23.2% to 55.8%, WGSL 35.1% to 58.6%, JSON 81.2% to 50.0%.

**What it says.** The loss in JSON is the words of the file pushing `true`, `false` and `null` down, which are the few keywords that are always right in a value. That points at the fix, which is E35.

## E35: do a few keywords still come before the words of the file?

**Status:** Done · **Decision:** Adopted (`WordsBeforeKeywords`, `KeywordPriors` and `HeadKeywords = 12`), by a hair

**Hypothesis.** Put only the few most used keywords of the language before the words of the file and the others after them; that keeps E34's gain and removes its loss.

**Test.** E34's files, words and priors. The engine as it is; the words of the file first with the keywords by use (0 keywords in front); and the same with the 3, 6 and 12 most used keywords in front. E35 was written after seeing E34: the variants react to its tables, and the test files are the same, so a gain is partly fitted to them. The hand-written files are the check.

**Criterion.** The best variant with keywords in front at least 3 points better within the first 5 on the test files; no language more than 1 point worse; not worse on the hand-written files.

**Result.** Met, at the limit. The best is 12 keywords in front: 73.9% to 76.9% within the first 5 (**+3.0** points, the criterion is 3); no language is worse (GraphQL 60.9% to 61.8%, JSON 75.2% to 75.2%, GLSL 71.8% to 82.6%, WGSL 82.8% to 89.9%, HTML 85.9% to 88.6%). On the hand-written files: 58.3% to 69.4% (+11.1), JSON back to 81.2%, GLSL 23.2% to 60.9%, WGSL 35.1% to 60.1%, GraphQL 55.0% to 62.1%.

**On the 800 suggestions of the review**, with the plugin's engine: the word is first in 339 cases (281 before the corrections of the grammar, 303 after them), among the first five in 444 (382, 395); with Ctrl+Space among the first twenty in 71 of 110 (42, 51). GLSL goes from 36 to 56 first places and WGSL from 37 to 48.

**Rerun after the missing vocabulary was added** (`main` and `gl_` of GLSL, the attributes, address spaces and access modes of WGSL, below): the test files give +2.96 points (74.5% to 77.4%), 0.04 under the criterion, with no language worse and +11.0 on the hand-written files. The criterion is a threshold on a measure that moves by a few tenths of a point with changes that have nothing to do with it, so the adoption stays: the result is **at the limit**, and the hand-written files are the evidence that holds.

**What it says, and what it does not.** The order of the keywords by use is a **prior learned from generated code**: it says which keywords this corpus uses, not which ones real projects use. The generators and the person reading the result are the same, and training and test files come from the same generators; the hand-written files are the only independent check. The prior is in [KeywordUse](../NestLight/Completion/KeywordUse.cs), written by `dotnet run --project NestLight.Experiments -- --priors <file>`, and is meant to be replaced by what the files of the user say.

**Revisit when.** There are real files to learn the order from, or the plugin learns it from the files the user opens.

## E36: should words of two letters be offered?

**Status:** Closed · **Decision:** Not met as written; replaced by E38

**Hypothesis.** The completion skips the words under 3 letters, so `id`, `db`, `in`, `uv` and `if` are never offered, and they are among the most written words of SQL, YAML and shaders ([SQL-5](suggestion-review/sql.md#sql-5), [YAML-39](suggestion-review/yaml.md#yaml-39), [WGSL-23](suggestion-review/wgsl.md#wgsl-23)). Offering them, after all the longer words, lets a person who types `i` find `id` without crowding the list for the person who wants a longer word.

**Test.** The corpus of 500 snippets for each language (the odd files) and the hand-written files of the review: 600 words per language typed with 1 or 2 letters, only the words that exist elsewhere in the file or are keywords. The engine of the plugin as it is (minimum length 3), with the minimum lowered to 2 for every word, and with the two-letter words in a tier after all the others.

**Criterion (as written).** A variant raises the words of 2 letters by at least 15 points within the first 5; lowers the longer words by no more than 0.5 point overall and 1 point in any language; and on the hand-written files the words of 2 letters gain at least 10 points while the longer ones lose no more than 1.

**Result.** Not met, because the criterion could not be met: on the test files the words of 2 letters were already at 85.2% within the first 5 (the follow-the-context tier has always accepted two letters), so a gain of 15 points was out of reach (the ceiling is +14.8). Minimum 2 reached 97.5% (+12.3), the two-letter tier 93.1% (+7.9). On the hand-written files the words of 2 letters went from 77.8% to 99.1% and 97.4% (+21.4 and +19.7). The longer words moved by -0.1 and 0.0 points. The threshold was a mistake of the person who wrote it; the experiment is closed and E38 restates it.

## E38: should words of two letters be offered? (E36 with a criterion that can be met)

**Status:** Done · **Decision:** Adopted (`ShortWordsLast`: the two-letter words after all the others)

**Criterion.** The same test as E36. A variant closes at least half of the distance to 100% for the words of 2 letters on the test files and on the hand-written files, and lowers the longer words by no more than 0.5 point overall and 1 point in any language (1 point on the hand-written files). Among the variants that meet it, the one that lowers the longer words least is adopted, then the one that gains most. It was written **after** E36's numbers: the numbers are the same, the criterion is the one E36 should have had.

**Result.** Both variants meet it. Minimum 2 closes 83% of the gap on the test files and 96% on the hand-written ones, with the longer words at -0.1 (worst language -0.4); the two-letter tier closes 53% and 89%, with the longer words unchanged (0.0 everywhere). The two-letter tier is adopted because it costs nothing in the corpus and because it keeps the longer words first whatever the host code has: the real host code is full of `if`, `in`, `of` and `fn`, which the corpus does not have, and the tier cannot put them in front of a longer word.

**On the 800 suggestions of the review** the cases that could not be solved now are: `id` after `c.` and `p.` ([SQL-5](suggestion-review/sql.md#sql-5), [SQL-86](suggestion-review/sql.md#sql-86)), `ci` and `db` in YAML, `uv` in GLSL and WGSL, `in` and `id` in WGSL; the word is first in 342 cases instead of 339 and among the first five in 456 instead of 444.

**Rerun after the missing vocabulary was added:** the baseline of the words of 2 letters moved from 85.2% to 86.6% (GLSL and WGSL now find `uv` and `id` through their own places), the two-letter tier gains +6.5 points, 0.2 under half of the gap (+6.7), and minimum 2 meets the criterion and would be the one chosen. The tier stays because the decision was made on the numbers before the vocabulary and for a reason the corpus cannot measure (the host code), and the difference between the variants is small; it is the most fragile decision of this work.

**Limits.** The words of 2 letters in the corpus are the ones its generators write.

## E37: does the similar-words stage make noise with short prefixes, and what removes it?

**Status:** Done · **Decision:** Not met; the stage stays as it is

**Hypothesis.** A review of 800 suggestions found the stage that corrects mistakes inventing suggestions with no relation when the person is typing a new word with 3 letters ([GraphQL-47](suggestion-review/graphql.md#graphql-47) `fir` offers `fragment`, [JSON-7](suggestion-review/json.md#json-7) `scr` offers `src`). With 3 letters one edit is a third of the word, so almost any word is "similar". Looking for similar words only from 4 letters, showing at most 3, or only the words of the file at 3 letters should remove most of that noise and keep most of the recovery.

**Test.** The corpus (odd files) and the hand-written files. Recovery: words typed with one mistake (a swap, a missing letter, a wrong one, an extra one; never the first letter) that leaves 3, 4, 5 or 6 letters typed, only the words that exist elsewhere in the file or are keywords (11,423 mistakes); the meant word within the first 5. Noise: words written once in the file, not keywords, typed with a correct prefix of 3, 4 and 5 letters (2,787 new words); how often any similar item is shown. Five variants: as it is; from 4 letters; at most 3 items at 3 letters; only the words of the file at 3 letters; both of the last two.

**Criterion.** A variant keeps at least 85% of the recovery of the current engine with 3 letters typed (relative), shows noise in at most half as many cases with a correct 3-letter prefix, keeps the recovery with 4 and 5 letters within 1 point, and goes the same way on the hand-written files.

**Result.** Not met. As it is: recovery 89.5% with 3 letters typed, 96.4% with 4, 96.9% with 5; noise 30.9% with 3 letters (a similar item is shown in about one new word in three), 17.0% with 4, 7.3% with 5.
- **From 4 letters** removes the noise at 3 letters (0.0%) and with it the recovery (16.1%, what the first stage finds by itself).
- **At most 3 items** changes nothing in the measure (a similar item is still shown), and loses recovery (78.7%).
- **Only the words of the file at 3 letters** is better on both counts in the corpus (recovery 93.9%, noise 27.6%), but it falls short of halving the noise, and on the hand-written files the recovery drops from 87.1% to 81.3%.

**What it says.** At 3 letters the stage corrects 9 mistakes in 10 and shows something unrelated in 3 new words in 10; no gate that was tried separates the two. The cost of the noise is a few extra items under the exact ones, that disappear with the next letter (17% at 4 letters, 7% at 5). Without knowing how often people mistype against how often they type a new word, there is no basis to take the recovery away. The measure also counts a single similar item as noise, which hides what the cap of 3 does to the size of the list.

**Revisit when.** There are real sessions to tell how many 3-letter prefixes are mistakes, or the stage can use how often a candidate is used (a candidate used five times is likelier than one used once).

## The missing vocabulary

The review of 800 suggestions listed words the plugin never offered. They are now, in three ways:
- **Words offered but not colored** ([Vocabularies.ForCompletion](../NestLight/Completion/Vocabularies.cs)): `main` and the `gl_` variables of GLSL (`gl_FragColor`, `gl_Position`, `gl_FragCoord`...) and `main` of WGSL. They are not in the vocabulary the tokenizers share, so E19 (offering a word and coloring it agree) keeps measuring the colored vocabulary only.
- **Places** ([Positions](../NestLight/Completion/Positions.cs)): GLSL after `#` (`version`, `define`, `ifdef`, `endif`...) and after `#version 300 ` (`es`, `core`); WGSL after `@` (`builtin`, `location`, `group`, `binding`, `vertex`, `fragment`, `compute`, `workgroup_size`...), inside `@builtin(` (`position`, `global_invocation_id`...), inside `@interpolate(`, inside `var<` (`uniform`, `storage`...) and after the comma (`read`, `write`, `read_write`).
- **Values** of HTML attributes (the ARIA roles, `aria-*`, `fill` and `stroke` with `currentColor`, `stroke-linecap`, `autocomplete`, `enctype`, `loading`, `meta name`, `script type`...) and of CSS properties (`font-family`, `background-size`, `background-repeat`, `scroll-behavior`, `mix-blend-mode`, `border-collapse`...).

On the 800 suggestions of the review the 15 cases of this kind that had no answer now have one: [GLSL-1](suggestion-review/glsl.md#glsl-1) `es`, [GLSL-15](suggestion-review/glsl.md#glsl-15) `main`, [GLSL-98](suggestion-review/glsl.md#glsl-98) `gl_FragColor`, [WGSL-19](suggestion-review/wgsl.md#wgsl-19) `builtin`, [WGSL-25](suggestion-review/wgsl.md#wgsl-25) `vertex`, [WGSL-64](suggestion-review/wgsl.md#wgsl-64) `read`, [HTML-63](suggestion-review/html.md#html-63) `region`, [HTML-85](suggestion-review/html.md#html-85) `currentColor`... They are not experiments: a word either exists or not, and the 58 tests of the places check each one.

## CSS in general

A pass over the CSS of the plugin, with a battery of tricky style sheets read token by token and in the completion.

**The tokenizer** ([CssTokenizer](../NestLight/Languages/Css/CssTokenizer.cs), it colors `css`, `<style>` and `style=""`):
- A selector no longer paints its punctuation as a name: `,`, `>`, `+`, `~`, parentheses and brackets are punctuation.
- `:not(.a, #b)`, `:is()`, `:where()`, `:has()` hold selectors, and `:nth-child(2n+1)` holds a formula (numbers), instead of one blob in the color of a tag.
- `input[type="text" i]`, `a[href^=http]`, `[disabled]`: the brackets and the operator are punctuation, the attribute is named, the value is a string or a word.
- The condition of an at-rule: `(min-width: 600px)`, `(width > 600px)`, `(hover)`, `@supports (display: grid)`, `@container card (min-width: 400px)` name the feature like a property (it was a value). A function in a prelude (`url()`, `layer()`) is still a function.
- Keyframe percentages (`50%`) are numbers; `1e3ms` and `2.5E-2` are one number each.

**The completion:**
- After `@` the at-rule names (`media`, `supports`, `keyframes`, `font-face`, `container`, `layer`...); inside `@supports (` the properties, inside `@container (` the size features; after `!` the word `important`.
- After a number the units: `10|` and `10r|` offer `px`, `rem`... (a hex color, a keyframe percentage and the `2n` of `:nth-child` are not units).
- All 148 named colors for the properties that take a color, and after the values of the shorthands that can have one (`border: 1px solid salm|`).
- 200 modern properties (`margin-inline`, `padding-block`, `container-type`, `accent-color`, `scrollbar-gutter`, `text-wrap`, the SVG ones), 360 in all; and the values of more of them.
- An attribute selector offers the attributes most tested (`type`, `href`, `disabled`...).
- **The properties come in the order of use** (the order of the keywords by use of E35, learned from generated code) and the rest alphabetically. The longer list made the alphabetical one worse: before the order, `display` was 71st in an empty property position and `box-sizing` 31st after `bo`; with it, 5th and 5th.

On the 800 suggestions of the review: the word first in 348 cases (345 before this pass), among the first five in 464 (456), among the first twenty in 494 (486); `display`, `color`, `border-radius` and `font-weight` in an empty property position went from 71st, 62nd, 41st and 92nd to 5th, 2nd, 10th and 9th. E35 rerun on the generated corpus: +5.4 points on the test files (it was +3.0), because the order by use also applies to the properties; that gain is fitted to the generator, and the hand-written files give +12.6.

Not done: the order by use applies to the properties only; the values of a property are still in the order of their table.

## CSS colors

The colors of the CSS (the same in `css`, `<style>` and `style=""`).

**Tokens that were one color and are now their own** ([CssTokenizer](../NestLight/Languages/Css/CssTokenizer.cs)):
- `#id` is no longer the color of a class: it has its own classification, in the same color and in bold.
- The name inside `[attr=...]` is not a property: it has its own classification, in the color of the properties and in italic.
- The unit is separated from its number (`1.5` `rem`, `50` `%`, `1e3` `ms`, also the `px` after a `${...}`).
- `!important` is not an at-rule: its own classification, in bold.

**The palette** ([CssClassificationDefinitions](../NestLight/VisualStudio/CssClassificationDefinitions.cs)): the colors were tuned for the dark theme only (one RGB per type, the plugin cannot read the theme without a reference to the Shell). A single color cannot reach 4.5:1 on a white and on a #1E1E1E editor at once (the best is about 4.0:1 on both), so the colors were moved to that balanced luminance and the checks are: at least 3.5:1 against both backgrounds, at least 40 apart in RGB when they differ, and two kinds of token that share a color differ in style ([CssColorTests](../NestLight.Tests/Highlighting/CssColorTests.cs), read from the source of the formats). The cost: on the dark theme the colors are a little darker than before.

Not done: a palette for each theme (needs the Shell assemblies in the project); the other languages still use the dark-only palette.

## CSS inside HTML

The highlighter has always colored the CSS of a `<style>` element and of a `style="..."` attribute as CSS; the completion saw only the string as a whole, so inside them it completed as HTML (words of the text and, with a mistake, tag names: `clipPath`, `main`, `map`). Now [NestedLanguages](../NestLight/Completion/NestedLanguages.cs) finds the CSS inside an HTML or SVG string (it skips comments and the interpolations of the host, so a `>` inside `${a => a > 1}` does not end a tag), and `Locate` gives a site of language `css` there: every rule of the CSS applies (properties, values, `:hover`, `@media`, `@keyframes`, functions, the words of the style sheets of the file). A style attribute starts inside a declaration list, with no selectors. An unfinished `<style>` or `style="` runs to the end of the string, which is how it is while typing.

Found on the way: the similar-words stage ignored the places where no keyword belongs (text, attribute values, JSON keys, literals) and offered tag names and keywords for a mistake. It now takes only the words of the file there ([HTML-30](suggestion-review/html.md#html-30) `scp` offers `scope` first instead of `script`).

Not done: the CSS inside the HTML is found for completion only; the highlighter has its own reading of the same places (`HtmlTokenizer`) and the two could share it. `<script>` is not completed: the plugin has no JavaScript.

## Notes on the context ranking

- **What the plugin runs with** (`CompletionFeatures.Default`): the previous word (E28), the words of the language (E29), the grammar (E30) and, where no rule decides, the words of the file first with the 12 most used keywords in front of them (E35); the words of two letters are offered after all the others (E38). Within the first 5, on the generated files: 72.5% with the order by distance alone, 85.5% with the previous word, 88.1% with the language, 97.2% with the grammar. The schema (E31) and the order by count (E32) stay in the code, off.
- **What the numbers are not.** Every context experiment ran on code made by a generator that was written with the same structure the features look for (a schema for the SQL, a value table for the CSS, an attribute table for the HTML). The gains say that the ideas work on code that has that structure. How much real code does is the open question, and the first thing to measure with files from real projects. The hit rate of 97% is a ceiling that real code will not reach.
- **After the review of 800 suggestions** (`docs/suggestion-review`) the places of the grammar were corrected and extended (E30 rerun: 88.1% to 97.8%). The review is by hand-written files and is a different measure from E28 to E33, which use generated code.
- **The order reaches the screen** only through the sort text of each item (`NestLightCompletionSource`), which has not been compiled or run in this work.
- **Cost:** the start of a session at 60,000 lines goes from about 4.0 ms (distance alone) to about 5.1 ms with the three features on; with the schema it would be 6.4 to 22 ms (the spread between runs is large). The words of the language need the scan of the strings, which the completion gets from the cache it shares with the classifier; on a scan that is not cached one more scan is paid (about 12 ms at 60,000 lines).
- **Tests:** 1,025 unit tests, among them the places of the grammar (80 cases), the schema (15), the previous word, the scope and the order of the words (22), and every cut of a text for the look-behind of the grammar and of the schema; E33 runs 85,633 carets with every feature on and finds no violation.

## Notes on the second stage

- **RNF2, the hot path** (E22 re-run on this commit, against the same file measured three times on the previous commit): the median of the 24 cells is 4% lower for the session with the scan shared, 4% lower for `Suggest` alone and 1.5% lower for the session with nothing shared. One cell, at 60,000 lines, is 7% higher, which is within the difference between identical runs; the bound of 5% holds as a median, not in every cell.
- **Tests:** 932 unit tests, among them the comparison of the banded distance with the definition on 200,000 random pairs (small alphabets, letters whose case changes in surprising ways, ranges inside longer strings), the same from 8 threads, the equality of the first stage with the engine without a matcher on 2,000 carets, cancellation, and the order of the similar words.

