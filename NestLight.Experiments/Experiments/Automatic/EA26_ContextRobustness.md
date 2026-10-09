# EA26: completion with the context rankings on incomplete and cut code

**Hypothesis.** EA20 again with every context feature on (the previous word, the words of the language, the grammar of SQL, CSS and HTML, the schema of the SQL, the blend of count and distance), over the structured files that have the statements, rules and tags those features read. The look-behind of the grammar and of the schema, the pointers into the ranges of the strings, the ranked words and the short words offered after a context must not throw on a text cut anywhere, repeat a word, break the limits, or offer an exact suggestion that does not start with what was typed.

**Test.** EA20's: every prefix cut at a stride and every single-character deletion at a stride (12,269 texts), the caret at the start, at the end and at 5 random places (85,633 carets, 35,116 of them inside embedded code), and the same position with a mistake in the word (16,841), with every feature of `CompletionFeatures` on, schema and blend included.

**Criterion.** Zero violations.
