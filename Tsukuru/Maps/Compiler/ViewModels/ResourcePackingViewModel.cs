using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler.ViewModels
{
	public class ResourcePackingViewModel : ViewModelBase
    {
	    private ObservableCollection<ResourceFolderViewModel> _foldersToPack;
	    private bool _performResourcePacking;
	    private bool _generateMapSpecificFiles;
	    private bool _performRepack;
	    private ResourceFolderViewModel _selectedFolder;

	    public bool PerformResourcePacking
	    {
		    get => _performResourcePacking;
		    set
		    {
			    Set(() => PerformResourcePacking, ref _performResourcePacking, value);

			    SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.IsEnabled = value;
			    SettingsManager.Save();
		    }
	    }

	    public bool GenerateMapSpecificFiles
	    {
		    get => _generateMapSpecificFiles;
		    set
		    {
			    Set(() => GenerateMapSpecificFiles, ref _generateMapSpecificFiles, value);

			    SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.GenerateMapSpecificFiles = value;
			    SettingsManager.Save();
		    }
	    }

	    public bool PerformRepack
	    {
		    get => _performRepack;
		    set
		    {
			    Set(() => PerformRepack, ref _performRepack, value);

			    SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.PerformRepackCompress = value;
			    SettingsManager.Save();
		    }
	    }

	    public ObservableCollection<ResourceFolderViewModel> FoldersToPack => _foldersToPack ?? (_foldersToPack = new ObservableCollection<ResourceFolderViewModel>());

		public RelayCommand AddFolderCommand { get; }

		public RelayCommand RemoveFolderCommand { get; }

		public ResourceFolderViewModel SelectedFolder
		{
			get => _selectedFolder;
			set => Set(() => SelectedFolder, ref _selectedFolder, value);
		}

		public ResourcePackingViewModel()
		{
			PerformResourcePacking = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.IsEnabled;
			GenerateMapSpecificFiles = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.GenerateMapSpecificFiles;
			PerformRepack = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.PerformRepackCompress;

			foreach (var folder in SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders)
			{
				FoldersToPack.Add(new ResourceFolderViewModel
				{
					Folder = folder.Path,
					Intelligent = folder.Intelligent
				});
			}

			AddFolderCommand = new RelayCommand(AddFolderToPack);
			RemoveFolderCommand = new RelayCommand(RemoveSelectedFolder);
		}

	    private void AddFolderToPack()
	    {
		    var dialog = new Ookii.Dialogs.VistaFolderBrowserDialog();

		    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
		    {
			    return;
		    }

		    if (FoldersToPack.All(x => x.Folder != dialog.SelectedPath))
		    {
			    FoldersToPack.Add(new ResourceFolderViewModel
			    {
					Folder = dialog.SelectedPath,
					Intelligent = false
			    });

				SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.Add(new ResourcePackingFolderSetting
				{
					Path = dialog.SelectedPath,
					Intelligent = false
				});
				SettingsManager.Save();
		    }
	    }

	    private void RemoveSelectedFolder()
	    {
		    if (SelectedFolder == null)
		    {
			    return;
		    }

		    var folder = SelectedFolder;

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
