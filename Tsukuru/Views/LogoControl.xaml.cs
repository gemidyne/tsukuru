using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Tsukuru.Views
{
    public partial class LogoControl : UserControl
    {
        public LogoControl()
        {
            InitializeComponent();
        }

        private void OnChipClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://gemini.software/");
        }
    }
}
