using System.Windows;
using Tsukuru.ViewModels;

namespace Tsukuru
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}
