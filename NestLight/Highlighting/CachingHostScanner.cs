using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Highlighting
{
    /// <summary>
    /// Remembers the result of the last scan. The classifier and the completion ask for the strings of the same snapshot one after
    /// the other, and scanning is the largest part of both (EA10): with the same text instance, the second one is free.
    /// The text is compared by reference, which is cheap and never wrong; a different instance with the same content is just a miss.
    /// The text is held weakly, so the cache never keeps a copy of a large file alive.
    /// </summary>
    /// <remarks>
    /// A miss is cheap too when the scanner can resume (<see cref="IResumableHostScanner"/>) and the text of the last scan is still
    /// alive: the new text is compared with it and only the lines around the edit are scanned again (<see cref="IncrementalHostScan"/>).
    /// If that text has been collected, the scan is whole, as it always was.
    /// </remarks>
    internal sealed class CachingHostScanner : IHostScanner
    {
        private readonly IHostScanner _inner;
        private readonly object _gate = new object();
        private WeakReference<string> _text;
        private IReadOnlyList<EmbeddedString> _result;
        private HostScan _scan; // of the same text, when the scanner can resume
        private readonly Func<int> _stateVersion;
        private int _version;

        /// <param name="stateVersion">
        /// The version of whatever else decides what the scanner finds (the detection options). When it changes, what is remembered is
        /// stale and the next scan is whole; the strings of an edit are only reused while the version is the one they were found under.
        /// </param>
        public CachingHostScanner(IHostScanner inner, Func<int> stateVersion = null)
        {
            if (inner == null) throw new ArgumentNullException("inner");
            _inner = inner;
            _stateVersion = stateVersion;
        }

        public IReadOnlyList<EmbeddedString> Scan(string text)
        {
            if (text == null) return _inner.Scan(text);
            lock (_gate)
            {
                int version = _stateVersion == null ? 0 : _stateVersion();
                if (version != _version)
                {
                    _text = null; _result = null; _scan = null;
                    _version = version;
                }
                string known = null;
                if (_text != null) _text.TryGetTarget(out known);
                if (known != null && ReferenceEquals(known, text)) return _result;

                IReadOnlyList<EmbeddedString> result;
                HostScan scan = null;
                var resumable = _inner as IResumableHostScanner;
                if (resumable == null) result = _inner.Scan(text); // a failure is not cached
                else
                {
                    if (_scan != null && known != null) scan = IncrementalHostScan.Update(resumable, _scan, known, text);
                    if (scan == null) scan = resumable.ScanAll(text);
                    result = scan.Strings;
                }
                _text = new WeakReference<string>(text);
                _result = result;
                _scan = scan;
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
