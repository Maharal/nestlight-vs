using System;
using System.Collections.Generic;
using System.Linq;

namespace NestLight.Detection
{
    internal sealed class SqlDetector : ILanguageDetector
    {
        private static readonly string[] Verbs = { "select", "insert", "update", "delete", "create", "alter", "drop", "with", "set" };
        private static readonly string[] Followers = { "from", "into", "set", "table", "values", "where", "join" };
        private static readonly string[] Articles = { "an", "the", "your", "my", "this", "that", "some", "any" };

        public string Id { get { return "sql"; } }

        public bool CanStartWith(char first)
        {
            char c = (char)(first | 0x20);
            return c == 's' || c == 'i' || c == 'u' || c == 'd' || c == 'c' || c == 'a' || c == 'w';
        }

        public int Score(string t, int start, int end)
        {
            char last = t[end - 1];
            if (last == '.' || last == '?' || last == '!') return 0;
            foreach (string verb in Verbs)
            {
                if (!Probe.StartsWithWord(t, start, end, verb)) continue;
                foreach (string follower in Followers)
                    if (Probe.ContainsWord(t, start, end, follower))
                    {
                        if (LooksLikeProse(t, start, end, verb)) return 0;
                        return 3;
                    }
                return 0;
            }
            return 0;
        }

        private static bool LooksLikeProse(string t, int start, int end, string verb)
        {
            int afterVerb = start + verb.Length;
            while (afterVerb < end && char.IsWhiteSpace(t[afterVerb])) afterVerb++;
            foreach (string article in Articles)
                if (Probe.StartsWithWord(t, afterVerb, end, article)) return true;
            return false;
        }
    }

    internal sealed class HtmlDetector : ILanguageDetector
    {
        public string Id { get { return "html"; } }

        public bool CanStartWith(char first) { return first == '<'; }

        public int Score(string t, int start, int end)
        {
            if (end - start < 3) return 0;
            char next = t[start + 1];
            if (!char.IsLetter(next) && next != '!' && next != '/') return 0;
            if (t[end - 1] != '>') return 0;
            return Probe.Contains(t, start, end, "</") || Probe.Contains(t, start, end, "/>") ? 3 : 2;
        }
    }

    internal sealed class JsonDetector : ILanguageDetector
    {
        public string Id { get { return "json"; } }

        public bool CanStartWith(char first) { return first == '{' || first == '['; }

        public int Score(string t, int start, int end)
        {
            char open = t[start], close = t[end - 1];
            char next = Probe.NextSignificant(t, start + 1, end);
            if (open == '{')
                return close == '}' && (next == '"' || next == '}') ? 3 : 0;
            if (open == '[')
                return close == ']' && (next == '"' || next == '{' || next == '[' || next == ']' || next == '-' || char.IsDigit(next)) ? 2 : 0;
            return 0;
        }
    }

    internal sealed class CssDetector : ILanguageDetector
    {
        public string Id { get { return "css"; } }

        public bool CanStartWith(char first) { return first == '.' || first == '#' || first == '@' || first == ':' || first == '/' || char.IsLetter(first); }

        public int Score(string t, int start, int end)
        {
            if (t[end - 1] != '}') return 0;
            int s = Probe.SkipCStyleComments(t, start, end);
            if (s >= end) return 0;
            if (t[s] == '#' && s + 1 < end && (t[s + 1] == ' ' || t[s + 1] == '\t')) return 0;
            int brace = Probe.IndexOf(t, s, end, '{');
            if (brace < 0) return 0;
            int colon = Probe.IndexOf(t, brace, end, ':');
            if (colon < 0) return 0;
            if (Probe.IndexOf(t, colon, end, ';') >= 0) return 3;
            return StartsWithProperty(t, brace + 1, colon) ? 2 : 0;
        }

        /// <summary>Whether the text between the brace and the colon is a lone property name, as in <c>{ color: red }</c>.</summary>
        private static bool StartsWithProperty(string t, int from, int colon)
        {
            int i = from;
            while (i < colon && char.IsWhiteSpace(t[i])) i++;
            int nameStart = i;
            while (i < colon && (char.IsLetterOrDigit(t[i]) || t[i] == '-' || t[i] == '_')) i++;
            if (i == nameStart) return false;
            while (i < colon && char.IsWhiteSpace(t[i])) i++;
            return i == colon;
        }
    }

    internal sealed class GraphQlDetector : ILanguageDetector
    {
        public string Id { get { return "graphql"; } }

        public bool CanStartWith(char first) { return first == 'q' || first == 'm' || first == 's' || first == 'f' || first == '{' || first == '#'; }

        public int Score(string t, int start, int end)
        {
            if (t[end - 1] != '}') return 0;
            int s = Probe.SkipLineComments(t, start, end, '#');
            if (s >= end) return 0;
            if (t[s] == '{')
            {
                if (!char.IsLetter(Probe.NextSignificant(t, s + 1, end))) return 0;
                int firstClose = Probe.IndexOf(t, s + 1, end, '}');
                if (firstClose >= 0 && firstClose < end - 1)
                {
                    char afterClose = Probe.NextSignificant(t, firstClose + 1, end);
                    if (afterClose != '\0' && afterClose != '}') return 0;
                }
                return 2;
            }
            if (Probe.StartsWithWord(t, s, end, "query") || Probe.StartsWithWord(t, s, end, "mutation")
                || Probe.StartsWithWord(t, s, end, "subscription") || Probe.StartsWithWord(t, s, end, "fragment"))
                return Probe.IndexOf(t, s, end, '{') >= 0 ? 3 : 0;
            return 0;
        }
    }

    internal sealed class XmlDetector : ILanguageDetector
    {
        public string Id { get { return "xml"; } }

        public bool CanStartWith(char first) { return first == '<'; }

        public int Score(string t, int start, int end)
        {
            if (end - start < 5) return 0;
            if (Probe.Contains(t, start, end, "<?xml")) return 3;
            char next = t[start + 1];
            if (!char.IsLetter(next) && next != '!' && next != '/' && next != '?') return 0;
            if (t[end - 1] != '>') return 0;
            if (Probe.Contains(t, start, end, "xmlns") || Probe.Contains(t, start, end, "xmlns:")) return 3;
            bool hasColon = false;
            for (int i = start + 1; i < end && t[i] != '>' && t[i] != ' '; i++)
                if (t[i] == ':') { hasColon = true; break; }
            if (hasColon && (Probe.Contains(t, start, end, "</") || Probe.Contains(t, start, end, "/>"))) return 3;
            return 0;
        }
    }

    internal sealed class YamlDetector : ILanguageDetector
    {
        public string Id { get { return "yaml"; } }

        public bool CanStartWith(char first) { return first == '-' || char.IsLetter(first) || first == '#'; }

        public int Score(string t, int start, int end)
        {
            if (Probe.StartsWithWord(t, start, end, "---") || (t[start] == '-' && start + 3 < end && t[start + 1] == '-' && t[start + 2] == '-'))
                return Probe.IndexOf(t, start, end, ':') >= 0 ? 3 : 2;
            int colons = 0, lines = 0;
            for (int i = start; i < end; i++)
            {
                if (t[i] == '\n') lines++;
                if (t[i] == ':' && i + 1 < end && (t[i + 1] == ' ' || t[i + 1] == '\n'))
                {
                    if (i > start && t[i - 1] != ' ' && t[i - 1] != '\t') colons++;
                }
            }
            if (colons >= 2 && lines >= 1) return 3;
            if (colons >= 1 && lines >= 1) return 2;
            return 0;
        }
    }

    internal sealed class ShellDetector : ILanguageDetector
    {
        private static readonly string[] ShellStarters = { "#!/", "if ", "for ", "while ", "case ", "echo ", "export ", "set ", "cd ", "mkdir ", "rm ", "cp ", "mv ", "grep ", "find ", "curl ", "wget ", "sudo ", "apt ", "docker " };

        public string Id { get { return "shell"; } }

        public bool CanStartWith(char first) { return first == '#' || first == 'i' || first == 'f' || first == 'w' || first == 'c' || first == 'e' || first == 's' || first == 'm' || first == 'r' || first == 'g' || first == 'd' || first == 'a'; }

        public int Score(string t, int start, int end)
        {
            if (Probe.Contains(t, start, Math.Min(start + 30, end), "#!/")) return 3;
            int pipes = 0, dollars = 0;
            for (int i = start; i < end; i++)
            {
                if (t[i] == '|' && i + 1 < end && t[i + 1] != '|') pipes++;
                if (t[i] == '$') dollars++;
            }
            if (pipes >= 1 && dollars >= 1) return 3;
            foreach (string s in ShellStarters)
                if (Probe.StartsWithWord(t, start, end, s.TrimEnd()) && (pipes >= 1 || dollars >= 1)) return 2;
            return 0;
        }
    }

    internal sealed class TomlDetector : ILanguageDetector
    {
        public string Id { get { return "toml"; } }

        public bool CanStartWith(char first) { return first == '[' || char.IsLetter(first) || first == '#'; }

        public int Score(string t, int start, int end)
        {
            bool hasTable = false, hasKeyValue = false;
            int i = start;
            while (i < end)
            {
                int lineEnd = t.IndexOf('\n', i, end - i);
                if (lineEnd < 0) lineEnd = end;
                int p = i;
                while (p < lineEnd && (t[p] == ' ' || t[p] == '\t')) p++;
                if (p < lineEnd)
                {
                    if (t[p] == '[' && Probe.IndexOf(t, p, lineEnd, ']') >= 0) hasTable = true;
                    else if (t[p] != '#')
                    {
                        int eq = Probe.IndexOf(t, p, lineEnd, '=');
                        if (eq >= 0)
                        {
                            bool bareKey = true;
                            for (int k = p; k < eq; k++)
                            {
                                char c = t[k];
                                if (!(char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.' || c == ' ' || c == '\t' || c == '"' || c == '\'')) { bareKey = false; break; }
                            }
                            if (bareKey) hasKeyValue = true;
                        }
                    }
                }
                i = lineEnd + 1;
            }
            if (hasTable && hasKeyValue) return 3;
            if (hasTable || hasKeyValue) return 0;
            return 0;
        }
    }

    /// <summary>
    /// The context of the strategy pattern: asks each strategy and answers with the language of the highest score, or null when none
    /// reaches the threshold. With <c>gate</c> a strategy is only asked when it can start with the first character of the text.
    /// </summary>
    internal sealed class LanguageDetector
    {
        public const int MinLength = 8;
        public const int Threshold = 2;

        private readonly ILanguageDetector[] _strategies;
        private readonly bool _gate;

        public LanguageDetector(ILanguageDetector[] strategies, bool gate)
        {
            _strategies = strategies;
            _gate = gate;
        }

        public static LanguageDetector Create(bool gate)
        {
            return new LanguageDetector(Strategies(), gate);
        }

        /// <summary>One strategy per language that can be detected. The only place that lists them: the options page and the settings file follow it.</summary>
        public static ILanguageDetector[] Strategies()
        {
            return new ILanguageDetector[] { new SqlDetector(), new HtmlDetector(), new JsonDetector(), new CssDetector(), new GraphQlDetector(), new XmlDetector(), new YamlDetector(), new ShellDetector(), new TomlDetector() };
        }

        /// <summary>The ids of the languages that can be detected, in the order of <see cref="Strategies"/>.</summary>
        public static readonly IReadOnlyList<string> Languages = Array.AsReadOnly(Strategies().Select(s => s.Id).ToArray());

        public string Detect(string text, int start, int end)
        {
            while (start < end && char.IsWhiteSpace(text[start])) start++;
            while (end > start && char.IsWhiteSpace(text[end - 1])) end--;
            if (end - start < MinLength) return null;

            char first = text[start];
            string best = null;
            int bestScore = Threshold - 1;
            foreach (ILanguageDetector strategy in _strategies)
            {
                if (_gate && !strategy.CanStartWith(first)) continue;
                int score = strategy.Score(text, start, end);
                if (score > bestScore) { best = strategy.Id; bestScore = score; }
            }
            return best;
        }
    }
}
