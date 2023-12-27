using Newtonsoft.Json;

namespace Tsukuru.Settings;

internal class ResourcePackingFolderSetting
{
	[JsonProperty("path")]
	public string Path { get; set; }

	[JsonProperty("intelligent")]
	public bool Intelligent { get; set; }
}