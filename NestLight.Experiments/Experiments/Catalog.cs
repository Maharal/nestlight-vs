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
                new EA_01_Baseline(),
                new EA_02_SkipScan(),
                new EA_03_TextCopy(),
                new EA_04_Registry(),
                new EA_05_Allocations(),
                new EA_06_Density(),
                new EA_07_Interpolations(),
                new EA_08_Throughput(),
                new EA_09_Malformed(),
                new EA_14_CompletionLatency(),
                new EA_15_CompletionLimits(),
                new EA_17_VocabularyConsistency(),
                new EA_18_RankingQuality(),
                new EA_19_ScopeAndSavings(),
                new EA_20_CompletionFastPath(),
                new EA_21_SimilarLatency(),
                new EA_22_SimilarRecovery(),
                new EA_23_SimilarNoise(),
                new EA_24_SimilarRobustness(),
                new EA_25_PreviousWord(),
                new EA_26_SameLanguageWords(),
                new EA_27_GrammarPosition(),
                new EA_28_SqlSchema(),
                new EA_29_CountAndDistance(),
                new EA_30_ContextRobustness(),
                new EA_31_KeywordOrder(),
                new EA_32_HeadKeywords(),
                new EA_33_ShortWords(),
                new EA_34_SimilarNoiseShortPrefix(),
                new EA_35_ShortWordsRestated(),
                new EA_36_CombinationMatrix(),
            };
        }
    }
}
