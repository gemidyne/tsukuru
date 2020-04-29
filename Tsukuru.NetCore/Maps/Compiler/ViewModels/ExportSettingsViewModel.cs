using System.IO;
using AdonisUI.Controls;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using Tsukuru.Maps.Compiler.Messages;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class ExportSettingsViewModel : ViewModelBaseWithValidation, IApplicationContentView
    {
        private bool _isLoading;

        public string Name => "Export settings";

        public string Description => "This page allows you to export Map Compiler settings to a file.";

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
            set => Set(() => SettingsFilePath, ref _settingsFilePath, value);
        }

        public bool IsButtonEnabled
        {
            get => _isButtonEnabled;
            set => Set(() => IsButtonEnabled, ref _isButtonEnabled, value);
        }

        public RelayCommand SelectFileCommand { get; }

        public RelayCommand ExportCommand { get; }

        public ExportSettingsViewModel()
        {
            SelectFileCommand = new RelayCommand(SelectFile);
            ExportCommand = new RelayCommand(DoExport);

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

            var result = SettingsExporter.Export(file);

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
    }
}