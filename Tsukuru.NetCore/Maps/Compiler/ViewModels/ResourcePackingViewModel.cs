using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ookii.Dialogs.Wpf;
using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class ResourcePackingViewModel : ViewModelBase, IApplicationContentView
    {
        private ObservableCollection<ResourceFolderViewModel> _foldersToPack;
        private bool _performResourcePacking;
        private bool _isLoading;

        public bool PerformResourcePacking
        {
            get => _performResourcePacking;
            set
            {
                Set(() => PerformResourcePacking, ref _performResourcePacking, value);

                SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.IsEnabled = value;

                if (!IsLoading)
                {
                    SettingsManager.Save();
                }
            }
        }

        public ObservableCollection<ResourceFolderViewModel> FoldersToPack
        {
            get => _foldersToPack;
            set => Set(() => FoldersToPack, ref _foldersToPack, value);
        }

        public RelayCommand AddFolderCommand { get; }

        public string Name => "Resource Packing";

        public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public ResourcePackingViewModel()
        {
            AddFolderCommand = new RelayCommand(AddFolderToPack);
            FoldersToPack = new ObservableCollection<ResourceFolderViewModel>();

            MessengerInstance.Register<ResourceFolderViewModel>(this, "RemoveResourceFolderFromPacking", RemoveFolder);
            PerformResourcePacking = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.IsEnabled;

            foreach (var folder in SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders)
            {
                FoldersToPack.Add(new ResourceFolderViewModel(folder.Path, folder.Intelligent));
            }
        }

        public void Init()
        {
        }

        private void AddFolderToPack()
        {
            var dialog = new VistaFolderBrowserDialog();

            if (!dialog.ShowDialog().GetValueOrDefault())
            {
                return;
            }

            if (FoldersToPack.All(x => x.Folder != dialog.SelectedPath))
            {
                FoldersToPack.Add(new ResourceFolderViewModel(dialog.SelectedPath, false));

                SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.Add(new ResourcePackingFolderSetting
                {
                    Path = dialog.SelectedPath,
                    Intelligent = false
                });
                SettingsManager.Save();
            }
        }

        private void RemoveFolder(ResourceFolderViewModel viewModel)
        {
            if (viewModel == null)
            {
                return;
            }

            var folder = viewModel;

            FoldersToPack.Remove(folder);

            var settingsFolder = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.SingleOrDefault(f => f.Path == folder.Folder);

            if (settingsFolder == null)
            {
                return;
            }

            SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.Remove(settingsFolder);
            SettingsManager.Save();
        }


    }
}
