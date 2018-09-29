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

        public bool LDR
        {
            get => _ldr;
            set
            {
                Set(() => LDR, ref _ldr, value);
                OnArgumentChanged();
            }
        }

        public bool HDR
        {
            get => _hdr;
            set
            {
                Set(() => HDR, ref _hdr, value);
                OnArgumentChanged();
            }
        }

        public bool Fast
        {
            get => _fast;
            set
            {
                Set(() => Fast, ref _fast, value);
                OnArgumentChanged();
            }
        }

        public bool Final
        {
            get => _final;
            set
            {
                Set(() => Final, ref _final, value);
                OnArgumentChanged();
            }
        }

        public bool StaticPropLighting
        {
            get => _staticPropLighting;
            set
            {
                Set(() => StaticPropLighting, ref _staticPropLighting, value);
                OnArgumentChanged();
            }
        }

        public bool StaticPropPolys
        {
            get => _staticPropPolys;
            set
            {
                Set(() => StaticPropPolys, ref _staticPropPolys, value);
                OnArgumentChanged();
            }
        }

        public bool TextureShadows
        {
            get => _textureShadows;
            set
            {
                Set(() => TextureShadows, ref _textureShadows, value);
                OnArgumentChanged();
            }
        }

        public bool LowPriority
        {
            get => _lowPriority;
            set
            {
                Set(() => LowPriority, ref _lowPriority, value);
                OnArgumentChanged();
            }
        }

        public string OtherArguments
        {
            get => _otherArguments;
            set
            {
                Set(() => OtherArguments, ref _otherArguments, value);
                OnArgumentChanged();
            }
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
