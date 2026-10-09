using System;
using System.Collections.Generic;
using System.Linq;

namespace NestLight.Completion
{
    /// <summary>SQL: keywords that follow the case being typed, a grammar for the place of the caret, and the tables and columns the document declares.</summary>
    internal sealed class SqlCompletion : CompletionLanguage, ISchemaCompletion
    {
        public SqlCompletion() : base(new[] { "sql" }, Vocabularies.For("sql")) { }

        public override bool KeywordsFollowTypedCase { get { return true; } }

        protected override Position ReadPosition(string text, int floor, CompletionSite site) { return Positions.Sql(text, floor, site); }

        public List<string> SchemaWords(string text, CompletionSite site, Position place, int[] ranges, int from, int to)
        {
            SqlSchema.Statement current;
            SqlSchema schema = SqlSchema.Read(text, ranges, from, to, site.Start, site.End, out current);

            var words = new List<string>();
            switch (place.Role)
            {
                case PlaceRole.Table:
                    foreach (SqlSchema.Table t in schema.Tables.OrderByDescending(t => t.Declared).ThenByDescending(t => t.Uses).ThenBy(t => t.Name, StringComparer.OrdinalIgnoreCase))
                        words.Add(t.Name);
                    break;
                case PlaceRole.Member:
                {
                    SqlSchema.Table table;
                    if (!current.Names.TryGetValue(place.Qualifier, out table)) table = schema.Find(place.Qualifier);
                    if (table != null) words.AddRange(table.Columns);
                    break;
                }
                case PlaceRole.Column:
                    foreach (SqlSchema.Table table in current.Tables)
                        foreach (string column in table.Columns)
                            if (!words.Contains(column, StringComparer.OrdinalIgnoreCase)) words.Add(column);
                    break;
            }
            return words.Count == 0 ? null : words;
        }
    }
}
