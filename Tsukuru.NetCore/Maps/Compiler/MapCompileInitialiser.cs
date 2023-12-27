using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using Tsukuru.Maps.Compiler.Business;
using Tsukuru.Maps.Compiler.Business.CompileSteps;
using Tsukuru.Maps.Compiler.ViewModels;
using Tsukuru.Settings;
using Tsukuru.ViewModels;

namespace Tsukuru.Maps.Compiler;

public static class MapCompileInitialiser
{
    public static async Task<bool> ExecuteAsync(CompileConfirmationViewModel compileConfirmationViewModel)
    {
        MapCompileSessionInfo.Instance.InputVmfFile = new FileInfo(SettingsManager.Manifest.MapCompilerSettings.LastVmfPath);

        var mainWindow = Ioc.Default.GetRequiredService<MainWindowViewModel>();
        var logView = mainWindow.Pages.OfType<ResultsViewModel>().Single();

        logView.Initialise();

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

        return await stepRunner.RunAsync();
    }
}