using System.Linq;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>JavaScript / TypeScript: every embedded language, and the `//` marker, `language=` and `lang=` forms.</summary>
    public class JavaScriptMarkersTests
    {
        private static string[] Ids(string code)
        {
            return Lexer.Templates(code).Select(t => t.EmbeddedLanguageId).ToArray();
        }

        [Theory]
        [InlineData("html")]
        [InlineData("htm")]
        [InlineData("svg")]
        [InlineData("css")]
        [InlineData("sql")]
        [InlineData("json")]
        [InlineData("graphql")]
        [InlineData("gql")]
        [InlineData("xml")]
        [InlineData("markdown")]
        [InlineData("md")]
        [InlineData("yaml")]
        [InlineData("yml")]
        [InlineData("regex")]
        [InlineData("regexp")]
        [InlineData("glsl")]
        [InlineData("wgsl")]
        public void Every_documented_id_works_as_a_tag(string id)
        {
            Assert.Equal(new[] { id }, Ids(id + "`x`"));
        }

        [Theory]
        [InlineData("html")]
        [InlineData("sql")]
        [InlineData("wgsl")]
        public void Every_documented_id_works_as_a_block_comment_marker(string id)
        {
            Assert.Equal(new[] { id }, Ids("const a = /* " + id + " */ `x`;"));
        }

        [Theory]
        [InlineData("// html\nconst a = `x`;", "html")]
        [InlineData("// language=sql\nconst a = `x`;", "sql")]
        [InlineData("// lang=css\nconst a = `x`;", "css")]
        [InlineData("const a = // json\n  `x`;", "json")]
        [InlineData("const a = // html\r\n  `x`;", "html")]
        [InlineData("const a = /* language=yaml */ `x`;", "yaml")]
        [InlineData("const a = /* lang=xml */ `x`;", "xml")]
        [InlineData("const a = /* LANG=GQL */ `x`;", "gql")]
        public void Line_and_block_markers_apply_on_the_same_line_or_the_line_above(string code, string id)
        {
            Assert.Equal(new[] { id }, Ids(code));
        }

        [Theory]
        [InlineData("// html\n\nconst a = `x`;")]
        [InlineData("// html\nconst b = 1;\nconst a = `x`;")]
        [InlineData("// html\nfoo(); const a = `x`;")]
        [InlineData("// html is great\nconst a = `x`;")]
        [InlineData("// TODO: html\nconst a = `x`;")]
        public void Markers_that_do_not_touch_the_string_are_ignored(string code)
        {
            Assert.Empty(Ids(code));
        }

        [Fact]
        public void A_marker_marks_only_the_first_string_after_it()
        {
            Assert.Equal(new[] { "html" }, Ids("// html\nconst a = [`x`, `y`];"));
        }

        [Fact]
        public void An_ordinary_string_in_between_takes_the_marker()
        {
            Assert.Empty(Ids("// html\nconst a = f('k', `x`);"));
        }

        [Fact]
        public void A_tag_wins_over_a_marker_comment()
        {
            Assert.Equal(new[] { "sql" }, Ids("// html\nconst a = sql`x`;"));
        }

        [Fact]
        public void An_unknown_tag_falls_back_to_the_marker()
        {
            Assert.Equal(new[] { "css" }, Ids("// css\nconst a = styled`x`;"));
        }

        [Fact]
        public void Member_access_tags_work()
        {
            Assert.Equal(new[] { "sql" }, Ids("db.sql`select 1`"));
        }

        [Fact]
        public void Untagged_templates_are_embedded_only_when_marked()
        {
            Assert.Empty(Ids("const a = `select 1`;"));
            Assert.Equal(new[] { "sql" }, Ids("const a = /* sql */ `select 1`;"));
        }

        [Fact]
        public void Marker_inside_a_nested_expression_marks_the_nested_template()
        {
            Assert.Equal(new[] { "html", "css" }, Ids("html`${ /* css */ `a{}` }`"));
        }
    }
}
