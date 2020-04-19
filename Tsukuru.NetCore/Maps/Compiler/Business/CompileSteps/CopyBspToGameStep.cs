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

            log.AppendLine("CopyBspToGameStep", $"Copying BSP to the game maps folder at: {GameMapsFolderPath}");

            try
            {
                bspFile.CopyTo(Path.Combine(GameMapsFolderPath, bspFile.Name), overwrite: true);
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