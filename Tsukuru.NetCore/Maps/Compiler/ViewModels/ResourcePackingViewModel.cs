using System.Collections.ObjectModel;
using System.Linq;
using Chiaki;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Ookii.Dialogs.Wpf;
using Tsukuru.Maps.Compiler.Messages;
using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class ResourcePackingViewModel : ViewModelBase, IApplicationContentView
{
    private static readonly object _door = new();

    private ObservableCollection<ResourceFolderViewModel> _foldersToPack;
    private bool _performResourcePacking;
    private bool _isLoading;

    public bool PerformResourcePacking
    {
        get => _performResourcePacking;
        set
        {
            SetProperty(ref _performResourcePacking, value);

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
        set => SetProperty(ref _foldersToPack, value);
    }

    public RelayCommand AddFolderCommand { get; }

    public string Name => "Resource Packing (BSPZIP)";

    public string Description => "Pack custom textures, models and sounds into your BSP file. Uses BSPZIP.exe.";

    public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ResourcePackingViewModel()
    {
        AddFolderCommand = new RelayCommand(AddFolderToPack);
        FoldersToPack = new AsyncObservableCollection<ResourceFolderViewModel>();

        if (IsInDesignMode)
        {
            FoldersToPack.Add(new ResourceFolderViewModel("c:\\test", intelligent: true));
            FoldersToPack.Add(new ResourceFolderViewModel("c:\\test2", intelligent: false));
            FoldersToPack.Add(new ResourceFolderViewModel("c:\\test3", intelligent: true));
        }
        else
        {
            Messenger.Register<RemoveResourceFolderFromPackingMessage>(this, (_, message) => RemoveFolder(message));
        }
    }

    public void Init()
    {
        lock (_door)
        {
            FoldersToPack.Clear();

            PerformResourcePacking = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.IsEnabled;

            foreach (var folder in SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.IfNullThenEmpty().ToArray())
            {
                FoldersToPack.Add(new ResourceFolderViewModel(folder.Path, folder.Intelligent));
            }
        }
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

    private void RemoveFolder(RemoveResourceFolderFromPackingMessage message)
    {
        if (message == null)
        {
            return;
        }

        var folder = message.Folder;

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