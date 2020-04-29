using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace Tsukuru
{
    public class BrowserHyperlink : Hyperlink
    {
        public BrowserHyperlink()
        {
            RequestNavigate += OnRequestNavigate;
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            string url = e.Uri.AbsoluteUri;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }

            e.Handled = true;
        }
    }
}