using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.SourcePawn.ViewModels;

public class PostBuildActionsViewModel : ViewModelBase, IApplicationContentView
{
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

            SettingsManager.Manifest.SourcePawnCompiler.CopySmxOnSuccess = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool ExecutePostBuildScripts
    {
        get => _executePostBuildScripts;
        set
        {
            SetProperty(ref _executePostBuildScripts, value);

            SettingsManager.Manifest.SourcePawnCompiler.ExecutePostBuildScripts = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool IncrementVersion
    {
        get => _incrementVersion;
        set
        {
            SetProperty(ref _incrementVersion, value);

            SettingsManager.Manifest.SourcePawnCompiler.Versioning = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public void Init()
    {
        ExecutePostBuildScripts = SettingsManager.Manifest.SourcePawnCompiler.ExecutePostBuildScripts;
        IncrementVersion = SettingsManager.Manifest.SourcePawnCompiler.Versioning;
        CopySmxToClipboardOnCompile = SettingsManager.Manifest.SourcePawnCompiler.CopySmxOnSuccess;
    }
}