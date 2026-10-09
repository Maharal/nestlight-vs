# EA17: does the second stage of the completion fit in a frame?

**Hypothesis.** The second stage compares what was typed with every distinct word of the document that passes two cheap filters (the first letter, the length) and keeps the best few. Even forced to run in every session, with thousands of words one edit away, a session still fits in the 16 ms of a frame.

**Test.** EA16's files and session (the scan shared, a new text instance each time), 4 hosts, typical code and distinct words, 1,200 / 12,000 / 60,000 lines, with the second stage forced (asked for below any number of items). Two typed texts: `comp` (everything is compared, nothing new is found) and `cmop` (in the distinct-words file all 60,000 words are one edit away). Also the same session with the second stage off, and the bytes allocated.

**Criterion.** Under 16 ms at 60,000 lines in every host, shape and typed text.
