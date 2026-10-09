# NestLight experiments report

| | |
|---|---|
| Date | 2026-10-08 23:55 UTC |
| Commit | `3108820` (**uncommitted changes**: this run cannot be reproduced from the commit) |
| Runtime | .NET 10.0.0, Release |
| Machine | CPU not identified, 16 logical cores |
| OS | Linux Mint 22 |
| Mode | full |

Definitions of the experiments: [docs/experiments.md](../experiments.md). Each result is valid only for the commit and the machine above.

## Summary

| Id | Experiment | Criterion | Finding |
|---|---|---|---|
| E16 | Completion latency against the size of the file | Met | worst case 9.77 ms at 60000 lines |
| E17 | Do the limits of the completion change its cost? | Not met | largest difference 3.06x (distinct words, max 10000, min length 3) |
| E19 | Does the tokenizer agree with the vocabulary? | Met | vocabulary and tokenizers agree |
| E20 | Order of the words of the document | Met | nearest 72.1% against alphabetical 64.0% within the first 5 (locality 0.5) |
| E21 | Where the words come from, and how many keystrokes completion saves | Not met | whole document saves 55.3% of the characters; best narrower scope (all embedded strings) 58.0% |
| E22 | Sharing the scan and not creating the words of the completion | Met | worst case with the scan shared 1.37 ms at 60000 lines |
| E23 | Does the second stage of the completion fit in a frame? | Met | worst case with the second stage forced 10.48 ms at 60000 lines |
| E24 | Does the second stage recover the word after one mistake, and which tie-break works? | Met | best tie-break (nearest to the caret (the engine)) 72.0% within the first 5 |
| E25 | Does the second stage get in the way when the prefix is right? | Met | with FuzzyBelow = 1 similar items are added in 1.1% of the cases |
| E27 | Completion with similar words on incomplete and cut code | Met | 0 violations in 86900 carets, 21275 similar items checked |
| E28 | Does the word before the caret help to rank the suggestions? | Met | within the first 5: 72.5% to 85.5% (+13.0 points); worst session 3.68 ms |

*Criterion met* is the statement of each experiment's own criterion, not a good/bad grade: see *If met* and *If not met* under each experiment.

## E16: Completion latency against the size of the file

**Hypothesis.** Every keystroke in an embedded string scans the host to find the string (Locate) and the whole text for words (Suggest). Those two scans, plus a dictionary of the matching words, are cheap on a normal file but could be noticeable on a large one, mostly when thousands of distinct words share the prefix.

**Method.** For each host, a file of increasing size with the caret at the end of an open `comp` in a marked SQL string. Two shapes: typical code (E01's file, a few distinct words) and a file whose every line declares a new `compNNNNN` identifier (thousands of distinct matches). Time of Locate alone and of Locate + Suggest, without the text copy of the editor (E03).

**Criterion.** Locate + Suggest stay under 16 ms (one frame) at the largest size, in every host and shape.

**Median time of one completion**

| Host | Shape | Lines | Characters | Locate | Locate + Suggest | Suggestions |
|---|---|---|---|---|---|---|
| JavaScript | typical code | 1200 | 41771 | 118 µs | 198 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 1.09 ms | 2.18 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 4.60 ms | 6.07 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 31 µs | 109 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 292 µs | 947 µs | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 1.46 ms | 2.85 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 94 µs | 190 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 927 µs | 1.85 ms | 1 |
| CSharp | typical code | 60000 | 1748126 | 5.10 ms | 6.40 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 43 µs | 119 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 418 µs | 1.06 ms | 100 |
| CSharp | distinct words | 60000 | 1200032 | 2.39 ms | 3.75 ms | 100 |
| Python | typical code | 1200 | 29494 | 165 µs | 244 µs | 1 |
| Python | typical code | 12000 | 294167 | 1.60 ms | 2.38 ms | 1 |
| Python | typical code | 60000 | 1470512 | 8.41 ms | 9.77 ms | 1 |
| Python | distinct words | 1200 | 18026 | 72 µs | 132 µs | 100 |
| Python | distinct words | 12000 | 180026 | 712 µs | 1.16 ms | 100 |
| Python | distinct words | 60000 | 900026 | 3.53 ms | 4.77 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 81 µs | 172 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 791 µs | 1.66 ms | 1 |
| Cpp | typical code | 60000 | 1629061 | 4.11 ms | 5.48 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 48 µs | 127 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 475 µs | 1.12 ms | 100 |
| Cpp | distinct words | 60000 | 1200036 | 2.37 ms | 3.73 ms | 100 |

**Analysis.**

- The slowest case at 60000 lines is Python, typical code: 9.77 ms (the frame budget is 16 ms).
- The word scan only looks at 500,000 characters on each side of the caret, so beyond about 1 million characters the cost no longer depends on the size of the file; Locate always scans all of it.

**Criterion met.** Completion is not a latency risk; the cache of the scan between the classifier and the completion is not needed.

*Ran in 3.6 s.*

## E17: Do the limits of the completion change its cost?

**Hypothesis.** The limit on the number of suggestions (100) and the minimum word length (3) were picked without measuring. If the cost is in the scan, no value of them is a performance lever and they can be chosen for quality alone; if it is in the dictionary and the sort, they are.

**Method.** The two shapes of E16 at the second size (JavaScript), with the maximum number of suggestions at 10, 100, 1,000 and 10,000 and the minimum word length at 1, 3 and 5, one knob at a time from the default (100, 3). Median of Locate + Suggest, against the faster of two measurements of the default (first and last).

**Criterion.** Every combination is within 25% of the default, in both shapes.

**Median time of Locate + Suggest (JavaScript)**

| Shape | Max suggestions | Min word length | Time | Against the default | Suggestions |
|---|---|---|---|---|---|
| typical code | 100 | 3 | 1.95 ms | 1.01x | 1 |
| typical code | 10 | 3 | 1.91 ms | 0.99x | 1 |
| typical code | 1000 | 3 | 1.91 ms | 0.99x | 1 |
| typical code | 10000 | 3 | 1.96 ms | 1.02x | 1 |
| typical code | 100 | 1 | 2.05 ms | 1.07x | 1 |
| typical code | 100 | 5 | 1.87 ms | 0.97x | 1 |
| typical code | 100 | 3 (default again) | 1.92 ms | 1.00x | 1 |
| distinct words | 100 | 3 | 937 µs | 1.00x | 100 |
| distinct words | 10 | 3 | 929 µs | 0.99x | 10 |
| distinct words | 1000 | 3 | 1.08 ms | 1.15x | 1000 |
| distinct words | 10000 | 3 | 2.87 ms | 3.06x | 10000 |
| distinct words | 100 | 1 | 940 µs | 1.00x | 100 |
| distinct words | 100 | 5 | 863 µs | 0.92x | 100 |
| distinct words | 100 | 3 (default again) | 940 µs | 1.00x | 100 |

**Analysis.**

- The largest difference from the default is 3.06x (distinct words, max 10000, min length 3).

**Criterion not met.** The limits matter: the defaults need a justification, or the dictionary needs to be cheaper.

*Ran in 2.8 s.*

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

*Ran in 11.4 s.*

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

*Ran in 8.5 s.*

## E22: Sharing the scan and not creating the words of the completion

**Hypothesis.** E16 found that completion takes more than a frame above ~1 million characters, and that most of it is the scan of the host. In Visual Studio the session of a completion scans the text twice (once to decide whether to open, once to fill the list) after the classifier already did it for the same snapshot, and the word pass created a string and a sort entry for every match. Sharing one scan between the classifier and the completion, and creating a word only when it is offered, brings the worst case under a frame.

**Method.** The files of E16 (4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines). The cost of the start of one completion session, as the editor does it: Locate, Locate again and Suggest, over a new text instance each time. Two cases: nothing shared (a plain scanner, as in E16) and the scan shared (the classifier has already highlighted that text, through a CachingHostScanner). Also Suggest alone.

**Criterion.** The session with the scan shared stays under 16 ms at 60,000 lines, in every host and shape.

**Median time of the start of one completion session**

| Host | Shape | Lines | Characters | Nothing shared | Scan shared | Suggest alone | Suggestions |
|---|---|---|---|---|---|---|---|
| JavaScript | typical code | 1200 | 41771 | 280 µs | 117 µs | 113 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 2.94 ms | 1.09 ms | 1.08 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 10.52 ms | 1.32 ms | 1.28 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 170 µs | 78 µs | 78 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 1.22 ms | 637 µs | 609 µs | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 4.31 ms | 1.25 ms | 1.25 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 290 µs | 96 µs | 95 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 2.89 ms | 935 µs | 918 µs | 1 |
| CSharp | typical code | 60000 | 1748126 | 11.37 ms | 1.33 ms | 1.30 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 164 µs | 78 µs | 78 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 1.44 ms | 653 µs | 616 µs | 100 |
| CSharp | distinct words | 60000 | 1200032 | 6.03 ms | 1.25 ms | 1.26 ms | 100 |
| Python | typical code | 1200 | 29494 | 415 µs | 82 µs | 81 µs | 1 |
| Python | typical code | 12000 | 294167 | 4.11 ms | 787 µs | 784 µs | 1 |
| Python | typical code | 60000 | 1470512 | 18.26 ms | 1.37 ms | 1.39 ms | 1 |
| Python | distinct words | 1200 | 18026 | 201 µs | 60 µs | 65 µs | 100 |
| Python | distinct words | 12000 | 180026 | 1.84 ms | 458 µs | 413 µs | 100 |
| Python | distinct words | 60000 | 900026 | 8.26 ms | 1.37 ms | 1.17 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 254 µs | 91 µs | 90 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 2.47 ms | 875 µs | 876 µs | 1 |
| Cpp | typical code | 60000 | 1629061 | 9.50 ms | 1.36 ms | 1.38 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 176 µs | 111 µs | 78 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 1.57 ms | 649 µs | 610 µs | 100 |
| Cpp | distinct words | 60000 | 1200036 | 6.05 ms | 1.25 ms | 1.25 ms | 100 |

**Analysis.**

- The slowest session with the scan shared at 60000 lines is Python, distinct words: 1.37 ms (the frame budget is 16 ms).
- "Nothing shared" is the start of a session as the editor makes it without the shared scan: Locate twice and Suggest. E16 measured one Locate and Suggest.

**Criterion met.** Completion fits in a frame on any file the classifier can handle.

*Ran in 7.3 s.*

## E23: Does the second stage of the completion fit in a frame?

**Hypothesis.** The second stage compares what was typed with every distinct word of the document that passes two cheap filters, and keeps the best few. Even forced to run in every session, with thousands of words that are one edit away, one session still fits in the 16 ms of a frame, as E22 showed for the first stage.

**Method.** E22's files and session (Locate, Locate again and Suggest, over a new text instance, the classifier having highlighted it first), 4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines, with the second stage forced to run (it is asked for below any number of items). Two things typed: the exact prefix `comp` (the second stage compares everything and finds nothing new) and `cmop`, two letters swapped (in the distinct-words file, every one of the words is one edit away). Also the same session with the second stage off.

**Criterion.** The session with the second stage forced stays under 16 ms at 60,000 lines, in every host, shape and typed text.

**Median time of the start of one completion session (the scan shared)**

| Host | Shape | Typed | Lines | Characters | Second stage off | Second stage forced | Allocated off | Allocated forced | Similar items |
|---|---|---|---|---|---|---|---|---|---|
| JavaScript | typical code | comp | 1200 | 41771 | 116 µs | 241 µs | 9 KB | 10 KB | 1 |
| JavaScript | typical code | comp | 12000 | 417611 | 1.08 ms | 2.32 ms | 65 KB | 66 KB | 1 |
| JavaScript | typical code | comp | 60000 | 2088011 | 1.31 ms | 2.81 ms | 65 KB | 66 KB | 1 |
| JavaScript | typical code | cmop | 1200 | 41771 | 109 µs | 235 µs | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 12000 | 417611 | 985 µs | 2.23 ms | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 60000 | 2088011 | 1.20 ms | 2.70 ms | 1 KB | 2 KB | 1 |
| JavaScript | distinct words | comp | 1200 | 24011 | 78 µs | 78 µs | 67 KB | 67 KB | 0 |
| JavaScript | distinct words | comp | 12000 | 240011 | 614 µs | 611 µs | 291 KB | 291 KB | 0 |
| JavaScript | distinct words | comp | 60000 | 1200011 | 1.24 ms | 1.24 ms | 547 KB | 547 KB | 0 |
| JavaScript | distinct words | cmop | 1200 | 24011 | 58 µs | 402 µs | 1 KB | 168 KB | 10 |
| JavaScript | distinct words | cmop | 12000 | 240011 | 551 µs | 4.20 ms | 1 KB | 1.5 MB | 10 |
| JavaScript | distinct words | cmop | 60000 | 1200011 | 1.14 ms | 8.63 ms | 1 KB | 3.2 MB | 10 |
| CSharp | typical code | comp | 1200 | 35058 | 109 µs | 201 µs | 9 KB | 10 KB | 1 |
| CSharp | typical code | comp | 12000 | 349703 | 923 µs | 1.91 ms | 65 KB | 66 KB | 1 |
| CSharp | typical code | comp | 60000 | 1748126 | 1.33 ms | 2.75 ms | 65 KB | 66 KB | 1 |
| CSharp | typical code | cmop | 1200 | 35058 | 89 µs | 195 µs | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 12000 | 349703 | 852 µs | 1.84 ms | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 60000 | 1748126 | 1.23 ms | 2.65 ms | 1 KB | 2 KB | 1 |
| CSharp | distinct words | comp | 1200 | 24032 | 79 µs | 78 µs | 67 KB | 67 KB | 0 |
| CSharp | distinct words | comp | 12000 | 240032 | 613 µs | 611 µs | 291 KB | 291 KB | 0 |
| CSharp | distinct words | comp | 60000 | 1200032 | 1.26 ms | 1.37 ms | 547 KB | 547 KB | 0 |
| CSharp | distinct words | cmop | 1200 | 24032 | 58 µs | 449 µs | 1 KB | 168 KB | 10 |
| CSharp | distinct words | cmop | 12000 | 240032 | 547 µs | 4.22 ms | 1 KB | 1.5 MB | 10 |
| CSharp | distinct words | cmop | 60000 | 1200032 | 1.14 ms | 8.66 ms | 1 KB | 3.2 MB | 10 |
| Python | typical code | comp | 1200 | 29494 | 85 µs | 179 µs | 9 KB | 10 KB | 1 |
| Python | typical code | comp | 12000 | 294167 | 787 µs | 1.64 ms | 65 KB | 66 KB | 1 |
| Python | typical code | comp | 60000 | 1470512 | 1.36 ms | 2.80 ms | 129 KB | 130 KB | 1 |
| Python | typical code | cmop | 1200 | 29494 | 76 µs | 167 µs | 1 KB | 1 KB | 1 |
| Python | typical code | cmop | 12000 | 294167 | 715 µs | 1.57 ms | 1 KB | 1 KB | 1 |
| Python | typical code | cmop | 60000 | 1470512 | 1.22 ms | 2.66 ms | 1 KB | 1 KB | 1 |
| Python | distinct words | comp | 1200 | 18026 | 59 µs | 59 µs | 67 KB | 67 KB | 0 |
| Python | distinct words | comp | 12000 | 180026 | 409 µs | 413 µs | 291 KB | 291 KB | 0 |
| Python | distinct words | comp | 60000 | 900026 | 1.16 ms | 1.34 ms | 1.0 MB | 1.0 MB | 0 |
| Python | distinct words | cmop | 1200 | 18026 | 38 µs | 362 µs | 1 KB | 168 KB | 10 |
| Python | distinct words | cmop | 12000 | 180026 | 346 µs | 3.51 ms | 1 KB | 1.5 MB | 10 |
| Python | distinct words | cmop | 60000 | 900026 | 1.02 ms | 10.48 ms | 1 KB | 3.7 MB | 10 |
| Cpp | typical code | comp | 1200 | 32804 | 96 µs | 198 µs | 9 KB | 10 KB | 1 |
| Cpp | typical code | comp | 12000 | 325951 | 875 µs | 1.83 ms | 65 KB | 66 KB | 1 |
| Cpp | typical code | comp | 60000 | 1629061 | 1.36 ms | 2.82 ms | 129 KB | 130 KB | 1 |
| Cpp | typical code | cmop | 1200 | 32804 | 81 µs | 186 µs | 1 KB | 2 KB | 1 |
| Cpp | typical code | cmop | 12000 | 325951 | 794 µs | 1.76 ms | 1 KB | 2 KB | 1 |
| Cpp | typical code | cmop | 60000 | 1629061 | 1.23 ms | 2.69 ms | 1 KB | 2 KB | 1 |
| Cpp | distinct words | comp | 1200 | 24036 | 113 µs | 79 µs | 67 KB | 67 KB | 0 |
| Cpp | distinct words | comp | 12000 | 240036 | 613 µs | 615 µs | 291 KB | 291 KB | 0 |
| Cpp | distinct words | comp | 60000 | 1200036 | 1.25 ms | 1.26 ms | 547 KB | 547 KB | 0 |
| Cpp | distinct words | cmop | 1200 | 24036 | 58 µs | 404 µs | 1 KB | 168 KB | 10 |
| Cpp | distinct words | cmop | 12000 | 240036 | 549 µs | 4.18 ms | 1 KB | 1.5 MB | 10 |
| Cpp | distinct words | cmop | 60000 | 1200036 | 1.14 ms | 8.14 ms | 1 KB | 3.2 MB | 10 |

**Analysis.**

- The slowest session with the second stage forced at 60000 lines is Python, distinct words, typed cmop: 10.48 ms (the frame budget is 16 ms).
- Allocated is what the session allocates (the same meaning as E05): the difference between forced and off is the cost of the second stage; no string is created for a word that is too far, only a small record to skip its next occurrences.
- In the typical file there are few distinct words and nearly all of them fail the first-letter filter; in the file of distinct words every word is a candidate and, for `cmop`, every one is one edit away.

**Criterion met.** The second stage needs no cache of the distinct words and no index.

*Ran in 13.2 s.*

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

*Ran in 5.4 s.*

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

## E28: Does the word before the caret help to rank the suggestions?

**Hypothesis.** The words that already followed the same word (with the same punctuation) elsewhere in the document are the likely ones: after `from ` the word that followed `from` before, after `display: ` the value that followed `display:`. Putting them first puts the word the user means among the first 5 more often than the order by distance alone, and one more pass over the window still fits in a frame.

**Method.** 50 generated files with structure (SQL over a schema with a CREATE TABLE for each table, CSS whose values belong to the property, HTML whose attributes belong to the tag; locality 0.5). A sample of the words of the embedded strings, 4 or more letters long, is typed as a prefix of 1, 2 and 3 letters, with the rest of the word removed. Only the cases where the word exists elsewhere in the document or is a keyword count (reachable). The list is the one the editor gets (100 items, the second stage on). Two variants: the order by distance alone, and the previous word first. Also the start of a session (Locate twice and Suggest, the scan shared) on files of 1,200 to 60,000 lines.

**Criterion.** Over all the reachable cases the word is within the first 5 in at least 3 percentage points more cases with the previous word than without; in no language it falls by more than 1 point; and the session with the previous word stays under 16 ms at 60,000 lines, in every host.

**The word typed with 1 to 3 letters: 9564 typed prefixes, 9162 reachable (95.8%)**

| Variant | Word first | Within the first 5 | Within the first 10 | Mean reciprocal rank |
|---|---|---|---|---|
| By distance alone | 36.8% | 72.5% | 86.0% | 0.528 |
| Previous word first | 68.5% | 85.5% | 92.6% | 0.764 |

**Within the first 5, by language of the string**

| Language | Cases | By distance alone | Previous word first |
|---|---|---|---|
| css | 732 | 74.3% | 80.7% |
| graphql | 657 | 88.7% | 90.1% |
| html | 1125 | 61.1% | 70.7% |
| sql | 6648 | 72.7% | 88.1% |

**Within the first 5, by number of letters typed**

| Letters | Cases | By distance alone | Previous word first |
|---|---|---|---|
| 1 | 3054 | 39.1% | 67.5% |
| 2 | 3054 | 83.4% | 92.0% |
| 3 | 3054 | 95.0% | 97.0% |

**Within the first 5, by the word before the one typed (the most frequent)**

| Before | Cases | By distance alone | Previous word first |
|---|---|---|---|
| `text,` | 555 | 60.7% | 88.8% |
| `o.` | 297 | 63.0% | 91.9% |
| `from` | 264 | 39.4% | 86.4% |
| `t.` | 234 | 51.3% | 91.0% |
| `into` | 180 | 37.8% | 82.2% |
| `key,` | 165 | 62.4% | 78.8% |
| `select` | 165 | 64.8% | 83.6% |
| `update` | 159 | 42.1% | 81.8% |

**Median time of the start of one completion session (typed `comp` in an SQL string, structured files)**

| Host | Lines | Characters | By distance alone | Previous word first |
|---|---|---|---|---|
| JavaScript | 1200 | 58760 | 400 µs | 400 µs |
| JavaScript | 12000 | 577766 | 3.18 ms | 3.18 ms |
| JavaScript | 60000 | 2875581 | 3.60 ms | 3.59 ms |
| CSharp | 1200 | 68234 | 433 µs | 436 µs |
| CSharp | 12000 | 670911 | 3.02 ms | 3.03 ms |
| CSharp | 60000 | 3339574 | 3.43 ms | 3.43 ms |
| Python | 1200 | 59973 | 416 µs | 417 µs |
| Python | 12000 | 588974 | 3.28 ms | 3.30 ms |
| Python | 60000 | 2931341 | 3.68 ms | 3.68 ms |
| Cpp | 1200 | 59938 | 387 µs | 387 µs |
| Cpp | 12000 | 588296 | 3.04 ms | 3.04 ms |
| Cpp | 60000 | 2927600 | 3.34 ms | 3.34 ms |

**Analysis.**

- The gain over all the reachable cases is +13.0 points within the first 5; the worst language changes by +1.4 points.
- The slowest session with the previous word at the largest size is Python at 60000 lines: 3.68 ms (the frame budget is 16 ms).
- The corpus is generated. It has the structure the idea looks for (the values of a property, the table after `from`), because the generator was written with it; real code may repeat its structure more or less than that.

**Criterion met.** Keep the previous word on.

*Ran in 37.1 s.*

## Limits

- Synthetic code, not real files.
- The experiments run in a console process with a small heap, away from the Visual Studio UI thread and its GC pressure. They compare versions of the code: they are not times the user will see.
- Times are medians of repeated runs on one machine; differences of a few percent are noise.
