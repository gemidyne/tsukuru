using Newtonsoft.Json;

namespace Tsukuru.Settings
{
    internal class MapVersioningSettings
    {
        [JsonProperty("mode")]
        public EMapVersionMode Mode { get; set; } = EMapVersionMode.VersionedDateTime;

        [JsonProperty("fileNamePrefix")] 
        public string FileNamePrefix { get; set; } = "TsukuruMap-";

        [JsonProperty("fileNameSuffix")]
        public string FileNameSuffix { get; set; }

        [JsonProperty("nextBuildNumber")] 
        public int NextBuildNumber { get; set; } = 1;
    }
}