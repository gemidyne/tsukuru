using System;
using System.IO;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
    internal class CopyBspToGameStep : BaseVProjectStep
    {
        public override string StepName => "Copy map to game";

        public override bool Run(ResultsLogContainer log)
        {
            var bspFile = MapCompileSessionInfo.Instance.GeneratedBspFile;

            if (string.IsNullOrWhiteSpace(VProject))
            {
                log.AppendLine("CopyBspToGameStep", "VProject is not set. Set your VPROJECT environment variable in Windows system environment variables and then restart Tsukuru. It should be the full path to your game directory, for example: A:\\SteamLibrary\\steamapps\\common\\Team Fortress 2\\tf");
                return false;
            }

            var destinationFolder = new DirectoryInfo(Path.Combine(VProject, "maps"));

            if (!destinationFolder.Exists)
            {
                log.AppendLine("CopyBspToGameStep", "Unable to determine game maps folder. Ensure your VProject is the game folder and not the root folder. (Example: \"C:\\SteamApps\\Common\\Team Fortress 2\\tf\")");
                return false;
            }

            log.AppendLine("CopyBspToGameStep", $"Copying BSP to the game maps folder at: {destinationFolder.FullName}");

            if (!bspFile.Exists)
            {
                log.AppendLine("CopyBspToGameStep", $"BSP file does not exist at path: {bspFile.FullName}");
                return false;
            }

            try
            {
                bspFile.CopyTo(Path.Combine(destinationFolder.FullName, bspFile.Name), overwrite: true);
                return true;
            }
            catch (Exception ex)
            {
                log.AppendLine("CopyBspToGameStep", $"Exception thrown: {ex}");
                return false;
            }
        }
    }
}