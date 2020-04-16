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
            var viewModel = SimpleIoc.Default.GetInstance<CompileConfirmationViewModel>();

            if (!File.Exists(viewModel.VMFPath))
            {
                log.WriteLine("PrepareVmf", $"File not found at location: {viewModel.VMFPath}");
                return false;
            }

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
