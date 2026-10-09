namespace NestLight.Completion
{
    /// <summary>Regular expressions: no closed vocabulary and no grammar, so only the words of the document.</summary>
    internal sealed class RegexCompletion : CompletionLanguage
    {
        public RegexCompletion() : base(new[] { "regex", "regexp" }) { }
    }
}
