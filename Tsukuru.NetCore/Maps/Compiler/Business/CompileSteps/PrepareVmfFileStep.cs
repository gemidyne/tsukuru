using System;
using System.IO;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps;

internal class PrepareVmfFileStep : ICompileStep
{
    public string StepName => "Prepare VMF file for compile";

    public bool Run(ResultsLogContainer log)
    {
        var input = MapCompileSessionInfo.Instance.InputVmfFile;

        if (!input.Exists)
        {
            log.AppendLine("PrepareVmf", $"File not found at location: {input.FullName}");
            return false;
        }

        string generatedVmfFile;

        try
        {
            generatedVmfFile = DoCopyFile(log, input, MapCompileSessionInfo.Instance.MapName);
        }
        catch (Exception ex)
        {
            log.AppendLine("PrepareVmf", $"Error thrown when trying to generate VMF file for compile: {ex.Message}");
            return false;
        }

        if (string.IsNullOrWhiteSpace(generatedVmfFile))
        {
            log.AppendLine("PrepareVmf", "Unable to generate VMF file.");
            return false;
        }

        MapCompileSessionInfo.Instance.GeneratedVmfFile = new FileInfo(generatedVmfFile);

        log.AppendLine("PrepareVmf", $"Copied generated VMF file to {MapCompileSessionInfo.Instance.GeneratedVmfFile.FullName}");

        return true;
    }

    private static string DoCopyFile(ResultsLogContainer log, FileInfo inputVmf, string destinationFileName)
    {
        var destinationFullPath = Path.Combine(inputVmf.DirectoryName, "generated");

        if (!Directory.Exists(destinationFullPath))
        {
            Directory.CreateDirectory(destinationFullPath);

            log.AppendLine("PrepareVmf", $"Created generated directory at {destinationFullPath}");
        }

        var destinationFullFile = Path.Combine(destinationFullPath, destinationFileName + ".vmf");

        return inputVmf.CopyTo(destinationFullFile, overwrite: true).FullName;
    }
}