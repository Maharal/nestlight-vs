using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NestLight.Common;

namespace NestLight.Detection
{
    /// <summary>
    /// Whether the plugin guesses the language of strings nobody marked, and for which languages. Off until the user turns it on. It is
    /// read by the scanners on every scan and changed from the options page, so it is a snapshot swapped as a whole: a scan sees either
    /// the old options or the new ones, never a mix, and <see cref="Version"/> tells whoever caches a result that it is stale.
    /// </summary>
    internal sealed class DetectionOptions
    {
        private sealed class State
        {
            public bool Enabled;
            public HashSet<string> Languages;
            public int Version;
        }

        private volatile State _state = new State { Enabled = false, Languages = new HashSet<string>(LanguageDetector.Languages), Version = 0 };

        public event EventHandler Changed;

        /// <summary>The languages the user can choose from: those that have a detector.</summary>
        public static IReadOnlyList<string> Available { get { return LanguageDetector.Languages; } }

        public bool Enabled { get { return _state.Enabled; } }

        /// <summary>Grows with every change; a result computed under another version is stale.</summary>
        public int Version { get { return _state.Version; } }

        /// <summary>The languages chosen, whether or not the detection as a whole is on.</summary>
        public IReadOnlyCollection<string> Languages { get { return _state.Languages; } }

        /// <summary>The detection is on and this language is one of those chosen.</summary>
        public bool IsOn(string languageId)
        {
            State state = _state;
            return state.Enabled && languageId != null && state.Languages.Contains(languageId);
        }

        /// <param name="languages">Ids that are not <see cref="Available"/> are ignored.</param>
        public void Set(bool enabled, IEnumerable<string> languages)
        {
            var chosen = new HashSet<string>((languages ?? new string[0]).Where(id => id != null && Available.Contains(id, StringComparer.OrdinalIgnoreCase)).Select(id => id.ToLowerInvariant()));
            State old = _state;
            if (old.Enabled == enabled && old.Languages.SetEquals(chosen)) return;
            _state = new State { Enabled = enabled, Languages = chosen, Version = old.Version + 1 };
            EventHandler handler = Changed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }

    /// <summary>Keeps <see cref="DetectionOptions"/> in a small text file (<c>enabled=true</c>, then one <c>language=id</c> line for each language chosen).</summary>
    internal static class DetectionOptionsFile
    {
        public static string DefaultPath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NestLight", "detection.txt"); }
        }

        /// <summary>The options saved in the file. A file that is missing, cannot be read or is not an options file gives the defaults: off, every language chosen.</summary>
        public static DetectionOptions Load(string path)
        {
            var options = new DetectionOptions();
            try
            {
                if (!File.Exists(path)) return options;
                bool enabled = false, recognized = false;
                var languages = new List<string>();
                foreach (string raw in File.ReadAllLines(path))
                {
                    int eq = raw.IndexOf('=');
                    if (eq <= 0) continue;
                    string key = raw.Substring(0, eq).Trim().ToLowerInvariant(), value = raw.Substring(eq + 1).Trim();
                    if (key == "enabled") { enabled = string.Equals(value, "true", StringComparison.OrdinalIgnoreCase); recognized = true; }
                    else if (key == "language") languages.Add(value);
                }
                if (recognized) options.Set(enabled, languages); // a file without the key is not ours: the defaults stand
            }
            catch (Exception)
            {
                // a damaged or locked file never stops the editor: the defaults stand
            }
            return options;
        }

        /// <returns>False when the file could not be written.</returns>
        public static bool Save(string path, DetectionOptions options)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                var lines = new List<string> { "enabled=" + (options.Enabled ? "true" : "false") };
                lines.AddRange(options.Languages.OrderBy(id => id, StringComparer.Ordinal).Select(id => "language=" + id));
                File.WriteAllLines(path, lines);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>The guess the scanners ask for, under the user's options.</summary>
    internal sealed class OptionsLanguageGuesser : ILanguageGuesser
    {
        private readonly DetectionOptions _options;
        private readonly LanguageDetector _detector;

        public OptionsLanguageGuesser(DetectionOptions options, LanguageDetector detector)
        {
            if (options == null) throw new ArgumentNullException("options");
            if (detector == null) throw new ArgumentNullException("detector");
            _options = options;
            _detector = detector;
        }

        public string Guess(string text, int start, int end)
        {
            if (!_options.Enabled) return null; // the common case costs one read
            string id = _detector.Detect(text, start, end);
            return _options.IsOn(id) ? id : null;
        }
    }
}
