using System;
using System.Collections.Generic;

namespace NestLight.Completion
{
    /// <summary>Maps ids and aliases to the completion strategy of each embedded language.</summary>
    internal sealed class CompletionLanguages : ICompletionLanguages
    {
        private static readonly ICompletionLanguage Unknown = new UnknownCompletionLanguage();

        /// <summary>Every language the plugin knows. The strategies keep no state that depends on a buffer, so they are shared.</summary>
        public static readonly CompletionLanguages Default = new CompletionLanguages(Standard());

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

        private static IEnumerable<ICompletionLanguage> Standard()
        {
            // the strategies that lean on another language get it from here (HTML completes the CSS inside it)
            var css = new CssCompletion();
            return new ICompletionLanguage[]
            {
                new SqlCompletion(), css, new HtmlCompletion(css), new GraphQlCompletion(), new JsonCompletion(), new YamlCompletion(),
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
