using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Common;
using NestLight.Completion;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA13_VocabularyConsistency : Experiment
    {
        public override string Id { get { return "EA13"; } }
        public override string Title { get { return "Does the tokenizer agree with the vocabulary?"; } }
        public override string Hypothesis { get { return "The keywords offered by the completion and the words the tokenizers color live in two places. The SQL, GraphQL, YAML and shader lists reuse the sets of the tokenizers, but the HTML tags and the CSS properties were written by hand. A word that the completion offers and the tokenizer then splits in two, or colors as plain text, tells the user the plugin does not know what it just offered."; } }
        public override string Method { get { return "For every word of every vocabulary, put it in one or more contexts of its language (`select`: `{0} x`; a CSS property: `.a { {0}: 1 }`...), run the real tokenizer, and check (1) that one token covers exactly the word and (2) that its type is the one expected for the language, or, for SQL and YAML, which only separate keywords from identifiers, that it differs from the type of an unknown word (`zzqx`) in the same context."; } }
        public override string Criterion { get { return "Every word is one token, and at least 95% of the words of each language are classified as expected."; } }
        public override string IfMet { get { return "Offering a word and coloring it agree."; } }
        public override string IfNotMet { get { return "The failing words are listed: fix the list or the tokenizer."; } }

        private sealed class Spec
        {
            public string[] Ids;
            public string[] Contexts;
            /// <summary>The classification names that count as recognized; null: any type that differs from the one of an unknown word.</summary>
            public string[] Accepted;
        }

        private static readonly Spec[] Specs =
        {
            new Spec { Ids = new[] { "sql" }, Contexts = new[] { "{0} x" } },
            new Spec { Ids = new[] { "graphql" }, Contexts = new[] { "{0} Foo", "fragment F {0} Foo", "type Foo { a: {0} }", "query Q { a(b: {0}) }", "query Q { a @{0} }" },
                       Accepted = new[] { ClassificationNames.GqlOperation, ClassificationNames.GqlKeyword, ClassificationNames.GqlType, ClassificationNames.GqlDirective } },
            new Spec { Ids = new[] { "glsl", "wgsl" }, Contexts = new[] { "{0} x;", "void main() { x = {0}(1.0); }" },
                       Accepted = new[] { ClassificationNames.ShaderKeyword, ClassificationNames.ShaderType, ClassificationNames.ShaderBuiltin } },
            new Spec { Ids = new[] { "json" }, Contexts = new[] { "{\"a\": {0}}" }, Accepted = new[] { ClassificationNames.JsonLiteral } },
            new Spec { Ids = new[] { "yaml" }, Contexts = new[] { "a: {0}", "- {0}" } },
            new Spec { Ids = new[] { "html" }, Contexts = new[] { "<{0}>" }, Accepted = new[] { ClassificationNames.Tag } },
            new Spec { Ids = new[] { "css" }, Contexts = new[] { ".a { {0}: 1 }", ".a { color: {0} }" },
                       Accepted = new[] { ClassificationNames.CssProperty, ClassificationNames.CssValue } },
        };

        private sealed class Found { public int Start, End; public string Type; }

        private static List<Found> Tokens(IEmbeddedLanguageTokenizer tokenizer, string text)
        {
            var tokens = new List<Found>();
            char[] chars = text.ToCharArray();
            tokenizer.Tokenize(chars, 0, chars.Length, (a, b, type) => tokens.Add(new Found { Start = a, End = b, Type = type }));
            return tokens;
        }

        private static string TypeOfWord(IEmbeddedLanguageTokenizer tokenizer, string context, string word, out bool whole)
        {
            int at = context.IndexOf("{0}", StringComparison.Ordinal);
            string text = context.Replace("{0}", word);
            Found token = Tokens(tokenizer, text).FirstOrDefault(t => t.Start <= at && at < t.End);
            whole = token != null && token.Start == at && token.End == at + word.Length;
            return token == null ? null : token.Type;
        }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            IEmbeddedLanguageRegistry registry = NestLightComposition.CreateEmbeddedLanguages();
            var table = new Table("Vocabulary against the tokenizer", "Language", "Words", "One token", "Classified as expected", "Examples that failed");
            bool met = true;

            foreach (Spec spec in Specs)
                foreach (string id in spec.Ids)
                {
                    IEmbeddedLanguageTokenizer tokenizer = registry.Find(id);
                    IReadOnlyList<string> words = CompletionLanguages.Default.Find(id).Keywords;
                    int whole = 0, expected = 0;
                    var failures = new List<string>();
                    foreach (string word in words)
                    {
                        bool anyWhole = false, anyExpected = false;
                        foreach (string context in spec.Contexts)
                        {
                            bool isWhole;
                            string type = TypeOfWord(tokenizer, context, word, out isWhole);
                            bool control;
                            string unknown = TypeOfWord(tokenizer, context, "zzqx", out control);
                            anyWhole |= isWhole;
                            if (isWhole && type != null && (spec.Accepted != null ? spec.Accepted.Contains(type) : type != unknown)) anyExpected = true;
                        }
                        if (anyWhole) whole++;
                        if (anyExpected) expected++;
                        if ((!anyWhole || !anyExpected) && failures.Count < 8) failures.Add(word + (anyWhole ? "" : " (split)"));
                    }
                    table.Add(id, words.Count, whole + " (" + Percent(whole, words.Count) + ")", expected + " (" + Percent(expected, words.Count) + ")", string.Join(", ", failures));
                    met &= whole == words.Count && expected * 100 >= words.Count * 95;
                }

            outcome.Tables.Add(table);
            outcome.CriterionMet = met;
            outcome.Headline = met ? "vocabulary and tokenizers agree" : "some words are split or not recognized";
            outcome.Analysis.Add("A word counts as one token if some context gives it a single token that covers it exactly. For CSS the words of both kinds (properties and values) share one list, so each is tried as a property and as a value.");
            return outcome;
        }

        private static string Percent(int part, int whole)
        {
            return whole == 0 ? "-" : (100.0 * part / whole).ToString("F0") + "%";
        }
    }
}
