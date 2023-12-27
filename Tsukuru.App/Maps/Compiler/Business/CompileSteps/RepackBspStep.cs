using System.Diagnostics;
using System.IO;
using System.Threading;
using Humanizer;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps;

internal class RepackBspStep : BaseVProjectStep
{
    public override string StepName => "Repack BSP";

    public override bool Run(ResultsLogContainer log)
    {
        log.AppendLine(nameof(RepackBspStep), "Repacking with compression for better BSP file size...");

        return PerformRepack(log);
    }

    private bool PerformRepack(ResultsLogContainer log)
    {
        long bspSizeBeforeRepack = MapCompileSessionInfo.Instance.GeneratedBspFile.Length;
        log.AppendLine("Info", $"Size before repack: {bspSizeBeforeRepack.Bytes().ToString()}");

        var args = $" -repack -compress \"{MapCompileSessionInfo.Instance.GeneratedBspFile.FullName}\"";

        var bspzip = new FileInfo(Path.Combine(SdkToolsPath, "bin", "bspzip.exe"));

        if (!bspzip.Exists)
        {
            log.AppendLine(nameof(RepackBspStep), $"bspzip.exe not found at path: {bspzip.FullName}");
            return false;
        }

        var startInfo = new ProcessStartInfo(bspzip.FullName, args)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        log.AppendLine("REPACK", "Started repacking. This might take some time.");

        using (var process = Process.Start(startInfo))
        {
            var outputReader = new Thread(() =>
            {
                int ch;

                while ((ch = process.StandardOutput.Read()) >= 0)
                {
                    log.Append((char)ch);
                }
            });

            outputReader.Start();

            process.WaitForExit();

            outputReader.Join();

            log.AppendLine("REPACK", $"BSPZIP exited with code {process.ExitCode}");
        }
        
        long bspSizeAfterRepack = MapCompileSessionInfo.Instance.GeneratedBspFile.Length;
        
        log.AppendLine("Info", $"Size after repack: {bspSizeAfterRepack.Bytes().ToString()}");
        
        log.AppendLine("Info", $"Repacking the BSP reduced file size by {(bspSizeBeforeRepack - bspSizeAfterRepack).Bytes().ToString()}");

        return true;
    }
}