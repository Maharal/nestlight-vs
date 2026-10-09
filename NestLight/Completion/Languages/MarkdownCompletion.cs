namespace NestLight.Completion
{
    /// <summary>Markdown: no closed vocabulary and no grammar, so only the words of the document.</summary>
    internal sealed class MarkdownCompletion : CompletionLanguage
    {
        public MarkdownCompletion() : base(new[] { "markdown", "md" }) { }
    }
}
