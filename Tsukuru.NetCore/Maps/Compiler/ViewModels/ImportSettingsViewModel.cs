using System.IO;
using AdonisUI.Controls;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using Tsukuru.Maps.Compiler.Messages;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class ImportSettingsViewModel : ViewModelBaseWithValidation, IApplicationContentView
    {
        private bool _isLoading;

        public string Name => "Import settings";

        public string Description => "This page allows you to select and import Map Compiler settings from a file.";

        public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        private string _settingsFilePath;
        private bool _isButtonEnabled;

        public string SettingsFilePath
        {
            get => _settingsFilePath;
            set
            {
                Set(() => SettingsFilePath, ref _settingsFilePath, value);

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
            set => Set(() => IsButtonEnabled, ref _isButtonEnabled, value);
        }

        public RelayCommand SelectFileCommand { get; }

        public RelayCommand ImportCommand { get; }

        public ImportSettingsViewModel()
        {
            SelectFileCommand = new RelayCommand(SelectFile);
            ImportCommand = new RelayCommand(DoImport);

            MessengerInstance.Register<MapCompileStartMessage>(this, OnMapCompileStart);
            MessengerInstance.Register<MapCompileEndMessage>(this, OnMapCompileEnd);

            IsButtonEnabled = true;
        }

        public void Init()
        {
        }

        private void OnMapCompileStart(MapCompileStartMessage message)
        {
            IsButtonEnabled = false;
        }

        private void OnMapCompileEnd(MapCompileEndMessage message)
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

            var result = SettingsImporter.Import(file);

            if (result)
            {
                MessageBox.Show(
                    text: "Settings imported successfully.",
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
    }
}