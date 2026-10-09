using System.Collections.Generic;
using System.Linq;
using NestLight.EmbeddedLanguages;

namespace NestLight.Completion
{
    /// <summary>GraphQL: its keywords and the words of the document. It has no grammar yet.</summary>
    internal sealed class GraphQlCompletion : CompletionLanguage
    {
        public GraphQlCompletion() : base(new[] { "graphql", "gql" }, KeywordWords) { }

        private static readonly IReadOnlyList<string> KeywordWords = GraphQlTokenizer.Operations
            .Concat(GraphQlTokenizer.TypeDefinitions)
            .Concat(GraphQlTokenizer.OtherKeywords)
            .Concat(GraphQlTokenizer.Literals)
            .Concat(Words("on Int Float String Boolean ID skip include deprecated specifiedBy"))
            .ToList();
    }
}
