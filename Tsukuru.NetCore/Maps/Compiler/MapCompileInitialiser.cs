﻿using GalaSoft.MvvmLight.Ioc;
using Tsukuru.Maps.Compiler.Business;
using Tsukuru.Maps.Compiler.Business.CompileSteps;
using Tsukuru.Maps.Compiler.ViewModels;

namespace Tsukuru.Maps.Compiler
{
    public static class MapCompileInitialiser
    {
        public static bool Execute(MapCompilerViewModel mapCompilerViewModel)
        {
            var logView = SimpleIoc.Default.GetInstance<MapCompilerResultsViewModel>();
            var mainWindow = SimpleIoc.Default.GetInstance<MainWindowViewModel>();
            var mapPacking = SimpleIoc.Default.GetInstance<ResourcePackingViewModel>();
            var postBuild = SimpleIoc.Default.GetInstance<PostCompileActionsViewModel>();

            logView.Initialise(mapCompilerViewModel.MapName);

#warning TODO amend this
            //mainWindow.DisplayMapCompilerResultsView = true;
            //mainWindow.DisplayMapCompilerView = false;
            //mainWindow.DisplaySourcePawnCompilerView = false;

            MapCompileSessionInfo.Clear();
            MapCompileSessionInfo.Instance.MapName = mapCompilerViewModel.MapName;

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

            return true;
        }
    }
}
