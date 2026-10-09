# EA22: does the place in the grammar help to rank the suggestions?

**Hypothesis.** What belongs at the caret can be told from a few characters of look-behind ([the grammar of each language](../../../NestLight/Completion/Languages), no parser): a table after `FROM`, a column after `SELECT`, `BY` after `GROUP`, the properties inside the braces of CSS and the values of the property after its colon, the attributes of the tag inside `<button `. Putting what belongs first and what does not last (nothing is dropped) puts the meant word in the first 5 more often than the previous word and the language alone, and fits in a frame.

**Test.** EA20's probes, four variants: the order by distance alone; the grammar; the previous word and the language (what the plugin ran with before); all three. Reported by the place of the caret. Also the start of a session on files of 1,200 to 60,000 lines.

**Criterion.** Adding the grammar to the previous word and the language is at least 2 points better within the first 5; SQL, CSS and HTML each do not fall; the session under 16 ms at 60,000 lines in every host.
