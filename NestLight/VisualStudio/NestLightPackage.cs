using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Shell;
using NestLight.Detection;

namespace NestLight.VisualStudio
{
    /// <summary>
    /// Only here to register the options page under Tools > Options. The colors and the completion are MEF components and do not wait for it.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(PackageGuidString)]
    [ProvideOptionPage(typeof(DetectionOptionsPage), "NestLight", "Automatic detection", 0, 0, true)]
    public sealed class NestLightPackage : AsyncPackage
    {
        public const string PackageGuidString = "6f1d3c52-8b7a-4e0d-9a41-2c5e7b9d0f13";
    }

    /// <summary>
    /// Tools > Options > NestLight > Automatic detection: a switch for the whole feature and one box for each language the plugin can
    /// detect. The languages come from the detectors, so a new detector appears here without touching this page.
    /// </summary>
    [ComVisible(true)]
    [Guid("a3b86f10-5c2e-4d79-b1e4-7d09c8a52e36")]
    public sealed class DetectionOptionsPage : UIElementDialogPage
    {
        private DetectionOptionsPanel _panel;

        protected override UIElement Child
        {
            get { return _panel ?? (_panel = new DetectionOptionsPanel()); }
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);
            ((DetectionOptionsPanel)Child).Show(NestLightOptions.Detection);
        }

        protected override void OnApply(PageApplyEventArgs e)
        {
            if (e.ApplyBehavior == ApplyKind.Apply)
            {
                var panel = (DetectionOptionsPanel)Child;
                NestLightOptions.ApplyDetection(panel.Enabled, panel.Languages);
            }
            base.OnApply(e);
        }
    }

    internal sealed class DetectionOptionsPanel : StackPanel
    {
        private static readonly Dictionary<string, string> Names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "sql", "SQL" }, { "html", "HTML" }, { "css", "CSS" }, { "json", "JSON" }, { "graphql", "GraphQL" }
        };

        private readonly CheckBox _enabled;
        private readonly StackPanel _languages;
        private readonly Dictionary<string, CheckBox> _boxes = new Dictionary<string, CheckBox>();

        public DetectionOptionsPanel()
        {
            Margin = new Thickness(8);
            Children.Add(new TextBlock
            {
                Text = "NestLight colors a string as another language when you mark it (html`...`, // language=sql). "
                     + "With this on it also guesses the language of strings you did not mark, from what they contain. "
                     + "It is a guess: a string that only looks like code is colored as code. A mark always wins over the guess.",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 12)
            });

            _enabled = new CheckBox { Content = "Guess the language of strings that are not marked", Margin = new Thickness(0, 0, 0, 8) };
            _enabled.Checked += (s, e) => UpdateLanguagesEnabled();
            _enabled.Unchecked += (s, e) => UpdateLanguagesEnabled();
            Children.Add(_enabled);

            _languages = new StackPanel { Margin = new Thickness(20, 0, 0, 0) };
            _languages.Children.Add(new TextBlock { Text = "Guess these languages:", Margin = new Thickness(0, 0, 0, 4) });
            foreach (string id in DetectionOptions.Available)
            {
                string name;
                var box = new CheckBox { Content = Names.TryGetValue(id, out name) ? name : id.ToUpperInvariant(), Margin = new Thickness(0, 2, 0, 2) };
                _boxes[id] = box;
                _languages.Children.Add(box);
            }
            Children.Add(_languages);
        }

        public bool Enabled { get { return _enabled.IsChecked == true; } }

        public IEnumerable<string> Languages
        {
            get { return _boxes.Where(p => p.Value.IsChecked == true).Select(p => p.Key).ToList(); }
        }

        public void Show(DetectionOptions options)
        {
            _enabled.IsChecked = options.Enabled;
            foreach (KeyValuePair<string, CheckBox> p in _boxes) p.Value.IsChecked = options.Languages.Contains(p.Key);
            UpdateLanguagesEnabled();
        }

        private void UpdateLanguagesEnabled()
        {
            _languages.IsEnabled = _enabled.IsChecked == true;
        }
    }
}
