using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using GalaSoft.MvvmLight.Ioc;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
    internal class RunVVisStep : BaseVProjectStep
    {
        private FileInfo _executable;

        public override string StepName => "VVIS";

        public override bool Run(ILogReceiver log)
        {
            CalculateVvisPath();

            var viewModel = SimpleIoc.Default.GetInstance<CompileConfirmationViewModel>();

            return RunVvisExecutable(log, viewModel.VVISSettings, MapCompileSessionInfo.Instance.GeneratedFileNameNoExtension) == 0;
        }

        private void CalculateVvisPath()
        {
            if (string.IsNullOrWhiteSpace(VProject))
            {
                throw new NotSupportedException("VProject is not set");
            }

            if (_executable == null)
            {
                _executable = new FileInfo(Path.Combine(SdkToolsPath, "bin", "vvis.exe"));

                if (!_executable.Exists)
                {
                    throw new FileNotFoundException("VVIS executable not found.", _executable.FullName);
                }
            }
        }

        private string GenerateArgs(ICompilationSettings settings, string vmfPathWithoutExtension)
        {
            return $" -game \"{VProject}\" {settings.FormattedArguments} \"{vmfPathWithoutExtension}\"";
        }

        private int RunVvisExecutable(ILogReceiver log, ICompilationSettings settings, string vmfPathWithoutExtension)
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

                log.WriteLine("VVIS", "Redirecting process output:");

                process.Start();
                process.BeginErrorReadLine();

                var outputReader = new Thread(() =>
                {
                    int ch;

                    while ((ch = process.StandardOutput.Read()) >= 0)
                    {
                        log.Write(message: ((char)ch).ToString());
                    }
                });

                outputReader.Start();

                process.WaitForExit();

                outputReader.Join();

                log.WriteLine("VVIS", $"Exited with code {process.ExitCode}");

                if (errors.Length > 0)
                {
                    log.WriteLine("VVIS", errors.ToString());
                }

                return process.ExitCode;
            }
        }
    }
}