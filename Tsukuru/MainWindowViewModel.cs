using GalaSoft.MvvmLight;

namespace Tsukuru
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _displayMapCompilerResultsView;
        private bool _displaySourcePawnCompilerView;
        private bool _displayMapCompilerView;

        public bool DisplaySourcePawnCompilerView
        {
            get => _displaySourcePawnCompilerView;
            set => Set(() => DisplaySourcePawnCompilerView, ref _displaySourcePawnCompilerView, value);
        }

        public bool DisplayMapCompilerView
        {
            get => _displayMapCompilerView;
            set => Set(() => DisplayMapCompilerView, ref _displayMapCompilerView, value);
        }

        public bool DisplayMapCompilerResultsView
        {
            get => _displayMapCompilerResultsView;
            set => Set(() => DisplayMapCompilerResultsView, ref _displayMapCompilerResultsView, value);
        }

        public MainWindowViewModel()
        {
            DisplayMapCompilerResultsView = false;
            DisplayMapCompilerView = true;
            DisplaySourcePawnCompilerView = true;
        }
    }
}