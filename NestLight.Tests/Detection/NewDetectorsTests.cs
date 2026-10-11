using NestLight.Detection;
using Xunit;

namespace NestLight.Tests
{
    public class XmlDetectorTests
    {
        private readonly XmlDetector _detector = new XmlDetector();

        [Fact]
        public void Detects_xml_declaration()
        {
            string text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><root><item/></root>";
            Assert.Equal(3, _detector.Score(text, 0, text.Length));
        }

        [Fact]
        public void Detects_xmlns_attribute()
        {
            string text = "<config xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"><item/></config>";
            Assert.Equal(3, _detector.Score(text, 0, text.Length));
        }

        [Fact]
        public void Detects_namespaced_tags()
        {
            string text = "<ns:root><ns:item>value</ns:item></ns:root>";
            Assert.Equal(3, _detector.Score(text, 0, text.Length));
        }

        [Fact]
        public void Does_not_detect_plain_html()
        {
            string text = "<div class=\"test\"><p>hello</p></div>";
            Assert.Equal(0, _detector.Score(text, 0, text.Length));
        }

        [Fact]
        public void Does_not_detect_short_text()
        {
            string text = "<a/>";
            Assert.Equal(0, _detector.Score(text, 0, text.Length));
        }
    }

    public class YamlDetectorTests
    {
        private readonly YamlDetector _detector = new YamlDetector();

        [Fact]
        public void Detects_yaml_with_document_marker()
        {
            string text = "---\nname: test\nversion: 1";
            Assert.Equal(3, _detector.Score(text, 0, text.Length));
        }

        [Fact]
        public void Detects_yaml_with_multiple_key_value_pairs()
        {
            string text = "name: test\nversion: 1\nauthor: me";
            Assert.Equal(3, _detector.Score(text, 0, text.Length));
        }

        [Fact]
        public void Does_not_detect_single_key_value()
        {
            string text = "name: test and things";
            Assert.Equal(0, _detector.Score(text, 0, text.Length));
        }

        [Fact]
        public void Does_not_detect_plain_text()
        {
            string text = "this is just some regular text that is long enough";
            Assert.Equal(0, _detector.Score(text, 0, text.Length));
        }
    }

    public class ShellDetectorTests
    {
        private readonly ShellDetector _detector = new ShellDetector();

        [Fact]
        public void Detects_shebang()
        {
            string text = "#!/bin/bash\necho hello";
            Assert.Equal(3, _detector.Score(text, 0, text.Length));
        }

        [Fact]
        public void Detects_pipe_and_dollar()
        {
            string text = "echo $HOME | grep test";
            Assert.Equal(3, _detector.Score(text, 0, text.Length));
        }

        [Fact]
        public void Does_not_detect_plain_text()
        {
            string text = "this is just some regular text that is long enough";
            Assert.Equal(0, _detector.Score(text, 0, text.Length));
        }
    }

    public class TomlDetectorTests
    {
        private readonly TomlDetector _detector = new TomlDetector();

        [Fact]
        public void Detects_table_and_key_value()
        {
            string text = "[package]\nname = \"test\"";
            Assert.Equal(3, _detector.Score(text, 0, text.Length));
        }

        [Fact]
        public void Does_not_detect_key_value_alone()
        {
            string text = "name = test and things are here for length";
            Assert.Equal(0, _detector.Score(text, 0, text.Length));
        }

        [Fact]
        public void Does_not_detect_plain_text()
        {
            string text = "this is just some regular text that is long enough";
            Assert.Equal(0, _detector.Score(text, 0, text.Length));
        }
    }

    public class NewDetectorIntegrationTests
    {
        private readonly LanguageDetector _detector = LanguageDetector.Create(true);

        [Fact]
        public void Xml_with_declaration_is_detected()
        {
            string text = "<?xml version=\"1.0\"?><root><item>value</item></root>";
            Assert.Equal("xml", _detector.Detect(text, 0, text.Length));
        }

        [Fact]
        public void Yaml_with_markers_is_detected()
        {
            string text = "---\nname: test\nversion: 1\nauthor: me";
            Assert.Equal("yaml", _detector.Detect(text, 0, text.Length));
        }

        [Fact]
        public void Shell_with_shebang_is_detected()
        {
            string text = "#!/bin/bash\necho hello world $var";
            Assert.Equal("shell", _detector.Detect(text, 0, text.Length));
        }

        [Fact]
        public void Toml_with_table_and_values_is_detected()
        {
            string text = "[package]\nname = \"test\"\nversion = 1";
            Assert.Equal("toml", _detector.Detect(text, 0, text.Length));
        }

        [Theory]
        [InlineData("xml")]
        [InlineData("yaml")]
        [InlineData("shell")]
        [InlineData("toml")]
        public void New_detectable_language_is_in_strategies(string id)
        {
            bool found = false;
            foreach (var s in LanguageDetector.Strategies())
                if (s.Id == id) { found = true; break; }
            Assert.True(found, id + " not found in strategies");
        }
    }
}
