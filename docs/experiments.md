# Experiments

Besides the unit tests (`NestLight.Tests`, which say whether the code is right), the experiments ask how well the plugin behaves. Each one is a hypothesis, a test that can answer it and a criterion written before the run. This file holds only the definitions; the results of a run are in the report it writes (see *How it works*), valid for one commit, one runtime and one machine, and are not kept in the repository.

The main question behind the performance experiments: does NestLight slow Visual Studio down? The number of languages is not the problem, because the tokenizers only run on marked strings. What grows is the cost **per edit**, proportional to the file size: every new snapshot is copied (`GetText()`) and scanned, on the UI thread ([NestLightClassifier](../NestLight/VisualStudio/NestLightClassifier.cs), [SnapshotTokenCache](../NestLight/Highlighting/SnapshotTokenCache.cs)).

## Automatic and manual experiments

The ids say which kind it is, and each kind is numbered from 1: **EAnn** are automatic (class `EAnn_Name`, in `Experiments/Automatic/`), **EMnn** are manual (in `Experiments/Manual/`). 

The experiments are split by folder in [NestLight.Experiments/Experiments](../NestLight.Experiments/Experiments):

| Folder | What it is | How to run |
|---|---|---|
| `Automatic/` | they measure, apply their own criterion and write the report | `dotnet run -c Release --project NestLight.Experiments` |
| `Manual/` | a qualitative review with no criterion: one file per host x embedded language combination, to be read case by case | `dotnet run -c Release --project NestLight.Experiments -- --manual <dir>` |

"Manual" is for whoever reads: running it is one command. [CombinationReview](../NestLight.Experiments/Experiments/Manual/CombinationReview.cs) generates the code of every combination, runs the plugin over it and writes for each one the source, the strings found, every token with its type, the words of the string that got no color, the completion for each word typed with 1 and 2 letters (site, place in the grammar, rank of the intended word, top 10) and the carets in host code and in interpolations, where the plugin must offer nothing. `INDEX.md` counts the gaps, the misses and the wrong sites to say what to read first; a count is a pointer, not a verdict (plain text in Markdown is a gap that is correct). `--host` and `--language` narrow the run.

### The code generator

[CombinationGenerator](../NestLight.Experiments/Generator/CombinationGenerator.cs) writes source code for every combination of **host** (JavaScript, C#, Python, C++), **embedded language** (the 11 of the README), **way to mark the string** (tag, bare id, `language=id`) and **interpolation** (with or without). One sample of typical code per language lives in [LanguageSamples](../NestLight.Experiments/Generator/LanguageSamples.cs) and each host knows how to carry it (template literal, `$$"""` raw string, `rf"""`, `R"x(...)x"`), escaping what the host needs. A combination the plugin does not color is listed with its reason and not generated (tags outside JavaScript, interpolation in C++, `json` and `regex` in C#): 168 of 264 are applicable.

```
dotnet run -c Release --project NestLight.Experiments -- --generate out                 # one file per combination, 3 copies of the sample each
dotnet run -c Release --project NestLight.Experiments -- --generate out --host python --language sql --repeat 1000   # one big file
```

To add a language, add its sample to `LanguageSamples`; to add a host, add a rule to `Reason` and a writer to the generator. A new check over the matrix goes through `CombinationGenerator.Applicable()` and `CombinationCheck.Check` (EA36); a new section of the review goes in `CombinationReview.Review`.

## How it works

```
dotnet run -c Release --project NestLight.Experiments                  # the automatic experiments; writes reports/experiments-<date>-<commit>.md
dotnet run -c Release --project NestLight.Experiments -- --only EA03,EA07
dotnet run -c Release --project NestLight.Experiments -- --list
dotnet run -c Release --project NestLight.Experiments -- --quick       # smoke run, numbers not worth keeping
```

The *Experiments* workflow runs the same suite on Windows, on `net48` (the runtime of Visual Studio), and uploads the report.

## How to read this file

- The **hypothesis, the test and the criterion** are the stable part of an experiment. If one of them changes, the experiment is replaced: the old id is retired and a new one opened.
- A **result is a snapshot**: absolute times move between sessions even on the same machine, so compare numbers only inside one report, and run at least three times before trusting a verdict.
- *Criterion met* is the statement of the experiment's own criterion, not a grade. For an optimization, met means it is worth doing; for a risk, met means the plugin is healthy.

## Index

| Id | Question | Test |
|---|---|---|
| [EA01](#ea01-baseline-cost-of-one-highlight-call) | How much does one `Highlight` call cost? | Automated |
| [EA02](#ea02-skip-the-scan-when-the-text-has-no-language-id) | Does skipping the scan without language ids help? | Automated |
| [EA03](#ea03-the-text-copy-on-every-edit) | How much does the `GetText()` copy cost? | Automated |
| [EA04](#ea04-the-language-registry-built-per-buffer) | Does building the registry per buffer matter? | Automated |
| [EA05](#ea05-where-highlight-allocates) | Where do the allocations of `Highlight` come from? | Automated |
| [EA06](#ea06-number-of-marked-strings) | Does the cost stay linear as the strings multiply? | Automated |
| [EA07](#ea07-many-interpolations-in-one-string) | Does one string with many interpolations scale? | Automated |
| [EA08](#ea08-throughput-of-each-embedded-language) | Is any tokenizer much slower than the others? | Automated |
| [EA09](#ea09-malformed-and-pathological-input) | Does bad input make the cost explode? | Automated |
| [EA10](#ea10-incremental-analysis) | Is re-analyzing only the edited strings worth it? | Both |
| [EA11](#ea11-clip-tokens-without-rescanning-the-interpolations) | Does fixing the quadratic clipping make EA07 linear? | Automated |
| [EA12](#ea12-fewer-allocations-in-the-scan) | How much does the scan's allocation fall if comments stop allocating? | Automated |
| [EA13](#ea13-why-the-cost-grows-faster-than-the-text-above-400k-characters) | What makes the cost superlinear on very large strings? | Automated |
| [EA14](#ea14-completion-latency-against-the-size-of-the-file) | Is completion fast on large files? | Automated |
| [EA15](#ea15-do-the-limits-of-the-completion-change-its-cost) | Do the limits of the completion change its cost? | Automated |
| [EA16](#ea16-completion-on-incomplete-and-cut-code) | Can completion be triggered anywhere in a file being edited? | Automated |
| [EA17](#ea17-does-the-tokenizer-agree-with-the-vocabulary) | Does the tokenizer agree with the vocabulary? | Automated |
| [EA18](#ea18-order-of-the-words-of-the-document) | Is nearest-first the best order for the words of the document? | Automated, generated code |
| [EA19](#ea19-where-the-words-come-from-and-how-many-keystrokes-completion-saves) | Which words should completion offer? | Automated, generated code |
| [EA20](#ea20-sharing-the-scan-and-not-creating-the-words-of-the-completion) | Does sharing the scan and not creating the words bring completion under a frame? | Automated |
| [EA21](#ea21-does-the-second-stage-of-the-completion-fit-in-a-frame) | Does the second stage of the completion fit in a frame? | Automated |
| [EA22](#ea22-does-the-second-stage-recover-the-word-after-one-mistake-and-which-tie-break-works) | Does the second stage recover the word after one mistake? | Automated, generated code |
| [EA23](#ea23-does-the-second-stage-get-in-the-way-when-the-prefix-is-right) | Does it get in the way when the prefix is right? | Automated, generated code |
| [EA24](#ea24-completion-with-similar-words-on-incomplete-and-cut-code) | Is completion still robust with the second stage? | Automated |
| [EA25](#ea25-does-the-word-before-the-caret-help-to-rank-the-suggestions) | Does the word before the caret help to rank the suggestions? | Automated, generated code |
| [EA26](#ea26-do-the-words-of-the-same-language-come-first) | Do the words of the same language come first? | Automated, generated code |
| [EA27](#ea27-does-the-place-in-the-grammar-help-to-rank-the-suggestions) | Does the place in the grammar help to rank the suggestions? | Automated, generated code |
| [EA28](#ea28-does-the-schema-read-from-the-sql-of-the-document-help) | Does the schema read from the SQL of the document help? | Automated, generated code |
| [EA29](#ea29-do-the-words-used-most-often-come-before-the-nearest-ones) | Do the words used most often come before the nearest ones? | Automated, generated code |
| [EA30](#ea30-completion-with-the-context-rankings-on-incomplete-and-cut-code) | Is completion robust with the context rankings on? | Automated |
| [EA31](#ea31-where-the-place-of-the-caret-says-nothing-do-the-words-of-the-file-and-the-most-used-keywords-come-first) | Do the words of the file and the most used keywords come first where no rule decides? | Automated, generated code |
| [EA32](#ea32-do-a-few-keywords-still-come-before-the-words-of-the-file) | Do a few keywords still come before the words of the file? | Automated, generated code |
| [EA33](#ea33-should-words-of-two-letters-be-offered) | Should words of two letters be offered? | Automated, generated code |
| [EA34](#ea34-does-the-similar-words-stage-make-noise-with-short-prefixes-and-what-removes-it) | Does the similar-words stage make noise with short prefixes? | Automated, generated code |
| [EA35](#ea35-should-words-of-two-letters-be-offered-ea33-with-a-criterion-that-can-be-met) | EA33 with a criterion that can be met | Automated, generated code |
| [EA36](#ea36-every-host-with-every-embedded-language) | Does every host work with every embedded language? | Automated, generated code |
| [EM01](#em01-typing-latency-and-gc-inside-visual-studio) | What does the user feel while typing in a large file? | Manual |
| [EM02](#em02-background-re-analysis) | Does analyzing off the UI thread improve typing latency? | Manual |
| [EM03](#em03-do-the-similar-suggestions-show-up-in-visual-studio) | Do the similar suggestions show up in Visual Studio? | Manual |
| [EM04](#em04-review-of-every-host-with-every-embedded-language) | What does the plugin do, case by case, with every host and language? | Manual, generated code |

## EA01: baseline cost of one `Highlight` call

**Hypothesis.** Tokenizing is cheap enough that the number of languages and the scan of the host are not what could make the editor slow.

**Test.** `Highlight(text)` (host scan + tokenization) for the 4 hosts, with no marked string and with one unit in 10 carrying a marked string, at 1.2k, 12k and 60k lines.

**Criterion.** Every case stays under 16 ms (one frame) at 60k lines.

## EA02: skip the scan when the text has no language id

**Hypothesis.** A string is only embedded code when a tag or comment carries a known language id. If the text contains none of the 17 ids, the scan could not find anything, so skipping it saves time on C# and Python, which have no shortcut of their own.

**Test.** A decorator around the real highlighter looks for each id with `IndexOf(OrdinalIgnoreCase)` and returns no tokens when none appears. Same files, with and without the decorator, measured in the same process.

**Criterion.** At least 2x faster on every file without marked strings, and no more than 1.1x slower on every file with them.

## EA03: the text copy on every edit

**Hypothesis.** On every new snapshot the classifier copies the whole text into a string. Above ~85 KB (about 42k characters) that string lands on the Large Object Heap, which only gen2 collections reclaim: the copy costs time, and the collections cost more.

**Test.** Simulated typing on a synthetic C# file, 300 edits per size. Each edit copies the text into a new string and highlights it. Three variants: copy + `Highlight` (current), copy only, `Highlight` only on an existing string. Mean time per edit, so that GC pauses count.

**Criterion.** Removing the copy would save at least 1 ms per edit on the 400,000-character file. *(Restated when automated: the first wording said "1 ms or 30%" and mixed the two.)*

## EA04: the language registry built per buffer

**Hypothesis.** `CreateHighlighter` builds all the tokenizers again for every open file, and that cost adds up.

**Test.** `CreateEmbeddedLanguages()` 200 times, and 100 consecutive `CreateHighlighter` calls for each host.

**Criterion.** Opening a buffer costs less than 1 ms and 1 MB, for every host.

## EA05: where `Highlight` allocates

**Hypothesis.** `Highlight` allocates about twice the size of the text per call, and one of its two stages, the host scan or the rest (decoding, tokenizing, mapping back), accounts for most of it.

**Test.** Bytes allocated by the host scan alone and by the whole call on the same file, with and without marked strings, for the 4 hosts at 12k lines.

**Criterion.** On every host, with marked strings, one stage accounts for at least 70% of the bytes.

## EA06: number of marked strings

**Hypothesis.** The cost grows only linearly with the number of embedded strings: nothing in the engine (a sort, a nested scan) grows faster than the number of strings it has to handle.

**Test.** A file where every unit carries a marked string, at 100k, 200k, 400k and 800k characters, for each host. Time and gen2 collections.

**Criterion.** 8 times more embedded strings cost less than 10 times the time (linear is 8x, quadratic is 64x), for every host.

## EA07: many interpolations in one string

**Hypothesis.** The engine clips every token against the interpolations of its string starting from the first one, so a single large template with many interpolations costs O(tokens x interpolations). Suspected from reading `HighlightEngine.AddClipped`.

**Test.** One marked HTML string with 100, 1,000 and 10,000 interpolations, in JavaScript, C# and Python.

**Criterion.** Ten times more interpolations cost less than 20 times the time (linear is 10x, quadratic is 100x), for every host.

## EA08: throughput of each embedded language

**Hypothesis.** All the tokenizers run at a similar speed in characters per millisecond, and none gets slower per character on larger inputs.

**Test.** One marked string per language, in a JavaScript host, of 100,000 and of 1,000,000 characters made by repeating a snippet of typical code of that language.

**Criterion.** The slowest language is less than 5 times slower than the median one, and every language takes less than 12 times longer on 10 times the text.

## EA09: malformed and pathological input

**Hypothesis.** Code being typed is often malformed. An unterminated string or comment early in a large file, deep nesting, or a very long line can make the scan or a tokenizer quadratic.

**Test.** For each host: an unterminated marked string and an unterminated block comment at the top of a file of 250,000 characters, deep nesting (200 and 400 levels), and one line of 250,000 characters. Each case at N and at 2N. Cases under 5 ms at 2N are ignored.

**Criterion.** Doubling the input multiplies the time by less than 2.5 (linear is 2, quadratic is 4) in every case.

## EA10: incremental analysis

**Hypothesis.** Re-analyzing only the strings hit by the edit and reusing the tokens of the rest is much cheaper than the full scan.

**Test.** Automated: the cost of a one-character edit in the middle of a file against the full scan, over the EA01 sizes. Manual: the EM01 session. Plus a differential test: after any sequence of edits, the incremental tokens must equal the tokens of a full scan.

**Criterion.** At least 5x cheaper per edit at 60k lines, and the differential test passes on the existing samples and on random edits.

## EA11: clip tokens without rescanning the interpolations

**Hypothesis.** EA07 is quadratic because `AddClipped` walks the interpolations of the string from the first one for every token. Starting from the first one that can matter, found by binary search, makes the cost linear.

**Test.** Change the clipping and run EA07 again. Check that the tokens are exactly the same: the unit tests, plus a differential run of 3,000 random templates (all 4 hosts, random interpolations, nested templates, text cut at any point) whose tokens are compared before and after.

**Criterion.** EA07 criterion met (10x the interpolations cost less than 20x the time), no token changes, and no slowdown above 5% in EA01.

## EA12: fewer allocations in the scan

**Hypothesis.** The scan allocates more than the size of the text even when there is nothing to find, mostly in the handling of comments (a `Substring`, a `Trim` and a `ToLowerInvariant` for each one, plus an array of keys per call). Parsing the marker on the text itself, without creating strings, removes most of it. If EA06, EA08 and EA09 are limited by the garbage collector, they improve together.

**Test.** Change `MarkerComment.Parse` to read a range of the text, then run EA05 again, and EA06, EA08 and EA09 to see whether the superlinear growth goes away. Check that nothing changes: the unit tests (19 new ones compare the two overloads), a differential fuzz of 200,000 random comments (including Unicode that changes case in unexpected ways) against the old implementation, and the 3,000 random templates of EA11.

**Criterion.** EA05: the scan allocates less than 0.5x the size of the text without marked strings, and no token changes.

## EA13: why the cost grows faster than the text above ~400k characters

**Hypothesis.** When one file or one string is large, the cost per character grows even without the scan's allocations. Candidates: the decoding buffers (`List<char>` and `List<int>` of 12 bytes per character of the string, which land on the Large Object Heap and double as they grow), the list of tokens and its sort, and the processor cache.

**Test.** For one marked string of 100k, 400k and 800k characters, time the stages separately (decode, tokenize, map back, sort) and record gen2 collections and bytes allocated per stage. A variant presizes the buffers.

**Criterion.** One stage accounts for the extra cost, so that the superlinear growth can be attributed. Success is a cause, not a speed-up.

## EA14: completion latency against the size of the file

**Hypothesis.** Every keystroke in an embedded string scans the host to find the string (`Locate`) and the whole text for words (`Suggest`). That is cheap on a normal file but could be noticeable on a large one, mostly when thousands of distinct words share the prefix.

**Test.** For each host, a file of 1,200, 12,000 and 60,000 lines with the caret at the end of an open `comp` in a marked SQL string. Two shapes: EA01's file (typical code) and a file where every line declares a new `compNNNNN` identifier. Time of `Locate` alone and of `Locate` + `Suggest`, without the text copy of the editor (EA03).

**Criterion.** `Locate` + `Suggest` under 16 ms at the largest size, in every host and shape.

## EA15: do the limits of the completion change its cost?

**Hypothesis.** The limit on the number of suggestions (100) and the minimum word length (3) were picked without measuring. If the cost is in the scan, no value of them is a performance lever.

**Test.** The two shapes of EA14 at 12,000 lines (JavaScript), with the maximum number of suggestions at 10, 100, 1,000 and 10,000 and the minimum word length at 1, 3 and 5, one knob at a time from the default. Median of 100 runs, against the faster of two measurements of the default (first and last).

**Criterion.** Every combination within 25% of the default, in both shapes.

## EA16: completion on incomplete and cut code

**Hypothesis.** Completion runs while code is being typed, so it sees unterminated strings, half-written interpolations and carets anywhere. For every text and caret, `Locate` and `Suggest` must not throw, the site must lie inside the text around the caret with only word characters, and the suggestions must start with the typed prefix, add something to it and not repeat.

**Test.** 50 generated files. Every prefix cut and every single-character deletion at a stride (12,450 texts), the caret at the start, at the end and at 5 seeded random positions (86,900 carets, 16,374 of them inside the code of an embedded string). 6 invariants on each result.

**Criterion.** Zero violations.

## EA17: does the tokenizer agree with the vocabulary?

**Hypothesis.** The words the completion offers and the words the tokenizers color live in two places. SQL, GraphQL, YAML and shader lists reuse the tokenizers' sets, but HTML tags and CSS properties were written by hand. A word the completion offers and the tokenizer then splits, or colors as plain text, shows the plugin does not know what it just offered.

**Test.** Every word of every vocabulary (SQL 154, GraphQL 28, GLSL 215, WGSL 162, JSON 3, YAML 17, HTML 130, CSS 227) in one or more contexts of its language, through the real tokenizer. It checks that one token covers exactly the word and that its type is the expected one (for SQL and YAML: different from the type of an unknown word in the same context).

**Criterion.** Every word is one token, and at least 95% of the words of each language are classified as expected.

## EA18: order of the words of the document

**Hypothesis.** Listing the words that already exist in the document nearest to the caret first puts the right word among the first five more often than alphabetical, by frequency or by first appearance.

**Test.** 50 generated files, typed 1, 2 and 3 characters of a sample of the words of the embedded strings (9,651 cases; 95% are reachable: the word exists elsewhere or is a keyword). The position of the right word in five orderings. The same files are generated with locality 0.0, 0.5 and 0.9.

**Criterion.** At locality 0.5, nearest-first within the first 5 at least 5 percentage points above alphabetical.

## EA19: where the words come from, and how many keystrokes completion saves

**Hypothesis.** Offering the words of the whole document (host code included, as Visual Studio Code does) saves more keystrokes than offering only the words inside embedded strings, or only those of the string being typed.

**Test.** The corpus of EA18 at locality 0.5. 3,217 words typed one character at a time, up to 5. The completion is accepted at the first prefix where the right word is within the first 5; saving = length of the word - characters typed - 1 for the accepting key. Four scopes, always with the keywords except the first.

**Criterion.** The saving of the whole document within 2 percentage points of the best narrower scope, or above it.

## EA20: sharing the scan and not creating the words of the completion

**Hypothesis.** EA14 found that completion takes more than a frame above ~1 million characters, mostly the scan of the host. In Visual Studio a session scans the text twice (once to decide whether to open, once to fill the list) after the classifier already did it for the same snapshot, and the word pass created a string and a sort entry for every match. Sharing one scan between the classifier and the completion, and creating a word only when it is offered, brings the worst case under a frame.

**Test.** EA14's files. The start of one completion session, as the editor makes it (`Locate`, `Locate` again, `Suggest`) over a new text instance each time, in two cases: nothing shared, and the scan shared (the classifier has already highlighted that text, not timed). Plus `Suggest` alone. Check that nothing changes: 897 unit tests, among them a comparison of the order of the words with the first, straightforward implementation (dictionary and sort) on 4,800 random carets of small generated programs (those inside embedded strings are compared), and a test of two words at the same distance on both sides.

**Criterion.** The session with the scan shared stays under 16 ms at 60,000 lines, in every host and shape.

## EA21: does the second stage of the completion fit in a frame?

**Hypothesis.** The second stage compares what was typed with every distinct word of the document that passes two cheap filters (the first letter, the length) and keeps the best few. Even forced to run in every session, with thousands of words one edit away, a session still fits in the 16 ms of a frame.

**Test.** EA20's files and session (the scan shared, a new text instance each time), 4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines, with the second stage forced (asked for below any number of items). Two typed texts: `comp` (everything is compared, nothing new is found) and `cmop` (in the distinct-words file all 60,000 words are one edit away). Also the same session with the second stage off, and the bytes allocated.

**Criterion.** Under 16 ms at 60,000 lines in every host, shape and typed text.

## EA22: does the second stage recover the word after one mistake, and which tie-break works?

**Hypothesis.** When the typed text has one edit (an extra letter, a missing one, a wrong one, two swapped) in a prefix of 4 to 8 letters, the meant word is among the first 5 suggestions in most cases. Among words the same number of edits away, the nearest to the caret is no worse a tie-break than the most frequent.

**Test.** The corpus of EA18 (50 generated files, locality 0.5). A sample of the words of the embedded strings is typed as a prefix of 4 to 8 letters with one edit of each kind at a random place over the prefix (the first letter included). Only the reachable cases count (the word exists elsewhere in the document or is a keyword). The list is also reordered inside each group of the same kind and distance: nearest first (the engine), most frequent first, most frequent and then nearest.

**Criterion.** The meant word within the first 5 in at least 70% of the reachable cases with the best of the three tie-breaks; if more than one reaches 70%, the best result enters, and on a tie the nearest stays.

## EA23: does the second stage get in the way when the prefix is right?

**Hypothesis.** If the second stage ran whenever the first found few items, a correct prefix would often get a list with words that are only similar by chance. Running it only when nothing matched (`FuzzyBelow` = 1) keeps that rare.

**Test.** The corpus of EA18. A sample of the words of the embedded strings typed correctly, 3 to 8 letters, with the rest of the word removed. The engine with `FuzzyBelow` = 1, 3 and 5; the cases where similar items are added are counted, apart for those where the first stage found something and those where it found nothing.

**Criterion.** With `FuzzyBelow` = 1, similar items are added in at most 5% of the cases. The default is chosen among the values that meet it.

## EA24: completion with similar words on incomplete and cut code

**Hypothesis.** Completion runs while code is being typed. For every text and caret, `Suggest` must not throw; the site must lie inside the text with only word characters; the exact suggestions must start with the typed text; the similar ones must be at the distance they claim (as the definition computes it), between 1 and the tolerance, with the first letter typed, after the exact ones, keywords before words and fewer edits first; nothing repeats; and the limits hold.

**Test.** EA16's: 50 generated files, every prefix cut and every single-character deletion at a stride (12,450 texts), the caret at the start, at the end and at 5 random places (86,900 carets, about 16,000 of them inside embedded code); plus, at each of those, the same text with a one-letter mistake put in the word under the caret. The distance of every similar item is recomputed with the whole matrix.

**Criterion.** Zero violations.

## EA25: does the word before the caret help to rank the suggestions?

**Hypothesis.** The words that already followed the same word (with the same punctuation) elsewhere in the document are the likely ones: after `from ` the word that followed `from` before, after `display: ` the value that followed `display:`, after `group ` the `BY`. Putting them first puts the meant word in the first 5 more often than the order by distance alone.

**Test.** 50 generated files with structure, a sample of the words of the embedded strings typed with 1 to 3 letters (9,564 prefixes, 9,162 reachable), the list the editor gets. The order by distance alone against the previous word first; the same words ordered by the nearest occurrence of the context. Also the start of a session on files of 1,200 to 60,000 lines.

**Criterion.** At least 3 points more within the first 5 over all the reachable cases; no language more than 1 point worse; the session under 16 ms at 60,000 lines in every host.

## EA26: do the words of the same language come first?

**Hypothesis.** A word written in the code of another string of the same language (a column in another SQL string) is likelier than a word of the host code or of a string of another language that happens to start with the same letters, even when the other one is nearer to the caret. Putting the words of the language first, and taking the context of the previous word from them only, puts the meant word in the first 5 more often. It is EA19's question (where should the words come from) asked as a ranking and not as a filter: nothing is dropped, the other words come after.

**Test.** EA25's probes, four variants: the order by distance alone; the words of the language first; the previous word; both. Interpolations are host code. Also the start of a session on files of 1,200 to 60,000 lines with the scan shared.

**Criterion.** Adding the words of the language to the previous word is at least 2 points better within the first 5; no language more than 1 point worse; the session under 16 ms at 60,000 lines in every host.

## EA27: does the place in the grammar help to rank the suggestions?

**Hypothesis.** What belongs at the caret can be told from a few characters of look-behind ([the grammar of each language](../NestLight/Completion/Languages), no parser): a table after `FROM`, a column after `SELECT`, `BY` after `GROUP`, the properties inside the braces of CSS and the values of the property after its colon, the attributes of the tag inside `<button `. Putting what belongs first and what does not last (nothing is dropped) puts the meant word in the first 5 more often than the previous word and the language alone, and fits in a frame.

**Test.** EA25's probes, four variants: the order by distance alone; the grammar; the previous word and the language (what the plugin ran with before); all three. Reported by the place of the caret. Also the start of a session on files of 1,200 to 60,000 lines.

**Criterion.** Adding the grammar to the previous word and the language is at least 2 points better within the first 5; SQL, CSS and HTML each do not fall; the session under 16 ms at 60,000 lines in every host.

## EA28: does the schema read from the SQL of the document help?

**Hypothesis.** The tables and columns the SQL of the file talks about ([SqlSchema](../NestLight/Completion/Languages/SqlSchema.cs): a `CREATE TABLE`, the `FROM` and `JOIN` of the statement, the aliases, the column list of an `INSERT`, the `SET` of an `UPDATE`) tell which table to offer after `FROM` and which columns belong after `u.` or in the select list. That is more precise than the previous word, because an alias means a different table in every statement. The reader is a scan of the common shapes, not a parser.

**Test.** EA25's probes reported by the place of the caret: the order by distance alone; the previous word, the language and the grammar (what the plugin ran with); the same plus the schema; the schema alone. Files with a `CREATE TABLE` for each table and joins with the aliases `t` and `o` reused for a different table in every statement. Also, apart from the criterion, 200 short files (4 functions each), where the previous word has little history. Also the start of a session with `select u.comp| from users u` typed at the end of files of 1,200 to 60,000 lines, where every SQL string in the window is read.

**Criterion.** At least 1 point better within the first 5 over all the reachable cases; the places that need a table or a column (`sql:table`, `sql:member`, `sql:expression`) do not fall; the session under 16 ms at 60,000 lines in every host.

## EA29: do the words used most often come before the nearest ones?

**Hypothesis.** The order by distance alone sends to the end a word that is used all over the file and is not close to the caret, while the nearest word may have been used once. A blend, `ln(1 + count) - weight * ln(1 + distance)`, puts the meant word in the first 5 more often, whatever the locality of the code. The engine has no edit history, so how near an occurrence is to the caret stands for how recently the word was used; real recency (the words accepted or typed last) would need the editor to tell the engine and is not done here.

**Test.** EA25's probes on files of three localities (0, 0.5, 0.9), 50 each, with the previous word, the language and the grammar on. Five orders of the words of the document and of the words that followed the context: distance alone, count alone, and the blend with a weight of 1, 0.5 and 0.25. Also, apart from the criterion, the same orders with no other feature, and the start of a session.

**Criterion.** The best of the four other orders at least 1.5 points better within the first 5 than distance alone at locality 0.5; at no locality worse by more than 1 point; no language worse by more than 1 point; the session under 16 ms at 60,000 lines.

## EA30: completion with the context rankings on incomplete and cut code

**Hypothesis.** EA24 again with every context feature on (the previous word, the words of the language, the grammar of SQL, CSS and HTML, the schema of the SQL, the blend of count and distance), over the structured files that have the statements, rules and tags those features read. The look-behind of the grammar and of the schema, the pointers into the ranges of the strings, the ranked words and the short words offered after a context must not throw on a text cut anywhere, repeat a word, break the limits, or offer an exact suggestion that does not start with what was typed.

**Test.** EA24's: every prefix cut at a stride and every single-character deletion at a stride (12,269 texts), the caret at the start, at the end and at 5 random places (85,633 carets, 35,116 of them inside embedded code), and the same position with a mistake in the word (16,841), with every feature of `CompletionFeatures` on, schema and blend included.

**Criterion.** Zero violations.

## EA31: where the place of the caret says nothing, do the words of the file and the most used keywords come first?

**Hypothesis.** A review of 800 hand-checked suggestions found that, without a rule for the place, the list is the vocabulary in alphabetical order, cut at 100, with the words of the file after it: with nothing or one letter typed the word that is wanted is often out of the first five or out of the list (GLSL, WGSL, GraphQL, and the keyword soup in the others). The words of the file first, and the keywords in the order of how much code uses them, should put it among the first five more often.

**Test.** A corpus of 500 snippets for each of the 8 languages ([the generators](../NestLight.Experiments/Corpus), 50 files of 10 snippets each; `--corpus <dir>` writes it). The even files are used to learn how often each keyword is used, the odd files to measure: 600 words per language, typed with no letter (a request with Ctrl+Space) and with one letter. A second test on the hand-written files of the review, which come from another source. Four variants of the engine the plugin runs: as it is; the words of the file before the keywords; the keywords by use; both.

**Criterion.** Over the words typed with 0 or 1 letter, the best of the three variants at least 3 points better within the first 5 on the test files; no language more than 1 point worse; not worse on the hand-written files.

## EA32: do a few keywords still come before the words of the file?

**Hypothesis.** Put only the few most used keywords of the language before the words of the file and the others after them; that keeps EA31's gain and removes its loss.

**Test.** EA31's files, words and priors. The engine as it is; the words of the file first with the keywords by use (0 keywords in front); and the same with the 3, 6 and 12 most used keywords in front. EA32 was written after seeing EA31: the variants react to its tables, and the test files are the same, so a gain is partly fitted to them. The hand-written files are the check.

**Criterion.** The best variant with keywords in front at least 3 points better within the first 5 on the test files; no language more than 1 point worse; not worse on the hand-written files.

## EA33: should words of two letters be offered?

**Hypothesis.** The completion skips the words under 3 letters, so `id`, `db`, `in`, `uv` and `if` are never offered, and they are among the most written words of SQL, YAML and shaders. Offering them, after all the longer words, lets a person who types `i` find `id` without crowding the list for the person who wants a longer word.

**Test.** The corpus of 500 snippets for each language (the odd files) and the hand-written files of the review: 600 words per language typed with 1 or 2 letters, only the words that exist elsewhere in the file or are keywords. The engine of the plugin as it is (minimum length 3), with the minimum lowered to 2 for every word, and with the two-letter words in a tier after all the others.

## EA34: does the similar-words stage make noise with short prefixes, and what removes it?

**Hypothesis.** A review of 800 suggestions found the stage that corrects mistakes inventing suggestions with no relation when the person is typing a new word with 3 letters (`fir` offers `fragment`, `scr` offers `src`). With 3 letters one edit is a third of the word, so almost any word is "similar". Looking for similar words only from 4 letters, showing at most 3, or only the words of the file at 3 letters should remove most of that noise and keep most of the recovery.

**Test.** The corpus (odd files) and the hand-written files. Recovery: words typed with one mistake (a swap, a missing letter, a wrong one, an extra one; never the first letter) that leaves 3, 4, 5 or 6 letters typed, only the words that exist elsewhere in the file or are keywords (11,423 mistakes); the meant word within the first 5. Noise: words written once in the file, not keywords, typed with a correct prefix of 3, 4 and 5 letters (2,787 new words); how often any similar item is shown. Five variants: as it is; from 4 letters; at most 3 items at 3 letters; only the words of the file at 3 letters; both of the last two.

**Criterion.** A variant keeps at least 85% of the recovery of the current engine with 3 letters typed (relative), shows noise in at most half as many cases with a correct 3-letter prefix, keeps the recovery with 4 and 5 letters within 1 point, and goes the same way on the hand-written files.

## EA35: should words of two letters be offered? (EA33 with a criterion that can be met)

**Criterion.** The same test as EA33. A variant closes at least half of the distance to 100% for the words of 2 letters on the test files and on the hand-written files, and lowers the longer words by no more than 0.5 point overall and 1 point in any language (1 point on the hand-written files). Among the variants that meet it, the one that lowers the longer words least is adopted, then the one that gains most. It was written **after** EA33's numbers: the numbers are the same, the criterion is the one EA33 should have had.

## EA36: every host with every embedded language

**Hypothesis.** The unit tests try each host with some languages and each language mostly in a JavaScript host. A combination nobody wrote a test for may be missed or read as another language.

**Test.** The [generator](#the-code-generator) writes a file for each of the 168 applicable combinations; each is run through the scan and the highlighter at 1 and at 200 copies of the sample and compared with what the generator put there (strings, interpolations, language, tokens inside the strings). The time to highlight 200 copies is reported by host and language.

**Criterion.** Every applicable combination passes at both sizes.

## EM01: typing latency and GC inside Visual Studio

**Hypothesis.** In a real session, the gen2 collections caused by the text copy (EA03) and the analysis on the UI thread are enough to make typing in a large file noticeable.

**Test (manual).** Open a large real file (generated code, a JS bundle of 5k to 50k lines), type continuously, and record the per-keystroke latency and the gen2 count and pause time with and without the extension. A fixed file and script, kept with the result, so runs can be repeated.

**Criterion.** A p95 latency increase under 8 ms, and no gen2 pause above 20 ms.

## EM02: background re-analysis

**Hypothesis.** Returning the tokens of the previous snapshot and raising `ClassificationChanged` when the new analysis finishes takes the scan off the UI thread and improves typing latency.

**Test (manual).** The EM01 session before and after the change.

**Criterion.** The p95 latency in EM01 goes back to within 2 ms of the baseline without the extension.

## EM03: do the similar suggestions show up in Visual Studio?

**Hypothesis.** The item manager of the editor filters the list at every key against the text of the span. A similar item is created with a filter text equal to what was typed, and shows and inserts the word it suggests, so the manager keeps it. The documentation does not say whether the default filter tolerates this.

**Test (manual).** In a marked string, type a word of 6 letters with a mistake, press Ctrl+Space, and keep typing.

**Criterion.** The meant word is in the list at the first step.

## EM04: review of every host with every embedded language

**Hypothesis.** EA36 says every combination is found and tokenized, not whether the colors and the suggestions are good. Reading what the plugin does with each combination, one by one, finds what a count cannot: a word that should have a color and has none, a suggestion that does not belong (a variable of the host offered inside SQL), a caret in host code that gets a site.

**Test (manual, qualitative).** `--manual <dir>` generates the 168 combinations, runs the plugin over them and writes a file for each with the source, the tokens, the words with no token, the completion for each word typed with 1 and 2 letters and the carets that must get nothing. A reader goes through the files and judges each case.

**Criterion.** None: the result is a list of findings, each with the file and the case.
