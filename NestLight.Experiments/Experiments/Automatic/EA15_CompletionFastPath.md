# EA15: sharing the scan and not creating the words of the completion

**Hypothesis.** EA10 found that completion takes more than a frame above ~1 million characters, mostly the scan of the host. In Visual Studio a session scans the text twice (once to decide whether to open, once to fill the list) after the classifier already did it for the same snapshot, and the word pass created a string and a sort entry for every match. Sharing one scan between the classifier and the completion, and creating a word only when it is offered, brings the worst case under a frame.

**Test.** EA10's files. The start of one completion session, as the editor makes it (`Locate`, `Locate` again, `Suggest`) over a new text instance each time, in two cases: nothing shared, and the scan shared (the classifier has already highlighted that text, not timed). Plus `Suggest` alone. Check that nothing changes: 897 unit tests, among them a comparison of the order of the words with the first, straightforward implementation (dictionary and sort) on 4,800 random carets of small generated programs (those inside embedded strings are compared), and a test of two words at the same distance on both sides.

**Criterion.** The session with the scan shared stays under 16 ms at 60,000 lines, in every host and shape.
