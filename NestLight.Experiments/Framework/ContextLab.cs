using System;
using System.Collections.Generic;
using System.Linq;
using NestLight.Completion;
using NestLight.Common;
using NestLight.Highlighting;

namespace NestLight.Experiments
{
    /// <summary>A word of an embedded string typed up to <see cref="Prefix"/> letters, and the word that came before it.</summary>
    internal sealed class RankProbe
    {
        public HostLanguage Host;
        public string EmbeddedLanguageId;
        public string Text;
        public int Caret;
        public string Word;
        public int Prefix;
        /// <summary>The word before the one typed, with the punctuation between them ("from", "display:", "u."); empty when there is none.</summary>
        public string Previous = "";
        /// <summary>The place of the caret in the grammar (<c>sql:table</c>, <c>css:value</c>, <c>html:attribute</c>); "(none)" where the language has no grammar or the place says nothing.</summary>
        public string Place = "(none)";
        /// <summary>Whether anything can offer the word: it is a keyword or it exists elsewhere in the document. Set by <see cref="ContextLab.MarkReachable"/>.</summary>
        public bool Reachable;
    }

    /// <summary>
    /// The harness of the experiments on the ranking of the suggestions: the same words typed the same way, and the position of the
    /// word in the list of each variant of the engine.
    /// </summary>
    internal static class ContextLab
    {
        /// <summary>The default limit of the list, as the editor gets it.</summary>
        public const int ListSize = CompletionEngine.DefaultMaxItems;

        public static List<RankProbe> Probes(List<CorpusFile> corpus, int perFile, int maxPrefix)
        {
            var probes = new List<RankProbe>();
            foreach (CorpusFile file in corpus)
            {
                CompletionEngine locator = CompletionLab.Engine(file.Host);
                foreach (var w in CompletionLab.Sample(file, perFile, 4))
                    for (int k = 1; k <= maxPrefix && k < w.Value.Length; k++)
                    {
                        string text = file.Text.Remove(w.Key + k, w.Value.Length - k);
                        CompletionSite site = locator.Locate(text, w.Key + k);
                        if (site == null) continue;
                        probes.Add(new RankProbe { Host = file.Host, EmbeddedLanguageId = site.EmbeddedLanguageId, Text = text, Caret = w.Key + k, Word = w.Value, Prefix = k, Previous = PreviousOf(file.Text, w.Key, site.OwnerStart), Place = PlaceOf(text, site) });
                    }
            }
            return probes;
        }

        private static string PlaceOf(string text, CompletionSite site)
        {
            Position position = CompletionLanguages.Default.Find(site.EmbeddedLanguageId).PositionAt(text, site);
            if (position == null) return "(none)";
            string[] parts = position.Name.Split(':');
            return parts.Length > 2 && (parts[1] == "value" || parts[1] == "attribute") ? parts[0] + ":" + parts[1] : position.Name;
        }

        private static string PreviousOf(string text, int wordStart, int floor)
        {
            int i = wordStart;
            var punctuation = new List<char>();
            while (i > Math.Max(floor, 0) && !(char.IsLetterOrDigit(text[i - 1]) || text[i - 1] == '_'))
            {
                if (!char.IsWhiteSpace(text[i - 1])) punctuation.Insert(0, text[i - 1]);
                i--;
            }
            int end = i;
            while (i > Math.Max(floor, 0) && (char.IsLetterOrDigit(text[i - 1]) || text[i - 1] == '_' || text[i - 1] == '-')) i--;
            if (i == end) return "";
            return text.Substring(i, end - i).ToLowerInvariant() + new string(punctuation.ToArray());
        }

        /// <summary>Marks the probes whose word some list could hold, whatever the ranking: the unlimited list of the first stage has it.</summary>
        public static void MarkReachable(List<RankProbe> probes)
        {
            var engines = new Dictionary<HostLanguage, CompletionEngine>();
            foreach (HostLanguage h in SyntheticCode.Hosts) engines[h] = CompletionLab.Engine(h);
            foreach (RankProbe p in probes)
            {
                CompletionEngine engine = engines[p.Host];
                CompletionSite site = engine.Locate(p.Text, p.Caret);
                p.Reachable = site != null && CompletionLab.RankOf(engine.Suggest(p.Text, site).Select(s => s.Text).ToList(), p.Word) >= 0;
            }
        }

        /// <summary>The 0-based position of the word of each probe in the list of the engine; -1 when it is not in the first <see cref="ListSize"/>.</summary>
        public static int[] Ranks(List<RankProbe> probes, Func<IHostScanner, CompletionEngine> factory)
        {
            var engines = new Dictionary<HostLanguage, CompletionEngine>();
            foreach (HostLanguage h in SyntheticCode.Hosts) engines[h] = factory(NestLightComposition.CreateScanner(h, NestLightComposition.CreateEmbeddedLanguages()));
            var ranks = new int[probes.Count];
            for (int i = 0; i < probes.Count; i++)
            {
                RankProbe p = probes[i];
                CompletionEngine engine = engines[p.Host];
                CompletionSite site = engine.Locate(p.Text, p.Caret);
                ranks[i] = site == null ? -1 : CompletionLab.RankOf(engine.Suggest(p.Text, site).Select(s => s.Text).ToList(), p.Word);
            }
            return ranks;
        }

        public sealed class Comparison
        {
            public string[] Names;
            public List<RankProbe> Probes;
            public int[][] Ranks;
            public int Cases;

            public double Rate(int variant, Func<RankProbe, bool> where, int top)
            {
                int cases = 0, hits = 0;
                for (int i = 0; i < Probes.Count; i++)
                {
                    if (!Probes[i].Reachable || !where(Probes[i])) continue;
                    cases++;
                    int r = Ranks[variant][i];
                    if (r >= 0 && r < top) hits++;
                }
                return cases == 0 ? 0 : 100.0 * hits / cases;
            }

            public double Mrr(int variant, Func<RankProbe, bool> where)
            {
                double sum = 0; int cases = 0;
                for (int i = 0; i < Probes.Count; i++)
                {
                    if (!Probes[i].Reachable || !where(Probes[i])) continue;
                    cases++;
                    if (Ranks[variant][i] >= 0) sum += 1.0 / (Ranks[variant][i] + 1);
                }
                return cases == 0 ? 0 : sum / cases;
            }

            public int Count(Func<RankProbe, bool> where) { return Probes.Count(p => p.Reachable && where(p)); }
        }

        /// <summary>The engine of the plugin (the second stage included) with the given context features.</summary>
        public static Func<IHostScanner, CompletionEngine> Plugin(CompletionFeatures features)
        {
            return scanner => new CompletionEngine(scanner, matcher: new BandedPrefixMatcher(), features: features);
        }

        public static Comparison Compare(List<RankProbe> probes, string[] names, params Func<IHostScanner, CompletionEngine>[] factories)
        {
            var c = new Comparison { Names = names, Probes = probes, Ranks = factories.Select(f => Ranks(probes, f)).ToArray() };
            c.Cases = c.Count(p => true);
            return c;
        }

        // ---- the tables every ranking experiment reports ----------------------------------------------------------------------

        public static void AddTables(Outcome outcome, Comparison c, string subject, int previousWords = 8, bool byPlace = false)
        {
            var all = new Func<RankProbe, bool>(p => true);
            var overall = new Table(subject + ": " + c.Probes.Count + " typed prefixes, " + c.Cases + " reachable (" + (100.0 * c.Cases / Math.Max(1, c.Probes.Count)).ToString("F1") + "%)",
                "Variant", "Word first", "Within the first 5", "Within the first 10", "Mean reciprocal rank");
            for (int v = 0; v < c.Names.Length; v++)
                overall.Add(c.Names[v], Pct(c.Rate(v, all, 1)), Pct(c.Rate(v, all, 5)), Pct(c.Rate(v, all, 10)), c.Mrr(v, all).ToString("F3"));
            outcome.Tables.Add(overall);

            var byLanguage = new Table("Within the first 5, by language of the string", new[] { "Language", "Cases" }.Concat(c.Names).ToArray());
            foreach (string id in c.Probes.Where(p => p.Reachable).Select(p => p.EmbeddedLanguageId).Distinct().OrderBy(x => x))
            {
                string language = id;
                byLanguage.Add(new object[] { id, c.Count(p => p.EmbeddedLanguageId == language) }.Concat(Enumerable.Range(0, c.Names.Length).Select(v => (object)Pct(c.Rate(v, p => p.EmbeddedLanguageId == language, 5)))).ToArray());
            }
            outcome.Tables.Add(byLanguage);

            var byPrefix = new Table("Within the first 5, by number of letters typed", new[] { "Letters", "Cases" }.Concat(c.Names).ToArray());
            foreach (int k in c.Probes.Select(p => p.Prefix).Distinct().OrderBy(x => x))
            {
                int letters = k;
                byPrefix.Add(new object[] { k, c.Count(p => p.Prefix == letters) }.Concat(Enumerable.Range(0, c.Names.Length).Select(v => (object)Pct(c.Rate(v, p => p.Prefix == letters, 5)))).ToArray());
            }
            outcome.Tables.Add(byPrefix);

            if (byPlace)
            {
                var places = new Table("Within the first 5, by the place of the caret in the grammar", new[] { "Place", "Cases" }.Concat(c.Names).ToArray());
                foreach (string place in c.Probes.Where(p => p.Reachable).GroupBy(p => p.Place).OrderByDescending(g => g.Count()).Select(g => g.Key))
                {
                    string where = place;
                    places.Add(new object[] { "`" + place + "`", c.Count(p => p.Place == where) }.Concat(Enumerable.Range(0, c.Names.Length).Select(v => (object)Pct(c.Rate(v, p => p.Place == where, 5)))).ToArray());
                }
                outcome.Tables.Add(places);
            }

            var byPrevious = new Table("Within the first 5, by the word before the one typed (the most frequent)", new[] { "Before", "Cases" }.Concat(c.Names).ToArray());
            foreach (string previous in c.Probes.Where(p => p.Reachable && p.Previous.Length > 0).GroupBy(p => p.Previous).OrderByDescending(g => g.Count()).Take(previousWords).Select(g => g.Key))
            {
                string before = previous;
                byPrevious.Add(new object[] { "`" + previous + "`", c.Count(p => p.Previous == before) }.Concat(Enumerable.Range(0, c.Names.Length).Select(v => (object)Pct(c.Rate(v, p => p.Previous == before, 5)))).ToArray());
            }
            outcome.Tables.Add(byPrevious);
        }

        // ---- the cost of a session with the features on --------------------------------------------------------------------

        private static string Tail(HostLanguage host, string typed, string after)
        {
            switch (host)
            {
                case HostLanguage.JavaScript: return "sql`" + typed + "|" + after + "`;\n";
                case HostLanguage.CSharp: return "// language=sql\nvar q = \"" + typed + "|" + after + "\";\n";
                case HostLanguage.Python: return "# language=sql\nq = \"" + typed + "|" + after + "\"\n";
                default: return "// language=sql\nauto q = R\"(" + typed + "|" + after + ")\";\n";
            }
        }

        /// <summary>
        /// The median time of the start of a completion session (the classifier has highlighted the text, the scan is shared, the text
        /// is a new instance) on structured files of increasing size, for each variant. Returns the slowest at the largest size.
        /// </summary>
        public static Table Latency(Settings settings, string[] names, Func<IHostScanner, CompletionEngine>[] factories, string typed, out double worst, out string worstCase, string after = "")
        {
            var table = new Table("Median time of the start of one completion session (typed `" + typed + "|" + after + "` in an SQL string, structured files)",
                new[] { "Host", "Lines", "Characters" }.Concat(names).ToArray());
            int largest = settings.Lines.Max();
            worst = 0; worstCase = "";
            foreach (HostLanguage host in SyntheticCode.Hosts)
                foreach (int lines in settings.Lines)
                {
                    string body = SyntheticCorpus.File(host, 77, 0.5, Math.Max(1, lines / 10), true);
                    string tail = Tail(host, typed, after);
                    int caret = body.Length + tail.IndexOf('|');
                    string baseText = body + tail.Remove(tail.IndexOf('|'), 1);
                    int copies = settings.Warmup + settings.Runs;

                    var row = new List<object> { host, lines, baseText.Length };
                    for (int v = 0; v < factories.Length; v++)
                    {
                        IEmbeddedLanguageRegistry languages = NestLightComposition.CreateEmbeddedLanguages();
                        var scanner = new CachingHostScanner(NestLightComposition.CreateScanner(host, languages));
                        var highlighter = new HighlightEngine(scanner, languages);
                        CompletionEngine engine = factories[v](scanner);
                        var times = new double[copies];
                        var sw = new System.Diagnostics.Stopwatch();
                        for (int i = 0; i < copies; i++)
                        {
                            string text = new string(baseText.ToCharArray());
                            highlighter.Highlight(text);
                            sw.Restart();
                            engine.Locate(text, caret);
                            CompletionSite site = engine.Locate(text, caret);
                            engine.Suggest(text, site);
                            sw.Stop();
                            times[i] = sw.Elapsed.TotalMilliseconds;
                        }
                        double median = times.Skip(settings.Warmup).OrderBy(t => t).ElementAt(settings.Runs / 2);
                        row.Add(Measure.Ms(median));
                        if (lines == largest && v == factories.Length - 1 && median > worst) { worst = median; worstCase = host + " at " + lines + " lines"; }
                    }
                    table.Add(row.ToArray());
                }
            return table;
        }

        /// <summary>A difference in points with its sign; a difference that rounds to zero has none.</summary>
        public static string Signed(double value)
        {
            return (Math.Round(value, 1) + 0.0).ToString("+0.0;-0.0;0.0");
        }

        public static string Pct(double value) { return value.ToString("F1") + "%"; }
    }
}
