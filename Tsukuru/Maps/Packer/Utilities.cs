using System.IO;
using System.Windows.Forms;

namespace Gemini.ResourcePacker
{
    public static class Utilities
    {
        public static string ShowOpenFileDialog(string title, string filter = "")
        {
            var fileChooser = new OpenFileDialog();
            fileChooser.CheckFileExists = true;
            fileChooser.CheckPathExists = true;
            fileChooser.Multiselect = false;
            fileChooser.ShowHelp = false;
            fileChooser.Filter = filter;
            fileChooser.InitialDirectory = Directory.GetCurrentDirectory();
            fileChooser.Title = title;

            while (fileChooser.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("Please select a config file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return fileChooser.FileName;
        }

    }
}
