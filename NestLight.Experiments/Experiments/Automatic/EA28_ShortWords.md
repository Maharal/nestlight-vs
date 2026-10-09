# EA28: should words of two letters be offered?

**Hypothesis.** The completion skips the words under 3 letters, so `id`, `db`, `in`, `uv` and `if` are never offered, and they are among the most written words of SQL, YAML and shaders. Offering them, after all the longer words, lets a person who types `i` find `id` without crowding the list for the person who wants a longer word.

**Test.** The corpus of 500 snippets for each language (the odd files) and the hand-written files of the review: 600 words per language typed with 1 or 2 letters, only the words that exist elsewhere in the file or are keywords. The engine of the plugin as it is (minimum length 3), with the minimum lowered to 2 for every word, and with the two-letter words in a tier after all the others.
