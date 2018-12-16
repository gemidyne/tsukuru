using System.Windows;
using System.Windows.Controls;

namespace Tsukuru.Maps.Compiler
{
    public partial class ExecutionWindow : Window
    {
        public ExecutionWindow()
        {
            InitializeComponent();
        }

        private void tbConsole_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox?.ScrollToEnd();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
