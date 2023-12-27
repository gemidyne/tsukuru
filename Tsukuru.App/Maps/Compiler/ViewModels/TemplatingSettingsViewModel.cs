using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class TemplatingSettingsViewModel : ViewModelBase, IApplicationContentView
{
    private readonly ISettingsManager _settingsManager;
    private bool _isLoading;
    private bool _runTemplating;

    public string Name => "File Templating";

    public string Description => "File Templating allows you to dynamically generate files from templates as part of the compile process. NOTE: You must have Resource Packing enabled to use this feature.";

    public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool RunTemplating
    {
        get => _runTemplating;
        set
        {
            SetProperty(ref _runTemplating, value);

            _settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.GenerateMapSpecificFiles = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public TemplatingSettingsViewModel(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public void Init()
    {
        RunTemplating = _settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.GenerateMapSpecificFiles;
    }
}