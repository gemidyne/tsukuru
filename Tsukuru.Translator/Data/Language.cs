using Newtonsoft.Json;

namespace Tsukuru.Translator.Data
{
    public class Language
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}