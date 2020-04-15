using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class VvisCompilationSettingsViewModel : BaseCompilationSettings, IApplicationContentView
    {
        private bool _fast;
        private bool _lowPriority;
        private string _otherArguments;
        private bool _isLoading;

        public bool Fast
        {
            get => _fast;
            set
            {
                Set(() => Fast, ref _fast, value);
                OnArgumentChanged();

                SettingsManager.Manifest.MapCompilerSettings.VvisSettings.Fast = value;

                if (!IsLoading)
                {
                    SettingsManager.Save();
                }
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

                if (!IsLoading)
                {
                    SettingsManager.Save();
                }
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

                if (!IsLoading)
                {
                    SettingsManager.Save();
                }
            }
        }

        public string Name => "VVIS Settings";

        public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public void Init()
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
