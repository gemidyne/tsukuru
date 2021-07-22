using GalaSoft.MvvmLight;

namespace Tsukuru.SourcePawn
{
    public class CompilationMessage : ViewModelBase
    {
        private int? _firstLine;
        private int _lastLine;
        private string _fileName;
        private string _prefix;
        private string _message;
        private string _rawLine;

        public int? FirstLine
        {
            get => _firstLine;
            set
            {
                Set(() => FirstLine, ref _firstLine, value);
                RaisePropertyChanged(nameof(LineNumberDisplay));
            }
        }

        public int LastLine
        {
            get => _lastLine;
            set
            {
                Set(() => LastLine, ref _lastLine, value);
                RaisePropertyChanged(nameof(LineNumberDisplay));
            }
        }

        public string FileName
        {
            get => _fileName;
            set => Set(() => FileName, ref _fileName, value);
        }

        public string Prefix
        {
            get => _prefix;
            set => Set(() => Prefix, ref _prefix, value);
        }

        public string Message
        {
            get => _message;
            set => Set(() => Message, ref _message, value);
        }

        public string RawLine
        {
            get => _rawLine;
            set => Set(() => RawLine, ref _rawLine, value);
        }

        public string LineNumberDisplay => FirstLine.HasValue 
            ? FirstLine + " - " + LastLine 
            : LastLine.ToString();
    }
}
