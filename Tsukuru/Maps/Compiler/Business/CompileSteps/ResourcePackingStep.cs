using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.Maps.Packer;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
	internal class ResourcePackingStep : ICompileStep
	{
		public string StepName => "Pack resources into BSP";

		public bool Run(ILogReceiver log)
		{
			var viewModel = SimpleIoc.Default.GetInstance<ResourcePackingViewModel>();

			var session = new PackerSessionDetails
			{
				MapFile = MapCompileSessionInfo.Instance.GeneratedBspFile, 
				GamePath = MapCompileSessionInfo.Instance.SdkToolsPath,
				CompleteFoldersToAdd = viewModel.FoldersToPack.Where(x => !x.Intelligent).Select(x => x.Folder).ToList(),
				IntelligentFoldersToAdd = viewModel.FoldersToPack.Where(x => x.Intelligent).Select(x => x.Folder).ToList()
			};

			var packer = new BspPackEngine(log, session);

			if (viewModel.GenerateMapSpecificFiles)
			{
				using (var mapSpecificFileGenerator = new MapSpecificFileGenerator(session.CompleteFoldersToAdd))
				{
					log.WriteLine("ResourcePackingStep", "Scanning and generating map specific files...");
					mapSpecificFileGenerator.Generate(mapName: MapCompileSessionInfo.Instance.MapName);

					log.WriteLine("ResourcePackingStep", "Performing resource packing...");
					packer.PackData();
				}
			}
			else
			{
				log.WriteLine("ResourcePackingStep", "Performing resource packing...");
				packer.PackData();
			}

			if (viewModel.PerformRepack)
			{
				log.WriteLine("ResourcePackingStep", "Repacking with compression for better BSP file size...");
				packer.RepackCompressBsp();
			}

			return true;
		}
	}
}