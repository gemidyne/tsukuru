using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler.Business.CompileSteps;

internal class RunVRadStep : BaseVProjectStep
{
    private readonly ISettingsManager _settingsManager;
    private FileInfo _executable;

    public override string StepName => "VRAD";

    public RunVRadStep(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }

    public override bool Run(ResultsLogContainer log)
    {
        var settings = new VradCompilationSettingsViewModel(_settingsManager);

        using (new ApplicationContentViewLoader(settings))
        {
            if (settings.UseModifiedVrad)
            {
                var modifiedLib = new FileInfo(Path.Combine(SdkToolsPath, "bin", "vrad_dll-optimized.dll"));
                var modifiedExe = new FileInfo(Path.Combine(SdkToolsPath, "bin", "vrad_optimized.exe"));

                if (!modifiedLib.Exists)
                {
                    log.AppendLine("VRAD", "Writing modified VRAD DLL...");
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tsukuru.Maps.Compiler.ModdedVrad.vrad_dll-optimized.dll"))
                    using (var fileStream = modifiedLib.OpenWrite())
                    {
                        stream.CopyTo(fileStream);
                    }
                }

                if (!modifiedExe.Exists)
                {
                    log.AppendLine("VRAD", "Writing modified VRAD EXE...");
                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tsukuru.Maps.Compiler.ModdedVrad.vrad_optimized.exe"))
                    using (var fileStream = modifiedExe.OpenWrite())
                    {
                        stream.CopyTo(fileStream);
                    }
                }
            }

            if (!CalculateExecutablePath(settings.UseModifiedVrad, log))
            {
                return false;
            }

            return RunExecutable(
                log: log, 
                settings: settings,
                vmfPathWithoutExtension: MapCompileSessionInfo.Instance.GeneratedFileNameNoExtension) == 0;
        }
    }

    private bool CalculateExecutablePath(bool useModdedExecutable, ResultsLogContainer log)
    {
        if (string.IsNullOrWhiteSpace(VProject))
        {
            log.AppendLine("VRAD", "VProject is not set. Set your VPROJECT environment variable in Windows system environment variables and then restart Tsukuru. It should be the full path to your game directory, for example: A:\\SteamLibrary\\steamapps\\common\\Team Fortress 2\\tf");
            return false;
        }

        if (_executable == null)
        {
            string vradFileName = useModdedExecutable
                ? "vrad_optimized.exe"
                : "vrad.exe";

            _executable = new FileInfo(Path.Combine(SdkToolsPath, "bin", vradFileName));
        }

        if (_executable.Exists)
        {
            return true;
        }

        log.AppendLine("VRAD", $"Unable to find a vrad.exe at expected path: {_executable.FullName}");
        return false;
    }

    private string GenerateArgs(ICompilationSettings settings, string vmfPathWithoutExtension)
    {
        return $" -game \"{VProject}\" {settings.FormattedArguments} \"{vmfPathWithoutExtension}\"";
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

            log.AppendLine("VRAD", "Redirecting process output:");

            process.Start();
            process.BeginErrorReadLine();

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

            log.AppendLine("VRAD", $"Exited with code {process.ExitCode}");

            if (errors.Length > 0)
            {
                log.AppendLine("VRAD", errors.ToString());
            }

            return process.ExitCode;
        }
    }


}