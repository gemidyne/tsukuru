using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Tsukuru.SourcePawn.ViewModels;
using Tsukuru.ViewModels;

namespace Tsukuru
{
    internal class ViewModelLocator
    {
        private static readonly SimpleIoc _ioc = SimpleIoc.Default;

        public SourcePawnCompileViewModel SourcePawnCompileViewModel => GetOrRegister<SourcePawnCompileViewModel>();

        public MainWindowViewModel MainWindowViewModel => GetOrRegister<MainWindowViewModel>();

        public static bool IsDesignMode => ViewModelBase.IsInDesignModeStatic;

        private static T GetOrRegister<T>()
            where T : class
        {
            if (!_ioc.IsRegistered<T>())
            {
                _ioc.Register<T>();
            }

            return _ioc.GetInstance<T>();
        }
    }
}
