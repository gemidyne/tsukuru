using System.IO;
using System.Linq;
using Chiaki;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.Maps.Packer;
using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
    internal class ResourcePackingStep : BaseVProjectStep
    {
        public override string StepName => "Pack resources into BSP";

        public override bool Run(ResultsLogContainer log)
        {
            var folders = SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.IfNullThenEmpty().ToArray();

            var session = new PackerSessionDetails
            {
                MapFile = MapCompileSessionInfo.Instance.GeneratedBspFile.FullName,
                GamePath = SdkToolsPath,
                CompleteFoldersToAdd = folders.Where(x => !x.Intelligent).Select(x => x.Path).ToList(),
                IntelligentFoldersToAdd =
                    folders.Where(x => x.Intelligent).Select(x => x.Path).ToList()
            };

            if (folders.Select(x => x.Path).Any(x => !Directory.Exists(x)))
            {
                log.AppendLine("ResourcePackingStep", "Unable to start packing. A directory specified does not exist.");
                return false;
            }

            var packer = new BspPackEngine(log, session);

            if (SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.GenerateMapSpecificFiles)
            {
                var allFolders = session.CompleteFoldersToAdd.Concat(session.IntelligentFoldersToAdd).ToList();

                using (var engine = new TemplatingEngine(log, allFolders, MapCompileSessionInfo.Instance.MapName))
                {
                    engine.Generate();

                    log.AppendLine("ResourcePackingStep", "Performing resource packing...");
                    packer.PackData();
                }
            }
            else
            {
                log.AppendLine("ResourcePackingStep", "Performing resource packing...");
                packer.PackData();
            }

            return true;
        }
    }
}