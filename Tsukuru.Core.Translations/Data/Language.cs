using Newtonsoft.Json;

namespace Tsukuru.Core.Translations.Data
{
    public class Language
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}