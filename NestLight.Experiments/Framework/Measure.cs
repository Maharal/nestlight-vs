using System;
using System.Diagnostics;

namespace NestLight.Experiments
{
    /// <summary>What one measured action cost, per call.</summary>
    internal struct Sample
    {
        public double Ms;          // median
        public double MeanMs;      // mean: it keeps the spikes (a GC pause) that the median hides
        public double AllocKb;     // average bytes allocated per call, in KB
        public int Gen0, Gen1, Gen2; // collections during all the measured calls
    }

    internal static class Measure
    {
        public static Sample Run(Action action, int warmup, int runs)
        {
            for (int i = 0; i < warmup; i++) action();
            Collect();

            var times = new double[runs];
            int g0 = GC.CollectionCount(0), g1 = GC.CollectionCount(1), g2 = GC.CollectionCount(2);
            long bytes = AllocatedBytes();
            for (int i = 0; i < runs; i++)
            {
                var sw = Stopwatch.StartNew();
                action();
                times[i] = sw.Elapsed.TotalMilliseconds;
            }
            bytes = AllocatedBytes() - bytes;

            double total = 0;
            foreach (double t in times) total += t;
            Array.Sort(times);
            return new Sample
            {
                Ms = times[runs / 2],
                MeanMs = total / runs,
                AllocKb = bytes / 1024.0 / runs,
                Gen0 = GC.CollectionCount(0) - g0,
                Gen1 = GC.CollectionCount(1) - g1,
                Gen2 = GC.CollectionCount(2) - g2,
            };
        }

        /// <summary>Total bytes allocated by this thread (the experiments are single-threaded).</summary>
        public static long AllocatedBytes()
        {
#if NETFRAMEWORK
            return AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize;
#else
            return GC.GetAllocatedBytesForCurrentThread();
#endif
        }

        public static void EnableAllocationTracking()
        {
#if NETFRAMEWORK
            AppDomain.MonitoringIsEnabled = true;
#endif
        }

        public static void Collect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public static string Ms(double ms)
        {
            if (ms < 1) return (ms * 1000).ToString("F0") + " µs";
            return ms < 100 ? ms.ToString("F2") + " ms" : ms.ToString("F0") + " ms";
        }

        public static string Kb(double kb)
        {
            return kb < 1024 ? kb.ToString("F0") + " KB" : (kb / 1024).ToString("F1") + " MB";
        }

        public static string Ratio(double r)
        {
            return r.ToString("F2") + "x";
        }
    }
}
