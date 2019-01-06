using GalaSoft.MvvmLight.Ioc;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
	internal class RunVRadStep : ICompileStep
	{
		public string StepName => "VRAD";

		public bool Run(ILogReceiver log)
		{
			var viewModel = SimpleIoc.Default.GetInstance<MapCompilerViewModel>();

			var compileEngine = new SourceCompilationEngine(
				log: log,
				useModifiedVrad: viewModel.VRADSettings.UseModifiedVrad,
				vmfFile: MapCompileSessionInfo.Instance.GeneratedVmfFile);

			return compileEngine.InvokeVRad(viewModel.VRADSettings);
		}
	}
}