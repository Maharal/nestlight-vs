# NestLight experiments report

| | |
|---|---|
| Date | 2026-10-09 01:56 UTC |
| Commit | `812de3c` (**uncommitted changes**: this run cannot be reproduced from the commit) |
| Runtime | .NET 10.0.0, Release |
| Machine | CPU not identified, 16 logical cores |
| OS | Linux Mint 22 |
| Mode | full |

Definitions of the experiments: [docs/experiments.md](../experiments.md). Each result is valid only for the commit and the machine above.

## Summary

| Id | Experiment | Criterion | Finding |
|---|---|---|---|
| E19 | Does the tokenizer agree with the vocabulary? | Met | vocabulary and tokenizers agree |
| E22 | Sharing the scan and not creating the words of the completion | Met | worst case with the scan shared 1.38 ms at 60000 lines |
| E23 | Does the second stage of the completion fit in a frame? | Met | worst case with the second stage forced 10.19 ms at 60000 lines |
| E24 | Does the second stage recover the word after one mistake, and which tie-break works? | Met | best tie-break (nearest to the caret (the engine)) 72.0% within the first 5 |
| E25 | Does the second stage get in the way when the prefix is right? | Met | with FuzzyBelow = 1 similar items are added in 1.1% of the cases |
| E27 | Completion with similar words on incomplete and cut code | Met | 0 violations in 86900 carets, 21275 similar items checked |
| E28 | Does the word before the caret help to rank the suggestions? | Met | within the first 5: 72.5% to 85.5% (+13.0 points); worst session 3.17 ms |
| E29 | Do the words of the same language come first? | Met | within the first 5: previous word 85.5%, with the language 88.1% (+2.6 points); worst session 4.23 ms |
| E30 | Does the place in the grammar help to rank the suggestions? | Met | within the first 5: 88.1% to 97.8% (+9.7 points); worst session 4.24 ms |
| E33 | Completion with the context rankings on incomplete and cut code | Met | 0 violations in 85633 carets, 29806 similar items checked |
| E34 | Where the place of the caret says nothing, do the words of the file and the most used keywords come first? | Not met | best: Both within the first 5 74.5% to 76.6% (+2.1 points); hand-written +9.7 |
| E35 | Do a few keywords still come before the words of the file? | Not met | best: 12 keywords in front within the first 5 74.5% to 77.4% (+3.0 points); hand-written +11.0 |
| E36 | Should words of two letters be offered? | Not met | no variant meets the criterion |
| E37 | Does the similar-words stage make noise with short prefixes, and what removes it? | Not met | no variant meets the criterion |
| E38 | Should words of two letters be offered? (E36 with a criterion that can be met) | Met | adopt: Minimum 2, the words of 2 letters +10.9 points |

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
| JavaScript | typical code | 1200 | 41771 | 363 µs | 116 µs | 112 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 3.11 ms | 1.06 ms | 1.08 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 11.84 ms | 1.29 ms | 1.26 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 166 µs | 91 µs | 90 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 1.44 ms | 740 µs | 708 µs | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 4.96 ms | 1.34 ms | 1.33 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 281 µs | 98 µs | 95 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 2.92 ms | 937 µs | 924 µs | 1 |
| CSharp | typical code | 60000 | 1748126 | 11.30 ms | 1.35 ms | 1.31 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 178 µs | 119 µs | 90 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 1.52 ms | 720 µs | 715 µs | 100 |
| CSharp | distinct words | 60000 | 1200032 | 6.10 ms | 1.36 ms | 1.36 ms | 100 |
| Python | typical code | 1200 | 29494 | 408 µs | 82 µs | 80 µs | 1 |
| Python | typical code | 12000 | 294167 | 4.06 ms | 766 µs | 761 µs | 1 |
| Python | typical code | 60000 | 1470512 | 18.06 ms | 1.32 ms | 1.29 ms | 1 |
| Python | distinct words | 1200 | 18026 | 203 µs | 60 µs | 57 µs | 100 |
| Python | distinct words | 12000 | 180026 | 1.83 ms | 445 µs | 411 µs | 100 |
| Python | distinct words | 60000 | 900026 | 8.18 ms | 1.36 ms | 1.16 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 250 µs | 93 µs | 92 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 2.46 ms | 876 µs | 881 µs | 1 |
| Cpp | typical code | 60000 | 1629061 | 9.53 ms | 1.38 ms | 1.34 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 190 µs | 112 µs | 91 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 1.65 ms | 716 µs | 704 µs | 100 |
| Cpp | distinct words | 60000 | 1200036 | 6.15 ms | 1.38 ms | 1.39 ms | 100 |

**Analysis.**

- The slowest session with the scan shared at 60000 lines is Cpp, distinct words: 1.38 ms (the frame budget is 16 ms).
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
| JavaScript | typical code | comp | 1200 | 41771 | 117 µs | 213 µs | 10 KB | 10 KB | 1 |
| JavaScript | typical code | comp | 12000 | 417611 | 1.05 ms | 2.01 ms | 66 KB | 67 KB | 1 |
| JavaScript | typical code | comp | 60000 | 2088011 | 1.29 ms | 2.44 ms | 66 KB | 67 KB | 1 |
| JavaScript | typical code | cmop | 1200 | 41771 | 103 µs | 204 µs | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 12000 | 417611 | 964 µs | 1.92 ms | 1 KB | 2 KB | 1 |
| JavaScript | typical code | cmop | 60000 | 2088011 | 1.17 ms | 2.31 ms | 1 KB | 2 KB | 1 |
| JavaScript | distinct words | comp | 1200 | 24011 | 91 µs | 91 µs | 67 KB | 67 KB | 0 |
| JavaScript | distinct words | comp | 12000 | 240011 | 701 µs | 714 µs | 291 KB | 291 KB | 0 |
| JavaScript | distinct words | comp | 60000 | 1200011 | 1.37 ms | 1.34 ms | 547 KB | 547 KB | 0 |
| JavaScript | distinct words | cmop | 1200 | 24011 | 56 µs | 451 µs | 1 KB | 168 KB | 10 |
| JavaScript | distinct words | cmop | 12000 | 240011 | 522 µs | 4.03 ms | 1 KB | 1.5 MB | 10 |
| JavaScript | distinct words | cmop | 60000 | 1200011 | 1.09 ms | 8.71 ms | 1 KB | 3.2 MB | 10 |
| CSharp | typical code | comp | 1200 | 35058 | 97 µs | 174 µs | 10 KB | 10 KB | 1 |
| CSharp | typical code | comp | 12000 | 349703 | 938 µs | 1.64 ms | 66 KB | 66 KB | 1 |
| CSharp | typical code | comp | 60000 | 1748126 | 1.34 ms | 2.35 ms | 66 KB | 66 KB | 1 |
| CSharp | typical code | cmop | 1200 | 35058 | 87 µs | 166 µs | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 12000 | 349703 | 839 µs | 1.54 ms | 1 KB | 2 KB | 1 |
| CSharp | typical code | cmop | 60000 | 1748126 | 1.21 ms | 2.22 ms | 1 KB | 2 KB | 1 |
| CSharp | distinct words | comp | 1200 | 24032 | 91 µs | 91 µs | 67 KB | 67 KB | 0 |
| CSharp | distinct words | comp | 12000 | 240032 | 690 µs | 690 µs | 291 KB | 291 KB | 0 |
| CSharp | distinct words | comp | 60000 | 1200032 | 1.34 ms | 1.36 ms | 547 KB | 547 KB | 0 |
| CSharp | distinct words | cmop | 1200 | 24032 | 56 µs | 451 µs | 1 KB | 168 KB | 10 |
| CSharp | distinct words | cmop | 12000 | 240032 | 522 µs | 4.21 ms | 1 KB | 1.5 MB | 10 |
| CSharp | distinct words | cmop | 60000 | 1200032 | 1.09 ms | 8.14 ms | 1 KB | 3.2 MB | 10 |
| Python | typical code | comp | 1200 | 29494 | 81 µs | 150 µs | 10 KB | 10 KB | 1 |
| Python | typical code | comp | 12000 | 294167 | 766 µs | 1.38 ms | 66 KB | 66 KB | 1 |
| Python | typical code | comp | 60000 | 1470512 | 1.34 ms | 2.41 ms | 130 KB | 130 KB | 1 |
| Python | typical code | cmop | 1200 | 29494 | 74 µs | 144 µs | 1 KB | 2 KB | 1 |
| Python | typical code | cmop | 12000 | 294167 | 699 µs | 1.32 ms | 1 KB | 2 KB | 1 |
| Python | typical code | cmop | 60000 | 1470512 | 1.18 ms | 2.25 ms | 1 KB | 2 KB | 1 |
| Python | distinct words | comp | 1200 | 18026 | 60 µs | 60 µs | 67 KB | 67 KB | 0 |
| Python | distinct words | comp | 12000 | 180026 | 415 µs | 420 µs | 291 KB | 291 KB | 0 |
| Python | distinct words | comp | 60000 | 900026 | 1.14 ms | 1.37 ms | 1.0 MB | 1.0 MB | 0 |
| Python | distinct words | cmop | 1200 | 18026 | 38 µs | 367 µs | 1 KB | 168 KB | 10 |
| Python | distinct words | cmop | 12000 | 180026 | 337 µs | 3.76 ms | 1 KB | 1.5 MB | 10 |
| Python | distinct words | cmop | 60000 | 900026 | 928 µs | 10.19 ms | 1 KB | 3.7 MB | 10 |
| Cpp | typical code | comp | 1200 | 32804 | 94 µs | 171 µs | 10 KB | 10 KB | 1 |
| Cpp | typical code | comp | 12000 | 325951 | 892 µs | 1.61 ms | 66 KB | 66 KB | 1 |
| Cpp | typical code | comp | 60000 | 1629061 | 1.39 ms | 2.51 ms | 130 KB | 130 KB | 1 |
| Cpp | typical code | cmop | 1200 | 32804 | 82 µs | 160 µs | 1 KB | 2 KB | 1 |
| Cpp | typical code | cmop | 12000 | 325951 | 787 µs | 1.51 ms | 1 KB | 2 KB | 1 |
| Cpp | typical code | cmop | 60000 | 1629061 | 1.21 ms | 2.32 ms | 1 KB | 2 KB | 1 |
| Cpp | distinct words | comp | 1200 | 24036 | 93 µs | 93 µs | 67 KB | 67 KB | 0 |
| Cpp | distinct words | comp | 12000 | 240036 | 730 µs | 731 µs | 291 KB | 291 KB | 0 |
| Cpp | distinct words | comp | 60000 | 1200036 | 1.45 ms | 1.48 ms | 547 KB | 547 KB | 0 |
| Cpp | distinct words | cmop | 1200 | 24036 | 56 µs | 453 µs | 1 KB | 168 KB | 10 |
| Cpp | distinct words | cmop | 12000 | 240036 | 539 µs | 4.27 ms | 1 KB | 1.5 MB | 10 |
| Cpp | distinct words | cmop | 60000 | 1200036 | 1.11 ms | 8.59 ms | 1 KB | 3.2 MB | 10 |

**Analysis.**

- The slowest session with the second stage forced at 60000 lines is Python, distinct words, typed cmop: 10.19 ms (the frame budget is 16 ms).
- Allocated is what the session allocates (the same meaning as E05): the difference between forced and off is the cost of the second stage; no string is created for a word that is too far, only a small record to skip its next occurrences.
- In the typical file there are few distinct words and nearly all of them fail the first-letter filter; in the file of distinct words every word is a candidate and, for `cmop`, every one is one edit away.

**Criterion met.** The second stage needs no cache of the distinct words and no index.

*Ran in 13.1 s.*

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

*Ran in 5.3 s.*

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
| JavaScript | 1200 | 58758 | 347 µs | 344 µs |
| JavaScript | 12000 | 577746 | 2.72 ms | 2.73 ms |
| JavaScript | 60000 | 2875519 | 3.12 ms | 3.14 ms |
| CSharp | 1200 | 68232 | 367 µs | 369 µs |
| CSharp | 12000 | 670891 | 2.52 ms | 2.52 ms |
| CSharp | 60000 | 3339512 | 2.87 ms | 2.91 ms |
| Python | 1200 | 59971 | 357 µs | 358 µs |
| Python | 12000 | 588954 | 2.81 ms | 2.81 ms |
| Python | 60000 | 2931279 | 3.17 ms | 3.17 ms |
| Cpp | 1200 | 59936 | 329 µs | 335 µs |
| Cpp | 12000 | 588276 | 2.53 ms | 2.53 ms |
| Cpp | 60000 | 2927538 | 2.84 ms | 2.85 ms |

**Analysis.**

- The gain over all the reachable cases is +13.0 points within the first 5; the worst language changes by +1.4 points.
- The slowest session with the previous word at the largest size is Python at 60000 lines: 3.17 ms (the frame budget is 16 ms).
- The corpus is generated. It has the structure the idea looks for (the values of a property, the table after `from`), because the generator was written with it; real code may repeat its structure more or less than that.

**Criterion met.** Keep the previous word on.

*Ran in 35.8 s.*

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
| JavaScript | 1200 | 58758 | 348 µs | 361 µs | 341 µs | 364 µs |
| JavaScript | 12000 | 577746 | 2.71 ms | 2.89 ms | 2.70 ms | 2.89 ms |
| JavaScript | 60000 | 2875519 | 3.06 ms | 4.09 ms | 3.09 ms | 4.07 ms |
| CSharp | 1200 | 68232 | 363 µs | 386 µs | 365 µs | 379 µs |
| CSharp | 12000 | 670891 | 2.50 ms | 2.70 ms | 2.50 ms | 2.69 ms |
| CSharp | 60000 | 3339512 | 2.89 ms | 3.87 ms | 2.89 ms | 3.88 ms |
| Python | 1200 | 59971 | 362 µs | 380 µs | 361 µs | 379 µs |
| Python | 12000 | 588954 | 2.82 ms | 3.00 ms | 2.80 ms | 2.99 ms |
| Python | 60000 | 2931279 | 3.22 ms | 4.17 ms | 3.22 ms | 4.23 ms |
| Cpp | 1200 | 59936 | 329 µs | 345 µs | 338 µs | 344 µs |
| Cpp | 12000 | 588276 | 2.55 ms | 2.70 ms | 2.55 ms | 2.68 ms |
| Cpp | 60000 | 2927538 | 2.90 ms | 3.54 ms | 2.89 ms | 3.56 ms |

**Analysis.**

- The words of the language alone move the share within the first 5 from 72.5% to 77.3%; added to the previous word, +2.6 points; the worst language changes by +1.1 points.
- The slowest session with both at the largest size is Python at 60000 lines: 4.23 ms (the frame budget is 16 ms).
- The corpus is generated: its host variables are named after the same nouns as the tables, so a host word with the same first letters is common by construction. How often real code has that is not measured.

**Criterion met.** Keep the words of the language first.

*Ran in 70.7 s.*

## E30: Does the place in the grammar help to rank the suggestions?

**Hypothesis.** What belongs at the caret can be told from a few characters of look-behind: a table after `from`, a column after `select`, `by` after `group`, the properties inside the braces of CSS and the values of the property after its colon, the attributes of the tag inside `<button `. Putting what belongs first and what does not last puts the meant word in the first 5 more often than the previous word and the language alone, in every language that has a grammar, and it fits in a frame.

**Method.** E28's probes (50 generated files with structure, words typed with 1 to 3 letters, the list the editor gets). Four variants: the order by distance alone; the grammar; the previous word and the language (what the plugin ran with before this experiment); all three. Reported by the place of the caret. Also the start of a session on files of 1,200 to 60,000 lines.

**Criterion.** Adding the grammar to the previous word and the language is at least 2 points better within the first 5 over all the reachable cases; SQL, CSS and HTML each do not fall; and the session with all three stays under 16 ms at 60,000 lines, in every host.

**The word typed with 1 to 3 letters: 9564 typed prefixes, 9165 reachable (95.8%)**

| Variant | Word first | Within the first 5 | Within the first 10 | Mean reciprocal rank |
|---|---|---|---|---|
| By distance alone | 36.8% | 72.5% | 86.0% | 0.528 |
| Grammar | 63.8% | 88.5% | 96.4% | 0.743 |
| Previous word and language | 70.6% | 88.1% | 94.9% | 0.788 |
| All three | 83.8% | 97.8% | 99.3% | 0.898 |

**Within the first 5, by language of the string**

| Language | Cases | By distance alone | Grammar | Previous word and language | All three |
|---|---|---|---|---|---|
| css | 732 | 74.3% | 87.4% | 81.8% | 89.6% |
| graphql | 657 | 88.7% | 88.7% | 96.0% | 96.0% |
| html | 1128 | 61.1% | 80.6% | 77.4% | 94.6% |
| sql | 6648 | 72.7% | 89.9% | 89.8% | 99.4% |

**Within the first 5, by number of letters typed**

| Letters | Cases | By distance alone | Grammar | Previous word and language | All three |
|---|---|---|---|---|---|
| 1 | 3055 | 39.1% | 76.8% | 69.2% | 94.8% |
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
| `html:value` | 345 | 39.7% | 52.8% | 73.3% | 92.2% |
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
| JavaScript | 1200 | 58758 | 342 µs | 346 µs | 364 µs | 367 µs |
| JavaScript | 12000 | 577746 | 2.72 ms | 2.73 ms | 2.92 ms | 2.91 ms |
| JavaScript | 60000 | 2875519 | 3.11 ms | 3.21 ms | 4.23 ms | 4.22 ms |
| CSharp | 1200 | 68232 | 370 µs | 363 µs | 390 µs | 388 µs |
| CSharp | 12000 | 670891 | 2.57 ms | 2.52 ms | 2.71 ms | 2.70 ms |
| CSharp | 60000 | 3339512 | 2.90 ms | 2.92 ms | 3.90 ms | 3.92 ms |
| Python | 1200 | 59971 | 364 µs | 363 µs | 382 µs | 387 µs |
| Python | 12000 | 588954 | 2.82 ms | 2.81 ms | 3.01 ms | 3.01 ms |
| Python | 60000 | 2931279 | 3.21 ms | 3.21 ms | 4.28 ms | 4.24 ms |
| Cpp | 1200 | 59936 | 336 µs | 335 µs | 341 µs | 343 µs |
| Cpp | 12000 | 588276 | 2.54 ms | 2.54 ms | 2.69 ms | 2.68 ms |
| Cpp | 60000 | 2927538 | 2.87 ms | 2.86 ms | 3.52 ms | 3.51 ms |

**Analysis.**

- The grammar alone moves the share within the first 5 from 72.5% to 88.5%; added to the previous word and the language, +9.7 points; the worst of SQL, CSS and HTML changes by +7.8 points.
- The slowest session with all three at the largest size is Python at 60000 lines: 4.24 ms (the frame budget is 16 ms).
- The corpus is generated by the same person who wrote the rules of the grammar, and it follows them: the attributes of a tag are the ones in the table of attributes, the values of a property the ones in the table of values. A real file will have words the tables do not know, and the grammar then costs nothing but gains nothing. This experiment says the rules do not get in the way of code that follows them, not how often real code does.

**Criterion met.** Keep the grammar on.

*Ran in 74.6 s.*

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
| As it is | 43.2% | 61.0% | 68.2% | 87.9% | 74.5% | 0.644 |
| Words of the file first | 43.0% | 62.4% | 69.1% | 89.9% | 76.1% | 0.654 |
| Keywords by use | 43.9% | 62.9% | 70.2% | 90.3% | 76.6% | 0.661 |
| Both | 43.1% | 62.6% | 69.7% | 90.6% | 76.6% | 0.658 |

**Within the first 5, by language, test files**

| Language | Cases | As it is | Words of the file first | Keywords by use | Both |
|---|---|---|---|---|---|
| sql | 1032 | 76.7% | 77.0% | 77.6% | 77.3% |
| css | 1116 | 52.2% | 52.2% | 52.2% | 52.2% |
| html | 1032 | 85.9% | 85.9% | 88.7% | 88.7% |
| graphql | 976 | 60.9% | 59.8% | 60.9% | 59.8% |
| json | 992 | 75.2% | 73.2% | 75.2% | 73.2% |
| yaml | 1084 | 85.1% | 85.4% | 85.2% | 85.4% |
| glsl | 1190 | 72.9% | 82.3% | 79.9% | 83.0% |
| wgsl | 1198 | 85.7% | 90.3% | 90.7% | 90.3% |

**The hand-written files of the review (another source)**

| Variant | No letter: first | No letter: first 5 | One letter: first | One letter: first 5 | Both: first 5 | Mean reciprocal rank |
|---|---|---|---|---|---|---|
| As it is | 24.9% | 40.8% | 51.3% | 77.4% | 59.1% | 0.478 |
| Words of the file first | 24.6% | 44.9% | 56.9% | 87.7% | 66.3% | 0.523 |
| Keywords by use | 26.7% | 48.9% | 58.8% | 84.1% | 66.5% | 0.535 |
| Both | 24.9% | 45.8% | 58.5% | 91.8% | 68.8% | 0.537 |

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
| wgsl | 268 | 41.0% | 59.0% | 56.3% | 63.1% |

**Analysis.**

- On the test files the best variant is Both: +2.1 points within the first 5; the worst language changes by -2.0 points; on the hand-written files +9.7 points.
- The corpus is made by generators written by the same person who reads the result, with the idioms they know. Training and test files come from the same generators, so the order of the keywords by use is learned from the same distribution it is measured on; the hand-written files are the check that is not.
- A prior learned from generated code says which keywords this corpus uses, not which ones real projects use. The plugin could learn the order from the files of the user.

**Criterion not met.** Look at the tables by language: a language that gains and another that loses suggests an order for each.

*Ran in 2.4 s.*

## E35: Do a few keywords still come before the words of the file?

**Hypothesis.** E34 found that the words of the file first and the keywords by use gain in GLSL, WGSL and HTML but lose in JSON and GraphQL: in JSON the words of the file pushed `true`, `false` and `null` down, and they are the few keywords that are always right in a value. Putting only the few most used keywords of the language before the words of the file, and the others after them, keeps that gain and the loss goes away.

**Method.** E34's files, words and priors (the corpus of 500 snippets for each language, the even files for the order of the keywords, the odd files for measuring, and the hand-written files of the review as a second test). Variants: the engine as it is; words of the file first with the keywords by use (E34's best, 0 keywords in front); and the same with the 3, 6 and 12 most used keywords of the language in front of the words.

**Criterion.** The best of the variants with keywords in front is at least 3 points better than the engine as it is within the first 5, over the words typed with 0 or 1 letter on the test files; no language is worse by more than 1 point; and on the hand-written files it is not worse than the engine as it is.

**The test files**

| Variant | No letter: first | No letter: first 5 | One letter: first | One letter: first 5 | Both: first 5 | Mean reciprocal rank |
|---|---|---|---|---|---|---|
| As it is | 43.2% | 61.0% | 68.2% | 87.9% | 74.5% | 0.644 |
| Words first, 0 in front | 43.1% | 62.6% | 69.7% | 90.6% | 76.6% | 0.658 |
| 3 keywords in front | 43.9% | 62.9% | 69.6% | 90.5% | 76.7% | 0.660 |
| 6 keywords in front | 43.9% | 62.9% | 69.1% | 90.6% | 76.7% | 0.659 |
| 12 keywords in front | 43.9% | 62.9% | 70.8% | 92.0% | 77.4% | 0.666 |

**Within the first 5, by language, test files**

| Language | Cases | As it is | Words first, 0 in front | 3 keywords in front | 6 keywords in front | 12 keywords in front |
|---|---|---|---|---|---|---|
| sql | 1032 | 76.7% | 77.3% | 77.6% | 77.5% | 77.6% |
| css | 1116 | 52.2% | 52.2% | 52.2% | 52.2% | 52.2% |
| html | 1032 | 85.9% | 88.7% | 88.7% | 88.7% | 88.7% |
| graphql | 976 | 60.9% | 59.8% | 57.7% | 55.8% | 61.8% |
| json | 992 | 75.2% | 73.2% | 75.2% | 75.2% | 75.2% |
| yaml | 1084 | 85.1% | 85.4% | 85.3% | 85.2% | 85.2% |
| glsl | 1190 | 72.9% | 83.0% | 84.1% | 83.4% | 83.6% |
| wgsl | 1198 | 85.7% | 90.3% | 90.0% | 92.5% | 92.5% |

**The hand-written files of the review (another source)**

| Variant | No letter: first | No letter: first 5 | One letter: first | One letter: first 5 | Both: first 5 | Mean reciprocal rank |
|---|---|---|---|---|---|---|
| As it is | 24.9% | 40.8% | 51.3% | 77.4% | 59.1% | 0.478 |
| Words first, 0 in front | 24.9% | 45.8% | 58.5% | 91.8% | 68.8% | 0.537 |
| 3 keywords in front | 26.7% | 48.4% | 59.9% | 91.9% | 70.1% | 0.551 |
| 6 keywords in front | 26.7% | 48.9% | 59.7% | 91.7% | 70.3% | 0.552 |
| 12 keywords in front | 26.7% | 48.9% | 61.1% | 91.4% | 70.1% | 0.556 |

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
| wgsl | 268 | 41.0% | 63.1% | 63.4% | 64.9% | 64.9% |

**Analysis.**

- On the test files the best variant is 12 keywords in front: +3.0 points within the first 5; the worst language changes by 0.0 points; on the hand-written files +11.0 points.
- E35 was written after seeing E34, whose criterion was not met: the variants here are a reaction to its tables, not a prediction made before them. The same test files are used, so a gain here is partly fitted to them; the hand-written files are the check.

**Criterion not met.** Keep the order as it is for the languages that lose (JSON, GraphQL) and apply the variant only where it gains.

*Ran in 2.8 s.*

## E36: Should words of two letters be offered?

**Hypothesis.** The completion skips the words under 3 letters, so `id`, `db`, `in`, `uv` and `if` are never offered, and they are among the most written words of SQL, YAML and shaders. Offering them, after all the longer words, lets a person who types `i` find `id` without crowding the list for the person who wants a longer word.

**Method.** The corpus of 500 snippets for each of the 8 languages (the odd files, for measuring) and the hand-written files of the review. 600 words per language typed with 1 or 2 letters, only words that exist elsewhere in the file or are keywords. Three variants of the engine the plugin runs: as it is (minimum length 3); the minimum length lowered to 2 for every word; and the two-letter words in a tier after all the others. Reported for the words of 2 letters and for the longer ones.

**Criterion.** A variant raises the words of 2 letters by at least 15 points within the first 5; it lowers the longer words by no more than 0.5 point overall and no more than 1 point in any language; and on the hand-written files the words of 2 letters gain at least 10 points while the longer ones lose no more than 1.

**The test files**

| Variant | 2-letter words: cases | 2-letter: first | 2-letter: first 5 | Longer: cases | Longer: first | Longer: first 5 |
|---|---|---|---|---|---|---|
| Minimum 3 (as it is) | 144 | 79.2% | 86.6% | 2585 | 78.4% | 95.2% |
| Minimum 2 | 144 | 83.6% | 97.5% | 2585 | 78.3% | 95.2% |
| Two-letter words last | 144 | 79.2% | 93.1% | 2585 | 78.4% | 95.2% |

**Within the first 5, by language, test files**

| Language | 2-letter cases | 2-letter: Minimum 3 (as it is) | 2-letter: Minimum 2 | 2-letter: Two-letter words last | Longer: Minimum 3 (as it is) | Longer: Minimum 2 | Longer: Two-letter words last |
|---|---|---|---|---|---|---|---|
| sql | 38 | 69.0% | 93.1% | 80.2% | 99.1% | 98.9% | 99.1% |
| css | 1 | 100.0% | 100.0% | 100.0% | 78.6% | 78.5% | 78.6% |
| html | 26 | 100.0% | 100.0% | 100.0% | 99.1% | 99.1% | 99.1% |
| graphql | 32 | 95.8% | 96.9% | 95.8% | 94.1% | 94.1% | 94.1% |
| json | 6 | 52.6% | 100.0% | 100.0% | 96.5% | 96.5% | 96.5% |
| yaml | 14 | 84.1% | 100.0% | 93.2% | 97.6% | 97.2% | 97.6% |
| glsl | 10 | 100.0% | 100.0% | 100.0% | 98.9% | 98.9% | 98.9% |
| wgsl | 14 | 95.5% | 100.0% | 100.0% | 99.8% | 99.8% | 99.8% |

**The hand-written files of the review (another source)**

| Variant | 2-letter words: cases | 2-letter: first | 2-letter: first 5 | Longer: cases | Longer: first | Longer: first 5 |
|---|---|---|---|---|---|---|
| Minimum 3 (as it is) | 39 | 62.4% | 77.8% | 554 | 72.3% | 95.8% |
| Minimum 2 | 39 | 68.4% | 99.1% | 554 | 72.0% | 95.7% |
| Two-letter words last | 39 | 65.8% | 97.4% | 554 | 72.3% | 95.8% |

**Within the first 5, by language, hand-written files**

| Language | 2-letter cases | 2-letter: Minimum 3 (as it is) | 2-letter: Minimum 2 | 2-letter: Two-letter words last | Longer: Minimum 3 (as it is) | Longer: Minimum 2 | Longer: Two-letter words last |
|---|---|---|---|---|---|---|---|
| sql | 12 | 78.9% | 100.0% | 94.7% | 98.6% | 98.4% | 98.6% |
| css | 0 | 0.0% | 0.0% | 0.0% | 76.8% | 76.8% | 76.8% |
| html | 10 | 100.0% | 100.0% | 100.0% | 97.4% | 97.4% | 97.4% |
| graphql | 6 | 100.0% | 100.0% | 100.0% | 97.1% | 97.1% | 97.1% |
| json | 0 | 0.0% | 0.0% | 0.0% | 100.0% | 100.0% | 100.0% |
| yaml | 2 | 0.0% | 85.7% | 85.7% | 100.0% | 100.0% | 100.0% |
| glsl | 2 | 71.4% | 100.0% | 100.0% | 95.0% | 95.0% | 95.0% |
| wgsl | 4 | 35.7% | 100.0% | 100.0% | 99.6% | 99.6% | 99.6% |

**Analysis.**

- Minimum 2: the words of 2 letters +10.9 points, the longer ones -0.1 (worst language -0.4); hand-written: +21.4 and -0.1 (does not meet)
- Two-letter words last: the words of 2 letters +6.5 points, the longer ones 0.0 (worst language 0.0); hand-written: +19.7 and 0.0 (does not meet)
- The corpus is generated; 2-letter words in it are the ones the generators write (`id`, `db`, `as`, `on`, `uv`...).

**Criterion not met.** Offer them only where the place says a short word is likely (after a dot, after a word that was followed by one before).

*Ran in 1.4 s.*

## E37: Does the similar-words stage make noise with short prefixes, and what removes it?

**Hypothesis.** A review of 800 suggestions found the stage that corrects mistakes inventing suggestions with no relation when the person is typing a new word with 3 letters (`fir` offers `fragment`, `scr` offers `src`). With 3 letters one edit is a third of the word, so almost any word is "similar". Looking for similar words only from 4 letters, or showing fewer of them, or only the words of the file at 3 letters, removes most of that noise and keeps most of the recovery of real mistakes.

**Method.** The corpus of 500 snippets for each of the 8 languages (the odd files) and the hand-written files of the review. Recovery: words typed with one mistake (a swap, a missing letter, a wrong one, an extra one, never in the first letter) in a prefix that leaves 3, 4, 5 or 6 letters typed; only words that exist elsewhere in the file or are keywords; the meant word within the first 5. Noise: words written once in the file, not keywords, typed with a correct prefix of 3, 4 and 5 letters (a new word: nothing similar is wanted); how often any similar item is shown. Five variants: as it is; similar words only from 4 letters; at most 3 similar words with 3 letters; only words of the file as similar words with 3 letters; both of the last two.

**Criterion.** A variant keeps at least 85% of the recovery of the current engine with 3 letters typed (relative) and shows noise in at most half as many cases with a correct 3-letter prefix; recovery with 4 and 5 letters stays within 1 point of the current engine; and the hand-written files go the same way (noise lower, recovery with 3 letters at least 85% of the current).

**Recovery of a mistake, by letters typed: the meant word within the first 5 (test files), 11423 mistakes**

| Variant | 3 letters | 4 letters | 5 letters | 6 letters |
|---|---|---|---|---|
| As it is | 89.5% | 96.4% | 96.9% | 96.5% |
| From 4 letters | 16.1% | 96.4% | 96.9% | 96.5% |
| At most 3 items at 3 letters | 78.7% | 96.4% | 96.9% | 96.5% |
| File words only at 3 letters | 93.9% | 96.4% | 96.9% | 96.5% |
| Both of the last two | 90.3% | 96.4% | 96.9% | 96.5% |

**Noise on a new word, by letters typed: how often a similar item is shown (test files), 2787 new words**

| Variant | 3 letters | 4 letters | 5 letters |
|---|---|---|---|
| As it is | 29.8% | 16.9% | 7.2% |
| From 4 letters | 0.0% | 16.9% | 7.2% |
| At most 3 items at 3 letters | 29.8% | 16.9% | 7.2% |
| File words only at 3 letters | 26.6% | 16.9% | 7.2% |
| Both of the last two | 26.6% | 16.9% | 7.2% |

**Recovery of a mistake (hand-written files), 1728 mistakes**

| Variant | 3 letters | 4 letters | 5 letters | 6 letters |
|---|---|---|---|---|
| As it is | 87.3% | 98.3% | 97.6% | 95.1% |
| From 4 letters | 13.9% | 98.3% | 97.6% | 95.1% |
| At most 3 items at 3 letters | 80.5% | 98.3% | 97.6% | 95.1% |
| File words only at 3 letters | 81.5% | 98.3% | 97.6% | 95.1% |
| Both of the last two | 81.3% | 98.3% | 97.6% | 95.1% |

**Noise on a new word (hand-written files), 178 new words**

| Variant | 3 letters | 4 letters | 5 letters |
|---|---|---|---|
| As it is | 22.5% | 9.0% | 4.5% |
| From 4 letters | 0.0% | 9.0% | 4.5% |
| At most 3 items at 3 letters | 22.5% | 9.0% | 4.5% |
| File words only at 3 letters | 16.3% | 9.0% | 4.5% |
| Both of the last two | 16.3% | 9.0% | 4.5% |

**Analysis.**

- From 4 letters: recovery with 3 letters 16.1% against 89.5%, noise with 3 letters 0.0% against 29.8% (does not meet)
- At most 3 items at 3 letters: recovery with 3 letters 78.7% against 89.5%, noise with 3 letters 29.8% against 29.8% (does not meet)
- File words only at 3 letters: recovery with 3 letters 93.9% against 89.5%, noise with 3 letters 26.6% against 29.8% (does not meet)
- Both of the last two: recovery with 3 letters 90.3% against 89.5%, noise with 3 letters 26.6% against 29.8% (does not meet)
- The mistakes are made by the experiment (uniform over the letters but the first), not the way people mistype; and a new word that the generator writes once is a word that is rare in the corpus. Both numbers are about the corpus.

**Criterion not met.** Keep the stage as it is and say what it costs.

*Ran in 5.0 s.*

## E38: Should words of two letters be offered? (E36 with a criterion that can be met)

**Hypothesis.** The completion skips the words under 3 letters, so `id`, `db`, `in`, `uv` and `if` are never offered, and they are among the most written words of SQL, YAML and shaders. Offering them, after all the longer words, lets a person who types `i` find `id` without crowding the list for the person who wants a longer word.

**Method.** The corpus of 500 snippets for each of the 8 languages (the odd files, for measuring) and the hand-written files of the review. 600 words per language typed with 1 or 2 letters, only words that exist elsewhere in the file or are keywords. Three variants of the engine the plugin runs: as it is (minimum length 3); the minimum length lowered to 2 for every word; and the two-letter words in a tier after all the others. Reported for the words of 2 letters and for the longer ones.

**Criterion.** A variant closes at least half of the distance to 100% for the words of 2 letters within the first 5, on the test files and on the hand-written files; it lowers the longer words by no more than 0.5 point overall and 1 point in any language (and 1 point on the hand-written files). Among the variants that meet it, the one that lowers the longer words least is adopted, and then the one that gains most.

**The test files**

| Variant | 2-letter words: cases | 2-letter: first | 2-letter: first 5 | Longer: cases | Longer: first | Longer: first 5 |
|---|---|---|---|---|---|---|
| Minimum 3 (as it is) | 144 | 79.2% | 86.6% | 2585 | 78.4% | 95.2% |
| Minimum 2 | 144 | 83.6% | 97.5% | 2585 | 78.3% | 95.2% |
| Two-letter words last | 144 | 79.2% | 93.1% | 2585 | 78.4% | 95.2% |

**Within the first 5, by language, test files**

| Language | 2-letter cases | 2-letter: Minimum 3 (as it is) | 2-letter: Minimum 2 | 2-letter: Two-letter words last | Longer: Minimum 3 (as it is) | Longer: Minimum 2 | Longer: Two-letter words last |
|---|---|---|---|---|---|---|---|
| sql | 38 | 69.0% | 93.1% | 80.2% | 99.1% | 98.9% | 99.1% |
| css | 1 | 100.0% | 100.0% | 100.0% | 78.6% | 78.5% | 78.6% |
| html | 26 | 100.0% | 100.0% | 100.0% | 99.1% | 99.1% | 99.1% |
| graphql | 32 | 95.8% | 96.9% | 95.8% | 94.1% | 94.1% | 94.1% |
| json | 6 | 52.6% | 100.0% | 100.0% | 96.5% | 96.5% | 96.5% |
| yaml | 14 | 84.1% | 100.0% | 93.2% | 97.6% | 97.2% | 97.6% |
| glsl | 10 | 100.0% | 100.0% | 100.0% | 98.9% | 98.9% | 98.9% |
| wgsl | 14 | 95.5% | 100.0% | 100.0% | 99.8% | 99.8% | 99.8% |

**The hand-written files of the review (another source)**

| Variant | 2-letter words: cases | 2-letter: first | 2-letter: first 5 | Longer: cases | Longer: first | Longer: first 5 |
|---|---|---|---|---|---|---|
| Minimum 3 (as it is) | 39 | 62.4% | 77.8% | 554 | 72.3% | 95.8% |
| Minimum 2 | 39 | 68.4% | 99.1% | 554 | 72.0% | 95.7% |
| Two-letter words last | 39 | 65.8% | 97.4% | 554 | 72.3% | 95.8% |

**Within the first 5, by language, hand-written files**

| Language | 2-letter cases | 2-letter: Minimum 3 (as it is) | 2-letter: Minimum 2 | 2-letter: Two-letter words last | Longer: Minimum 3 (as it is) | Longer: Minimum 2 | Longer: Two-letter words last |
|---|---|---|---|---|---|---|---|
| sql | 12 | 78.9% | 100.0% | 94.7% | 98.6% | 98.4% | 98.6% |
| css | 0 | 0.0% | 0.0% | 0.0% | 76.8% | 76.8% | 76.8% |
| html | 10 | 100.0% | 100.0% | 100.0% | 97.4% | 97.4% | 97.4% |
| graphql | 6 | 100.0% | 100.0% | 100.0% | 97.1% | 97.1% | 97.1% |
| json | 0 | 0.0% | 0.0% | 0.0% | 100.0% | 100.0% | 100.0% |
| yaml | 2 | 0.0% | 85.7% | 85.7% | 100.0% | 100.0% | 100.0% |
| glsl | 2 | 71.4% | 100.0% | 100.0% | 95.0% | 95.0% | 95.0% |
| wgsl | 4 | 35.7% | 100.0% | 100.0% | 99.6% | 99.6% | 99.6% |

**Analysis.**

- Minimum 2: the words of 2 letters +10.9 points, the longer ones -0.1 (worst language -0.4); hand-written: +21.4 and -0.1 (meets)
- Two-letter words last: the words of 2 letters +6.5 points, the longer ones 0.0 (worst language 0.0); hand-written: +19.7 and 0.0 (does not meet)
- The corpus is generated; 2-letter words in it are the ones the generators write (`id`, `db`, `as`, `on`, `uv`...).

**Criterion met.** Offer the two-letter words in the way of that variant.

*Ran in 1.3 s.*

## Limits

- Synthetic code, not real files.
- The experiments run in a console process with a small heap, away from the Visual Studio UI thread and its GC pressure. They compare versions of the code: they are not times the user will see.
- Times are medians of repeated runs on one machine; differences of a few percent are noise.
