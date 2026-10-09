# EA07: many interpolations in one string

**Hypothesis.** The engine clips every token against the interpolations of its string starting from the first one, so a single large template with many interpolations costs O(tokens x interpolations). Suspected from reading `HighlightEngine.AddClipped`.

**Test.** One marked HTML string with 100, 1,000 and 10,000 interpolations, in JavaScript, C# and Python.

**Criterion.** Ten times more interpolations cost less than 20 times the time (linear is 10x, quadratic is 100x), for every host.
