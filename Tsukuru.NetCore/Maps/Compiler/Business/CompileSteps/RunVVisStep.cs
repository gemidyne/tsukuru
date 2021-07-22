using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Chiaki;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
    internal class RunVVisStep : BaseVProjectStep
    {
        private FileInfo _executable;

        public override string StepName => "VVIS";

        public override bool Run(ResultsLogContainer log)
        {
            if (!CalculateExecutablePath(log))
            {
                return false;
            }

            var settings = new VvisCompilationSettingsViewModel();

            using (new ApplicationContentViewLoader(settings))
            {
                return RunExecutable(log, settings, MapCompileSessionInfo.Instance.GeneratedFileNameNoExtension) == 0;
            }
        }

        private bool CalculateExecutablePath(ResultsLogContainer log)
        {
            if (string.IsNullOrWhiteSpace(VProject))
            {
                log.AppendLine("VVIS", "VProject is not set. Set your VPROJECT environment variable in Windows system environment variables and then restart Tsukuru. It should be the full path to your game directory, for example: A:\\SteamLibrary\\steamapps\\common\\Team Fortress 2\\tf");
                return false;
            }

            if (_executable == null)
            {
                _executable = new FileInfo(Path.Combine(SdkToolsPath, "bin", "vvis.exe"));
            }

            if (_executable.Exists)
            {
                return true;
            }

            log.AppendLine("VVIS", $"Unable to find a vvis.exe at expected path: {_executable.FullName}");
            return false;
        }

        private string GenerateArgs(ICompilationSettings settings, string vmfPathWithoutExtension)
        {
            return $" -game \"{VProject}\" {settings.FormattedArguments} {vmfPathWithoutExtension.PrependIfNeeded('"').AppendIfNeeded('"')}";
        }

        private int RunExecutable(ResultsLogContainer log, ICompilationSettings settings, string vmfPathWithoutExtension)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = _executable.FullName,
                Arguments = GenerateArgs(settings, vmfPathWithoutExtension),

                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using (var process = new Process())
            {
                process.StartInfo = startInfo;

                var errors = new StringBuilder();

                process.ErrorDataReceived += (s, e) =>
                {
                    errors.Append(e.Data);
                };

                log.AppendLine("VVIS", "Redirecting process output:");

                process.Start();
                process.BeginErrorReadLine();

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

                log.AppendLine("VVIS", $"Exited with code {process.ExitCode}");

                if (errors.Length > 0)
                {
                    log.AppendLine("VVIS", errors.ToString());
                }

                return process.ExitCode;
            }
        }
    }
}