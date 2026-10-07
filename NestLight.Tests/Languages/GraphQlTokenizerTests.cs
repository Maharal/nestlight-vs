using System.Linq;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>GraphQL: operations, fields, arguments, variables, types, directives.</summary>
    public class GraphQlTokenizerTests
    {
        private static string[] G(string graphql) { return Lexer.Language("graphql", graphql); }

        [Fact]
        public void Query_with_variables_and_arguments()
        {
            Assert.Equal(new[]
            {
                "graphql.operation|query", "graphql.operation|Q", "graphql.variable|$id", "graphql.type|ID",
                "graphql.field|user", "graphql.argument|id", "graphql.variable|$id", "graphql.field|name"
            }, G("query Q($id: ID!) { user(id: $id) { name } }"));
        }

        [Theory]
        [InlineData("query")]
        [InlineData("mutation")]
        [InlineData("subscription")]
        public void Operation_keywords(string keyword)
        {
            Assert.Equal(new[] { "graphql.operation|" + keyword, "graphql.field|a" }, G(keyword + " { a }"));
        }

        [Fact]
        public void Anonymous_operation_has_no_name_to_color()
        {
            Assert.Equal(new[] { "graphql.operation|query", "graphql.field|a" }, G("query { a }"));
        }

        [Fact]
        public void Shorthand_query_is_all_fields()
        {
            Assert.Equal(new[] { "graphql.field|a", "graphql.field|b" }, G("{ a b }"));
        }

        [Fact]
        public void Commas_are_whitespace()
        {
            Assert.Equal(new[] { "graphql.field|a", "graphql.field|b", "graphql.field|c" }, G("{ a, b,, c }"));
        }

        [Fact]
        public void Alias_before_a_colon_is_a_field()
        {
            Assert.Equal(new[] { "graphql.field|me", "graphql.field|user" }, G("{ me: user }"));
        }

        [Fact]
        public void Fragment_definition_and_inline_fragment()
        {
            Assert.Equal(new[]
            {
                "graphql.operation|fragment", "graphql.operation|F", "graphql.keyword|on", "graphql.type|User", "graphql.field|id"
            }, G("fragment F on User { id }"));
            Assert.Equal(new[] { "graphql.field|a", "graphql.keyword|on", "graphql.type|User", "graphql.field|id" },
                G("{ a ... on User { id } }"));
        }

        [Fact]
        public void A_field_called_on_is_a_field_outside_fragment_syntax()
        {
            Assert.Equal(new[] { "graphql.field|on" }, G("{ on }"));
        }

        [Fact]
        public void Directives_and_literals()
        {
            Assert.Equal(new[]
            {
                "graphql.field|a", "graphql.directive|@include", "graphql.argument|if", "graphql.keyword|true",
                "graphql.directive|@skip", "graphql.argument|if", "graphql.variable|$x"
            }, G("{ a @include(if: true) @skip(if: $x) }"));
        }

        [Theory]
        [InlineData("null")]
        [InlineData("false")]
        public void Null_and_boolean_literals_are_keywords(string literal)
        {
            Assert.Equal(new[] { "graphql.field|f", "graphql.argument|a", "graphql.keyword|" + literal },
                G("{ f(a: " + literal + ") }"));
        }

        [Theory]
        [InlineData("1")]
        [InlineData("-1")]
        [InlineData("1.5")]
        [InlineData("-1.5e3")]
        [InlineData("2E+4")]
        public void Numbers(string number)
        {
            Assert.Equal(new[] { "graphql.field|f", "graphql.argument|a", "graphql.number|" + number },
                G("{ f(a: " + number + ") }"));
        }

        [Fact]
        public void Strings_and_block_strings_and_comments()
        {
            Assert.Equal(new[]
            {
                "graphql.comment|# note", "graphql.field|f", "graphql.argument|a", "graphql.string|\"x\\\"y\"",
                "graphql.argument|b", "graphql.string|\"\"\"a \" b\n\"\"\""
            }, G("# note\n{ f(a: \"x\\\"y\", b: \"\"\"a \" b\n\"\"\") }"));
        }

        [Fact]
        public void Variable_types_with_lists_and_non_null()
        {
            Assert.Equal(new[]
            {
                "graphql.operation|query", "graphql.variable|$a", "graphql.type|Int", "graphql.variable|$b",
                "graphql.type|String", "graphql.field|f"
            }, G("query ($a: Int!, $b: [String!]!) { f }"));
        }

        [Fact]
        public void Default_values_are_not_types()
        {
            Assert.Equal(new[]
            {
                "graphql.operation|query", "graphql.variable|$a", "graphql.type|Int", "graphql.number|5", "graphql.field|f"
            }, G("query ($a: Int = 5) { f }"));
        }

        [Fact]
        public void Object_values_have_argument_like_keys_inside_parentheses()
        {
            Assert.Equal(new[]
            {
                "graphql.field|f", "graphql.argument|filter", "graphql.argument|a", "graphql.number|1"
            }, G("{ f(filter: { a: 1 }) }"));
        }

        // ---- schema definitions ---------------------------------------------------------------------------

        [Fact]
        public void Object_type_definition()
        {
            Assert.Equal(new[]
            {
                "graphql.keyword|type", "graphql.type|User", "graphql.keyword|implements", "graphql.type|Node",
                "graphql.type|Entity", "graphql.directive|@key",
                "graphql.field|id", "graphql.type|ID",
                "graphql.field|tags", "graphql.type|String",
                "graphql.field|posts", "graphql.argument|first", "graphql.type|Int", "graphql.number|10", "graphql.type|Post"
            }, G("type User implements Node & Entity @key { id: ID! tags: [String!]! posts(first: Int = 10): [Post] }"));
        }

        [Theory]
        [InlineData("interface")]
        [InlineData("input")]
        [InlineData("enum")]
        [InlineData("scalar")]
        public void Other_definition_keywords_are_followed_by_a_type_name(string keyword)
        {
            Assert.Equal(new[] { "graphql.keyword|" + keyword, "graphql.type|Thing" }, G(keyword + " Thing"));
        }

        [Fact]
        public void Union_members_are_types()
        {
            Assert.Equal(new[] { "graphql.keyword|union", "graphql.type|U", "graphql.type|A", "graphql.type|B" }, G("union U = A | B"));
        }

        [Fact]
        public void Extend_and_directive_definitions()
        {
            Assert.Equal(new[] { "graphql.keyword|extend", "graphql.keyword|type", "graphql.type|Q", "graphql.field|x", "graphql.type|Int" },
                G("extend type Q { x: Int }"));
            var seq = G("directive @auth(role: String) on FIELD | OBJECT");
            Assert.Equal("graphql.keyword|directive", seq[0]);
            Assert.Equal("graphql.directive|@auth", seq[1]);
            Assert.Equal("graphql.argument|role", seq[2]);
            Assert.Equal("graphql.type|String", seq[3]);
            Assert.Equal("graphql.keyword|on", seq[4]);
        }

        [Fact]
        public void Enum_values_are_fields_of_the_enum()
        {
            Assert.Equal(new[] { "graphql.keyword|enum", "graphql.type|Color", "graphql.field|RED", "graphql.field|GREEN" },
                G("enum Color { RED GREEN }"));
        }

        [Fact]
        public void Interpolations_are_skipped()
        {
            var seq = G("query { ${field} other(id: ${id}) }");
            Assert.Equal(new[] { "graphql.operation|query", "graphql.field|other", "graphql.argument|id" }, seq);
        }

        [Fact]
        public void Incomplete_code_never_throws()
        {
            foreach (string code in new[] { "query", "query (", "{ f(", "type", "union U =", "\"abc", "\"\"\"abc", "$", "@", "...", "fragment F on" })
                Assert.NotNull(G(code).ToArray());
        }
    }
}
