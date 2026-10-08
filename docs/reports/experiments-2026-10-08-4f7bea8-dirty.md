# NestLight experiments report

| | |
|---|---|
| Date | 2026-10-08 23:32 UTC |
| Commit | `4f7bea8` (**uncommitted changes**: this run cannot be reproduced from the commit) |
| Runtime | .NET 10.0.0, Release |
| Machine | CPU not identified, 16 logical cores |
| OS | Linux Mint 22 |
| Mode | full |

Definitions of the experiments: [docs/experiments.md](../experiments.md). Each result is valid only for the commit and the machine above.

## Summary

| Id | Experiment | Criterion | Finding |
|---|---|---|---|
| E01 | Baseline cost of one Highlight call | Met | worst case 9.03 ms at 60000 lines |
| E02 | Skip the scan when the text has no language id | Not met | best 2.51x without markers, worst 0.79x with them |
| E03 | The text copy on every edit | Not met | possible saving 314 µs per edit at 400k characters |
| E04 | The language registry built per buffer | Met | worst 15 µs and 11 KB per buffer |
| E05 | Where Highlight allocates | Not met | Highlight allocates 1.18x the text on average; weakest dominant stage 69% |
| E06 | Number of marked strings | Not met | worst growth 16.73x for 8x the strings |
| E07 | Many interpolations in one string | Met | worst growth 13.95x for 10x interpolations |
| E08 | Throughput of each embedded language | Not met | slowest sql at 1.4x below the median; worst growth 22.08x |
| E09 | Malformed and pathological input | Not met | worst growth 3.48x when doubling (CSharp, one very long line) |
| E16 | Completion latency against the size of the file | Met | worst case 9.33 ms at 60000 lines |
| E17 | Do the limits of the completion change its cost? | Not met | largest difference 3.23x (distinct words, max 10000, min length 3) |
| E19 | Does the tokenizer agree with the vocabulary? | Met | vocabulary and tokenizers agree |
| E20 | Order of the words of the document | Met | nearest 72.1% against alphabetical 64.0% within the first 5 (locality 0.5) |
| E21 | Where the words come from, and how many keystrokes completion saves | Not met | whole document saves 55.3% of the characters; best narrower scope (all embedded strings) 58.0% |
| E22 | Sharing the scan and not creating the words of the completion | Met | worst case with the scan shared 1.17 ms at 60000 lines |
| E23 | Does the second stage of the completion fit in a frame? | Met | worst case with the second stage forced 10.44 ms at 60000 lines |
| E24 | Does the second stage recover the word after one mistake, and which tie-break works? | Met | best tie-break (nearest to the caret (the engine)) 72.0% within the first 5 |
| E25 | Does the second stage get in the way when the prefix is right? | Met | with FuzzyBelow = 1 similar items are added in 1.1% of the cases |
| E27 | Completion with similar words on incomplete and cut code | Met | 0 violations in 86900 carets, 21275 similar items checked |

*Criterion met* is the statement of each experiment's own criterion, not a good/bad grade: see *If met* and *If not met* under each experiment.

## E01: Baseline cost of one Highlight call

**Hypothesis.** Tokenizing is cheap enough that the number of languages and the scan of the host are not what could make the editor slow.

**Method.** Highlight(text) (host scan + tokenization) for the 4 hosts, with no marked string and with one unit in 10 carrying a marked string, on files of increasing size.

**Criterion.** Every case stays under 16 ms (one frame) at the largest size.

**Median time of one Highlight call**

| Host | Marked strings | Lines | Characters | Time | Tokens |
|---|---|---|---|---|---|
| JavaScript | none | 1200 | 41550 | 126 µs | 0 |
| JavaScript | none | 12000 | 415500 | 1.39 ms | 0 |
| JavaScript | none | 60000 | 2077500 | 4.81 ms | 0 |
| JavaScript | 1 unit in 10 | 1200 | 41760 | 110 µs | 165 |
| JavaScript | 1 unit in 10 | 12000 | 417600 | 1.11 ms | 1650 |
| JavaScript | 1 unit in 10 | 60000 | 2088000 | 6.80 ms | 8250 |
| CSharp | none | 1200 | 34974 | 89 µs | 0 |
| CSharp | none | 12000 | 348174 | 916 µs | 0 |
| CSharp | none | 60000 | 1740087 | 4.98 ms | 0 |
| CSharp | 1 unit in 10 | 1200 | 35026 | 108 µs | 112 |
| CSharp | 1 unit in 10 | 12000 | 349671 | 1.18 ms | 1056 |
| CSharp | 1 unit in 10 | 60000 | 1748094 | 6.34 ms | 5280 |
| Python | none | 1200 | 29346 | 156 µs | 0 |
| Python | none | 12000 | 292146 | 1.58 ms | 0 |
| Python | none | 60000 | 1460073 | 7.98 ms | 0 |
| Python | 1 unit in 10 | 1200 | 29468 | 166 µs | 112 |
| Python | 1 unit in 10 | 12000 | 294141 | 1.72 ms | 1056 |
| Python | 1 unit in 10 | 60000 | 1470486 | 9.03 ms | 5280 |
| Cpp | none | 1200 | 32550 | 76 µs | 0 |
| Cpp | none | 12000 | 325500 | 758 µs | 0 |
| Cpp | none | 60000 | 1627500 | 3.93 ms | 0 |
| Cpp | 1 unit in 10 | 1200 | 32768 | 87 µs | 60 |
| Cpp | 1 unit in 10 | 12000 | 325915 | 846 µs | 596 |
| Cpp | 1 unit in 10 | 60000 | 1629025 | 4.65 ms | 2964 |

**Analysis.**

- The slowest case at 60000 lines is Python, with markers: 9.03 ms (the frame budget is 16 ms).

**Criterion met.** One Highlight call is not what makes typing slow, even on very large files; look elsewhere (copy, UI thread, GC).

*Ran in 2.2 s.*

## E02: Skip the scan when the text has no language id

**Hypothesis.** A string is only embedded code when a tag or comment carries a known language id. If the text contains none of them, the scan could not find anything, so skipping it saves time on C# and Python, which have no shortcut of their own.

**Method.** A decorator around the real highlighter looks for each language id with IndexOf(OrdinalIgnoreCase) and returns no tokens when none appears. Same files, with and without the decorator.

**Criterion.** At least 2x faster on every file without marked strings, and no more than 1.1x slower on every file with them.

**Median time, current engine against the decorated one**

| Host | Marked strings | Lines | Current | With shortcut | Speed-up |
|---|---|---|---|---|---|
| CSharp | none | 1200 | 88 µs | 74 µs | 1.19x |
| CSharp | none | 12000 | 859 µs | 764 µs | 1.12x |
| CSharp | none | 60000 | 4.60 ms | 3.91 ms | 1.18x |
| CSharp | 1 unit in 10 | 1200 | 109 µs | 123 µs | 0.88x |
| CSharp | 1 unit in 10 | 12000 | 1.02 ms | 1.24 ms | 0.83x |
| CSharp | 1 unit in 10 | 60000 | 5.79 ms | 7.33 ms | 0.79x |
| Python | none | 1200 | 161 µs | 64 µs | 2.51x |
| Python | none | 12000 | 1.55 ms | 655 µs | 2.36x |
| Python | none | 60000 | 7.79 ms | 3.30 ms | 2.36x |
| Python | 1 unit in 10 | 1200 | 170 µs | 183 µs | 0.93x |
| Python | 1 unit in 10 | 12000 | 1.67 ms | 1.86 ms | 0.90x |
| Python | 1 unit in 10 | 60000 | 8.95 ms | 9.88 ms | 0.91x |

**Analysis.**

- Best speed-up on files without markers: 2.51x. Worst result on files with markers: 0.79x (below 1x means slower).

**Criterion not met.** The shortcut is not worth adding: it either gains too little where the scan is already cheap or penalizes the files that use the plugin.

*Ran in 2.2 s.*

## E03: The text copy on every edit

**Hypothesis.** On every new snapshot the classifier copies the whole text into a string. Above ~85 KB (about 42k characters) that string lands on the Large Object Heap, which only gen2 collections reclaim: the copy costs time, and the collections cost more.

**Method.** Simulated typing on a synthetic C# file: each edit does what the classifier does (copy the text into a new string, then Highlight it). Three variants: copy + Highlight (current), copy only, Highlight only on an existing string. The time is the mean per edit, so that GC pauses are counted.

**Criterion.** Removing the copy would save at least 1 ms per edit on the 400,000-character file (about 12k lines, a large but ordinary file).

**Mean time per edit**

| File | Copy + highlight | Copy only | Highlight only | Possible saving | Gen2 collections (copy + highlight / highlight only) | Allocated per edit (copy + highlight) |
|---|---|---|---|---|---|---|
| 196 KB | 452 µs | 114 µs | 307 µs | 145 µs (32%) | 18 / 0 | 480 KB |
| 781 KB | 1.59 ms | 222 µs | 1.28 ms | 314 µs (20%) | 66 / 0 | 1.9 MB |
| 1.9 MB | 4.42 ms | 759 µs | 3.30 ms | 1.12 ms (25%) | 108 / 0 | 4.7 MB |
| 7.6 MB | 18.88 ms | 2.81 ms | 14.93 ms | 3.95 ms (21%) | 123 / 107 | 18.6 MB |

**Analysis.**

- At 400,000 characters, removing the copy would save at most 314 µs per edit (the "highlight only" column is the best case: reading from a snapshot instead of a string would be slower).
- Gen2 collections over the 300 edits of every size: 315 with the copy, 107 without it.
- The benchmark process has a tiny heap; a gen2 collection in Visual Studio walks a much larger one, so the real cost of the collections is probably higher (see E10).

**Criterion not met.** The copy is not worth removing: touching every scanner would buy less than a millisecond on files of ordinary size.

*Ran in 15.3 s.*

## E04: The language registry built per buffer

**Hypothesis.** CreateHighlighter builds all the tokenizers again for every open file, and that cost adds up when many files are open.

**Method.** Time and allocation of CreateLanguages(), and of 100 consecutive CreateHighlighter calls (100 buffers) for each host.

**Criterion.** Opening a buffer costs less than 1 ms and 1 MB, for every host.

**Cost of opening one buffer**

| Host | Time per buffer | Allocated per buffer |
|---|---|---|
| CreateLanguages() alone | 15 µs | 11 KB |
| CreateHighlighter(JavaScript) | 14 µs | 11 KB |
| CreateHighlighter(CSharp) | 14 µs | 11 KB |
| CreateHighlighter(Python) | 14 µs | 11 KB |
| CreateHighlighter(Cpp) | 14 µs | 11 KB |

**Analysis.**

- Opening a buffer costs at most 15 µs and 11 KB across the hosts.

**Criterion met.** The registry per buffer is not a cost worth removing.

*Ran in 0.2 s.*

## E05: Where Highlight allocates

**Hypothesis.** Highlight allocates about twice the size of the text per call (seen while running E03), and one of its two stages, the host scan or the rest (decoding, tokenizing, mapping back), accounts for most of it.

**Method.** Bytes allocated by the host scan alone and by the whole Highlight call, on the same file; the rest is the difference. With and without marked strings, for the 4 hosts, at the second size.

**Criterion.** On every host, with marked strings, one stage accounts for at least 70% of the bytes.

**Allocation per call**

| Host | Marked strings | Text | Scan | Whole Highlight | Highlight / text | Scan share | Rest share |
|---|---|---|---|---|---|---|---|
| JavaScript | none | 812 KB | 610 KB | 610 KB | 0.75x | 100% | 0% |
| JavaScript | 1 unit in 10 | 816 KB | 618 KB | 801 KB | 0.98x | 77% | 23% |
| CSharp | none | 680 KB | 834 KB | 834 KB | 1.23x | 100% | 0% |
| CSharp | 1 unit in 10 | 683 KB | 833 KB | 996 KB | 1.46x | 84% | 16% |
| Python | none | 571 KB | 803 KB | 803 KB | 1.41x | 100% | 0% |
| Python | 1 unit in 10 | 574 KB | 802 KB | 965 KB | 1.68x | 83% | 17% |
| Cpp | none | 636 KB | 258 KB | 258 KB | 0.41x | 100% | 0% |
| Cpp | 1 unit in 10 | 637 KB | 264 KB | 380 KB | 0.60x | 69% | 31% |

**Analysis.**

- With marked strings, Highlight allocates 1.18x the size of the text on average (UTF-16, 2 bytes per character).
- The stage that dominates the least accounts for 69% of the bytes of its host.
- This splits the allocation by stage only; finding the call sites needs an allocation profile of the stage that dominates.

**Criterion not met.** The allocations are spread over both stages: reducing them would take several separate changes.

*Ran in 0.6 s.*

## E06: Number of marked strings

**Hypothesis.** The cost grows only linearly with the number of embedded strings: nothing in the engine (a sort, a nested scan) grows faster than the number of strings it has to handle.

**Method.** A file where every unit carries a marked string, at 1x, 2x, 4x and 8x a base size, for each host. The number of embedded strings grows with the file, and so should the time.

**Criterion.** 8 times more embedded strings cost less than 10 times the time (linear is 8x, quadratic is 64x), for every host.

**Time by number of embedded strings**

| Host | Characters | Embedded strings | Time | Per embedded string | Gen2 collections |
|---|---|---|---|---|---|
| JavaScript | 100104 | 344 | 602 µs | 2 µs | 0 in 25 runs |
| JavaScript | 200208 | 688 | 1.23 ms | 2 µs | 0 in 25 runs |
| JavaScript | 400125 | 1375 | 2.77 ms | 2 µs | 1 in 25 runs |
| JavaScript | 800250 | 2750 | 10.08 ms | 4 µs | 4 in 25 runs |
| CSharp | 100264 | 332 | 623 µs | 2 µs | 0 in 25 runs |
| CSharp | 200226 | 663 | 1.24 ms | 2 µs | 0 in 25 runs |
| CSharp | 400150 | 1325 | 2.63 ms | 2 µs | 1 in 25 runs |
| CSharp | 800300 | 2650 | 9.53 ms | 4 µs | 4 in 25 runs |
| Python | 100233 | 387 | 912 µs | 2 µs | 0 in 25 runs |
| Python | 200207 | 773 | 1.83 ms | 2 µs | 0 in 25 runs |
| Python | 400155 | 1545 | 3.90 ms | 3 µs | 2 in 25 runs |
| Python | 800051 | 3089 | 12.56 ms | 4 µs | 4 in 25 runs |
| Cpp | 100122 | 407 | 496 µs | 1 µs | 0 in 25 runs |
| Cpp | 200244 | 814 | 999 µs | 1 µs | 0 in 25 runs |
| Cpp | 400242 | 1627 | 2.03 ms | 1 µs | 0 in 25 runs |
| Cpp | 800238 | 3253 | 4.89 ms | 2 µs | 2 in 25 runs |

**Analysis.**

- JavaScript: 8x the embedded strings multiplied the time by 16.73x.
- CSharp: 8x the embedded strings multiplied the time by 15.29x.
- Python: 8x the embedded strings multiplied the time by 13.77x.
- Cpp: 8x the embedded strings multiplied the time by 9.85x.

**Criterion not met.** The cost grows faster than the number of embedded strings: look for a quadratic step in the engine.

*Ran in 2.2 s.*

## E07: Many interpolations in one string

**Hypothesis.** The engine clips every token against the interpolations of its string starting from the first one, so a single large template with many interpolations costs O(tokens x interpolations). Suspected from reading HighlightEngine.AddClipped, not measured before.

**Method.** One marked HTML string with 100, 1,000 and 10,000 interpolations, in JavaScript, C# and Python. Time against the number of interpolations.

**Criterion.** Ten times more interpolations cost less than 20 times the time (linear is 10x, quadratic is 100x), for every host.

**Time by number of interpolations**

| Host | Interpolations | Time | Per interpolation | Tokens |
|---|---|---|---|---|
| JavaScript | 100 | 119 µs | 1 µs | 1100 |
| JavaScript | 1000 | 1.59 ms | 2 µs | 11000 |
| JavaScript | 10000 | 22.21 ms | 2 µs | 110000 |
| CSharp | 100 | 124 µs | 1 µs | 1100 |
| CSharp | 1000 | 1.58 ms | 2 µs | 11000 |
| CSharp | 10000 | 21.86 ms | 2 µs | 110000 |
| Python | 100 | 126 µs | 1 µs | 1100 |
| Python | 1000 | 1.62 ms | 2 µs | 11000 |
| Python | 10000 | 21.88 ms | 2 µs | 110000 |

**Analysis.**

- JavaScript: 100 to 1000 interpolations multiplied the time by 13.41x.
- JavaScript: 1000 to 10000 interpolations multiplied the time by 13.95x.
- CSharp: 100 to 1000 interpolations multiplied the time by 12.72x.
- CSharp: 1000 to 10000 interpolations multiplied the time by 13.85x.
- Python: 100 to 1000 interpolations multiplied the time by 12.84x.
- Python: 1000 to 10000 interpolations multiplied the time by 13.54x.

**Criterion met.** Large templates scale roughly linearly.

*Ran in 0.6 s.*

## E08: Throughput of each embedded language

**Hypothesis.** All the tokenizers run at a similar speed in characters per millisecond, and none gets slower per character on larger inputs. One of them (regex, markdown, YAML...) may be much slower.

**Method.** One marked string per language, in a JavaScript host, of 100,000 and of 1,000,000 characters made by repeating a snippet of typical code of that language. Characters per millisecond, and the ratio between the two times.

**Criterion.** The slowest language is less than 5 times slower than the median one, and every language takes less than 12 times longer on 10 times the text.

**Throughput by language**

| Language | Small time | Small chars/ms | Large time | Large chars/ms | Large / small time | Tokens (large) |
|---|---|---|---|---|---|---|
| html | 2.59 ms | 38625 | 49.39 ms | 20248 | 19.06x | 268668 |
| css | 2.64 ms | 37949 | 44.88 ms | 22284 | 17.01x | 291964 |
| sql | 3.13 ms | 31902 | 69.21 ms | 14450 | 22.08x | 250029 |
| json | 3.09 ms | 32335 | 65.35 ms | 15302 | 21.13x | 367111 |
| graphql | 2.80 ms | 35698 | 41.00 ms | 24392 | 14.63x | 170460 |
| xml | 2.71 ms | 36873 | 52.33 ms | 19109 | 19.29x | 333342 |
| markdown | 1.11 ms | 90117 | 15.02 ms | 66570 | 13.53x | 68184 |
| yaml | 4.08 ms | 24495 | 58.41 ms | 17120 | 14.31x | 360000 |
| regex | 3.64 ms | 27474 | 65.95 ms | 15162 | 18.12x | 404770 |
| glsl | 1.90 ms | 52611 | 30.61 ms | 32665 | 16.10x | 103896 |
| wgsl | 1.69 ms | 59275 | 31.24 ms | 32015 | 18.50x | 101451 |

**Analysis.**

- Median throughput 20248 chars/ms; the slowest language (sql) runs at 14450 chars/ms, 1.4x below the median.
- Worst time growth for 10x the text: 22.08x (linear is 10x).

**Criterion not met.** At least one tokenizer is an outlier or superlinear: it is where a large embedded string will hurt first.

*Ran in 5.5 s.*

## E09: Malformed and pathological input

**Hypothesis.** Code being typed is often malformed. An unterminated string or comment early in a large file, deep nesting, or a very long line can make the scan or a tokenizer quadratic. The robustness tests of the test project only require less than 5 seconds, which would hide a slowdown of several orders of magnitude.

**Method.** For each host: an unterminated marked string and an unterminated block comment at the top of a large file, deep nesting, and one very long line. Each case at a size N and at 2N; the ratio between the two times shows the growth.

**Criterion.** Doubling the input multiplies the time by less than 2.5 (linear is 2, quadratic is 4) in every case, ignoring cases that take less than 5 ms at 2N.

**Time at N and at 2N**

| Host | Case | N | Time at N | Time at 2N | Growth | Gen2 collections (N / 2N) |
|---|---|---|---|---|---|---|
| JavaScript | unterminated marked string at the top | 250000 chars | 8.68 ms | 21.68 ms | 2.50x | 6 / 9 |
| JavaScript | one very long line | 250000 chars | 1.10 ms | 2.20 ms | 2.00x (ignored: under 5 ms) | 0 / 0 |
| JavaScript | unterminated block comment at the top | 250000 chars | 9 µs | 15 µs | 1.72x (ignored: under 5 ms) | 0 / 0 |
| JavaScript | deep nesting | 200 levels | 8 µs | 24 µs | 3.20x (ignored: under 5 ms) | 0 / 0 |
| CSharp | unterminated marked string at the top | 250000 chars | 9.98 ms | 21.91 ms | 2.20x | 6 / 9 |
| CSharp | one very long line | 250000 chars | 1.55 ms | 5.38 ms | 3.48x | 0 / 0 |
| CSharp | unterminated block comment at the top | 250000 chars | 38 µs | 74 µs | 1.96x (ignored: under 5 ms) | 0 / 0 |
| CSharp | deep nesting | 200 levels | 24 µs | 65 µs | 2.74x (ignored: under 5 ms) | 0 / 0 |
| Python | unterminated marked string at the top | 250000 chars | 9.81 ms | 25.58 ms | 2.61x | 6 / 8 |
| Python | one very long line | 250000 chars | 3.48 ms | 7.21 ms | 2.07x | 0 / 0 |
| Python | deep nesting | 200 levels | 6 µs | 17 µs | 2.84x (ignored: under 5 ms) | 0 / 0 |
| Cpp | unterminated marked string at the top | 250000 chars | 8.74 ms | 23.26 ms | 2.66x | 6 / 8 |
| Cpp | one very long line | 250000 chars | 2.18 ms | 4.17 ms | 1.91x (ignored: under 5 ms) | 0 / 0 |
| Cpp | unterminated block comment at the top | 250000 chars | 38 µs | 74 µs | 1.97x (ignored: under 5 ms) | 0 / 0 |
| Cpp | deep nesting | 200 levels | 3 µs | 8 µs | 2.59x (ignored: under 5 ms) | 0 / 0 |

**Analysis.**

- The worst growth when doubling the input is 3.48x (CSharp, one very long line); linear is 2x, quadratic is 4x.
- Nesting stops at 400 levels: the stack of the process limits how deep the engine can recurse (the test project covers 400 levels).

**Criterion not met.** At least one kind of malformed input is superlinear: it will freeze the editor on a large file while the user is typing.

*Ran in 1.4 s.*

## E16: Completion latency against the size of the file

**Hypothesis.** Every keystroke in an embedded string scans the host to find the string (Locate) and the whole text for words (Suggest). Those two scans, plus a dictionary of the matching words, are cheap on a normal file but could be noticeable on a large one, mostly when thousands of distinct words share the prefix.

**Method.** For each host, a file of increasing size with the caret at the end of an open `comp` in a marked SQL string. Two shapes: typical code (E01's file, a few distinct words) and a file whose every line declares a new `compNNNNN` identifier (thousands of distinct matches). Time of Locate alone and of Locate + Suggest, without the text copy of the editor (E03).

**Criterion.** Locate + Suggest stay under 16 ms (one frame) at the largest size, in every host and shape.

**Median time of one completion**

| Host | Shape | Lines | Characters | Locate | Locate + Suggest | Suggestions |
|---|---|---|---|---|---|---|
| JavaScript | typical code | 1200 | 41771 | 93 µs | 191 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 920 µs | 1.83 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 4.95 ms | 6.10 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 36 µs | 102 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 455 µs | 906 µs | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 2.29 ms | 3.44 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 91 µs | 175 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 904 µs | 1.70 ms | 1 |
| CSharp | typical code | 60000 | 1748126 | 4.88 ms | 6.15 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 43 µs | 112 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 419 µs | 974 µs | 100 |
| CSharp | distinct words | 60000 | 1200032 | 2.08 ms | 3.50 ms | 100 |
| Python | typical code | 1200 | 29494 | 157 µs | 226 µs | 1 |
| Python | typical code | 12000 | 294167 | 1.54 ms | 2.27 ms | 1 |
| Python | typical code | 60000 | 1470512 | 8.14 ms | 9.33 ms | 1 |
| Python | distinct words | 1200 | 18026 | 72 µs | 122 µs | 100 |
| Python | distinct words | 12000 | 180026 | 709 µs | 1.11 ms | 100 |
| Python | distinct words | 60000 | 900026 | 3.57 ms | 4.63 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 80 µs | 158 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 772 µs | 1.52 ms | 1 |
| Cpp | typical code | 60000 | 1629061 | 4.05 ms | 5.21 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 54 µs | 116 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 479 µs | 1.05 ms | 100 |
| Cpp | distinct words | 60000 | 1200036 | 2.38 ms | 3.57 ms | 100 |

**Analysis.**

- The slowest case at 60000 lines is Python, typical code: 9.33 ms (the frame budget is 16 ms).
- The word scan only looks at 500,000 characters on each side of the caret, so beyond about 1 million characters the cost no longer depends on the size of the file; Locate always scans all of it.

**Criterion met.** Completion is not a latency risk; the cache of the scan between the classifier and the completion is not needed.

*Ran in 3.4 s.*

## E17: Do the limits of the completion change its cost?

**Hypothesis.** The limit on the number of suggestions (100) and the minimum word length (3) were picked without measuring. If the cost is in the scan, no value of them is a performance lever and they can be chosen for quality alone; if it is in the dictionary and the sort, they are.

**Method.** The two shapes of E16 at the second size (JavaScript), with the maximum number of suggestions at 10, 100, 1,000 and 10,000 and the minimum word length at 1, 3 and 5, one knob at a time from the default (100, 3). Median of Locate + Suggest, against the faster of two measurements of the default (first and last).

**Criterion.** Every combination is within 25% of the default, in both shapes.

**Median time of Locate + Suggest (JavaScript)**

| Shape | Max suggestions | Min word length | Time | Against the default | Suggestions |
|---|---|---|---|---|---|
| typical code | 100 | 3 | 1.84 ms | 1.00x | 1 |
| typical code | 10 | 3 | 1.83 ms | 1.00x | 1 |
| typical code | 1000 | 3 | 1.83 ms | 1.00x | 1 |
| typical code | 10000 | 3 | 1.83 ms | 1.00x | 1 |
| typical code | 100 | 1 | 1.89 ms | 1.03x | 1 |
| typical code | 100 | 5 | 1.79 ms | 0.98x | 1 |
| typical code | 100 | 3 (default again) | 1.83 ms | 1.00x | 1 |
| distinct words | 100 | 3 | 902 µs | 1.00x | 100 |
| distinct words | 10 | 3 | 888 µs | 0.99x | 10 |
| distinct words | 1000 | 3 | 1.03 ms | 1.14x | 1000 |
| distinct words | 10000 | 3 | 2.91 ms | 3.23x | 10000 |
| distinct words | 100 | 1 | 1.01 ms | 1.12x | 100 |
| distinct words | 100 | 5 | 927 µs | 1.03x | 100 |
| distinct words | 100 | 3 (default again) | 902 µs | 1.00x | 100 |

**Analysis.**

- The largest difference from the default is 3.23x (distinct words, max 10000, min length 3).

**Criterion not met.** The limits matter: the defaults need a justification, or the dictionary needs to be cheaper.

*Ran in 2.7 s.*

## E19: Does the tokenizer agree with the vocabulary?

**Hypothesis.** The keywords offered by the completion and the words the tokenizers color live in two places. The SQL, GraphQL, YAML and shader lists reuse the sets of the tokenizers, but the HTML tags and the CSS properties were written by hand. A word that the completion offers and the tokenizer then splits in two, or colors as plain text, tells the user the plugin does not know what it just offered.

**Method.** For every word of every vocabulary, put it in one or more contexts of its language (`select`: `{0} x`; a CSS property: `.a { {0}: 1 }`...), run the real tokenizer, and check (1) that one token covers exactly the word and (2) that its type is the one expected for the language, or, for SQL and YAML, which only separate keywords from identifiers, that it differs from the type of an unknown word (`zzqx`) in the same context.

**Criterion.** Every word is one token, and at least 95% of the words of each language are classified as expected.

**Vocabulary against the tokenizer**

| Language | Words | One token | Classified as expected | Examples that failed |
|---|---|---|---|---|
| sql | 154 | 154 (100%) | 154 (100%) |  |
| graphql | 28 | 28 (100%) | 28 (100%) |  |
| glsl | 215 | 215 (100%) | 215 (100%) |  |
| wgsl | 162 | 162 (100%) | 162 (100%) |  |
| json | 3 | 3 (100%) | 3 (100%) |  |
| yaml | 17 | 17 (100%) | 17 (100%) |  |
| html | 130 | 130 (100%) | 130 (100%) |  |
| css | 227 | 227 (100%) | 227 (100%) |  |

**Analysis.**

- A word counts as one token if some context gives it a single token that covers it exactly. For CSS the words of both kinds (properties and values) share one list, so each is tried as a property and as a value.

**Criterion met.** Offering a word and coloring it agree.

*Ran in 0.0 s.*

## E20: Order of the words of the document

**Hypothesis.** Listing the words that already exist in the document nearest to the caret first puts the word the user wants among the first five more often than listing them alphabetically, by frequency or by first appearance.

**Method.** 50 generated files (SyntheticCorpus: 4 hosts, SQL / HTML / CSS / GraphQL strings, a pool of 40 names reused as variables, columns, classes and fields). A sample of the words of the embedded strings is typed 1, 2 and 3 characters at a time, with the rest of the word removed, and the position of the right word in each ordering is recorded. Only the cases where the word still exists in the document or in the vocabulary (reachable) count. The same 50 files are generated with 3 settings of locality: how much the generator reuses the names it used last.

**Criterion.** With medium locality (0.5), the nearest-first order has a hit rate within the first 5 at least 5 percentage points above the alphabetical order.

**Locality 0.5: 50 files, 9651 cases, 9189 reachable (95.2%)**

| Order | Right word first | Within the first 5 | Within the first 10 | Mean reciprocal rank |
|---|---|---|---|---|
| keywords, then words nearest first (the engine) | 36.0% | 72.1% | 87.0% | 0.518 |
| keywords, then words alphabetical | 29.8% | 64.0% | 81.8% | 0.453 |
| keywords, then words by frequency | 34.3% | 69.6% | 85.5% | 0.497 |
| keywords, then words by first appearance | 33.8% | 69.8% | 85.5% | 0.496 |
| words nearest first, then keywords | 37.3% | 69.2% | 85.5% | 0.523 |

**Locality 0.0: 50 files, 9651 cases, 9207 reachable (95.4%)**

| Order | Right word first | Within the first 5 | Within the first 10 | Mean reciprocal rank |
|---|---|---|---|---|
| keywords, then words nearest first (the engine) | 33.7% | 68.9% | 84.9% | 0.494 |
| keywords, then words alphabetical | 29.7% | 64.3% | 81.6% | 0.451 |
| keywords, then words by frequency | 34.1% | 69.1% | 84.6% | 0.495 |
| keywords, then words by first appearance | 34.4% | 69.4% | 85.0% | 0.499 |
| words nearest first, then keywords | 32.3% | 64.7% | 84.3% | 0.477 |

**Locality 0.9: 50 files, 9651 cases, 9411 reachable (97.5%)**

| Order | Right word first | Within the first 5 | Within the first 10 | Mean reciprocal rank |
|---|---|---|---|---|
| keywords, then words nearest first (the engine) | 38.3% | 76.3% | 90.1% | 0.545 |
| keywords, then words alphabetical | 29.1% | 65.5% | 82.4% | 0.450 |
| keywords, then words by frequency | 34.2% | 70.7% | 87.0% | 0.501 |
| keywords, then words by first appearance | 33.4% | 69.3% | 85.5% | 0.492 |
| words nearest first, then keywords | 45.9% | 78.4% | 89.5% | 0.604 |

**Analysis.**

- At medium locality, the right word is within the first 5 in 72.1% of the cases with the nearest-first order and 64.0% with the alphabetical order.
- The corpus is generated. The advantage of nearest-first comes from how the generator reuses names (locality), so the table for locality 0.0 is the fairest comparison and the one for 0.9 the most favorable. Real code was not measured.

**Criterion met.** Keep the order by distance.

*Ran in 11.3 s.*

## E21: Where the words come from, and how many keystrokes completion saves

**Hypothesis.** Offering the words of the whole document (the host code included, as Visual Studio Code does) saves more keystrokes than offering only the words that appear inside embedded strings, or only the words of the string being typed. The names of the host code pollute the list less than they help.

**Method.** The corpus of E20 (50 generated files, locality 0.5). A sample of the words of the embedded strings is typed one character at a time, up to 5, with the rest removed. The completion is accepted at the first prefix where the right word is within the first 5 suggestions; the saving is the length of the word minus the characters typed minus one for the accepting key. Four scopes: keywords only, the string being typed, all embedded strings, the whole document (always with the keywords).

**Criterion.** The saving of the whole-document scope is within 2 percentage points of the best narrower scope, or above it.

**Hit rate and saving by scope (3217 words typed, 14237 cases)**

| Scope | Within the first 5 after 1 character | after 2 | after 3 | Characters saved |
|---|---|---|---|---|
| keywords only | 35.6% | 49.7% | 49.7% | 17.8% |
| the string being typed | 39.1% | 60.6% | 62.8% | 32.8% |
| all embedded strings | 42.8% | 79.9% | 90.1% | 58.0% |
| the whole document | 42.1% | 77.2% | 86.7% | 55.3% |

**Analysis.**

- Saving = characters of the words that completion would have typed, over all the characters of those words.
- The corpus is generated, and the words that are typed come from the embedded strings only. That favors the narrower scopes: a word that a user types in a string and that exists only in host code (for example the name of a variable) is never a target here.

**Criterion not met.** Restrict the words to the embedded strings (or to the current one).

*Ran in 8.4 s.*

## E22: Sharing the scan and not creating the words of the completion

**Hypothesis.** E16 found that completion takes more than a frame above ~1 million characters, and that most of it is the scan of the host. In Visual Studio the session of a completion scans the text twice (once to decide whether to open, once to fill the list) after the classifier already did it for the same snapshot, and the word pass created a string and a sort entry for every match. Sharing one scan between the classifier and the completion, and creating a word only when it is offered, brings the worst case under a frame.

**Method.** The files of E16 (4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines). The cost of the start of one completion session, as the editor does it: Locate, Locate again and Suggest, over a new text instance each time. Two cases: nothing shared (a plain scanner, as in E16) and the scan shared (the classifier has already highlighted that text, through a CachingHostScanner). Also Suggest alone.

**Criterion.** The session with the scan shared stays under 16 ms at 60,000 lines, in every host and shape.

**Median time of the start of one completion session**

| Host | Shape | Lines | Characters | Nothing shared | Scan shared | Suggest alone | Suggestions |
|---|---|---|---|---|---|---|---|
| JavaScript | typical code | 1200 | 41771 | 284 µs | 95 µs | 95 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 2.91 ms | 922 µs | 916 µs | 1 |
| JavaScript | typical code | 60000 | 2088011 | 11.10 ms | 1.13 ms | 1.09 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 144 µs | 69 µs | 69 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 1.26 ms | 560 µs | 525 µs | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 5.77 ms | 1.07 ms | 1.07 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 275 µs | 83 µs | 83 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 2.76 ms | 810 µs | 803 µs | 1 |
| CSharp | typical code | 60000 | 1748126 | 11.22 ms | 1.16 ms | 1.13 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 155 µs | 69 µs | 69 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 1.35 ms | 532 µs | 526 µs | 100 |
| CSharp | distinct words | 60000 | 1200032 | 5.74 ms | 1.07 ms | 1.07 ms | 100 |
| Python | typical code | 1200 | 29494 | 388 µs | 72 µs | 71 µs | 1 |
| Python | typical code | 12000 | 294167 | 3.91 ms | 679 µs | 675 µs | 1 |
| Python | typical code | 60000 | 1470512 | 17.46 ms | 1.17 ms | 1.13 ms | 1 |
| Python | distinct words | 1200 | 18026 | 198 µs | 53 µs | 52 µs | 100 |
| Python | distinct words | 12000 | 180026 | 1.78 ms | 393 µs | 355 µs | 100 |
| Python | distinct words | 60000 | 900026 | 8.11 ms | 986 µs | 991 µs | 100 |
| Cpp | typical code | 1200 | 32804 | 237 µs | 79 µs | 78 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 2.30 ms | 751 µs | 747 µs | 1 |
| Cpp | typical code | 60000 | 1629061 | 9.20 ms | 1.17 ms | 1.21 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 169 µs | 70 µs | 70 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 1.49 ms | 534 µs | 526 µs | 100 |
| Cpp | distinct words | 60000 | 1200036 | 5.87 ms | 1.08 ms | 1.08 ms | 100 |

**Analysis.**

- The slowest session with the scan shared at 60000 lines is Python, typical code: 1.17 ms (the frame budget is 16 ms).
- "Nothing shared" is the start of a session as the editor makes it without the shared scan: Locate twice and Suggest. E16 measured one Locate and Suggest.

**Criterion met.** Completion fits in a frame on any file the classifier can handle.

*Ran in 6.9 s.*

## E23: Does the second stage of the completion fit in a frame?

**Hypothesis.** The second stage compares what was typed with every distinct word of the document that passes two cheap filters, and keeps the best few. Even forced to run in every session, with thousands of words that are one edit away, one session still fits in the 16 ms of a frame, as E22 showed for the first stage.

**Method.** E22's files and session (Locate, Locate again and Suggest, over a new text instance, the classifier having highlighted it first), 4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines, with the second stage forced to run (it is asked for below any number of items). Two things typed: the exact prefix `comp` (the second stage compares everything and finds nothing new) and `cmop`, two letters swapped (in the distinct-words file, every one of the words is one edit away). Also the same session with the second stage off.

**Criterion.** The session with the second stage forced stays under 16 ms at 60,000 lines, in every host, shape and typed text.

**Median time of the start of one completion session (the scan shared)**

| Host | Shape | Typed | Lines | Characters | Second stage off | Second stage forced | Allocated off | Allocated forced | Similar items |
|---|---|---|---|---|---|---|---|---|---|
| JavaScript | typical code | comp | 1200 | 41771 | 99 µs | 230 µs | 9 KB | 10 KB | 1 |
| JavaScript | typical code | comp | 12000 | 417611 | 947 µs | 2.17 ms | 65 KB | 66 KB | 1 |
| JavaScript | typical code | comp | 60000 | 2088011 | 1.12 ms | 2.62 ms | 65 KB | 66 KB | 1 |
| JavaScript | typical code | cmop | 1200 | 41771 | 87 µs | 218 µs | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 12000 | 417611 | 836 µs | 2.08 ms | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 60000 | 2088011 | 1.02 ms | 2.50 ms | 1 KB | 2 KB | 1 |
| JavaScript | distinct words | comp | 1200 | 24011 | 69 µs | 69 µs | 67 KB | 67 KB | 0 |
| JavaScript | distinct words | comp | 12000 | 240011 | 518 µs | 524 µs | 291 KB | 291 KB | 0 |
| JavaScript | distinct words | comp | 60000 | 1200011 | 1.11 ms | 1.06 ms | 547 KB | 547 KB | 0 |
| JavaScript | distinct words | cmop | 1200 | 24011 | 49 µs | 405 µs | 1 KB | 168 KB | 10 |
| JavaScript | distinct words | cmop | 12000 | 240011 | 464 µs | 4.26 ms | 1 KB | 1.5 MB | 10 |
| JavaScript | distinct words | cmop | 60000 | 1200011 | 961 µs | 8.65 ms | 1 KB | 3.2 MB | 10 |
| CSharp | typical code | comp | 1200 | 35058 | 89 µs | 189 µs | 9 KB | 10 KB | 1 |
| CSharp | typical code | comp | 12000 | 349703 | 800 µs | 1.80 ms | 65 KB | 66 KB | 1 |
| CSharp | typical code | comp | 60000 | 1748126 | 1.16 ms | 2.58 ms | 65 KB | 66 KB | 1 |
| CSharp | typical code | cmop | 1200 | 35058 | 77 µs | 183 µs | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 12000 | 349703 | 729 µs | 1.71 ms | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 60000 | 1748126 | 1.05 ms | 2.46 ms | 1 KB | 2 KB | 1 |
| CSharp | distinct words | comp | 1200 | 24032 | 69 µs | 70 µs | 67 KB | 67 KB | 0 |
| CSharp | distinct words | comp | 12000 | 240032 | 567 µs | 523 µs | 291 KB | 291 KB | 0 |
| CSharp | distinct words | comp | 60000 | 1200032 | 1.06 ms | 1.18 ms | 547 KB | 547 KB | 0 |
| CSharp | distinct words | cmop | 1200 | 24032 | 49 µs | 397 µs | 1 KB | 168 KB | 10 |
| CSharp | distinct words | cmop | 12000 | 240032 | 461 µs | 4.21 ms | 1 KB | 1.5 MB | 10 |
| CSharp | distinct words | cmop | 60000 | 1200032 | 956 µs | 8.10 ms | 1 KB | 3.2 MB | 10 |
| Python | typical code | comp | 1200 | 29494 | 71 µs | 162 µs | 9 KB | 10 KB | 1 |
| Python | typical code | comp | 12000 | 294167 | 676 µs | 1.52 ms | 65 KB | 66 KB | 1 |
| Python | typical code | comp | 60000 | 1470512 | 1.17 ms | 2.60 ms | 129 KB | 130 KB | 1 |
| Python | typical code | cmop | 1200 | 29494 | 64 µs | 157 µs | 1 KB | 1 KB | 1 |
| Python | typical code | cmop | 12000 | 294167 | 596 µs | 1.44 ms | 1 KB | 1 KB | 1 |
| Python | typical code | cmop | 60000 | 1470512 | 1.04 ms | 2.49 ms | 1 KB | 1 KB | 1 |
| Python | distinct words | comp | 1200 | 18026 | 54 µs | 52 µs | 67 KB | 67 KB | 0 |
| Python | distinct words | comp | 12000 | 180026 | 357 µs | 362 µs | 291 KB | 291 KB | 0 |
| Python | distinct words | comp | 60000 | 900026 | 1.06 ms | 979 µs | 1.0 MB | 1.0 MB | 0 |
| Python | distinct words | cmop | 1200 | 18026 | 32 µs | 417 µs | 1 KB | 168 KB | 10 |
| Python | distinct words | cmop | 12000 | 180026 | 290 µs | 3.68 ms | 1 KB | 1.5 MB | 10 |
| Python | distinct words | cmop | 60000 | 900026 | 808 µs | 10.44 ms | 1 KB | 3.7 MB | 10 |
| Cpp | typical code | comp | 1200 | 32804 | 84 µs | 185 µs | 9 KB | 10 KB | 1 |
| Cpp | typical code | comp | 12000 | 325951 | 749 µs | 1.70 ms | 65 KB | 66 KB | 1 |
| Cpp | typical code | comp | 60000 | 1629061 | 1.18 ms | 2.65 ms | 129 KB | 130 KB | 1 |
| Cpp | typical code | cmop | 1200 | 32804 | 71 µs | 173 µs | 1 KB | 1 KB | 1 |
| Cpp | typical code | cmop | 12000 | 325951 | 667 µs | 1.62 ms | 1 KB | 1 KB | 1 |
| Cpp | typical code | cmop | 60000 | 1629061 | 1.03 ms | 2.49 ms | 1 KB | 1 KB | 1 |
| Cpp | distinct words | comp | 1200 | 24036 | 70 µs | 71 µs | 67 KB | 67 KB | 0 |
| Cpp | distinct words | comp | 12000 | 240036 | 528 µs | 566 µs | 291 KB | 291 KB | 0 |
| Cpp | distinct words | comp | 60000 | 1200036 | 1.20 ms | 1.08 ms | 547 KB | 547 KB | 0 |
| Cpp | distinct words | cmop | 1200 | 24036 | 49 µs | 402 µs | 1 KB | 168 KB | 10 |
| Cpp | distinct words | cmop | 12000 | 240036 | 467 µs | 4.22 ms | 1 KB | 1.5 MB | 10 |
| Cpp | distinct words | cmop | 60000 | 1200036 | 973 µs | 8.31 ms | 1 KB | 3.2 MB | 10 |

**Analysis.**

- The slowest session with the second stage forced at 60000 lines is Python, distinct words, typed cmop: 10.44 ms (the frame budget is 16 ms).
- Allocated is what the session allocates (the same meaning as E05): the difference between forced and off is the cost of the second stage; no string is created for a word that is too far, only a small record to skip its next occurrences.
- In the typical file there are few distinct words and nearly all of them fail the first-letter filter; in the file of distinct words every word is a candidate and, for `cmop`, every one is one edit away.

**Criterion met.** The second stage needs no cache of the distinct words and no index.

*Ran in 12.8 s.*

## E24: Does the second stage recover the word after one mistake, and which tie-break works?

**Hypothesis.** When the typed text has one edit (an extra letter, a missing one, a wrong one, two swapped) in a prefix of 4 to 8 letters, the word the user meant is among the first 5 suggestions in most cases. Among the words that are the same number of edits away, the nearest to the caret is no worse a tie-break than the most frequent.

**Method.** 50 generated files (E20's corpus, locality 0.5). A sample of the words of the embedded strings, 5 or more letters long, is typed as a prefix of 4 to 8 letters with one edit of each kind, at a place picked at random over the whole prefix (the first letter included). Only the cases where the word exists elsewhere in the document, or is a keyword, count (reachable). The second stage is on, with the defaults, and the list is also reordered in three ways inside each group of the same kind and distance: nearest to the caret first (the engine), most frequent first, most frequent and then nearest.

**Criterion.** The meant word is within the first 5 in at least 70% of the reachable cases, with the best of the three tie-breaks. If more than one reaches 70%, the one with the best result enters; if they tie, the nearest to the caret stays (it is the first stage's order).

**One mistake in the prefix: 50 files, 13040 cases, 12244 reachable (93.9%)**

| Tie-break | Meant word first | Within the first 5 | Within the first 10 |
|---|---|---|---|
| nearest to the caret (the engine) | 47.5% | 72.0% | 77.7% |
| most frequent | 44.3% | 70.1% | 77.7% |
| most frequent, then nearest | 44.3% | 70.3% | 77.7% |

**By kind of mistake (the engine's order)**

| Mistake | Cases | Within the first 5 |
|---|---|---|
| deletion | 3061 | 70.6% |
| insertion | 3061 | 77.2% |
| substitution | 3061 | 73.3% |
| transposition | 3061 | 67.0% |

**By length of what was typed (the engine's order)**

| Typed letters | Cases | Within the first 5 |
|---|---|---|
| 3 | 1382 | 67.1% |
| 4 | 3421 | 68.5% |
| 5 | 3102 | 74.0% |
| 6 | 1795 | 72.6% |
| 7 | 1348 | 72.9% |
| 8 | 906 | 80.5% |
| 9 | 290 | 81.4% |

**By place of the mistake (the engine's order)**

| Place | Cases | Within the first 5 |
|---|---|---|
| a later letter | 9578 | 91.9% |
| the first letter | 2666 | 0.8% |

**Analysis.**

- In 657 of the 12244 reachable cases the mistake happened to leave an exact prefix of another word, so the first stage answered and the second did not run; in 9536 the second stage ran and found something.
- A mistake in the first letter cannot be recovered with the first letter required (a design decision); it is in the table by place. The cases not reachable (796) are words that exist nowhere else in the document.
- The corpus is generated and its names repeat by construction; the mistakes are uniform over the prefix, not the way people really mistype.

**Criterion met.** Keep the second stage as designed, with the tie-break that won.

*Ran in 6.5 s.*

## E25: Does the second stage get in the way when the prefix is right?

**Hypothesis.** If the second stage ran whenever the first one found few items, a correct prefix would often get a list full of words that are only similar by chance. Running it only when nothing matched (FuzzyBelow = 1) keeps that noise rare.

**Method.** The corpus of E20 (50 generated files). A sample of the words of the embedded strings is typed correctly, from 3 to 8 letters, with the rest of the word removed. The engine runs with FuzzyBelow = 1, 3 and 5, and the cases where it adds similar items are counted, apart for those where the first stage found something and those where it found nothing.

**Criterion.** With FuzzyBelow = 1, the second stage adds items in at most 5% of the cases. The default is then chosen among the values that meet it.

**Correct prefixes of 3 to 8 letters: 50 files, 12096 cases (129 with no exact match, 11967 with one or more)**

| FuzzyBelow | Cases with similar items added | of which the first stage found nothing | of which it found something | Similar items per case that has them |
|---|---|---|---|---|
| 1 | 1.1% | 128 | 0 | 4.1 |
| 3 | 25.9% | 128 | 3003 | 3.9 |
| 5 | 33.5% | 128 | 3926 | 3.9 |

**Analysis.**

- With FuzzyBelow = 1 the second stage can only run when the first found nothing: 1.1% of the cases here (the words that exist nowhere else in the document). Those are not mistakes of the user, but the list gains similar words in some of them.
- The corpus is generated, so how often a word has no twin in the document is a property of the generator.

**Criterion met.** Keep FuzzyBelow = 1 (the second stage as a fallback).

*Ran in 5.1 s.*

## E27: Completion with similar words on incomplete and cut code

**Hypothesis.** This replaces E18, whose invariant (every suggestion starts with what was typed) no longer holds for similar words. Completion still runs while the code is being typed: for every text and every caret, Suggest must not throw; the site must lie inside the text and cover only word characters; the exact suggestions must start with the typed text; the similar ones must be at the distance they claim (as the definition computes it) between 1 and the tolerance, with the first letter typed, and come after the exact ones; nothing repeats; and the limits hold.

**Method.** 50 generated files (E20's generator, 4 hosts). For each file: every prefix cut at a stride, and every single-character deletion at a stride; in each text, the caret at the end, at the start, and at 5 seeded random positions. In every position inside embedded code, also the same text with a one-letter mistake put in the word under the caret, so that the second stage runs often. The distance of every similar item is recomputed with the whole matrix of the definition.

**Criterion.** Zero violations.

**Checks**

| Check | Value |
|---|---|
| Files | 50 |
| Texts (cuts and deletions) | 12450 |
| Carets tried | 86900 |
| Carets inside embedded code | 16639 |
| Of them, with a mistake put in the word | 9586 |
| Similar items checked against the definition | 21275 |
| Violations | 0 |

**Analysis.**

- 16639 of the 86900 carets were inside the code of an embedded string and went through Suggest; 9586 more with a mistake in the word.

**Criterion met.** Completion can be triggered anywhere in a file being edited, with the second stage on.

*Ran in 1.5 s.*

## Limits

- Synthetic code, not real files.
- The experiments run in a console process with a small heap, away from the Visual Studio UI thread and its GC pressure. They compare versions of the code: they are not times the user will see.
- Times are medians of repeated runs on one machine; differences of a few percent are noise.
