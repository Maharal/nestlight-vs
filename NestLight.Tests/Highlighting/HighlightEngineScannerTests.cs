using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class HighlightEngineScannerTests
    {
        [Fact]
        public void Scanner_property_exposes_the_injected_scanner()
        {
            IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
            IHostScanner scanner = NestLightComposition.CreateScanner(HostLanguage.JavaScript, languages);
            var engine = new HighlightEngine(scanner, languages);

            Assert.Same(scanner, engine.Scanner);
        }

        [Theory]
        [InlineData(HostLanguage.JavaScript)]
        [InlineData(HostLanguage.CSharp)]
        [InlineData(HostLanguage.Python)]
        [InlineData(HostLanguage.Cpp)]
        public void Scanner_is_not_null_for_any_host(HostLanguage host)
        {
            var highlighter = NestLightComposition.CreateHighlighter(host);
            Assert.NotNull(highlighter.Scanner);
        }
    }
}
