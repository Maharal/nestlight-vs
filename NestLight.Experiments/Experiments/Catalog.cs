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
                new EA14_CompletionLatency(),
                new EA15_CompletionLimits(),
                new EA17_VocabularyConsistency(),
                new EA18_RankingQuality(),
                new EA19_ScopeAndSavings(),
                new EA20_CompletionFastPath(),
                new EA21_SimilarLatency(),
                new EA22_SimilarRecovery(),
                new EA23_SimilarNoise(),
                new EA24_SimilarRobustness(),
                new EA25_PreviousWord(),
                new EA26_SameLanguageWords(),
                new EA27_GrammarPosition(),
                new EA28_SqlSchema(),
                new EA29_CountAndDistance(),
                new EA30_ContextRobustness(),
                new EA31_KeywordOrder(),
                new EA32_HeadKeywords(),
                new EA33_ShortWords(),
                new EA34_SimilarNoiseShortPrefix(),
                new EA35_ShortWordsRestated(),
                new EA36_CombinationMatrix(),
            };
        }
    }
}
