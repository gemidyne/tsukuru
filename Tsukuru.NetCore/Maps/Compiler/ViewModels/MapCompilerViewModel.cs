using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using Tsukuru.Core.SourceEngine;
using Tsukuru.Settings;
using Tsukuru.Steam;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class MapCompilerViewModel : ViewModelBase
    {
        private readonly ResourcePackingViewModel _resourcePackingViewModel;
        private string _mapName;
        private string _vmfPath;
        private bool _compressMapToBZip2;

        private VbspCompilationSettingsViewModel _vbspSettings;
        private VvisCompilationSettingsViewModel _vvisSettings;
        private VradCompilationSettingsViewModel _vradSettings;
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

        public VbspCompilationSettingsViewModel VBSPSettings => _vbspSettings ?? (_vbspSettings = new VbspCompilationSettingsViewModel());

        public VvisCompilationSettingsViewModel VVISSettings => _vvisSettings ?? (_vvisSettings = new VvisCompilationSettingsViewModel());

        public VradCompilationSettingsViewModel VRADSettings => _vradSettings ?? (_vradSettings = new VradCompilationSettingsViewModel());

        public RelayCommand MapCompileCommand { get; }

        public RelayCommand LaunchMapCommand { get; }

        public RelayCommand SelectVmfFileCommand { get; }

        public string VProject { get; }

        public bool IsVProjectSet => !IsVProjectNotSet;

        public bool IsVProjectNotSet => string.IsNullOrWhiteSpace(VProject);

        public bool IsExecuteButtonEnabled => !string.IsNullOrWhiteSpace(VMFPath);

        public string GameInfo => GameHelper.GetGameInfo();

        public MapCompilerViewModel(ResourcePackingViewModel resourcePackingViewModel)
        {
            _resourcePackingViewModel = resourcePackingViewModel;

            if (IsInDesignMode)
            {
                VProject = "??";
            }
            else
            {
                VProject = VProjectHelper.Path;
            }

            VMFPath = SettingsManager.Manifest.MapCompilerSettings.LastVmfPath;
            CompressMapToBZip2 = SettingsManager.Manifest.MapCompilerSettings.CompressMapToBZip2;
            CopyMapToGameMapsFolder = SettingsManager.Manifest.MapCompilerSettings.CopyMapToGameMapsFolder;
            LaunchMapInGame = SettingsManager.Manifest.MapCompilerSettings.LaunchMapInGame;

            if (string.IsNullOrWhiteSpace(VMFPath))
            {
                MapName = $"TsukuruMap-{DateTime.Now:yyyyMMdd}";
            }

            MapCompileCommand = new RelayCommand(DoMapCompile);
            LaunchMapCommand = new RelayCommand(DoMapLaunch);
            SelectVmfFileCommand = new RelayCommand(SelectVmfFile);
        }

        private async void DoMapCompile()
        {
            await Task.Run(() =>
            {
                MapCompileInitialiser.Execute(this);
            });

            SystemSounds.Asterisk.Play();
        }

        private async void DoMapLaunch()
        {
            SteamHelper.LaunchAppWithMap(MapName);

#warning  TODO Update to use something else
            //await DialogHost.Show(new ProgressSpinner(), async delegate (object sender, DialogOpenedEventArgs args)
            //{
            //    await Task.Delay(5000);
            //    args.Session.Close(false);
            //});
        }

        private void SelectVmfFile()
        {
            var dialog = new VistaOpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false,
                Filter = "Valve Map File|*.vmf",
                InitialDirectory = Directory.GetCurrentDirectory(),
                Title = "Tsukuru - Select a VMF file."
            };

            if (!dialog.ShowDialog().GetValueOrDefault())
            {
                return;
            }

            VMFPath = dialog.FileName;
        }
    }
}
