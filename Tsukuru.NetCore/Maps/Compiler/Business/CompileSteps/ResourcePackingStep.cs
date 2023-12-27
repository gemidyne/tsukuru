using System.IO;
using System.Linq;
using Chiaki;
using Humanizer;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.Maps.Packer;
using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps;

internal class ResourcePackingStep : BaseVProjectStep
{
    private readonly ISettingsManager _settingsManager;
    public override string StepName => "Pack resources into BSP";

    public ResourcePackingStep(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public override bool Run(ResultsLogContainer log)
    {
        var folders = _settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.Folders.IfNullThenEmpty().ToArray();

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

        long sizeBeforePacking = MapCompileSessionInfo.Instance.GeneratedBspFile.Length;

        var packer = new BspPackEngine(log, session);

        if (_settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.GenerateMapSpecificFiles)
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

        long sizeAfterPacking = MapCompileSessionInfo.Instance.GeneratedBspFile.Length;
        
        log.AppendLine("Info", $"BSP Size before resource packing: {sizeBeforePacking.Bytes().ToString()}");
        log.AppendLine("Info", $"BSP Size after resource packing: {sizeAfterPacking.Bytes().ToString()}");
        log.AppendLine("Info", $"Packed resource size: {(sizeAfterPacking - sizeBeforePacking).Bytes().ToString()}");

        return true;
    }
}