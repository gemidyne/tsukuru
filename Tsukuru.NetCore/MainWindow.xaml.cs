using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Tsukuru.Settings;
using Tsukuru.SourcePawn.ViewModels;

namespace Tsukuru
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new SourcePawnCompileViewModel();

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
                        messageBoxText: "An update for Tsukuru is available for download. Do you want to open the update page?",
                        caption: "Update Available",
                        button: MessageBoxButton.YesNo,
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
