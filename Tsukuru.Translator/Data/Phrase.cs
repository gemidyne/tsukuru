using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tsukuru.Translator.Data
{
    public class Phrase
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("text")]
        public string EnglishText { get; set; }

        [JsonProperty("formatArguments")]
        public List<IFormatArgument> FormatArguments { get; set; }

        [JsonProperty("translations")]
        public Dictionary<string, string> Translations { get; set; }

        public Phrase()
        {
            FormatArguments = new List<IFormatArgument>();
            Translations = new Dictionary<string, string>();
        }
    }
}