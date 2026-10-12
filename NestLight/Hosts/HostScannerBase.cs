using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Hosts
{
    /// <summary>
    /// What every host scanner shares: the three ways to run one scan (whole, whole keeping safe points, resumed from a safe point).
    /// A host only says how to scan its code and, optionally, how to tell at a glance that a text holds no embedded string.
    /// </summary>
    internal abstract class HostScannerBase : IResumableHostScanner
    {
        private readonly int _safePointGap;

        /// <param name="safePointGap">The distance between the safe points a scan keeps to resume from (see <see cref="SafePoints"/>).</param>
        protected HostScannerBase(AcceptedEmbeddedLanguages languages, int safePointGap)
        {
            if (languages == null) throw new ArgumentNullException("languages");
            Languages = languages;
            _safePointGap = safePointGap;
        }

        protected AcceptedEmbeddedLanguages Languages { get; private set; }

        /// <summary>False when the text cannot hold a string that could be embedded code, so a full scan can be skipped.</summary>
        protected virtual bool MayHoldStrings(string text)
        {
            return true;
        }

        /// <summary>Scans text from <paramref name="start"/>, adding the strings to <paramref name="result"/>; with <paramref name="safe"/>, it stops where that says so.</summary>
        protected abstract void ScanCode(string text, List<EmbeddedString> result, SafePoints safe, int start);

        public IReadOnlyList<EmbeddedString> Scan(string text)
        {
            var result = new List<EmbeddedString>();
            if (!MayHoldStrings(text)) return result;
            ScanCode(text, result, null, 0);
            return result;
        }

        public HostScan ScanAll(string text)
        {
            var result = new List<EmbeddedString>();
            if (!MayHoldStrings(text)) return new HostScan(result, new int[0]);
            var safe = new SafePoints(_safePointGap);
            ScanCode(text, result, safe, 0);
            return new HostScan(result, safe.ToArray());
        }

        public HostScan Resume(string text, int from, int minStop, Func<int, bool> knownSafe)
        {
            var result = new List<EmbeddedString>();
            var safe = new SafePoints(minStop, knownSafe, _safePointGap);
            ScanCode(text, result, safe, from);
            return new HostScan(result, safe.ToArray(), safe.StoppedAt);
        }
    }
}
