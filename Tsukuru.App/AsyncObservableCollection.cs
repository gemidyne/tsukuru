using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Tsukuru;

public class AsyncObservableCollection<T> : ObservableCollection<T>
{
    private AsyncOperation _asyncOp = null;

    public AsyncObservableCollection()
    {
        CreateAsyncOp();
    }

    public AsyncObservableCollection(IEnumerable<T> list)
        : base(list)
    {
        CreateAsyncOp();
    }

    private void CreateAsyncOp()
    {
        _asyncOp = AsyncOperationManager.CreateOperation(null);
    }

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        _asyncOp.Post(RaiseCollectionChanged, e);
    }

    private void RaiseCollectionChanged(object param)
    {
        base.OnCollectionChanged((NotifyCollectionChangedEventArgs)param);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        _asyncOp.Post(RaisePropertyChanged, e);
    }

    private void RaisePropertyChanged(object param)
    {
        base.OnPropertyChanged((PropertyChangedEventArgs)param);
    }
}