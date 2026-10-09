using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    /// <summary>What can follow a column or a table name, by the clause it is in: one entry of the table for each clause.</summary>
    public class SqlContinuationTests
    {
        private static Position PositionOf(string codeWithCaret)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), features: new CompletionFeatures(grammar: true));
            CompletionSite site = engine.Locate(code, caret);
            return CompletionLanguages.Default.Find(site.EmbeddedLanguageId).PositionAt(code, site);
        }

        [Theory]
        [InlineData("sql`select a |`", "select", "from")]
        [InlineData("sql`select a from t |`", "from", "where")]
        [InlineData("sql`select a from t join u |`", "join", "on")]
        [InlineData("sql`update t |`", "update", "set")]
        [InlineData("sql`update t set a = 1 |`", "set", "where")]
        [InlineData("sql`select a from t limit 5 |`", "limit", "offset")]
        [InlineData("sql`insert into t |`", "into", "values")]
        [InlineData("sql`select a from t group by a |`", "group", "having")]
        [InlineData("sql`select a from t order by a |`", "order", "asc")]
        [InlineData("sql`select a from t where b = 1 |`", "where", "and")]
        [InlineData("sql`select a from t join u on t.id = u.id |`", "on", "and")]
        [InlineData("sql`select a from t group by a having count(a) > 1 |`", "having", "and")]
        public void Each_clause_offers_what_continues_it(string code, string clause, string firstWord)
        {
            Position position = PositionOf(code);
            Assert.NotNull(position);
            Assert.Equal("sql:continue-" + clause, position.Name);
            Assert.Equal(firstWord, position.Expected[0]);
        }

        [Fact]
        public void The_clauses_that_share_a_list_share_the_same_list()
        {
            Position where = PositionOf("sql`select a from t where b = 1 |`"), on = PositionOf("sql`select a from t join u on t.id = u.id |`");
            Assert.Same(where.Expected, on.Expected);
        }

        [Fact]
        public void A_statement_with_no_clause_that_continues_offers_no_continuation()
        {
            Position position = PositionOf("sql`truncate t |`");
            Assert.True(position == null || !position.Name.StartsWith("sql:continue-"));
        }
    }
}
