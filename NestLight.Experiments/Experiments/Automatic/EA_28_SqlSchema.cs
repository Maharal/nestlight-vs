using System.Linq;
using NestLight.Completion;

namespace NestLight.Experiments
{
    internal sealed class EA_28_SqlSchema : Experiment
    {
        public override string Id { get { return "EA_28"; } }
        public override string Title { get { return "Does the schema read from the SQL of the document help?"; } }
        public override string Hypothesis { get { return "The tables and columns that the SQL of the file talks about (a CREATE TABLE, the FROM and JOIN of the statement, the aliases) tell which table to offer after FROM and which columns belong after `u.` or in the select list, and that is more precise than the words that happened to follow the same word elsewhere: an alias means a different table in every statement. Putting those first puts the meant word in the first 5 more often, in the places where a table or a column is typed, and the whole file can be read in a frame."; } }
        public override string Method { get { return "EA_25's probes (50 generated files with structure, words typed with 1 to 3 letters, the list the editor gets), reported by the place of the caret. Four variants: the order by distance alone; the previous word, the language and the grammar (what the plugin ran with before this experiment); the same plus the schema; the schema alone. The files hold a CREATE TABLE for each table, queries on the columns of their table, joins on the foreign keys with the aliases `t` and `o`, UPDATEs and INSERTs. Also the start of a session on files of 1,200 to 60,000 lines, where every SQL string is read for the schema."; } }
        public override string Criterion { get { return "Adding the schema is at least 1 point better within the first 5 over all the reachable cases; the places that need a table or a column (`sql:table`, `sql:member`, `sql:expression`) do not fall; and the session stays under 16 ms at 60,000 lines in every host."; } }
        public override string IfMet { get { return "Keep the schema on."; } }
        public override string IfNotMet { get { return "If it is the time: read only the strings near the caret, or keep the schema of a text between sessions. If it is the gain: the previous word already did the work and the reader is not worth its cost."; } }

        public override Outcome Run(Settings settings)
        {
            var outcome = new Outcome();
            var corpus = SyntheticCorpus.Generate(settings.Quick ? 8 : 50, 4321, 0.5, 40, true);
            var probes = ContextLab.Probes(corpus, settings.Quick ? 20 : 60, 3);
            ContextLab.MarkReachable(probes);

            var names = new[] { "By distance alone", "Before the schema", "With the schema", "Schema alone" };
            var features = new[]
            {
                CompletionFeatures.None,
                new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true),
                new CompletionFeatures(previousWord: true, sameLanguageWords: true, grammar: true, schema: true),
                new CompletionFeatures(schema: true),
            };
            var factories = features.Select(ContextLab.Plugin).ToArray();
            ContextLab.Comparison c = ContextLab.Compare(probes, names, factories);
            ContextLab.AddTables(outcome, c, "The word typed with 1 to 3 letters", 6, true);

            // not part of the criterion: files so short that the previous word has little history to learn from
            var small = SyntheticCorpus.Generate(settings.Quick ? 16 : 200, 987, 0.5, 4, true);
            var smallProbes = ContextLab.Probes(small, settings.Quick ? 20 : 30, 3);
            ContextLab.MarkReachable(smallProbes);
            ContextLab.Comparison cs = ContextLab.Compare(smallProbes, new[] { "Before the schema", "With the schema" }, factories[1], factories[2]);
            var shortFiles = new Table("Short files (4 functions each, " + small.Count + " files, " + cs.Cases + " reachable prefixes): within the first 5, by the place of the caret", "Place", "Cases", "Before the schema", "With the schema");
            foreach (string place in new[] { "sql:table", "sql:member", "sql:expression" })
            {
                string where = place;
                shortFiles.Add("`" + place + "`", cs.Count(p => p.Place == where), ContextLab.Pct(cs.Rate(0, p => p.Place == where, 5)), ContextLab.Pct(cs.Rate(1, p => p.Place == where, 5)));
            }
            shortFiles.Add("all", cs.Cases, ContextLab.Pct(cs.Rate(0, p => true, 5)), ContextLab.Pct(cs.Rate(1, p => true, 5)));
            outcome.Tables.Add(shortFiles);

            double worst; string worstCase;
            outcome.Tables.Add(ContextLab.Latency(settings, names, factories, "select u.comp", out worst, out worstCase, " from users u"));

            double gain = c.Rate(2, p => true, 5) - c.Rate(1, p => true, 5);
            var places = new[] { "sql:table", "sql:member", "sql:expression" };
            double worstPlace = places.Select(place => c.Rate(2, p => p.Place == place, 5) - c.Rate(1, p => p.Place == place, 5)).Min();
            outcome.CriterionMet = gain >= 1 && worstPlace >= 0 && worst < 16;
            outcome.Headline = "within the first 5: " + ContextLab.Pct(c.Rate(1, p => true, 5)) + " to " + ContextLab.Pct(c.Rate(2, p => true, 5)) + " (" + ContextLab.Signed(gain) + " points); worst session " + Measure.Ms(worst);
            outcome.Analysis.Add("The schema alone moves the share within the first 5 from " + ContextLab.Pct(c.Rate(0, p => true, 5)) + " to " + ContextLab.Pct(c.Rate(3, p => true, 5)) + "; added to the rest, " + ContextLab.Signed(gain) + " points; the worst of the table, member and expression places changes by " + ContextLab.Signed(worstPlace) + " points.");
            outcome.Analysis.Add("The slowest session with the schema at the largest size is " + worstCase + ": " + Measure.Ms(worst) + " (the frame budget is 16 ms). Every SQL string in the window of 500,000 characters around the caret is read at each request.");
            outcome.Analysis.Add("The corpus is generated with a schema in mind: every column belongs to one table, a CREATE TABLE exists for each, and the aliases are reused with a different table in every statement. Real code with no CREATE TABLE in the same file has only what its queries reveal.");
            return outcome;
        }
    }
}
