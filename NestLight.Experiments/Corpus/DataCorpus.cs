using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NestLight.Experiments
{
    /// <summary>GraphQL, JSON and YAML the way application and infrastructure code has them.</summary>
    internal static class DataCorpus
    {
        private sealed class Entity
        {
            public string Type, Field;
            public string[] Fields;
            public string[] Children;
        }

        private static readonly Entity[] Entities =
        {
            new Entity { Type = "User", Field = "user", Fields = new[] { "id", "name", "email", "avatarUrl", "role", "createdAt", "isActive" }, Children = new[] { "posts", "orders" } },
            new Entity { Type = "Post", Field = "post", Fields = new[] { "id", "title", "body", "slug", "publishedAt", "likes", "draft" }, Children = new[] { "comments", "tags" } },
            new Entity { Type = "Comment", Field = "comment", Fields = new[] { "id", "text", "createdAt", "score" }, Children = new[] { "replies" } },
            new Entity { Type = "Product", Field = "product", Fields = new[] { "id", "sku", "title", "description", "price", "inStock", "rating" }, Children = new[] { "reviews", "variants" } },
            new Entity { Type = "Order", Field = "order", Fields = new[] { "id", "total", "status", "currency", "placedAt" }, Children = new[] { "items", "payments" } },
            new Entity { Type = "Project", Field = "project", Fields = new[] { "id", "name", "description", "deadline", "archived" }, Children = new[] { "tasks", "members" } },
            new Entity { Type = "Task", Field = "task", Fields = new[] { "id", "title", "priority", "done", "dueDate" }, Children = new[] { "subtasks", "assignees" } },
            new Entity { Type = "Event", Field = "event", Fields = new[] { "id", "title", "startsAt", "endsAt", "venue", "capacity" }, Children = new[] { "attendees", "speakers" } },
        };

        private static readonly string[] Scalars = { "String", "Int", "Float", "Boolean", "ID" };

        private static string Indent(string text, string prefix) { return string.Join("\n", text.Split('\n').Select(l => l.Length == 0 ? l : prefix + l)); }

        // ---- GraphQL --------------------------------------------------------------------------------------------------

        public static CorpusDocument GraphQl(Dice d, int count)
        {
            var sb = new StringBuilder("import { gql } from '@apollo/client';\n\n");
            for (int n = 0; n < count; n++)
            {
                Entity e = d.Pick(Entities);
                string name = e.Type + d.Pick(new[] { "Details", "List", "Summary", "Page", "Card" });
                sb.Append("export const ").Append(name.ToUpperInvariant()).Append(n).Append(" = gql`\n").Append(Indent(GraphQlBody(d, e, name), "  ")).Append("\n`;\n\n");
            }
            return new CorpusDocument { Language = "graphql", Text = sb.ToString(), Snippets = count };
        }

        private static string Selection(Dice d, Entity e, int depth)
        {
            var parts = d.Some(e.Fields, d.Between(2, e.Fields.Length)).ToList();
            if (!parts.Contains("id")) parts.Insert(0, "id");
            string text = string.Join("\n", parts);
            if (depth < 2 && d.Chance(0.6))
            {
                string child = d.Pick(e.Children);
                Entity ce = d.Pick(Entities);
                text += "\n" + child + (d.Chance(0.5) ? "(first: " + d.Pick(new[] { "10", "20", "$first" }) + ")" : "") + " {\n" + Indent(Selection(d, ce, depth + 1), "  ") + "\n}";
            }
            return text;
        }

        private static string GraphQlBody(Dice d, Entity e, string name)
        {
            switch (d.Next(7))
            {
                case 0:
                    return "query Get" + name + "($id: ID!, $first: Int = 10) {\n  " + e.Field + "(id: $id) {\n" + Indent(Selection(d, e, 0), "    ") + "\n  }\n}";
                case 1:
                    return "query List" + name + "($filter: " + e.Type + "Filter, $after: String, $limit: Int = 20) {\n  " + e.Field + "s(filter: $filter, after: $after, first: $limit) {\n    edges {\n      node {\n" + Indent(Selection(d, e, 1), "        ") + "\n      }\n    }\n    pageInfo { hasNextPage endCursor }\n    totalCount\n  }\n}";
                case 2:
                    return "mutation Create" + e.Type + "($input: Create" + e.Type + "Input!) {\n  create" + e.Type + "(input: $input) {\n    " + e.Field + " {\n" + Indent(Selection(d, e, 1), "      ") + "\n    }\n    errors { field message }\n  }\n}";
                case 3:
                    return "fragment " + e.Type + "Fields on " + e.Type + " {\n" + Indent(Selection(d, e, 1), "  ") + "\n}";
                case 4:
                    return "type " + e.Type + (d.Chance(0.3) ? " implements Node" : "") + " {\n" + string.Join("\n", e.Fields.Select(f => "  " + f + ": " + (f == "id" ? "ID!" : d.Pick(Scalars) + (d.Chance(0.5) ? "!" : "")))) + "\n  " + d.Pick(e.Children) + "(first: Int, after: String): " + d.Pick(Entities).Type + "Connection!\n}";
                case 5:
                    return "input " + e.Type + "Filter {\n" + string.Join("\n", d.Some(e.Fields, 3).Select(f => "  " + f + ": " + d.Pick(Scalars))) + "\n  search: String\n  orderBy: " + e.Type + "Order = CREATED_AT_DESC\n}\n\nenum " + e.Type + "Order {\n  CREATED_AT_ASC\n  CREATED_AT_DESC\n  NAME_ASC\n}";
                default:
                    return "subscription On" + e.Type + "Changed($id: ID) {\n  " + e.Field + "Changed(id: $id) {\n" + Indent(Selection(d, e, 1), "    ") + "\n  }\n}";
            }
        }

        // ---- JSON -----------------------------------------------------------------------------------------------------

        private static readonly string[] Words = { "alpha", "beta", "gateway", "worker", "cache", "queue", "billing", "search", "mailer", "scheduler", "metrics", "auth" };

        public static CorpusDocument Json(Dice d, int count)
        {
            var sb = new StringBuilder("export const configs = {\n");
            for (int n = 0; n < count; n++)
                sb.Append("  ").Append(d.Pick(Words)).Append(n).Append(": json`\n").Append(Indent(JsonBody(d), "  ")).Append("\n  `,\n");
            sb.Append("};\n");
            return new CorpusDocument { Language = "json", Text = sb.ToString(), Snippets = count };
        }

        private static string JsonBody(Dice d)
        {
            string service = d.Pick(Words);
            switch (d.Next(6))
            {
                case 0:
                    return "{\n  \"name\": \"" + service + "\",\n  \"version\": \"" + d.Between(0, 3) + "." + d.Between(0, 9) + "." + d.Between(0, 20) + "\",\n  \"private\": true,\n  \"scripts\": {\n    \"build\": \"tsc -p .\",\n    \"test\": \"jest --coverage\",\n    \"lint\": \"eslint src\"\n  },\n  \"dependencies\": {\n    \"express\": \"^4.18.0\",\n    \"lodash\": \"^4.17.21\"\n  }\n}";
                case 1:
                    return "{\n  \"compilerOptions\": {\n    \"target\": \"es2020\",\n    \"module\": \"commonjs\",\n    \"strict\": true,\n    \"esModuleInterop\": true,\n    \"outDir\": \"dist\",\n    \"sourceMap\": " + (d.Chance(0.5) ? "true" : "false") + "\n  },\n  \"include\": [\"src/**/*\"],\n  \"exclude\": [\"node_modules\", \"dist\"]\n}";
                case 2:
                    return "{\n  \"status\": \"ok\",\n  \"data\": {\n    \"id\": " + d.Between(1, 9999) + ",\n    \"displayName\": \"Ada Lovelace\",\n    \"emailVerified\": " + (d.Chance(0.5) ? "true" : "false") + ",\n    \"lastLogin\": null,\n    \"roles\": [\"admin\", \"editor\"],\n    \"preferences\": {\n      \"theme\": \"dark\",\n      \"notifications\": true,\n      \"language\": \"en\"\n    }\n  },\n  \"errors\": []\n}";
                case 3:
                    return "{\n  \"server\": { \"host\": \"localhost\", \"port\": " + d.Pick(new[] { 3000, 8080, 5000 }) + ", \"secure\": false },\n  \"database\": { \"host\": \"db.internal\", \"port\": 5432, \"poolSize\": " + d.Between(5, 50) + ", \"ssl\": true },\n  \"features\": { \"betaSearch\": true, \"legacyExport\": false, \"maintenanceBanner\": null },\n  \"logging\": { \"level\": \"" + d.Pick(new[] { "info", "debug", "warn" }) + "\", \"destination\": \"stdout\" }\n}";
                case 4:
                    return "{\n  \"items\": [\n    { \"id\": 1, \"title\": \"First\", \"tags\": [\"a\", \"b\"], \"done\": false },\n    { \"id\": 2, \"title\": \"Second\", \"tags\": [], \"done\": true }\n  ],\n  \"total\": 2,\n  \"page\": 1,\n  \"hasMore\": false\n}";
                default:
                    return "{\n  \"root\": true,\n  \"extends\": [\"eslint:recommended\", \"plugin:@typescript-eslint/recommended\"],\n  \"rules\": {\n    \"no-console\": \"warn\",\n    \"semi\": [\"error\", \"always\"],\n    \"quotes\": [\"error\", \"single\"]\n  },\n  \"env\": { \"node\": true, \"jest\": true }\n}";
            }
        }

        // ---- YAML -----------------------------------------------------------------------------------------------------

        public static CorpusDocument Yaml(Dice d, int count)
        {
            var sb = new StringBuilder("export const manifests = {\n");
            for (int n = 0; n < count; n++)
                sb.Append("  ").Append(d.Pick(Words)).Append(n).Append(": yaml`\n").Append(Indent(YamlBody(d), "  ")).Append("\n  `,\n");
            sb.Append("};\n");
            return new CorpusDocument { Language = "yaml", Text = sb.ToString(), Snippets = count };
        }

        private static string YamlBody(Dice d)
        {
            string app = d.Pick(Words);
            switch (d.Next(6))
            {
                case 0:
                    return "apiVersion: apps/v1\nkind: Deployment\nmetadata:\n  name: " + app + "\n  labels:\n    app: " + app + "\nspec:\n  replicas: " + d.Between(1, 5) + "\n  selector:\n    matchLabels:\n      app: " + app + "\n  template:\n    metadata:\n      labels:\n        app: " + app + "\n    spec:\n      containers:\n        - name: " + app + "\n          image: registry.example.com/" + app + ":" + d.Between(1, 3) + ".0\n          ports:\n            - containerPort: " + d.Pick(new[] { 8080, 3000 }) + "\n          env:\n            - name: LOG_LEVEL\n              value: info\n          resources:\n            limits:\n              memory: 256Mi\n              cpu: 500m";
                case 1:
                    return "apiVersion: v1\nkind: Service\nmetadata:\n  name: " + app + "\nspec:\n  type: ClusterIP\n  selector:\n    app: " + app + "\n  ports:\n    - protocol: TCP\n      port: 80\n      targetPort: " + d.Pick(new[] { 8080, 3000 });
                case 2:
                    return "name: ci\non:\n  push:\n    branches: [main]\n  pull_request:\njobs:\n  build:\n    runs-on: ubuntu-latest\n    steps:\n      - uses: actions/checkout@v4\n      - name: Install\n        run: npm ci\n      - name: Test\n        run: npm test\n        env:\n          CI: true\n      - name: Upload coverage\n        if: always()\n        uses: actions/upload-artifact@v4\n        with:\n          name: coverage\n          path: coverage/";
                case 3:
                    return "services:\n  " + app + ":\n    build: ./" + app + "\n    ports:\n      - \"3000:3000\"\n    environment:\n      DATABASE_URL: postgres://db:5432/app\n      DEBUG: " + (d.Chance(0.5) ? "true" : "false") + "\n    depends_on:\n      - db\n    restart: unless-stopped\n  db:\n    image: postgres:16\n    volumes:\n      - pgdata:/var/lib/postgresql/data\n    healthcheck:\n      test: [\"CMD\", \"pg_isready\"]\n      interval: 10s\nvolumes:\n  pgdata: ~";
                case 4:
                    return "openapi: 3.0.3\ninfo:\n  title: " + app + " API\n  version: 1.0.0\npaths:\n  /" + app + "s/{id}:\n    get:\n      summary: Get one\n      parameters:\n        - name: id\n          in: path\n          required: true\n          schema:\n            type: string\n      responses:\n        '200':\n          description: OK\n        '404':\n          description: Not found";
                default:
                    return "apiVersion: v1\nkind: ConfigMap\nmetadata:\n  name: " + app + "-config\n  namespace: default\ndata:\n  LOG_LEVEL: debug\n  FEATURE_BETA: \"true\"\n  TIMEOUT_SECONDS: \"30\"\n---\napiVersion: networking.k8s.io/v1\nkind: Ingress\nmetadata:\n  name: " + app + "\n  annotations:\n    nginx.ingress.kubernetes.io/rewrite-target: /\nspec:\n  rules:\n    - host: " + app + ".example.com\n      http:\n        paths:\n          - path: /\n            pathType: Prefix\n            backend:\n              service:\n                name: " + app + "\n                port:\n                  number: 80";
            }
        }
    }
}
