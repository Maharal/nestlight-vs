# EA18: does the second stage get in the way when the prefix is right?

**Hypothesis.** If the second stage ran whenever the first found few items, a correct prefix would often get a list with words that are only similar by chance. Running it only when nothing matched (`FuzzyBelow` = 1) keeps that rare.

**Test.** The corpus of EA13. A sample of the words of the embedded strings typed correctly, 3 to 8 letters, with the rest of the word removed. The engine with `FuzzyBelow` = 1, 3 and 5; the cases where similar items are added are counted, apart for those where the first stage found something and those where it found nothing.

**Criterion.** With `FuzzyBelow` = 1, similar items are added in at most 5% of the cases. The default is chosen among the values that meet it.
