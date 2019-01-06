using System.IO;
using System.Linq;
using GalaSoft.MvvmLight;
using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler.ViewModels
{
	public class ResourceFolderViewModel : ViewModelBase
	{
		private string _folder;
		private bool _intelligent;

		public string Folder
		{
			get => _folder;
			set
			{
				Set(() => Folder, ref _folder, value);
				RaisePropertyChanged(nameof(File));
			}
		}

		public bool Intelligent
		{
			get => _intelligent;
			set
			{
				Set(() => Intelligent, ref _intelligent, value);
				RaisePropertyChanged(nameof(File));

				var folder = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.SingleOrDefault(f => f.Path == Folder);

				if (folder == null)
				{
					return;
				}

				folder.Intelligent = value;
				SettingsManager.Save();
			}
		}

		public string File => GetHeading();

		private string GetHeading()
		{
			string packType = Intelligent
				? "(Pack mode: Only files which are used)"
				: "(Pack mode: All files in folder and subfolders)";

			var directoryInfo = new DirectoryInfo(Folder);

			return $"{directoryInfo.Name} - {packType}";
		}
	}
}
