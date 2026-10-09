# EA10: completion latency against the size of the file

**Hypothesis.** Every keystroke in an embedded string scans the host to find the string (`Locate`) and the whole text for words (`Suggest`). That is cheap on a normal file but could be noticeable on a large one, mostly when thousands of distinct words share the prefix.

**Test.** For each host, a file of 1,200, 12,000 and 60,000 lines with the caret at the end of an open `comp` in a marked SQL string. Two shapes: EA01's file (typical code) and a file where every line declares a new `compNNNNN` identifier. Time of `Locate` alone and of `Locate` + `Suggest`, without the text copy of the editor (EA03).

**Criterion.** `Locate` + `Suggest` under 16 ms at the largest size, in every host and shape.

**Alternative tried and dropped: the words kept in memory.** Instead of reading the whole text for words at every request, an index of the words of the text (start, length, first letter) was kept and updated after each edit by comparing the old text with the new one and reading again only the words around the change. Same suggestions as reading the text (unit tests against the straightforward implementation, random edit chains, every caret of a document, several threads), but not worth it:
- On the files above with 12,000 lines of typical code, `Suggest` was 1.4x to 1.6x faster (about 0.5 to 0.6 ms per keystroke, against a frame of 16 ms); with one new word per line, 1.0x to 1.2x, because the cost is in the thousands of words that match, not in reading the text. With 1,200 lines the request already costs about 0.2 ms.
- Beyond the window of the word scan (more than 500,000 characters on each side of the caret, which is every file of 60,000 lines here) the engine reads the text as before, so there was no gain at all.
- Each edit made new arrays (more than 85 KB, so on the large object heap): about 65% more allocated per request and, over 500 requests at 12,000 lines, 61 collections of generation 2 instead of 2. That pause is the kind the editor's UI thread feels, and it is not in the medians.
- What was left of a request was `Locate`, the scan of the host (1 to 2 ms at 12,000 lines), larger than the `Suggest` that the index made faster. That is what EA33 attacks instead.
Measured on .NET 8 with synthetic code: the plugin runs on .NET Framework, whose collector behaves differently.
