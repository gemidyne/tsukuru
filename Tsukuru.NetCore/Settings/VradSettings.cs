using Newtonsoft.Json;

namespace Tsukuru.Settings;

public class VradSettings
{
    [JsonProperty("fast")]
    public bool Fast { get; set; }

    [JsonProperty("final")]
    public bool Final { get; set; }

    [JsonProperty("ldr")]
    public bool Ldr { get; set; }

    [JsonProperty("hdr")]
    public bool Hdr { get; set; }

    [JsonProperty("staticPropLighting")]
    public bool StaticPropLighting { get; set; }

    [JsonProperty("staticPropPolys")]
    public bool StaticPropPolys { get; set; }

    [JsonProperty("textureShadows")]
    public bool TextureShadows { get; set; }

    [JsonProperty("useModifiedVrad")]
    public bool UseModifiedVrad { get; set; }

    [JsonProperty("low")]
    public bool LowPriority { get; set; }

    [JsonProperty("otherArguments")]
    public string OtherArguments { get; set; }

    [JsonProperty("largeDispSampleRadius")]
    public bool LargeDispSampleRadius { get; set; }
}