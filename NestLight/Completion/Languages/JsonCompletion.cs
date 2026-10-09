
namespace NestLight.Completion
{
    /// <summary>JSON: <c>true</c>, <c>false</c> and <c>null</c>, and a grammar that tells keys from values.</summary>
    internal sealed class JsonCompletion : CompletionLanguage
    {
        public JsonCompletion() : base(new[] { "json" }, Vocabularies.For("json")) { }

        protected override Position ReadPosition(string text, int floor, CompletionSite site) { return Positions.Json(text, floor, site); }
    }
}
