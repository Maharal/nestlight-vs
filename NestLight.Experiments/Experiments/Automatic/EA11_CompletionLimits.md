# EA11: do the limits of the completion change its cost?

**Hypothesis.** The limit on the number of suggestions (100) and the minimum word length (3) were picked without measuring. If the cost is in the scan, no value of them is a performance lever.

**Test.** The two shapes of EA10 at 12,000 lines (JavaScript), with the maximum number of suggestions at 10, 100, 1,000 and 10,000 and the minimum word length at 1, 3 and 5, one knob at a time from the default. Median of 100 runs, against the faster of two measurements of the default (first and last).

**Criterion.** Every combination within 25% of the default, in both shapes.
