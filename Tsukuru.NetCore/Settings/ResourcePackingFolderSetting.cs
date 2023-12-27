using Newtonsoft.Json;

namespace Tsukuru.Settings;

public class ResourcePackingFolderSetting
{
	[JsonProperty("path")]
	public string Path { get; set; }

	[JsonProperty("intelligent")]
	public bool Intelligent { get; set; }
}