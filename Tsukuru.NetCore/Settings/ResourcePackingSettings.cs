using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tsukuru.Settings
{
    internal class ResourcePackingSettings
    {
        [JsonProperty("enabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("generateMapSpecificFiles")]
        public bool GenerateMapSpecificFiles { get; set; }

        [JsonProperty("repackCompress")]
        public bool PerformRepackCompress { get; set; }

        [JsonProperty("folders")]
        public List<ResourcePackingFolderSetting> Folders { get; set; }

        public ResourcePackingSettings()
        {
            Folders = new List<ResourcePackingFolderSetting>();
        }
    }
}