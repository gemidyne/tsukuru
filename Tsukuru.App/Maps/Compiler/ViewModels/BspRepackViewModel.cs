using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class BspRepackViewModel : ViewModelBase, IApplicationContentView
{
    private readonly ISettingsManager _settingsManager;
    private bool _isLoading;
    private bool _performRepack;

    public string Name => "BSP Repack";

    public string Description =>
        "Repacking your map can significantly decrease the file size. This will compress all data within the map. NOTE: You must have Resource Packing enabled to use this feature.";

    public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool PerformRepack
    {
        get => _performRepack;
        set
        {
            SetProperty(ref _performRepack, value);

            _settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.PerformRepackCompress = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public BspRepackViewModel(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public void Init()
    {
        PerformRepack = _settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.PerformRepackCompress;
    }
}