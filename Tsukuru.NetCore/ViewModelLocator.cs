using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Tsukuru.SourcePawn.ViewModels;
using Tsukuru.ViewModels;

namespace Tsukuru
{
    internal class ViewModelLocator
    {
        private readonly SimpleIoc _ioc;

        public SourcePawnCompileViewModel SourcePawnCompileViewModel => _ioc.GetInstance<SourcePawnCompileViewModel>();

        public ResultsWindowViewModel ResultsWindowViewModel => _ioc.GetInstance<ResultsWindowViewModel>();

        public MainWindowViewModel MainWindowViewModel => _ioc.GetInstance<MainWindowViewModel>();

        public static bool IsDesignMode => ViewModelBase.IsInDesignModeStatic;

        public ViewModelLocator()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {

            }
            else
            {
                _ioc = SimpleIoc.Default;

                RegisterViewModels();
            }
        }

        private void RegisterViewModels()
        {
            if (!_ioc.IsRegistered<MainWindowViewModel>())
            {
                _ioc.Register<MainWindowViewModel>();
            }

            if (!_ioc.IsRegistered<SourcePawnCompileViewModel>())
            {
                _ioc.Register<SourcePawnCompileViewModel>();
            }

            if (!_ioc.IsRegistered<ResultsWindowViewModel>())
            {
                _ioc.Register<ResultsWindowViewModel>();
            }
        }
    }
}
