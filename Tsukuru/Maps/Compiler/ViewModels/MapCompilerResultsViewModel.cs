using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class MapCompilerResultsViewModel : ViewModelBase, ILogReceiver
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private static readonly object _door = new object();
        private readonly StringBuilder _consoleText = new StringBuilder();

        private bool _isCloseButtonOnExecutionEnabled;
        private string _mapNameDisplay;

        public string MapNameDisplay
        {
            get => _mapNameDisplay;
            set => Set(() => MapNameDisplay, ref _mapNameDisplay, value);
        }

        public string ConsoleText
        {
            get
            {
                lock (_door)
                {
                    return _consoleText.ToString();
                }
            }
        }

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

        public RelayCommand CloseCommand { get; private set; }

        public MapCompilerResultsViewModel(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;

            CloseCommand = new RelayCommand(CloseResults);
        }

        private void CloseResults()
        {
            _mainWindowViewModel.DisplayMapCompilerResultsView = false;
            _mainWindowViewModel.DisplayMapCompilerView = true;
            _mainWindowViewModel.DisplaySourcePawnCompilerView = true;
        }

        public void StartNewSession(string mapName)
        {
            MapNameDisplay = $"Compiling map: {mapName}";
            IsCloseButtonOnExecutionEnabled = false;

            lock (_door)
            {
                _consoleText.Clear();
            }
        }

        public void Write(string message)
        {
            lock (_door)
            {
                _consoleText.Append(message);
                RaisePropertyChanged("ConsoleText");
            }
        }

        public void WriteLine(string category, string message)
        {
            lock (_door)
            {
                _consoleText.AppendLine($"[{category}]: {message}");
                RaisePropertyChanged("ConsoleText");
            }
        }
    }
}
