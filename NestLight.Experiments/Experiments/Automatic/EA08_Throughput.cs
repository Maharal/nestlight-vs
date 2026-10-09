using NestLight.Common;
using System;
using System.Collections.Generic;
using System.Text;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class EA08_Throughput : Experiment
    {
        public override string Id { get { return "EA08"; } }
        public override string Title { get { return "Throughput of each embedded language"; } }
        public override string Hypothesis { get { return "All the tokenizers run at a similar speed in characters per millisecond, and none gets slower per character on larger inputs. One of them (regex, markdown, YAML...) may be much slower."; } }
        public override string Method { get { return "One marked string per language, in a JavaScript host, of 100,000 and of 1,000,000 characters made by repeating a snippet of typical code of that language. Characters per millisecond, and the ratio between the two times."; } }
        public override string Criterion { get { return "The slowest language is less than 5 times slower than the median one, and every language takes less than 12 times longer on 10 times the text."; } }
        public override string IfMet { get { return "The languages are balanced and scale linearly; no tokenizer needs attention."; } }
        public override string IfNotMet { get { return "At least one tokenizer is an outlier or superlinear: it is where a large embedded string will hurt first."; } }

        private static readonly KeyValuePair<string, string>[] Snippets =
        {
            P("html", "<div class=\"card\" id=\"c1\"><a href=\"/x?y=1\">link</a><!-- note --><span style=\"color: red; margin: 0 auto\">text &amp; more</span></div>\n"),
            P("css", ".card > a:hover, #id[data-x=\"1\"] { color: #fff; margin: 0 auto !important; --v: calc(1px + 2em); }\n@media (min-width: 600px) { .a { background: url(\"x.png\") } }\n"),
            P("sql", "SELECT a.id, COUNT(*) AS n FROM users a LEFT JOIN orders o ON o.uid = a.id WHERE a.name LIKE 'x%' AND o.total > 10.5 -- note\nGROUP BY a.id ORDER BY n DESC;\n"),
            P("json", "{\"id\": 1, \"name\": \"x\", \"tags\": [\"a\", \"b\"], \"ok\": true, \"n\": null, \"v\": -1.5e3}\n"),
            P("graphql", "query Q($id: ID!) { user(id: $id) @skip(if: false) { name friends(first: 10) { id } } }\n"),
            P("xml", "<a b=\"c\"><![CDATA[x]]><!-- c --><d e='f'>text</d></a>\n"),
            P("markdown", "# Title\n*emphasis* and **strong** with [a link](http://x.y) text\n- item one\n- item two\n\n"),
            P("yaml", "a: 1\nb: [x, \"y\"] # comment\nc:\n  - d\n  - &anchor e\n"),
            P("regex", "^(?<y>\\d{4})-[a-z]+(\\s|\\.)*$|[^abc]{2,5}?\n"),
            P("glsl", "#version 300 es\nuniform vec3 c; void main() { gl_FragColor = vec4(c, 1.0); }\n"),
            P("wgsl", "@vertex fn main() -> @builtin(position) vec4f { return vec4f(1.0); }\n"),
        };

        private static KeyValuePair<string, string> P(string id, string snippet) { return new KeyValuePair<string, string>(id, snippet); }

        private static string Build(string id, string snippet, int chars)
        {
            var sb = new StringBuilder(id + "`");
            while (sb.Length < chars) sb.Append(snippet);
            return sb.Append('`').ToString();
        }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var table = new Table("Throughput by language", "Language", "Small time", "Small chars/ms", "Large time", "Large chars/ms", "Large / small time", "Tokens (large)");
            int small = settings.Quick ? 20000 : 100000, large = small * 10;
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(HostLanguage.JavaScript);
            var throughputs = new List<double>();
            bool met = true;
            string slowest = "";
            double slowestThroughput = double.MaxValue, worstGrowth = 0;

            foreach (var snippet in Snippets)
            {
                string a = Build(snippet.Key, snippet.Value, small), b = Build(snippet.Key, snippet.Value, large);
                int tokens = 0;
                Sample sa = Measure.Run(() => highlighter.Highlight(a), 2, settings.SlowRuns);
                Sample sb = Measure.Run(() => tokens = highlighter.Highlight(b).Count, 2, settings.SlowRuns);
                double ta = a.Length / sa.Ms, tb = b.Length / sb.Ms, growth = sb.Ms / sa.Ms;
                table.Add(snippet.Key, Measure.Ms(sa.Ms), ta.ToString("F0"), Measure.Ms(sb.Ms), tb.ToString("F0"), Measure.Ratio(growth), tokens);
                throughputs.Add(tb);
                if (tb < slowestThroughput) { slowestThroughput = tb; slowest = snippet.Key; }
                worstGrowth = Math.Max(worstGrowth, growth);
                met &= growth < 12;
                if (tokens == 0) outcome.Analysis.Add(snippet.Key + " produced no tokens: the snippet is not recognized, so its row measures nothing.");
            }

            throughputs.Sort();
            double median = throughputs[throughputs.Count / 2];
            met &= median / slowestThroughput < 5;
            outcome.Tables.Add(table);
            outcome.CriterionMet = met;
            outcome.Headline = "slowest " + slowest + " at " + (median / slowestThroughput).ToString("F1") + "x below the median; worst growth " + Measure.Ratio(worstGrowth);
            outcome.Analysis.Add("Median throughput " + median.ToString("F0") + " chars/ms; the slowest language (" + slowest + ") runs at " + slowestThroughput.ToString("F0") + " chars/ms, " + (median / slowestThroughput).ToString("F1") + "x below the median.");
            outcome.Analysis.Add("Worst time growth for 10x the text: " + Measure.Ratio(worstGrowth) + " (linear is 10x).");
            return outcome;
        }
    }
}
