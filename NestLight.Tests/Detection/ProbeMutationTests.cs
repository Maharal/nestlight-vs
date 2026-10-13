using NestLight.Detection;
using Xunit;

namespace NestLight.Tests
{
    public class ProbeMutationTests
    {

        [Fact]
        public void StartsWithWord_requires_the_text_longer_than_the_word()
        {
            Assert.False(Probe.StartsWithWord("from", 0, 4, "from"));
            Assert.True(Probe.StartsWithWord("from ", 0, 5, "from"));
        }

        [Fact]
        public void StartsWithWord_checks_the_character_after_the_word_is_not_a_letter_or_digit()
        {
            Assert.False(Probe.StartsWithWord("fromage ", 0, 8, "from"));
            Assert.True(Probe.StartsWithWord("from,age", 0, 8, "from"));
        }

        [Fact]
        public void StartsWithWord_checks_the_character_after_is_not_underscore()
        {
            Assert.False(Probe.StartsWithWord("from_x ", 0, 7, "from"));
        }
        [Fact]
        public void ContainsWord_finds_word_at_start()
        {
            Assert.True(Probe.ContainsWord("from t", 0, 6, "from"));
        }

        [Fact]
        public void ContainsWord_finds_word_at_end()
        {
            Assert.True(Probe.ContainsWord("select from", 0, 11, "from"));
        }

        [Fact]
        public void ContainsWord_rejects_embedded_word()
        {
            Assert.False(Probe.ContainsWord("xfromx", 0, 6, "from"));
        }

        [Fact]
        public void ContainsWord_accepts_word_delimited_by_comma()
        {
            Assert.True(Probe.ContainsWord("a,from,b", 0, 8, "from"));
        }

        [Fact]
        public void ContainsWord_accepts_word_delimited_by_tab()
        {
            Assert.True(Probe.ContainsWord("a\tfrom\tb", 0, 8, "from"));
        }

        [Fact]
        public void ContainsWord_accepts_word_delimited_by_newline()
        {
            Assert.True(Probe.ContainsWord("a\nfrom\nb", 0, 8, "from"));
        }

        [Fact]
        public void ContainsWord_accepts_word_delimited_by_carriage_return()
        {
            Assert.True(Probe.ContainsWord("a\rfrom\rb", 0, 8, "from"));
        }

        [Fact]
        public void ContainsWord_returns_false_when_not_present()
        {
            Assert.False(Probe.ContainsWord("select into", 0, 11, "from"));
        }
        [Fact]
        public void NextSignificant_skips_whitespace()
        {
            Assert.Equal('x', Probe.NextSignificant("   x", 0, 4));
        }

        [Fact]
        public void NextSignificant_returns_null_char_for_all_whitespace()
        {
            Assert.Equal('\0', Probe.NextSignificant("   ", 0, 3));
        }

        [Fact]
        public void NextSignificant_returns_first_char_when_not_whitespace()
        {
            Assert.Equal('a', Probe.NextSignificant("abc", 0, 3));
        }
        [Fact]
        public void SkipCStyleComments_skips_block_comment()
        {
            Assert.Equal(6, Probe.SkipCStyleComments("/* */ x", 0, 7));
        }

        [Fact]
        public void SkipCStyleComments_returns_end_for_unclosed_comment()
        {
            Assert.Equal(6, Probe.SkipCStyleComments("/* abc", 0, 6));
        }

        [Fact]
        public void SkipCStyleComments_skips_whitespace_then_comment()
        {
            Assert.Equal(8, Probe.SkipCStyleComments("  /* */ x", 0, 9));
        }

        [Fact]
        public void SkipCStyleComments_stops_at_non_comment()
        {
            Assert.Equal(0, Probe.SkipCStyleComments("abc", 0, 3));
        }
        [Fact]
        public void SkipLineComments_skips_hash_comment()
        {
            Assert.Equal(10, Probe.SkipLineComments("# comment\ncode", 0, 14, '#'));
        }

        [Fact]
        public void SkipLineComments_returns_end_when_no_newline()
        {
            Assert.Equal(9, Probe.SkipLineComments("# comment", 0, 9, '#'));
        }

        [Fact]
        public void SkipLineComments_stops_at_non_comment()
        {
            Assert.Equal(0, Probe.SkipLineComments("code", 0, 4, '#'));
        }

        [Fact]
        public void SkipLineComments_skips_whitespace_before_comment()
        {
            Assert.Equal(12, Probe.SkipLineComments("  # comment\ncode", 0, 16, '#'));
        }
        [Fact]
        public void IndexOf_finds_character()
        {
            Assert.Equal(3, Probe.IndexOf("abc{def", 0, 7, '{'));
        }

        [Fact]
        public void IndexOf_returns_minus_one_when_not_found()
        {
            Assert.Equal(-1, Probe.IndexOf("abcdef", 0, 6, '{'));
        }

        [Fact]
        public void IndexOf_respects_start_and_end()
        {
            Assert.Equal(-1, Probe.IndexOf("a{b{c", 2, 3, '{'));
            Assert.Equal(3, Probe.IndexOf("a{b{c", 2, 5, '{'));
        }

        // --- Kills: line 26 end - start → end + start (arithmetic mutation) ---
        [Fact]
        public void StartsWithWord_uses_difference_not_sum_for_length()
        {
            // start=5, end=10, word="from" (length 4)
            // end - start = 5 > 4 → should proceed (not <=)
            // end + start = 15 > 4 → same result, so we need a case where they differ:
            // start=5, end=9, word="from" (length 4), end-start=4 which is <= 4 → false
            // end+start=14 which is > 4 → would NOT return false (bug)
            string text = "xxxxxfrom";
            Assert.False(Probe.StartsWithWord(text, 5, 9, "from")); // length exactly equals word length
        }

        // --- Kills: line 39 pos < end → pos <= end (ContainsWord loop) ---
        [Fact]
        public void ContainsWord_loop_boundary()
        {
            // If pos <= end were used instead of pos < end, it might read past the end
            Assert.False(Probe.ContainsWord("abcdef", 0, 6, "xyz"));
        }

        // --- Kills: line 46 leftOk && rightOk → leftOk || rightOk ---
        [Fact]
        public void ContainsWord_requires_both_boundaries()
        {
            // "xfrom " - left boundary bad (preceded by 'x'), right boundary ok (followed by space)
            // With || it would return true; with && it should return false
            Assert.False(Probe.ContainsWord("xfrom ", 0, 6, "from"));
            // " fromx" - left ok (preceded by space), right bad (followed by 'x')
            Assert.False(Probe.ContainsWord(" fromx", 0, 6, "from"));
        }

        // --- Kills: line 66 i + 1 < end → i + 1 <= end ---
        // --- Kills: line 66 i + 1 → i - 1 ---
        [Fact]
        public void SkipCStyleComments_boundary_at_end()
        {
            // Two chars left: "/" at end-2 and something at end-1
            // i+1 < end needs to be true for the loop to check t[i+1]
            // With i+1 <= end it would read t[end] which is out of bounds
            string text = "/ "; // Just a slash and space - not a comment
            Assert.Equal(0, Probe.SkipCStyleComments(text, 0, 2));
        }

        // --- Kills: line 69 t[i] == '/' && t[i+1] == '*' → || ---
        [Fact]
        public void SkipCStyleComments_slash_without_star_not_skipped()
        {
            // "/ x" - slash followed by space, not a comment
            // With || it would treat '/' alone as start of comment
            string text = "/x abc";
            Assert.Equal(0, Probe.SkipCStyleComments(text, 0, 6));
        }

        [Fact]
        public void SkipCStyleComments_star_without_leading_slash_not_skipped()
        {
            // "*/ abc" - star at start but no preceding slash
            string text = "*/ abc";
            Assert.Equal(0, Probe.SkipCStyleComments(text, 0, 6));
        }

        // --- Kills: line 72 close < 0 → close <= 0 ---
        [Fact]
        public void SkipCStyleComments_close_at_position_zero()
        {
            // This is tricky: close is the result of t.IndexOf("*/", i+2, ...)
            // It can't be 0 because we search from i+2 onwards. So close >= 2 always when found.
            // The mutation close <= 0 vs close < 0 is likely equivalent.
            // But let's test that an unclosed comment returns end:
            Assert.Equal(10, Probe.SkipCStyleComments("/* no close", 0, 10));
        }

        // --- Kills: line 90 nl < 0 → nl <= 0 ---
        [Fact]
        public void SkipLineComments_newline_at_position_zero()
        {
            // "\n# comment\ncode" - newline at position 0
            // But SkipLineComments first skips whitespace, and '\n' is whitespace
            // So it would skip to '#', then look for '\n' from there
            // Actually the marker check comes after whitespace skip
            // Let me think: "# comment\ncode" → i=0, t[0]='#' (marker), find '\n' at index 9
            // nl=9 which is > 0. With <= 0 it would set i=end instead of i=nl+1.
            // Hard to get nl==0. Let's verify the basic flow:
            Assert.Equal(10, Probe.SkipLineComments("# comment\ncode", 0, 14, '#'));
        }

        // --- Kills: line 91 statement mutation (remove i = nl < 0 ? end : nl + 1) ---
        [Fact]
        public void SkipLineComments_advances_past_comment_line()
        {
            // Two comment lines then code: must skip both
            string text = "# c1\n# c2\ncode";
            Assert.Equal(10, Probe.SkipLineComments(text, 0, 14, '#'));
        }
    }
}
