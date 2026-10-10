using System.Collections.Generic;
using System.Text;
using NestLight.Highlighting;

namespace NestLight.Experiments.Detection
{
    /// <summary>
    /// The strings the detector is tried on, and the files that carry them. The proportions are set here and are part of the experiment:
    /// of the 8 unmarked strings added to each unit of code, 6 are ordinary text and 2 are code.
    /// </summary>
    internal static class DetectionCorpus
    {
        /// <summary>Ordinary strings. The last ones are on purpose hard: they look a little like code.</summary>
        public static readonly string[] Ordinary =
        {
            "hello world", "Error: file not found", "C:\\temp\\output.txt", "https://example.com/api/v1/users", "user@example.com",
            "Select an option from the list", "The query returned no rows", "application/json", "{0} items selected", "Settings saved successfully",
            "/usr/local/bin/tool", "Invalid argument: count must be positive", "yyyy-MM-dd HH:mm:ss", "Loading, please wait", "id",
            "Delete the file from disk?", "Update the set of items", "<none>", "style: bold; weight: 700", "Click here to continue"
        };

        /// <summary>The hard ones among <see cref="Ordinary"/>: how many from the end.</summary>
        public const int HardOrdinary = 3;

        public static readonly KeyValuePair<string, string>[] Code =
        {
            new KeyValuePair<string, string>("sql", "SELECT id, name FROM customers WHERE active = 1 ORDER BY name"),
            new KeyValuePair<string, string>("sql", "INSERT INTO orders (customer_id, total) VALUES (1, 99.90)"),
            new KeyValuePair<string, string>("sql", "UPDATE products SET price = price * 1.1 WHERE category = 'toys'"),
            new KeyValuePair<string, string>("sql", "delete from sessions where expires < now()"),
            new KeyValuePair<string, string>("html", "<div class=\"card\"><h2>Title</h2><p>Body</p></div>"),
            new KeyValuePair<string, string>("html", "<li class=\"item\">Item</li>"),
            new KeyValuePair<string, string>("html", "<img src=\"logo.png\" alt=\"Logo\" />"),
            new KeyValuePair<string, string>("json", "{\"name\": \"Ada\", \"age\": 36, \"tags\": [\"a\", \"b\"]}"),
            new KeyValuePair<string, string>("json", "[1, 2, 3, 4, 5]"),
            new KeyValuePair<string, string>("json", "{ \"enabled\": true, \"retries\": 3 }"),
            new KeyValuePair<string, string>("css", ".card { color: red; margin: 0 auto; }"),
            new KeyValuePair<string, string>("css", "a:hover { text-decoration: underline }"),
            new KeyValuePair<string, string>("css", "@media (min-width: 600px) { .a { display: none; } }"),
            new KeyValuePair<string, string>("graphql", "query Users($id: ID!) { user(id: $id) { name email } }"),
            new KeyValuePair<string, string>("graphql", "{ viewer { login repositories(first: 5) { totalCount } } }"),
            new KeyValuePair<string, string>("graphql", "mutation { addTodo(text: \"x\") { id } }"),
        };

        private static string Quote(string s)
        {
            return "\"" + s.Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
        }

        private static string Statement(HostLanguage host, int n, string literal)
        {
            switch (host)
            {
                case HostLanguage.CSharp: return "    var s" + n + " = " + literal + ";\n";
                case HostLanguage.JavaScript: return "  const s" + n + " = " + literal + ";\n";
                case HostLanguage.Python: return "    s" + n + " = " + literal + "\n";
                default: return "    auto s" + n + " = " + literal + ";\n";
            }
        }

        /// <summary>
        /// The synthetic file of the other experiments (a unit of typical code, a marked string in one unit out of ten) with 8 unmarked
        /// strings added to each unit: 6 ordinary and 2 of code, rotating through the pools.
        /// </summary>
        public static string Build(HostLanguage host, int lines)
        {
            var sb = new StringBuilder();
            string plain = SyntheticCode.Unit(host, false), marked = SyntheticCode.Unit(host, true);
            int count = 0, ordinary = 0, code = 0;
            for (int unit = 0; count < lines; unit++)
            {
                string body = unit % 10 == 0 ? marked : plain;
                sb.Append(body);
                count += SyntheticCode.Lines(body);
                for (int k = 0; k < 6; k++, ordinary++) sb.Append(Statement(host, ordinary, Quote(Ordinary[ordinary % Ordinary.Length])));
                for (int k = 0; k < 2; k++, code++) sb.Append(Statement(host, 1000 + code, Quote(Code[code % Code.Length].Value)));
                count += 8;
            }
            return sb.ToString();
        }
    }
}
