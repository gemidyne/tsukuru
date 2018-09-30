using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using System.Collections.ObjectModel;
using System.Windows;

namespace Tsukuru.SourcePawn.ViewModels
{
    public class CompilationFileViewModel : ViewModelBase
    {
		private string _file;
		private ObservableCollection<CompilationMessage> _messages;
		private CompilationResult _result = CompilationResult.Unknown;
        private bool _canShowDetails;
        private bool _isSuccessfulCompile;
        private bool _isBusy;
        private bool _isCompiledWithErrors;
        private bool _isCompiledWithWarnings;
        private bool _isUnknownState;

        public string File
        {
            get { return _file; }
	        set { Set(() => File, ref _file, value); } 
        }

        public CompilationResult Result
        {
            get { return _result; }
	        set { Set(() => Result, ref _result, value); }
        }

        public ObservableCollection<CompilationMessage> Messages
        {
            get { return _messages ?? (_messages = new ObservableCollection<CompilationMessage>()); }
        }

        public bool IsSuccessfulCompile
        {
            get => _isSuccessfulCompile;
            set => Set(() => IsSuccessfulCompile, ref _isSuccessfulCompile, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(() => IsBusy, ref _isBusy, value);
        }

        public bool IsCompiledWithErrors
        {
            get => _isCompiledWithErrors;
            set => Set(() => IsCompiledWithErrors, ref _isCompiledWithErrors, value);
        }

        public bool IsCompiledWithWarnings
        {
            get => _isCompiledWithWarnings;
            set => Set(() => IsCompiledWithWarnings, ref _isCompiledWithWarnings, value);
        }

        public bool IsUnknownState
        {
            get => _isUnknownState;
            set => Set(() => IsUnknownState, ref _isUnknownState, value);
        }

        public bool CanShowDetails
        {
            get => _canShowDetails;
            set
            {
                Set(() => CanShowDetails, ref _canShowDetails, value);
            }
        }

        public RelayCommand ShowDetailsCommand { get; private set; }

	    public CompilationFileViewModel()
	    {
		    ShowDetailsCommand = new RelayCommand(ShowDetails);
	    }

	    private void ShowDetails()
	    {
		    var viewModel = SimpleIoc.Default.GetInstance<ResultsWindowViewModel>();

			viewModel.SetResults(Messages, $"Results - {File} - Total {Messages.Count} message(s)");

		    var info = new ResultsWindow
		    {
			    Owner = Application.Current.MainWindow
		    };

		    info.ShowDialog();
	    }
    }
}
