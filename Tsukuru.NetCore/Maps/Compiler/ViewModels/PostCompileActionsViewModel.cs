using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class PostCompileActionsViewModel : ViewModelBase, IApplicationContentView
{
    private readonly ISettingsManager _settingsManager;
    private bool _isLoading;
    private bool _copyMapToGameMapsFolder;
    private bool _launchMapInGame;
    private bool _compressMapToBZip2;

    public string Name => "Post compile actions";

    public string Description =>
        "These actions can be toggled to run once the map has been compiled (and packed if enabled).";

    public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool CompressMapToBZip2
    {
        get => _compressMapToBZip2;
        set
        {
            SetProperty(ref _compressMapToBZip2, value);

            _settingsManager.Manifest.MapCompilerSettings.CompressMapToBZip2 = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public bool CopyMapToGameMapsFolder
    {
        get => _copyMapToGameMapsFolder;
        set
        {
            SetProperty(ref _copyMapToGameMapsFolder, value);

            _settingsManager.Manifest.MapCompilerSettings.CopyMapToGameMapsFolder = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public bool LaunchMapInGame
    {
        get => _launchMapInGame;
        set
        {
            SetProperty(ref _launchMapInGame, value);

            _settingsManager.Manifest.MapCompilerSettings.LaunchMapInGame = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public PostCompileActionsViewModel(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public void Init()
    {
        CompressMapToBZip2 = _settingsManager.Manifest.MapCompilerSettings.CompressMapToBZip2;
        CopyMapToGameMapsFolder = _settingsManager.Manifest.MapCompilerSettings.CopyMapToGameMapsFolder;
        LaunchMapInGame = _settingsManager.Manifest.MapCompilerSettings.LaunchMapInGame;
    }
}