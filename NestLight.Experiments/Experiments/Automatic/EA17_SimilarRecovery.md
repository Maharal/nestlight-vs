# EA17: does the second stage recover the word after one mistake, and which tie-break works?

**Hypothesis.** When the typed text has one edit (an extra letter, a missing one, a wrong one, two swapped) in a prefix of 4 to 8 letters, the meant word is among the first 5 suggestions in most cases. Among words the same number of edits away, the nearest to the caret is no worse a tie-break than the most frequent.

**Test.** The corpus of EA13 (50 generated files, locality 0.5). A sample of the words of the embedded strings is typed as a prefix of 4 to 8 letters with one edit of each kind at a random place over the prefix (the first letter included). Only the reachable cases count (the word exists elsewhere in the document or is a keyword). The list is also reordered inside each group of the same kind and distance: nearest first (the engine), most frequent first, most frequent and then nearest.

**Criterion.** The meant word within the first 5 in at least 70% of the reachable cases with the best of the three tie-breaks; if more than one reaches 70%, the best result enters, and on a tie the nearest stays.
