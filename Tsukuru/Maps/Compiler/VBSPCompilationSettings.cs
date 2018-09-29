namespace Tsukuru.Maps.Compiler
{
    public class VbspCompilationSettings : BaseCompilationSettings
    {
        private bool _onlyEntities;
        private bool _onlyProps;
        private bool _noDetailEntities;
        private bool _noWaterBrushes;
        private bool _lowPriority;
        private bool _keepStalePackedData;
        private string _otherArguments;

        public bool OnlyEntities
        {
            get => _onlyEntities;
            set
            {
                Set(() => OnlyEntities, ref _onlyEntities, value);
                OnArgumentChanged();
            }
        }

        public bool OnlyProps
        {
            get => _onlyProps;
            set
            {
                Set(() => OnlyProps, ref _onlyProps, value);
                OnArgumentChanged();
            }
        }

        public bool NoDetailEntities
        {
            get => _noDetailEntities;
            set
            {
                Set(() => NoDetailEntities, ref _noDetailEntities, value);
                OnArgumentChanged();
            }
        }

        public bool NoWaterBrushes
        {
            get => _noWaterBrushes;
            set
            {
                Set(() => NoWaterBrushes, ref _noWaterBrushes, value);
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

        public bool KeepStalePackedData
        {
            get => _keepStalePackedData;
            set
            {
                Set(() => KeepStalePackedData, ref _keepStalePackedData, value);
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
