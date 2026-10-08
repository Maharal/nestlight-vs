# NestLight experiments report

| | |
|---|---|
| Date | 2026-10-08 23:08 UTC |
| Commit | `70e9ffd` |
| Runtime | .NET 8.0.31, Release |
| Machine | CPU not identified, 4 logical cores |
| OS | Ubuntu 24.04.5 LTS |
| Mode | full |

Definitions of the experiments: [docs/experiments.md](../experiments.md). Each result is valid only for the commit and the machine above.

## Summary

| Id | Experiment | Criterion | Finding |
|---|---|---|---|
| E22 | Sharing the scan and not creating the words of the completion | Met | worst case with the scan shared 2.97 ms at 60000 lines |
| E23 | Does the second stage of the completion fit in a frame? | Not met | worst case with the second stage forced 16.13 ms at 60000 lines |
| E27 | Completion with similar words on incomplete and cut code | Met | 0 violations in 86900 carets, 21275 similar items checked |

*Criterion met* is the statement of each experiment's own criterion, not a good/bad grade: see *If met* and *If not met* under each experiment.

## E22: Sharing the scan and not creating the words of the completion

**Hypothesis.** E16 found that completion takes more than a frame above ~1 million characters, and that most of it is the scan of the host. In Visual Studio the session of a completion scans the text twice (once to decide whether to open, once to fill the list) after the classifier already did it for the same snapshot, and the word pass created a string and a sort entry for every match. Sharing one scan between the classifier and the completion, and creating a word only when it is offered, brings the worst case under a frame.

**Method.** The files of E16 (4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines). The cost of the start of one completion session, as the editor does it: Locate, Locate again and Suggest, over a new text instance each time. Two cases: nothing shared (a plain scanner, as in E16) and the scan shared (the classifier has already highlighted that text, through a CachingHostScanner). Also Suggest alone.

**Criterion.** The session with the scan shared stays under 16 ms at 60,000 lines, in every host and shape.

**Median time of the start of one completion session**

| Host | Shape | Lines | Characters | Nothing shared | Scan shared | Suggest alone | Suggestions |
|---|---|---|---|---|---|---|---|
| JavaScript | typical code | 1200 | 41771 | 543 µs | 185 µs | 174 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 4.96 ms | 1.79 ms | 1.75 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 19.28 ms | 2.29 ms | 2.17 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 263 µs | 132 µs | 130 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 2.47 ms | 1.12 ms | 1.07 ms | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 9.02 ms | 2.22 ms | 2.21 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 555 µs | 154 µs | 150 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 5.77 ms | 1.61 ms | 1.57 ms | 1 |
| CSharp | typical code | 60000 | 1748126 | 24.10 ms | 2.49 ms | 2.21 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 328 µs | 134 µs | 132 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 3.09 ms | 1.13 ms | 1.07 ms | 100 |
| CSharp | distinct words | 60000 | 1200032 | 11.96 ms | 2.21 ms | 2.21 ms | 100 |
| Python | typical code | 1200 | 29494 | 688 µs | 129 µs | 128 µs | 1 |
| Python | typical code | 12000 | 294167 | 7.28 ms | 1.32 ms | 1.31 ms | 1 |
| Python | typical code | 60000 | 1470512 | 31.10 ms | 2.36 ms | 2.19 ms | 1 |
| Python | distinct words | 1200 | 18026 | 383 µs | 103 µs | 100 µs | 100 |
| Python | distinct words | 12000 | 180026 | 3.46 ms | 854 µs | 802 µs | 100 |
| Python | distinct words | 60000 | 900026 | 15.80 ms | 2.53 ms | 2.23 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 417 µs | 142 µs | 143 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 4.12 ms | 1.46 ms | 2.35 ms | 1 |
| Cpp | typical code | 60000 | 1629061 | 15.57 ms | 2.34 ms | 2.23 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 327 µs | 134 µs | 130 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 3.28 ms | 1.22 ms | 1.16 ms | 100 |
| Cpp | distinct words | 60000 | 1200036 | 11.45 ms | 2.97 ms | 2.21 ms | 100 |

**Analysis.**

- The slowest session with the scan shared at 60000 lines is Cpp, distinct words: 2.97 ms (the frame budget is 16 ms).
- "Nothing shared" is the start of a session as the editor makes it without the shared scan: Locate twice and Suggest. E16 measured one Locate and Suggest.

**Criterion met.** Completion fits in a frame on any file the classifier can handle.

*Ran in 13.4 s.*

## E23: Does the second stage of the completion fit in a frame?

**Hypothesis.** The second stage compares what was typed with every distinct word of the document that passes two cheap filters, and keeps the best few. Even forced to run in every session, with thousands of words that are one edit away, one session still fits in the 16 ms of a frame, as E22 showed for the first stage.

**Method.** E22's files and session (Locate, Locate again and Suggest, over a new text instance, the classifier having highlighted it first), 4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines, with the second stage forced to run (it is asked for below any number of items). Two things typed: the exact prefix `comp` (the second stage compares everything and finds nothing new) and `cmop`, two letters swapped (in the distinct-words file, every one of the words is one edit away). Also the same session with the second stage off.

**Criterion.** The session with the second stage forced stays under 16 ms at 60,000 lines, in every host, shape and typed text.

**Median time of the start of one completion session (the scan shared)**

| Host | Shape | Typed | Lines | Characters | Second stage off | Second stage forced | Allocated off | Allocated forced | Similar items |
|---|---|---|---|---|---|---|---|---|---|
| JavaScript | typical code | comp | 1200 | 41771 | 178 µs | 364 µs | 9 KB | 10 KB | 1 |
| JavaScript | typical code | comp | 12000 | 417611 | 1.81 ms | 3.60 ms | 65 KB | 66 KB | 1 |
| JavaScript | typical code | comp | 60000 | 2088011 | 2.87 ms | 4.58 ms | 65 KB | 66 KB | 1 |
| JavaScript | typical code | cmop | 1200 | 41771 | 192 µs | 359 µs | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 12000 | 417611 | 1.89 ms | 3.48 ms | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 60000 | 2088011 | 2.14 ms | 4.40 ms | 1 KB | 2 KB | 1 |
| JavaScript | distinct words | comp | 1200 | 24011 | 138 µs | 135 µs | 67 KB | 67 KB | 0 |
| JavaScript | distinct words | comp | 12000 | 240011 | 1.10 ms | 1.41 ms | 291 KB | 291 KB | 0 |
| JavaScript | distinct words | comp | 60000 | 1200011 | 2.29 ms | 2.25 ms | 547 KB | 547 KB | 0 |
| JavaScript | distinct words | cmop | 1200 | 24011 | 97 µs | 548 µs | 1 KB | 168 KB | 10 |
| JavaScript | distinct words | cmop | 12000 | 240011 | 955 µs | 5.89 ms | 1 KB | 1.5 MB | 10 |
| JavaScript | distinct words | cmop | 60000 | 1200011 | 2.03 ms | 13.93 ms | 1 KB | 3.2 MB | 10 |
| CSharp | typical code | comp | 1200 | 35058 | 158 µs | 306 µs | 9 KB | 10 KB | 1 |
| CSharp | typical code | comp | 12000 | 349703 | 1.67 ms | 5.35 ms | 65 KB | 66 KB | 1 |
| CSharp | typical code | comp | 60000 | 1748126 | 2.36 ms | 4.51 ms | 65 KB | 66 KB | 1 |
| CSharp | typical code | cmop | 1200 | 35058 | 143 µs | 284 µs | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 12000 | 349703 | 2.33 ms | 4.14 ms | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 60000 | 1748126 | 2.16 ms | 4.18 ms | 1 KB | 2 KB | 1 |
| CSharp | distinct words | comp | 1200 | 24032 | 132 µs | 132 µs | 67 KB | 67 KB | 0 |
| CSharp | distinct words | comp | 12000 | 240032 | 1.09 ms | 1.10 ms | 291 KB | 291 KB | 0 |
| CSharp | distinct words | comp | 60000 | 1200032 | 2.26 ms | 2.47 ms | 547 KB | 547 KB | 0 |
| CSharp | distinct words | cmop | 1200 | 24032 | 102 µs | 909 µs | 1 KB | 168 KB | 10 |
| CSharp | distinct words | cmop | 12000 | 240032 | 1.06 ms | 5.75 ms | 1 KB | 1.5 MB | 10 |
| CSharp | distinct words | cmop | 60000 | 1200032 | 2.02 ms | 12.63 ms | 1 KB | 3.2 MB | 10 |
| Python | typical code | comp | 1200 | 29494 | 130 µs | 253 µs | 9 KB | 10 KB | 1 |
| Python | typical code | comp | 12000 | 294167 | 1.32 ms | 2.56 ms | 65 KB | 66 KB | 1 |
| Python | typical code | comp | 60000 | 1470512 | 2.39 ms | 4.33 ms | 129 KB | 130 KB | 1 |
| Python | typical code | cmop | 1200 | 29494 | 190 µs | 423 µs | 1 KB | 1 KB | 1 |
| Python | typical code | cmop | 12000 | 294167 | 1.21 ms | 2.41 ms | 1 KB | 1 KB | 1 |
| Python | typical code | cmop | 60000 | 1470512 | 2.12 ms | 4.22 ms | 1 KB | 1 KB | 1 |
| Python | distinct words | comp | 1200 | 18026 | 176 µs | 173 µs | 67 KB | 67 KB | 0 |
| Python | distinct words | comp | 12000 | 180026 | 803 µs | 1.26 ms | 291 KB | 291 KB | 0 |
| Python | distinct words | comp | 60000 | 900026 | 2.31 ms | 2.32 ms | 1.0 MB | 1.0 MB | 0 |
| Python | distinct words | cmop | 1200 | 18026 | 66 µs | 516 µs | 1 KB | 168 KB | 10 |
| Python | distinct words | cmop | 12000 | 180026 | 645 µs | 5.67 ms | 1 KB | 1.5 MB | 10 |
| Python | distinct words | cmop | 60000 | 900026 | 1.81 ms | 16.13 ms | 1 KB | 3.7 MB | 10 |
| Cpp | typical code | comp | 1200 | 32804 | 143 µs | 282 µs | 9 KB | 10 KB | 1 |
| Cpp | typical code | comp | 12000 | 325951 | 1.50 ms | 3.05 ms | 65 KB | 66 KB | 1 |
| Cpp | typical code | comp | 60000 | 1629061 | 2.35 ms | 4.39 ms | 129 KB | 130 KB | 1 |
| Cpp | typical code | cmop | 1200 | 32804 | 132 µs | 269 µs | 1 KB | 1 KB | 1 |
| Cpp | typical code | cmop | 12000 | 325951 | 1.32 ms | 2.75 ms | 1 KB | 1 KB | 1 |
| Cpp | typical code | cmop | 60000 | 1629061 | 2.14 ms | 4.39 ms | 1 KB | 1 KB | 1 |
| Cpp | distinct words | comp | 1200 | 24036 | 134 µs | 133 µs | 67 KB | 67 KB | 0 |
| Cpp | distinct words | comp | 12000 | 240036 | 1.09 ms | 1.16 ms | 291 KB | 291 KB | 0 |
| Cpp | distinct words | comp | 60000 | 1200036 | 2.57 ms | 2.54 ms | 547 KB | 547 KB | 0 |
| Cpp | distinct words | cmop | 1200 | 24036 | 98 µs | 546 µs | 1 KB | 168 KB | 10 |
| Cpp | distinct words | cmop | 12000 | 240036 | 1.03 ms | 6.22 ms | 1 KB | 1.5 MB | 10 |
| Cpp | distinct words | cmop | 60000 | 1200036 | 2.03 ms | 12.92 ms | 1 KB | 3.2 MB | 10 |

**Analysis.**

- The slowest session with the second stage forced at 60000 lines is Python, distinct words, typed cmop: 16.13 ms (the frame budget is 16 ms).
- Allocated is what the session allocates (the same meaning as E05): the difference between forced and off is the cost of the second stage; no string is created for a word that is too far, only a small record to skip its next occurrences.
- In the typical file there are few distinct words and nearly all of them fail the first-letter filter; in the file of distinct words every word is a candidate and, for `cmop`, every one is one edit away.

**Criterion not met.** Cache the distinct words of a text (as CachingHostScanner does with the scan), shrink the window, and only then think of an index.

*Ran in 24.6 s.*

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

*Ran in 2.7 s.*

## Limits

- Synthetic code, not real files.
- The experiments run in a console process with a small heap, away from the Visual Studio UI thread and its GC pressure. They compare versions of the code: they are not times the user will see.
- Times are medians of repeated runs on one machine; differences of a few percent are noise.
