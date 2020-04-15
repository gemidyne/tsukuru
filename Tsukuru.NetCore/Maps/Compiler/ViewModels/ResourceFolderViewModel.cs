using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class ResourceFolderViewModel : ViewModelBase
    {
        private bool _suppressSave;
        private string _folder;
        private bool _intelligent;

        public string Folder
        {
            get => _folder;
            set
            {
                Set(() => Folder, ref _folder, value);
                RaisePropertyChanged(nameof(File));
            }
        }

        public bool Intelligent
        {
            get => _intelligent;
            set
            {
                Set(() => Intelligent, ref _intelligent, value);
                RaisePropertyChanged(nameof(NotIntelligent));

                var folder = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.SingleOrDefault(f => f.Path == Folder);

                if (folder == null)
                {
                    return;
                }

                folder.Intelligent = value;

                if (!_suppressSave)
                {
                    SettingsManager.Save();
                }
            }
        }

        public string File => GetHeading();

        public bool NotIntelligent
        {
            get => !Intelligent;
            set => Intelligent = !value;
        }

        public RelayCommand RemoveFolderCommand { get; }


        public ResourceFolderViewModel(string folder, bool intelligent)
        {
            RemoveFolderCommand = new RelayCommand(RemoveSelectedFolder);
            _suppressSave = true;

            Folder = folder;
            Intelligent = intelligent;

            _suppressSave = false;
        }

        private string GetHeading()
        {
            var directoryInfo = new DirectoryInfo(Folder);

            return directoryInfo.Name;
        }

        private void RemoveSelectedFolder()
        {
            MessengerInstance.Send(this, "RemoveResourceFolderFromPacking");
        }
    }
}
