using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NestLight.Experiments
{
    /// <summary>
    /// Queries written the way application code has them: a schema of a few dozen tables, joins on the foreign keys, aggregates,
    /// subqueries, windows, inserts, updates, migrations. Upper-case or lower-case keywords, the same in a whole file.
    /// </summary>
    internal static class SqlCorpus
    {
        private sealed class Table
        {
            public string Name, Alias;
            public string[] Columns;
            public string[] Parents; // names of the tables this one points to
        }

        private static readonly Table[] Schema =
        {
            new Table { Name = "users", Alias = "u", Columns = new[] { "id", "email", "display_name", "role", "active", "created_at", "last_login_at" }, Parents = new string[0] },
            new Table { Name = "orders", Alias = "o", Columns = new[] { "id", "user_id", "total", "status", "currency", "placed_at", "shipped_at" }, Parents = new[] { "users" } },
            new Table { Name = "order_items", Alias = "oi", Columns = new[] { "id", "order_id", "product_id", "quantity", "unit_price", "discount" }, Parents = new[] { "orders", "products" } },
            new Table { Name = "products", Alias = "p", Columns = new[] { "id", "sku", "title", "price", "stock", "category_id", "archived" }, Parents = new[] { "categories" } },
            new Table { Name = "categories", Alias = "c", Columns = new[] { "id", "name", "slug", "parent_id", "position" }, Parents = new string[0] },
            new Table { Name = "payments", Alias = "pay", Columns = new[] { "id", "order_id", "amount", "method", "paid_at", "refunded" }, Parents = new[] { "orders" } },
            new Table { Name = "invoices", Alias = "i", Columns = new[] { "id", "customer_id", "number", "amount", "issued_at", "due_at", "paid" }, Parents = new[] { "customers" } },
            new Table { Name = "customers", Alias = "cu", Columns = new[] { "id", "company", "country", "vat_number", "credit_limit", "created_at" }, Parents = new string[0] },
            new Table { Name = "sessions", Alias = "s", Columns = new[] { "id", "user_id", "token", "ip_address", "user_agent", "expires_at", "last_seen" }, Parents = new[] { "users" } },
            new Table { Name = "events", Alias = "e", Columns = new[] { "id", "user_id", "type", "payload", "created_at" }, Parents = new[] { "users" } },
            new Table { Name = "posts", Alias = "po", Columns = new[] { "id", "author_id", "title", "body", "published", "published_at", "views" }, Parents = new[] { "users" } },
            new Table { Name = "comments", Alias = "cm", Columns = new[] { "id", "post_id", "author_id", "body", "created_at", "approved" }, Parents = new[] { "posts", "users" } },
            new Table { Name = "employees", Alias = "emp", Columns = new[] { "id", "first_name", "last_name", "department_id", "manager_id", "salary", "hired_on" }, Parents = new[] { "departments" } },
            new Table { Name = "departments", Alias = "d", Columns = new[] { "id", "name", "budget", "location" }, Parents = new string[0] },
            new Table { Name = "tickets", Alias = "t", Columns = new[] { "id", "project_id", "assignee_id", "title", "priority", "status", "opened_at", "closed_at" }, Parents = new[] { "projects", "users" } },
            new Table { Name = "projects", Alias = "pr", Columns = new[] { "id", "name", "owner_id", "deadline", "budget", "archived" }, Parents = new[] { "users" } },
        };

        private static readonly string[] Params = { "${{{0}}}", "$1", "?", ":{0}", "@{0}" };
        private static readonly string[] Ops = { "=", "<>", ">", "<", ">=", "<=" };
        private static readonly string[] Literals = { "'active'", "'pending'", "true", "false", "0", "1", "100", "'USD'", "NULL" };
        private static readonly string[] Aggregates = { "COUNT", "SUM", "AVG", "MIN", "MAX" };
        private static readonly string[] Types = { "INTEGER", "BIGINT", "TEXT", "VARCHAR(255)", "BOOLEAN", "TIMESTAMP", "NUMERIC(12, 2)", "UUID", "JSONB", "DATE" };

        private sealed class Style
        {
            public bool Upper;
            public string Param;
            public bool Aliases;
            public string K(string keyword) { return Upper ? keyword.ToUpperInvariant() : keyword.ToLowerInvariant(); }
        }

        /// <summary>One file: host code with about <paramref name="count"/> queries, all in the style of one team.</summary>
        public static CorpusDocument Document(Dice d, int count)
        {
            var style = new Style { Upper = d.Chance(0.6), Param = d.Pick(Params), Aliases = d.Chance(0.7) };
            var sb = new StringBuilder();
            sb.AppendLine("import { db } from './db';");
            sb.AppendLine();
            for (int n = 0; n < count; n++)
            {
                Table t = d.Pick(Schema);
                string function = FunctionName(d, t);
                sb.Append("export async function ").Append(function).AppendLine("(params) {");
                string body = Statement(d, style, t);
                sb.Append("  return db.query(sql`").Append('\n').Append(Indent(body, "    ")).Append("\n  `);\n}\n\n");
            }
            return new CorpusDocument { Language = "sql", Text = sb.ToString(), Snippets = count };
        }

        private static string FunctionName(Dice d, Table t)
        {
            string singular = t.Name.EndsWith("s") ? t.Name.Substring(0, t.Name.Length - 1) : t.Name;
            string[] verbs = { "find", "list", "load", "count", "update", "remove", "create", "search", "archive", "report" };
            return d.Pick(verbs) + char.ToUpperInvariant(singular[0]) + singular.Substring(1).Replace("_", "") + (d.Chance(0.4) ? d.Pick(new[] { "ById", "ForUser", "Summary", "Page", "Since" }) : "");
        }

        private static string Indent(string text, string prefix)
        {
            return string.Join("\n", text.Split('\n').Select(l => prefix + l));
        }

        private static string Param(Style s, string name)
        {
            return string.Format(s.Param, name);
        }

        private static string Statement(Dice d, Style s, Table t)
        {
            switch (d.Weighted(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }))
            {
                case 0: return SimpleSelect(d, s, t);
                case 1: return JoinSelect(d, s, t);
                case 2: return Aggregate(d, s, t);
                case 3: return Insert(d, s, t);
                case 4: return Update(d, s, t);
                case 5: return Subquery(d, s, t);
                case 6: return Delete(d, s, t);
                case 7: return Cte(d, s, t);
                case 8: return Create(d, s, t);
                case 9: return Alter(d, s, t);
                case 10: return CaseWhen(d, s, t);
                default: return Union(d, s, t);
            }
        }

        private static string Cond(Dice d, Style s, Table t, string alias)
        {
            string column = d.Pick(t.Columns);
            string q = alias == null ? column : alias + "." + column;
            switch (d.Next(6))
            {
                case 0: return q + " " + d.Pick(Ops) + " " + Param(s, column);
                case 1: return q + " " + s.K("IS NOT NULL");
                case 2: return q + " " + s.K("IN") + " (" + Param(s, column + "s") + ")";
                case 3: return q + " " + s.K("LIKE") + " " + Param(s, column);
                case 4: return q + " " + s.K("BETWEEN") + " " + Param(s, "from") + " " + s.K("AND") + " " + Param(s, "to");
                default: return q + " " + d.Pick(Ops) + " " + d.Pick(Literals);
            }
        }

        private static string Where(Dice d, Style s, Table t, string alias)
        {
            var sb = new StringBuilder(s.K("WHERE") + " " + Cond(d, s, t, alias));
            for (int i = d.Next(3); i > 0; i--) sb.Append("\n  ").Append(s.K(d.Chance(0.75) ? "AND" : "OR")).Append(' ').Append(Cond(d, s, t, alias));
            return sb.ToString();
        }

        private static string SimpleSelect(Dice d, Style s, Table t)
        {
            var columns = d.Some(t.Columns, d.Between(2, 5));
            var sb = new StringBuilder(s.K("SELECT") + " " + (d.Chance(0.15) ? "*" : string.Join(", ", columns)) + "\n" + s.K("FROM") + " " + t.Name + "\n" + Where(d, s, t, null));
            if (d.Chance(0.6)) sb.Append("\n" + s.K("ORDER BY") + " " + d.Pick(t.Columns) + (d.Chance(0.5) ? " " + s.K("DESC") : ""));
            if (d.Chance(0.5)) sb.Append("\n" + s.K("LIMIT") + " " + (d.Chance(0.5) ? Param(s, "limit") : "50"));
            if (d.Chance(0.2)) sb.Append("\n" + s.K("OFFSET") + " " + Param(s, "offset"));
            return sb.ToString();
        }

        private static Table Parent(Table t)
        {
            return Schema.FirstOrDefault(x => t.Parents.Contains(x.Name));
        }

        private static string Fk(Table child, Table parent)
        {
            string singular = parent.Name.EndsWith("s") ? parent.Name.Substring(0, parent.Name.Length - 1) : parent.Name;
            string candidate = child.Columns.FirstOrDefault(c => c == singular + "_id" || c == "author_id" && parent.Name == "users" || c == "owner_id" && parent.Name == "users" || c == "assignee_id" && parent.Name == "users" || c == "customer_id" && parent.Name == "customers");
            return candidate ?? "id";
        }

        private static string JoinSelect(Dice d, Style s, Table t)
        {
            Table other = Parent(t);
            if (other == null) return SimpleSelect(d, s, t);
            string a = s.Aliases ? t.Alias : t.Name, b = s.Aliases ? other.Alias : other.Name;
            string join = d.Pick(new[] { "JOIN", "INNER JOIN", "LEFT JOIN" });
            var sb = new StringBuilder(s.K("SELECT") + " " + string.Join(", ", d.Some(t.Columns, 2).Select(c => a + "." + c).Concat(d.Some(other.Columns, 2).Select(c => b + "." + c))) + "\n");
            sb.Append(s.K("FROM") + " " + t.Name + (s.Aliases ? " " + a : "") + "\n");
            sb.Append(s.K(join) + " " + other.Name + (s.Aliases ? " " + b : "") + " " + s.K("ON") + " " + b + ".id = " + a + "." + Fk(t, other) + "\n");
            sb.Append(Where(d, s, t, a));
            if (d.Chance(0.5)) sb.Append("\n" + s.K("ORDER BY") + " " + a + "." + d.Pick(t.Columns));
            if (d.Chance(0.4)) sb.Append("\n" + s.K("LIMIT") + " " + Param(s, "limit"));
            return sb.ToString();
        }

        private static string Aggregate(Dice d, Style s, Table t)
        {
            string group = d.Pick(t.Columns);
            string measure = d.Pick(t.Columns);
            string agg = d.Pick(Aggregates);
            var sb = new StringBuilder(s.K("SELECT") + " " + group + ", " + s.K(agg) + "(" + (agg == "COUNT" ? "*" : measure) + ") " + s.K("AS") + " " + (agg == "COUNT" ? "total" : agg.ToLowerInvariant() + "_" + measure) + "\n");
            sb.Append(s.K("FROM") + " " + t.Name + "\n");
            if (d.Chance(0.6)) sb.Append(Where(d, s, t, null) + "\n");
            sb.Append(s.K("GROUP BY") + " " + group);
            if (d.Chance(0.5)) sb.Append("\n" + s.K("HAVING") + " " + s.K(agg) + "(" + (agg == "COUNT" ? "*" : measure) + ") > " + Param(s, "min"));
            sb.Append("\n" + s.K("ORDER BY") + " 2 " + s.K("DESC"));
            return sb.ToString();
        }

        private static string Insert(Dice d, Style s, Table t)
        {
            var columns = t.Columns.Where(c => c != "id").Take(d.Between(2, 5)).ToList();
            var sb = new StringBuilder(s.K("INSERT INTO") + " " + t.Name + " (" + string.Join(", ", columns) + ")\n");
            sb.Append(s.K("VALUES") + " (" + string.Join(", ", columns.Select(c => Param(s, c))) + ")");
            if (d.Chance(0.4)) sb.Append("\n" + s.K("ON CONFLICT") + " (" + columns[0] + ") " + s.K("DO UPDATE SET") + " " + columns[columns.Count - 1] + " = " + s.K("EXCLUDED") + "." + columns[columns.Count - 1]);
            if (d.Chance(0.4)) sb.Append("\n" + s.K("RETURNING") + " id");
            return sb.ToString();
        }

        private static string Update(Dice d, Style s, Table t)
        {
            var columns = d.Some(t.Columns.Where(c => c != "id").ToList(), d.Between(1, 3));
            var sb = new StringBuilder(s.K("UPDATE") + " " + t.Name + "\n" + s.K("SET") + " " + string.Join(",\n    ", columns.Select(c => c + " = " + (d.Chance(0.2) ? s.K("NOW") + "()" : Param(s, c)))) + "\n");
            sb.Append(s.K("WHERE") + " id = " + Param(s, "id"));
            if (d.Chance(0.3)) sb.Append("\n" + s.K("RETURNING") + " *");
            return sb.ToString();
        }

        private static string Delete(Dice d, Style s, Table t)
        {
            return s.K("DELETE FROM") + " " + t.Name + "\n" + Where(d, s, t, null);
        }

        private static string Subquery(Dice d, Style s, Table t)
        {
            Table other = Parent(t) ?? Schema[0];
            string fk = Fk(t, other);
            string inner = s.K("SELECT") + " id " + s.K("FROM") + " " + other.Name + " " + Where(d, s, other, null).Replace("\n  ", " ");
            if (d.Chance(0.5))
                return s.K("SELECT") + " " + string.Join(", ", d.Some(t.Columns, 3)) + "\n" + s.K("FROM") + " " + t.Name + "\n" + s.K("WHERE") + " " + fk + " " + s.K("IN") + " (\n  " + inner + "\n)";
            return s.K("SELECT") + " " + string.Join(", ", d.Some(t.Columns, 3)) + "\n" + s.K("FROM") + " " + t.Name + " " + (s.Aliases ? t.Alias : "x") + "\n" + s.K("WHERE") + " " + s.K("EXISTS") + " (\n  " + s.K("SELECT") + " 1 " + s.K("FROM") + " " + other.Name + " " + (s.Aliases ? other.Alias : "y") + " " + s.K("WHERE") + " " + (s.Aliases ? other.Alias : "y") + ".id = " + (s.Aliases ? t.Alias : "x") + "." + fk + "\n)";
        }

        private static string Cte(Dice d, Style s, Table t)
        {
            string key = d.Pick(t.Columns.Where(c => c != "id").ToList());
            string measure = d.Pick(t.Columns);
            var sb = new StringBuilder(s.K("WITH") + " ranked " + s.K("AS") + " (\n");
            sb.Append("  " + s.K("SELECT") + " " + key + ", " + measure + ",\n");
            sb.Append("         " + s.K("ROW_NUMBER") + "() " + s.K("OVER") + " (" + s.K("PARTITION BY") + " " + key + " " + s.K("ORDER BY") + " " + measure + " " + s.K("DESC") + ") " + s.K("AS") + " position\n");
            sb.Append("  " + s.K("FROM") + " " + t.Name + "\n)\n");
            sb.Append(s.K("SELECT") + " " + key + ", " + measure + "\n" + s.K("FROM") + " ranked\n" + s.K("WHERE") + " position <= " + d.Pick(new[] { "1", "3", "10" }));
            return sb.ToString();
        }

        private static string Create(Dice d, Style s, Table t)
        {
            var sb = new StringBuilder(s.K("CREATE TABLE") + (d.Chance(0.4) ? " " + s.K("IF NOT EXISTS") : "") + " " + t.Name + " (\n");
            var lines = new List<string>();
            foreach (string c in t.Columns)
            {
                string type = c == "id" ? s.K("SERIAL") + " " + s.K("PRIMARY KEY") : c.EndsWith("_id") ? s.K("INTEGER") + " " + s.K("NOT NULL") : c.EndsWith("_at") ? s.K("TIMESTAMP") + (d.Chance(0.5) ? " " + s.K("DEFAULT") + " " + s.K("now") + "()" : "") : s.K(d.Pick(Types)) + (d.Chance(0.4) ? " " + s.K("NOT NULL") : "") + (d.Chance(0.15) ? " " + s.K("UNIQUE") : "") + (d.Chance(0.15) ? " " + s.K("DEFAULT") + " " + d.Pick(Literals) : "");
                lines.Add("  " + c + " " + type);
            }
            foreach (string parent in t.Parents.Take(1))
                lines.Add("  " + s.K("FOREIGN KEY") + " (" + Fk(t, Schema.First(x => x.Name == parent)) + ") " + s.K("REFERENCES") + " " + parent + " (id)");
            sb.Append(string.Join(",\n", lines)).Append("\n)");
            return sb.ToString();
        }

        private static string Alter(Dice d, Style s, Table t)
        {
            switch (d.Next(4))
            {
                case 0: return s.K("ALTER TABLE") + " " + t.Name + " " + s.K("ADD COLUMN") + " " + d.Pick(new[] { "notes", "deleted_at", "external_ref", "score" }) + " " + s.K(d.Pick(Types));
                case 1: return s.K("ALTER TABLE") + " " + t.Name + " " + s.K("DROP COLUMN") + " " + d.Pick(t.Columns.Where(c => c != "id").ToList());
                case 2: return s.K("CREATE") + (d.Chance(0.3) ? " " + s.K("UNIQUE") : "") + " " + s.K("INDEX") + " idx_" + t.Name + "_" + d.Pick(t.Columns) + " " + s.K("ON") + " " + t.Name + " (" + string.Join(", ", d.Some(t.Columns, d.Between(1, 2))) + ")";
                default: return s.K("ALTER TABLE") + " " + t.Name + " " + s.K("RENAME COLUMN") + " " + d.Pick(t.Columns.Where(c => c != "id").ToList()) + " " + s.K("TO") + " " + d.Pick(new[] { "label", "kind", "owner", "state" });
            }
        }

        private static string CaseWhen(Dice d, Style s, Table t)
        {
            string column = d.Pick(t.Columns.Where(c => c != "id").ToList());
            return s.K("SELECT") + " id,\n       " + s.K("CASE") + " " + s.K("WHEN") + " " + column + " " + d.Pick(Ops) + " " + d.Pick(Literals) + " " + s.K("THEN") + " 'high'\n            " + s.K("WHEN") + " " + column + " " + s.K("IS NULL") + " " + s.K("THEN") + " 'none'\n            " + s.K("ELSE") + " 'normal' " + s.K("END") + " " + s.K("AS") + " bucket\n" + s.K("FROM") + " " + t.Name + "\n" + s.K("ORDER BY") + " id";
        }

        private static string Union(Dice d, Style s, Table t)
        {
            Table other = d.Pick(Schema);
            string a = d.Pick(t.Columns), b = d.Pick(other.Columns);
            return s.K("SELECT") + " " + a + " " + s.K("AS") + " label " + s.K("FROM") + " " + t.Name + "\n" + s.K("UNION ALL") + "\n" + s.K("SELECT") + " " + b + " " + s.K("AS") + " label " + s.K("FROM") + " " + other.Name + "\n" + s.K("ORDER BY") + " label";
        }
    }
}
