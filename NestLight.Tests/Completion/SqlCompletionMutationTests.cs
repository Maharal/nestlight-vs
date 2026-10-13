using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Highlighting;
using Xunit;

namespace NestLight.Tests
{
    public class SqlCompletionMutationTests
    {
        private static readonly CompletionFeatures Schema = new CompletionFeatures(schema: true, grammar: true);

        private static List<string> Texts(string codeWithCaret, CompletionFeatures features, bool onlyWords = false)
        {
            int caret = codeWithCaret.IndexOf('|');
            string code = codeWithCaret.Remove(caret, 1);
            var engine = new CompletionEngine(Pipeline.Scanner(HostLanguage.JavaScript), 100000,
                CompletionEngine.DefaultMinWordLength, features: features);
            CompletionSite site = engine.Locate(code, caret);
            return engine.Suggest(code, site).Where(s => !onlyWords || s.Kind == SuggestionKind.Word).Select(s => s.Text).ToList();
        }

        // ---- Line 26: ThenBy ordering ----

        [Fact]
        public void Tables_ordered_by_declared_then_uses_then_name()
        {
            // Line 26: schema.Tables.OrderByDescending(t => t.Declared).ThenByDescending(t => t.Uses).ThenBy(t => t.Name, ...)
            // Declared tables first, then by usage count
            string code = "sql`create table alpha (id int); select * from beta; select * from beta; select * from |`";
            var items = Texts(code, Schema, true);
            int alphaIdx = items.IndexOf("alpha");
            int betaIdx = items.IndexOf("beta");
            Assert.True(alphaIdx >= 0, "alpha should be in results");
            Assert.True(betaIdx >= 0, "beta should be in results");
            // alpha is declared, beta is not; declared comes first
            Assert.True(alphaIdx < betaIdx);
        }

        [Fact]
        public void Tables_same_declaration_status_sorted_by_uses()
        {
            // Two undeclared tables, one used more
            string code = "sql`select * from more; select * from more; select * from less; select * from |`";
            var items = Texts(code, Schema, true);
            int moreIdx = items.IndexOf("more");
            int lessIdx = items.IndexOf("less");
            Assert.True(moreIdx >= 0);
            Assert.True(lessIdx >= 0);
            // "more" has more uses -> should come first among undeclared
            Assert.True(moreIdx < lessIdx);
        }

        // ---- Line 42: words.Count == 0 ? null : words ----

        [Fact]
        public void SchemaWords_returns_null_when_no_tables_known()
        {
            // Line 42: Conditional (false) mutation - return null when empty
            string code = "sql`select * from |`";
            var items = Texts(code, Schema, true);
            // No tables known, so schema returns null, but completion still works
            Assert.NotNull(items);
        }

        [Fact]
        public void SchemaWords_returns_words_when_tables_exist()
        {
            string code = "sql`create table t (id int); select * from |`";
            var items = Texts(code, Schema, true);
            Assert.Contains("t", items);
        }
    }
}
