using System.IO;
using System.Linq;
using Chiaki;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Ookii.Dialogs.Wpf;
using Tsukuru.Maps.Compiler.Messages;
using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class ResourceFolderViewModel : ViewModelBase
{
    private readonly ISettingsManager _settingsManager;
    private bool _suppressSave;
    private string _folder;
    private EResourceFolderPackingMode _packingMode;

    public string Folder
    {
        get => _folder;
        set => SetProperty(ref _folder, value);
    }

    public EResourceFolderPackingMode[] PackingModes { get; }

    public EResourceFolderPackingMode PackingMode
    {
        get => _packingMode;
        set
        {
            SetProperty(ref _packingMode, value);

            var folder = _settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.SingleOrDefault(f => f.Path == Folder);

            if (folder == null)
            {
                return;
            }

            folder.Intelligent = value == EResourceFolderPackingMode.NecessaryAssetsOnly;

            if (!_suppressSave)
            {
                _settingsManager.Save();
            }
        }
    }

    public RelayCommand RemoveFolderCommand { get; }

    public RelayCommand ChangePathCommand { get; }

    public ResourceFolderViewModel(ISettingsManager settingsManager, string folder, bool intelligent)
    {
        _settingsManager = settingsManager;
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
        Messenger.Send(new RemoveResourceFolderFromPackingMessage
        {
            Folder = this
        });
    }
}