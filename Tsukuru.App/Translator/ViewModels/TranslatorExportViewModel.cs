using System.Threading.Tasks;
using AdonisUI.Controls;
using CommunityToolkit.Mvvm.Input;
using Ookii.Dialogs.Wpf;
using Tsukuru.Core.Translations;
using Tsukuru.ViewModels;

namespace Tsukuru.Translator.ViewModels;

public class TranslatorExportViewModel : ViewModelBase, IApplicationContentView
{
    private readonly ITranslatorEngine _translatorEngine;
    private string _selectedFile;
    private bool _isLoading;

    public string SelectedFile
    {
        get => _selectedFile;
        set => SetProperty(ref _selectedFile, value);
    }

    public RelayCommand BrowseFileCommand { get; }
    public RelayCommand ExportCommand { get; }

    public string Name => "Translator Export";

    public string Description => "Export Tsukuru translator projects to SourceMod translation files.";

    public EShellNavigationPage Group => EShellNavigationPage.Translations;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public TranslatorExportViewModel(ITranslatorEngine translatorEngine)
    {
        _translatorEngine = translatorEngine;
        BrowseFileCommand = new RelayCommand(BrowseFile);
        ExportCommand = new RelayCommand(DoExport);
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
            _translatorEngine.ExportToSourceMod(SelectedFile);
        });

        MessageBox.Show(
            text: "Export completed.",
            caption: "Success",
            buttons: MessageBoxButton.OK,
            icon: MessageBoxImage.Information);
    }
}