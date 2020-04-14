using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using Tsukuru.Core.Translations;

namespace Tsukuru.Translator.ViewModels
{
    public class TranslatorImportViewModel : ViewModelBase
    {
        private string _selectedFile;

        public string SelectedFile
        {
            get => _selectedFile;
            set { Set(() => SelectedFile, ref _selectedFile, value); }
        }

        public RelayCommand BrowseFileCommand { get; }
        public RelayCommand ImportCommand { get; }

        public TranslatorImportViewModel()
        {
            BrowseFileCommand = new RelayCommand(BrowseFile);
            ImportCommand = new RelayCommand(DoImport);
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
                        messageBoxText:
                        "Import completed.",
                        caption: "Success",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Information);
                    break;

                case EProjectGenerateResult.SourceFileNotFound:
                    MessageBox.Show(
                        messageBoxText:
                        "Source file not found.",
                        caption: "Error",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                    break;

                case EProjectGenerateResult.BadRootTranslationFile:
                    MessageBox.Show(
                        messageBoxText:
                        "Invalid KeyValue data. Check the format of your translation files and try again later.",
                        caption: "Error",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                    break;

                default:
                    MessageBox.Show(
                        messageBoxText:
                        "There was a general error when importing. Check the format of your translation files and try again later.",
                        caption: "Error",
                        button: MessageBoxButton.OK,
                        icon: MessageBoxImage.Error);
                    break;
            }
        }
    }
}
