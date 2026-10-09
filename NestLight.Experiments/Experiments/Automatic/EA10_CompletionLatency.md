# EA10: completion latency against the size of the file

**Hypothesis.** Every keystroke in an embedded string scans the host to find the string (`Locate`) and the whole text for words (`Suggest`). That is cheap on a normal file but could be noticeable on a large one, mostly when thousands of distinct words share the prefix.

**Test.** For each host, a file of 1,200, 12,000 and 60,000 lines with the caret at the end of an open `comp` in a marked SQL string. Two shapes: EA01's file (typical code) and a file where every line declares a new `compNNNNN` identifier. Time of `Locate` alone and of `Locate` + `Suggest`, without the text copy of the editor (EA03).

**Criterion.** `Locate` + `Suggest` under 16 ms at the largest size, in every host and shape.
