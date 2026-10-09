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
                new EA12_VocabularyConsistency(),
                new EA13_RankingQuality(),
                new EA14_ScopeAndSavings(),
                new EA15_CompletionFastPath(),
                new EA16_SimilarLatency(),
                new EA17_SimilarRecovery(),
                new EA18_SimilarNoise(),
                new EA19_SimilarRobustness(),
                new EA20_PreviousWord(),
                new EA21_SameLanguageWords(),
                new EA22_GrammarPosition(),
                new EA23_SqlSchema(),
                new EA24_CountAndDistance(),
                new EA25_ContextRobustness(),
                new EA26_KeywordOrder(),
                new EA27_HeadKeywords(),
                new EA28_ShortWords(),
                new EA29_SimilarNoiseShortPrefix(),
                new EA30_ShortWordsRestated(),
                new EA31_CombinationMatrix(),
                new EA32_WordIndex(),
            };
        }
    }
}
