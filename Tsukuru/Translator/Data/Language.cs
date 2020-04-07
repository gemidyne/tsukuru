using Newtonsoft.Json;

namespace Tsukuru.Translator.Data
{
    internal class Language
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}