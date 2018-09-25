using System.Windows;
using System.Windows.Controls;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler
{
    /// <summary>
    /// Interaction logic for ResourcePackingControl.xaml
    /// </summary>
    public partial class ResourcePackingControl : UserControl
    {
        public ResourcePackingControl()
        {
            InitializeComponent();
        }

        private void btnFoldersToPackAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.VistaFolderBrowserDialog();
            dialog.Description = "Choose a folder to pack into the BSP.";

            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            var dc = DataContext as MapCompilerViewModel;

            if (!dc.FoldersToPack.Contains(dialog.SelectedPath))
            {
                dc.FoldersToPack.Add(dialog.SelectedPath);
            }
        }

        private void btnFoldersToPackRemove_Click(object sender, RoutedEventArgs e)
        {
            var dc = DataContext as MapCompilerViewModel;

            var text = lbFoldersToPack.SelectedItem as string;
            dc.FoldersToPack.Remove(text);
        }
    }
}
