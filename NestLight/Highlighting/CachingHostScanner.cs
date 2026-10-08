using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Highlighting
{
    /// <summary>
    /// Remembers the result of the last scan. The classifier and the completion ask for the strings of the same snapshot one after
    /// the other, and scanning is the largest part of both (E16): with the same text instance, the second one is free.
    /// The text is compared by reference, which is cheap and never wrong; a different instance with the same content is just a miss.
    /// The text is held weakly, so the cache never keeps a copy of a large file alive.
    /// </summary>
    internal sealed class CachingHostScanner : IHostScanner
    {
        private readonly IHostScanner _inner;
        private readonly object _gate = new object();
        private WeakReference<string> _text;
        private IReadOnlyList<EmbeddedString> _result;

        public CachingHostScanner(IHostScanner inner)
        {
            if (inner == null) throw new ArgumentNullException("inner");
            _inner = inner;
        }

        public IReadOnlyList<EmbeddedString> Scan(string text)
        {
            if (text == null) return _inner.Scan(text);
            lock (_gate)
            {
                string known;
                if (_text != null && _text.TryGetTarget(out known) && ReferenceEquals(known, text)) return _result;

                IReadOnlyList<EmbeddedString> result = _inner.Scan(text); // a failure is not cached
                _text = new WeakReference<string>(text);
                _result = result;
                return result;
            }
        }
    }

    /// <summary>
    /// The text of the last snapshot, so that everything that reads the same snapshot gets the same string instance (what
    /// <see cref="CachingHostScanner"/> needs to recognize it). The text is held weakly: when it has been collected, it is read again.
    /// </summary>
    internal sealed class SnapshotTextCache<TSnapshot> where TSnapshot : class
    {
        private readonly Func<TSnapshot, string> _getText;
        private readonly object _gate = new object();
        private TSnapshot _snapshot;
        private WeakReference<string> _text;

        public SnapshotTextCache(Func<TSnapshot, string> getText)
        {
            if (getText == null) throw new ArgumentNullException("getText");
            _getText = getText;
        }

        public string Of(TSnapshot snapshot)
        {
            lock (_gate)
            {
                string text;
                if (ReferenceEquals(_snapshot, snapshot) && _text != null && _text.TryGetTarget(out text)) return text;
                text = _getText(snapshot);
                _snapshot = snapshot;
                _text = new WeakReference<string>(text);
                return text;
            }
        }
    }
}
