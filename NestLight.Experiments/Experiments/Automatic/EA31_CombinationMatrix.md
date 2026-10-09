# EA31: every host with every embedded language

**Hypothesis.** The unit tests try each host with some languages and each language mostly in a JavaScript host. A combination nobody wrote a test for may be missed or read as another language.

**Test.** The [generator](../../../docs/experiments.md#the-code-generator) writes a file for each of the 168 applicable combinations; each is run through the scan and the highlighter at 1 and at 200 copies of the sample and compared with what the generator put there (strings, interpolations, language, tokens inside the strings). The time to highlight 200 copies is reported by host and language.

**Criterion.** Every applicable combination passes at both sizes.
