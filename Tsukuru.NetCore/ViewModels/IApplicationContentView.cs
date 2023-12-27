namespace Tsukuru.ViewModels;

public interface IApplicationContentView
{
    string Name { get; }

    string Description { get; }

    EShellNavigationPage Group { get; }

    bool IsLoading { get; set; }

    void Init();
}