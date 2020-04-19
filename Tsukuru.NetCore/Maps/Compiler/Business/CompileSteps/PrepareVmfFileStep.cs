using System;
using System.IO;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
    internal class PrepareVmfFileStep : ICompileStep
    {
        public string StepName => "Prepare VMF file for compile";

        public bool Run(ILogReceiver log)
        {
            var input = MapCompileSessionInfo.Instance.InputVmfFile;

            if (!input.Exists)
            {
                log.WriteLine("PrepareVmf", $"File not found at location: {input.FullName}");
                return false;
            }

            string generatedVmfFile;

            try
            {
                generatedVmfFile = DoCopyFile(input, MapCompileSessionInfo.Instance.MapName);
            }
            catch (Exception ex)
            {
                log.WriteLine("PrepareVmf", $"Error thrown when trying to generate VMF file for compile: {ex.Message}");
                return false;
            }

            if (string.IsNullOrWhiteSpace(generatedVmfFile))
            {
                log.WriteLine("PrepareVmf", "Unable to generate VMF file.");

                return false;
            }

            MapCompileSessionInfo.Instance.GeneratedVmfFile = new FileInfo(generatedVmfFile);

            return true;
        }

        private static string DoCopyFile(FileInfo inputVmf, string destinationFileName)
        {
            var destinationFullPath = Path.Combine(inputVmf.DirectoryName, "generated");

            if (!Directory.Exists(destinationFullPath))
            {
                Directory.CreateDirectory(destinationFullPath);
            }

            var destinationFullFile = Path.Combine(destinationFullPath, destinationFileName + ".vmf");

            return inputVmf.CopyTo(destinationFullFile, overwrite: true).FullName;
        }
    }
}
