using Newtonsoft.Json;

namespace Tsukuru.Schemas.Translations;

public class StringFormatArgument : IFormatArgument
{
    [JsonProperty("description")]
    public string Description { get; set; }

    public string Render(int index)
    {
        return $"{{{index}:s}}";
    }
}