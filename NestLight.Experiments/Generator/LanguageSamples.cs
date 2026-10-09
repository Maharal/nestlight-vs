using System.Collections.Generic;

namespace NestLight.Experiments
{
    /// <summary>
    /// One snippet of typical code for each embedded language, written once and independent of the host. <see cref="Slot"/> marks
    /// where the host puts an interpolation (or a plain word, in a host that has none). The snippets avoid backticks, triple
    /// quotes and <c>)x"</c> so that every host can carry them in a raw or template string.
    /// </summary>
    internal static class LanguageSamples
    {
        public const string Slot = "@@";

        /// <summary>The id the plugin knows each language by, in the order of the README.</summary>
        public static readonly string[] Languages =
            { "html", "css", "sql", "json", "graphql", "xml", "markdown", "yaml", "regex", "glsl", "wgsl" };

        private static readonly Dictionary<string, string> Snippets = new Dictionary<string, string>
        {
            { "html", "<ul class=\"list\">\n  <li class=\"item\" data-id=\"1\">@@</li>\n  <!-- note -->\n</ul>" },
            { "css", ".item > a:hover {\n  color: @@;\n  margin: 0 auto !important;\n  --gap: calc(1px + 2em);\n}" },
            { "sql", "SELECT u.id, COUNT(*) AS n\nFROM users u\nWHERE u.id = @@ AND u.name LIKE 'a%' -- note\nGROUP BY u.id;" },
            { "json", "{\"id\": @@, \"tags\": [\"a\", \"b\"], \"ok\": true, \"n\": null, \"v\": -1.5e3}" },
            { "graphql", "query User {\n  user(id: @@) @skip(if: false) {\n    id\n    name\n  }\n}" },
            { "xml", "<item id=\"1\">\n  <name>@@</name>\n  <![CDATA[x]]>\n  <!-- note -->\n</item>" },
            { "markdown", "# Title\n\n*emphasis* and **strong** with [a link](http://x.y) and @@\n\n- item one\n- item two" },
            { "yaml", "name: @@\nitems:\n  - a\n  - &anchor b\ntags: [x, \"y\"] # note" },
            { "regex", "^(?<year>\\d{4})-[a-z]+(\\s|\\.)*@@$" },
            { "glsl", "#version 300 es\nuniform vec3 c;\nvoid main() { gl_FragColor = vec4(@@, 1.0); }" },
            { "wgsl", "@fragment\nfn main() -> @location(0) vec4f { return vec4f(@@, 1.0); }" },
        };

        public static string For(string language) { return Snippets[language]; }
    }
}
