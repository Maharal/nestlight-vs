# NestLight experiments report

| | |
|---|---|
| Date | 2026-10-07 21:35 UTC |
| Commit | `1fa1fbe` |
| Runtime | .NET 8.0.31, Release |
| Machine | CPU not identified, 4 logical cores |
| OS | Ubuntu 24.04.5 LTS |
| Mode | full |

Definitions of the experiments: [docs/experiments.md](../experiments.md). Each result is valid only for the commit and the machine above.

## Summary

| Id | Experiment | Criterion | Finding |
|---|---|---|---|
| E16 | Completion latency against the size of the file | Not met | worst case 17.53 ms at 60000 lines |
| E17 | Do the limits of the completion change its cost? | Not met | largest difference 2.69x (distinct words, max 10000, min length 3) |
| E18 | Completion on incomplete and cut code | Met | 0 violations in 86900 carets |
| E19 | Does the tokenizer agree with the vocabulary? | Met | vocabulary and tokenizers agree |
| E20 | Order of the words of the document | Met | nearest 72.1% against alphabetical 64.0% within the first 5 (locality 0.5) |
| E21 | Where the words come from, and how many keystrokes completion saves | Not met | whole document saves 55.3% of the characters; best narrower scope (all embedded strings) 58.0% |
| E22 | Sharing the scan and not creating the words of the completion | Met | worst case with the scan shared 2.79 ms at 60000 lines |

*Criterion met* is the statement of each experiment's own criterion, not a good/bad grade: see *If met* and *If not met* under each experiment.

## E16: Completion latency against the size of the file

**Hypothesis.** Every keystroke in an embedded string scans the host to find the string (Locate) and the whole text for words (Suggest). Those two scans, plus a dictionary of the matching words, are cheap on a normal file but could be noticeable on a large one, mostly when thousands of distinct words share the prefix.

**Method.** For each host, a file of increasing size with the caret at the end of an open `comp` in a marked SQL string. Two shapes: typical code (E01's file, a few distinct words) and a file whose every line declares a new `compNNNNN` identifier (thousands of distinct matches). Time of Locate alone and of Locate + Suggest, without the text copy of the editor (E03).

**Criterion.** Locate + Suggest stay under 16 ms (one frame) at the largest size, in every host and shape.

**Median time of one completion**

| Host | Shape | Lines | Characters | Locate | Locate + Suggest | Suggestions |
|---|---|---|---|---|---|---|
| JavaScript | typical code | 1200 | 41771 | 204 µs | 348 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 2.00 ms | 3.66 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 8.30 ms | 11.07 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 61 µs | 206 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 616 µs | 1.87 ms | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 3.31 ms | 6.00 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 312 µs | 422 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 2.13 ms | 5.79 ms | 1 |
| CSharp | typical code | 60000 | 1748126 | 11.19 ms | 13.57 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 105 µs | 250 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 1.09 ms | 2.33 ms | 100 |
| CSharp | distinct words | 60000 | 1200032 | 5.71 ms | 8.20 ms | 100 |
| Python | typical code | 1200 | 29494 | 284 µs | 419 µs | 1 |
| Python | typical code | 12000 | 294167 | 2.87 ms | 4.26 ms | 1 |
| Python | typical code | 60000 | 1470512 | 14.94 ms | 17.53 ms | 1 |
| Python | distinct words | 1200 | 18026 | 131 µs | 239 µs | 100 |
| Python | distinct words | 12000 | 180026 | 1.32 ms | 2.22 ms | 100 |
| Python | distinct words | 60000 | 900026 | 6.72 ms | 9.23 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 124 µs | 280 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 1.32 ms | 2.85 ms | 1 |
| Cpp | typical code | 60000 | 1629061 | 7.00 ms | 10.95 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 81 µs | 292 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 848 µs | 2.10 ms | 100 |
| Cpp | distinct words | 60000 | 1200036 | 4.39 ms | 6.95 ms | 100 |

**Analysis.**

- The slowest case at 60000 lines is Python, typical code: 17.53 ms (the frame budget is 16 ms).
- The word scan only looks at 500,000 characters on each side of the caret, so beyond about 1 million characters the cost no longer depends on the size of the file; Locate always scans all of it.

**Criterion not met.** Reuse the scan of the classifier, lower the window of the word scan, or index the words incrementally.

*Ran in 7.2 s.*

## E17: Do the limits of the completion change its cost?

**Hypothesis.** The limit on the number of suggestions (100) and the minimum word length (3) were picked without measuring. If the cost is in the scan, no value of them is a performance lever and they can be chosen for quality alone; if it is in the dictionary and the sort, they are.

**Method.** The two shapes of E16 at the second size (JavaScript), with the maximum number of suggestions at 10, 100, 1,000 and 10,000 and the minimum word length at 1, 3 and 5, one knob at a time from the default (100, 3). Median of Locate + Suggest, against the faster of two measurements of the default (first and last).

**Criterion.** Every combination is within 25% of the default, in both shapes.

**Median time of Locate + Suggest (JavaScript)**

| Shape | Max suggestions | Min word length | Time | Against the default | Suggestions |
|---|---|---|---|---|---|
| typical code | 100 | 3 | 3.54 ms | 1.00x | 1 |
| typical code | 10 | 3 | 3.69 ms | 1.04x | 1 |
| typical code | 1000 | 3 | 3.59 ms | 1.01x | 1 |
| typical code | 10000 | 3 | 3.58 ms | 1.01x | 1 |
| typical code | 100 | 1 | 3.67 ms | 1.04x | 1 |
| typical code | 100 | 5 | 3.53 ms | 1.00x | 1 |
| typical code | 100 | 3 (default again) | 3.58 ms | 1.01x | 1 |
| distinct words | 100 | 3 | 1.97 ms | 1.00x | 100 |
| distinct words | 10 | 3 | 1.91 ms | 0.97x | 10 |
| distinct words | 1000 | 3 | 2.16 ms | 1.10x | 1000 |
| distinct words | 10000 | 3 | 5.30 ms | 2.69x | 10000 |
| distinct words | 100 | 1 | 1.94 ms | 0.99x | 100 |
| distinct words | 100 | 5 | 1.80 ms | 0.92x | 100 |
| distinct words | 100 | 3 (default again) | 1.97 ms | 1.00x | 100 |

**Analysis.**

- The largest difference from the default is 2.69x (distinct words, max 10000, min length 3).

**Criterion not met.** The limits matter: the defaults need a justification, or the dictionary needs to be cheaper.

*Ran in 5.9 s.*

## E18: Completion on incomplete and cut code

**Hypothesis.** Completion runs while the code is being typed, so it sees unterminated strings, half-written interpolations and carets anywhere. For every text and every caret, Locate and Suggest must not throw, the site must lie inside the text and cover only word characters, and the suggestions must start with what was typed, without repeats.

**Method.** 50 generated files (E20's generator, 4 hosts). For each file: every prefix cut at a stride, and every single-character deletion at a stride; in each text, the caret at the end, at the start, and at 5 seeded random positions. 6 invariants checked on every result.

**Criterion.** Zero violations.

**Checks**

| Check | Value |
|---|---|
| Files | 50 |
| Texts (cuts and deletions) | 12450 |
| Carets tried | 86900 |
| Carets inside embedded code | 16374 |
| Violations | 0 |

**Analysis.**

- 16374 of the 86900 carets were inside the code of an embedded string and went through Suggest.

**Criterion met.** Completion can be triggered anywhere in a file being edited.

*Ran in 1.4 s.*

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

*Ran in 21.4 s.*

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

*Ran in 16.4 s.*

## E22: Sharing the scan and not creating the words of the completion

**Hypothesis.** E16 found that completion takes more than a frame above ~1 million characters, and that most of it is the scan of the host. In Visual Studio the session of a completion scans the text twice (once to decide whether to open, once to fill the list) after the classifier already did it for the same snapshot, and the word pass created a string and a sort entry for every match. Sharing one scan between the classifier and the completion, and creating a word only when it is offered, brings the worst case under a frame.

**Method.** The files of E16 (4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines). The cost of the start of one completion session, as the editor does it: Locate, Locate again and Suggest, over a new text instance each time. Two cases: nothing shared (a plain scanner, as in E16) and the scan shared (the classifier has already highlighted that text, through a CachingHostScanner). Also Suggest alone.

**Criterion.** The session with the scan shared stays under 16 ms at 60,000 lines, in every host and shape.

**Median time of the start of one completion session**

| Host | Shape | Lines | Characters | Nothing shared | Scan shared | Suggest alone | Suggestions |
|---|---|---|---|---|---|---|---|
| JavaScript | typical code | 1200 | 41771 | 844 µs | 287 µs | 194 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 5.26 ms | 2.02 ms | 1.98 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 19.43 ms | 2.59 ms | 2.54 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 287 µs | 149 µs | 141 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 2.55 ms | 1.25 ms | 1.17 ms | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 9.31 ms | 2.45 ms | 2.41 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 591 µs | 163 µs | 162 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 6.06 ms | 1.65 ms | 1.64 ms | 1 |
| CSharp | typical code | 60000 | 1748126 | 26.74 ms | 2.61 ms | 2.41 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 395 µs | 142 µs | 141 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 3.47 ms | 1.31 ms | 1.18 ms | 100 |
| CSharp | distinct words | 60000 | 1200032 | 16.42 ms | 2.45 ms | 2.39 ms | 100 |
| Python | typical code | 1200 | 29494 | 706 µs | 138 µs | 135 µs | 1 |
| Python | typical code | 12000 | 294167 | 7.08 ms | 1.40 ms | 1.36 ms | 1 |
| Python | typical code | 60000 | 1470512 | 35.41 ms | 2.53 ms | 2.41 ms | 1 |
| Python | distinct words | 1200 | 18026 | 563 µs | 109 µs | 105 µs | 100 |
| Python | distinct words | 12000 | 180026 | 3.61 ms | 912 µs | 980 µs | 100 |
| Python | distinct words | 60000 | 900026 | 17.08 ms | 2.79 ms | 2.33 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 429 µs | 153 µs | 150 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 4.46 ms | 1.58 ms | 1.52 ms | 1 |
| Cpp | typical code | 60000 | 1629061 | 16.22 ms | 2.53 ms | 2.41 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 318 µs | 144 µs | 138 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 2.97 ms | 1.22 ms | 1.19 ms | 100 |
| Cpp | distinct words | 60000 | 1200036 | 11.54 ms | 2.60 ms | 2.46 ms | 100 |

**Analysis.**

- The slowest session with the scan shared at 60000 lines is Python, distinct words: 2.79 ms (the frame budget is 16 ms).
- "Nothing shared" is the start of a session as the editor makes it without the shared scan: Locate twice and Suggest. E16 measured one Locate and Suggest.

**Criterion met.** Completion fits in a frame on any file the classifier can handle.

*Ran in 15.4 s.*

## Limits

- Synthetic code, not real files.
- The experiments run in a console process with a small heap, away from the Visual Studio UI thread and its GC pressure. They compare versions of the code: they are not times the user will see.
- Times are medians of repeated runs on one machine; differences of a few percent are noise.
