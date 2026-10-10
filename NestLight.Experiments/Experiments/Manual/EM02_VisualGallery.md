# EM02: visual gallery of random snippets

**Hypothesis.** EM01 reads one fixed sample per language, so it only finds what that sample happens to contain. Random snippets, from a seed, hit more shapes of the same language (a join or no join in SQL, a nested rule or a flat one in CSS, an anchor in YAML), and seeing the colors painted in a browser finds what a list of tokens hides: a piece that has the color of another role, a piece with no color where its neighbors have one, a string the detector misses.

**Test (manual).** `--gallery [dir]` ([EM02_VisualGallery](EM02_VisualGallery.cs)) writes, for every host, every language, every way to mark the string and with and without interpolation, `--samples n` random snippets (default 3, from [RandomSnippets](../../Generator/RandomSnippets.cs)), runs the real highlighter over each and paints the result in HTML with the colors of an editor (dark and light). Nothing needs Windows or Visual Studio: open `index.html` in a browser.
- One page per host and language (`<host>/<language>.html`), a card per snippet with its way to mark, its seed and the number of strings and tokens. Hovering a piece shows its token type.
- A gray background is a piece inside the string with no token: plain text, or a gap in the tokenizer. Blanks are not painted.
- The same snippets with the **marker removed** and the automatic detection on: the card says what the detector recognized (a language with no rule in the detector is not expected to be recognized).
- `FINDINGS.md` lists the cards worth reading first (a marked snippet that gave no string or no token).

```
dotnet run -c Release --project NestLight.Experiments -- --gallery
dotnet run -c Release --project NestLight.Experiments -- --gallery out --host python --language sql --samples 10
```

**Criterion.** None: the result is a list of findings, each with the page, the way to mark and the seed, which repeat the case.
