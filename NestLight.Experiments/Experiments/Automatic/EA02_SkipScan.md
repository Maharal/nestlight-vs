# EA02: skip the scan when the text has no language id

**Hypothesis.** A string is only embedded code when a tag or comment carries a known language id. If the text contains none of the 17 ids, the scan could not find anything, so skipping it saves time on C# and Python, which have no shortcut of their own.

**Test.** A decorator around the real highlighter looks for each id with `IndexOf(OrdinalIgnoreCase)` and returns no tokens when none appears. Same files, with and without the decorator, measured in the same process.

**Criterion.** At least 2x faster on every file without marked strings, and no more than 1.1x slower on every file with them.
