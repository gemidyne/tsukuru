using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Media;
using System.Threading.Tasks;
using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public partial class MapCompilerViewModel : ViewModelBase
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
            }
        }

        public ObservableCollection<string> FoldersToPack => _foldersToPack ?? (_foldersToPack = new ObservableCollection<string>());

        public VbspCompilationSettings VBSPSettings => _vbspSettings ?? (_vbspSettings = new VbspCompilationSettings());

        public VvisCompilationSettings VVISSettings => _vvisSettings ?? (_vvisSettings = new VvisCompilationSettings());

        public VradCompilationSettings VRADSettings => _vradSettings ?? (_vradSettings = new VradCompilationSettings());

        public RelayCommand MapCompileCommand { get; private set; }

        public string VProject => SourceCompilationEngine.VProject;

        public bool IsVProjectSet => !string.IsNullOrWhiteSpace(VProject);

        public bool IsVProjectNotSet => string.IsNullOrWhiteSpace(VProject);

        public bool IsExecuteButtonEnabled => !string.IsNullOrWhiteSpace(VMFPath);

        public MapCompilerViewModel()
        {
            VMFPath = SettingsManager.Manifest.MapCompilerSettings.LastVmfPath;

            if (string.IsNullOrWhiteSpace(VMFPath))
            {
                MapName = string.Format("TsukuruMap-{0:yyyyMMdd}", DateTime.Now);
            }

            MapCompileCommand = new RelayCommand(DoMapCompile);
        }

        private async void DoMapCompile()
        {
            var executionWin = new ExecutionWindow();

            executionWin.Show();

            await Task.Run(() =>
            {
                MapCompiler.Execute(this);
            });

            SystemSounds.Asterisk.Play();
        }
    }
}
