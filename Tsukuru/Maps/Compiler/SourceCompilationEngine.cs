using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;

namespace Tsukuru.Maps.Compiler
{
    public class SourceCompilationEngine
    {
        private readonly ILogReceiver _log;
        private static string _vProject;
        private string _vBSPPath;
        private string _vVISPath;
        private string _vRADPath;

        public string VMFPath { get; set; }

        public string VMFPathWithoutExtension
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(VMFPath), Path.GetFileNameWithoutExtension(VMFPath));
            }
        }

        public static string VProject
        {
            get 
            { 
                if (string.IsNullOrWhiteSpace(_vProject))
                {
                    _vProject = Environment.GetEnvironmentVariable("VPROJECT", EnvironmentVariableTarget.User);
                }

                return _vProject; 
            }
        }

        public string SDKToolsPath
        {
            get
            {
                var parent = Directory.GetParent(VProject);
                return parent.FullName;
            }
        }

        public string VBSPPath
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(VProject))
                {
                    throw new NotSupportedException("VProject is not set");
                }

                if (string.IsNullOrWhiteSpace(_vBSPPath))
                {
                    _vBSPPath = Path.Combine(SDKToolsPath, "bin\\vbsp.exe");

                    if (!File.Exists(_vBSPPath))
                    {
                        throw new FileNotFoundException("Valve Binary Space Partition Creation executable not found.", _vBSPPath);
                    }
                }

                return _vBSPPath; 
            }
        }

        private string VBSPArguments { get; set; }

        public string VVISPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VProject))
                {
                    throw new NotSupportedException("VProject is not set");
                }

                if (string.IsNullOrWhiteSpace(_vVISPath))
                {
                    _vVISPath = Path.Combine(SDKToolsPath, "bin\\vvis.exe");

                    if (!File.Exists(_vVISPath))
                    {
                        throw new FileNotFoundException("Valve Visibility Embed executable not found.", _vVISPath);
                    }
                }

                return _vVISPath;
            }
        }

        private string VVISArguments { get; set; }

        public string VRADPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VProject))
                {
                    throw new NotSupportedException("VProject is not set");
                }

                if (string.IsNullOrWhiteSpace(_vRADPath))
                {
                    _vRADPath = Path.Combine(SDKToolsPath, "bin\\vrad.exe");

                    if (!File.Exists(_vRADPath))
                    {
                        throw new FileNotFoundException("Valve Lighting Radiosity Calculation executable not found.", _vRADPath);
                    }
                }

                return _vRADPath;
            }
        }

        private string VRADArguments { get; set; }

        public SourceCompilationEngine(ILogReceiver log)
        {
            _log = log;
        }

        /// <summary>
        /// Performs map compilation.
        /// </summary>
        /// <param name="vbspSettings"></param>
        /// <param name="vvisSettings"></param>
        /// <param name="vradSettings"></param>
        /// <returns>Path to BSP.</returns>
        public string DoCompile(ICompilationSettings vbspSettings, ICompilationSettings vvisSettings, ICompilationSettings vradSettings)
        {
            if (!EnsureRequiredExecutablesExist())
            {
                return null;
            }

            _log.WriteLine("SCE", "Cleaning logs...");
            CleanupLogs();

            _log.WriteLine("SCE", "Generating executable arguments...");
            GenerateExecutableArguments(vbspSettings, vvisSettings, vradSettings);

            _log.WriteLine("SCE", "Compiling map using VBSP...");
            if (RunBspExecutable() != 0)
            {
                return null;
            }

            _log.WriteLine("SCE", "Compiling map using VVIS...");
            if (RunVvisExecutable() != 0)
            {
                return null;
            }

            _log.WriteLine("SCE", "Compiling map using VRAD...");
            if (RunVradExecutable() != 0)
            {
                return null;
            }

            return VMFPathWithoutExtension + ".bsp";
        }

        private void GenerateExecutableArguments(ICompilationSettings vbspSettings, ICompilationSettings vvisSettings, ICompilationSettings vradSettings)
        {
            VBSPArguments = string.Format(" -game \"{0}\" {1} \"{2}\"", VProject, vbspSettings.FormattedArguments, VMFPathWithoutExtension);

            VVISArguments = string.Format(" -game \"{0}\" {1} \"{2}\"", VProject, vvisSettings.FormattedArguments, VMFPathWithoutExtension);

            VRADArguments = string.Format(" -game \"{0}\" {1} \"{2}\"", VProject, vradSettings.FormattedArguments, VMFPathWithoutExtension);
        }

        private void CleanupLogs()
        {
            string path = VMFPathWithoutExtension + ".log";
            string oldLog = VMFPathWithoutExtension + "_old.log";

            if (File.Exists(oldLog))
            {
                File.Delete(oldLog);
            }

            if (File.Exists(path))
            {
                File.Move(path, oldLog);
            }

            string vbspLog = VMFPathWithoutExtension + "_vbsp.log";
            string vvisLog = VMFPathWithoutExtension + "_vvis.log";
            string vradLog = VMFPathWithoutExtension + "_vrad.log";
            string vbspOldLog = VMFPathWithoutExtension + "_vbsp_old.log";
            string vvisOldLog = VMFPathWithoutExtension + "_vvis_old.log";
            string vradOldLog = VMFPathWithoutExtension + "_vrad_old.log";

            if (File.Exists(vbspOldLog))
            {
                File.Delete(vbspOldLog);
                _log.WriteLine("SCE", $"NOTE: old vbsp log file deleted: {vbspOldLog}");
            }
            if (File.Exists(vvisOldLog))
            {
                File.Delete(vvisOldLog);
                _log.WriteLine("SCE", $"NOTE: old vvis log file deleted: {vvisOldLog}");
            }
            if (File.Exists(vradOldLog))
            {
                File.Delete(vradOldLog);
                _log.WriteLine("SCE", $"NOTE: old vrad log file deleted: {vradOldLog}");
            }
            if (File.Exists(vbspLog))
            {
                File.Move(vbspLog, vbspOldLog);
                _log.WriteLine("SCE", $"NOTE: existing vbsp log file renamed to : {vbspOldLog}");
            }
            if (File.Exists(vvisLog))
            {
                File.Move(vvisLog, vvisOldLog);
                _log.WriteLine("SCE", $"NOTE: existing vvis log file renamed to : {vvisOldLog}");
            }
            if (File.Exists(vradLog))
            {
                File.Move(vradLog, vradOldLog);
                _log.WriteLine("SCE", $"NOTE: existing vrad log file renamed to : {vradOldLog}");
            }
        }

        private bool EnsureRequiredExecutablesExist()
        {
            try
            {
                _log.WriteLine("SCE", $"VBSP located at: {VBSPPath}");
                _log.WriteLine("SCE", $"VVIS located at: {VVISPath}");
                _log.WriteLine("SCE", $"VRAD located at: {VRADPath}");
            }
            catch (Exception ex)
            {
                if (ex is NotSupportedException)
                {
                    MessageBox.Show(ex.Message, "Error thrown.");
                    return false;
                }
                else if (ex is FileNotFoundException)
                {
                    MessageBox.Show(ex.Message, "Error thrown.");
                    return false;
                }
            }

            return true;
        }

        private int RunBspExecutable()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = VBSPPath,
                Arguments = VBSPArguments,
                
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

                _log.WriteLine("SCE", "Redirecting VBSP output:");

                process.Start();
                process.BeginErrorReadLine();

                var outputReader = new Thread(() =>
                {
                    int ch;

                    while ((ch = process.StandardOutput.Read()) >= 0)
                    {
                        _log.Write(message: ((char)ch).ToString());
                    }
                });
                    
                outputReader.Start();

                process.WaitForExit();

                outputReader.Join();

                _log.WriteLine("SCE", $"VBSP exited with code {process.ExitCode}");

                if (errors.Length > 0)
                {
                    _log.WriteLine("VBSPERR", errors.ToString());
                }

                return process.ExitCode;
            }
        }

        private int RunVvisExecutable()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = VVISPath,
                Arguments = VVISArguments,

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

                _log.WriteLine("SCE", "Redirecting VVIS output:");

                process.Start();
                process.BeginErrorReadLine();

                var outputReader = new Thread(() =>
                {
                    int ch;

                    while ((ch = process.StandardOutput.Read()) >= 0)
                    {
                        _log.Write(message: ((char)ch).ToString());
                    }
                });

                outputReader.Start();

                process.WaitForExit();

                outputReader.Join();

                _log.WriteLine("SCE", $"VVIS exited with code {process.ExitCode}");

                if (errors.Length > 0)
                {
                    _log.WriteLine("VVISERR", errors.ToString());
                }

                return process.ExitCode;
            }
        }

        private int RunVradExecutable()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = VRADPath,
                Arguments = VRADArguments,

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

                _log.WriteLine("SCE", "Redirecting VRAD output:");

                process.Start();
                process.BeginErrorReadLine();

                var outputReader = new Thread(() =>
                {
                    int ch;

                    while ((ch = process.StandardOutput.Read()) >= 0)
                    {
                        _log.Write(message: ((char)ch).ToString());
                    }
                });

                outputReader.Start();

                process.WaitForExit();

                outputReader.Join();

                _log.WriteLine("SCE", $"VRAD exited with code {process.ExitCode}");

                if (errors.Length > 0)
                {
                    _log.WriteLine("VRADERR", errors.ToString());
                }

                return process.ExitCode;
            }
        }
    }
}
