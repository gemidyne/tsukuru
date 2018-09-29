namespace Tsukuru.Maps.Compiler
{
    public class VvisCompilationSettings : BaseCompilationSettings, ICompilationSettings
    {
        private bool _fast;
        private bool _lowPriority;
        private string _otherArguments;

        public bool Fast
        {
            get => _fast;
            set
            {
                Set(() => Fast, ref _fast, value);
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
                ConditionalArg(() => Fast, "-fast") +
                ConditionalArg(() => LowPriority, "-low") +
                OtherArguments;
        }
    }
}
