using System;
using System.Windows;
using Sentry;

namespace Tsukuru.NetCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IDisposable _sentry;

        protected override void OnStartup(StartupEventArgs e)
        {
            _sentry = SentrySdk.Init("https://61e2edee88024c7792bf5c43aa0984a4@o385499.ingest.sentry.io/5218363");
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _sentry.Dispose();
            base.OnExit(e);
        }
    }
}
