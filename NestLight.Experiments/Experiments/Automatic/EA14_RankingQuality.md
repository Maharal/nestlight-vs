# EA14: order of the words of the document

**Hypothesis.** Listing the words that already exist in the document nearest to the caret first puts the right word among the first five more often than alphabetical, by frequency or by first appearance.

**Test.** 50 generated files, typed 1, 2 and 3 characters of a sample of the words of the embedded strings (9,651 cases; 95% are reachable: the word exists elsewhere or is a keyword). The position of the right word in five orderings. The same files are generated with locality 0.0, 0.5 and 0.9.

**Criterion.** At locality 0.5, nearest-first within the first 5 at least 5 percentage points above alphabetical.
