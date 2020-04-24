using System.Collections.Generic;
using System.IO;
using System.Linq;
using SteamDatabase.ValvePak;

namespace Tsukuru.Core.SourceEngine
{
    internal class SearchPathExplorer
    {
        private readonly IEnumerable<FileInfo> _vpkPaths;
        private readonly Dictionary<string, List<string>> _filesPerVpk = new Dictionary<string, List<string>>();

        public string[] Paths { get; }

        public SearchPathExplorer(string gameDirectory)
        {
            Paths = Directory.GetDirectories(Path.Combine(gameDirectory, "custom"));
            Paths = Paths.Union(new[] { gameDirectory }).ToArray();

            _vpkPaths = GameInfoHelper.GetAllVpkPaths();
        }

        public void Init()
        {
            PreloadVpkEntries();
        }

        /// <summary>
        /// Tries to get a game asset from the file system. If it does not exist or exists within a VPK, it will return null.
        /// </summary>
        /// <param name="gameAssetPath"></param>
        /// <returns></returns>
        public string TryGetFileSystemPath(string gameAssetPath)
        {
            if (IsFileInVpk(gameAssetPath))
            {
                // If in VPK, does not exist on file system then. VPKs are used first in engine before filesystem directories
                return null;
            }

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

        private bool IsFileInVpk(string gameAssetPath)
        {
            foreach (var vpk in _filesPerVpk)
            {
                var exists = vpk.Value.Any(x => string.Equals(x, gameAssetPath));

                if (exists)
                {
                    return true;
                }
            }

            return false;
        }

        private void PreloadVpkEntries()
        {
            foreach (var vpk in _vpkPaths)
            {
                using (var stream = vpk.OpenRead())
                using (var package = new Package())
                {
                    package.SetFileName(vpk.Name);
                    package.Read(stream);

                    var entries = package.Entries.Values.SelectMany(x => x).Select(x => x.GetFullPath()).ToList();

                    _filesPerVpk[vpk.Name] = entries;
                }
            }
        }
    }
}
