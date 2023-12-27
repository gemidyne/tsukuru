using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels;

public class VbspCompilationSettingsViewModel : BaseCompilationSettings, IApplicationContentView
{
    private readonly ISettingsManager _settingsManager;
    private bool _onlyEntities;
    private bool _onlyProps;
    private bool _noDetailEntities;
    private bool _noWaterBrushes;
    private bool _lowPriority;
    private bool _keepStalePackedData;
    private string _otherArguments;
    private bool _isLoading;

    public bool OnlyEntities
    {
        get => _onlyEntities;
        set
        {
            SetProperty(ref _onlyEntities, value);
            OnArgumentChanged();

            _settingsManager.Manifest.MapCompilerSettings.VbspSettings.OnlyEntities = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public bool OnlyProps
    {
        get => _onlyProps;
        set
        {
            SetProperty(ref _onlyProps, value);
            OnArgumentChanged();

            _settingsManager.Manifest.MapCompilerSettings.VbspSettings.OnlyProps = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public bool NoDetailEntities
    {
        get => _noDetailEntities;
        set
        {
            SetProperty(ref _noDetailEntities, value);
            OnArgumentChanged();

            _settingsManager.Manifest.MapCompilerSettings.VbspSettings.NoDetailEntities = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public bool NoWaterBrushes
    {
        get => _noWaterBrushes;
        set
        {
            SetProperty(ref _noWaterBrushes, value);
            OnArgumentChanged();

            _settingsManager.Manifest.MapCompilerSettings.VbspSettings.NoWaterBrushes = value;

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

            _settingsManager.Manifest.MapCompilerSettings.VbspSettings.LowPriority = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public bool KeepStalePackedData
    {
        get => _keepStalePackedData;
        set
        {
            SetProperty(ref _keepStalePackedData, value);
            OnArgumentChanged();

            _settingsManager.Manifest.MapCompilerSettings.VbspSettings.KeepStalePackedData = value;

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

            _settingsManager.Manifest.MapCompilerSettings.VbspSettings.OtherArguments = value;

            if (!IsLoading)
            {
                _settingsManager.Save();
            }
        }
    }

    public string Name => "VBSP Settings";

    public string Description =>
        "VBSP is the command-line tool that compiles a raw VMF file into the Binary Space Partition format. It is followed by VVIS and VRAD.";

    public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public VbspCompilationSettingsViewModel(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public void Init()
    {
        OnlyEntities = _settingsManager.Manifest.MapCompilerSettings.VbspSettings.OnlyEntities;
        OnlyProps = _settingsManager.Manifest.MapCompilerSettings.VbspSettings.OnlyProps;
        NoDetailEntities = _settingsManager.Manifest.MapCompilerSettings.VbspSettings.NoDetailEntities;
        NoWaterBrushes = _settingsManager.Manifest.MapCompilerSettings.VbspSettings.NoWaterBrushes;
        KeepStalePackedData = _settingsManager.Manifest.MapCompilerSettings.VbspSettings.KeepStalePackedData;
        LowPriority = _settingsManager.Manifest.MapCompilerSettings.VvisSettings.LowPriority;
        OtherArguments = _settingsManager.Manifest.MapCompilerSettings.VvisSettings.OtherArguments;
    }

    public override string BuildArguments()
    {
        return
            ConditionalArg(() => OnlyEntities, "-onlyents") +
            ConditionalArg(() => OnlyProps, "-onlyprops") +
            ConditionalArg(() => NoDetailEntities, "-nodetail") +
            ConditionalArg(() => NoWaterBrushes, "-nowater") +
            ConditionalArg(() => LowPriority, "-low") +
            ConditionalArg(() => KeepStalePackedData, "-keepstalezip") +
            OtherArguments;
    }


}