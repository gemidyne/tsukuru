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

public class ImportSettingsViewModel : ViewModelBaseWithValidation, IApplicationContentView
{
    private readonly ISettingsManager _settingsManager;
    private bool _isLoading;

    public string Name => "Import settings";

    public string Description => "This page allows you to select and import Map Compiler settings from a file.";

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
        set
        {
            SetProperty(ref _settingsFilePath, value);

            if (string.IsNullOrWhiteSpace(SettingsFilePath) || !File.Exists(SettingsFilePath))
            {
                ClearValidationErrors(nameof(SettingsFilePath));
                AddValidationError(nameof(SettingsFilePath), "The specified file does not exist.");
            }
        }
    }

    public bool IsButtonEnabled
    {
        get => _isButtonEnabled;
        set => SetProperty(ref _isButtonEnabled, value);
    }

    public RelayCommand SelectFileCommand { get; }

    public RelayCommand ImportCommand { get; }

    public ImportSettingsViewModel(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        
        SelectFileCommand = new RelayCommand(SelectFile);
        ImportCommand = new RelayCommand(DoImport);

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
        var dialog = new VistaOpenFileDialog
        {
            CheckFileExists = true,
            CheckPathExists = true,
            Multiselect = false,
            Filter = "Tsukuru Map Compiler settings|*.tsumc",
            InitialDirectory = Directory.GetCurrentDirectory(),
            Title = "Tsukuru - Select a Map Compiler settings file to import."
        };

        if (!dialog.ShowDialog().GetValueOrDefault())
        {
            return;
        }

        SettingsFilePath = dialog.FileName;
    }

    private void DoImport()
    {
        var file = new FileInfo(SettingsFilePath);

        if (!file.Exists)
        {
            MessageBox.Show(
                text: "The settings file you specified does not exist.",
                caption: "Import error",
                buttons: MessageBoxButton.OK,
                icon: MessageBoxImage.Error);
            return;
        }

        var prompt = MessageBox.Show(
            text: "Importing this settings file will cause your current Map Compiler settings to be overwritten. Are you sure you want to continue importing?",
            caption: "Import Confirmation",
            buttons: MessageBoxButton.YesNo,
            icon: MessageBoxImage.Warning);

        if (prompt != MessageBoxResult.Yes)
        {
            MessageBox.Show(
                text: "Import cancelled.",
                caption: "Information",
                buttons: MessageBoxButton.OK,
                icon: MessageBoxImage.Information);
            return;
        }

        var result = DoImport(file);

        if (result)
        {
            MessageBox.Show(
                text: "Settings imported successfully. You may want to review file paths in Resource Packing (BSPZIP) to ensure they are valid folder paths.",
                caption: "Success",
                buttons: MessageBoxButton.OK,
                icon: MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(
                text: "Failed to import settings from file. This may be due to the file being corrupted or not in a valid format.",
                caption: "Import error",
                buttons: MessageBoxButton.OK,
                icon: MessageBoxImage.Error);
        }
    }
    
    private bool DoImport(FileInfo file)
    {
        if (!file.Exists)
        {
            return false;
        }

        using (var stream = file.OpenText())
        {
            try
            {
                var json = stream.ReadToEnd();

                var settings = JsonConvert.DeserializeObject<MapCompilerSettings>(json);

                _settingsManager.Manifest.MapCompilerSettings = settings;
                _settingsManager.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}