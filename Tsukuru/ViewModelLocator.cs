using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.SourcePawn.ViewModels;

namespace Tsukuru
{
    internal class ViewModelLocator
	{
		private readonly SimpleIoc _ioc;

		public SourcePawnCompileViewModel SourcePawnCompileViewModel => _ioc.GetInstance<SourcePawnCompileViewModel>();

	    public ResultsWindowViewModel ResultsWindowViewModel => _ioc.GetInstance<ResultsWindowViewModel>();

	    public MainWindowViewModel MainWindowViewModel => _ioc.GetInstance<MainWindowViewModel>();

	    public MapCompilerViewModel MapCompilerViewModel => _ioc.GetInstance<MapCompilerViewModel>();

	    public MapCompilerResultsViewModel MapCompilerResultsViewModel =>
	        _ioc.GetInstance<MapCompilerResultsViewModel>();

	    public static bool IsDesignMode => ViewModelBase.IsInDesignModeStatic;

        public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

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

		    if (!_ioc.IsRegistered<MapCompilerViewModel>())
		    {
                _ioc.Register<MapCompilerViewModel>();
		    }

		    if (!_ioc.IsRegistered<MapCompilerResultsViewModel>())
		    {
                _ioc.Register<MapCompilerResultsViewModel>();
		    }
		}
	}
}
