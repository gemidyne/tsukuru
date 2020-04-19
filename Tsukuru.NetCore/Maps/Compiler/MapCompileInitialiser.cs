using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using Tsukuru.Maps.Compiler.Business;
using Tsukuru.Maps.Compiler.Business.CompileSteps;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler
{
    public static class MapCompileInitialiser
    {
        public static bool Execute(CompileConfirmationViewModel compileConfirmationViewModel)
        {
            var mainWindow = SimpleIoc.Default.GetInstance<MainWindowViewModel>();
            var logView = mainWindow.Pages.OfType<ResultsViewModel>().Single();


            var mapPacking = mainWindow.Pages.OfType<ResourcePackingViewModel>().Single();
            var postBuild = mainWindow.Pages.OfType<PostCompileActionsViewModel>().Single();

            using (new ApplicationContentViewLoader(mapPacking))
            using (new ApplicationContentViewLoader(postBuild))
            {
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

                if (mapPacking.PerformResourcePacking)
                {
                    stepRunner.AddStep(new ResourcePackingStep());
                }

                if (postBuild.CopyMapToGameMapsFolder)
                {
                    stepRunner.AddStep(new CopyBspToGameStep());
                }

                if (postBuild.CompressMapToBZip2)
                {
                    stepRunner.AddStep(new CompressBspToBzip2Step());
                }

                if (postBuild.LaunchMapInGame)
                {
                    stepRunner.AddStep(new LaunchMapInGameStep());
                }

                stepRunner.Run();

#warning TODO amend this
                //mainWindow.DisplayMapCompilerResultsView = true;
                //mainWindow.DisplayMapCompilerView = true;
                //mainWindow.DisplaySourcePawnCompilerView = true;

            }

            return true;
        }
    }
}
