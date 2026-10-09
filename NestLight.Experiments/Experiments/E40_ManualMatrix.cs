using System.Collections.Generic;

namespace NestLight.Experiments
{
    internal sealed class E40_ManualMatrix : ManualExperiment
    {
        public override string Id { get { return "E40"; } }
        public override string Title { get { return "Every host with every embedded language, in Visual Studio"; } }
        public override string Hypothesis { get { return "E39 shows that the pipeline finds and tokenizes every combination. What it cannot show is what the person sees: that the colors are the ones of the theme, that completion opens inside the string and not outside, and that typing in the string does not leave the colors behind."; } }
        public override string Method { get { return "The generator writes one file for each applicable combination (3 copies of the sample, the same files E39 checked) under hosts/. The person opens each in a Visual Studio with the extension installed and follows the steps."; } }
        public override string Criterion { get { return "In every file: the colors are there and match the language, Ctrl+Space inside the string offers the keywords of the language, and none of the typing steps leaves the colors wrong or freezes the editor."; } }
        public override string IfMet { get { return "What E39 measured is what the user sees."; } }
        public override string IfNotMet { get { return "The files that failed show a gap between the pipeline and the Visual Studio side (classification names, themes, the completion source), which no automatic experiment here can reach."; } }

        public override IList<string> Steps
        {
            get
            {
                return new[]
                {
                    "Install the extension built from the commit under test and open a solution with the folders of the kit (javascript, csharp, python, cpp).",
                    "Open a file. Every sample string must be colored as its language: no string with the color of a plain string, and none of the host code colored as the embedded language.",
                    "In an *-interpolated file, the interpolation must keep the colors of the host and must not be colored as the embedded language.",
                    "Inside a sample string, type the first letters of a keyword of its language and press Ctrl+Space: the keywords of that language must be offered. Outside any string nothing of NestLight must be offered.",
                    "Type a new line of code in the string, then delete a closing quote or backtick and put it back: the colors must follow within a moment and never stay wrong after the edit.",
                    "Switch between the Light and Dark themes: the colors must stay readable.",
                    "Note every file that fails, with what was seen, in the result."
                };
            }
        }

        protected override int Prepare(Settings settings, string dir)
        {
            return CombinationGenerator.WriteAll(dir, 3).Count;
        }
    }
}
