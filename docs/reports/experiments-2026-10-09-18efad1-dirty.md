# NestLight experiments report

| | |
|---|---|
| Date | 2026-10-09 01:16 UTC |
| Commit | `18efad1` (**uncommitted changes**: this run cannot be reproduced from the commit) |
| Runtime | .NET 10.0.0, Release |
| Machine | CPU not identified, 16 logical cores |
| OS | Linux Mint 22 |
| Mode | full |

Definitions of the experiments: [docs/experiments.md](../experiments.md). Each result is valid only for the commit and the machine above.

## Summary

| Id | Experiment | Criterion | Finding |
|---|---|---|---|
| E19 | Does the tokenizer agree with the vocabulary? | Met | vocabulary and tokenizers agree |
| E22 | Sharing the scan and not creating the words of the completion | Met | worst case with the scan shared 1.39 ms at 60000 lines |
| E23 | Does the second stage of the completion fit in a frame? | Met | worst case with the second stage forced 10.70 ms at 60000 lines |
| E24 | Does the second stage recover the word after one mistake, and which tie-break works? | Met | best tie-break (nearest to the caret (the engine)) 72.0% within the first 5 |
| E25 | Does the second stage get in the way when the prefix is right? | Met | with FuzzyBelow = 1 similar items are added in 1.1% of the cases |
| E27 | Completion with similar words on incomplete and cut code | Met | 0 violations in 86900 carets, 21275 similar items checked |
| E28 | Does the word before the caret help to rank the suggestions? | Met | within the first 5: 72.5% to 85.5% (+13.0 points); worst session 3.64 ms |
| E29 | Do the words of the same language come first? | Met | within the first 5: previous word 85.5%, with the language 88.1% (+2.6 points); worst session 4.70 ms |
| E30 | Does the place in the grammar help to rank the suggestions? | Met | within the first 5: 88.1% to 97.8% (+9.8 points); worst session 4.62 ms |
| E33 | Completion with the context rankings on incomplete and cut code | Met | 0 violations in 85633 carets, 29806 similar items checked |

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
| JavaScript | typical code | 1200 | 41771 | 369 µs | 113 µs | 109 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 3.07 ms | 1.01 ms | 1.05 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 11.45 ms | 1.25 ms | 1.21 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 162 µs | 110 µs | 87 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 1.41 ms | 734 µs | 684 µs | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 6.09 ms | 1.39 ms | 1.36 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 284 µs | 92 µs | 90 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 2.88 ms | 869 µs | 876 µs | 1 |
| CSharp | typical code | 60000 | 1748126 | 11.41 ms | 1.25 ms | 1.24 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 175 µs | 112 µs | 88 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 1.55 ms | 714 µs | 671 µs | 100 |
| CSharp | distinct words | 60000 | 1200032 | 6.18 ms | 1.39 ms | 1.41 ms | 100 |
| Python | typical code | 1200 | 29494 | 398 µs | 78 µs | 76 µs | 1 |
| Python | typical code | 12000 | 294167 | 4.05 ms | 744 µs | 729 µs | 1 |
| Python | typical code | 60000 | 1470512 | 18.14 ms | 1.26 ms | 1.21 ms | 1 |
| Python | distinct words | 1200 | 18026 | 200 µs | 58 µs | 57 µs | 100 |
| Python | distinct words | 12000 | 180026 | 1.81 ms | 432 µs | 397 µs | 100 |
| Python | distinct words | 60000 | 900026 | 8.15 ms | 1.39 ms | 1.11 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 253 µs | 88 µs | 87 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 2.42 ms | 824 µs | 817 µs | 1 |
| Cpp | typical code | 60000 | 1629061 | 9.59 ms | 1.28 ms | 1.28 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 187 µs | 114 µs | 85 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 1.63 ms | 722 µs | 683 µs | 100 |
| Cpp | distinct words | 60000 | 1200036 | 6.15 ms | 1.37 ms | 1.38 ms | 100 |

**Analysis.**

- The slowest session with the scan shared at 60000 lines is CSharp, distinct words: 1.39 ms (the frame budget is 16 ms).
- "Nothing shared" is the start of a session as the editor makes it without the shared scan: Locate twice and Suggest. E16 measured one Locate and Suggest.

**Criterion met.** Completion fits in a frame on any file the classifier can handle.

*Ran in 7.4 s.*

## E23: Does the second stage of the completion fit in a frame?

**Hypothesis.** The second stage compares what was typed with every distinct word of the document that passes two cheap filters, and keeps the best few. Even forced to run in every session, with thousands of words that are one edit away, one session still fits in the 16 ms of a frame, as E22 showed for the first stage.

**Method.** E22's files and session (Locate, Locate again and Suggest, over a new text instance, the classifier having highlighted it first), 4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines, with the second stage forced to run (it is asked for below any number of items). Two things typed: the exact prefix `comp` (the second stage compares everything and finds nothing new) and `cmop`, two letters swapped (in the distinct-words file, every one of the words is one edit away). Also the same session with the second stage off.

**Criterion.** The session with the second stage forced stays under 16 ms at 60,000 lines, in every host, shape and typed text.

**Median time of the start of one completion session (the scan shared)**

| Host | Shape | Typed | Lines | Characters | Second stage off | Second stage forced | Allocated off | Allocated forced | Similar items |
|---|---|---|---|---|---|---|---|---|---|
| JavaScript | typical code | comp | 1200 | 41771 | 113 µs | 235 µs | 9 KB | 10 KB | 1 |
| JavaScript | typical code | comp | 12000 | 417611 | 1.01 ms | 2.25 ms | 66 KB | 66 KB | 1 |
| JavaScript | typical code | comp | 60000 | 2088011 | 1.23 ms | 2.72 ms | 66 KB | 66 KB | 1 |
| JavaScript | typical code | cmop | 1200 | 41771 | 94 µs | 224 µs | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 12000 | 417611 | 913 µs | 2.16 ms | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 60000 | 2088011 | 1.10 ms | 2.61 ms | 1 KB | 2 KB | 1 |
| JavaScript | distinct words | comp | 1200 | 24011 | 113 µs | 112 µs | 67 KB | 67 KB | 0 |
| JavaScript | distinct words | comp | 12000 | 240011 | 688 µs | 680 µs | 291 KB | 291 KB | 0 |
| JavaScript | distinct words | comp | 60000 | 1200011 | 1.43 ms | 1.37 ms | 547 KB | 547 KB | 0 |
| JavaScript | distinct words | cmop | 1200 | 24011 | 52 µs | 451 µs | 1 KB | 168 KB | 10 |
| JavaScript | distinct words | cmop | 12000 | 240011 | 497 µs | 4.28 ms | 1 KB | 1.5 MB | 10 |
| JavaScript | distinct words | cmop | 60000 | 1200011 | 1.02 ms | 8.96 ms | 1 KB | 3.2 MB | 10 |
| CSharp | typical code | comp | 1200 | 35058 | 92 µs | 199 µs | 9 KB | 10 KB | 1 |
| CSharp | typical code | comp | 12000 | 349703 | 868 µs | 1.86 ms | 66 KB | 66 KB | 1 |
| CSharp | typical code | comp | 60000 | 1748126 | 1.25 ms | 2.67 ms | 66 KB | 66 KB | 1 |
| CSharp | typical code | cmop | 1200 | 35058 | 80 µs | 187 µs | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 12000 | 349703 | 768 µs | 1.76 ms | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 60000 | 1748126 | 1.12 ms | 2.53 ms | 1 KB | 2 KB | 1 |
| CSharp | distinct words | comp | 1200 | 24032 | 87 µs | 87 µs | 67 KB | 67 KB | 0 |
| CSharp | distinct words | comp | 12000 | 240032 | 685 µs | 690 µs | 291 KB | 291 KB | 0 |
| CSharp | distinct words | comp | 60000 | 1200032 | 1.37 ms | 1.39 ms | 547 KB | 547 KB | 0 |
| CSharp | distinct words | cmop | 1200 | 24032 | 52 µs | 444 µs | 1 KB | 168 KB | 10 |
| CSharp | distinct words | cmop | 12000 | 240032 | 490 µs | 4.30 ms | 1 KB | 1.5 MB | 10 |
| CSharp | distinct words | cmop | 60000 | 1200032 | 1.02 ms | 8.87 ms | 1 KB | 3.2 MB | 10 |
| Python | typical code | comp | 1200 | 29494 | 78 µs | 171 µs | 9 KB | 10 KB | 1 |
| Python | typical code | comp | 12000 | 294167 | 729 µs | 1.57 ms | 66 KB | 66 KB | 1 |
| Python | typical code | comp | 60000 | 1470512 | 1.26 ms | 2.69 ms | 130 KB | 130 KB | 1 |
| Python | typical code | cmop | 1200 | 29494 | 68 µs | 158 µs | 1 KB | 2 KB | 1 |
| Python | typical code | cmop | 12000 | 294167 | 636 µs | 1.48 ms | 1 KB | 2 KB | 1 |
| Python | typical code | cmop | 60000 | 1470512 | 1.10 ms | 2.53 ms | 1 KB | 2 KB | 1 |
| Python | distinct words | comp | 1200 | 18026 | 59 µs | 59 µs | 67 KB | 67 KB | 0 |
| Python | distinct words | comp | 12000 | 180026 | 398 µs | 396 µs | 291 KB | 291 KB | 0 |
| Python | distinct words | comp | 60000 | 900026 | 1.16 ms | 1.07 ms | 1.0 MB | 1.0 MB | 0 |
| Python | distinct words | cmop | 1200 | 18026 | 34 µs | 347 µs | 1 KB | 168 KB | 10 |
| Python | distinct words | cmop | 12000 | 180026 | 316 µs | 3.92 ms | 1 KB | 1.5 MB | 10 |
| Python | distinct words | cmop | 60000 | 900026 | 844 µs | 10.70 ms | 1 KB | 3.7 MB | 10 |
| Cpp | typical code | comp | 1200 | 32804 | 91 µs | 192 µs | 9 KB | 10 KB | 1 |
| Cpp | typical code | comp | 12000 | 325951 | 818 µs | 1.77 ms | 66 KB | 66 KB | 1 |
| Cpp | typical code | comp | 60000 | 1629061 | 1.28 ms | 2.75 ms | 130 KB | 130 KB | 1 |
| Cpp | typical code | cmop | 1200 | 32804 | 76 µs | 178 µs | 1 KB | 2 KB | 1 |
| Cpp | typical code | cmop | 12000 | 325951 | 720 µs | 1.68 ms | 1 KB | 2 KB | 1 |
| Cpp | typical code | cmop | 60000 | 1629061 | 1.12 ms | 2.60 ms | 1 KB | 2 KB | 1 |
| Cpp | distinct words | comp | 1200 | 24036 | 84 µs | 81 µs | 67 KB | 67 KB | 0 |
| Cpp | distinct words | comp | 12000 | 240036 | 681 µs | 692 µs | 291 KB | 291 KB | 0 |
| Cpp | distinct words | comp | 60000 | 1200036 | 1.40 ms | 1.55 ms | 547 KB | 547 KB | 0 |
| Cpp | distinct words | cmop | 1200 | 24036 | 52 µs | 444 µs | 1 KB | 168 KB | 10 |
| Cpp | distinct words | cmop | 12000 | 240036 | 496 µs | 4.29 ms | 1 KB | 1.5 MB | 10 |
| Cpp | distinct words | cmop | 60000 | 1200036 | 1.02 ms | 8.20 ms | 1 KB | 3.2 MB | 10 |

**Analysis.**

- The slowest session with the second stage forced at 60000 lines is Python, distinct words, typed cmop: 10.70 ms (the frame budget is 16 ms).
- Allocated is what the session allocates (the same meaning as E05): the difference between forced and off is the cost of the second stage; no string is created for a word that is too far, only a small record to skip its next occurrences.
- In the typical file there are few distinct words and nearly all of them fail the first-letter filter; in the file of distinct words every word is a candidate and, for `cmop`, every one is one edit away.

**Criterion met.** The second stage needs no cache of the distinct words and no index.

*Ran in 13.4 s.*

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

*Ran in 6.6 s.*

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
| JavaScript | 1200 | 58758 | 404 µs | 401 µs |
| JavaScript | 12000 | 577746 | 3.17 ms | 3.19 ms |
| JavaScript | 60000 | 2875519 | 3.63 ms | 3.59 ms |
| CSharp | 1200 | 68232 | 424 µs | 422 µs |
| CSharp | 12000 | 670891 | 2.95 ms | 2.96 ms |
| CSharp | 60000 | 3339512 | 3.35 ms | 3.34 ms |
| Python | 1200 | 59971 | 412 µs | 410 µs |
| Python | 12000 | 588954 | 3.27 ms | 3.31 ms |
| Python | 60000 | 2931279 | 3.63 ms | 3.64 ms |
| Cpp | 1200 | 59936 | 393 µs | 391 µs |
| Cpp | 12000 | 588276 | 2.99 ms | 2.98 ms |
| Cpp | 60000 | 2927538 | 3.31 ms | 3.30 ms |

**Analysis.**

- The gain over all the reachable cases is +13.0 points within the first 5; the worst language changes by +1.4 points.
- The slowest session with the previous word at the largest size is Python at 60000 lines: 3.64 ms (the frame budget is 16 ms).
- The corpus is generated. It has the structure the idea looks for (the values of a property, the table after `from`), because the generator was written with it; real code may repeat its structure more or less than that.

**Criterion met.** Keep the previous word on.

*Ran in 36.6 s.*

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
| JavaScript | 1200 | 58758 | 396 µs | 414 µs | 398 µs | 414 µs |
| JavaScript | 12000 | 577746 | 3.16 ms | 3.36 ms | 3.16 ms | 3.33 ms |
| JavaScript | 60000 | 2875519 | 3.54 ms | 4.49 ms | 3.53 ms | 4.52 ms |
| CSharp | 1200 | 68232 | 426 µs | 440 µs | 419 µs | 440 µs |
| CSharp | 12000 | 670891 | 2.94 ms | 3.13 ms | 2.95 ms | 3.15 ms |
| CSharp | 60000 | 3339512 | 3.32 ms | 4.33 ms | 3.36 ms | 4.30 ms |
| Python | 1200 | 59971 | 416 µs | 435 µs | 414 µs | 430 µs |
| Python | 12000 | 588954 | 3.25 ms | 3.48 ms | 3.24 ms | 3.44 ms |
| Python | 60000 | 2931279 | 3.63 ms | 4.59 ms | 3.67 ms | 4.70 ms |
| Cpp | 1200 | 59936 | 382 µs | 401 µs | 383 µs | 404 µs |
| Cpp | 12000 | 588276 | 3.01 ms | 3.12 ms | 2.99 ms | 3.13 ms |
| Cpp | 60000 | 2927538 | 3.31 ms | 4.08 ms | 3.42 ms | 3.96 ms |

**Analysis.**

- The words of the language alone move the share within the first 5 from 72.5% to 77.3%; added to the previous word, +2.6 points; the worst language changes by +1.1 points.
- The slowest session with both at the largest size is Python at 60000 lines: 4.70 ms (the frame budget is 16 ms).
- The corpus is generated: its host variables are named after the same nouns as the tables, so a host word with the same first letters is common by construction. How often real code has that is not measured.

**Criterion met.** Keep the words of the language first.

*Ran in 71.0 s.*

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
| JavaScript | 1200 | 58758 | 399 µs | 398 µs | 425 µs | 417 µs |
| JavaScript | 12000 | 577746 | 3.15 ms | 3.17 ms | 3.36 ms | 3.36 ms |
| JavaScript | 60000 | 2875519 | 3.52 ms | 3.53 ms | 4.47 ms | 4.59 ms |
| CSharp | 1200 | 68232 | 426 µs | 424 µs | 438 µs | 444 µs |
| CSharp | 12000 | 670891 | 2.95 ms | 2.95 ms | 3.15 ms | 3.14 ms |
| CSharp | 60000 | 3339512 | 3.30 ms | 3.33 ms | 4.36 ms | 4.29 ms |
| Python | 1200 | 59971 | 411 µs | 413 µs | 430 µs | 431 µs |
| Python | 12000 | 588954 | 3.26 ms | 3.27 ms | 3.50 ms | 3.51 ms |
| Python | 60000 | 2931279 | 3.71 ms | 3.57 ms | 4.61 ms | 4.62 ms |
| Cpp | 1200 | 59936 | 388 µs | 384 µs | 403 µs | 400 µs |
| Cpp | 12000 | 588276 | 2.99 ms | 2.99 ms | 3.15 ms | 3.12 ms |
| Cpp | 60000 | 2927538 | 3.29 ms | 3.31 ms | 3.98 ms | 3.96 ms |

**Analysis.**

- The grammar alone moves the share within the first 5 from 72.5% to 88.5%; added to the previous word and the language, +9.8 points; the worst of SQL, CSS and HTML changes by +7.8 points.
- The slowest session with all three at the largest size is Python at 60000 lines: 4.62 ms (the frame budget is 16 ms).
- The corpus is generated by the same person who wrote the rules of the grammar, and it follows them: the attributes of a tag are the ones in the table of attributes, the values of a property the ones in the table of values. A real file will have words the tables do not know, and the grammar then costs nothing but gains nothing. This experiment says the rules do not get in the way of code that follows them, not how often real code does.

**Criterion met.** Keep the grammar on.

*Ran in 70.7 s.*

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

*Ran in 3.7 s.*

## Limits

- Synthetic code, not real files.
- The experiments run in a console process with a small heap, away from the Visual Studio UI thread and its GC pressure. They compare versions of the code: they are not times the user will see.
- Times are medians of repeated runs on one machine; differences of a few percent are noise.
