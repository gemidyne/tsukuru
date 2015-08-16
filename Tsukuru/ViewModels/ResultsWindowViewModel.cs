using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using Tsukuru.Data;

namespace Tsukuru.ViewModels
{
    public class ResultsWindowViewModel : ViewModelBase
    {
		private ObservableCollection<CompilationMessage> _errors;
		private ObservableCollection<CompilationMessage> _warnings;

		private string _errorsHeader;
		private string _rawOutput;
		private string _warningsHeader;
		private string _windowTitle;

        public ObservableCollection<CompilationMessage> Messages { get; set; }

        public ObservableCollection<CompilationMessage> Errors
        {
	        get { return _errors; }
			set { Set(() => Errors, ref _errors, value); }
        }

        public ObservableCollection<CompilationMessage> Warnings
        {
	        get { return _warnings; }
			set { Set(() => Warnings, ref _warnings, value); }
        }

        public string RawOutput
        {
			get { return _rawOutput; }
			set { Set(() => RawOutput, ref _rawOutput, value); }
        }

        public string ErrorsHeader
        {
            get { return _errorsHeader; }
	        set { Set(() => ErrorsHeader, ref _errorsHeader, value); }
        }

        public string WarningsHeader
        {
            get { return _warningsHeader; }
	        set { Set(() => WarningsHeader, ref _warningsHeader, value); }
        }

        public string WindowTitle
        {
            get { return _windowTitle; }
	        set { Set(() => WindowTitle, ref _windowTitle, value); } 
        }

	    public void SetResults(ObservableCollection<CompilationMessage> messages, string windowTitle)
	    {
		    Messages = messages;
		    WindowTitle = windowTitle;

			Errors = new ObservableCollection<CompilationMessage>(Messages.Where(m => CompilationMessageHelper.IsLineError(m.Prefix)));
			ErrorsHeader = string.Format("Errors ({0})", _errors.Count);

			Warnings = new ObservableCollection<CompilationMessage>(Messages.Where(m => CompilationMessageHelper.IsLineWarning(m.Prefix)));
			WarningsHeader = string.Format("Warnings ({0})", _warnings.Count);

			RawOutput = string.Join("\r\n", Messages.Except(Errors).Except(Warnings).Select(m => m.RawLine));
	    }
    }
}
