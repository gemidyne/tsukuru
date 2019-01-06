using System.Globalization;
using System.IO;
using Tsukuru.Maps.Compiler;
using ValveKeyValue;

namespace Tsukuru.Steam
{
	internal static class GameHelper
	{
		private static KVObject _gameInfoKeyValues;

		public static int? GetAppId()
		{
			var gameInfo = TryGetGameInfo();

			if (gameInfo == null)
			{
				return null;
			}

			int appId = gameInfo["FileSystem"]["SteamAppId"].ToInt32(CultureInfo.InvariantCulture);

			return appId;
		}

		public static string GetGameInfo()
		{
			var gameInfo = TryGetGameInfo();

			if (gameInfo == null)
			{
				return null;
			}

			return $"{gameInfo["game"]} (App ID {GetAppId()})";
		}

		private static KVObject TryGetGameInfo()
		{
			if (_gameInfoKeyValues != null)
			{
				return _gameInfoKeyValues;
			}

			var file = new FileInfo($"{SourceCompilationEngine.VProject}\\gameinfo.txt");

			if (!file.Exists)
			{
				return null;
			}

			var serialiser = KVSerializer.Create(KVSerializationFormat.KeyValues1Text);

			using (var stream = file.OpenRead())
				_gameInfoKeyValues = serialiser.Deserialize(stream, KVSerializerOptions.DefaultOptions);

			return _gameInfoKeyValues;
		}
	}
}