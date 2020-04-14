using System.Threading.Tasks;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using Tsukuru.Core.Translations;

namespace Tsukuru.Translator.ViewModels
{
    public class TranslatorExportViewModel : ViewModelBase
    {
        private string _selectedFile;

        public string SelectedFile
        {
            get => _selectedFile;
            set { Set(() => SelectedFile, ref _selectedFile, value); }
        }

        public RelayCommand BrowseFileCommand { get; }
        public RelayCommand ExportCommand { get; }

        public TranslatorExportViewModel()
        {
            BrowseFileCommand = new RelayCommand(BrowseFile);
            ExportCommand = new RelayCommand(DoExport);
        }

        private void BrowseFile()
        {
            var dialog = new VistaOpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Filter = "Tsukuru Translator project|*.tsutproj",
                InitialDirectory = System.IO.Directory.GetCurrentDirectory(),
                Title = "Choose a Tsukuru Translator project file."
            };

            if (!dialog.ShowDialog().GetValueOrDefault(false))
            {
                return;
            }

            SelectedFile = dialog.FileName;
        }

        private async void DoExport()
        {
            await Task.Run(() =>
            {
                var engine = new TranslatorEngine();

                engine.ExportToSourceMod(SelectedFile);
            });

            MessageBox.Show(
                messageBoxText:
                "Export completed.",
                caption: "Success",
                button: MessageBoxButton.OK,
                icon: MessageBoxImage.Information);
        }
    }
}
