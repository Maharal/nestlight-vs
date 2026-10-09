using NestLight.Completion;

namespace NestLight.Experiments
{
    /// <summary>EA33 with its criterion restated: the first one could not be met.</summary>
    internal sealed class EA35_ShortWordsRestated : EA33_ShortWords
    {
        public override string Id { get { return "EA35"; } }
        public override string Title { get { return "Should words of two letters be offered? (EA33 with a criterion that can be met)"; } }
        public override string Criterion { get { return "A variant closes at least half of the distance to 100% for the words of 2 letters within the first 5, on the test files and on the hand-written files; it lowers the longer words by no more than 0.5 point overall and 1 point in any language (and 1 point on the hand-written files). Among the variants that meet it, the one that lowers the longer words least is adopted, and then the one that gains most."; } }
        public override string IfMet { get { return "Offer the two-letter words in the way of that variant."; } }
        public override string IfNotMet { get { return "Offer them only where the place says a short word is likely."; } }

        protected override bool Meets(double gain, double baseline, double harm, double worstLanguage, double handGain, double handBaseline, double handHarm)
        {
            return gain >= 0.5 * (100 - baseline) && harm >= -0.5 && worstLanguage >= -1 && handGain >= 0.5 * (100 - handBaseline) && handHarm >= -1;
        }

        protected override bool Better(double gain, double harm, double bestGain, double bestHarm)
        {
            return harm > bestHarm || (harm == bestHarm && gain > bestGain);
        }
    }
}
