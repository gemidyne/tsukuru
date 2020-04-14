using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler
{
    public class VvisCompilationSettings : BaseCompilationSettings
    {
        private bool _fast;
        private bool _lowPriority;
        private string _otherArguments;

        public bool Fast
        {
            get => _fast;
            set
            {
                Set(() => Fast, ref _fast, value);
                OnArgumentChanged();

                SettingsManager.Manifest.MapCompilerSettings.VvisSettings.Fast = value;
                SettingsManager.Save();
            }
        }

        public bool LowPriority
        {
            get => _lowPriority;
            set
            {
                Set(() => LowPriority, ref _lowPriority, value);
                OnArgumentChanged();

                SettingsManager.Manifest.MapCompilerSettings.VvisSettings.LowPriority = value;
                SettingsManager.Save();
            }
        }

        public string OtherArguments
        {
            get => _otherArguments;
            set
            {
                Set(() => OtherArguments, ref _otherArguments, value);
                OnArgumentChanged();

                SettingsManager.Manifest.MapCompilerSettings.VvisSettings.OtherArguments = value;
                SettingsManager.Save();
            }
        }

        public VvisCompilationSettings()
        {
            Fast = SettingsManager.Manifest.MapCompilerSettings.VvisSettings.Fast;
            LowPriority = SettingsManager.Manifest.MapCompilerSettings.VvisSettings.LowPriority;
            OtherArguments = SettingsManager.Manifest.MapCompilerSettings.VvisSettings.OtherArguments;
        }

        public override string BuildArguments()
        {
            return
                ConditionalArg(() => Fast, "-fast") +
                ConditionalArg(() => LowPriority, "-low") +
                OtherArguments;
        }
    }
}
