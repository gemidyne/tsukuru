using System;
using System.IO;
using AdonisUI.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using Ookii.Dialogs.Wpf;
using Tsukuru.Maps.Compiler.Messages;
using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class ExportSettingsViewModel : ViewModelBaseWithValidation, IApplicationContentView
{
    private readonly ISettingsManager _settingsManager;
    private bool _isLoading;

    public string Name => "Export settings";

    public string Description => "This page allows you to export Map Compiler settings to a file.";

    public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private string _settingsFilePath;
    private bool _isButtonEnabled;

    public string SettingsFilePath
    {
        get => _settingsFilePath;
        set => SetProperty(ref _settingsFilePath, value);
    }

    public bool IsButtonEnabled
    {
        get => _isButtonEnabled;
        set => SetProperty(ref _isButtonEnabled, value);
    }

    public RelayCommand SelectFileCommand { get; }

    public RelayCommand ExportCommand { get; }

    public ExportSettingsViewModel(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        
        SelectFileCommand = new RelayCommand(SelectFile);
        ExportCommand = new RelayCommand(DoExport);

        Messenger.Register<MapCompileStartMessage>(this, OnMapCompileStart);
        Messenger.Register<MapCompileEndMessage>(this, OnMapCompileEnd);

        IsButtonEnabled = true;
    }

    public void Init()
    {
    }

    private void OnMapCompileStart(object recipient, MapCompileStartMessage message)
    {
        IsButtonEnabled = false;
    }

    private void OnMapCompileEnd(object recipient, MapCompileEndMessage message)
    {
        IsButtonEnabled = true;
    }

    private void SelectFile()
    {
        var dialog = new VistaSaveFileDialog
        {
            CheckFileExists = true,
            CheckPathExists = true,
            Filter = "Tsukuru Map Compiler settings|*.tsumc",
            InitialDirectory = Directory.GetCurrentDirectory(),
            Title = "Tsukuru - Export Map Compiler settings",
            DefaultExt = ".tsumc",
            AddExtension = true
        };

        if (!dialog.ShowDialog().GetValueOrDefault())
        {
            return;
        }

        SettingsFilePath = dialog.FileName;
    }

    private void DoExport()
    {
        var file = new FileInfo(SettingsFilePath);

        var result = DoExport(file);

        if (result)
        {
            MessageBox.Show(
                text: "Settings exported successfully.",
                caption: "Success",
                buttons: MessageBoxButton.OK,
                icon: MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(
                text: "Failed to export settings from file. This may be due to the file being in use by another process or you do not have permission to save to the selected folder or file.",
                caption: "Export error",
                buttons: MessageBoxButton.OK,
                icon: MessageBoxImage.Error);
        }
    }
    
    private bool DoExport(FileInfo file)
    {
        try
        {
            var json = JsonConvert.SerializeObject(_settingsManager.Manifest.MapCompilerSettings, Formatting.Indented);

            File.WriteAllText(file.FullName, json);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}