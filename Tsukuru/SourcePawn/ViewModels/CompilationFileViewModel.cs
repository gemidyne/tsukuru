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
		private string _statusIcon = "/Tsukuru;component/Resources/script_code.png";
        private bool _canShowDetails;

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

        public string StatusIcon 
        {
            get { return _statusIcon; }
	        set { Set(() => StatusIcon, ref _statusIcon, value); }
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
