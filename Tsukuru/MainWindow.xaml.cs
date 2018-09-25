using System.Windows;
using Tsukuru.SourcePawn.ViewModels;

namespace Tsukuru
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new SourcePawnCompileViewModel();
        }
    }
}
