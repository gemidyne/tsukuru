﻿using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class VradCompilationSettingsViewModel : BaseCompilationSettings, IApplicationContentView
{
    private bool _ldr;
    private bool _hdr;
    private bool _fast;
    private bool _final;
    private bool _staticPropLighting;
    private bool _staticPropPolys;
    private bool _textureShadows;
    private bool _lowPriority;
    private string _otherArguments;
    private bool _useModifiedVrad;
    private bool _largeDispSampleRadius;
    private bool _isLoading;

    public bool LDR
    {
        get => _ldr;
        set
        {
            SetProperty(ref _ldr, value);
            OnArgumentChanged();

            SettingsManager.Manifest.MapCompilerSettings.VradSettings.Ldr = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool HDR
    {
        get => _hdr;
        set
        {
            SetProperty(ref _hdr, value);
            OnArgumentChanged();

            SettingsManager.Manifest.MapCompilerSettings.VradSettings.Hdr = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool Fast
    {
        get => _fast;
        set
        {
            SetProperty(ref _fast, value);
            OnArgumentChanged();

            SettingsManager.Manifest.MapCompilerSettings.VradSettings.Fast = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool Final
    {
        get => _final;
        set
        {
            SetProperty(ref _final, value);
            OnArgumentChanged();

            SettingsManager.Manifest.MapCompilerSettings.VradSettings.Final = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool StaticPropLighting
    {
        get => _staticPropLighting;
        set
        {
            SetProperty(ref _staticPropLighting, value);
            OnArgumentChanged();

            SettingsManager.Manifest.MapCompilerSettings.VradSettings.StaticPropLighting = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool StaticPropPolys
    {
        get => _staticPropPolys;
        set
        {
            SetProperty(ref _staticPropPolys, value);
            OnArgumentChanged();

            SettingsManager.Manifest.MapCompilerSettings.VradSettings.StaticPropPolys = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool TextureShadows
    {
        get => _textureShadows;
        set
        {
            SetProperty(ref _textureShadows, value);
            OnArgumentChanged();

            SettingsManager.Manifest.MapCompilerSettings.VradSettings.TextureShadows = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool LowPriority
    {
        get => _lowPriority;
        set
        {
            SetProperty(ref _lowPriority, value);
            OnArgumentChanged();

            SettingsManager.Manifest.MapCompilerSettings.VradSettings.LowPriority = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool UseModifiedVrad
    {
        get => _useModifiedVrad;
        set
        {
            SetProperty(ref _useModifiedVrad, value);

            SettingsManager.Manifest.MapCompilerSettings.VradSettings.UseModifiedVrad = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public string OtherArguments
    {
        get => _otherArguments;
        set
        {
            SetProperty(ref _otherArguments, value);
            OnArgumentChanged();

            SettingsManager.Manifest.MapCompilerSettings.VradSettings.OtherArguments = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public bool LargeDispSampleRadius
    {
        get => _largeDispSampleRadius;
        set
        {
            SetProperty(ref _largeDispSampleRadius, value);
            OnArgumentChanged();

            SettingsManager.Manifest.MapCompilerSettings.VradSettings.LargeDispSampleRadius = value;

            if (!IsLoading)
            {
                SettingsManager.Save();
            }
        }
    }

    public string Name => "VRAD Settings";

    public string Description =>
        "VRAD is the command-line tool that takes a compiled BSP map and embeds lighting data into it. VRAD's static and pre-compiled light is bounced around the world with a radiosity algorithm.";

    public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public void Init()
    {
        LDR = SettingsManager.Manifest.MapCompilerSettings.VradSettings.Ldr;
        HDR = SettingsManager.Manifest.MapCompilerSettings.VradSettings.Hdr;
        Fast = SettingsManager.Manifest.MapCompilerSettings.VradSettings.Fast;
        Final = SettingsManager.Manifest.MapCompilerSettings.VradSettings.Final;
        StaticPropLighting = SettingsManager.Manifest.MapCompilerSettings.VradSettings.StaticPropLighting;
        StaticPropPolys = SettingsManager.Manifest.MapCompilerSettings.VradSettings.StaticPropPolys;
        TextureShadows = SettingsManager.Manifest.MapCompilerSettings.VradSettings.TextureShadows;
        LowPriority = SettingsManager.Manifest.MapCompilerSettings.VradSettings.LowPriority;
        UseModifiedVrad = SettingsManager.Manifest.MapCompilerSettings.VradSettings.UseModifiedVrad;
        LargeDispSampleRadius = SettingsManager.Manifest.MapCompilerSettings.VradSettings.LargeDispSampleRadius;
        OtherArguments = SettingsManager.Manifest.MapCompilerSettings.VradSettings.OtherArguments;
    }

    public override string BuildArguments()
    {
        return
            ConditionalArg(() => HDR && !LDR, "-hdr") +
            ConditionalArg(() => HDR && LDR, "-both") +
            ConditionalArg(() => !HDR && LDR, "-ldr") +
            ConditionalArg(() => Fast && !Final, "-fast") +
            ConditionalArg(() => !Fast && Final, "-final") +
            ConditionalArg(() => StaticPropLighting, "-StaticPropLighting") +
            ConditionalArg(() => StaticPropPolys, "-StaticPropPolys") +
            ConditionalArg(() => TextureShadows, "-TextureShadows") +
            ConditionalArg(() => LowPriority, "-low") +
            ConditionalArg(() => LargeDispSampleRadius, "-LargeDispSampleRadius") +
            OtherArguments;
    }
}