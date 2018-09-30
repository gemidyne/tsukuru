using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tsukuru.Maps.Packer
{
    public class BspPackEngine
    {
        private readonly ILogReceiver _log;

        public PackerSessionDetails Details { get; set; }

        public Dictionary<string, string> FilesToPack { get; set; }

        public BspPackEngine(ILogReceiver log, PackerSessionDetails sessionDetails)
        {
            _log = log;
            Details = sessionDetails;
        }

        public void Pack()
        {
            _log.WriteLine("BspPackEngine", "Preparing to pack files...");
            PrepareFilesForPacking();

            _log.WriteLine("BspPackEngine", $"Files to pack: {FilesToPack.Count}");
            _log.WriteLine("BspPackEngine", "Packing...");

            PackBsp();
            _log.WriteLine("BspPackEngine", "Pack complete.");
        }

        private void PrepareFilesForPacking()
        {
            FilesToPack = new Dictionary<string, string>();

            foreach (var folder in Details.FoldersToPackIn)
            {
                var raw = Directory.GetFiles(folder, "*", SearchOption.AllDirectories).ToList();

                RemoveForbiddenFiles(raw);

                foreach (var file in raw)
                {
                    var key = file.Replace(folder, string.Empty);

                    if (key.StartsWith("\\"))
                    {
                        key = key.Substring(1);
                    }

                    FilesToPack.Add(key, file);
                }
            }

            foreach (var fileType in FilesToPack.GroupBy(f => Path.GetExtension(f.Key)))
            {
                _log.WriteLine("BspPackEngine", $"{fileType.Key} count: {fileType.Count()}");
            }

            System.Threading.Thread.Sleep(3000);

            var contents = new StringBuilder();
            foreach (var file in FilesToPack)
            {
                contents.AppendLine(file.Key);
                contents.AppendLine(file.Value);
            }

            File.WriteAllText(Details.FileListFile, contents.ToString());
        }

        private void RemoveForbiddenFiles(List<string> fileList)
        {
            string[] exts = { ".cache", ".bz2", ".zip", ".7z", ".vpk", ".ztmp" };

            foreach (var ext in exts)
            {
                var filesToRemove = fileList.Where(f => Path.GetExtension(f).ToLower() == ext).ToList();
                foreach (var removedFile in filesToRemove)
                {
                    fileList.Remove(removedFile);
                }
            }
        }

        private void PackBsp()
        {
            var input = Details.MapFile + ".bak";

            if (File.Exists(input))
            {
                File.Delete(input);
            }

            File.Move(Details.MapFile, input);

            var args = GetArgumentForBspZip(input, Details.FileListFile, Details.MapFile);

            var startInfo = new ProcessStartInfo(Path.Combine(Details.GamePath, "bin", "bspzip.exe"), args)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            _log.WriteLine("SCE", "Redirecting BSPZIP output:");

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

                _log.WriteLine("SCE", $"BSPZIP exited with code {process.ExitCode}");
            }
        }

        private static string GetArgumentForBspZip(string inputMap, string fileList, string outputMap)
        {
            return $"-addorupdatelist \"{inputMap}\" \"{fileList}\" \"{outputMap}\"";
        }
    }
}
