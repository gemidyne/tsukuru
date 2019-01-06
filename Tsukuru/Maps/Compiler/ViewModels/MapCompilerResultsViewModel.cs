using System;
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
        private string _heading;
        private string _mapName;
        private string _subtitle;
        private int _progressValue;
        private int _progressMaximum;

        public string Heading
        {
            get => _heading;
            set => Set(() => Heading, ref _heading, value);
        }

        public string Subtitle
        {
	        get => _subtitle;
	        set => Set(() => Subtitle, ref _subtitle, value);
        }

        public int ProgressValue
        {
	        get => _progressValue;
	        set => Set(() => ProgressValue, ref _progressValue, value);
        }

        public int ProgressMaximum
        {
	        get => _progressMaximum;
	        set => Set(() => ProgressMaximum, ref _progressMaximum, value);
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
            get => _isCloseButtonOnExecutionEnabled;
            set
            {
                _isCloseButtonOnExecutionEnabled = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsProgressBarIndeterminate));
            }
        }

        public bool IsProgressBarIndeterminate => false;

        public RelayCommand CloseCommand { get; }

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

        public void Initialise(string mapName)
        {
	        _mapName = mapName;

	        Heading = $"Map Compiler: {_mapName}";
	        Subtitle = "Please wait...";
            IsCloseButtonOnExecutionEnabled = false;

            lock (_door)
            {
                _consoleText.Clear();
            }
        }

        public void NotifyComplete(TimeSpan timeElapsed)
        {
	        Heading = $"Map Compiler {_mapName}";
	        Subtitle = $"Completed in {timeElapsed}";
	        WriteLine("Tsukuru", $"Completed in {timeElapsed}");

	        IsCloseButtonOnExecutionEnabled = true;
	        ProgressValue = ProgressMaximum;
        }

        public void Write(string message)
        {
            lock (_door)
            {
                _consoleText.Append(message);
                RaisePropertyChanged(nameof(ConsoleText));
            }
        }

        public void WriteLine(string category, string message)
        {
            lock (_door)
            {
                _consoleText.AppendLine($"[{category}]: {message}");
                RaisePropertyChanged(nameof(ConsoleText));
            }
        }
    }
}
