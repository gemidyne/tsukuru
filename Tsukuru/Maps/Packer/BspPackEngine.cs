using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Chiaki;
using Tsukuru.Maps.Compiler.Business;
using Tsukuru.SourceEngineTools;

namespace Tsukuru.Maps.Packer
{
    public class BspPackEngine : IProgress<string>
    {
        private readonly ILogReceiver _log;

        public PackerSessionDetails Details { get; }

        public Dictionary<string, string> FilesToPack { get; private set; }

        public BspPackEngine(ILogReceiver log, PackerSessionDetails sessionDetails)
        {
            _log = log;
            Details = sessionDetails;
        }

        public void PackData()
        {
            FilesToPack = new Dictionary<string, string>();

            Report("Generating file list for folders marked as include all...");
            GenerateFileListForcedFolders();

            Report("Generating dependency file list from inspection...");
            GenerateFileListIntelligent();

            foreach (var fileType in FilesToPack.GroupBy(f => Path.GetExtension(f.Key)))
            {
                Report($"{fileType.Key} count: {fileType.Count()}");
            }

            _log.WriteLine("BspPackEngine", $"Number of files to pack: {FilesToPack.Count}");

            WriteFileList();
            _log.WriteLine("BspPackEngine", "Packing...");

            PackBsp();
            _log.WriteLine("BspPackEngine", "Pack complete.");
        }

        public void RepackCompressBsp()
        {
            var args = GetArgumentsForRepackCompress(Details.MapFile);

            var startInfo = new ProcessStartInfo(Path.Combine(Details.GamePath, "bin", "bspzip.exe"), args)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            _log.WriteLine("REPACK", "Redirecting repack output:");

            using (var process = Process.Start(startInfo))
            {
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

                _log.WriteLine("REPACK", $"BSPZIP exited with code {process.ExitCode}");
            }
        }

        private void GenerateFileListForcedFolders()
        {
            foreach (var folder in Details.CompleteFoldersToAdd)
            {
                var raw = Directory.GetFiles(folder, "*", SearchOption.AllDirectories).ToList();

                RemoveForbiddenFiles(raw);

                foreach (var file in raw)
                {
                    var key = file.TrimStart(folder);

                    if (key.StartsWith("\\"))
                    {
                        key = key.Substring(1);
                    }

                    FilesToPack.Add(key, file);
                }
            }
        }

        private void GenerateFileListIntelligent()
        {
            var results = BspDependencyAnalyser.Analyse(this, VProjectHelper.Path, Details.MapFile);

            var customAssets = results.CustomSounds.Union(results.CustomMaterials).Union(results.CustomModels).ToArray();

            foreach (var folder in Details.IntelligentFoldersToAdd)
            {
                var raw = Directory.GetFiles(folder, "*", SearchOption.AllDirectories).ToList();

                RemoveForbiddenFiles(raw);

                foreach (var file in raw)
                {
                    var key = file.TrimStart(folder);

                    if (key.StartsWith("\\"))
                    {
                        key = key.Substring(1);
                    }

                    if (customAssets.Any(a => string.Equals(a, key.Replace("\\", "/"), StringComparison.InvariantCultureIgnoreCase)))
                    {
                        FilesToPack.Add(key, file);
                    }
                }
            }
        }

        private static void RemoveForbiddenFiles(List<string> fileList)
        {
            string[] exts = { ".cache", ".bz2", ".zip", ".7z", ".vpk", ".ztmp", ".tsutmpl", ".png", ".psd", ".exe", ".dll", ".rar", ".tmp", ".inf", ".db", ".ctx" };

            foreach (var ext in exts)
            {
                var filesToRemove = fileList.Where(f => Path.GetExtension(f)?.ToLower() == ext).ToList();
                foreach (var removedFile in filesToRemove)
                {
                    fileList.Remove(removedFile);
                }
            }
        }

        private void WriteFileList()
        {
            var fileListContents = new StringBuilder();
            foreach (var file in FilesToPack)
            {
                fileListContents.AppendLine(file.Key);
                fileListContents.AppendLine(file.Value);
            }

            Report("Writing file list...");
            File.WriteAllText(Details.FileListFile, fileListContents.ToString());
        }

        private void PackBsp()
        {
            var input = Details.MapFile + ".bak";

            if (File.Exists(input))
            {
                File.Delete(input);
            }

            File.Move(Details.MapFile, input);

            var args = GetArgumentsForPack(input, Details.FileListFile, Details.MapFile);

            var startInfo = new ProcessStartInfo(Path.Combine(Details.GamePath, "bin", "bspzip.exe"), args)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            _log.WriteLine("PACK", "Redirecting process output:");

            using (var process = Process.Start(startInfo))
            {
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

                _log.WriteLine("PACK", $"Exited with code {process.ExitCode}");
            }
        }

        private static string GetArgumentsForPack(string inputMap, string fileList, string outputMap)
        {
            return $"-addorupdatelist \"{inputMap}\" \"{fileList}\" \"{outputMap}\"";
        }

        private static string GetArgumentsForRepackCompress(string bspFile)
        {
            return $" -repack -compress \"{bspFile}\"";
        }

        public void Report(string value)
        {
            _log.WriteLine(nameof(BspPackEngine), value);
        }
    }
}
