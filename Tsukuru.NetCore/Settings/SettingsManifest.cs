using Newtonsoft.Json;

namespace Tsukuru.Settings;

internal class SettingsManifest
{
    [JsonProperty("sourcePawnSettings")]
    public SourcePawnCompilerSettings SourcePawnCompiler { get; set; }

    [JsonProperty("mapCompilerSettings")]
    public MapCompilerSettings MapCompilerSettings { get; set; }

    [JsonProperty("checkForUpdatesOnStartup")]
    public bool CheckForUpdatesOnStartup { get; set; }

    public SettingsManifest()
    {
        SourcePawnCompiler = new SourcePawnCompilerSettings();
        MapCompilerSettings = new MapCompilerSettings();
        CheckForUpdatesOnStartup = true;
    }
}