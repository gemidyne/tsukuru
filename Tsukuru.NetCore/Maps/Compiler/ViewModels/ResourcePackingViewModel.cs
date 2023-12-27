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
    private readonly ISettingsManager _settingsManager;
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

            _settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.IsEnabled = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
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

    public ResourcePackingViewModel(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
        
        AddFolderCommand = new RelayCommand(AddFolderToPack);
        FoldersToPack = new AsyncObservableCollection<ResourceFolderViewModel>();

        if (IsInDesignMode)
        {
            FoldersToPack.Add(new ResourceFolderViewModel(_settingsManager, "c:\\test", intelligent: true));
            FoldersToPack.Add(new ResourceFolderViewModel(_settingsManager, "c:\\test2", intelligent: false));
            FoldersToPack.Add(new ResourceFolderViewModel(_settingsManager, "c:\\test3", intelligent: true));
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

            PerformResourcePacking = _settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.IsEnabled;

            foreach (var folder in _settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.IfNullThenEmpty().ToArray())
            {
                if (FoldersToPack.Any(x => x.Folder == folder.Path))
                {
                    continue;
                }
                
                FoldersToPack.Add(new ResourceFolderViewModel(_settingsManager, folder.Path, folder.Intelligent));
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

        lock (_door)
        {
            if (FoldersToPack.All(x => x.Folder != dialog.SelectedPath))
            {
                FoldersToPack.Add(new ResourceFolderViewModel(_settingsManager, dialog.SelectedPath, false));

                _settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.Add(
                    new ResourcePackingFolderSetting
                    {
                        Path = dialog.SelectedPath,
                        Intelligent = false
                    });
                _settingsManager.Save();
            }
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

        var settingsFolder = _settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.SingleOrDefault(f => f.Path == folder.Folder);

        if (settingsFolder == null)
        {
            return;
        }

        _settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.Remove(settingsFolder);
        _settingsManager.Save();
    }


}