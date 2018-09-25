using ICSharpCode.SharpZipLib.BZip2;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.Maps.Packer;

namespace Tsukuru.Maps.Compiler
{
    public static class MapCompiler
    {
        public static bool Execute(MapCompilerViewModel vm, ILogReceiver log)
        {
            var stopwatch = Stopwatch.StartNew();
            vm.ConsoleText = null;

            log.WriteLine("Tsukuru", "Preparing VMF...");

            var generatedVmfFile = VmfFileCopier.CopyFile(vm.VMFPath, vm.MapName);

            var compilation = new SourceCompilationEngine(log)
            {
                VMFPath = generatedVmfFile
            };

            log.WriteLine("Tsukuru", "Compiling map...");

            var mapFile = compilation.DoCompile(
                vm.VBSPSettings,
                vm.VVISSettings,
                vm.VRADSettings);

            if (string.IsNullOrWhiteSpace(mapFile))
            {
                return false;
            }

            if (vm.PerformResourcePacking)
            {
                log.WriteLine("Tsukuru", "Performing resource packing");

                var session = new PackerSessionDetails
                {
                     MapFile = mapFile, 
                     GamePath = compilation.SDKToolsPath,
                     FoldersToPackIn = vm.FoldersToPack.ToList()
                };

                var packer = new BspPackEngine(log, session);
                packer.Pack();
            }

            if (vm.CompressMapToBZip2)
            {
                log.WriteLine("Tsukuru", "Compressing map to BZip2...");
                CompressFile(mapFile);
            }

            stopwatch.Stop();

            log.WriteLine("Tsukuru", string.Format("Completed in {0}", stopwatch.Elapsed));

            vm.IsCloseButtonOnExecutionEnabled = true;
            return true;
        }

        private static void CompressFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                throw new FileNotFoundException("Unable to find file.", fileName);
            }

            using (var input = File.OpenRead(fileName))
            {
                using (var output = File.Create(fileName + ".bz2"))
                {
                    BZip2.Compress(input, output, true, 4096);
                }
            }
        }
    }
}
