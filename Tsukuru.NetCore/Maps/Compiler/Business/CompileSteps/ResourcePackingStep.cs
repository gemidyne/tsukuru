using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.Maps.Packer;
using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
    internal class ResourcePackingStep : BaseVProjectStep
    {
        public override string StepName => "Pack resources into BSP";

        public override bool Run(ILogReceiver log)
        {
            var viewModel = SimpleIoc.Default.GetInstance<ResourcePackingViewModel>();

            var session = new PackerSessionDetails
            {
                MapFile = MapCompileSessionInfo.Instance.GeneratedBspFile.FullName,
                GamePath = SdkToolsPath,
                CompleteFoldersToAdd = viewModel.FoldersToPack.Where(x => !x.Intelligent).Select(x => x.Folder).ToList(),
                IntelligentFoldersToAdd = viewModel.FoldersToPack.Where(x => x.Intelligent).Select(x => x.Folder).ToList()
            };

            if (viewModel.FoldersToPack.Select(x => x.Folder).Any(x => !Directory.Exists(x)))
            {
                log.WriteLine("ResourcePackingStep", "Unable to start packing. A directory specified does not exist.");
                return false;
            }

            var packer = new BspPackEngine(log, session);

            if (SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.GenerateMapSpecificFiles)
            {
                using (var mapSpecificFileGenerator = new TemplatingEngine(log, session.CompleteFoldersToAdd, MapCompileSessionInfo.Instance.MapName))
                {
                    mapSpecificFileGenerator.Generate();

                    log.WriteLine("ResourcePackingStep", "Performing resource packing...");
                    packer.PackData();
                }
            }
            else
            {
                log.WriteLine("ResourcePackingStep", "Performing resource packing...");
                packer.PackData();
            }

            if (SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.PerformRepackCompress)
            {
                log.WriteLine("ResourcePackingStep", "Repacking with compression for better BSP file size...");
                packer.RepackCompressBsp();
            }

            return true;
        }
    }
}