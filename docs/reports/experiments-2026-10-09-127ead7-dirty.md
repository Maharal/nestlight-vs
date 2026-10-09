# NestLight experiments report

| | |
|---|---|
| Date | 2026-10-09 01:28 UTC |
| Commit | `127ead7` (**uncommitted changes**: this run cannot be reproduced from the commit) |
| Runtime | .NET 10.0.0, Release |
| Machine | CPU not identified, 16 logical cores |
| OS | Linux Mint 22 |
| Mode | full |

Definitions of the experiments: [docs/experiments.md](../experiments.md). Each result is valid only for the commit and the machine above.

## Summary

| Id | Experiment | Criterion | Finding |
|---|---|---|---|
| E19 | Does the tokenizer agree with the vocabulary? | Met | vocabulary and tokenizers agree |
| E22 | Sharing the scan and not creating the words of the completion | Met | worst case with the scan shared 1.44 ms at 60000 lines |
| E23 | Does the second stage of the completion fit in a frame? | Met | worst case with the second stage forced 10.04 ms at 60000 lines |
| E24 | Does the second stage recover the word after one mistake, and which tie-break works? | Met | best tie-break (nearest to the caret (the engine)) 72.0% within the first 5 |
| E25 | Does the second stage get in the way when the prefix is right? | Met | with FuzzyBelow = 1 similar items are added in 1.1% of the cases |
| E27 | Completion with similar words on incomplete and cut code | Met | 0 violations in 86900 carets, 21275 similar items checked |
| E28 | Does the word before the caret help to rank the suggestions? | Met | within the first 5: 72.5% to 85.5% (+13.0 points); worst session 3.82 ms |
| E29 | Do the words of the same language come first? | Met | within the first 5: previous word 85.5%, with the language 88.1% (+2.6 points); worst session 4.99 ms |
| E30 | Does the place in the grammar help to rank the suggestions? | Met | within the first 5: 88.1% to 97.8% (+9.8 points); worst session 4.66 ms |
| E33 | Completion with the context rankings on incomplete and cut code | Met | 0 violations in 85633 carets, 29806 similar items checked |
| E34 | Where the place of the caret says nothing, do the words of the file and the most used keywords come first? | Not met | best: Both within the first 5 73.9% to 76.1% (+2.2 points); hand-written +9.9 |
| E35 | Do a few keywords still come before the words of the file? | Met | best: 12 keywords in front within the first 5 73.9% to 76.9% (+3.0 points); hand-written +11.2 |

*Criterion met* is the statement of each experiment's own criterion, not a good/bad grade: see *If met* and *If not met* under each experiment.

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

*Ran in 0.1 s.*

## E22: Sharing the scan and not creating the words of the completion

**Hypothesis.** E16 found that completion takes more than a frame above ~1 million characters, and that most of it is the scan of the host. In Visual Studio the session of a completion scans the text twice (once to decide whether to open, once to fill the list) after the classifier already did it for the same snapshot, and the word pass created a string and a sort entry for every match. Sharing one scan between the classifier and the completion, and creating a word only when it is offered, brings the worst case under a frame.

**Method.** The files of E16 (4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines). The cost of the start of one completion session, as the editor does it: Locate, Locate again and Suggest, over a new text instance each time. Two cases: nothing shared (a plain scanner, as in E16) and the scan shared (the classifier has already highlighted that text, through a CachingHostScanner). Also Suggest alone.

**Criterion.** The session with the scan shared stays under 16 ms at 60,000 lines, in every host and shape.

**Median time of the start of one completion session**

| Host | Shape | Lines | Characters | Nothing shared | Scan shared | Suggest alone | Suggestions |
|---|---|---|---|---|---|---|---|
| JavaScript | typical code | 1200 | 41771 | 374 µs | 111 µs | 109 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 3.14 ms | 1.00 ms | 1.03 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 11.64 ms | 1.23 ms | 1.21 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 160 µs | 114 µs | 87 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 1.42 ms | 719 µs | 680 µs | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 6.09 ms | 1.34 ms | 1.40 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 280 µs | 92 µs | 90 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 2.84 ms | 856 µs | 852 µs | 1 |
| CSharp | typical code | 60000 | 1748126 | 11.33 ms | 1.24 ms | 1.21 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 174 µs | 92 µs | 87 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 1.51 ms | 708 µs | 674 µs | 100 |
| CSharp | distinct words | 60000 | 1200032 | 6.08 ms | 1.39 ms | 1.35 ms | 100 |
| Python | typical code | 1200 | 29494 | 403 µs | 78 µs | 75 µs | 1 |
| Python | typical code | 12000 | 294167 | 3.98 ms | 731 µs | 716 µs | 1 |
| Python | typical code | 60000 | 1470512 | 17.53 ms | 1.26 ms | 1.22 ms | 1 |
| Python | distinct words | 1200 | 18026 | 202 µs | 89 µs | 59 µs | 100 |
| Python | distinct words | 12000 | 180026 | 1.82 ms | 447 µs | 404 µs | 100 |
| Python | distinct words | 60000 | 900026 | 8.23 ms | 1.44 ms | 1.16 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 254 µs | 88 µs | 85 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 2.48 ms | 826 µs | 821 µs | 1 |
| Cpp | typical code | 60000 | 1629061 | 9.65 ms | 1.27 ms | 1.32 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 186 µs | 113 µs | 86 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 1.64 ms | 716 µs | 681 µs | 100 |
| Cpp | distinct words | 60000 | 1200036 | 6.20 ms | 1.34 ms | 1.39 ms | 100 |

**Analysis.**

- The slowest session with the scan shared at 60000 lines is Python, distinct words: 1.44 ms (the frame budget is 16 ms).
- "Nothing shared" is the start of a session as the editor makes it without the shared scan: Locate twice and Suggest. E16 measured one Locate and Suggest.

**Criterion met.** Completion fits in a frame on any file the classifier can handle.

*Ran in 7.5 s.*

## E23: Does the second stage of the completion fit in a frame?

**Hypothesis.** The second stage compares what was typed with every distinct word of the document that passes two cheap filters, and keeps the best few. Even forced to run in every session, with thousands of words that are one edit away, one session still fits in the 16 ms of a frame, as E22 showed for the first stage.

**Method.** E22's files and session (Locate, Locate again and Suggest, over a new text instance, the classifier having highlighted it first), 4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines, with the second stage forced to run (it is asked for below any number of items). Two things typed: the exact prefix `comp` (the second stage compares everything and finds nothing new) and `cmop`, two letters swapped (in the distinct-words file, every one of the words is one edit away). Also the same session with the second stage off.

**Criterion.** The session with the second stage forced stays under 16 ms at 60,000 lines, in every host, shape and typed text.

**Median time of the start of one completion session (the scan shared)**

| Host | Shape | Typed | Lines | Characters | Second stage off | Second stage forced | Allocated off | Allocated forced | Similar items |
|---|---|---|---|---|---|---|---|---|---|
| JavaScript | typical code | comp | 1200 | 41771 | 112 µs | 234 µs | 9 KB | 10 KB | 1 |
| JavaScript | typical code | comp | 12000 | 417611 | 1.01 ms | 2.25 ms | 66 KB | 66 KB | 1 |
| JavaScript | typical code | comp | 60000 | 2088011 | 1.23 ms | 2.72 ms | 66 KB | 66 KB | 1 |
| JavaScript | typical code | cmop | 1200 | 41771 | 94 µs | 235 µs | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 12000 | 417611 | 910 µs | 2.14 ms | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 60000 | 2088011 | 1.09 ms | 2.60 ms | 1 KB | 2 KB | 1 |
| JavaScript | distinct words | comp | 1200 | 24011 | 85 µs | 85 µs | 67 KB | 67 KB | 0 |
| JavaScript | distinct words | comp | 12000 | 240011 | 719 µs | 701 µs | 291 KB | 291 KB | 0 |
| JavaScript | distinct words | comp | 60000 | 1200011 | 1.50 ms | 1.46 ms | 547 KB | 547 KB | 0 |
| JavaScript | distinct words | cmop | 1200 | 24011 | 53 µs | 453 µs | 1 KB | 168 KB | 10 |
| JavaScript | distinct words | cmop | 12000 | 240011 | 490 µs | 4.07 ms | 1 KB | 1.5 MB | 10 |
| JavaScript | distinct words | cmop | 60000 | 1200011 | 1.07 ms | 9.17 ms | 1 KB | 3.2 MB | 10 |
| CSharp | typical code | comp | 1200 | 35058 | 92 µs | 203 µs | 9 KB | 10 KB | 1 |
| CSharp | typical code | comp | 12000 | 349703 | 902 µs | 1.94 ms | 66 KB | 66 KB | 1 |
| CSharp | typical code | comp | 60000 | 1748126 | 1.31 ms | 2.92 ms | 66 KB | 66 KB | 1 |
| CSharp | typical code | cmop | 1200 | 35058 | 83 µs | 194 µs | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 12000 | 349703 | 840 µs | 1.85 ms | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 60000 | 1748126 | 1.14 ms | 2.63 ms | 1 KB | 2 KB | 1 |
| CSharp | distinct words | comp | 1200 | 24032 | 88 µs | 90 µs | 67 KB | 67 KB | 0 |
| CSharp | distinct words | comp | 12000 | 240032 | 726 µs | 684 µs | 291 KB | 291 KB | 0 |
| CSharp | distinct words | comp | 60000 | 1200032 | 1.55 ms | 1.58 ms | 547 KB | 547 KB | 0 |
| CSharp | distinct words | cmop | 1200 | 24032 | 52 µs | 434 µs | 1 KB | 168 KB | 10 |
| CSharp | distinct words | cmop | 12000 | 240032 | 496 µs | 4.17 ms | 1 KB | 1.5 MB | 10 |
| CSharp | distinct words | cmop | 60000 | 1200032 | 1.02 ms | 8.13 ms | 1 KB | 3.2 MB | 10 |
| Python | typical code | comp | 1200 | 29494 | 78 µs | 170 µs | 9 KB | 10 KB | 1 |
| Python | typical code | comp | 12000 | 294167 | 742 µs | 1.60 ms | 66 KB | 66 KB | 1 |
| Python | typical code | comp | 60000 | 1470512 | 1.27 ms | 2.75 ms | 130 KB | 130 KB | 1 |
| Python | typical code | cmop | 1200 | 29494 | 82 µs | 161 µs | 1 KB | 2 KB | 1 |
| Python | typical code | cmop | 12000 | 294167 | 639 µs | 1.49 ms | 1 KB | 2 KB | 1 |
| Python | typical code | cmop | 60000 | 1470512 | 1.11 ms | 2.55 ms | 1 KB | 2 KB | 1 |
| Python | distinct words | comp | 1200 | 18026 | 59 µs | 60 µs | 67 KB | 67 KB | 0 |
| Python | distinct words | comp | 12000 | 180026 | 405 µs | 416 µs | 291 KB | 291 KB | 0 |
| Python | distinct words | comp | 60000 | 900026 | 1.20 ms | 1.12 ms | 1.0 MB | 1.0 MB | 0 |
| Python | distinct words | cmop | 1200 | 18026 | 35 µs | 398 µs | 1 KB | 168 KB | 10 |
| Python | distinct words | cmop | 12000 | 180026 | 349 µs | 3.73 ms | 1 KB | 1.5 MB | 10 |
| Python | distinct words | cmop | 60000 | 900026 | 973 µs | 10.04 ms | 1 KB | 3.7 MB | 10 |
| Cpp | typical code | comp | 1200 | 32804 | 88 µs | 192 µs | 9 KB | 10 KB | 1 |
| Cpp | typical code | comp | 12000 | 325951 | 823 µs | 1.79 ms | 66 KB | 66 KB | 1 |
| Cpp | typical code | comp | 60000 | 1629061 | 1.30 ms | 2.74 ms | 130 KB | 130 KB | 1 |
| Cpp | typical code | cmop | 1200 | 32804 | 75 µs | 180 µs | 1 KB | 2 KB | 1 |
| Cpp | typical code | cmop | 12000 | 325951 | 720 µs | 1.68 ms | 1 KB | 2 KB | 1 |
| Cpp | typical code | cmop | 60000 | 1629061 | 1.11 ms | 2.60 ms | 1 KB | 2 KB | 1 |
| Cpp | distinct words | comp | 1200 | 24036 | 88 µs | 88 µs | 67 KB | 67 KB | 0 |
| Cpp | distinct words | comp | 12000 | 240036 | 682 µs | 680 µs | 291 KB | 291 KB | 0 |
| Cpp | distinct words | comp | 60000 | 1200036 | 1.38 ms | 1.38 ms | 547 KB | 547 KB | 0 |
| Cpp | distinct words | cmop | 1200 | 24036 | 53 µs | 376 µs | 1 KB | 168 KB | 10 |
| Cpp | distinct words | cmop | 12000 | 240036 | 500 µs | 4.10 ms | 1 KB | 1.5 MB | 10 |
| Cpp | distinct words | cmop | 60000 | 1200036 | 1.04 ms | 7.75 ms | 1 KB | 3.2 MB | 10 |

**Analysis.**

- The slowest session with the second stage forced at 60000 lines is Python, distinct words, typed cmop: 10.04 ms (the frame budget is 16 ms).
- Allocated is what the session allocates (the same meaning as E05): the difference between forced and off is the cost of the second stage; no string is created for a word that is too far, only a small record to skip its next occurrences.
- In the typical file there are few distinct words and nearly all of them fail the first-letter filter; in the file of distinct words every word is a candidate and, for `cmop`, every one is one edit away.

**Criterion met.** The second stage needs no cache of the distinct words and no index.

*Ran in 13.7 s.*

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

*Ran in 1.6 s.*

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
| JavaScript | 1200 | 58758 | 399 µs | 403 µs |
| JavaScript | 12000 | 577746 | 3.15 ms | 3.15 ms |
| JavaScript | 60000 | 2875519 | 3.59 ms | 3.71 ms |
| CSharp | 1200 | 68232 | 431 µs | 434 µs |
| CSharp | 12000 | 670891 | 2.99 ms | 2.99 ms |
| CSharp | 60000 | 3339512 | 3.47 ms | 3.39 ms |
| Python | 1200 | 59971 | 421 µs | 422 µs |
| Python | 12000 | 588954 | 3.37 ms | 3.32 ms |
| Python | 60000 | 2931279 | 3.68 ms | 3.82 ms |
| Cpp | 1200 | 59936 | 388 µs | 387 µs |
| Cpp | 12000 | 588276 | 3.02 ms | 3.01 ms |
| Cpp | 60000 | 2927538 | 3.35 ms | 3.47 ms |

**Analysis.**

- The gain over all the reachable cases is +13.0 points within the first 5; the worst language changes by +1.4 points.
- The slowest session with the previous word at the largest size is Python at 60000 lines: 3.82 ms (the frame budget is 16 ms).
- The corpus is generated. It has the structure the idea looks for (the values of a property, the table after `from`), because the generator was written with it; real code may repeat its structure more or less than that.

**Criterion met.** Keep the previous word on.

*Ran in 38.3 s.*

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
| JavaScript | 1200 | 58758 | 395 µs | 430 µs | 408 µs | 427 µs |
| JavaScript | 12000 | 577746 | 3.22 ms | 3.44 ms | 3.21 ms | 3.43 ms |
| JavaScript | 60000 | 2875519 | 3.87 ms | 4.86 ms | 3.76 ms | 4.99 ms |
| CSharp | 1200 | 68232 | 439 µs | 461 µs | 438 µs | 460 µs |
| CSharp | 12000 | 670891 | 3.03 ms | 3.23 ms | 3.01 ms | 3.30 ms |
| CSharp | 60000 | 3339512 | 3.56 ms | 4.61 ms | 3.48 ms | 4.48 ms |
| Python | 1200 | 59971 | 415 µs | 442 µs | 417 µs | 436 µs |
| Python | 12000 | 588954 | 3.27 ms | 3.48 ms | 3.27 ms | 3.46 ms |
| Python | 60000 | 2931279 | 3.64 ms | 4.63 ms | 3.69 ms | 4.66 ms |
| Cpp | 1200 | 59936 | 379 µs | 395 µs | 389 µs | 403 µs |
| Cpp | 12000 | 588276 | 2.98 ms | 3.14 ms | 3.02 ms | 3.15 ms |
| Cpp | 60000 | 2927538 | 3.32 ms | 4.01 ms | 3.32 ms | 4.01 ms |

**Analysis.**

- The words of the language alone move the share within the first 5 from 72.5% to 77.3%; added to the previous word, +2.6 points; the worst language changes by +1.1 points.
- The slowest session with both at the largest size is JavaScript at 60000 lines: 4.99 ms (the frame budget is 16 ms).
- The corpus is generated: its host variables are named after the same nouns as the tables, so a host word with the same first letters is common by construction. How often real code has that is not measured.

**Criterion met.** Keep the words of the language first.

*Ran in 73.9 s.*

## E30: Does the place in the grammar help to rank the suggestions?

**Hypothesis.** What belongs at the caret can be told from a few characters of look-behind: a table after `from`, a column after `select`, `by` after `group`, the properties inside the braces of CSS and the values of the property after its colon, the attributes of the tag inside `<button `. Putting what belongs first and what does not last puts the meant word in the first 5 more often than the previous word and the language alone, in every language that has a grammar, and it fits in a frame.

**Method.** E28's probes (50 generated files with structure, words typed with 1 to 3 letters, the list the editor gets). Four variants: the order by distance alone; the grammar; the previous word and the language (what the plugin ran with before this experiment); all three. Reported by the place of the caret. Also the start of a session on files of 1,200 to 60,000 lines.

**Criterion.** Adding the grammar to the previous word and the language is at least 2 points better within the first 5 over all the reachable cases; SQL, CSS and HTML each do not fall; and the session with all three stays under 16 ms at 60,000 lines, in every host.

**The word typed with 1 to 3 letters: 9564 typed prefixes, 9165 reachable (95.8%)**

| Variant | Word first | Within the first 5 | Within the first 10 | Mean reciprocal rank |
|---|---|---|---|---|
| By distance alone | 36.8% | 72.5% | 86.0% | 0.528 |
| Grammar | 63.9% | 88.5% | 96.4% | 0.743 |
| Previous word and language | 70.6% | 88.1% | 94.9% | 0.788 |
| All three | 83.8% | 97.8% | 99.3% | 0.898 |

**Within the first 5, by language of the string**

| Language | Cases | By distance alone | Grammar | Previous word and language | All three |
|---|---|---|---|---|---|
| css | 732 | 74.3% | 87.4% | 81.8% | 89.6% |
| graphql | 657 | 88.7% | 88.7% | 96.0% | 96.0% |
| html | 1128 | 61.1% | 80.7% | 77.4% | 94.9% |
| sql | 6648 | 72.7% | 89.9% | 89.8% | 99.4% |

**Within the first 5, by number of letters typed**

| Letters | Cases | By distance alone | Grammar | Previous word and language | All three |
|---|---|---|---|---|---|
| 1 | 3055 | 39.1% | 76.9% | 69.2% | 94.9% |
| 2 | 3055 | 83.4% | 92.3% | 95.7% | 99.1% |
| 3 | 3055 | 95.0% | 96.3% | 99.2% | 99.4% |

**Within the first 5, by the place of the caret in the grammar**

| Place | Cases | By distance alone | Grammar | Previous word and language | All three |
|---|---|---|---|---|---|
| `sql:expression` | 1296 | 66.7% | 87.3% | 81.1% | 99.3% |
| `(none)` | 903 | 91.8% | 91.8% | 97.1% | 97.1% |
| `sql:statement` | 831 | 84.4% | 100.0% | 84.4% | 100.0% |
| `sql:table` | 819 | 38.6% | 60.0% | 86.7% | 98.4% |
| `sql:column-name` | 720 | 61.1% | 89.9% | 88.2% | 97.9% |
| `sql:column-type` | 642 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:member` | 531 | 57.8% | 81.9% | 92.1% | 99.2% |
| `html:attribute` | 360 | 67.2% | 99.7% | 75.8% | 99.7% |
| `html:value` | 345 | 39.7% | 53.0% | 73.3% | 93.3% |
| `css:property` | 333 | 80.2% | 86.5% | 80.5% | 86.5% |
| `css:value` | 312 | 78.8% | 100.0% | 93.6% | 100.0% |
| `sql:continue-select` | 285 | 66.7% | 100.0% | 88.4% | 100.0% |
| `sql:continue-from` | 219 | 100.0% | 100.0% | 100.0% | 100.0% |
| `html:tag` | 216 | 74.5% | 74.5% | 84.7% | 84.7% |
| `html:closing-tag` | 207 | 72.0% | 100.0% | 79.2% | 100.0% |
| `sql:continue-into` | 183 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:after-select` | 165 | 64.8% | 92.7% | 84.8% | 100.0% |
| `sql:continue-on` | 132 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:column-constraint` | 126 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:after-create` | 123 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:after-insert` | 117 | 33.3% | 100.0% | 100.0% | 100.0% |
| `sql:continue-where` | 87 | 100.0% | 100.0% | 100.0% | 100.0% |
| `sql:after-not` | 72 | 100.0% | 100.0% | 100.0% | 100.0% |
| `css:class` | 72 | 29.2% | 37.5% | 40.3% | 59.7% |
| `sql:continue-order` | 54 | 66.7% | 100.0% | 77.8% | 100.0% |
| `css:selector` | 15 | 66.7% | 86.7% | 66.7% | 86.7% |

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
| JavaScript | 1200 | 58758 | 398 µs | 401 µs | 418 µs | 417 µs |
| JavaScript | 12000 | 577746 | 3.14 ms | 3.15 ms | 3.34 ms | 3.35 ms |
| JavaScript | 60000 | 2875519 | 3.56 ms | 3.53 ms | 4.56 ms | 4.52 ms |
| CSharp | 1200 | 68232 | 426 µs | 431 µs | 437 µs | 443 µs |
| CSharp | 12000 | 670891 | 2.96 ms | 2.95 ms | 3.14 ms | 3.14 ms |
| CSharp | 60000 | 3339512 | 3.31 ms | 3.33 ms | 4.35 ms | 4.34 ms |
| Python | 1200 | 59971 | 412 µs | 414 µs | 431 µs | 432 µs |
| Python | 12000 | 588954 | 3.24 ms | 3.24 ms | 3.44 ms | 3.43 ms |
| Python | 60000 | 2931279 | 3.65 ms | 3.64 ms | 4.64 ms | 4.66 ms |
| Cpp | 1200 | 59936 | 383 µs | 384 µs | 398 µs | 401 µs |
| Cpp | 12000 | 588276 | 2.97 ms | 2.99 ms | 3.12 ms | 3.14 ms |
| Cpp | 60000 | 2927538 | 3.31 ms | 3.31 ms | 3.99 ms | 4.00 ms |

**Analysis.**

- The grammar alone moves the share within the first 5 from 72.5% to 88.5%; added to the previous word and the language, +9.8 points; the worst of SQL, CSS and HTML changes by +7.8 points.
- The slowest session with all three at the largest size is Python at 60000 lines: 4.66 ms (the frame budget is 16 ms).
- The corpus is generated by the same person who wrote the rules of the grammar, and it follows them: the attributes of a tag are the ones in the table of attributes, the values of a property the ones in the table of values. A real file will have words the tables do not know, and the grammar then costs nothing but gains nothing. This experiment says the rules do not get in the way of code that follows them, not how often real code does.

**Criterion met.** Keep the grammar on.

*Ran in 71.5 s.*

## E33: Completion with the context rankings on incomplete and cut code

**Hypothesis.** E27 again, with every context feature on (the previous word, the words of the language, the grammar of SQL, CSS and HTML, the schema of the SQL, the blend of count and distance) over the structured files, which have the statements, the rules and the tags those features read. The look-behind of the grammar and of the schema, the pointers into the ranges of the strings, the ranked words and the short words offered after a context must not throw on a text cut anywhere, must not offer a word twice, must not break the limit, and the exact suggestions must still start with what was typed.

**Method.** E27's test (every prefix cut at a stride, every single-character deletion at a stride, the caret at the end, at the start and at 5 random positions, and the same position with a mistake in the word), over 50 structured files, with every feature of CompletionFeatures on.

**Criterion.** Zero violations.

**Checks**

| Check | Value |
|---|---|
| Files | 50 |
| Texts (cuts and deletions) | 12269 |
| Carets tried | 85633 |
| Carets inside embedded code | 35116 |
| Of them, with a mistake put in the word | 16841 |
| Similar items checked against the definition | 29806 |
| Violations | 0 |

**Analysis.**

- 35116 of the 85633 carets were inside the code of an embedded string and went through Suggest; 16841 more with a mistake in the word.

**Criterion met.** Completion can be triggered anywhere in a file being edited, with the context rankings on.

*Ran in 3.8 s.*

## E34: Where the place of the caret says nothing, do the words of the file and the most used keywords come first?

**Hypothesis.** A review of 800 suggestions found that, without a rule for the place, the list is the vocabulary in alphabetical order and cut at 100, with the words of the file after it: with nothing or one letter typed the word the person wants is often out of the first five (or out of the list). Putting the words of the file first, and the keywords in the order of how much code uses them, puts it among the first five more often.

**Method.** A corpus of 500 snippets for each of the 8 languages, written by generators with the idioms of application code (SQL over a schema, CSS components, HTML templates, GraphQL, JSON and YAML configuration, GLSL and WGSL shaders), in 50 files of 10 snippets each. The even files are for learning the order of the keywords (how many times each is used), the odd files for measuring: 600 words per language typed with no letter (a request with Ctrl+Space) and with one letter. A second test on the hand-written files of the review of 800 suggestions, which come from another source. Four variants of the engine the plugin runs: as it is; the words of the file before the keywords; the keywords by use; both.

**Criterion.** Over the words typed with 0 or 1 letter, the best of the three variants is at least 3 points better than the current engine within the first 5, on the test files; no language is worse by more than 1 point; and on the hand-written files it is not worse than the current engine.

**The test files: 1077 requests with no letter and 1077 with one letter (words that exist in the file or are keywords)**

| Variant | No letter: first | No letter: first 5 | One letter: first | One letter: first 5 | Both: first 5 | Mean reciprocal rank |
|---|---|---|---|---|---|---|
| As it is | 42.9% | 60.2% | 67.9% | 87.7% | 73.9% | 0.640 |
| Words of the file first | 42.8% | 61.5% | 69.2% | 89.7% | 75.6% | 0.652 |
| Keywords by use | 43.6% | 62.0% | 69.8% | 90.0% | 76.0% | 0.657 |
| Both | 42.9% | 61.7% | 69.7% | 90.4% | 76.1% | 0.656 |

**Within the first 5, by language, test files**

| Language | Cases | As it is | Words of the file first | Keywords by use | Both |
|---|---|---|---|---|---|
| sql | 1032 | 76.7% | 77.0% | 77.6% | 77.3% |
| css | 1116 | 52.2% | 52.2% | 52.2% | 52.2% |
| html | 1032 | 85.9% | 85.9% | 88.6% | 88.6% |
| graphql | 976 | 60.9% | 59.8% | 60.9% | 59.8% |
| json | 992 | 75.2% | 73.2% | 75.2% | 73.2% |
| yaml | 1084 | 85.1% | 85.4% | 85.2% | 85.4% |
| glsl | 1190 | 71.8% | 81.3% | 78.9% | 82.0% |
| wgsl | 1198 | 82.8% | 87.7% | 87.8% | 87.7% |

**The hand-written files of the review (another source)**

| Variant | No letter: first | No letter: first 5 | One letter: first | One letter: first 5 | Both: first 5 | Mean reciprocal rank |
|---|---|---|---|---|---|---|
| As it is | 24.4% | 39.4% | 50.8% | 77.1% | 58.3% | 0.472 |
| Words of the file first | 24.1% | 43.6% | 57.2% | 87.7% | 65.6% | 0.521 |
| Keywords by use | 26.2% | 47.5% | 58.3% | 83.8% | 65.6% | 0.528 |
| Both | 24.4% | 44.6% | 58.8% | 91.8% | 68.2% | 0.535 |

**Within the first 5, by language, hand-written files**

| Language | Cases | As it is | Words of the file first | Keywords by use | Both |
|---|---|---|---|---|---|
| sql | 570 | 76.0% | 76.8% | 77.7% | 78.2% |
| css | 168 | 43.5% | 43.5% | 43.5% | 43.5% |
| html | 336 | 80.7% | 80.7% | 85.1% | 85.1% |
| graphql | 140 | 55.0% | 65.0% | 61.4% | 65.7% |
| json | 32 | 81.2% | 50.0% | 81.2% | 50.0% |
| yaml | 108 | 63.0% | 64.8% | 63.9% | 64.8% |
| glsl | 276 | 23.2% | 51.1% | 46.4% | 55.8% |
| wgsl | 268 | 35.1% | 54.5% | 50.4% | 58.6% |

**Analysis.**

- On the test files the best variant is Both: +2.2 points within the first 5; the worst language changes by -2.0 points; on the hand-written files +9.9 points.
- The corpus is made by generators written by the same person who reads the result, with the idioms they know. Training and test files come from the same generators, so the order of the keywords by use is learned from the same distribution it is measured on; the hand-written files are the check that is not.
- A prior learned from generated code says which keywords this corpus uses, not which ones real projects use. The plugin could learn the order from the files of the user.

**Criterion not met.** Look at the tables by language: a language that gains and another that loses suggests an order for each.

*Ran in 2.3 s.*

## E35: Do a few keywords still come before the words of the file?

**Hypothesis.** E34 found that the words of the file first and the keywords by use gain in GLSL, WGSL and HTML but lose in JSON and GraphQL: in JSON the words of the file pushed `true`, `false` and `null` down, and they are the few keywords that are always right in a value. Putting only the few most used keywords of the language before the words of the file, and the others after them, keeps that gain and the loss goes away.

**Method.** E34's files, words and priors (the corpus of 500 snippets for each language, the even files for the order of the keywords, the odd files for measuring, and the hand-written files of the review as a second test). Variants: the engine as it is; words of the file first with the keywords by use (E34's best, 0 keywords in front); and the same with the 3, 6 and 12 most used keywords of the language in front of the words.

**Criterion.** The best of the variants with keywords in front is at least 3 points better than the engine as it is within the first 5, over the words typed with 0 or 1 letter on the test files; no language is worse by more than 1 point; and on the hand-written files it is not worse than the engine as it is.

**The test files**

| Variant | No letter: first | No letter: first 5 | One letter: first | One letter: first 5 | Both: first 5 | Mean reciprocal rank |
|---|---|---|---|---|---|---|
| As it is | 42.9% | 60.2% | 67.9% | 87.7% | 73.9% | 0.640 |
| Words first, 0 in front | 42.9% | 61.7% | 69.7% | 90.4% | 76.1% | 0.656 |
| 3 keywords in front | 43.6% | 62.0% | 69.6% | 90.3% | 76.2% | 0.658 |
| 6 keywords in front | 43.6% | 62.0% | 69.2% | 90.4% | 76.2% | 0.657 |
| 12 keywords in front | 43.6% | 62.0% | 70.9% | 91.8% | 76.9% | 0.664 |

**Within the first 5, by language, test files**

| Language | Cases | As it is | Words first, 0 in front | 3 keywords in front | 6 keywords in front | 12 keywords in front |
|---|---|---|---|---|---|---|
| sql | 1032 | 76.7% | 77.3% | 77.6% | 77.5% | 77.6% |
| css | 1116 | 52.2% | 52.2% | 52.2% | 52.2% | 52.2% |
| html | 1032 | 85.9% | 88.6% | 88.6% | 88.6% | 88.6% |
| graphql | 976 | 60.9% | 59.8% | 57.7% | 55.8% | 61.8% |
| json | 992 | 75.2% | 73.2% | 75.2% | 75.2% | 75.2% |
| yaml | 1084 | 85.1% | 85.4% | 85.3% | 85.2% | 85.2% |
| glsl | 1190 | 71.8% | 82.0% | 83.1% | 82.4% | 82.6% |
| wgsl | 1198 | 82.8% | 87.7% | 87.4% | 89.9% | 89.9% |

**The hand-written files of the review (another source)**

| Variant | No letter: first | No letter: first 5 | One letter: first | One letter: first 5 | Both: first 5 | Mean reciprocal rank |
|---|---|---|---|---|---|---|
| As it is | 24.4% | 39.4% | 50.8% | 77.1% | 58.3% | 0.472 |
| Words first, 0 in front | 24.4% | 44.6% | 58.8% | 91.8% | 68.2% | 0.535 |
| 3 keywords in front | 26.2% | 47.1% | 60.1% | 91.9% | 69.5% | 0.548 |
| 6 keywords in front | 26.2% | 47.5% | 60.0% | 91.7% | 69.6% | 0.549 |
| 12 keywords in front | 26.2% | 47.5% | 61.3% | 91.4% | 69.4% | 0.553 |

**Within the first 5, by language, hand-written files**

| Language | Cases | As it is | Words first, 0 in front | 3 keywords in front | 6 keywords in front | 12 keywords in front |
|---|---|---|---|---|---|---|
| sql | 570 | 76.0% | 78.2% | 78.6% | 78.6% | 78.6% |
| css | 168 | 43.5% | 43.5% | 43.5% | 43.5% | 43.5% |
| html | 336 | 80.7% | 85.1% | 85.1% | 85.1% | 85.1% |
| graphql | 140 | 55.0% | 65.7% | 65.7% | 62.1% | 62.1% |
| json | 32 | 81.2% | 50.0% | 81.2% | 81.2% | 81.2% |
| yaml | 108 | 63.0% | 64.8% | 64.8% | 63.9% | 63.9% |
| glsl | 276 | 23.2% | 55.8% | 60.1% | 62.0% | 60.9% |
| wgsl | 268 | 35.1% | 58.6% | 59.0% | 60.1% | 60.1% |

**Analysis.**

- On the test files the best variant is 12 keywords in front: +3.0 points within the first 5; the worst language changes by 0.0 points; on the hand-written files +11.2 points.
- E35 was written after seeing E34, whose criterion was not met: the variants here are a reaction to its tables, not a prediction made before them. The same test files are used, so a gain here is partly fitted to them; the hand-written files are the check.

**Criterion met.** Adopt the variant: the words of the file first where no rule decides, with that many keywords in front, in the order of use.

*Ran in 2.9 s.*

## Limits

- Synthetic code, not real files.
- The experiments run in a console process with a small heap, away from the Visual Studio UI thread and its GC pressure. They compare versions of the code: they are not times the user will see.
- Times are medians of repeated runs on one machine; differences of a few percent are noise.
