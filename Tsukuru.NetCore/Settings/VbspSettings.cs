using Newtonsoft.Json;

namespace Tsukuru.Settings;

public class VbspSettings
{
    [JsonProperty("entitiesOnly")]
    public bool OnlyEntities { get; set; }

    [JsonProperty("propsOnly")]
    public bool OnlyProps { get; set; }

    [JsonProperty("noDetail")]
    public bool NoDetailEntities { get; set; }

    [JsonProperty("noWater")]
    public bool NoWaterBrushes { get; set; }

    [JsonProperty("low")]
    public bool LowPriority { get; set; }

    [JsonProperty("keepStalePackedData")]
    public bool KeepStalePackedData { get; set; }

    [JsonProperty("otherArguments")]
    public string OtherArguments { get; set; }
}