using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using Tsukuru.Maps.Compiler.Business;
using Tsukuru.Maps.Compiler.Business.CompileSteps;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.Settings;

namespace Tsukuru.Maps.Compiler
{
    public static class MapCompileInitialiser
    {
        public static async Task<bool> ExecuteAsync(CompileConfirmationViewModel compileConfirmationViewModel)
        {
            var mainWindow = SimpleIoc.Default.GetInstance<MainWindowViewModel>();
            var logView = mainWindow.Pages.OfType<ResultsViewModel>().Single();

#warning TODO amend this
            //mainWindow.DisplayMapCompilerResultsView = true;
            //mainWindow.DisplayMapCompilerView = false;
            //mainWindow.DisplaySourcePawnCompilerView = false;

#warning TODO this needs to be rewritten so criteria is built up before hand by the compileconfirmationviewmodel

            logView.Initialise(MapCompileSessionInfo.Instance.MapName);

            var stepRunner = new CompileStepRunner(logView);

            stepRunner.AddStep(new PrepareVmfFileStep());
            stepRunner.AddStep(new RunVBspStep());
            stepRunner.AddStep(new RunVVisStep());
            stepRunner.AddStep(new RunVRadStep());

            if (SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.IsEnabled)
            {
                stepRunner.AddStep(new ResourcePackingStep());
            }

            if (SettingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.PerformRepackCompress)
            {
                stepRunner.AddStep(new RepackBspStep());
            }

            if (SettingsManager.Manifest.MapCompilerSettings.CopyMapToGameMapsFolder)
            {
                stepRunner.AddStep(new CopyBspToGameStep());
            }

            if (SettingsManager.Manifest.MapCompilerSettings.CompressMapToBZip2)
            {
                stepRunner.AddStep(new CompressBspToBzip2Step());
            }

            if (SettingsManager.Manifest.MapCompilerSettings.LaunchMapInGame)
            {
                stepRunner.AddStep(new LaunchMapInGameStep());
            }

            await stepRunner.RunAsync();

#warning TODO amend this
            //mainWindow.DisplayMapCompilerResultsView = true;
            //mainWindow.DisplayMapCompilerView = true;
            //mainWindow.DisplaySourcePawnCompilerView = true;

            return true;
        }
    }
}
