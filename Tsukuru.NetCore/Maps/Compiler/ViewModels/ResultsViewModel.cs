using System;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Tsukuru.Maps.Compiler.Business;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.ViewModels
{
    public class ResultsViewModel : ViewModelBase, IApplicationContentView
    {
        private bool _isCloseButtonOnExecutionEnabled;
        private string _heading;
        private string _mapName;
        private string _subtitle;
        private int _progressValue;
        private int _progressMaximum;
        private bool _isLoading;
        private int _activeLogsIndex;

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

        public ObservableCollection<ResultsLogContainer> Logs { get; } = new ObservableCollection<ResultsLogContainer>();

        public int ActiveLogsIndex
        {
            get => _activeLogsIndex;
            set => Set(() => ActiveLogsIndex, ref _activeLogsIndex, value);
        }

        public bool IsProgressBarIndeterminate => false;

        public RelayCommand CloseCommand { get; }

        public string Name => "Compiler Results";

        public string Description => "View current compiler progress and results.";

        public EShellNavigationPage Group => EShellNavigationPage.SourceMapCompiler;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public ResultsViewModel()
        {
            CloseCommand = new RelayCommand(CloseResults);
        }

        private void CloseResults()
        {
#warning TODO AMEND THIS
            //_mainWindowViewModel.DisplayMapCompilerResultsView = false;
            //_mainWindowViewModel.DisplayMapCompilerView = true;
            //_mainWindowViewModel.DisplaySourcePawnCompilerView = true;

            lock (Logs)
            {
                Logs.Clear();
            }
        }

        public void Init()
        {
            _mapName = MapCompileSessionInfo.Instance.MapName;
            Subtitle = "Ready";
        }

        public void Initialise(string mapName)
        {
            Heading = $"Compiling {_mapName}...";
            Subtitle = "Please wait...";
            IsCloseButtonOnExecutionEnabled = false;

            Logs.Clear();
        }

        public void NotifyComplete(TimeSpan timeElapsed)
        {
            Heading = $"Compiled {_mapName}";
            Subtitle = $"Completed in {timeElapsed}";

            IsCloseButtonOnExecutionEnabled = true;
            ProgressValue = ProgressMaximum;
        }

        public ResultsLogContainer GetLogDestination(string category)
        {
            if (Logs.All(x => x.Category != category))
            {
                Logs.Add(new ResultsLogContainer { Category = category });
            }

            return Logs.Single(x => x.Category == category);
        }

        public void NavigateToLogTab(string category)
        {
            var tab = Logs.SingleOrDefault(x => x.Category == category);

            if (tab == null)
            {
                return;
            }

            int idx = Logs.IndexOf(tab);

            ActiveLogsIndex = idx;
        }
    }
}
