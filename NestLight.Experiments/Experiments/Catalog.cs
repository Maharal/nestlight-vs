using System.Collections.Generic;

namespace NestLight.Experiments
{
    internal static class Catalog
    {
        /// <summary>Experiments/Automatic: they measure and decide their own criterion.</summary>
        public static IList<Experiment> Automatic()
        {
            return new Experiment[]
            {
                new EA01_Baseline(),
                new EA02_SkipScan(),
                new EA03_TextCopy(),
                new EA04_Registry(),
                new EA05_Allocations(),
                new EA06_Density(),
                new EA07_Interpolations(),
                new EA08_Throughput(),
                new EA09_Malformed(),
                new EA10_CompletionLatency(),
                new EA11_CompletionLimits(),
                new EA13_VocabularyConsistency(),
                new EA14_RankingQuality(),
                new EA15_ScopeAndSavings(),
                new EA16_CompletionFastPath(),
                new EA17_SimilarLatency(),
                new EA18_SimilarRecovery(),
                new EA19_SimilarNoise(),
                new EA20_SimilarRobustness(),
                new EA21_PreviousWord(),
                new EA22_SameLanguageWords(),
                new EA23_GrammarPosition(),
                new EA24_SqlSchema(),
                new EA25_CountAndDistance(),
                new EA26_ContextRobustness(),
                new EA27_KeywordOrder(),
                new EA28_HeadKeywords(),
                new EA29_ShortWords(),
                new EA30_SimilarNoiseShortPrefix(),
                new EA31_ShortWordsRestated(),
                new EA32_CombinationMatrix(),
            };
        }
    }
}
