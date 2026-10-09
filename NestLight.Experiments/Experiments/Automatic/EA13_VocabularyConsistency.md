# EA13: does the tokenizer agree with the vocabulary?

**Hypothesis.** The words the completion offers and the words the tokenizers color live in two places. SQL, GraphQL, YAML and shader lists reuse the tokenizers' sets, but HTML tags and CSS properties were written by hand. A word the completion offers and the tokenizer then splits, or colors as plain text, shows the plugin does not know what it just offered.

**Test.** Every word of every vocabulary (SQL 154, GraphQL 28, GLSL 215, WGSL 162, JSON 3, YAML 17, HTML 130, CSS 227) in one or more contexts of its language, through the real tokenizer. It checks that one token covers exactly the word and that its type is the expected one (for SQL and YAML: different from the type of an unknown word in the same context).

**Criterion.** Every word is one token, and at least 95% of the words of each language are classified as expected.
