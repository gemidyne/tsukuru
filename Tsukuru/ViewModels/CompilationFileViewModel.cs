using System.Collections.ObjectModel;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Tsukuru.Data;

namespace Tsukuru.ViewModels
{
    public class CompilationFileViewModel : ViewModelBase
    {
		private string _file;
		private ObservableCollection<CompilationMessage> _messages;
		private CompilationResult _result = CompilationResult.Unknown;
		private string _statusIcon = "/Tsukuru;component/Resources/script_code.png";

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

		public RelayCommand ShowDetailsCommand { get; private set; }

	    public CompilationFileViewModel()
	    {
		    ShowDetailsCommand = new RelayCommand(ShowDetails);
	    }

	    private void ShowDetails()
	    {
		    var viewModel = SimpleIoc.Default.GetInstance<ResultsWindowViewModel>();

			viewModel.SetResults(Messages, string.Format("Results - {0} - Total {1} message(s)", File, Messages.Count));

		    var info = new ResultsWindow
		    {
			    Owner = Application.Current.MainWindow
		    };

		    info.ShowDialog();
	    }
    }
}
