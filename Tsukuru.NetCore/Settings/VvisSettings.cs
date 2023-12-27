using Newtonsoft.Json;

namespace Tsukuru.Settings;

public class VvisSettings
{
    [JsonProperty("fast")]
    public bool Fast { get; set; }

    [JsonProperty("low")]
    public bool LowPriority { get; set; }

    [JsonProperty("otherArguments")]
    public string OtherArguments { get; set; }
}