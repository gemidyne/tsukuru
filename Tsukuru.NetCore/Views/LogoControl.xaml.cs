using System.Diagnostics;
using System.Windows.Controls;

namespace Tsukuru.Views
{
	public partial class LogoControl : UserControl
    {
        public LogoControl()
        {
            InitializeComponent();
        }

		private void Image_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Process.Start("https://www.gemidyne.com/");
		}
	}
}
