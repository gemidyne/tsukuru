using System.Diagnostics;
using System.IO;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using ICSharpCode.SharpZipLib.BZip2;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.Maps.Packer;

namespace Tsukuru.Maps.Compiler
{
    public static class MapCompiler
    {
        public static bool Execute(MapCompilerViewModel mapCompilerViewModel)
        {
            var logView = SimpleIoc.Default.GetInstance<MapCompilerResultsViewModel>();
            var mainWindow = SimpleIoc.Default.GetInstance<MainWindowViewModel>();

            var stopwatch = Stopwatch.StartNew();

            logView.StartNewSession(mapCompilerViewModel.MapName);
            mainWindow.DisplayMapCompilerResultsView = true;
            mainWindow.DisplayMapCompilerView = false;
            mainWindow.DisplaySourcePawnCompilerView = false;

            logView.WriteLine("Tsukuru", "Preparing VMF...");

            var generatedVmfFile = VmfFileCopier.CopyFile(mapCompilerViewModel.VMFPath, mapCompilerViewModel.MapName);

	        var compilation = new SourceCompilationEngine(
		        log: logView,
		        useModifiedVrad: mapCompilerViewModel.VRADSettings.UseModifiedVrad,
		        copyToGameMapsOnComplete: mapCompilerViewModel.CopyMapToGameMapsFolder,
		        launchMapInGame: mapCompilerViewModel.LaunchMapInGame,
		        vmfFile: generatedVmfFile);

            logView.WriteLine("Tsukuru", "Compiling map...");

            var mapFile = compilation.DoCompile(
                mapCompilerViewModel.VBSPSettings,
                mapCompilerViewModel.VVISSettings,
                mapCompilerViewModel.VRADSettings);

            if (string.IsNullOrWhiteSpace(mapFile))
            {
                stopwatch.Stop();

                logView.WriteLine("Tsukuru", string.Format("An error was encountered in the compilation process. Completed in {0}", stopwatch.Elapsed));
                logView.IsCloseButtonOnExecutionEnabled = true;

                return false;
            }

            if (mapCompilerViewModel.PerformResourcePacking)
            {
                logView.WriteLine("Tsukuru", "Performing resource packing");

                var session = new PackerSessionDetails
                {
                     MapFile = mapFile, 
                     GamePath = compilation.SdkToolsPath,
                     FoldersToPackIn = mapCompilerViewModel.FoldersToPack.ToList()
                };

                var packer = new BspPackEngine(logView, session);
                packer.Pack();
            }

            if (mapCompilerViewModel.CompressMapToBZip2)
            {
                logView.WriteLine("Tsukuru", "Compressing map to BZip2...");
                CompressFile(mapFile);
            }

            stopwatch.Stop();

            logView.WriteLine("Tsukuru", string.Format("Completed in {0}", stopwatch.Elapsed));
            logView.IsCloseButtonOnExecutionEnabled = true;

            mainWindow.DisplayMapCompilerResultsView = true;
            mainWindow.DisplayMapCompilerView = true;
            mainWindow.DisplaySourcePawnCompilerView = true;

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
