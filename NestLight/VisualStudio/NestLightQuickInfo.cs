using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using Microsoft.VisualStudio.Utilities;
using NestLight.Common;
using NestLight.Highlighting;

namespace NestLight.VisualStudio
{
    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("NestLight quick info (JavaScript / TypeScript)")]
    [ContentType("TypeScript")][ContentType("JavaScript")]
    internal sealed class JavaScriptQuickInfoSourceProvider : NestLightQuickInfoSourceProvider
    {
        public JavaScriptQuickInfoSourceProvider() : base(HostLanguage.JavaScript) { }
    }

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("NestLight quick info (C#)")]
    [ContentType("CSharp")]
    internal sealed class CSharpQuickInfoSourceProvider : NestLightQuickInfoSourceProvider
    {
        public CSharpQuickInfoSourceProvider() : base(HostLanguage.CSharp) { }
    }

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("NestLight quick info (Python)")]
    [ContentType("Python")]
    internal sealed class PythonQuickInfoSourceProvider : NestLightQuickInfoSourceProvider
    {
        public PythonQuickInfoSourceProvider() : base(HostLanguage.Python) { }
    }

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name("NestLight quick info (C++)")]
    [ContentType("C/C++")]
    internal sealed class CppQuickInfoSourceProvider : NestLightQuickInfoSourceProvider
    {
        public CppQuickInfoSourceProvider() : base(HostLanguage.Cpp) { }
    }

    internal abstract class NestLightQuickInfoSourceProvider : IAsyncQuickInfoSourceProvider
    {
        private readonly HostLanguage _host;

        protected NestLightQuickInfoSourceProvider(HostLanguage host) { _host = host; }

        public IAsyncQuickInfoSource CreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(
                typeof(NestLightQuickInfoSource),
                () => new NestLightQuickInfoSource(textBuffer, NestLightBuffer.Get(textBuffer, _host)));
        }
    }

    internal sealed class NestLightQuickInfoSource : IAsyncQuickInfoSource
    {
        private readonly ITextBuffer _buffer;
        private readonly NestLightBuffer _shared;

        public NestLightQuickInfoSource(ITextBuffer buffer, NestLightBuffer shared)
        {
            _buffer = buffer;
            _shared = shared;
        }

        public Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken)
        {
            SnapshotPoint? triggerPoint = session.GetTriggerPoint(_buffer.CurrentSnapshot);
            if (!triggerPoint.HasValue) return Task.FromResult<QuickInfoItem>(null);

            ITextSnapshot snapshot = triggerPoint.Value.Snapshot;
            string text = _shared.Text.Of(snapshot);
            int position = triggerPoint.Value.Position;

            foreach (Token token in _shared.Analysis.Highlighter.Highlight(text))
            {
                if (token.Start > position || token.End <= position) continue;
                if (token.End > snapshot.Length) continue;

                string description = DescriptionOf(token.Type);
                if (description == null) continue;

                var span = snapshot.CreateTrackingSpan(token.Start, token.Length, SpanTrackingMode.EdgeInclusive);
                var element = new ContainerElement(ContainerElementStyle.Stacked,
                    new ClassifiedTextElement(new ClassifiedTextRun("keyword", description)));
                return Task.FromResult(new QuickInfoItem(span, element));
            }

            return Task.FromResult<QuickInfoItem>(null);
        }

        private static string DescriptionOf(string classificationType)
        {
            if (classificationType == null) return null;

            if (classificationType == ClassificationNames.SqlKeyword) return "SQL keyword";
            if (classificationType == ClassificationNames.SqlParameter) return "SQL parameter";
            if (classificationType == ClassificationNames.SqlIdentifier) return "SQL identifier";
            if (classificationType == ClassificationNames.SqlString) return "SQL string literal";
            if (classificationType == ClassificationNames.SqlNumber) return "SQL number";
            if (classificationType == ClassificationNames.SqlOperator) return "SQL operator";
            if (classificationType == ClassificationNames.SqlComment) return "SQL comment";

            if (classificationType == ClassificationNames.Tag) return "HTML tag";
            if (classificationType == ClassificationNames.Attribute) return "HTML attribute";
            if (classificationType == ClassificationNames.AttributeEvent) return "HTML event handler attribute";
            if (classificationType == ClassificationNames.AttributeProperty) return "HTML property binding";
            if (classificationType == ClassificationNames.AttributeBoolean) return "HTML boolean attribute";
            if (classificationType == ClassificationNames.AttributeValue) return "HTML attribute value";
            if (classificationType == ClassificationNames.Comment) return "HTML comment";

            if (classificationType == ClassificationNames.CssProperty) return "CSS property";
            if (classificationType == ClassificationNames.CssCustomProperty) return "CSS custom property (variable)";
            if (classificationType == ClassificationNames.CssSelector) return "CSS selector";
            if (classificationType == ClassificationNames.CssSelectorClass) return "CSS class selector";
            if (classificationType == ClassificationNames.CssSelectorId) return "CSS ID selector";
            if (classificationType == ClassificationNames.CssAtRule) return "CSS at-rule";
            if (classificationType == ClassificationNames.CssFunction) return "CSS function";
            if (classificationType == ClassificationNames.CssValue) return "CSS value";
            if (classificationType == ClassificationNames.CssUnit) return "CSS unit";
            if (classificationType == ClassificationNames.CssNumber) return "CSS number";
            if (classificationType == ClassificationNames.CssPseudo) return "CSS pseudo-class / pseudo-element";
            if (classificationType == ClassificationNames.CssComment) return "CSS comment";

            if (classificationType == ClassificationNames.JsonKey) return "JSON key";
            if (classificationType == ClassificationNames.JsonString) return "JSON string";
            if (classificationType == ClassificationNames.JsonNumber) return "JSON number";
            if (classificationType == ClassificationNames.JsonLiteral) return "JSON literal (true / false / null)";

            if (classificationType == ClassificationNames.GqlOperation) return "GraphQL operation";
            if (classificationType == ClassificationNames.GqlKeyword) return "GraphQL keyword";
            if (classificationType == ClassificationNames.GqlType) return "GraphQL type";
            if (classificationType == ClassificationNames.GqlDirective) return "GraphQL directive";
            if (classificationType == ClassificationNames.GqlVariable) return "GraphQL variable";

            if (classificationType == ClassificationNames.XmlTag) return "XML element";
            if (classificationType == ClassificationNames.XmlAttribute) return "XML attribute";
            if (classificationType == ClassificationNames.XmlCData) return "XML CDATA section";
            if (classificationType == ClassificationNames.XmlComment) return "XML comment";

            if (classificationType == ClassificationNames.YamlKey) return "YAML key";
            if (classificationType == ClassificationNames.YamlScalar) return "YAML scalar (number / bool / null)";
            if (classificationType == ClassificationNames.YamlAnchor) return "YAML anchor / alias / tag";
            if (classificationType == ClassificationNames.YamlComment) return "YAML comment";

            if (classificationType == ClassificationNames.MdHeading) return "Markdown heading";
            if (classificationType == ClassificationNames.MdCode) return "Markdown code span";
            if (classificationType == ClassificationNames.MdLink) return "Markdown link";
            if (classificationType == ClassificationNames.MdMath) return "Markdown math expression";

            if (classificationType == ClassificationNames.RegexGroup) return "Regex group";
            if (classificationType == ClassificationNames.RegexClass) return "Regex character class";
            if (classificationType == ClassificationNames.RegexQuantifier) return "Regex quantifier";
            if (classificationType == ClassificationNames.RegexEscape) return "Regex escape sequence";
            if (classificationType == ClassificationNames.RegexAnchor) return "Regex anchor";

            if (classificationType == ClassificationNames.ShaderKeyword) return "Shader keyword";
            if (classificationType == ClassificationNames.ShaderType) return "Shader type";
            if (classificationType == ClassificationNames.ShaderBuiltin) return "Shader built-in function";

            if (classificationType == ClassificationNames.ShellKeyword) return "Shell keyword";
            if (classificationType == ClassificationNames.ShellBuiltin) return "Shell built-in command";
            if (classificationType == ClassificationNames.ShellCommand) return "Shell command";
            if (classificationType == ClassificationNames.ShellVariable) return "Shell variable";
            if (classificationType == ClassificationNames.ShellFlag) return "Shell command flag";
            if (classificationType == ClassificationNames.ShellComment) return "Shell comment";

            if (classificationType == ClassificationNames.TomlTable) return "TOML table header";
            if (classificationType == ClassificationNames.TomlKey) return "TOML key";
            if (classificationType == ClassificationNames.TomlLiteral) return "TOML literal (true / false)";
            if (classificationType == ClassificationNames.TomlDate) return "TOML date / time";
            if (classificationType == ClassificationNames.TomlComment) return "TOML comment";

            return null;
        }

        public void Dispose() { }
    }
}
