# EA20: does the word before the caret help to rank the suggestions?

**Hypothesis.** The words that already followed the same word (with the same punctuation) elsewhere in the document are the likely ones: after `from ` the word that followed `from` before, after `display: ` the value that followed `display:`, after `group ` the `BY`. Putting them first puts the meant word in the first 5 more often than the order by distance alone.

**Test.** 50 generated files with structure, a sample of the words of the embedded strings typed with 1 to 3 letters (9,564 prefixes, 9,162 reachable), the list the editor gets. The order by distance alone against the previous word first; the same words ordered by the nearest occurrence of the context. Also the start of a session on files of 1,200 to 60,000 lines.

**Criterion.** At least 3 points more within the first 5 over all the reachable cases; no language more than 1 point worse; the session under 16 ms at 60,000 lines in every host.
