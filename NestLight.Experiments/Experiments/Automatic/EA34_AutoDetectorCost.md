# EA34: what an automatic language detector costs

**Hypothesis.** An automatic detector that guesses the language of the strings nobody marked, with one cheap heuristic per language behind a common interface (the strategy pattern), makes `Highlight` at most 1.5x slower on files where one string in four looks like code, allocates almost nothing for the strings it asks about, and keeps almost nothing in memory.

**Test.** The detector of the plugin (`NestLight/Detection`): five strategies (SQL, HTML, JSON, CSS, GraphQL), each an `ILanguageDetector` that reads the text where it is, and a context (`LanguageDetector`) that asks them and keeps the highest score, optionally ruling a strategy out by the first character before asking it. The files are EA05's (typical code, a marked string in one unit out of ten) with 8 unmarked strings added to each unit: 6 ordinary and 2 of code. The 4 hosts, at the second and the largest size. Measured on the same file, in the same process:

- **A**: the current `Highlight`.
- **B**: finding the unmarked strings (a simplified lexer; in the plugin the host scanner would do it in the pass it already makes, so B is an upper bound of that part and not a cost the detector adds).
- **C**: B plus asking the detector for every string, with the first-character check (gated) and without it (every strategy).
- **D**: everything: `Highlight`, finding, detecting and tokenizing the strings detected.

The cost of detection is C minus B. Retained memory is what a built detector keeps alive. The report also has the cost of each strategy alone, and the right and wrong answers of the detector on pools of strings (ordinary ones, some of them on purpose hard, and code), which says how the heuristics fare but not how they would fare on real code.

**Criterion.** At the largest size, in every host: D at most 1.5x the time of A; the detection allocates at most 100 bytes per string it asks about; and the detector keeps at most 100 KB.
