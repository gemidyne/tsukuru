using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class VbspCompilationSettingsViewModel : BaseCompilationSettings, IApplicationContentView
    {
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
                Set(() => OnlyEntities, ref _onlyEntities, value);
                OnArgumentChanged();

                SettingsManager.Manifest.MapCompilerSettings.VbspSettings.OnlyEntities = value;

                if (!IsLoading)
                {
                    SettingsManager.Save();
                }
            }
        }

        public bool OnlyProps
        {
            get => _onlyProps;
            set
            {
                Set(() => OnlyProps, ref _onlyProps, value);
                OnArgumentChanged();

                SettingsManager.Manifest.MapCompilerSettings.VbspSettings.OnlyProps = value;

                if (!IsLoading)
                {
                    SettingsManager.Save();
                }
            }
        }

        public bool NoDetailEntities
        {
            get => _noDetailEntities;
            set
            {
                Set(() => NoDetailEntities, ref _noDetailEntities, value);
                OnArgumentChanged();

                SettingsManager.Manifest.MapCompilerSettings.VbspSettings.NoDetailEntities = value;

                if (!IsLoading)
                {
                    SettingsManager.Save();
                }
            }
        }

        public bool NoWaterBrushes
        {
            get => _noWaterBrushes;
            set
            {
                Set(() => NoWaterBrushes, ref _noWaterBrushes, value);
                OnArgumentChanged();

                SettingsManager.Manifest.MapCompilerSettings.VbspSettings.NoWaterBrushes = value;

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
                Set(() => LowPriority, ref _lowPriority, value);
                OnArgumentChanged();

                SettingsManager.Manifest.MapCompilerSettings.VbspSettings.LowPriority = value;

                if (!IsLoading)
                {
                    SettingsManager.Save();
                }
            }
        }

        public bool KeepStalePackedData
        {
            get => _keepStalePackedData;
            set
            {
                Set(() => KeepStalePackedData, ref _keepStalePackedData, value);
                OnArgumentChanged();

                SettingsManager.Manifest.MapCompilerSettings.VbspSettings.KeepStalePackedData = value;

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
                Set(() => OtherArguments, ref _otherArguments, value);
                OnArgumentChanged();

                SettingsManager.Manifest.MapCompilerSettings.VbspSettings.OtherArguments = value;

                if (!IsLoading)
                {
                    SettingsManager.Save();
                }
            }
        }

        public string Name => "VBSP Settings";

        public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public void Init()
        {
            OnlyEntities = SettingsManager.Manifest.MapCompilerSettings.VbspSettings.OnlyEntities;
            OnlyProps = SettingsManager.Manifest.MapCompilerSettings.VbspSettings.OnlyProps;
            NoDetailEntities = SettingsManager.Manifest.MapCompilerSettings.VbspSettings.NoDetailEntities;
            NoWaterBrushes = SettingsManager.Manifest.MapCompilerSettings.VbspSettings.NoWaterBrushes;
            KeepStalePackedData = SettingsManager.Manifest.MapCompilerSettings.VbspSettings.KeepStalePackedData;
            LowPriority = SettingsManager.Manifest.MapCompilerSettings.VvisSettings.LowPriority;
            OtherArguments = SettingsManager.Manifest.MapCompilerSettings.VvisSettings.OtherArguments;
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
}
