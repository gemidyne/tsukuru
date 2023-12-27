using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Tsukuru.SourcePawn.ViewModels;
using Tsukuru.ViewModels;

namespace Tsukuru;

internal class ViewModelLocator
{
    private static readonly Ioc _ioc;
    
    public SourcePawnCompileViewModel SourcePawnCompileViewModel => _ioc.GetRequiredService<SourcePawnCompileViewModel>();

    public MainWindowViewModel MainWindowViewModel => _ioc.GetRequiredService<MainWindowViewModel>();

    public static bool IsDesignMode => false;

    static ViewModelLocator()
    {
        _ioc = Ioc.Default;
        
        _ioc.ConfigureServices(
            new ServiceCollection()
                .AddSingleton<SourcePawnCompileViewModel>()
                .AddSingleton<MainWindowViewModel>()
                .BuildServiceProvider());
    }
}