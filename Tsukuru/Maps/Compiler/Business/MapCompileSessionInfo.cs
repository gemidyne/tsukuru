namespace Tsukuru.Maps.Compiler.Business
{
	public class MapCompileSessionInfo
	{
		private static MapCompileSessionInfo _instance;

		public string MapName { get; set; }

		public string GeneratedVmfFile { get; set; }

		public string GeneratedBspFile { get; set; }

		public string SdkToolsPath { get; set; }

		public string GameMapsPath { get; set; }

		public static MapCompileSessionInfo Instance => _instance ?? (_instance = new MapCompileSessionInfo());

		public static void Clear()
		{
			_instance = new MapCompileSessionInfo();
		}
	}
}
