using System;

namespace NestLight.Experiments.Detection
{
    internal sealed class SqlDetector : ILanguageDetector
    {
        private static readonly string[] Verbs = { "select", "insert", "update", "delete", "create", "alter", "drop", "with" };
        private static readonly string[] Followers = { " from ", " into ", " set ", " table ", " values", " where ", " join " };

        public string Id { get { return "sql"; } }

        public bool CanStartWith(char first)
        {
            char c = (char)(first | 0x20);
            return c == 's' || c == 'i' || c == 'u' || c == 'd' || c == 'c' || c == 'a' || c == 'w';
        }

        public int Score(string t, int start, int end)
        {
            char last = t[end - 1];
            if (last == '.' || last == '?' || last == '!') return 0; // a sentence, not a statement
            foreach (string verb in Verbs)
            {
                if (!Probe.StartsWithWord(t, start, end, verb)) continue;
                foreach (string follower in Followers)
                    if (Probe.Contains(t, start, end, follower)) return 3;
                return 0;
            }
            return 0;
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

        public bool CanStartWith(char first) { return first == '.' || first == '#' || first == '@' || first == ':' || char.IsLetter(first); }

        public int Score(string t, int start, int end)
        {
            if (t[end - 1] != '}') return 0;
            int brace = Probe.IndexOf(t, start, end, '{');
            if (brace < 0) return 0;
            int colon = Probe.IndexOf(t, brace, end, ':');
            if (colon < 0) return 0;
            return Probe.IndexOf(t, colon, end, ';') >= 0 ? 3 : 2;
        }
    }

    internal sealed class GraphQlDetector : ILanguageDetector
    {
        public string Id { get { return "graphql"; } }

        public bool CanStartWith(char first) { return first == 'q' || first == 'm' || first == 's' || first == 'f' || first == '{'; }

        public int Score(string t, int start, int end)
        {
            if (t[end - 1] != '}') return 0;
            if (t[start] == '{') return char.IsLetter(Probe.NextSignificant(t, start + 1, end)) ? 2 : 0;
            if (Probe.StartsWithWord(t, start, end, "query") || Probe.StartsWithWord(t, start, end, "mutation")
                || Probe.StartsWithWord(t, start, end, "subscription") || Probe.StartsWithWord(t, start, end, "fragment"))
                return Probe.IndexOf(t, start, end, '{') >= 0 ? 3 : 0;
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
            return new LanguageDetector(new ILanguageDetector[] { new SqlDetector(), new HtmlDetector(), new JsonDetector(), new CssDetector(), new GraphQlDetector() }, gate);
        }

        public static ILanguageDetector[] Strategies()
        {
            return new ILanguageDetector[] { new SqlDetector(), new HtmlDetector(), new JsonDetector(), new CssDetector(), new GraphQlDetector() };
        }

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
