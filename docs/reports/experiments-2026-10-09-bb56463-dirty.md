# NestLight experiments report

| | |
|---|---|
| Date | 2026-10-09 00:44 UTC |
| Commit | `bb56463` (**uncommitted changes**: this run cannot be reproduced from the commit) |
| Runtime | .NET 10.0.0, Release |
| Machine | CPU not identified, 16 logical cores |
| OS | Linux Mint 22 |
| Mode | full |

Definitions of the experiments: [docs/experiments.md](../experiments.md). Each result is valid only for the commit and the machine above.

## Summary

| Id | Experiment | Criterion | Finding |
|---|---|---|---|
| E22 | Sharing the scan and not creating the words of the completion | Met | worst case with the scan shared 1.60 ms at 60000 lines |

*Criterion met* is the statement of each experiment's own criterion, not a good/bad grade: see *If met* and *If not met* under each experiment.

## E22: Sharing the scan and not creating the words of the completion

**Hypothesis.** E16 found that completion takes more than a frame above ~1 million characters, and that most of it is the scan of the host. In Visual Studio the session of a completion scans the text twice (once to decide whether to open, once to fill the list) after the classifier already did it for the same snapshot, and the word pass created a string and a sort entry for every match. Sharing one scan between the classifier and the completion, and creating a word only when it is offered, brings the worst case under a frame.

**Method.** The files of E16 (4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines). The cost of the start of one completion session, as the editor does it: Locate, Locate again and Suggest, over a new text instance each time. Two cases: nothing shared (a plain scanner, as in E16) and the scan shared (the classifier has already highlighted that text, through a CachingHostScanner). Also Suggest alone.

**Criterion.** The session with the scan shared stays under 16 ms at 60,000 lines, in every host and shape.

**Median time of the start of one completion session**

| Host | Shape | Lines | Characters | Nothing shared | Scan shared | Suggest alone | Suggestions |
|---|---|---|---|---|---|---|---|
| JavaScript | typical code | 1200 | 41771 | 358 µs | 134 µs | 132 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 3.32 ms | 1.25 ms | 1.27 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 10.95 ms | 1.51 ms | 1.49 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 147 µs | 87 µs | 85 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 1.64 ms | 734 µs | 703 µs | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 5.08 ms | 1.41 ms | 1.41 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 286 µs | 111 µs | 110 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 2.95 ms | 1.04 ms | 1.45 ms | 1 |
| CSharp | typical code | 60000 | 1748126 | 11.09 ms | 1.50 ms | 1.52 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 171 µs | 117 µs | 112 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 1.53 ms | 711 µs | 714 µs | 100 |
| CSharp | distinct words | 60000 | 1200032 | 6.07 ms | 1.40 ms | 1.49 ms | 100 |
| Python | typical code | 1200 | 29494 | 415 µs | 94 µs | 94 µs | 1 |
| Python | typical code | 12000 | 294167 | 4.17 ms | 890 µs | 885 µs | 1 |
| Python | typical code | 60000 | 1470512 | 18.15 ms | 1.54 ms | 1.58 ms | 1 |
| Python | distinct words | 1200 | 18026 | 212 µs | 69 µs | 86 µs | 100 |
| Python | distinct words | 12000 | 180026 | 1.94 ms | 526 µs | 489 µs | 100 |
| Python | distinct words | 60000 | 900026 | 8.55 ms | 1.60 ms | 1.35 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 266 µs | 109 µs | 108 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 2.60 ms | 993 µs | 1.00 ms | 1 |
| Cpp | typical code | 60000 | 1629061 | 9.99 ms | 1.56 ms | 1.54 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 184 µs | 89 µs | 90 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 1.67 ms | 732 µs | 695 µs | 100 |
| Cpp | distinct words | 60000 | 1200036 | 6.34 ms | 1.42 ms | 1.45 ms | 100 |

**Analysis.**

- The slowest session with the scan shared at 60000 lines is Python, distinct words: 1.60 ms (the frame budget is 16 ms).
- "Nothing shared" is the start of a session as the editor makes it without the shared scan: Locate twice and Suggest. E16 measured one Locate and Suggest.

**Criterion met.** Completion fits in a frame on any file the classifier can handle.

*Ran in 7.5 s.*

## Limits

- Synthetic code, not real files.
- The experiments run in a console process with a small heap, away from the Visual Studio UI thread and its GC pressure. They compare versions of the code: they are not times the user will see.
- Times are medians of repeated runs on one machine; differences of a few percent are noise.
