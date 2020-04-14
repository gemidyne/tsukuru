using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace Tsukuru.Steam
{
	internal static class SteamHelper
	{
		private static string _steamExePath;

		public static string GetExecutableLocation()
		{
			if (!string.IsNullOrWhiteSpace(_steamExePath))
			{
				return _steamExePath;
			}

			var softwareKey = Registry.CurrentUser.OpenSubKey("Software");
			var valveKey = softwareKey?.OpenSubKey("Valve");
			var steamKey = valveKey?.OpenSubKey("Steam");
			var exePathKey = steamKey?.GetValue("SteamExe");

			_steamExePath = exePathKey as string;

			return _steamExePath;
		}

		public static bool LaunchAppByIdWithMap(int appId, string map)
		{
			string steamPath = GetExecutableLocation();

			if (string.IsNullOrWhiteSpace(steamPath) || !File.Exists(steamPath))
			{
				return false;
			}

			var process = Process.Start(steamPath, $"-applaunch {appId} -dev -console +clear +echo \"[Tsukuru] Loading map: {map}...\" +map \"{map}\"");

			return process?.Id != 0;
		}

		public static bool LaunchAppWithMap(string map)
		{
			int? appId = GameHelper.GetAppId();

			if (!appId.HasValue)
			{
				return false;
			}

			return LaunchAppByIdWithMap(appId.Value, map);
		}
	}
}
