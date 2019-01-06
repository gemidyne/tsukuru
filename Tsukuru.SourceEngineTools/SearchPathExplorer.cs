using System.IO;
using System.Linq;

namespace Tsukuru.SourceEngineTools
{
	internal class SearchPathExplorer
	{
		public string[] Paths { get; }

		public SearchPathExplorer(string gameDirectory)
		{
			Paths = Directory.GetDirectories(Path.Combine(gameDirectory, "custom"));
			Paths = Paths.Union(new [] { gameDirectory }).ToArray();
		}

		public string GetFileSystemPath(string gameAssetPath)
		{
			foreach (string path in Paths)
			{
				string expectedAssetPath = Path.Combine(path, gameAssetPath);

				if (File.Exists(expectedAssetPath))
				{
					return expectedAssetPath;
				}
			}

			return null;
		}
	}
}
