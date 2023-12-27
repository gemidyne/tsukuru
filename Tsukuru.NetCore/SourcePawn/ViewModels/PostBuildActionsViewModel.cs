using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.SourcePawn.ViewModels;

public class PostBuildActionsViewModel : ViewModelBase, IApplicationContentView
{
    private readonly ISettingsManager _settingsManager;
    private bool _isLoading;
    private bool _copySmxToClipboardOnCompile;
    private bool _executePostBuildScripts;
    private bool _incrementVersion;

    public string Name => "Post-build actions";

    public string Description =>
        "This page allows you to toggle actions that will be run once the plugins have been compiled.";

    public EShellNavigationPage Group => EShellNavigationPage.SourcePawnCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool CopySmxToClipboardOnCompile
    {
        get => _copySmxToClipboardOnCompile;
        set
        {
            SetProperty(ref _copySmxToClipboardOnCompile, value);

            _settingsManager.Manifest.SourcePawnCompiler.CopySmxOnSuccess = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public bool ExecutePostBuildScripts
    {
        get => _executePostBuildScripts;
        set
        {
            SetProperty(ref _executePostBuildScripts, value);

            _settingsManager.Manifest.SourcePawnCompiler.ExecutePostBuildScripts = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public bool IncrementVersion
    {
        get => _incrementVersion;
        set
        {
            SetProperty(ref _incrementVersion, value);

            _settingsManager.Manifest.SourcePawnCompiler.Versioning = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public PostBuildActionsViewModel(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public void Init()
    {
        ExecutePostBuildScripts = _settingsManager.Manifest.SourcePawnCompiler.ExecutePostBuildScripts;
        IncrementVersion = _settingsManager.Manifest.SourcePawnCompiler.Versioning;
        CopySmxToClipboardOnCompile = _settingsManager.Manifest.SourcePawnCompiler.CopySmxOnSuccess;
    }
}