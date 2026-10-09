# EA19: completion with similar words on incomplete and cut code

**Hypothesis.** Completion runs while code is being typed. For every text and caret, `Suggest` must not throw; the site must lie inside the text with only word characters; the exact suggestions must start with the typed text; the similar ones must be at the distance they claim (as the definition computes it), between 1 and the tolerance, with the first letter typed, after the exact ones, keywords before words and fewer edits first; nothing repeats; and the limits hold.

**Test.** EA12's: 50 generated files, every prefix cut and every single-character deletion at a stride (12,450 texts), the caret at the start, at the end and at 5 random places (86,900 carets, about 16,000 of them inside embedded code); plus, at each of those, the same text with a one-letter mistake put in the word under the caret. The distance of every similar item is recomputed with the whole matrix.

**Criterion.** Zero violations.
