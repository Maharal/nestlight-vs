using System.Collections.Generic;

namespace NestLight.Experiments
{
    internal static class Catalog
    {
        public static IList<Experiment> All()
        {
            return new Experiment[]
            {
                new E01_Baseline(),
                new E02_SkipScan(),
                new E03_TextCopy(),
                new E04_Registry(),
                new E05_Allocations(),
                new E06_Density(),
                new E07_Interpolations(),
                new E08_Throughput(),
                new E09_Malformed(),
            };
        }
    }
}
