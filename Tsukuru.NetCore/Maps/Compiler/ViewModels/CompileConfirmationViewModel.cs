﻿using System;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Tsukuru.Maps.Compiler.Business;
using Tsukuru.Maps.Compiler.Messages;
using Tsukuru.Settings;
using Tsukuru.Steam;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class CompileConfirmationViewModel : ViewModelBase, IApplicationContentView
{
    private bool _isLoading;
    private string _vbspFormattedArgs;
    private string _vvisFormattedArgs;
    private string _vradFormattedArgs;
    private bool _isPackingEnabled;
    private string _folderPackInfo;
    private string _templatingInfo;
    private string _repackInfo;
    private bool _isButtonEnabled;

    public RelayCommand MapCompileCommand { get; }

    public RelayCommand LaunchMapCommand { get; }

    public string Name => "Run compiler";

    public string Description => "Check below for summary of your compilation settings.";

    public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public string VbspFormattedArgs
    {
        get => _vbspFormattedArgs;
        set => SetProperty(ref _vbspFormattedArgs, value);
    }

    public string VvisFormattedArgs
    {
        get => _vvisFormattedArgs;
        set => SetProperty(ref _vvisFormattedArgs, value);
    }

    public string VradFormattedArgs
    {
        get => _vradFormattedArgs;
        set => SetProperty(ref _vradFormattedArgs, value);
    }

    public bool IsPackingEnabled
    {
        get => _isPackingEnabled;
        set => SetProperty(ref _isPackingEnabled, value);
    }

    public string FolderPackInfo
    {
        get => _folderPackInfo;
        set => SetProperty(ref _folderPackInfo, value);
    }

    public string TemplatingInfo
    {
        get => _templatingInfo;
        set => SetProperty(ref _templatingInfo, value);
    }

    public string RepackInfo
    {
        get => _repackInfo;
        set => SetProperty(ref _repackInfo, value);
    }

    public bool IsButtonEnabled
    {
        get => _isButtonEnabled;
        set => SetProperty(ref _isButtonEnabled, value);
    }

    public CompileConfirmationViewModel()
    {
        MapCompileCommand = new RelayCommand(DoMapCompile);
        LaunchMapCommand = new RelayCommand(DoMapLaunch);

        IsButtonEnabled = true;
    }

    public void Init()
    {
        var vbsp = new VbspCompilationSettingsViewModel();
        var vvis = new VvisCompilationSettingsViewModel();
        var vrad = new VradCompilationSettingsViewModel();

        using (new ApplicationContentViewLoader(vbsp))
            VbspFormattedArgs = vbsp.FormattedArguments;

        using (new ApplicationContentViewLoader(vvis))
            VvisFormattedArgs = vvis.FormattedArguments;

        using (new ApplicationContentViewLoader(vrad))
            VradFormattedArgs = vrad.FormattedArguments;

        TemplatingInfo = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.GenerateMapSpecificFiles
            ? "Templating will be run to generate files after the map is compiled."
            : "Templating will not be run.";

        RepackInfo = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.PerformRepackCompress
            ? "The map will be repacked to compress file size even further."
            : "The map will NOT be repacked.";

        IsPackingEnabled = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.IsEnabled;

        if (IsPackingEnabled)
        {
            FolderPackInfo = "The following folders will be packed into the BSP file:\n" + string.Join("\n", SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.Select(x =>
            {
                string mode = x.Intelligent ? "Only used files" : "All files";
                return $"{x.Path} (Pack mode: {mode})";
            }));

        }
        else
        {
            FolderPackInfo = "N/A";
        }
    }

    private async void DoMapCompile()
    {
        IsButtonEnabled = false;
        Messenger.Send(new MapCompileStartMessage());

        Ioc.Default
            .GetRequiredService<MainWindowViewModel>()
            .NavigateToPage<ResultsViewModel>();

        bool result = await MapCompileInitialiser.ExecuteAsync(this);

        if (result)
        {
            SystemSounds.Asterisk.Play();
        }
        else
        {
            SystemSounds.Exclamation.Play();
        }

        SettingsManager.Manifest.MapCompilerSettings.MapVersioningSettings.NextBuildNumber++;
        SettingsManager.Save();

        IsButtonEnabled = true;
        Messenger.Send(new MapCompileEndMessage());
    }

    private async void DoMapLaunch()
    {
        IsButtonEnabled = false;
        SteamHelper.LaunchAppWithMap(MapCompileSessionInfo.Instance.MapName);

        await Task.Delay(TimeSpan.FromSeconds(10));

        IsButtonEnabled = true;
    }
}