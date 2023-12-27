using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class VvisCompilationSettingsViewModel : BaseCompilationSettings, IApplicationContentView
{
    private readonly ISettingsManager _settingsManager;
    private bool _fast;
    private bool _lowPriority;
    private string _otherArguments;
    private bool _isLoading;

    public bool Fast
    {
        get => _fast;
        set
        {
            SetProperty(ref _fast, value);
            OnArgumentChanged();

            _settingsManager.Manifest.MapCompilerSettings.VvisSettings.Fast = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public bool LowPriority
    {
        get => _lowPriority;
        set
        {
            SetProperty(ref _lowPriority, value);
            OnArgumentChanged();

            _settingsManager.Manifest.MapCompilerSettings.VvisSettings.LowPriority = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public string OtherArguments
    {
        get => _otherArguments;
        set
        {
            SetProperty(ref _otherArguments, value);
            OnArgumentChanged();

            _settingsManager.Manifest.MapCompilerSettings.VvisSettings.OtherArguments = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public string Name => "VVIS Settings";

    public string Description =>
        "VVIS is the command-line tool that takes a compiled BSP map and embeds visibility data into it. VVIS tests which visleaves can see each other, and which cannot.";

    public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public VvisCompilationSettingsViewModel(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public void Init()
    {
        Fast = _settingsManager.Manifest.MapCompilerSettings.VvisSettings.Fast;
        LowPriority = _settingsManager.Manifest.MapCompilerSettings.VvisSettings.LowPriority;
        OtherArguments = _settingsManager.Manifest.MapCompilerSettings.VvisSettings.OtherArguments;
    }

    public override string BuildArguments()
    {
        return
            ConditionalArg(() => Fast, "-fast") +
            ConditionalArg(() => LowPriority, "-low") +
            OtherArguments;
    }

}