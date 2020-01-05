using System.IO;
using GalaSoft.MvvmLight.Ioc;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
    internal class PrepareVmfFileStep : ICompileStep
    {
        public string StepName => "Prepare VMF file for compile";

        public bool Run(ILogReceiver log)
        {
            var viewModel = SimpleIoc.Default.GetInstance<MapCompilerViewModel>();

            var generatedVmfFile = VmfFileCopier.CopyFile(viewModel.VMFPath, viewModel.MapName);

            if (string.IsNullOrWhiteSpace(generatedVmfFile))
            {
                return false;
            }

            MapCompileSessionInfo.Instance.GeneratedVmfFile = new FileInfo(generatedVmfFile);

            return true;
        }
    }
}
