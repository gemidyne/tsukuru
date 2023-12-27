using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class VradCompilationSettingsViewModel : BaseCompilationSettings, IApplicationContentView
{
    private readonly ISettingsManager _settingsManager;
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

            _settingsManager.Manifest.MapCompilerSettings.VradSettings.Ldr = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
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

            _settingsManager.Manifest.MapCompilerSettings.VradSettings.Hdr = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
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

            _settingsManager.Manifest.MapCompilerSettings.VradSettings.Fast = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
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

            _settingsManager.Manifest.MapCompilerSettings.VradSettings.Final = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
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

            _settingsManager.Manifest.MapCompilerSettings.VradSettings.StaticPropLighting = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
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

            _settingsManager.Manifest.MapCompilerSettings.VradSettings.StaticPropPolys = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
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

            _settingsManager.Manifest.MapCompilerSettings.VradSettings.TextureShadows = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
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

            _settingsManager.Manifest.MapCompilerSettings.VradSettings.LowPriority = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public bool UseModifiedVrad
    {
        get => _useModifiedVrad;
        set
        {
            SetProperty(ref _useModifiedVrad, value);

            _settingsManager.Manifest.MapCompilerSettings.VradSettings.UseModifiedVrad = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
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

            _settingsManager.Manifest.MapCompilerSettings.VradSettings.OtherArguments = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
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

            _settingsManager.Manifest.MapCompilerSettings.VradSettings.LargeDispSampleRadius = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
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

    public VradCompilationSettingsViewModel(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public void Init()
    {
        LDR = _settingsManager.Manifest.MapCompilerSettings.VradSettings.Ldr;
        HDR = _settingsManager.Manifest.MapCompilerSettings.VradSettings.Hdr;
        Fast = _settingsManager.Manifest.MapCompilerSettings.VradSettings.Fast;
        Final = _settingsManager.Manifest.MapCompilerSettings.VradSettings.Final;
        StaticPropLighting = _settingsManager.Manifest.MapCompilerSettings.VradSettings.StaticPropLighting;
        StaticPropPolys = _settingsManager.Manifest.MapCompilerSettings.VradSettings.StaticPropPolys;
        TextureShadows = _settingsManager.Manifest.MapCompilerSettings.VradSettings.TextureShadows;
        LowPriority = _settingsManager.Manifest.MapCompilerSettings.VradSettings.LowPriority;
        UseModifiedVrad = _settingsManager.Manifest.MapCompilerSettings.VradSettings.UseModifiedVrad;
        LargeDispSampleRadius = _settingsManager.Manifest.MapCompilerSettings.VradSettings.LargeDispSampleRadius;
        OtherArguments = _settingsManager.Manifest.MapCompilerSettings.VradSettings.OtherArguments;
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