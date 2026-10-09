# EA01: baseline cost of one `Highlight` call

**Hypothesis.** Tokenizing is cheap enough that the number of languages and the scan of the host are not what could make the editor slow.

**Test.** `Highlight(text)` (host scan + tokenization) for the 4 hosts, with no marked string and with one unit in 10 carrying a marked string, at 1.2k, 12k and 60k lines.

**Criterion.** Every case stays under 16 ms (one frame) at 60k lines.
