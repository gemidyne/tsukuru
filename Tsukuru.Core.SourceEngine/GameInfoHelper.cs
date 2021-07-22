using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chiaki;
using SteamKit2;

namespace Tsukuru.Core.SourceEngine
{
    public static class GameInfoHelper
    {
        private static KeyValue _gameInfoKeyValues;

        public static int? GetAppId()
        {
            var gameInfo = TryGetGameInfo();

            if (gameInfo == null)
            {
                return null;
            }

            try
            {
                int appId = gameInfo["FileSystem"]["SteamAppId"].AsInteger();

                return appId;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetGameInfo()
        {
            var gameInfo = TryGetGameInfo();

            if (gameInfo == null)
            {
                return null;
            }

            return $"{gameInfo["game"].Value}";
        }

        public static IEnumerable<FileInfo> GetAllVpkPaths()
        {
            var gameInfo = TryGetGameInfo();

            if (gameInfo == null)
            {
                yield break;
            }

            var gameDir = VProjectHelper.Path.Substring(VProjectHelper.Path.Replace('\\', '/').LastIndexOf('/')).Trim('/', '\\');

            var kv = gameInfo["FileSystem"]["SearchPaths"];

            var paths = kv.Children
                .Select(x => x.Value)
                .Where(x => !string.IsNullOrWhiteSpace(x) && x.EndsWith(".vpk", StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Replace("|all_source_engine_paths|", "../"))
                .Select(x => x.TrimStart(gameDir).TrimStart("/"))
                .Select(x => Path.Combine(VProjectHelper.Path, x))
                .Select(x => new FileInfo(x))
                .ToList();

            foreach (FileInfo fileInfo in paths)
            {
                var directoryVpk = new FileInfo(fileInfo.FullName.Replace(".vpk", "_dir.vpk"));

                if (directoryVpk.Exists)
                {
                    yield return directoryVpk;
                }
                else if (fileInfo.Exists)
                {
                    yield return directoryVpk;
                }
            }
        }

        private static KeyValue TryGetGameInfo()
        {
            if (string.IsNullOrWhiteSpace(VProjectHelper.Path))
            {
                return null;
            }

            if (_gameInfoKeyValues != null)
            {
                return _gameInfoKeyValues;
            }

            var file = new FileInfo(Path.Combine(VProjectHelper.Path, "gameinfo.txt"));

            if (!file.Exists)
            {
                return null;
            }

            _gameInfoKeyValues = KeyValue.LoadAsText(file.FullName);

            return _gameInfoKeyValues;
        }
    }
}