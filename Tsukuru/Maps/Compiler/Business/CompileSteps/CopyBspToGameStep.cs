using System;
using System.IO;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
    internal class CopyBspToGameStep : BaseVProjectStep
    {
        public override string StepName => "Copy map to game";

        public override bool Run(ILogReceiver log)
        {
            var bspFile = MapCompileSessionInfo.Instance.GeneratedBspFile;

            log.WriteLine("CopyBspToGameStep", $"Copying BSP to the game maps folder at: {GameMapsFolderPath}");

            try
            {
                bspFile.CopyTo(Path.Combine(GameMapsFolderPath, bspFile.Name), overwrite: true);
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