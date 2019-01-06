using System.IO;
using Tsukuru.Steam;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
	internal class LaunchMapInGameStep : ICompileStep
	{
		public string StepName => "Launch map in game";

		public bool Run(ILogReceiver log)
		{
			log.WriteLine("LaunchMapInGameStep", "Launching game and loading map...");
			SteamHelper.LaunchAppWithMap(Path.GetFileNameWithoutExtension(MapCompileSessionInfo.Instance.GeneratedBspFile));

			return true;
		}
	}
}