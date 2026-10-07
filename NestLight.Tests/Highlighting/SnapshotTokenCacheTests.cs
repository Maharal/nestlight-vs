using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>The cache between the editor and the highlighter, with a fake snapshot type and a fake highlighter.</summary>
    public class SnapshotTokenCacheTests
    {
        private sealed class Snapshot
        {
            public readonly string Text;
            public Snapshot(string text) { Text = text; }
        }

        private sealed class CountingHighlighter : IHighlighter
        {
            public int Calls;
            public Func<string, IReadOnlyList<Token>> Behavior = text => new Token[0];
            public IReadOnlyList<Token> Highlight(string text) { Calls++; return Behavior(text); }
        }

        private static Token[] Spread(string text)
        {
            // one token of length 2 every 5 characters
            return Enumerable.Range(0, text.Length / 5).Select(i => new Token(i * 5, 2, "t" + i)).ToArray();
        }

        private static SnapshotTokenCache<Snapshot> Cache(CountingHighlighter highlighter)
        {
            return new SnapshotTokenCache<Snapshot>(highlighter, s => s.Text);
        }

        [Fact]
        public void The_same_snapshot_is_analyzed_once()
        {
            var h = new CountingHighlighter();
            var cache = Cache(h);
            var snapshot = new Snapshot("0123456789");

            cache.TokensIn(snapshot, 0, 5);
            cache.TokensIn(snapshot, 5, 10);
            cache.TokensIn(snapshot, 0, 10);

            Assert.Equal(1, h.Calls);
        }

        [Fact]
        public void A_new_snapshot_is_analyzed_again_even_with_equal_text()
        {
            var h = new CountingHighlighter();
            var cache = Cache(h);

            cache.TokensIn(new Snapshot("abc"), 0, 3);
            cache.TokensIn(new Snapshot("abc"), 0, 3);

            Assert.Equal(2, h.Calls);
        }

        [Fact]
        public void The_highlighter_receives_the_text_of_the_snapshot()
        {
            string received = null;
            var h = new CountingHighlighter { Behavior = text => { received = text; return new Token[0]; } };

            Cache(h).TokensIn(new Snapshot("hello"), 0, 5);

            Assert.Equal("hello", received);
        }

        [Theory]
        [InlineData(0, 100, 10)]  // everything
        [InlineData(0, 2, 1)]     // exactly the first token
        [InlineData(0, 1, 1)]     // touches the first token
        [InlineData(2, 5, 0)]     // the gap between tokens 0 and 1
        [InlineData(1, 6, 2)]     // the end of token 0 and the start of token 1
        [InlineData(5, 6, 1)]
        [InlineData(7, 10, 0)]    // starts where token 1 ends
        [InlineData(49, 50, 0)]
        [InlineData(45, 50, 1)]
        [InlineData(100, 200, 0)]
        [InlineData(3, 3, 0)]     // empty span
        public void Tokens_intersecting_the_span_are_returned(int start, int end, int expected)
        {
            var h = new CountingHighlighter { Behavior = Spread };
            var tokens = Cache(h).TokensIn(new Snapshot(new string('x', 50)), start, end);

            Assert.Equal(expected, tokens.Count);
            Assert.All(tokens, t => Assert.True(t.End > start && t.Start < end));
        }

        [Fact]
        public void Returned_tokens_keep_their_order()
        {
            var h = new CountingHighlighter { Behavior = Spread };
            var tokens = Cache(h).TokensIn(new Snapshot(new string('x', 50)), 0, 50);

            Assert.Equal(tokens.Select(t => t.Start).OrderBy(x => x), tokens.Select(t => t.Start));
        }

        [Fact]
        public void A_failing_highlighter_gives_no_tokens_instead_of_an_exception()
        {
            var h = new CountingHighlighter { Behavior = text => { throw new InvalidOperationException("boom"); } };
            var cache = Cache(h);
            var snapshot = new Snapshot("abc");

            Assert.Empty(cache.TokensIn(snapshot, 0, 3));
            Assert.Empty(cache.TokensIn(snapshot, 0, 3));
            Assert.Equal(1, h.Calls); // the failure is cached too: no retry storm on every redraw
        }

        [Fact]
        public void A_failure_does_not_poison_the_next_snapshot()
        {
            bool fail = true;
            var h = new CountingHighlighter
            {
                Behavior = text => { if (fail) throw new Exception(); return new[] { new Token(0, 1, "t") }; }
            };
            var cache = Cache(h);

            Assert.Empty(cache.TokensIn(new Snapshot("a"), 0, 1));
            fail = false;
            Assert.Single(cache.TokensIn(new Snapshot("b"), 0, 1));
        }

        [Fact]
        public void Zero_length_tokens_are_never_returned()
        {
            var h = new CountingHighlighter { Behavior = text => new[] { new Token(1, 0, "empty"), new Token(2, 1, "ok") } };
            var tokens = Cache(h).TokensIn(new Snapshot("abc"), 0, 3);

            Assert.Equal(new[] { "ok" }, tokens.Select(t => t.Type).ToArray());
        }

        [Fact]
        public void Dependencies_are_required()
        {
            Assert.Throws<ArgumentNullException>(() => new SnapshotTokenCache<Snapshot>(null, s => s.Text));
            Assert.Throws<ArgumentNullException>(() => new SnapshotTokenCache<Snapshot>(new CountingHighlighter(), null));
        }

        [Fact]
        public void Works_with_the_real_pipeline()
        {
            var cache = new SnapshotTokenCache<Snapshot>(NestLightComposition.CreateHighlighter(HostLanguage.JavaScript), s => s.Text);
            var snapshot = new Snapshot("const a = html`<b></b>`;");

            var tokens = cache.TokensIn(snapshot, 0, snapshot.Text.Length);

            Assert.Contains(tokens, t => t.Type == ClassificationNames.Tag);
        }
    }
}
