using GalaSoft.MvvmLight;
using Tsukuru.Settings;

namespace Tsukuru.ViewModels
{
    public class OptionsViewModel : ViewModelBase
    {
        private bool _checkForUpdatesOnStartup;

        public bool CheckForUpdatesOnStartup
        {
            get => _checkForUpdatesOnStartup;
            set
            {
                Set(() => CheckForUpdatesOnStartup, ref _checkForUpdatesOnStartup, value);

                SettingsManager.Manifest.CheckForUpdatesOnStartup = value;
                SettingsManager.Save();
            }
        }

        public OptionsViewModel()
        {
            CheckForUpdatesOnStartup = SettingsManager.Manifest.CheckForUpdatesOnStartup;
        }
    }
}
