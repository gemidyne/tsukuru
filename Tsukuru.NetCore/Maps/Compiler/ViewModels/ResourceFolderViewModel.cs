using System.IO;
using System.Linq;
using Chiaki;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class ResourceFolderViewModel : ViewModelBase
    {
        private bool _suppressSave;
        private string _folder;
        private EResourceFolderPackingMode _packingMode;

        public string Folder
        {
            get => _folder;
            set => Set(() => Folder, ref _folder, value);
        }

        public EResourceFolderPackingMode[] PackingModes { get; }

        public EResourceFolderPackingMode PackingMode
        {
            get => _packingMode;
            set
            {
                Set(() => PackingMode, ref _packingMode, value);

                var folder = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.SingleOrDefault(f => f.Path == Folder);

                if (folder == null)
                {
                    return;
                }

                folder.Intelligent = value == EResourceFolderPackingMode.NecessaryAssetsOnly;

                if (!_suppressSave)
                {
                    SettingsManager.Save();
                }
            }
        }

        public RelayCommand RemoveFolderCommand { get; }

        public RelayCommand ChangePathCommand { get; }

        public ResourceFolderViewModel(string folder, bool intelligent)
        {
            _suppressSave = true;

            RemoveFolderCommand = new RelayCommand(RemoveSelectedFolder);
            ChangePathCommand = new RelayCommand(ChangePath);

            Folder = folder;
            PackingModes = Enum<EResourceFolderPackingMode>.GetValues().ToArray();
            PackingMode = intelligent
                ? EResourceFolderPackingMode.NecessaryAssetsOnly
                : EResourceFolderPackingMode.Everything;

            _suppressSave = false;
        }

        private void ChangePath()
        {
            var dialog = new VistaFolderBrowserDialog
            {
                ShowNewFolderButton = false,
                Description = "Select a folder to pack.",
                SelectedPath = Folder,
                UseDescriptionForTitle = true
            };

            if (!dialog.ShowDialog().GetValueOrDefault())
            {
                return;
            }

            var directoryInfo = new DirectoryInfo(Folder);

            if (!directoryInfo.Exists)
            {
                return;
            }

            Folder = dialog.SelectedPath;
        }

        private void RemoveSelectedFolder()
        {
            MessengerInstance.Send(this, "RemoveResourceFolderFromPacking");
        }
    }
}
