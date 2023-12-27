using Microsoft.Extensions.DependencyInjection;
using Tsukuru.ViewModels;

namespace Tsukuru;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a page viewmodel to the dependency injection container, and makes the page viewmodel available on the Main Window
    /// </summary>
    public static IServiceCollection AddAppPage<T>(this IServiceCollection services)
        where T : class, IApplicationContentView
        => services.AddSingleton<IApplicationContentView, T>();
}