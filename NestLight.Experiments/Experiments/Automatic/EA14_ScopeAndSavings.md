# EA14: where the words come from, and how many keystrokes completion saves

**Hypothesis.** Offering the words of the whole document (host code included, as Visual Studio Code does) saves more keystrokes than offering only the words inside embedded strings, or only those of the string being typed.

**Test.** The corpus of EA13 at locality 0.5. 3,217 words typed one character at a time, up to 5. The completion is accepted at the first prefix where the right word is within the first 5; saving = length of the word - characters typed - 1 for the accepting key. Four scopes, always with the keywords except the first.

**Criterion.** The saving of the whole document within 2 percentage points of the best narrower scope, or above it.
