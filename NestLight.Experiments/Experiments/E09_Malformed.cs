using NestLight.Common;
using System;
using System.Collections.Generic;
using System.Text;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    internal sealed class E09_Malformed : Experiment
    {
        public override string Id { get { return "E09"; } }
        public override string Title { get { return "Malformed and pathological input"; } }
        public override string Hypothesis { get { return "Code being typed is often malformed. An unterminated string or comment early in a large file, deep nesting, or a very long line can make the scan or a tokenizer quadratic. The robustness tests of the test project only require less than 5 seconds, which would hide a slowdown of several orders of magnitude."; } }
        public override string Method { get { return "For each host: an unterminated marked string and an unterminated block comment at the top of a large file, deep nesting, and one very long line. Each case at a size N and at 2N; the ratio between the two times shows the growth."; } }
        public override string Criterion { get { return "Doubling the input multiplies the time by less than 2.5 (linear is 2, quadratic is 4) in every case, ignoring cases that take less than 5 ms at 2N."; } }
        public override string IfMet { get { return "Bad input does not make the cost explode."; } }
        public override string IfNotMet { get { return "At least one kind of malformed input is superlinear: it will freeze the editor on a large file while the user is typing."; } }

        private const string Body = "<li class='a'>item</li>\n";

        private static string Repeat(string unit, int chars)
        {
            var sb = new StringBuilder();
            while (sb.Length < chars) sb.Append(unit);
            return sb.ToString();
        }

        private static string Opener(HostLanguage host)
        {
            switch (host)
            {
                case HostLanguage.JavaScript: return "html`";
                case HostLanguage.CSharp: return "// language=html\nvar s = @\"";
                case HostLanguage.Python: return "# language=html\ns = \"\"\"";
                default: return "// language=html\nauto s = R\"(";
            }
        }

        private static string NestingOpener(HostLanguage host)
        {
            switch (host)
            {
                case HostLanguage.JavaScript: return "html`";
                case HostLanguage.CSharp: return "// html\n$\"{";
                case HostLanguage.Python: return "# html\nf'{";
                default: return "// html\nR\"(";
            }
        }

        private static string Statement(HostLanguage host)
        {
            switch (host)
            {
                case HostLanguage.JavaScript: return "f(`a ${b}`, 'c'); ";
                case HostLanguage.CSharp: return "f($\"a {b}\", \"c\"); ";
                case HostLanguage.Python: return "f(f'a {b}', 'c'); ";
                default: return "f(\"a\", R\"(c)\"); ";
            }
        }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var table = new Table("Time at N and at 2N", "Host", "Case", "N", "Time at N", "Time at 2N", "Growth", "Gen2 collections (N / 2N)");
            int n = settings.Quick ? 25000 : 250000, depth = settings.Quick ? 100 : 200;
            bool met = true;
            double worst = 0;
            string worstCase = "";

            foreach (HostLanguage host in SyntheticCode.Hosts)
            {
                var cases = new List<KeyValuePair<string, Func<int, string>>>
                {
                    new KeyValuePair<string, Func<int, string>>("unterminated marked string at the top", k => Opener(host) + Repeat(Body, k)),
                    new KeyValuePair<string, Func<int, string>>("one very long line", k => Repeat(Statement(host), k)),
                };
                if (host != HostLanguage.Python)
                    cases.Add(new KeyValuePair<string, Func<int, string>>("unterminated block comment at the top", k => "/* " + Repeat(Body, k)));

                foreach (var c in cases) Compare(c.Key, host, c.Value, n, n * 2, "chars", settings, table, ref met, ref worst, ref worstCase);
                Compare("deep nesting", host, k => Repeat(NestingOpener(host), k), depth, depth * 2, "levels", settings, table, ref met, ref worst, ref worstCase,
                    sizeOf: k => k * NestingOpener(host).Length);
            }

            outcome.Tables.Add(table);
            outcome.CriterionMet = met;
            outcome.Headline = "worst growth " + Measure.Ratio(worst) + " when doubling (" + worstCase + ")";
            outcome.Analysis.Add("The worst growth when doubling the input is " + Measure.Ratio(worst) + " (" + worstCase + "); linear is 2x, quadratic is 4x.");
            outcome.Analysis.Add("Nesting stops at " + depth * 2 + " levels: the stack of the process limits how deep the engine can recurse (the test project covers 400 levels).");
            return outcome;
        }

        private static void Compare(string name, HostLanguage host, Func<int, string> build, int a, int b, string unit, Settings settings, Table table,
            ref bool met, ref double worst, ref string worstCase, Func<int, int> sizeOf = null)
        {
            IHighlighter highlighter = NestLightComposition.CreateHighlighter(host);
            string small = build(a), large = build(b);
            Sample sa = Measure.Run(() => highlighter.Highlight(small), 1, settings.SlowRuns);
            Sample sb = Measure.Run(() => highlighter.Highlight(large), 1, settings.SlowRuns);
            double growth = sb.Ms / sa.Ms;
            table.Add(host, name, a + " " + unit, Measure.Ms(sa.Ms), Measure.Ms(sb.Ms), sb.Ms < 5 ? Measure.Ratio(growth) + " (ignored: under 5 ms)" : Measure.Ratio(growth), sa.Gen2 + " / " + sb.Gen2);
            if (sb.Ms < 5) return;
            met &= growth < 2.5;
            if (growth > worst) { worst = growth; worstCase = host + ", " + name; }
        }
    }
}
