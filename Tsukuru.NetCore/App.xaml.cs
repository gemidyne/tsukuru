using System;
using System.Windows;
using System.Windows.Threading;
using Sentry;
using MessageBox = AdonisUI.Controls.MessageBox;
using MessageBoxButton = AdonisUI.Controls.MessageBoxButton;
using MessageBoxImage = AdonisUI.Controls.MessageBoxImage;

namespace Tsukuru.NetCore;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IDisposable _sentry;

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
}