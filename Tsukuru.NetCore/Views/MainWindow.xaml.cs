using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using AdonisUI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Tsukuru.Core.SourceEngine;
using Tsukuru.Services;
using Tsukuru.Settings;
using Tsukuru.ViewModels;
using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxButton = AdonisUI.Controls.MessageBoxButton;
using MessageBoxImage = AdonisUI.Controls.MessageBoxImage;
using MessageBoxResult = AdonisUI.Controls.MessageBoxResult;

namespace Tsukuru.Views;

public partial class MainWindow : AdonisWindow
{
    private readonly IAppUpdateProvider _updateProvider;

    public MainWindow()
    {
        InitializeComponent();

        DataContext = App.Services.GetRequiredService<MainWindowViewModel>();

        _updateProvider = App.Services.GetRequiredService<IAppUpdateProvider>();
        
        Title = $"Tsukuru - v{_updateProvider.AppVersion}";

        var settingsManager = App.Services.GetRequiredService<ISettingsManager>();
        
        if (settingsManager.Manifest.CheckForUpdatesOnStartup)
        {
            Task.Run(RunUpdateCheck);
        }
    }

    private async Task RunUpdateCheck()
    {
        try
        {
            var latestRelease = await _updateProvider.CheckAsync();

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