using CommunityToolkit.Mvvm.Input;
using Ookii.Dialogs.Wpf;
using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.SourcePawn.ViewModels;

public class SettingsViewModel : ViewModelBase, IApplicationContentView
{
    private string _sourcePawnCompiler;
    private bool _isLoading;

    public string SourcePawnCompiler
    {
        get => _sourcePawnCompiler;
        set
        {
            SetProperty(ref _sourcePawnCompiler, value);

            SettingsManager.Manifest.SourcePawnCompiler.CompilerPath = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public RelayCommand BrowseCompilerCommand { get; private set; }

    public string Name => "Compiler Settings";

    public string Description => "This page allows you to set settings for the SourcePawn compiler.";

    public EShellNavigationPage Group => EShellNavigationPage.SourcePawnCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public SettingsViewModel()
    {
        BrowseCompilerCommand = new RelayCommand(BrowseCompiler);
    }

    public void Init()
    {
        SourcePawnCompiler = SettingsManager.Manifest.SourcePawnCompiler.CompilerPath;
    }

    private void BrowseCompiler()
    {
        var dialog = new VistaOpenFileDialog
        {
            CheckFileExists = true,
            CheckPathExists = true,
            Multiselect = false,
            Filter = "SourcePawn Compiler|*.exe",
            InitialDirectory = System.IO.Directory.GetCurrentDirectory(),
            Title = "Choose a SourcePawn Compiler."
        };

        if (!dialog.ShowDialog().GetValueOrDefault())
        {
            return;
        }

        SourcePawnCompiler = dialog.FileName;
    }
}