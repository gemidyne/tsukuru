﻿using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class PostCompileActionsViewModel : ViewModelBase, IApplicationContentView
{
    private bool _isLoading;
    private bool _copyMapToGameMapsFolder;
    private bool _launchMapInGame;
    private bool _compressMapToBZip2;

    public string Name => "Post compile actions";

    public string Description =>
        "These actions can be toggled to run once the map has been compiled (and packed if enabled).";

    public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public bool CompressMapToBZip2
    {
        get => _compressMapToBZip2;
        set
        {
            SetProperty(ref _compressMapToBZip2, value);

            SettingsManager.Manifest.MapCompilerSettings.CompressMapToBZip2 = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool CopyMapToGameMapsFolder
    {
        get => _copyMapToGameMapsFolder;
        set
        {
            SetProperty(ref _copyMapToGameMapsFolder, value);

            SettingsManager.Manifest.MapCompilerSettings.CopyMapToGameMapsFolder = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool LaunchMapInGame
    {
        get => _launchMapInGame;
        set
        {
            SetProperty(ref _launchMapInGame, value);

            SettingsManager.Manifest.MapCompilerSettings.LaunchMapInGame = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public void Init()
    {
        CompressMapToBZip2 = SettingsManager.Manifest.MapCompilerSettings.CompressMapToBZip2;
        CopyMapToGameMapsFolder = SettingsManager.Manifest.MapCompilerSettings.CopyMapToGameMapsFolder;
        LaunchMapInGame = SettingsManager.Manifest.MapCompilerSettings.LaunchMapInGame;
    }
}