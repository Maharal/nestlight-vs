# Experiments

A log of performance questions about NestLight: each one is a hypothesis, an automated test that can answer it, and the results observed so far.

The main question behind all of them: does NestLight slow Visual Studio down? Reading the code
([NestLightClassifier](../NestLight/VisualStudio/NestLightClassifier.cs), [SnapshotTokenCache](../NestLight/Highlighting/SnapshotTokenCache.cs)):
the number of languages is not the problem, because the tokenizers only run on marked strings.
What grows is the cost **per edit**, proportional to the file size: every new snapshot is copied (`GetText()`) and scanned, on the UI thread.

## How to read this file

- A **result is a snapshot**, valid for one commit, one runtime and one machine. The code changes, so results go stale: never edit a past run, add a new row to the experiment's *Runs* table.
- Every run records the **date, the commit, the runtime and the numbers**. A run without a commit cannot be compared with anything.
- A **decision** records why, and a **Revisit when** line says what would make the old answer wrong (a code change, a bigger file size, a new host).
- The **hypothesis, the test and the success criterion** are the stable part of an experiment. If one of them changes, the experiment gets a new id and the old one is closed.
- Statuses: `Planned`, `Done`, `Closed` (superseded). A done experiment has a decision: `Adopted`, `Rejected` or `Inconclusive`.
- Tests marked **Automated** are meant to live in the test project under `Category=Experiment`, so the regular test run skips them. **None of them is in the repository yet**: E01 to E04 ran from throwaway code, and the descriptions below are the specification for rewriting them. Tests marked **Manual** need Visual Studio.
- The success criteria of E01 to E04 were written **after** the runs, so they describe the decision rather than predict it. From E05 on, the criterion is written before the first run.

## Index

| Id | Question | Test | Status | Decision |
|---|---|---|---|---|
| [E01](#e01-baseline-cost-of-one-highlight-call) | How much does one `Highlight` call cost? | Automated | Done | Baseline |
| [E02](#e02-skip-the-scan-when-the-text-has-no-language-id) | Does skipping the scan without language ids help? | Automated | Done | Rejected |
| [E03](#e03-the-text-copy-on-every-edit) | How much does the `GetText()` copy cost? | Automated | Done | Rejected for now |
| [E04](#e04-the-language-registry-built-per-buffer) | Does building the registry per buffer matter? | Automated | Done | Rejected |
| [E05](#e05-allocations-inside-highlight) | Where do the allocations of `Highlight` come from? | Automated | Planned | |
| [E06](#e06-density-of-marked-strings) | Does the cost stay linear as more strings are marked? | Automated | Planned | |
| [E07](#e07-many-interpolations-in-one-string) | Does one string with many interpolations scale? | Automated | Planned | |
| [E08](#e08-throughput-of-each-embedded-language) | Is any tokenizer much slower than the others? | Automated | Planned | |
| [E09](#e09-malformed-and-pathological-input) | Does bad input make the cost explode? | Automated | Planned | |
| [E10](#e10-typing-latency-and-gc-inside-visual-studio) | What does the user feel while typing in a large file? | Manual | Planned | |
| [E11](#e11-background-re-analysis) | Does analyzing off the UI thread improve typing latency? | Manual | Planned | |
| [E12](#e12-incremental-analysis) | Is re-analyzing only the edited strings worth it? | Both | Planned | |

## Test conventions

Common to the automated experiments, so that runs can be compared:

- Release build, warm-up runs first, then the **median** of at least 25 measured runs.
- Synthetic files are built from a fixed unit of typical host code (comments, plain strings, interpolation) repeated up to the size under test. The unit is part of the test, so a change to it is a change to the experiment.
- Sizes: 1.2k, 12k and 60k lines unless the experiment says otherwise.
- Besides time, record bytes allocated (`GC.GetAllocatedBytesForCurrentThread`) and the GC collection counts per generation.
- A run is recorded with the commit, the date, the runtime version and the machine (CPU and core count).

Limit shared by all of them: synthetic code, not real files, and a runtime faster than the `net48` of Visual Studio. The numbers compare versions of the code, they are not times the user will see.

## E01: baseline cost of one `Highlight` call

**Status:** Done · **Decision:** Baseline

**Hypothesis.** Tokenizing is cheap enough that the number of languages and the scan are not what could make the editor slow.

**Test (Automated).** `Highlight(text)` (host scan + tokenization) for the 4 hosts, without and with marked strings. In the "with marker" cases, 1 unit in 10 has a marked string (`// language=sql`, `` html`...` ``, `R"(...)"`).

**Success criterion (written after the run).** Cost per edit stays well under a frame (16 ms) up to 60k lines.

**Runs.**

| Date | Commit | Runtime | JS 1.2k / 12k / 60k lines | C# | Python | C++ |
|---|---|---|---|---|---|---|
| 2026-10-06 | `b0e099f` | modern .NET, Release (machine not recorded) | 0.20 / 2.1 / 8.7 ms | 0.22 / 2.2 / 6.7 ms | 0.55 / 5.8 / 8.3 ms | 0.24 / 2.1 / 4.7 ms |

With marked strings (same run): C# 0.13 / 1.9 / 8.5 ms, Python 0.18 / 2.1 / 10.2 ms.

**Notes.** The test code of this run was a throwaway and is not in the repository: the run can be repeated only by rewriting the test from the description above.

**Decision.** Tokenizing is not the bottleneck. Any gain has to come from something that costs more than this, or from outside the scan.

## E02: skip the scan when the text has no language id

**Status:** Done · **Decision:** Rejected

**Hypothesis.** A string is only embedded code when a tag or comment carries a known id (`html`, `sql`, `json`...). If the text contains none of the 17 ids, the scan could not find anything, so skipping it saves time. JavaScript and C++ already have similar shortcuts (no `` ` `` / no `R"`); C# and Python have none.

**Test (Automated).** Prototype in `HighlightEngine.Highlight`: before the scan, `text.IndexOf(id, OrdinalIgnoreCase)` for each id, returning no tokens if none appears. The E01 benchmark, before and after.

**Success criterion (written after the run).** At least 2x faster on files without markers, and no slowdown on files with them.

**Runs.**

| Date | Commit | Case | Baseline | With shortcut |
|---|---|---|---:|---:|
| 2026-10-06 | `b0e099f` | C#, 1.3k lines, no marker | 0.22 ms | 0.04 ms |
| | | C#, 13.5k lines, no marker | 2.2 ms | 0.45 ms |
| | | C#, 67k lines, no marker | 6.7 ms | 3.9 ms |
| | | JS, no marker (the code uses `JSON.stringify`) | 0.20 ms | 0.25 ms |
| | | C#, 1.3k lines, **with** marker | 0.13 ms | 0.33 ms |
| | | C#, 13.6k lines, **with** marker | 1.9 ms | 2.8 ms |
| | | Python, 1.4k lines, **with** marker | 0.18 ms | 0.70 ms |
| | | Python, 13.6k lines, **with** marker | 2.1 ms | 5.2 ms |

**Decision.** Rejected, the second half of the criterion fails (up to 3.9x slower with markers). The gain shows up only where the scan already costs a fraction of a millisecond, and short ids (`json`, `md`, `xml`) appear in ordinary code and cancel the shortcut.

**Revisit when.** The scan becomes much more expensive per character, or the id list becomes long enough that a single pass (a trie, `SearchValues`) is the natural way to look for them.

## E03: the text copy on every edit

**Status:** Done · **Decision:** Rejected for now

**Hypothesis.** On every new snapshot the classifier copies the whole text into a string. Above ~85 KB (about 42k characters) that string lands on the Large Object Heap, which only gen2 collections reclaim. The copy costs time, and the collections cost more.

**Test (Automated).** Simulated typing on a synthetic C# file: 1000 edits (300 for the largest file), each doing what the classifier does, in three variants: copy + `Highlight` (current), copy only, and `Highlight` only on an existing string.

**Success criterion (written after the run).** Removing the copy would save at least 1 ms per edit, or at least 30% of the cost, on files up to 1 MB.

**Runs.**

| Date | Commit | File | Copy + highlight | Copy only | Highlight only | Gen2 (copy + highlight / highlight only) |
|---|---|---|---:|---:|---:|---:|
| 2026-10-06 | `b0e099f` | 78 KB | 0.32 ms | 0.01 ms | 0.24 ms | 0 / 0 |
| | | 195 KB | 0.63 ms | 0.24 ms | 0.39 ms | 62 / 0 |
| | | 781 KB | 2.5 ms | 0.19 ms | 1.5 ms | 208 / 0 |
| | | 1.9 MB | 6.2 ms | 0.67 ms | 4.0 ms | 447 / 1 |
| | | 7.8 MB (300 edits) | 22 ms | 3.4 ms | 18 ms | 148 / 55 |

**What it shows.**
- The mechanism is real: once the copy goes to the LOH, a gen2 collection happens every 2 to 5 edits, against almost none without it.
- The time cost is small. The copy takes 0.2 to 0.7 ms up to 2 MB. Removing it would save at most 0.1 to 1 ms per edit up to ~800 KB (25 to 40%) and 2.2 ms at 1.9 MB. That is an upper bound: scanning directly over `ITextSnapshot` instead of a `string` makes every character access slower.
- Typical large files (under 200 KB) spend 0.6 ms per edit in total.

**Decision.** Rejected for now: the change touches every scanner and its tests for a gain below a millisecond on files of normal size.

**Caveat.** The benchmark process has a tiny heap. A gen2 collection in Visual Studio walks a heap of hundreds of megabytes, so each one costs more there than here. E10 measures that.

**Revisit when.** E10 shows gen2 pauses while typing, or the scanners are rewritten to take a span or a snapshot.

## E04: the language registry built per buffer

**Status:** Done · **Decision:** Rejected

**Hypothesis.** `CreateHighlighter` builds all the tokenizers again for every open file, and that cost adds up.

**Test (Automated).** 200 calls to `CreateLanguages()` (time and bytes), and 100 consecutive `CreateHighlighter` calls for each host.

**Success criterion (written after the run).** More than 1 ms or 1 MB per open file.

**Runs.**

| Date | Commit | `CreateLanguages()` | 100 buffers |
|---|---|---|---|
| 2026-10-06 | `b0e099f` | 45 µs (p99 64 µs), 11 KB | 3 to 6 ms, ~1.1 MB in total |

**Decision.** Rejected: the cost per open file is far below the criterion.

**Revisit when.** The number of tokenizers grows by an order of magnitude, or a tokenizer starts building large tables in its constructor.

## E05: allocations inside `Highlight`

**Status:** Planned

**Hypothesis.** `Highlight` allocates about twice the size of the text per edit (seen in E03: 1.5 MB for a 781 KB file), more than the copy itself. A few call sites probably account for most of it: one `Substring` per comment in `MarkerTracker.Comment`, the token and embedded-string lists, the `Decode` buffers.

**Test (Automated).** Bytes allocated per `Highlight` call and per character of input, per host, using `GC.GetAllocatedBytesForCurrentThread`. To find the call sites, an allocation profile of one call on a 1 MB file (an allocation-sampling trace).

**Success criterion.** Two or three call sites account for at least half of the bytes, and removing them cuts the allocation by a third or more with no change in the tokens.

**Preliminary observation (from the E03 runs, `b0e099f`).** About 2 bytes allocated per byte of text in the highlight-only variant. Mostly gen0, so cheap to collect.

**Runs.** None yet.

## E06: density of marked strings

**Status:** Planned

**Hypothesis.** The cost depends on the size of the file, and each marked string adds a roughly constant amount, with no step in between (a sort, a nested scan).

**Test (Automated).** A fixed 400 KB file where 0%, 1%, 10%, 50% and 100% of the units have a marked string. Time against the number of embedded strings.

**Success criterion.** The cost per marked string differs by less than 25% between 1% and 100% density.

**Runs.** None yet. E01 compared only 0% and 10%.

## E07: many interpolations in one string

**Status:** Planned

**Hypothesis.** `AddClipped` walks the interpolations of a string from the first one for every token it emits, so a single large template with many `${...}` costs O(tokens x interpolations). Suspected from reading the code, not measured.

**Test (Automated).** One `html` template with 10, 100, 1,000 and 10,000 interpolations, in JavaScript and C#. Time against the number of interpolations.

**Success criterion.** Doubling the interpolations less than 2.5x the time (close to linear).

**Runs.** None yet.

## E08: throughput of each embedded language

**Status:** Planned

**Hypothesis.** All the tokenizers run at a similar speed in characters per millisecond. One of them (regex, markdown, YAML) may be much slower on large inputs.

**Test (Automated).** A single marked string of 100 KB and of 1 MB for each language, in a JavaScript host. Characters per millisecond, and the ratio between the two sizes.

**Success criterion.** No language is more than 5x slower than the median, and the 1 MB time is under 12x the 100 KB time.

**Runs.** None yet.

## E09: malformed and pathological input

**Status:** Planned

**Hypothesis.** Code being typed is often malformed. An unterminated string, comment or template early in a large file, very deep nesting, or a very long line can make the scan or a tokenizer quadratic. The existing robustness tests only require less than 5 seconds, which would hide a slowdown of several orders of magnitude.

**Test (Automated).** For each host: an unterminated string / comment / template at the top of a 1 MB file, 1,000 levels of nesting, and a 1 MB line. Run at N and 2N and compare.

**Success criterion.** time(2N) / time(N) stays under 2.5 for every case.

**Runs.** None yet.

## E10: typing latency and GC inside Visual Studio

**Status:** Planned

**Hypothesis.** In a real session, the gen2 collections caused by the text copy (E03) and the analysis on the UI thread are enough to make typing in a large file noticeable.

**Test (Manual).** Open a large real file (generated code, a JS bundle of 5k to 50k lines), type continuously, and record the per-keystroke latency and the gen2 count and pause time with and without the extension. A fixed file and script, kept with the result, so runs can be repeated.

**Success criterion.** A p95 latency increase under 8 ms, and no gen2 pause above 20 ms.

**Runs.** None yet.

## E11: background re-analysis

**Status:** Planned (depends on E10 showing a problem)

**Hypothesis.** Returning the tokens of the previous snapshot and raising `ClassificationChanged` when the new analysis finishes takes the scan off the UI thread and improves typing latency.

**Test (Manual).** The E10 session before and after the change.

**Success criterion.** The p95 latency in E10 goes back to within 2 ms of the baseline without the extension.

**Risks.** Colors stale for an instant, and more concurrency complexity.

**Runs.** None yet.

## E12: incremental analysis

**Status:** Planned (depends on E11 not being enough)

**Hypothesis.** Re-analyzing only the strings hit by the edit and reusing the tokens of the rest is much cheaper than the full scan.

**Test (Both).** Automated: the cost of a one-character edit in the middle of a file, against the full scan, over the E01 sizes. Manual: the E10 session. Plus a differential test: after any sequence of edits, the incremental tokens must equal the tokens of a full scan.

**Success criterion.** At least 5x cheaper per edit at 60k lines, and the differential test passes on the existing samples and on random edits.

**Risks.** The largest of all: escapes, nested interpolations and unterminated strings shift everything after the edit point.

**Runs.** None yet.
