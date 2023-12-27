using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace Tsukuru.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    protected IMessenger Messenger => WeakReferenceMessenger.Default;

    protected bool IsInDesignMode => App.IsInDesignMode;
}