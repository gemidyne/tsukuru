using GalaSoft.MvvmLight;
using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.SourcePawn.ViewModels
{
    public class PostBuildActionsViewModel : ViewModelBase, IApplicationContentView
    {
        private bool _isLoading;
        private bool _copySmxToClipboardOnCompile;
        private bool _executePostBuildScripts;
        private bool _incrementVersion;

        public string Name => "Post-build actions";

        public EShellNavigationPage Group => EShellNavigationPage.SourcePawnCompiler;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public bool CopySmxToClipboardOnCompile
        {
            get => _copySmxToClipboardOnCompile;
            set
            {
                Set(() => CopySmxToClipboardOnCompile, ref _copySmxToClipboardOnCompile, value);

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
                Set(() => ExecutePostBuildScripts, ref _executePostBuildScripts, value);

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
                Set(() => IncrementVersion, ref _incrementVersion, value);

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
}