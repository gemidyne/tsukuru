using Newtonsoft.Json;

namespace Tsukuru.Settings
{
    internal class MapCompilerSettings
    {
        [JsonProperty("vmfPath")]
        public string LastVmfPath { get; set; }

        [JsonProperty("compressToBZip2")]
        public bool CompressMapToBZip2 { get; set; }

        [JsonProperty("copyToGameMapsOnComplete")]
        public bool CopyMapToGameMapsFolder { get; set; }

        [JsonProperty("vbsp")]
        public VbspSettings VbspSettings { get; set; }

        [JsonProperty("vvis")]
        public VvisSettings VvisSettings { get; set; }

	    [JsonProperty("vrad")]
	    public VradSettings VradSettings { get; set; }

		[JsonProperty("launchMapInGame")]
	    public bool LaunchMapInGame { get; set; }

	    public MapCompilerSettings()
        {
            VbspSettings = new VbspSettings();
            VvisSettings = new VvisSettings();
	        VradSettings = new VradSettings();
        }
    }
}