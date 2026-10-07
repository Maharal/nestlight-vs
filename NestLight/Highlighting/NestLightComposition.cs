using System;
using NestLight.Common;
using NestLight.Hosts;
using NestLight.Languages;

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

        public static ILanguageRegistry CreateLanguages()
        {
            return new LanguageRegistry(registry => new ILanguageTokenizer[]
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

        public static IHostScanner CreateScanner(HostLanguage host, ILanguageRegistry languages)
        {
            switch (host)
            {
                case HostLanguage.JavaScript: return new JavaScriptHostScanner(new HostLanguages(languages));
                case HostLanguage.CSharp: return new CSharpHostScanner(new HostLanguages(languages, LeftToVisualStudio));
                case HostLanguage.Python: return new PythonHostScanner(new HostLanguages(languages));
                case HostLanguage.Cpp: return new CppHostScanner(new HostLanguages(languages));
                default: throw new ArgumentOutOfRangeException("host");
            }
        }

        public static IHighlighter CreateHighlighter(HostLanguage host)
        {
            ILanguageRegistry languages = CreateLanguages();
            return new HighlightEngine(CreateScanner(host, languages), languages);
        }
    }
}
