using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;

namespace NestLight.Completion
{
    /// <summary>
    /// The language of a string for the completion, with nothing special: it offers its words and the words of the document. A language
    /// overrides only what is different about it, which is also where a model that learns from the document (an adaptive n-gram, for the
    /// languages without a grammar) would be plugged in.
    /// </summary>
    internal abstract class CompletionLanguage : ICompletionLanguage
    {
        /// <summary>How far back from the caret the grammar looks for the place.</summary>
        protected const int Reach = 4000;

        /// <summary>The "unlikely" test that is true for every word: no keyword belongs here.</summary>
        protected static readonly Func<string, bool> Always = w => true;

        private readonly string[] _ids;
        private readonly string[] _extraWords;
        private readonly IReadOnlyList<string> _keywords;
        private readonly object _gate = new object();
        private IReadOnlyList<string> _completionWords;
        private Dictionary<string, string> _index;

        /// <param name="ids">Ids and aliases, lower case.</param>
        /// <param name="keywords">The words the tokenizer colors; sorted and without repeats here.</param>
        /// <param name="extraWords">Words that are offered but not colored (the tokenizers know them as plain identifiers).</param>
        protected CompletionLanguage(string[] ids, IEnumerable<string> keywords = null, string[] extraWords = null)
        {
            _ids = ids;
            _extraWords = extraWords ?? new string[0];
            _keywords = keywords == null
                ? new string[0]
                : (IReadOnlyList<string>)keywords.Distinct(StringComparer.Ordinal).OrderBy(w => w, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public IReadOnlyList<string> Ids { get { return _ids; } }

        public IReadOnlyList<string> Keywords { get { return _keywords; } }

        public IReadOnlyList<string> CompletionWords
        {
            get
            {
                lock (_gate)
                {
                    if (_completionWords != null) return _completionWords;
                    if (_extraWords.Length == 0) return _completionWords = _keywords;
                    var list = new List<string>(_keywords);
                    foreach (string w in _extraWords) if (!list.Contains(w, StringComparer.OrdinalIgnoreCase)) list.Add(w);
                    list.Sort(StringComparer.OrdinalIgnoreCase);
                    return _completionWords = list;
                }
            }
        }

        public string FindKeyword(string word)
        {
            lock (_gate)
            {
                if (_index == null)
                {
                    _index = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    foreach (string w in _keywords) if (!_index.ContainsKey(w)) _index[w] = w;
                }
                string found;
                return _index.TryGetValue(word, out found) ? found : null;
            }
        }

        public string FindCompletionWord(string word)
        {
            string colored = FindKeyword(word);
            if (colored != null) return colored;
            return _extraWords.FirstOrDefault(w => string.Equals(w, word, StringComparison.OrdinalIgnoreCase));
        }

        public virtual bool KeywordsFollowTypedCase { get { return false; } }

        public virtual bool IsExtraWordChar(char c) { return false; }

        public virtual bool TryAdjustWordStart(string text, int ownerStart, ref int start, int caret) { return true; }

        public Position PositionAt(string text, CompletionSite site)
        {
            if (text == null || site == null || site.Start > text.Length) return null;
            int floor = Math.Max(site.OwnerStart >= 0 ? site.OwnerStart : 0, site.Start - Reach);
            return ReadPosition(text, floor, site);
        }

        /// <summary>The place of the caret, reading back from the caret to <paramref name="floor"/>; null when the language has no rules here.</summary>
        protected virtual Position ReadPosition(string text, int floor, CompletionSite site) { return null; }

        protected static bool IsWordChar(char c) { return char.IsLetterOrDigit(c) || c == '_'; }
    }

    /// <summary>The language of a string whose id the completion does not know: only the words of the document.</summary>
    internal sealed class UnknownCompletionLanguage : CompletionLanguage
    {
        public UnknownCompletionLanguage() : base(new string[0]) { }
    }
}
