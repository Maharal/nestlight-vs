# EA09: malformed and pathological input

**Hypothesis.** Code being typed is often malformed. An unterminated string or comment early in a large file, deep nesting, or a very long line can make the scan or a tokenizer quadratic.

**Test.** For each host: an unterminated marked string and an unterminated block comment at the top of a file of 250,000 characters, deep nesting (200 and 400 levels), and one line of 250,000 characters. Each case at N and at 2N. Cases under 5 ms at 2N are ignored.

**Criterion.** Doubling the input multiplies the time by less than 2.5 (linear is 2, quadratic is 4) in every case.
