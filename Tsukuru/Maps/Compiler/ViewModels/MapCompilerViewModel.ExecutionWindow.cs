using System;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public partial class MapCompilerViewModel  : ILogReceiver
    {
        private string _consoleText;
        private bool _isCloseButtonOnExecutionEnabled;

        public string MapNameDisplay => MapName.Replace("_", "__");

        public string ConsoleText
        {
            get => _consoleText;
            set
            {
                Set(() => ConsoleText, ref _consoleText, value);
            }
        }
   
        public bool IsExecuteButtonEnabled => !string.IsNullOrWhiteSpace(VMFPath);

        public bool IsCloseButtonOnExecutionEnabled
        {
            get { return _isCloseButtonOnExecutionEnabled; }
            set
            {
                _isCloseButtonOnExecutionEnabled = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsProgressBarIndeterminate");
            }
        }

        public bool IsProgressBarIndeterminate => !_isCloseButtonOnExecutionEnabled;

        public void Write(string message)
        {
            ConsoleText += message;
        }

        public void WriteLine(string category, string message)
        {
            ConsoleText += $"[{category}]: {message}{Environment.NewLine}";
        }
    }
}
