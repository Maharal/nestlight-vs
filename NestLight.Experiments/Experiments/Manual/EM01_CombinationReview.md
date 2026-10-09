# EM01: review of every host with every embedded language

**Hypothesis.** EA31 says every combination is found and tokenized, not whether the colors and the suggestions are good. Reading what the plugin does with each combination, one by one, finds what a count cannot: a word that should have a color and has none, a suggestion that does not belong (a variable of the host offered inside SQL), a caret in host code that gets a site.

**Test (manual).** `--manual <dir>` generates the 168 combinations ([EM01_CombinationReview](EM01_CombinationReview.cs)), runs the plugin over them and writes, for each one, a Markdown file and an HTML page (the artifacts) in `<dir>/<host>/`:
- the generated **source**;
- the **final view**: the source with every token written `⟦text|role⟧`, so the color of each piece can be read, and the same view painted in the HTML page (a legend names each role);
- the strings found, every token with its position and type, and the words of the string with no token;
- the **completion**: each word of the string typed with 1 and 2 letters, with the place in the grammar, the rank of the intended word and the top 10 (`*` keyword, `~` similar word);
- the **carets that must get nothing**, in host code and inside interpolations.

`INDEX.md` lists the combinations with the counts of gaps, misses and wrong sites, to say what to read first. A count is a pointer, not a verdict: plain text in Markdown is a gap that is correct. `--host` and `--language` narrow the run, so a fix can be checked on the cases it touches.

**Criterion.** None: the result is a list of findings, each with the file and the case.
