using System;
using System.IO;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
	internal class CopyBspToGameStep : ICompileStep
	{
		public string StepName => "Copy map to game";

		public bool Run(ILogReceiver log)
		{
			string mapsFolder = MapCompileSessionInfo.Instance.GameMapsPath;
			string bspFile = MapCompileSessionInfo.Instance.GeneratedBspFile;

			log.WriteLine("CopyBspToGameStep", $"Copying BSP to the game maps folder at: {mapsFolder}");

			try
			{
				File.Copy(bspFile, Path.Combine(mapsFolder, Path.GetFileName(bspFile)), true);
				return true;
			}
			catch (Exception ex)
			{
				log.WriteLine("CopyBspToGameStep", $"Exception thrown: {ex}");

				return false;
			}
		}
	}
}