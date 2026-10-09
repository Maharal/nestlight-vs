using System;
using NestLight.Common;
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

        public static IHostScanner CreateScanner(HostLanguage host, IEmbeddedLanguageRegistry languages)
        {
            switch (host)
            {
                case HostLanguage.JavaScript: return new JavaScriptHostScanner(new AcceptedEmbeddedLanguages(languages));
                case HostLanguage.CSharp: return new CSharpHostScanner(new AcceptedEmbeddedLanguages(languages, LeftToVisualStudio));
                case HostLanguage.Python: return new PythonHostScanner(new AcceptedEmbeddedLanguages(languages));
                case HostLanguage.Cpp: return new CppHostScanner(new AcceptedEmbeddedLanguages(languages));
                default: throw new ArgumentOutOfRangeException("host");
            }
        }

        public static IHighlighter CreateHighlighter(HostLanguage host)
        {
            IEmbeddedLanguageRegistry languages = CreateEmbeddedLanguages();
            return new HighlightEngine(CreateScanner(host, languages), languages);
        }

        /// <summary>
        /// The pieces of one buffer. The highlighter and the completion share one scanner that remembers its last scan, so the
        /// strings of a snapshot are found once for both.
        /// </summary>
        public static BufferAnalysis CreateForBuffer(HostLanguage host)
        {
            IEmbeddedLanguageRegistry languages = CreateEmbeddedLanguages();
            IHostScanner scanner = new CachingHostScanner(CreateScanner(host, languages));
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
