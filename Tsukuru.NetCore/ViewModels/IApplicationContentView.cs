namespace Tsukuru.ViewModels
{
    public interface IApplicationContentView
    {
        string Name { get; }

        EShellNavigationPage Group { get; }

        bool IsLoading { get; set; }

        void Init();
    }
}