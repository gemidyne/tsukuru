using System;

namespace Tsukuru.ViewModels;

public class ApplicationContentViewLoader : IDisposable
{
    private readonly IApplicationContentView _view;

    public ApplicationContentViewLoader(IApplicationContentView view)
    {
        _view = view;

        _view.IsLoading = true;
        _view.Init();
    }

    public void Dispose()
    {
        _view.IsLoading = false;
    }
}