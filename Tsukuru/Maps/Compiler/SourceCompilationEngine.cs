using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using Tsukuru.Steam;

namespace Tsukuru.Maps.Compiler
{
    public class SourceCompilationEngine
    {
        private static string _vProject;
        private readonly ILogReceiver _log;
	    private readonly string _vmfFile;

        private readonly bool _useModifiedVrad;
        private readonly bool _copyToGameMapsOnComplete;
	    private readonly bool _launchMapInGame;

        private string _vBSPPath;
        private string _vVISPath;
        private string _vRADPath;
        private string _gameMapsFolderPath;

        public string VmfPathWithoutExtension
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(_vmfFile), Path.GetFileNameWithoutExtension(_vmfFile));
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

        public string SdkToolsPath
        {
            get
            {
                var parent = Directory.GetParent(VProject);
                return parent.FullName;
            }
        }

        public string VbspPath
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(VProject))
                {
                    throw new NotSupportedException("VProject is not set");
                }

                if (string.IsNullOrWhiteSpace(_vBSPPath))
                {
                    _vBSPPath = Path.Combine(SdkToolsPath, "bin\\vbsp.exe");

                    if (!File.Exists(_vBSPPath))
                    {
                        throw new FileNotFoundException("VBSP executable not found.", _vBSPPath);
                    }
                }

                return _vBSPPath; 
            }
        }

        private string VbspArguments { get; set; }

        public string VvisPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VProject))
                {
                    throw new NotSupportedException("VProject is not set");
                }

                if (string.IsNullOrWhiteSpace(_vVISPath))
                {
                    _vVISPath = Path.Combine(SdkToolsPath, "bin\\vvis.exe");

                    if (!File.Exists(_vVISPath))
                    {
                        throw new FileNotFoundException("VVIS executable not found.", _vVISPath);
                    }
                }

                return _vVISPath;
            }
        }

        private string VvisArguments { get; set; }

        public string VradPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VProject))
                {
                    throw new NotSupportedException("VProject is not set");
                }

                if (string.IsNullOrWhiteSpace(_vRADPath))
                {
                    string vradFileName = _useModifiedVrad
                        ? "vrad_optimized.exe"
                        : "vrad.exe";

                    _vRADPath = Path.Combine(SdkToolsPath, "bin", vradFileName);

                    if (!_useModifiedVrad && !File.Exists(_vRADPath))
                    {
                        throw new FileNotFoundException("VRAD executable not found.", _vRADPath);
                    }
                }

                return _vRADPath;
            }
        }

        private string VradArguments { get; set; }

        public string GameMapsFolderPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(VProject))
                {
                    throw new NotSupportedException("VProject is not set");
                }

                if (string.IsNullOrWhiteSpace(_gameMapsFolderPath))
                {
                    _gameMapsFolderPath = Path.Combine(VProject, "maps");

                    if (!Directory.Exists(_gameMapsFolderPath))
                    {
                        throw new DirectoryNotFoundException("Unable to determine game maps folder. Ensure your VProject is the game folder and not the root folder. (Example: \"SteamApps\\Common\\Team Fortress 2\\tf\")");
                    }
                }

                return _gameMapsFolderPath;
            }
        }

        public SourceCompilationEngine(
	        ILogReceiver log,
			string vmfFile,
	        bool useModifiedVrad,
	        bool copyToGameMapsOnComplete, 
	        bool launchMapInGame)
        {
	        _vmfFile = vmfFile;
            _log = log;
            _useModifiedVrad = useModifiedVrad;
            _copyToGameMapsOnComplete = copyToGameMapsOnComplete;
	        _launchMapInGame = launchMapInGame;
        }

        /// <summary>
        /// Performs map compilation.
        /// </summary>
        /// <param name="vbspSettings"></param>
        /// <param name="vvisSettings"></param>
        /// <param name="vradSettings"></param>
        /// <returns>Path to BSP.</returns>
        public string DoCompile(
            ICompilationSettings vbspSettings, 
            ICompilationSettings vvisSettings, 
            ICompilationSettings vradSettings)
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

            string result = VmfPathWithoutExtension + ".bsp";

            if (_copyToGameMapsOnComplete)
            {
                _log.WriteLine("SCE", $"Copying map to the game maps folder at: {GameMapsFolderPath}");
                File.Copy(result, Path.Combine(GameMapsFolderPath, Path.GetFileName(result)), true);
            }

	        if (_launchMapInGame)
	        {
				_log.WriteLine("SCE", "Launching game and loading map...");
		        SteamHelper.LaunchAppWithMap(Path.GetFileNameWithoutExtension(_vmfFile));
	        }

            return result;
        }

        private void GenerateExecutableArguments(ICompilationSettings vbspSettings, ICompilationSettings vvisSettings, ICompilationSettings vradSettings)
        {
            VbspArguments = string.Format(" -game \"{0}\" {1} \"{2}\"", VProject, vbspSettings.FormattedArguments, VmfPathWithoutExtension);

            VvisArguments = string.Format(" -game \"{0}\" {1} \"{2}\"", VProject, vvisSettings.FormattedArguments, VmfPathWithoutExtension);

            VradArguments = string.Format(" -game \"{0}\" {1} \"{2}\"", VProject, vradSettings.FormattedArguments, VmfPathWithoutExtension);
        }

        private void CleanupLogs()
        {
            string path = VmfPathWithoutExtension + ".log";
            string oldLog = VmfPathWithoutExtension + "_old.log";

            if (File.Exists(oldLog))
            {
                File.Delete(oldLog);
            }

            if (File.Exists(path))
            {
                File.Move(path, oldLog);
            }

            string vbspLog = VmfPathWithoutExtension + "_vbsp.log";
            string vvisLog = VmfPathWithoutExtension + "_vvis.log";
            string vradLog = VmfPathWithoutExtension + "_vrad.log";
            string vbspOldLog = VmfPathWithoutExtension + "_vbsp_old.log";
            string vvisOldLog = VmfPathWithoutExtension + "_vvis_old.log";
            string vradOldLog = VmfPathWithoutExtension + "_vrad_old.log";

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
                _log.WriteLine("SCE", $"VBSP located at: {VbspPath}");
                _log.WriteLine("SCE", $"VVIS located at: {VvisPath}");

                if (_useModifiedVrad)
                {
                    string vradLib = Path.Combine(Path.GetDirectoryName(VradPath), "vrad_dll-optimized.dll");
                    string vradExe = VradPath;

                    if (!File.Exists(vradLib))
                    {
                        _log.WriteLine("SCE", "Copying VRAD DLL");
                        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tsukuru.Maps.Compiler.ModdedVrad.vrad_dll-optimized.dll"))
                        using (var fileStream = new FileStream(vradLib, FileMode.OpenOrCreate))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }

                    if (!File.Exists(vradExe))
                    {
                        _log.WriteLine("SCE", "Copying VRAD EXE");
                        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tsukuru.Maps.Compiler.ModdedVrad.vrad_optimized.exe"))
                        using (var fileStream = new FileStream(vradExe, FileMode.OpenOrCreate))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                }

                _log.WriteLine("SCE", $"VRAD located at: {VradPath}");
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
                FileName = VbspPath,
                Arguments = VbspArguments,
                
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
                FileName = VvisPath,
                Arguments = VvisArguments,

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
                FileName = VradPath,
                Arguments = VradArguments,

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
