using System.IO;
using SteamKit2;
using Tsukuru.Core.SourceEngine;

namespace Tsukuru.Steam
{
    internal static class GameHelper
    {
        private static KeyValue _gameInfoKeyValues;

        public static int? GetAppId()
        {
            var gameInfo = TryGetGameInfo();

            if (gameInfo == null)
            {
                return null;
            }

            int appId = gameInfo["FileSystem"]["SteamAppId"].AsInteger();

            return appId;
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

        private static KeyValue TryGetGameInfo()
        {
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