namespace NestLight.Completion
{
    /// <summary>XML: no closed vocabulary and no grammar yet, so only the words of the document.</summary>
    internal sealed class XmlCompletion : CompletionLanguage
    {
        public XmlCompletion() : base(new[] { "xml" }) { }
    }
}
