# Experiments

Besides the unit tests (`NestLight.Tests`, which say whether the code is right), the experiments ask how well the plugin behaves. Each one is a hypothesis, a test that can answer it and a criterion written before the run. This file holds only the definitions; the results of a run are in the report it writes (see *How it works*), valid for one commit, one runtime and one machine, and are not kept in the repository.

The main question behind the performance experiments: does NestLight slow Visual Studio down? The number of languages is not the problem, because the tokenizers only run on marked strings. What grows is the cost **per edit**, proportional to the file size: every new snapshot is copied (`GetText()`) and scanned, on the UI thread ([NestLightClassifier](../NestLight/VisualStudio/NestLightClassifier.cs), [SnapshotTokenCache](../NestLight/Highlighting/SnapshotTokenCache.cs)).

## Automatic and manual experiments

An experiment is a hypothesis, a test that can answer it and a criterion. There are two kinds, numbered separately from 1: **EAnn** (automatic, class `EAnn_Name` in `Experiments/Automatic/`) and **EMnn** (manual, in `Experiments/Manual/`).

**Automatic** experiments are code that measures and decides. The criterion is written before the run and the result is *met* or *not met*. They write a report with tables and analysis to `reports/`.

**Manual** experiments are a qualitative test of the result of the plugin, **without Visual Studio**: looking at the final result with the eye. The generator writes examples, the plugin runs over them, and the harness leaves **artifacts**: one file per case with everything the plugin did. A person or an AI agent then reads the artifacts case by case and judges their quality: is this the color the piece should have, does this suggestion belong, should the plugin have stayed out here. When a case is bad, the code is changed, the experiment is run again and the same cases are read again; that loop is the experiment. It is meant to be half automated: the machine generates and runs, the reader judges.

Rules:
- Every manual experiment **generates artifacts**. An experiment without artifacts to read is not manual, it is automatic.
- They have no criterion. The result is a list of findings, each pointing at its file and case.
- Their input is always the code of the generator.
- **What is tried by hand inside Visual Studio is not documented here** (no experiment, no id): it needs a person at the IDE, leaves no artifact and cannot be repeated by an agent. Manual experiments exist to avoid depending on the IDE.

| | Automatic | Manual |
|---|---|---|
| Who judges | the code | a person or an AI agent reading the artifacts |
| Criterion | written before the run | none; findings |
| Input | its own corpora and synthetic files, or the generator | the generator |
| Output | a report in `reports/` | artifacts in `artifacts/`, one set per case |
| Run | `dotnet run -c Release --project NestLight.Experiments` | `dotnet run -c Release --project NestLight.Experiments -- --manual` (EM01), `-- --gallery` (EM02) |

### The code generator

[CombinationGenerator](../NestLight.Experiments/Generator/CombinationGenerator.cs) writes source code for every combination of **host** (JavaScript, C#, Python, C++), **embedded language** (the 11 of the README), **way to mark the string** (tag, bare id, `language=id`) and **interpolation** (with or without). One sample of typical code per language lives in [LanguageSamples](../NestLight.Experiments/Generator/LanguageSamples.cs) and each host knows how to carry it (template literal, `$$"""` raw string, `rf"""`, `R"x(...)x"`), escaping what the host needs. A combination the plugin does not color is listed with its reason and not generated (tags outside JavaScript, interpolation in C++, `json` and `regex` in C#): 168 of 264 are applicable.

```
dotnet run -c Release --project NestLight.Experiments -- --generate out                 # one file per combination, 3 copies of the sample each
dotnet run -c Release --project NestLight.Experiments -- --generate out --host python --language sql --repeat 1000   # one big file
```

To add a language, add its sample to `LanguageSamples`; to add a host, add a rule to `Reason` and a writer to the generator. A new automatic check over the matrix goes through `CombinationGenerator.Applicable()` and `CombinationCheck.Check` (EA31); a new section of the manual review goes in `EM01_CombinationReview.Review`.

## How it works

```
dotnet run -c Release --project NestLight.Experiments                  # the automatic experiments; writes reports/<time>-<commit>.md
dotnet run -c Release --project NestLight.Experiments -- --only EA03,EA07
dotnet run -c Release --project NestLight.Experiments -- --list
dotnet run -c Release --project NestLight.Experiments -- --quick       # smoke run, numbers not worth keeping
dotnet run -c Release --project NestLight.Experiments -- --manual      # the manual experiments; writes artifacts/EM01/<time>/
```

The *Experiments* workflow runs the same suite on Windows, on `net48` (the runtime of Visual Studio), and uploads the report.

## What is versioned and what is not

The repository holds only **timeless** documentation: what the project is, what each experiment asks (hypothesis, test, criterion) and how to run it. Nothing that records one execution is committed.

| Folder | What | In git |
|---|---|---|
| `reports/` | the report of an automatic run, named by the time it ran (UTC) and the commit: `2026-10-09_14-30-05Z-<commit>.md` | no |
| `artifacts/` | what a manual run leaves to be read, in a folder named by the time of the run: `artifacts/EM01/2026-10-09_14-30-05Z/` | no |

Both are in `.gitignore`. Every report and every set of artifacts belongs to a moment, one commit and one machine, so a new run adds a new file or folder and never replaces an old one. A result that must outlive the run (a decision, a number a release depends on) goes, at most, into the changelog, never into the docs of an experiment: those docs are rewritten when the definition changes, not when a result does.

## Each experiment has its own document

An experiment is one source file with a document of the same name beside it: `Experiments/Automatic/EA01_Baseline.cs` and `EA01_Baseline.md`, `Experiments/Manual/EM01_CombinationReview.cs` and `.md`. The document holds the hypothesis, the test and the criterion, and nothing else. This file is the general part (kinds, generator, how to run, what is versioned) and the index.

## How to read the experiment documents

- The **hypothesis, the test and the criterion** are the stable part of an experiment. If one of them changes, the experiment is replaced: the old id is retired and a new one opened.
- *Criterion met* is the statement of the experiment's own criterion, not a grade. For an optimization, met means it is worth doing; for a risk, met means the plugin is healthy.
- Absolute times move between sessions even on the same machine: compare numbers only inside one report, and run at least three times before trusting a verdict.

## Index

| Id | Question | Kind |
|---|---|---|
| [EA01](../NestLight.Experiments/Experiments/Automatic/EA01_Baseline.md) | How much does one `Highlight` call cost? | Automatic |
| [EA02](../NestLight.Experiments/Experiments/Automatic/EA02_SkipScan.md) | Does skipping the scan without language ids help? | Automatic |
| [EA03](../NestLight.Experiments/Experiments/Automatic/EA03_TextCopy.md) | How much does the `GetText()` copy cost? | Automatic |
| [EA04](../NestLight.Experiments/Experiments/Automatic/EA04_Registry.md) | Does building the registry per buffer matter? | Automatic |
| [EA05](../NestLight.Experiments/Experiments/Automatic/EA05_Allocations.md) | Where do the allocations of `Highlight` come from? | Automatic |
| [EA06](../NestLight.Experiments/Experiments/Automatic/EA06_Density.md) | Does the cost stay linear as the strings multiply? | Automatic |
| [EA07](../NestLight.Experiments/Experiments/Automatic/EA07_Interpolations.md) | Does one string with many interpolations scale? | Automatic |
| [EA08](../NestLight.Experiments/Experiments/Automatic/EA08_Throughput.md) | Is any tokenizer much slower than the others? | Automatic |
| [EA09](../NestLight.Experiments/Experiments/Automatic/EA09_Malformed.md) | Does bad input make the cost explode? | Automatic |
| [EA10](../NestLight.Experiments/Experiments/Automatic/EA10_CompletionLatency.md) | Is completion fast on large files? | Automatic |
| [EA11](../NestLight.Experiments/Experiments/Automatic/EA11_CompletionLimits.md) | Do the limits of the completion change its cost? | Automatic |
| [EA12](../NestLight.Experiments/Experiments/Automatic/EA12_VocabularyConsistency.md) | Does the tokenizer agree with the vocabulary? | Automatic |
| [EA13](../NestLight.Experiments/Experiments/Automatic/EA13_RankingQuality.md) | Is nearest-first the best order for the words of the document? | Automatic |
| [EA14](../NestLight.Experiments/Experiments/Automatic/EA14_ScopeAndSavings.md) | Which words should completion offer? | Automatic |
| [EA15](../NestLight.Experiments/Experiments/Automatic/EA15_CompletionFastPath.md) | Does sharing the scan and not creating the words bring completion under a frame? | Automatic |
| [EA16](../NestLight.Experiments/Experiments/Automatic/EA16_SimilarLatency.md) | Does the second stage of the completion fit in a frame? | Automatic |
| [EA17](../NestLight.Experiments/Experiments/Automatic/EA17_SimilarRecovery.md) | Does the second stage recover the word after one mistake? | Automatic |
| [EA18](../NestLight.Experiments/Experiments/Automatic/EA18_SimilarNoise.md) | Does it get in the way when the prefix is right? | Automatic |
| [EA19](../NestLight.Experiments/Experiments/Automatic/EA19_SimilarRobustness.md) | Is completion still robust with the second stage? | Automatic |
| [EA20](../NestLight.Experiments/Experiments/Automatic/EA20_PreviousWord.md) | Does the word before the caret help to rank the suggestions? | Automatic |
| [EA21](../NestLight.Experiments/Experiments/Automatic/EA21_SameLanguageWords.md) | Do the words of the same language come first? | Automatic |
| [EA22](../NestLight.Experiments/Experiments/Automatic/EA22_GrammarPosition.md) | Does the place in the grammar help to rank the suggestions? | Automatic |
| [EA23](../NestLight.Experiments/Experiments/Automatic/EA23_SqlSchema.md) | Does the schema read from the SQL of the document help? | Automatic |
| [EA24](../NestLight.Experiments/Experiments/Automatic/EA24_CountAndDistance.md) | Do the words used most often come before the nearest ones? | Automatic |
| [EA25](../NestLight.Experiments/Experiments/Automatic/EA25_ContextRobustness.md) | Is completion robust with the context rankings on? | Automatic |
| [EA26](../NestLight.Experiments/Experiments/Automatic/EA26_KeywordOrder.md) | Do the words of the file and the most used keywords come first where no rule decides? | Automatic |
| [EA27](../NestLight.Experiments/Experiments/Automatic/EA27_HeadKeywords.md) | Do a few keywords still come before the words of the file? | Automatic |
| [EA28](../NestLight.Experiments/Experiments/Automatic/EA28_ShortWords.md) | Should words of two letters be offered? | Automatic |
| [EA29](../NestLight.Experiments/Experiments/Automatic/EA29_SimilarNoiseShortPrefix.md) | Does the similar-words stage make noise with short prefixes? | Automatic |
| [EA30](../NestLight.Experiments/Experiments/Automatic/EA30_ShortWordsRestated.md) | EA28 with a criterion that can be met | Automatic |
| [EA31](../NestLight.Experiments/Experiments/Automatic/EA31_CombinationMatrix.md) | Does every host work with every embedded language? | Automatic |
| [EA33](../NestLight.Experiments/Experiments/Automatic/EA33_IncrementalLocate.md) | Does scanning only the lines around the edit make `Locate` faster? | Automatic |
| [EA34](../NestLight.Experiments/Experiments/Automatic/EA34_AutoDetectorCost.md) | What does an automatic language detector cost in time and memory? | Automatic |
| [EM01](../NestLight.Experiments/Experiments/Manual/EM01_CombinationReview.md) | What does the plugin do, case by case, with every host and language? | Manual |
| [EM02](../NestLight.Experiments/Experiments/Manual/EM02_VisualGallery.md) | Do random snippets, painted in a browser, show a color that is wrong or missing? | Manual |
