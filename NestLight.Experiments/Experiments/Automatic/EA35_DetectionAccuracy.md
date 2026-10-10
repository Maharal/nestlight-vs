# EA35: Does the automatic detector recognize the code, and leave the text alone?

**Hypothesis.** EA34 says what the detector costs, not whether it is right. Its heuristics were tried on a few dozen hand-written strings. On random code of each language, written on one line and on several, it recognizes the code; and on ordinary strings (messages, paths, prose, on purpose some that look like code) it stays silent, because a false positive paints plain text.

**Test.** RandomSnippets writes 200 snippets of each language the detector has a rule for (SQL, HTML, JSON, CSS, GraphQL), from seeds 1 to 200, in two shapes: as written (several lines) and with every line break turned into a space (one line). The detector (the one of the plugin, gated) is asked about each. A hit is the language of the snippet; a miss is nothing; a confusion is another language. The ordinary strings are the pool of EA34 plus some written on several lines and some that start with a word of code. The detector is asked about each: any answer is a false positive.

**Criterion.** For every language and both shapes, at least 90% of the snippets are recognized, none is recognized as another language, and at most 1% of the ordinary strings are recognized as code.
