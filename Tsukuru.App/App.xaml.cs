using System;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Sentry;
using Tsukuru.Core.Translations;
using Tsukuru.Maps.Compiler;
using Tsukuru.Schemas.Translations;
using Tsukuru.Services;
using Tsukuru.Settings;
using Tsukuru.ViewModels;
using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxButton = AdonisUI.Controls.MessageBoxButton;
using MessageBoxImage = AdonisUI.Controls.MessageBoxImage;

namespace Tsukuru;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IDisposable _sentry;

    public static IServiceProvider Services => Ioc.Default;

    public static bool IsInDesignMode => false;

    public App()
    {
        var ioc = Ioc.Default;
        
        var services = new ServiceCollection().AddSingleton<MainWindowViewModel>();

        // Register all app pages 
        RegisterPages(services);
        
        // Register other services
        services
            .AddSingleton<ISettingsManager, SettingsManager>()
            .AddSingleton<IAppUpdateProvider, AppUpdateProvider>()
            .AddTransient<ITranslationProjectSerializer, TranslationProjectSerializer>()
            .AddTransient<ITranslatorEngine, TranslatorEngine>()
            .AddTransient<IMapCompiler, MapCompiler>();

        ioc.ConfigureServices(services.BuildServiceProvider());
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        _sentry = SentrySdk.Init("https://61e2edee88024c7792bf5c43aa0984a4@o385499.ingest.sentry.io/5218363");

        DispatcherUnhandledException += OnDispatcherUnhandledException;

        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        
        base.OnStartup(e);
    }

    private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        HandleException(e.ExceptionObject as Exception);
    }

    private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        e.Handled = true;
        HandleException(e.Exception);
    }

    private static void HandleException(Exception ex)
    {
        if (ex == null)
        {
            MessageBox.Show(
                text:
                $"An error has been encountered in Tsukuru. This error could not be submitted to gemidyne. The error object was null.",
                caption: "Tsukuru Error",
                buttons: MessageBoxButton.OK,
                icon: MessageBoxImage.Error);
            return;
        }

        SentrySdk.CaptureException(ex);

        MessageBox.Show(
            text:
            $"An error has been encountered in Tsukuru. This error has been submitted to gemidyne. Here are the details: \n\n\n{ex}",
            caption: "Tsukuru Error",
            buttons: MessageBoxButton.OK,
            icon: MessageBoxImage.Error);
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _sentry.Dispose();
        base.OnExit(e);
    }
    
    private static IServiceCollection RegisterPages(IServiceCollection services) =>
        services
            .AddAppPage<SourcePawn.ViewModels.SettingsViewModel>()
            .AddAppPage<SourcePawn.ViewModels.PostBuildActionsViewModel>()
            .AddAppPage<SourcePawn.ViewModels.SourcePawnCompileViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.ImportSettingsViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.ExportSettingsViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.GameInfoViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.MapSettingsViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.VbspCompilationSettingsViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.VvisCompilationSettingsViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.VradCompilationSettingsViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.ResourcePackingViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.TemplatingSettingsViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.BspRepackViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.PostCompileActionsViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.CompileConfirmationViewModel>()
            .AddAppPage<Maps.Compiler.ViewModels.ResultsViewModel>()
            .AddAppPage<Translator.ViewModels.TranslatorImportViewModel>()
            .AddAppPage<Translator.ViewModels.TranslatorExportViewModel>()
            .AddAppPage<AboutViewModel>()
            .AddAppPage<OptionsViewModel>();
}