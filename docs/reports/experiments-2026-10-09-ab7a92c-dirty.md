# NestLight experiments report

| | |
|---|---|
| Date | 2026-10-09 00:20 UTC |
| Commit | `ab7a92c` (**uncommitted changes**: this run cannot be reproduced from the commit) |
| Runtime | .NET 10.0.0, Release |
| Machine | CPU not identified, 16 logical cores |
| OS | Linux Mint 22 |
| Mode | full |

Definitions of the experiments: [docs/experiments.md](../experiments.md). Each result is valid only for the commit and the machine above.

## Summary

| Id | Experiment | Criterion | Finding |
|---|---|---|---|
| E16 | Completion latency against the size of the file | Met | worst case 9.57 ms at 60000 lines |
| E17 | Do the limits of the completion change its cost? | Not met | largest difference 2.78x (distinct words, max 10000, min length 3) |
| E19 | Does the tokenizer agree with the vocabulary? | Met | vocabulary and tokenizers agree |
| E20 | Order of the words of the document | Met | nearest 72.1% against alphabetical 64.0% within the first 5 (locality 0.5) |
| E21 | Where the words come from, and how many keystrokes completion saves | Not met | whole document saves 55.3% of the characters; best narrower scope (all embedded strings) 58.0% |
| E22 | Sharing the scan and not creating the words of the completion | Met | worst case with the scan shared 1.65 ms at 60000 lines |
| E23 | Does the second stage of the completion fit in a frame? | Met | worst case with the second stage forced 10.60 ms at 60000 lines |
| E24 | Does the second stage recover the word after one mistake, and which tie-break works? | Met | best tie-break (nearest to the caret (the engine)) 72.0% within the first 5 |
| E25 | Does the second stage get in the way when the prefix is right? | Met | with FuzzyBelow = 1 similar items are added in 1.1% of the cases |
| E27 | Completion with similar words on incomplete and cut code | Met | 0 violations in 86900 carets, 21275 similar items checked |
| E28 | Does the word before the caret help to rank the suggestions? | Met | within the first 5: 72.5% to 85.5% (+13.0 points); worst session 4.09 ms |
| E29 | Do the words of the same language come first? | Met | within the first 5: previous word 85.5%, with the language 88.1% (+2.6 points); worst session 5.16 ms |
| E30 | Does the place in the grammar help to rank the suggestions? | Met | within the first 5: 88.1% to 97.2% (+9.2 points); worst session 5.03 ms |
| E31 | Does the schema read from the SQL of the document help? | Not met | within the first 5: 97.2% to 97.5% (+0.2 points); worst session 16.08 ms |

*Criterion met* is the statement of each experiment's own criterion, not a good/bad grade: see *If met* and *If not met* under each experiment.

## E16: Completion latency against the size of the file

**Hypothesis.** Every keystroke in an embedded string scans the host to find the string (Locate) and the whole text for words (Suggest). Those two scans, plus a dictionary of the matching words, are cheap on a normal file but could be noticeable on a large one, mostly when thousands of distinct words share the prefix.

**Method.** For each host, a file of increasing size with the caret at the end of an open `comp` in a marked SQL string. Two shapes: typical code (E01's file, a few distinct words) and a file whose every line declares a new `compNNNNN` identifier (thousands of distinct matches). Time of Locate alone and of Locate + Suggest, without the text copy of the editor (E03).

**Criterion.** Locate + Suggest stay under 16 ms (one frame) at the largest size, in every host and shape.

**Median time of one completion**

| Host | Shape | Lines | Characters | Locate | Locate + Suggest | Suggestions |
|---|---|---|---|---|---|---|
| JavaScript | typical code | 1200 | 41771 | 128 µs | 219 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 935 µs | 2.14 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 5.06 ms | 6.52 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 36 µs | 123 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 348 µs | 1.08 ms | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 1.72 ms | 3.41 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 93 µs | 200 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 913 µs | 1.95 ms | 1 |
| CSharp | typical code | 60000 | 1748126 | 4.97 ms | 6.46 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 42 µs | 129 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 414 µs | 1.14 ms | 100 |
| CSharp | distinct words | 60000 | 1200032 | 2.08 ms | 3.93 ms | 100 |
| Python | typical code | 1200 | 29494 | 163 µs | 251 µs | 1 |
| Python | typical code | 12000 | 294167 | 1.57 ms | 2.48 ms | 1 |
| Python | typical code | 60000 | 1470512 | 8.20 ms | 9.57 ms | 1 |
| Python | distinct words | 1200 | 18026 | 71 µs | 140 µs | 100 |
| Python | distinct words | 12000 | 180026 | 712 µs | 1.24 ms | 100 |
| Python | distinct words | 60000 | 900026 | 3.55 ms | 5.14 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 80 µs | 181 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 789 µs | 1.76 ms | 1 |
| Cpp | typical code | 60000 | 1629061 | 4.15 ms | 5.59 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 49 µs | 136 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 475 µs | 1.21 ms | 100 |
| Cpp | distinct words | 60000 | 1200036 | 2.38 ms | 3.95 ms | 100 |

**Analysis.**

- The slowest case at 60000 lines is Python, typical code: 9.57 ms (the frame budget is 16 ms).
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
| typical code | 100 | 3 | 2.14 ms | 1.00x | 1 |
| typical code | 10 | 3 | 2.14 ms | 1.00x | 1 |
| typical code | 1000 | 3 | 2.13 ms | 0.99x | 1 |
| typical code | 10000 | 3 | 2.13 ms | 0.99x | 1 |
| typical code | 100 | 1 | 2.17 ms | 1.01x | 1 |
| typical code | 100 | 5 | 2.11 ms | 0.98x | 1 |
| typical code | 100 | 3 (default again) | 2.15 ms | 1.00x | 1 |
| distinct words | 100 | 3 | 1.08 ms | 1.00x | 100 |
| distinct words | 10 | 3 | 1.06 ms | 0.99x | 10 |
| distinct words | 1000 | 3 | 1.20 ms | 1.12x | 1000 |
| distinct words | 10000 | 3 | 2.99 ms | 2.78x | 10000 |
| distinct words | 100 | 1 | 1.07 ms | 1.00x | 100 |
| distinct words | 100 | 5 | 1.03 ms | 0.96x | 100 |
| distinct words | 100 | 3 (default again) | 1.07 ms | 1.00x | 100 |

**Analysis.**

- The largest difference from the default is 2.78x (distinct words, max 10000, min length 3).

**Criterion not met.** The limits matter: the defaults need a justification, or the dictionary needs to be cheaper.

*Ran in 3.1 s.*

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

*Ran in 11.8 s.*

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

*Ran in 8.7 s.*

## E22: Sharing the scan and not creating the words of the completion

**Hypothesis.** E16 found that completion takes more than a frame above ~1 million characters, and that most of it is the scan of the host. In Visual Studio the session of a completion scans the text twice (once to decide whether to open, once to fill the list) after the classifier already did it for the same snapshot, and the word pass created a string and a sort entry for every match. Sharing one scan between the classifier and the completion, and creating a word only when it is offered, brings the worst case under a frame.

**Method.** The files of E16 (4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines). The cost of the start of one completion session, as the editor does it: Locate, Locate again and Suggest, over a new text instance each time. Two cases: nothing shared (a plain scanner, as in E16) and the scan shared (the classifier has already highlighted that text, through a CachingHostScanner). Also Suggest alone.

**Criterion.** The session with the scan shared stays under 16 ms at 60,000 lines, in every host and shape.

**Median time of the start of one completion session**

| Host | Shape | Lines | Characters | Nothing shared | Scan shared | Suggest alone | Suggestions |
|---|---|---|---|---|---|---|---|
| JavaScript | typical code | 1200 | 41771 | 315 µs | 132 µs | 130 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 3.28 ms | 1.24 ms | 1.25 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 11.74 ms | 1.51 ms | 1.50 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 161 µs | 88 µs | 86 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 1.45 ms | 743 µs | 713 µs | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 6.30 ms | 1.54 ms | 1.57 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 305 µs | 113 µs | 112 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 3.14 ms | 1.06 ms | 1.07 ms | 1 |
| CSharp | typical code | 60000 | 1748126 | 11.94 ms | 1.52 ms | 1.52 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 175 µs | 90 µs | 87 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 1.56 ms | 706 µs | 708 µs | 100 |
| CSharp | distinct words | 60000 | 1200032 | 6.33 ms | 1.45 ms | 1.47 ms | 100 |
| Python | typical code | 1200 | 29494 | 428 µs | 94 µs | 92 µs | 1 |
| Python | typical code | 12000 | 294167 | 4.25 ms | 902 µs | 897 µs | 1 |
| Python | typical code | 60000 | 1470512 | 18.45 ms | 1.55 ms | 1.52 ms | 1 |
| Python | distinct words | 1200 | 18026 | 215 µs | 71 µs | 67 µs | 100 |
| Python | distinct words | 12000 | 180026 | 1.99 ms | 555 µs | 497 µs | 100 |
| Python | distinct words | 60000 | 900026 | 8.86 ms | 1.65 ms | 1.40 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 268 µs | 110 µs | 109 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 2.62 ms | 1.02 ms | 1.01 ms | 1 |
| Cpp | typical code | 60000 | 1629061 | 10.04 ms | 1.61 ms | 1.56 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 194 µs | 128 µs | 87 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 1.74 ms | 772 µs | 709 µs | 100 |
| Cpp | distinct words | 60000 | 1200036 | 6.59 ms | 1.47 ms | 1.46 ms | 100 |

**Analysis.**

- The slowest session with the scan shared at 60000 lines is Python, distinct words: 1.65 ms (the frame budget is 16 ms).
- "Nothing shared" is the start of a session as the editor makes it without the shared scan: Locate twice and Suggest. E16 measured one Locate and Suggest.

**Criterion met.** Completion fits in a frame on any file the classifier can handle.

*Ran in 7.8 s.*

## E23: Does the second stage of the completion fit in a frame?

**Hypothesis.** The second stage compares what was typed with every distinct word of the document that passes two cheap filters, and keeps the best few. Even forced to run in every session, with thousands of words that are one edit away, one session still fits in the 16 ms of a frame, as E22 showed for the first stage.

**Method.** E22's files and session (Locate, Locate again and Suggest, over a new text instance, the classifier having highlighted it first), 4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines, with the second stage forced to run (it is asked for below any number of items). Two things typed: the exact prefix `comp` (the second stage compares everything and finds nothing new) and `cmop`, two letters swapped (in the distinct-words file, every one of the words is one edit away). Also the same session with the second stage off.

**Criterion.** The session with the second stage forced stays under 16 ms at 60,000 lines, in every host, shape and typed text.

**Median time of the start of one completion session (the scan shared)**

| Host | Shape | Typed | Lines | Characters | Second stage off | Second stage forced | Allocated off | Allocated forced | Similar items |
|---|---|---|---|---|---|---|---|---|---|
| JavaScript | typical code | comp | 1200 | 41771 | 135 µs | 268 µs | 9 KB | 10 KB | 1 |
| JavaScript | typical code | comp | 12000 | 417611 | 1.25 ms | 2.54 ms | 65 KB | 66 KB | 1 |
| JavaScript | typical code | comp | 60000 | 2088011 | 1.49 ms | 3.01 ms | 66 KB | 66 KB | 1 |
| JavaScript | typical code | cmop | 1200 | 41771 | 123 µs | 256 µs | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 12000 | 417611 | 1.18 ms | 2.45 ms | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 60000 | 2088011 | 1.41 ms | 2.92 ms | 1 KB | 2 KB | 1 |
| JavaScript | distinct words | comp | 1200 | 24011 | 88 µs | 88 µs | 67 KB | 67 KB | 0 |
| JavaScript | distinct words | comp | 12000 | 240011 | 748 µs | 716 µs | 291 KB | 291 KB | 0 |
| JavaScript | distinct words | comp | 60000 | 1200011 | 1.44 ms | 1.44 ms | 547 KB | 547 KB | 0 |
| JavaScript | distinct words | cmop | 1200 | 24011 | 67 µs | 396 µs | 1 KB | 168 KB | 10 |
| JavaScript | distinct words | cmop | 12000 | 240011 | 645 µs | 4.24 ms | 1 KB | 1.5 MB | 10 |
| JavaScript | distinct words | cmop | 60000 | 1200011 | 1.33 ms | 8.26 ms | 1 KB | 3.2 MB | 10 |
| CSharp | typical code | comp | 1200 | 35058 | 113 µs | 222 µs | 9 KB | 10 KB | 1 |
| CSharp | typical code | comp | 12000 | 349703 | 1.06 ms | 2.07 ms | 66 KB | 66 KB | 1 |
| CSharp | typical code | comp | 60000 | 1748126 | 1.53 ms | 2.98 ms | 66 KB | 66 KB | 1 |
| CSharp | typical code | cmop | 1200 | 35058 | 109 µs | 209 µs | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 12000 | 349703 | 992 µs | 2.02 ms | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 60000 | 1748126 | 1.40 ms | 2.82 ms | 1 KB | 2 KB | 1 |
| CSharp | distinct words | comp | 1200 | 24032 | 88 µs | 86 µs | 67 KB | 67 KB | 0 |
| CSharp | distinct words | comp | 12000 | 240032 | 692 µs | 690 µs | 291 KB | 291 KB | 0 |
| CSharp | distinct words | comp | 60000 | 1200032 | 1.43 ms | 1.42 ms | 547 KB | 547 KB | 0 |
| CSharp | distinct words | cmop | 1200 | 24032 | 67 µs | 383 µs | 1 KB | 168 KB | 10 |
| CSharp | distinct words | cmop | 12000 | 240032 | 637 µs | 4.25 ms | 1 KB | 1.5 MB | 10 |
| CSharp | distinct words | cmop | 60000 | 1200032 | 1.31 ms | 8.18 ms | 1 KB | 3.2 MB | 10 |
| Python | typical code | comp | 1200 | 29494 | 96 µs | 188 µs | 9 KB | 10 KB | 1 |
| Python | typical code | comp | 12000 | 294167 | 890 µs | 1.73 ms | 65 KB | 66 KB | 1 |
| Python | typical code | comp | 60000 | 1470512 | 1.51 ms | 2.97 ms | 130 KB | 130 KB | 1 |
| Python | typical code | cmop | 1200 | 29494 | 85 µs | 177 µs | 1 KB | 2 KB | 1 |
| Python | typical code | cmop | 12000 | 294167 | 816 µs | 1.67 ms | 1 KB | 2 KB | 1 |
| Python | typical code | cmop | 60000 | 1470512 | 1.40 ms | 2.88 ms | 1 KB | 2 KB | 1 |
| Python | distinct words | comp | 1200 | 18026 | 67 µs | 72 µs | 67 KB | 67 KB | 0 |
| Python | distinct words | comp | 12000 | 180026 | 506 µs | 511 µs | 291 KB | 291 KB | 0 |
| Python | distinct words | comp | 60000 | 900026 | 1.46 ms | 1.38 ms | 1.0 MB | 1.0 MB | 0 |
| Python | distinct words | cmop | 1200 | 18026 | 47 µs | 416 µs | 1 KB | 168 KB | 10 |
| Python | distinct words | cmop | 12000 | 180026 | 433 µs | 3.84 ms | 1 KB | 1.5 MB | 10 |
| Python | distinct words | cmop | 60000 | 900026 | 1.20 ms | 10.60 ms | 1 KB | 3.7 MB | 10 |
| Cpp | typical code | comp | 1200 | 32804 | 110 µs | 205 µs | 9 KB | 10 KB | 1 |
| Cpp | typical code | comp | 12000 | 325951 | 984 µs | 1.95 ms | 65 KB | 66 KB | 1 |
| Cpp | typical code | comp | 60000 | 1629061 | 1.53 ms | 3.01 ms | 130 KB | 130 KB | 1 |
| Cpp | typical code | cmop | 1200 | 32804 | 95 µs | 198 µs | 1 KB | 2 KB | 1 |
| Cpp | typical code | cmop | 12000 | 325951 | 913 µs | 1.88 ms | 1 KB | 2 KB | 1 |
| Cpp | typical code | cmop | 60000 | 1629061 | 1.42 ms | 2.89 ms | 1 KB | 2 KB | 1 |
| Cpp | distinct words | comp | 1200 | 24036 | 90 µs | 87 µs | 67 KB | 67 KB | 0 |
| Cpp | distinct words | comp | 12000 | 240036 | 702 µs | 704 µs | 291 KB | 291 KB | 0 |
| Cpp | distinct words | comp | 60000 | 1200036 | 1.43 ms | 1.43 ms | 547 KB | 547 KB | 0 |
| Cpp | distinct words | cmop | 1200 | 24036 | 67 µs | 386 µs | 1 KB | 168 KB | 10 |
| Cpp | distinct words | cmop | 12000 | 240036 | 642 µs | 4.27 ms | 1 KB | 1.5 MB | 10 |
| Cpp | distinct words | cmop | 60000 | 1200036 | 1.32 ms | 8.02 ms | 1 KB | 3.2 MB | 10 |

**Analysis.**

- The slowest session with the second stage forced at 60000 lines is Python, distinct words, typed cmop: 10.60 ms (the frame budget is 16 ms).
- Allocated is what the session allocates (the same meaning as E05): the difference between forced and off is the cost of the second stage; no string is created for a word that is too far, only a small record to skip its next occurrences.
- In the typical file there are few distinct words and nearly all of them fail the first-letter filter; in the file of distinct words every word is a candidate and, for `cmop`, every one is one edit away.

**Criterion met.** The second stage needs no cache of the distinct words and no index.

*Ran in 13.6 s.*

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

*Ran in 6.7 s.*

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

*Ran in 6.1 s.*

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

**The word typed with 1 to 3 letters: 9564 typed prefixes, 9165 reachable (95.8%)**

| Variant | Word first | Within the first 5 | Within the first 10 | Mean reciprocal rank |
|---|---|---|---|---|
| By distance alone | 36.8% | 72.5% | 86.0% | 0.528 |
| Previous word first | 68.5% | 85.5% | 92.6% | 0.764 |

**Within the first 5, by language of the string**

| Language | Cases | By distance alone | Previous word first |
|---|---|---|---|
| css | 732 | 74.3% | 80.7% |
| graphql | 657 | 88.7% | 90.1% |
| html | 1128 | 61.1% | 70.7% |
| sql | 6648 | 72.7% | 88.1% |

**Within the first 5, by number of letters typed**

| Letters | Cases | By distance alone | Previous word first |
|---|---|---|---|
| 1 | 3055 | 39.1% | 67.5% |
| 2 | 3055 | 83.4% | 92.0% |
| 3 | 3055 | 95.0% | 97.0% |

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

**Median time of the start of one completion session (typed `comp|` in an SQL string, structured files)**

| Host | Lines | Characters | By distance alone | Previous word first |
|---|---|---|---|---|
| JavaScript | 1200 | 58758 | 448 µs | 445 µs |
| JavaScript | 12000 | 577746 | 3.57 ms | 3.57 ms |
| JavaScript | 60000 | 2875519 | 3.99 ms | 3.98 ms |
| CSharp | 1200 | 68232 | 475 µs | 476 µs |
| CSharp | 12000 | 670891 | 3.32 ms | 3.31 ms |
| CSharp | 60000 | 3339512 | 3.76 ms | 3.76 ms |
| Python | 1200 | 59971 | 468 µs | 467 µs |
| Python | 12000 | 588954 | 3.69 ms | 3.68 ms |
| Python | 60000 | 2931279 | 4.10 ms | 4.09 ms |
| Cpp | 1200 | 59936 | 427 µs | 430 µs |
| Cpp | 12000 | 588276 | 3.39 ms | 3.38 ms |
| Cpp | 60000 | 2927538 | 3.67 ms | 3.68 ms |

**Analysis.**

- The gain over all the reachable cases is +13.0 points within the first 5; the worst language changes by +1.4 points.
- The slowest session with the previous word at the largest size is Python at 60000 lines: 4.09 ms (the frame budget is 16 ms).
- The corpus is generated. It has the structure the idea looks for (the values of a property, the table after `from`), because the generator was written with it; real code may repeat its structure more or less than that.

**Criterion met.** Keep the previous word on.

*Ran in 37.6 s.*

## E29: Do the words of the same language come first?

**Hypothesis.** A word written in the code of another string of the same language (a column in another SQL string) is likelier than a word of the host code or of a string of another language that happens to start with the same letters, even when the other one is nearer to the caret. Putting the words of the language first, and taking the context of the previous word from them only, puts the meant word in the first 5 more often.

**Method.** E28's probes (50 generated files with structure, words typed with 1 to 3 letters, the list the editor gets). Four variants: the order by distance alone; the words of the language first; the previous word; both. Interpolations are host code. Also the start of a session on files of 1,200 to 60,000 lines, with the scan shared (the scope needs the strings, which the classifier already found).

**Criterion.** Adding the words of the language to the previous word is at least 2 points better within the first 5 over all the reachable cases; in no language it falls by more than 1 point; and the session with both stays under 16 ms at 60,000 lines, in every host.

**The word typed with 1 to 3 letters: 9564 typed prefixes, 9165 reachable (95.8%)**

| Variant | Word first | Within the first 5 | Within the first 10 | Mean reciprocal rank |
|---|---|---|---|---|
| By distance alone | 36.8% | 72.5% | 86.0% | 0.528 |
| Words of the language | 41.1% | 77.3% | 90.6% | 0.575 |
| Previous word | 68.5% | 85.5% | 92.6% | 0.764 |
| Previous word and language | 70.6% | 88.1% | 94.9% | 0.788 |

**Within the first 5, by language of the string**

| Language | Cases | By distance alone | Words of the language | Previous word | Previous word and language |
|---|---|---|---|---|---|
| css | 732 | 74.3% | 75.5% | 80.7% | 81.8% |
| graphql | 657 | 88.7% | 95.9% | 90.1% | 96.0% |
| html | 1128 | 61.1% | 69.5% | 70.7% | 77.4% |
| sql | 6648 | 72.7% | 77.0% | 88.1% | 89.8% |

**Within the first 5, by number of letters typed**

| Letters | Cases | By distance alone | Words of the language | Previous word | Previous word and language |
|---|---|---|---|---|---|
| 1 | 3055 | 39.1% | 42.1% | 67.5% | 69.2% |
| 2 | 3055 | 83.4% | 90.7% | 92.0% | 95.7% |
| 3 | 3055 | 95.0% | 99.1% | 97.0% | 99.2% |

**Within the first 5, by the word before the one typed (the most frequent)**

| Before | Cases | By distance alone | Words of the language | Previous word | Previous word and language |
|---|---|---|---|---|---|
| `text,` | 555 | 60.7% | 63.4% | 88.8% | 91.0% |
| `o.` | 297 | 63.0% | 68.4% | 91.9% | 92.6% |
| `from` | 264 | 39.4% | 63.6% | 86.4% | 92.0% |
| `t.` | 234 | 51.3% | 58.1% | 91.0% | 91.5% |
| `into` | 180 | 37.8% | 58.3% | 82.2% | 87.8% |
| `key,` | 165 | 62.4% | 64.2% | 78.8% | 78.8% |
| `select` | 165 | 64.8% | 68.5% | 83.6% | 84.8% |
| `update` | 159 | 42.1% | 62.3% | 81.8% | 89.9% |

**Median time of the start of one completion session (typed `comp|` in an SQL string, structured files)**

| Host | Lines | Characters | By distance alone | Words of the language | Previous word | Previous word and language |
|---|---|---|---|---|---|---|
| JavaScript | 1200 | 58758 | 443 µs | 465 µs | 443 µs | 462 µs |
| JavaScript | 12000 | 577746 | 3.54 ms | 3.79 ms | 3.62 ms | 3.84 ms |
| JavaScript | 60000 | 2875519 | 4.09 ms | 5.16 ms | 4.09 ms | 5.16 ms |
| CSharp | 1200 | 68232 | 476 µs | 501 µs | 479 µs | 498 µs |
| CSharp | 12000 | 670891 | 3.37 ms | 3.56 ms | 3.36 ms | 3.57 ms |
| CSharp | 60000 | 3339512 | 3.83 ms | 4.71 ms | 3.72 ms | 4.76 ms |
| Python | 1200 | 59971 | 463 µs | 484 µs | 462 µs | 479 µs |
| Python | 12000 | 588954 | 3.65 ms | 3.85 ms | 3.66 ms | 3.85 ms |
| Python | 60000 | 2931279 | 4.05 ms | 5.06 ms | 4.07 ms | 5.09 ms |
| Cpp | 1200 | 59936 | 430 µs | 440 µs | 433 µs | 439 µs |
| Cpp | 12000 | 588276 | 3.38 ms | 3.51 ms | 3.39 ms | 3.52 ms |
| Cpp | 60000 | 2927538 | 3.67 ms | 4.35 ms | 3.68 ms | 4.36 ms |

**Analysis.**

- The words of the language alone move the share within the first 5 from 72.5% to 77.3%; added to the previous word, +2.6 points; the worst language changes by +1.1 points.
- The slowest session with both at the largest size is JavaScript at 60000 lines: 5.16 ms (the frame budget is 16 ms).
- The corpus is generated: its host variables are named after the same nouns as the tables, so a host word with the same first letters is common by construction. How often real code has that is not measured.

**Criterion met.** Keep the words of the language first.

*Ran in 74.3 s.*

## E30: Does the place in the grammar help to rank the suggestions?

**Hypothesis.** What belongs at the caret can be told from a few characters of look-behind: a table after `from`, a column after `select`, `by` after `group`, the properties inside the braces of CSS and the values of the property after its colon, the attributes of the tag inside `<button `. Putting what belongs first and what does not last puts the meant word in the first 5 more often than the previous word and the language alone, in every language that has a grammar, and it fits in a frame.

**Method.** E28's probes (50 generated files with structure, words typed with 1 to 3 letters, the list the editor gets). Four variants: the order by distance alone; the grammar; the previous word and the language (what the plugin ran with before this experiment); all three. Reported by the place of the caret. Also the start of a session on files of 1,200 to 60,000 lines.

**Criterion.** Adding the grammar to the previous word and the language is at least 2 points better within the first 5 over all the reachable cases; SQL, CSS and HTML each do not fall; and the session with all three stays under 16 ms at 60,000 lines, in every host.

**The word typed with 1 to 3 letters: 9564 typed prefixes, 9165 reachable (95.8%)**

| Variant | Word first | Within the first 5 | Within the first 10 | Mean reciprocal rank |
|---|---|---|---|---|
| By distance alone | 36.8% | 72.5% | 86.0% | 0.528 |
| Grammar | 57.6% | 87.8% | 96.2% | 0.698 |
| Previous word and language | 70.6% | 88.1% | 94.9% | 0.788 |
| All three | 83.0% | 97.2% | 99.2% | 0.892 |

**Within the first 5, by language of the string**

| Language | Cases | By distance alone | Grammar | Previous word and language | All three |
|---|---|---|---|---|---|
| css | 732 | 74.3% | 86.7% | 81.8% | 88.0% |
| graphql | 657 | 88.7% | 88.7% | 96.0% | 96.0% |
| html | 1128 | 61.1% | 75.6% | 77.4% | 91.2% |
| sql | 6648 | 72.7% | 89.9% | 89.8% | 99.4% |

**Within the first 5, by number of letters typed**

| Letters | Cases | By distance alone | Grammar | Previous word and language | All three |
|---|---|---|---|---|---|
| 1 | 3055 | 39.1% | 75.0% | 69.2% | 93.3% |
| 2 | 3055 | 83.4% | 92.1% | 95.7% | 99.0% |
| 3 | 3055 | 95.0% | 96.3% | 99.2% | 99.4% |

**Within the first 5, by the place of the caret in the grammar**

| Place | Cases | By distance alone | Grammar | Previous word and language | All three |
|---|---|---|---|---|---|
| `sql:expression` | 2016 | 64.7% | 88.2% | 83.6% | 98.8% |
| `(none)` | 1671 | 95.6% | 95.6% | 98.4% | 98.4% |
| `sql:statement` | 831 | 84.4% | 100.0% | 84.4% | 100.0% |
| `sql:table` | 819 | 38.6% | 60.0% | 86.7% | 98.4% |
| `sql:member` | 531 | 57.8% | 81.9% | 92.1% | 99.2% |
| `html:tag` | 423 | 73.3% | 73.3% | 82.0% | 82.0% |
| `html:attribute` | 360 | 67.2% | 100.0% | 75.8% | 100.0% |
| `html:value` | 345 | 39.7% | 53.0% | 73.3% | 93.3% |
| `css:property` | 333 | 80.2% | 86.5% | 80.5% | 86.5% |
| `css:value` | 312 | 78.8% | 100.0% | 93.6% | 100.0% |
| `sql:continue-select` | 285 | 66.7% | 100.0% | 88.4% | 100.0% |
| `sql:continue-from` | 219 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:continue-into` | 183 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:after-select` | 165 | 64.8% | 92.7% | 84.8% | 100.0% |
| `sql:continue-on` | 132 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:after-create` | 123 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:after-insert` | 117 | 33.3% | 100.0% | 100.0% | 100.0% |
| `css:selector` | 87 | 35.6% | 40.2% | 44.8% | 50.6% |
| `sql:continue-where` | 87 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:after-not` | 72 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:continue-order` | 54 | 66.7% | 100.0% | 77.8% | 100.0% |

**Within the first 5, by the word before the one typed (the most frequent)**

| Before | Cases | By distance alone | Grammar | Previous word and language | All three |
|---|---|---|---|---|---|
| `text,` | 555 | 60.7% | 88.8% | 91.0% | 97.7% |
| `o.` | 297 | 63.0% | 87.2% | 92.6% | 99.0% |
| `from` | 264 | 39.4% | 62.5% | 92.0% | 98.9% |
| `t.` | 234 | 51.3% | 75.2% | 91.5% | 99.6% |
| `into` | 180 | 37.8% | 57.8% | 87.8% | 99.4% |
| `key,` | 165 | 62.4% | 93.3% | 78.8% | 98.8% |

**Median time of the start of one completion session (typed `comp|` in an SQL string, structured files)**

| Host | Lines | Characters | By distance alone | Grammar | Previous word and language | All three |
|---|---|---|---|---|---|---|
| JavaScript | 1200 | 58758 | 444 µs | 448 µs | 468 µs | 468 µs |
| JavaScript | 12000 | 577746 | 3.56 ms | 3.58 ms | 3.77 ms | 3.78 ms |
| JavaScript | 60000 | 2875519 | 3.98 ms | 3.96 ms | 4.95 ms | 4.94 ms |
| CSharp | 1200 | 68232 | 475 µs | 480 µs | 495 µs | 492 µs |
| CSharp | 12000 | 670891 | 3.30 ms | 3.31 ms | 3.53 ms | 3.54 ms |
| CSharp | 60000 | 3339512 | 3.77 ms | 3.73 ms | 4.72 ms | 4.70 ms |
| Python | 1200 | 59971 | 462 µs | 464 µs | 482 µs | 483 µs |
| Python | 12000 | 588954 | 3.66 ms | 3.67 ms | 3.86 ms | 3.86 ms |
| Python | 60000 | 2931279 | 4.06 ms | 4.05 ms | 5.04 ms | 5.03 ms |
| Cpp | 1200 | 59936 | 423 µs | 433 µs | 445 µs | 440 µs |
| Cpp | 12000 | 588276 | 3.38 ms | 3.38 ms | 3.52 ms | 3.51 ms |
| Cpp | 60000 | 2927538 | 3.67 ms | 3.67 ms | 4.34 ms | 4.33 ms |

**Analysis.**

- The grammar alone moves the share within the first 5 from 72.5% to 87.8%; added to the previous word and the language, +9.2 points; the worst of SQL, CSS and HTML changes by +6.1 points.
- The slowest session with all three at the largest size is Python at 60000 lines: 5.03 ms (the frame budget is 16 ms).
- The corpus is generated by the same person who wrote the rules of the grammar, and it follows them: the attributes of a tag are the ones in the table of attributes, the values of a property the ones in the table of values. A real file will have words the tables do not know, and the grammar then costs nothing but gains nothing. This experiment says the rules do not get in the way of code that follows them, not how often real code does.

**Criterion met.** Keep the grammar on.

*Ran in 72.6 s.*

## E31: Does the schema read from the SQL of the document help?

**Hypothesis.** The tables and columns that the SQL of the file talks about (a CREATE TABLE, the FROM and JOIN of the statement, the aliases) tell which table to offer after FROM and which columns belong after `u.` or in the select list, and that is more precise than the words that happened to follow the same word elsewhere: an alias means a different table in every statement. Putting those first puts the meant word in the first 5 more often, in the places where a table or a column is typed, and the whole file can be read in a frame.

**Method.** E28's probes (50 generated files with structure, words typed with 1 to 3 letters, the list the editor gets), reported by the place of the caret. Four variants: the order by distance alone; the previous word, the language and the grammar (what the plugin ran with before this experiment); the same plus the schema; the schema alone. The files hold a CREATE TABLE for each table, queries on the columns of their table, joins on the foreign keys with the aliases `t` and `o`, UPDATEs and INSERTs. Also the start of a session on files of 1,200 to 60,000 lines, where every SQL string is read for the schema.

**Criterion.** Adding the schema is at least 1 point better within the first 5 over all the reachable cases; the places that need a table or a column (`sql:table`, `sql:member`, `sql:expression`) do not fall; and the session stays under 16 ms at 60,000 lines in every host.

**The word typed with 1 to 3 letters: 9564 typed prefixes, 9165 reachable (95.8%)**

| Variant | Word first | Within the first 5 | Within the first 10 | Mean reciprocal rank |
|---|---|---|---|---|
| By distance alone | 36.8% | 72.5% | 86.0% | 0.528 |
| Before the schema | 83.0% | 97.2% | 99.2% | 0.892 |
| With the schema | 86.8% | 97.5% | 99.2% | 0.915 |
| Schema alone | 56.0% | 84.6% | 93.0% | 0.687 |

**Within the first 5, by language of the string**

| Language | Cases | By distance alone | Before the schema | With the schema | Schema alone |
|---|---|---|---|---|---|
| css | 732 | 74.3% | 88.0% | 88.0% | 74.3% |
| graphql | 657 | 88.7% | 96.0% | 96.0% | 88.7% |
| html | 1128 | 61.1% | 91.2% | 91.2% | 61.1% |
| sql | 6648 | 72.7% | 99.4% | 99.7% | 89.3% |

**Within the first 5, by number of letters typed**

| Letters | Cases | By distance alone | Before the schema | With the schema | Schema alone |
|---|---|---|---|---|---|
| 1 | 3055 | 39.1% | 93.3% | 94.0% | 63.4% |
| 2 | 3055 | 83.4% | 99.0% | 99.0% | 92.9% |
| 3 | 3055 | 95.0% | 99.4% | 99.4% | 97.5% |

**Within the first 5, by the place of the caret in the grammar**

| Place | Cases | By distance alone | Before the schema | With the schema | Schema alone |
|---|---|---|---|---|---|
| `sql:expression` | 2016 | 64.7% | 98.8% | 99.1% | 81.5% |
| `(none)` | 1671 | 95.6% | 98.4% | 98.4% | 95.6% |
| `sql:statement` | 831 | 84.4% | 100.0% | 100.0% | 84.4% |
| `sql:table` | 819 | 38.6% | 98.4% | 100.0% | 100.0% |
| `sql:member` | 531 | 57.8% | 99.2% | 99.8% | 97.2% |
| `html:tag` | 423 | 73.3% | 82.0% | 82.0% | 73.3% |
| `html:attribute` | 360 | 67.2% | 100.0% | 100.0% | 67.2% |
| `html:value` | 345 | 39.7% | 93.3% | 93.3% | 39.7% |
| `css:property` | 333 | 80.2% | 86.5% | 86.5% | 80.2% |
| `css:value` | 312 | 78.8% | 100.0% | 100.0% | 78.8% |
| `sql:continue-select` | 285 | 66.7% | 100.0% | 100.0% | 66.7% |
| `sql:continue-from` | 219 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:continue-into` | 183 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:after-select` | 165 | 64.8% | 100.0% | 100.0% | 100.0% |
| `sql:continue-on` | 132 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:after-create` | 123 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:after-insert` | 117 | 33.3% | 100.0% | 100.0% | 33.3% |
| `css:selector` | 87 | 35.6% | 50.6% | 50.6% | 35.6% |
| `sql:continue-where` | 87 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:after-not` | 72 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:continue-order` | 54 | 66.7% | 100.0% | 100.0% | 66.7% |

**Within the first 5, by the word before the one typed (the most frequent)**

| Before | Cases | By distance alone | Before the schema | With the schema | Schema alone |
|---|---|---|---|---|---|
| `text,` | 555 | 60.7% | 97.7% | 97.7% | 60.7% |
| `o.` | 297 | 63.0% | 99.0% | 99.7% | 94.9% |
| `from` | 264 | 39.4% | 98.9% | 100.0% | 100.0% |
| `t.` | 234 | 51.3% | 99.6% | 100.0% | 100.0% |
| `into` | 180 | 37.8% | 99.4% | 100.0% | 100.0% |
| `key,` | 165 | 62.4% | 98.8% | 98.8% | 62.4% |

**Short files (4 functions each, 200 files, 18006 reachable prefixes): within the first 5, by the place of the caret**

| Place | Cases | Before the schema | With the schema |
|---|---|---|---|
| `sql:table` | 789 | 93.8% | 99.5% |
| `sql:member` | 363 | 97.5% | 100.0% |
| `sql:expression` | 6102 | 98.7% | 98.9% |
| all | 18006 | 98.5% | 98.8% |

**Median time of the start of one completion session (typed `select u.comp| from users u` in an SQL string, structured files)**

| Host | Lines | Characters | By distance alone | Before the schema | With the schema | Schema alone |
|---|---|---|---|---|---|---|
| JavaScript | 1200 | 58780 | 447 µs | 468 µs | 741 µs | 740 µs |
| JavaScript | 12000 | 577768 | 3.56 ms | 3.78 ms | 6.05 ms | 6.05 ms |
| JavaScript | 60000 | 2875541 | 3.95 ms | 4.95 ms | 7.29 ms | 7.22 ms |
| CSharp | 1200 | 68254 | 472 µs | 496 µs | 760 µs | 764 µs |
| CSharp | 12000 | 670913 | 3.32 ms | 3.53 ms | 5.47 ms | 5.44 ms |
| CSharp | 60000 | 3339534 | 3.70 ms | 4.69 ms | 6.81 ms | 6.70 ms |
| Python | 1200 | 59993 | 466 µs | 481 µs | 756 µs | 766 µs |
| Python | 12000 | 588976 | 3.71 ms | 3.91 ms | 6.17 ms | 6.16 ms |
| Python | 60000 | 2931301 | 4.16 ms | 5.21 ms | 7.57 ms | 7.36 ms |
| Cpp | 1200 | 59958 | 433 µs | 448 µs | 702 µs | 701 µs |
| Cpp | 12000 | 588298 | 3.37 ms | 3.52 ms | 5.64 ms | 5.63 ms |
| Cpp | 60000 | 2927560 | 3.68 ms | 4.33 ms | 6.39 ms | 16.08 ms |

**Analysis.**

- The schema alone moves the share within the first 5 from 72.5% to 84.6%; added to the rest, +0.2 points; the worst of the table, member and expression places changes by +0.3 points.
- The slowest session with the schema at the largest size is Cpp at 60000 lines: 16.08 ms (the frame budget is 16 ms). Every SQL string in the window of 500,000 characters around the caret is read at each request.
- The corpus is generated with a schema in mind: every column belongs to one table, a CREATE TABLE exists for each, and the aliases are reused with a different table in every statement. Real code with no CREATE TABLE in the same file has only what its queries reveal.

**Criterion not met.** If it is the time: read only the strings near the caret, or keep the schema of a text between sessions. If it is the gain: the previous word already did the work and the reader is not worth its cost.

*Ran in 79.0 s.*

## Limits

- Synthetic code, not real files.
- The experiments run in a console process with a small heap, away from the Visual Studio UI thread and its GC pressure. They compare versions of the code: they are not times the user will see.
- Times are medians of repeated runs on one machine; differences of a few percent are noise.
