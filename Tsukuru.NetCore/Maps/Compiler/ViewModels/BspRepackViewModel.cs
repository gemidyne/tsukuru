using GalaSoft.MvvmLight;
using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class BspRepackViewModel : ViewModelBase, IApplicationContentView
    {
        private bool _isLoading;
        private bool _performRepack;

        public string Name => "Repack";

        public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public bool PerformRepack
        {
            get => _performRepack;
            set
            {
                Set(() => PerformRepack, ref _performRepack, value);

                SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.PerformRepackCompress = value;

                if (!IsLoading)
                {
                    SettingsManager.Save();
                }
            }
        }

        public void Init()
        {
            PerformRepack = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.PerformRepackCompress;
        }
    }
}
