using System.Windows.Controls;

namespace Tsukuru.Maps.Compiler
{
    public partial class MapCompilerResultsView : UserControl
    {
        public MapCompilerResultsView()
        {
            InitializeComponent();
        }

        private void tbConsole_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox?.ScrollToEnd();
        }
    }
}
