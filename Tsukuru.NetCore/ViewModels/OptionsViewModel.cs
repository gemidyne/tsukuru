using Tsukuru.Settings;

namespace Tsukuru.ViewModels;

public class OptionsViewModel : ViewModelBase, IApplicationContentView
{
    private readonly ISettingsManager _settingsManager;
    private bool _checkForUpdatesOnStartup;
    private bool _isLoading;

    public bool CheckForUpdatesOnStartup
    {
        get => _checkForUpdatesOnStartup;
        set
        {
            SetProperty(ref _checkForUpdatesOnStartup, value);

            _settingsManager.Manifest.CheckForUpdatesOnStartup = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public string Name => "Settings";

    public string Description => "Configure various settings of Tsukuru.";

    public EShellNavigationPage Group => EShellNavigationPage.Meta;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public OptionsViewModel(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public void Init()
    {
        CheckForUpdatesOnStartup = _settingsManager.Manifest.CheckForUpdatesOnStartup;
    }
}