# EA24: do the words used most often come before the nearest ones?

**Hypothesis.** The order by distance alone sends to the end a word that is used all over the file and is not close to the caret, while the nearest word may have been used once. A blend, `ln(1 + count) - weight * ln(1 + distance)`, puts the meant word in the first 5 more often, whatever the locality of the code. The engine has no edit history, so how near an occurrence is to the caret stands for how recently the word was used; real recency (the words accepted or typed last) would need the editor to tell the engine and is not done here.

**Test.** EA20's probes on files of three localities (0, 0.5, 0.9), 50 each, with the previous word, the language and the grammar on. Five orders of the words of the document and of the words that followed the context: distance alone, count alone, and the blend with a weight of 1, 0.5 and 0.25. Also, apart from the criterion, the same orders with no other feature, and the start of a session.

**Criterion.** The best of the four other orders at least 1.5 points better within the first 5 than distance alone at locality 0.5; at no locality worse by more than 1 point; no language worse by more than 1 point; the session under 16 ms at 60,000 lines.
