using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Tsukuru.Core.Translations.Data;

public class TranslatorProjectSchema
{
    [JsonProperty("id")]
    public Guid Id { get; set; }

    [JsonProperty("name")]
    public string ProjectName { get; set; }

    [JsonProperty("author")]
    public string Author { get; set; }

    [JsonProperty("website")]
    public string Website { get; set; }

    [JsonProperty("outputFileName")]
    public string OutputFileName { get; set; }

    [JsonProperty("languages")]
    public List<Language> Languages { get; set; }

    [JsonProperty("phrases")]
    public List<Phrase> Phrases { get; set; }

    public TranslatorProjectSchema()
    {
        Id = Guid.NewGuid();
        Languages = new List<Language>();
        Phrases = new List<Phrase>();
    }
}