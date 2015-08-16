using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Windows;
using Tsukuru.Data;
using Tsukuru.ViewModels;

namespace Tsukuru
{
    public class Processor
    {
        public void CompileBatch(MainWindowViewModel vm)
        {
            vm.ProgressBarValue = 0;
            vm.ProgressBarMaximum = vm.FilesToCompile.Count;

            foreach (var compilationFile in vm.FilesToCompile)
            {
                UpdateCompilationDataStatus(compilationFile, isCompiling: true);
            }

            foreach (var compilationFile in vm.FilesToCompile)
            {
                Compile(vm, compilationFile);
                vm.ProgressBarValue++;
            }

            vm.ProgressBarValue = 0;
            SystemSounds.Asterisk.Play();
        }

        public void Compile(MainWindowViewModel vm, CompilationFileViewModel compilationFileViewModel)
        {
            string file = compilationFileViewModel.File;
            compilationFileViewModel.Messages.Clear();

            Version ver;
            
            GetAndIncrementVersionFile(Path.GetDirectoryName(compilationFileViewModel.File), vm.IncrementVersion, out ver);

            string rewrittenFilePath = RewriteSourceFile(compilationFileViewModel.File, ver);

            try
            {
                using (var compiler = new Process())
                {
                    compiler.StartInfo.FileName = SettingsManager.CompilerLocation;
                    compiler.StartInfo.Arguments = string.Format(
						"{0} -o=\"{1}\"", 
                        rewrittenFilePath,
                        Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)));

                    compiler.StartInfo.UseShellExecute = false;
                    compiler.StartInfo.RedirectStandardOutput = true;
                    compiler.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    compiler.StartInfo.CreateNoWindow = true;
                    compiler.EnableRaisingEvents = true;
                    compiler.Start();

                    string output = compiler.StandardOutput.ReadToEnd();

                    if (File.Exists(rewrittenFilePath))
                    {
                        File.Delete(rewrittenFilePath);
                    }

                    if (!string.IsNullOrWhiteSpace(output))
                    {
                        foreach (CompilationMessage message in GetCompilationMessagesFromOutput(output.Split('\n')))
                        {
                            compilationFileViewModel.Messages.Add(message);
                        }
                    }

                    UpdateCompilationDataStatus(compilationFileViewModel);

                    if (!compilationFileViewModel.Messages.Any(m => CompilationMessageHelper.IsLineError(m.Prefix)) && SettingsManager.ExecutePostBuildScripts)
                    {
                        RunPostBuildScript(Path.GetDirectoryName(file));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private static void RunPostBuildScript(string workingDirectory)
        {
            SettingsManager.ExecutePostBuildScripts = true;

            string postBuildFile = Path.Combine(workingDirectory, "post_build.cmd");

	        if (!File.Exists(postBuildFile))
	        {
		        return;
	        }

	        Process process = Process.Start(new ProcessStartInfo(postBuildFile)
	        {
		        WorkingDirectory = workingDirectory,
		        CreateNoWindow = true
	        });

	        if (process != null)
	        {
		        process.WaitForExit();
	        }
        }

        private static string RewriteSourceFile(string file, Version version)
        {
            var fileTxt = new StringBuilder();
            string output = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + "_v" + version + Path.GetExtension(file));

            foreach (string line in File.ReadAllLines(file))
            {
                fileTxt.AppendLine(line.Replace("{TsukuruVersion}", version.ToString()));
            }

            File.WriteAllText(output, fileTxt.ToString());

            return output;
        }

        private static void GetAndIncrementVersionFile(string workingDirectory, bool increment, out Version version)
        {
            string versionFile = Path.Combine(workingDirectory, "version");
            version = new Version(1, 0, 0, 0);

            if (File.Exists(versionFile))
            {
                string text = File.ReadAllText(versionFile);

                if (Version.TryParse(text, out version))
                {
                    int newMinor = version.Minor;
                    int newBuild = version.Build;
                    int newRev = version.Revision;

                    if (increment)
                    {
                        newRev++;

                        if (newRev == 9)
                        {
                            newBuild++;
                            newRev = 0;
                        }

                        if (newBuild == 9)
                        {
                            newRev = 0;
                            newBuild = 0;
                            newMinor++;
                        }
                    }

                    version = new Version(version.Major, newMinor, newBuild, newRev);
                }
                else
                {
                    version = new Version(1, 0, 0, 0);
                }
            }

            File.WriteAllText(versionFile, version.ToString());
        }

        private static IEnumerable<CompilationMessage> GetCompilationMessagesFromOutput(string[] lines)
        {
            var messages = new List<CompilationMessage>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                CompilationMessage msg = CompilationMessageHelper.ParseFromString(line);

                if (msg == null)
                {
                    continue;
                }

                messages.Add(msg);
            }

            return messages;
        }

        private static void UpdateCompilationDataStatus(CompilationFileViewModel fileViewModel, bool isCompiling = false)
        {
            if (isCompiling)
            {
                fileViewModel.Result = CompilationResult.Compiling;
                fileViewModel.StatusIcon = "/Tsukuru;component/Resources/script_code.png";
                return;
            }

            if (!fileViewModel.Messages.Any())
            {
                fileViewModel.Result = CompilationResult.Unknown;
				fileViewModel.StatusIcon = "/Tsukuru;component/Resources/script_code.png";
                return;
            }

            if (fileViewModel.Messages.Any(m => CompilationMessageHelper.IsLineError(m.Prefix)))
            {
                fileViewModel.Result = CompilationResult.FailedWithErrors;
				fileViewModel.StatusIcon = "/Tsukuru;component/Resources/cross.png";
            }
            else if (fileViewModel.Messages.Any(m => CompilationMessageHelper.IsLineWarning(m.Prefix)))
            {
                fileViewModel.Result = CompilationResult.CompletedWithWarnings;
				fileViewModel.StatusIcon = "/Tsukuru;component/Resources/error.png";
            }
            else 
            {
                fileViewModel.Result = CompilationResult.Completed;
				fileViewModel.StatusIcon = "/Tsukuru;component/Resources/tick.png";
            }
        }
    }
}
