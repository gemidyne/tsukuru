using GalaSoft.MvvmLight;
using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class TemplatingSettingsViewModel : ViewModelBase, IApplicationContentView
    {
        private bool _isLoading;
        private bool _runTemplating;

        public string Name => "File Templating";

        public string Description => "File Templating allows you to dynamically generate files from templates as part of the compile process. NOTE: You must have Resource Packing enabled to use this feature.";

        public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public bool RunTemplating
        {
            get => _runTemplating;
            set
            {
                Set(() => RunTemplating, ref _runTemplating, value);

                SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.GenerateMapSpecificFiles = value;

                if (!IsLoading)
                {
                    SettingsManager.Save();
                }
            }
        }

        public void Init()
        {
            RunTemplating = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.GenerateMapSpecificFiles;
        }
    }
}