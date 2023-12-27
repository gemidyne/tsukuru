using System;
using System.Collections.Generic;
using System.IO;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Ookii.Dialogs.Wpf;
using Tsukuru.Maps.Compiler.Business;
using Tsukuru.Maps.Compiler.Messages;
using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class MapSettingsViewModel : ViewModelBaseWithValidation, IApplicationContentView
{
    private bool _isLoading;

    public string Name => "Map information";

    public string Description => "This page allows you to select the VMF and output map filename.";

    public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private string _mapName;
    private string _vmfPath;
    private EMapVersionMode _versioningMode;
    private string _fileNameSuffix;
    private string _fileNamePrefix;
    private int _buildNumber;
    private Dictionary<EMapVersionMode, string> _versioningModes;

    public string MapName
    {
        get => _mapName;
        set
        {
            SetProperty(ref _mapName, value);

            MapCompileSessionInfo.Instance.MapName = MapName;
        }
    }

    public EMapVersionMode VersioningMode
    {
        get => _versioningMode;
        set
        {
            SetProperty(ref _versioningMode, value);

            SettingsManager.Manifest.MapCompilerSettings.MapVersioningSettings.Mode = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }

            SetMapName();
        }
    }

    public Dictionary<EMapVersionMode, string> VersioningModes
    {
        get => _versioningModes;
        set => SetProperty(ref _versioningModes, value);
    }

    public string FileNamePrefix
    {
        get => _fileNamePrefix;
        set
        {
            SetProperty(ref _fileNamePrefix, value);

            SettingsManager.Manifest.MapCompilerSettings.MapVersioningSettings.FileNamePrefix = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }

            SetMapName();
        }
    }

    public string FileNameSuffix
    {
        get => _fileNameSuffix;
        set
        {
            SetProperty(ref _fileNameSuffix, value);

            SettingsManager.Manifest.MapCompilerSettings.MapVersioningSettings.FileNameSuffix = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }

            SetMapName();
        }
    }

    public int BuildNumber
    {
        get => _buildNumber;
        set
        {
            SetProperty(ref _buildNumber, value);

            SettingsManager.Manifest.MapCompilerSettings.MapVersioningSettings.NextBuildNumber = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }

            SetMapName();
        }
    }

    public string VmfPath
    {
        get => _vmfPath;
        set
        {
            SetProperty(ref _vmfPath, value);

            if (string.IsNullOrWhiteSpace(VmfPath) || !File.Exists(VmfPath))
            {
                ClearValidationErrors(nameof(VmfPath));
                AddValidationError(nameof(VmfPath), "The VMF file specified does not exist.");
            }
            else
            {
                SettingsManager.Manifest.MapCompilerSettings.LastVmfPath = VmfPath;

                if (!IsLoading)
                {
                    SettingsManager.Save();
                }

                SetMapName();
            }
        }
    }

    public RelayCommand SelectVmfFileCommand { get; }

    public MapSettingsViewModel()
    {
        SelectVmfFileCommand = new RelayCommand(SelectVmfFile);

        VersioningModes = new Dictionary<EMapVersionMode, string>
        {
            {
                EMapVersionMode.NoVersioning,
                "No versioning"
            },
            {
                EMapVersionMode.VersionedDateTime,
                "Version with year, month & date"
            },
            {
                EMapVersionMode.VersionedBuildNumber,
                "Version with an incrementing build number"
            }
        };

        Messenger.Register<MapCompileEndMessage>(this, OnMapCompileEnd);

        Init();
    }

    public void Init()
    {
        VmfPath = SettingsManager.Manifest.MapCompilerSettings.LastVmfPath;

        if (SettingsManager.Manifest.MapCompilerSettings.MapVersioningSettings == null)
        {
            SettingsManager.Manifest.MapCompilerSettings.MapVersioningSettings = new MapVersioningSettings();
        }

        VersioningMode = SettingsManager.Manifest.MapCompilerSettings.MapVersioningSettings.Mode;
        FileNamePrefix = SettingsManager.Manifest.MapCompilerSettings.MapVersioningSettings.FileNamePrefix;
        FileNameSuffix = SettingsManager.Manifest.MapCompilerSettings.MapVersioningSettings.FileNameSuffix;
        BuildNumber = SettingsManager.Manifest.MapCompilerSettings.MapVersioningSettings.NextBuildNumber;
    }

    private void OnMapCompileEnd(object recipient, MapCompileEndMessage obj)
    {
        IsLoading = true;
        Init();
        IsLoading = false;
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

        VmfPath = dialog.FileName;
    }

    private void SetMapName()
    {
        switch (VersioningMode)
        {
            case EMapVersionMode.VersionedDateTime:
                MapName = $"{FileNamePrefix}{DateTime.Now:yyyyMMdd}{FileNameSuffix}";
                break;

            case EMapVersionMode.VersionedBuildNumber:
                MapName = $"{FileNamePrefix}{BuildNumber}{FileNameSuffix}";
                break;
        }
    }
}