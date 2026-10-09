namespace NestLight.Completion
{
    /// <summary>GraphQL: its keywords and the words of the document. It has no grammar yet.</summary>
    internal sealed class GraphQlCompletion : CompletionLanguage
    {
        public GraphQlCompletion() : base(new[] { "graphql", "gql" }, Vocabularies.For("graphql")) { }
    }
}
