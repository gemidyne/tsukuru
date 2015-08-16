using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Shell;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Tsukuru.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
		private bool _executePostBuildScripts;
		private bool _incrementVersion;
		private bool _areCommandButtonsEnabled = true;

		private int _progressBarValue;
		private int _progressBarMaximum = 100;
		private string _appVersion;
		private string _sourcePawnCompiler;
		private ObservableCollection<CompilationFileViewModel> _filesToCompile;
	    private CompilationFileViewModel _selectedFile;

		public string AppVersion
		{
			get { return _appVersion; }
			set { Set(() => AppVersion, ref _appVersion, value); }
		}

        public string SourcePawnCompiler
        {
            get { return _sourcePawnCompiler; }
            set 
            {
				Set(() => SourcePawnCompiler, ref _sourcePawnCompiler, value);
                SettingsManager.CompilerLocation = value;
            }
        }

        public ObservableCollection<CompilationFileViewModel> FilesToCompile
        {
            get { return _filesToCompile ?? (_filesToCompile = new ObservableCollection<CompilationFileViewModel>()); }
        }

        public bool ExecutePostBuildScripts
        {
            get { return _executePostBuildScripts; }
            set 
            {
				Set(() => ExecutePostBuildScripts, ref _executePostBuildScripts, value);
                SettingsManager.ExecutePostBuildScripts = value;
            }
        }

        public bool IncrementVersion
        {
            get { return _incrementVersion; }
            set 
            {
				Set(() => IncrementVersion, ref _incrementVersion, value);
                SettingsManager.IncrementVersion = value;
            }
        }

        public bool AreCommandButtonsEnabled
        {
            get { return _areCommandButtonsEnabled; }
            set
            {
				Set(() => AreCommandButtonsEnabled, ref _areCommandButtonsEnabled, value);
                RaisePropertyChanged("TaskbarProgress");
                RaisePropertyChanged("TaskbarState");
            }
        }

        public int ProgressBarValue
        {
            get { return _progressBarValue; }
            set 
            {
				Set(() => ProgressBarValue, ref _progressBarValue, value);
                RaisePropertyChanged("TaskbarProgress");
                RaisePropertyChanged("TaskbarState");
				RaisePropertyChanged("IsProgressIndeterminate");
            }
        }

        public int ProgressBarMaximum
        {
            get { return _progressBarMaximum; }
            set { Set(() => ProgressBarMaximum, ref _progressBarMaximum, value); }
        }

        public double TaskbarProgress
        {
            get { return ((double)ProgressBarValue / ProgressBarMaximum); }
        }

        public TaskbarItemProgressState TaskbarState
        {
            get
            {
                if (AreCommandButtonsEnabled)
                {
                    return TaskbarItemProgressState.None;
                }

                return ProgressBarValue == 0 
					? TaskbarItemProgressState.Indeterminate 
					: TaskbarItemProgressState.Normal;
            }
        }

	    public bool IsProgressIndeterminate
	    {
		    get
		    {
				if (AreCommandButtonsEnabled)
				{
					return false;
				}

			    return !AreCommandButtonsEnabled && ProgressBarValue == 0;
		    }
	    }

	    public CompilationFileViewModel SelectedFile
	    {
		    get { return _selectedFile; }
		    set { Set(() => SelectedFile, ref _selectedFile, value); }
	    }

		public RelayCommand AddFileCommand { get; private set; }
		public RelayCommand BrowseCompilerCommand { get; private set; }
		public RelayCommand BuildCommand { get; private set; }
		public RelayCommand RemoveFileCommand { get; private set; }

	    public MainWindowViewModel()
        {
            SourcePawnCompiler = SettingsManager.CompilerLocation;
            ExecutePostBuildScripts = SettingsManager.ExecutePostBuildScripts;
            IncrementVersion = SettingsManager.IncrementVersion;

			AppVersion = string.Format("v{0}", Assembly.GetExecutingAssembly().GetName().Version);

			AddFileCommand = new RelayCommand(AddFile);
			BrowseCompilerCommand = new RelayCommand(BrowseCompiler);
			BuildCommand = new RelayCommand(Build);
			RemoveFileCommand = new RelayCommand(RemoveFile);
        }

	    private async void Build()
	    {
			await Task.Run(() =>
			{
				AreCommandButtonsEnabled = false;

				var proc = new Processor();
				proc.CompileBatch(this);

				AreCommandButtonsEnabled = true;
			});
	    }

	    private void BrowseCompiler()
	    {
		    var dialog = new Ookii.Dialogs.VistaOpenFileDialog
		    {
			    CheckFileExists = true,
			    CheckPathExists = true,
			    Multiselect = false,
			    Filter = "SourcePawn Compiler|*.exe",
			    InitialDirectory = System.IO.Directory.GetCurrentDirectory(),
			    Title = "Choose a SourcePawn Compiler."
		    };

		    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}

			SourcePawnCompiler = dialog.FileName;
	    }

	    private void AddFile()
	    {
			var dialog = new Ookii.Dialogs.VistaOpenFileDialog
			{
				CheckFileExists = true,
				CheckPathExists = true,
				Multiselect = false,
				Filter = "SourcePawn Files|*.sp",
				InitialDirectory = System.IO.Directory.GetCurrentDirectory(),
				Title = "Choose a file."
			};

			if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
			{
				return;
			}

			if (FilesToCompile.All(c => c.File != dialog.FileName))
			{
				FilesToCompile.Add(new CompilationFileViewModel { File = dialog.FileName });
			}
	    }

	    private void RemoveFile()
	    {
			FilesToCompile.Remove(SelectedFile);
	    }
    }
}
