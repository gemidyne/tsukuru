using System.Diagnostics;
using System.IO;
using System.Threading;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
    internal class RepackBspStep : BaseVProjectStep
    {
        public override string StepName => "Repack BSP";

        public override bool Run(ResultsLogContainer log)
        {
            log.AppendLine(nameof(RepackBspStep), "Repacking with compression for better BSP file size...");
            PerformRepack(log);

            return true;
        }

        private void PerformRepack(ResultsLogContainer log)
        {
            var args = $" -repack -compress \"{MapCompileSessionInfo.Instance.GeneratedBspFile.FullName}\"";

            var startInfo = new ProcessStartInfo(Path.Combine(SdkToolsPath, "bin", "bspzip.exe"), args)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            log.AppendLine("REPACK", "Started repacking...");

            using (var process = Process.Start(startInfo))
            {
                var outputReader = new Thread(() =>
                {
                    int ch;

                    while ((ch = process.StandardOutput.Read()) >= 0)
                    {
                        log.Append(((char)ch));
                    }
                });

                outputReader.Start();

                process.WaitForExit();

                outputReader.Join();

                log.AppendLine("REPACK", $"BSPZIP exited with code {process.ExitCode}");
            }
        }
    }
}