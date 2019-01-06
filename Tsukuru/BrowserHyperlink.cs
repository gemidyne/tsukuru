using System.Diagnostics;
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
			Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}