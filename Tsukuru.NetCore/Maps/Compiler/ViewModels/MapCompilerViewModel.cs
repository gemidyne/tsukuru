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
        private string _mapName;
        private string _vmfPath;

        private VbspCompilationSettingsViewModel _vbspSettings;
        private VvisCompilationSettingsViewModel _vvisSettings;
        private VradCompilationSettingsViewModel _vradSettings;


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

        public MapCompilerViewModel()
        {
            if (IsInDesignMode)
            {
                VProject = "??";
            }
            else
            {
                VProject = VProjectHelper.Path;
            }

            VMFPath = SettingsManager.Manifest.MapCompilerSettings.LastVmfPath;

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
