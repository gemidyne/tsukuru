using GalaSoft.MvvmLight;
using Tsukuru.Settings;

namespace Tsukuru.ViewModels
{
    public class OptionsViewModel : ViewModelBase, IApplicationContentView
    {
        private bool _checkForUpdatesOnStartup;
        private bool _isLoading;

        public bool CheckForUpdatesOnStartup
        {
            get => _checkForUpdatesOnStartup;
            set
            {
                Set(() => CheckForUpdatesOnStartup, ref _checkForUpdatesOnStartup, value);

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
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public void Init()
        {
            CheckForUpdatesOnStartup = SettingsManager.Manifest.CheckForUpdatesOnStartup;
        }
    }
}
