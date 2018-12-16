using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler
{
    public class VradCompilationSettings : BaseCompilationSettings
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

        public bool LDR
        {
            get => _ldr;
            set
            {
                Set(() => LDR, ref _ldr, value);
                OnArgumentChanged();

	            SettingsManager.Manifest.MapCompilerSettings.VradSettings.Ldr = value;
                SettingsManager.Save();
            }
        }

        public bool HDR
        {
            get => _hdr;
            set
            {
                Set(() => HDR, ref _hdr, value);
                OnArgumentChanged();

	            SettingsManager.Manifest.MapCompilerSettings.VradSettings.Hdr = value;
	            SettingsManager.Save();
            }
        }

        public bool Fast
        {
            get => _fast;
            set
            {
                Set(() => Fast, ref _fast, value);
                OnArgumentChanged();

	            SettingsManager.Manifest.MapCompilerSettings.VradSettings.Fast = value;
	            SettingsManager.Save();
            }
        }

        public bool Final
        {
            get => _final;
            set
            {
                Set(() => Final, ref _final, value);
                OnArgumentChanged();

	            SettingsManager.Manifest.MapCompilerSettings.VradSettings.Final = value;
	            SettingsManager.Save();
            }
        }

        public bool StaticPropLighting
        {
            get => _staticPropLighting;
            set
            {
                Set(() => StaticPropLighting, ref _staticPropLighting, value);
                OnArgumentChanged();

	            SettingsManager.Manifest.MapCompilerSettings.VradSettings.StaticPropLighting = value;
	            SettingsManager.Save();
            }
        }

        public bool StaticPropPolys
        {
            get => _staticPropPolys;
            set
            {
                Set(() => StaticPropPolys, ref _staticPropPolys, value);
                OnArgumentChanged();

	            SettingsManager.Manifest.MapCompilerSettings.VradSettings.StaticPropPolys = value;
	            SettingsManager.Save();
            }
        }

        public bool TextureShadows
        {
            get => _textureShadows;
            set
            {
                Set(() => TextureShadows, ref _textureShadows, value);
                OnArgumentChanged();

	            SettingsManager.Manifest.MapCompilerSettings.VradSettings.TextureShadows = value;
	            SettingsManager.Save();
            }
        }

        public bool LowPriority
        {
            get => _lowPriority;
            set
            {
                Set(() => LowPriority, ref _lowPriority, value);
                OnArgumentChanged();

	            SettingsManager.Manifest.MapCompilerSettings.VradSettings.LowPriority = value;
	            SettingsManager.Save();
            }
        }

        public bool UseModifiedVrad
        {
            get => _useModifiedVrad;
            set
            {
                Set(() => UseModifiedVrad, ref _useModifiedVrad, value);

	            SettingsManager.Manifest.MapCompilerSettings.VradSettings.UseModifiedVrad = value;
	            SettingsManager.Save();
            }
        }

        public string OtherArguments
        {
            get => _otherArguments;
            set
            {
                Set(() => OtherArguments, ref _otherArguments, value);
                OnArgumentChanged();

	            SettingsManager.Manifest.MapCompilerSettings.VradSettings.OtherArguments = value;
	            SettingsManager.Save();
            }
        }

	    public VradCompilationSettings()
	    {
		    LDR = SettingsManager.Manifest.MapCompilerSettings.VradSettings.Ldr;
		    HDR = SettingsManager.Manifest.MapCompilerSettings.VradSettings.Hdr;
		    Fast = SettingsManager.Manifest.MapCompilerSettings.VradSettings.Fast;
		    Final = SettingsManager.Manifest.MapCompilerSettings.VradSettings.Final;
		    StaticPropLighting = SettingsManager.Manifest.MapCompilerSettings.VradSettings.StaticPropLighting;
		    StaticPropPolys = SettingsManager.Manifest.MapCompilerSettings.VradSettings.StaticPropPolys;
		    TextureShadows = SettingsManager.Manifest.MapCompilerSettings.VradSettings.TextureShadows;

		    LowPriority = SettingsManager.Manifest.MapCompilerSettings.VvisSettings.LowPriority;
		    UseModifiedVrad = SettingsManager.Manifest.MapCompilerSettings.VradSettings.UseModifiedVrad;
		    OtherArguments = SettingsManager.Manifest.MapCompilerSettings.VvisSettings.OtherArguments;
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
                OtherArguments;
        }
    }
}
