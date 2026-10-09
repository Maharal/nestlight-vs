# EA22: do the words of the same language come first?

**Hypothesis.** A word written in the code of another string of the same language (a column in another SQL string) is likelier than a word of the host code or of a string of another language that happens to start with the same letters, even when the other one is nearer to the caret. Putting the words of the language first, and taking the context of the previous word from them only, puts the meant word in the first 5 more often. It is EA15's question (where should the words come from) asked as a ranking and not as a filter: nothing is dropped, the other words come after.

**Test.** EA21's probes, four variants: the order by distance alone; the words of the language first; the previous word; both. Interpolations are host code. Also the start of a session on files of 1,200 to 60,000 lines with the scan shared.

**Criterion.** Adding the words of the language to the previous word is at least 2 points better within the first 5; no language more than 1 point worse; the session under 16 ms at 60,000 lines in every host.
