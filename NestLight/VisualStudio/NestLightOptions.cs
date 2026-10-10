using System;
using NestLight.Detection;

namespace NestLight.VisualStudio
{
    /// <summary>
    /// The options of the plugin, shared by every buffer and by the options page. They live in a small file, read the first time they
    /// are needed, so the editor does not wait for the package to load to know whether the detection is on.
    /// </summary>
    internal static class NestLightOptions
    {
        private static readonly Lazy<DetectionOptions> Current = new Lazy<DetectionOptions>(() => DetectionOptionsFile.Load(DetectionOptionsFile.DefaultPath));

        /// <summary>Whether strings nobody marked get the language guessed from their content, and for which languages. Off until the user turns it on.</summary>
        public static DetectionOptions Detection { get { return Current.Value; } }

        /// <summary>Changes the options, saves them and tells every buffer that its colors are stale.</summary>
        /// <returns>False when the file could not be saved: the options hold until Visual Studio closes.</returns>
        public static bool ApplyDetection(bool enabled, System.Collections.Generic.IEnumerable<string> languages)
        {
            Detection.Set(enabled, languages);
            return DetectionOptionsFile.Save(DetectionOptionsFile.DefaultPath, Detection);
        }
    }
}
