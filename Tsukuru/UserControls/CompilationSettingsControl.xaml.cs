using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Tsukuru.ViewModels;

namespace Tsukuru.UserControls
{
    public partial class CompilationSettingsControl : UserControl
    {
        public CompilationSettingsControl()
        {
            InitializeComponent();
        }

        private void btnSourcePawnCompilerBrowse_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnBuild_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainWindowViewModel;


        }
    }
}
