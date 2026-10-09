# EA03: the text copy on every edit

**Hypothesis.** On every new snapshot the classifier copies the whole text into a string. Above ~85 KB (about 42k characters) that string lands on the Large Object Heap, which only gen2 collections reclaim: the copy costs time, and the collections cost more.

**Test.** Simulated typing on a synthetic C# file, 300 edits per size. Each edit copies the text into a new string and highlights it. Three variants: copy + `Highlight` (current), copy only, `Highlight` only on an existing string. Mean time per edit, so that GC pauses count.

**Criterion.** Removing the copy would save at least 1 ms per edit on the 400,000-character file. *(Restated when automated: the first wording said "1 ms or 30%" and mixed the two.)*
