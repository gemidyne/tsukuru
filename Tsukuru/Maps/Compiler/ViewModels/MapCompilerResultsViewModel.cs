using GalaSoft.MvvmLight;
using System.Text;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class MapCompilerResultsViewModel : ViewModelBase, ILogReceiver
    {
        private readonly MapCompilerViewModel _mapCompilerViewModel;
        private readonly StringBuilder _consoleText = new StringBuilder();

        private bool _isCloseButtonOnExecutionEnabled;

        public string MapNameDisplay => _mapCompilerViewModel.MapName.Replace("_", "__");

        public string ConsoleText => _consoleText.ToString();

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

        public MapCompilerResultsViewModel(MapCompilerViewModel mapCompilerViewModel)
        {
            _mapCompilerViewModel = mapCompilerViewModel;
        }


        public void Write(string message)
        {
            _consoleText.Append(message);
            RaisePropertyChanged("ConsoleText");
        }

        public void WriteLine(string category, string message)
        {
            _consoleText.AppendLine($"[{category}]: {message}");
            RaisePropertyChanged("ConsoleText");
        }

        public void ClearLog()
        {
            _consoleText.Clear();
        }
    }
}
