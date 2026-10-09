using System.Collections.Generic;
using NestLight.Completion;

namespace NestLight.Experiments
{
    /// <summary>The languages of the plugin with the order by use of the keywords learned from a corpus instead of the one the plugin ships with.</summary>
    internal static class LearnedLanguages
    {
        /// <param name="orders">The keywords of each language by use, under the id of the language; a language without an entry keeps the order of the plugin.</param>
        public static ICompletionLanguages With(IReadOnlyDictionary<string, IReadOnlyList<string>> orders)
        {
            var languages = new List<ICompletionLanguage>(CompletionLanguages.CreateStandard());
            foreach (ICompletionLanguage language in languages)
            {
                var learnable = language as CompletionLanguage;
                if (learnable == null) continue;
                foreach (string id in language.Ids)
                {
                    IReadOnlyList<string> order;
                    if (!orders.TryGetValue(id, out order)) continue;
                    learnable.LearnUseOrder(order);
                    break;
                }
            }
            return new CompletionLanguages(languages);
        }
    }
}
