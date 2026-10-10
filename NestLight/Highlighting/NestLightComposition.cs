using System;
using NestLight.Common;
using NestLight.Detection;
using NestLight.Completion;
using NestLight.Hosts;
using NestLight.EmbeddedLanguages;

namespace NestLight.Highlighting
{
    public enum HostLanguage { JavaScript, CSharp, Python, Cpp }

    /// <summary>
    /// The composition root: the only place that knows every concrete class and wires them together.
    /// Everything else receives its dependencies through the constructor.
    /// </summary>
    internal static class NestLightComposition
    {
        /// <summary>Strings marked json or regex in C# are left to the built-in support of Visual Studio.</summary>
        private static readonly string[] LeftToVisualStudio = { "json", "regex", "regexp" };

        public static IEmbeddedLanguageRegistry CreateEmbeddedLanguages()
        {
            return new EmbeddedLanguageRegistry(registry => new IEmbeddedLanguageTokenizer[]
            {
                new HtmlTokenizer(registry),
                new CssTokenizer(),
                new SqlTokenizer(),
                new JsonTokenizer(),
                new GraphQlTokenizer(),
                new XmlTokenizer(),
                new MarkdownTokenizer(),
                new YamlTokenizer(),
                new RegexTokenizer(),
                new ShaderTokenizer(new[] { "glsl" }, ShaderVocabulary.GlslKeywords, ShaderVocabulary.GlslTypes,
                                    ShaderVocabulary.GlslBuiltins, ShaderVocabulary.GlslBuiltinPrefix),
                new ShaderTokenizer(new[] { "wgsl" }, ShaderVocabulary.WgslKeywords, ShaderVocabulary.WgslTypes,
                                    ShaderVocabulary.WgslBuiltins)
            });
        }

        /// <param name="detection">When given, strings nobody marked get the language guessed from their content, under these options; null (the default) never guesses.</param>
        public static IHostScanner CreateScanner(HostLanguage host, IEmbeddedLanguageRegistry languages, DetectionOptions detection = null)
        {
            ILanguageGuesser guesser = detection == null ? null : new OptionsLanguageGuesser(detection, LanguageDetector.Create(true));
            switch (host)
            {
                case HostLanguage.JavaScript: return new JavaScriptHostScanner(new AcceptedEmbeddedLanguages(languages, null, guesser));
                case HostLanguage.CSharp: return new CSharpHostScanner(new AcceptedEmbeddedLanguages(languages, LeftToVisualStudio, guesser));
                case HostLanguage.Python: return new PythonHostScanner(new AcceptedEmbeddedLanguages(languages, null, guesser));
                case HostLanguage.Cpp: return new CppHostScanner(new AcceptedEmbeddedLanguages(languages, null, guesser));
                default: throw new ArgumentOutOfRangeException("host");
            }
        }

        public static IHighlighter CreateHighlighter(HostLanguage host, DetectionOptions detection = null)
        {
            IEmbeddedLanguageRegistry languages = CreateEmbeddedLanguages();
            return new HighlightEngine(CreateScanner(host, languages, detection), languages);
        }

        /// <summary>
        /// The pieces of one buffer. The highlighter and the completion share one scanner that remembers its last scan, so the
        /// strings of a snapshot are found once for both.
        /// </summary>
        public static BufferAnalysis CreateForBuffer(HostLanguage host, DetectionOptions detection = null)
        {
            IEmbeddedLanguageRegistry languages = CreateEmbeddedLanguages();
            Func<int> version = detection == null ? (Func<int>)null : () => detection.Version;
            IHostScanner scanner = new CachingHostScanner(CreateScanner(host, languages, detection), version);
            return new BufferAnalysis(new HighlightEngine(scanner, languages), new CompletionEngine(scanner, matcher: new BandedPrefixMatcher(), features: CompletionFeatures.Default));
        }
    }

    /// <summary>The highlighter and the completion of one buffer, built over the same scan.</summary>
    internal sealed class BufferAnalysis
    {
        public BufferAnalysis(IHighlighter highlighter, ICompletionProvider completion)
        {
            Highlighter = highlighter;
            Completion = completion;
        }

        public IHighlighter Highlighter { get; private set; }
        public ICompletionProvider Completion { get; private set; }
    }
}
