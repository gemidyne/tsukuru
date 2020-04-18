using System.Linq;
using System.Media;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Tsukuru.Maps.Compiler.Business;
using Tsukuru.Steam;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class CompileConfirmationViewModel : ViewModelBase, IApplicationContentView
    {
        private bool _isLoading;
        private string _vbspFormattedArgs;
        private string _vvisFormattedArgs;
        private string _vradFormattedArgs;
        private bool _isPackingEnabled;
        private string _folderPackInfo;
        private string _templatingInfo;
        private string _repackInfo;

        public RelayCommand MapCompileCommand { get; }

        public RelayCommand LaunchMapCommand { get; }

        public string Name => "Run compiler";

        public string Description => "Check below for summary of your compilation settings.";

        public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public string VbspFormattedArgs
        {
            get => _vbspFormattedArgs;
            set => Set(() => VbspFormattedArgs, ref _vbspFormattedArgs, value);
        }

        public string VvisFormattedArgs
        {
            get => _vvisFormattedArgs;
            set => Set(() => VvisFormattedArgs, ref _vvisFormattedArgs, value);
        }

        public string VradFormattedArgs
        {
            get => _vradFormattedArgs;
            set => Set(() => VradFormattedArgs, ref _vradFormattedArgs, value);
        }

        public bool IsPackingEnabled
        {
            get => _isPackingEnabled;
            set => Set(() => IsPackingEnabled, ref _isPackingEnabled, value);
        }

        public string FolderPackInfo
        {
            get => _folderPackInfo;
            set => Set(() => FolderPackInfo, ref _folderPackInfo, value);
        }

        public string TemplatingInfo
        {
            get => _templatingInfo;
            set => Set(() => TemplatingInfo, ref _templatingInfo, value);
        }

        public string RepackInfo
        {
            get => _repackInfo;
            set => Set(() => RepackInfo, ref _repackInfo, value);
        }

        public CompileConfirmationViewModel()
        {
            MapCompileCommand = new RelayCommand(DoMapCompile);
            LaunchMapCommand = new RelayCommand(DoMapLaunch);
        }

        public void Init()
        {
            var vbsp = new VbspCompilationSettingsViewModel();
            var vvis = new VvisCompilationSettingsViewModel();
            var vrad = new VradCompilationSettingsViewModel();
            var packing = new ResourcePackingViewModel();
            var templating = new TemplatingSettingsViewModel();
            var repack = new BspRepackViewModel();

            using (new ApplicationContentViewLoader(vbsp))
                VbspFormattedArgs = vbsp.FormattedArguments;

            using (new ApplicationContentViewLoader(vvis))
                VvisFormattedArgs = vvis.FormattedArguments;

            using (new ApplicationContentViewLoader(vrad))
                VradFormattedArgs = vrad.FormattedArguments;

            using (new ApplicationContentViewLoader(templating))
                TemplatingInfo = templating.RunTemplating
                    ? "Templating will be run to generate files after the map is compiled."
                    : "Templating will not be run.";

            using (new ApplicationContentViewLoader(repack))
                RepackInfo = repack.PerformRepack
                    ? "The map will be repacked to compress file size even further."
                    : "The map will NOT be repacked.";

            IsPackingEnabled = packing.PerformResourcePacking;

            if (IsPackingEnabled)
            {
                FolderPackInfo = "The following folders will be packed into the BSP file:\n" + string.Join("\n", packing.FoldersToPack.Select(x =>
                {
                    string mode = x.Intelligent ? "Only used files" : "All files";
                    return $"{x.Folder} (Pack mode: {mode})";
                }));

            }
            else
            {
                FolderPackInfo = "N/A";
            }
        }

        private async void DoMapCompile()
        {
            await Task.Run(() =>
            {
                MapCompileInitialiser.Execute(this);
            });

            SystemSounds.Asterisk.Play();
        }

        private void DoMapLaunch()
        {
            SteamHelper.LaunchAppWithMap(MapCompileSessionInfo.Instance.MapName);

#warning  TODO Update to use something else
            //await DialogHost.Show(new ProgressSpinner(), async delegate (object sender, DialogOpenedEventArgs args)
            //{
            //    await Task.Delay(5000);
            //    args.Session.Close(false);
            //});
        }



    }
}
