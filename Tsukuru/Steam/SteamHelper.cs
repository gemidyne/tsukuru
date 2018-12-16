using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Tsukuru.Maps.Compiler;

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
			var appIdInfo = new FileInfo($"{SourceCompilationEngine.VProject}\\..\\steam_appid.txt");

			if (!appIdInfo.Exists)
			{
				return false;
			}

			string text = File.ReadAllText(appIdInfo.FullName).Replace("\r", string.Empty).Replace("\n", string.Empty).Trim();
			int appId;

			if (string.IsNullOrWhiteSpace(text) || !int.TryParse(text, out appId))
			{
				return false;
			}

			return LaunchAppByIdWithMap(appId, map);
		}
	}
}
