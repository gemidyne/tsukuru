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

public class MapCompiler : IMapCompiler
{
    private readonly ISettingsManager _settingsManager;

    public MapCompiler(
        ISettingsManager settingsManager)
    {
        _settingsManager = settingsManager;
    }
    
    public async Task<bool> ExecuteAsync()
    {
        MapCompileSessionInfo.Instance.InputVmfFile = new FileInfo(_settingsManager.Manifest.MapCompilerSettings.LastVmfPath);

        var mainWindow = Ioc.Default.GetRequiredService<MainWindowViewModel>(); // Deferred as otherwise there will be an infinite loop on DI resolution on MainWindow
        var logView = mainWindow.Pages.OfType<ResultsViewModel>().Single();

        logView.Initialise();

        var stepRunner = new CompileStepRunner(logView);

        stepRunner.AddStep(new PrepareVmfFileStep());
        stepRunner.AddStep(new RunVBspStep(_settingsManager));
        stepRunner.AddStep(new RunVVisStep(_settingsManager));
        stepRunner.AddStep(new RunVRadStep(_settingsManager));

        if (_settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.IsEnabled)
        {
            stepRunner.AddStep(new ResourcePackingStep(_settingsManager));
        }

        if (_settingsManager.Manifest.MapCompilerSettings.ResourcePackingSettings.PerformRepackCompress)
        {
            stepRunner.AddStep(new RepackBspStep());
        }

        if (_settingsManager.Manifest.MapCompilerSettings.CopyMapToGameMapsFolder)
        {
            stepRunner.AddStep(new CopyBspToGameStep());
        }

        if (_settingsManager.Manifest.MapCompilerSettings.CompressMapToBZip2)
        {
            stepRunner.AddStep(new CompressBspToBzip2Step());
        }

        if (_settingsManager.Manifest.MapCompilerSettings.LaunchMapInGame)
        {
            stepRunner.AddStep(new LaunchMapInGameStep());
        }

        return await stepRunner.RunAsync();
    }
}