using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NestLight.Common;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class LanguageRegistryTests
    {
        private sealed class Stub : ILanguageTokenizer
        {
            private readonly string[] _ids;
            public Stub(params string[] ids) { _ids = ids; }
            public IReadOnlyList<string> Ids { get { return _ids; } }
            public void Tokenize(char[] text, int from, int to, TokenSink emit) { }
        }

        [Fact]
        public void Ids_and_aliases_resolve_to_the_same_tokenizer_case_insensitively()
        {
            var stub = new Stub("html", "htm");
            var registry = new LanguageRegistry(_ => new ILanguageTokenizer[] { stub });

            Assert.Same(stub, registry.Find("html"));
            Assert.Same(stub, registry.Find("HTM"));
            Assert.True(registry.IsKnown("Html"));
        }

        [Fact]
        public void Unknown_and_null_ids_are_not_found()
        {
            var registry = new LanguageRegistry(_ => new ILanguageTokenizer[] { new Stub("a") });
            Assert.Null(registry.Find("b"));
            Assert.Null(registry.Find(null));
            Assert.False(registry.IsKnown("b"));
            Assert.False(registry.IsKnown(null));
        }

        [Fact]
        public void The_same_id_twice_is_a_configuration_error()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new LanguageRegistry(_ => new ILanguageTokenizer[] { new Stub("a"), new Stub("A") }));
        }

        [Fact]
        public void The_factory_receives_the_registry_so_languages_can_delegate_to_each_other()
        {
            ILanguageRegistry received = null;
            var registry = new LanguageRegistry(r => { received = r; return new ILanguageTokenizer[0]; });
            Assert.Same(registry, received);
        }

        [Fact]
        public void A_factory_is_required()
        {
            Assert.Throws<ArgumentNullException>(() => new LanguageRegistry(null));
        }
    }

    public class CompositionTests
    {
        // the ids of the "Embedded languages" table of the README
        public static IEnumerable<object[]> DocumentedIds()
        {
            foreach (string id in new[]
            {
                "html", "htm", "svg", "css", "sql", "json", "graphql", "gql", "xml", "markdown", "md",
                "yaml", "yml", "regex", "regexp", "glsl", "wgsl"
            })
                yield return new object[] { id };
        }

        [Theory]
        [MemberData(nameof(DocumentedIds))]
        public void Every_documented_language_id_is_registered(string id)
        {
            Assert.True(Pipeline.Languages.IsKnown(id));
            Assert.Contains(id, Pipeline.Languages.Find(id).Ids);
        }

        [Fact]
        public void Ten_documented_languages_are_served_by_eleven_tokenizers()
        {
            var tokenizers = ((LanguageRegistry)Pipeline.Languages).Ids.Select(Pipeline.Languages.Find).Distinct().ToList();
            Assert.Equal(11, tokenizers.Count); // ten README rows, shaders being two tokenizers (glsl, wgsl)
        }

        [Theory]
        [InlineData(HostLanguage.JavaScript)]
        [InlineData(HostLanguage.CSharp)]
        [InlineData(HostLanguage.Python)]
        [InlineData(HostLanguage.Cpp)]
        public void Every_host_gets_a_working_highlighter(HostLanguage host)
        {
            Assert.NotNull(NestLightComposition.CreateHighlighter(host));
            Assert.Empty(NestLightComposition.CreateHighlighter(host).Highlight(""));
        }

        [Fact]
        public void An_unknown_host_is_rejected()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                NestLightComposition.CreateScanner((HostLanguage)99, Pipeline.Languages));
        }

        [Fact]
        public void Each_call_builds_independent_object_graphs()
        {
            Assert.NotSame(NestLightComposition.CreateLanguages(), NestLightComposition.CreateLanguages());
        }

        [Fact]
        public void Components_reject_missing_dependencies()
        {
            Assert.Throws<ArgumentNullException>(() => new HighlightEngine(null, Pipeline.Languages));
            Assert.Throws<ArgumentNullException>(() => new HighlightEngine(Pipeline.Scanner(HostLanguage.JavaScript), null));
            Assert.Throws<ArgumentNullException>(() => new NestLight.Hosts.JavaScriptHostScanner(null));
            Assert.Throws<ArgumentNullException>(() => new NestLight.Hosts.CSharpHostScanner(null));
            Assert.Throws<ArgumentNullException>(() => new NestLight.Hosts.PythonHostScanner(null));
            Assert.Throws<ArgumentNullException>(() => new NestLight.Hosts.CppHostScanner(null));
            Assert.Throws<ArgumentNullException>(() => new NestLight.Languages.HtmlTokenizer(null));
            Assert.Throws<ArgumentNullException>(() => new HostLanguages(null));
        }
    }

    /// <summary>
    /// The editor only knows a classification type if the extension declares it, so every name the tokenizers
    /// can produce must have a type and a default format in the VisualStudio folder.
    /// </summary>
    public class VisualStudioContractTests
    {
        private static string VisualStudioSources()
        {
            string dir = AppContext.BaseDirectory;
            while (dir != null && !Directory.Exists(Path.Combine(dir, "NestLight", "VisualStudio")))
                dir = Path.GetDirectoryName(dir);
            Assert.NotNull(dir);
            return string.Join("\n", Directory.GetFiles(Path.Combine(dir, "NestLight", "VisualStudio"), "*.cs")
                .Select(File.ReadAllText));
        }

        private static IEnumerable<FieldInfo> Names()
        {
            return typeof(ClassificationNames)
                .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.IsLiteral);
        }

        [Fact]
        public void Every_classification_name_has_a_type_definition()
        {
            string sources = VisualStudioSources();
            foreach (FieldInfo f in Names())
                Assert.True(sources.Contains("[Name(ClassificationNames." + f.Name + ")]"), "no type definition for " + f.Name);
        }

        [Fact]
        public void Every_classification_name_has_a_default_format()
        {
            string sources = VisualStudioSources();
            foreach (FieldInfo f in Names())
                Assert.True(sources.Contains("ClassificationTypeNames = ClassificationNames." + f.Name + ")]"), "no format for " + f.Name);
        }

        [Fact]
        public void Every_host_has_a_classifier_provider()
        {
            string sources = VisualStudioSources();
            foreach (HostLanguage host in Enum.GetValues(typeof(HostLanguage)))
                Assert.True(sources.Contains("HostLanguage." + host), "no provider for " + host);
        }

        [Theory]
        [InlineData("TypeScript")]
        [InlineData("JavaScript")]
        [InlineData("CSharp")]
        [InlineData("Python")]
        [InlineData("C/C++")]
        public void Each_documented_host_content_type_is_served(string contentType)
        {
            Assert.Contains("[ContentType(\"" + contentType + "\")]", VisualStudioSources());
        }

        [Fact]
        public void Every_token_type_a_language_emits_is_a_declared_name()
        {
            var declared = new HashSet<string>(Names().Select(f => (string)f.GetRawConstantValue()));
            string sample = string.Join("\n",
                "html`<a href=\"x\" @c=${f} .p=${v} ?b=${t} style=\"a:b\"><style>x{y:z}</style><!-- c --></a>`",
                "css`a.b:hover{--x:1px;color:#fff!important;w:calc(1px+2%);@media(x){}}`",
                "sql`select a, 'b', 1 from \"t\" where x <> 2 -- c`",
                "json`{\"a\": [1, true, null]}`",
                "gql`query Q($a: Int) { u(id: $a) @skip(if: true) { name } } type T { f: Int }`",
                "xml`<?xml v=\"1\"?><a b=\"c\"><![CDATA[x]]><!-- c --></a>`",
                "md`# h\\n*e* **s** \\`c\\` [l](u)\\n- i`",
                "yaml`a: 1\\nb: &x \"s\"\\n# c\\nc: *x`",
                "regex`^(a|b)[c-d]+\\\\d{2}$`",
                "glsl`#version 300 es\\nuniform vec3 a; void main() { gl_Position = vec4(1.0); } // c`",
                "wgsl`@vertex fn f() -> vec4f { return vec4f(1.0); }`");

            foreach (var t in Lexer.Lex(sample))
                Assert.True(declared.Contains(t.Type), "undeclared classification: " + t.Type);
        }
    }
}
