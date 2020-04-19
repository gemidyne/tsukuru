using Chiaki;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.Steam;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
    internal class LaunchMapInGameStep : ICompileStep
    {
        public string StepName => "Launch map in game";

        public bool Run(ResultsLogContainer log)
        {
            log.AppendLine("LaunchMapInGameStep", "Launching game and loading map...");

            return SteamHelper.LaunchAppWithMap(MapCompileSessionInfo.Instance.GeneratedBspFile.Name.TrimEnd(".bsp"));
        }
    }
}