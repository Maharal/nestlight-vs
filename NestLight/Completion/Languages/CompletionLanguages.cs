using System;
using System.Collections.Generic;

namespace NestLight.Completion
{
    /// <summary>Maps ids and aliases to the completion strategy of each embedded language.</summary>
    internal sealed class CompletionLanguages : ICompletionLanguages
    {
        private static readonly ICompletionLanguage Unknown = new UnknownCompletionLanguage();

        /// <summary>Every language the plugin knows. The strategies keep no state that depends on a buffer, so they are shared.</summary>
        public static readonly CompletionLanguages Default = new CompletionLanguages(CreateStandard());

        private readonly Dictionary<string, ICompletionLanguage> _byId = new Dictionary<string, ICompletionLanguage>(StringComparer.OrdinalIgnoreCase);

        public CompletionLanguages(IEnumerable<ICompletionLanguage> languages)
        {
            if (languages == null) throw new ArgumentNullException("languages");
            foreach (ICompletionLanguage language in languages)
                foreach (string id in language.Ids)
                {
                    if (_byId.ContainsKey(id)) throw new InvalidOperationException("Completion language registered twice: " + id);
                    _byId[id] = language;
                }
        }

        public IEnumerable<string> Ids { get { return _byId.Keys; } }

        /// <summary>New instances of every language the plugin knows: for a registry of its own (the experiments change the order by use of a copy).</summary>
        internal static IEnumerable<ICompletionLanguage> CreateStandard()
        {
            return new ICompletionLanguage[]
            {
                new SqlCompletion(), new CssCompletion(), new HtmlCompletion(), new GraphQlCompletion(), new JsonCompletion(), new YamlCompletion(),
                new GlslCompletion(), new WgslCompletion(), new XmlCompletion(), new MarkdownCompletion(), new RegexCompletion()
            };
        }

        public ICompletionLanguage Find(string embeddedLanguageId)
        {
            ICompletionLanguage language;
            return embeddedLanguageId != null && _byId.TryGetValue(embeddedLanguageId, out language) ? language : Unknown;
        }

        public bool Same(string a, string b)
        {
            if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) return true;
            ICompletionLanguage x, y;
            return a != null && b != null && _byId.TryGetValue(a, out x) && _byId.TryGetValue(b, out y) && ReferenceEquals(x, y);
        }
    }
}
