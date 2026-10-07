using System;
using System.Collections.Generic;
using NestLight.Common;

namespace NestLight.Highlighting
{
    /// <summary>
    /// Maps ids and aliases to tokenizers. The tokenizers are built by a factory that receives the registry,
    /// so that a language can delegate to another one (HTML to CSS) without a circular construction.
    /// </summary>
    internal sealed class LanguageRegistry : ILanguageRegistry
    {
        private readonly Dictionary<string, ILanguageTokenizer> _byId =
            new Dictionary<string, ILanguageTokenizer>(StringComparer.OrdinalIgnoreCase);

        public LanguageRegistry(Func<ILanguageRegistry, IEnumerable<ILanguageTokenizer>> tokenizers)
        {
            if (tokenizers == null) throw new ArgumentNullException("tokenizers");
            foreach (ILanguageTokenizer tokenizer in tokenizers(this))
                foreach (string id in tokenizer.Ids)
                {
                    if (_byId.ContainsKey(id))
                        throw new InvalidOperationException("Language id registered twice: " + id);
                    _byId[id] = tokenizer;
                }
        }

        public IEnumerable<string> Ids { get { return _byId.Keys; } }

        public bool IsKnown(string id)
        {
            return id != null && _byId.ContainsKey(id);
        }

        public ILanguageTokenizer Find(string id)
        {
            ILanguageTokenizer tokenizer;
            return id != null && _byId.TryGetValue(id, out tokenizer) ? tokenizer : null;
        }
    }
}
