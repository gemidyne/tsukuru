using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Tsukuru.SourcePawn.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _sourcePawnCompileSelected = true;
        private bool _mapCompilerSelected;

        public bool SourcePawnCompileSelected
        {
            get => _sourcePawnCompileSelected;
            set { Set(() => SourcePawnCompileSelected, ref _sourcePawnCompileSelected, value); }
        }

        public bool MapCompilerSelected
        {
            get => _mapCompilerSelected;
            set { Set(() => MapCompilerSelected, ref _mapCompilerSelected, value); }
        }

        public RelayCommand SelectSourcePawnCompilerCommand { get; private set; }

        public RelayCommand SelectMapCompilerCommand { get; private set; }

        public MainWindowViewModel()
        {
            SelectMapCompilerCommand = new RelayCommand(SelectMapCompiler);
            SelectSourcePawnCompilerCommand = new RelayCommand(SelectSourcePawnCompiler);
        }

        private void SelectSourcePawnCompiler()
        {
            SourcePawnCompileSelected = true;
            MapCompilerSelected = false;
        }

        private void SelectMapCompiler()
        {
            SourcePawnCompileSelected = false;
            MapCompilerSelected = true;
        }
    }
}