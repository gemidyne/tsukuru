using Newtonsoft.Json;

namespace Tsukuru.Schemas.Translations;

public class Language
{
    [JsonProperty("code")]
    public string Code { get; set; }
}