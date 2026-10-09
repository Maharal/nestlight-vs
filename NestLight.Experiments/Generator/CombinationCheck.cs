using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    /// <summary>What the real pipeline found in a generated file.</summary>
    internal sealed class CombinationResult
    {
        public Combination Combination;
        public int Characters;
        public int Strings;
        public int Interpolations;
        public int Tokens;
        public List<string> Problems = new List<string>();
        public bool Passed { get { return Problems.Count == 0; } }
    }

    /// <summary>Runs the plugin over the file generated for a combination and compares what it finds with what the generator put there.</summary>
    internal static class CombinationCheck
    {
        public static CombinationResult Check(Combination c, int repeat)
        {
            string text = CombinationGenerator.Generate(c, repeat);
            Expectation expected = CombinationGenerator.Expect(c, repeat);
            IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
            IHostScanner scanner = NestLightComposition.CreateScanner(c.Host, languages);
            var result = new CombinationResult { Combination = c, Characters = text.Length };

            IReadOnlyList<EmbeddedString> strings = scanner.Scan(text);
            result.Strings = strings.Count;
            result.Interpolations = strings.Sum(s => s.Interpolations.Count);
            if (result.Strings != expected.Strings) result.Problems.Add("found " + result.Strings + " strings, expected " + expected.Strings);
            if (result.Interpolations != expected.Interpolations) result.Problems.Add("found " + result.Interpolations + " interpolations, expected " + expected.Interpolations);
            IEmbeddedLanguageTokenizer tokenizer = languages.Find(c.Language);
            if (strings.Any(s => languages.Find(s.EmbeddedLanguageId) != tokenizer)) result.Problems.Add("a string was read as another language");

            IReadOnlyList<Token> tokens = new HighlightEngine(scanner, languages).Highlight(text);
            result.Tokens = tokens.Count;
            if (tokens.Count == 0) result.Problems.Add("no tokens");
            if (tokens.Any(t => t.Start < 0 || t.Length <= 0 || t.End > text.Length)) result.Problems.Add("a token outside the text");
            if (tokens.Any(t => !strings.Any(s => t.Start >= s.Start && t.End <= s.End))) result.Problems.Add("a token outside every string");
            return result;
        }
    }
}
