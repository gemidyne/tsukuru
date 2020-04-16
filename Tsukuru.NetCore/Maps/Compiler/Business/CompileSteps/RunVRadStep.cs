﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using GalaSoft.MvvmLight.Ioc;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps
{
    internal class RunVRadStep : BaseVProjectStep
    {
        private FileInfo _executable;

        public override string StepName => "VRAD";

        public override bool Run(ILogReceiver log)
        {
            var viewModel = SimpleIoc.Default.GetInstance<CompileConfirmationViewModel>();

            if (viewModel.VRADSettings.UseModifiedVrad)
            {
                var modifiedLib = new FileInfo(Path.Combine(SdkToolsPath, "bin", "vrad_dll-optimized.dll"));
                var modifiedExe = new FileInfo(Path.Combine(SdkToolsPath, "bin", "vrad_optimized.exe"));

                if (!modifiedLib.Exists)
                {
                    log.WriteLine("VRAD", "Writing modified VRAD DLL...");
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tsukuru.Maps.Compiler.ModdedVrad.vrad_dll-optimized.dll"))
                    using (var fileStream = modifiedLib.OpenWrite())
                    {
                        stream.CopyTo(fileStream);
                    }
                }

                if (!modifiedExe.Exists)
                {
                    log.WriteLine("VRAD", "Writing modified VRAD EXE...");
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tsukuru.Maps.Compiler.ModdedVrad.vrad_optimized.exe"))
                    using (var fileStream = modifiedExe.OpenWrite())
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }

            CalculateVradPath(viewModel.VRADSettings.UseModifiedVrad);

            return RunVradExecutable(log, viewModel.VRADSettings, MapCompileSessionInfo.Instance.GeneratedFileNameNoExtension) == 0;
        }

        private void CalculateVradPath(bool useModdedExecutable)
        {
            if (string.IsNullOrWhiteSpace(VProject))
            {
                throw new NotSupportedException("VProject is not set");
            }

            if (_executable == null)
            {
                string vradFileName = useModdedExecutable
                    ? "vrad_optimized.exe"
                    : "vrad.exe";

                _executable = new FileInfo(Path.Combine(SdkToolsPath, "bin", vradFileName));

                if (!useModdedExecutable && !_executable.Exists)
                {
                    throw new FileNotFoundException("VRAD executable not found.", _executable.FullName);
                }
            }
        }

        private string GenerateArgs(ICompilationSettings settings, string vmfPathWithoutExtension)
        {
            return $" -game \"{VProject}\" {settings.FormattedArguments} \"{vmfPathWithoutExtension}\"";
        }

        private int RunVradExecutable(ILogReceiver log, ICompilationSettings settings, string vmfPathWithoutExtension)
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

                log.WriteLine("VRAD", "Redirecting process output:");

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

                log.WriteLine("VRAD", $"Exited with code {process.ExitCode}");

                if (errors.Length > 0)
                {
                    log.WriteLine("VRAD", errors.ToString());
                }

                return process.ExitCode;
            }
        }


    }
}