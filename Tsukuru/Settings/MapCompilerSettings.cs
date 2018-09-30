using Newtonsoft.Json;

namespace Tsukuru.Settings
{
    internal class MapCompilerSettings
    {
        [JsonProperty("vmfPath")]
        public string LastVmfPath { get; set; }
    }
}