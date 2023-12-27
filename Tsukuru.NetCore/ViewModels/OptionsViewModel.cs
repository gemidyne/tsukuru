using Tsukuru.Settings;

namespace Tsukuru.ViewModels;

public class OptionsViewModel : ViewModelBase, IApplicationContentView
{
    private bool _checkForUpdatesOnStartup;
    private bool _isLoading;

    public bool CheckForUpdatesOnStartup
    {
        get => _checkForUpdatesOnStartup;
        set
        {
            SetProperty(ref _checkForUpdatesOnStartup, value);

            SettingsManager.Manifest.CheckForUpdatesOnStartup = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
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

    public void Init()
    {
        CheckForUpdatesOnStartup = SettingsManager.Manifest.CheckForUpdatesOnStartup;
    }
}