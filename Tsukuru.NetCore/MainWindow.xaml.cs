using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AdonisUI.Controls;
using Tsukuru.Settings;

namespace Tsukuru
{
    public partial class MainWindow : AdonisWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = $"Tsukuru - v{UpdateManager.Instance.AppVersion}";

            if (SettingsManager.Manifest.CheckForUpdatesOnStartup)
            {
                Task.Run(RunUpdateCheck);
            }
        }

        private static async Task RunUpdateCheck()
        {
            try
            {
                var latestRelease = await UpdateManager.Instance.Check();

                if (latestRelease == null)
                {
                    return;
                }

                var prompt =
                    MessageBox.Show(
                        text: "An update for Tsukuru is available for download. Do you want to open the update page?",
                        caption: "Update Available",
                        buttons: MessageBoxButton.YesNo,
                        icon: MessageBoxImage.Information);

                if (prompt == MessageBoxResult.Yes)
                {
                    Process.Start(latestRelease.HtmlUrl);
                }
            }
            catch (Exception)
            {
                // Ignore
            }
        }
    }
}
