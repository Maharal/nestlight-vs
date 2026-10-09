# EA12: completion on incomplete and cut code

**Hypothesis.** Completion runs while code is being typed, so it sees unterminated strings, half-written interpolations and carets anywhere. For every text and caret, `Locate` and `Suggest` must not throw, the site must lie inside the text around the caret with only word characters, and the suggestions must start with the typed prefix, add something to it and not repeat.

**Test.** 50 generated files. Every prefix cut and every single-character deletion at a stride (12,450 texts), the caret at the start, at the end and at 5 seeded random positions (86,900 carets, 16,374 of them inside the code of an embedded string). 6 invariants on each result.

**Criterion.** Zero violations.
