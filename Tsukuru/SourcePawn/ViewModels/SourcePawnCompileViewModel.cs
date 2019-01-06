using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Tsukuru.Settings;

namespace Tsukuru.SourcePawn.ViewModels
{
	public class SourcePawnCompileViewModel : ViewModelBase
    {
		private bool _executePostBuildScripts;
		private bool _incrementVersion;
		private bool _areCommandButtonsEnabled = true;

		private int _progressBarValue;
		private int _progressBarMaximum = 100;
		private string _sourcePawnCompiler;
		private ObservableCollection<CompilationFileViewModel> _filesToCompile;
	    private CompilationFileViewModel _selectedFile;
	    private bool _copySmxToClipboardOnCompile;
	    private FileSystemWatcher watcher;
	    private bool _isWatchingOrBuilding;

	    public string SourcePawnCompiler
        {
            get => _sourcePawnCompiler;
            set 
            {
				Set(() => SourcePawnCompiler, ref _sourcePawnCompiler, value);

                SettingsManager.Manifest.SourcePawnCompiler.CompilerPath = value;
                SettingsManager.Save();
            }
        }

        public ObservableCollection<CompilationFileViewModel> FilesToCompile => _filesToCompile ?? (_filesToCompile = new ObservableCollection<CompilationFileViewModel>());

        public bool ExecutePostBuildScripts
        {
            get => _executePostBuildScripts;
            set 
            {
				Set(() => ExecutePostBuildScripts, ref _executePostBuildScripts, value);

                SettingsManager.Manifest.SourcePawnCompiler.ExecutePostBuildScripts = value;
                SettingsManager.Save();
            }
        }

        public bool IncrementVersion
        {
            get => _incrementVersion;
            set 
            {
				Set(() => IncrementVersion, ref _incrementVersion, value);

                SettingsManager.Manifest.SourcePawnCompiler.Versioning = value;
                SettingsManager.Save();
            }
        }

        public bool AreCommandButtonsEnabled
        {
            get => _areCommandButtonsEnabled;
            set
            {
				Set(() => AreCommandButtonsEnabled, ref _areCommandButtonsEnabled, value);
                RaisePropertyChanged(nameof(TaskbarProgress));
                RaisePropertyChanged(nameof(TaskbarState));
                RaisePropertyChanged(nameof(IsProgressIndeterminate));
            }
        }

        public int ProgressBarValue
        {
            get => _progressBarValue;
            set 
            {
				Set(() => ProgressBarValue, ref _progressBarValue, value);
				RaisePropertyChanged(nameof(TaskbarProgress));
				RaisePropertyChanged(nameof(TaskbarState));
				RaisePropertyChanged(nameof(IsProgressIndeterminate));
            }
        }

        public int ProgressBarMaximum
        {
            get => _progressBarMaximum;
            set { Set(() => ProgressBarMaximum, ref _progressBarMaximum, value); }
        }

        public double TaskbarProgress => ((double)ProgressBarValue / ProgressBarMaximum);

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
		    get => _selectedFile;
		    set { Set(() => SelectedFile, ref _selectedFile, value); }
	    }

	    public bool CopySmxToClipboardOnCompile
	    {
		    get => _copySmxToClipboardOnCompile;
		    set => Set(() => CopySmxToClipboardOnCompile, ref _copySmxToClipboardOnCompile, value);
	    }

	    public bool IsWatchingOrBuilding
	    {
		    get => _isWatchingOrBuilding;
		    set
		    {
			    Set(() => IsWatchingOrBuilding, ref _isWatchingOrBuilding, value);
				RaisePropertyChanged(nameof(CanClickWatchButton));
		    }
	    }

	    public bool CanClickWatchButton => !IsWatchingOrBuilding;

	    public RelayCommand AddFileCommand { get; private set; }
		public RelayCommand BrowseCompilerCommand { get; private set; }
		public RelayCommand BuildCommand { get; private set; }
		public RelayCommand RemoveFileCommand { get; private set; }

		public RelayCommand WatchCommand { get; }

		public SourcePawnCompileViewModel()
        {
            SourcePawnCompiler = SettingsManager.Manifest.SourcePawnCompiler.CompilerPath;
            ExecutePostBuildScripts = SettingsManager.Manifest.SourcePawnCompiler.ExecutePostBuildScripts;
            IncrementVersion = SettingsManager.Manifest.SourcePawnCompiler.Versioning;

			AddFileCommand = new RelayCommand(AddFile);
			BrowseCompilerCommand = new RelayCommand(BrowseCompiler);
			RemoveFileCommand = new RelayCommand(RemoveFile);
			BuildCommand = new RelayCommand(Build);
			WatchCommand = new RelayCommand(Watch);
        }

		private async void Build()
	    {
		    AreCommandButtonsEnabled = false;

			await Task.Run(() =>
			{
				var proc = new SourcePawnCompiler();
				proc.CompileBatch(this);
			});

			AreCommandButtonsEnabled = true;

			if (CopySmxToClipboardOnCompile && FilesToCompile.All(x => x.IsSuccessfulCompile || x.IsCompiledWithWarnings))
			{
				var files = FilesToCompile
					.Where(f => f != null && !string.IsNullOrWhiteSpace(f.File))
					.Select(f => Path.ChangeExtension(f.File, ".smx"))
					.ToArray();

				if (files.Any())
				{
					var fileDropList = new StringCollection();

					fileDropList.AddRange(files);

					Clipboard.SetFileDropList(fileDropList);
				}
			}
	    }

		private void Watch()
		{
			if (watcher != null)
			{
				IsWatchingOrBuilding = false;
				watcher.EnableRaisingEvents = false;
				watcher.Dispose();
				watcher = null;
				return;
			}

			if (!FilesToCompile.Any())
			{
				return;
			}

			IsWatchingOrBuilding = true;

			watcher = new FileSystemWatcher(Path.GetDirectoryName(FilesToCompile.First().File), "*.sp")
			{
				IncludeSubdirectories = true
			};

			watcher.Changed += WatcherOnChanged;
			watcher.Created += WatcherOnChanged;
			watcher.Deleted += WatcherOnChanged;
			watcher.Renamed += WatcherOnChanged;
			watcher.EnableRaisingEvents = true;
		}

		private async void WatcherOnChanged(object sender, FileSystemEventArgs e)
		{
			AreCommandButtonsEnabled = false;

			await Task.Run(() =>
			{
				var proc = new SourcePawnCompiler();
				proc.Compile(this, FilesToCompile.First());
			});

			AreCommandButtonsEnabled = true;
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

			if (FilesToCompile.Count > 1)
			{
				CopySmxToClipboardOnCompile = false;
			}
	    }

	    private void RemoveFile()
	    {
			FilesToCompile.Remove(SelectedFile);

			if (FilesToCompile.Count > 1)
			{
				CopySmxToClipboardOnCompile = false;
			}
	    }
    }
}
