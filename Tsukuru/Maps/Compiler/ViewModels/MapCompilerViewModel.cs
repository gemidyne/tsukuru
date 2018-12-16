using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using Tsukuru.Maps.Compiler.Views;
using Tsukuru.Settings;
using Tsukuru.Steam;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class MapCompilerViewModel : ViewModelBase
    {
        private string _mapName;
        private string _vmfPath;
        private bool _performResourcePacking;
        private bool _performMapTextFileGeneration;
        private bool _compressMapToBZip2;
        private ObservableCollection<string> _foldersToPack;
        private VbspCompilationSettings _vbspSettings;
        private VvisCompilationSettings _vvisSettings;
        private VradCompilationSettings _vradSettings;
        private bool _copyMapToGameMapsFolder;
	    private bool _launchMapInGame;

	    public string MapName
        {
            get => _mapName;
            set 
            { 
                Set(() => MapName, ref _mapName, value);
            }
        }

        public string VMFPath
        {
            get => _vmfPath;
            set
            {
                Set(() => VMFPath, ref _vmfPath, value);

                SettingsManager.Manifest.MapCompilerSettings.LastVmfPath = VMFPath;
                SettingsManager.Save();

                string fileName = Path.GetFileNameWithoutExtension(VMFPath);

                MapName = string.Format("{0}-{1:yyyyMMdd}", fileName, DateTime.Now);

                RaisePropertyChanged("IsExecuteButtonEnabled");
            }
        }

        public bool PerformResourcePacking
        {
            get => _performResourcePacking;
            set
            {
                Set(() => PerformResourcePacking, ref _performResourcePacking, value);
            }
        }

        public bool PerformMapTextFileGeneration
        {
            get => _performMapTextFileGeneration;
            set
            {
                Set(() => PerformMapTextFileGeneration, ref _performMapTextFileGeneration, value);
            }
        }

        public bool CompressMapToBZip2
        {
            get => _compressMapToBZip2;
            set
            {
                Set(() => CompressMapToBZip2, ref _compressMapToBZip2, value);

                SettingsManager.Manifest.MapCompilerSettings.CompressMapToBZip2 = value;
                SettingsManager.Save();
            }
        }

        public bool CopyMapToGameMapsFolder
        {
            get => _copyMapToGameMapsFolder;
            set
            {
                Set(() => CopyMapToGameMapsFolder, ref _copyMapToGameMapsFolder, value);

                SettingsManager.Manifest.MapCompilerSettings.CopyMapToGameMapsFolder = value;
                SettingsManager.Save();
            }
        }

	    public bool LaunchMapInGame
	    {
		    get => _launchMapInGame;
		    set
		    {
			    Set(() => LaunchMapInGame, ref _launchMapInGame, value);

			    SettingsManager.Manifest.MapCompilerSettings.LaunchMapInGame = value;
			    SettingsManager.Save();
		    }
	    }

	    public ObservableCollection<string> FoldersToPack => _foldersToPack ?? (_foldersToPack = new ObservableCollection<string>());

        public VbspCompilationSettings VBSPSettings => _vbspSettings ?? (_vbspSettings = new VbspCompilationSettings());

        public VvisCompilationSettings VVISSettings => _vvisSettings ?? (_vvisSettings = new VvisCompilationSettings());

        public VradCompilationSettings VRADSettings => _vradSettings ?? (_vradSettings = new VradCompilationSettings());

        public RelayCommand MapCompileCommand { get; private set; }

		public RelayCommand LaunchMapCommand { get; }

        public string VProject { get; }

        public bool IsVProjectSet => !IsVProjectNotSet;

        public bool IsVProjectNotSet => string.IsNullOrWhiteSpace(VProject);

        public bool IsExecuteButtonEnabled => !string.IsNullOrWhiteSpace(VMFPath);

        public MapCompilerViewModel()
        {
	        if (IsInDesignMode)
	        {
		        VProject = "??";
	        }
	        else
	        {
		        VProject = SourceCompilationEngine.VProject;
	        }

            VMFPath = SettingsManager.Manifest.MapCompilerSettings.LastVmfPath;
            CompressMapToBZip2 = SettingsManager.Manifest.MapCompilerSettings.CompressMapToBZip2;
            CopyMapToGameMapsFolder = SettingsManager.Manifest.MapCompilerSettings.CopyMapToGameMapsFolder;
	        LaunchMapInGame = SettingsManager.Manifest.MapCompilerSettings.LaunchMapInGame;

            if (string.IsNullOrWhiteSpace(VMFPath))
            {
                MapName = string.Format("TsukuruMap-{0:yyyyMMdd}", DateTime.Now);
            }

            MapCompileCommand = new RelayCommand(DoMapCompile);
	        LaunchMapCommand = new RelayCommand(DoMapLaunch);
        }

	    private async void DoMapCompile()
        {
            await Task.Run(() =>
            {
                MapCompiler.Execute(this);
            });

            SystemSounds.Asterisk.Play();
        }

	    private async void DoMapLaunch()
	    {
		    SteamHelper.LaunchAppWithMap(MapName);
		    
		    await DialogHost.Show(new ProgressView(), async delegate (object sender, DialogOpenedEventArgs args)
		    {
			    await Task.Delay(5000);
			    args.Session.Close(false);
		    });
	    }
    }
}
