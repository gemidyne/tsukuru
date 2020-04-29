using GalaSoft.MvvmLight;

namespace Tsukuru.ViewModels
{
    public class AboutViewModel : ViewModelBase, IApplicationContentView
    {
        private bool _isLoading;

        public string Name => "About Tsukuru";

        public string Description => "Learn more about Tsukuru";

        public EShellNavigationPage Group => EShellNavigationPage.Meta;

        public bool IsLoading
        {
            get => _isLoading;
            set => Set(() => IsLoading, ref _isLoading, value);
        }

        public void Init()
        {
        }
    }
}