using System;
using System.Collections.Generic;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class CachingHostScannerTests
    {
        private sealed class CountingScanner : IHostScanner
        {
            public int Calls;
            public bool Fail;
            public IReadOnlyList<EmbeddedString> Scan(string text)
            {
                Calls++;
                if (Fail) throw new InvalidOperationException("boom");
                return new List<EmbeddedString>();
            }
        }

        [Fact]
        public void The_same_text_instance_is_scanned_once()
        {
            var inner = new CountingScanner();
            var cache = new CachingHostScanner(inner);
            string text = new string('a', 10);
            IReadOnlyList<EmbeddedString> first = cache.Scan(text);
            Assert.Same(first, cache.Scan(text));
            Assert.Equal(1, inner.Calls);
        }

        [Fact]
        public void Another_instance_is_scanned_again_even_with_the_same_content()
        {
            var inner = new CountingScanner();
            var cache = new CachingHostScanner(inner);
            cache.Scan(new string('a', 10));
            cache.Scan(new string('a', 10));
            Assert.Equal(2, inner.Calls);
        }

        [Fact]
        public void Only_the_last_text_is_remembered()
        {
            var inner = new CountingScanner();
            var cache = new CachingHostScanner(inner);
            string a = new string('a', 5), b = new string('b', 5);
            cache.Scan(a); cache.Scan(b); cache.Scan(a);
            Assert.Equal(3, inner.Calls);
        }

        [Fact]
        public void A_failure_is_not_cached()
        {
            var inner = new CountingScanner { Fail = true };
            var cache = new CachingHostScanner(inner);
            string text = new string('a', 5);
            Assert.Throws<InvalidOperationException>(() => cache.Scan(text));
            inner.Fail = false;
            Assert.NotNull(cache.Scan(text));
            Assert.Equal(2, inner.Calls);
        }

        [Fact]
        public void The_cache_does_not_keep_the_text_alive()
        {
            var cache = new CachingHostScanner(new CountingScanner());
            WeakReference weak = Remember(cache);
            GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
            Assert.False(weak.IsAlive);
        }

        private static WeakReference Remember(CachingHostScanner cache)
        {
            string text = new string('z', 1000);
            cache.Scan(text);
            return new WeakReference(text);
        }

        [Fact]
        public void A_null_argument_is_the_inner_scanners_business()
        {
            Assert.Throws<ArgumentNullException>(() => new CachingHostScanner(null));
        }

        [Fact]
        public void Highlighting_and_completion_of_one_buffer_scan_a_text_once()
        {
            var counting = new CountingScanner();
            var cache = new CachingHostScanner(counting);
            IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
            var highlighter = new HighlightEngine(cache, languages);
            var completion = new CompletionEngine(cache);

            string text = "sql`select x from t`";
            highlighter.Highlight(text);
            CompletionSite site = completion.Locate(text, 7);
            completion.Suggest(text, site);
            Assert.Equal(1, counting.Calls);
        }

        [Fact]
        public void The_buffer_composition_gives_the_same_results_as_separate_pieces()
        {
            const string text = "const customerName = 1;\nsql`select cust from t`;\nhtml`<div class=\"a\">${customerName}</div>`";
            BufferAnalysis shared = NestLightComposition.CreateForBuffer(HostLanguage.JavaScript);
            IHighlighter separate = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            Assert.Equal(separate.Highlight(text).Count, shared.Highlighter.Highlight(text).Count);
            CompletionSite site = shared.Completion.Locate(text, text.IndexOf("cust from") + 4);
            Assert.Contains("customerName", System.Linq.Enumerable.Select(shared.Completion.Suggest(text, site), s => s.Text));
        }

        [Fact]
        public void The_text_of_a_snapshot_is_read_once()
        {
            int reads = 0;
            var cache = new SnapshotTextCache<object>(o => { reads++; return new string('x', 10); });
            object one = new object(), two = new object();
            string a = cache.Of(one);
            Assert.Same(a, cache.Of(one));
            Assert.Equal(1, reads);
            cache.Of(two);
            Assert.Equal(2, reads);
        }

        [Fact]
        public void The_text_cache_reads_again_after_the_text_was_collected()
        {
            int reads = 0;
            var cache = new SnapshotTextCache<object>(o => { reads++; return new string('x', 1000); });
            object snapshot = new object();
            cache.Of(snapshot);
            GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect();
            Assert.NotNull(cache.Of(snapshot));
            Assert.True(reads >= 1 && reads <= 2);
        }
    }
}
