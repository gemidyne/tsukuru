using System.Threading.Tasks;
using AdonisUI.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using Tsukuru.Core.Translations;
using Tsukuru.ViewModels;

namespace Tsukuru.Translator.ViewModels
{
    public class TranslatorImportViewModel : ViewModelBase, IApplicationContentView
    {
        private string _selectedFile;
        private bool _isLoading;

        public string SelectedFile
        {
            get => _selectedFile;
            set { Set(() => SelectedFile, ref _selectedFile, value); }
        }

        public string Name => "SourceMod Translation Importer";

        public string Description => "Import translations from SourceMod into a Tsukuru translation project.";

        public EShellNavigationPage Group => EShellNavigationPage.Translations;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public RelayCommand BrowseFileCommand { get; }
        public RelayCommand ImportCommand { get; }

        public TranslatorImportViewModel()
        {
            BrowseFileCommand = new RelayCommand(BrowseFile);
            ImportCommand = new RelayCommand(DoImport);
        }

        public void Init()
        {
        }

        private void BrowseFile()
        {
            var dialog = new VistaOpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Filter = "SourceMod translation file|*.phrases.txt",
                InitialDirectory = System.IO.Directory.GetCurrentDirectory(),
                Title = "Choose a SourceMod translation file."
            };

            if (!dialog.ShowDialog().GetValueOrDefault())
            {
                return;
            }

            SelectedFile = dialog.FileName;
        }

        private async void DoImport()
        {
            EProjectGenerateResult result = EProjectGenerateResult.GeneralFailure;

            await Task.Run(() =>
            {
                var engine = new TranslatorEngine();

                result = engine.ImportFromSourceMod(SelectedFile);
            });

            switch (result)
            {
                case EProjectGenerateResult.CompleteNoErrors:
                    MessageBox.Show(
                        text:
                        "Import completed.",
                        caption: "Success",
                        buttons: MessageBoxButton.OK,
                        icon: MessageBoxImage.Information);
                    break;

                case EProjectGenerateResult.SourceFileNotFound:
                    MessageBox.Show(
                        text:
                        "Source file not found.",
                        caption: "Error",
                        buttons: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                    break;

                case EProjectGenerateResult.BadRootTranslationFile:
                    MessageBox.Show(
                        text:
                        "Invalid KeyValue data. Check the format of your translation files and try again later.",
                        caption: "Error",
                        buttons: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                    break;

                default:
                    MessageBox.Show(
                        text:
                        "There was a general error when importing. Check the format of your translation files and try again later.",
                        caption: "Error",
                        buttons: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                    break;
            }
        }
    }
}
