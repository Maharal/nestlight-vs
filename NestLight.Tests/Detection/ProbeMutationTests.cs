using NestLight.Detection;
using Xunit;

namespace NestLight.Tests
{
    public class ProbeMutationTests
    {
        // StartsWithWord: mutations at L26 (equality on length), L28 (logical/boolean negation)

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

        // ContainsWord: mutations at L39 (equality), L42-L46 (logical/equality in boundary checks)

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

        // NextSignificant: mutations at L59 (equality on loop)

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

        // SkipCStyleComments: mutations at L66-L78

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

        // SkipLineComments: mutations at L82-L96

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

        // IndexOf: mutations at L53

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
    }
}
