using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using AdonisUI.Controls;
using Tsukuru.Core.SourceEngine;
using Tsukuru.Settings;
using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxButton = AdonisUI.Controls.MessageBoxButton;
using MessageBoxImage = AdonisUI.Controls.MessageBoxImage;
using MessageBoxResult = AdonisUI.Controls.MessageBoxResult;

namespace Tsukuru.Views;

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

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(VProjectHelper.Path))
        {
            MessageBox.Show(
                text:
                "You need to set your VProject in order to use the Map Compiler feature of Tsukuru. \n You can set this in your Windows system environment variables. \n\nIt should be the full path to your game directory, for example:\n\n A:\\SteamLibrary\\steamapps\\common\\Team Fortress 2\\tf\n\nAfter you set this, restart Tsukuru.",
                owner: this,
                caption: "Tsukuru",
                buttons: MessageBoxButton.OK,
                icon: MessageBoxImage.Warning);
        }
    }
}