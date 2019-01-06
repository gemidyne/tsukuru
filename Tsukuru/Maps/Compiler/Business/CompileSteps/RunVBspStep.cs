using GalaSoft.MvvmLight.Ioc;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
	internal class RunVBspStep : ICompileStep
	{
		public string StepName => "VBSP";

		public bool Run(ILogReceiver log)
		{
			var viewModel = SimpleIoc.Default.GetInstance<MapCompilerViewModel>();

			var compileEngine = new SourceCompilationEngine(
				log: log,
				useModifiedVrad: viewModel.VRADSettings.UseModifiedVrad,
				vmfFile: MapCompileSessionInfo.Instance.GeneratedVmfFile);

			MapCompileSessionInfo.Instance.SdkToolsPath = compileEngine.SdkToolsPath;
			MapCompileSessionInfo.Instance.GameMapsPath = compileEngine.GameMapsFolderPath;
			MapCompileSessionInfo.Instance.GeneratedBspFile = compileEngine.VmfPathWithoutExtension + ".bsp";

			return compileEngine.InvokeVBsp(viewModel.VBSPSettings);
		}
	}
}