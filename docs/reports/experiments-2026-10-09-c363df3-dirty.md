# NestLight experiments report

| | |
|---|---|
| Date | 2026-10-09 02:12 UTC |
| Commit | `c363df3` (**uncommitted changes**: this run cannot be reproduced from the commit) |
| Runtime | .NET 10.0.0, Release |
| Machine | CPU not identified, 16 logical cores |
| OS | Linux Mint 22 |
| Mode | full |

Definitions of the experiments: [docs/experiments.md](../experiments.md). Each result is valid only for the commit and the machine above.

## Summary

| Id | Experiment | Criterion | Finding |
|---|---|---|---|
| E19 | Does the tokenizer agree with the vocabulary? | Met | vocabulary and tokenizers agree |
| E22 | Sharing the scan and not creating the words of the completion | Met | worst case with the scan shared 1.49 ms at 60000 lines |
| E24 | Does the second stage recover the word after one mistake, and which tie-break works? | Met | best tie-break (nearest to the caret (the engine)) 72.0% within the first 5 |
| E25 | Does the second stage get in the way when the prefix is right? | Met | with FuzzyBelow = 1 similar items are added in 1.1% of the cases |
| E27 | Completion with similar words on incomplete and cut code | Met | 0 violations in 86900 carets, 21275 similar items checked |
| E30 | Does the place in the grammar help to rank the suggestions? | Met | within the first 5: 88.1% to 97.8% (+9.7 points); worst session 4.73 ms |
| E33 | Completion with the context rankings on incomplete and cut code | Met | 0 violations in 85633 carets, 28057 similar items checked |

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
| JavaScript | typical code | 1200 | 41771 | 341 µs | 125 µs | 114 µs | 1 |
| JavaScript | typical code | 12000 | 417611 | 3.03 ms | 1.09 ms | 1.11 ms | 1 |
| JavaScript | typical code | 60000 | 2088011 | 11.09 ms | 1.38 ms | 1.29 ms | 1 |
| JavaScript | distinct words | 1200 | 24011 | 176 µs | 113 µs | 80 µs | 100 |
| JavaScript | distinct words | 12000 | 240011 | 1.26 ms | 655 µs | 623 µs | 100 |
| JavaScript | distinct words | 60000 | 1200011 | 5.13 ms | 1.31 ms | 1.35 ms | 100 |
| CSharp | typical code | 1200 | 35058 | 308 µs | 98 µs | 97 µs | 1 |
| CSharp | typical code | 12000 | 349703 | 3.12 ms | 938 µs | 923 µs | 1 |
| CSharp | typical code | 60000 | 1748126 | 11.82 ms | 1.33 ms | 1.33 ms | 1 |
| CSharp | distinct words | 1200 | 24032 | 162 µs | 82 µs | 78 µs | 100 |
| CSharp | distinct words | 12000 | 240032 | 1.46 ms | 647 µs | 626 µs | 100 |
| CSharp | distinct words | 60000 | 1200032 | 6.06 ms | 1.28 ms | 1.36 ms | 100 |
| Python | typical code | 1200 | 29494 | 416 µs | 84 µs | 391 µs | 1 |
| Python | typical code | 12000 | 294167 | 4.28 ms | 796 µs | 780 µs | 1 |
| Python | typical code | 60000 | 1470512 | 18.41 ms | 1.41 ms | 1.39 ms | 1 |
| Python | distinct words | 1200 | 18026 | 200 µs | 91 µs | 58 µs | 100 |
| Python | distinct words | 12000 | 180026 | 1.86 ms | 455 µs | 412 µs | 100 |
| Python | distinct words | 60000 | 900026 | 8.43 ms | 1.48 ms | 1.19 ms | 100 |
| Cpp | typical code | 1200 | 32804 | 320 µs | 114 µs | 92 µs | 1 |
| Cpp | typical code | 12000 | 325951 | 2.50 ms | 887 µs | 880 µs | 1 |
| Cpp | typical code | 60000 | 1629061 | 9.76 ms | 1.49 ms | 1.56 ms | 1 |
| Cpp | distinct words | 1200 | 24036 | 181 µs | 124 µs | 81 µs | 100 |
| Cpp | distinct words | 12000 | 240036 | 1.60 ms | 647 µs | 612 µs | 100 |
| Cpp | distinct words | 60000 | 1200036 | 6.28 ms | 1.29 ms | 1.32 ms | 100 |

**Analysis.**

- The slowest session with the scan shared at 60000 lines is Cpp, typical code: 1.49 ms (the frame budget is 16 ms).
- "Nothing shared" is the start of a session as the editor makes it without the shared scan: Locate twice and Suggest. E16 measured one Locate and Suggest.

**Criterion met.** Completion fits in a frame on any file the classifier can handle.

*Ran in 7.8 s.*

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

*Ran in 6.8 s.*

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

*Ran in 5.5 s.*

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
| JavaScript | 1200 | 58758 | 404 µs | 410 µs | 428 µs | 430 µs |
| JavaScript | 12000 | 577746 | 3.23 ms | 3.23 ms | 3.44 ms | 3.42 ms |
| JavaScript | 60000 | 2875519 | 3.80 ms | 3.65 ms | 4.62 ms | 4.64 ms |
| CSharp | 1200 | 68232 | 430 µs | 431 µs | 452 µs | 454 µs |
| CSharp | 12000 | 670891 | 2.98 ms | 2.99 ms | 3.17 ms | 3.19 ms |
| CSharp | 60000 | 3339512 | 3.40 ms | 3.41 ms | 4.51 ms | 4.55 ms |
| Python | 1200 | 59971 | 420 µs | 421 µs | 439 µs | 439 µs |
| Python | 12000 | 588954 | 3.31 ms | 3.31 ms | 3.51 ms | 3.54 ms |
| Python | 60000 | 2931279 | 3.78 ms | 3.79 ms | 4.84 ms | 4.73 ms |
| Cpp | 1200 | 59936 | 394 µs | 387 µs | 402 µs | 416 µs |
| Cpp | 12000 | 588276 | 3.02 ms | 3.01 ms | 3.16 ms | 3.16 ms |
| Cpp | 60000 | 2927538 | 3.33 ms | 3.38 ms | 4.10 ms | 4.09 ms |

**Analysis.**

- The grammar alone moves the share within the first 5 from 72.5% to 88.5%; added to the previous word and the language, +9.7 points; the worst of SQL, CSS and HTML changes by +7.8 points.
- The slowest session with all three at the largest size is Python at 60000 lines: 4.73 ms (the frame budget is 16 ms).
- The corpus is generated by the same person who wrote the rules of the grammar, and it follows them: the attributes of a tag are the ones in the table of attributes, the values of a property the ones in the table of values. A real file will have words the tables do not know, and the grammar then costs nothing but gains nothing. This experiment says the rules do not get in the way of code that follows them, not how often real code does.

**Criterion met.** Keep the grammar on.

*Ran in 73.7 s.*

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
| Similar items checked against the definition | 28057 |
| Violations | 0 |

**Analysis.**

- 35116 of the 85633 carets were inside the code of an embedded string and went through Suggest; 16841 more with a mistake in the word.

**Criterion met.** Completion can be triggered anywhere in a file being edited, with the context rankings on.

*Ran in 3.8 s.*

## Limits

- Synthetic code, not real files.
- The experiments run in a console process with a small heap, away from the Visual Studio UI thread and its GC pressure. They compare versions of the code: they are not times the user will see.
- Times are medians of repeated runs on one machine; differences of a few percent are noise.
