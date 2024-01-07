using Newtonsoft.Json;

namespace Tsukuru.Schemas.Translations;

public class Phrase
{
    /// <summary>
    /// Unique phrase key
    /// </summary>
    [JsonProperty("key")]
    public string Key { get; set; }

    /// <summary>
    /// Description can be used to provide context or extra understanding to the purpose of this phrase
    /// </summary>
    [JsonProperty("description")]
    public string Description { get; set; }

    /// <summary>
    /// The phrase in English 
    /// </summary>
    [JsonProperty("text")]
    public string EnglishText { get; set; }
    
    /// <summary>
    /// Category used to group a number of translation phrases together
    /// </summary>
    [JsonProperty("category")]
    public string Category { get; set; }

    /// <summary>
    /// Arguments can be used to inject dynamic data into a translation string
    /// </summary>
    [JsonProperty("formatArguments")]
    public List<IFormatArgument> FormatArguments { get; set; }

    /// <summary>
    /// Other language translations for this phrase
    /// </summary>
    [JsonProperty("translations")]
    public Dictionary<string, string> Translations { get; set; }

    /// <summary>
    /// Controls whether an empty value is allowed on this phrase
    /// </summary>
    [JsonProperty("ignoreEmptyValue")]
    public bool IgnoreEmptyValue { get; set; }

    public Phrase()
    {
        FormatArguments = new List<IFormatArgument>();
        Translations = new Dictionary<string, string>();
    }
}