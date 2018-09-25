using System.Windows;
using System.Windows.Controls;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler
{
    /// <summary>
    /// Interaction logic for MapDetailsControl.xaml
    /// </summary>
    public partial class MapDetailsControl : UserControl
    {
        public MapDetailsControl()
        {
            InitializeComponent();
        }

        private void btnVMFPathChooser_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.VistaOpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Multiselect = false;
            dialog.Filter = "Valve Map File|*.vmf";
            dialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            dialog.Title = "Choose a VMF.";

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var dc = DataContext as MapCompilerViewModel;
            dc.VMFPath = dialog.FileName;
        }
    }
}
