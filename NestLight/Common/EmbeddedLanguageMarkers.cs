using System;
using System.Collections.Generic;

namespace NestLight.Common
{
    /// <summary>The languages a host accepts: the known ones, minus those the host leaves to the IDE.</summary>
    internal sealed class HostLanguages
    {
        private readonly ILanguageRegistry _registry;
        private readonly HashSet<string> _excluded;

        public HostLanguages(ILanguageRegistry registry, IEnumerable<string> excluded = null)
        {
            if (registry == null) throw new ArgumentNullException("registry");
            _registry = registry;
            _excluded = new HashSet<string>(excluded ?? new string[0], StringComparer.OrdinalIgnoreCase);
        }

        public bool Accepts(string id)
        {
            return !string.IsNullOrEmpty(id) && !_excluded.Contains(id) && _registry.IsKnown(id);
        }
    }

    /// <summary>Reads the language out of a marker comment: <c>html</c>, <c>language=html</c> or <c>lang=html</c>.</summary>
    internal static class MarkerComment
    {
        /// <param name="body">Comment text without its delimiters.</param>
        /// <returns>The lower-case id, or null when the comment is not a marker.</returns>
        public static string Parse(string body)
        {
            string s = body.Trim();
            string id = s;
            foreach (string key in new[] { "language", "lang" })
            {
                if (!s.StartsWith(key, StringComparison.OrdinalIgnoreCase)) continue;
                int p = key.Length;
                while (p < s.Length && char.IsWhiteSpace(s[p])) p++;
                if (p < s.Length && s[p] == '=')
                {
                    id = s.Substring(p + 1).Trim();
                    break;
                }
            }
            return IsId(id) ? id.ToLowerInvariant() : null;
        }

        /// <summary>The id of a tag glued to a backtick, like <c>html</c> in <c>ui.html`...`</c>.</summary>
        public static string IdBefore(string text, int index)
        {
            int j = index - 1;
            while (j >= 0 && (char.IsLetterOrDigit(text[j]) || text[j] == '_' || text[j] == '$')) j--;
            int length = index - 1 - j;
            return length > 0 ? text.Substring(j + 1, length).ToLowerInvariant() : null;
        }

        private static bool IsId(string s)
        {
            if (s.Length == 0) return false;
            foreach (char c in s)
                if (!(char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '+' || c == '#')) return false;
            return true;
        }
    }

    /// <summary>
    /// Remembers the last marker comment during a scan. It marks the first string literal that follows it
    /// on the same line or on the next one, as long as no statement ends in between.
    /// </summary>
    internal sealed class MarkerTracker
    {
        private readonly HostLanguages _languages;
        private string _id;
        private int _commentEnd;

        public MarkerTracker(HostLanguages languages)
        {
            _languages = languages;
        }

        /// <summary>Call for every comment; a comment that is not a marker cancels the previous one.</summary>
        public void Comment(string text, int bodyStart, int bodyEnd, int commentEnd)
        {
            string id = MarkerComment.Parse(text.Substring(bodyStart, Math.Max(0, bodyEnd - bodyStart)));
            if (_languages.Accepts(id)) { _id = id; _commentEnd = commentEnd; }
            else _id = null;
        }

        /// <summary>Call for every string literal; returns the marked language and always forgets the marker.</summary>
        public string Take(string text, int stringStart)
        {
            string id = _id;
            _id = null;
            if (id == null || stringStart < _commentEnd) return null;

            int lineBreaks = 0;
            for (int k = _commentEnd; k < stringStart; k++)
            {
                char c = text[k];
                if (c == '\n' && ++lineBreaks > 1) return null;
                if (c == ';' || c == '{' || c == '}') return null;
            }
            return id;
        }
    }

    /// <summary>The escape sequences that stand for one character in the value of a string.</summary>
    internal static class Escapes
    {
        public static bool TryDecode(char next, out char value)
        {
            switch (next)
            {
                case '"': value = '"'; return true;
                case '\'': value = '\''; return true;
                case '\\': value = '\\'; return true;
                case 'n': value = '\n'; return true;
                case 'r': value = '\r'; return true;
                case 't': value = '\t'; return true;
                default: value = '\0'; return false;
            }
        }
    }
}
