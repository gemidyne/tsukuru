using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.SourcePawn.ViewModels;
using Tsukuru.Translator.ViewModels;
using Tsukuru.ViewModels;

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

        public ResourcePackingViewModel ResourcePackingViewModel => _ioc.GetInstance<ResourcePackingViewModel>();

        public static bool IsDesignMode => ViewModelBase.IsInDesignModeStatic;

        public OptionsViewModel OptionsViewModel => _ioc.GetInstance<OptionsViewModel>();

        public TranslatorImportViewModel TranslatorImportViewModel => _ioc.GetInstance<TranslatorImportViewModel>();

        public TranslatorExportViewModel TranslatorExportViewModel => _ioc.GetInstance<TranslatorExportViewModel>();

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

            if (!_ioc.IsRegistered<ResourcePackingViewModel>())
            {
                _ioc.Register<ResourcePackingViewModel>();
            }

            if (!_ioc.IsRegistered<MapCompilerViewModel>())
            {
                _ioc.Register<MapCompilerViewModel>();
            }

            if (!_ioc.IsRegistered<MapCompilerResultsViewModel>())
            {
                _ioc.Register<MapCompilerResultsViewModel>();
            }

            if (!_ioc.IsRegistered<OptionsViewModel>())
            {
                _ioc.Register<OptionsViewModel>();
            }

            if (!_ioc.IsRegistered<TranslatorImportViewModel>())
            {
                _ioc.Register<TranslatorImportViewModel>();
            }

            if (!_ioc.IsRegistered<TranslatorExportViewModel>())
            {
                _ioc.Register<TranslatorExportViewModel>();
            }
        }
    }
}
