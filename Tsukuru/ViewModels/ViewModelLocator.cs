using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Tsukuru.ViewModels
{
	internal class ViewModelLocator
	{
		private readonly SimpleIoc _ioc;

		public MainWindowViewModel MainWindowViewModel
		{
			get { return _ioc.GetInstance<MainWindowViewModel>(); }
		}

		public ResultsWindowViewModel ResultsWindowViewModel
		{
			get { return _ioc.GetInstance<ResultsWindowViewModel>(); }
		}

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

			if (!_ioc.IsRegistered<ResultsWindowViewModel>())
			{
				_ioc.Register<ResultsWindowViewModel>();
			}
		}
	}
}
