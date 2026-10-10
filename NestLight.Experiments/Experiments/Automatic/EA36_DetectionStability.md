# EA36: Does the language of a string flicker while it is typed?

**Hypothesis.** The detector is asked again at every edit. While a person types a string one character at a time, the answer should settle: nothing, then the language, and nothing again only if the string is broken. If it flips back and forth, the color of the string blinks at every key.

**Test.** For 100 random snippets of each language the detector has a rule for (seeds 1 to 100, written on several lines), the detector is asked about every prefix of the snippet, as if the string were typed from its first character. The number of changes of the answer along the prefixes is counted (nothing to a language, a language to nothing and a language to another all count), and so is the length at which the right language is first recognized, as a share of the whole snippet.

**Criterion.** For every language, at least 95% of the snippets change their answer at most twice (nothing, the language, nothing) and none is ever recognized as another language.
