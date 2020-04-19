using System.Windows.Controls;

namespace Tsukuru.Maps.Compiler.Views
{
    public partial class ResultsView : UserControl
    {
        public ResultsView()
        {
            InitializeComponent();
        }

        private void OnTextBoxConsoleChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox?.ScrollToEnd();
        }
    }
}
